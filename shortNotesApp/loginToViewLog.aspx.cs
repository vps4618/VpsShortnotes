using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class loginToViewLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Replace with your actual user validation
            if (username == "admin" && password == "admin@123")
            {
                Session["IsAuthenticated"] = true;
                string returnUrl = Request.QueryString["returnUrl"] ?? "user_access_log.aspx";
                Server.Transfer(returnUrl);
            }
            else
            {
                lblMessage.Text = "Invalid credentials.";
            }
        }
    }
}