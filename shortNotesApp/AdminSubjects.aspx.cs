using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace shortNotesApp
{
    public partial class AdminSubjects : System.Web.UI.Page
    {
        private DataAccess dataAccess = new DataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSubjectsGrid();
            }
        }

        private void BindSubjectsGrid()
        {
            try
            {
                List<SubjectDto> subjects = dataAccess.GetAllSubjects();
                gvSubjects.DataSource = subjects;
                gvSubjects.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading subjects: " + ex.Message, "alert-danger");
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                string name = txtName.Text.Trim();
                string slug = txtSlug.Text.Trim().ToLower();
                string description = txtDescription.Text.Trim();

                // Check if slug already exists
                var existingSubject = dataAccess.GetSubjectBySlug(slug);
                if (existingSubject != null)
                {
                    ShowMessage("A subject with this slug already exists. Please use a different slug.", "alert-warning");
                    return;
                }

                int newId = dataAccess.CreateSubject(name, slug, description);
                
                if (newId > 0)
                {
                    ShowMessage("Subject created successfully!", "alert-success");
                    ClearForm();
                    BindSubjectsGrid();
                }
                else
                {
                    ShowMessage("Failed to create subject. Please try again.", "alert-danger");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error creating subject: " + ex.Message, "alert-danger");
            }
        }

        protected void gvSubjects_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int subjectId = Convert.ToInt32(gvSubjects.DataKeys[e.RowIndex].Value);
                bool success = dataAccess.DeleteSubject(subjectId);

                if (success)
                {
                    ShowMessage("Subject deleted successfully!", "alert-success");
                    BindSubjectsGrid();
                }
                else
                {
                    ShowMessage("Failed to delete subject. Please try again.", "alert-danger");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting subject: " + ex.Message, "alert-danger");
            }
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "alert " + cssClass;
            lblMessage.Visible = true;
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtSlug.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }
    }
}
