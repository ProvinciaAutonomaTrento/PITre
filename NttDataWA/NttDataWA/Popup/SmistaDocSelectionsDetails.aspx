<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmistaDocSelectionsDetails.aspx.cs"
    Inherits="NttDataWA.Popup.SmistaDocSelectionsDetails" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="ctw" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">

        .contenuto a:link
        {
            font-weight:normal;
        }
        .contenuto a:visited
        {
            font-weight:normal;
        }
        .contenuto a:hover
        {
           font-weight:bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="contenuto">
        <ctw:CustomTreeView ID="TreeSmistaDocSelection" runat="server" OnSelectedNodeChanged="TreeSmistaDocSelection_SelectedNodeChanged"
            CssClass="TreeAddressBook">
        </ctw:CustomTreeView>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <ctw:CustomButton ID="SmistaDocSelDetBtnClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SmistaDocSelDetBtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
