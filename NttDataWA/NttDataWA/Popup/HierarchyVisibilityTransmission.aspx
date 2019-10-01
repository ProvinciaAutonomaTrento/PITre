<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="HierarchyVisibilityTransmission.aspx.cs" Inherits="NttDataWA.Popup.HierarchyVisibilityTransmission" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
    <p align="center">
                <asp:Label runat="server" ID="HierarchyVisibilityMessage"></asp:Label></p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
<asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="HierarchyVisibilityMessageBtnYes" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="HierarchyVisibilityMessageBtnYes_Click" />
            <cc1:CustomButton ID="HierarchyVisibilityMessageBtnNo" runat="server" CssClass="btnEnable"  OnClientClick="disallowOp('Content2');"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="HierarchyVisibilityMessageBtnNo_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
