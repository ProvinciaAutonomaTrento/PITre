<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="VisibilityHistoryRole.aspx.cs" Inherits="NttDataWA.Popup.VisibilityHistoryRole" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tabPopup td
        {
            padding: 5px;
            text-align: center;
            color: #666666;
            font-size: 1em;
            cursor:default;
        }
     </style>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="ReportGenerator" runat="server" Url="../Popup/ReportGenerator.aspx"
        Width="700" Height="800" IsFullScreen="False" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closeReportGenerator');}" />
    <div id="content" style="padding-left:15px">
        <div id="containerGridHistoryRole" runat="server" style="padding-top:15px">
            <asp:UpdatePanel ID="UpPanelGridHistoryRole" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                <ContentTemplate>
                        <asp:GridView ID="GrdGridHistoryRole" runat="server" Width="97%" AutoGenerateColumns="false"
                            AllowPaging="True" PageSize="4" CssClass="tabPopup tbl" BorderWidth="0" OnPageIndexChanging="GrdGridHistoryRole_PageIndexChanging"
                            ShowHeaderWhenEmpty="true">
                            <AlternatingRowStyle CssClass="AltRow" />
                            <PagerStyle CssClass="recordNavigator2" />
                            <Columns>
                                <asp:TemplateField HeaderText='<%$ localizeByText:VisibilityHistoryRoleAction%>'>
                                    <ItemTemplate>
                                        <strong><asp:Label ID="instanceId" runat="server" Text='<%# Bind("HistoryAction") %>' ></asp:Label></strong>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText='<%$ localizeByText:VisibilityHistoryRoleDateAction%>'>
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_instanceId" runat="server" Text="<%# this.FormatDate(((NttDataWA.DocsPaWR.RoleHistoryItem)Container.DataItem).ActionDate) %>" ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText='<%$ localizeByText:VisibilityHistoryRoleDetails%>'>
                                    <ItemTemplate>
                                    <div style="float: left;">
                                        <strong><asp:Literal ID="litDescription" runat="server" Text='<%$ localizeByText:VisibilityHistoryRoleDescription%>' />:</strong>&nbsp;
                                    </div>
                                    <div style="float: left;">
                                        <asp:Label ID="lblRoleDescription" runat="server" Text="<%# this.GetRoleDescription(((NttDataWA.DocsPaWR.RoleHistoryItem)Container.DataItem)) %>" />
                                    </div>
                                    <div style="clear: both;" />
                                    <div style="float: left; margin-bottom: 3;">
                                        <strong><asp:Literal ID="litDescriptionUO" runat="server" Text='<%$ localizeByText:VisibilityHistoryRoleDescriptionUO%>' />:</strong>&nbsp;
                                    </div>
                                    <div style="float: left;">
                                        <asp:Label ID="lblUoDescription" runat="server" Text='<%# Bind("UoDescription") %>' /><br />
                                    </div>
                                    <div style="clear: both; margin-bottom: 3;" />
                                    <div style="float: left;">
                                        <strong><asp:Literal ID="litDescriptionRoleType" runat="server" Text='<%$ localizeByText:VisibilityHistoryRoleDescriptionRoleType%>' />:</strong>&nbsp;
                                    </div>
                                    <div style="float: left;">
                                        <asp:Label ID="lblRoleType" runat="server" Text='<%# Bind("RoleTypeDescription") %>' />
                                    </div>
                                    <div style="clear: both;" />
                                </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="HiddenRemoveInstanceConservation" runat="server" ClientIDMode="Static" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="row"  style="padding-top:15px">
            <asp:Literal ID="litLegendaAzione" runat="server" />
            <ul class="legend-style">
                <li><strong>C</strong> = <asp:Literal ID="litCreatingRole" runat="server" /></li>
                <li><strong>M</strong> = <asp:Literal ID="litRoleChange" runat="server" /></li>
                <li><strong>S</strong> = <asp:Literal ID="litHistoricizingRole" runat="server" /></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="VisibilityHistoryRoleBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="VisibilityHistoryRoleBtnClose_Click" OnClientClick="disallowOp('Content2');"/>
            <cc1:CustomButton ID="VisibilityHistoryRoleBtnExport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="VisibilityHistoryRoleBtnExport_Click" OnClientClick="disallowOp('Content2');"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>