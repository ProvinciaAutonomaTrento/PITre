<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="NoteDataentry.aspx.cs" Inherits="NttDataWA.Popup.NoteDataentry" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <div class="colHalf"><asp:Literal ID="lblRf" runat="server" /></div>
        <div class="colHalf2"><div style="text-align: left;"><asp:DropDownList ID="ddlRf" runat="server" CssClass="chzn-select-deselect" Width="100%" /></div></div>
    </div>
    <div class="row">
        <div class="colHalf"><asp:Literal ID="lblDesc" runat="server" /></div>
        <div class="colHalf2"><cc1:CustomTextArea ID="txt_desc" runat="server" TextMode="MultiLine" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" /></div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConfirm_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
