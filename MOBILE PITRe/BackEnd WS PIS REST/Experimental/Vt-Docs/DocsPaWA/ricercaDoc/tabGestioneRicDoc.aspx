<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="tabGestioneRicDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.tabGestioneRicDoc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>DOCSPA > tabGestioneRicDoc</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	</HEAD>
	<body topmargin="1" leftmargin="1">
		<form id="tabGestioneRicDoc" method="post" runat="server">
			<table height="100%" cellSpacing="0" cellPadding="0" width="415">
				<tr height="5%" >
					<td>
						<table id="tbl_tab" cellSpacing="0" cellPadding="0" border="0">
							<tr vAlign="bottom" height="17">
								<td onmouseover="ChangeCursorT('hand','btn_veloce');" style="HEIGHT: 18px" width="67"><asp:imagebutton id="btn_veloce" Height="16px" Runat="server" BorderWidth="0px" Width="67px" ImageUrl="../images/ricerca/veloce_nonattivo.gif"></asp:imagebutton></td>
								<td style="HEIGHT: 18px" width="1" bgColor="#ffffff"></td>
								<td onmouseover="ChangeCursorT('hand','btn_estesa');" style="HEIGHT: 18px" width="67"><asp:imagebutton id="btn_estesa" Height="16px" Runat="server" BorderWidth="0px" Width="67px" ImageUrl="../images/ricerca/estesa_nonattivo.gif"
										BorderStyle="None"></asp:imagebutton></td>
								<td style="HEIGHT: 18px" width="1" bgColor="#ffffff"></td>
								<td onmouseover="ChangeCursorT('hand','btn_completa');" style="HEIGHT: 18px" width="67"><asp:imagebutton id="btn_completa" Height="16px" Runat="server" BorderWidth="0px" Width="67px" ImageUrl="../images/ricerca/completa_nonattivo.gif"></asp:imagebutton></td>
								<td style="HEIGHT: 18px" width="1" bgColor="#ffffff"></td>
								<td onmouseover="ChangeCursorT('hand','btn_completamento');" style="HEIGHT: 18px" width="67"><asp:imagebutton id="btn_completamento" Height="16px" Runat="server" BorderWidth="0px" Width="67px"
										ImageUrl="../images/ricerca/completamento_nonattivo.gif"></asp:imagebutton></td>
								<td style="HEIGHT: 18px" width="1" bgColor="#ffffff" height="18"></td>
								<% if (this.ricercaGrigi.Equals("1")){ %>
								<td onmouseover="ChangeCursorT('hand','btn_Grigia');" style="HEIGHT: 18px" width="67"
									height="18"><asp:imagebutton id="btn_Grigia" runat="server" Height="16px" BorderWidth="0px" Width="67px" ImageUrl="../images/ricerca/grigia_nonattivo.gif"></asp:imagebutton></td>
								<td style="HEIGHT: 18px" width="1" bgColor="#ffffff" height="18"></td>
								<%} %>
								<td onmouseover="ChangeCursorT('hand','btn_StampaReg');" style="HEIGHT: 18px" width="67"
									height="18"><asp:imagebutton id="btn_StampaReg" runat="server" Height="16px" BorderWidth="0px" Width="67px" ImageUrl="../images/ricerca/stampaReg_nonattivo.gif"></asp:imagebutton></td>
                                <td onmouseover="ChangeCursorT('hand','btn_StampaRep');" style="HEIGHT: 18px" width="67"
									height="18"><asp:imagebutton id="btn_StampaRep" runat="server" Height="16px" BorderWidth="0px" Width="67px" ImageUrl="../images/ricerca/stampaRep_nonattivo.gif"></asp:imagebutton></td>
								<td style="HEIGHT: 18px" width="1" bgColor="#ffffff" height="18"></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr vAlign="top" align="center" width="360">
					<td>
					    <cc1:iframewebcontrol id="IframeTabs" runat="server" Align="center" Frameborder="0" Scrolling="auto" Marginwidth="0" Marginheight="0" iHeight="100%" iWidth="415"></cc1:iframewebcontrol>
                    </td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
