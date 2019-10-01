<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CampiComuni.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc.CampiComuni" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	<base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Campi Comuni" />
        <table width="100%">
            <tr>
				<td class="titolo"  align="left" bgColor="#e0e0e0" height="20">
					<asp:Label id="lbl_titolo" runat="server"></asp:Label></td>
				<td align="center" width="40%" bgColor="#e0e0e0">
                    <asp:Button ID="btn_chiudi" runat="server"  Text="Chiudi" CssClass="testo_btn_p" OnClick="btn_chiudi_Click"/>&nbsp;
                    <asp:Button id="btn_conferma" runat="server" Text="Conferma" CssClass="testo_btn_p" OnClick="btn_conferma_Click"/></td>
			</tr>
			<tr>
			    <td colspan="2">
			        <div id="gw_listaCampiComuni" style="OVERFLOW: auto; HEIGHT: 320px; width:100%;">
                        <br/>
                        <asp:GridView ID="gw_CampiComuni" runat="server" AutoGenerateColumns="False" Width="100%">
                            <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                            <RowStyle CssClass="bg_grigioN"></RowStyle>
			                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></HeaderStyle>
			                <Columns>
                                <asp:BoundField DataField="ID_CAMPO" HeaderText="ID_CAMPO" />
                                <asp:BoundField DataField="DESCRIZIONE" HeaderText="DESCRIZIONE">
                                    <HeaderStyle Width="80%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="SELEZIONE">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cb_selezione" runat="server" AutoPostBack="True" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="20%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>			        
			        </div>
			    </td>
			</tr>
        </table>
    </form>
</body>
</html>
