<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetailsLFAutomaticMode.aspx.cs"
    Inherits="NttDataWA.Popup.DetailsLFAutomaticMode" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
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
        
        .TreeSignatureProcess img
        {
            width: 20px;
            height: 20px;
        }


        .disabled
        {
            color: #848484;
        }
        
        .TreeSignatureProcessRoot
        {
            padding: 0;
            margin-right: 30px;
            color: #0f64a1;
            overflow: auto;
        }

       .TreeSignatureProcessRoot img
        {
            width: 30px;
            height: 24px;
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
            border-bottom: 1px solid #2e82bc;
            border-left: 1px solid #2e82bc;
            border-right: 1px solid #2e82bc;
            background-color: #edf4f8;
            float: left;
            width: 100%;
            min-height: 300px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="InterruptionSignatureProcess" runat="server" Url="../popup/InterruptionSignatureProcess.aspx"
        Width="500" Height="350" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', '');}" />
    <div id="contentAddressBook">
        <div id="topContentPopupSearch">
            <asp:UpdatePanel ID="UpTypeResult" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                <ContentTemplate>
                    <ul>
                        <li class="addressTab" id="liInExecuteList" runat="server">
                            <asp:LinkButton runat="server" ID="InExecuteLinkList" OnClick="InExecuteLinkList_Click"
                                OnClientClick="disallowOp('ContentPlaceHolderContent')"></asp:LinkButton></li>
                        <li class="otherAddressTab" id="liConcludedList" runat="server">
                            <asp:LinkButton runat="server" ID="InConcludedLinkList" OnClick="InConcludedLinkList_Click"
                                OnClientClick="disallowOp('ContentPlaceHolderContent')"></asp:LinkButton></li>
                    </ul>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="centerContentAddressbook">
            <div id="contentTab">
                <div class="row">
                    <asp:UpdatePanel ID="upPnlTreeSignatureProcess" runat="server" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:TreeView ID="TreeSignatureProcess" runat="server" ExpandLevel="10" ShowLines="true" CssClass="TreeSignatureProcess"
                                Width="100%" OnSelectedNodeChanged="TreeSignatureProcess_SelectedNodeChanged" OnTreeNodeCollapsed="TreeSignatureProcess_Collapsed"
                                OnTreeNodeExpanded="TreeSignatureProcess_Expanded">
                                <NodeStyle CssClass="TreeSignatureProcess_node" />
                                <LeafNodeStyle CssClass="TreeSignatureProcess" />
                                <RootNodeStyle CssClass="TreeSignatureProcessRoot" />
                                <SelectedNodeStyle CssClass="TreeSignatureProcess_selected" />
                            </asp:TreeView>
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
                        <asp:Panel ID="pnlProponente" runat="server">
                            <asp:PlaceHolder ID="PlcNotificationOption" runat="server">
                                <div class="row">
                                    <div class="col-marginSx2">
                                        <span class="weight">
                                            <asp:Literal ID="ltlNotificationOption" runat="server"></asp:Literal>
                                        </span>
                                    </div>
                                    <div class="row">
                                        <asp:CheckBoxList ID="cbxNotificationOption" runat="server" RepeatDirection="Vertical">
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </asp:Panel>
                        </asp:Panel> </div>
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
            <cc1:CustomButton ID="DetailsLFAutomaticModeInterruption" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="DetailsLFAutomaticModeInterruption_Click" />
            <cc1:CustomButton ID="DetailsLFAutomaticModeModify" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="DetailsLFAutomaticModeModify_Click" />
            <cc1:CustomButton ID="DetailsLFAutomaticModeClose" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="DetailsLFAutomaticModeClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
