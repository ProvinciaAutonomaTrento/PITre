<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssociaRuolo.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamica.AssociaRuolo" validateRequest="false"%>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head runat = "server">
        <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	<base target="_self" />
</head>
<body MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Associazione Ruolo - Oggetto Custom" />
        <table width="100%">
		<tr>
			<td class="titolo"  align="center" bgColor="#e0e0e0" height="20">
				<asp:Label id="lbl_titolo" runat="server"></asp:Label></td>
			<td align="center" width="30%" bgColor="#e0e0e0">
                <asp:Button ID="btn_selezione" runat="server" CssClass="testo_btn_p" Text="Sel. Tutti" OnClick="btn_selezione_Click" />&nbsp;
                <asp:Button id="btn_conferma" runat="server" Text="Conferma" CssClass="testo_btn_p" OnClick="btn_conferma_Click"></asp:Button></td>
		</tr>
		<!-- Ricerca ruoli per descrizione   -->
		<tr>
		    <td colspan="2" class="testo_piccoloB">Ricerca per:&nbsp;&nbsp;
			    <asp:DropDownList id="ddl_ricTipo" Runat="server" CssClass="testo_grigio_scuro" 
                    AutoPostBack="True" onselectedindexchanged="ddl_ricTipo_SelectedIndexChanged">
                    <asp:ListItem Value="T">Tutti</asp:ListItem>
				    <asp:ListItem Value="C">Codice</asp:ListItem>
					<asp:ListItem Value="D">Descrizione</asp:ListItem>
				</asp:DropDownList>&nbsp;&nbsp;
			    <asp:textbox id="txt_ricerca"  CssClass="testo_grigio_scuro_grande" Runat="server" Width="232px" Enabled=false></asp:textbox>&nbsp;
				<asp:button id="btn_find"  CssClass="testo_btn" Runat="server" Text="Cerca" OnClick="btn_find_Click"></asp:button>
			</td>
		</tr>
	    <tr>
	        <td colspan="2">
	            <asp:label id="lbl_ricerca" runat="server" CssClass="titolo"></asp:label>
			</td>
	    </tr>
		<tr>
		    <td colspan="2"></td>
		</tr>
		<tr>
			<td colspan="2">
			<DIV id="div_listaRuoli" style="OVERFLOW: auto" visible=true>
			<asp:DataGrid id="dg_Ruoli" runat="server" AutoGenerateColumns="False" Width="95%" Visible="true">
					<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					<ItemStyle CssClass="bg_grigioN"></ItemStyle>
					<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
					<Columns>
						<asp:BoundColumn Visible="False" DataField="ID_GRUPPO" HeaderText="id_gruppo"></asp:BoundColumn>
						<asp:BoundColumn DataField="DESCRIZIONE RUOLO"  HeaderText="Descrizione Ruolo">
							<HeaderStyle Width="60%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Inserimento">
							<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox id="CheckBox1" runat="server" AutoPostBack="True"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>
				</DIV>
			</td>
		</table>   	
    </form>
</body>
</html>
