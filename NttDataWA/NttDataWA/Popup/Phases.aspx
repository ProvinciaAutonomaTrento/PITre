<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="Phases.aspx.cs" Inherits="NttDataWA.Popup.Phases" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="container1">
        <div class="containerTitle">
            <div class="row">
                <div class="col2">
                    <span class="weight">
                        <asp:Literal ID="ltlSelectedPhase" runat="server"></asp:Literal></span>
                </div>
                <div class="col2">
                    <asp:Label ID="lblSelectedPhase" runat="server" />
                </div>
            </div>
            <fieldset class="filterAddressbook">
            <span class="weight">
                <asp:Label ID="lblSelectState" runat="server"></asp:Label></span>
            </fieldset>
        </div>
        <div class="containerInfo">
            <div class="row">
                <div class="col2">
                    <span class="weight">
                        <asp:Literal ID="ltlCurrentState" runat="server"></asp:Literal></span>
                </div>
                <div class="col2">
                    <asp:Label ID="lblCurrentState" runat="server" />
                </div>
            </div>
            <div class="row">
                <div class="col2">
                    <span class="weight">
                        <asp:Literal ID="ltlCorrespondentStates" runat="server"></asp:Literal></span>
                </div>
                <div class="col2">
                    <asp:UpdatePanel ID="UpPnlCorrespondentStates" runat="server" ClientIDMode="Static"
                        UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="corrispondentStates">
                                <asp:Panel ID="pnlCorrespondentStates" runat="server">
                                </asp:Panel>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpPnlHiddenField" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="HiddenSelectedState" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="HiddenChangeStateDiagramm" runat="server" ClientIDMode="Static" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="PhaseClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="PhaseClose_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
