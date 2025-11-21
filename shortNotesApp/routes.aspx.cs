using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            if (!IsPostBack)
            {
                LoadRoutesToGrid();
            }
        }


        private void LoadRoutesToGrid()
        {
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
    }
}