<%@ Page language="c#" Codebehind="ModalAnteprimaProfDinamica.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamica.ModalAnteprimaProfDinamica" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
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
      <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Anteprima" />
    <iframe width="100%" height="100%" src="<%Response.Write(Request.QueryString["Chiamante"]);%>?IdOggCustom=<%Response.Write(Request.QueryString["IdOggCustom"]);%>&IdTemplate=<%Response.Write(Request.QueryString["IdTemplate"]);%>"></iframe>
  </body>
</html>
