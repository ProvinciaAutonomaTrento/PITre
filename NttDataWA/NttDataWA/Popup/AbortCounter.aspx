<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AbortCounter.aspx.cs" Inherits="NttDataWA.Popup.AbortCounter" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
<p><asp:Label runat="server" ID="AbortCounterLliDesc" CssClass="NormalBold"></asp:Label></p>
    <cc1:CustomTextArea runat="server" ID="TxtTextAborCounter" CssClass="txt_textarea_view_object" TextMode="MultiLine"  ></cc1:CustomTextArea>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="AbortCounterBtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');" 
        onclick="AbortCounterBtnOk_Click" />
    <cc1:CustomButton ID="AbortCounterBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" 
        onclick="AbortCounterBtnClose_Click" />
</asp:Content>