using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Transfer Object for Subject
/// </summary>
public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Data Transfer Object for Note
/// </summary>
public class NoteDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Data Access Layer for Subjects and Notes
/// </summary>
public class DataAccess
{
    private readonly string connectionString;

    public DataAccess()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    /// <summary>
    /// Get all subjects from the database
    /// </summary>
    public List<SubjectDto> GetAllSubjects()
    {
        var subjects = new List<SubjectDto>();

        using (var connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Name, Slug, Description, CreatedDate FROM dbo.Subjects ORDER BY Name";
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subjects.Add(new SubjectDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Slug = reader.GetString(2),
                            Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            CreatedDate = reader.GetDateTime(4)
                        });
                    }
                }
            }
        }

        return subjects;
    }

    /// <summary>
    /// Get a subject by its slug
    /// </summary>
    public SubjectDto GetSubjectBySlug(string slug)
    {
        SubjectDto subject = null;

        using (var connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Name, Slug, Description, CreatedDate FROM dbo.Subjects WHERE Slug = @Slug";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Slug", slug);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        subject = new SubjectDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Slug = reader.GetString(2),
                            Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            CreatedDate = reader.GetDateTime(4)
                        };
                    }
                }
            }
        }

        return subject;
    }

    /// <summary>
    /// Create a new subject
    /// </summary>
    public int CreateSubject(string name, string slug, string description)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO dbo.Subjects (Name, Slug, Description, CreatedDate) 
                           VALUES (@Name, @Slug, @Description, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Slug", slug);
                command.Parameters.AddWithValue("@Description", description ?? string.Empty);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
    }

    /// <summary>
    /// Delete a subject by ID
    /// </summary>
    public bool DeleteSubject(int subjectId)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            // First delete all notes for this subject
            string deleteNotesQuery = "DELETE FROM dbo.Notes WHERE SubjectId = @SubjectId";
            using (var command = new SqlCommand(deleteNotesQuery, connection))
            {
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                connection.Open();
                command.ExecuteNonQuery();
            }

            // Then delete the subject
            string deleteSubjectQuery = "DELETE FROM dbo.Subjects WHERE Id = @SubjectId";
            using (var command = new SqlCommand(deleteSubjectQuery, connection))
            {
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }

    /// <summary>
    /// Get all notes for a specific subject
    /// </summary>
    public List<NoteDto> GetNotesBySubject(int subjectId)
    {
        var notes = new List<NoteDto>();

        using (var connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, SubjectId, Title, Content, CreatedDate FROM dbo.Notes WHERE SubjectId = @SubjectId ORDER BY CreatedDate DESC";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notes.Add(new NoteDto
                        {
                            Id = reader.GetInt32(0),
                            SubjectId = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            Content = reader.GetString(3),
                            CreatedDate = reader.GetDateTime(4)
                        });
                    }
                }
            }
        }

        return notes;
    }

    /// <summary>
    /// Create a new note
    /// </summary>
    public int CreateNote(int subjectId, string title, string content)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO dbo.Notes (SubjectId, Title, Content, CreatedDate) 
                           VALUES (@SubjectId, @Title, @Content, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Content", content);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
    }
}
