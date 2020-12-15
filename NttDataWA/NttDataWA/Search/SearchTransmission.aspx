<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchTransmission.aspx.cs"
    Inherits="NttDataWA.Search.SearchTransmission" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
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

        function creatorePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoCreatore');
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

        function creatoreSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneCreatore.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneCreatore.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneCreatore.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceCreatore.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneCreatore.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceCreatore.ClientID%>', '');
        }

        function aceSelectedRole(sender, e) {
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

            var searchText = $get('<%=TxtDescriptionRole.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionRole.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionRole.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeRole.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionRole.ClientID%>").value = descrizione;

            __doPostBack('<%=this.TxtCodeRole.ClientID%>', '');
        }


        function acePopulatedRole(sender, e) {
            var behavior = $find('AutoCompletRole');
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

        function enableField(id) {
            var prePracticeCodeTextBox = $('#<%=PlcWithImage.ClientID%>');
            var element = $('#M_si_img')[0];

            if (element.checked) {
                prePracticeCodeTextBox.show();
            }
            else {
                prePracticeCodeTextBox.hide();
            }

            swapFileCheckboxs(id);
        }

        function swapFileCheckboxs(id) {
            if ($get('<%=P_Prot.ClientID%>') && $get('<%=M_Fasc.ClientID%>') && $get('<%=M_si_img.ClientID%>') && $get('<%=M_Img.ClientID%>')) {
                if (id == '<%=P_Prot.ClientID%>') {
                    $get('<%=M_Fasc.ClientID%>').checked = false;
                    $get('<%=M_si_img.ClientID%>').checked = false;
                    $get('<%=M_Img.ClientID%>').checked = false;
                    var prePracticeCodeTextBox = $('#<%=PlcWithImage.ClientID%>');
                    prePracticeCodeTextBox.hide();

                }
                else if (id == '<%=M_Fasc.ClientID%>') {
                    $get('<%=P_Prot.ClientID%>').checked = false;
                    $get('<%=M_si_img.ClientID%>').checked = false;
                    $get('<%=M_Img.ClientID%>').checked = false;
                    var prePracticeCodeTextBox = $('#<%=PlcWithImage.ClientID%>');
                    prePracticeCodeTextBox.hide();
                }
                else if (id == '<%=M_si_img.ClientID%>') {
                    $get('<%=P_Prot.ClientID%>').checked = false;
                    $get('<%=M_Fasc.ClientID%>').checked = false;
                    $get('<%=M_Img.ClientID%>').checked = false;

                }
                else if (id == '<%=M_Img.ClientID%>') {
                    $get('<%=P_Prot.ClientID%>').checked = false;
                    $get('<%=M_Fasc.ClientID%>').checked = false;
                    $get('<%=M_si_img.ClientID%>').checked = false;
                    var prePracticeCodeTextBox = $('#<%=PlcWithImage.ClientID%>');
                    prePracticeCodeTextBox.hide();
                }
            }
        }

        function swapSignedCheckboxs(id) {
            if ($get('<%=chk_firmati.ClientID%>') && $get('<%=chk_non_firmati.ClientID%>')) {
                if (id == '<%=chk_firmati.ClientID%>' && $get('<%=chk_firmati.ClientID%>').checked) {
                    $get('<%=chk_non_firmati.ClientID%>').checked = false;
                }
                else if (id == '<%=chk_non_firmati.ClientID%>' && $get('<%=chk_non_firmati.ClientID%>').checked) {
                    $get('<%=chk_firmati.ClientID%>').checked = false;
                }
            }
        }

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
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="SaveSearch" runat="server" Url="../Popup/SaveSearch.aspx" Width="650"
        Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ModifySearch" runat="server" Url="../Popup/SaveSearch.aspx?modify=true"
        Width="650" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ViewDetailTransmission" runat="server" Url="../Popup/ViewDetailTransmission.aspx"
        PermitClose="false" Width="1100" Height="1000" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closePopupViewDetailTransmission');}" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=trasm"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closePopupOpenTitolario'); }" />
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../Popup/SearchProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'closePopupSearchProject');}" />
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
                                        <asp:Literal ID="LitSearchTransmissions" runat="server"></asp:Literal></p>
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
                        <asp:UpdatePanel runat="server" ID="UpSearchDocumentTabs" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="containerDocumentTabOrangeSx">
                                    <ul>
                                        <li id="LiTransmLiReceived" runat="server" class="searchIAmSearch">
                                            <asp:LinkButton ID="TransmLinkReceived" runat="server" OnClick="TransmLinkReceived_Click"></asp:LinkButton></li>
                                        <li id="LiTransmLiEffettuated" runat="server" class="searchOther">
                                            <asp:LinkButton ID="TransmLinkEffettuated" runat="server" OnClick="TransmLinkEffettuated_Click"></asp:LinkButton></li>
                                    </ul>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="colMassiveOperationSx">
                                            <cc1:CustomImageButton runat="server" ID="SearchTransmissionExportExcel" ImageUrl="~/Images/Icons/massive_export.png"
                                                ImageUrlDisabled="~/Images/Icons/massive_export_disabled.png" OnMouseOverImage="../Images/Icons/massive_export_hover.png"
                                                OnMouseOutImage="../Images/Icons/massive_export.png" CssClass="clickableLeft"
                                                OnClientClick="return ajaxModalPopupExportDati();" />
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
                            <div class="row">
                                <!-- filters -->
                                <fieldset class="basic">
                                    <!-- ricerche salvate -->
                                    <asp:UpdatePanel ID="UpPnlRapidSearch" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder ID="plcSavedSearches" runat="server">
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="SearchDocumentLitRapidSearch" runat="server" /></span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <asp:DropDownList runat="server" ID="DdlRapidSearch" CssClass="chzn-select-deselect"
                                                            Width="97%" OnSelectedIndexChanged="DdlRapidSearch_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                                <div class="row">
                                    <fieldset class="basic">
                                        <asp:UpdatePanel runat="server" ID="UpPnlType" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <h2 class="expand">
                                                        <asp:Literal runat="server" ID="SearchTransmissionRecipient"></asp:Literal>
                                                    </h2>
                                                    <div id="Div1" class="collapse shown" runat="server">
                                                        <div class="row">
                                                            <asp:PlaceHolder runat="server" ID="PlcUserFilter">
                                                                <div class="col2">
                                                                    <asp:CheckBox ID="chk_me_stesso" runat="server" />
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LitUserTransmission"></asp:Literal>
                                                                </div>
                                                            </asp:PlaceHolder>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col2">
                                                                <asp:CheckBox ID="chk_mio_ruolo" runat="server" />
                                                            </div>
                                                            <div class="col2">
                                                                <asp:Literal runat="server" ID="LitRoleTransmission"></asp:Literal>
                                                            </div>
                                                            <div class="col2">
                                                                <div class="colHalf">
                                                                    <asp:HiddenField ID="IdRole" runat="server" />
                                                                    <cc1:CustomTextArea ID="TxtCodeRole" runat="server" CssClass="txt_addressBookLeft"
                                                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                                        OnTextChanged="TxtCode_OnTextChanged">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                                <div class="colHalf2">
                                                                    <div class="colHalf3">
                                                                        <cc1:CustomTextArea ID="TxtDescriptionRole" runat="server" CssClass="txt_addressBookRight"
                                                                            CssClassReadOnly="txt_addressBookRight_disabled" AutoCompleteType="Disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                              <uc1:AutoCompleteExtender runat="server" ID="RapidRole" TargetControlID="TxtDescriptionRole"
                                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                    UseContextKey="true" OnClientItemSelected="aceSelectedRole" BehaviorID="AutoCompletRole"
                                                                    OnClientPopulated="acePopulatedRole" Enabled="false">
                                                                </uc1:AutoCompleteExtender>
                                                            </div>
                                                            <div class="col-right-no-margin22">
                                                                <cc1:CustomImageButton runat="server" ID="DocumentImgRoleAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                    OnClick="DocumentImgAddressBook_Click" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-right-no-margin">
                                                                <asp:CheckBox ID="chkHistoricizedRole" runat="server" />
                                                            </div>
                                                            <div class="col-right-no-margin">
                                                                <asp:Literal ID="LitSearchExtendStor" runat="server"></asp:Literal>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col2">
                                                                <p>
                                                                    <asp:Literal ID="LitSearchNotifyUser" runat="server"></asp:Literal>
                                                                </p>
                                                            </div>
                                                            <div class="col">
                                                                <p>
                                                                    <asp:DropDownList runat="server" ID="select_sottoposto" DataTextField="FULL_NAME"
                                                                        DataValueField="SYSTEM_ID"  Width="270" AppendDataBoundItems="true"
                                                                        EnableViewState="true">
                                                                    </asp:DropDownList>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col2">
                                                                <p>
                                                                    <asp:CheckBox ID="chk_visSott" runat="server" />
                                                                </p>
                                                            </div>
                                                            <div class="col2">
                                                                <p>
                                                                    <asp:Literal ID="LitSearchTransUnder" runat="server"></asp:Literal>
                                                                </p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic">
                                        <asp:UpdatePanel runat="server" ID="UpPnlObject" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LitSearchTransObj"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList runat="server" ID="ddl_oggetto"
                                                            Width="150" OnSelectedIndexChanged="ddl_oggetto_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="D"></asp:ListItem>
                                                            <asp:ListItem Value="F"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col">
                                                        <asp:DropDownList runat="server" ID="ddl_tipo_doc" 
                                                            Width="200">
                                                            <asp:ListItem Value="Tutti"></asp:ListItem>
                                                            <asp:ListItem Value="P"></asp:ListItem>
                                                            <asp:ListItem Value="PA" id="opArr" runat="server"></asp:ListItem>
                                                            <asp:ListItem Value="PP" id="opPart" runat="server"></asp:ListItem>
                                                            <asp:ListItem Value="PI" id="opInt" runat="server"></asp:ListItem>
                                                            <asp:ListItem Value="NP">Non Protocollato</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <%-- ************** TIPOLOGIA ******************** --%>
                                <div class="row">
                                    <fieldset class="azure">
                                        <asp:UpdatePanel ID="UpTotalTypeDocument" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <span class="weight">
                                                    <asp:Literal ID="SearchDocumentLitTypology" runat="server" />
                                                </span>
                                                <asp:UpdatePanel ID="UpPnlDocType" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <div class="row">
                                                                    <div class="col-full">
                                                                        <div class="styled-select_full">
                                                                            <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" OnSelectedIndexChanged="DocumentDdlTypeDocument_OnSelectedIndexChanged"
                                                                                AutoPostBack="True" CssClass="chzn-select-deselect" Width="97%">
                                                                                <asp:ListItem Text=""></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                                    <div class="row">
                                                                        <div class="col-full">
                                                                            <asp:DropDownList ID="ddlStateCondition" runat="server" 
                                                                                Width="100%">
                                                                                <asp:ListItem id="opt_StateConditionEquals" Value="Equals" />
                                                                                <asp:ListItem id="opt_StateConditionUnequals" Value="Unequals" />
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-full">
                                                                            <div class="styled-select_full">
                                                                                <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" 
                                                                                    Width="100%" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <asp:PlaceHolder runat="server" ID="PnlDocumentStateDiagramDate" Visible="false">
                                                                        <div class="row">
                                                                            <div class="col">
                                                                                <p>
                                                                                    <span class="weight">
                                                                                        <asp:Literal ID="DocumentDateStateDiagram" runat="server"></asp:Literal>
                                                                                    </span>
                                                                                </p>
                                                                            </div>
                                                                        </div>
                                                                    </asp:PlaceHolder>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="PnlTypeDocument" runat="server"></asp:PlaceHolder>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <asp:UpdatePanel runat="server" ID="UpPnlDocInWorking" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder runat="server" ID="PlcDocInWorking">
                                                <fieldset class="basic">
                                                    <div class="row">
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="LiSearchTransDocInComp"></asp:Literal>
                                                        </span>
                                                        <div class="row">
                                                            <div class="full_width">
                                                                <div class="col">
                                                                    <asp:CheckBox ID="P_Prot" runat="server" onclick="swapFileCheckboxs(this.id);" ClientIDMode="Static">
                                                                    </asp:CheckBox>
                                                                </div>
                                                                <div class="col">
                                                                    <asp:CheckBox ID="M_Fasc" runat="server" onclick="swapFileCheckboxs(this.id);" ClientIDMode="Static">
                                                                    </asp:CheckBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="full_width">
                                                                <div class="col">
                                                                    <asp:CheckBox ID="M_si_img" runat="server" ClientIDMode="static"></asp:CheckBox>
                                                                </div>
                                                                <div class="col">
                                                                    <asp:CheckBox ID="M_Img" runat="server" onclick="swapFileCheckboxs(this.id);" ClientIDMode="Static">
                                                                    </asp:CheckBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <asp:Panel runat="server" ID="PlcWithImage" ClientIDMode="Static" CssClass="hidden">
                                                        <div class="row">
                                                            <div class="col">
                                                                <asp:CheckBox ID="chk_firmati" runat="server" ClientIDMode="Static" onclick="swapSignedCheckboxs(this.id);">
                                                                </asp:CheckBox>
                                                            </div>
                                                            <div class="col">
                                                                <asp:CheckBox ID="chk_non_firmati" runat="server" ClientIDMode="Static" onclick="swapSignedCheckboxs(this.id);">
                                                                </asp:CheckBox>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <asp:Literal runat="server" ID="LitSearchTransTypeFileAcq"></asp:Literal>
                                                            </div>
                                                            <div class="col">
                                                                <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" 
                                                                    Width="200">
                                                                    <asp:ListItem></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>
                                                </fieldset>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div class="row">
                                    <fieldset class="basic">
                                        <%-- MITTENRE/DESTINATARIO --%>
                                        <asp:UpdatePanel ID="upPnlCreatore" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LiSearchTransSender"></asp:Literal>
                                                    </span>
                                                    <div class="row">
                                                        <div class="colHalf">
                                                            &nbsp;</div>
                                                        <div class="col">
                                                            <asp:RadioButtonList ID="rblOwnerType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                                                                <asp:ListItem id="optUO" runat="server" Value="U" />
                                                                <asp:ListItem id="optRole" runat="server" Value="R" Selected="True" />
                                                                <asp:ListItem id="optUser" runat="server" Value="P" />
                                                            </asp:RadioButtonList>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton runat="server" ID="ImgCreatoreAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnClick="ImgCreatoreAddressBook_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:HiddenField ID="idCreatore" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="txtCodiceCreatore" runat="server" CssClass="txt_addressBookLeft"
                                                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                AutoCompleteType="Disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="txtDescrizioneCreatore" runat="server" CssClass="txt_projectRight"
                                                                    CssClassReadOnly="txt_ProjectRight_disabled">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                         <uc1:AutoCompleteExtender runat="server" ID="RapidCreatore" TargetControlID="txtDescrizioneCreatore"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="creatoreSelected" BehaviorID="AutoCompleteExIngressoCreatore"
                                                            OnClientPopulated="creatorePopulated" Enabled="false">
                                                        </uc1:AutoCompleteExtender>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-right">
                                                            <asp:CheckBox ID="chkCreatoreExtendHistoricized" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic">
                                        <asp:UpdatePanel ID="UpPnlDateTransmission" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LitSearchTransDate"></asp:Literal>
                                                    </span>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_dataTrasm" runat="server" AutoPostBack="true" Width="140px"
                                                               OnSelectedIndexChanged="ddl_dataTrasm_SelectedIndexChanged">
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
                                                            <asp:Literal runat="server" ID="lbl_initdataTrasm"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_initDataTrasm" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="lbl_finedataTrasm"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_fineDataTrasm" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled" Visible="false"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic">
                                        <asp:UpdatePanel ID="UpPnlTransmReason" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LitSearchTransReason"></asp:Literal>
                                                    </span>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <asp:DropDownList ID="ddl_ragioni" runat="server" 
                                                                Width="100%">
                                                                <asp:ListItem Value=""></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic">
                                        <asp:UpdatePanel ID="UpPnlOtherFilters" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LitSearchTransOtherFilters"></asp:Literal>
                                                    </span>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:CheckBox ID="cbx_Acc" runat="server"  AutoPostBack="true" OnCheckedChanged="swapOtherFiltersCheckboxs" />
                                                        </div>
                                                        <div class="col">
                                                            <asp:CheckBox ID="cbx_Rif" runat="server" AutoPostBack="true" OnCheckedChanged="swapOtherFiltersCheckboxs" />
                                                        </div>
                                                        <div class="col">
                                                            <asp:CheckBox ID="cbx_Pendenti" runat="server" AutoPostBack="true" OnCheckedChanged="swapOtherFiltersCheckboxs" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:Panel runat="server" ClientIDMode="Static" ID="PnlDateOthers">
                                                            <div class="row">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_TAR" runat="server" AutoPostBack="true" Width="140px" 
                                                                        OnSelectedIndexChanged="ddl_TAR_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LiSearchData_1"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="dataUno_TAR" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LiSearchData_2"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="dataDue_TAR" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled" Visible="false"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                    <asp:Panel ID="pnlNoLavorate" runat="server" Visible="false">
                                                        <div class="row">
                                                            <div class="col">
                                                                <asp:CheckBox ID="cbx_no_lavorate" runat="server" />
                                                            </div>
                                                        </div>
                                                    </asp:Panel>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LinSearchTransGeneralNote"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-marginSx-full">
                                                            <div class="full_width">
                                                                <cc1:CustomTextArea ID="txt_note_generali" Width="99%" runat="server" class="txt_input_full"
                                                                    CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LinSearchTransIndividualNote"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-marginSx-full">
                                                            <div class="full_width">
                                                                <cc1:CustomTextArea ID="txt_note_individuali" Width="99%" runat="server" class="txt_input_full"
                                                                    CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LitSearchTransEnd"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LitSearchEndFrom"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="cld_scadenza_dal" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LitSearchEndTo"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="cld_scadenza_al" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LitSearchOrderTrans"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:DropDownList ID="ddlOrder" runat="server" Width="200" />
                                                        </div>
                                                        <div class="col">
                                                            <asp:DropDownList ID="ddlOrderDirection" runat="server" 
                                                                Width="130">
                                                                <asp:ListItem Text="Crescente" Value="ASC" />
                                                                <asp:ListItem Text="Decrescente" Value="DESC" />
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                            </div>
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
                                            <asp:GridView ID="gridViewResult" runat="server" AllowSorting="false" AllowPaging="false"
                                                AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                                PageSize='<%# this.GetPageSize() %>' OnPreRender="gridViewResult_PreRender"
                                                OnRowCreated="gridViewResult_ItemCreated">
                                                <Columns>
                                                    <asp:TemplateField Visible="False" HeaderText="Chiave">
                                                        <ItemTemplate>
                                                            <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetSystemID((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
									                <asp:TemplateField visible="false">
										                <ItemTemplate>
                                                            <asp:CheckBox ID="cbSel" runat="server" CssClass="clickable" />
										                </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
									                </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label7" runat="server" Text='<%# this.GetDataTrasm((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label18" runat="server" Text='<%# this.GetSenderUser((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                            <br />
                                                            (
                                                            <asp:Label ID="Label17" runat="server" Text='<%# this.GetRoleUser((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>)
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Ragione">
                                                        <HeaderStyle></HeaderStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label10" runat="server" Text='<%# this.GetReasonTrans((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label102" runat="server" Text='<%# this.GetRecipientsTrans((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label11" runat="server" Text='<%# this.GetDataScadenzaTrans((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label14" runat="server" Text='<%# this.GetIdDocument((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label12" runat="server" Text='<%# this.GetInfoTrasm((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                            <asp:Label ID="lbl_righe" runat="server" Text='<%# this.ShowSeparator((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'></asp:Label>
                                                            <asp:Label ID="Label19" runat="server" Text='<%# this.GetSenderDoc((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <cc1:CustomImageButton runat="server" ID="SearchTransmissionsViewDetails" ImageUrl="../Images/Icons/massive_transmissions.png"
                                                                OnMouseOutImage="../Images/Icons/massive_transmissions.png" OnMouseOverImage="../Images/Icons/massive_transmissions_hover.png"
                                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/massive_transmissions_disabled.png"
                                                                AlternateText='<%# this.GetDescriptionImgTrans() %>' ToolTip='<%# this.GetDescriptionImgTrans() %>'
                                                                OnClick="SearchTransmissionsViewDetails_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="visualizzadocumento" ImageUrl='<%# this.GetimgObject((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'
                                                                OnMouseOutImage='<%# this.GetimgObject((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'
                                                                OnMouseOverImage='<%# this.GetimgObjectHover((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'
                                                                CssClass="clickableLeft" ImageUrlDisabled='<%# this.GetimgObjectDisabled((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'
                                                                AlternateText='<%# this.GetDescriptionImg((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'
                                                                ToolTip='<%# this.GetDescriptionImg((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>'
                                                                OnClick="ViewDocument_Click" Enabled='<%# this.GetEnableImgViewDocument((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="False">
                                                        <ItemTemplate>
                                                            <asp:Label ID="ID_OBJECT" runat="server" Text='<%# this.GetIdObject((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="true" HeaderText='<%$ localizeByText:SearchTrasmLblRep%>'>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LabelContatore" runat="server" Text='<%# this.GetRepertorio((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="False">
                                                        <ItemTemplate>
                                                            <asp:Label ID="TYPE_OBJECT" runat="server" Text='<%# this.GetTypeObject((NttDataWA.DocsPaWR.Trasmissione)Container.DataItem) %>' />
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="SearchTransmissionSearch" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchTransmissionSearch_Click" />
            <cc1:CustomButton ID="SearchTransmissionSave" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchTransmissionSave_Click" />
            <cc1:CustomButton ID="SearchDocumentAdvancedRemove" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" OnClick="SearchTransmissionAdvancedRemove_Click"
                ClientIDMode="Static" />
            <cc1:CustomButton ID="SearchTransmissionRemoveFilters" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchTransmissionRemoveFilters_Click" />
            <cc1:CustomButton ID="BtnUrgesAll" runat="server" CssClass="btnEnable" Visible="false"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnUrgesAll_Click" />
            <cc1:CustomButton ID="BtnUrgesSelected" runat="server" CssClass="btnEnable" Visible="false"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnUrgesSelected_Click" />

            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:HiddenField ID="HiddenRemoveUsedSearch" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
