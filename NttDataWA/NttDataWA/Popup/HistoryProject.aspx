<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="HistoryProject.aspx.cs" Inherits="NttDataWA.Popup.HistoryProject" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .recordNavigator2 table
        {
            margin: 0 auto;
        }
        .recordNavigator2 td
        {
            border: 0;
        }
        
        .recordNavigator, .recordNavigator2
        {
            color: #00487A;
            font-size: 1.1em;
            text-align: center;
            width: 100%;
            padding: 5px 0;
            margin-top: 0;
        }
        .recordNavigator a:link, .recordNavigator2 a:link
        {
            padding: 3px 7px;
            margin: 0 3px;
            font-weight: normal;
            color: #0000a4;
            text-decoration: none;
        }
        
        .recordNavigator a:visited, .recordNavigator2 a:visited
        {
            padding: 3px 7px;
            margin: 0 3px;
            font-weight: normal;
            color: #0000a4;
            text-decoration: none;
        }
        
        .recordNavigator a:hover, .recordNavigator2 a:hover
        {
            padding: 3px 7px;
            margin: 0 3px;
            font-weight: normal;
            color: #ffffff;
            text-decoration: none;
            background-color: #00487A;
        }
        /*.recordNavigator span, .recordNavigator2 span
{
    background: #00487A;
    padding: 3px 7px;
    margin: 0 3px;
    color: #fff;
}*/
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:Panel ID="GridViewDocHistoryPnl" runat="server">
            <asp:UpdatePanel ID="UpGridHistory" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div>
                        <p>
                            <span class="weight">
                                <asp:Label runat="server" ID="HistoryProjectLblObject"></asp:Label></span>
                            <div>
                                <span>
                                    <asp:Label runat="server" ID="lblDettagli" Visible="false"></asp:Label></span></div>
                        </p>
                    </div>
                    <asp:GridView ID="GridViewHistoryProject" runat="server" AutoGenerateColumns="false"
                        Width="99%" CssClass="tbl_rounded_custom round_onlyextreme" AllowPaging="True"
                        PageSize="10" OnPageIndexChanging="GridViewHistoryProject__PageIndexChanging">
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDate" Text='<%# Bind("Data") %>' ToolTip='<%# Bind("Tooltip0")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRule" Text='<%# Bind("Operatore") %>' ToolTip='<%# Bind("Tooltip1")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblUser" Text='<%# Bind("Azione") %>' ToolTip='<%# Bind("Tooltip2")%>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <div>
            <asp:Panel ID="DatagridStoricoStati" runat="server" Visible="false">
                <div>
                    <p>
                        <span class="weight">
                            <asp:Label runat="server" ID="HistoryProjectLblState"></asp:Label></span>
                    </p>
                </div>
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
                    OnMouseOver="btnHover" OnClick="HistoryProjectBtnChiudi_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
