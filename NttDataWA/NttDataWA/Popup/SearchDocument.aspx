<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchDocument.aspx.cs"
    MasterPageFile="~/MasterPages/Popup.Master" Inherits="NttDataWA.Popup.SearchDocument" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .col_search
        {
            float: left;
            margin-left: 3px;
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
        .gridViewResult
        {
            min-width: 99%;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#lnkAdvFilters').click(function () {
                var active = $('#<%= advFilters.ClientID %>').val();
                $('#pnlAdvFilters').toggle();
                $('#<%= advFilters.ClientID %>').val((active == "false") ? "true" : "false");
                var active2 = $('#<%= advFilters.ClientID %>').val();
                return false;
            });
        });
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




        function proprietarioPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoProprietario');
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

        function proprietarioSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneProprietario.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneProprietario.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneProprietario.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceProprietario.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneProprietario.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceProprietario.ClientID%>', '');
        }

        function recipientPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoRecipient');
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

        function recipientSelected(sender, e) {
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

            var searchText = $get('<%=txt_descrMit_E.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_descrMit_E.ClientID%>").focus();
            document.getElementById("<%=this.txt_descrMit_E.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_codMit_E.ClientID%>").value = codice;
            document.getElementById("<%=txt_descrMit_E.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txt_codMit_E.ClientID%>', '');
        }
        function resizeDiv() {
            var height = document.documentElement.clientHeight;
            height -= 170; /* whatever you set your body bottom margin/padding to be */
            document.getElementById('centerContentAddressbookSx').style.height = height + "px";
            document.getElementById('centerContentAddressbookDx').style.height = height + "px";
        }

        function acePopulatedCod(sender, e) {
            var behavior = $find('AutoCompleteDescriptionProject');
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

        function aceSelectedDescr(sender, e) {

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

            var searchText = $get('<%=txt_DescFascicolo.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_DescFascicolo.ClientID%>").focus();
            document.getElementById("<%=this.txt_DescFascicolo.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_CodFascicolo.ClientID%>").value = codice;
            document.getElementById("<%=txt_DescFascicolo.ClientID%>").value = descrizione;
        }

        function swapConservationCheckboxs(id) {
            if ($get('<%=cb_Conservato.ClientID %>') && $get('<%=cb_NonConservato.ClientID %>')) {
                if (id == '<%=cb_Conservato.ClientID %>' && $get('<%=cb_Conservato.ClientID %>').checked) {
                    $get('<%=cb_NonConservato.ClientID %>').checked = false;
                }
                else if (id == '<%=cb_NonConservato.ClientID %>' && $get('<%=cb_NonConservato.ClientID %>').checked) {
                    $get('<%=cb_Conservato.ClientID %>').checked = false;
                }
            }
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

        function enableField() {
            var prePracticeCodeTextBox = $('#<%=PnlAtt.ClientID%>');
            var element = $('#opAll').find('input').get(0);

            if (element.checked == true) {
                prePracticeCodeTextBox.show();
            }
            else {
                prePracticeCodeTextBox.hide();
            }
        }

    </script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx?rt=serachDocPopup"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { $('#btnAddressBookPostback').click(); }" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx?rt=serachDocPopup"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="800" Height="1000"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx?popup=SearchDoc"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../popup/SearchProject.aspx?fromf=Instance&caller=profilo"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <div class="container">
        <div id="contentAddressBook">
            <div id="topContentPopupSearch">
                <asp:UpdatePanel ID="UpTypeResult" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <ul>
                            <li class="addressTab" id="liAddressBookLinkList" runat="server">
                                <asp:LinkButton runat="server" ID="AddressBookLinkList">Ricerca</asp:LinkButton></li>
                            <li id="lbl_countRecord" class="blue" runat="server">
                                <asp:Literal ID="LitPopupSearchDocumentFound" runat="server"></asp:Literal>
                                <asp:Literal ID="DocumentCount" runat="server" Text="0" />
                                <asp:Literal ID="LitPopupSearchDocumentFound2" runat="server"></asp:Literal></li>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="centerContentAddressbook">
                <div id="contentTab">
                    <div id="centerContentAddressbookSx" style="overflow: auto;">
                        <div style="width: 98%;">
                            <asp:UpdatePanel ID="UplnRadioButton" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row nowrap">
                                        <asp:UpdatePanel runat="server" ID="UpPnlType" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-marginSx2">
                                                        <p>
                                                            <span class="weight" style="color: #333333;">
                                                                <asp:Literal runat="server" ID="SearchDocumentTypeDocument"></asp:Literal></span></p>
                                                    </div>
                                                    <div class="col">
                                                        <asp:CheckBoxList ID="cbl_archDoc_E" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="A" Selected="True" runat="server" id="opArr"></asp:ListItem>
                                                            <asp:ListItem Value="P" Selected="True" runat="server" id="opPart"></asp:ListItem>
                                                            <asp:ListItem Value="I" Selected="True" runat="server" id="opInt"></asp:ListItem>
                                                            <asp:ListItem Value="G" Selected="True" runat="server" id="opGrigio"></asp:ListItem>
                                                            <asp:ListItem Value="Pr" Selected="False" id="opPredisposed" runat="server"></asp:ListItem>
                                                            <asp:ListItem Value="ALL" Selected="False" runat="server" id="opAll"></asp:ListItem>
                                                            <asp:ListItem Value="R" Selected="False" id="opPrints" runat="server"></asp:ListItem>
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                                <asp:Panel runat="server" ID="PnlAtt" ClientIDMode="Static" CssClass="hidden">
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:RadioButtonList ID="rblFiltriAllegati" runat="server" CssClass="testo_grigio"
                                                                RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="tutti" Text="Tutti" />
                                                                <asp:ListItem Value="pec" Text="PEC" />
                                                                <asp:ListItem Value="user" Text="Utente" Selected="True" />
                                                                <asp:ListItem Value="esterni" Text="Sist. esterni" />
                                                                <asp:ListItem Value="SIMPLIFIEDINTEROPERABILITY" runat="server" id="rbOpIS" />
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <!--filtri-->
                            <asp:UpdatePanel ID="UplnFiltri" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="plh_filtri" runat="server">
                                        <%-- ******************** PROTOCOLLO ******************** --%>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <h2 class="expand">
                                                        <asp:Literal runat="server" ID="SearchProtocolloLit"></asp:Literal>
                                                    </h2>
                                                    <div id="Div2" class="collapse shown" runat="server">
                                                        <asp:UpdatePanel runat="server" ID="UpProtocollo" UpdateMode="Conditional" ClientIDMode="static">
                                                            <ContentTemplate>
                                                                <%-- NUMERO PROTOCOLLO --%>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlNumProto"></asp:Literal>
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col2">
                                                                        <asp:DropDownList ID="ddl_numProt_E" runat="server" AutoPostBack="True" Width="140px"
                                                                            OnSelectedIndexChanged="ddl_numProt_E_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                            <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlDaNumProto"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_initNumProt_E" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlANumProto"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_fineNumProt_E" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                                <%-- DATA PROTOCOLLO --%>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <p>
                                                                            <span class="weight" style="color: #333333;">
                                                                                <asp:Literal runat="server" ID="LtlDataProto"></asp:Literal>
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col2">
                                                                        <asp:DropDownList ID="ddl_dataProt_E" runat="server" AutoPostBack="true" Width="140px"
                                                                            OnSelectedIndexChanged="ddl_dataProt_E_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                            <asp:ListItem Value="0"></asp:ListItem>
                                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                                            <asp:ListItem Value="2"></asp:ListItem>
                                                                            <asp:ListItem Value="3"></asp:ListItem>
                                                                            <asp:ListItem Value="4"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlDaDataProto"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_initDataProt_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlADataProto"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_fineDataProt_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </div>
                                            </fieldset>
                                            <div class="row" style="margin-top: 5px;">
                                                <fieldset>
                                                    <h2 class="expand">
                                                        <asp:Literal runat="server" ID="SearchDocumentLit"></asp:Literal>
                                                    </h2>
                                                    <div id="Div1" class="collapse shown" runat="server">
                                                        <asp:UpdatePanel runat="server" ID="UpDoc" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <%-- ID DOCUMENTO--%>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <p>
                                                                            <span class="weight" style="color: #333333;">
                                                                                <asp:Literal runat="server" ID="LtlIdDoc"></asp:Literal>
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col2">
                                                                        <asp:DropDownList ID="ddl_idDocumento_C" runat="server" Width="140px" AutoPostBack="true"
                                                                            OnSelectedIndexChanged="ddl_idDocumento_C_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                            <asp:ListItem Value="0"></asp:ListItem>
                                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlDaIdDoc"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_initIdDoc_C" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlAIdDoc"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_fineIdDoc_C" runat="server" Width="80px" Visible="true"
                                                                            CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                                <%-- DATA CREAZIONE --%>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <p>
                                                                            <span class="weight" style="color: #333333;">
                                                                                <asp:Literal runat="server" ID="LtlDataCreazione"></asp:Literal>
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col2">
                                                                        <asp:DropDownList ID="ddl_dataCreazione_E" runat="server" AutoPostBack="true" Width="140px"
                                                                            OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                                            <asp:ListItem Value="0"></asp:ListItem>
                                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                                            <asp:ListItem Value="2"></asp:ListItem>
                                                                            <asp:ListItem Value="3"></asp:ListItem>
                                                                            <asp:ListItem Value="4"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlDaDataCreazione"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_initDataCreazione_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlADataCreazione"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_finedataCreazione_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--oggetto-->
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Label runat="server" ID="LblAddDocOgetto" Text="Oggetto" Style="color: #333333;"></asp:Label></span>
                                            </p>
                                        </div>
                                        <div class="col-right">
                                            <cc1:CustomImageButton runat="server" ID="DocumentImgObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                                                OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                                OnClientClick="return ajaxModalPopupObject();" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                            <ContentTemplate>
                                                <asp:Panel ID="PnlCodeObject" runat="server" Visible="false">
                                                    <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                                        CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true" OnTextChanged="TxtCodeObject_Click"
                                                        onchange="disallowOp('Content2');">
                                                    </cc1:CustomTextArea>
                                                </asp:Panel>
                                                <asp:Panel ID="PnlCodeObject2" runat="server">
                                                    <asp:Panel ID="PnlCodeObject3" runat="server">
                                                        <cc1:CustomTextArea ID="TxtObject" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                                            CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static" Height="40">
                                                        </cc1:CustomTextArea>
                                                    </asp:Panel>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="row2">
                                        <div class="col-right-no-margin-no-top">
                                            <span class="charactersAvailable">
                                                <asp:Literal ID="projectLitVisibleObjectChars" runat="server" />: <span id="TxtObject_chars"
                                                    runat="server" clientidmode="Static"></span></span>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="plcRegistry" runat="server">
                                        <asp:UpdatePanel ID="UpPnlRegistry" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="litRegistry" runat="server" /></strong></div>
                                                    <div class="col">
                                                        <asp:DropDownList runat="server" ID="DdlRegistries" CssClass="chzn-select-deselect"
                                                            Width="90" AutoPostBack="true" OnSelectedIndexChanged="DdlRegistries_SelectedIndexChanged"
                                                            onchange="disallowOp('Content2');">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:PlaceHolder ID="OlcOtherFilters" runat="server">
                                <asp:UpdatePanel ID="UpPnlOtherFilters" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <a href="#" id="lnkAdvFilters">
                                                        <asp:Literal runat="server" ID="LitPopupSearchProjectFilters"></asp:Literal></a></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <asp:Panel ID="pnlAdvFilters" CssClass="basic3 hidden" ClientIDMode="Static" runat="server">
                                                <asp:HiddenField ID="advFilters" Value="false" runat="server" />
                                                <%-- CREATORE --%>
                                                <asp:UpdatePanel ID="upPnlCreatore" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight" style="color: #333333;">
                                                                        <asp:Literal ID="litCreator" runat="server" />
                                                                    </span>
                                                                </p>
                                                            </div>
                                                            <div style="float: left; width: 300px; margin-top: 5px; margin-left: 22px;">
                                                                <asp:RadioButtonList ID="rblOwnerType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                                                                    <asp:ListItem id="optUO" runat="server" Value="U" />
                                                                    <asp:ListItem id="optRole" runat="server" Value="R" Selected="True" />
                                                                    <asp:ListItem id="optUser" runat="server" Value="P" />
                                                                </asp:RadioButtonList>
                                                            </div>
                                                            <div class="col-right-no-margin">
                                                                <cc1:CustomImageButton runat="server" ID="ImgCreatoreAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                    OnClick="ImgCreatoreAddressBook_Click" OnClientClick="disallowOp('Content2');" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <asp:HiddenField ID="idCreatore" runat="server" />
                                                            <div class="colHalf">
                                                                <cc1:CustomTextArea ID="txtCodiceCreatore" runat="server" CssClass="txt_addressBookLeft"
                                                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                    AutoCompleteType="Disabled" onchange="disallowOp('Content2');">
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
                                                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="creatoreSelected" BehaviorID="AutoCompleteExIngressoCreatore"
                                                                OnClientPopulated="creatorePopulated ">
                                                            </uc1:AutoCompleteExtender>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-right" style="font-size: 10px;">
                                                                <asp:CheckBox ID="chkCreatoreExtendHistoricized" runat="server" Checked="true" onchange="disallowOp('Content2');"
                                                                    OnCheckedChanged="chkCreatoreExtendHistoricized_Click" />
                                                            </div>
                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <%-- PROPRIETARIO --%>
                                                <asp:UpdatePanel ID="upPnlProprietario" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight" style="color: #333333;">
                                                                        <asp:Literal ID="litOwner" runat="server" />
                                                                    </span>
                                                                </p>
                                                            </div>
                                                            <div style="float: left; width: 300px; margin-top: 5px;">
                                                                <asp:RadioButtonList ID="rblProprietarioType" runat="server" CssClass="rblHorizontal"
                                                                    RepeatLayout="UnorderedList">
                                                                    <asp:ListItem id="optPropUO" runat="server" Value="U" />
                                                                    <asp:ListItem id="optPropRole" runat="server" Value="R" Selected="True" />
                                                                    <asp:ListItem id="optPropUser" runat="server" Value="P" />
                                                                </asp:RadioButtonList>
                                                            </div>
                                                            <div class="col-right-no-margin">
                                                                <cc1:CustomImageButton runat="server" ID="ImgProprietarioAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                    OnClick="ImgProprietarioAddressBook_Click" OnClientClick="disallowOp('Content2');" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <asp:HiddenField ID="idProprietario" runat="server" />
                                                            <div class="colHalf">
                                                                <cc1:CustomTextArea ID="txtCodiceProprietario" runat="server" CssClass="txt_addressBookLeft"
                                                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                    AutoCompleteType="Disabled" onchange="disallowOp('Content2');">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                            <div class="colHalf2">
                                                                <div class="colHalf3">
                                                                    <cc1:CustomTextArea ID="txtDescrizioneProprietario" runat="server" CssClass="txt_projectRight"
                                                                        CssClassReadOnly="txt_ProjectRight_disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                            <uc1:AutoCompleteExtender runat="server" ID="RapidProprietario" TargetControlID="txtDescrizioneProprietario"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="proprietarioSelected" BehaviorID="AutoCompleteExIngressoProprietario"
                                                                OnClientPopulated="proprietarioPopulated ">
                                                            </uc1:AutoCompleteExtender>
                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <%-- MITTENTE/DESTINATARIO --%>
                                                <asp:UpdatePanel ID="UpPnlSenderRecipient" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight" style="color: #333333;">
                                                                        <asp:Literal runat="server" ID="LtlMitDest"></asp:Literal>
                                                                    </span>
                                                                </p>
                                                            </div>
                                                            <div class="col-right-no-margin">
                                                                <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientAddressBookMittDest"
                                                                    ImageUrl="../Images/Icons/address_book.png" OnMouseOutImage="../Images/Icons/address_book.png"
                                                                    OnMouseOverImage="../Images/Icons/address_book_hover.png" CssClass="clickableLeft"
                                                                    ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="DocumentImgRecipientAddressBookMittDest_Click"
                                                                    OnClientClick="disallowOp('Content2');" />
                                                            </div>
                                                        </div>
                                                        <asp:HiddenField runat="server" ID="IdRecipient" />
                                                        <asp:HiddenField runat="server" ID="RecipientTypeOfCorrespondent" />
                                                        <div class="row">
                                                            <div class="colHalf">
                                                                <cc1:CustomTextArea ID="txt_codMit_E" runat="server" CssClass="txt_addressBookLeft"
                                                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                                    OnTextChanged="txt_codMit_E_TextChanged" onchange="disallowOp('Content2');">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                            <div class="colHalf2">
                                                                <div class="colHalf3">
                                                                    <cc1:CustomTextArea ID="txt_descrMit_E" runat="server" CssClass="txt_addressBookRight"
                                                                        CssClassReadOnly="txt_addressBookRight" AutoCompleteType="Disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                            <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="txt_descrMit_E"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="recipientSelected" BehaviorID="AutoCompleteExIngressoRecipient"
                                                                OnClientPopulated="recipientPopulated ">
                                                            </uc1:AutoCompleteExtender>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-right" style="font-size: 10px;">
                                                                <asp:CheckBox ID="chk_mitt_dest_storicizzati" runat="server" Checked="true" AutoPostBack="true"
                                                                    OnCheckedChanged="chk_mitt_dest_storicizzati_Clik" onchange="disallowOp('Content2');" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <fieldset class="azure" style="width: 96%;">
                                                                <span class="weight">
                                                                    <asp:Literal ID="SearchDocumentLitTypology" runat="server" /></span>
                                                                <div class="collapse shown" style="border-width: 0">
                                                                    <asp:UpdatePanel ID="UpPnlDocType" runat="server">
                                                                        <ContentTemplate>
                                                                            <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                                                                <ContentTemplate>
                                                                                    <div class="row">
                                                                                        <div class="col-full">
                                                                                            <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" AutoPostBack="True"
                                                                                                CssClass="chzn-select-deselect" Width="450px" OnSelectedIndexChanged="DocumentDdlTypeDocument_OnSelectedIndexChanged"
                                                                                                onchange="disallowOp('Content2');">
                                                                                                <asp:ListItem Text=""></asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </div>
                                                                                        <%--</div>--%>
                                                                                    </div>
                                                                                    <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                                                        <div class="row">
                                                                                            <div class="col-full">
                                                                                                <asp:DropDownList ID="ddlStateCondition" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                                                                    Width="500px">
                                                                                                    <asp:ListItem id="opt_StateConditionEquals" Value="Equals" />
                                                                                                    <asp:ListItem id="opt_StateConditionUnequals" Value="Unequals" />
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="row">
                                                                                            <div class="col-full">
                                                                                                <div class="col">
                                                                                                    <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" CssClass="chzn-select-deselect"
                                                                                                        Width="500px" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <asp:PlaceHolder runat="server" ID="PnlDocumentStateDiagramDate" Visible="false">
                                                                                            <div class="row">
                                                                                                <div class="col">
                                                                                                    <p>
                                                                                                        <span class="black">
                                                                                                            <asp:Literal ID="DocumentDateStateDiagram" runat="server"></asp:Literal>
                                                                                                        </span>
                                                                                                    </p>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:PlaceHolder>
                                                                                    </asp:PlaceHolder>
                                                                                    <span class="black">
                                                                                        <asp:PlaceHolder ID="PnlTypeDocument" runat="server"></asp:PlaceHolder>
                                                                                    </span>
                                                                                </ContentTemplate>
                                                                            </asp:UpdatePanel>
                                                                        </ContentTemplate>
                                                                    </asp:UpdatePanel>
                                                                </div>
                                                            </fieldset>
                                                        </div>
                                                        <%--******************* CODICE FASC. GEN./PROC. ***************--%>
                                                        <asp:UpdatePanel runat="server" ID="UpCodFasc" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <p>
                                                                            <span class="weight" style="color: #333333;">
                                                                                <asp:Literal runat="server" ID="LtlCodFascGenProc"></asp:Literal>
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                    <div class="col-right">
                                                                        <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                                            OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                                            OnClientClick="return ajaxModalPopupOpenTitolario();" CssClass="clickableLeft"
                                                                            ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png" />
                                                                        <cc1:CustomImageButton runat="server" ID="SearchProjectImg" ImageUrl="../Images/Icons/search_projects.png"
                                                                            OnMouseOutImage="../Images/Icons/search_projects.png" OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                                                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/search_projects_disabled.png"
                                                                            OnClientClick="return ajaxModalPopupSearchProject();" />
                                                                    </div>
                                                                    <div class="row">
                                                                        <asp:HiddenField ID="IdProject" runat="server" />
                                                                        <div class="colHalf">
                                                                            <cc1:CustomTextArea ID="txt_CodFascicolo" runat="server" CssClass="txt_addressBookLeft"
                                                                                OnTextChanged="txt_CodFascicolo_OnTextChanged" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                                                OnClientClick="disallowOp('Content2');">
                                                                            </cc1:CustomTextArea>
                                                                        </div>
                                                                        <div class="colHalf2">
                                                                            <div class="colHalf3">
                                                                                <cc1:CustomTextArea ID="txt_DescFascicolo" runat="server" CssClass="txt_addressBookRight"
                                                                                    CssClassReadOnly="txt_addressBookRight_disabled">
                                                                                </cc1:CustomTextArea>
                                                                            </div>
                                                                        </div>
                                                                        <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProject" TargetControlID="txt_DescFascicolo"
                                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                                                                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                            UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                                                            OnClientPopulated="acePopulatedCod">
                                                                        </uc1:AutoCompleteExtender>
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        <%--******************* CONSERVATO/NON ***************--%>
                                                        <asp:UpdatePanel runat="server" ID="UpConservatoNon" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <span class="weight">
                                                                            <asp:CheckBox ID="cb_Conservato" runat="server" onclick="swapConservationCheckboxs(this.id)" />
                                                                        </span>
                                                                    </div>
                                                                    <div class="col">
                                                                        <span class="weight">
                                                                            <asp:CheckBox ID="cb_NonConservato" runat="server" onclick="swapConservationCheckboxs(this.id)" />
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </asp:Panel>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                    <div id="centerContentAddressbookDx" style="overflow: auto;">
                        <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                            <ContentTemplate>
                                <asp:GridView ID="gridViewResult" runat="server" AllowSorting="false" AllowPaging="false"
                                    AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                    HorizontalAlign="Center"  AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                    Width="100%" OnPreRender="gridViewResult_PreRender" OnRowCreated="gridViewResult_ItemCreated">
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="SearchDocumentProjectBtnInsert" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="SearchDocumentProjectBtnInsert_Click" />
            <cc1:CustomButton ID="SearchDocumentBtnSearch" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentBtnSearch_Click"
                OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="SearchDocumentClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentClose_Click"
                OnClientClick="disallowOp('Content2');" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnCbSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="addAll_Click" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsAll" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });
        });
    </script>
</asp:Content>
