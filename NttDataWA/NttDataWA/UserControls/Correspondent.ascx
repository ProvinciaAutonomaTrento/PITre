<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Correspondent.ascx.cs" Inherits="NttDataWA.UserControls.Correspondent" %>

<input type="hidden" runat="server" id="txtCounterIncludiInSpedizione" />
 
    <asp:GridView ID="new_grdSpedizioni" AutoGenerateColumns="False" 
            Width="100%" Height="50%" OnRowDataBound="DataGrid_ItemDataBound" runat="server" CssClass="tbl_rounded">
        <AlternatingRowStyle CssClass="bg_alternateA" />
        <SelectedRowStyle CssClass="bg_selectedItem" />
        <PagerStyle CssClass="NumericPages" />
        <Columns>
            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelezionaDeseleziona" runat="server" 
                                Visible = 'true' oncheckedchanged="chkSelezionaDeseleziona_CheckedChanged" AutoPostBack="true"
                                ToolTip = "" CssClass="grey_text" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID = "chkIncludiInSpedizione" runat="server" 
                            Checked = 'false'
                            Visible = 'false' />
                </ItemTemplate>
                <HeaderStyle Width="3%" HorizontalAlign="Center" Font-Bold="False" 
                    Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
                    Font-Underline="False" />
                <ItemStyle Width="3%" HorizontalAlign="Center" Font-Bold="False" 
                    Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
                    Font-Underline="False" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:BoundField DataField="DataUltimaSpedizione" HeaderText="" 
                DataFormatString="{0:d}">
                <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" />
                <HeaderStyle Width="12%" Font-Bold="False" Font-Italic="False" 
                    Font-Overline="False" Font-Strikeout="False" Font-Underline="False" 
                    HorizontalAlign="Center" />
                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                    VerticalAlign="Middle" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:Label ID="email" CssClass="grey_text_bold" Visible="false" runat="server"></asp:Label>
                    <asp:DropDownList Width="99%" ID="ddl_caselle_corr_est" Visible="false" runat="server"></asp:DropDownList>
                </ItemTemplate>
                <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                    Width="25%" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Id Destinatario" Visible="False">
                <ItemTemplate>
                    <asp:Label ID="lblIdDestinatario" runat="server" Text=''></asp:Label>
                </ItemTemplate>
                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:Label ID = "lblDestinatario" runat = "server" Text=''></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                    Width="25%" />
                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Left" 
                    VerticalAlign="Middle" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:Label ID="lblStatoSpedizione" runat="server" Text = ''></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                    Width="8%" />
                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                    Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                    VerticalAlign="Middle" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>



 