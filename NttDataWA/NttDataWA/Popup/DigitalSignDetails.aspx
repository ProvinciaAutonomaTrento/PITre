<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="DigitalSignDetails.aspx.cs" Inherits="NttDataWA.Popup.DigitalSignDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        
        .TreeAddressBook
        {
            padding: 0;
        }
        
        .TreeAddressBook td, .TreeAddressBook th, .TreeAddressBook tr
        {
            border: 0;
            padding: 0;
            margin: 0;
            height: 20px;
            overflow: hidden;
        }
        
        .TreeAddressBook table
        {
            padding: 0;
            margin: 0;
            height: 0;
            border: 0;
        }
        
        .TreeAddressBook_node a:link, .TreeAddressBook_node a:visited, .TreeAddressBook_node a:hover
        {
            padding: 0 5px;
        }
        
        .TreeAddressBook_selected
        {
            background-color: #477FAF;
        }
        
        .TreeAddressBook_selected a:link, .TreeAddressBook_selected a:visited, .TreeAddressBook_selected a:hover
        {
            padding: 0 5px;
            background-color: transparent;
            color: #fff;
        }
        
        .tbl
        {
            width: 100%;
            border-spacing: 0;
            border-collapse: collapse;
        }
        .tbl th
        {
            font-weight: bold;
            padding: 0 0 0 10px;
        }
        .tbl td
        {
            background: #F0F7FD;
            vertical-align: top;
            padding: 0;
            margin: 0;
            cursor: default;
        }
        .tbl td:first-child
        {
            width: 240px;
            font-weight: bold;
            color: #666;
        }
        .tbl td.header
        {
            border-bottom: 0;
            border-left: 0;
            color: #2369A5;
            font-weight: bold;
        }
        .tbl td.header-image
        {
            border-bottom: 0;
            border-right: 0;
            border-left: 0;
            padding: 0 10px;
        }
        .tbl td.normal
        {
            border-top: 0;
            border-bottom: 0;
            border-left: 0;
        }
        .tbl td.image
        {
            border-top: 0;
            border-bottom: 0;
            border-right: 0;
            padding: 0 10px;
        }
        .tbl tr.AlternateRow td
        {
            background: #fff;
        }
        div.original
        {
            width: 500px;
            margin: 0 auto;
        }
        div.original p.icon
        {
            text-align: center;
            margin: 0 auto 20px auto;
            color: #286EA9;
            font-weight: bold;
            padding: 0;
        }
        div.original p.icon img
        {
            display: block;
            margin: 40px auto;
        }
        
        div.original p.row
        {
            padding: 0;
            margin: 0 0 0 50px;
            font-weight: normal;
        }
        div.original p.row strong
        {
            float: left;
            width: 180px;
        }
        .row9
        {
            clear: both;
            text-align: left;
            vertical-align: top;
        }
        .col6
        {
            float: left;
            width: 180px;
            color: #666666;
        }
        .TreeSignatureProcess
        {
            padding: 0;
            margin-right: 30px;
            color: #0f64a1;
            overflow: auto;
            position: relative;
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
    </style>
    <script type="text/javascript">
        function CheckDate() {
            if ($('#txtDate').val().length > 0 && IsDate($('#txtDate').val())) {
                $('#hdnDate').val($('#txtDate').val());
                return true;
            }
            else {
                ajaxDialogModal('WarningVerifyCLRDateInvalid', 'warning');
                return false;
            }
        }

        function resizeTable() {
            if (document.getElementById('tbl') && document.getElementById('tbl').offsetTop) {
                var height = document.documentElement.clientHeight;
                height -= document.getElementById('tbl').offsetTop; // not sure how to get this dynamically
                height -= 80; /* whatever you set your body bottom margin/padding to be */
                document.getElementById('tbl').style.height = height + "px";
                document.getElementById('tbl').rows[0].style.height = "35px";

                var rowHeight = (height - 35) / (document.getElementById('tbl').rows.length - 1);
                for (var i = 1; i < document.getElementById('tbl').rows.length; i++)
                    document.getElementById('tbl').rows[i].style.height = rowHeight + 'px';

                window.onresize = resizeIframe
            }
        };

        $(function () {
            resizeTable();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closeZoom');}" />
    <uc:ajaxpopup Id="VerifyCLR" runat="server" Url="../popup/VerifyCLR.aspx" Width="500"
        Height="300" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', '');}" />
    <div class="container">
        <div class="colonnasx2">
            <asp:UpdatePanel ID="UpPnlTreeview" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:TreeView ID="trvDettagliFirma" runat="server" ExpandLevel="10" CssClass="TreeAddressBook"
                        ShowLines="true" NodeStyle-CssClass="TreeAddressBook_node" SelectedNodeStyle-CssClass="TreeAddressBook_selected"
                        OnSelectedNodeChanged="trvDettagliFirma_SelectedNodeChanged" OnTreeNodeCollapsed="trvDettagliFirma_Collapsed"
                        OnTreeNodeExpanded="trvDettagliFirma_Collapsed" onClick="disallowOp('Content2');" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row" style="padding-top: 10px">
                <asp:UpdatePanel ID="UpPnlTreeViewFirmaElettronica" runat="server" ClientIDMode="Static"
                    UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnlDetailsFirmaElettronica" runat="server">
                            <asp:TreeView ID="trvDettagliFirmaElettronica" runat="server" ExpandLevel="10" ShowLines="true"
                                NodeStyle-CssClass="TreeSignatureProcess_node" SelectedNodeStyle-CssClass="TreeSignatureProcess_selected"
                                OnTreeNodeCollapsed="trvDettagliFirmaElettronica_Collapsed" OnTreeNodeExpanded="trvDettagliFirmaElettronica_Collapsed"
                                OnSelectedNodeChanged="TrvDettagliFirmaElettronica_SelectedNodeChanged" onClick="disallowOp('Content2');"
                                CssClass="TreeSignatureProcess" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div class="colonnadx2">
            <asp:UpdatePanel ID="UpPnlDetails" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:PlaceHolder ID="plcOriginalDocument" runat="server">
                        <table class="tbl" id="tbl">
                            <tbody>
                                <tr>
                                    <th>
                                        <asp:Literal ID="lblMainHeader" runat="server" />
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="original">
                                            <p class="icon">
                                                <asp:Literal ID="litMainDocImg" runat="server" /></p>
                                            <p class="row">
                                                <strong>
                                                    <asp:Literal ID="lblMainDocStatus" runat="server" /></strong>
                                                <asp:Literal ID="litMainDocStatus" runat="server" /></p>
                                            <asp:Panel ID="pnlMainDocType" Visible="false" runat="server">
                                                <p class="row">
                                                    <strong>
                                                        <asp:Literal ID="lblMainDocType" runat="server" /></strong>
                                                    <asp:Literal ID="litMainDocType" runat="server" /></p>
                                            </asp:Panel>
                                            <p class="row">
                                                <strong>
                                                    <asp:Literal ID="lblMainDocOriginalFilename" runat="server" /></strong>
                                                <asp:Literal ID="litMainDocOriginalFilename" runat="server" /></p>
                                            <p class="row">
                                                <strong>
                                                    <asp:Literal ID="lblMainDocSize" runat="server" /></strong>
                                                <asp:Literal ID="litMainDocSize" runat="server" /></p>
                                            <asp:Panel ID="pnlMainDocFilename" Visible="false" runat="server">
                                                <p class="row">
                                                    <strong>
                                                        <asp:Literal ID="lblMainDocFilename" runat="server" /></strong>
                                                    <asp:Literal ID="litMainDocFilename" runat="server" /></p>
                                            </asp:Panel>
                                            <%--<asp:PlaceHolder ID="plcMainVerifyStatus" runat="server" Visible="false"><p class="row"><strong><asp:Literal ID="lblMainVerifyStatus" runat="server" /></strong> <asp:Literal ID="litMainVerifyStatus" runat="server" /></p></asp:PlaceHolder>--%>
                                            <p>
                                                <asp:PlaceHolder ID="plcStatusSignedAndCRL" runat="server" Visible="false">
                                                    <div class="row9" style="margin: 0 0 0 45px">
                                                        <div class="col6">
                                                            <asp:Literal runat="server" ID="ltDateCheckUltimate"></asp:Literal>
                                                        </div>
                                                        <div class="col7" style="font-weight: normal">
                                                            <asp:Label ID="lblDateCheckUltimateText" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="row9" style="margin: 0 0 0 45px">
                                                        <div class="col6">
                                                            <asp:Literal runat="server" ID="ltCheckSigned"></asp:Literal>
                                                        </div>
                                                        <div class="col7" style="font-weight: normal">
                                                            <asp:Label ID="lblCheckSigned" runat="server"></asp:Label>
                                                        </div>
                                                        <div class="col4">
                                                            <asp:Image ID="imgCheckSignedResult" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="row9" style="margin: 0 0 0 45px">
                                                        <div class="col6">
                                                            <asp:Literal runat="server" ID="ltCheckCRL"></asp:Literal>
                                                        </div>
                                                        <div class="col7" style="font-weight: normal">
                                                            <asp:Label ID="lblCRL" runat="server"></asp:Label>
                                                        </div>
                                                        <div class="col4">
                                                            <asp:Image ID="imgCheckCRLResult" runat="server" />
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </p>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcTable" runat="server">
                        <asp:Table ID="tblSignedDocument" runat="server" CssClass="tbl" />
                    </asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnView" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnView_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnVerifyCRL" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="CheckDate();" OnClick="BtnVerifyCRL_Click" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
            <asp:HiddenField ID="hdnDate" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
