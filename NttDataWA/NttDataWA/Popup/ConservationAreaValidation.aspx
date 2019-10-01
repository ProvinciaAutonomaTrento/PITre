<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConservationAreaValidation.aspx.cs" Inherits="NttDataWA.Popup.ConservationAreaValidation" MasterPageFile="~/MasterPages/Popup.Master"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>

<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">

    <asp:TextBox ID="txtReport" runat="server" ReadOnly="true" TextMode="MultiLine" Rows="25" Wrap="false" Width="99%"></asp:TextBox>

</asp:Content>

<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnConservationAreaValidationSend" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConservationAreaValidationSend_Click" />
            <cc1:CustomButton ID="BtnConservationAreaValidationClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConservationAreaValidationClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

