<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DirittiRuoloOggettoCustomFasc.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc.DirittiRuoloOggettoCustomFasc" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	<base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Visibilita' Tipologia Fascicoli" />
    <table width="100%">
		<tr>
			<td class="titolo"  align="left" bgColor="#e0e0e0" height="20">
				<asp:Label id="lbl_titolo" runat="server"></asp:Label>				
			</td>
			<td align="right" bgColor="#e0e0e0" width="17%" style="padding-right:13px;">
			    <asp:Button id="btnChiudi" runat="server" Text="Chiudi" CssClass="testo_btn_p_large" OnClientClick="window.close();"></asp:Button>             
			</td>
		</tr>
		<!-- Lista Ruoli -->
		<tr>
			<td colspan="2">
		    <DIV id="div_listaRuoli" align="center" runat="server" style="OVERFLOW: auto; HEIGHT: 350px; width:100%;">
		    <asp:DataGrid id="dg_Ruoli" runat="server" AutoGenerateColumns="False" Width="100%" Visible="true">
                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
				<ItemStyle CssClass="bg_grigioN"></ItemStyle>
				<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="DESCRIZIONE RUOLO"  HeaderText="Descrizione Ruolo" ItemStyle-HorizontalAlign="Left">
						<HeaderStyle Width="60%"></HeaderStyle>
					    <ItemStyle HorizontalAlign="Left" />
					</asp:BoundColumn>
					<asp:BoundColumn DataField="MODIFICA"  HeaderText="Modifica" ItemStyle-HorizontalAlign="Center">
						<HeaderStyle Width="20%"></HeaderStyle>
					    <ItemStyle HorizontalAlign="Center" />
					</asp:BoundColumn>
					<asp:BoundColumn DataField="VISIBILITA"  HeaderText="Visibilità" ItemStyle-HorizontalAlign="Center">
						<HeaderStyle Width="20%"></HeaderStyle>
					    <ItemStyle HorizontalAlign="Center" />
					</asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
			</DIV>
			</td>
		</tr>		
	</table>		
    </form>
</body>
</html>
