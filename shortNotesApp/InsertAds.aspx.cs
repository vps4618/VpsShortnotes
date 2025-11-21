using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class InsertShortNotesChemistry : System.Web.UI.Page
    {
        string connectionString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            lblInfo1.Text = "";
            lblInfo.Text = "";
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

        protected void sourceAds_Updated(object sender, SqlDataSourceStatusEventArgs e)
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

        // for help send emails
        // page-level fields (or use ViewState)
        private string _pendingAdTitle;
        private string _pendingAdBody;
        private string _pendingAdUrl;

        protected void sourceAds_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // parameter names should match your InsertCommand parameters (usually @Title, @Body, @Url)
            object t = e.Command.Parameters["@Title"]?.Value;
            object b = e.Command.Parameters["@Message"]?.Value;
            object u = e.Command.Parameters["@Link"]?.Value;

            _pendingAdTitle = t?.ToString() ?? string.Empty;
            _pendingAdBody = b?.ToString() ?? string.Empty;
            _pendingAdUrl = u?.ToString() ?? string.Empty;
        }

        protected void sourceAds_Inserted(object sender, SqlDataSourceStatusEventArgs e)
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

                SendAdNotifications(_pendingAdTitle, _pendingAdBody, _pendingAdUrl);

                // optionally clear the fields
                _pendingAdTitle = _pendingAdBody = _pendingAdUrl = null;
            }
        }

        // send emails about ad
        private void SendAdNotifications(string title, string body, string urlOrId)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
                var emails = new List<string>();
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT Email FROM Subscribers", conn))
                {
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            emails.Add(rdr.GetString(0));
                        }
                    }
                }

                foreach (var e in emails)
                {
                    try
                    {
                        /*
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
                         */
                        // Build safe HTML body (encode user content, convert newlines to <br/>)
                        var sb = new StringBuilder();
                        sb.Append("<!doctype html><html><body style=\"font-family:Arial,Helvetica,sans-serif;color:#222;\">");
                        sb.AppendFormat("<h2>{0}</h2>", HttpUtility.HtmlEncode("A new ad was posted"));
                        sb.AppendFormat("<p><strong>Title:</strong> {0}</p>", HttpUtility.HtmlEncode(title));
                        sb.AppendFormat("<p><strong>Message:</strong><br/>{0}</p>", HttpUtility.HtmlEncode(body).Replace("\n", "<br/>"));
                        sb.AppendFormat("<p><strong>Link:</strong> <a href=\"{0}\">{1}</a></p>", HttpUtility.HtmlEncode(urlOrId), HttpUtility.HtmlEncode(urlOrId));
                        sb.Append("<hr/>");
                        sb.Append("</body></html>");
                        string htmlBody = sb.ToString();

                        // Plain-text fallback
                        string plainText = $"A new ad was posted\n\nTitle: {title}\n\nMessage:\n{body}\n\nLink: {urlOrId}";


                        var msg = new System.Net.Mail.MailMessage();
                        msg.To.Add(e);
                        msg.Subject = "New ad on ShortNotes: " + title;
                    /*    msg.Body = $"A new ad was posted:\n\nTitle : {title}\n\nMessage : {body}\n\nLink: {urlOrId}";*/
                        // Add both plain text and HTML alternate views (best practice)
                        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainText, Encoding.UTF8, "text/plain"));
                        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html"));

                        // If you prefer the simple approach, you can also do:
                        // msg.Body = htmlBody;
                        // msg.IsBodyHtml = true;

                        msg.From = new System.Net.Mail.MailAddress("vishwapraveen4618@gmail.com");

                        using (var smtp = new System.Net.Mail.SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential("vishwapraveen4618@gmail.com", "pvpc nnno kmjt nzyh");
                            smtp.EnableSsl = true;
                            smtp.Send(msg);
                        }
                    }
                    catch
                    {
                        // log, but keep sending to others
                    }
                }

                lblInfo1.Text += $"Messages were sent to {emails.Count} emails successfully !";
                lblInfo1.ForeColor = System.Drawing.Color.Gray;
            }
            catch
            {
                // log global error
                lblInfo1.Text += "Messages didnt send . error";
                lblInfo1.ForeColor = System.Drawing.Color.Red;

            }
        }
        protected void sourceAds_Deleted(object sender, SqlDataSourceStatusEventArgs e)
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

        protected void sourceAds_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                lblInfo1.Text = e.Exception.Message;
                lblInfo1.ForeColor = System.Drawing.Color.Red;

                e.ExceptionHandled = true;
            }
        }
    }
}