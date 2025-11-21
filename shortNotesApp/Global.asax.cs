using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace shortNotesApp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // shortnotes route not added because then auto complete doesn't work.so url mapping added.

            RouteTable.Routes.MapPageRoute("shortnotes-insert",
"insertnotes", "~/InsertShortNotes.aspx");
            RouteTable.Routes.MapPageRoute("shortnotes-user-access-log-view",
"useraccesslog", "~/user_access_log.aspx");
            RouteTable.Routes.MapPageRoute("shortnotes-view-routes",
"routes", "~/routes.aspx");
            RouteTable.Routes.MapPageRoute("shortnotes-name-testing",
"testname", "~/testingName.aspx");
            RouteTable.Routes.MapPageRoute("subject-adding",
"addsubjects", "~/AddSubjects.aspx");
            RouteTable.Routes.MapPageRoute("ads-adding",
"insertads", "~/InsertAds.aspx");

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}