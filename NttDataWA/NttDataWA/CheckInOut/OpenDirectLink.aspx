<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpenDirectLink.aspx.cs"
    Inherits="NttDataWA.CheckInOut.OpenDirectLink" %>

<html>
<head runat="server">
<link runat="server" type="text/css" rel="stylesheet" id="Link1" visible="false" href="~/Css/servicepage.css" />
</head>
<body>
<iframe id="fra_main" name="fra_main" frameborder="0" width="99%" height="99%" marginwidth="0" marginheight="0"  noresize="noresize" scrolling="auto" runat="server"/>
<asp:PlaceHolder ID="messager" runat="server" Visible="false">
        <div class="messager">
            <div class="messager_c1"><img src="../Images/Common/messager_error.png" alt="" /></div>
            <div class="messager_c2"><span><asp:Label ID="lblTxt" runat="server" CssClass="lblTxt"></asp:Label></span></div>
            <div class="messager_c3"><img src="../Images/Common/messager_error.png" alt="" /></div>
        </div>
        <div class="Space"></div>
</asp:PlaceHolder>
</body>
</html>
