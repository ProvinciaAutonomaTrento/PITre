<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="HistoryPreserved.aspx.cs" Inherits="NttDataWA.Popup.HistoryPreserved" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <fieldset>
        <div class="container">
            <asp:Panel ID="GridViewHistoryPnl" runat="server">
                <asp:UpdatePanel ID="UpGrid" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div>
                            <p>
                                <span class="weight">
                                    <asp:Label runat="server" ID="HistroyPreservedLblCaption"></asp:Label></span></p>
                        </div>
                        <asp:GridView ID="GridViewHistory" runat="server" AutoGenerateColumns="false"
                            Width="99%" HeaderStyle-CssClass="tableHeaderPopup" CssClass="tbl" OnRowDataBound="RowSelected_RowCommand">
                            <SelectedRowStyle CssClass="selectedrow" />
                            <RowStyle CssClass="NormalRow" />
                            <AlternatingRowStyle CssClass="AltRow" />
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>                                   
                                        <asp:Label runat="server" ID="lnkCodice0" Text='<%# Bind("ID_Istanza") %>' ToolTip='<%# Bind("Tooltip0")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lnkCodice1" Text='<%# Bind("Descrizione") %>' ToolTip='<%# Bind("Tooltip1")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lnkCodice2" Text='<%# Bind("Data_Conservazione") %>'
                                            ToolTip='<%# Bind("Tooltip2")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lnkCodice3" Text='<%# Bind("Utente") %>' ToolTip='<%# Bind("Tooltip3")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lnkCodice4" Text='<%# Bind("Collocazione") %>' ToolTip='<%# Bind("Tooltip4")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lnkCodice5" Text='<%# Bind("Tipo_Conservazione") %>'
                                            ToolTip='<%# Bind("Tooltip5")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                                
                                 <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="false">
                                    <ItemTemplate >
                                        <asp:Label runat="server" ID="lnkCodice6" Text='<%# Bind("Num_Doc_In_Fasc") %>'
                                            ToolTip='<%# Bind("Tooltip6")%>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="Comunication"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="LblIdSel" Text='<%# Bind("IdSel") %>'> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:TextBox ID="grid_rowindex" runat="server" CssClass="hidden" OnTextChanged="SetSelectedRow"
                            AutoPostBack="true" ClientIDMode="Static"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <asp:Panel ID="HistroyPreservedPnlNotFound" runat="server" Visible="false">
                <asp:Label ID="HistroyPreservedLblNotFound" runat="server"></asp:Label>
            </asp:Panel>
        </div>
    </fieldset>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpPnlButtons" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div style="float: left">
                <cc1:CustomButton ID="HistoryPreservedBtnChiudi" runat="server" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="HistoryPreservedBtnChiudi_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
