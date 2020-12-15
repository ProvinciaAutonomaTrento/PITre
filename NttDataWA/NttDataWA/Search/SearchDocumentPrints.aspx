<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchDocumentPrints.aspx.cs"
    MasterPageFile="~/MasterPages/Base.Master" Inherits="NttDataWA.Search.SearchDocumentPrints" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Src="~/UserControls/SearchDocumentsTabs.ascx" TagPrefix="uc2" TagName="SearchDocumentTabs" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $('#contentSx input, #contentSx textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentSx select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        });

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

        function cb_selectall() {
            $('#HiddenItemsAll').val('true');
            $('#btnCbSelectAll').click();
        }

        function SetItemCheck(obj, id) {
            $('#HiddenItemsAll').val('');

            if (obj.checked) {
                var value = $('#HiddenItemsChecked').val();
                var values = new Array(value);
                if (value.indexOf(',') >= 0) values = value.split(',');

                values.push(id);
                value = values.join(',');
                if (value.substring(0, 1) == ',')
                    value = value.substring(1);
                $('#HiddenItemsChecked').val(value);
            }
            else {
                var value = $('#HiddenItemsChecked').val();
                var values = new Array(value);
                if (value.indexOf(',') >= 0) values = value.split(',');
                var found = false;

                for (var i = 0; i < values.length; i++) {
                    if (values[i] == id) {
                        values.splice(i, 1);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    $(".gridViewResult th input[type='checkbox']").attr('checked', false);

                    value = $('#HiddenItemsUnchecked').val();
                    values = new Array(value);
                    if (value.indexOf(',') >= 0) values = value.split(',');
                    values.push(id);

                    value = values.join(',');
                    if (value.substring(0, 1) == ',')
                        value = value.substring(1);
                    $('#HiddenItemsUnchecked').val(value);
                }
                else {
                    value = values.join(',');
                    if (value.substring(0, 1) == ',')
                        value = value.substring(1);
                    $('#HiddenItemsChecked').val(value);
                }
            }
        }

        function clearCheckboxes(all, v) {
            if (all == 'false') {
                $(".gridViewResult td input[type='checkbox']").attr('checked', false);

                var values = new Array(v);
                if (v.indexOf(',') >= 0) values = v.split(',');
                for (var i = 0; i < values.length; i++) {
                    $(".gridViewResult span.pr" + values[i] + " input[type='checkbox']").attr('checked', true);
                }
            }
            else {
                $(".gridViewResult td input[type='checkbox']").attr('checked', true);
            }
        }
    </script>
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
        #gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        .gridViewResult
        {
            min-width: 99%;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="GrigliaPersonalizzata" runat="server" Url="../popup/GridPersonalization.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzataSave" runat="server" Url="../popup/GridPersonalizationSave.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GridPersonalizationPreferred" runat="server" Url="../popup/GridPersonalizationPreferred.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup2 Id="MassiveAddAdlUser" runat="server" Url="../Popup/MassiveAddAdlUser.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlUser" runat="server" Url="../Popup/MassiveRemoveAdlUser.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveAddAdlRole" runat="server" Url="../Popup/MassiveAddAdlRole.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlRole" runat="server" Url="../Popup/MassiveRemoveAdlRole.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConservation" runat="server" Url="../Popup/MassiveConservation.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveVersPARER" runat="server" Url="../Popup/MassiveVers.aspx?isParer=true" 
         Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveTransmission" runat="server" Url="../Popup/MassiveTransmission.aspx?objType=D"
        IsFullScreen="true" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConversion" runat="server" Url="../Popup/MassivePdfConversion.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveTimestamp" runat="server" Url="../Popup/MassiveTimestamp.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConsolidation" runat="server" Url="../Popup/MassiveConsolidation.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConsolidationMetadati" runat="server" Url="../Popup/MassiveConsolidation.aspx?objType=D&metadati=true"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveForward" runat="server" Url="../Popup/MassiveForward.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveCollate" runat="server" Url="../Popup/MassiveCollate.aspx"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveVersions" runat="server" Url="../Popup/MassiveRemoveVersions.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignature" runat="server" Url="../Popup/MassiveDigitalSignature.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignatureApplet" runat="server" Url="../Popup/MassiveDigitalSignature_applet.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
        <uc:ajaxpopup2 Id="MassiveDigitalSignatureSocket" runat="server" Url="../Popup/MassiveDigitalSignature_Socket.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=doc&fromMassiveOperation=1"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolarioMassive" runat="server" Url="../Popup/ClassificationScheme.aspx?from=search&massive=true"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="SearchProjectMassive" runat="server" Url="../popup/SearchProject.aspx?caller=search"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
         <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeZoom');}" />
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
                                    <p>
                                        <asp:Literal ID="LitSearchProject" runat="server"></asp:Literal></p>
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
                                <uc2:SearchDocumentTabs ID="SearchDocumentTabs" runat="server" PageCaller="PRINTS" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="colMassiveOperationSx">
                                            <asp:DropDownList runat="server" ID="SearchDocumentDdlMassiveOperation" Width="400"
                                                AutoPostBack="true" CssClass="chzn-select-deselect" OnSelectedIndexChanged="SearchDocumentDdlMassiveOperation_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="colMassiveOperationDx">
                                            <cc1:CustomImageButton ID="projectImgSaveGrid" ImageUrl="../Images/Icons/save_grid.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/save_grid_hover.png" OnMouseOutImage="../Images/Icons/save_grid.png"
                                                ImageUrlDisabled="../Images/Icons/save_grid_disabled.png" CssClass="clickableLeft"
                                                Enabled="false" OnClientClick="return ajaxModalPopupGrigliaPersonalizzataSave();" />
                                            <cc1:CustomImageButton ID="projectImgEditGrid" ImageUrl="../Images/Icons/edit_grid.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/edit_grid.png" OnMouseOutImage="../Images/Icons/edit_grid.png"
                                                ImageUrlDisabled="../Images/Icons/edit_grid_disabled.png" CssClass="clickableLeft"
                                                OnClientClick="return ajaxModalPopupGrigliaPersonalizzata();" />
                                            <cc1:CustomImageButton ID="projectImgPreferredGrids" ImageUrl="../Images/Icons/preferred_grids.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/preferred_grids.png" OnMouseOutImage="../Images/Icons/preferred_grids.png"
                                                ImageUrlDisabled="../Images/Icons/preferred_grids_disabled.png" CssClass="clickableLeft"
                                                OnClientClick="return ajaxModalPopupGridPersonalizationPreferred();" />
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
                        <div class="box_inside">
                            <asp:PlaceHolder runat="server" ID="PlcRepertories" Visible="false">
                                <div class="row">
                                    <fieldset class="basic">
                                        <div class="row">
                                            <div class="col">
                                                <asp:UpdatePanel runat="server" ID="UpPnlSelectPrintType" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:RadioButtonList runat="server" ID="RblTypePrints" RepeatDirection="Horizontal"
                                                            AutoPostBack="true" OnSelectedIndexChanged="RblTypePrints_OnSelectedIndexChanged">
                                                            <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                            </asp:PlaceHolder>
                            <asp:UpdatePanel runat="server" ID="UpPnlPrintRegistry" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:PlaceHolder runat="server" ID="PnlPrintRegistry">
                                        <%-- ******************** PROTOCOLLO ******************** --%>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <asp:UpdatePanel ID="UpPnlYear" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="SearchDocumentYear"></asp:Literal></span></p>
                                                                </div>
                                                                <div class="col">
                                                                    <p>
                                                                        <cc1:CustomTextArea ID="TxtYear" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled"
                                                                            ClientIDMode="Static" /></p>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                                <div class="row">
                                                    <asp:UpdatePanel runat="server" ID="UpProtocollo" UpdateMode="Conditional" ClientIDMode="static">
                                                        <ContentTemplate>
                                                            <%-- NUMERO PROTOCOLLO --%>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlNumProto"></asp:Literal></p>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <asp:DropDownList ID="cboFilterTypeNumProtocollo" runat="server" AutoPostBack="True"
                                                                        CssClass="chzn-select-deselect" Width="140px" OnSelectedIndexChanged="ddl_numProt_E_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaNumProto" Visible="false"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txtInitNumProtocollo" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                        CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlANumProto" Visible="false"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txtEndNumProtocollo" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                        CssClassReadOnly="txt_input_full_disabled" Visible="false"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </fieldset>
                                        </div>
                                        <%--******************* REGISTRO ***************--%>
                                        <asp:UpdatePanel runat="server" ID="UpRegistro" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:PlaceHolder ID="plcRegistro" runat="server">
                                                    <div class="row">
                                                        <fieldset>
                                                            <div class="col">
                                                                <strong>
                                                                    <asp:Literal runat="server" ID="LtlRegistro"></asp:Literal></strong>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <asp:ListBox ID="lb_reg_C" runat="server" AutoPostBack="True" CssClass="txt_textarea"
                                                                        CssClassReadOnly="txt_textarea_disabled" Width="240px" Rows="3" SelectionMode="Multiple">
                                                                    </asp:ListBox>
                                                                </div>
                                                                <div class="col">
                                                                    <asp:RadioButtonList ID="rbl_Reg_C" runat="server" AutoPostBack="True" Width="98px" />
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder runat="server" ID="PnlPrintRepertories" Visible="false">
                                        <asp:UpdatePanel runat="server" ID="UpPnlRegistry" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal runat="server" ID="LitSearchRepertories"></asp:Literal>
                                                                    </span>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <asp:DropDownList ID="ddl_repertori" runat="server" OnSelectedIndexChanged="ddl_repertori_SelectedIndexChanged"
                                                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="97%">
                                                                    <asp:ListItem Text=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal runat="server" ID="LitSearchRepertoriesAoo"></asp:Literal>
                                                                    </span>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <asp:DropDownList ID="ddl_aoo_rf" runat="server" CssClass="chzn-select-deselect"
                                                                    Width="97%">
                                                                    <asp:ListItem Text=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel runat="server" ID="UpPnlYearNumberRepertory" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal runat="server" ID="SearchDocumentPrintsLitNum"></asp:Literal>
                                                                    </span>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <asp:DropDownList ID="Ddl_num_rep" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                                    Width="140px" OnSelectedIndexChanged="Ddl_num_rep_SelectedIndexChanged">
                                                                    <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                                    <asp:ListItem Value="1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                            <div class="col2">
                                                                <asp:Literal runat="server" ID="LtlDaNumRep" Visible="false"></asp:Literal>
                                                            </div>
                                                            <div class="col4">
                                                                <cc1:CustomTextArea ID="txtInitNumRep" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                    CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                            </div>
                                                            <div class="col2">
                                                                <asp:Literal runat="server" ID="LtlANumRep" Visible="false"></asp:Literal>
                                                            </div>
                                                            <div class="col4">
                                                                <cc1:CustomTextArea ID="txtEndNumRep" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                    CssClassReadOnly="txt_input_full_disabled" Visible="false"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal runat="server" ID="SearchDocumentPrintsLitYearRep"></asp:Literal>
                                                                    </span>
                                                                </p>
                                                            </div>
                                                            <div class="col">
                                                                <p>
                                                                    <cc1:CustomTextArea ID="TxtYearRep" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled"
                                                                        ClientIDMode="Static" /></p>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel runat="server" ID="UpPnlTypePrints" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="row">
                                                            <div class="col">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="SearchDocumentPrintsLitTypePrints"></asp:Literal>
                                                                </span>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <asp:RadioButtonList runat="server" ID="rbl_TipiStampe" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="TUTTE" Selected="True"></asp:ListItem>
                                                                    <asp:ListItem Value="NON_FIRMATE"></asp:ListItem>
                                                                    <asp:ListItem Value="FIRMATE"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" ID="UpPnlDocNumber" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="SearchDocumentPrintsLitIdDoc"></asp:Literal>
                                                        </span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="ddl_idDocumento_C" runat="server" Width="140px" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                         OnSelectedIndexChanged ="ddl_idDocumento_C_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlDaIdDoc" Visible="false"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initIdDoc_C" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                  CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlAIdDoc" Visible="false"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineIdDoc_C" runat="server" Width="80px" Visible="false"
                                                                 CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" ID="UpDataArrivo" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LtlDataArrivo"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="cboFilterTypeDataStampa" runat="server" AutoPostBack="true"
                                                        Width="140px" CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_dataArrivo_C_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlDaDataArrivo"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txtInitDataStampa" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlADataArrivo" Visible="false"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txtEndDataStampa" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled" Visible="false"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <!-- STATO CONSERVAZIONE -->
                            <asp:UpdatePanel runat="server" ID="UpStatoConservazione" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LtlStatoConservazione"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:CheckBoxList ID="cbl_Conservazione" runat="server" CssClass="testo_grigio" RepeatDirection="Vertical">
                                                        <asp:ListItem Value="N" Selected="True" runat="server" id="optConsNC"></asp:ListItem>
                                                        <asp:ListItem Value="V" Selected="True" runat="server" id="optConsAtt"></asp:ListItem>
                                                        <asp:ListItem Value="W" Selected="True" runat="server" id="optConsVer"></asp:ListItem>
                                                        <asp:ListItem Value="C" Selected="True" runat="server" id="optConsPre"></asp:ListItem>
                                                        <asp:ListItem Value="R" Selected="True" runat="server" id="optConsRif"></asp:ListItem>
                                                        <asp:ListItem Value="E" Selected="True" runat="server" id="optConsErr"></asp:ListItem>
                                                        <asp:ListItem Value="T" Selected="True" runat="server" id="optConsTim"></asp:ListItem>
                                                        <asp:ListItem Value="F" Selected="True" runat="server" id="optConsFld"></asp:ListItem>
                                                    </asp:CheckBoxList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LtlDataVers"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_DataVers" runat="server" AutoPostBack="true" Width="140px" 
                                                                OnSelectedIndexChanged="ddl_dataVers_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                    </asp:DropDownList>                              
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlDaDataVers"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initDataVers" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlADataVers"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineDataVers" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LtlPolicy"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Literal runat="server" ID="LtlCodPolicy"></asp:Literal>
                                                </div>
                                                <div class="col">
                                                    <cc1:CustomTextArea runat="server" ID="txtCodPolicy" Columns="8" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col">
                                                    <asp:Literal runat="server" ID="LtlCounterPolicy"></asp:Literal>
                                                </div>
                                                <div class="col">
                                                    <cc1:CustomTextArea runat="server" ID="txtCounterPolicy" Columns="4" CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Literal runat="server" ID="ltlDatePolicy"></asp:Literal> 
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_datePolicy" runat="server" AutoPostBack="true" Width="140px" OnSelectedIndexChanged="ddl_datePolicy_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlDaDatePolicy"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initDatePolicy" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlADatePolicy"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineDatePolicy" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
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
                                                    <asp:Label runat="server" ID="projectLblNumeroDocumenti"></asp:Label></p>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
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
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="SearchDocumentPrintsSearch" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentAdvancedSearch_Click" />
            <cc1:CustomButton ID="SearchDocumentRemoveFilters" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentRemoveFilters_Click" />
            <asp:HiddenField ID="HiddenRemoveUsedSearch" runat="server" ClientIDMode="Static" />
            <asp:Button ID="btnCbSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="addAll_Click" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsAll" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
