<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubjects.aspx.cs" Inherits="shortNotesApp.AddSubjects" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add subjects</title>
    <link href="/Content/jquery.mobile-1.4.5.css" rel="stylesheet" />
    <script type="text/javascript" src="/Scripts/jquery-1.8.0.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.mobile-1.4.5.js"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        iframe[src*="somee.com"],
        script[src*="somee.com"],
        a[href*="somee.com"],
        div[style*="z-index"] {
            display: none !important;
            visibility: hidden !important;
        }
    </style>
    <style>
        td {
            padding: 10px;
        }

        html, body {
            height: 100%;
            margin: 0;
        }

        .half-height {
            height: 100vh; /* 50% of the viewport height */
            box-sizing: border-box;
            padding: 1em;
        }

        .scroll-container {
            overflow-x: auto;
            width: 100%;
        }

            .scroll-container table {
                width: 100%;
                /*  min-height:600px; */ /* makes sure scroll appears on small screens */
            }
    </style>
    <script>
        $(document).on("mobileinit", function () {
            $.mobile.ajaxEnabled = false;
            $.mobile.linkBindingEnabled = false;
            $.mobile.hashListeningEnabled = false;
        });
    </script>

    <!--caching password--->
    <script>
        function saveCredentials(username, password) {
            localStorage.setItem("sn_username", username);
            localStorage.setItem("sn_password", password);
        }

    </script>
    <script type="text/javascript">
        function askForConfirmDelete() {
            var answer = confirm("Do you really want to delete this subject?");
            // store result in hidden field
            document.getElementById('<%= hfDeleteChoice.ClientID %>').value = answer;
        }
    </script>

</head>
<body>
    <form id="form1" runat="server" data-ajax="false">
        <div data-role="page" data-theme="b">
            <div data-role="header">
                Add Subjects
           
            </div>
            <div data-role="content">
                <asp:Panel runat="server" ID="pnlPwd">
                    UserName : 
                   
                    <asp:TextBox runat="server" ID="txtUName"></asp:TextBox>
                    Password : 
                   
                    <asp:TextBox runat="server" ID="txtPwd" TextMode="Password"></asp:TextBox>
                    <asp:Button runat="server" ID="Button1" Text="Submit" OnClick="Button1_Click" />
                    <asp:Label runat="server" ID="lblInfo"></asp:Label>
                </asp:Panel>
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
        </div>
    </form>
    
    <!--script to remove ads-->
    <script>
        window.onload = function () {

            // Remove all scripts containing "somee.com"
            document.querySelectorAll("script").forEach(script => {
                if (script.src.includes("somee.com") || script.innerHTML.includes("somee.com")) {
                    script.remove();
                }
            });

            // Remove the "Web hosting by Somee.com" banner
            document.querySelectorAll("a[href='http://somee.com']").forEach(link => {
                link.parentElement.remove();
            });

            // Remove injected comments
            document.body.childNodes.forEach(node => {
                if (node.nodeType === Node.COMMENT_NODE && node.nodeValue.includes("SCRIPT GENERATED BY SERVER")) {
                    node.remove();
                }
            });
        };
    </script>
</body>
</html>
