<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AjaxPopupControl._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<script src="http://code.jquery.com/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/FDB_tools.js" type="text/javascript"></script><!-- to remove in def page -->
    <link href="css/flick/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <website:ajaxpopup2 id="filippo" runat="server"
            text="Apri popup"
            title="Titolo popup di prova"
            url="page1.aspx"
            isfullscreen="true"
            permitclose="false"
            permitscroll="false"
         />
    </form>
</body>
</html>
