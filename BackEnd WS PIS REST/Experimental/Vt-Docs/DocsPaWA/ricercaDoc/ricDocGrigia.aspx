<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Page language="c#" Codebehind="ricDocGrigia.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.ricDocGrigia" %>
<%@ Register TagPrefix="uc1" TagName="Creatore" Src="../UserControls/Creatore.ascx" %>
<%@ Import Namespace="DocsPAWA.ricercaDoc" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register src="../UserControls/RicercaNote.ascx" tagname="RicercaNote" tagprefix="uc2" %>
<%@ Register TagPrefix="aof" TagName="AuthorOwnerFilter" Src="~/UserControls/AuthorOwnerFilter.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>DOCSPA > ricDocGrigia</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" id="enterKeySimulator_click" event="onclick()" for="enterKeySimulator">
			window.document.body.style.cursor='wait';
			
			WndWait();
		</script>
		<script language="javascript" id="btn_ricerca_click" event="onclick()" for="btn_ricerca">
			window.document.body.style.cursor='wait';
			
			WndWait();
		</script>
		<SCRIPT language="javascript">
			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-100)/2;
			var new_h = (h-400)/2;
			
			function apriPopupAnteprima() {
				//window.open('../documento/AnteprimaProfDinRicerche.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
				window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicerche.aspx','','dialogWidth:700px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
			}		
			
		function apriSalvaRicerca()
		{
			 window.showModalDialog('../popup/salvaRicerca.aspx?tipo=D',window.self,'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
		}
		function apriSalvaRicercaADL()
		{
			 window.showModalDialog('../popup/salvaRicerca.aspx?ricADL=1&tipo=D',window.self,'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
		}
			
		function OnChangeSavedFilter()
		{
			var ctl=document.getElementById("ddl_Ric_Salvate");

			if (ctl!=null && ctl.value>0)
				top.principale.iFrame_dx.location='../waitingpage.htm';	
		}

		    //Apre la PopUp Modale per la ricerca dei fascicoli
		    //utile per la fascicolazione rapida
		function ApriRicercaFascicoli(codiceClassifica, NodoMultiReg) 
		{
			var newUrl;
			
			newUrl="../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica+"&caller=profilo&NodoMultiReg="+NodoMultiReg;
			
			var newLeft=(screen.availWidth-615);
			var newTop=(screen.availHeight-704);	
			
			// apertura della ModalDialog
			rtnValue = window.showModalDialog(newUrl,"","dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
			if (rtnValue == "N")
			{
			    window.document.ricDocGrigia.txt_CodFascicolo.value = "";
			    window.document.ricDocGrigia.txt_DescFascicolo.value = "";
			}
			if (rtnValue == "Y")
			{
				window.document.ricDocGrigia.submit();
			}
		}
					
		</SCRIPT>
		<script language="javascript">				
		function _ApriRubrica(target)
		{
			var r = new Rubrica();
		
			switch (target) {
				case "ric_estesa":
					r.CallType = r.CALLTYPE_RICERCA_ESTESA;
					break;								
			}
			var res = r.Apri(); 		
		}	
			</SCRIPT>
	</HEAD>
	<body leftMargin="0" MS_POSITIONING="GridLayout">
		<form id="ricDocGrigia" method="post" runat="server">
			<table id="tbl_contenitore" cellSpacing="0" cellPadding="0" align="center" width="395" border="0" height="100%">
				<tr vAlign="top">
					<td align="center" valign="top">
						<table class="contenitore" cellSpacing="0" cellPadding="0" width="100%" border="0" height="97%">
							<tr>
								<td style="HEIGHT: 2px" height="2"><asp:imagebutton id="enterKeySimulator" runat="server" ImageUrl="../images/spacer.gif"></asp:imagebutton></td>
							</tr>
							<TR>
								<TD vAlign="top">
									<TABLE class="info_grigio" id="Table2" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0" >
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
											<asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label> 
											</TD>
										</TR>
										<TR>
											<TD valign="top"><FONT size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></FONT>
												<asp:dropdownlist id="ddl_Ric_Salvate" runat="server" AutoPostBack="True" CssClass="testo_grigio"
													Width="344px"></asp:dropdownlist></TD>
											<td align="left" valign="top"><asp:imagebutton id="btn_Canc_Ric" ImageUrl="../images/proto/cancella.gif" Width="19px" Height="17px"
													Runat="server" AlternateText="Rimuove la ricerca selezionata"></asp:imagebutton><FONT size="1"></FONT></td>
										</TR>
										<tr><td><IMG height="4" src="../images/proto/spaziatore.gif"  border="0"></td></tr>
							
									</TABLE>
								</TD>
							</TR>
							<tr>
								<td height="2"></td>
							</tr>
							<tr id = "trFiltroTipiDocumento" runat = "server">
								<td valign="top">
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="95%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="19"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />Tipo</td>
											<td>
											    <asp:CheckBoxList ID="cblTipiDocumento" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
											        <asp:ListItem Value="G" Selected="True">Non Protocollati</asp:ListItem>
											        <asp:ListItem Value="ALL" Selected="True">Allegati</asp:ListItem>
											    </asp:CheckBoxList>
										    </td>
										</tr>
									</table>
								</td>
							</tr>	
							<tr>
								<td height="2"></td>
							</tr>													
							<tr vAlign="top">
								<td>
									<!-- DATA CREAZIONE -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="17"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data creazione</TD>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblDAdataCreaz_C" runat="server" CssClass="testo_grigio" Width="10px" Visible="False">Da</asp:label></td>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblAdataCreaz_C" runat="server" CssClass="testo_grigio" Width="10px" Visible="False">A</asp:label></td>
										</TR>
										<TR height="20">
											<TD vAlign="middle" width="120"><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
												<asp:dropdownlist id="ddl_dataCreazione_C" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="true">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
									                <asp:ListItem Value="2">Oggi</asp:ListItem>
								                    <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
								                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></TD>
											<TD vAlign="top" width="100"><uc3:Calendario id="txt_initDataCreaz_C" runat="server" Visible="true" /></TD>
											<TD vAlign="top" width="100"><uc3:Calendario id="txt_fineDataCreaz_C" runat="server" Visible="true" /></td>
										</TR>
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="17"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Id documento</TD>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblDAidDoc_C" runat="server" CssClass="testo_grigio" Width="18px" Visible="False">Da</asp:label></td>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblAidDoc_C" runat="server" CssClass="testo_grigio" Width="12px" Visible="False">A</asp:label></td>
										</TR>
										<TR height="20">
											<TD vAlign="middle" width="140"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:dropdownlist id="ddl_idDocumento_C" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="true">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
												</asp:dropdownlist></TD>
											<TD vAlign="top" width="100"><asp:textbox id="txt_initIdDoc_C" runat="server" CssClass="testo_grigio" Width="80px"></asp:textbox></TD>
											<TD vAlign="top" width="100"><asp:textbox id="txt_fineIdDoc_C" runat="server" CssClass="testo_grigio" Width="80px" Visible="False"></asp:textbox></td>
										</TR>
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="17"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Scadenza</TD>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lbl_da_dtaScadenza_G" runat="server" CssClass="testo_grigio" Width="10px" Visible="False">Da</asp:label></td>
											<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lbl_a_dtaScadenza_G" runat="server" CssClass="testo_grigio" Width="10px" Visible="False">A</asp:label></td>
										</TR>
										<TR height="20">
											<TD vAlign="middle" width="120"><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
												<asp:dropdownlist id="ddl_dataScadenza_G" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="true">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
									                <asp:ListItem Value="2">Oggi</asp:ListItem>
								                    <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
								                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></TD>
											<td vAlign="top" width="100"><uc3:Calendario id="txt_initScadenza_G" runat="server" Visible="true" /></td>
											<td valign="top" width="100"><uc3:Calendario id="txt_fineScadenza_G" runat="server" Visible="false" /></td>
										</TR>
										<tr><td colspan="3"><IMG height="2" src="../images/proto/spaziatore.gif" width="5" border="0"></td></tr>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr vAlign="top">
								<td>
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="top" height="12"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Documenti<br />
											<IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">privi di</TD>
											<TD>
												<table>
													<tr>
														<td><asp:radiobuttonlist id="rbl_documentiInCompletamento" runat="server" CssClass="testo_grigio" RepeatColumns="3"
																RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0">
																<asp:ListItem Value="M_Asg">Assegnatario&nbsp;</asp:ListItem>
																<asp:ListItem Value="M_Img">Immagine&nbsp;</asp:ListItem>
																<asp:ListItem Value="M_Fasc">Fascicolazione</asp:ListItem>
															</asp:radiobuttonlist></td>
													</tr>
												</table>
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
									<!--tabella oggetto -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Oggetto</TD>
											<TD vAlign="middle" align="right" width="29"><asp:imagebutton id="btn_oggettario" ImageUrl="../images/proto/ico_oggettario.gif" Width="19px" Runat="server"
													Height="17px" AlternateText="Seleziona un oggetto nell'oggettario"></asp:imagebutton></TD>
										</TR>
										<TR>
											<TD colSpan="2" height="37"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:textbox id="txt_oggetto" runat="server" CssClass="testo_grigio" Width="350px" Height="30px"
													TextMode="MultiLine"></asp:textbox></TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr align="center">
							    <td><aof:AuthorOwnerFilter ID="aofAuthor" runat="server" ControlType="Author" /></td>
                            </tr>
                            <tr>
								<td height="2"></td>
							</tr>
                            <tr align="center">
                                <td><aof:AuthorOwnerFilter ID="aofOwner" runat="server" ControlType="Owner" /></td>
                            </tr>
							<tr><td height="2"></td></tr>
							<tr>
								<td>
									<!-- INIZIO TABELLA PAROLE CHIAVE -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Parole 
												chiave</TD>
											<TD vAlign="middle" align="right" width="29"><asp:imagebutton id="btn_selezionaParoleChiave" ImageUrl="../images/proto/ico_parole.gif" Width="19px"
													Runat="server" Height="17px" AlternateText="Seleziona parole chiave"></asp:imagebutton></TD>
										</TR>
										<TR>
											<td colSpan="2" height="40"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"><asp:listbox id="ListParoleChiave" runat="server" CssClass="testo_grigio" Width="350px" Height="34px"
													Rows="2"></asp:listbox></td>
										</TR>
									</TABLE>
								</td>
							</tr>
							<asp:panel id="panel_numOgg_commRef" runat="server" Visible="False">
								<TR>
									<TD>
										<TABLE cellSpacing="0" cellPadding="0" width="95%" align="center">
											<TR>
												<TD>
													<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center"
														border="0">
														<TR>
															<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Numero 
																oggetto</TD>
														</TR>
														<TR>
															<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
																<asp:textbox id="txt_numOggetto" runat="server" Width="332px" CssClass="testo_grigio"></asp:textbox></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD height="2"></TD>
											</TR>
											<TR>
												<TD align="center">
													<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center"
														border="0">
														<TR>
															<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Commissione 
																referente</TD>
														</TR>
														<TR>
															<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
																<asp:textbox id="txt_commRef" runat="server" Width="332px" CssClass="testo_grigio"></asp:textbox></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
							</asp:panel>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
									<!-- INIZIO TABELLA TIPO_ATTO -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px" vAlign="middle"
												width="35%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipologia 
												documento</TD>
											<td style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><asp:dropdownlist id="ddl_tipoAtto" runat="server" CssClass="testo_grigio" Width="200px" AutoPostBack="True"
													Height="25px"></asp:dropdownlist></td>
											<td style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><asp:imagebutton id="btn_CampiPersonalizzati" runat="server" ImageUrl="../images/proto/ico_oggettario.gif"></asp:imagebutton></td>
											<td style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>
										</TR>
										<asp:panel id="Panel_StatiDocumento" Runat="server" Visible="false">
											<TR>
												<TD class="titolo_scheda" style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px" vAlign="middle"
													height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Stato 
													documento</TD>
												<TD style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px" colSpan="2">
													<asp:DropDownList id="ddl_statiDoc" runat="server" AutoPostBack="True" Width="230px" CssClass="testo_grigio"></asp:DropDownList></TD>
												<TD style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></TD>
											</TR>
										</asp:panel></TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
									<!-- INIZIO RICERCA PER CODICE FASCICOLO -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Codice 
												fascicolo Generale/Procedimentale</TD>
											<TD vAlign="middle" align="right">
											    <cc1:imagebutton class="ImgHand" id="imgFasc" Runat="server" AlternateText="Ricerca Fascicoli" DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/proto/ico_fascicolo_noattivo.gif"></cc1:imagebutton>
                                                &nbsp;&nbsp;
											</TD>
				                        </TR>
										<TR>
											<TD height="25" colspan="2"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												<asp:textbox id="txt_CodFascicolo" CssClass="testo_grigio" Width="75px" AutoPostBack="True" Runat="server"
													ReadOnly="False"></asp:textbox>&nbsp;&nbsp;
												<asp:textbox id="txt_DescFascicolo" CssClass="testo_grigio" Width="270px" Runat="server" ReadOnly="True"></asp:textbox>
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
									<!--tabella NOTE -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Note</TD>
										</TR>
										<TR>
											<TD height="35"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												<uc2:RicercaNote ID="rn_note" runat="server" TextMode="SingleLine" 
                                                                TextBoxHeight="" TextBoxWidth="320" DdlWidth="320"  />
										</TR>
									</TABLE>
								</td>
							</tr>
                            <!-- Ordinamento -->
                            <div>
                                <tr>
                                    <td height="3" />
                                </tr>
							    <tr>
								    <td>
									    <table class="info_grigio" cellspacing="0" cellpadding="0" width="95%" align="center" border="0">
										    <tr>
											    <td class="titolo_scheda" valign="middle" height="19" width="35%" style="padding-bottom: 5px; padding-top: 5px">
                                                    &nbsp;Ordinamento
                                                </td>
                                            </tr>
                                            <tr>
										        <td style="padding-bottom: 5px; padding-top: 5px" class="style1">
                                                    &nbsp;<asp:DropDownList ID="ddlOrder" runat="server" CssClass="testo_grigio" />
                                                    <asp:DropDownList ID="ddlOrderDirection" runat="server" CssClass="testo_grigio">
                                                        <asp:ListItem Text="Crescente" Value="ASC" />
                                                        <asp:ListItem Text="Decrescente" Value="DESC" Selected="True" />
                                                    </asp:DropDownList>
                                                </td>
									        </tr>
                                        </table>
								    </td>
							    </tr>
							</div>	
							<tr>
								<td height="2"></td>
							</tr>
							<!--
							<tr>
								<td height="55">
									tabella firmatari
									<TABLE width="95%" align="center" class="info_grigio" cellSpacing="0" cellPadding="0" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Firmatario</TD>
										</TR>
										<TR>
											<TD height="25">
												<table cellSpacing="0" cellPadding="0" width="100%" border="0">
													<tr>
														<td><asp:label id="lbl_nomeF" CssClass="testo_grigio" Runat="server"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Nome</asp:label></td>
														<td><asp:textbox id="txt_nomeFirma" CssClass="testo_grigio" Width="120px" Runat="server"></asp:textbox>&nbsp;
														</td>
														<td><asp:label id="lbl_cognomeF" CssClass="testo_grigio" Runat="server">Cognome</asp:label></td>
														<td><asp:textbox id="txt_cognomeFirma" CssClass="testo_grigio" Width="120px" Runat="server"></asp:textbox></td>
													</tr>
												</table>
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							-->
							<!--tr>
								<td height="3">&nbsp;</td>
							</tr--></table>
					</td>
				</tr>
				<tr>
					<td height="10%">
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<TD>
								    <asp:Button ID="btn_ricerca" Text="Ricerca" runat="server" CssClass="pulsante69" ToolTip="Ricerca documenti grigi" />
								    <asp:Button ID="btn_salva" Text="Salva" runat="server" CssClass="pulsante69" ToolTip="Salva i criteri di ricerca" /></TD>
							</TR>
							<!--TR>
								<TD width="100%" bgColor="#810d06"><IMG height="2" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
							</TR--></TABLE>
						<!--FINE BOTTONIERA --></td>
				</tr>
			</table>
			<cc2:messagebox id="mb_ConfirmDelete" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 952px"
				runat="server"></cc2:messagebox></form>
	</body>
</HTML>
