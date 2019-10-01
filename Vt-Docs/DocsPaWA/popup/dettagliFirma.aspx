<%@ Page language="c#" Codebehind="dettagliFirma.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.dettagliFirma" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" onblur="self.focus()">
		<form id="dettagliFirma" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Dettagli firma" />
			<TABLE id="tbl_dettagliCorrispondenti" class="info" width="430" align="center" border="0">
				<TR>
					<td colspan="2" class="item_editbox">
						<P class="boxform_item">
							<asp:Label id="Label1" runat="server" CssClass="menu_grigio_popup">Dettagli</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="middle">
						<asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" Width="95%" BorderColor="Gray" BorderWidth="1px" CellPadding="1" HorizontalAlign="Center" AllowPaging="True" PageSize="8" AutoGenerateColumns="False">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="Nome" HeaderText="Nome">
									<HeaderStyle Width="25%"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Cognome" HeaderText="Cognome">
									<HeaderStyle Width="25%"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CodiceFiscale" HeaderText="Codice Fiscale">
									<HeaderStyle Width="35%"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="DataNascita" HeaderText="Data di Nascita">
									<HeaderStyle Width="15%"></HeaderStyle>
								</asp:BoundColumn>
							</Columns>
							<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#c2c2c2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD height="5"></TD>
				</TR>
				<tr>
					<td align="middle" height="30">
						<asp:label id="lb_dettagli" runat="server" CssClass="testo_msg_grigio" Visible="False"></asp:label>
					</td>
				</tr>
				<TR>
					<TD align="middle" height="30">
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
