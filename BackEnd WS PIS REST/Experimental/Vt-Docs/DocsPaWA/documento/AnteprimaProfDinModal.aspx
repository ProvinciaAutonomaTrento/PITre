<%@ Page language="c#" Codebehind="AnteprimaProfDinModal.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.AntperimaProfDinModal" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head id="Head1" runat="server">
    <title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="GridLayout">
	
    <form id="Form1" method="post" runat="server">
	<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Anteprima" />
		<iframe width="100%" height="100%" frameborder="0" scrolling="auto" src="<%Response.Write(Request.QueryString["Chiamante"]);%>?reset=<%Response.Write(Request.QueryString["reset"]);%>"></iframe>
	
     </form>
	
  </body>
</html>
