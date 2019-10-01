<%@ Page language="c#" Codebehind="Utenti_TipoRuolo.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_Ruoli.Utenti_TipoRuolo" %>
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
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Utenti presenti nel ruolo" />
			<table width="690" align="center">
				<tr>
					<!-- OPZIONI DI TESTA -->
					<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><IMG height="48" src="../Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td><asp:label id="lbl_info" tabIndex="1" runat="server" CssClass="label_percorso"></asp:label></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" height="2">&nbsp;</td>
				</tr>
				<tr>
					<td>
						<!-- RISULTATO DELLA RICERCA -->
						<DIV style="OVERFLOW: auto; HEIGHT: 331px">
							<asp:datagrid id="dg_utenti" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
								AutoGenerateColumns="False" Width="100%">
								<SelectedItemStyle HorizontalAlign="Left" CssClass="bg_grigioS"></SelectedItemStyle>
								<EditItemStyle HorizontalAlign="Left"></EditItemStyle>
								<AlternatingItemStyle HorizontalAlign="Left" CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle HorizontalAlign="Left" CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								<Columns>
									<asp:BoundColumn DataField="utente" ReadOnly="True" HeaderText="Utenti">
										<ItemStyle HorizontalAlign="Left" Width="40%" VerticalAlign="Top"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="ruolo" HeaderText="Ruolo in UO">
										<ItemStyle HorizontalAlign="Left" Width="60%" VerticalAlign="Top"></ItemStyle>
									</asp:BoundColumn>
								</Columns>
							</asp:datagrid>
						</DIV>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
