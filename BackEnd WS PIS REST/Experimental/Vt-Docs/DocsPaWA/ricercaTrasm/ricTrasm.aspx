<%@ Page language="c#" Codebehind="ricTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaTrasm.ricTrasmCompleta" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<script language="javascript" id="btn_ricFascicoli_Click" event="onclick()" for="btn_ricercaTrasm">
			window.document.body.style.cursor='wait';
			
			WndWait();
				
		</script>
		<script language="javascript">
			function _ApriRubricaRicercaTrasm(tipo_corr)
			{
				var r = new Rubrica();
				r.MoreParams = "tipo_corr=" + tipo_corr;
				r.CallType = r.CALLTYPE_RICERCA_TRASM;
				var res = r.Apri(); 		
			}
			
			function apriSalvaRicerca()
		    {
			    window.showModalDialog('../popup/salvaRicerca.aspx?tipo=T',window.self,'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
			}

            //RICERCA SOTTOPOSTI
			function _ApriRubricaRicercaTrasmSottoposti() {
			    var r = new Rubrica();
			    r.MoreParams = "tipo_corr=" + "R";
			    r.CallType = r.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO;
			    var res = r.Apri();
			}

			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-100)/2;
			var new_h = (h-400)/2;
			
			function apriPopupAnteprima() {
					window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicerche.aspx','','dialogWidth:700px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
			}

			function ApriFinestraMultiCorrispondenti() {
			    var rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M&page=ricerca&corrId=ricTrasm', '', 'dialogWidth:730px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
			    window.document.ricTrasmCompleta.submit();
			}
				
		</script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
        <style>
                .tab_scelta_trasm_uno{
             padding-left:10px;
             text-align:left;
         }
        .tab_scelta_trasm_due{
             padding-right:5px;
             text-align:right;
         }
        .tab_scelta_trasm_tre{
            padding-left:5px;
            text-align:left;
         }
        
        </style>
	</HEAD>
	<body topMargin="0" onload="" MS_POSITIONING="GridLayout">
		<form id="ricTrasmCompleta" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca Trasmissioni" />
			<table id="tbl_contenitore" height="100%" width="395" cellspacing="0" cellpadding="0" align="center" border="0">
				<tr valign="top">
					<td>
						<table class="contenitore" height="100%" cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td width="97%" align="center">
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"><asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label></td>
										</tr>
										<tr>
											<td><FONT size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></FONT>
												<asp:dropdownlist id="ddl_Ric_Salvate" runat="server" Width="280px" AutoPostBack="True" CssClass="testo_grigio"></asp:dropdownlist></td>
											<td align="left"><asp:imagebutton id="btn_Canc_Ric" ImageUrl="../images/proto/cancella.gif" Width="19px" Runat="server"
													AlternateText="Rimuove la ricerca selezionata" Height="17px"></asp:imagebutton><FONT size="1"></FONT></td>
										</tr>
								        <TR>
									        <TD height="2"></TD>
								        </TR>
									</table>
								</td>
							</tr>
                            <tr>
								<td height="2" style="width: 9px"></td>
							</tr>
					        <tr>
							    <td>
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="19" style="padding-left:8px;"><asp:Label ID="lbl_tipo_ricerca" runat="server"></asp:Label></td>
										</tr>
                                        <tr>
                                            <td style="padding-left:5px;"> 
                                                <table cellSpacing="0" cellPadding="0" width="100%" align="center" border="0"> 
                                                    <asp:Panel ID="pnl_tipo_ricerca" runat="server" Visible="false">  
                                                    <tr>
                                                        <td style="width:10px"><asp:CheckBox ID="chk_me_stesso" runat="server" AutoPostBack="True" Visible="false"/></td>  
                                                        <td style="width:45px" class="testo_grigio">Utente</td>	
                                                        <td class="testo_grigio"><asp:Label ID="lbl_nome_utente" runat="server"></asp:Label></td>
                                                        <td></td>
                                                    </tr>
                                                    </asp:Panel>
										            <tr>
										                <td style="width:10px"><asp:CheckBox ID="chk_mio_ruolo" runat="server" AutoPostBack="True" Visible="true"/></td>	
                                                        <td style="width:45px" class="testo_grigio">Ruolo</td>		
                                                        <td class="testo_grigio">
                                                            <asp:textbox id="txt1_corr_sott" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="75px"></asp:textbox>
													        <asp:textbox id="txt2_corr_sott" runat="server" CssClass="testo_grigio" Width="190px"></asp:textbox>
                                                        </td>		
                                                        <td style="HEIGHT: 27px" vAlign="middle" align="right" width="29">
													            <asp:Image id="btn_img_sott_rubr" runat="server" ToolTip="Seleziona un corrispondente mittente/destinatario nella rubrica"
														        ImageUrl="../images/proto/rubrica.gif" AlternateText="Seleziona un corrispondente mittente/destinatario nella rubrica"></asp:Image>
                                                        </td>						
										            </tr>
                                                    <tr>
                                                        <td colspan="4">
                                                            <div style="float:right; margin-right:4px;">
                                                                <asp:CheckBox ID="chkHistoricizedRole" runat="server" Text="Estendi a storicizzati" ToolTip="Estendi la ricerca a ruoli storicizzati" CssClass="titolo_scheda" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>     
                                        <tr>
                                            <td style="padding-left:25px;" class="testo_grigio"><asp:Label ID="lbl_notifica" runat="server"></asp:Label> <asp:dropdownlist id="select_sottoposto" runat="server" DataTextField="FULL_NAME" DataValueField="SYSTEM_ID" AutoPostBack="true" CssClass="testo_grigio" Width="248px" Enabled="false" AppendDataBoundItems="true" EnableViewState="true">
							                                    </asp:dropdownlist> 
                                           </td>
                                        </tr> 
                                        <tr>
                                            <td style="padding-left:5px;" class="testo_grigio"><asp:checkbox id="chk_visSott" runat="server" Width="272px" Visible="True" Text="Visualizza trasmissioni sottoposti"></asp:checkbox></td>
                                        </tr>                                
									</table>
							    </td>
							</tr>
							<tr>
								<td height="2" style="width: 9px"></td>
							</tr>
					        <tr>
							    <td>
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Oggetto trasmesso:</td>
										</tr>
										<tr>
										    <TD class="titolo_scheda" height="21">
										        <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">&nbsp; 
								                <asp:DropDownList ID="ddl_oggetto" runat="server" AutoPostBack="true" CssClass="testo_grigio">
										            <asp:ListItem Value="D">Documento</asp:ListItem>
										            <asp:ListItem Value="F">Fascicolo</asp:ListItem>
								                </asp:DropDownList>
								            </TD>
							                <asp:Panel ID="tipo_doc" runat="server">
											<td><asp:dropdownlist id="ddl_tipo_doc" runat="server" AutoPostBack="True" Height="33" CssClass="testo_grigio">
													<asp:ListItem Value="Tutti">Tutti</asp:ListItem>
													<asp:ListItem Value="P">Protocollato</asp:ListItem>
													<asp:ListItem Value="PA" id="opArr" runat="server"></asp:ListItem>
													<asp:ListItem Value="PP" id="opPart" runat="server"></asp:ListItem>
                                                    <asp:ListItem Value="PI" id="opInt" runat="server"></asp:ListItem>
													<asp:ListItem Value="NP">Non Protocollato</asp:ListItem>
												</asp:dropdownlist></td>
							                </asp:Panel>
										</tr>
									</table>
							    </td>
							</tr>
							<asp:panel id="completamento" Runat="server">
						    <tr>
								<td height="2" style="width: 9px"></td>
							</tr>
                            <tr>
                                <td>
                                <!-- Tipologie documento-->
                                    <table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
							            <tr>
							                <td class="titolo_scheda" vAlign="middle" height="19" width="41%"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipologia documento:
                                            </td>
								            <td>
                                                 <asp:dropdownlist id="ddl_tipoDoc_C" runat="server" Width="162px" CssClass="testo_grigio" AutoPostBack="True" Height="16px"></asp:dropdownlist>
                                            </td>
									        <td style="padding-bottom: 5px; padding-top: 5px">
                                                <asp:imagebutton id="btn_CampiPersonalizzati" runat="server" ImageUrl="../images/proto/ico_oggettario.gif"></asp:imagebutton>
                                            </td>
									        <td style="padding-bottom: 5px; padding-top: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
							            </tr>
								        <asp:Panel ID="Panel_StatiDocumento" Runat="server" Visible="false">
								        <tr>
								            <td class="titolo_scheda" style="padding-bottom: 5px; padding-top: 5px" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Stato documento
                                            </td>
									        <td style="padding-bottom: 5px; padding-top: 5px" colSpan="2">
									            <asp:DropDownList id="ddl_statiDoc" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="220px"></asp:DropDownList>
                                            </td>
									        <td style="padding-bottom: 5px; padding-top: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
								        </tr>
								        </asp:Panel>
								    </table>
                                </td>
                            </tr>
                            <tr>
								<td height="2" style="width: 9px"></td>
							</tr>
							<tr>
							    <td>
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="13"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Documenti in completamento</td>
										</tr>
										<tr>
											<td>
												<table cellSpacing="0" cellPadding="0">
													<tr>
														<td class="testo_grigio"><asp:checkbox id="P_Prot" runat="server" Text="Predisposti alla Protocollazione" AutoPostBack="true"></asp:checkbox></td>
                                                        <td class="testo_grigio"><asp:checkbox id="M_Fasc" runat="server" Text="Mancanza fascicolazione" AutoPostBack="true"></asp:checkbox></td>
													</tr>
                                                    <tr>											
														<td class="testo_grigio"><asp:checkbox id="M_si_img" runat="server" Text="Con immagine" AutoPostBack="true"></asp:checkbox></td>
                                                        <td class="testo_grigio"><asp:checkbox id="M_Img" runat="server" Text="Mancanza immagine" AutoPostBack="true"></asp:checkbox></td>
													</tr>

                                                    <asp:Panel ID="lbl_panel_con_imm" Visible="false" runat="server"> 
                                                    <tr>
                                                        <td class="testo_grigio" style="padding-left:10px;">
                                                            <asp:checkbox id="chk_firmati" runat="server" Visible="false" Text="Firmati" AutoPostBack="true"></asp:checkbox>
                                                            <asp:checkbox id="chk_non_firmati" runat="server"  Visible="false" Text="Non Firmati" AutoPostBack="true"></asp:checkbox>
                                                        </td>
                                                        <td class="testo_grigio">Tipo file acquisito &nbsp;<asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="testo_grigio">
							                                <asp:ListItem></asp:ListItem>
							                                </asp:DropDownList>
                                                            </td>
                                                    </tr>
                                                    </asp:Panel>

												</table>
											</td>
										</tr>
									</table>
							    </td>
							</tr>
							</asp:panel>
                            <asp:panel id="pnl_ric_fasc_prof" runat="server" Visible="false">
                            <tr>
								<td height="2" style="width: 9px"></td>
							</tr>
                            <tr>
                                <td>
                                <!-- Tipologie fascicoli-->
                                    <table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
							            <tr>
							                <td class="titolo_scheda" vAlign="middle" height="19" width="41%"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipologia fascicolo:
                                            </td>
								            <td>
                                                 <asp:dropdownlist id="ddl_tipoFasc" runat="server" Width="162px" CssClass="testo_grigio" AutoPostBack="True" Height="16px"></asp:dropdownlist>
                                            </td>
									        <td style="padding-bottom: 5px; padding-top: 5px">
                                                <asp:imagebutton id="img_dettagliProfilazione" runat="server" ImageUrl="../images/proto/ico_oggettario.gif"></asp:imagebutton>
                                            </td>
									        <td style="padding-bottom: 5px; padding-top: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
							            </tr>
                                         <asp:Panel id="Panel_StatiFascicolo" runat="server" visible="false">
										<tr>
                                            <td class="titolo_scheda" style="padding-bottom: 5px; padding-top: 5px" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Stato fasc.</asp:label>
                                            </td>
                                            <td style="padding-bottom: 5px; padding-top: 5px" colSpan="2">	<asp:DropDownList id="ddl_statiFasc" runat="server" CssClass="testo_grigio" AutoPostBack="true" Width="220px"></asp:DropDownList>
											</td>
                                            <td style="padding-bottom: 5px; padding-top: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
										</tr>
										</asp:Panel>
								    </table>
                                </td>
                            </tr>                        
                            </asp:panel>
							<asp:panel id="pnl_filtro" Runat="server">
								<TR>
									<TD height="2"></TD>
								</TR> 
								<TR>
									<TD><!--tabella mittente/destinatario utente-->
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" style="HEIGHT: 27px" vAlign="middle" width="100%" height="27">
													<TABLE cellSpacing="0" cellPadding="0">
														<TR class="titolo_scheda" vAlign="middle">
															<TD>
                                                                <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">&nbsp;
																<asp:Label id="lbl_corr" runat="server" Width="70">Mittente</asp:Label>
                                                            </TD>
															<TD>
																<asp:RadioButtonList id="rbl_tipo_corr" runat="server" AutoPostBack="True" Height="20" Cssclass="testo_grigio"
																	RepeatColumns="3">
																	<asp:ListItem Value="U">UO</asp:ListItem>
																	<asp:ListItem Value="R" Selected="True">Ruolo</asp:ListItem>
																	<asp:ListItem Value="P">Persona</asp:ListItem>
																</asp:RadioButtonList>
                                                            </TD>
														</TR>
													</TABLE>
												</TD>
												<TD style="HEIGHT: 27px" vAlign="middle" align="right" width="29">
													<asp:Image id="btn_Rubrica_C" runat="server" ToolTip="Seleziona un corrispondente mittente/destinatario nella rubrica"
														ImageUrl="../images/proto/rubrica.gif" AlternateText="Seleziona un corrispondente mittente/destinatario nella rubrica"></asp:Image></TD>
											</TR>
											<TR>
												<TD colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
													<asp:textbox id="txt_codCorr" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="75px"></asp:textbox>&nbsp;
													<asp:textbox id="txt_descrCorr" runat="server" CssClass="testo_grigio" Width="220px"></asp:textbox></TD>
											</TR>
                                            <tr>
                                                <td colspan="2">
                                                    <div style="float:right; margin-right:5px;">
                                                        <asp:CheckBox ID="chkHistoricized" runat="server" ToolTip="Estendi ricerca a ruoli storicizzati" Text="Estendi a storicizzati" CssClass="titolo_scheda" />
                                                    </div>
                                                </td>
                                            </tr>
										</TABLE>
									</TD>
								</TR>
								<TR>
									<TD height="2"></TD>
								</TR>   
								<TR>
									<TD><!-- tabella Data trasmissione-->
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data trasmissione</TD>
												<td class="titolo_scheda" vAlign="middle" height="19"><asp:label id="lbl_initdataTrasm" runat="server" CssClass="testo_grigio" Width="18px" Visible="False">Da</asp:label></td>
												<td class="titolo_scheda" vAlign="middle" height="19"><asp:label id="lbl_finedataTrasm" runat="server" CssClass="testo_grigio" Width="10px" Visible="False">A</asp:label></td>
											</TR>
											<TR>
												<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
													<asp:dropdownlist id="ddl_dataTrasm" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
														<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
														<asp:ListItem Value="1">Intervallo</asp:ListItem>
														<asp:ListItem Value="2">Oggi</asp:ListItem>
												        <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
												        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
							                        </asp:dropdownlist>
							                    </TD>
												<td height="25"><uc3:Calendario id="txt_initDataTrasm" runat="server" Visible="true" /></td>
												<td height="25"><uc3:Calendario id="txt_fineDataTrasm" runat="server" Visible="false" /></td>
											</TR>
										</TABLE>
									</TD>
								</TR>
								<TR>
									<TD height="2"></TD>
								</TR>
								<TR>
									<TD align="left" width="100%"><!-- tabella Ragione trasm-->
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR align="left">
												<TD width="4"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></TD>
												<TD class="titolo_scheda" vAlign="middle" align="left" height="19">Ragione 
													Trasmissione</TD>
											</TR>
											<TR align="left">
												<TD width="4"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></TD>
												<TD height="25">
													<asp:dropdownlist id="ddl_ragioni" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="180px"></asp:dropdownlist></TD>
											</TR>											
										</TABLE>
									</TD>
								</TR>
								<TR>
									<TD height="2"></TD>
								</TR> <!-- ALTRI FILTRI -->
								    <asp:Panel ID="AccettateRifiutate" runat="server">
					                <tr>
									<td align="left" width="100%">
									    <TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
					                        <tr>
					                            <td align="left" style="padding-left:5px" colspan="3">
					                                <asp:CheckBox ID="cbx_Acc" runat="server" AutoPostBack="True" Text="Accettate" CssClass="testo_grigio"/>
					                                <asp:CheckBox ID="cbx_Rif" runat="server" AutoPostBack="True" Text="Rifiutate" CssClass="testo_grigio"/>		
					                                <asp:CheckBox ID="cbx_Pendenti" runat="server" AutoPostBack="True" Text="Pendenti" CssClass="testo_grigio"/>		                                                                
					                            </td>					            
					                        </tr>
					                        <tr>
												<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></TD>
												<td class="titolo_scheda" vAlign="middle" height="19"><asp:label id="lbl1_TAR" runat="server" CssClass="testo_grigio" Width="18px" Visible="False">Da</asp:label></td>
												<td class="titolo_scheda" vAlign="middle" height="19"><asp:label id="lbl2_TAR" runat="server" CssClass="testo_grigio" Width="10px" Visible="False">A</asp:label></td>
											</TR>
											<TR>
												<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
													<asp:dropdownlist id="ddl_TAR" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
														<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
														<asp:ListItem Value="1">Intervallo</asp:ListItem>
											            <asp:ListItem Value="2">Oggi</asp:ListItem>
										                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
										                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
													</asp:dropdownlist></TD>
												<td height="25"><uc3:Calendario id="dataUno_TAR" runat="server" Visible="true" /></td>
												<td height="25"><uc3:Calendario id="dataDue_TAR" runat="server" Visible="false" /></td>
											</TR>
						                </TABLE>						                
				                    </td>				
				                </tr>
				                </asp:Panel>
				                <TR>
									<TD height="2"></TD>
								</TR>
								<TR>
									<TD align="left" width="100%"><!-- altri filtri-->
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle" colSpan="3" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Altri filtri</TD>
											</TR>
                                            <tr>
                                                 <td class="testo_grigio"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Note generali</td>
                                                 <td class="testo_grigio"><asp:textbox id="txt_note_generali" runat="server" CssClass="testo_grigio" Width="230px"></asp:textbox></td>
                                            </tr>
                                            <tr>
                                                <td class="testo_grigio"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Note individuali</td>
                                                <td class="testo_grigio"><asp:textbox id="txt_note_individuali" runat="server" CssClass="testo_grigio" Width="230px"></asp:textbox></td>
                                            </tr>
                                            <tr>                               
                                                <td class="testo_grigio"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data scadenza</td>
                                                <td class="testo_grigio">Dal <uc3:Calendario id="cld_scadenza_dal" runat="server"/> al <uc3:Calendario id="cld_scadenza_al" runat="server"/></td>
                                            </tr>
                                            <tr>
                                                <td class="testo_grigio">&nbsp;&nbsp;Ordinamento</td>
                                                <td class="testo_grigio">
                                                    <asp:DropDownList ID="ddlOrder" runat="server" CssClass="testo_grigio" />&nbsp;
                                                    <asp:DropDownList ID="ddlOrderDirection" runat="server" CssClass="testo_grigio">
                                                        <asp:ListItem Text="Crescente" Value="ASC" />
                                                        <asp:ListItem Text="Decrescente" Value="DESC" />
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
											<TR>
												<TD height="2"></TD>
											</TR> 
										</TABLE>
									</TD>
								</TR>													
							</asp:panel>
							<tr>
								<td class="titolo_scheda"><asp:checkbox id="chk_DaCompletare" runat="server" Width="272px" Visible="False" Text="Mancanza notifica conclusione attività"></asp:checkbox></td>
							</tr>
                            <tr>
                <td height="4" class="testo_grigio">
                    <font size="1">
                        <img height="15" src="../images/proto/spaziatore.gif" width="4" border="0"/></font></td>
            </tr>		
						</TABLE>

					</td>
				</tr>		
				<tr height="10%">
					<td style="width: 98%">
						<!--1-->
						<!-- BOTTONIERA --><INPUT id="hd_systemIdCorr" type="hidden" size="1" name="hd_systemIdMit" runat="server">
                        <INPUT id="hd_systemIdCorrSott" type="hidden" size="1" name="hd_systemIdSott" runat="server">
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
<%--								<td><cc1:imagebutton id="btn_ricercaTrasm" Runat="server" Thema="btn_" SkinID="cerca_attivo"
										AlternateText="Cerca" Tipologia="TRAS_CERCA"></cc1:imagebutton></td>
--%>
								<TD><asp:Button ID="btn_ricercaTrasm" CssClass="pulsante79" Text="Cerca" runat="server" ToolTip="Ricerca trasmissioni" /></TD>
								<td><asp:Button ID="btn_salva" CssClass="pulsante79" Text="Salva" runat="server" ToolTip="Salva i criteri di ricerca" /></td>
							</TR>
						</TABLE> <!--FINE BOTTONIERA --></td>
				</tr>
			</table>
       <cc2:messagebox id="mb_ConfirmDelete" style="z-index: 101; left: 576px; position: absolute;
            top: 24px" runat="server">
        </cc2:messagebox>
		</form>
	</body>
</HTML>