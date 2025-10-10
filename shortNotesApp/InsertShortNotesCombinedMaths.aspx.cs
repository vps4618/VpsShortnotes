using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class InsertShortNotesCombinedMaths : System.Web.UI.Page
    {
        string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            lblInfo1.Text = "";
            lblInfo.Text = "";
            if (!this.IsPostBack)
            {
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
            }
            else
            {
                lblInfo.Text = "Incorrect username or password.";
                lblInfo.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void sourceShortNotes_Updated(object sender, SqlDataSourceStatusEventArgs e)
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

        protected void sourceShortNotes_Inserted(object sender, SqlDataSourceStatusEventArgs e)
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

        protected void sourceShortNotes_Deleted(object sender, SqlDataSourceStatusEventArgs e)
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

        protected void sourceShortNotes_Selected(object sender, SqlDataSourceStatusEventArgs e)
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