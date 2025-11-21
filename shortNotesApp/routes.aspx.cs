using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class routes : System.Web.UI.Page
    {
        // This class used to display all possible routes in this app , in one gridview
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
                pnlData.Visible = false;
                pnlPwd.Visible = true;
            }
        }


        private void LoadRoutesToGrid()
        {
            gridRoutes.DataSource = null;
            var list = new List<object>();

            foreach (var routeBase in RouteTable.Routes)
            {
                if (routeBase is Route route)
                {
                    list.Add(new
                    {
                        Url = route.Url,
                        Defaults = GetRouteValues(route.Defaults),
                        Constraints = GetRouteValues(route.Constraints),
                        DataTokens = GetRouteValues(route.DataTokens)
                    });
                }
            }

            list.Add(new
            {
                Url = "shortnotes",
                Defaults = "",
                Constraints = "",
                DataTokens = ""
            });

            gridRoutes.DataSource = list;
            gridRoutes.DataBind();
        }

        private string GetRouteValues(RouteValueDictionary values)
        {
            if (values == null || values.Count == 0) return "";

            var items = new List<string>();
            foreach (var v in values)
            {
                items.Add($"{v.Key} = {v.Value}");
            }

            return string.Join(", ", items);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string pwd = txtPwd.Text;
            string uName = txtUName.Text;
            string password = WebConfigurationManager.ConnectionStrings["Password"].ConnectionString;
            string username = WebConfigurationManager.ConnectionStrings["Username"].ConnectionString;
            if (password == txtPwd.Text && username == txtUName.Text)
            {
                pnlData.Visible = true;
                pnlPwd.Visible = false;
                LoadRoutesToGrid();

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