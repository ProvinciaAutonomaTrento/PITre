<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="confirm.aspx.cs" Inherits="NttDataWA.Popup.confirm" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">      
        .container {text-align: left; line-height: 2em; width: 90%; min-height: 130px; margin: 40px auto 0 auto;}
        .container img {float: left; display: block; margin: 0 20px 20px 0;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div class="container">
                <asp:Literal ID="msg" runat="server" />
            </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
            <cc1:CustomButton ID="DialogBtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" />
            <cc1:CustomButton ID="DialogBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" />
</asp:Content>
