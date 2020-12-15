<%@ Page language="c#" Codebehind="testata2.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.testata2"  %>
<%@ Register TagPrefix="cc2" Namespace="docsPaMenu" Assembly="DocsPaMenu" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"  %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="LIBRERIE/rubrica.js"></script>
		<LINK href="CSS/DocsPA.css" type="text/css" rel="stylesheet">
		<SCRIPT language="JavaScript">

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
						showbox.top=56;
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
						// curEl.style.background = "#C08682"   
						showBox = document.all.box[x];
						showBox.style.visibility = "visible";
						showBox.style.top =56;
						var items = 6
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
				
				/*
				try
				{
				//commentato elisa 11/08/2005 per evitare che il menu scompaia al mouseOut
					var items = 6 
					for (i=0; i<items; i++) 
					{
						if(ie)
						{
							document.all.box[i].style.visibility = "hidden"
							barEl=document.getElementsByName('mnubar'+i.toString());
						}
						if(ns)
						{ 
							document.layers[i+1].visibility = "hide"
						}          
					}
				}
				catch(e)
				{ 
					return false;
				}
				
				*/
			}
			
			function GestioneRubrica()
			{
				r = new Rubrica();
				r.CallType = r.CALLTYPE_MANAGE;
				r.Apri();
			}
			
			function prospettiRiepilogativi()
			{
				var pageAddress='ProspettiRiepilogativi/Frontend/Index.htm';
				var newUrl=pageAddress;
				var _width = (screen.width-10);
				var _height = (screen.height-90); 
				window.open(newUrl,'_blank','top=0,left=0,width='+_width+',height='+_height+',fullscreen=no,toolbar=no,directories=no,status=yes,menubar=no,resizable=yes, scrollbars=auto');
			}
			
			function ModelliTrasmissione()
			{
				var pageAddress='popup/GestioneModelliTrasm_frame.aspx';
				var newUrl=pageAddress;
				var _width = (screen.width-10);
				var _height = (screen.height-10); 
				//window.open(newUrl,'_blank','top=0,left=0,width='+_width+',height='+_height+',fullscreen=no,toolbar=no,directories=no,status=yes,menubar=no,resizable=yes, scrollbars=auto');
				window.showModalDialog(newUrl,'','dialogWidth:' + _width + 'px;dialogHeight:' + _height + 'px;fullscreen:no;toolbar:no;status:yes;resizable:yes;scroll:auto;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}
			function OpenProtocollazioneIngresso() 
			{		
				var pageHeight=window.screen.availHeight;
				var pageWidth=window.screen.availWidth;
				
				window.showModalDialog('ProtocollazioneIngresso/Protocollazione.aspx',
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
							
				//window.open('organigramma/Stampa_Organigramma.aspx','','top=0,left=0,width='+pageWidth+',height='+pageHeight+',toolbar=no,directories=no,status=no,menubar=yes,resizable=yes,scrollbars=yes');	
			}
			
			function OpenCambiaPassword() 
			{		
				var pageHeight= 220;
				var pageWidth = 410;
				
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				window.showModalDialog('popup/cambiaPassword.aspx',
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:no;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:no;help:no');
			}
						
			/*	
			function CheckTestataTastoSel()
			{
			try{

			
				document.all.btn_doc.src='images/testata/btn_documenti.gif';
				document.all.btn_fasc.src='images/testata/btn_fascicoli.gif';
				document.all.btn_trasm.src='images/testata/btn_trasmissioni.gif';
				document.all.btn_gest.src='images/testata/btn_gestione.gif';
				switch(top.principale.document.title)
				{
					case 'gestioneDoc':
					
					document.all.btn_doc.src='images/testata/btn_documenti.gif';
					break;
					
					case 'gestioneFasc':
					
					document.all.btn_fasc.src='images/testata/btn_fascicoli.gif';
					break;
					
					case 'gestioneTrasm':
					
					document.all.btn_trasm.src='images/testata/btn_trasmissioni.gif';
					break;
					
					case 'gestioneRicTrasm':
					
					document.all.btn_trasm.src='images/testata/btn_trasmissioni.gif';
					break;
					
					case 'gestioneRicDoc':
					
					document.all.btn_doc.src='images/testata/btn_documenti.gif';
					break;
					
					case 'gestioneRicFasc':
					
					document.all.btn_fasc.src='images/testata/btn_fascicoli.gif';
					break;
					
					case 'gestioneReg':
					
					document.all.btn_gest.src='images/testata/btn_gestione.gif';
					break;
					
				}
				}
				catch(e)
				{
				//alert(e.message);
				}
			}
			*/
		</SCRIPT>
	</HEAD>
	<body language="javascript" MS_POSITIONING="GridLayout">
		<form id="testata" method="post" runat="server">
			<TABLE id="mainmenu" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR>
					<TD id="mnubar0" vAlign="top" width="141"><asp:imagebutton id="img_logo" runat="server" AlternateText="Torna alla pagina iniziale" height="56"
							ImageUrl="images/logo.gif" width="141" ImageAlign="AbsMiddle"></asp:imagebutton></TD>
					<td width="100%" height="56">
						<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr height="30">
								<td width="1" bgColor="#810d06"><IMG height="30" src="images/spaziatore.gif" width="1" border="0"></td>
								<td height="100%"><IMG src="images/testata/protocolloinfo.gif"></td>
								<td width="100%"><IMG src="images/testata/sfondotestata.gif"></td>
							</tr>
							<tr vAlign="bottom">
								<td colSpan="3">
									<table borderColor="#810d06" cellSpacing="0" cellPadding="0" rules="none" border="1" frame="border">
										<tr>
											<TD id="mnubar1" width="91" runat="server"><cc1:imagebutton id="btn_doc" runat="server" AlternateText="Documenti" ImageUrl="images/testata/btn_documenti.gif"
													Tipologia="MENU_DOCUMENTI" DisabledUrl="images/testata/btn_documenti.gif"></cc1:imagebutton></TD>
											<TD id="mnubar2" runat="server"><cc1:imagebutton id="btn_search" runat="server" AlternateText="Ricerca" ImageUrl="images/testata/btn_ricerca.gif"
													Tipologia="MENU_RICERCA" DisabledUrl="images/testata/btn_ricerca.gif" borderWidth="0"></cc1:imagebutton></TD>
											<TD id="mnubar3" width="91" runat="server"><cc1:imagebutton id="btn_fasc" runat="server" AlternateText="Fascicoli" ImageUrl="images/testata/btn_fascicoli.gif"
													DisabledUrl="images/testata/btn_fascicoli.gif" Visible="False"></cc1:imagebutton></TD>
											<TD id="mnubar4" width="91" runat="server"><cc1:imagebutton id="btn_trasm" runat="server" AlternateText="Trasmissione" ImageUrl="images/testata/btn_trasmissioni.gif"
													DisabledUrl="images/testata/btn_trasmissioni.gif" Visible="False"></cc1:imagebutton></TD>
											<TD id="mnubar5" width="91" runat="server"><cc1:imagebutton id="btn_gest" runat="server" AlternateText="Gestione" ImageUrl="images/testata/btn_gestione.gif"
													Tipologia="MENU_GESTIONE" DisabledUrl="images/testata/btn_gestione.gif"></cc1:imagebutton></TD>
											<TD id="mnubar6" runat="server"><cc1:imagebutton id="btn_config" runat="server" AlternateText="Opzioni" ImageUrl="images/testata/btn_opzioni.gif"
													Tipologia="MENU_OPZIONI" DisabledUrl="images/testata/btn_opzioni.gif" borderWidth="0" Enabled="False"></cc1:imagebutton></TD>
											<TD><cc1:imagebutton id="btn_logout" runat="server" AlternateText="Log out" ImageUrl="images/testata/btn_logout.gif"
													borderWidth="0" DESIGNTIMEDRAGDROP="66"></cc1:imagebutton></TD>
											<TD width="100%"><IMG src="images/testata/btn_sfondo.gif"></TD>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</TR>
				<TR>
					<TD bgColor="#c2c2c2" colSpan="2" height="23"><IMG height="1" src="images\spacer.gif" width="1" border="0">&nbsp;</TD>
				</TR>
				<TR>
					<TD width="91" bgColor="#9e9e9e" colSpan="22" height="1"><IMG height="1" src="images\spacer.gif" width="1" border="0"></TD>
				</TR>
			</TABLE>
			<!--EMOSCA 06/12/2004 - spostati fuori della table per evitare lo scroll-->
			<div style="LEFT: 0px; VISIBILITY: hidden; POSITION: absolute; TOP: 0px"><cc2:docspamenuwc id="menuDoc" onclick="btn_doc_Click" runat="server" MenuPosLeft="147" CssHLC="HOME"
					Height="13px" Width="110px">
					<cc2:myLink WndOpenProprities="" Type="DO_NUOVOPROT" Visible="True" Text="Nuovo Protocollo"
						WidthCell="0" Target="principale" Href="documento/gestioneDoc.aspx?tab=protocollo&IsNew=1"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="DO_NUOVODOC" Visible="True" Text="Nuovo Documento" WidthCell="0"
						Target="principale" Href="documento/gestioneDoc.aspx?tab=profilo&IsNew=1"></cc2:myLink>
					<cc2:myLink Key="PROTO_INGRESSO_SEMPLIFICATO" Type="PROTO_IN" Visible="true" Text="Prot. Ingresso"
						ClientScriptAction="OpenProtocollazioneIngresso();"></cc2:myLink>
				</cc2:docspamenuwc><cc2:docspamenuwc id="menuFasc" onclick="btn_fasc_Click" runat="server" CssHLC="HOME" Height="13px"
					Width="110px">
					<cc2:myLink WndOpenProprities="" Type="FASC_GESTIONE" Visible="True" Text="Gestione fascicolo"
						WidthCell="0" Target="principale" Href="RicercaFascicoli/gestioneRicFasc.aspx?tab=documenti"></cc2:myLink>
				</cc2:docspamenuwc><cc2:docspamenuwc id="menuTrasm" onclick="btn_trasm_Click" runat="server" CssHLC=" HOME" Height="13px"
					Width="110px">
					<cc2:myLink WndOpenProprities="" Type="TRAS_CERCA" Visible="True" Text="Cerca" WidthCell="0"
						Target="principale" Href="ricercaTrasm/gestioneRicTrasm.aspx?tab=trasmissioni"></cc2:myLink>
				</cc2:docspamenuwc><cc2:docspamenuwc id="menuGest" onclick="btn_gest_Click" runat="server" CssHLC="HOME" Height="13px"
					Width="110px">
					<cc2:myLink WndOpenProprities="" Type="GEST_REGISTRI" Visible="True" Text="Registri" WidthCell="0"
						Target="principale" Href="gestione/registro/gestioneReg.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_STAMPE" Visible="True" Text="Stampe e Rapporti"
						WidthCell="0" Target="principale" Href="gestione/report/gestioneReport.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_PROSPETTI" Visible="True" Text="Prospetti Riepilogativi"
						WidthCell="0" Target="" href="ProspettiRiepilogativi/Frontend/Index.htm" ClientScriptAction="prospettiRiepilogativi();"></cc2:myLink>
					<cc2:myLink WndOpenProprities="JS" Type="GEST_RUBRICA" Visible="True" Text="Rubrica" WidthCell="0"
						Target="principale" Href="javascript:GestioneRubrica();"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_REGISTRI" Visible="false" Text="Log" WidthCell="0"
						Target="principale" Href="ricercaTrasm/gestioneRicTrasm.aspx?tab=log"></cc2:myLink>
					<cc2:myLink WndOpenProprities="top=100,left=100, width=680,height=450,scrollbars=yes" Type="GEST_AREA_LAV"
						Visible="True" Text="ADL Documenti" WidthCell="0" Target="AreaLavoro" Href="popup/areaDiLavoro.aspx?action=gestAreaLav&amp;tipoDoc=T"></cc2:myLink>
					<cc2:myLink WndOpenProprities="top=100,left=100, width=680,height=450,scrollbars=yes" Type="GEST_AREA_LAV"
						Visible="True" Text="ADL Fascicoli" WidthCell="0" Target="AreaLavoro" Href="popup/areaDiLavoroFasc.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_EMERGENZA" Visible="True" Text="Sessione Emergenza"
						WidthCell="0" Target="principale" Href="javascript:return false;"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="GEST_FAX" Visible="true" Text="FAX" WidthCell="0" Target="self"
						Href="javascript:return false;"></cc2:myLink>
					<cc2:myLink WndOpenProprities="top=10,left=200, width=600,height=550,scrollbars=no" Type=""
						Visible="True" Text="Liste" WidthCell="0" Target="Liste" Href="popup/ListeDistrFrame.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Type="" ClientScriptAction="ModelliTrasmissione();" Visible="True"
						Text="Modelli Trasmissione" WidthCell="0" Target="ModelliTrasmissione" Href="popup/GestioneModelliTrasm.aspx"></cc2:myLink>
					<cc2:myLink WndOpenProprities="JS" Type="" Visible="True" Text="Organigramma" WidthCell="0"
						Target="" Href="javascript:StampaOrganigramma();"></cc2:myLink>
				</cc2:docspamenuwc><cc2:docspamenuwc id="menuRic" onclick="btn_search_Click" runat="server" CssHLC=" HOME" Height="13px"
					Width="110px">
					<cc2:myLink WndOpenProprities="" Visible="True" Text="Documenti" WidthCell="0" Target="principale"
						Href="RicercaDoc/gestioneRicDoc.aspx?tab=estesa" Type="DO_CERCA"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Visible="True" Text="Fascicoli" WidthCell="0" Target="principale"
						Href="RicercaFascicoli/gestioneRicFasc.aspx" Type="FASC_GESTIONE"></cc2:myLink>
					<cc2:myLink WndOpenProprities="" Visible="True" Text="Trasmissioni" WidthCell="0" Target="principale"
						Href="RicercaTrasm/gestioneRicTrasm.aspx" Type="TRAS_CERCA"></cc2:myLink>
				</cc2:docspamenuwc><cc2:docspamenuwc id="menuConf" runat="server" CssHLC=" HOME" Height="13px" Width="110px">
					<cc2:myLink WndOpenProprities="top=100,left=100, width=450,height=230,scrollbars=no" Visible="True"
						Text="Cambia Password" WidthCell="0" Target="CambiaPassword" ClientScriptAction="OpenCambiaPassword();"
						Type="OPZIONI_CAMBIA_PWD"></cc2:myLink>
				</cc2:docspamenuwc></div>
		</form>
	</body>
</HTML>
