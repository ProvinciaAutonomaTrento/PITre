<%@ Page language="c#" Codebehind="RisultatoRicercaOrg.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.organigramma.RisultatoRicercaOrg" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > Risultato ricerca in Organigramma</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../AdminTool/CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table width="740" align="center">
				<tr>
					<!-- TITOLO PAGINA -->
					<td>
						<asp:Label CssClass="label_percorso" id="lbl_percorso" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" height="2">&nbsp;</td>
				</tr>
				<tr>
					<td><asp:label id="lbl_ContaRec" CssClass="testo" Runat="server"></asp:label></td>
				</tr>
				<tr>
					<td><asp:panel id="pnl_risultato" runat="server" Visible="true">
							<DIV style="OVERFLOW: auto; HEIGHT: 223px">
								<asp:DataGrid id="dg_listaRicerca" runat="server" SkinID="datagrid" BorderColor="Gray" BorderWidth="1px" CellPadding="1"
									AutoGenerateColumns="False" Width="100%">
									<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
									<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
									<ItemStyle CssClass="bg_grigioN"></ItemStyle>
									<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
									<Columns>
										<asp:BoundColumn Visible="False" DataField="IDCorrGlob" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
										<asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice">
											<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
											<ItemStyle Width="20%"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="Descrizione" ReadOnly="True">
											<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
											<ItemStyle Width="40%"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn Visible="False" DataField="IDParent" ReadOnly="True" HeaderText="IDParent"></asp:BoundColumn>
										<asp:BoundColumn DataField="DescParent" ReadOnly="True">
											<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
											<ItemStyle Width="40%"></ItemStyle>
										</asp:BoundColumn>
									</Columns>
								</asp:DataGrid></DIV>
						</asp:panel></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
