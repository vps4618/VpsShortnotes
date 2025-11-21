<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubjects.aspx.cs" Inherits="shortNotesApp.AddSubjects" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<script type="text/javascript">
    function askForConfirmDelete() {
        var answer = confirm("Do you really want to delete this subject?");
        // store result in hidden field
        document.getElementById('<%= hfDeleteChoice.ClientID %>').value = answer;
    }
</script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <!--This panel will use for insert new subjects-->
            <asp:Panel runat="server" ID="pnlInsert">
                <asp:Label Text="Subject : " runat="server"></asp:Label>
                &nbsp;
            <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" />
                &nbsp;&nbsp;
            <asp:Button ID="btnViewUpdatePnl" runat="server" OnClick="btnViewUpdatePnl_Click" Text="Click here to Update,Delete Subjects" />
            </asp:Panel>

            <!--This panel will use for subject deleting and updating-->
            <asp:Panel runat="server" ID="pnlUpdateDelete">
                <asp:Label Text="Id : " runat="server"></asp:Label>
                &nbsp;
            <asp:DropDownList ID="lstIds" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstIds_SelectedIndexChanged"></asp:DropDownList>
                <br />
                <br />
                <asp:Label Text="Subject Name : " runat="server"></asp:Label>
                &nbsp;
            <asp:TextBox ID="txtSubjectName" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" />
                &nbsp;&nbsp;

                   <!-- HiddenField to store user choice -->
                <asp:HiddenField ID="hfDeleteChoice" runat="server" />

                <!-- Delete button with client and server events -->
                <asp:Button ID="btnDelete" runat="server" Text="Delete"
                    OnClientClick="askForConfirmDelete();"
                    OnClick="btnDelete_Click" />

                &nbsp;&nbsp;
                <asp:Button ID="btnGoBack" runat="server" Text="Go Back To Insert" OnClick="btnGoBack_Click" />
            </asp:Panel>
            <br />
            <br />
            <asp:Label runat="server" ID="info"></asp:Label>
            <br />
            <asp:GridView ID="gridSubjects" runat="server" />
        </div>
    </form>
</body>
</html>
