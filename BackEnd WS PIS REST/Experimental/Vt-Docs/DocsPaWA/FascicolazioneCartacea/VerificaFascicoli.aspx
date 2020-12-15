<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificaFascicoli.aspx.cs" Inherits="DocsPAWA.FascicolazioneCartacea.VerificaFascicoli" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <base target="_self" />
    <script type="text/javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
</head>
<body bottomMargin="1" leftMargin="1" topMargin="4" rightMargin="1">
    <form id="frmVerificaFascicoli" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Allineamento archivio cartaceo" />
        <div align="center">
            <table class="info" id="tblContainer" cellSpacing="0" cellPadding="5" width="400" align="center"
				border="0" runat="server">
				<tr>
					<td>
				        <asp:label id="lblTitle" CssClass="testo_grigio" runat="server">Fascicoli cartacei in cui risulta già presente il documento</asp:label>
					</td>
				</tr>
                <tr>
                    <td align=left>
                        <div style="overflow: auto; height: 150px">
                            <asp:DataGrid ID="grdFascicoliCartacei" runat="server" SkinID="datagrid" AllowCustomPaging="True"
                                AutoGenerateColumns="False" BorderColor="Gray" BorderStyle="Inset" BorderWidth="1px"
                                CellPadding="1" HorizontalAlign="Center"
                                Width="95%">
                                <SelectedItemStyle CssClass="bg_grigioS" />
                                <AlternatingItemStyle CssClass="bg_grigioA" />
                                <ItemStyle CssClass="bg_grigioN" Height="20px" />
                                <HeaderStyle CssClass="menu_1_bianco_dg" ForeColor="White" Height="20px"
                                    Wrap="False" />
                                <Columns>
                                    <asp:BoundColumn DataField="IdFascicolo" Visible="False"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="CodiceFascicolo" HeaderText="Codice">
                                        <HeaderStyle Width="30%" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="DescrizioneFascicolo" HeaderText="Descrizione">
                                        <HeaderStyle Width="70%" />
                                    </asp:BoundColumn>
                                </Columns>
                                
                            </asp:DataGrid></div>
                    </td>
                </tr>
            </table>
            <br />
            
            <asp:imagebutton id="btnClose" ImageUrl="../images/proto/btn_chiudi_little_attivo.gif" Runat="server" AlternateText="Chiudi" />
        </div>
    </form>
</body>
</html>
