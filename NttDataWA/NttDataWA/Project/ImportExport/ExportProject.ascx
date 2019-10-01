<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportProject.ascx.cs" Inherits="NttDataWA.ImportExport.ExportProject" %>
<table id="tableLog" width="100%" cellpadding="2" cellspacing="0">
    <tr style="background-color: Silver;">
        <td align="left" height="90%">
            <asp:Label ID="titolo" runat="server" CssClass="titolo">Log dell'esportazione</asp:Label>
        </td>
        <td class="testo_grigio_scuro" valign="middle" align="right" height="10%">
            <asp:ImageButton ID="btn_stampa" Visible="true" runat="server" AlternateText="Esporta il log delle operazioni."
                ImageUrl="Images/export.gif" OnClientClick="lanciaVisPdf();"></asp:ImageButton>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div style="overflow: auto; height: 206px;">
                <asp:DataGrid ID="dgrLog" runat="server" BorderColor="Gray" AutoGenerateColumns="false"
                    BorderWidth="1px" CellPadding="1" AllowPaging="false" Width="100%">
                    <ItemStyle CssClass="bg_grigioN" />
                    <SelectedItemStyle CssClass="bg_grigioS" />
                    <AlternatingItemStyle CssClass="bg_grigioA" />
                    <HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg" BackColor="#810D06" />
                    <Columns>
                        <asp:BoundColumn DataField="Type" HeaderText="Tipo" ItemStyle-Width="10%" ReadOnly="true"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundColumn DataField="Path" HeaderText="Percorso" ItemStyle-Width="55%" ReadOnly="true"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundColumn DataField="Result" HeaderText="Esito" ItemStyle-Width="15%" ReadOnly="true"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundColumn DataField="Notes" HeaderText="Note" ItemStyle-Width="20%"
                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Red" />
                    </Columns>
                </asp:DataGrid>
            </div>
        </td>
    </tr>
</table>
