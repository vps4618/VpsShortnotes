<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testingName.aspx.cs" Inherits="shortNotesApp.testingName" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function clearStorage() {
            localStorage.removeItem("userName");
            alert("done clearing !");
        }

        function setName() {
            let name = document.getElementById("txtName").value;
            localStorage.removeItem("userName");
            localStorage.setItem("userName", name);
            alert("done saving !")
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <input id="txtName" type="text" />
            <br />
            <br />
            <button onclick="clearStorage()">
                clear
            </button>
            <br /><br />
            <button onclick="setName()">
                Set Name
            </button>
            
         </div>       
    </form>
</body>
</html>
