<%@ Page language="c#" Codebehind="ricDocStampaReg.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.ricDocStampaReg" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>DOCSPA > ricDocStampaReg</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" id="butt_ricerca_click" event="onclick()" for="butt_ricerca">
			window.document.body.style.cursor='wait';
			
			WndWait();
		</script>
		<script language="javascript">
			// Permette di inserire solamente caratteri numerici
			function ValidateNumericKey()
			{
				var inputKey=event.keyCode;
				var returnCode=true;
				
				if(inputKey > 47 && inputKey < 58)
				{
					return;
				}
				else
				{
					returnCode=false; 
					event.keyCode=0;
				}
				
				event.returnValue = returnCode;
			}
		</script>
	</HEAD>
	<body leftMargin="0" MS_POSITIONING="GridLayout">
		<form id="ricDocStampaReg" method="post" runat="server">
			<TABLE id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="413" align="center"
				border="0">
				<tr vAlign="top">
					<td align="left">
						<table class="contenitore" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td height="15"></td>
							</tr>
							<tr vAlign="top">
								<td>
									<TABLE class="info_grigio" id="tbl_dataCreazione" cellSpacing="0" cellPadding="0" width="97%"
										align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" colSpan="2" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registro</TD>
										</TR>
										<TR>
											<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></TD>
											<TD height="25"><asp:listbox id="lbxRegistro" runat="server" Width="343px" CssClass="testo_grigio"></asp:listbox></TD>
										</TR>
										<tr>
											<TD><IMG height="5" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										</tr>
									</TABLE>
									<br>
									<table class="info_grigio" id="tblNumProtocollo" cellSpacing="0" cellPadding="0" width="97%"
										align="center" border="0" runat="server">
										<TR height="30">
											<TD class="titolo_scheda" width="15%" style="padding-left:5px;"><asp:label id="lblNumProtocollo" runat="server">Num. prot.</asp:label></TD>
											<TD class="titolo_scheda" width="22%"><asp:dropdownlist id="cboFilterTypeNumProtocollo" runat="server" Width="100%" CssClass="testo_grigio"
													AutoPostBack="True"></asp:dropdownlist></TD>
											<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblInitNumProtocollo" runat="server" CssClass="titolo_scheda">Da:</asp:label></TD>
											<TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtInitNumProtocollo" runat="server" Width="98%" CssClass="testo_grigio"></asp:textbox></TD>
											<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblEndNumProtocollo" runat="server" CssClass="titolo_scheda">a:</asp:label></TD>
											<TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtEndNumProtocollo" runat="server" Width="98%" CssClass="testo_grigio"></asp:textbox></TD>
										</TR>
										<TR height="30">
											<TD class="titolo_scheda" width="15%" style="padding-left:5px;"><asp:label id="lblAnnoProtocollo" runat="server">Anno prot.</asp:label></TD>
											<TD class="titolo_scheda" colspan="5"><asp:textbox id="txtAnnoProtocollo" runat="server" Width="112px" CssClass="testo_grigio" MaxLength="4"></asp:textbox></TD>
										</TR>
									</table>
									<br>
									<table class="info_grigio" id="tblStampa" height="30" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0" runat="server">
			<TR height="30">
											<TD class="titolo_scheda" vAlign="middle" height="17" style="padding-left:5px;"><asp:label id="lblDataStampa" runat="server"> Data Stampa</asp:label></TD>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblInitDataStampa" runat="server" CssClass="titolo_scheda">Da:</asp:label></td>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblEndDataStampa" runat="server" CssClass="titolo_scheda">a:</asp:label></td>
										</TR>
										<TR height="30">
											<TD vAlign="middle" width="120" style="padding-left:5px;"><asp:dropdownlist id="cboFilterTypeDataStampa" runat="server" Width="100%" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist></TD>
											<td vAlign="middle" width="100"><uc3:Calendario id="txtInitDataStampa" runat="server" Visible="true" Width="98%" CssClass="testo_grigio" /></td>
											<td valign="middle" width="100"><uc3:Calendario id="txtEndDataStampa" runat="server" Visible="false" Width="98%" CssClass="testo_grigio" /></td>
										</TR>
									</table>
								</td>
							</tr>
						</table>
						<!--fine tabella ricerca--></td>
				</tr>
				<tr>
					<td height="10%">
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<TD><asp:Button ID="butt_ricerca" runat="server" Text="Ricerca" CssClass="pulsante69" ToolTip="Ricerca documenti protocollati" /></TD>
							</TR>
							<!--TR>
								<TD width="100%" bgColor="#810d06"><IMG height="2" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
							</TR--></TABLE>
						<!--FINE BOTTONIERA --></td>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
