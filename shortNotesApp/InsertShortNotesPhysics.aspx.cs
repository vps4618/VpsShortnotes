using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;

namespace shortNotesApp
{
    public partial class InsertShortNotesPhysics : System.Web.UI.Page
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

        protected void sourceShortNotes_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            
            
        }

        // check for similar titles . if so cancel the transaction
        protected void dtlView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            try
            {
                string title = e.Values["Title"]?.ToString();

                if (IsTitleExists(title))
                {
                    // Cancel the insert
                    e.Cancel = true;

                    // Show a message
                    lblInfo1.ForeColor = System.Drawing.Color.Red;
                    lblInfo1.Text = "The item with this Title already exists.";
                }
            }catch(Exception error)
            {
                lblInfo1.ForeColor = System.Drawing.Color.Red;
                lblInfo1.Text = error.Message;
            }
        }

        private bool IsTitleExists(string Title)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM ShortNotesPhysics WHERE Title=@Title";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Title", Title);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                exists = count > 0;
            }
                return exists;
        }
    }
}