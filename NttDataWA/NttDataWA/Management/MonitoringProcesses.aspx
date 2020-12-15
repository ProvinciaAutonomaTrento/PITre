<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonitoringProcesses.aspx.cs"
    Inherits="NttDataWA.Management.MonitoringProcesses" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.collapsed" });
        });

        function SetItemCheck(obj, id) {

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
    </script>
    <style type="text/css">
        .containerTreeView {
            clear: both;
            margin-bottom: 10px;
            min-height: 18px;
            margin: 0 0 10px 0;
            text-align: left;
            vertical-align: top;
            max-height: 150px;
            max-width: 100%;
            overflow: auto;
        }

        .TreeSignatureProcess {
            padding: 0;
            color: #0f64a1;
        }


            .TreeSignatureProcess img {
                width: 20px;
                height: 20px;
            }

        .TreeSignatureProcess_node a:link, .TreeSignatureProcess_node a:visited, .TreeSignatureProcess_node a:hover {
            padding: 0 5px;
        }

        .TreeSignatureProcess_selected {
            background-color: #477FAF;
            color: #fff;
        }

            .TreeSignatureProcess_selected a:link, .TreeSignatureProcess_selected a:visited, .TreeSignatureProcess_selected a:hover {
                padding: 0 5px;
                background-color: transparent;
                color: #fff;
            }


        #content {
            float: left;
            width: 99%;
            height: 99%;
            margin: 5px;
            overflow: auto;
        }

        .contentResult {
            margin-top: 10px;
        }

        #contentFilter {
            width: 98.5%;
        }

        .col2 p {
            margin: 0px;
            padding: 0px;
        }

        .row3 {
            clear: both;
            min-height: 25px;
            margin: 0 0 10px 0;
            text-align: left;
            vertical-align: top;
        }

        .col-marginSx2 {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }

            .col-marginSx2 p {
                margin: 0px;
                padding: 0px;
                font-weight: normal;
                margin-top: 4px;
            }

        .filterAddressbook {
            margin: 0px;
            margin-left: 5px;
            background-color: #e4f1f6;
            border: 0px;
            padding-left: 5px;
            padding-right: 5px;
            padding-bottom: 5px;
            padding-top: 5px;
            width: 100%;
            margin-top: 5px;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }

        .gridViewResult {
            min-width: 100%;
            overflow: auto;
        }

            .gridViewResult th {
                text-align: center;
                white-space: nowrap;
            }

            .gridViewResult td {
                text-align: center;
                padding: 5px;
            }

        #gridViewResult tr.selectedrow {
            background: #f3edc6;
            color: #333333;
        }

        .tbl_rounded tr.Borderrow {
            border-top: 2px solid #FFFFFF;
        }

        .tbl_rounded td {
            background: #fff;
            min-height: 1em;
            border: 1px solid #A8A8A8;
            border-top: 0;
            text-align: center;
        }

        .margin-left {
            padding-left: 5px;
        }

        .tbl_rounded tr.Borderrow:hover td {
            background-color: #b2d7f8;
        }

        .col-marginSx3 {
            float: left;
            width: 130px;
            margin: 0px 3px 0px 3px;
        }

            .col-marginSx3 p {
                margin: 0px;
                padding: 0px;
            }

        .col-marginSx4 {
            float: left;
            width: 100px;
            margin: 0px 3px 0px 3px;
        }

            .col-marginSx4 p {
                margin: 0px;
                padding: 0px;
            }

        .col-marginSx5 {
            float: left;
            width: 100px;
            margin: 0px 3px 0px 3px;
        }

            .col-marginSx5 p {
                margin: 0px;
                padding: 0px;
            }

        .col-marginSx6 {
            float: left;
            width: 50px;
            margin: 0px 3px 0px 3px;
        }

            .col-marginSx6 p {
                margin: 0px;
                padding: 0px;
            }

        .col5 {
            float: left;
            margin: 0px 35px 0px 0px;
            text-align: left;
        }

        .contentFilterMonitoring {
            width:85%;
        }
    </style>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', 'closePopupAddressBook');}" />
    <uc:ajaxpopup2 Id="StateInstanceProcessSignature" runat="server" Url="../Popup/StateInstanceProcessSignature.aspx"
        PermitClose="false" Width="750" Height="500" PermitScroll="false" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="SendingReportMonitoring" runat="server" Url="../Popup/ReportSpedizioni.aspx?caller=monitoring"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('upPnlInfoUser', '');}" />
    <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx">
                </div>
                <div id="containerStandardTopCx">
                    <p>
                        <asp:Literal ID="ManagementMonitoringProcesses" runat="server"></asp:Literal>
                    </p>
                </div>
                <div id="containerStandardTopDx">
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
        <div id="containerStandard" runat="server" clientidmode="Static" style="overflow: hidden">
            <div id="content">
                <asp:Panel ID="pnlNoProcesses" runat="server" Visible="false">
                    <div style="text-align: left; font-size: small; font-weight: bold; padding-top: 10px; padding-bottom: 10px; padding-left: 5px">
                        <asp:Label ID="lblNoProcesses" runat="server"></asp:Label>
                    </div>
                </asp:Panel>
                <asp:UpdatePanel ID="UpFilters" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="contentFilter">
                            <fieldset class="filterAddressbook">
                                <asp:Panel ID="pnlFilter" runat="server">
                                    <div class="row">
                                        <asp:PlaceHolder ID="phFiltriProcesso" runat="server">
                                            <div class="row">
                                                <h2 class="expand">
                                                    <asp:Literal runat="server" ID="ltlProcesso"></asp:Literal>
                                                </h2>
                                                <div id="Div1" class="collapse shown" runat="server">
                                                    <div id="contentSxMonitoring">
                                                        <div class="contentFilterMonitoring">
                                                            <%-- ************** PROCESSO ******************** --%>
                                                            <div class="containerTreeView">
                                                                <asp:UpdatePanel ID="UpPnlProcesses" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Panel ID="PnlProcesses" runat="server">
                                                                            <asp:TreeView ID="TreeProcessSignature" runat="server" ExpandLevel="10" ShowLines="true"
                                                                                NodeStyle-CssClass="TreeSignatureProcess_node" SelectedNodeStyle-CssClass="TreeSignatureProcess_selected"
                                                                                OnSelectedNodeChanged="TreeSignatureProcess_SelectedNodeChanged" OnTreeNodeCollapsed="TreeSignatureProcess_Collapsed"
                                                                                OnTreeNodeExpanded="TreeSignatureProcess_Collapsed" CssClass="TreeSignatureProcess" />
                                                                        </asp:Panel>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                                                                                          <%-- STATO --%>
                                                              <div class="row">
                                                                <div class="col5">
                                                                    <div class="col-marginSx6">
                                                                        <p>
                                                                            <span class="weight">
                                                                                <asp:Literal runat="server" ID="LtlState"></asp:Literal>
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <asp:CheckBoxList ID="cbxState" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                                            <asp:ListItem Value="IN_EXEC" Selected="True" runat="server" id="opIN_EXEC"></asp:ListItem>
                                                                            <asp:ListItem Value="STOPPED" Selected="True" runat="server" id="opSTOPPED"></asp:ListItem>
                                                                            <asp:ListItem Value="CLOSED" Selected="True" runat="server" id="opCLOSED"></asp:ListItem>
                                                                            <asp:ListItem Value="IN_ERROR" Selected="True" runat="server" id="opIN_ERROR"></asp:ListItem>
                                                                        </asp:CheckBoxList>
                                                                    </div>
                                                                </div>
                                                            </div>    
                                                            <div class="row">
                                                                <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="ltlNomeProcesso"></asp:Literal>
                                                                        </span>
                                                                    </p>
                                                                </div>
                                                              </div>
                                                             <div class="row3">
                                                                <cc1:CustomTextArea ID="ctxNomeProcesso" runat="server" CssClass="txt_addressBookLeft"
                                                                    CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                                             </div>
                                                                                                                  
                                                            <%-- TIPO VISIBILITA --%>
                                                            <div class="row">
                                                                <div class="col-marginSx5">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlTipoVisibilita"></asp:Literal>
                                                                        </span>
                                                                    </p>
                                                                </div>
                                                                <div class="col4">
                                                                    <asp:CheckBoxList ID="CbxTipoVisibilita" runat="server" RepeatDirection="Vertical">
                                                                        <asp:ListItem Id="cb_avviatiDaMe" Value="U" Selected="True" runat="server"></asp:ListItem>
                                                                        <asp:ListItem Id="cb_avviatiDalRuolo"  Value="R" runat="server"></asp:ListItem>
                                                                        <asp:ListItem Id="cb_avviatoDaAltroRuolo"  Value="M" runat="server"></asp:ListItem>
                                                                    </asp:CheckBoxList>
                                                                </div>
                                                            </div>
                                                            <asp:Panel ID="PnlTitolare" runat="server">
                                                                <div class="contentTitolare">
                                                                    <div class="row3">
                                                                        <%-- ************** RUOLO TITOLARE******************** --%>
                                                                        <asp:UpdatePanel ID="UpdPnlRuoloTitolare" runat="server" UpdateMode="Conditional">
                                                                            <ContentTemplate>
                                                                                <div class="row">
                                                                                    <div class="col">
                                                                                        <p>
                                                                                            <span class="weight">
                                                                                                <asp:Literal ID="ltlRuoloTitolare" runat="server" /></span>
                                                                                        </p>
                                                                                    </div>
                                                                                    <div class="col-right-no-margin">
                                                                                        <cc1:CustomImageButton runat="server" ID="ImgAddressBookRuoloTitolare" ImageUrl="../Images/Icons/address_book.png"
                                                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                                            OnClick="BtnAddressBookRuoloTitolare_Click" />
                                                                                    </div>
                                                                                </div>
                                                                                <div class="row">
                                                                                    <asp:HiddenField ID="idRuoloTitolare" runat="server" />
                                                                                    <div class="colHalf">
                                                                                        <cc1:CustomTextArea ID="txtCodiceRuoloTitolare" runat="server" CssClass="txt_addressBookLeft"
                                                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoComplete="off"
                                                                                            OnTextChanged="TxtCode_OnTextChanged">
                                                                                        </cc1:CustomTextArea>
                                                                                    </div>
                                                                                    <div class="colHalf2">
                                                                                        <div class="colHalf3">
                                                                                            <cc1:CustomTextArea ID="txtDescrizioneRuoloTitolare" runat="server" CssClass="txt_addressBookRight"
                                                                                                CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <asp:Button ID="btnRuoloTitolare" runat="server" Text="vai" Style="display: none;" />
                                                                                <%--                                              <uc1:AutoCompleteExtender runat="server" ID="RapidRuoloTitolare" TargetControlID="txtDescrizioneRuoloTitolare"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                                                                MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                                OnClientPopulated="acePopulated">
                                                            </uc1:AutoCompleteExtender>--%>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </div>
                                                                    <div class="row3">
                                                                        <%-- ************** UTENTE TITOLARE******************** --%>
                                                                        <asp:UpdatePanel ID="UpdPnlUtenteTitolare" runat="server" UpdateMode="Conditional">
                                                                            <ContentTemplate>
                                                                                <div class="row">
                                                                                    <div class="col">
                                                                                        <p>
                                                                                            <span class="weight">
                                                                                                <asp:Literal ID="ltlUtenteTitolare" runat="server" /></span>
                                                                                        </p>
                                                                                    </div>
                                                                                    <div class="col-right-no-margin">
                                                                                        <cc1:CustomImageButton runat="server" ID="ImgAddressBookUtenteTitolare" ImageUrl="../Images/Icons/address_book.png"
                                                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                                            OnClick="BtnAddressBookUtenteTitolare_Click" />
                                                                                    </div>
                                                                                </div>
                                                                                <div class="row3">
                                                                                    <asp:HiddenField ID="idUtenteTitolare" runat="server" />
                                                                                    <div class="colHalf">
                                                                                        <cc1:CustomTextArea ID="txtCodiceUtenteTitolare" runat="server" CssClass="txt_addressBookLeft"
                                                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoComplete="off"
                                                                                            OnTextChanged="TxtCode_OnTextChanged">
                                                                                        </cc1:CustomTextArea>
                                                                                    </div>
                                                                                    <div class="colHalf2">
                                                                                        <div class="colHalf3">
                                                                                            <cc1:CustomTextArea ID="txtDescrizioneUtenteTitolare" runat="server" CssClass="txt_addressBookRight"
                                                                                                CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled">
                                                                                            </cc1:CustomTextArea>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <asp:Button ID="btnUtenteTitolare" runat="server" Text="vai" Style="display: none;" />
                                                                                <%--                                                            <uc1:AutoCompleteExtender runat="server" ID="RapidUtenteTitolare" TargetControlID="txtDescrizioneUtenteTitolare"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                                                                MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="aceSelectedUser" BehaviorID="AutoCompleteExIngressoBIS"
                                                                OnClientPopulated="acePopulatedUser">
                                                            </uc1:AutoCompleteExtender>--%>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </div>
                                                                </div>
                                                            </asp:Panel>
                                                            <div class="row3">
                                                                <%-- ARRESTATO --%>
                                                                    <div class="col">
                                                                        <asp:CheckBox Value="TRUNCATED" Checked="False" runat="server" ID="opTRUNCATED" />
                                                                    </div>
                                                            </div>
                                                            <%-- PASSI AUTOMATICI --%>
                                                            <asp:Panel ID="PnlTipoPassoAutomatico" runat="server">
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <p>
                                                                            <span class="weight">
                                                                                <asp:Literal ID="ltlTipoPassoAutomatico" runat="server" /></span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                                <div class="row3">
                                                                    <asp:DropDownList ID="DdlTipoPassoAutomatico" runat="server" Width="100%" CssClass="chzn-select-deselect">
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                    <div id="contentDxMonitoring">
                                                        <div class="contentFilterMonitoring">
                                                            <%-- ************** ID OGGETTO ******************** --%>
                                                            <asp:UpdatePanel runat="server" ID="UpPnlIdDoc" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <div class="row">
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlIdDoc"></asp:Literal>
                                                                        </span>
                                                                    </div>
                                                                    <div class="row3">
                                                                        <div class="col2">
                                                                            <asp:DropDownList ID="ddl_idDoc" runat="server" Width="140px" AutoPostBack="true"
                                                                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_idDoc_SelectedIndexChanged">
                                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col2">
                                                                            <asp:Literal runat="server" ID="LtlDaIdDoc"></asp:Literal>
                                                                        </div>
                                                                        <div class="col4">
                                                                            <cc1:CustomTextArea ID="txt_initIdDoc" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                        </div>
                                                                        <div class="col2">
                                                                            <asp:Literal runat="server" ID="LtlAIdDoc"></asp:Literal>
                                                                        </div>
                                                                        <div class="col2">
                                                                            <cc1:CustomTextArea ID="txt_fineIdDoc" runat="server" Width="80px" Visible="true"
                                                                                CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                        </div>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <div class="row">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlObject"></asp:Literal>
                                                                </span>
                                                            </div>
                                                            <div class="row3">
                                                                <cc1:CustomTextArea ID="txt_object" runat="server" CssClass="txt_addressBookLeft"
                                                                    CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                                            </div>

                                                            <%-- DATA DI AVVIO --%>
                                                            <div class="row">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlStartDate"></asp:Literal>
                                                                </span>
                                                            </div>
                                                            <div class="row3">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_StartDate" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="ddl_StartDate_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                        <asp:ListItem Value="5"></asp:ListItem>
                                                                        <asp:ListItem Value="6"></asp:ListItem>
                                                                        <asp:ListItem Value="7"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaStartDate"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_initStartDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlAStartDate"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_finedataStartDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlNotesStartup"></asp:Literal>
                                                                </span>
                                                            </div>
                                                            <div class="row3">
                                                                <cc1:CustomTextArea ID="txtNotesSturtup" runat="server" CssClass="txt_addressBookLeft"
                                                                    CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                                            </div>

                                                            <%-- DATA CONCLUSIONE --%>
                                                            <div class="row">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlCompletitionDate"></asp:Literal>
                                                                </span>
                                                            </div>
                                                            <div class="row3">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_CompletitionDate" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="ddl_CompletitionDate_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                        <asp:ListItem Value="5"></asp:ListItem>
                                                                        <asp:ListItem Value="6"></asp:ListItem>
                                                                        <asp:ListItem Value="7"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaCompletitionDate"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_initCompletitionDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlACompletitionDate"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_finedataCompletitionDate" runat="server" Width="80px"
                                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>

                                                            <%-- DATA INTERRUZIONE --%>
                                                            <div class="row">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlInterruptionDate"></asp:Literal>
                                                                </span>
                                                            </div>
                                                            <div class="row3">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_InterruptionDate" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="ddl_InterruptionDate_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                        <asp:ListItem Value="5"></asp:ListItem>
                                                                        <asp:ListItem Value="6"></asp:ListItem>
                                                                        <asp:ListItem Value="7"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaInterruptionDate"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_initInterruptionDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlAInterruptionDate"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_finedataInterruptionDate" runat="server" Width="80px"
                                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                            <%-- NOTE INTERRUZIONE --%>
                                                            <div class="row">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlNotesInterruption"></asp:Literal>
                                                                </span>
                                                            </div>
                                                            <div class="row3">
                                                                <cc1:CustomTextArea ID="txtNotesInterruption" runat="server" CssClass="txt_addressBookLeft"
                                                                    CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </div>
                                </asp:Panel>
                            </fieldset>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="contentResult">
                    <div class="row">
                        <%--Label numero di elementi --%>
                        <asp:UpdatePanel runat="server" ID="UpnlNumeroInstanceStarted" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col">
                                        <div class="p-padding-left">
                                            <p>
                                                <asp:Label runat="server" ID="monitoringProcessesResultCount"></asp:Label>
                                        </div>
                                    </div>
                                    <div>
                                        <cc1:CustomImageButton runat="server" ID="ImgRitenta" ImageUrl="../Images/Icons/retry.png"
                                            OnMouseOutImage="../Images/Icons/retry.png" OnMouseOverImage="../Images/Icons/retry_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/retry.png"
                                            OnClick="ImgRitenta_Click" />
                                        <cc1:CustomImageButton runat="server" ID="ImgReportSpedizioni" ImageUrl="../Images/Icons/received_sending.png"
                                            OnMouseOutImage="../Images/Icons/received_sending.png" OnMouseOverImage="../Images/Icons/received_sending_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/received_sending.png"
                                            OnClick="ImgReportSpedizioni_Click" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="row">
                            <asp:UpdatePanel runat="server" ID="UpPnlGridView" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gridViewResult" runat="server" Width="99%" AllowPaging="false"
                                        PageSize="10" AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                        HorizontalAlign="Center" ShowHeader="true" ShowHeaderWhenEmpty="true" OnRowDataBound="gridViewResult_RowDataBound"
                                        OnPreRender="gridViewResult_PreRender" OnRowCommand="gridViewResult_RowCommand" OnRowCreated="gridViewResult_ItemCreated">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="systemIdIstanzaProcesso" Text='<%# Bind("idIstanzaProcesso") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="cbxSelAll" runat="server" OnCheckedChanged="cbxSelAll_CheckedChanged" AutoPostBack="true" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="cbxSel" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:MonitoringProcessesTipo%>' HeaderStyle-Width="3%"
                                                ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="BtnDocument" Width="20px" CssClass="clickableRight"
                                                        CommandName="viewLinkObject" ToolTip='<%$ localizeByText:MonitoringProcessesGoToDocument%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="idDocumento" Text='<%# Bind("docNumber") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:MonitoringProcessesIdObject%>'>
                                                <ItemTemplate>
                                                    <span class="noLink"><b>
                                                        <asp:LinkButton runat="server" ID="idDocNumProto" Text='<%#this.GetIdDocNumProto((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>'
                                                            CommandName="viewLinkObject" CssClass="clickableRight" ToolTip='<%$ localizeByText:MonitoringProcessesGoToDocument%>'></asp:LinkButton>
                                                    </b></span>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:MonitoringProcessesObject%>'>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="oggetto" Text='<%#this.GetObject((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:MonitoringProcessesNomeProcesso%>' HeaderStyle-Width="15%">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="nomeProcesso" Text='<%# Bind("Descrizione") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessStartDate%>'
                                                HeaderStyle-Width="10%">
                                                <ItemTemplate>
                                                    <%--
                                                <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataAttivazione").ToString())%>--%>
                                                    <asp:Label runat="server" ID="DtaAvvio" Text='<%# Bind("dataAttivazione") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessNotesStartup%>'>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="NotesStartup" Text='<%# Bind("NoteDiAvvio") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessCompletitionDate%>'
                                                HeaderStyle-Width="10%">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="CompletitionDate" Text='<%#this.GetCompletitionDate((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessInterruptionDate%>'
                                                HeaderStyle-Width="10%" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="InterruptionDate" Text='<%#this.GetInterruptionDate((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessNotesInterruption%>'>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="NotesInterruption" Text='<%# Bind("MotivoRespingimento") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessState%>'
                                                HeaderStyle-Width="16%" ItemStyle-Wrap="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="State" Text='<%#this.GetState((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessNota%>'
                                                HeaderStyle-Width="10%" ItemStyle-Wrap="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="Nota" Text='<%#this.GetNota((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-Width="3%" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="BtnInstanceDetails" Width="20px" CssClass="clickableLeft"
                                                        CommandName="viewStateInstanceProcess" ToolTip='<%$ localizeByText:MonitoringImgProcessStateInstanceTooltip%>'
                                                        ImageUrl="../Images/Icons/LibroFirma/Process_State.png" OnMouseOutImage="../Images/Icons/LibroFirma/Process_State.png"
                                                        OnMouseOverImage="../Images/Icons/LibroFirma/Process_State_hover.png" ImageUrlDisabled="../Images/Icons/LibroFirma/Process_State_disabled.png" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
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
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="MonitoringProcessesSearch" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="MonitoringProcessesSearch_Click" />
            <cc1:CustomButton ID="MonitoringProcessesClearFilter" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="MonitoringProcessesClearFilter_Click" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
