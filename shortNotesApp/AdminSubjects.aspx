<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminSubjects.aspx.cs" Inherits="shortNotesApp.AdminSubjects" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin - Manage Subjects</title>
    <link href="/Content/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            padding: 20px;
        }
        .container {
            max-width: 1200px;
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
            margin-bottom: 30px;
        }
        .alert {
            margin-top: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="page-header">
                <h1>Manage Subjects</h1>
                <p class="text-muted">Create and manage subjects for your short notes application</p>
            </div>

            <!-- TODO: Add authentication/authorization to restrict access to admin users only -->
            
            <div class="form-section">
                <h3>Add New Subject</h3>
                <div class="row">
                    <div class="col-md-4">
                        <div class="form-group">
                            <label for="txtName">Subject Name:</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="e.g., Physics" />
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                                ControlToValidate="txtName" 
                                ErrorMessage="Name is required" 
                                CssClass="text-danger" 
                                Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group">
                            <label for="txtSlug">Slug (URL-friendly):</label>
                            <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control" placeholder="e.g., physics" />
                            <asp:RequiredFieldValidator ID="rfvSlug" runat="server" 
                                ControlToValidate="txtSlug" 
                                ErrorMessage="Slug is required" 
                                CssClass="text-danger" 
                                Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revSlug" runat="server" 
                                ControlToValidate="txtSlug" 
                                ValidationExpression="^[a-z0-9-]+$" 
                                ErrorMessage="Slug must be lowercase letters, numbers, and hyphens only" 
                                CssClass="text-danger" 
                                Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group">
                            <label for="txtDescription">Description:</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="Optional description" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <asp:Button ID="btnCreate" runat="server" Text="Create Subject" CssClass="btn btn-primary" OnClick="btnCreate_Click" />
                        <asp:Label ID="lblMessage" runat="server" CssClass="alert" />
                    </div>
                </div>
            </div>

            <div>
                <h3>Existing Subjects</h3>
                <asp:GridView ID="gvSubjects" runat="server" 
                    AutoGenerateColumns="False" 
                    CssClass="table table-striped table-bordered" 
                    DataKeyNames="Id"
                    OnRowDeleting="gvSubjects_RowDeleting"
                    EmptyDataText="No subjects found. Create your first subject above.">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Slug" HeaderText="Slug" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" 
                            ControlStyle-CssClass="btn btn-danger btn-sm" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
    <script src="/Scripts/bootstrap.min.js"></script>
</body>
</html>
