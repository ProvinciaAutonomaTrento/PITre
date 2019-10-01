<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Disservizio.aspx.cs" Inherits="NttDataWA.Popup.Disservizio"
    MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    ClientIDMode="static" runat="server">
    <div style="text-align: left; padding-top: 20px; width: 95%;margin-left:5px;line-height:20px;">
       <asp:Label runat="server" ID="txtDisservizio"></asp:Label>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="DisservizioClosePage" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="DisservizioClosePage_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
