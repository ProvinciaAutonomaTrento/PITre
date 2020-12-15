<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ChooseRFSegnature.aspx.cs" Inherits="NttDataWA.Popup.ChooseRFSegnature" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        .title {font-size: 1.2em; font-weight: bold; text-align: center;}
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row title">
        <asp:Literal ID="litTitle" runat="server" />
    </div>

    <asp:UpdatePanel id="divRF" runat="server" class="row">
        <ContentTemplate>
            <asp:label id="lbl_doc_rf" runat="server"></asp:label><br />
            <asp:DropDownList ID="ddl_regRF" runat="server" OnSelectedIndexChanged="ddl_regRF_IndexChanged" AutoPostBack="true" CssClass="chzn-select-deselect" Width="450"></asp:DropDownList>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel id="divCaselle" runat="server" class="row">
        <ContentTemplate>
            <asp:label id="lblMailAddress" runat="server" /><br />
            <asp:DropDownList ID="ddlCaselle" runat="server" OnSelectedIndexChanged="ddlCaselle_IndexChanged" AutoPostBack="true" CssClass="chzn-select-deselect" Width="450"></asp:DropDownList>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel id="UpPnlButtons" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnOk_Click" enabled="false" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
