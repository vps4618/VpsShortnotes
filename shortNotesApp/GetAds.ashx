<%@ WebHandler Language="C#" Class="GetAds" %>
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;

public class GetAds : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        string conString = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
        var list = new List<object>();

        try
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                // return only active ads; adjust WHERE as needed
                string sql = "SELECT AdId, Title, Message, Link, CreatedAt FROM Ads WHERE IsActive = 1 ORDER BY CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new
                            {
                                id = Convert.ToInt32(rdr["AdId"]),
                                title = rdr["Title"].ToString(),
                                message = rdr["Message"] == DBNull.Value ? "" : rdr["Message"].ToString(),
                                link = rdr["Link"] == DBNull.Value ? "" : rdr["Link"].ToString(),
                                createdAt = ((DateTime)rdr["CreatedAt"]).ToString("o") // ISO 8601
                            });
                        }
                    }
                }
                con.Close();
            }
        }
        catch (Exception ex)
        {
            var err = new { error = true, message = ex.Message };
            context.Response.Write(new JavaScriptSerializer().Serialize(err));
            return;
        }

        context.Response.Write(new JavaScriptSerializer().Serialize(list));
    }

    public bool IsReusable { get { return false; } }
}