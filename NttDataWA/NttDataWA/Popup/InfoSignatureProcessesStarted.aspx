<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfoSignatureProcessesStarted.aspx.cs"
    Inherits="NttDataWA.Popup.InfoSignatureProcessesStarted" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            padding: 10px;
            text-align: left;
            line-height: 2em;
            width: 95%;
            height: 80%;
            top: 20%;
        }
        .title
        {
            color: #521110;
            font-size: 14px;
        }
        .description
        {
            color: #521110;
            font-size: small;
        }
        .definition
        {
            color: #151052;
            font-size: small;
        }
        .divisor
        {
            border-bottom-style: solid;
            border-bottom-width: thin;
            border-bottom-color: Silver;
        }
        
        .message
        {
            min-height: 1px;
            margin: 1px 0 0 0;
        }
        
        .message li
        {
            margin: 0 0 0 50px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div class="container">
        <div class="divisor">
            <span class="weight">
                <asp:Label ID="lblMessage" runat="server" class="title"></asp:Label>
            </span>
        </div>
        <div class="row">
            <asp:Label ID="lblMessageDetailsMainDocument" runat="server"></asp:Label>
        </div>
        <div class="row2">
            <asp:Label ID="lblMessageDetailsAttachments" runat="server"></asp:Label>
        </div>
        <div class="message">
            <asp:Label ID="lblListAttach" runat="server"></asp:Label>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentPlaceOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="InfoSignatureProcessesStartedClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="InfoSignatureProcessesStartedClose_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
