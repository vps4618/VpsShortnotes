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
        string conString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            lblInfo1.Text = "";
            lblInfo.Text = "";
            if (!this.IsPostBack)
            {
                pnlContent.Visible = false;
                pnlPwd.Visible = true;
                pnlSelect.Visible = false;


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
                pnlContent.Visible = false;
                pnlPwd.Visible = false;
                pnlSelect.Visible = true;

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

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            string tableName = lstSubjects.SelectedItem.Text;
            lblHeading.Text = "Insert Short Notes"+tableName;

            tableName = tableName.Replace(" ", "");

            // Build dynamic SQL based on table name
            sourceShortNotes.SelectCommand = $"SELECT * FROM ShortNotes{tableName}";
            sourceShortNotes.InsertCommand =
                $"INSERT INTO ShortNotes{tableName} (Title,LessonNo,Link,Optional1,Optional2,Optional3) " +
                $"VALUES (@Title,@LessonNo,@Link,@Optional1,@Optional2,@Optional3)";
            sourceShortNotes.UpdateCommand =
                $"UPDATE ShortNotes{tableName} SET Title=@Title,LessonNo=@LessonNo,Link=@Link,Optional1=@Optional1," +
                $"Optional2=@Optional2,Optional3=@Optional3 WHERE Id=@Id";
            sourceShortNotes.DeleteCommand = $"DELETE FROM ShortNotes{tableName} WHERE Id=@Id";

            // Refresh data
            sourceShortNotes.DataBind();
            dtlView.DataBind();

            pnlSelect.Visible = false;
            pnlContent.Visible = true;
        }
    }
}