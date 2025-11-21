using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class InsertShortNotesCombinedMaths : System.Web.UI.Page
    {
        // 
        //
        //
        //  HERE , I MOSTLY USED CHATGPT AGAIN AGAIN TO FIX ERRORS COMING IN MANY TIMES.Now its working.
        //
        //
        //

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SelectedTable"] != null)
            {
                string table = Session["SelectedTable"].ToString();
                CreateAndAttachDataSource(table);
            }
        }

        string conString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
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


                pnlContent.Visible = false;
                pnlPwd.Visible = true;
                pnlSelect.Visible = false;
                lblInfo1.Text = "";
                lblInfo.Text = "";

                try
                {
                    // load subjects to drop down list
                    string selectSQL = "SELECT name FROM subjects";

                    SqlConnection con = new SqlConnection(conString);
                    SqlCommand cmd = new SqlCommand(selectSQL, con);
                    SqlDataReader reader;

                    con.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ListItem newItem = new ListItem();
                        newItem.Text = reader["name"].ToString();
                        newItem.Value = reader["name"].ToString();
                        lstSubjects.Items.Add(newItem);
                    }

                }
                catch (Exception error)
                {
                    lblInfo1.Text = error.Message;
                    lblInfo1.ForeColor = System.Drawing.Color.Red;
                }


                // Recreate data source if a table was already selected
                if (Session["SelectedTable"] != null)
                {
                    string tableName = Session["SelectedTable"].ToString();
                    SqlDataSource ds = CreateDataSource(tableName);
                    this.Controls.Add(ds);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string pwd = txtPwd.Text;
            string uName = txtUName.Text;
            string password = WebConfigurationManager.ConnectionStrings["Password"].ConnectionString;
            string username = WebConfigurationManager.ConnectionStrings["Username"].ConnectionString;
            if (password == txtPwd.Text && username == txtUName.Text)
            {
                pnlContent.Visible = false;
                pnlPwd.Visible = false;
                pnlSelect.Visible = true;

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


        private SqlDataSource CreateDataSource(string tableName)
        {
            SqlDataSource ds = new SqlDataSource();
            ds.ID = "ds_" + tableName;
            ds.ConnectionString = conString;
            ds.ProviderName = "System.Data.SqlClient";

            ds.SelectCommand = $"SELECT * FROM ShortNotes{tableName}";
            ds.InsertCommand = $"INSERT INTO ShortNotes{tableName} (Title,LessonNo,Link,Optional1,Optional2,Optional3) VALUES (@Title,@LessonNo,@Link,@Optional1,@Optional2,@Optional3)";
            ds.UpdateCommand = $"UPDATE ShortNotes{tableName} SET Title=@Title,LessonNo=@LessonNo,Link=@Link,Optional1=@Optional1,Optional2=@Optional2,Optional3=@Optional3 WHERE Id=@Id";
            ds.DeleteCommand = $"DELETE FROM ShortNotes{tableName} WHERE Id=@Id";

            // define parameters
            ds.InsertParameters.Add("Title", TypeCode.String, "");
            ds.InsertParameters.Add("LessonNo", TypeCode.Int32, "0");
            ds.InsertParameters.Add("Link", TypeCode.String, "");
            ds.InsertParameters.Add("Optional1", TypeCode.String, "");
            ds.InsertParameters.Add("Optional2", TypeCode.String, "");
            ds.InsertParameters.Add("Optional3", TypeCode.String, "");

            ds.UpdateParameters.Add("Id", TypeCode.Int32, "0");
            ds.UpdateParameters.Add("Title", TypeCode.String, "");
            ds.UpdateParameters.Add("LessonNo", TypeCode.Int32, "0");
            ds.UpdateParameters.Add("Link", TypeCode.String, "");
            ds.UpdateParameters.Add("Optional1", TypeCode.String, "");
            ds.UpdateParameters.Add("Optional2", TypeCode.String, "");
            ds.UpdateParameters.Add("Optional3", TypeCode.String, "");

            ds.DeleteParameters.Add("Id", TypeCode.Int32, "0");

            return ds;
        }


        private void CreateAndAttachDataSource(string tableName)
        {
            // Remove previous dynamic datasource (if exists)
            var old = pnlContent.FindControl("dsDynamic");
            if (old != null)
                pnlContent.Controls.Remove(old);

            SqlDataSource ds = CreateDataSource(tableName);
            ds.ID = "dsDynamic";

            pnlContent.Controls.Add(ds);
            dtlView.DataSourceID = ds.ID;
        }

        protected void dtlView_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            dtlView.ChangeMode(e.NewMode);
            dtlView.DataBind();
        }

        protected void dtlView_PageIndexChanging(object sender, DetailsViewPageEventArgs e)
        {
            dtlView.PageIndex = e.NewPageIndex;
            dtlView.DataBind();
        }

        protected void dtlView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            dtlView.DataBind();
        }

        protected void dtlView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            dtlView.DataBind();
        }

        protected void dtlView_ItemDeleting(object sender, DetailsViewDeleteEventArgs e)
        {
            dtlView.DataBind();
        }


        protected void btnSelect_Click(object sender, EventArgs e)
        {
            string tableName = lstSubjects.SelectedItem.Text.Replace(" ", "");
            lblHeading.Text = "Insert Short Notes " + tableName;
            Session["SelectedTable"] = tableName;

            CreateAndAttachDataSource(tableName);

            dtlView.ChangeMode(DetailsViewMode.ReadOnly);
            dtlView.DataBind();

            pnlSelect.Visible = false;
            pnlContent.Visible = true;
        }

        protected void dtlView_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
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

        protected void dtlView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
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

        protected void dtlView_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
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
    }
}