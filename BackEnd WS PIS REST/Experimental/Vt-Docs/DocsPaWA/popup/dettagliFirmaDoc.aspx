<%@ Page language="c#" Codebehind="dettagliFirmaDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.dettagliFirmaDoc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<TITLE>Dettagli firma digitale</TITLE>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
		
		function CloseForm()
		{
			this.close();
		}
		
		</script>
	</HEAD>
	<form id="dettagliFirmaDoc" method="post" runat="server" >
	<frameset rows="95%,5%">
		<frameset cols="25%,75%">
			<frame name="left" src="dettagliFirmaTree.aspx?SIGNED_DOCUMENT_ON_SESSION=<%=Request.QueryString["SIGNED_DOCUMENT_ON_SESSION"]%>" DESIGNTIMEDRAGDROP="28">
			<frame name="right" src="..\waitingPageVerifica.htm">
		</frameset>
		<frame name="footer" src="dettagliFirmaButtons.aspx?SIGNED_DOCUMENT_ON_SESSION=<%=Request.QueryString["SIGNED_DOCUMENT_ON_SESSION"]%>" scrolling="no" noresize frameborder="no">
		<noframes>
		</noframes>
	</frameset>
	</form>
</HTML>
