<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="RegisterModify.aspx.cs" Inherits="NttDataWA.Popup.RegisterModify" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#RegisterModifyOldPwTxt").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="containerField">
        <div style=" margin:0; padding-left:10px; padding-top:35px;">
            <asp:Label ID="RegisterModifyOldPw" runat="server" Width="150" CssClass="NormalBold"/>
            <cc1:CustomTextArea ID="RegisterModifyOldPwTxt" ClientIDMode="Static" runat="server"  CssClass="txt_registerModify" TextMode="Password" />
        </div>
        <div style=" margin:0; padding-left:10px; padding-top:35px;">
            <asp:Label ID="RegisterModifyNewPw" runat="server" Width="150" CssClass="NormalBold"/>
            <cc1:CustomTextArea ID="RegisterModifyNewPwTxt" ClientIDMode="Static" runat="server"  CssClass="txt_registerModify" TextMode="Password" />
        </div>
        <div style=" margin:0; padding-left:10px; padding-top:35px;">
            <asp:Label ID="RegisterModifyConfirmPw" runat="server" Width="150" CssClass="NormalBold"/>
            <cc1:CustomTextArea ID="RegisterModifyConfirmPwTxt" ClientIDMode="Static" runat="server"  CssClass="txt_registerModify" TextMode="Password" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
        <cc1:CustomButton ID="RegisterModifyBtnSavePw" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" ClientIDMode="Static" OnClick="RegisterModifyBtnSavePw_Click"/>
        <cc1:CustomButton ID="RegisterModifyBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" ClientIDMode="Static" OnClick="RegisterModifyBtnClose_Click" />
</asp:Content>