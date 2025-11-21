using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class InsertShortNotesChemistry : System.Web.UI.Page
    {
        string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            lblInfo1.Text = "";
            lblInfo.Text = "";
            if (!this.IsPostBack)
            {
                // this help to load credentals at first
                string script = @"
            let uname = localStorage.getItem('sn_username');
            let pwd = localStorage.getItem('sn_password');
            if (uname && pwd) {
                document.getElementById('" + txtUName.ClientID + @"').value = uname;
                document.getElementById('" + txtPwd.ClientID + @"').value = pwd;
            }";
                ClientScript.RegisterStartupScript(this.GetType(), "loadCredentials", script, true);


                pnlContent.Visible = false;
                pnlPwd.Visible = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string pwd = txtPwd.Text;
            string uName = txtUName.Text;
            string password = WebConfigurationManager.ConnectionStrings["Password"].ConnectionString;
            string username = WebConfigurationManager.ConnectionStrings["Username"].ConnectionString;
            if(password == txtPwd.Text && username == txtUName.Text)
            {
                pnlContent.Visible = true;
                pnlPwd.Visible = false;

                // Call JS to save
                ScriptManager.RegisterStartupScript(this, this.GetType(), "saveCreds",
                    $"saveCredentials('{txtUName.Text}','{txtPwd.Text}');", true);
            }
            else
            {
                lblInfo.Text = "Incorrect username or password.";
                lblInfo.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void sourceAds_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.Exception != null)
            {
                lblInfo1.Text = e.Exception.Message;
                lblInfo1.ForeColor = System.Drawing.Color.Red;

                e.ExceptionHandled = true;
            }
            else
            {
                lblInfo1.Text = "Record updated successfully.";
                lblInfo1.ForeColor = System.Drawing.Color.LightGray;
            }
        }

        protected void sourceAds_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                lblInfo1.Text = e.Exception.Message;
                lblInfo1.ForeColor = System.Drawing.Color.Red;

                e.ExceptionHandled = true;
            }
            else
            {
                lblInfo1.Text = "Record inserted successfully.";
                lblInfo1.ForeColor = System.Drawing.Color.LightGray;
            }
        }

        protected void sourceAds_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                lblInfo1.Text = e.Exception.Message;
                lblInfo1.ForeColor = System.Drawing.Color.Red;

                e.ExceptionHandled = true;
            }
            else
            {
                lblInfo1.Text = "Record deleted successfully.";
                lblInfo1.ForeColor = System.Drawing.Color.LightGray;
            }
        }

        protected void sourceAds_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                lblInfo1.Text = e.Exception.Message;
                lblInfo1.ForeColor = System.Drawing.Color.Red;

                e.ExceptionHandled = true;
            }
        }
    }
}