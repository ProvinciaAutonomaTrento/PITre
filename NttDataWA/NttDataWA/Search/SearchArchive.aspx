<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="SearchArchive.aspx.cs" Inherits="NttDataWA.Search.SearchArchive" %>
<%@ Register Src="~/UserControls/SearchProjectsTabs.ascx" TagPrefix="uc2" TagName="SearchProjectsTabs" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="ctw" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
     <style type="text/css">
        .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        .row
        {
            min-height: 25px;
        }
        #gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        
        .gridViewResult
        {
            min-width:99%;
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
        <uc:ajaxpopup2 Id="GrigliaPersonalizzata" runat="server" Url="../popup/GridPersonalization.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzataSave" runat="server" Url="../popup/GridPersonalizationSave.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GridPersonalizationPreferred" runat="server" Url="../popup/GridPersonalizationPreferred.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx" ForceDontClose="true"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeZoom');}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p><asp:Literal ID="LitSearchProject" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:SearchProjectsTabs ID="SearchProjectsTabs" runat="server" PageCaller="ARCHIVE" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="colMassiveOperationSx">
                                          
                                        </div>
                                        <div class="colMassiveOperationDx">
                                         
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside3">
                            <asp:UpdatePanel ID="UpPnlRegistry" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="plcRegistry" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="colHalf">
                                                <strong><asp:Literal ID="litRegistry" runat="server" /></strong>
                                            </div>
                                            <div class="col">
                                                <asp:DropDownList runat="server" ID="ddlRegistri" Width="300"
                                                    AutoPostBack="true" onselectedindexchanged="ddlRegistri_SelectedIndexChanged" CssClass="chzn-select-deselect" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel ID="UpPnlTitolario" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="plcTitolario" runat="server" Visible="true">
                                        <div class="row" style="background-color: #eeeeee">
                                            <div class="colHalf">
                                                <strong>
                                                    <asp:Literal ID="litTitolario" runat="server" /></strong></div>
                                            <div class="col">
                                                <asp:DropDownList ID="ddlTitolario" runat="server" CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddlTitolario_SelectedIndexChanged" AutoPostBack="true"
                                                    Width="300" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <asp:UpdatePanel ID="UpPnlTree" runat="server">
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="plcDescription" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="colHalf">
                                                <strong><asp:Literal ID="litDescription" runat="server" /></strong>
                                            </div>
                                            <div class="col">
                                                <cc1:CustomTextArea ID="TxtDescription" runat="server" CssClass="txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>

                                    <div class="row">
                                        <div id="TreeVieweArchive">
                                            <ctw:CustomTreeView ID="TreeTitolario" runat="server"
                                            OnTreeNodeExpanded="ExpandeTreeView"
                                            OnSelectedNodeChanged="TreeTitolario_SelectedNodeChanged">
                                                <SelectedNodeStyle CssClass="treeViewSelected" />
                                
                                            </ctw:CustomTreeView>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="contentDx">
                        <div id="contentDxTopProject">
                            <%--Label numero di elementi --%>
                            <asp:UpdatePanel runat="server" ID="UpnlNumerodocumenti" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col">
                                            <div class="p-padding-left">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="projectLitNomeGriglia" />
                                                        <asp:Label runat="server" ID="projectLblDocumentiFascicoliCount"></asp:Label></span>
                                                    <asp:Label runat="server" ID="projectLblNumeroDocumenti"></asp:Label>
                                                    <asp:PlaceHolder ID="plcGoFascicolo" runat="server" Visible="false">
                                                        <asp:LinkButton ID="lbGoFascicolo" runat="server" OnClick="lbGoFascicolo_Click" />
                                                    </asp:PlaceHolder>
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                                        <ContentTemplate>
                                            <asp:GridView ID="gridViewResult" runat="server" AllowSorting="false"
                                                AllowPaging="false" AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                                OnRowDataBound="gridViewResult_RowDataBound" OnPreRender="gridViewResult_PreRender" OnRowCommand="GridView_RowCommand">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
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
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="SearchProjectArchiveSearch" runat="server" CssClass="btnEnable" Visible="false"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectArchiveSearch_Click" />
            <cc1:CustomButton ID="SearchProjectArchiveRemove" runat="server" CssClass="btnEnable" Visible="false"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectArchiveRemove_Click" />

            <asp:HiddenField ID="TreeTitolario_SelectedNode" runat="server" />
            <asp:HiddenField ID="HiddenIsSearching" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>