using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.Services;
using System.Web.Script.Services;

namespace shortNotesApp
{
    public partial class ShortNotesMain : System.Web.UI.Page
    {
        // This will use to send user messages to owner email
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SendContactMessage(string userName, string userMessage)
        {
            // Compose email with both name and message
            MailMessage mail = new MailMessage();
            mail.To.Add("vishwapraveen30145@gmail.com");
            mail.Subject = "Contact Message from Short Notes";
            mail.Body = $"Message from user: {userName}\n\n{userMessage}";
            mail.From = new MailAddress("vishwapraveen4618@gmail.com");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("vishwapraveen4618@gmail.com", "pvpc nnno kmjt nzyh");
            smtp.EnableSsl = true;
            smtp.Send(mail);

            return "{\"status\":\"sent\"}";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string LogUserName(string userName)
        {
            // Log user name to file
            string logPath = HttpContext.Current.Server.MapPath("~/user_access_log.txt");
            string accessTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string userIP = HttpContext.Current.Request.UserHostAddress;
            string userAgent = HttpContext.Current.Request.UserAgent;

            string logEntry = $"Time: {accessTime}, Name: {userName}, IP: {userIP}, Agent: {userAgent}\n";
            System.IO.File.AppendAllText(logPath, logEntry);

            // Optionally send email here as well
            SendLogEmail(logEntry);


            return "{\"status\":\"success\"}";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
                
            {
                
                // set theme
                string theme = Session["Theme"] as string ?? "b"; // Default to "b"
                ApplyThemeToControls(theme);
            }
            catch (Exception ex)
            {
                // Prepare your email body with exception details
                string subject = "Exception in shortnotes.aspx";
                string body = $"Exception Message: {ex.Message}\nStack Trace:\n{ex.StackTrace}";

                // Call your send email function
                SendLogEmailExceptions(subject, body);

                // Optionally rethrow or handle as needed
                // throw; // or show a user-friendly message
            }
        }

        // send email when exception occurring
        public void SendLogEmailExceptions(string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("vishwapraveen30145@gmail.com");
                mail.Subject = subject;
                mail.Body = body;
                mail.From = new MailAddress("vishwapraveen4618@gmail.com");

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("vishwapraveen4618@gmail.com", "pvpc nnno kmjt nzyh");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception emailEx)
            {
                // Optional: log email sending errors somewhere, but don’t throw further
            }
        }

        public static void SendLogEmail(string logEntry)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("vishwapraveen30145@gmail.com");
                mail.Subject = "New User Access Log Entry";
                mail.Body = logEntry;
                mail.From = new MailAddress("vishwapraveen4618@gmail.com"); // Use a valid sender email

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; // Your SMTP server
                smtp.Port = 587; // Or your SMTP port
                smtp.Credentials = new System.Net.NetworkCredential("vishwapraveen4618@gmail.com", "pvpc nnno kmjt nzyh");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // Optionally log email errors
            }
        }

        // SAVE THEME
        [System.Web.Services.WebMethod]
        public static void SetTheme(string theme)
        {
            HttpContext.Current.Session["Theme"] = theme;
        }

        private void ApplyThemeToControls(string theme)
        {
            string cssClass = theme == "b" ? "ui-body-b ui-bar-b" : "ui-body-a ui-bar-a";

            // Apply theme to the page or specific controls
            home.Attributes["data-theme"] = theme;
            home.Attributes["class"] = cssClass;
        }

        // combined maths search
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static List<string> GetCombinedSuggestions(string prefixText, int count)
        {
            List<string> names = null;
            // Check if the list is in the cache.
            if (HttpContext.Current.Cache["CombinedMathsList"] == null)
            {
                // If not, regenerate the list. The ADO.NET code for this part
                // isn't shown (but you can see it in the downloadable examples
                // for this chapter.
                names = GetCombinedTitleListFromDB();

                // Store the name list in the cache for sixty minutes.
                HttpContext.Current.Cache.Insert("CombinedMathsList", names, null,
                  DateTime.Now.AddMinutes(60), TimeSpan.Zero);

            }
            else
            {
                // Get the name list out of the cache.
                names = (List<string>)HttpContext.Current.Cache["CombinedMathsList"];

            }

            List<string> wordList = new List<string>();
            for (int i = 0; i < names.Count; i++)
            {

                if (names[i].Contains(prefixText.ToLower()))
                {
                    wordList.Add(names[i]);
                }

            }

            return wordList;
        }



        private static List<string> GetCombinedTitleListFromDB()
        {

            string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Title FROM ShortNotesCombinedMaths", con);

            List<string> names = new List<string>();

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    names.Add((string)reader["Title"]);
                }
                reader.Close();

                return names;
            }
            finally
            {
                con.Close();
            }
        }

        // chemistry search
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static List<string> GetChemistrySuggestions(string prefixText, int count)
        {
            List<string> names = null;
            // Check if the list is in the cache.
            if (HttpContext.Current.Cache["ChemistryList"] == null)
            {
                // If not, regenerate the list. The ADO.NET code for this part
                // isn't shown (but you can see it in the downloadable examples
                // for this chapter.
                names = GetChemistryTitleListFromDB();

                // Store the name list in the cache for sixty minutes.
                HttpContext.Current.Cache.Insert("ChemistryList", names, null,
                  DateTime.Now.AddMinutes(60), TimeSpan.Zero);
            }
            else
            {
                // Get the name list out of the cache.
                names = (List<string>)HttpContext.Current.Cache["ChemistryList"];

            }

            List<string> wordList = new List<string>();
            for (int i = 0; i < names.Count; i++)
            {

                if (names[i].Contains(prefixText.ToLower()))
                {
                    wordList.Add(names[i]);
                }

            }

            return wordList;
        }



        private static List<string> GetChemistryTitleListFromDB()
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Title FROM ShortNotesChemistry", con);

            List<string> names = new List<string>();

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    names.Add((string)reader["Title"]);
                }
                reader.Close();

                return names;
            }
            finally
            {
                con.Close();
            }
        }

        // physics search
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static List<string> GetPhysicsSuggestions(string prefixText, int count)
        {
            List<string> names = null;
            // Check if the list is in the cache.
            if (HttpContext.Current.Cache["PhysicsList"] == null)
            {
                // If not, regenerate the list. The ADO.NET code for this part
                // isn't shown (but you can see it in the downloadable examples
                // for this chapter.
                names = GetPhysicsTitleListFromDB();

                // Store the name list in the cache for sixty minutes.
                HttpContext.Current.Cache.Insert("PhysicsList", names, null,
                  DateTime.Now.AddMinutes(60), TimeSpan.Zero);
            }
            else
            {
                // Get the name list out of the cache.
                names = (List<string>)HttpContext.Current.Cache["PhysicsList"];

            }

            List<string> wordList = new List<string>();
            for (int i = 0; i < names.Count; i++)
            {

                if (names[i].Contains(prefixText.ToLower()))
                {
                    wordList.Add(names[i]);
                }

            }

            return wordList;
        }



        private static List<string> GetPhysicsTitleListFromDB()
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Title FROM ShortNotesPhysics", con);

            List<string> names = new List<string>();

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    names.Add((string)reader["Title"]);
                }
                reader.Close();

                return names;
            }
            finally
            {
                con.Close();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() != "")
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
                string selectedSubject = lstSubjects.SelectedItem.Text;
                DataSet dtContent = new DataSet();
                string searchText = "%" + txtName.Text + "%";

                try
                {
                    if (selectedSubject == "Combined Maths")
                    {
                        string selectSql = "SELECT * FROM ShortNotesCombinedMaths WHERE Title LIKE @Phrase";
                        SqlConnection con = new SqlConnection(connectionString);
                        con.Open();
                        SqlCommand cmd = new SqlCommand(selectSql, con);
                        cmd.Parameters.AddWithValue("@Phrase", searchText);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        // Fill the dtset
                        adapter.Fill(dtContent, "ShortNotesCombinedMaths");

                        // Perform the binding
                        gridShortNotes.DataSource = dtContent;
                        gridShortNotes.DataBind();
                        con.Close();
                    }
                    else if (selectedSubject == "Chemistry")
                    {
                        string selectSql = "SELECT * FROM ShortNotesChemistry WHERE Title LIKE @Phrase";
                        SqlConnection con = new SqlConnection(connectionString);
                        con.Open();
                        SqlCommand cmd = new SqlCommand(selectSql, con);
                        cmd.Parameters.AddWithValue("@Phrase", searchText);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        // Fill the dtset
                        adapter.Fill(dtContent, "ShortNotesChemistry");

                        // Perform the binding
                        gridShortNotes.DataSource = dtContent;
                        gridShortNotes.DataBind();
                        con.Close();
                    }
                    else if (selectedSubject == "Physics")
                    {
                        string selectSql = "SELECT * FROM ShortNotesPhysics WHERE Title LIKE @Phrase";
                        SqlConnection con = new SqlConnection(connectionString);
                        con.Open();
                        SqlCommand cmd = new SqlCommand(selectSql, con);
                        cmd.Parameters.AddWithValue("@Phrase", searchText);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        // Fill the dtset
                        adapter.Fill(dtContent, "ShortNotesPhysics");

                        // Perform the binding
                        gridShortNotes.DataSource = dtContent;
                        gridShortNotes.DataBind();
                        con.Close();
                    }
                }
                catch (Exception error)
                {
                    lblInfo.Text = error.Message;
                    lblInfo.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void btnGetAll_Click(object sender, EventArgs e)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
            string selectedSubject = lstSubjects.SelectedItem.Text;
            DataSet dtContent = new DataSet();

            try
            {
                if (selectedSubject == "Combined Maths")
                {
                    string selectSql = "SELECT * FROM ShortNotesCombinedMaths";
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand(selectSql, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    // Fill the dtset
                    adapter.Fill(dtContent, "ShortNotesCombinedMaths");

                    // Perform the binding
                    gridShortNotes.DataSource = dtContent;
                    gridShortNotes.DataBind();
                    con.Close();
                }
                else if (selectedSubject == "Chemistry")
                {
                    string selectSql = "SELECT * FROM ShortNotesChemistry";
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand(selectSql, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    // Fill the dtset
                    adapter.Fill(dtContent, "ShortNotesChemistry");

                    // Perform the binding
                    gridShortNotes.DataSource = dtContent;
                    gridShortNotes.DataBind();
                    con.Close();
                }
                else if (selectedSubject == "Physics")
                {
                    string selectSql = "SELECT * FROM ShortNotesPhysics";
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand(selectSql, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    // Fill the dtset
                    adapter.Fill(dtContent, "ShortNotesPhysics");

                    // Perform the binding
                    gridShortNotes.DataSource = dtContent;
                    gridShortNotes.DataBind();
                    con.Close();
                }
            }
            catch (Exception error)
            {
                lblInfo.Text = error.Message;
                lblInfo.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}