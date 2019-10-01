<%@ Page language="c#" Codebehind="ListeDistrFrame.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.RubricaDocsPA.ListeDistrFrame" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head runat = "server">
  		<title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Liste distribuzione" />
		<iframe src=ListeDistr.aspx scrolling=no width="100%" height="100%"></iframe>
     </form>
  </body>
</html>
