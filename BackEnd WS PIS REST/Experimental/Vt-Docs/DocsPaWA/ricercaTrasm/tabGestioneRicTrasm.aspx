<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="tabGestioneRicTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaTrasm.tabGestioneRicTrasm" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body leftMargin="1" topMargin="1" MS_POSITIONING="GridLayout">
		<form id="tabGestioneRicTrasm" method="post" runat="server">
		    <uc1:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Ricerca Trasmissioni" />
			<TABLE class="testo_grigio" height="100%" cellSpacing="0" cellPadding="0" width="415" align="center"
				border="0">
				<tr height="5%">
					<td>
						<TABLE class="testo_grigio" cellSpacing="0" cellPadding="0" width="415" border="0">
							<!--
							<tr>
								<td>
									<TABLE class="info" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
										<TR height="30">
											<TD class="testo_grigio_scuro"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ruolo&nbsp;&nbsp;</TD>
											<TD class="testo_grigio" width="100%" height="20"><asp:label id="lbl_ruolo" runat="server"></asp:label></TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							-->
							<tr>
								<td height="2"></td>
							</tr>
							<TR>
								<td vAlign="center" align="middle">
									<asp:radiobuttonlist id="RadioRE" runat="server" Width="238px" AutoPostBack="True" CssClass="testo_grigio" RepeatDirection="Horizontal" Height="5px">
										<asp:ListItem Value="R">Ricevute&nbsp;&nbsp;</asp:ListItem>
										<asp:ListItem Value="E" Selected="True">Effettuate</asp:ListItem>
									</asp:radiobuttonlist>
								</td>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td height="2"></td>
				</tr>
				<tr height="95%">
					<td>
						<table height="100%" cellSpacing="0" cellPadding="0" width="380" border="0">
							<tr height="2%">
								<TD width="59" height="17"><asp:imagebutton id="btn_completa" Height="17px" Width="67px" 
										BorderWidth="0px" Runat="server"></asp:imagebutton></TD>
								<TD width="1" bgColor="#ffffff" height="17"></TD>
								<TD width="59" height="17"><asp:imagebutton id="btn_toDoList" Height="17px" Width="67px" 
										BorderWidth="0px" Runat="server"></asp:imagebutton></TD>
								<td width="240" height="17"></td>
							</tr>
							<tr vAlign="top" align="center" height="98%">
								<td colSpan="4"><cc1:iframewebcontrol id="IframeTabs" runat="server" Align="center" Frameborder="0" Scrolling="auto" Marginwidth="0"
										Marginheight="0" iHeight="100%" iWidth="415"></cc1:iframewebcontrol></td>
							</tr>
						</table>
					</td>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
