<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InformationFile.aspx.cs"
    Inherits="NttDataWA.Popup.InformationFile" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tabPopup td
        {
            padding: 5px;
            text-align: center;
            color: #666666;
            font-size: 1em;
        }
        
        .row9
        {
            clear: both;
            min-height: 30px;
            margin: 0 0 5px 0;
            text-align: left;
            vertical-align: top;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    ClientIDMode="static" runat="server">
    <div id="container">
        <div id="containerResultRoles" style="padding-left: 10px">
            <p>
                <asp:Label ID="lblContainerResultRoles" runat="server" Style="color: #6495ED; font-weight: bold"></asp:Label></p>
            <div class="row9">
                <div class="col6">
                    <asp:Literal runat="server" ID="ltFormat"></asp:Literal>
                </div>
                <div class="col7">
                    <asp:Label ID="lblFormatText" runat="server"></asp:Label>
                </div>
            </div>
            <div class="row9">
                <div class="col6">
                    <asp:Literal runat="server" ID="ltDateAcquiredFormatDetails"></asp:Literal>
                </div>
                <div class="col7">
                    <asp:Label ID="lblDateAcquiredFormatDetailsText" runat="server"></asp:Label>
                </div>
            </div>
            <div class="row9">
                <div class="col6">
                    <asp:Literal runat="server" ID="ltAdmittedToTheSigned"></asp:Literal>
                </div>
                <div class="col7">
                    <asp:Label ID="lblAdmittedToTheSignedText" runat="server"></asp:Label>
                </div>
            </div>
            <div class="row9">
                <div class="col6">
                    <asp:Literal runat="server" ID="ltAdmittedToTheConservation"></asp:Literal>
                </div>
                <div class="col7">
                    <asp:Label ID="lblAdmittedToTheConservationText" runat="server"></asp:Label>
                </div>
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="panelResult" runat="server">
            <div id="containerResult" style="padding-top: 20px; padding-left: 10px">
                <p>
                    <asp:Label ID="lblResultSearch" runat="server" Style="color: #6495ED; font-weight: bold"></asp:Label></p>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltFilename"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblFilenameText" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltDateCheckUltimate"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblDateCheckUltimateText" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltCheckPresenceMacro"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblCheckPresenceMacro" runat="server"></asp:Label>
                    </div>
                    <div class="col4">
                        <asp:Image ID="imgCheckPresenceMacroResult" runat="server" />
                    </div>
                </div>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltFormatCompliantToTheExtension"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblFormatCompliantToTheExtension" runat="server"></asp:Label>
                    </div>
                    <div class="col4">
                        <asp:Image ID="imgFormatCompliantToTheExtensionResult" runat="server" />
                    </div>
                </div>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltCheckSigned"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblCheckSigned" runat="server"></asp:Label>
                    </div>
                    <div class="col4">
                        <asp:Image ID="imgCheckSignedResult" runat="server" />
                    </div>
                </div>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltCheckCRL"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblCRL" runat="server"></asp:Label>
                    </div>
                    <div class="col4">
                        <asp:Image ID="imgCheckCRLResult" runat="server" />
                    </div>
                </div>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltCheckTimestamp"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblCheckTimestamp" runat="server"></asp:Label>
                    </div>
                    <div class="col4">
                        <asp:Image ID="imgCheckTimestampResult" runat="server" />
                    </div>
                </div>
                <asp:PlaceHolder ID="panelVersionPdf" runat="server" Visible="false">
                    <div class="row9">
                        <div class="col6">
                            <asp:Literal runat="server" ID="ltVersionPdf"></asp:Literal>
                        </div>
                        <div class="col7">
                            <asp:Label ID="lblVersionPdf" runat="server"></asp:Label>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="row9">
                    <div class="col6">
                        <asp:Literal runat="server" ID="ltlCheckElectronicSignature"></asp:Literal>
                    </div>
                    <div class="col7">
                        <asp:Label ID="lblCheckElectronicSignature" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnInformationFileClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnInformationFileClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
