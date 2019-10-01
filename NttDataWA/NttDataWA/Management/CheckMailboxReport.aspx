<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="CheckMailboxReport.aspx.cs" Inherits="NttDataWA.Management.CheckMailboxReport" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">

</asp:Content>

<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckMailboxReportExport" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="CheckMailboxReportExport_Click" />
            <cc1:CustomButton ID="CheckMailboxReportCreateDoc" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="CheckMailboxReportCreateDoc_Click" />
            <cc1:CustomButton ID="CheckMailboxReportClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="CheckMailboxReportClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

