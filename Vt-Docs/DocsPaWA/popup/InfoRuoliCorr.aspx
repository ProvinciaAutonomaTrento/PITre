<%@ Page language="c#" Codebehind="InfoRuoliCorr.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.InfoRuoliCorr" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Info ruoli corr" />
			<TABLE class="info_grigio" id="Table1" cellSpacing="1" cellPadding="1" width="95%" align="center"
				border="0">
				<!--tr height="20">
					<td align="center" class="titolo_grigio">Dettagli Corrispondente:
						<asp:Label id="lbl_nomeCorr" runat="server"></asp:Label>
					</td>
				</tr-->
				<TR>
					<TD align="center"><asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" Width="95%" BorderColor="Gray" AutoGenerateColumns="False"
							BorderWidth="1px" PageSize="6" CellPadding="1">
							<FooterStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></FooterStyle>
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ruolo_cod_rubrica" HeaderText="Cod. Rubrica">
									<ItemStyle Height="25px" Width="15%"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ruolo_desc" HeaderText="Descr. Ruolo">
									<ItemStyle Height="25px" Width="30%"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="descrizioneUO" HeaderText="Descr. UO">
									<ItemStyle Height="25px" Width="40%"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="cha_preferito" HeaderText="Preferito">
									<ItemStyle Height="25px" Width="15%"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn>
									<ItemStyle Height="25px" Width="5%" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton1" runat="server" ImageUrl="../images/ico_visto.gif" ImageAlign="Middle"
											AlternateText="Ruolo principale"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
