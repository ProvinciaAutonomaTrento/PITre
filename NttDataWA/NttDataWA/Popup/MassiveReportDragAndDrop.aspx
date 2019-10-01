<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveReportDragAndDrop.aspx.cs" Inherits="NttDataWA.Popup.MassiveReportDragAndDrop" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        .message {text-align: left; line-height: 2em; width: 90%; min-height: 200px; margin: 150px auto 0 auto;}
        .message img {float: left; display: block; margin: 0 20px 20px 0;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />

<div class="container">
    <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlReport" runat="server" CssClass="row" Visible="false">
                <asp:GridView id="grdReport" runat="server" Width="100%" AutoGenerateColumns="False" CssClass="tbl_rounded_custom round_onlyextreme">         
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
                        <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReport%>' DataField="ObjId">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Esito" DataField="Result">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Dettagli" DataField="Details">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
