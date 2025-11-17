<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubjects.aspx.cs" Inherits="shortNotesApp.AddSubjects" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label Text="Subject : " runat="server"></asp:Label>
            &nbsp;
            <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" />
            <br />
            <br />
            <asp:Label runat="server" ID="info"></asp:Label>
            <br />
            <asp:GridView ID="gridSubjects" runat="server"
 />
        </div>
    </form>
</body>
</html>
