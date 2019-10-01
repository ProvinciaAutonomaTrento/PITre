<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="ax1" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="ax2" %>
<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="ax3" %>

<%@ Page language="c#" Codebehind="Titolario.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Titolario.Titolario" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<base target=_self>
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<SCRIPT language="JavaScript">
				
			var cambiapass;
			var hlp;
			var wndricerca;
			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-500)/2;
			var new_h = (h-400)/2;
			
			function apriPopup() {
				hlp = window.open('../help.aspx?from=TI','','width=450,height=500,scrollbars=YES');
			}					
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}	
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
				if(typeof(wndricerca) != 'undefined')
				{
					if(!wndricerca.closed)
						wndricerca.close();
				}
			}		
			function CheckAllDataGridCheckBoxes(aspCheckBoxID, checkVal) 
			{
				re = new RegExp(':' + aspCheckBoxID + '$')  
				for(i = 0; i < document.forms[0].elements.length; i++) 
				{
					elm = document.forms[0].elements[i]
					if (elm.type == 'checkbox') 
					{
						if (re.test(elm.name)) 
						{						
							elm.checked = checkVal;
						}
					}
				}
			}	
			
			function apriPopupIndiceSistematico()
			{
			    //window.open('IndiceSistematico.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=160,scrollbars=NO');				
				window.showModalDialog('Modal.aspx?Chiamante=IndiceSistematico.aspx','','dialogWidth:600px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);				
			}
			
			function apriGestioneEtichetteTit(idTitolario)
			{
			   //window.open('GestioneEtichette.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=160,scrollbars=NO');				
				window.showModalDialog("Modal.aspx?Chiamante=GestioneEtichette.aspx?idTitolario="+idTitolario,'','dialogWidth:400px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);				
			}	
		</SCRIPT>
		<script language="javascript" id="btn_find_click" event="onclick()" for="btn_find">
			window.document.body.style.cursor='wait';
			void(0);
		</script>
		<script language="javascript" id="btnFrecciaSu_click" event="onclick()" for="btnFrecciaSu">
			window.document.body.style.cursor='wait';
			void(0);
		</script>
		<script language="javascript" id="btnFrecciaGiu_click" event="onclick()" for="btnFrecciaGiu">
			window.document.body.style.cursor='wait';
			void(0);
		</script>
		<script language="javascript" id="btn_aggiungiNew_click" event="onclick()" for="btn_aggiungiNew">
			//document.frmTitolario.btn_aggiungiNew.disabled=true;
			window.document.body.style.cursor='wait';			
		</script>
		<script language="javascript" id="btn_elimina_click" event="onclick()" for="btn_elimina">
			//document.frmTitolario.btn_elimina.disabled=true;
			window.document.body.style.cursor='wait';
			void(0);
		</script>
		<script language="javascript" id="btn_salvaInfo_click" event="onclick()" for="btn_salvaInfo">			
			//document.frmTitolario.btn_salvaInfo.disabled=true;
			window.document.body.style.cursor='wait';			
		</script>
		<script language="javascript">
			// Impostazione del focus su un controllo
			function SetControlFocus(controlID)
			{	
				try
				{
					var control=document.getElementById(controlID);
					
					if (control!=null)
					{
						control.focus();
					}
				}
				catch (e)
				{
				
				}
			}
			
			// Visualizzazione finestra di dialogo
			// per la gestione dei ruoli che hanno
			// la visibilità sul nodo di titolario corrente
			function ShowDialogRuoliTitolario()
			{
				var queryString=frmTitolario.txtQueryStringDialogRuoliTitolario.value;
			
				return window.showModalDialog(
								'RuoliTitolario.aspx' + queryString,
								'',
								'dialogWidth:580px;dialogHeight:640px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
			}
			
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
			
			function ApriImportaTitolario()
			{
			    var myUrl = "ImportaTitolario.aspx";				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:430px;dialogHeight:150px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
				//rtnValue = window.open(myUrl,"","dialogWidth:430px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
				frmTitolario.submit();
			}	
			
			function ApriImportaIndice()
			{
			    var myUrl = "ImportaIndice.aspx";				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:430px;dialogHeight:150px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
				//rtnValue = window.open(myUrl,"","dialogWidth:430px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
				frmTitolario.submit();
			}			
		</script>
		<script language="javascript">
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
				    //fso = CreateObject("Scripting.FileSystemObject");
				    fso = FsoWrapper_CreateFsoObject();
                    // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
                    folder = fso.GetSpecialFolder(2);                    
                    path =  folder.Path;
					filePath = path + "\\exportDocspa.xls";
					applName = "Microsoft Excel";	
					exportUrl= "..\\..\\exportDati\\exportDatiPage.aspx";				
					http = CreateObject("MSXML2.XMLHTTP");
					http.Open("POST",exportUrl,false);
					http.send();					
				
				       
				    var content=http.responseBody;
					if (content!=null)
					{
					    AdoStreamWrapper_SaveBinaryData(filePath,content);
						
						ShellWrappers_Execute(filePath);
						
						//var adoStream=CreateObject("DocsPa_ActivexWrappers.AdoStreamWrapper");
						//adoStream.SaveBinaryData(filePath,content);
						
						//var shellObj=CreateObject("DocsPa_ActivexWrappers.ShellWrapper");
						//shellObj.ShellExecute(filePath);											
					}								 					
				}
				catch (ex)
				{			
				    alert(ex.message.toString());
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
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()"
		MS_POSITIONING="GridLayout">
		<form id="frmTitolario" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Titolario" />
			<INPUT id="txtQueryStringDialogRuoliTitolario" type="hidden" name="txtQueryStringDialogRuoliTitolario"
				runat="server"> 
			<!-- Gestione del menu a tendina -->
			<uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<!-- TESTATA CON MENU' -->
				<tr>
					<td style="HEIGHT: 8px"><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<!-- POSIZIONE -->
				<tr>
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<!-- TITOLO -->
				<tr>
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Titolario</td>
				</tr>
				<tr>
					<td class="titolo" align="center" bgColor="#e0e0e0"></td>
				</tr>
                <!-- Autenticazione Sistemi Esterni: ritorno alla configurazione per i sistemi esterni -->
                <tr id="tr_backToExtSys" runat="server">
                    <td valign="top" align="left" bgcolor="#f6f4f4" height="20">
                        <asp:Button ID="btn_toExtSys" runat="server" UseSubmitBehavior="false" CssClass="testo_btn"
                            Text="Ritorna a Sistemi Esterni" OnClientClick="location.href='../Gestione_SistemiEsterni/SistemiEsterni.aspx'; return false;">
                        </asp:Button>
                    </td>
                </tr>
                <!-- CORPO CENTRALE -->
                <tr>
                    <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
						<table width="95%" align="center">
							<tr>
								<td class="pulsanti">
									<table cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td align="left" width="50%">
												<table cellSpacing="0" cellPadding="3" border="0">
													<tr>
														<!-- Registro -->
														<td class="testo_blu" >Nodi associati a:</td>
														<td class="testo_piccoloB" ><asp:dropdownlist id="ddl_registri" CssClass="testo_blu" Runat="server" AutoPostBack="True" Width="340px"></asp:dropdownlist></td>
													</tr>
												</table>
											</td>
											<!-- La ricerca non contepla la presenz adi piuù di un titolario e per il momento è stata resa invisibile -->
											<asp:Panel id="pnl_RicercaNodo" runat="server" Visible="true">
											<td align="right" width="50%">
												<table cellSpacing="0" cellPadding="3" border="0">
													<tr>
														<!-- Ricerca nodo -->
														<td class="testo_piccoloB"><asp:label id="lblTipoRicerca" runat="server" CssClass="testo_grigio_scuro">Ricerca per:</asp:label></td>
														<td class="testo_piccoloB"><asp:dropdownlist id="cboTipoRicerca" runat="server" CssClass="testo" Width="120"></asp:dropdownlist></td>
														<td class="testo_piccoloB"><asp:textbox id="txtFieldRicerca" runat="server" CssClass="testo" Width="200"></asp:textbox></td>
														<td class="testo_piccoloB"><asp:button id="btnSearch" CssClass="testo_btn" Runat="server" Text="Cerca"></asp:button></td>
													</tr>
												</table>
											</td>
											</asp:Panel>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td>
									<!-- TREEVIEW --><iewc:treeview id="trvNodiTitolario" runat="server" AutoPostBack="True" DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"
										backcolor="antiquewhite" borderwidth="1px" borderstyle="solid" bordercolor="maroon" font="verdana" width="100%"
										height="200px"></iewc:treeview></td>
							</tr>
							<!--
							<tr id="tblRowSpostaNodoTitolario" runat="server">
								<td class="testo_grigio_scuro"><asp:label id="lblSpostaNodo" runat="server">Sposta nodo:</asp:label>&nbsp;
									<asp:imagebutton id="btnMoveUp" runat="server" ToolTip="Sposta il nodo selezionato verso l'alto"
										ImageUrl="../Images/freccia_su.gif" ImageAlign="AbsMiddle"></asp:imagebutton>&nbsp;&nbsp;&nbsp;
									<asp:imagebutton id="btnMoveDown" runat="server" ToolTip="Sposta il nodo selezionato verso il basso"
										ImageUrl="../Images/freccia_giu.gif" ImageAlign="AbsMiddle"></asp:imagebutton></td>
							</tr>
							-->
							<tr>
							    <td>
							        <asp:Panel ID="pnl_DettagliTitolario" runat="server">
							            <table class="contenitore" width="100%" border="0">
										    <tr>
											    <td class="titolo_pnl" vAlign="top">
											        Dettagli titolario <asp:label id="lbl_DescrizioneTitolario" Width="80%" runat="server" CssClass="titolo_pnl"></asp:label>
										        </td>
										    </tr>
									        <tr>
											    <td>
												    <table cellSpacing="1" cellPadding="3" width="100%" border="0">
													    <tr>
													        <td Width="10%"><asp:label id="lbl_DescrizioneTit" Width="100%" runat="server" CssClass="testo_grigio_scuro">Descrizione *</asp:label></td>
													        <td Width="30%"><asp:TextBox ID="txt_DescrizioneTit" Width="100%" runat="server" CssClass="testo"></asp:TextBox></td>
													        <td Width="20%"></td>
													        <td Width="40%"></td>
													    </tr>
													    <tr>
													        <td><asp:label id="lbl_CommentoTit" Width="100%" runat="server" CssClass="testo_grigio_scuro">Commento</asp:label></td>
													        <td><asp:TextBox ID="txt_CommentoTit" Width="100%" runat="server" CssClass="testo"></asp:TextBox></td>
													        <td align="right"><asp:label id="lbl_LivelliTit" Width="100%" runat="server" CssClass="testo_grigio_scuro">Massimo num. livelli titolario</asp:label></td>
													        <td>
													            <asp:DropDownList ID="ddl_LivelliTit" runat="server" CssClass="testo_grigio_scuro">
										                            <asp:ListItem>6</asp:ListItem>
										                            <asp:ListItem>5</asp:ListItem>
										                            <asp:ListItem>4</asp:ListItem>
										                            <asp:ListItem>3</asp:ListItem>
										                            <asp:ListItem>2</asp:ListItem>
										                            <asp:ListItem>1</asp:ListItem>
										                        </asp:DropDownList>
													        </td>
													    </tr>													        							                
										            </table>
										        </td>										        
										    </tr>
								        </table>
							        </asp:Panel>							        
							    </td>
							</tr>
							<tr>
								<td>
								    <asp:Panel id="pnl_DettagliNodo" runat="server">
									<table class="contenitore" width="100%" border="0">
										<tr>
											<td class="titolo_pnl" vAlign="top">Dettagli nodo</td>
										</tr>
										<tr>
											<td>
												<table cellSpacing="1" cellPadding="3" width="100%" border="0">
												    <tr>
												        <td width="13%"><asp:label id="lbl_codiceInfo" runat="server" CssClass="testo_grigio_scuro" Width="100%">Codice *</asp:label></td>
												        <td width="30%">
												            <asp:textbox id="txt_codiceInfo" runat="server" CssClass="testo_grigio_scuro" Width="45%" MaxLength="64"></asp:textbox>
												            <asp:label id="lbl_codiceInfo_provv" runat="server" CssClass="testo_grigio_scuro" Visible="False" Width="5%" Text="."></asp:label>
												            <asp:textbox id="txt_codiceInfo_provv" runat="server" CssClass="testo" Visible="False" Width="45%"></asp:textbox>
												        </td>
												        <td width="20%" align="right"><asp:label id="lbl_descrizioneInfo" runat="server" CssClass="testo_grigio_scuro" Width="100%">Descrizione *</asp:label></td>
												        <td colspan="3"><asp:textbox id="txt_descrizioneInfo" runat="server" CssClass="testo" Width="100%" MaxLength="256"></asp:textbox></td>
												    </tr>
													<tr>
													    <td><asp:label id="lbl_registroInfo" runat="server" CssClass="testo_grigio_scuro" Width="100%">Registro</asp:label></td>
													    <td><asp:dropdownlist id="ddl_registriInfo" runat="server" CssClass="testo" Width="100%"></asp:dropdownlist></td>
													    <td align="right"><asp:label id="Label1" runat="server" CssClass="testo_grigio_scuro" Width="100%">Consenti creazione fascicoli</asp:label></td>
													    <td>
													        <asp:dropdownlist id="ddl_rw" CssClass="testo" Runat="server" Width="100%">
																<asp:ListItem Value="R">No</asp:ListItem>
																<asp:ListItem Value="W">Sì</asp:ListItem>
															</asp:dropdownlist>
														</td>
													    <td align="right"><asp:label id="lblNumeroMesiConservazione" runat="server" CssClass="testo_grigio_scuro" Width="100%">Mesi conservazione</asp:label></td>													   
													    <td><asp:textbox id="txtMesiConservazione" runat="server" CssClass="testo" Width="100%" MaxLength="6"></asp:textbox></td>													   													    
													</tr>
													<tr>
													    <td><asp:label id="lbl_noteNodo" runat="server" CssClass="testo_grigio_scuro" Width="100%">Note</asp:label></td>
													    <td><asp:TextBox ID="txt_noteNodo" runat="server" CssClass="testo" Width="100%"></asp:TextBox></td>
													    <td align="right"><asp:label id="lbl_creazioneFigli" runat="server" CssClass="testo_grigio_scuro" Width="100%">Blocca creazione figli</asp:label></td>
													    <td>
													        <asp:dropdownlist id="ddl_creazioneFigli" CssClass="testo" Runat="server" Width="100%">
																<asp:ListItem Value="NO">No</asp:ListItem>
																<asp:ListItem Value="SI">Si</asp:ListItem>
															</asp:dropdownlist>
														</td>
														<td colspan="2">&nbsp;&nbsp;
                                                            <asp:label id="lbl_classificazione" runat="server" CssClass="testo_grigio_scuro">Consenti classificazione</asp:label>&nbsp;
                                                            <asp:dropdownlist id="ddl_classificazione" CssClass="testo" Runat="server" AutoPostBack="true">
																<asp:ListItem Value="NO">No</asp:ListItem>
																<asp:ListItem Value="SI">Sì</asp:ListItem>
															</asp:dropdownlist>
                                                        </td>												
													</tr>
													<asp:Panel ID="pnl_ProtocolloTitolario" runat="server" Visible="false">
													<tr>
													    <td></td>
													    <td></td>
													    <td align="right"><asp:label id="lbl_attivaContatore" runat="server" CssClass="testo_grigio_scuro" Width="100%">Attiva contatore</asp:label></td>
												        <td>
															<asp:dropdownlist id="ddl_attivaContatore" CssClass="testo" Runat="server" Width="100%">
																<asp:ListItem Value="NO">No</asp:ListItem>
																<asp:ListItem Value="SI">Sì</asp:ListItem>
															</asp:dropdownlist>
														</td>
													    <td align="right"><asp:Label ID="lbl_protoTtitolario"	runat="server" CssClass="testo_grigio_scuro" Width="100%"></asp:label></td>													   
													    <td><asp:TextBox id="txt_protoTitolario" runat="server" CssClass="testo" Width="100%"  MaxLength="6"></asp:TextBox></td>
													</tr>
													</asp:Panel>    
													<asp:Panel ID="pnl_ProfilazioneFascicoli" runat="server" Visible="false">
													<tr>
													    <td><asp:label id="lbl_Tipologia" runat="server" CssClass="testo_grigio_scuro" Width="100%">Tipologia fascicoli</asp:label></td>
													    <td><asp:DropDownList ID="ddl_tipologiaFascicoli" runat="server" CssClass="testo" Width="100%"></asp:DropDownList></td>
													    <td align="right"><asp:label ID="lbl_bloccaTipologia" runat="server" CssClass="testo_grigio_scuro" Width="100%">Blocca tipologia fascicoli</asp:label></td>
													    <td><asp:CheckBox ID="cb_bloccaTipologia" runat="server" Width="100%"></asp:CheckBox></td>													    													    
													    <td></td>													   
													    <td></td>
													</tr>
													</asp:Panel>
												</table>
											</td>
										</tr>
									</table>
									</asp:Panel>									
								</td>
							</tr>
							<tr>
								<td>
							        <asp:Panel ID="pnl_PulsantiAmministrazione" runat="server">
							            <table>
										    <tr>
							                    <!--<td align="left" style="width:130px;"><asp:Label ID="lbl_OperazioniAmministrazione" runat="server" CssClass="testo_grigio_scuro">Op. Amministrazione :</asp:Label></td>-->
										        <td align="left" width="100%">
										            <asp:button id="btn_gestioneEtichetteTit" runat="server" CssClass="testo_btn" 
                                                        Text="Etichette Titolari" onclick="btn_gestioneEtichetteTit_Click"></asp:button>
											    </td>										        
										    </tr>
										</table>
								    </asp:Panel>
							    </td>
							</tr>
							<tr>
								<td>
							        <asp:Panel ID="pnl_PulsantiTitolario" runat="server">
							            <table>
										    <tr>
										        <!--<td align="left" style="width:130px;"><asp:Label ID="lbl_OperazioniTitolario" runat="server" CssClass="testo_grigio_scuro">Op. Titolario :</asp:Label></td>-->
										        <td align="left" width="100%">
										        <asp:button id="btn_NuovoTitolario" runat="server" CssClass="testo_btn_p_m" Text="Nuovo" OnClick="btn_NuovoTitolario_Click"></asp:button>
										        <asp:Button ID="btn_SalvaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Salva" OnClick="btn_SalvaTitolario_Click"></asp:button>
										        <asp:button id="btn_AttivaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Attiva" OnClick="btn_AttivaTitolario_Click"></asp:button>
										        <asp:button id="btn_EliminaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Elimina" OnClick="btn_EliminaTitolario_Click"></asp:button>
										        <asp:button id="btn_CopiaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Copia" OnClick="btn_CopiaTitolario_Click"></asp:button>
										        <asp:button id="btn_ImportaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Importa" OnClick="btn_ImportaTitolario_Click"></asp:button>
										        <asp:button id="btn_EsportaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Esporta" OnClick="btn_EsportaTitolario_Click"></asp:button>
										        <asp:button id="btn_VisibilitaTitolario" runat="server" CssClass="testo_btn_p_m" Text="Visibilità" OnClick="btn_VisibilitaTitolario_Click"></asp:button>
										        <asp:button id="btn_ImportaIndice" runat="server" CssClass="testo_btn_p_m" Text="Importa Ind." onclick="btn_ImportaIndice_Click"></asp:button>
										        <asp:button id="btn_EsportaIndice" runat="server" CssClass="testo_btn_p_m" Text="Esporta Ind." onclick="btn_EsportaIndice_Click"></asp:button>
										        </td>
										    </tr>
										</table>							        
							        </asp:Panel>
							    </td>
							</tr>
							<tr>
								<td>
								    <asp:Panel ID="pnl_PulsantiNodo" runat="server">
									    <table>
										    <tr>
										        <!--<td align="left" style="width:130px;"><asp:Label ID="lbl_OperazioniNodo" runat="server" CssClass="testo_grigio_scuro">Op. Nodo :</asp:Label></td>-->
											    <td align="left" width="100%">
											    <asp:button id="btn_aggiungiNew" runat="server" CssClass="testo_btn" Text="Aggiungi nodo figlio"></asp:button>
											    <asp:button id="btn_VisibilitaRuoli" runat="server" CssClass="testo_btn_p_m" Text="Visibilità"></asp:button>
											    <asp:button id="btn_elimina" runat="server" CssClass="testo_btn_p_m" Text="Elimina"></asp:button>
											    <asp:button id="btn_salvaInfo" runat="server" CssClass="testo_btn_p_m" Text="Modifica"></asp:button>
											    <asp:button id="btn_indiceSis" runat="server" CssClass="testo_btn_p_m" Text="Indice Sis."></asp:button>
											    </td>
										    </tr>
									    </table>
									</asp:Panel>
								</td>
							</tr>																					
						</table>
					</td>
				</tr>
				<tr>
				    <td>
				        <cc2:MessageBox id="messageBox_AttivaTitolario" runat="server"></cc2:messagebox>
				        <cc2:MessageBox id="messageBox_EliminaTitolario" runat="server"></cc2:messagebox>
				        <cc2:MessageBox id="messageBox_CopiaTitolario" runat="server"></cc2:messagebox>
                        <cc2:MessageBox id="messageBox_ClassificazioneNodiFiglio" runat="server"></cc2:messagebox>
				        <ax1:ShellWrapper ID="shellWrapper" runat="server" />
			            <ax2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
                        <ax3:FsoWrapper ID="fsoWrapper" runat="server" />
					</td>
				</tr>
				<!-- FINE CORPO CENTRALE --></table>				
		</form>
	</body>
</HTML>
