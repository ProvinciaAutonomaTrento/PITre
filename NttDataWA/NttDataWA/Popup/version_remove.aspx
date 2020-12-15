<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="version_remove.aspx.cs" Inherits="NttDataWA.Popup.version_remove" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div id="rowMessage" runat="server" />     
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
        &nbsp;<cc1:CustomButton ID="AttachmentBtn" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Elimina" ClientIDMode="Static" 
                onclick="AttachmentBtn_Click" OnClientClick="disallowOp('Content2')" />
        <cc1:CustomButton ID="AttachmentBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" ClientIDMode="Static" onclick="AttachmentBtnClose_Click" OnClientClick="disallowOp('Content2')"/>
</asp:Content>
