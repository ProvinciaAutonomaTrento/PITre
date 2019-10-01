<%@ Page language="c#" Codebehind="Stampa_Organigramma.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.organigramma.Stampa_Organigramma" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title><%= appTitle %> > Organigramma</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../AdminTool/CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self" />
		<script language="javascript">
			function VisualizzaAttendi()
			{				
				window.document.body.style.cursor='wait';
				
				var w_width = 200;
				var w_height = 50;	
				var t = (screen.availHeight - w_height) / 2;
				var l = (screen.availWidth - w_width) / 2;	
				if(document.getElementById ("WAIT"))
				{		
					document.getElementById ("WAIT").style.top = t;
					document.getElementById ("WAIT").style.left = l;
					document.getElementById ("WAIT").style.width = w_width;
					document.getElementById ("WAIT").style.height = w_height;				
					document.getElementById ("WAIT").style.visibility = "visible";
				}				
			}
			
			function ApriRisultRic(idAmm) 
			{																								
				if(document.Form1.txt_ricCod.value.length > 0 || document.Form1.txt_ricDesc.value.length > 0)
				{
					var myUrl = "RisultatoRicercaOrg.aspx?idAmm="+idAmm+"&tipo="+document.Form1.ddl_ricTipo.value+"&cod="+document.Form1.txt_ricCod.value+"&desc="+document.Form1.txt_ricDesc.value;
					rtnValue = window.showModalDialog(myUrl,"","dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
					Form1.hd_returnValueModal.value = rtnValue;
				}
			}
			function stampa()
			{				
				//var args = new Object;
				//args.window = window;				
				//window.showModalDialog("VisualPdfToPrint.aspx",args,"dialogWidth:800px;dialogHeight:600px;status:no;resizable:yes;scroll:yes;center:yes;help:no;");
				//window.open('VisualPdfToPrint.aspx','','width=800,height=600,toolbar=no,statusbar=no,menubar=no,resizable=yes,scrollbars=auto');
				OpenFile();
			}	
			function openHelp()
			{
				var w_width = 500;
				var w_height = 500;					
				var l = ((screen.availWidth - 27) - w_width);	
				if(document.getElementById ("HELP"))
				{		
					document.getElementById ("HELP").style.top = 43;
					document.getElementById ("HELP").style.left = l;
					document.getElementById ("HELP").style.width = w_width;
					document.getElementById ("HELP").style.height = w_height;				
					document.getElementById ("HELP").style.visibility = "visible";
				}
			}
			function closeHelp()
			{
				if(document.getElementById ("HELP"))
				{										
					document.getElementById ("HELP").style.visibility = "hidden";
				}
			}

			function OpenHelp(from) 
			{		
				var pageHeight= 600;
				var pageWidth = 800;
				//alert(from);
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				var newwin = window.showModalDialog('../Help/Manuale.aspx?from=' + from,
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
								
				//newwin.location.hash=from;
				//return false;
			}		
			
			function OpenFile()
			{
				var filePath;
				var exportUrl;
				var http;
				var applName;				
				var fso;                
                var folder;                    
                var path;

				try
				{
				    fso = FsoWrapper_CreateFsoObject();
                    // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

                    path = fso.GetSpecialFolder(2).Path;                    

					filePath = path + "\\stampaOrgDocspa.pdf";
					applName = "Adobe Acrobat";						
					
					exportUrl= "VisualPdfToPrint.aspx";				
					http = CreateObject("MSXML2.XMLHTTP");
					http.Open("POST",exportUrl,false);
					http.send();					
					
					var content=http.responseBody;
					
					if (content!=null)
					{
						AdoStreamWrapper_SaveBinaryData(filePath,content);
						
						ShellWrappers_Execute(filePath);
					}
				}
				catch (ex)
				{			
				alert(ex.message.toString());
				
				//	alert("Impossibile aprire il file generato!\n\nPossibili motivi:\n- il browser non è abilitato ad eseguire controlli ActiveX\n- il sito intranet DocsPA non compare tra i siti attendibili di questo computer;\n- estensione '"+typeFile+"' non associata all'applicazione "+applName+";\n- "+applName+" non installato su questo computer;\n- applicazione "+applName+" relativa ad esportazioni precedentente effettuate ancora attiva.");					
				}						
			}
			
	        // Creazione oggetto activex con gestione errore
	        function CreateObject(objectType)
	        {
		        try { return new ActiveXObject(objectType); }
		        catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }	
	        }					
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0">
		<form id="Form1" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Organigramma" />
 			<input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<input id="hd_lastReturnValueModal" type="hidden" name="hd_lastReturnValueModal" runat="server">
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server">
			<TABLE cellSpacing="1" cellPadding="1" width="100%" align="center" border="0">
				<TR>
					<TD id="td_ricerca">
						<TABLE cellSpacing="2" cellPadding="0" width="100%" align="center" border="0">
							<TR>
								<TD class="testo_piccoloB" width="200px">Visualizza:</TD>
								<TD class="testo_piccoloB" width="100px">Ricerca tra:</TD>
								<TD class="testo_piccoloB" id="td_codRicerca" width="80px" runat="server">Codice:</TD>
								<TD class="testo_piccoloB" id="td_descRicerca" width="210px" runat="server">Nome UO:</TD>
								<TD class="testo_piccoloB" width="50px"></TD>
								<TD class="testo_piccoloB" width="100%"></TD>
							</TR>
							<TR>
								<TD class="testo_piccoloB" width="200px"><asp:dropdownlist id="ddl_visualizza" CssClass="testo_grigio_scuro" Runat="server" AutoPostBack="True">
										<asp:ListItem Value="1" Selected="True">UO</asp:ListItem>
										<asp:ListItem Value="2">UO + Ruoli</asp:ListItem>
										<asp:ListItem Value="3">UO + Ruoli + Utenti</asp:ListItem>
									</asp:dropdownlist></TD>
								<TD class="testo_piccoloB" width="100px"><asp:dropdownlist id="ddl_ricTipo" CssClass="testo_grigio_scuro" Runat="server" AutoPostBack="True">
										<asp:ListItem Value="U" Selected="True">UO</asp:ListItem>
										<asp:ListItem Value="R">Ruolo</asp:ListItem>
										<asp:ListItem Value="PN">Nome</asp:ListItem>
										<asp:ListItem Value="PC">Cognome</asp:ListItem>
									</asp:dropdownlist></TD>
								<TD class="testo_piccoloB" width="80px"><asp:textbox id="txt_ricCod" tabIndex="1" CssClass="testo_grigio_scuro" Runat="server" Width="80px"></asp:textbox><asp:DropDownList ID="ddl_RF" runat="server" Visible="false" Width="250px" CssClass="testo_grigio_scuro"></asp:DropDownList></TD>
								<TD class="testo_piccoloB" width="210px"><asp:textbox id="txt_ricDesc" tabIndex="2" CssClass="testo_grigio_scuro" Runat="server" Width="210px"></asp:textbox></TD>
								<TD class="testo_piccoloB" width="50px"><asp:imagebutton id="btn_find" Runat="server" AlternateText="Cerca in organigramma" SkinID="btnFind"></asp:imagebutton>
                                    <asp:ImageButton ID="btn_findRF" Runat="server" 
                                        AlternateText="Cerca tutti i ruoli con questo RF" 
                                        SkinID="btnFind" Visible="false" onclick="btn_findRF_Click" /></TD>
								<TD class="testo_piccoloB" align="right" width="100%">
									<asp:imagebutton id="btn_clear" Runat="server" AlternateText="Pulisci tutto" SkinID="clear_flag"></asp:imagebutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:imagebutton id="btn_root" Runat="server" AlternateText="Pagina iniziale" SkinID="btnRoot"></asp:imagebutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:imagebutton id="btn_back" Runat="server" AlternateText="UO padre" SkinID="btnBack"></asp:imagebutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:imagebutton id="btn_impostaRoot" Runat="server" AlternateText="Imposta come UO radice" SkinID="btnImpostaRoot"></asp:imagebutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:imagebutton id="btn_espandeOrg" Runat="server" AlternateText="Espandi completamente la UO radice" SkinID="btnEspandeOrg"></asp:imagebutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:imagebutton id="btn_stampa" Runat="server" AlternateText="Converti in PDF per la stampa" SkinID="btnIcoStampa"></asp:imagebutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:ImageButton ID="ImageButton1" runat="server" AlternateText="Aiuto?" SkinID="btnHelp" OnClientClick="OpenHelp('GestioneOrganigramma')" />
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD id="treeview" vAlign="top">
						<DIV style="OVERFLOW: auto; HEIGHT: 100%"><IEWC:TREEVIEW id="treeViewUO" runat="server" 
						DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"
								backcolor="antiquewhite" borderwidth="1px" borderstyle="solid" font="verdana" width="100%" AutoPostBack="True"></IEWC:TREEVIEW></DIV>
					</TD>
				</TR>
			</TABLE>
			<DIV id="WAIT" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; BACKGROUND-COLOR: #ffc37d">
				<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Attendere 
							prego...
						</td>
					</tr>
				</table>
			</DIV>
			<DIV id="HELP" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; BACKGROUND-COLOR: #cfffcf">
				<table height="90%" cellSpacing="0" cellPadding="0" width="90%" align="center" border="0">
					<tr>
						<td style="FONT-SIZE: 8pt; FONT-FAMILY: Arial, Verdana" vAlign="middle">
							<p align="center"><b>Aiuto</b></p>
							Questa funzionalità permette di visualizzare l'organigramma tramite una 
							rappresentazione ad albero.<br>
							E' possibile compiere ricerche (per codice esatto o descrizione anche parziale) 
							rispetto alle entità che costituiscono l'organigramma: unità organizzative, 
							ruoli e utenti (tramite nome o cognome).<br>
							Se la ricerca ha come risultato un unico dato, questo verrà rappresentato 
							direttamente a video. Differentemente, se una ricerca produce più risultati, 
							questi saranno elencati in una schermata secondaria dove potranno essere 
							selezionati a scelta dall'utente.<br>
							<!--
							La rappresentazione ad albero dell'organigramma avviene in due modi:
							<ul>
								<li>
									<b>parziale</b>, quando si effettua una ricerca: viene visualizzata solo la 
								porzione di organigramma partendo dall'unità organizzativa ricercata ovvero 
								padre del ruolo o utente ricercati;
								<li>
									<b>totale</b>, quando viene premuto il pulsante <IMG src="../images/ico_tit.gif" border="0">.
								</li>
							</ul>
							-->
							La struttura dell'organigramma visualizzata può essere più o meno dettagliata 
							scegliendo tra le opzioni del menù a tendina "Visualizza" posto in alto a 
							sinistra.
							<br>
							<p align="center"><b>Descrizione dei pulsanti</b></p>
							<IMG src="../images/ico_find.gif" border="0"> Permette la ricerca 
							nell'organigramma rispetto ai criteri inseriti dall'utente.<br>
							<br>
							<IMG src="../images/clearFlag.gif" border="0"> Pulisce i dati a video e 
							ripristina la condizione iniziale di ricerca.<br>							
							<br>
							<IMG src="../images/ico_tit_home.gif" border="0"> Riporta alla pagina iniziale.<br>
							<br>
							<IMG src="../images/ico_tit_back.gif" border="0"> Riporta alla UO padre di quella visualizzata come radice.<br>
							<br>
							<IMG src="../images/ico_tit_root.gif" border="0"> Imposta la UO selezionata nell'albero di visualizzazione come radice.<br>
							<br>
							<IMG src="../images/ico_tit.gif" border="0"> Visualizza tutto l'organigramma partendo dalla UO radice
							(l'operazione di visualizzazione potrebbe impiegare alcuni secondi).<br>
							<br>
							<IMG alt="Stampa" src="../images/ico_stampa.gif" border="0"> I dati visualizzati ad albero vengono importati in un formato stampabile utilizzando un file Adobe Acrobat PDF.
							<br>
							<br>
						</td>
					</tr>
				</table>
			</DIV>
			<uc1:ShellWrapper ID="shellWrapper" runat="server" />
            <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
            <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
		</form>
	</body>
</HTML>
