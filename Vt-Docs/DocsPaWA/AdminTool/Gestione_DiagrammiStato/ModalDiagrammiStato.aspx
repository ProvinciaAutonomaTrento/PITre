<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ModalDiagrammiStato.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_DiagrammiStato.ModalDiagrammiStato" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
<head>
    <title>DOCSPA - AMMINISTRAZIONE > Diagrammi Stato</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
</head>
<body MS_POSITIONING="GridLayout">
   
   <iframe width="100%" height="100%" src="<%Response.Write(Request.QueryString["Chiamante"]);%>"></iframe>
   
</body>
</html>
