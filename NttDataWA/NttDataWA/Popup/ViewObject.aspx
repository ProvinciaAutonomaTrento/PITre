<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewObject.aspx.cs" Inherits="NttDataWA.Popup.ViewObject"
    MasterPageFile="~/MasterPages/Popup.Master" %>
   
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#TxtViewObject").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <cc1:CustomTextArea ID="TxtViewObject" TextMode="MultiLine" runat="server" ClientIDMode="Static"
        CssClass="txt_textarea_view_object" CssClassReadOnly="txt_textarea_view_object_disabled"></cc1:CustomTextArea>
    <div class="availableCarachtersObject">
        <span class="charactersAvailable">
            <asp:Literal ID="ViewObjectLitObjectChAv" runat="server"></asp:Literal>
            <span id="TxtViewObject_chars"></span></span>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left;">
        <cc1:CustomButton ID="ViewObjectBtnOk" runat="server" OnClick="btnOk_Click" CssClass="btnEnable" OnClientClick="disallowOp('Content2');" 
            CssClassDisabled="btnDisable" OnMouseOver="btnHover"/>
    </div>
    <div style="float: left;">
        <cc1:CustomButton ID="ViewObjectBtnClose" runat="server" OnClick="ObjectBtnChiudi_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
    </div>
</asp:Content>
