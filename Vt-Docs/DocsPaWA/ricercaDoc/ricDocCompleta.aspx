<%@ Page language="c#" Codebehind="ricDocCompleta.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.f_Ricerca_C" %>
<%@ Register TagPrefix="uc1" TagName="Creatore" Src="../UserControls/Creatore.ascx" %>
<%@ Register TagPrefix="uc2" TagName="RicercaNote" Src="../UserControls/RicercaNote.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Import Namespace="DocsPAWA.ricercaDoc" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register TagPrefix="aof" TagName="AuthorOwnerFilter" Src="~/UserControls/AuthorOwnerFilter.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>DOCSPA > ricDocCompleta</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
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
					//window.open('../documento/AnteprimaProfDinRicerche.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');
			    window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicerche.aspx&reset=yes', '', 'dialogWidth:800px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
			}	
			
			function apriPopupContatore() {
					//window.open('../documento/AnteprimaProfDinRicerche.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');
					window.showModalDialog('../popup/visualizzaContatore.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
			}		
			
		function _ApriRubrica(target)
		{
			var r = new Rubrica();
		
			switch (target) {
				case "ric_mittdest":
					r.CallType = r.CALLTYPE_RICERCA_MITTDEST;
					break;
					
				case "ric_uffref":
					r.CallType = r.CALLTYPE_RICERCA_UFFREF;
					break;	
					
				case "ric_mittintermedio":
					r.CallType = r.CALLTYPE_RICERCA_MITTINTERMEDIO;
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
		        window.document.f_Ricerca_C.txt_CodFascicolo.value = "";
		        window.document.f_Ricerca_C.txt_DescFascicolo.value = "";
		    }
		    if (rtnValue == "Y") {
		        window.document.f_Ricerca_C.submit();
		    }
		}
		
	    // Permette di inserire solamente caratteri numerici
	    function ValidateNumericKey() {
	        var inputKey = event.keyCode;
	        var returnCode = true;

	        if (inputKey > 47 && inputKey < 58) {
	            return;
	        }
	        else {
	            returnCode = false;
	            event.keyCode = 0;
	        }

	        event.returnValue = returnCode;
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
                window.document.f_Ricerca_C.txt_CodFascicolo.value = "";
                window.document.f_Ricerca_C.txt_DescFascicolo.value = "";
            }
            if (rtnValue == "Y") {
                window.document.f_Ricerca_C.submit();
            }

        }

        function apriModificaRicerca(codiceRicerca) {
            window.showModalDialog("../popup/ModificaRicerca.aspx?idRicerca=" + codiceRicerca + "", window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
        }

        function ApriFinestraMultiCorrispondenti() {
            rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M&page=ricerca&corrId=ricCompleta', '', 'dialogWidth:730px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
            window.document.f_Ricerca_C.submit();
        }

		</SCRIPT>
	    <style type="text/css">
            .style1
            {
                width: 187px;
            }
        </style>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="f_Ricerca_C" method="post" runat="server">
		    <div id="DivRicCompleta" runat="server">
			<cc2:MessageBox id="mb_ConfirmDelete" style="Z-INDEX: 101; LEFT: 576px; POSITION: absolute; TOP: 48px" runat="server"></cc2:MessageBox>
			<INPUT id="hd_systemIdUffRef" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="395" align="center" border="0">
				<tr vAlign="top">
					<td align="left">
						<table class="contenitore" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td height="2"><asp:imagebutton id="enterKeySimulator" runat="server" ImageUrl="../images/spacer.gif"></asp:imagebutton></td>
							</tr>
							<TR>
								<TD class="testo_grigio" style="vAlign="top">
									<TABLE class="info_grigio" id="Table1" style="HEIGHT: 48px" cellSpacing="0" cellPadding="0"
										width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
										<asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label> </TD>
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
                             <tr>
                            <td height="2"></td>
                            </tr>
							<tr vAlign="top">
								<td>
									<!-- archivio documento -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipo</TD>
											<td>
											    <asp:CheckBoxList ID="cbl_archDoc_C" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
											   <asp:ListItem Value="A" Selected="True" runat="server" id="opArr"></asp:ListItem>
											        <asp:ListItem Value="P" Selected="True" runat="server" id="opPart"></asp:ListItem>
											        <asp:ListItem Value="I" Selected="True" runat="server" id="opInt"></asp:ListItem>
											        <asp:ListItem Value="G" Selected="True" runat="server" id="opGrigio"></asp:ListItem>
											        <asp:ListItem Value="Pr" Selected="False">Pred.</asp:ListItem>
											        <asp:ListItem Value="ALL" Selected="False" runat="server" id="opAll"></asp:ListItem>
											        <asp:ListItem Value="R" Selected="False">Stampe</asp:ListItem>
											    </asp:CheckBoxList>
										    </td>
										</TR>
                                        
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

									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
									<!-- tabella registro-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="top" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registro</TD>
										</tr>
                                        <tr>	<TD valign="top">
												<table border="0" cellspacing="1" cellpadding="1">
													<tr>
														<td><FONT size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></FONT></td>
														<td><asp:listbox id="lb_reg_C" runat="server" Width="234px" CssClass="testo_grigio" AutoPostBack="True"
																Rows="3" SelectionMode="Multiple"></asp:listbox></td>
														<td><asp:radiobuttonlist id="rbl_Reg_C" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="78px" Height="32"></asp:radiobuttonlist></td>
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
									<!-- tabella Numero protocollo-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Numero protocollo</TD>
											<td class="titolo_scheda" align="left" height="18">Anno</td>
										</TR>
										<TR>
				                            <td class="titolo_scheda" valign="middle">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:dropdownlist id="ddl_numProt_C" runat="server" autopostback="True" cssclass="testo_grigio" width="110px">
													    <asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
													    <asp:ListItem Value="1">Intervallo</asp:ListItem>
												    </asp:dropdownlist>
                                                &nbsp;
                                                <asp:label id="lblDAnumprot_C" runat="server" cssclass="testo_grigio" width="8px">Da</asp:label>
                                                <asp:textbox id="txt_initNumProt_C" runat="server" cssclass="testo_grigio" width="60px"></asp:textbox>
                                                &nbsp;<asp:label id="lblAnumprot_C" runat="server" cssclass="testo_grigio" width="8px">A</asp:label>
                                                <asp:textbox id="txt_fineNumProt_C" runat="server" cssclass="testo_grigio" width="60px"></asp:textbox>
                                            </td>
                                            <td align="left"><asp:textbox id="tbAnnoProt" runat="server" cssclass="testo_grigio" width="40px" maxlength="4"></asp:textbox>&nbsp;&nbsp;</td>
										</TR>
										<tr>
										    <td colspan="2">
										        <table width="100%">
										            <tr>
									            <!-- tabella Data protocollo-->
											            <TD class="titolo_scheda" vAlign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">Data protocollo</TD>
											            <td class="titolo_scheda" width="100" vAlign="middle" height="18"><asp:label id="lbl_initdataProt_C" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:label></td>
											            <td class="titolo_scheda" width="100" vAlign="middle" height="18"><asp:label id="lbl_finedataProt_C" runat="server" Width="12px" CssClass="testo_grigio">A</asp:label></td>
										            </tr>
										            <tr>
											            <td><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
												            <asp:dropdownlist id="ddl_dataProt_C" runat="server" Width="110px" CssClass="testo_grigio" AutoPostBack="true">
													            <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													            <asp:ListItem Value="1">Intervallo</asp:ListItem>
													            <asp:ListItem Value="2">Oggi</asp:ListItem>
												                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
												                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												            </asp:dropdownlist></td>
											            <td><uc3:Calendario id="txt_initDataProt_C" runat="server" Visible="true" /></td>
											            <td><uc3:Calendario id="txt_fineDataProt_C" runat="server" Visible="false" /></td>
										            </tr>
										        </table>
										    </td>
										</tr>
									</TABLE>
								</td>
							</tr>
                            <tr>
								<td height="2"></td>
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
                            <td height="2"></td>
                        </tr>
                    </asp:Panel>
							<div id="ProtocolloMittente" runat="server">
							<tr>
								<td>
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
							<tr>
								<td height="2"></td>
							</tr>
						    <tr>
							    <td valign="top">
								    <table id="Table3" class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0" runat="server">
									    <tr>
										    <td class="titolo_scheda" width="140" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Scadenza</td>
										    <td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_initDataScadenza_C" runat="server" CssClass="testo_grigio" Width="10px" Visible="false">Da</asp:label></td>
										    <td class="titolo_scheda" width="100" valign="middle" height="18"><asp:label id="lbl_fineDataScadenza_C" runat="server" CssClass="testo_grigio" Width="10px" Visible="false">A</asp:label></td>
									    </tr>
									    <tr>
										    <td><font size="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
											    </font>
											    <asp:dropdownlist id="ddl_dataScadenza_C" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="110px">
												    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												    <asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
											    </asp:dropdownlist></td>
										    <td><uc3:Calendario id="txt_initDataScadenza_C" runat="server" Visible="true" /></td>
										    <td><uc3:Calendario id="txt_fineDataScadenza_C" runat="server" Visible="false" /></td>
									    </tr>
                                    </table>
							    </td>
						    </tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
								<!-- Tabella Id Documento e Data Creazione -->
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
										<tr>
											<td class="titolo_scheda" valign="middle" height="18"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data creazione</td>
											<td class="titolo_scheda" valign="middle" height="18"><asp:label id="lbl_dataCreazioneDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label></td>
											<td class="titolo_scheda" valign="middle" height="18"><asp:label id="lbl_dataCreazioneA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label></td>
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
											<td><uc3:Calendario id="txt_finedataCreazione_E" runat="server" Visible="false" /></td>
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
								<td height="2"></td>
							</tr>
							<div id="TabellaOggetto" runat="server">
							<tr>
								<td>
									<!--tabella oggetto -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" style="WIDTH: 337px" vAlign="middle" width="337" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Oggetto</TD>
											<TD vAlign="middle" align="right" width="29"><asp:imagebutton id="btn_RubrDest_P" ImageUrl="../images/proto/ico_oggettario.gif" Width="19px" Runat="server"
													AlternateText="Seleziona un oggetto nell'oggettario" Height="17px"></asp:imagebutton></TD>
										</TR>
										<TR>
											<td colSpan="2" height="25">
												<table cellSpacing="0" cellPadding="0" border="0">
													<tr vAlign="top">
														<TD style="WIDTH: 367px" colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
															<asp:textbox id="txt_ogg_C" runat="server" Width="350px" CssClass="testo_grigio" Height="40px"
																TextMode="MultiLine"></asp:textbox></TD>
													</tr>
                                                    <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
												</table>
											</td>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="MittenteDestinatario" runat="server">
							<tr>
								<td> <!--tabella mittente/destinatario -->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Mittente/Destinatario
										        &nbsp;&nbsp;&nbsp;&nbsp;
										        <asp:CheckBox ID="cb_mitt_dest_storicizzati" Checked="true" runat="server" Text="Ricerca storicizzati" CssClass="titolo_scheda"  AutoPostBack="true" />
											</td>
											<TD vAlign="middle" align="right" width="29"><IMG id="btn_Rubrica_C" runat="server" style="CURSOR: hand" height="19" alt="Seleziona un mittente/destinatario nella rubrica"
													src="../images/proto/rubrica.gif" width="29">
                                                    <INPUT class="testo_grigio" id="hd_systemIdMit_Com" type="hidden" size="3" name="hd_systemIdMit_Com"
													runat="server" height="3" width="5">
                                            </TD>
										</tr>
										<TR>
											<TD colSpan="2" height="25">
											    <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:textbox id="txt_codMitt_C" runat="server" Width="75px" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>&nbsp;
												<asp:textbox id="txt_descrMitt_C" runat="server" Width="265px" CssClass="testo_grigio"></asp:textbox>
                                            </TD>                                                
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
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<asp:Panel ID="pnl_mezzoSpedizione" runat="server" Visible="true">
							<tr>
								<td>
									<!-- tabella Mezzo Spedizione-->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										    <tr><td height="2px" colspan="2"></td></tr>
										<tr>
										    <td class="titolo_scheda">&nbsp;&nbsp;Mezzo di spedizione</td>
										    <td align="left" class="titolo_scheda"><asp:DropDownList ID="ddl_spedizione" runat="server" CssClass="testo_grigio"></asp:DropDownList></td>
										</tr>
										    <tr><td height="2px" colspan="2"></td></tr>
					                </table>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							</asp:Panel>
							<asp:Panel id=pnl_cons runat=server>
							<tr>
								<td>
									<!-- tabella Conservazione-->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										    <tr><td height="2px" colspan="2"></td></tr>
										<tr>
										    <td class="titolo_scheda">
										        <asp:CheckBox ID="cb_Conservato" AutoPostBack="true" runat="server" />&nbsp;Conservato
										        &nbsp;&nbsp;
										        <asp:CheckBox ID="cb_NonConservato" AutoPostBack="true" runat="server" />&nbsp;Mai Conservato
							                </td>
										</tr>
										    <tr><td height="2px" colspan="2"></td></tr>
					                </table>
								</td>
							</tr>
							</asp:Panel>
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
							<tr>
								<td height="2"></td>
							</tr>
							<div id="AnnullamentoDoc" runat="server">
							<tr>
								<td>
									<!--tabella annullamento documento-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Stato 
												del documento</TD>
										</TR>
										<TR>
											<TD height="25"><asp:radiobuttonlist id="rb_annulla_C" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
													<asp:ListItem Value="1">Annullato</asp:ListItem>
													<asp:ListItem Value="0">Non Annullato</asp:ListItem>
													<asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
												</asp:radiobuttonlist></TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<asp:Panel id=pnl_trasfDesp runat=server>
                            <tr>
								<td>
									<!-- Trasferimento in deposito -->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
								        <TR>
									        <TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Archivio deposito</TD>
									    </TR>
									    <TR>
										    <TD height="25">
										        <asp:radiobuttonlist id="rbl_TrasfDep" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
												    <asp:ListItem Value="C">Corrente</asp:ListItem>
												    <asp:ListItem Value="D">In Deposito</asp:ListItem>
												    <asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
											    </asp:radiobuttonlist>
										    </TD>
									    </TR>
									</table>
								</td>
							</tr>
							</asp:Panel>
							<tr>
							   <td height="2"></td>
							</tr>
							<div id="RicercaSegn" runat="server">
							<tr>
								<td>
									<!-- ricerca segnatura -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Segnatura&nbsp;</TD>
										</TR>
										<TR>
											<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"><asp:textbox id="txt_segnatura" Width="348px" CssClass="testo_grigio" Runat="server"></asp:textbox>
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>                         
							<!-- Ricerca per Tipologia documento -->
							<tr>
								<td height="2"></td>
							</tr>
							<div id="TipologiaDoc" runat="server">
							<tr>
								<td>
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19" width="35%" style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipologia 
												documento</TD>
											<td style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px" class="style1">
												<asp:dropdownlist id="ddl_tipoDoc_C" runat="server" Width="162px" 
                                                    CssClass="testo_grigio" AutoPostBack="True" Height="16px"></asp:dropdownlist></td>
											<td style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><asp:imagebutton id="btn_CampiPersonalizzati" runat="server" ImageUrl="../images/proto/ico_oggettario.gif"></asp:imagebutton>
                                                </td>
											<td style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>
										</TR>
										<asp:Panel ID="Panel_StatiDocumento" Runat="server" Visible="false">
											<TR>
												<TD class="titolo_scheda" style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px" vAlign="middle"
													height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Stato 
													documento</TD>
												<TD style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px" colspan="2">
                                                    <asp:DropDownList ID="ddlStateCondition" runat="server" CssClass="testo_grigio" Width="90px">
                                                        <asp:ListItem Value="Equals">Uguale a</asp:ListItem>
                                                        <asp:ListItem Value="Unequals">Diverso da</asp:ListItem>
                                                    </asp:DropDownList>
													<asp:DropDownList id="ddl_statiDoc" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="140px"></asp:DropDownList></TD>
												<TD style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></TD>
											</TR>
										</asp:Panel>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="UfficioReferente" runat="server">
							<asp:panel id="pnl_uffRef" Runat="server" Visible="False">
								<TR>
									<TD><!--tabella ufficio referente -->
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ufficio 
													Referente</TD>
												<TD vAlign="middle" align="right" width="29"><IMG id="btn_Rubrica_ref" style="CURSOR: hand" height="19" alt="Seleziona un Ufficio Referente dalla rubrica"
														src="../images/proto/rubrica.gif" width="29" runat="server"></TD>
											</TR>
											<TR>
												<TD colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
													<asp:textbox id="txt_codUffRef" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="75px"></asp:textbox>&nbsp;
													<asp:textbox id="txt_descUffRef" runat="server" CssClass="testo_grigio" Width="265px" ReadOnly="True"></asp:textbox></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
							    <tr>
								       <td height="2"></td>
							    </tr>
							</asp:panel>
							</div>
							<div id="MittenteIntermedio" runat="server">
							<tr>
								<td>
									<!--tabella mittente intermedio -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Mittente 
												intermedio</TD>
											<TD vAlign="middle" align="right" width="29"><asp:imagebutton id="btn_rubricaMitInt_C" ImageUrl="../images/proto/rubrica.gif" Width="29" Runat="server"
													AlternateText="Seleziona un mittente intermedio nella rubrica" Height="19"></asp:imagebutton></TD>
										</TR>
										<TR>
											<TD colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:textbox id="txt_codMittInter_C" runat="server" Width="75px" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>&nbsp;
												<asp:textbox id="txt_descrMittInter_C" runat="server" Width="265px" CssClass="testo_grigio"></asp:textbox></TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="CodiceFascicolo" runat="server">
							<TR>
								<TD><!-- RICERCA PER CODICE FASCICOLO -->
									<TABLE class="info_grigio" id="tbl_fasc_rapida" cellSpacing="0" cellPadding="0" width="97%"
										align="center" border="0">
										<asp:panel id="pnl_fasc_rapida" Runat="server" Visible="True">
											<TBODY>
												<TR>
													<TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Codice fascicolo Generale/Procedimentale</TD>
											        <TD vAlign="middle" align="right">
											            <cc1:imagebutton class="ImgHand" id="imgFasc" Runat="server" AlternateText="Ricerca Fascicoli" DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/proto/ico_fascicolo_noattivo.gif"></cc1:imagebutton>
                                                &nbsp;&nbsp;
													</TD>
													<!-- <TD vAlign="middle" align="right">&nbsp;</TD> -->
												</TR> <!--<TR>
												<TD class="testo_grigio" colSpan="2" height="15">&nbsp;&nbsp;codice fascicolo 
													&nbsp; descrizione</TD>
											</TR>-->
												<TR>
													<TD colSpan="2" height="25">&nbsp;
														<asp:textbox id="txt_CodFascicolo" CssClass="testo_grigio" AutoPostBack="True" Width="75px" Runat="server"
															ReadOnly="False"></asp:textbox>&nbsp;
														<asp:textbox id="txt_DescFascicolo" CssClass="testo_grigio" Width="273px" Runat="server" ReadOnly="True"></asp:textbox>
													</TD>
												</TR>
										</asp:panel>
									</TABLE>
								</TD>
							</TR>
							</div>
							
							
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td>
									<!-- tabella Data protocollo mittente-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" width="140" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data prot. mittente</TD>
											<td class="titolo_scheda" width="100" vAlign="middle" height="19"><asp:label id="lbl_initdataProtMitt_C" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:label></td>
											<td class="titolo_scheda" width="100" vAlign="middle" height="19"><asp:label id="lbl_finedataProtMitt_C" runat="server" Width="18px" CssClass="testo_grigio">A</asp:label></td>
										</TR>
										<TR>
											<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:dropdownlist id="ddl_dataProtMitt_C" runat="server" Width="110px" CssClass="testo_grigio" AutoPostBack="True">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></TD>
											<td height="25"><uc3:Calendario id="txt_initDataProtMitt_C" runat="server" Visible="true" /></td>
											<td height="25"><uc3:Calendario id="txt_fineDataProtMitt_C" runat="server" Visible="false" /></td>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="DataArrivo" runat="server">
							<tr>
								<td>
									<!-- tabella Data arrivo-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" width="140" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data arrivo</TD>
											<td class="titolo_scheda" width="100" vAlign="middle" height="19"><asp:label id="lbl_initDataArrivo_C" runat="server" Width="18px" CssClass="testo_grigio" Visible="False">Da</asp:label></td>
											<td class="titolo_scheda" width="100" vAlign="middle" height="19"><asp:label id="lbl_fineDataArrivo_C" runat="server" Width="18px" CssClass="testo_grigio" Visible="False">A</asp:label></td>
										</TR>
										<TR>
											<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:dropdownlist id="ddl_dataArrivo_C" runat="server" Width="110px" CssClass="testo_grigio" AutoPostBack="True">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></TD>
											<td height="25"><uc3:Calendario id="txt_initDataArrivo_C" runat="server" Visible="true" /></td>
											<td height="25"><uc3:Calendario id="txt_fineDataArrivo_C" runat="server" Visible="false" /></td>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="ParoleChiave" runat="server">
							<tr>
								<td>
									<!-- INIZIO TABELLA PAROLE CHIAVE -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Parole 
												chiave</TD>
											<TD><asp:imagebutton id="btn_selezionaParoleChiave" ImageUrl="../images/proto/ico_parole.gif" Width="19px"
													Runat="server" AlternateText="Seleziona parole chiave" Height="17px"></asp:imagebutton></TD>
										</TR>
										<TR>
											<TD colSpan="2" height="25">
												<table cellSpacing="0" cellPadding="0" border="0">
													<tr vAlign="top">
														<td>&nbsp;&nbsp;<asp:listbox id="ListParoleChiave" runat="server" Width="350px" CssClass="testo_grigio" Rows="5"
																Height="44px"></asp:listbox></td>
														<td></td>
													</tr>
												</table>
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="NumeroOggetto" runat="server">
							<asp:panel id="panel_numOgg_commRef" runat="server" Width="381px" Visible="False">
								<TR>
									<TD>
										<TABLE cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD>
													<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center"
														border="0">
														<TR>
															<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Numero 
																oggetto</TD>
														</TR>
														<TR>
															<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
																<asp:textbox id="txt_numOggetto" CssClass="testo_grigio" Width="173px" Runat="server"></asp:textbox>&nbsp;
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD height="5"></TD>
											</TR>
											<TR>
												<TD>
													<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center"
														border="0">
														<TR>
															<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Commissione 
																referente</TD>
														</TR>
														<TR>
															<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
																<asp:textbox id="txt_commRef" CssClass="testo_grigio" Width="268px" Runat="server"></asp:textbox>&nbsp;
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
							    <tr>
								   <td height="2"></td>
							    </tr>
							</asp:panel>
								</div>
							<div id="divNote" runat="server">
							<tr>
								<td>
									<!--tabella NOTE -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Note 
											</TD>
										</TR>
										<TR>
											<TD colSpan="2" height="25">
												<uc2:RicercaNote ID="rn_note" runat="server" />
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							</div>
							<tr>
								<td height="2"></td>
							</tr>
							<div id="DocCompletamento" runat="server">
	                        <tr vAlign="top">
								<td class="testo_grigio" vAlign="top">
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Documenti in completamento</td>
										</tr>
										<tr>
											<td class="testo_grigio">
												<asp:CheckBoxList ID="cbl_docInCompl" runat="server" CssClass="testo_grigio" RepeatColumns="2" RepeatDirection="Horizontal">
													<asp:ListItem Value="C_Img">Con Immagine</asp:ListItem>
													<asp:ListItem Value="S_Img">Senza Immagine</asp:ListItem>
													<asp:ListItem Value="C_Fasc">Con Fascicolazione&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
													<asp:ListItem Value="S_Fasc">Senza Fascicolazione</asp:ListItem>
												</asp:CheckBoxList>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
								<td class="testo_grigio" vAlign="top">
									<!-- tabella registro-->
									<table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
                                        <tr>
											<td class="testo_grigio">
											    <table width="100%" border="0"><tr>
                   					            <td width="50%"><asp:CheckBox ID="cbx_Trasm" runat="server" AutoPostBack="True" Text="Trasmesse con" CssClass="testo_grigio"/></td>
					                            <td width="50%"><asp:CheckBox ID="cbx_TrasmSenza" runat="server" AutoPostBack="True" Text="Trasmesse senza" CssClass="testo_grigio"/></td>
					                            </tr></table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="testo_grigio">
                                                 <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ragione:&nbsp;
                                                 <asp:DropDownList ID="ddl_ragioneTrasm" runat="server" CssClass="testo_grigio"></asp:DropDownList>
                                            </td> 
										</tr>
										<tr><td height="2"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td></tr>
									</table>
								</td>
							</tr>
							</div>
							    <tr>
								  <td height="2"></td>
							    </tr>
							    <div id="ProtocolloEmergenza" runat="server">							
							<asp:panel id="pnl_protoEme" Runat="server" Visible="True">
								<TR>
									<TD><!-- protocollo emergenza --> <!-- tabella Data protocollo mittente-->
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Segnatura 
													di emergenza</TD>
											</TR>
											<TR>
												<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
													<asp:textbox id="txt_protoEme" runat="server" CssClass="testo_grigio" Width="348px"></asp:textbox></TD>
											</TR>
											<TR>
												<TD height="5"></TD>
											</TR>
					        <tr>
								<td>
									<!-- tabella Data arrivo-->
									<TABLE cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" width="140" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data segn. emergenza</TD>
											<td class="titolo_scheda" width="100" vAlign="middle" height="19"><asp:label id="lbl_dataProtoEmeInizio" runat="server" Width="18px" CssClass="testo_grigio" Visible="False">Da</asp:label></td>
											<td class="titolo_scheda" width="100" vAlign="middle" height="19"><asp:label id="lbl_dataProtoEmeFine" runat="server" Width="18px" CssClass="testo_grigio" Visible="False">A</asp:label></td>
										</TR>
										<TR>
											<TD height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:dropdownlist id="ddl_dataProtoEme" runat="server" Width="110px" CssClass="testo_grigio" AutoPostBack="True">
													<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
										            <asp:ListItem Value="2">Oggi</asp:ListItem>
									                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
									                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
												</asp:dropdownlist></TD>
											<td height="25"><uc3:Calendario id="txt_dataProtoEmeInizio" runat="server" Visible="true" /></td>
											<td height="25"><uc3:Calendario id="txt_dataProtoEmeFine" runat="server" Visible="false" /></td>
										</TR>
									</TABLE>
								</td>
							</tr>
 
										</TABLE>
									</TD>
								</TR>
							    <tr>
								    <td height="2"></td>
							    </tr>
							</asp:panel>
                               </div>
							<!--Firmatario  -->
							<div id="Firmatario" runat="server">
							<asp:Panel ID="pnl" Runat="server" Visible="False">
								<TR>
									<TD>
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Firmatario</TD>
											</TR>
											<TR>
												<TD height="25">
													<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
														<TR>
															<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
																<asp:label id="lbl_nomeF" CssClass="testo_grigio" Runat="server">Nome</asp:label></TD>
															<TD>
																<asp:textbox id="txt_nomeFirma_C" CssClass="testo_grigio" Width="120px" Runat="server"></asp:textbox>&nbsp;
															</TD>
															<TD>
																<asp:label id="lbl_cognomeF" CssClass="testo_grigio" Runat="server">Cognome</asp:label></TD>
															<TD>
																<asp:textbox id="txt_cognomeFirma_C" CssClass="testo_grigio" Width="120px" Runat="server"></asp:textbox></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
							    <tr>
								    <td height="2"></td>
							    </tr>
							</asp:Panel>
								</div>
							<tr>
								<td>
									<!--tabella visualizza riferimenti -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<!--<TR>


											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Visualizza 
												riferimenti</TD>
										</TR>-->
										<TR>
											<!--<TD height="25"><asp:radiobuttonlist id="rbl_Rif_C" runat="server" RepeatDirection="Horizontal" CssClass="testo_grigio">
													<asp:ListItem Value="S">Si&nbsp;&nbsp;</asp:ListItem>
													<asp:ListItem Value="N" Selected="True">No</asp:ListItem>
												</asp:radiobuttonlist><INPUT id="hd_systemIdMit" type="hidden" size="1" name="hd_systemIdMit" runat="server">
												<INPUT id="hd_systemIdMitInt" type="hidden" size="1" name="hd_systemIdMitInt" runat="server"></TD>--></TR>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td>
									<!--tabella in EVIDENZA: priorità del documento nell'invio delle mail-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Evidenza&nbsp;</TD>
										</TR>
										<TR>
											<TD height="25"><asp:radiobuttonlist id="rb_evidenza_C" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
													<asp:ListItem Value="1">Sì&nbsp;&nbsp;</asp:ListItem>
													<asp:ListItem Value="0">No&nbsp;&nbsp;</asp:ListItem>
													<asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
												</asp:radiobuttonlist></TD>
										</TR>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="2"></td>
							</tr>
							<tr>
							    <td>
							        <table class="info_grigio" cellspacing="0" cellPadding="0" width="97%" align="center" border="0">
							            <tr>
							                <td class="titolo_scheda" valign="middle" style="height:19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipo file acquisito&nbsp;</td>
							                <%--<td class="titolo_scheda"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Firmato&nbsp;</td>
							                <td class="titolo_scheda"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Non firmato&nbsp;</td>--%>
							            </tr>
							            <tr>
							                <td><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
							                 <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="testo_grigio" Width="150">
							                <asp:ListItem></asp:ListItem>
							                </asp:DropDownList>
							                </td>
							                <td>
							                    <asp:CheckBox ID="chkFirmato" runat="server" CssClass="testo_grigio"  Text="Firmati" onclick="SingleSelect('chk',this);"/>
							                </td>
							                <td>
							                    <asp:CheckBox ID="chkNonFirmato" runat="server" CssClass="testo_grigio" Text="Non firmati" onclick="SingleSelect('chk',this);" />
							                </td>
							            </tr>
							        </table>
							    </td>
							</tr>
							    <tr>
								  <td height="2"></td>
							    </tr>
							<asp:Panel ID="pnl_ProtocolloTitolario" runat="server" Visible="false">
							<tr>
							    <td>
							        <table class="info_grigio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" vAlign="middle" height="19">
											    <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
											    <asp:Label ID="lbl_ProtocolloTitolario" runat="server"></asp:Label>
											</td>
										</tr>
										<tr>
											<td height="25">
											<IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
											<asp:DropDownList ID="ddl_Titolario" CssClass="testo_grigio" runat="server" Width="70%"></asp:DropDownList>
											<b>/</b>
											<asp:TextBox id="txt_ProtocolloTitolario" CssClass="testo_grigio" runat="server" Width="20%" MaxLength="6"></asp:TextBox>
											</td>
										</tr>
									</table>
							    </td>
							</tr>
							<tr>
							   <td height="2"></td>
						    </tr>
							</asp:Panel> 
                                                                <!--VERSIONI-->
                                <tr>
								    <td>
									    <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										    <tr>
											    <td class="titolo_scheda"  valign="middle" height="19" colspan="2" style="padding-bottom: 5px; padding-top: 5px; padding-left:10px">
                                                    Versioni
                                                </td>
                                            </tr>
                                            <tr>
										        <td style="padding-bottom: 5px; padding-top: 5px; padding-left:10px" class="style1" >
                                                    <asp:label id="Label1" runat="server" cssclass="testo_grigio" Width="250">Numero di versioni del documento: </asp:label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddl_op_versioni" CssClass="testo_grigio" runat="server">
                                                      <asp:ListItem Text="<" Value="<" />
                                                      <asp:ListItem Text="=" Value="=" />
                                                      <asp:ListItem Text=">" Value=">" />
                                                    </asp:DropDownList>
                                                    <asp:TextBox ID="txt_versioni" runat="server" Columns="3" CssClass="testo_grigio" />
                                                </td>
									        </tr>
                                        </table>
								    </td>
							    </tr>
                                <tr>
                                <td height="2"></td>
                                </tr>

                                <!-- ALLEGATI -->
                                <tr>
								    <td>
									    <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										    <tr>
											    <td class="titolo_scheda"  valign="middle" height="19" colspan="2" style="padding-bottom: 5px; padding-top: 5px; padding-left:10px">
                                                    Allegati
                                                </td>
                                            </tr>
                                            <tr>
										        <td style="padding-bottom: 5px; padding-top: 5px; padding-left:10px" class="style1" >
                                                    <asp:label id="num_allegati" runat="server" cssclass="testo_grigio" Width="200">Numero di allegati al documento: </asp:label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddl_op_allegati" CssClass="testo_grigio" runat="server">
                                                      <asp:ListItem Text="<" Value="<" />
                                                      <asp:ListItem Text="=" Value="=" />
                                                      <asp:ListItem Text=">" Value=">" />
                                                    </asp:DropDownList>
                                                    <asp:TextBox ID="txt_allegati" runat="server" Columns="3" CssClass="testo_grigio" />
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="rblFiltriNumAllegati" runat="server" CssClass="testo_grigio">
                                                        <asp:ListItem Value="tutti" Text="Tutti" Selected="True" />
                                                        <asp:ListItem Value="pec" Text="PEC" />
                                                        <asp:ListItem Value="user" Text="Utente"/>
                                                        <asp:ListItem Value="esterni" Text="Sist. esterni"/>
                                                    </asp:RadioButtonList>
                                                </td>
									        </tr>
                                        </table>
								    </td>
							    </tr>
                                <tr>
                                <td height="2"></td>
                                </tr>
                                            <!-- Ordinamento -->
							    <tr>
								    <td>
									    <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
										    <tr>
											    <td class="titolo_scheda" colspan="2" valign="middle" height="19" style="padding-bottom: 5px; padding-top: 5px; padding-left:10px">
                                                    Ordinamento
                                                </td>
                                            </tr>
                                            <tr>
										        <td style="padding-bottom: 5px; padding-top: 5px; padding-left:10px" class="style1">
                                                    <asp:DropDownList ID="ddlOrder" runat="server" CssClass="testo_grigio" Width="270" />
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
                                <tr>
                                <td height="2"></td>
                                </tr>
                                <!--Visiblità Documenti-->
                                <asp:Panel ID="pnl_visiblitaDoc" runat="server" Visible="false">
                                <tr>
                                    <td>
                                        <table class="info_grigio" cellspacing="0" cellpadding="0" width="97%" align="center" border="0">
                                            <tr>
                                                <td class="titolo_scheda" style="padding-left:10px">
                                                    Visibilità :
                                                </td>
                                                <td>                                                    
                                                    <asp:RadioButtonList id="rbl_visibilita" runat="server" RepeatDirection="Horizontal" CssClass="testo_grigio" Width="90%">
                                                        <asp:ListItem Text="Tipica e Atipica" Value="T_A" Selected="True"/>
                                                        <asp:ListItem Text="Tipica" Value="T" />
                                                        <asp:ListItem Text="Atipica" Value="A" />                                                                                        
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                </asp:Panel>                                
                                <tr>
                                   <td height="2"></td>
                                </tr>							   							    
						</table>
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
								<TD><asp:Button ID="btn_ricerca" CssClass="pulsante69" Text="Ricerca" runat="server" ToolTip="Ricerca documenti protocollati" />
								<asp:Button ID="btn_salva" CssClass="pulsante69" Text="Salva" runat="server" ToolTip="Salva i criteri di ricerca" />
                                 <asp:Button ID="btn_modifica" CssClass="pulsante79" runat="server" Text="Modifica" ToolTip="Modifica la ricerca salvata" OnClick="ModifyRapidSearch_Click" />
                                 </TD>
							</TR>
							<!--TR>
								<TD width="100%" bgColor="#810d06"><IMG height="2" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
							</TR--></TABLE>
					</td>
				</tr>
			</div>	
		</form>
	</body>
</HTML>
