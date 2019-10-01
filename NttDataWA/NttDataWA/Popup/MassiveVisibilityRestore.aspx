<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveVisibilityRestore.aspx.cs" Inherits="NttDataWA.Popup.MassiveVisibilityRestore" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        .message {text-align: left; line-height: 2em; width: 90%; margin: 50px auto 0 auto;}
        .message img {float: left; display: block; margin: 0 20px 20px 0;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <asp:UpdatePanel ID="UpPnlNote" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcNote" runat="server">
                <div class="row">
                    <div class="colonnasx"><asp:Literal ID="litNote" runat="server" /></div>
                    <div class="colonnadx"><cc1:CustomTextArea ID="txtNote" runat="server" TextMode="MultiLine" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" ClientIDMode="static" /></div>
                </div>
                <div class="row">
                    <div class="col-right-no-margin">
                        <span class="charactersAvailable">
                            <asp:Literal ID="litNotesChars" runat="server" ClientIDMode="static"></asp:Literal>
                            <span id="txtNote_chars" runat="server" clientidmode="static"></span></span>
                    </div>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcMessage" runat="server">
                <div class="row">
                    <div class="message">
                        <asp:Literal ID="litMessage" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
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
