<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="gestioneProspetti.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ProspettiRiepilogativi.Frontend.gestioneProspetti" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > gestioneProspetti</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="gestioneProspetti" method="post" runat="server">
			<table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				<tr valign="top" height="100%">
					<td width="425" align="left">
						<cf1:IFrameWebControl id="iFrame_elenco" runat="server" Marginwidth="0" Marginheight="0" iHeight="100%"
							iWidth="100%" Frameborder="0" Scrolling="auto" NavigateTo="ProspettiRiepilogativi_LF.aspx" Align="left"></cf1:IFrameWebControl>
					</td>
					<td width="*">
						<cf1:IFrameWebControl id="iFrame_dettagli" runat="server" Marginwidth="0" Marginheight="0" iHeight="100%"
							iWidth="100%" NavigateTo="ProspettiRiepilogativi_RF.aspx" Frameborder="0" Scrolling="no"></cf1:IFrameWebControl>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
