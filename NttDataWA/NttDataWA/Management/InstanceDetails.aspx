<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Base.Master"
    CodeBehind="~/Management/InstanceDetails.aspx.cs" Inherits="NttDataWA.Management.InstaceDetails" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/InstanceTabs.ascx" TagPrefix="uc2" TagName="InstanceTabs" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function richiedentePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoRichiedente');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function richiedenteSelected(sender, e) {
            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=txtDescrizioneRichiedente.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneRichiedente.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneRichiedente.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceRichiedente.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneRichiedente.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceRichiedente.ClientID%>', '');
        }

        function cb_selectall() {
            $('#HiddenItemsAll').val('true');
            $('#btnCbSelectAll').click();
        }

        function SetProjectCheck(obj, id) {
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
                    $(".gridInstanceAccessDocuments th input[type='checkbox']").attr('checked', false);

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
                $(".gridInstanceAccessDocuments td input[type='checkbox']").attr('checked', false);

                var values = new Array(v);
                if (v.indexOf(',') >= 0) values = v.split(',');
                for (var i = 0; i < values.length; i++) {
                    $(".gridInstanceAccessDocuments span.pr" + values[i] + " input[type='checkbox']").attr('checked', true);
                }
            }
            else {
                $(".gridInstanceAccessDocuments td input[type='checkbox']").attr('checked', true);
            }
        }

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
    <style type="text/css">
        .tbl_rounded
        {
            width: 97%;
            border-collapse: collapse;
        }
        .tbl_rounded td
        {
            background: #fff;
            min-height: 1em;
            border: 1px solid #d4d4d4;
            border-top: 0;
            padding: 5px;
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
        .gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        .gridViewResult
        {
            min-width: 99%;
        }
        
        .stateDocumentGreen
        {
            margin-top:3px;
            float:right;
            width:15px; 
            height:15px;
            background-color:Green;
        }
        
        .stateDocumentRed
        {
            margin-top:3px;
            float:right;
            width:15px; 
            height:15px;
            background-color:Red;
        }
        
        .stateDocumentYellow
        {
            margin-top:3px;
            float:right;
            width:15px; 
            height:15px;
            background-color:Yellow;
        }
        
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 td
        {
            background: #ffffff;
            border: 0;
        }
        .recordNavigator2, .recordNavigator2 td
        {
            border: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="SearchDocument" runat="server" Url="../popup/SearchDocument.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpContainerProjectTab', '');}" />
    <uc:ajaxpopup2 Id="OpenAddDocCustom" runat="server" Url="../popup/AddDocInProject.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddDocPostback').click();}" />
    <uc:ajaxpopup2 Id="ChooseCorrespondent" runat="server" Url="../popup/ChooseCorrespondent.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="AddFilterDocInstanceAccess" runat="server" Url="../popup/AddFilterDocInstanceAccess.aspx"
        PermitClose="false" PermitScroll="false" Width="600" Height="700" CloseFunction="function (event, ui)  { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closePopupObject');}" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx?from=search"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../popup/SearchProject.aspx?caller=search"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'closePopupSearchProject');}" />
    <uc:ajaxpopup2 Id="SearchDocumentsInProject" runat="server" Url="../popup/SearchProject.aspx?caller=searchInstance"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'closePopupSearchDocumentsInProject');}" />
    <uc:ajaxpopup2 Id="CorrespondentDetails" runat="server" Url="../popup/CorrespondentDetails.aspx?instanceDetails=r"
        Width="680" Height="700" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ViewDetailsInstanceAccessDocument" runat="server" Url="../popup/ViewDetailsInstanceAccessDocument.aspx"
        Width="450" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../Popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="RemoveProfile" runat="server" Url="../popup/RemoveProfile.aspx"
        Width="500" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('container', '');}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerProjectTop">
                                <div id="containerInstanceTopSx">
                                    <p>
                                        <strong>
                                            <asp:Label runat="server" ID="projectLblCodice" Text="Istanza n° "></asp:Label></strong><span
                                                class="weight"><asp:Label runat="server" ID="projectLblCodiceGenerato"></asp:Label></span></p>
                                </div>
                                <div id="containerInstanceTopCx">
                                </div>
                                <div id="containerInstanceTopDx">
                                </div>
                            </div>
                            <div id="containerInstanceBottom">
                                <div id="containerProjectCxBottom">
                                    <div id="containerProjectCxBottomSx">
                                    </div>
                                    <div id="containerProjectCxBottomDx">
                                        <div id="containerProjectTopCxOrangeDxDx">
                                        </div>
                                    </div>
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
                <div id="containerDocumentTab" class="containerInstanceTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:InstanceTabs ID="InstanceTabs" runat="server" PageCaller="CONTENT" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel runat="server" ID="UpnlButtonTop" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col" style="margin-left: 80px;">
                                            <cc1:CustomImageButton ID="projectImgAddDoc" ImageUrl="../Images/Icons/add_doc_in_project.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/add_doc_in_project_hover.png"
                                                OnMouseOutImage="../Images/Icons/add_doc_in_project.png" CssClass="clickable"
                                                ImageUrlDisabled="../Images/Icons/add_doc_in_project_disabled.png" OnClientClick="return ajaxModalPopupSearchDocument();" />
                                            <cc1:CustomImageButton ID="InstanceImgAddPrj" ImageUrl="../Images/Icons/add_doc_from_project.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/add_doc_from_project_hover.png"
                                                OnMouseOutImage="../Images/Icons/add_doc_from_project.png" CssClass="clickable"
                                                ImageUrlDisabled="../Images/Icons/add_doc_from_project_disabled.png" OnClientClick="return ajaxModalPopupSearchDocumentsInProject();" />
                                            <cc1:CustomImageButton ID="projectImgAddFilter" ImageUrl="../Images/Icons/add_filters.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/add_filters_hover.png" OnMouseOutImage="../Images/Icons/add_filters.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/add_filters_disabled.png"
                                                Enabled="true" OnClientClick="return ajaxModalPopupAddFilterDocInstanceAccess();" />
                                            <cc1:CustomImageButton ID="projectImgRemoveFilter" ImageUrl="../Images/Icons/remove_filters.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/remove_filters_hover.png" OnMouseOutImage="../Images/Icons/remove_filters.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/remove_filters_disabled.png"
                                                Enabled="false" OnClick="projectImgRemoveFilter_Click" />
                                        </div>
                                        <div id="col" style="padding-top: 5px;">
                                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                                <ContentTemplate>
                                                    <asp:DropDownList runat="server" ID="InstanceDdlMassiveOperation" Width="400" AutoPostBack="true"
                                                        CssClass="chzn-select-deselect" OnSelectedIndexChanged="InstanceDdlMassiveOperation_SelectedIndexChanged">
                                                        
                                                    </asp:DropDownList>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="containerInstanceTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerInstance">
                <div id="content">
                    <div id="contentSxAccess">
                        <div class="box_inside">
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpPnlDateProject">
                                <ContentTemplate>
                                    <div class="row">
                                        <div id="dateProject">
                                            <div class="row">
                                                <div class="col" style="margin-top: 6px;">
                                                    <strong>
                                                        <asp:Label runat="server" ID="InstanceLblDataApertura"></asp:Label></strong>&nbsp;<asp:Label
                                                            runat="server" ID="projectLblDataAperturaGenerata"></asp:Label>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <strong>
                                                        <asp:Label runat="server" ID="InstancelblDataChiusura"></asp:Label></strong>&nbsp;<asp:Label
                                                            runat="server" ID="projectlblDataChiusuraGenerata"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="UpPnlDescription" UpdateMode="Conditional" ClientIDMode="Static">
                                    <ContentTemplate>
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Label runat="server" ID="InstanceLblDescrizione"> </asp:Label></span><span class="little">*</span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <cc1:CustomTextArea ID="projectTxtDescrizione" runat="server" TextMode="MultiLine"
                                                    CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                                            </div>
                                            <div class="row">
                                                <div class="col-right-no-margin">
                                                    <span class="charactersAvailable">
                                                        <asp:Literal ID="projectLtrDescrizione" runat="server" ClientIDMode="Static"> </asp:Literal>
                                                        <span id="projectTxtDescrizione_chars" clientidmode="Static" runat="server"></span>
                                                    </span>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row">
                                <div class="col" style="margin-left: 5px;">
                                    <p>
                                        <span class="weight">Estremi procedimenti</span>
                                    </p>
                                </div>
                            </div>
                            <div class="row">
                                <fieldset style="margin-top: 5px;">
                                    <asp:UpdatePanel ID="upPnlRichiedente" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="litRichiedente" runat="server" /></span><span class="little">*</span>
                                                    </p>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <cc1:CustomImageButton runat="server" ID="ImgRichiedenteAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                        OnClick="ImgRichiedenteAddressBook_Click" />
                                                    <cc1:CustomImageButton runat="server" ID="ImgRichiedenteDetails" ImageUrl="../Images/Icons/address_book_details.png"
                                                        OnMouseOutImage="../Images/Icons/address_book_details.png" OnMouseOverImage="../Images/Icons/address_book_details_hover.png"
                                                        CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_details_disabled.png"
                                                        OnClick="ImgRichiedenteDetails_Click" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <asp:HiddenField ID="idRichiedente" runat="server" />
                                                <div class="colHalf">
                                                    <cc1:CustomTextArea ID="txtCodiceRichiedente" runat="server" CssClass="txt_addressBookLeft"
                                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                        AutoCompleteType="Disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                                <div class="colHalf2">
                                                    <div class="colHalf3">
                                                        <cc1:CustomTextArea ID="txtDescrizioneRichiedente" runat="server" CssClass="txt_projectRight"
                                                            CssClassReadOnly="txt_ProjectRight_disabled">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <uc1:AutoCompleteExtender runat="server" ID="RapidRichiedente" TargetControlID="txtDescrizioneRichiedente"
                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                    MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                    UseContextKey="true" OnClientItemSelected="richiedenteSelected" BehaviorID="AutoCompleteExIngressoRichiedente"
                                                    OnClientPopulated="richiedentePopulated ">
                                                </uc1:AutoCompleteExtender>
                                            </div>
                                            <div class="row">
                                                <div class="col-right">
                                                    <asp:CheckBox ID="chkRichiedenteExtendHistoricized" runat="server" Checked="true"
                                                        AutoPostBack="true" OnCheckedChanged="chkRichiedenteExtendHistoricized_Click" />
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <!-- DATA RICHIESTA -->
                                    <asp:UpdatePanel runat="server" ID="UpdateRequestDatePanel" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="lit_dtaRequest" runat="server" /></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="dtaRequest_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="row">
                                        <%-- ************** PROTOCOLLO RICHIESTA ******************** --%>
                                        <asp:UpdatePanel ID="UpdPnlProtoRequest" runat="server" UpdateMode="Conditional"
                                            ClientIDMode="static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="litProtoRequest" runat="server"></asp:Literal></span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="InstanceAccessCercaProto" ImageUrl="../Images/Icons/search_response_documents.png"
                                                            OnMouseOutImage="../Images/Icons/search_response_documents.png" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                            OnClick="InstanceAccessCercaProto_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="LinkDocFascBtn_Reset" ImageUrl="../Images/Icons/delete.png"
                                                            OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                            EnableViewState="true" OnClick="InstanceAccessResetProtoRequest_Click" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-marginSx-full">
                                                        <div class="full_width">
                                                            <cc1:CustomTextArea ID="TxtProtoRequest" Width="99%" runat="server" class="txt_input_full"
                                                                CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </div>
                                                <asp:HiddenField ID="idProtoRequest" runat="server" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="row">
                                        <%-- ************** NOTE ******************** --%>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="NoteInstance" runat="server"></asp:Literal></span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-marginSx-full">
                                                <div class="full_width">
                                                    <asp:UpdatePanel ID="UpdPnlNote" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                                        <ContentTemplate>
                                                            <cc1:CustomTextArea ID="TxtNote" Width="99%" runat="server" class="txt_input_full"
                                                                CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDxAccess">
                        <div id="contentDxTopProject">
                            <%--Azioni massive --%>
                            <%--Label numero di elementi --%>
                            <asp:UpdatePanel runat="server" ID="UpnlNumeroDocumenti" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col">
                                            <div class="p-padding-left">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label runat="server" ID="lblElencoDocumenti"></asp:Label></span>
                                                    <asp:Label runat="server" ID="lblNumeroDocumenti"></asp:Label>
                                                    <asp:Label runat="server" ID="lblNumeroTotaleFile"></asp:Label></p>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" ID="UpPanelFrame" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <iframe id="frame" style=" height:0; width:0; border:0" runat="server" clientidmode="Static"></iframe>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel runat="server" ID="UpnlGridInstanceAccessDocuments" UpdateMode="Conditional"
                                        class="UpnlGrid" EnableViewState="true">
                                        <ContentTemplate>
                                            <asp:GridView ID="gridInstanceAccessDocuments" runat="server" AllowSorting="false"
                                            AllowPaging="false" AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                            HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true" OnPageIndexChanging="GridInstanceAccessDocuments_PageIndexChanging"
                                            OnPreRender="gridInstanceAccessDocuments_PreRender" OnRowDataBound="GridInstanceAccessDocuments_RowDataBound" 
                                            OnRowCommand="GridInstanceAccessDocuments_RowCommand" >
                                                <PagerStyle CssClass="recordNavigator2" />
                                                <Columns>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblIdInstanceDocumentId" Text='<%# Bind("ID_INSTANCE_ACCESS_DOCUMENT") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblIdInstanceDocnumber" Text='<%# Bind("INFO_DOCUMENT.DOCNUMBER") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <div align="center">
                                                                <asp:CheckBox ID="ChkSelectedAllDocuments" CssClass="clickableLeftN" runat="server"
                                                                    onclick="cb_selectall();" />
                                                            </div>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <asp:CheckBox ID="ChkSelectedDocument" CssClass="clickableLeftN" runat="server" />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentDocnumber%>'>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <span class="noLink"><asp:LinkButton  ID="lblIdInstanceDocumentDocnumber" runat="server" Text='<%#this.GetLabelDoc((NttDataWA.DocsPaWR.InstanceAccessDocument) Container.DataItem) %>' CommandName="viewLinkDocument"></asp:LinkButton></span>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentRep%>'>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <asp:Label runat="server" ID="lblProtoRep" Text='<%# Bind("INFO_DOCUMENT.COUNTER_REPERTORY") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentRegister%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblRegister" Text='<%#this.GetLabelRegister((NttDataWA.DocsPaWR.InstanceAccessDocument) Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                    <%--<asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentIdDocPrincipale%>'>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <asp:Label runat="server" ID="lblIdDocPrincipale" Text='<%# Bind("INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentTypeProto%>'>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <asp:Label runat="server" ID="lblTypeProto" Text='<%#this.GetLabelTypeProto((NttDataWA.DocsPaWR.InstanceAccessDocument) Container.DataItem) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentObject%>'>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <asp:Label runat="server" ID="lblObject" Text='<%# Bind("INFO_DOCUMENT.OBJECT") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentMittDest%>'>
                                                        <ItemTemplate>
                                                            <div align="center">
                                                                <asp:Label runat="server" ID="lblMittDest" Text='<%# Bind("INFO_DOCUMENT.MITT_DEST") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
<%--                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentHash%>'>
                                                        <ItemTemplate>
                                                            <div id="instanceHash" class="instanceHash" runat="server" style="width: 100px">
                                                                <asp:Label runat="server" ID="lblHash" Text='<%# Bind("INFO_DOCUMENT.HASH") %>' ToolTip='<%# Bind("INFO_DOCUMENT.HASH") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                    <%--<asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentFileName%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblFileName" Text='<%# Bind("INFO_DOCUMENT.FILE_NAME") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentTotalNumberAttachment%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblTotalNumberAttachment" Text='<%#this.GetLabelTotalNumberAttachment((NttDataWA.DocsPaWR.InstanceAccessDocument) Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentClass%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblClass" Text='<%# Bind("INFO_PROJECT.CODE_CLASSIFICATION") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentCodeProject%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblCodeProject" Text='<%# Bind("INFO_PROJECT.CODE_PROJECT") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceDocumentDescProject%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblDescProject" Text='<%# Bind("INFO_PROJECT.DESCRIPTION_PROJECT") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText='<%$ localizeByText:InstanceDocumenRequest%>'>
                                                        <ItemTemplate>
                                                            <div class="row">
                                                                <div class="col-full">
                                                                    <p>
                                                                        <asp:DropDownList ID="DdlRequest" runat="server" CssClass="chzn-select-deselect"
                                                                            AutoPostBack="true" OnSelectedIndexChanged="DdlRequest_SelectedIndexChanged">
                                                                            <asp:ListItem Value="0" Text='<%$ localizeByText:InstanceDocumenRequestCopiaSemp%>'></asp:ListItem>
                                                                            <asp:ListItem Value="1" Text='<%$ localizeByText:InstanceDocumenRequestCopiaConf%>'></asp:ListItem>
                                                                            <asp:ListItem Value="2" Text='<%$ localizeByText:InstanceDocumenRequestEstr%>'></asp:ListItem>
                                                                            <asp:ListItem Value="3" Text='<%$ localizeByText:InstanceDocumenRequestDup%>'></asp:ListItem>                                             
                                                                        </asp:DropDownList>
                                                                    </p>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <cc1:CustomImageButton ID="ImgViewDocument" CommandName="ViewDocument"
                                                                runat="server" ImageUrl="../Images/Icons/ico_previous_details.png" OnMouseOutImage="../Images/Icons/ico_previous_details.png"
                                                                OnMouseOverImage="../Images/Icons/ico_previous_details_hover.png" CssClass="clickableLeft"
                                                                ImageUrlDisabled="../Images/Icons/ico_previous_details_disabled.png"
                                                                ToolTip='<%$ localizeByText:InstanceDocumenViewDetailsDocTooltip%>' />
                                                            <cc1:CustomImageButton ID="btnTypeDoc" CommandName="ViewDocumentFile" runat="server"
                                                                CssClass="clickableLeft" ToolTip='<%$ localizeByText:IndexBtnTypeDocTooltip%>' Visible="false"/>
                                                            <cc1:CustomImageButton ID="btnSignatureDetails" CommandName="SignatureDetails" runat="server" ImageUrl="../Images/Icons/icon_p7m.png"
                                                                OnMouseOutImage="../Images/Icons/icon_p7m.png" OnMouseOverImage="../Images/Icons/icon_p7m_hover.png"
                                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/icon_p7m_disabled.png"
                                                                ToolTip='<%$ localizeByText:DocumentSignatureDetails%>' Visible="false" />
                                                            <cc1:CustomImageButton ID="IndexImgDetailsDocument" CommandName="ViewDetailsDocument"
                                                                runat="server" ImageUrl="../Images/Icons/ico_view_detail_instance.png" OnMouseOutImage="../Images/Icons/ico_view_detail_instance.png"
                                                                OnMouseOverImage="../Images/Icons/ico_view_detail_instance_hover.png" CssClass="clickableLeft"
                                                                ImageUrlDisabled="../Images/Icons/ico_view_detail_instance_disabled.png"
                                                                ToolTip='<%$ localizeByText:IndexImgDetailsDocumentTooltip%>' />
                                                            <cc1:CustomImageButton ID="ImgRemoveNotify" CommandName="RemoveDocument" runat="server"
                                                                ImageUrl="../Images/Icons/delete3.png" OnMouseOutImage="../Images/Icons/delete3.png"
                                                                ToolTip='<%$ localizeByText:InstanceDocumenRemoveDocTooltip%>' OnMouseOverImage="../Images/Icons/delete3_hover.png"
                                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/delete3_disabled.png" />
                                                                <asp:Image ID="imgStateInstanceAccessDocument"  runat="server"  HeaderStyle-HorizontalAlign="Center" CssClass="clickableLeft" />
                                                                <asp:Image ID="stateInstanceAccessDocument"  runat="server"  HeaderStyle-HorizontalAlign="Center" CssClass="clickableLeft"  Visible="false"/>
                                                                <%--<div id="stateInstanceAccessDocument" runat="server"></div> --%>                   
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
                                            <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="HiddenRemoveDocuments" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="HiddenConfirmCreateDeclaration" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="HiddenRemoveDichiarazione" runat="server" ClientIDMode="Static" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- end of container -->
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="InstanceDetailsSave" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceDetailsSave_Click" />
            <cc1:CustomButton ID="InstanceDetailsCreate" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceDetailsCreate_Click" />
            <cc1:CustomButton ID="InstanceDownload" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceDownload_Click" />
            <cc1:CustomButton ID="InstancePrepareDownloadUpdate" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstancePrepareDownloadUpdate_Click" />
            <cc1:CustomButton ID="InstancePrepareDownload" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstancePrepareDownload_Click" />
            <cc1:CustomButton ID="InstanceForward" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceForward_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnAddDocPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddDocPostback_Click" />
            <asp:Button ID="btnCbSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="ChkSelectedAllDocuments_CheckedChanged" />
            <asp:HiddenField ID="HiddenRemoveUsedSearch" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsAll" runat="server" ClientIDMode="Static" />
<%--<asp:Button ID="btnGridInstanceDetailsRow" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnGridInstanceDetailsRow_Click" />--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
