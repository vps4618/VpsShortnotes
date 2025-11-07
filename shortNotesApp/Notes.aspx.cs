using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class Notes : System.Web.UI.Page
    {
        private DataAccess dataAccess = new DataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSubjects();
            }
        }

        private void LoadSubjects()
        {
            try
            {
                List<SubjectDto> subjects = dataAccess.GetAllSubjects();
                
                ddlSubjects.Items.Clear();
                ddlSubjects.Items.Add(new ListItem("-- Select a Subject --", ""));

                foreach (var subject in subjects)
                {
                    ddlSubjects.Items.Add(new ListItem(subject.Name, subject.Id.ToString()));
                }

                // Check if slug is provided in query string
                string slug = Request.QueryString["slug"];
                if (!string.IsNullOrEmpty(slug))
                {
                    var subject = dataAccess.GetSubjectBySlug(slug);
                    if (subject != null)
                    {
                        ListItem item = ddlSubjects.Items.FindByValue(subject.Id.ToString());
                        if (item != null)
                        {
                            ddlSubjects.SelectedValue = subject.Id.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error - in production, use proper logging
                Response.Write($"Error loading subjects: {ex.Message}");
            }
        }

        /// <summary>
        /// WebMethod to get notes for a specific subject
        /// </summary>
        [WebMethod]
        public static List<NoteDto> GetNotes(int subjectId)
        {
            try
            {
                var dataAccess = new DataAccess();
                return dataAccess.GetNotesBySubject(subjectId);
            }
            catch (Exception ex)
            {
                // Log error - in production, use proper logging
                throw new Exception("Error retrieving notes: " + ex.Message);
            }
        }

        /// <summary>
        /// WebMethod to add a new note
        /// </summary>
        [WebMethod]
        public static int AddNote(int subjectId, string title, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Title is required");

                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("Content is required");

                var dataAccess = new DataAccess();
                return dataAccess.CreateNote(subjectId, title, content);
            }
            catch (Exception ex)
            {
                // Log error - in production, use proper logging
                throw new Exception("Error adding note: " + ex.Message);
            }
        }
    }
}
