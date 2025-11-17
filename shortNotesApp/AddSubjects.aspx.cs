using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class AddSubjects : System.Web.UI.Page
    {
        string conString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
        private void LoadGrid()
        {
            string selectSQL = "SELECT * FROM subjects";
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand(selectSQL, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            // Fill the DataSet.
            DataSet ds = new DataSet();
            adapter.Fill(ds, "Subjects");
            // Perform the binding.
            if (gridSubjects == null)
            {
                throw new Exception("gridSubjects is NULL!");
            }

            if (ds == null)
            {
                throw new Exception("ds is NULL!");
            }
            gridSubjects.DataSource = ds;
            gridSubjects.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // creating datatables and new user at first when needed
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();
                    string queryString = @"IF OBJECT_ID('dbo.subjects', 'U') IS NULL
begin
CREATE TABLE subjects (
  subjectId INTEGER PRIMARY KEY IDENTITY(1,1) NOT NULL,
  name VARCHAR(255) NOT NULL,
  optional1 VARCHAR(255) NOT NULL,
  optional2 VARCHAR(255) NOT NULL
);
end
";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                }

                if (!IsPostBack)
                {
                    LoadGrid();
                }
            }
            catch (Exception error)
            {
                info.Text = error.Message;
                info.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string subject = txtSubject.Text;

            try
            {
                // creating datatables and new user at first when needed
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    string queryString2 = "INSERT INTO subjects (name,optional1,optional2) VALUES (@name,'','')";
                    using (SqlCommand command2 = new SqlCommand(queryString2, connection))
                    {
                        command2.Parameters.AddWithValue("@name", SqlDbType.NVarChar).Value = subject;
                        command2.ExecuteNonQuery();

                        // create new subject table
                        subject = subject.Replace(" ", "");
                        // sanitization before putting in sql, cause if these characters include, then errors will occur
                        subject = subject.Replace("]", "").Replace("[", "").Replace(";", "");

                        string queryString3 = $@"IF OBJECT_ID('dbo.ShortNotes{subject}', 'U') IS NULL
begin
CREATE TABLE ShortNotes{subject} (
	[Id] INTEGER PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[LessonNo] [int] NOT NULL,
	[Link] [nvarchar](max) NOT NULL,
	[Optional1] [nvarchar](max) NULL,
	[Optional2] [nvarchar](max) NULL,
	[Optional3] [nvarchar](max) NULL,
);
end
";
                        using (SqlCommand command3 = new SqlCommand(queryString3, connection))
                        {
                            command3.ExecuteNonQuery();

                            //add dummy item
                            string queryString4 = $"INSERT INTO ShortNotes{subject} (Title,LessonNo,Link) VALUES ('dummy',0,'dummylink')";
                            using (SqlCommand command4 = new SqlCommand(queryString4, connection))
                            {
                                command4.ExecuteNonQuery();
                            }
                        }
                    }

                    connection.Close();
                    info.Text = "Subject added successfully !";
                    info.ForeColor = System.Drawing.Color.Blue;
                    LoadGrid();
                }
            }
            catch (Exception error)
            {
                info.Text = error.Message;
                info.ForeColor = System.Drawing.Color.Red;
            }

        }
    }
}