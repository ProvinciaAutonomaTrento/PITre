<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="RemoveProfile.aspx.cs" Inherits="NttDataWA.Popup.RemoveProfile" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        .txt_textarea, .txt_textarea_disabled {vertical-align: top;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <asp:PlaceHolder id="plcMessageCheckOut" runat="server" Visible="false">
            <p align="center">
                <asp:Literal ID="lbl_messageCheckOut" runat="server" />
                <asp:Literal ID="lbl_messageOwnerCheckOut" runat="server" />
            </p>
        </asp:PlaceHolder>
        <asp:PlaceHolder id="plcMessageConfirm" runat="server" Visible="false">
            <p align="center">
                <asp:Literal ID="lbl_mess_conf_rimuovi" runat="server" />
            </p>
        </asp:PlaceHolder>
        <asp:PlaceHolder id="plcMessageResult" runat="server" Visible="false">
            <p align="center">
                <asp:Literal ID="lbl_result" runat="server" />
            </p>
        </asp:PlaceHolder>
    </div>
    <asp:PlaceHolder id="plcNote" runat="server">
        <div class="row">
            <cc1:CustomTextArea ID="TxtNote" runat="server" TextMode="MultiLine" ClientIDMode="static" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
            <span class="col-right"><asp:Literal ID="litNotesChars" runat="server" />: <span id="TxtNote_chars" runat="server" clientidmode="Static"></span></span>
        </div>
    </asp:PlaceHolder>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" onclick="BtnOk_Click" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" onclick="BtnClose_Click" />
</asp:Content>
