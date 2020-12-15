<%@ Page CodeBehind="SmistaDoc_Container.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="DocsPAWA.smistaDoc.SmistaDoc_Container" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN">
<HTML>
	<HEAD>
		<TITLE>Smistamento dei documenti</TITLE>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<frameset border="3" frameSpacing="3" frameBorder="3" style="cursor: e-resize;" cols="50%,50%">
		<frame name="left" src="../blank_page.htm" marginheight="0px", marginwidth="0px" style="cursor: e-resize; border-right-style:solid; border-right-width: medium" scrolling="no">
		<frame name="right" src="SmistaDoc_Detail.aspx?DOC_NUMBER=<%=Request.QueryString["DOC_NUMBER"]%>" marginheight="0px", marginwidth="0px" style="padding:0;" scrolling="auto">
	</frameset>
</HTML>
