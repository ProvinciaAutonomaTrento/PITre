<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="docProtocolloSemplice.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.docProtocolloSemplice" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa.css" type="text/css" rel="stylesheet">
		<script language="javascript" id="btn_protocolla_P_click" for="btn_protocolla_P" event="onclick()">
			DocsPa_FuncJS_WaitWindows();
		</script>
		<script language="javascript" id="btn_protocollaGiallo_P_click" for="btn_protocollaGiallo_P" event="onclick()">
			DocsPa_FuncJS_WaitWindows();
		</script>
		<script language="javascript">
<!--
function info()
{	
	try
	{
		alert(eval(document.docProtocolloSemplice.txtTest.value));
	}
	catch(e)
	{
		alert(e);
	}
}
//-->
		</script>
	</HEAD>
	<body leftMargin="0" topMargin="0" MS_POSITIONING="GridLayout">
		<form id="docProtocolloSemplice" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Protocollo Semplice" />
			<!--
			<input  type=text id=txtTest>
			<input type=button id=btnTest value=Test onclick="info()">
		-->
			<table id="tbl_contenitore" cellSpacing="0" cellPadding="0" height="100%" width="452" align="center" border="0">
				<TBODY>
					<tr vAlign="top">
						<td vAlign="top" width="450" height="5%">
							<table class="testo_bianco" height="17" cellSpacing="0" cellPadding="0" width="450" bgColor="#810d06" border="0">
								<tr vAlign="top" align="middle" height="5">
									<td class="testo_bianco" vAlign="top" width="100%" height="5"><asp:radiobuttonlist id="rbl_InOut_P" ForeColor="Transparent" AutoPostBack="true" RepeatDirection="Horizontal" CssClass="testo_grigio_large" Runat="server" Height="5px">
											<asp:ListItem Value="In" Selected="True">
												&lt;font color=&quot;#ffffff&quot;&gt;Ingresso&lt;/font&gt;</asp:ListItem>
											<asp:ListItem Value="Out">
												&lt;font color=&quot;#ffffff&quot;&gt;Uscita&lt;/font&gt;</asp:ListItem>
										</asp:radiobuttonlist></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr vAlign="top">
						<td vAlign="top" width="410" height="85%">
							<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
								<TBODY>
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2">
											<TABLE cellSpacing="0" cellPadding="0" border="0">
												<TBODY>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD vAlign="center" height="5">
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TBODY>
																	<TR>
																		<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
																	</TR>
																</TBODY>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD>
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TBODY>
																	<tr>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																		<TD vAlign="center" align="middle" width="29"><cc1:imagebutton id="btn_stampaSegn_P" Runat="server" Height="17" AlternateText="Stampa" ImageUrl="../images/proto/stampa.gif" Width="18" Tipologia="DO_PROT_SE_STAMPA" DisabledUrl="../images/proto/stampa.gif"></cc1:imagebutton>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	</tr>
																</TBODY>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
												</TBODY>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="50%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">segnatura&nbsp;</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_dataSegn" CssClass="testo_grigio_large" Runat="server" width="90px" ReadOnly="True"></asp:textbox><asp:textbox id="lbl_segnatura" CssClass="testo_grigio_large" Runat="server" width="270px" ReadOnly="True"></asp:textbox></TD>
									</TR>
								</TBODY>
							</TABLE>
							<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
								<TBODY>
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2">
											<TABLE id="tbl_ogget" cellSpacing="0" cellPadding="0" align="right" border="0">
												<TBODY>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD height="5">
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TBODY>
																	<TR>
																		<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
																	</TR>
																</TBODY>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD>
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TBODY>
																	<tr>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																		<TD vAlign="center" align="middle" width="29"><cc1:imagebutton id="btn_RubrOgget_P" Runat="server" Height="17px" AlternateText="Seleziona un  oggetto nell'oggettario" ImageUrl="../images/proto/ico_oggettario.gif" Width="19px" Tipologia="DO_PROT_OG_OGGETTARIO" DisabledUrl="../images/proto/ico_oggettario.gif"></cc1:imagebutton></TD>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																		<TD vAlign="center" align="middle" width="29"><cc1:imagebutton id="btn_modificaOgget_P" Runat="server" AlternateText="Modifica" ImageUrl="../images/proto/matita.gif" Width="18" Tipologia="DO_PROT_OG_MODIFICA" DisabledUrl="../images/proto/matita.gif"></cc1:imagebutton></TD>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																		<TD vAlign="center" align="middle" width="29"><cc1:imagebutton id="btn_storiaOgg_P" Runat="server" Height="17" AlternateText="Storia" ImageUrl="../images/proto/storia.gif" Width="18" Tipologia="DO_PROT_OG_STORIA" DisabledUrl="../images/proto/storia.gif" Enabled="False"></cc1:imagebutton></TD>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	</tr>
																</TBODY>
															</TABLE>
														</TD>
														<TD vAlign="top" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
												</TBODY>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="50%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Oggetto&nbsp;</TD>
									</TR>
									<TR>
										<TD style="WIDTH: 410px" bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_oggetto_P" CssClass="testo_grigio_large" Runat="server" Height="40px" Width="350px" TextMode="MultiLine"></asp:textbox></TD>
									</TR>
								</TBODY>
							</TABLE>
							<asp:panel id="panel_Mit" runat="server" Visible="True"> <!-- mittente -->
								<TABLE id="tblMitt" cellSpacing="0" cellPadding="0" width="450" Runat="server">
									<TR id="tr2">
										<TD class="menu_1_grigio_large" id="tc" colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD class="menu_1_grigio_large" id="tc1" width="77%"></TD>
									</TR>
								</TABLE>
								<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2"><TABLE cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD vAlign="center" height="5">
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD>
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_RubrMit_P" Height="19" Runat="server" DisabledUrl="../images/proto/rubrica.gif" Tipologia="DO_IN_MIT_RUBRICA" Width="29" ImageUrl="../images/proto/rubrica.gif" AlternateText="Seleziona mittente nella rubrica"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_DetMit_P" Height="16" Runat="server" DisabledUrl="../images/proto/dettaglio.gif" Tipologia="DO_IN_MIT_DETTAGLI" Width="18" ImageUrl="../images/proto/dettaglio.gif" AlternateText="Dettagli"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_ModMit_P" Runat="server" DisabledUrl="../images/proto/matita.gif" Tipologia="DO_IN_MIT_MODIFICA" Width="18" ImageUrl="../images/proto/matita.gif" AlternateText="Modifica"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_storiaMit_P" Height="17" Runat="server" DisabledUrl="../images/proto/storia.gif" Tipologia="DO_IN_MIT_STORIA" Width="18" ImageUrl="../images/proto/doc.gif" AlternateText="Storia" Enabled="False"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="50%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Mittente&nbsp;</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_CodMit_P" Runat="server" CssClass="testo_grigio_large" AutoPostBack="True" Width="75px"></asp:textbox>
											<asp:textbox id="txt_DescMit_P" Runat="server" CssClass="testo_grigio_large" Width="270px"></asp:textbox></TD>
									</TR>
								</TABLE>
								<asp:Panel id="panel_mitInt" Runat="server" Visible="false"> <!-- mittente intermedio -->
									<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
										<TR>
											<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
											<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
											<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2"><TABLE cellSpacing="0" cellPadding="0" border="0">
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD vAlign="center" height="5">
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TR>
																	<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
																</TR>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD>
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TR>
																	<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	<TD vAlign="center" align="middle" width="29">
																		<cc1:imagebutton id="btn_RubrMitInt_P" Height="19" Runat="server" DisabledUrl="../images/proto/rubrica.gif" Tipologia="DO_IN_MII_RUBRICA" Width="29" ImageUrl="../images/proto/rubrica.gif" AlternateText="Seleziona mittente intermedio nella rubrica"></cc1:imagebutton></TD>
																	<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	<TD vAlign="center" align="middle" width="29">
																		<cc1:imagebutton id="btn_DetMitInt_P" Height="16" Runat="server" DisabledUrl="../images/proto/dettaglio.gif" Tipologia="DO_IN_MII_DETTAGLI" Width="18" ImageUrl="../images/proto/dettaglio.gif" AlternateText="Dettagli"></cc1:imagebutton></TD>
																	<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	<TD vAlign="center" align="middle" width="29">
																		<cc1:imagebutton id="btn_ModMitInt_P" Runat="server" DisabledUrl="../images/proto/matita.gif" Tipologia="DO_IN_MII_MODIFICA" Width="18" ImageUrl="../images/proto/matita.gif" AlternateText="Modifica"></cc1:imagebutton></TD>
																	<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	<TD vAlign="center" align="middle" width="29">
																		<cc1:imagebutton id="btn_storiaMitInt_P" Height="17" Runat="server" DisabledUrl="../images/proto/storia.gif" Tipologia="DO_IN_MII_STORIA" Width="18" ImageUrl="../images/proto/doc.gif" AlternateText="Storia" Enabled="False"></cc1:imagebutton></TD>
																	<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																</TR>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
										<TR>
											<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
											<TD class="menu_1_grigio_large" vAlign="center" width="50%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Mittente 
												intermedio&nbsp;</TD>
										</TR>
										<TR>
											<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
												<asp:textbox id="txt_CodMitInt_P" Runat="server" CssClass="testo_grigio_large" AutoPostBack="True" Width="75px"></asp:textbox>
												<asp:textbox id="txt_DescMitInt_P" Runat="server" CssClass="testo_grigio_large" Width="270px"></asp:textbox></TD>
										</TR>
									</TABLE>
								</asp:Panel> <!-- protocollo mittente -->
								<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2"><TABLE cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD vAlign="center" height="5">
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD>
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_verificaPrec_P" Height="16" Runat="server" DisabledUrl="../images/proto/ico_risposta.gif" Tipologia="DO_IN_PRO_VERIFICAPREC" Width="18" ImageUrl="../images/proto/ico_risposta.gif" AlternateText="Verifica Prec"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="50%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Protocollo 
											Mittente&nbsp;</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_DataProtMit_P" Runat="server" CssClass="testo_grigio_large" Width="75px"></asp:textbox>
											<asp:textbox id="txt_NumProtMit_P" Runat="server" CssClass="testo_grigio_large" Width="270px"></asp:textbox></TD>
									</TR>
								</TABLE>
							</asp:panel>
							<!--	data arrivo-->
							<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
								<TR>
									<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
								</TR>
								<TR>
									<TD></TD>
									<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
									<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2">
										<TABLE cellSpacing="0" cellPadding="0" border="0">
											<TR>
												<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												<TD vAlign="center" height="5">
													<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
														</TR>
													</TABLE>
												</TD>
												<TD vAlign="top" width="1" height="5" style="WIDTH: 1px"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
											</TR>
											<TR>
												<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												<TD>
													<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"></TD>
															<TD vAlign="center" align="middle" width="29"></TD>
															<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"></TD>
														</TR>
													</TABLE>
												</TD>
												<TD vAlign="top" width="1" height="20" style="WIDTH: 1px"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
								<TR>
									<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
									<TD class="menu_1_grigio_large" vAlign="center" width="70%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">data 
										arrivo&nbsp;</TD>
								</TR>
								<TR>
									<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
										<asp:textbox id="txt_DataArrivo_P" Runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="75px"></asp:textbox></TD>
								</TR>
							</TABLE>
							<!-- destinatario -->
							<asp:panel id="panel_Dest" runat="server" Visible="True" Width="414px">
								<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="450" Runat="server">
									<TR>
										<TD class="menu_1_grigio_large" colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD class="menu_1_grigio_large" width="77%"></TD>
									</TR>
								</TABLE>
								<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="450" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="450" bgColor="#d9d9d9" rowSpan="2">
											<TABLE id="tbl_dest" cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD vAlign="center" height="5">
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD>
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_RubrDest_P" Height="19" Runat="server" DisabledUrl="../images/proto/rubrica.gif" Tipologia="DO_OUT_DES_RUBRICA" Width="29" ImageUrl="../images/proto/rubrica.gif" AlternateText="Seleziona un destinatario nella rubrica"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_ModDest_P" Runat="server" DisabledUrl="../images/proto/matita.gif" Tipologia="DO_OUT_DES_MODIFICA" Width="18" ImageUrl="../images/proto/matita.gif" AlternateText="Modifica"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_aggiungiDest_P" Height="17" Runat="server" DisabledUrl="../images/proto/aggiungi.gif" Tipologia="DO_OUT_DES_STORIA" Width="18" ImageUrl="../images/proto/aggiungi.gif" AlternateText="Aggiungi"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="410" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Destinatario&nbsp;</TD>
									</TR>
									<TR>
										<TD width="450" bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_CodDest_P" Runat="server" CssClass="testo_grigio_large" AutoPostBack="True" Width="75px"></asp:textbox>
											<asp:textbox id="txt_DescDest_P" Runat="server" CssClass="testo_grigio_large" Width="270px"></asp:textbox></TD>
									</TR>
								</TABLE> <!-- listBox Destinatari-->
								<TABLE id="tbl_dest9" cellSpacing="0" cellPadding="0" width="450" border="0">
									<TR height="19">
										<TD style="WIDTH: 54px" width="54" bgColor="#d9d9d9" height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" bgColor="#d9d9d9" colSpan="2" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Destinatari</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" width="410" bgColor="#d9d9d9">
											<asp:ListBox id="lbx_dest" runat="server" CssClass="testo_grigio_large" Width="350px" Rows="3"></asp:ListBox></TD>
										<TD vAlign="top" align="right" width="410" bgColor="#d9d9d9">
											<TABLE cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD vAlign="center" height="5">
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD align="middle" width="60">
														<TABLE id="Tbl_Dest1" cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29" height="20">
																	<cc1:imagebutton id="btn_dettDest" Height="16px" Runat="server" DisabledUrl="../images/proto/zoom.gif" Tipologia="DO_OUT_DES_DETTAGLI" Width="18px" ImageUrl="../images/proto/dettaglio.gif" AlternateText="Dettagli"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD vAlign="center" align="middle" width="29" height="20">
																	<cc1:imagebutton id="btn_cancDest" Height="16px" Runat="server" DisabledUrl="../images/proto/cancella.gif" Tipologia="DO_OUT_DES_ELIMINA" Width="18" ImageUrl="../images/proto/cancella.gif" AlternateText="Cancella"></cc1:imagebutton></TD>
																<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD class="testo_grigio_large" align="middle" width="410" bgColor="#d9d9d9" colSpan="2">
											<cc1:ImageButton id="btn_insDestCC" runat="server" DisabledUrl="../images/proto/ico_freccia_giu.gif" ImageUrl="../images/proto/ico_freccia_giu.gif" AlternateText="Inserisci tra i destinatari per Conoscenza"></cc1:ImageButton>&nbsp;
											<cc1:ImageButton id="btn_insDest" runat="server" DisabledUrl="../images/proto/ico_freccia_su.gif" ImageUrl="../images/proto/ico_freccia_su.gif" AlternateText="Inserisci tra i destinatari"></cc1:ImageButton></TD>
										<TD width="410" bgColor="#d9d9d9" colSpan="2"></TD>
									</TR>
									<TR height="19">
										<TD style="WIDTH: 54px" width="54" bgColor="#d9d9d9" height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" bgColor="#d9d9d9" colSpan="2" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Destinatari 
											CC</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" width="410" bgColor="#d9d9d9">
											<asp:ListBox id="lbx_destCC" runat="server" CssClass="testo_grigio_large" width="350px" Rows="3"></asp:ListBox></TD>
										<TD vAlign="top" align="right" width="410" bgColor="#d9d9d9">
											<TABLE id="tbl_cc" cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD vAlign="center" height="5">
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD>
														<TABLE id="Tbl_dest2" style="WIDTH: 62px; HEIGHT: 18px" cellSpacing="0" cellPadding="0" border="0">
															<TR>
																<TD style="HEIGHT: 20px" vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD style="HEIGHT: 20px" vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_dettDestCC" Height="16px" Runat="server" DisabledUrl="../images/proto/dettaglio.gif" Tipologia="DO_OUT_DES_DETTAGLI" Width="18px" ImageUrl="../images/proto/dettaglio.gif" AlternateText="Dettagli"></cc1:imagebutton></TD>
																<TD style="HEIGHT: 20px" vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																<TD style="HEIGHT: 20px" vAlign="center" align="middle" width="29">
																	<cc1:imagebutton id="btn_cancDestCC" Height="16px" Runat="server" DisabledUrl="../images/proto/cancella.gif" Tipologia="DO_OUT_DES_ELIMINA" Width="18" ImageUrl="../images/proto/cancella.gif" AlternateText="Cancella"></cc1:imagebutton></TD>
																<TD style="HEIGHT: 20px" vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
								</TABLE>
							</asp:panel>
							<!-- risposta al protocollo -->
							<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
								<TBODY>
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2">
											<TABLE id="tbl_icona" cellSpacing="0" cellPadding="0" border="0">
												<TBODY>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD vAlign="center" height="5">
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TBODY>
																	<TR>
																		<TD background="../images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
																	</TR>
																</TBODY>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
													<TR>
														<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
														<TD>
															<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																<TBODY>
																	<tr>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																		<TD vAlign="center" align="middle" width="29"><cc1:imagebutton id="btn_adl_P" Runat="server" AlternateText="Seleziona documenti protocollati da 'Area di lavoro'" ImageUrl="../images/proto/area_new.gif" Width="18px" Height="17" Tipologia="DO_GET_ADL" DisabledUrl="../images/proto/doc.gif"></cc1:imagebutton></TD>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																		<TD vAlign="center" align="middle" width="29"><cc1:imagebutton id="btn_catenaProt_P" Runat="server" Height="17px" AlternateText="Catena" ImageUrl="../images/proto/catena.gif" Width="19px" Tipologia="DO_IN_RIS_CATENA" DisabledUrl="../images/proto/catena.gif"></cc1:imagebutton></TD>
																		<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="20"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
																	</tr>
																</TBODY>
															</TABLE>
														</TD>
														<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													</TR>
												</TBODY>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="50%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Risposta 
											al protocollo&nbsp;</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_RispProtSegn_P" CssClass="testo_grigio_large" Runat="server" Width="350px" ReadOnly="True"></asp:textbox></TD>
									</TR>
								</TBODY>
							</TABLE>
							<!-- Annullamento --><asp:panel id="panel_Annul" runat="server" Visible="False">
								<TABLE cellSpacing="0" cellPadding="0" width="450" border="0">
									<TR>
										<TD colSpan="4" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD></TD>
										<TD height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9" height="25" rowSpan="2"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD vAlign="bottom" align="right" width="100%" bgColor="#d9d9d9" rowSpan="2"><TABLE cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD vAlign="center" height="5">
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
																<TD background="../images/proto/spaziatore.gif"><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0"></TD>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD vAlign="top" width="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
													<TD>
														<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
															<TR>
															</TR>
														</TABLE>
													</TD>
													<TD vAlign="top" width="3" height="20"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" align="left" width="6" bgColor="#d9d9d9"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
										<TD class="menu_1_grigio_large" vAlign="center" width="45%" bgColor="#d9d9d9" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">Annullamento&nbsp;</TD>
									</TR>
									<TR>
										<TD bgColor="#d9d9d9" colSpan="4" height="25">&nbsp;
											<asp:textbox id="txt_dataAnnul_P" Runat="server" CssClass="testo_grigio_large" Width="75px" ReadOnly="True"></asp:textbox>
											<asp:textbox id="txt_numAnnul_P" Runat="server" CssClass="testo_grigio_large" Width="270px" ReadOnly="true"></asp:textbox></TD>
									</TR>
								</TABLE>
							</asp:panel>
						</td>
					</tr>
					<tr valign="baseline">
						<td width="450" height="5%">
							<!-- BOTTONIERA -->
							<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" width="450" border="0">
								<TBODY>
									<TR>
										<TD vAlign="top" colSpan="14" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
									</TR>
									<TR>
										<TD colSpan="2"></TD>
										<TD vAlign="center" rowSpan="2">
											<TABLE cellSpacing="0" cellPadding="0" align="center" border="0">
												<TR>
													<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" height="18" rowSpan="2"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
													<TD class="menu_2_rosso" style="HEIGHT: 19px" vAlign="center" align="middle" width="59">salva</TD>
													<TD style="HEIGHT: 19px" vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
													<TD class="menu_2_rosso" style="WIDTH: 59px; HEIGHT: 19px" vAlign="center" align="middle" width="59">prot.lla</TD>
													<TD style="HEIGHT: 19px" vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
													<TD class="menu_2_rosso" style="HEIGHT: 19px" vAlign="center" align="middle" width="59"><asp:label id="lbl_ADL_SPED" runat="server" Width="38px">adl</asp:label></TD>
													<TD style="HEIGHT: 19px" vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
													<TD class="menu_2_rosso" vAlign="center" align="middle" width="59">Riproponi</TD>
													<TD style="WIDTH: 4px; HEIGHT: 19px" vAlign="top" width="4" background="../images/proto/tratteggio_gri_v.gif"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
													<TD class="menu_2_rosso" style="HEIGHT: 19px" vAlign="center" align="middle" width="59">Annulla</TD>
													<TD vAlign="top" width="1" background="../images/proto/tratteggio_gri_v.gif" rowSpan="2"><IMG height="6" src="../images/proto/tratteggio_gri_v.gif" width="1" border="0"></TD>
												</TR>
												<TR>
													<TD><cc1:imagebutton id="btn_salva_P" Runat="server" Height="30" AlternateText="Salva" ImageUrl="../images/bottoniera/salva_attivo.gif" Width="59" Tipologia="DO_PROT_SALVA" DisabledUrl="../images/bottoniera/salva_nonattivo.gif"></cc1:imagebutton></TD>
													<TD></TD>
													<TD><cc1:imagebutton id="btn_protocolla_P" ondblclick="return false;" Runat="server" Height="30" AlternateText="Protocolla" ImageUrl="../images/bottoniera/protocollo_attivo.gif" Width="59" Tipologia="DO_PROT_PROTOCOLLA" DisabledUrl="../images/bottoniera/protocollo_nonattivo.gif" ImageAlign="Top"></cc1:imagebutton><cc1:imagebutton ondblclick="return false;" id="btn_protocollaGiallo_P" Runat="server" Height="30" AlternateText="Protocolla in Giallo" ImageUrl="../images/bottoniera/protocollo_attivo.gif" Width="59" Tipologia="DO_PROT_PROTOCOLLAG" DisabledUrl="../images/bottoniera/protocollo_nonattivo.gif" Visible="False"></cc1:imagebutton></TD>
													<TD></TD>
													<TD><cc1:imagebutton id="btn_aggiungi_P" Runat="server" Height="30" AlternateText="Aggiungi ad Area di lavoro" ImageUrl="../images/bottoniera/area_attivo.gif" Width="59" Tipologia="DO_ADD_ADL" DisabledUrl="../images/bottoniera/area_nonattivo.gif"></cc1:imagebutton><cc1:imagebutton id="btn_spedisci_P" Runat="server" Height="30" AlternateText="Spedisci" ImageUrl="../images/bottoniera/scrivi_attivo.gif" Width="59" Tipologia="DO_OUT_SPEDISCI" DisabledUrl="../images/bottoniera/scrivi_nonattivo.gif" Visible="False"></cc1:imagebutton></TD>
													<TD></TD>
													<TD><cc1:imagebutton id="btn_riproponiDati_P" Runat="server" Height="30" AlternateText="Riproponi dati" ImageUrl="../images/bottoniera/protocolla_predisponi_attivo.gif" Width="59" Tipologia="DO_PROT_RIPROPONI" DisabledUrl="../images/bottoniera/protocolla_predisponi_nonattivo.gif"></cc1:imagebutton></TD>
													<TD></TD>
													<TD><cc1:imagebutton id="btn_annulla_P" Runat="server" Height="30" AlternateText="Annulla" ImageUrl="../images/bottoniera/protocolla_annulla_attivo.gif" Width="59" Tipologia="DO_PROT_ANNULLA" DisabledUrl="../images/bottoniera/protocolla_annulla_nonattiv.gif"></cc1:imagebutton></TD>
												</TR>
											</TABLE>
										</TD>
										<TD style="HEIGHT: 19px" colSpan="2"></TD>
									</TR>
									<TR>
										<td width="57" height="31"><IMG height="31" src="../images/proto/tratto_grigio.gif" width="57" border="0"></td>
										<TD width="17"><IMG style="WIDTH: 17px; HEIGHT: 31px" height="31" src="../images/proto/angolo_piede_sx.gif" width="17" border="0"></TD>
										<TD align="right" width="17"><IMG height="31" src="../images/proto/angolo_piede_dx.gif" width="17" border="0"></TD>
										<td width="58" height="31"><IMG height="31" src="../images/proto/tratto_grigio.gif" width="58" border="0"></td>
									</TR>
									<TR>
										<TD width="100%" bgColor="#9e9e9e" colSpan="14"><IMG height="5" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
									</TR>
								</TBODY>
							</TABLE> <!--FINE BOTTONIERA --></td>
					</tr>
				</TBODY>
			</table>
			<asp:Panel ID="activeX" Visible="False" Runat="server">
				<TABLE>
					<TR>
						<TD>
							<OBJECT id="ctrlPrintPen" codeBase="../activex/DocsPa_PrintPen.CAB#version=1,0,0,0" height="0" width="0" classid="CLSID:6C876809-0AF6-4964-8239-73E86A7F9B11" VIEWASTEXT>
								<PARAM NAME="_ExtentX" VALUE="26">
								<PARAM NAME="_ExtentY" VALUE="26">
								<PARAM NAME="PortaCOM" VALUE="1">
								<PARAM NAME="Text" VALUE="DFPrintPen Test">
								<PARAM NAME="NumCopie" VALUE="1">
								<PARAM NAME="TimeOut" VALUE="60">
							</OBJECT>
						</TD>
					</TR>
				</TABLE>
			</asp:Panel>
		</form>
	</body>
</HTML>
