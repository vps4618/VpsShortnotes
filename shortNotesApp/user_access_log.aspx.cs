using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class user_access_log : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is authenticated (assuming session "IsAuthenticated" is set on login)
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                Response.Redirect("loginToViewLog.aspx?returnUrl=user_access_log.aspx");
                return;
            }

            string logPath = Server.MapPath("~/user_access_log.txt");
            if (System.IO.File.Exists(logPath))
            {
                string logContent = System.IO.File.ReadAllText(logPath);
                Response.ContentType = "text/html";
                Response.Write("<h2>User Access Log</h2><pre>" + Server.HtmlEncode(logContent) + "</pre>");
            }
            else
            {
                Response.Write("No log found.");
            }
        }
    }
}