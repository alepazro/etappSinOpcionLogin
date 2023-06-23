<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="rptExport.aspx.vb" Inherits="etapp.rptExport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="https://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reports Export Utility</title>

    <link href="Styles/common.css" rel="stylesheet" type="text/css" />
    <link href="Styles/app.css" rel="stylesheet" type="text/css" />
    <link href="Styles/logo.css" rel="stylesheet" type="text/css" />

</head>
<body>
    <div id="header">
        <div id="topMenu">
            <a href="#" onclick="logout()" class="loginDivClass">Logout</a>
            <div class="logoPosition">
                <span id="dealerBrand" class="logoDesign"></span>
            </div>
            <div id="welcomeTitleDiv">
                <span id="welcomeTitleSpan"></span>
            </div>
        </div>
    </div>

    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
