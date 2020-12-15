<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Destinatari.ascx.cs" Inherits="DocsPAWA.Spedizione.Destinatari" %>
<p align="center" class="boxform_item">
    <asp:Label ID="lblTitle" runat="server" ></asp:Label>
</p>
<br />
<input type="hidden" runat="server" id="txtCounterIncludiInSpedizione" />
<div style="overflow:auto; height:140px;" align="center">        
    <asp:datagrid id="grdSpedizioni" runat="server" SkinID="datagrid" Width="95%" BorderWidth="1px" 
        BorderColor="Gray" CellPadding="1" AutoGenerateColumns="False" 
        BorderStyle="Inset" onitemdatabound="DataGrid_ItemDataBound"  >
            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
            <PagerStyle Mode="NumericPages" />
            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
            <ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>			
            <Columns>
                <asp:TemplateColumn>
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkSelezionaDeseleziona" runat="server" 
                            Visible = "<%#this.VisibilitySelectDeselectRecipients() %>"
                            ToolTip = "Seleziona / deseleziona tutti" CssClass="testo_grigio_scuro" 
                            AutoPostBack="True" oncheckedchanged="chkSelezionaDeseleziona_CheckedChanged" />
                    </HeaderTemplate>
                    <HeaderStyle Width="3%" HorizontalAlign="Center" Font-Bold="False" 
                        Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
                        Font-Underline="False" />
                    <ItemTemplate>
                        <asp:CheckBox ID = "chkIncludiInSpedizione" runat="server" 
                        Checked = '<%#this.IncludiDestinatarioInSpedizione((DocsPAWA.DocsPaWR.Destinatario) Container.DataItem)%>'
                        Visible = '<%#this.IsDestinatarioValidoPerSpedizione((DocsPAWA.DocsPaWR.Destinatario) Container.DataItem)%>' />
                    </ItemTemplate>
                    <ItemStyle Width="3%" HorizontalAlign="Center" Font-Bold="False" 
                        Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
                        Font-Underline="False" VerticalAlign="Top" />
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="DataUltimaSpedizione" HeaderText="Inviato il" 
                    DataFormatString="{0:d}">
                    <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" />
                    <HeaderStyle Width="12%" Font-Bold="False" Font-Italic="False" 
                        Font-Overline="False" Font-Strikeout="False" Font-Underline="False" 
                        HorizontalAlign="Center" />
                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                        VerticalAlign="Middle" />
                </asp:BoundColumn>

                <asp:TemplateColumn HeaderText="Email" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label id="email" CssClass="testo_grigio" Visible='<%# this.IsVisibleFieldMail() %>' runat="server"></asp:Label>
                        <asp:DropDownList Width="99%" CssClass="testo_grigio" ID="ddl_caselle_corr_est" Visible='<%# this.IsVisibleListMail()%>' runat="server"></asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" VerticalAlign="Top" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Id Destinatario" Visible="False">
                    <ItemTemplate>
                        <asp:Label ID="lblIdDestinatario" runat="server" Text='<%# this.GetIdDestinatario((DocsPAWA.DocsPaWR.Destinatario) Container.DataItem)%>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" VerticalAlign="Top" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Destinatario">
                    <ItemTemplate>
                        <asp:Label ID = "lblDestinatario" runat = "server" Text = '<%# this.GetDescrizioneDestinatario((DocsPAWA.DocsPaWR.Destinatario) Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                        Width="25%" />
                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Left" 
                        VerticalAlign="Middle" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Stato">
                    <ItemTemplate>
                        <asp:Label ID="lblStatoSpedizione" runat="server" Text = '<%# this.GetStatoSpedizione((DocsPAWA.DocsPaWR.Destinatario) Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                        Width="8%" />
                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                        VerticalAlign="Middle" />
                </asp:TemplateColumn>
            </Columns>
            <HeaderStyle Wrap="False" Font-Bold="True" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
    </asp:datagrid>
</div>
