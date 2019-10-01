<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="VisibilityRemove.aspx.cs" Inherits="NttDataWA.Popup.VisibilityRemove" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        .message {text-align: center; font-weight: bold; color: #f00;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <uc:messager id="messager" runat="server" />
    <div class="row">
        <div class="col"><asp:Literal ID="litNotes" runat="server" /></div>
        <cc1:CustomTextArea ID="txtNote" runat="server" TextMode="MultiLine" CssClass="txt_textarea" CssClassDisabled="txt_textarea_disabled"></cc1:CustomTextArea>
    </div>
    <div class="message"><asp:Literal ID="message" runat="server" /></div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"
        onclick="BtnOk_Click" OnClientClick="disallowOp('Content2');" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" 
        onclick="BtnClose_Click" />
</asp:Content>
