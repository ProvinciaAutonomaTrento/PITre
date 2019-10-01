<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="DownloadClientBigfile.aspx.cs" Inherits="NttDataWA.ClientBigfile.DownloadClientBigfile" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {
            width: 90%;
            margin: 0 auto;
            margin-top:15px;
        }

        .txt_text {
            text-align: left;
            font-family: Verdana, Arial, Verdana, sans-serif;
            font-size: 13px;
            line-height: 22px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:Panel ID="PnlLtlDownloadFile" runat="server" CssClass="txt_text">
            <asp:Literal ID="LtlDownloadFile" runat="server" />            
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="ClientBigfileOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" OnClick="ClientBigfileOk_Click" />
    <cc1:CustomButton ID="ClientBigfileClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" OnClick="ClientBigfileClose_Click"/>
</asp:Content>

