<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="SenderObjHistory.aspx.cs" Inherits="NttDataWA.Popup.SenderObjHistory" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div id="tabellaStorico" runat="server">

</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
<div style="float: left">
        <cc1:CustomButton ID="SenderObjHistoryBtnClose" runat="server" 
            CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" Text="Close" 
            onclick="SenderObjHistoryBtnClose_Click" />
    </div>
</asp:Content>
