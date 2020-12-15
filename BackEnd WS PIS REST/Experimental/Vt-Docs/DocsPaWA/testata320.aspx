<%@ Page Language="c#" CodeBehind="testata320.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.testata320" %>

<%@ Register TagPrefix="uc1" TagName="NavigationContext" Src="SiteNavigation/NavigationContext.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ScrollElementsList" Src="UserControls/ScrollElementsList/ScrollElementsList.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="docsPaMenu" Assembly="DocsPaMenu" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="HEAD1" runat="server">
    <title>DOCSPA</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
   
    <meta http-equiv="Expires" content="-1">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    

    <script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript" src="LIBRERIE/rubrica.js"></script>

    <link href="CSS/docspa.css" type="text/css" rel="stylesheet">

    <script src="LIBRERIE/NotificationCenterScript.js" language="javascript" type="text/javascript" ></script>

    
    
    <%--<script language="javascript" type="text/javascript">
        function HandleClose() {
            alert('x:' + event.clientX.toString());
            alert('body:' + document.body.clientWidth.toString());
            alert('y:' + event.clientY.toString());
            if (event.clientY < 0 && event.clientX >= document.body.clientWidth - 25) 
            {
                alert('entrato');
                PageMethods.AbandonSession();
            }
        }
    </script>--%>
   
    <script language="JavaScript" type="text/javascript">
		
			ns=window.navigator.appName == "Netscape"
			ie=window.navigator.appName == "Microsoft Internet Explorer"
			var IsPostBack;

			function openIt(x) 
			{
				try
				{
					if(ns) 
					{
						showbox= document.layers[x+1]
						showbox.visibility = "show";
						showbox.top = 53;
						var items = 6;
						for (i=1; i<=items; i++) 
						{
							elopen=document.layers[i]
							if (i != (x + 1)) 
							{ 
								elopen.visibility = "hide" 
							}
						}
					}    
					if(ie) 
					{
						curEl = event.toElement						
						showBox = document.all.box[x];
						showBox.style.visibility = "visible";
						showBox.style.top = 53;
						var items = 6;
						for (i=0; i<items; i++) 
						{
							elOpen=document.all.box[i]
							barEl=document.getElementsByName('mnubar'+i.toString());
							if (i != x)
							{ 
								elOpen.style.visibility = "hidden" 
							}
						}
					}
				}
				catch(e)
				{
					//alert(e.message);
				}
			}

			function closeIt() 
			{
				return;			
			}
					
			function GestioneRubrica()
			{
				r = new Rubrica();
				r.CallType = r.CALLTYPE_MANAGE;
				r.Apri();
			}
			
			function prospettiRiepilogativi()
			{
				var pageAddress='ProspettiRiepilogativi/Frontend/gestioneProspetti.aspx';
				var newUrl=pageAddress;
				var _width = (screen.width-10);
				var _height = (screen.height-90);
				window.open(newUrl, 'principale', 'top=0,left=0,width=' + _width + ',height=' + _height + ',fullscreen=no,toolbar=no,directories=no,status=yes,menubar=no,resizable=yes, scrollbars=auto');								
			}
			
			function ModelliTrasmissione()
			{
				var pageAddress='popup/GestioneModelliTrasm_frame.aspx';
				var newUrl=pageAddress;
				var _width = (screen.width-10);
				var _height = (screen.height-10); 	
							
				window.showModalDialog(newUrl,'','dialogWidth:' + _width + 'px;dialogHeight:' + _height + 'px;fullscreen:no;toolbar:no;status:yes;resizable:yes;scroll:auto;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}
			
			function Visibilita()
			{
				var pageAddress='popup/VisibilitaFrame.aspx';
				var newUrl=pageAddress;
				var _width = 600;
				var _height = 600; 	
							
				window.showModalDialog(newUrl,'','dialogWidth:' + _width + 'px;dialogHeight:' + _height + 'px;fullscreen:no;toolbar:no;status:no;resizable:yes;scroll:auto;dialogLeft:420;dialogTop:105;center:yes;help:no;close:no');
			}

			function OpenNews(pageAddress)
			{
				var newUrl='popup/NewsFrame.aspx?pagina=' + pageAddress;
				var _width = (screen.width-10);
				var _height = (screen.height-90); 
				window.open(newUrl,'popNews','top=0,left=0,width='+_width+',height='+_height+',fullscreen=no,toolbar=no,directories=no,status=yes,menubar=no,resizable=yes, scrollbars=auto');
			}

			//Codici IPA
			function OpenCodici_IPA(pageAddress) {
			    window.open(pageAddress);
			}

			function OpenFascicolazioneCartacea() 
			{	
                var pageAddress='FascicolazioneCartacea/GestioneAllineamento.aspx';
				var newUrl=pageAddress;
				var _width = (screen.width-10);
				var _height = (screen.height-90); 
				window.open(newUrl,'principale','top=0,left=0,width='+_width+',height='+_height+',fullscreen=no,toolbar=no,directories=no,status=yes,menubar=no,resizable=yes, scrollbars=auto');

//				var pageHeight=window.screen.availHeight;
//				var pageWidth=window.screen.availWidth;
//				
//				window.showModalDialog('FascicolazioneCartacea/GestioneAllineamento.aspx',
//								'',
//								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}		
			
			function OpenDeleghe()
			{		
				var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('Deleghe/GestioneDeleghe.aspx','',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}	
			
			function OpenElencoNote()
			{
			    var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('Note/ElencoNote.aspx','',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}
			
			function OpenProtocollazioneIngresso() 
			{		
				var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('ProtocollazioneIngresso/Protocollazione.aspx?proto=A',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}		
			
			function OpenProtocollazioneUscita() 
			{		
				var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('ProtocollazioneIngresso/Protocollazione.aspx?proto=P',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}	
			
			function OpenDocInCestino()
			{
			    var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('popup/DocInCestino.aspx',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			
			}
			
			function OpenADC()
			{
			    var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('popup/areaConservazione.aspx',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			
			}
			
			function StampaOrganigramma() 
			{		
				var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('organigramma/Stampa_Organigramma.aspx',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;');										
			}
			
			function OpenCambiaPassword() 
			{		
				var pageHeight= 230;
				var pageWidth = 410;
				
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				var newwin = window.showModalDialog('popup/cambiaPassword.aspx',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:no;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
			}							

			function OpenHelp(from) 
			{		
				var pageHeight= 700;
				var pageWidth = 950;
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				var newwin = window.showModalDialog('Help/ManualeFrame.htm?from=' + from,
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
								
				//newwin.location.hash=from;
				//return false;
			}							
			
			// Apertura finestra importazione documenti
			function OpenImportDocuments() {
			    var pageHeight = window.screen.availHeight;
			    var pageWidth = window.screen.availWidth;
			    var dialogArgs = new Object();
			    dialogArgs.window = window;
			    window.showModalDialog('Import/Documenti/ImportDocumenti.aspx',dialogArgs,
                                'dialogWidth:800px;dialogHeight:470px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
			    //'dialogWidth:690px;dialogHeight:430px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');

			}

			// Apertura stampa unione
			function OpenStampaUnione() {
			    var pageHeight = window.screen.availHeight;
			    var pageWidth = window.screen.availWidth;
			    var dialogArgs = new Object();
			    dialogArgs.window = window;
			    window.showModalDialog('Import/Documenti/ImportDocumenti.aspx?stampaUnione=true',dialogArgs,
                                'dialogWidth:800px;dialogHeight:470px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
			    //'dialogWidth:690px;dialogHeight:430px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');

			}
			
			// Apertura importazione fascicoli
			function OpenImportProjects() {
    			 var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('Import/Fascicoli/ImportFascicoli.aspx','',
								'dialogWidth:690px;dialogHeight:430px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
			}
			
			// Apertura importazione RDE
			function OpenImportRDE() {
    			 var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('Import/RDE/ImportRDE.aspx','',
								'dialogWidth:800px;dialogHeight:470px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
                                //'dialogWidth:690px;dialogHeight:430px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
}
                    // Apertura finestra importazione documenti pregressi
function OpenImportDocumentsPregressi() {
    var pageHeight = window.screen.availHeight;
    var pageWidth = window.screen.availWidth;
    var dialogArgs = new Object();
    dialogArgs.window = window;
    window.showModalDialog('Import/Documenti/ImportDocumentiPregressi.aspx', dialogArgs,
                                'dialogWidth:850px;dialogHeight:650px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
    //'dialogWidth:690px;dialogHeight:430px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
}
		</script>
	</head>
	<body language="javascript" onload="showClock();">
    
		<form id="testata" method="post" runat="server">
        
			<TABLE id="mainmenu" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR>
					<TD id="mnubar0" width="133" height="38"><asp:imagebutton id="img_logo" runat="server" ImageAlign="AbsMiddle" ImageUrl="images/testata/320/log_docspa4.gif"
							width="133" height="38" AlternateText="Torna alla pagina iniziale" BorderWidth="0"></asp:imagebutton></TD>
					<TD width="100%" height="38">
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td id="backgroundLogo" runat="server" width="455" height="38"><asp:image id="logoEnte" height="38" ImageUrl="images/testata/320/gestdocum.gif" width="455" border="0" runat="server" /></td>
								<td runat="server" id="backgroundLogoEnte" vAlign="top" align="right" width="100%" 
									height="38"><asp:label id="lbl_info_utente" CssClass="testo_grigio_bold2" Runat="server"></asp:label></td>
							</tr>
						</table>
					</TD>
				</TR>
				<TR>
					<asp:TableCell id="td_A" Height="15" runat="server" SkinID="pulsantiera"></asp:TableCell>
					<TD height="15">
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<!-- MENU_DOCUMENTI -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD id="mnubar1" width="91" runat="server"><cc1:imagebutton id="btn_doc" runat="server" Thema="btn_" SkinID="documenti320" AlternateText="Documenti"
										Tipologia="MENU_DOCUMENTI"></cc1:imagebutton></TD>
								<!-- MENU-RICERCA -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD id="mnubar2" runat="server"><cc1:imagebutton id="btn_search" runat="server" Thema="btn_" SkinID="ricerca320"
										AlternateText="Ricerca" Tipologia="MENU-RICERCA" borderWidth="0"></cc1:imagebutton></TD>
								<!-- MENU_GESTIONE -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD id="mnubar3" width="91" runat="server"><cc1:imagebutton id="btn_gest" runat="server" Thema="btn_" SkinID="gestione320" AlternateText="Gestione"
										Tipologia="MENU_GESTIONE"></cc1:imagebutton></TD>
								<!-- MENU_OPZIONI -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD id="mnubar4" runat="server"><cc1:imagebutton id="btn_config" runat="server" Thema="btn_" SkinID="opzioni320"
										AlternateText="Opzioni" Tipologia="MENU_OPZIONI" borderWidth="0" Enabled="False"></cc1:imagebutton></TD>
                                <!-- NOTIFICATION_CENTER -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD id="TD1" runat="server"><cc1:imagebutton id="imgNotificationCenter" runat="server" Thema="btn_" SkinID="notifiche320"
										AlternateText="Centro notifiche" Tipologia="" OnClientClick="openSearchWindow();" borderWidth="0" Enabled="False"></cc1:imagebutton></TD>
								<!-- MENU_HELP -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD id="mnubar5" runat="server"><cc1:imagebutton id="btn_help" runat="server" Thema="btn_" SkinID="aiuto320"
										AlternateText="Aiuto" Tipologia="MENU_HELP" borderWidth="0" Enabled="False"></cc1:imagebutton></TD>
								<!-- MENU_LOGOUT -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<TD><cc1:imagebutton id="btn_logout" runat="server" Thema="btn_" SkinID="esci" AlternateText="Esce dall'applicazione"
										Tipologia="MENU_LOGOUT" borderWidth="0"></cc1:imagebutton></TD>
								<!-- spazio fino alla fine -->
								<TD width="1" height="15"><IMG height="15" src="images/spaziatore.gif" width="1" border="0"></TD>
								<asp:TableCell Width="100%" Height="15" CssClass="testo_bianco" HorizontalAlign="Right" ID="td_B" runat="server" SkinID="pulsantiera">
									<SCRIPT language="JavaScript">
									<!--
										function showClock()
										{
											var Digital = new Date();
											var ore = Digital.getHours();
											var minuti = Digital.getMinutes();
											var secondi = Digital.getSeconds();
											if (ore<=9) ore = "0" + ore;
											if (minuti<=9) minuti = "0" + minuti;
											if (secondi<=9) secondi = "0" + secondi;
											myclock = ore + ":" + minuti + ":" + secondi;
											if (document.all) { liveclock.innerHTML = myclock; }
											else { return; }
											setTimeout("showClock()",1000);
										}
										function makeArray()
										{
											var args = makeArray.arguments;
											for (var i = 0; i < args.length; i++)
											{
												this[i] = args[i];
											}
											this.length = args.length;
											}
										function fixDate(date)
										{
											var base = new Date(0);
											var skew = base.getTime();
											if (skew > 0) date.setTime(date.getTime() - skew);
										}
										function getString(date)
										{
											var annovero = 0;
											var months = new makeArray("Gennaio", "Febbraio", "Marzo",
														"Aprile",  "Maggio",   "Giugno",
														"Luglio",  "Agosto",   "Settembre",
														"Ottobre", "Novembre", "Dicembre");
											var annovero = date.getYear();
											if (annovero < 1000) annovero = 1900 + annovero;
											return date.getDate() + " " + months[date.getMonth()] + " " + annovero;
										}
										var cur = new Date();
										fixDate(cur);
										var str = getString(cur);
										document.write(str);
									// -->
									</SCRIPT>
									<span id="liveclock">
										<SCRIPT language="JavaScript">
											var dataOdierna = new Date();
											var ore_1 = dataOdierna .getHours();
											var minuti_1 = dataOdierna .getMinutes();
											var secondi_1 = dataOdierna .getSeconds();
											if (ore_1<=9) ore_1 = "0" + ore_1;
											if (minuti_1<=9) minuti_1 = "0" + minuti_1;
											if (secondi_1<=9) secondi_1 = "0" + secondi_1;
											var myclock_1 = ore_1 + ":" + minuti_1 + ":" + secondi_1;
											document.write(myclock_1);										
										</SCRIPT>
									</span>&nbsp;
								</asp:TableCell>
							</tr>
						</table>
					</TD>
				</TR>
				<TR>
					<TD bgColor="#e2e2e2" height="18" colspan="2">
					    <table width="100%">
					        <tr>
					            <td align="left">
					                <uc1:NavigationContext id="NavigationContext" runat="server" ownerFrame="top.superiore" MaxItemsViewed="1"></uc1:NavigationContext>
					            </td>
					            <td align="right" style="padding-right:24px;">
					                <uc2:ScrollElementsList id="ScrollElementsList" runat="server" Visible="false"></uc2:ScrollElementsList>
					            </td>
					        </tr>
					    </table>
					</TD>
				</TR>
				<TR>
					<TD bgColor="#810d06" colspan="2" height="1"><IMG height="1" src="images\spacer.gif" width="1" border="0"></TD>
				</TR>
			</TABLE>
			<div style="LEFT: 0px; VISIBILITY: hidden; POSITION: absolute; TOP: 0px">
				<!-- DOCUMENTI --><cc2:docspamenuwc id="menuDoc" onclick="btn_doc_Click" runat="server" WidthTable="274" CssHLC="HOME">
					<cc2:myLink Type="DO_NUOVOPROT" Visible="True" Text="Nuovo Protocollo" Target="principale" Href="documento/gestioneDoc.aspx?tab=protocollo&IsNew=1"></cc2:myLink>
					<cc2:myLink Type="DO_NUOVODOC" Visible="True" Text="Nuovo Documento" Target="principale" Href="documento/gestioneDoc.aspx?tab=profilo&IsNew=1"></cc2:myLink>
					<cc2:myLink Type="PROTO_IN_SEMPL" Visible="true" Text="Prot. Ingresso" Key="PROTO_INGRESSO_SEMPLIFICATO"
						ClientScriptAction="OpenProtocollazioneIngresso();"></cc2:myLink>
					<cc2:myLink Type="PROTO_OUT_SEMPL" Visible="true" Text="Prot. Uscita"
						ClientScriptAction="OpenProtocollazioneUscita();" Key="PROTO_USCITA_SEMPLIFICATO"></cc2:myLink>
					<cc2:myLink Type="IMP_DOCS" Visible="true" Text="Imp. Documenti"
                        ClientScriptAction="OpenImportDocuments();"></cc2:myLink>
                        <cc2:myLink Type="IMP_DOC_PREG" Visible="true" Text="Imp. Doc. Pregressi"
                        ClientScriptAction="OpenImportDocumentsPregressi();"></cc2:myLink>
				</cc2:docspamenuwc>
				<!-- RICERCA --><cc2:docspamenuwc id="menuRic" onclick="btn_search_Click" runat="server" WidthTable="185" CssHLC="HOME">
					<cc2:myLink Type="DO_CERCA" Visible="True" Text="Documenti" Target="principale" Href="RicercaDoc/gestioneRicDoc.aspx?tab=estesa"></cc2:myLink>
					<cc2:myLink Type="FASC_GESTIONE" Visible="True" Text="Fascicoli" Target="principale" Href="RicercaFascicoli/gestioneRicFasc.aspx"></cc2:myLink>
					<cc2:myLink Type="TRAS_CERCA" Visible="True" Text="Trasmissioni" Target="principale" Href="RicercaTrasm/gestioneRicTrasm.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="top=10,left=200, width=200,height=200,scrollbars=no" Visible="True" Text="Visibilità"  Type="DO_RIC_VISIBILITA" WidthCell="0" Target="Visibilità" Href="popup/VisibilitaFrame.aspx" ClientScriptAction=" Visibilita();"></cc2:myLink>
					
                    <cc2:myLink Type="GEST_AREA_LAV" Visible="False" Text="ADL Doc." WidthCell="0" Target="principale" Href="popup/areaDiLavoro.aspx?action=gestAreaLav&amp;tipoDoc=T"></cc2:myLink>
					<cc2:myLink Type="GEST_AREA_LAV" Visible="False" Text="ADL Fasc." WidthCell="0" Target="principale" Href="popup/areaDiLavoroFasc.aspx"></cc2:myLink>
                    
                    <cc2:myLink Type="IMP_FASC" Visible="true" Text="Imp. Fascicoli" ClientScriptAction="OpenImportProjects();"></cc2:myLink>
                    <cc2:myLink Type="DO_RIC_CAMPI_COMUNI" Visible="True" Text="Ric. Campi Comuni" Target="principale" Href="RicercaCampiComuni/GestioneCampiComuni.aspx?forceInsertContext=true"></cc2:myLink>
					<%--<cc2:myLink Visible="true" Text="ADC"
						ClientScriptAction="OpenADC();"></cc2:myLink>--%>
				</cc2:docspamenuwc>
				<!-- GESTIONE --><cc2:docspamenuwc id="menuGest" onclick="btn_gest_Click" runat="server" CssHLC="HOME" Width="610px"
					Height="13px">
					<cc2:myLink WndOpenProprities="" Type="GEST_REGISTRI" Visible="True" Text="Registri" WidthCell="0"
						Target="principale" Href="gestione/registro/gestioneReg.aspx"></cc2:myLink>
                    <cc2:myLink WndOpenProprities="" Type="GEST_REGISTRO_REPERTORIO" Visible="True" Text="Reg. Repertorio" WidthCell="0"
						Target="principale" Href="gestione/registro/gestioneRegRepertorio.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_STAMPE" Visible="True" Text="Stampe"
						WidthCell="0" Target="principale" Href="gestione/report/gestioneReport.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_PROSPETTI" Visible="True" Text="Prospetti"
						WidthCell="0" Target="principale" href="ProspettiRiepilogativi/Frontend/gestioneProspetti.aspx"></cc2:myLink>
                    <cc2:myLink WndOpenProprities="" Type="GEST_PIANIRIENTRO" Visible="True" Text="Piani"
						WidthCell="0" Target="principale" Href="gestione/piani/gestionePiani.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="JS" Type="GEST_RUBRICA" Visible="True" Text="Rubrica" WidthCell="0"
						Target="principale" Href="javascript:GestioneRubrica();"></cc2:myLink>
                    <cc2:myLink WndOpenProprities="top=100,left=100, width=680,height=450,scrollbars=yes" Type="GEST_AREA_LAV"
	                    Visible="True" Text="ADL Doc." WidthCell="0" Target="AreaLavoro" Href="popup/areaDiLavoro.aspx?action=gestAreaLav&amp;tipoDoc=T"></cc2:myLink>
                    <cc2:myLink WndOpenProprities="top=100,left=100, width=680,height=450,scrollbars=yes" Type="GEST_AREA_LAV"
	                    Visible="True" Text="ADL Fasc." WidthCell="0" Target="AreaLavoro" Href="popup/areaDiLavoroFasc.aspx"></cc2:myLink>
	                <cc2:myLink WndOpenProprities="" Type="GEST_ARCHIVIO_CARTACEO" ClientScriptAction="OpenFascicolazioneCartacea();" Visible="True"
						Text="Arch. cart." WidthCell="0" Target="GestioneAllineamento" Href="FascicolazioneCartacea/GestioneAllineamento.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_FAX" Visible="False" Text="FAX" WidthCell="0" Target="self"
						Href="javascript:return false;"></cc2:myLink>
					<cc2:myLink WndOpenProprities="top=10,left=200, width=600,height=650,scrollbars=no" Type=""
						Visible="True" Text="Liste" WidthCell="0" Target="Liste" Href="popup/ListeDistrFrame.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="" ClientScriptAction="ModelliTrasmissione();" Visible="True"
						Text="Modelli Tras." WidthCell="0" Target="ModelliTrasmissione" Href="popup/GestioneModelliTrasm.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="JS" Type="GEST_ORGANIGRAMMA" Visible="True" Text="Organig."
						WidthCell="0" Target="" Href="javascript:StampaOrganigramma();"></cc2:myLink>
			        <cc2:myLink Type="GEST_DOC_CESTINO" Visible="true" Text="Doc. rimossi"
						ClientScriptAction="OpenDocInCestino();"></cc2:myLink>
					 <cc2:myLink WndOpenProprities="" Type="GEST_DEPOSITO" Visible="True" Text="Deposito" WidthCell="0"
						Target="principale" Href="Archivio/gestArchivio.aspx"></cc2:myLink>						
					<cc2:myLink WndOpenProprities="" Type="GEST_SCARTO" Visible="True" Text="Scarto" WidthCell="0"
						Target="principale" Href="Scarto/gestScarto.aspx"></cc2:myLink>						
					<cc2:myLink Visible="true" Text="Area Cons." Type="DO_CONS"
						ClientScriptAction="OpenADC();"></cc2:myLink>	
					<%--<cc2:myLink Visible="true" WndOpenProprities="top=10,left=100, width=800,height=650,scrollbars=yes" Text="News" />--%>
                    <cc2:myLink Visible="true" WndOpenProprities="top=10,left=100, width=800,height=650,scrollbars=yes" Text="Codici IPA" />						
					<cc2:myLink Type="GEST_DELEGHE" Visible="true" Text="Deleghe" ClientScriptAction="OpenDeleghe();"></cc2:myLink>
					<cc2:myLink Type="GEST_ELENCO_NOTE" Visible="true" Text="Elenco note" ClientScriptAction="OpenElencoNote();"></cc2:myLink>
					<cc2:myLink Type="IMP_RDE" Visible="true" Text="Imp. RDE" ClientScriptAction="OpenImportRDE();"></cc2:myLink>
                    <cc2:myLink Type="GEST_SU" Visible="true" Text="Stampa unione" ClientScriptAction="OpenStampaUnione();"></cc2:myLink>
				</cc2:docspamenuwc>
				<!-- OPZIONI --><cc2:docspamenuwc id="menuConf" runat="server" WidthTable="98" CssHLC="HOME">
					<cc2:myLink Type="OPZIONI_CAMBIA_PWD" Visible="True" Text="Cambia Password" WndOpenProprities="top=100,left=100, width=450,height=230,scrollbars=no"
						Target="CambiaPassword" ClientScriptAction="OpenCambiaPassword();"></cc2:myLink>
				</cc2:docspamenuwc>
				</div>
                <input type="hidden" id="hdSlogga" runat="server" />
		</form>
	</body>
</HTML>
