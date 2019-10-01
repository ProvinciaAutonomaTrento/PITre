<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="History.aspx.cs" Inherits="NttDataWA.Popup.History" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:Panel ID="GridViewDocHistoryPnl" runat="server">
            <asp:UpdatePanel ID="UpGridHistory" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div>
                        <p>
                            <span class="weight">
                                <asp:Label runat="server" ID="HistroyLblCaption"></asp:Label></span>
                            <div>
                                <span>
                                    <asp:Label runat="server" ID="lblDettagli" Visible="false"></asp:Label></span></div>
                        </p>
                    </div>
                    <asp:GridView ID="GridViewHistory" runat="server" AutoGenerateColumns="false" Width="99%"
                        CssClass="tbl_rounded_custom round_onlyextreme">
                        <AlternatingRowStyle CssClass="AltRow" />
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDate" Text='<%# Bind("Data") %>' ToolTip='<%# Bind("Tooltip0")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRule" Text='<%# Bind("Ruolo") %>' ToolTip='<%# Bind("Tooltip1")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblUser" Text='<%# Bind("Utente") %>' ToolTip='<%# Bind("Tooltip2")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblModify" Text='<%# Bind("Modifica") %>' ToolTip='<%# Bind("Tooltip3")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:GridView ID="GridViewDestinatari" runat="server" AutoGenerateColumns="false"
                        Width="99%" CssClass="tbl_rounded_custom round_onlyextreme">
                        <AlternatingRowStyle CssClass="AltRow" />
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDate" Text='<%# Bind("dataModifica") %>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRule" Text='<%# Bind("ruolo.descrizione") %>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblUser" Text='<%# Bind("utente.descrizione") %>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <ul>
                                        <li>
                                            <asp:Label runat="server" ID="lblModify" Text='<%# Bind("descrizione") %>'> </asp:Label></li>
                                    </ul>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <div>
            <asp:Panel ID="DatagridStoricoStati" runat="server" Visible="false">
                <asp:GridView ID="GridViewStoricoStati" runat="server" AutoGenerateColumns="false"
                    Width="99%" CssClass="tbl_rounded_custom round_onlyextreme">
                    <AlternatingRowStyle CssClass="AltRow" />
                    <Columns>
                        <asp:BoundField DataField="RUOLO" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                        <asp:BoundField DataField="UTENTE" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="DATA" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Nuovo Stato" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </div>
        <asp:Panel ID="DocHistroyPreservedPnlNotFound" runat="server" Visible="false">
            <asp:Label ID="DocHistroyPreservedLblNotFound" runat="server"></asp:Label>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpPnlButtons" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div style="float: left">
                <cc1:CustomButton ID="HistoryBtnChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="HistoryBtnChiudi_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
