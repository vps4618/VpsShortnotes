<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShortNotesMain.aspx.cs" Inherits="shortNotesApp.ShortNotesMain" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Short Notes</title>

    <link href="/Content/jquery.mobile-1.4.5.css" rel="stylesheet" />
    <script type="text/javascript" src="/Scripts/jquery-1.8.0.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.mobile-1.4.5.js"></script>
    <!--function for change web method based on subjects-->
    <script type="text/javascript">
        function subjectChanged() {
            var ddl = document.getElementById('<%= lstSubjects.ClientID %>');
            var selectedValue = ddl.options[ddl.selectedIndex].value;

            // Get the AutoCompleteExtender control
            var autoCompleteExtender = $find('<%= autoComplete1.ClientID %>');

            // Set the service method based on the selected option
            if (selectedValue == "1") {
                autoCompleteExtender.set_serviceMethod("GetCombinedSuggestions");
            } else if (selectedValue == "2") {
                autoCompleteExtender.set_serviceMethod("GetChemistrySuggestions");
            } else if (selectedValue == "3") {
                autoCompleteExtender.set_serviceMethod("GetPhysicsSuggestions");
            }
        }
    </script>

    <!--styles for theme toggle button-->
    <style>
        .toggle-btn {
            position: fixed;
            top: 10px;
            right: 10px;
            z-index: 999;
        }

        .toggle-btn-2 {
            position: fixed;
            top: 50px;
            right: 50px;
            z-index: 999;
        }
    </style>
    <style>
        iframe[src*="somee.com"],
        script[src*="somee.com"],
        a[href*="somee.com"],
        div[style*="z-index"] {
            display: none !important;
            visibility: hidden !important;
        }
    </style>
    <!--script for toggle-->
    <script>
        $(document).on("pagecreate", "#home", function () {
            // Get theme from ASP.NET session
            let currentTheme = "<%= Session["Theme"] ?? "b" %>"; // fallback to "a"

            $("#toggleTheme").on("click", function () {
                // Toggle between theme "a" and "b"
                currentTheme = currentTheme === "b" ? "a" : "b";

                // Change theme on the content
                changeTheme(currentTheme);
                setThemeToSession(currentTheme);
            });

            $("#contactMeLink").on("click", function () {
                sendContactMessage();
            });

            function changeTheme(theme) {
                // Change page background and content
                $("[data-role='page']")
                    .attr("data-theme", theme)
                    .removeClass("ui-body-a ui-body-b ui-bar-a ui-bar-b")
                    .addClass(theme === "a" ? "ui-body-a ui-bar-a" : "ui-body-b ui-bar-b");
            }
        });

        // save theme to session ***   change aspx file name when name changed    ***
        function setThemeToSession(theme) {
            $.ajax({
                type: "POST",
                url: "ShortNotesMain.aspx/SetTheme", // or your page name
                data: JSON.stringify({ theme: theme }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
        }

        // this will send message to owner
        function sendContactMessage() {
            var name = localStorage.getItem('userName');
            var message = prompt("Please enter your message.(If you need further help, also mention your whatsapp number or email)");
            if (name && name.trim() && message && message.trim()) {
                $.ajax({
                    type: "POST",
                    url: "ShortNotesMain.aspx/SendContactMessage",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ userName: name, userMessage: message }),
                    success: function () {
                        alert("Your message has been sent!");
                    },
                    error: function () { alert("Failed to send message."); }
                });
            } else {
                alert("Please enter your message.");
            }
        }
    </script>
    <style>
        td {
            padding: 10px;
        }

        html, body {
            height: 100%;
            margin: 0;
        }

        .half-height {
            height: 50vh; /* 50% of the viewport height */
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
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <!--Help to get name and log user info-->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Check if we already logged user data in this session
            if (!sessionStorage.getItem('userLogged')) {
                let name = localStorage.getItem('userName');
                if (!name) {
                    name = prompt("Please enter your name:");
                    if (name) localStorage.setItem('userName', name);
                }
                // Send to server only the first time
                if (name) {
                    fetch('ShortNotesMain.aspx/LogUserName', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ userName: name })
                    })
                        .then(response => response.json())
                        .then(data => {
                            // Mark as logged
                            sessionStorage.setItem('userLogged', 'true');
                        })
                        .catch(err => {
                            console.error('Error sending user name:', err);
                        });
                }
            }
        });
/*
        document.getElementById('contactLink').onclick = function (e) {
            e.preventDefault();
            var message = prompt("Type your message for the site owner:");
            var name = localStorage.getItem('userName');
            if (name && name.trim() && message && message.trim()) {
                $.ajax({
                    type: "POST",
                    url: "ShortNotesMain.aspx/SendContactMessage",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ userName: name, userMessage: message }),
                    success: function () {
                        alert("Your message has been sent!");
                        $('#contactPopup').popup('close');
                        $('#contactMessage').val(""); // Clear
                        $('#contactName').val("");
                    },
                    error: function () { alert("Failed to send message."); }
                });
            } else {
                alert("Please enter your name and message.");
            }
        };*/
    </script>

</head>
<body>




    <!-- Points -->
    <!-- 1.FOCUS ON SAVE THEME TO SESSION URL
        2. when uploaded to somee must set ShortNotesMain.aspx/SetTheme path to SetTheme
        -->








    <!--here data-ajax=false used for fix not loading name list when enter pressed on text box and post back with jquery mobile-->
    <form id="form1" runat="server" data-ajax="false" defaultbutton="btnSearch">
        <asp:HiddenField ID="hdnUserName" runat="server" />
        <!--
            don't wrap div with data-role=content inside data-role=page . then autocompleteextender will not work.style for div style="height:100vh" will cause not working auto completer-->

        <!-- TODO -->
        <!-- search with diffrent subject feature + datatable with 3 subjects with lesson no -->
        <div runat="server" data-role="page" id="home" data-theme="b">
            <div data-role="header">
                Short Notes
                <!--If data-role='none' then jquery styling not will affect to this element-->
                   <a href="#" data-role="none" data-rel="popup" class="ui-btn-right" id="contactMeLink" style="color:lightgray;background-color:darkslateblue;font-family:'Segoe UI';font-style:">Contact Me (සම්බන්ධ කරගැනීමට)</a>
            </div>
            <div data-role="content">
                <div class="half-height">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                    Subject : 
            <asp:DropDownList runat="server" ID="lstSubjects" onchange="subjectChanged()">
                <asp:ListItem Text="Combined Maths" Value="1"></asp:ListItem>
                <asp:ListItem Text="Chemistry" Value="2"></asp:ListItem>
                <asp:ListItem Text="Physics" Value="3"></asp:ListItem>
            </asp:DropDownList>
                    Title : 
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    <asp:Button Text="Search" runat="server" ID="btnSearch" OnClick="btnSearch_Click" />
                    <asp:Button Text="Get All" runat="server" ID="btnGetAll" OnClick="btnGetAll_Click" />
                    <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server"
                        TargetControlID="txtName" ServiceMethod="GetCombinedSuggestions" MinimumPrefixLength="0">
                    </ajaxToolkit:AutoCompleteExtender>

                    <asp:Label runat="server" ID="lblInfo"></asp:Label>
                </div>

                <div class="scroll-container half-height">
                    <asp:GridView runat="server" ID="gridShortNotes" AutoGenerateColumns="False"
                        CssClass="ui-responsive table-stroke">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="Link" HeaderText="" Text="View" Target="_blank" />
                            <asp:BoundField DataField="Title" HeaderText="Title" />
                            <asp:BoundField DataField="LessonNo" HeaderText="Lesson No." />
                        </Columns>
                    </asp:GridView>
                    <a href="#" id="toggleTheme" class="ui-btn ui-btn-icon-notext ui-icon-refresh toggle-btn" title="Toggle Theme" style="display:none" >Toggle</a>

                </div>
            </div>
        </div>

    </form>

   

    <!--this script is needed for properly work auto extender with jquery mobile-->
    <script type="text/javascript">
        Sys.UI.DomElement.getBounds = function (element) {
            var offset = $(element).offset();
            var width = $(element).outerWidth();
            var height = $(element).outerHeight();

            return new Sys.UI.Bounds(
                Math.round(offset.left),
                Math.round(offset.top),
                Math.round(width),
                Math.round(height)
            );
        };

    </script>

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
