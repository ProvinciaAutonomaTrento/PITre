<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StateInstanceProcessSignature.aspx.cs"
    Inherits="NttDataWA.Popup.StateInstanceProcessSignature" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <style type="text/css">
        #container
        {
            position: fixed;
            top: 1px;
            left: 0px;
            bottom: 71px;
            right: 0px;
            overflow: auto;
            background: #ffffff;
            text-align: left;
            padding: 10px;
            width: 97%;
        }
        .TreeSignatureProcess
        {
            padding: 0;
            margin-right: 30px;
            color: #0f64a1;
            overflow: auto;
        }
        
        .disabled
        {
            color: #848484;
        }
        
        .TreeSignatureProcess img
        {
            width: 20px;
            height: 20px;
        }
        
        .TreeSignatureProcess_node a:link, .TreeSignatureProcess_node a:visited, .TreeSignatureProcess_node a:hover
        {
            padding: 0 5px;
        }
        
        .TreeSignatureProcess_selected
        {
            background-color: #477FAF;
            color: #fff;
        }
        
        .TreeSignatureProcess_selected a:link, .TreeSignatureProcess_selected a:visited, .TreeSignatureProcess_selected a:hover
        {
            padding: 0 5px;
            background-color: transparent;
            color: #fff;
        }
        
        .col-marginSx2
        {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }
        
        #contentAddressBook
        {
            float: left;
            width: 100%;
        }
        
        #topContentAddressBook ul
        {
            margin: 0px;
            padding: 0px;
            text-align: center;
        }
        
        #topContentAddressBook li
        {
            margin: 0px;
            padding: 0px;
            list-style-type: none;
            display: inline;
            float: left;
            text-align: center;
        }
        
        #centerContentAddressbook
        {
            border: 1px solid #2e82bc;
            background-color: #edf4f8;
            float: left;
            width: 100%;
            min-height: 300px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="contentAddressBook">
        <div id="centerContentAddressbook">
            <div id="contentTab">
                <div class="row">
                    <asp:UpdatePanel ID="upPnlTreeSignatureProcess" runat="server" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:TreeView ID="TreeSignatureProcess" runat="server" ExpandLevel="10" ShowLines="true"
                                Width="100%" NodeStyle-CssClass="TreeSignatureProcess_node" SelectedNodeStyle-CssClass="TreeSignatureProcess_selected"
                                OnTreeNodeCollapsed="TreeSignatureProcess_Collapsed" OnTreeNodeExpanded="TreeSignatureProcess_Expanded"
                                CssClass="TreeSignatureProcess" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <asp:UpdatePanel ID="upPnlDetailsSignatureProcess" runat="server" UpdateMode="Conditional"
                    ClientIDMode="Static">
                    <ContentTemplate>
                        <div class="row">
                            <asp:Panel ID="pnlDetailsSignatureProcess" runat="server" Style="padding-top: 5px;">
                                <div class="row">
                                    <div class="col-marginSx2">
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="LtlNameSignatureProcess"></asp:Literal>
                                        </span>
                                    </div>
                                    <div class="col">
                                        <asp:Label ID="lblNameSignatureProcess" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-marginSx2">
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="LtlProponente"></asp:Literal>
                                        </span>
                                    </div>
                                    <div class="col" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
                                        width: 80%">
                                        <asp:Label ID="lblProponente" runat="server" CssClass="clickable"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-marginSx2">
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="LtlAvviatoIl"></asp:Literal>
                                        </span>
                                    </div>
                                    <div class="col">
                                        <asp:Label ID="LblAvviatoIl" runat="server"></asp:Label>
                                    </div>
                                    <asp:Panel ID="pnlConclusoIl" runat="server">
                                        <div class="col-marginSx2">
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlConclusoIl"></asp:Literal>
                                            </span>
                                        </div>
                                        <div class="col">
                                            <asp:Label ID="lblConclusoIl" runat="server"></asp:Label>
                                        </div>
                                    </asp:Panel>
                                </div>
                                <asp:Panel ID="pnlNoteAvvio" runat="server">
                                    <div class="row">
                                        <div class="col-marginSx2">
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlNoteAvvio"></asp:Literal>
                                            </span>
                                        </div>
                                        <div class="col">
                                            <asp:Label ID="lblNoteAvvio" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlMotivoInterruzione" runat="server">
                                    <div class="row">
                                        <div class="col-marginSx2">
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlMotivoInterruzione"></asp:Literal>
                                            </span>
                                        </div>
                                        <div class="col">
                                            <asp:Label ID="lblMotivoInterruzione" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="StateInstanceProcessSignatureClose" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="StateInstanceProcessSignatureClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
