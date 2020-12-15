<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="attachment_remove.aspx.cs" Inherits="NttDataWA.Popup.attachment_remove" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div id="rowMessage" runat="server" />    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server"> 
            <cc1:CustomButton ID="AttachmentBtn" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Elimina" ClientIDMode="Static" 
                onclick="AttachmentBtn_Click" />
            <cc1:CustomButton ID="AttachmentBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" ClientIDMode="Static" 
                onclick="AttachmentBtnClose_Click" />
</asp:Content>
