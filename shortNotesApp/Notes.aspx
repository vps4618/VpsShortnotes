<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notes.aspx.cs" Inherits="shortNotesApp.Notes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Short Notes - Dynamic Subject Viewer</title>
    <link href="/Content/bootstrap.min.css" rel="stylesheet" />
    <!-- DataTables CSS -->
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.11.5/css/jquery.dataTables.min.css" />
    <style>
        body {
            padding: 20px;
        }
        .container {
            max-width: 1400px;
            margin: 0 auto;
        }
        .page-header {
            border-bottom: 2px solid #007bff;
            padding-bottom: 10px;
            margin-bottom: 20px;
        }
        .form-section {
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 5px;
            margin-bottom: 20px;
        }
        #notesTable {
            width: 100%;
        }
        .note-content {
            max-width: 400px;
            white-space: pre-wrap;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="page-header">
                <h1>Short Notes</h1>
                <p class="text-muted">View and manage notes by subject</p>
            </div>

            <div class="form-section">
                <div class="row">
                    <div class="col-md-4">
                        <div class="form-group">
                            <label for="ddlSubjects">Select Subject:</label>
                            <asp:DropDownList ID="ddlSubjects" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col-md-2">
                        <label>&nbsp;</label>
                        <button type="button" id="btnLoadNotes" class="btn btn-primary form-control">Load Notes</button>
                    </div>
                </div>
            </div>

            <div class="form-section">
                <h4>Add New Note</h4>
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="txtTitle">Title:</label>
                            <input type="text" id="txtTitle" class="form-control" placeholder="Note title" />
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="form-group">
                            <label for="txtContent">Content:</label>
                            <textarea id="txtContent" class="form-control" rows="3" placeholder="Note content"></textarea>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <label>&nbsp;</label>
                        <button type="button" id="btnAddNote" class="btn btn-success form-control">Add Note</button>
                    </div>
                </div>
                <div id="addNoteMessage" class="alert" style="display:none;"></div>
            </div>

            <div>
                <h3>Notes</h3>
                <table id="notesTable" class="display">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Title</th>
                            <th>Content</th>
                            <th>Created Date</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
        </div>
    </form>

    <!-- jQuery -->
    <script src="/Scripts/jquery-1.8.0.js"></script>
    <!-- DataTables JS -->
    <script type="text/javascript" charset="utf8" src="https://cdn.datatables.net/1.11.5/js/jquery.dataTables.min.js"></script>
    <!-- Bootstrap JS -->
    <script src="/Scripts/bootstrap.min.js"></script>

    <script type="text/javascript">
        var notesDataTable;

        $(document).ready(function () {
            // Initialize DataTable
            notesDataTable = $('#notesTable').DataTable({
                columns: [
                    { data: 'Id' },
                    { data: 'Title' },
                    { data: 'Content', className: 'note-content' },
                    { data: 'CreatedDate' }
                ],
                order: [[0, 'desc']]
            });

            // Check for slug in query string and load notes if present
            var urlParams = new URLSearchParams(window.location.search);
            var slug = urlParams.get('slug');
            if (slug) {
                $('#<%= ddlSubjects.ClientID %>').val(slug);
                loadNotes();
            }

            // Load notes button click
            $('#btnLoadNotes').click(function () {
                loadNotes();
            });

            // Add note button click
            $('#btnAddNote').click(function () {
                addNote();
            });
        });

        function loadNotes() {
            var subjectId = $('#<%= ddlSubjects.ClientID %>').val();
            
            if (!subjectId) {
                showMessage('Please select a subject', 'alert-warning');
                return;
            }

            $.ajax({
                type: "POST",
                url: "Notes.aspx/GetNotes",
                data: JSON.stringify({ subjectId: parseInt(subjectId) }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    notesDataTable.clear();
                    if (response.d && response.d.length > 0) {
                        notesDataTable.rows.add(response.d);
                    }
                    notesDataTable.draw();
                },
                error: function (xhr, status, error) {
                    alert('Error loading notes: ' + error);
                }
            });
        }

        function addNote() {
            var subjectId = $('#<%= ddlSubjects.ClientID %>').val();
            var title = $('#txtTitle').val().trim();
            var content = $('#txtContent').val().trim();

            if (!subjectId) {
                showAddNoteMessage('Please select a subject', 'alert-warning');
                return;
            }

            if (!title) {
                showAddNoteMessage('Please enter a title', 'alert-warning');
                return;
            }

            if (!content) {
                showAddNoteMessage('Please enter content', 'alert-warning');
                return;
            }

            $.ajax({
                type: "POST",
                url: "Notes.aspx/AddNote",
                data: JSON.stringify({ 
                    subjectId: parseInt(subjectId), 
                    title: title, 
                    content: content 
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d > 0) {
                        showAddNoteMessage('Note added successfully!', 'alert-success');
                        $('#txtTitle').val('');
                        $('#txtContent').val('');
                        loadNotes();
                    } else {
                        showAddNoteMessage('Failed to add note', 'alert-danger');
                    }
                },
                error: function (xhr, status, error) {
                    showAddNoteMessage('Error adding note: ' + error, 'alert-danger');
                }
            });
        }

        function showAddNoteMessage(message, cssClass) {
            $('#addNoteMessage')
                .removeClass('alert-success alert-danger alert-warning')
                .addClass(cssClass)
                .text(message)
                .show();

            setTimeout(function () {
                $('#addNoteMessage').fadeOut();
            }, 3000);
        }
    </script>
</body>
</html>
