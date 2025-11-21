using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class testingName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                pnlMain.Visible = false;
                pnlPwd.Visible = true;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string pwd = txtPwd.Text;
            string uName = txtUName.Text;
            string password = WebConfigurationManager.ConnectionStrings["Password"].ConnectionString;
            string username = WebConfigurationManager.ConnectionStrings["Username"].ConnectionString;
            if (password == txtPwd.Text && username == txtUName.Text)
            {
                pnlMain.Visible = true;
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
    }
}