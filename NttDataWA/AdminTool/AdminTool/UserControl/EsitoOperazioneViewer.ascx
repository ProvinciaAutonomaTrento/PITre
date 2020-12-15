<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EsitoOperazioneViewer.ascx.cs" Inherits="SAAdminTool.AdminTool.UserControl.EsitoOperazioneViewer" %>
<asp:datagrid id="grdResult" runat="server" SkinID="datagrid" Width="97%" BorderWidth="1px"  BorderColor="Gray" CellPadding="1"
    AutoGenerateColumns="False" BorderStyle="Inset" >
    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
    <PagerStyle Mode="NumericPages" />
    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
    <ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>			
    <Columns>
        <asp:TemplateColumn HeaderText="Esito">
            <ItemTemplate>
                <asp:Label ID = "lblCodice" runat = "server" CssClass="testo_grigio_scuro" Text = '<%# this.GetCodice((SAAdminTool.DocsPaWR.EsitoOperazione) Container.DataItem) %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="15%" />
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Descrizione">
            <ItemTemplate>
                <asp:TextBox ID="txtDescrizione" Width="98%" runat="server" ReadOnly="true" CssClass="testo_grigio_scuro" TextMode= "MultiLine" Rows="3"  Wrap = "true" Text = '<%# this.GetDescrizione((SAAdminTool.DocsPaWR.EsitoOperazione) Container.DataItem) %>'></asp:TextBox>
            </ItemTemplate>
            <HeaderStyle Width="85%" />
        </asp:TemplateColumn>        
    </Columns>
    <HeaderStyle Wrap="False" Font-Bold="True" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
</asp:datagrid>