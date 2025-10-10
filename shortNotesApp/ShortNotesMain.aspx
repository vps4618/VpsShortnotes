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
</head>
<body>




    <!-- Points -->
    <!-- 1.FOCUS ON SAVE THEME TO SESSION URL
        2. when uploaded to somee must set ShortNotesMain.aspx/SetTheme path to SetTheme
        -->








    <!--here data-ajax=false used for fix not loading name list when enter pressed on text box and post back with jquery mobile-->
    <form id="form1" runat="server" data-ajax="false" defaultbutton="btnSearch">
        <!--
            don't wrap div with data-role=content inside data-role=page . then autocompleteextender will not work.style for div style="height:100vh" will cause not working auto completer-->

        <!-- TODO -->
        <!-- search with diffrent subject feature + datatable with 3 subjects with lesson no -->
        <div runat="server" data-role="page" id="home" data-theme="b">
            <div data-role="header">
                Short Notes
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
                    <a href="#" id="toggleTheme" class="ui-btn ui-btn-icon-notext ui-icon-refresh toggle-btn" title="Toggle Theme">Toggle</a>

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
