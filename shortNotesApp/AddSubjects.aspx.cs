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
            con.Open();
            SqlCommand cmd = new SqlCommand(selectSQL, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            // Fill the DataSet.
            DataSet ds = new DataSet();
            adapter.Fill(ds, "Subjects");
            // Perform the binding.
            gridSubjects.DataSource = ds;
            gridSubjects.DataBind();
            con.Close();
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
  optional2 VARCHAR(255) NOT NULL,
  CreatedAt DATETIME NOT NULL CONSTRAINT DF_subjects_CreatedAt DEFAULT (SWITCHOFFSET(TODATETIMEOFFSET(GETUTCDATE(), '+00:00'), '+05:30'))
);
end

IF OBJECT_ID('dbo.Ads', 'U') IS NULL
BEGIN
    CREATE TABLE Ads (
        AdId INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        Title NVARCHAR(255) NOT NULL,
        Message NVARCHAR(MAX) NULL,
        Link NVARCHAR(1024) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Ads_IsActive DEFAULT (1),
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_Ads_CreatedAt DEFAULT (SWITCHOFFSET(TODATETIMEOFFSET(GETUTCDATE(), '+00:00'), '+05:30'))
    );
END

INSERT INTO Ads (Title,Message,Link,IsActive) VALUES ('dummyTitle','dumMsg','dummylink.html',0);
";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                }

                if (!IsPostBack)
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


                    pnlPwd.Visible = true;
                    pnlInsert.Visible = false;
                    pnlUpdateDelete.Visible = false;
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

            if (subject == "")
            {
                info.Text = "Please enter a subject name";
                info.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // checking whether subject already exist in database or not
            string selectSQL = "SELECT * FROM subjects";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand(selectSQL, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            // Fill the DataSet.
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            bool isAlreadyIn = false;

            foreach (DataRow row in dt.Rows)
            {
                if (subject == row[1].ToString() || subject.Replace(" ", "") == row[1].ToString().Replace(" ", ""))
                {
                    isAlreadyIn = true;
                }
            }

            if (isAlreadyIn)
            {
                info.Text = "Subject name already exist in database.Subject name cannot duplicate.";
                info.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Close();



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

        protected void initializeViewDeletePanel()
        {
            lstIds.Items.Clear();
            string selectSQL = "SELECT * FROM subjects";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand(selectSQL, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            // Fill the DataSet.
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                ListItem item = new ListItem();
                item.Text = row[0].ToString();
                // for asp.net viewstate
                item.Value = row[0].ToString();
                lstIds.Items.Add(item);

                if (i == 0)
                {
                    txtSubjectName.Text = row[1].ToString();
                }
                i++;
            }
            con.Close();
        }
        protected void btnViewUpdatePnl_Click(object sender, EventArgs e)
        {
            pnlInsert.Visible = false;
            pnlUpdateDelete.Visible = true;

            lstIds.Items.Clear();
            initializeViewDeletePanel();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {

            if (txtSubjectName.Text == "")
            {
                info.Text = "Please enter a subject name";
                info.ForeColor = System.Drawing.Color.Red;
                return;
            }



            // checking whether subject already exist in database or not
            string subject = txtSubjectName.Text;
            string selectSQL1 = "SELECT * FROM subjects";
            SqlConnection con5 = new SqlConnection(conString);
            con5.Open();
            SqlCommand cmd5 = new SqlCommand(selectSQL1, con5);
            SqlDataAdapter adapter1 = new SqlDataAdapter(cmd5);
            // Fill the DataSet.
            DataTable dt1 = new DataTable();
            adapter1.Fill(dt1);

            bool isAlreadyIn = false;

            foreach (DataRow row in dt1.Rows)
            {
                if (subject == row[1].ToString() || subject.Replace(" ", "") == row[1].ToString().Replace(" ", ""))
                {
                    isAlreadyIn = true;
                }
            }

            if (isAlreadyIn)
            {
                info.Text = "Subject name already exist in database.Subject name cannot duplicate.";
                info.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con5.Close();


            // getting previous subject name
            string selectSQL = $"SELECT * FROM subjects WHERE subjectId={lstIds.SelectedItem.Text}";
            SqlConnection con1 = new SqlConnection(conString);
            con1.Open();
            SqlCommand cmd1 = new SqlCommand(selectSQL, con1);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd1);
            // Fill the DataSet.
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            con1.Close();

            string prevSubjectNameTrimmed = dt.Rows[0][1].ToString().Replace(" ", "").Trim().Replace("]", "").Replace("[", "").Replace(";", "");
            prevSubjectNameTrimmed = "ShortNotes" + prevSubjectNameTrimmed;
            // for update   shortnotes table
            string subjectNameTrimmed = txtSubjectName.Text.Replace(" ", "");
            subjectNameTrimmed = subjectNameTrimmed.Trim();
            subjectNameTrimmed = subjectNameTrimmed.Replace("]", "").Replace("[", "").Replace(";", "");
            subjectNameTrimmed = "ShortNotes" + subjectNameTrimmed;

            string updateSql = $"UPDATE subjects SET name=@name,CreatedAt=SWITCHOFFSET(TODATETIMEOFFSET(GETUTCDATE(), '+00:00'), '+05:30') WHERE subjectId=@id; EXEC sp_rename {prevSubjectNameTrimmed},{subjectNameTrimmed};";

            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand(updateSql, con);

            cmd.Parameters.AddWithValue("@name", txtSubjectName.Text);
            cmd.Parameters.AddWithValue("@id", lstIds.SelectedItem.Text);

            // Try to open database and excute the update
            int updated = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                updated = cmd.ExecuteNonQuery();
                info.Text = updated.ToString() + " record updated. Also table name changed.";
                info.ForeColor = System.Drawing.Color.Blue;
                LoadGrid();
                initializeViewDeletePanel();
            }
            catch (Exception err)
            {
                info.Text = "Error updating subject and renaming table.";
                info.Text += err.Message;
                info.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close();
            }


        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // ***** hidden field approach is the best approach to connect js and c# (without using ajax with webmethods - they cant access memebers like 'info' cause they are static


            // Read user choice from hidden field
            bool userChoice = bool.Parse(hfDeleteChoice.Value);

            if (!userChoice)
            {
                info.Text = "Subject is not deleted!";
                info.ForeColor = System.Drawing.Color.Blue;
                return;
            }

            // getting existed subject name
            string selectSQL = $"SELECT * FROM subjects WHERE subjectId={lstIds.SelectedItem.Text}";
            SqlConnection con1 = new SqlConnection(conString);
            con1.Open();
            SqlCommand cmd1 = new SqlCommand(selectSQL, con1);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd1);
            // Fill the DataSet.
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            con1.Close();

            string prevSubjectNameTrimmed = dt.Rows[0][1].ToString().Replace(" ", "").Trim().Replace("]", "").Replace("[", "").Replace(";", "");
            string deleteSql = $"DELETE FROM subjects WHERE subjectId=@id; DROP TABLE ShortNotes{prevSubjectNameTrimmed}";

            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand(deleteSql, con);

            cmd.Parameters.AddWithValue("@id", lstIds.SelectedItem.Text);

            // Try to open database and excute the update
            int deleted = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                deleted = cmd.ExecuteNonQuery();
                info.Text = deleted.ToString() + " record deleted. Also corresponding table deleted.";
                info.ForeColor = System.Drawing.Color.Blue;
                LoadGrid();
                initializeViewDeletePanel();
            }
            catch (Exception err)
            {
                info.Text = "Error deleting subject and deleting table.";
                info.Text += err.Message;
                info.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close();
            }


        }


        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            pnlInsert.Visible = true;
            pnlUpdateDelete.Visible = false;
        }

        protected void lstIds_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectSQL = $"SELECT * FROM subjects WHERE subjectId={lstIds.SelectedItem.Text}";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand(selectSQL, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            // Fill the DataSet.
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            txtSubjectName.Text = dt.Rows[0][1].ToString();
            con.Close();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string pwd = txtPwd.Text;
            string uName = txtUName.Text;
            string password = WebConfigurationManager.ConnectionStrings["Password"].ConnectionString;
            string username = WebConfigurationManager.ConnectionStrings["Username"].ConnectionString;
            if (password == txtPwd.Text && username == txtUName.Text)
            {
                pnlInsert.Visible = true;
                pnlPwd.Visible = false;
                pnlUpdateDelete.Visible = false;
                LoadGrid();

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