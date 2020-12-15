<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<%@ Page language="c#" Codebehind="FiltriRicercaDocumenti.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.FiltriRicercaDocumenti" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<SCRIPT language="JavaScript">	
			window.name="DialogFiltriDocumento";			
			var dialogRetValue=window.dialogArguments;					
		</SCRIPT>
		<script language="javascript">
	
			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-100)/2;
			var new_h = (h-400)/2;
			
			function apriPopupAnteprima() {
					//window.open('../documento/AnteprimaProfDinRicerche.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');
					window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicerche.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
			}		
			
			
			function CloseWindow(retValue)
			{
				window.returnValue=retValue;
				window.close();
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
			
			// Gestione visualizzazione maschera rubrica
			function ShowDialogRubrica()
			{
				var w_width = screen.availWidth - 40;
				var w_height = screen.availHeight - 35;
				
				var navapp = navigator.appVersion.toUpperCase();
				if ((navapp .indexOf("WIN") != -1) && (navapp .indexOf("NT 5.1") != -1))
					w_height = w_height + 20;
				
				var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
				
				var params="calltype=";
				if (document.frmFiltriRicercaDocumenti.txtRubricaCallTypeCorrInt.value=="true")
					params += Rubrica.prototype.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT;
				else
					params += Rubrica.prototype.CALLTYPE_RICERCA_DOCUMENTI;
				
				var urlRubrica="../popup/rubrica/Rubrica.aspx";
				var res=window.showModalDialog (urlRubrica + "?" + params,window,opts);				
			}
						
		</script>
	</HEAD>
	<body language="javascript" topMargin="5">
		<form id="frmFiltriRicercaDocumenti" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca Documenti" />
			<table class="info_grigio" id="tblContainer" height="100%" cellSpacing="0" cellPadding="0"
				width="95%" align="center" border="0" runat="server">
				<tr>
					<td>
						<DIV id="pnlContainer" style="OVERFLOW: auto; WIDTH: 99.95%; HEIGHT: 187px" runat="server">
							<table class="testo_grigio" id="tblTipoDocumento" height="34" cellSpacing="0" cellPadding="0"
								width="95%" align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" width="20%"><asp:label id="lblTipoDocumento" runat="server">Tipo documento:</asp:label></TD>
									<TD class="testo_grigio" width="80%"><asp:radiobuttonlist id="rbListTipoDocumento" runat="server" Width="100%" AutoPostBack="True" RepeatDirection="Horizontal"
											CssClass="titolo_scheda" CellPadding="0" CellSpacing="0"></asp:radiobuttonlist></TD>
								</tr>
							</table>
							<table class="testo_grigio" id="tblIDDocumento" height="25" cellSpacing="0" cellPadding="0"
								width="95%" align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" width="20%"><asp:label id="lblIDDocumento" runat="server">Id Documento:</asp:label></TD>
									<TD class="titolo_scheda" width="20%"><asp:dropdownlist id="cboTypeIDDocumento" runat="server" Width="100%" AutoPostBack="True" CssClass="testo_grigio"></asp:dropdownlist></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblInitIDDocumento" runat="server" CssClass="titolo_scheda">Da:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtInitIDDocumento" runat="server" Width="80px" CssClass="testo_grigio"></asp:textbox></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblEndIDDocumento" runat="server" CssClass="titolo_scheda">a:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtEndIDDocumento" runat="server" Width="80px" CssClass="testo_grigio"></asp:textbox></TD>
									<TD class="titolo_scheda" align="right" width="20%"></TD>
								</tr>
							</table>
							<table class="testo_grigio" id="tblDataCreazioneDocumento" height="25" cellSpacing="0"
								cellPadding="0" width="95%" align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" width="20%"><asp:label id="lblDataCreazione" runat="server">Data creazione:</asp:label></TD>
									<TD class="titolo_scheda" width="20%"><asp:dropdownlist id="cboTypeDataCreazione" runat="server" Width="100%" AutoPostBack="True" CssClass="testo_grigio"></asp:dropdownlist></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblInitDataCreazione" runat="server" CssClass="titolo_scheda">Da:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="20%"><uc3:Calendario id="txtInitDataCreazione" runat="server"></uc3:Calendario></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblEndDataCreazione" runat="server" CssClass="titolo_scheda">a:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="20%"><uc3:Calendario id="txtEndDataCreazione" runat="server"></uc3:Calendario></TD>
									<TD class="titolo_scheda" align="right" width="10%"></TD>
								</tr>
							</table>
							<table class="testo_grigio" id="tblNumeroProtocollo" height="25" cellSpacing="0" cellPadding="0"
								width="95%" align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" width="20%"><asp:label id="lblNumeroProtocollo" runat="server">Num. protocollo:</asp:label></TD>
									<TD class="titolo_scheda" width="20%"><asp:dropdownlist id="cboTypeNumProtocollo" runat="server" Width="100%" AutoPostBack="True" CssClass="testo_grigio"></asp:dropdownlist></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblInitNumProtocollo" runat="server" CssClass="titolo_scheda">Da:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtInitNumProtocollo" runat="server" Width="80px" CssClass="testo_grigio"></asp:textbox></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblEndNumProtocollo" runat="server" CssClass="titolo_scheda">a:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtEndNumProtocollo" runat="server" Width="80px" CssClass="testo_grigio"></asp:textbox></TD>
									<TD class="titolo_scheda" align="right" width="20%"><asp:label id="lblAnnoProtocollo" runat="server">Anno:</asp:label>&nbsp;&nbsp;
										<asp:textbox id="txtAnnoProtocollo" runat="server" Width="50px" CssClass="testo_grigio"></asp:textbox></TD>
								</tr>
							</table>
							<table class="testo_grigio" id="tblDataProtocollo" height="25" cellSpacing="0" cellPadding="0"
								width="95%" align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" width="20%"><asp:label id="lblDataProtocollo" runat="server">Data protocollo:</asp:label></TD>
									<TD class="titolo_scheda" width="20%"><asp:dropdownlist id="cboTypeDataProtocollo" runat="server" Width="100%" AutoPostBack="True" CssClass="testo_grigio"></asp:dropdownlist></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblInitDataProtocollo" runat="server" CssClass="titolo_scheda">Da:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="20%"><uc3:Calendario id="txtInitDataProtocollo" runat="server"></uc3:Calendario></TD>
									<TD class="testo_grigio" align="center" width="5%"><asp:label id="lblEndDataProtocollo" runat="server" CssClass="titolo_scheda">a:</asp:label></TD>
									<TD class="testo_grigio" align="left" width="20%"><uc3:Calendario id="txtEndDataProtocollo" runat="server"></uc3:Calendario></TD>
									<TD class="titolo_scheda" align="right" width="10%"></TD>
								</tr>
							</table>
							<table class="testo_grigio" id="tblOggetto" cellSpacing="0" cellPadding="0" width="95%"
								align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" vAlign="top" width="20%" style="HEIGHT: 42px"><asp:label id="lblOggetto" runat="server">Oggetto:</asp:label></TD>
									<TD class="testo_grigio" width="80%" style="HEIGHT: 42px"><asp:textbox id="txtOggetto" runat="server" Width="100%" CssClass="testo_grigio" TextMode="MultiLine"></asp:textbox></TD>
								</tr>
							</table>
							<table class="testo_grigio" id="tblMittenteDestinatario" height="25" cellSpacing="0" cellPadding="0"
								width="95%" align="center" border="0" runat="server">
								<tr>
									<TD class="titolo_scheda" width="20%"><asp:label id="lblMittDest" runat="server">Mitt. / Dest.:</asp:label></TD>
									<TD class="testo_grigio" width="80%"><asp:textbox id="txtCodMittDest" runat="server" Width="15%" AutoPostBack="True" CssClass="testo_grigio"></asp:textbox><asp:textbox id="txtDescrMittDest" runat="server" Width="78%" CssClass="testo_grigio"></asp:textbox>
										<asp:Image id="btnRubrica" runat="server" ImageUrl="../images/proto/rubrica.gif" Height="20px"
											Width="20px"></asp:Image></TD>
								</tr>
							</table>
							<table id="tblTipologia" class="testo_grigio" cellspacing="0" cellpadding="0" width="95%" align="center" border="0" runat="server">
								<tr>
									<td class="titolo_scheda" valign="middle" height="19" width="20%" style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipologia doc.</td>
									<td class="testo_grigio" width="80%">
										<asp:dropdownlist id="ddl_tipoDoc" runat="server" Width="200px" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
										<asp:imagebutton id="btn_CampiPersonalizzati" runat="server" ImageUrl="../images/proto/ico_oggettario.gif"></asp:imagebutton>
									</td>
								</tr>
							</table>
														<table id="tblFileAcquisito" class="testo_grigio" cellspacing="0" cellpadding="0" width="95%" align="center" border="0" runat="server">
							<tr>
							<td class="titolo_scheda" valign="middle" height="19" width="20%" style="PADDING-BOTTOM: 5px; PADDING-TOP: 5px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipo file acquisito</td>
							<td class="testo_grigio" width="20%">
							<asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="testo_grigio">
							                <asp:ListItem></asp:ListItem>
							                </asp:DropDownList>
							 </td>
							 <td><asp:CheckBox ID="cb_firmato" runat="server" CssClass="titolo_scheda"  Text="Firmato"/></td>
							 <td><asp:CheckBox ID="cb_nonFirmato" runat="server" CssClass="titolo_scheda" Text="Non firmato" /></td>
							</tr>
							</table>
						</DIV>
					</td>
				</tr>
				<tr>
					<td>
						<TABLE class="testo_grigio" id="tblButtonsContainer" height="25" cellSpacing="0" width="150"
							align="center" border="0" runat="server">
							<TR>
								<TD class="titolo_scheda" align="center"><asp:button id="btnOK" runat="server" Width="80px" CssClass="pulsante" Text="OK"></asp:button></TD>
								<TD class="titolo_scheda" align="center"><asp:button id="btnClose" runat="server" Width="80px" CssClass="pulsante" Text="Chiudi"></asp:button></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
			</table>
			<INPUT id="txtSystemIDMittDest" type="hidden" runat="server"> <INPUT id="txtRubricaCallTypeCorrInt" type="hidden" runat="server">
		</form>
	</body>
</HTML>
