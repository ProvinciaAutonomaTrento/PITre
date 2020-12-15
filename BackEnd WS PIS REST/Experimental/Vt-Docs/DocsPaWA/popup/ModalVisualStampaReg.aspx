<%@ Page language="c#" AutoEventWireup="false"%>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN">
<html>
  <head runat = "server">
  		<title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
<body>
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Stampa registro" />
<iframe id="_FRAME_REPORT"  name="_FRAME_REPORT" scrolling="no" src="<%Response.Write ("visualStampaReg.aspx" + Request.Url.Query);%>" width="100%" height="100%" />
</body>
</html>
