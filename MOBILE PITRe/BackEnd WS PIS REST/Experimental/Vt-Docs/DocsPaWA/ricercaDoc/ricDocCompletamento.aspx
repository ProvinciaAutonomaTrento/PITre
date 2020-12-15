<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Page language="c#" Codebehind="ricDocCompletamento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.f_Ricerca_Compl" %>
<%@ Register TagPrefix="uc1" TagName="Creatore" Src="../UserControls/Creatore.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/DocumentConsolidationSearchPanel.ascx" TagName="DocumentConsolidation" TagPrefix="uc4" %>
<%@ Register TagPrefix="aof" TagName="AuthorOwnerFilter" Src="~/UserControls/AuthorOwnerFilter.ascx" %>
<%@ Import Namespace="DocsPAWA.ricercaDoc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title>DOCSPA > Completamento</title>
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
		<script language="javascript">
			function _ApriRubrica(target)
			{
				var r = new Rubrica();
			
				switch (target) {
					case "ric_completamento":
						r.CallType = r.CALLTYPE_RICERCA_COMPLETAMENTO;
						break;
		
                    case "ric_stor":
		                r.CallType = r.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO;
		                break;
                }
				var res = r.Apri(); 		
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
        
        function SingleSelect(regex,current) 
        { 
            re = new RegExp(regex);
            for(i = 0; i < document.forms[0].elements.length;i++)
            {
        
                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name))
                {
                    elm.checked = false; 
                } 
            }
        }

        function apriModificaRicerca(codiceRicerca) {
            window.showModalDialog("../popup/ModificaRicerca.aspx?idRicerca=" + codiceRicerca + "", window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
        }

        function ApriFinestraMultiCorrispondenti() {
            rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M&page=ricerca&corrId=ricCompletamento', '', 'dialogWidth:730px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
            window.document.f_Ricerca_Compl.submit();
        }

		</script>
</HEAD>
	<body leftMargin="0" MS_POSITIONING="GridLayout">
		<form id="f_Ricerca_Compl" method="post" runat="server">

			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="398" align="center" border="0">
				<tr vAlign="top">
					<td align="left">
						<table class="contenitore" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<!-- <tr>
								<td style="HEIGHT: 4px" height="4"><asp:imagebutton id="enterKeySimulator" runat="server" ImageUrl="../images/spacer.gif"></asp:imagebutton></td>
							</tr> -->
                            <tr valign="top">
                                <td>
                                    <asp:imagebutton id="enterKeySimulator2" runat="server" imageurl="..\images\spacer.gif"></asp:imagebutton>
                                    <font size="1"></font>
                                </td>
                            </tr>
							<TR>
								<TD class="testo_grigio" vAlign="top">
									<TABLE class="info_grigio" id="Table1" style="HEIGHT: 48px" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
											<asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label> 
											</TD>
                                             <td align="left">
                                            <cc1:imagebutton id="btn_clear_fields" ImageUrl="../images/ricerca/remove_search_filter.gif"
													Runat="server" AlternateText="Pulisci i campi" ToolTip="Pulisci i campi" CssClass="clear_flag" OnClick="btnCleanUpField_Click">
                                                    </cc1:imagebutton>
                                            </td>
										</TR>
										<TR>
											<TD><FONT size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></FONT>
												<asp:dropdownlist id="ddl_Ric_Salvate" runat="server" Width="328px" AutoPostBack="True" CssClass="testo_grigio"></asp:dropdownlist></TD>
											<TD align="left"><asp:imagebutton id="btn_Canc_Ric" ImageUrl="../images/proto/cancella.gif" Width="19px" Runat="server"
													AlternateText="Rimuove la ricerca selezionata" Height="17px"></asp:imagebutton><FONT size="1"></FONT></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						    <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
								<td valign="top">
									<!-- INIZIO TABELLA Archivio documento-->
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="19"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />Tipo</td>
											<td>
											    <asp:CheckBoxList ID="cbl_archDoc_C" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
											                          <asp:ListItem Value="A" Selected="True" runat="server" id="opArr"></asp:ListItem>
											        <asp:ListItem Value="P" Selected="True" runat="server" id="opPart"></asp:ListItem>
											        <asp:ListItem Value="I" Selected="True" runat="server" id="opInt"></asp:ListItem>
											        <asp:ListItem Value="G" Selected="True" runat="server" id="opGrigio"></asp:ListItem>
											        <asp:ListItem Value="Pr" Selected="False">Predisp.</asp:ListItem>
													<asp:ListItem Value="ALL" Selected="False" runat="server" id="opAll"></asp:ListItem>
							                    </asp:CheckBoxList>
										    </td>
										</tr>

                                        <!-- radiobuttonlist per ricerca allegati -->
                                        <tr>
                                            <td>
                                        
                                            </td>
                                            <td>
                                                <asp:RadioButtonList ID="rblFiltriAllegati" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="tutti" Text="Tutti" />
                                                    <asp:ListItem Value="pec" Text="PEC" />
                                                    <asp:ListItem Value="user" Text="Utente" Selected="True" />
                                                    <asp:ListItem Value="esterni" Text="Sist. esterni" />
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>

									</table>
								</td>
							</tr>
						    <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
	                        <tr vAlign="top">
								<td class="testo_grigio" vAlign="top">
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
										    <td class="testo_grigio" align=right>
										        <asp:RadioButtonList ID="rbl_immagine" runat="server" CssClass="testo_grigio" RepeatColumns="3" RepeatDirection="Horizontal">
										            <asp:ListItem Value="0" Text="Con Immagine&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
										            <asp:ListItem Value="1" Text="Senza Immagine&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
										            <asp:ListItem Value="Reset" Text="Reset"></asp:ListItem>
										        </asp:RadioButtonList>
										    </td>
										</tr>
                                        <tr>
										    <td class="testo_grigio" align=right>
										        <asp:RadioButtonList ID="rbl_fascicolazione" runat="server" CssClass="testo_grigio" RepeatColumns="3" RepeatDirection="Horizontal">
										            <asp:ListItem Value="0" Text="Con fascicolazione&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
										            <asp:ListItem Value="1" Text="Senza fascicolazione&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
										            <asp:ListItem Value="Reset" Text="Reset"></asp:ListItem>
										        </asp:RadioButtonList>
										    </td>
										</tr>
								    </table>
								</td>
							</tr>
						    <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
                            <asp:Panel ID="pnl_timestamp" Visible="false" runat="server">
                            <tr valign="top">
                                <td class="testo_grigio" valign="top">
                                    <!--Tabella ricerca timestamp-->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
										    <td class="testo_grigio" align="right" colspan="2">
                                                <asp:RadioButtonList ID="rbl_timestamp" runat="server" CssClass="testo_grigio" RepeatColumns="3" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rbl_timestamp_SelectedIndexChanged">
										        <asp:ListItem Value="0" Text="Con Timestamp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
										        <asp:ListItem Value="1" Text="Senza Timestamp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>    
                                                <asp:ListItem Value="2" Text="Reset" Selected="True"></asp:ListItem>
										        </asp:RadioButtonList>                                                    
										    </td>
										</tr> 
                                        <tr>
                                            <td style="padding-left:10px; padding-bottom:5px">
                                                <asp:DropDownList id="ddl_timestamp" runat="server" CssClass="testo_grigio" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddl_timestamp_SelectedIndexChanged">
                                                    <asp:ListItem Value="0" Text="" Selected="True"></asp:ListItem>
													<asp:ListItem Value="1" Text="Scaduto"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Scade prima di"></asp:ListItem>                                                    
                                                </asp:DropDownList>   
                                                <uc3:Calendario ID="date_timestamp"  runat="server" Visible="false"/>                                                
                                            </td>
                                        </tr>                                       
								    </table>
								</td>
                            </tr>
                            <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
                            </asp:Panel>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<!-- tabella registro-->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
                                        <tr>
											<td class="testo_grigio">
											    <table width="100%" border="0"><tr>
                   					            <td width="50%"><asp:CheckBox ID="cbx_Trasm" runat="server" AutoPostBack="True" Text="Trasmesse con" CssClass="testo_grigio"/></td>
					                            <td width="50%"><asp:CheckBox ID="cbx_TrasmSenza" runat="server" AutoPostBack="True" Text="Mancanza trasmissione con" CssClass="testo_grigio"/></td>
					                            </tr></table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="testo_grigio">
                                                 <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ragione:&nbsp;
                                                 <asp:DropDownList ID="ddl_ragioneTrasm" runat="server" CssClass="testo_grigio"></asp:DropDownList>
                                            </td> 
										</tr>
									</table>
								</td>
							</tr>
                            
                            
              <!--va qui-->


						   <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<!-- tabella registro-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
                                        <TR>
											<TD class="titolo_scheda" vAlign="top" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registro</TD>
										</tr>
                                        <tr>	<TD valign="top">
												<table cellSpacing="1" cellPadding="1" border="0">
													<tr>
														<td><FONT size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></FONT></td>
														<td><asp:listbox id="lb_reg" runat="server" Width="245px" AutoPostBack="True" CssClass="testo_grigio"
																Rows="3" SelectionMode="Multiple"></asp:listbox>
														</td>
														<td><asp:radiobuttonlist id="rbl_Reg" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="48" Height="32">
																
															</asp:radiobuttonlist></td>
													</tr>
												</table>
											</TD>
										</TR>
										<tr>
											<tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
										</tr>
									</TABLE>
								</td>
							</tr>
						    <tr><td><img height="3" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<!--tabella oggetto -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Oggetto</TD>
											<TD vAlign="middle" align="right" width="29"><asp:imagebutton id="btn_RubrOgget" ImageUrl="../images/proto/ico_oggettario.gif" Width="19px" Runat="server"
													AlternateText="Seleziona un oggetto nell'oggettario" Height="17px"></asp:imagebutton></TD>
										</TR>
										<TR>
											<TD class="testo_grigio" colSpan="2" height="20">&nbsp;
												<asp:textbox id="txt_oggetto" runat="server" Width="353px" CssClass="testo_grigio" Height="30px"
													TextMode="MultiLine"></asp:textbox>&nbsp;</TD>
										</TR>
										<tr>
											<td colSpan="2"><IMG height="3" src="../images/proto/spaziatore.gif" width="8" border="0"></td>
										</tr>
									</TABLE>
								</td>
							</tr>
						   <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<!--tabella mittente/destinatario -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Mittente/Destinatario
											    &nbsp;&nbsp;&nbsp;&nbsp;
										        <asp:CheckBox ID="cb_mitt_dest_storicizzati" runat="server" Text="Ricerca storicizzati" Checked="true" CssClass="titolo_scheda" AutoPostBack="true" />
    										</TD>
											<td>
												<table cellSpacing="0" cellPadding="0">
													<tr>
														<TD vAlign="middle" align="right" width="29"><IMG id="btn_Rubrica" style="CURSOR: hand" height="19" alt="Seleziona un mittente/destinatario nella rubrica"
																src="../images/proto/rubrica.gif" width="29" runat="server">
														</TD>
													</tr>
												</table>
												<INPUT class="testo_grigio" id="hd_systemIdMit_Compl" type="hidden" size="3" name="hd_systemIdMit_Compl"
													runat="server" height="3" width="5">
											</td>
										</TR>
										<TR>
											<TD colSpan="2" height="25">&nbsp;
												<asp:textbox id="txt_codMit" runat="server" Width="75px" AutoPostBack="True" CssClass="testo_grigio"></asp:textbox>&nbsp;&nbsp;<asp:textbox id="txt_descrMit" runat="server" Width="264px" CssClass="testo_grigio"></asp:textbox></TD>
										</TR>

                                          <asp:Panel ID="p_cod_amm" runat="server">
                                          <tr>
                                      <td class="titolo_scheda" width="200" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                      <asp:Label ID="l_amm_interop"  Visible= "true" runat="server" Text="Amm.ne Interoperante"></asp:Label></td>
                                      </tr>
                                         <tr>
                                        <td colspan="2" height="25">
   									<!-- tabella codice  amministrazione mittente-->
                            			     <font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
											<asp:textbox id="txt_codDesc" runat="server" cssclass="testo_grigio" width="280px"></asp:textbox>
                                           </font>
										</tr>
                                        </asp:Panel>
									</TABLE>
								</td>
							</tr>
						    <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
					
                                   <tr runat="Server" id="trNumProto">
                                        <td class="titolo_scheda" height="18">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Numero protocollo
                                        </td>
                                        <td class="titolo_scheda" align="left" height="18">Anno</td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" valign="middle">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:dropdownlist id="ddl_numProt_C" runat="server" autopostback="True" cssclass="testo_grigio" width="110px">
													<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
												</asp:dropdownlist>
                                            &nbsp;&nbsp;
                                            <asp:label id="lblDAnumprot_C" runat="server" cssclass="testo_grigio" width="10px">Da</asp:label>
                                            <asp:textbox id="txt_initNumProt_C" runat="server" cssclass="testo_grigio" width="60px"></asp:textbox>
                                            &nbsp;&nbsp;<asp:label id="lblAnumprot_C" runat="server" cssclass="testo_grigio" width="10px">A</asp:label>
                                            <asp:textbox id="txt_fineNumProt_C" runat="server" cssclass="testo_grigio" width="60px"></asp:textbox>
                                        </td>
                                        <td align="left"><asp:textbox id="tbAnnoProtocollo" runat="server" cssclass="testo_grigio" width="40px" maxlength="4"></asp:textbox>&nbsp;&nbsp;</td>
                                    </tr>
                                    <asp:panel runat="Server" id="pnl_dataProt">
                                    <tr>
                                        <td colspan="2">
                                           <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
					
					
										<tr>
											<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data protocollo</td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_initdataProt" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_finedataProt" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
										</tr>
										<tr>
											<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:dropdownlist id="ddl_dataProt" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></td>
											<td><uc3:Calendario id="txt_initDataProt" runat="server" Visible="true" /></td>
											<td><uc3:Calendario id="txt_fineDataProt" runat="server" Visible="false" /></td>
																	</tr>
									</table>
                                        </td></tr>
                                        </asp:panel>
                                </table>
								</td>
							</tr>
                            <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
                                                          <asp:Panel runat="server" ID="pnl_riferimento" Visible="false">
                        <tr valign="top">
                            <td valign="top">
                                <!-- tabella riferimento-->
                                <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center"
                                    border="0" height="30px">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" style="padding-left: 10px;padding-top:2px;padding-bottom:2px;">
                                            Riferimento
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left: 10px;">
                                            <asp:TextBox ID="txt_rif_mittente" runat="server" CssClass="testo_grigio" Width="348px" MaxLength="100"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                         <tr>
                            <td><img height="2" src="../images/proto/spaziatore.gif" border="0"></td>
                        </tr>
                    </asp:Panel>
							<tr>
								<td style=" vAlign="top">
									<!--tabella protocollo mittente -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Protocollo 
												mittente</TD>
										</TR>
										<TR>
											<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:textbox id="txt_numProtMitt_C" Width="348px" CssClass="testo_grigio" Runat="server"></asp:textbox>&nbsp;
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
						    <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<table id="Table3" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
		                                <TR>
									        <TD class="titolo_scheda" vAlign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Id documento</TD>
									        <td class="titolo_scheda" vAlign="middle" height="18"><asp:label id="lblDAidDoc_C" runat="server" CssClass="testo_grigio" Width="18px" Visible="False">Da</asp:label></td>
									        <td class="titolo_scheda" vAlign="middle" height="18"><asp:label id="lblAidDoc_C" runat="server" CssClass="testo_grigio" Width="12px" Visible="False">A</asp:label></td>
								        </TR>
								        <TR height="20">
									        <TD vAlign="middle" width="140"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
										        <asp:dropdownlist id="ddl_idDocumento_C" runat="server" CssClass="testo_grigio" 
                                                    Width="110px" AutoPostBack="true" >
											        <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
											        <asp:ListItem Value="1">Intervallo</asp:ListItem>
										        </asp:dropdownlist></TD>
									        <TD vAlign="top" width="100"><asp:textbox id="txt_initIdDoc_C" runat="server" CssClass="testo_grigio" Width="80px"></asp:textbox></TD>
									        <TD vAlign="top" width="100"><asp:textbox id="txt_fineIdDoc_C" runat="server" CssClass="testo_grigio" Width="80px" Visible="False"></asp:textbox></td>
								        </TR>
								        <tr>
											<td class="titolo_scheda" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Creazione</td>
											<td class="titolo_scheda" valign="middle" height="18"><asp:label id="lbl_initdataCreaz" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" valign="middle" height="18"><asp:label id="lbl_finedataCreaz" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
										</tr>
										<tr>
											<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:dropdownlist id="ddl_dataCreaz" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></td>
											<td><uc3:Calendario id="txt_initDataCreaz" runat="server" Visible="true" /></td>
											<td><uc3:Calendario id="txt_fineDataCreaz" runat="server" Visible="false" /></td>
										</tr>
									</table>
								</td>
							</tr>
						    <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr align="center">
							    <td><aof:AuthorOwnerFilter ID="aofAuthor" runat="server" ControlType="Author" /></td>
                            </tr>
                            <tr>
								<td height="2"></td>
							</tr>
                            <tr align="center">
                                <td><aof:AuthorOwnerFilter ID="aofOwner" runat="server" ControlType="Owner" /></td>
                            </tr>
						   
						    <tr><td><img height="2" /></td></tr>
						    <div id="DivDataProtMittente" runat="server">
							<tr>
								<td class="testo_grigio" style="HEIGHT: 50px" vAlign="top">
									<!-- tabella Data protocollo mittente-->
									<table id="Table4" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
										<tr>
											<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data 
												prot. mittente</td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_initdataProtMitt_C" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_finedataProtMitt_C" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
										</tr>
										<tr>
											<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:dropdownlist id="ddl_dataProtMitt_C" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></td>
											<td><uc3:Calendario id="txt_initDataProtMitt_C" runat="server" Visible="true" /></td>
											<td><uc3:Calendario id="txt_fineDataProtMitt_C" runat="server" Visible="false" /></td>
										</tr>
										     <tr>
							                <td class="titolo_scheda" valign="middle" style="height:18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipo file acquisito&nbsp;</td>							               
							            </tr>
							            <tr>
							                <td><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
							                <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="testo_grigio" Width="150">
							                <asp:ListItem></asp:ListItem>
							                </asp:DropDownList>
							                </td>
							                <td>
							                    <asp:CheckBox ID="chkFirmato" runat="server" CssClass="testo_grigio"  Text="Firmati" onclick="SingleSelect('chk',this);"/>
							                </td>
							                <td>
							                    <asp:CheckBox ID="chkNonFirmato" runat="server" CssClass="testo_grigio" Text="Non firmati" onclick="SingleSelect('chk',this);"/>
							                </td>
							            </tr>	
									</table>
								</td>
							</tr>
							</div>
							<tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							<tr>
							    <td class="testo_grigio" vAlign="top">
							        <table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
							        <tr>
							            <td  class="titolo_scheda" valign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
							            Documenti Spediti&nbsp;&nbsp;
							            </td>
                                        <td align="right"><asp:CheckBox runat="server" ID="cbx_pec" Text="PEC" CssClass="testo_grigio" />&nbsp;&nbsp;<asp:CheckBox runat="server" ID="cbx_pitre" Text="PITRE" CssClass="testo_grigio" />&nbsp;&nbsp;</td>
							        </tr>
							        <tr>
							            <td class="titolo_scheda" valign="middle" height="19" colspan="2">
							            <asp:radiobuttonlist id="rb_docSpediti" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal" AutoPostBack="true">
								<asp:ListItem Value="1">Con ricevuta di ritorno</asp:ListItem>
								<asp:ListItem Value="0">In attesa di risposta</asp:ListItem>
								<asp:ListItem Value="T" >Tutti</asp:ListItem>
								<asp:ListItem Value="R" Selected="True" >Reset</asp:ListItem>
							</asp:radiobuttonlist>							           						           
							            </td>
							        </tr>
							        <tr>
							        <td class="titolo_scheda" valign="middle" height="19" colspan="2"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
							        Data spedizione
							        </td>							   
							        </tr>	
							        <tr>
							        <td class="titolo_scheda" valign="middle" height="19" colspan="2"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                  <asp:label id="lbl_dataSpedDa" runat="server" cssclass="testo_grigio" width="8px">Da</asp:label>
                                   <uc3:Calendario ID="txt_dataSpedDa" runat="server" Visible="true"/>&nbsp;&nbsp;&nbsp;
                            <asp:label id="lbl_dataSpedA" runat="server" cssclass="testo_grigio" width="8px">a</asp:label>
                        <uc3:Calendario ID="txt_dataSpedA" runat="server" Visible="true" /></td>
							        </tr>
							        <tr>
							                <td class="titolo_scheda" valign="middle" colspan="2"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>							               
							            </tr>
							        </table>
							    </td>
							</tr>
                            <tr>
                            <td class="titolo_scheda" valign="middle"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>							               
                            </tr>
                               <!--qui-->
                             <tr>
                            <td>
                                <asp:Panel ID="p_ricevute_pec" runat="server">
                                   <table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
                                    <tr>
                                        <TD class="titolo_scheda" vAlign="top" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ricevute PEC</td>
                                    </tr>
                                    <tr></tr>
                                    <tr>
                                          <td class="testo_grigio">
                                                 <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ricevute di&nbsp;
                                          <asp:DropDownList ID ="ddl_ricevute_pec" runat ="server" CssClass="testo_grigio" AutoPostBack="true">
                                            <asp:ListItem Text="" Value="" runat ="server"></asp:ListItem>
                                            <asp:ListItem Text="avvenuta accettazione" Value="accettazione" runat ="server"></asp:ListItem>
                                            <asp:ListItem Text="avvenuta consegna" Value="avvenuta-consegna" runat ="server"></asp:ListItem>
                                            <asp:ListItem Text="mancata accettazione" Value="non-accettazione" runat ="server"></asp:ListItem>
                                            <asp:ListItem Text="mancata consegna" Value="errore-consegna" runat ="server" ></asp:ListItem>
                                            <asp:ListItem Text="con errori" Value="errore" runat ="server" ></asp:ListItem>
                                        </asp:DropDownList>
                                        </asp:Label>
                                        </td>
                                    </tr>
                                    <tr></tr>
                                    <tr>
                                        <td colspan="2">
                                           <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Ricevuta</td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="L_da_ricevute_pec" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="L_a_ricevute_pec" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
										</tr>
										<tr>
											<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:dropdownlist id="ddl_data_ricevute_pec" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></td>
											<td><uc3:Calendario id="Cal_Da_pec" runat="server" Visible="true" /></td>
											<td><uc3:Calendario id="Cal_A_pec" runat="server" Visible="false" /></td>
										</tr>
									</table>
                                        </td></tr>
                                    </table>
                                </asp:Panel>
                                </td>
                            </tr>
                             <tr>
                            <td>
                                <asp:Panel ID="p_ricevute_pitre" runat="server" Visible="true">
                                   <table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
                                    <tr>
                                        <TD class="titolo_scheda" vAlign="top" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ricevute PITRE</td>
                                    </tr>
                                    <tr></tr>
                                    <tr>
                                          <td class="testo_grigio">
                                                 <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ricevute di&nbsp;
                                          <asp:DropDownList ID ="ddl_ricevute_pitre" runat ="server" CssClass="testo_grigio" AutoPostBack="true">
                                            <asp:ListItem Text="" Value="" runat ="server"></asp:ListItem>
                                            <asp:ListItem Text="Avvenuta consegna" Value="avvenuta-consegna" runat ="server"></asp:ListItem>
                                            <asp:ListItem Text="Mancata consegna" Value="errore-consegna" runat ="server" ></asp:ListItem>
                                        </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr></tr>
                                    <tr>
                                        <td colspan="2">
                                           <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Ricevuta</td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="L_da_ricevute_pitre" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="L_a_ricevute_pitre" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
										</tr>
										<tr>
											<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:dropdownlist id="ddl_data_ricevute_pitre" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></td>
											<td><uc3:Calendario id="Cal_Da_pitre" runat="server" Visible="true" /></td>
											<td><uc3:Calendario id="Cal_A_pitre" runat="server" Visible="false" /></td>
										</tr>
									</table>
                                        </td></tr>
                                    </table>
                                </asp:Panel>
                                </td>
                            </tr>
						    <tr>
                                <td>
                                    <uc4:DocumentConsolidation id="documentConsolidationSearch" runat="server"></uc4:DocumentConsolidation>
                                </td>
                            </tr>
                             <!-- Ordinamento -->
                            <div>
                               
							    <tr>
								    <td>
									    <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										    <tr>
											    <td class="titolo_scheda" valign="middle" height="19" width="35%" style="padding-bottom: 5px; padding-top: 5px">
                                                    &nbsp;Ordinamento
                                                </td>
                                            </tr>
                                            <tr>
										        <td style="padding-bottom: 5px; padding-top: 5px" class="style1">
                                                    &nbsp;<asp:DropDownList ID="ddlOrder" runat="server" CssClass="testo_grigio" Width="270" />
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
                                    <td height="2" />
                                </tr>  
						</table>
					</td>
				</tr>
               
				<TR>
					<TD >
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
								<TD vAlign="top" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<TD>
								    <asp:Button ID="btn_ricerca" runat="server" Text="Ricerca" CssClass="pulsante69" ToolTip="Ricerca documenti protocollati" OnClick="btn_ricerca_Click1" />
								    <asp:Button ID="btn_salva" Text="Salva" runat="server" CssClass="pulsante69" ToolTip="Salva i criteri di ricerca" />
                                     <asp:Button ID="btn_modifica" CssClass="pulsante79" runat="server" Text="Modifica" ToolTip="Modifica la ricerca salvata" OnClick="ModifyRapidSearch_Click" />
								<!--TR>
								<TD width="100%" bgColor="#810d06"><IMG height="2" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
							</TR--></TR>
						</TABLE> <!--FINE BOTTONIERA --></TD>
				</TR>
			</table>
			<cc2:messagebox id="mb_ConfirmDelete" style="Z-INDEX: 101; LEFT: 600px; POSITION: absolute; TOP: 384px"
				runat="server"></cc2:messagebox></form>
	</body>
</HTML>
