<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewNote.aspx.cs" Inherits="NttDataWA.Popup.ViewNote"
    MasterPageFile="~/MasterPages/Popup.Master" %>
   
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#TxtViewNote").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <cc1:CustomTextArea ID="TxtViewNote" TextMode="MultiLine" runat="server" ClientIDMode="Static"
        CssClass="txt_textarea_view_object" CssClassReadOnly="txt_textarea_view_object_disabled"></cc1:CustomTextArea>
    <div class="availableCarachtersObject">
        <span class="charactersAvailable">
            <asp:Literal ID="ViewNoteLitNoteChAv" runat="server"></asp:Literal>
            <span id="TxtViewNote_chars"></span></span>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left;">
        <cc1:CustomButton ID="ViewNoteBtnOk" runat="server" OnClick="ViewNoteBtnOk_Click" CssClass="btnEnable" OnClientClick="disallowOp('Content2');" 
            CssClassDisabled="btnDisable" OnMouseOver="btnHover"/>
    </div>
    <div style="float: left;">
        <cc1:CustomButton ID="ViewNoteBtnClose" runat="server" OnClick="ViewNoteBtnClose_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
    </div>
</asp:Content>
