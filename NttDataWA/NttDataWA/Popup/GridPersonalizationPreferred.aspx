<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="GridPersonalizationPreferred.aspx.cs" Inherits="NttDataWA.Popup.GridPersonalizationPreferred" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 98%;
            margin: 0 auto;
        }
        .tbl_rounded td
        {
            text-align: center;
        }
        .tbl_rounded td:first-child
        {
            text-align: left;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:UpdatePanel ID="UpnlGrid" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView runat="server" ID="GridPreferred" AllowPaging="true" AutoGenerateColumns="False"
                    HorizontalAlign="Center" CssClass="tbl_rounded round_onlyextreme" Width="100%"
                    Visible="true" OnPageIndexChanging="GridPreferred_PageIndexChanging" OnRowCreated="GridPreferred_RowCreated">
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="70%" ItemStyle-CssClass="tab_sx" HeaderStyle-HorizontalAlign="center"
                            HeaderStyle-CssClass="head_tab">
                            <ItemTemplate>
                                <asp:Label ID="gridName" runat="server" Text='<%# this.GetGridName((NttDataWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="7%" ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <asp:RadioButton ID="rbSelect" runat="server" Checked='<%# this.GetGridPreferred((NttDataWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="7%" HeaderText="Utente" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <image src='<%# this.GetImageUserGridID((NttDataWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></image>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="7%" ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <image src='<%# this.GetImageRoleGridID((NttDataWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></image>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <cc1:CustomImageButton ID="btn_Rimuovi" runat="server" CssClass="clickableLeft" CommandName="Rimuovi"
                                    ImageUrl="../Images/Icons/delete.png" OnMouseOutImage="../Images/Icons/delete.png"
                                    OnMouseOverImage="../Images/Icons/delete_hover.png" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                    Visible='<%# this.GetDeleteGridID((NttDataWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'
                                    OnClick="DeleteGrid" ToolTip='<%# this.GetTooltipRemove()%>' AlternateText='<%# this.GetTooltipRemove()%>'>
                                </cc1:CustomImageButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetGridID((NttDataWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpHiddenFields" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="HiddenConfirmDelete" runat="server" ClientIDMode="Static" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="GridPersonalizationPreferredBtnInserisci" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2')"
                OnClick="GridPersonalizationPreferredBtnInserisci_Click" />
            <cc1:CustomButton ID="GridPersonalizationPreferredBtnClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="GridPersonalizationPreferredBtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        //        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        //        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });



        function RadioCheck(rb) {
            var gv = document.getElementById("<%=GridPreferred.ClientID%>");
            var rbs = gv.getElementsByTagName("input");
            var row = rb.parentNode.parentNode;
            for (var i = 0; i < rbs.length; i++) {
                if (rbs[i].type == "radio") {
                    if (rbs[i].checked && rbs[i] != rb) {
                        rbs[i].checked = false;
                        break;
                    }
                }
            }
        }    
    </script>
</asp:Content>
