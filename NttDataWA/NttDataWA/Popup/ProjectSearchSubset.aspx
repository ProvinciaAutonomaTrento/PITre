<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ProjectSearchSubset.aspx.cs" Inherits="NttDataWA.Popup.ProjectSearchSubset" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#FolderSearch").focus();
        });
    </script>
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div id="rowMessage" runat="server" style="color: #f00;" />
        <p><span class="weight"><asp:Literal ID="lbl_search" runat="server" /></span>*</p>
        <p id="rowDescription" runat="server">
            <cc1:CustomTextArea ID="FolderSearch" runat="server" ClientIDMode="Static" CssClass="txt_input_full" />
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
            <cc1:CustomButton ID="BtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnSearch_Click" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnClose_Click" />
</asp:Content>
