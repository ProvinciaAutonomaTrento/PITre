<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ConfirmPopUp.aspx.cs" Inherits="NttDataWA.Popup.ConfirmPopUp" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <style type="text/css">
        .container {text-align: left; line-height: 2em; width: 270px; height: 130px; position: absolute; left: 50%; top: 70%; margin: -90px 0 0 -135px;}
        .container img {float: left; display: block; margin: 15px auto; margin: 0 10px 70px 0;}
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
