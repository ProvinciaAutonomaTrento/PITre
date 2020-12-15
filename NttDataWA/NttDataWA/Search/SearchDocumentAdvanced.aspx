<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchDocumentAdvanced.aspx.cs"
    Inherits="NttDataWA.Search.SearchDocumentAdvanced" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Src="~/UserControls/SearchDocumentsTabs.ascx" TagPrefix="uc2" TagName="SearchDocumentTabs" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.collapsed" });

            $('#contentSx input, #contentSx textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                    return false;
                }
            });

            $('#contentSx select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                    return false;
                }
            });
        });

        function swapUserPrivateCheckboxs(id) {
            if ($get('<%=cb_Conservato.ClientID%>') && $get('<%=cb_NonConservato.ClientID%>')) {
                if (id == '<%=cb_Conservato.ClientID%>' && $get('<%=cb_Conservato.ClientID%>').checked) {
                    $get('<%=cb_NonConservato.ClientID%>').checked = false;
                }
                else if (id == '<%=cb_NonConservato.ClientID%>' && $get('<%=cb_NonConservato.ClientID%>').checked) {
                    $get('<%=cb_Conservato.ClientID%>').checked = false;
                }
            }
        }

        function swapTrasmCheckboxs(id) {
            var PnlNeverTrasm = $('#<%=PnlNeverTrasm.ClientID%>');
            if ($get('<%=cbx_Trasm.ClientID%>') && $get('<%=cbx_TrasmSenza.ClientID%>')) {
                if (id == '<%=cbx_Trasm.ClientID%>' && $get('<%=cbx_Trasm.ClientID%>').checked) {
                    $get('<%=cbx_TrasmSenza.ClientID%>').checked = false;
                }
                else if (id == '<%=cbx_TrasmSenza.ClientID%>' && $get('<%=cbx_TrasmSenza.ClientID%>').checked) {
                    $get('<%=cbx_Trasm.ClientID%>').checked = false;
                }
            }
            if (id == '<%=cbx_TrasmSenza.ClientID%>' && $get('<%=cbx_TrasmSenza.ClientID%>').checked) {
                PnlNeverTrasm.show();
            }
            else{
                PnlNeverTrasm.hide();
            }
        }

        function SingleSelect(id) {
            if ($get('<%=chkFirmato.ClientID%>') && $get('<%=chkNonFirmato.ClientID%>')) {
                if (id == '<%=chkFirmato.ClientID%>' && $get('<%=chkFirmato.ClientID%>').checked) {
                    $get('<%=chkNonFirmato.ClientID%>').checked = false;
                }
                else if (id == '<%=chkNonFirmato.ClientID%>' && $get('<%=chkNonFirmato.ClientID%>').checked) {
                    $get('<%=chkFirmato.ClientID%>').checked = false;
                }
            }
        }

        function selectNeverSend() {
            var PnlNeverSendFrom = $('#<%=PnlNeverSendFrom.ClientID%>');
            if ($get('<%=cb_neverSend.ClientID%>').checked) {
                PnlNeverSendFrom.show();
            }
            else {
                PnlNeverSendFrom.hide();
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


          function protocollatorePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoProtocollatore');
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

        function protocollatoreSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneProtocollatore.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneProtocollatore.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneProtocollatore.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceProtocollatore.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneProtocollatore.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceProtocollatore.ClientID%>', '');
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


        function mittintPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoMittInt');
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

        function mittintSelected(sender, e) {
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

            var searchText = $get('<%=txt_descrMittInter_C.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_descrMittInter_C.ClientID%>").focus();
            document.getElementById("<%=this.txt_descrMittInter_C.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_codMittInter_C.ClientID%>").value = codice;
            document.getElementById("<%=txt_descrMittInter_C.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txt_codMittInter_C.ClientID%>', '');
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

        function firmatarioPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoFirmatario');
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

        function firmatarioSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneFirmatario.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneFirmatario.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneFirmatario.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceFirmatario.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneFirmatario.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceFirmatario.ClientID%>', '');
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

        function usrConsolidamentoPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoUsrConsolidamento');
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

        function usrConsolidamentoSelected(sender, e) {
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

            var searchText = $get('<%=txt_descrUsrConsolidamento.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_descrUsrConsolidamento.ClientID%>").focus();
            document.getElementById("<%=this.txt_descrUsrConsolidamento.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_codUsrConsolidamento.ClientID%>").value = codice;
            document.getElementById("<%=txt_descrUsrConsolidamento.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txt_codUsrConsolidamento.ClientID%>', '');
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

        function enableFiltersRep() {
            var prePracticeCodeTextBox = $('#<%=PnlFiltersRep.ClientID%>');
            var element = $('#opRepertorio').find('input').get(0);
            if (element.checked == true) {
                prePracticeCodeTextBox.show();
            }
            else {
                prePracticeCodeTextBox.hide();
                document.getElementById("<%=this.ddl_numRep.ClientID%>").selectedIndex = 0; 
                document.getElementById("<%=this.txt_initNumRep.ClientID%>").value = "";
                document.getElementById("<%=this.txt_fineNumRep.ClientID%>").value = "";
                document.getElementById("<%=this.ddl_dataRepertorio.ClientID%>").selectedIndex = 0;
                document.getElementById("<%=this.txt_initDataRep.ClientID%>").value = "";
                document.getElementById("<%=this.txt_fineDataRep.ClientID%>").value = "";
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

        function enableFieldPnlObjectAttach() {
            var prePracticeCodeTextBox = $('#<%=PnlAtt.ClientID%>');
            var panelObjectAttach = $('#<%=pnlOjectAttach.ClientID%>');
            var element = $('#opAll').find('input').get(0);
            var elOptArr = $('#opArr').find('input').get(0);
            var elOptPart = $('#opPart').find('input').get(0);
            var elOptInt = $('#opInt').find('input').get(0);
            var elOptGrigio = $('#opGrigio').find('input').get(0);
            var elOpPredisposed = $('#opPredisposed').find('input').get(0);
            var elOpPrints = $('#opPrints').find('input').get(0);
            if (element.checked == true) {
                prePracticeCodeTextBox.show();
                if (elOptArr.checked == false && elOptPart.checked == false && elOptInt.checked == false && elOptGrigio.checked == false && elOpPredisposed.checked == false && elOpPrints.checked == false) {
                    $('#<%=divDocumentLitObjectAttach.ClientID%>').show();
                    $('#<%=divDocumentLitObject.ClientID%>').hide();
                    panelObjectAttach.show();
                    $('#HiddenDisplayPanel').val('block');
                }
                else {
                    panelObjectAttach.hide();
                    $('#<%=divDocumentLitObjectAttach.ClientID%>').hide();
                    $('#<%=divDocumentLitObject.ClientID%>').show();
                    $('#TxtCodeObjectAttach').val('');
                    $('#TxtObjectAttach').val('');
                    $('#HiddenDisplayPanel').val('');
                }
            }
            else {
                $('#<%=divDocumentLitObjectAttach.ClientID%>').hide();
                $('#<%=divDocumentLitObject.ClientID%>').show();
                prePracticeCodeTextBox.hide();
                panelObjectAttach.hide();
                $('#TxtCodeObjectAttach').val('');
                $('#TxtObjectAttach').val('');
                $('#HiddenDisplayPanel').val('');
            }
        }

        function cb_selectall() {
            $('#HiddenItemsAll').val('true');
            $('#btnCbSelectAll').click();
        }

        function ClearFirmatario() {
            if (!$get('<%=chkFirmaElettronica.ClientID %>').checked) {
                document.getElementById("<%=this.txtCodiceFirmatario.ClientID%>").value = "";
                document.getElementById("<%=this.txtDescrizioneFirmatario.ClientID%>").value = "";
            }
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

        function setFocusOnTop() {
            $("#contentDx").scrollTop(0);
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
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ObjectFromMainDocument" runat="server" Url="../Popup/Object.aspx?fromMainDocument=true"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="800" Height="1000"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzata" runat="server" Url="../popup/GridPersonalization.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzataSave" runat="server" Url="../popup/GridPersonalizationSave.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GridPersonalizationPreferred" runat="server" Url="../popup/GridPersonalizationPreferred.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="SelectKeyword" runat="server" Url="../popup/SelectKeyword.aspx"
        Width="700" Height="550" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../popup/SearchProject.aspx?caller=search"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="SaveSearch" runat="server" Url="../Popup/SaveSearch.aspx" Width="650"
        Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ModifySearch" runat="server" Url="../Popup/SaveSearch.aspx?modify=true"
        Width="650" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx?from=search"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', ''); }" />
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
    <uc:ajaxpopup2 Id="MassiveSignatureHSM" runat="server" Url="../Popup/MassiveHSM_Signature.aspx?objType=D"
        Width="700" Height="400" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignatureApplet" runat="server" Url="../Popup/MassiveDigitalSignature_applet.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignatureSocket" runat="server" Url="../Popup/MassiveDigitalSignature_Socket.aspx?objType=D"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=doc&fromMassiveOperation=1"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolarioMassive" runat="server" Url="../Popup/ClassificationScheme.aspx?from=search&massive=true"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', ''); }" />
    <uc:ajaxpopup2 Id="SearchProjectMassive" runat="server" Url="../popup/SearchProject.aspx?caller=search"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ChooseCorrespondent" runat="server" Url="../popup/ChooseCorrespondent.aspx"
        Width="700" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="StartProcessSignature" runat="server" Url="../popup/StartProcessSignature.aspx?from=SearchDocument"
        Width="1200" Height="800" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="InfoSignatureProcessesStarted" runat="server" Url="../popup/InfoSignatureProcessesStarted.aspx"
        PermitClose="false" PermitScroll="false" Width="600" Height="300" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
     <uc:ajaxpopup2 Id="DetailsLFAutomaticMode" runat="server" Url="../popup/DetailsLFAutomaticMode.aspx"
        Width="750" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'SignatureProcessConcluted');}" />
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
                <div id="containerDocumentTab" class="containerStandardTab" style="overflow: visible;">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:SearchDocumentTabs ID="SearchDocumentTabs" runat="server" PageCaller="ADVANCED" />
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
                            <div class="row">
                                <!-- filters -->
                                <fieldset class="basic">
                                    <!-- ricerche salvate -->
                                    <asp:UpdatePanel ID="UpPnlRapidSearch" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder ID="plcSavedSearches" runat="server">
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="SearchDocumentLitRapidSearch" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <asp:DropDownList runat="server" ID="DdlRapidSearch" CssClass="chzn-select-deselect"
                                                            Width="100%" OnSelectedIndexChanged="DdlRapidSearch_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                                <asp:PlaceHolder runat="server" ID="PlcAdl" Visible="false">
                                    <div class="row">
                                        <fieldset class="basic">
                                            <div class="row">
                                                <div class="col">
                                                    <asp:RadioButtonList runat="server" ID="RblTypeAdl" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </asp:PlaceHolder>
                                <%-- ************** TIPO ******************** --%>
                                <div class="row">
                                    <fieldset class="basic">
                                        <asp:UpdatePanel runat="server" ID="UpPnlType" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-marginSx2">
                                                        <p>
                                                            <span class="weight">
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
                                                            <asp:ListItem Value="REP" Selected="False" id="opRepertorio" runat="server" Onclick="enableFiltersRep()"></asp:ListItem>
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
                                                                <asp:ListItem Value="albopubb" Text="Pubblicati" runat="server" />
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <%-- ************** FILTRI REPERTORIO ******************** --%>
                                                <asp:Panel ID="PnlFiltersRep" runat="server" CssClass="hidden">
                                                    <div class="row">
                                                        <asp:UpdatePanel ID="UpPnlFiltersRep" runat="server" ClientIDMode="static" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <div class="row">
                                                                    <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlNumRep"></asp:Literal>
                                                                        </span></p>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col2">
                                                                        <asp:DropDownList ID="ddl_numRep" runat="server" AutoPostBack="True" Width="140px"
                                                                            OnSelectedIndexChanged="ddl_numRep_SelectedIndexChanged">
                                                                            <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlDaNumRep"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_initNumRep" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlANumRep"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_fineNumRep" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                                <%-- DATA REPERTORIO --%>
                                                                <div class="row">
                                                                    <div class="col">
                                                                        <p>
                                                                            <span class="weight">
                                                                                <asp:Literal runat="server" ID="LtlDataRepertorio"></asp:Literal>
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col2">
                                                                        <asp:DropDownList ID="ddl_dataRepertorio" runat="server" AutoPostBack="true" Width="140px"
                                                                            OnSelectedIndexChanged="ddl_dataRepertorio_SelectedIndexChanged">
                                                                            <asp:ListItem Value="0"></asp:ListItem>
                                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                                            <asp:ListItem Value="2"></asp:ListItem>
                                                                            <asp:ListItem Value="3"></asp:ListItem>
                                                                            <asp:ListItem Value="4"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlDaDataRep"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_initDataRep" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="col2">
                                                                        <asp:Literal runat="server" ID="LtlADataRep"></asp:Literal>
                                                                    </div>
                                                                    <div class="col4">
                                                                        <cc1:CustomTextArea ID="txt_fineDataRep" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <%-- ************** OGGETTO ******************** --%>
                                        <asp:UpdatePanel ID="upPnlObject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div id="divDocumentLitObject" class="col-marginSx" runat="server">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="DocumentLitObject" runat="server"></asp:Literal></span></p>
                                                    </div>
                                                    <div id="divDocumentLitObjectAttach" class="col-marginSx" runat="server" style="display: none">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="DocumentLitObjectAttach" runat="server"></asp:Literal></span></p>
                                                    </div>
                                                    <div class="col-right-no-margin1">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                                                            OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                                            OnClientClick="return ajaxModalPopupObject();" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-marginSx-full">
                                                        <div class="full_width">
                                                            <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                                                <ContentTemplate>
                                                                    <asp:Panel ID="PnlCodeObject" runat="server" Visible="false">
                                                                        <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                                                            CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true" OnTextChanged="TxtCodeObject_Click">
                                                                        </cc1:CustomTextArea>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="PnlCodeObject2" runat="server">
                                                                        <asp:Panel ID="PnlCodeObject3" runat="server">
                                                                            <cc1:CustomTextArea ID="TxtObject" Width="99%" runat="server" class="txt_input_full"
                                                                                CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                                        </asp:Panel>
                                                                    </asp:Panel>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </div>
                                                    </div>
                                                </div>
                                                <%-- ************** OGGETTO ALLEGATO******************** --%>
                                                <asp:Panel ID="pnlOjectAttach" Style="display: none" runat="server">
                                                    <asp:HiddenField ID="HiddenDisplayPanel" runat="server" ClientIDMode="Static" />
                                                    <div class="row">
                                                        <div class="col-marginSx">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="LitObjectAttach" runat="server"></asp:Literal></span></p>
                                                        </div>
                                                        <div class="col-right-no-margin1">
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgObjectaryAttach" ImageUrl="../Images/Icons/obj_objects.png"
                                                                OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                                                OnClientClick="return ajaxModalPopupObjectFromMainDocument();" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-marginSx-full">
                                                            <div class="full_width">
                                                                <asp:UpdatePanel ID="UpdPnlObjectAttach" runat="server" UpdateMode="Conditional"
                                                                    ClientIDMode="static">
                                                                    <ContentTemplate>
                                                                        <asp:Panel ID="PnlCodeObjectAttach" runat="server" Visible="false">
                                                                            <cc1:CustomTextArea ID="TxtCodeObjectAttach" runat="server" CssClass="txt_addressBookLeft"
                                                                                CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true" OnTextChanged="TxtCodeObject_Click">
                                                                            </cc1:CustomTextArea>
                                                                        </asp:Panel>
                                                                        <asp:Panel ID="PnlCodeObjectAttach2" runat="server">
                                                                            <asp:Panel ID="PnlCodeObjectAttach3" runat="server">
                                                                                <cc1:CustomTextArea ID="TxtObjectAttach" Width="99%" runat="server" class="txt_input_full"
                                                                                    CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                                            </asp:Panel>
                                                                        </asp:Panel>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <%-- ************** ANNO ******************** --%>
                                        <asp:UpdatePanel ID="UpPnlYear" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-marginSx">
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
                                    </fieldset>
                                </div>
                                <%-- ******************** PROTOCOLLO ******************** --%>
                                <div class="row">
                                    <asp:PlaceHolder ID="phProtocol" runat="server">
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
                                                                        OnSelectedIndexChanged="ddl_numProt_E_SelectedIndexChanged">
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
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlDataProto"></asp:Literal>
                                                                        </span>
                                                                    </p>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_dataProt_E" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="ddl_dataProt_E_SelectedIndexChanged">
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
                                                            <%-- MITTENTE/DESTINATARIO --%>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="LtlMitDest"></asp:Literal>
                                                                        </span>
                                                                    </p>
                                                                </div>
                                                                <div class="col">
                                                                    <p>
                                                                        <asp:CheckBox ID="chk_mitt_dest_storicizzati" runat="server" Checked="true" AutoPostBack="true"
                                                                            OnCheckedChanged="chk_mitt_dest_storicizzati_Clik" />
                                                                    </p>
                                                                </div>
                                                                <div class="col-right-no-margin1">
                                                                    <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientAddressBookMittDest"
                                                                        ImageUrl="../Images/Icons/address_book.png" OnMouseOutImage="../Images/Icons/address_book.png"
                                                                        OnMouseOverImage="../Images/Icons/address_book_hover.png" CssClass="clickable"
                                                                        ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="DocumentImgRecipientAddressBookMittDest_Click" />
                                                                </div>
                                                            </div>
                                                            <asp:HiddenField runat="server" ID="IdRecipient" />
                                                            <asp:HiddenField runat="server" ID="RecipientTypeOfCorrespondent" />
                                                            <div class="row">
                                                                <div class="colHalf">
                                                                    <cc1:CustomTextArea ID="txt_codMit_E" runat="server" CssClass="txt_addressBookLeft"
                                                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                                        OnTextChanged="txt_codMit_E_TextChanged">
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
                                                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                    UseContextKey="true" OnClientItemSelected="recipientSelected" BehaviorID="AutoCompleteExIngressoRecipient"
                                                                    OnClientPopulated="recipientPopulated" Enabled="false">
                                                                </uc1:AutoCompleteExtender>
                                                            </div>
                                                            <%-- PROTOCOLLATORE --%>
                                                            <asp:UpdatePanel ID="UpPnlProtocollatore" runat="server" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <div class="row">
                                                                        <div class="col">
                                                                            <p>
                                                                                <span class="weight">
                                                                                    <asp:Literal runat="server" ID="litProtocollatore"></asp:Literal>
                                                                                </span>
                                                                            </p>
                                                                        </div>
                                                                    <div class="col">
                                                                        <p>
                                                                            <asp:RadioButtonList ID="rblProtocollatoreType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                                                                                <asp:ListItem id="optProtocollatoreUO" runat="server" Value="U" />
                                                                                <asp:ListItem id="optProtocollatoreRole" runat="server" Value="R" Selected="True" />
                                                                                <asp:ListItem id="optProtocollatoreUser" runat="server" Value="P" />
                                                                            </asp:RadioButtonList>
                                                                        </p>
                                                                    </div>
                                                                    <div class="colHalf">
                                                                        &nbsp;
                                                                    </div>
                                                                    <div class="col">
                                                                    </div>
                                                                    <div class="col-right-no-margin">
                                                                        <cc1:CustomImageButton runat="server" ID="ImgProtocollatoreAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                            OnClick="ImgProtocollatoreAddressBook_Click" />
                                                                    </div>
                                                                 </div>
                                                                <div class="row">
                                                                    <asp:HiddenField ID="idProtocollatore" runat="server" />
                                                                    <div class="colHalf">
                                                                        <cc1:CustomTextArea ID="txtCodiceProtocollatore" runat="server" CssClass="txt_addressBookLeft"
                                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                            AutoCompleteType="Disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="colHalf2">
                                                                        <div class="colHalf3">
                                                                            <cc1:CustomTextArea ID="txtDescrizioneProtocollatore" runat="server" CssClass="txt_projectRight"
                                                                                CssClassReadOnly="txt_ProjectRight_disabled">
                                                                            </cc1:CustomTextArea>
                                                                        </div>
                                                                    </div>
                                                                    <uc1:AutoCompleteExtender runat="server" ID="RapidProtocollatore" TargetControlID="txtDescrizioneProtocollatore"
                                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                        UseContextKey="true" OnClientItemSelected="protocollatoreSelected" BehaviorID="AutoCompleteExIngressoProtocollatore"
                                                                        OnClientPopulated="protocollatorePopulated " Enabled="false">
                                                                    </uc1:AutoCompleteExtender>
                                                                </div>
                                                                    <div class="row">
                                                                        <div class="col-right">
                                                                            <asp:CheckBox ID="chkProtocollatoreExtendHistoricized" runat="server" Checked="true" />
                                                                        </div>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </asp:PlaceHolder>
                                </div>
                                <%-- ************** DOCUMENTO ******************** --%>
                                <div class="row">
                                    <fieldset>
                                        <div class="row">
                                            <h2 class="expand">
                                                <asp:Literal runat="server" ID="SearchDocumentLit"></asp:Literal>
                                            </h2>
                                            <div id="Div1" class="collapse shown" runat="server">
                                                <asp:UpdatePanel runat="server" ID="UpDoc" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <%-- ID DOCUMENTO--%>
                                                        <div class="row">
                                                            <div class="col">
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlIdDoc"></asp:Literal>
                                                                </span>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <asp:DropDownList ID="ddl_idDocumento_C" runat="server" Width="140px" AutoPostBack="true"
                                                                    OnSelectedIndexChanged="ddl_idDocumento_C_SelectedIndexChanged">
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
                                                                    <span class="weight">
                                                                        <asp:Literal runat="server" ID="LtlDataCreazione"></asp:Literal>
                                                                    </span>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col2">
                                                                <asp:DropDownList ID="ddl_dataCreazione_E" runat="server" AutoPostBack="true" Width="140px"
                                                                    OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
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
                                                        <%-- CREATORE --%>
                                                        <asp:UpdatePanel ID="upPnlCreatore" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
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
                                                                        <strong>
                                                                            <asp:Literal ID="litCreator" runat="server" /></strong></div>
                                                                    <div class="colHalf">
                                                                        <cc1:CustomTextArea ID="txtCodiceCreatore" runat="server" CssClass="txt_addressBookLeft"
                                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                            AutoCompleteType="Disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="colHalf7">
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
                                                                        OnClientPopulated="creatorePopulated " Enabled="false">
                                                                    </uc1:AutoCompleteExtender>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-right">
                                                                        <asp:CheckBox ID="chkCreatoreExtendHistoricized" runat="server" Checked="true" />
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        <%-- PROPRIETARIO --%>
                                                        <asp:UpdatePanel ID="upPnlProprietario" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <div class="row">
                                                                    <div class="colHalf">
                                                                        &nbsp;</div>
                                                                    <div class="col">
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
                                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                            OnClick="ImgProprietarioAddressBook_Click" />
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <asp:HiddenField ID="idProprietario" runat="server" />
                                                                    <div class="colHalf">
                                                                        <strong>
                                                                            <asp:Literal ID="litOwner" runat="server" /></strong></div>
                                                                    <div class="colHalf">
                                                                        <cc1:CustomTextArea ID="txtCodiceProprietario" runat="server" CssClass="txt_addressBookLeft"
                                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                            AutoCompleteType="Disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                    <div class="colHalf7">
                                                                        <div class="colHalf3">
                                                                            <cc1:CustomTextArea ID="txtDescrizioneProprietario" runat="server" CssClass="txt_projectRight"
                                                                                CssClassReadOnly="txt_ProjectRight_disabled">
                                                                            </cc1:CustomTextArea>
                                                                        </div>
                                                                    </div>
                                                                    <uc1:AutoCompleteExtender runat="server" ID="RapidProprietario" TargetControlID="txtDescrizioneProprietario"
                                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                        UseContextKey="true" OnClientItemSelected="proprietarioSelected" BehaviorID="AutoCompleteExIngressoProprietario"
                                                                        OnClientPopulated="proprietarioPopulated " Enabled="false">
                                                                    </uc1:AutoCompleteExtender>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
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
                                                        <div class="col" style="width: 240px">
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
                                <%-- ************** TIPOLOGIA ******************** --%>
                                <div class="row">
                                    <fieldset class="azure">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="SearchDocumentLitTypology" runat="server" /></span>
                                                    </div>
                                                </div>
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
                                                                            <asp:DropDownList ID="ddlStateCondition" runat="server" Width="100%">
                                                                                <asp:ListItem id="opt_StateConditionEquals" Value="Equals" />
                                                                                <asp:ListItem id="opt_StateConditionUnequals" Value="Unequals" />
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-full">
                                                                            <div class="styled-select_full">
                                                                                <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" Width="100%" />
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
                                <asp:PlaceHolder ID="phProtoMitt" runat="server">
                                    <fieldset>
                                        <%--******************* PROTOCOLLO MITTENTE ***************--%>
                                        <asp:UpdatePanel runat="server" ID="UpPnlProtoMitt" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="LtlProtMitt"></asp:Literal>
                                                        </span>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea ID="txt_numProtMitt_C" Width="99%" runat="server" CssClass="txt_input_full"
                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <%--******************* DATA PROT. MITT. ***************--%>
                                        <asp:UpdatePanel runat="server" ID="UpDataProtMitt" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlDataProtMitt"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_dataProtMitt_C" runat="server" AutoPostBack="true" Width="140px"
                                                                OnSelectedIndexChanged="ddl_dataProtMitt_C_SelectedIndexChanged">
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="4"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlDaDataProtMitt"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_initDataProtMitt_C" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlADataProtMitt"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_fineDataProtMitt_C" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </asp:PlaceHolder>
                                <%--******************* DATA SCADENZA ***************--%>
                                <asp:PlaceHolder ID="phDataScadProtMitt" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpDataScadProtMitt" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlDataScad"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_dataScadenza_C" runat="server" AutoPostBack="true" Width="140px"
                                                                OnSelectedIndexChanged="ddl_dataScadenza_C_SelectedIndexChanged">
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="4"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlDaDataScad"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_initDataScadenza_C" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlADataScad"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_fineDataScadenza_C" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* DATA STAMPA ***************--%>
                                <asp:PlaceHolder ID="phDataStampa" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpDataStampa" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlDataStampa"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_dataStampa_E" runat="server" AutoPostBack="true" Width="140px"
                                                                OnSelectedIndexChanged="ddl_dataStampa_E_SelectedIndexChanged">
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="4"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlDaDataStampa"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_initDataStampa_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlADataStampa"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_finedataStampa_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* MEZZO DI SPEDIZIONE ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpMezzoSped" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder ID="plcMezzoSped" runat="server" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlMezzoSpediz"></asp:Literal>
                                                            </span>
                                                        </div>
                                                        <div class="col">
                                                            <asp:DropDownList ID="ddl_spedizione" runat="server" Width="250px" />
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* MAI TRASMESSI/SPEDITI AI DESTINATARI ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpPnlMaiTasmessiSpeditiDest" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder ID="PnlMaiTasmessiSpeditiDest" runat="server">
                                            <div class="row">
                                                <fieldset>
                                                     <div class="row">
                                                        <asp:RadioButtonList ID="rbl_maiTrasmessiMaiSpeditiDest" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem value="S"></asp:ListItem>
                                                            <asp:ListItem Value="T" ></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                    <%--<div class="row">
                                                        <div class="col">
                                                            <asp:CheckBox ID="cbx_maiTrasmDest" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:CheckBox ID="cbx_maiSpeditiDest" runat="server" />
                                                        </div>
                                                    </div>--%>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* CONSERVATO/NON ***************--%>
                                <asp:PlaceHolder ID="phConservatoNon" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpConservatoNon" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
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
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--****************** STATO CONSERVAZIONE ****************** --%>
                                <asp:PlaceHolder ID="phStatoConservazione" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpStatoConservazione" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlStatoCons"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col_full">
                                                            <asp:CheckBoxList ID="cbl_Conservazione" runat="server" CssClass="testo_grigio" RepeatDirection="Vertical">
                                                                <asp:ListItem Value="N" Selected="True" runat="server" id="optConsNC"></asp:ListItem>
                                                                <asp:ListItem Value="V" Selected="True" runat="server" id="optConsAtt"></asp:ListItem>
                                                                <asp:ListItem Value="W" Selected="True" runat="server" id="optConsVer"></asp:ListItem>
                                                                <asp:ListItem Value="C" Selected="True" runat="server" id="optConsPre"></asp:ListItem>
                                                                <asp:ListItem Value="R" Selected="True" runat="server" id="optConsRif"></asp:ListItem>
                                                                <asp:ListItem Value="E" Selected="True" runat="server" id="optConsErr"></asp:ListItem>
                                                                <asp:ListItem Value="T" Selected="True" runat="server" id="optConsTim"></asp:ListItem>
                                                                <asp:ListItem Value="F" Selected="True" runat="server" id="optConsFld"></asp:ListItem>
                                                                <asp:ListItem Value="B" Selected="True" runat="server" id="optConsBfw"></asp:ListItem>
                                                                <asp:ListItem Value="K" Selected="True" runat="server" id="optConsBfe"></asp:ListItem>
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
                                                            <cc1:CustomTextArea runat="server" ID="txtCodPolicy" Columns="8" CssClass="txt_input_full"
                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col">
                                                            <asp:Literal runat="server" ID="LtlCounterPolicy"></asp:Literal>
                                                        </div>
                                                        <div class="col">
                                                            <cc1:CustomTextArea runat="server" ID="txtCounterPolicy" Columns="4" CssClass="txt_input_full onlynumbers"
                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:Literal runat="server" ID="ltlDatePolicy"></asp:Literal>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_datePolicy" runat="server" AutoPostBack="true" Width="140px"
                                                                OnSelectedIndexChanged="ddl_datePolicy_SelectedIndexChanged">
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="4"></asp:ListItem>
                                                                <asp:ListItem Value="5"></asp:ListItem>
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
                                </asp:PlaceHolder>
                                <%--******************* STATO DEL DOCUMENTO ***************--%>
                                <asp:PlaceHolder ID="phStatoDoc" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpStatoDoc" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlStatodelDoc"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:RadioButtonList ID="rb_annulla_C" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="T" Selected="True"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* SEGNATURA ***************--%>
                                <asp:PlaceHolder ID="phSegnatura" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpPnlSegnatura" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlSegnatura"></asp:Literal>
                                                            </span>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <cc1:CustomTextArea ID="txt_segnatura" Width="99%" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled"
                                                                    runat="server"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* MITTENTE INTERMEDIO ***************--%>
                                <asp:PlaceHolder ID="phMittInter" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpMittInter" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlMittItermedio"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin1">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientAddressBookMittInter"
                                                            ImageUrl="../Images/Icons/address_book.png" OnMouseOutImage="../Images/Icons/address_book.png"
                                                            OnMouseOverImage="../Images/Icons/address_book_hover.png" CssClass="clickable"
                                                            ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="DocumentImgRecipientAddressBookMittInter_Click" />
                                                    </div>
                                                    <div class="row">
                                                        <div class="colHalf">
                                                            <asp:HiddenField ID="idMittItermedio" runat="server" />
                                                            <cc1:CustomTextArea ID="txt_codMittInter_C" runat="server" CssClass="txt_addressBookLeft"
                                                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                AutoCompleteType="Disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="txt_descrMittInter_C" runat="server" CssClass="txt_addressBookRight"
                                                                    CssClassReadOnly="txt_addressBookRight" AutoCompleteType="Disabled">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                        <uc1:AutoCompleteExtender runat="server" ID="RapidMittInter" TargetControlID="txt_descrMittInter_C"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="mittintSelected" BehaviorID="AutoCompleteExIngressoMittInt"
                                                            OnClientPopulated="mittintPopulated " Enabled="false">
                                                        </uc1:AutoCompleteExtender>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* CODICE FASC. GEN./PROC. ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpCodFasc" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="LtlCodFascGenProc"></asp:Literal>
                                                        </span>
                                                    </p>
                                                </div>
                                                <div class="col-right-no-margin1">
                                                    <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                        runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                        OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                        OnClientClick="return ajaxModalPopupOpenTitolario();" CssClass="clickable" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png" />
                                                    <cc1:CustomImageButton runat="server" ID="SearchProjectImg" ImageUrl="../Images/Icons/search_projects.png"
                                                        OnMouseOutImage="../Images/Icons/search_projects.png" OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/search_projects_disabled.png"
                                                        OnClientClick="return ajaxModalPopupSearchProject();" />
                                                </div>
                                                <div class="row">
                                                    <asp:HiddenField ID="IdProject" runat="server" />
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="txt_CodFascicolo" runat="server" CssClass="txt_addressBookLeft"
                                                            OnTextChanged="txt_CodFascicolo_OnTextChanged" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled">
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
                                                        MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                                        OnClientPopulated="acePopulatedCod" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                                <asp:Panel ID="PnlEstendiAFascicoli" runat ="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col-right">
                                                            <asp:CheckBox ID="cbxEstendiAFascicoli" runat="server" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* DATA ARRIVO ***************--%>
                                <asp:PlaceHolder ID="phDataArrivo" runat="server">
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
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_dataArrivo_C" runat="server" AutoPostBack="true" Width="140px"
                                                                OnSelectedIndexChanged="ddl_dataArrivo_C_SelectedIndexChanged">
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="4"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlDaDataArrivo"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_initDataArrivo_C" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlADataArrivo"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_fineDataArrivo_C" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* PAROLA CHIAVE ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpParolaChiave" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder runat="server" ID="PlcKeyWord" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="LtlParolaChiave" runat="server"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton ID="ImgSelectKeyword" ImageUrl="../Images/Icons/obj_objects.png"
                                                                runat="server" OnMouseOverImage="../Images/Icons/obj_objects_hover.png" OnMouseOutImage="../Images/Icons/obj_objects.png"
                                                                ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png" CssClass="clickable"
                                                                OnClick="DocumentImgSelectKeyword_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:ListBox ID="ListKeywords" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled">
                                                        </asp:ListBox>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* NUM OGGETTO ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpPnlNumOggetto" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder ID="plcNumOggetto" runat="server" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="litNumOggetto"></asp:Literal>
                                                            </span>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <cc1:CustomTextArea ID="txt_numOggetto" Width="99%" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled"
                                                                    runat="server"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="litCommRef"></asp:Literal>
                                                            </span>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <cc1:CustomTextArea ID="txt_commRef" Width="99%" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled"
                                                                    runat="server"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* NOTE ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpNote" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlNote" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:RadioButtonList ID="rl_visibilita" runat="server" RepeatDirection="Horizontal"
                                                            AutoPostBack="true" OnSelectedIndexChanged="DataChanged">
                                                            <asp:ListItem Value="Q" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Value="T"></asp:ListItem>
                                                            <asp:ListItem Value="R"></asp:ListItem>
                                                            <asp:ListItem Value="F"></asp:ListItem>
                                                            <asp:ListItem Value="P"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                    <div class="col-right-no-margin-alLeft">
                                                        <asp:DropDownList Visible="false" ID="ddlNoteRF" runat="server" Width="150px">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <cc1:CustomTextArea ID="Txtnote" Width="99%" runat="server" CssClass="txt_input_full"
                                                            CssClassReadOnly="txt_input_full_disabled" MaxLength="2000"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* DOC. IN COMPLETAMENTO ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpDocInColl" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlDocInCompletamento" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:CheckBoxList ID="cbl_docInCompl" runat="server" RepeatColumns="2" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="C_Img"></asp:ListItem>
                                                            <asp:ListItem Value="S_Img"></asp:ListItem>
                                                            <asp:ListItem Value="C_Fasc"></asp:ListItem>
                                                            <asp:ListItem Value="S_Fasc"></asp:ListItem>
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* TIMESTAMP ***************--%>
                                <asp:PlaceHolder ID="phTimestamp" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpPnlTimestamp" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:RadioButtonList ID="rbl_timestamp" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
                                                                AutoPostBack="True" OnSelectedIndexChanged="rbl_timestamp_SelectedIndexChanged">
                                                                <asp:ListItem id="rbl_timestamp0" Value="0"></asp:ListItem>
                                                                <asp:ListItem id="rbl_timestamp1" Value="1"></asp:ListItem>
                                                                <asp:ListItem id="rbl_timestamp2" Value="2" Selected="True"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:DropDownList ID="ddl_timestamp" runat="server" Width="200px" Visible="false"
                                                                AutoPostBack="true" OnSelectedIndexChanged="ddl_timestamp_SelectedIndexChanged">
                                                                <asp:ListItem id="ddl_timestamp0" Value="0" Text="" Selected="True"></asp:ListItem>
                                                                <asp:ListItem id="ddl_timestamp1" Value="1"></asp:ListItem>
                                                                <asp:ListItem id="ddl_timestamp2" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col">
                                                            <cc1:CustomTextArea ID="date_timestamp" runat="server" Visible="false" Width="80px"
                                                                CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* TRASMESSE ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpTrasm" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:CheckBox ID="cbx_Trasm" runat="server" onclick="swapTrasmCheckboxs(this.id)" />
                                                    </div>
                                                    <div class="col">
                                                        <asp:CheckBox ID="cbx_TrasmSenza" runat="server" onclick="swapTrasmCheckboxs(this.id)" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlRagione"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                    <div class="col">
                                                        <p>
                                                            <asp:DropDownList ID="ddl_ragioneTrasm" runat="server" Width="250px" />
                                                        </p>
                                                    </div>
                                                </div>
                                                <asp:Panel ID="PnlNeverTrasm" ClientIDMode="Static" CssClass="hidden" runat="server">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="LtlNeverTrasmFrom" runat="server"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <asp:RadioButtonList ID="rbl_neverTrasm" runat="server" RepeatDirection="Vertical">
                                                                    <asp:ListItem id="rb_allRoleTrasm" Value="T" Selected="true"></asp:ListItem>
                                                                    <asp:ListItem id="rb_roleUserTrasm" Value="R"></asp:ListItem>
                                                                    <asp:ListItem id="rb_onlyUserTrasm" Value="U"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* SEGNATURA DI EMERGENZA ***************--%>
                                <asp:PlaceHolder ID="phSegnEmer" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpSegnEmer" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="LtlSegnatDiEmerg"></asp:Literal>
                                                        </span>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea ID="txt_protoEme" Width="99%" runat="server" CssClass="txt_input_full"
                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlDataSegDiEmerg"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <asp:UpdatePanel runat="server" ID="UpDataSegnaEme" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_dataProtoEme" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="ddl_dataProtoEme_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaDataSegDiEmerg"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_dataProtoEmeInizio" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlADataSegDiEmerg"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txt_dataProtoEmeFine" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* EVIDENZA ***************--%>
                                <asp:PlaceHolder ID="phEvidenza" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpEvidenza" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal ID="LtlEvidenza" runat="server"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:RadioButtonList ID="rb_evidenza_C" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="T" Selected="True"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* TIPO FILE ACQUISITO ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpTipoFile" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlTipoFileAcq" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" Width="140px">
                                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col13">
                                                        <asp:CheckBox ID="chkFirmato" runat="server" onclick="SingleSelect(this.id);" />
                                                    </div>
                                                    <div class="col13">
                                                        <asp:CheckBox ID="chkNonFirmato" runat="server" onclick="SingleSelect(this.id);" />
                                                    </div>
                                                    <div class="col13">
                                                        <asp:CheckBox ID="cbx_nonConforme" runat="server" />
                                                    </div>    
                                                </div>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* FIRMA ELETTRONICA ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpPnlElectronicSignature" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlFirmaElettronica" runat="server" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <asp:CheckBox ID="chkFirmaElettronica" runat="server" OnClick="return ClearFirmatario();" />
                                                    </div>
                                                    <%-- FIRMATARIO --%>
                                                    <asp:UpdatePanel ID="UpPnlFirmatario" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="colHalf">
                                                                    &nbsp;</div>
                                                                <div class="col">
                                                                    <asp:RadioButtonList ID="rblFirmatarioType" runat="server" CssClass="rblHorizontal"
                                                                        RepeatLayout="UnorderedList">
                                                                        <asp:ListItem id="optFirmatarioRole" runat="server" Value="R" Selected="True" />
                                                                        <asp:ListItem id="optFirmatarioUser" runat="server" Value="P" />
                                                                    </asp:RadioButtonList>
                                                                </div>
                                                                <div class="col-right-no-margin">
                                                                    <cc1:CustomImageButton runat="server" ID="ImgFirmatarioAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                        OnClick="ImgFirmatarioAddressBook_Click" />
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <asp:HiddenField ID="idFirmatario" runat="server" />
                                                                <div class="colHalf">
                                                                    <strong>
                                                                        <asp:Literal ID="ltlFirmatario" runat="server" /></strong></div>
                                                                <div class="colHalf">
                                                                    <cc1:CustomTextArea ID="txtCodiceFirmatario" runat="server" CssClass="txt_addressBookLeft"
                                                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                        AutoCompleteType="Disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                                <div class="colHalf7">
                                                                    <div class="colHalf3">
                                                                        <cc1:CustomTextArea ID="txtDescrizioneFirmatario" runat="server" CssClass="txt_projectRight"
                                                                            CssClassReadOnly="txt_ProjectRight_disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                                <uc1:AutoCompleteExtender runat="server" ID="RapidFirmatario" TargetControlID="txtDescrizioneFirmatario"
                                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                    UseContextKey="true" OnClientItemSelected="firmatarioSelected" BehaviorID="AutoCompleteExIngressoFirmatario"
                                                                    OnClientPopulated="firmatarioPopulated " Enabled="false">
                                                                </uc1:AutoCompleteExtender>
                                                            </div>
                                                            <%--
                                                        <div class="row">
                                                            <div class="col-right">
                                                                <asp:CheckBox ID="chkFirmatarioExtendHistoricized" runat="server" Checked="true" />
                                                            </div>
                                                        </div>--%>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </fieldset>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* VERSIONI ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpVersioni" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlVersioni" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:Literal ID="LtlNumVersDoc" runat="server"></asp:Literal>
                                                    </div>
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_op_versioni" runat="server" Width="50px">
                                                            <asp:ListItem Text="<" Value="<" />
                                                            <asp:ListItem Text="=" Value="=" />
                                                            <asp:ListItem Text=">" Value=">" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col">
                                                        <cc1:CustomTextArea ID="txt_versioni" runat="server" Columns="3" CssClass="txt_input_full onlynumbers"
                                                            CssClassReadOnly="txt_input_full_disabled" />
                                                    </div>
                                                </div>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* ALLEGATI ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpAllegati" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlAllegati" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:Literal ID="LtlNumAllegatiDoc" runat="server"></asp:Literal>
                                                    </div>
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_op_allegati" runat="server" Width="50px">
                                                            <asp:ListItem Text="<" Value="<" />
                                                            <asp:ListItem Text="=" Value="=" />
                                                            <asp:ListItem Text=">" Value=">" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col">
                                                        <cc1:CustomTextArea ID="txt_allegati" runat="server" Columns="3" CssClass="txt_input_full onlynumbers"
                                                            CssClassReadOnly="txt_input_full_disabled" />
                                                    </div>
                                                    <div class="col">
                                                        <asp:RadioButtonList ID="rblFiltriNumAllegati" runat="server">
                                                            <asp:ListItem Value="tutti" Selected="True" />
                                                            <asp:ListItem Value="pec" />
                                                            <asp:ListItem Value="user" />
                                                            <asp:ListItem Value="esterni" />
                                                            <asp:ListItem Value="SIMPLIFIEDINTEROPERABILITY" runat="server" id="rblFiltriNumAllegatiOpIS" />
                                                            <asp:ListItem Value="albopubb" Text="Pubblicati" runat="server" />
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* MAI SPEDITI ***************--%>
                                <asp:UpdatePanel ID="UpPnlnNeverSend" runat="server" ClientIDMode="static" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlNeverSend" runat="server" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <asp:CheckBox runat="server" ID="cb_neverSend" onClick="selectNeverSend();" />
                                                    </div>
                                                    <asp:Panel ID="PnlNeverSendFrom" runat="server" CssClass="hidden">
                                                        <div class="col">
                                                            <asp:Literal ID="LtlNeverSendFrom" runat="server"></asp:Literal>
                                                        </div>
                                                        <div class="col-full">
                                                            <asp:RadioButtonList ID="rbl_NeverSend" runat="server" RepeatDirection="Vertical">
                                                                <asp:ListItem id="rb_allRole" Value="T" Selected="true"></asp:ListItem>
                                                                <asp:ListItem id="rb_roleUser" Value="R"></asp:ListItem>
                                                                <asp:ListItem id="rb_onlyUser" Value="U"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </asp:Panel>
                                                </fieldset>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* DOCUMENTI SPEDITI ***************--%>
                                <asp:PlaceHolder ID="phDocSpediti" runat="server">
                                    <asp:UpdatePanel runat="server" ID="UpDocSpeditiEsito" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="LtlDocSpeditiEsito" runat="server"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <asp:RadioButtonList ID="rb_docSpeditiEsito" runat="server" RepeatDirection="Vertical"
                                                                AutoPostBack="true">
                                                                <asp:ListItem Value="V"><img src="../Images/Common/messager_check.png" style="vertical-align: middle;"/></asp:ListItem>
                                                                <asp:ListItem Value="A"><img src="../Images/Common/messager_warning.png" style="vertical-align: middle;"/></asp:ListItem>
                                                                <asp:ListItem Value="X"><img src="../Images/Common/messager_error.png" style="vertical-align: middle;"/></asp:ListItem>
                                                                <asp:ListItem Value="R"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:UpdatePanel runat="server" ID="UpDocSpediti" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="LtlDocSpediti" runat="server"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin1">
                                                        <asp:CheckBox runat="server" ID="cbx_pec" Text="PEC" />&nbsp;&nbsp;<asp:CheckBox
                                                            runat="server" ID="cbx_pitre"/>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <asp:RadioButtonList ID="rb_docSpediti" runat="server" RepeatDirection="Vertical"
                                                                AutoPostBack="true">
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="T" style="visibility: hidden;"></asp:ListItem>
                                                                <asp:ListItem Value="R" Selected="True"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            &nbsp;
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlDataSpedizione"></asp:Literal>
                                                            </span>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlDaDataSpedizione"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_dataSpedDa" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlADataSpedizione"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_dataSpedA" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                                <%--******************* RICEVUTE PTRE ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpRicPTRE" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="p_ricevute_pitre" runat="server">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlRicPTRE" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            &nbsp;
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:Literal runat="server" ID="LtlRicDi"></asp:Literal>
                                                        </div>
                                                        <div class="col">
                                                            <asp:DropDownList ID="ddl_ricevute_pitre" runat="server" AutoPostBack="true" Width="220px">
                                                                <asp:ListItem Text="" Value="" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="avvenuta-consegna" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="errore-consegna" runat="server"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlDataRicevuta"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <%-- ********** DATA RICEVUTA ********** --%>
                                                    <asp:UpdatePanel runat="server" ID="UpDataRicevute" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <asp:DropDownList ID="ddl_data_ricevute_pitre" runat="server" AutoPostBack="true"
                                                                        Width="140px" OnSelectedIndexChanged="ddl_data_ricevute_pitre_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaDataRicevuta"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="Cal_Da_pitre" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlADataRicevuta"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="Cal_A_pitre" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </fieldset>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* RICEVUTE PEC ***************--%>
                                <asp:Panel ID="p_ricevute_pec" runat="server" Visible="true">
                                    <asp:UpdatePanel runat="server" ID="UpRicPEC" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal ID="LtlRicPEC" runat="server"></asp:Literal>
                                                        </span>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            &nbsp;
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <asp:Literal runat="server" ID="LtlRicDiPEC"></asp:Literal>
                                                        </div>
                                                        <div class="col">
                                                            <asp:DropDownList ID="ddl_ricevute_pec" runat="server" AutoPostBack="true" Width="220px">
                                                                <asp:ListItem Text="" Value="" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="accettazione" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="avvenuta-consegna" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="non-accettazione" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="errore-consegna" runat="server"></asp:ListItem>
                                                                <asp:ListItem Value="errore" runat="server"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlDataRicevutaPec"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <%-- ********** DATA RICEVUTA ********** --%>
                                                    <asp:UpdatePanel runat="server" ID="UpDataRicevutePEC" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col2">
                                                                    <asp:DropDownList ID="ddl_data_ricevute_pec" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="ddl_data_ricevute_pec_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaDataRicevutaPEC"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="Cal_Da_pec" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlADataRicevutaPEC"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="Cal_A_pec" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:Panel>
                                <%--******************* STATO CONSOLIDAMENTO ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpStatoConsolidamento" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <fieldset>
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal ID="LtlStatoConsolid" runat="server"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:CheckBoxList ID="lstFiltriConsolidamento" runat="server" RepeatDirection="Vertical"
                                                            AutoPostBack="true" OnSelectedIndexChanged="lstFiltriConsolidamento_SelectedIndexChanged">
                                                            <asp:ListItem Value="0"></asp:ListItem>
                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                            <asp:ListItem Value="2"></asp:ListItem>
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                                <asp:Panel ID="pnl_data_cons" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LtlDataConsolidamento"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <asp:UpdatePanel runat="server" ID="UpDataConsolidamento" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <asp:DropDownList ID="cboDataConsolidamento" runat="server" AutoPostBack="true" Width="140px"
                                                                        OnSelectedIndexChanged="cboDataConsolidamento_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlDaDataConsolidamento"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txtDataConsolidamento" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="col2">
                                                                    <asp:Literal runat="server" ID="LtlADataConsolidamento"></asp:Literal>
                                                                </div>
                                                                <div class="col4">
                                                                    <cc1:CustomTextArea ID="txtDataConsolidamentoFinale" runat="server" Width="80px"
                                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal runat="server" ID="litUsrConsolidamento"></asp:Literal>
                                                                        </span>
                                                                    </p>
                                                                </div>
                                                                <div class="col-right-no-margin1">
                                                                    <cc1:CustomImageButton runat="server" ID="ImgUsrConsolidamentoAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                        OnClick="DocumentImgAddressBookUsrConsolidamento_Click" />
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="colHalf">
                                                                    <asp:HiddenField ID="idUsrConsolidamento" runat="server" />
                                                                    <asp:HiddenField ID="UsrConsolidamentoTypeOfCorrespondent" runat="server" />
                                                                    <cc1:CustomTextArea ID="txt_codUsrConsolidamento" runat="server" CssClass="txt_addressBookLeft"
                                                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                        AutoCompleteType="Disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                                <div class="colHalf2">
                                                                    <div class="colHalf3">
                                                                        <cc1:CustomTextArea ID="txt_descrUsrConsolidamento" runat="server" CssClass="txt_addressBookRight"
                                                                            CssClassReadOnly="txt_addressBookRight" AutoCompleteType="Disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                                <uc1:AutoCompleteExtender runat="server" ID="RapidUsrConsolidamento" TargetControlID="txt_descrUsrConsolidamento"
                                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                    UseContextKey="true" OnClientItemSelected="usrConsolidamentoSelected" BehaviorID="AutoCompleteExIngressoUsrConsolidamento"
                                                                    OnClientPopulated="usrConsolidamentoPopulated " Enabled="false">
                                                                </uc1:AutoCompleteExtender>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </asp:Panel>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* AMMINISTRAZIONE INTEROPERANTE ***************--%>
                                <asp:UpdatePanel runat="server" ID="UpPnlCodAmm" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder ID="p_cod_amm" runat="server">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="litCodAmm"></asp:Literal>
                                                        </span>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea ID="txt_codDesc" Width="99%" runat="server" CssClass="txt_input_full"
                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--******************* VISIBILITA' ***************--%>
                                <asp:UpdatePanel ID="UpPnlVisibility" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder ID="plcVisibility" runat="server" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="col">
                                                        <asp:Literal ID="litVisibility" runat="server" /></div>
                                                    <div class="col">
                                                        <asp:RadioButtonList ID="rblVisibility" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem id="optVisibility1" Value="T_A" Selected="True" />
                                                            <asp:ListItem id="optVisibility2" Value="T" />
                                                            <asp:ListItem id="optVisibility3" Value="A" />
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- =================== FINE ======================== --%>
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
                                    <asp:Panel ID="PnlLblIntervalYears" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <div class="p-padding-left">
                                                    <p>
                                                        <span class="weight">
                                                        <asp:Label runat="server" ID="searchDocumentLblIntervalYears"></asp:Label></span></p>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                                        <ContentTemplate>
                                            <asp:GridView ID="gridViewResult" runat="server" AllowSorting="true" AllowPaging="false"
                                                AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                                OnRowDataBound="gridViewResult_RowDataBound" OnSorting="gridViewResult_Sorting"
                                                OnPreRender="gridViewResult_PreRender" OnRowCreated="gridViewResult_ItemCreated"
                                                OnRowCommand="GridView_RowCommand">
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
            <cc1:CustomButton ID="SearchDocumentAdvancedSearch" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentAdvancedSearch_Click" />
            <cc1:CustomButton ID="SearchDocumentAdvancedSave" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentAdvancedSave_Click" />
            <cc1:CustomButton ID="SearchDocumentAdvancedEdit" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static"
                OnClick="SearchDocumentAdvancedEdit_Click" />
            <cc1:CustomButton ID="SearchDocumentAdvancedRemove" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" OnClick="SearchDocumentAdvancedRemove_Click"
                ClientIDMode="Static" />
            <cc1:CustomButton ID="SearchDocumentRemoveFilters" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchDocumentRemoveFilters_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:HiddenField ID="HiddenRemoveUsedSearch" runat="server" ClientIDMode="Static" />
            <asp:Button ID="btnCbSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="addAll_Click" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsAll" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
