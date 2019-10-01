<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdlDocument.aspx.cs" Inherits="NttDataWA.Home.AdlDocument"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="../UserControls/HomeTabs.ascx" TagPrefix="uc1" TagName="HomeTabs" %>
<%@ Register Src="../UserControls/HeaderHome.ascx" TagPrefix="uc2" TagName="HeaderHome" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
   
    <%--<script src="<%: ResolveUrl("~/Scripts/bootstrap.min.js") %>" type="text/javascript"></script>--%>
    <link href="../Css/bootstrap.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .gridViewResult
        {
            min-width: 100%;
        }
          .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        #gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }

    </style>
    <script type="text/javascript">
        function CombineRowsHover() {
            $(".tbl_rounded tr.NormalRow td").hover(function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).addClass("NormalRowHover");
            }, function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).removeClass("NormalRowHover");
            });

            $(".tbl_rounded tr.AltRow td").hover(function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).addClass("AltRowHover");
            }, function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).removeClass("AltRowHover");
            });
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
     <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx" ForceDontClose="true"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="MassiveReportDragAndDrop" runat="server" Url="../popup/MassiveReportDragAndDrop.aspx"
        Width="700" Height="500" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'POPUP_DRAG_AND_DROP');}" />
    <uc:ajaxpopup2 Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <div id="containerTop">
        <div id="containerTabHome">
            <asp:UpdatePanel runat="server" ID="UpHeaderHome" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
                    <div id="containerStandardTop">
                        <div id="containerStandardTopCxHome">
                            <img src="../Images/Common/griff.png" alt="" title="" />
                        </div>
                        <div id="containerHomeHeader">
                            <div id="containerHeaderHomeSx">
                                <strong>
                                    <asp:Label runat="server" ID="headerHomeLblRole"></asp:Label>
                                </strong>
                            </div>
                            <div id="containerHeaderHomeDx">
                                <div class="styled-select_full">
                                    <asp:DropDownList ID="ddlRolesUser" runat="server" OnSelectedIndexChanged="HomeDdlRoles_SelectedIndexChange"
                                        CssClass="chzn-select-deselect" AutoPostBack="true" Width="700px">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <div id="containerTabIndex">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc1:HomeTabs runat="server" PageCaller="ADL_DOCUMENT" ID="HomeTabs"></uc1:HomeTabs>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:UpdatePanel runat="server" ID="UpdatePanelFilterSx">
                        <ContentTemplate>
                            <asp:PlaceHolder runat="server" ID="PnlAdlRoleUserHome" Visible="false">
                                <div id="containerAdlHome">
                                    <div class="weight">
                                        <asp:RadioButtonList runat="server" ID="RblTypeAdl" RepeatDirection="Horizontal"
                                            RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="RblTypeAdl_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="containerNotificationCenter">
                <div id="containerNotificationCenterAdlHome">
                    <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                        <ContentTemplate>
                            <asp:GridView ID="gridViewResult" runat="server" AllowSorting="true"
                                AllowPaging="false" AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                OnRowDataBound="gridViewResult_RowDataBound" OnSorting="gridViewResult_Sorting"
                                OnPreRender="gridViewResult_PreRender" OnRowCreated="gridViewResult_ItemCreated" OnRowCommand="GridView_RowCommand">
                                <Columns>
                                </Columns>
                            </asp:GridView>
                            <asp:PlaceHolder ID="plcNavigator" runat="server" />
                            <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <div id="UploadLiveuploads"  runat="server" Visible="false">
        <div class="upload-dialog" id="upload-liveuploads" data-bind="template: { name: 'template-uploads' }"></div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
     <asp:UpdatePanel runat="server" ID="UpDocumentButtons" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
        <script type="text/html" id="template-uploads">
            <div class="upload-dialog-title-container" data-bind="visible: inUpload()">
                <div class="upload-dialog-title">
                    <div class="upload-dialog-title-uploaded">
                        <span>File caricati</span>
                    </div>
                </div>
            </div>
            <div data-bind="visible: showTotalProgress()">
                <div>
                    <span data-bind="text: uploadSpeedFormatted()"></span>
                    <span data-bind="text: timeRemainingFormatted()" style="float: right;"></span>
                </div>
                <div class="upload-totalprogress">
                    <div class="upload-totalprogressbar" style="width: 0%;" data-bind="style: { width: totalProgress() + '%' }"></div>
                </div>
            </div>
            <div data-bind="visible: inUpload()" >
                <div class="upload-dialog-list" data-bind="foreach: uploads">
                    <div class="upload-upload">
                        <div class="upload-fileinfo upload-dialog-row-fluid">
                            <div class="upload-dialog-col-md-10" style="margin-top: 8px;">
                                <strong data-bind="text: fileName"></strong>
                                <span data-bind="text: fileSizeFormated"></span>
                                <span class="upload-progresspct" data-bind="visible: uploadProgress() < 100"><span data-bind="    text: uploadSpeedFormatted()"></span></span>
                            </div>
                            <div class="upload-dialog-col-md-2">
                                <div class="upload-uploadcompleted" data-bind="visible: uploadCompleted()">
                                    <div class="upload-uploadsuccessful" data-bind="visible: uploadSuccessful()"></div>
                                    <div class="upload-uploadfailed" data-bind="visible: !uploadSuccessful()"></div>
                                </div>
                            </div>
                        </div>
                        <div class="upload-progress">
                            <div class="upload-progressbar" style="width: 0%;" data-bind="style: { width: uploadProgress() + '%' }, visible: !uploadCompleted()"></div>
                        </div>
                    </div>
                </div>
            </div>
    </script>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
    <script type="text/javascript" data-main="<%=Page.ResolveClientUrl("~/Scripts/AdlDocumentDragAndDrop.js") %>" src="<%=Page.ResolveClientUrl("~/Scripts/require.js") %>"></script>
</asp:Content>
