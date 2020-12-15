<%@ Page language="c#" Codebehind="insertNewNodoTitolario.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.insertNewNodoTitolario" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" id="btn_insert_click" event="onclick()" for="btn_insert">
			window.document.body.style.cursor='wait';
		</script>
		<script language="JavaScript">
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
		</script>		
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="myForm" name="myForm" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Inserimento nuovo nodo di titolario" />
			<input id="hd_from" type="hidden" name="hd_from" runat="server">
			<table class="info" height="100%" width="100%" align="center" border="0">
				<tr>
					<td class="item_editbox" height="20">
						<P class="boxform_item">Inserimento nuovo nodo di titolario</P>
					</td>
				</tr>
				<tr>
					<td vAlign="top" align="center">
						<table cellSpacing="2" cellPadding="0" width="97%" border="0">
							<tr>
								<td><DIV ID="divDescPadre" style="OVERFLOW: auto; WIDTH:395; height:100;"><asp:label id="lbl_padre" CssClass="testo_grigio" Runat="server"></asp:label></div></td>
							</tr>
							<tr>
								<td class="titolo_scheda">Codice *</td>
							</tr>
							<tr>
								<td>
								    <asp:textbox id="txt_codice_padre" ReadOnly="True" Width="80px" CssClass="titolo_scheda" Runat="server"></asp:textbox>
								    <b>.</b>
								    <asp:textbox id="txt_codice" Width="80px" CssClass="titolo_scheda" Runat="server" MaxLength="64"></asp:textbox>
								</td>
								
							</tr>
							<asp:Panel ID="pnl_protTitolario" runat="server" Visible="false">
							<tr>
							    <td>
								    <asp:Label ID="lbl_protTitolario" runat="server" CssClass="titolo_scheda"></asp:Label>
								</td>
							</tr>
							<tr>
							    <td>
							        <asp:TextBox ID="txt_protTitolario" runat="server" CssClass="titolo_scheda" Width="80px"></asp:TextBox>
								</td>
							</tr>
							</asp:Panel>
							<tr>
								<td class="titolo_scheda">Descrizione *</td>
							</tr>
							<tr>
								<td><asp:textbox id="txt_descrizione" Width="390" CssClass="titolo_scheda" Runat="server" MaxLength="256"></asp:textbox></td>
							</tr>
							<tr>
								<td class="titolo_scheda">Mesi di conservazione *</td>
							</tr>
							<tr>
							    <td><asp:textbox id="txt_mesi" Width="40px" CssClass="titolo_scheda" Runat="server" MaxLength="64"></asp:textbox></td>
							</tr>
							<asp:Panel ID="pnl_ProfilazioneFascicoli" runat="server" Visible="false">
							    <tr>
							        <td class="titolo_scheda">Tipologia fascicoli</td>
							    </tr>
							    <tr>    
							        <td><asp:DropDownList ID="ddl_tipologiaFascicoli" CssClass="testo_grigio" Width="285" runat="server"></asp:DropDownList></td>
							    </tr>
							    <tr>
							        <td class="titolo_scheda">Blocca tipologia fascicoli</td>
							    </tr>
							    <tr>    
							        <td><asp:CheckBox id="cb_bloccaTipoFascicoli" runat="server"></asp:CheckBox></td>
							    </tr>
							</asp:Panel>
							<tr style="padding-top:40px;">
								<td align="center">
								    <asp:button id="btn_insert" CssClass="pulsante_hand" Runat="server" Text="Inserisci" Width="80px"></asp:button>
									<asp:button id="btn_chiudi" CssClass="pulsante_hand" Runat="server" Text="Chiudi" Width="80px"></asp:button>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td valign="bottom" align="center"><asp:label id="lbl_nota" CssClass="testo_grigio" Runat="server"><u>Nota</u>: 
							L'operazione di inserimento potrebbe richiedere alcuni secondi</asp:label></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
