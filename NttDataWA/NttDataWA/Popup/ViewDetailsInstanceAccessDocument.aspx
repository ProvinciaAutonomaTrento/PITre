<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDetailsInstanceAccessDocument.aspx.cs" Inherits="NttDataWA.Popup.ViewDetailsInstanceAccessDocument" MasterPageFile="~/MasterPages/Popup.Master"%>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .row
        {
            clear: both;
            min-height: 18px;
            margin: 0 0 15px 0;
            text-align: left;
            vertical-align: top;
        }

    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">
    <div class="content" style=" padding-left:10px; padding-top:10px; width:95%">
        <div class="row">
            <fieldset class="basic">
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="LtrRegister" runat="server" /></strong></div>
                    <div class="colHalf8">
                        <asp:Label  ID="lblRegisterText" runat="server"/></div>
                </div>
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="LtrHash" runat="server" /></strong></div>
                    <div class="colHalf8">
                        <div class="full_width">
                            <cc1:CustomTextArea ID="lblHashText" runat="server" class="txt_input_full" 
                                CssClassReadOnly="txt_input_full_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="LtrNomeFile" runat="server" /></strong></div>
                    <div class="colHal8">
                        <asp:Label  ID="lblNomeFileText" runat="server"/></div>
                </div>
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="LtrClassification" runat="server" /></strong></div>
                    <div class="colHalf8">
                        <asp:Label  ID="lblClassificationText" runat="server"/></div>
                </div>
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="LtrCodeProject" runat="server" /></strong></div>
                    <div class="colHalf8">
                        <asp:Label  ID="lblCodeProjectText" runat="server"/></div>
                </div>
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="LtrDescriptionProject" runat="server" /></strong></div>
                    <div class="colHalf8">
                        <asp:Label  ID="lblDescriptionProjectText" runat="server"/></div>
                </div>
            </fieldset>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>  
            <cc1:CustomButton ID="ViewDetailsInstanceDocumentBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewDetailsInstanceDocumentBtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
