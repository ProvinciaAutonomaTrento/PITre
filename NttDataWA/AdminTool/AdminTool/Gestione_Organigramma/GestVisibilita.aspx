<%@ Page language="c#" Codebehind="GestVisibilita.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_Organigramma.GestVisibilita" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" id="btn_esegui_click" event="onclick()" for="btn_esegui">
			if (!window.confirm('Attenzione,\nla procedura utilizza in modo esclusivo alcune tabelle della base dati,\npertanto la sua esecuzione richiede che l\'applicazione DocsPA non sia utilizzata.\n\nInoltre, vista la complessità delle operazioni da svolgere,\nil sistema potrebbe richiedere l\'attesa di un tempo piuttosto lungo.\n\nAvviare ora la procedura?')) {return false};
			//document.frm_GestVisibilita.btn_esegui.disabled=true;
			//document.frm_GestVisibilita.btn_chiudi.disabled=true;
			window.document.body.style.cursor='wait';
			document.getElementById("WAIT").style.visibility = "Visible";
		</script>
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout"
		onload="window.document.body.style.cursor='default'">
		<form id="frm_GestVisibilita" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Visibilità documenti" />	
			<input type="hidden" id="from" name="from" runat="server">
			<table width="540" align="center">
				<!--
				<tr>					
					<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>				
				<tr>
					<td align="left" height="48"><IMG height="48" src="../Images/logo.gif" width="218" border="0"></td>
				</tr>
				-->
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<td>
						<asp:label id="lbl_intestazione" tabIndex="3" runat="server" CssClass="testo">
							Selezionare uno o più registri ed avviare la procedura per estendere la visibilità al ruolo sui <b>
								documenti</b>, sul <b>titolario</b> e sui <b>fascicoli</b> appartenenti ai registri selezionati.
						</asp:label>
					</td>
				</tr>
                <tr>
                    <td><asp:CheckBox ID="cb_atipicita" Visible="false" runat="server" Checked="false" Text="Escludi documenti / fascicoli atipici" CssClass="testo_grigio_scuro"/></td>
                </tr>
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<td>
						<DIV style="OVERFLOW: auto; HEIGHT: 235px"><asp:datagrid id="dg_registri" runat="server" Width="100%" BorderColor="Gray" BorderWidth="1px"
								CellPadding="1" AutoGenerateColumns="False" Height="59px">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								<Columns>
									<asp:BoundColumn Visible="False" DataField="IDRegistro" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
									<asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Registri associati al ruolo">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
										<ItemStyle Width="80%"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm"></asp:BoundColumn>
									<asp:BoundColumn Visible="False" DataField="idCorrGlobUO" HeaderText="idCorrGlobUO"></asp:BoundColumn>
									<asp:BoundColumn Visible="False" DataField="idCorrGlobRuolo" HeaderText="idCorrGlobRuolo"></asp:BoundColumn>
									<asp:BoundColumn Visible="False" DataField="idGruppo" HeaderText="idGruppo"></asp:BoundColumn>
									<asp:TemplateColumn HeaderText="">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"></ItemStyle>
										<ItemTemplate>
											<asp:CheckBox ID="Chk_registri" Checked="False" runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn ReadOnly="True" HeaderText="Esito">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"></ItemStyle>
									</asp:BoundColumn>
								</Columns>
							</asp:datagrid></DIV>
					</td>
				</tr>
				<tr>
					<td height="3"></td>
				</tr>
				<tr>
					<td align="center"><asp:button id="btn_esegui" tabIndex="1" runat="server" CssClass="testo_btn_org" Text="Avvia procedura"></asp:button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:button id="btn_chiudi" tabIndex="2" runat="server" CssClass="testo_btn_org" Text="Chiudi"></asp:button></td>
				</tr>
			</table>
			<DIV id="WAIT" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; LEFT: 50px; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; TOP: 140px; BACKGROUND-COLOR: #ffefd5">
				<table border="0" cellSpacing="0" cellPadding="0" width="450" height="170">
					<tr>
						<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">
							Procedura in esecuzione...<br>
							<br>
							<br>
							Si raccomanda di attendere la fine della procedura<br>
							e<br>
							verificare l'esito delle operazioni.
						</td>
					</tr>
				</table>
			</DIV>
		</form>
	</body>
</HTML>
