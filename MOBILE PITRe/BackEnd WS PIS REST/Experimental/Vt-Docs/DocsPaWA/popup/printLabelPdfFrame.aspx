<%@ Page language="c#" Codebehind="printLabelPdfFrame.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.printLabelPdf_Frame" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Etichetta PDF</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</head>

	<body MS_POSITIONING="GridLayout">
		<form id="FrameLabel" method="post" runat="server">
		<iframe src=<% if(Request.QueryString["proto"]=="true") Response.Write ("printLabelPdf.aspx?proto=true"); else Response.Write("printLabelPdf.aspx"); %> scrolling=no width="100%" height="100%"></iframe> -->
		
		</form>
	</body>
</html>
