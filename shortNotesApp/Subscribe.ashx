<%@ WebHandler Language="C#" Class="SubscribeHandler" %>
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

public class SubscribeHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        try
        {
            string email = (context.Request.Form["email"] ?? "").Trim();

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"success\":false,\"message\":\"Invalid email\"}");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("IF NOT EXISTS (SELECT 1 FROM Subscribers WHERE Email = @Email) " +
                                           "INSERT INTO Subscribers (Email) VALUES (@Email)", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Notify owner by email
            SendOwnerNotification(email);

            context.Response.Write("{\"success\":true,\"message\":\"Subscribed\"}");
        }
        catch (Exception ex)
        {
            // Log exception as appropriate
            context.Response.StatusCode = 500;
            context.Response.Write("{\"success\":false,\"message\":\"Server error\"}");
        }
    }

    private bool IsValidEmail(string email)
    {
        // Simple regex; use a more robust validation for production
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private void SendOwnerNotification(string subscriberEmail)
    {
        try
        {

            /*    // Compose email with both name and message
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

        return "{\"status\":\"sent\"}";*/
            var mail =
                  new MailMessage();
            mail.To.Add("vishwapraveen30145@gmail.com");
            mail.Subject = "New subscriber to ShortNotes";
            mail.Body = $"A user subscribed with email: {subscriberEmail}";
            mail.From = new MailAddress("vishwapraveen4618@gmail.com");

            // From address taken from Web.config mailSettings 'from'.
            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("vishwapraveen4618@gmail.com", "pvpc nnno kmjt nzyh");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }
        catch
        {
            // Don't throw to user; log the failure instead
        }
    }

    public bool IsReusable => false;
}