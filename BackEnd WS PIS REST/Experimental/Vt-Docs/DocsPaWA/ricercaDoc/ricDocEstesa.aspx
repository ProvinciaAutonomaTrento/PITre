<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Page Language="c#" Codebehind="ricDocEstesa.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.ricercaDoc.f_Ricerca_E" %>
<%@ Register TagPrefix="uc1" TagName="Creatore" Src="../UserControls/Creatore.ascx" %>
<%@ Register TagPrefix="aof" TagName="AuthorOwnerFilter" Src="~/UserControls/AuthorOwnerFilter.ascx" %>

<%@ Import Namespace="DocsPAWA.ricercaDoc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>DOCSPA > ricDocEstesa</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>
    <script language="javascript" id="enterKeySimulator_click" event="onclick()" for="enterKeySimulator">
			window.document.body.style.cursor='wait';			
			WndWait();						
    </script>
    <script language="javascript" id="btn_Ricerca_click" event="onclick()" for="btn_Ricerca">
			window.document.body.style.cursor='wait';			
			WndWait();
    </script>
    <script language="javascript">
		function _ApriRubrica(target)
		{
		    var r = new Rubrica();
		
			switch (target) {
				case "ric_estesa":
					r.CallType = r.CALLTYPE_RICERCA_ESTESA;
					break;
            	case "ric_E":
            	    r.CallType = r.CALLTYPE_RICERCA_CORRISPONDENTE;
	                break;
	            case "ric_stor":
	                r.CallType = r.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO;
	                break;
	        }
			var res = r.Apri(); 		
		}		
		
		function scriviData(obj,sep)
		{
			if(document.getElementById(obj).value.length==2 || document.getElementById(obj).value.length==5)
			{
				document.getElementById(obj).value+=sep;
			}
		}		
		
		function apriSalvaRicerca()
		{
		    var retval = window.showModalDialog('../popup/salvaRicerca.aspx?tipo=D', window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
            if (retval != null) {
                top.principale.iFrame_dx.document.location = '../waitingpage.htm';
		    }
		}
		
		function apriSalvaRicercaADL()
		{
		    window.showModalDialog('../popup/salvaRicerca.aspx?ricADL=1&tipo=D', window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
		    if (retval != null) {
		        top.principale.iFrame_dx.document.location = '../waitingpage.htm';
		    }
		}

		function OnChangeSavedFilter()
		{
			var ctl=document.getElementById("ddl_Ric_Salvate");

			if (ctl!=null && ctl.value>0)
				top.principale.iFrame_dx.location='../waitingpage.htm';	
		}

		    //Apre la PopUp Modale per la ricerca dei fascicoli
		    //utile per la fascicolazione rapida
		function ApriRicercaFascicoli2(codiceClassifica, NodoMultiReg) {
		    var newUrl;

		    newUrl = "../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica + "&caller=profilo&NodoMultiReg=" + NodoMultiReg;

		    var newLeft = (screen.availWidth - 615);
		    var newTop = (screen.availHeight - 704);

		    // apertura della ModalDialog
		    rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:auto;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
		    if (rtnValue == "N") {
		        window.document.f_Ricerca_E.txt_CodFascicolo.value = "";
		        window.document.f_Ricerca_E.txt_DescFascicolo.value = "";
		    }
		    if (rtnValue == "Y") {
		        window.document.f_Ricerca_E.submit();
		    }
		}

		//Apre la PopUp Modale per la ricerca dei fascicoli
		//utile per la fascicolazione rapida
		function ApriRicercaFascicoli(codiceClassifica, NodoMultiReg) {
		    var newUrl;

		    newUrl = "../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica + "&caller=profilo&NodoMultiReg=" + NodoMultiReg;

		    var newLeft = (screen.availWidth - 615);
		    var newTop = (screen.availHeight - 704);

		    // apertura della ModalDialog
		    rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
		    if (rtnValue == "N") {
		        window.document.f_Ricerca_E.txt_CodFascicolo.value = "";
		        window.document.f_Ricerca_E.txt_DescFascicolo.value = "";
		    }
		    if (rtnValue == "Y") {
		        window.document.f_Ricerca_E.submit();
		    }
		}

		function apriModificaRicerca(codiceRicerca) {
		    var retval = window.showModalDialog("../popup/ModificaRicerca.aspx?idRicerca=" + codiceRicerca + "", window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
		    if (retval != null) {
		        top.principale.iFrame_dx.document.location = '../waitingpage.htm';
		    }
		}

		function ApriFinestraMultiCorrispondenti() {
		    var rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M&page=ricerca&corrId=ricEst', '', 'dialogWidth:730px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
		    window.document.f_Ricerca_E.submit();
		}
    </script>
    <style>

        .clear_flag img{
             border:0px;
             margin:0px;
             padding:0px;
        }
        </style>

</head>
<body leftmargin="0" ms_positioning="GridLayout">
    <form id="f_Ricerca_E" method="post" runat="server">
        <table id="tbl_contenitore" cellspacing="0" cellpadding="0" align="center" border="0" width="395" height="100%">
            <tr valign="top">
                <td align="center">
                    <table class="contenitore" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td style="HEIGHT: 2px" height="2"><asp:imagebutton id="enterKeySimulator" runat="server" ImageUrl="../images/spacer.gif"></asp:imagebutton></td>
							</tr>
                        <asp:panel runat="Server" id="pnlRicSalvate">
							<tr>
								<td valign="top" >
									<table class="info_grigio" id="Table1" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" >
										<tr>
											<td class="titolo_scheda" valign="middle" height="19"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />
											<asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label> 
											</td>
                                            <td align="left">
                                            <cc1:imagebutton id="btn_clear_fields" ImageUrl="../images/ricerca/remove_search_filter.gif"
													Runat="server" AlternateText="Pulisci i campi" ToolTip="Pulisci i campi" CssClass="clear_flag" OnClick="btnCleanUpField_Click">
                                                    </cc1:imagebutton>
                                            </td>
										</tr>
										<tr>
											<td><font size="1"><img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" /></font>
												<asp:dropdownlist id="ddl_Ric_Salvate" runat="server" AutoPostBack="True" CssClass="testo_grigio"
													Width="344px"></asp:dropdownlist></td>
											<td align="left"><asp:imagebutton id="btn_Canc_Ric" ImageUrl="../images/proto/cancella.gif" Width="19px" Height="17px"
													Runat="server" AlternateText="Rimuove la ricerca selezionata"></asp:imagebutton><font size="1"></font></td>
										</tr>
						                <tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
									</table>
								</td>
							</tr>
							<tr><td><IMG height="2" src="../images/proto/spaziatore.gif"  border="0"></td></tr>
						</asp:panel>
                        <asp:panel runat="Server" id="pnlTop">
							<tr>
								<td valign="top">
									<!-- INIZIO TABELLA Archivio documento-->
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" height="19"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />Tipo</td>
											<td>
											    <asp:CheckBoxList ID="cbl_archDoc_E" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
											    <asp:ListItem Value="A" Selected="True" runat="server" id="opArr"></asp:ListItem>
											        <asp:ListItem Value="P" Selected="True" runat="server" id="opPart"></asp:ListItem>
											        <asp:ListItem Value="I" Selected="True" runat="server" id="opInt"></asp:ListItem>
											        <asp:ListItem Value="G" Selected="True" runat="server" id="opGrigio"></asp:ListItem>
											        <asp:ListItem Value="Pr" Selected="False">Pred.</asp:ListItem>
											        <asp:ListItem Value="ALL" Selected="False" runat="server" id="opAll"></asp:ListItem>
											        <asp:ListItem Value="R" Selected="False">Stampe</asp:ListItem>
											    </asp:CheckBoxList>
										    </td>
										</tr>

                                        <!-- radiobutton per ricerca allegati -->
                                        <tr valign="top">
                                            <td valign="top">
                                        
                                            </td>
                                            <td valign="top">    
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
							<tr>
								<td valign="top">
									<!-- tabella registro-->
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="top" height="19"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />Registro</td>
                                        </tr>
                                        <tr>
											<td valign="top">
												<table cellspacing="1" cellpadding="1" border="0">
													<tr>
														<td><font size="1"><img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" /></font></td>
														<td align="left" valign="top"><asp:listbox  id="lb_reg_E" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="240px"
																Rows="3" SelectionMode="Multiple"></asp:listbox><font size="1"></font></td>
														<td valign="top">
                                                        <asp:radiobuttonlist id="rbl_Reg_E" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rbl_Reg_E_SelectedIndexChanged" CssClass="testo_grigio" Width="98px"></asp:radiobuttonlist>
                                                        <font size="1"></font></td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
							
							</asp:panel>
                        <asp:panel runat="Server" id="pnlLimitata">
							    <tr valign="top">
								    <td valign="top">
									    <!-- tabella registro-->
									    <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" height=30px>
										    <tr>
                                                <td>
                                               <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="lbl_registro" runat="server" Width="53px" CssClass="titolo_scheda">Registro</asp:label>
                                                <asp:dropdownlist id="ddl_registri" runat="server" Width="115px" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist></td>
                                            </tr>
                                            </table>
                                    </td>
                                </tr>
							<tr><td><img height="2" src="../images/proto/spaziatore.gif" border="0" /></td></tr>
                            </asp:panel>
                       <tr valign="top">
                            <td>
                                <!-- tabella Numero protocollo-->
                                <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
                                    <tr runat="Server" id="trNumProto">
                                        <td class="titolo_scheda">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Numero protocollo<asp:label runat="Server" id="star1">*&nbsp;</asp:label>
                                        </td>
                                        <td class="titolo_scheda" align="left">Anno<asp:label runat="Server" id="star">*&nbsp;</asp:label></td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" valign="middle">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:dropdownlist id="ddl_numProt_E" runat="server" autopostback="True" cssclass="testo_grigio" width="110px">
													<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
												</asp:dropdownlist>
                                            &nbsp;&nbsp;
                                            <asp:label id="lblDAnumprot_E" runat="server" cssclass="testo_grigio" width="10px">Da</asp:label>
                                            <asp:textbox id="txt_initNumProt_E" runat="server" cssclass="testo_grigio" width="60px"></asp:textbox>
                                            &nbsp;&nbsp;<asp:label id="lblAnumprot_E" runat="server" cssclass="testo_grigio" width="10px">A</asp:label>
                                            <asp:textbox id="txt_fineNumProt_E" runat="server" cssclass="testo_grigio" width="60px"></asp:textbox>
                                        </td>
                                        <td align="left"><asp:textbox id="tbAnnoProtocollo" runat="server" cssclass="testo_grigio" width="40px" maxlength="4"></asp:textbox>&nbsp;&nbsp;</td>
                                    </tr>
                                    <asp:panel runat="Server" id="pnl_dataProt">
                                    <tr>
                                        <td colspan="2">
									<!-- tabella Data protocollo-->
                                           <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data protocollo</td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_initdataProt_E" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_finedataProt_E" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
										</tr>
										<tr>
											<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:dropdownlist id="ddl_dataProt_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></td>
											<td><uc3:Calendario id="txt_initDataProt_E" runat="server" Visible="true" /></td>
											<td><uc3:Calendario id="txt_fineDataProt_E" runat="server" Visible="false" /></td>
										</tr>
									</table>
                                        </td></tr>
                                        </asp:panel>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td><img height="2" src="../images/proto/spaziatore.gif" border="0"></td>
                        </tr>
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
                         <asp:panel runat="Server" id="pnlBlank">
							<tr><td ></td></tr>
						</asp:panel>
                        <asp:panel runat="Server" id="pnlBottom">
							<tr>
								<td valign="top">
 									<table id="Table2" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
                                        <TR>
									        <TD class="titolo_scheda" vAlign="middle" height="17"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Id documento</TD>
									        <td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblDAidDoc_C" runat="server" CssClass="testo_grigio" Width="18px" Visible="False">Da</asp:label></td>
									        <td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblAidDoc_C" runat="server" CssClass="testo_grigio" Width="12px" Visible="False">A</asp:label></td>
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
								     </table>
								 </td>
						    </tr>
                        <tr>
                            <td><img height="2" src="../images/proto/spaziatore.gif" border="0"></td>
                        </tr>
						<tr>
							<td valign="top">
								<table id="Table4" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
									<tr>
										<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Creazione </td>
										<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_dataCreazioneDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
										<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_dataCreazioneA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
									</tr>
									<tr>
										<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
											</font>
											<asp:dropdownlist id="ddl_dataCreazione_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
											</asp:dropdownlist></td>
										<td><uc3:Calendario id="txt_initDataCreazione_E" runat="server" Visible="true" /></td>
										<td><uc3:Calendario id="txt_finedataCreazione_E" runat="server" /></td>
									</tr>
                                </table>
							</td>
						</tr>
                        <tr>
                            <td><img height="2" src="../images/proto/spaziatore.gif" border="0"></td>
                        </tr>
						<tr>
							<td valign="top">
								<table id="Table5" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
									<tr>
										<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Stampa </td>
										<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_dataStampaDa" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">Da</asp:label></td>
										<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_dataStampaA" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">A</asp:label></td>
									</tr>
									<tr>
										<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
											</font>
											<asp:dropdownlist id="ddl_dataStampa_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
											</asp:dropdownlist></td>
										<td><uc3:Calendario id="txt_initDataStampa_E" runat="server" Visible="true" /></td>
										<td><uc3:Calendario id="txt_finedataStampa_E" runat="server" Visible="false" /></td>
									</tr>
                                </table>
							</td>
						</tr>
						<tr>
						    <td><IMG height="2" src="../images/proto/spaziatore.gif"  border="0"></td>
						</tr>
                       <asp:panel runat="Server" id="pnl_dataScad">
						<tr>
							<td valign="top">
								<table id="Table3" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
									<tr>
										<td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Scadenza</td>
										<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_initDataScadenza_E" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
										<td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_fineDataScadenza_E" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
									</tr>
									<tr>
										<td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
											</font>
											<asp:dropdownlist id="ddl_dataScadenza_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
											</asp:dropdownlist></td>
										<td><uc3:Calendario id="txt_initDataScadenza_E" runat="server" Visible="true" /></td>
										<td><uc3:Calendario id="txt_fineDataScadenza_E" runat="server" Visible="false" /></td>
									</tr>
                                </table>
							</td>
						</tr>
                        <tr>
                            <td><img height="2" src="../images/proto/spaziatore.gif" border="0"></td>
                        </tr>
                        </asp:panel>
							<tr align="center">
							    <td><aof:AuthorOwnerFilter ID="aofAuthor" runat="server" ControlType="Author" /></td>
                            </tr>
                            <tr>
								<td height="2"></td>
							</tr>
                            <tr align="center">
                                <td><aof:AuthorOwnerFilter ID="aofOwner" runat="server" ControlType="Owner" /></td>
                            </tr>
                          
							<tr><td><IMG height="2" src="../images/proto/spaziatore.gif"  border="0" /></td></tr>
							<tr>
								<td valign="top">
									<!--tabella oggetto -->
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Oggetto</td>
											<td valign="middle" align="right" width="29"><asp:imagebutton id="btn_RubrOgget_E" ImageUrl="../images/proto/ico_oggettario.gif" Width="19px"
													Height="17px" Runat="server" AlternateText="Seleziona un oggetto nell'oggettario"></asp:imagebutton><font size="1"></font></td>
										</tr>
										<tr>
											<td colspan="2"><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
												</font>
												<asp:textbox id="txt_oggetto" runat="server" CssClass="testo_grigio" Width="370px" Height="32px"
													TextMode="MultiLine"></asp:textbox></td>
										</tr>
                                         <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
							
									</table>
								</td>
							</tr>
							<tr>
							    <td><IMG height="2" src="../images/proto/spaziatore.gif" border="0" /></td>
							</tr>
							
							<tr>
								<td valign="top">
									<!--tabella mittente/destinatario -->
									<table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" valign="middle" width="350" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Mittente/Destinatario
										        &nbsp;&nbsp;&nbsp;&nbsp;
										        <asp:CheckBox ID="chk_mitt_dest_storicizzati" runat="server" Text="Ricerca storicizzati"  Checked="true" CssClass="titolo_scheda" AutoPostBack="true" />
											</td>
											<td>
												<table>
													<tr>
														<td valign="middle" align="right" width="29"><font size="1"><IMG id="btn_Rubrica_E" style="CURSOR: hand" height="19" alt="Seleziona un mittente/destinatario nella rubrica"
																	src="../images/proto/rubrica.gif" width="29" runat="server"></font></td>
													</tr>
												</table>
												<input id="hd_systemIdMit_Est" type="hidden" size="1" name="hd_systemIdMit_Est" runat="server"><font size="1">
												</font>
											</td>
										</tr>
										<tr>
											<td colspan="2" height="25">
											    <font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></font>
												<asp:textbox id="txt_codMit_E" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="80px"></asp:textbox><font size="1">&nbsp;
												</font>
												<asp:textbox id="txt_descrMit_E" runat="server" CssClass="testo_grigio" Width="280px"></asp:textbox>
                                            </td>
										</tr>
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
                                        </td></tr>
                                        </tr>
									</table>
								</td>
							</tr>
							<tr><td><IMG height="2" src="../images/proto/spaziatore.gif"  border="0"></td></tr>
							<tr>
								<td valign="top"><!-- RICERCA PER CODICE FASCICOLO -->
									<table class="info_grigio" id="tbl_fasc_rapida" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<asp:panel id="pnl_fasc_rapida" Runat="server" Visible="True">
                                        <TBODY>
                                        <tr>
                                            <td class="titolo_scheda" valign="middle">&nbsp;&nbsp;Codice fascicolo Generale/Procedimentale</td>
				                            <td valign="middle" align="right">
					                            <cc1:imagebutton class="ImgHand" id="imgFasc" Runat="server" AlternateText="Ricerca Fascicoli" DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/proto/ico_fascicolo_noattivo.gif"></cc1:imagebutton>
                                                &nbsp;&nbsp;
				                            </td>
				                        </tr>
                                        <!-- 
                                        <td valign=middle align=right><font size=1>&nbsp;</font></td>
                                        <tr>
											<td class="testo_grigio" colSpan="2" height="15">&nbsp;&nbsp;codice fascicolo 
												&nbsp; descrizione</td>
										</tr>
									    -->
                                        <tr>
                                            <td colspan="2" height="25"><font size=1>&nbsp;</font>
                                                <asp:textbox id="txt_CodFascicolo" Width="75px" CssClass="testo_grigio" AutoPostBack="True" Runat="server" ReadOnly="False"></asp:textbox>&nbsp;
                                                <asp:textbox id="txt_DescFascicolo" Width="287px" CssClass="testo_grigio" Runat="server" ReadOnly="True"></asp:textbox><font size="1"></font>
                                            </td>
                                        </tr>
										</asp:panel>
						            </table>
						        </td>
				            </tr>
                            <!-- Ordinamento -->
                            <div>
                                <tr>
                                    <td height="3" />
                                </tr>
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
                                                </td>
                                                <td>
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
							<asp:panel id="panel_numOgg_commRef" runat="server" Width="412px" Visible="False">
							<tr>
								<td valign="top">
                                    <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
                                        <tr>
                                            <td class="titolo_scheda" valign="middle" height="18"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />Numero oggetto</td>
                                        </tr>
                                        <tr>
                                            <td><font size="1"><img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" /> </font>
                                            <asp:textbox id="txt_numOggetto" runat="server" Width="310px" CssClass="testo_grigio"></asp:textbox>
                                            </td>
                                        </tr>
                                    </table>
							    </asp:panel>
							</asp:panel>
							    </td>
			</tr>
			 <tr>
								<td valign="top">
									<!--tabella visualizza riferimenti -->
									<table cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" style="HEIGHT: 2px" valign="middle" height="2"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" /></td>
										</tr>
										<tr>
											<td class="testo_grigio"><font size="1"><img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" /></font><asp:radiobuttonlist id="rbl_Rif_E" runat="server" CssClass="testo_grigio" Height="10px" RepeatDirection="Horizontal" Visible="False">
													<asp:ListItem Value="S">Si&#160;&#160;</asp:ListItem>
													<asp:ListItem Value="N" Selected="True">No</asp:ListItem>
												</asp:radiobuttonlist></td>
										</tr>
									</table>
								</td>
							</tr>
                    
              </table>      
                    
            <tr>
                <td height="4" class="testo_grigio">
                    <font size="1">
                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" /></font></td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="btn_Ricerca" CssClass="pulsante69" runat="server" Text="Ricerca" ToolTip="Ricerca documenti protocollati"/>
                    <asp:Button ID="btn_salva" CssClass="pulsante69" runat="server" Text="Salva" ToolTip="Salva i criteri di ricerca" OnClick="btn_salva_Click" />
                    <asp:Button ID="btn_modifica" CssClass="pulsante79" runat="server" Text="Modifica" ToolTip="Modifica la ricerca salvata" OnClick="ModifyRapidSearch_Click" />
                </td>
            </tr>

        <cc2:messagebox id="mb_ConfirmDelete" style="z-index: 101; left: 576px; position: absolute;
            top: 24px" runat="server">
        </cc2:messagebox>
    </form>
</body>
</html>
