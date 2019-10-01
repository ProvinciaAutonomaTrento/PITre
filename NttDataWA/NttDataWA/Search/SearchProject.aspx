<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchProject.aspx.cs"
    Inherits="NttDataWA.Search.SearchProjects" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="~/UserControls/SearchProjectsTabs.ascx" TagPrefix="uc2" TagName="SearchProjectsTabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
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
            min-width: 99%;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
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

            var searchText = $get('<%=TxtDescriptionProject.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeProject.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionProject.ClientID%>").value = descrizione;
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

        function collocazionePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoCollocazione');
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

        function collocazioneSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneCollocazione.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneCollocazione.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneCollocazione.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceCollocazione.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneCollocazione.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceCollocazione.ClientID%>', '');
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

        function cb_selectall() {
            $('#HiddenProjectsAll').val('true');
            $('#btnCbSelectAll').click();
        }

        function SetProjectCheck(obj, id) {
            $('#HiddenProjectsAll').val('');

            if (obj.checked) {
                var value = $('#HiddenProjectsChecked').val();
                var values = new Array(value);
                if (value.indexOf(',') >= 0) values = value.split(',');

                values.push(id);
                value = values.join(',');
                if (value.substring(0, 1) == ',')
                    value = value.substring(1);
                $('#HiddenProjectsChecked').val(value);
            }
            else {
                var value = $('#HiddenProjectsChecked').val();
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

                    value = $('#HiddenProjectsUnchecked').val();
                    values = new Array(value);
                    if (value.indexOf(',') >= 0) values = value.split(',');
                    values.push(id);

                    value = values.join(',');
                    if (value.substring(0, 1) == ',')
                        value = value.substring(1);
                    $('#HiddenProjectsUnchecked').val(value);
                }
                else {
                    value = values.join(',');
                    if (value.substring(0, 1) == ',')
                        value = value.substring(1);
                    $('#HiddenProjectsChecked').val(value);
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
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzata" runat="server" Url="../popup/GridPersonalization.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzataSave" runat="server" Url="../popup/GridPersonalizationSave.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GridPersonalizationPreferred" runat="server" Url="../popup/GridPersonalizationPreferred.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="SaveSearch" runat="server" Url="../Popup/SaveSearch.aspx" Width="650"
        Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ModifySearch" runat="server" Url="../Popup/SaveSearch.aspx?modify=true"
        Width="650" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup2 Id="MassiveAddAdlUser" runat="server" Url="../Popup/MassiveAddAdlUser.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlUser" runat="server" Url="../Popup/MassiveRemoveAdlUser.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveAddAdlRole" runat="server" Url="../Popup/MassiveAddAdlRole.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlRole" runat="server" Url="../Popup/MassiveRemoveAdlRole.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConservation" runat="server" Url="../Popup/MassiveConservation.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveTransmission" runat="server" Url="../Popup/MassiveTransmission.aspx?objType=P"
        IsFullScreen="true" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=fasc&fromMassiveOperation=1"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ChooseCorrespondent" runat="server" Url="../popup/ChooseCorrespondent.aspx"
        Width="700" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
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
                                <uc2:SearchProjectsTabs ID="SearchProjectsTabs" runat="server" PageCaller="PROJECT" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="colMassiveOperationSx">
                                            <asp:DropDownList runat="server" ID="SearchDocumentDdlMassiveOperation" Width="400"
                                                AutoPostBack="true" CssClass="chzn-select-deselect" OnSelectedIndexChanged="SearchDocumentDdlMassiveOperation_SelectedIndexChanged" />
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
                                    <asp:UpdatePanel ID="UpPnlSavedSearches" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder ID="plcSavedSearches" runat="server">
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="litSavedSearches" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <asp:DropDownList runat="server" ID="ddlSavedSearches" CssClass="chzn-select-deselect"
                                                            Width="100%" OnSelectedIndexChanged="ddlSavedSearches_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:PlaceHolder runat="server" ID="PlcAdl" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <asp:RadioButtonList runat="server" ID="RblTypeAdl" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="1"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!-- registro -->
                                    <asp:UpdatePanel ID="UpPnlRegistry" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder ID="plcRegistry" runat="server">
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="litRegistry" runat="server" /></strong></div>
                                                    <div class="col">
                                                        <asp:DropDownList runat="server" ID="DdlRegistries" 
                                                            Width="90" OnSelectedIndexChanged="DdlRegistries_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <!-- codice -->
                                    <div class="row">
                                        <div class="col-right-no-margin">
                                            <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"
                                                OnClientClick="return ajaxModalPopupOpenTitolario();" />
                                        </div>
                                    </div>
                                    <!-- titolario -->
                                    <asp:UpdatePanel ID="UpPnlTitolario" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder ID="plcTitolario" runat="server">
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="litTitolario" runat="server" /></strong></div>
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddlTitolario" runat="server" 
                                                            Width="300" />
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <!-- codice proj -->
                                    <div class="row">
                                        <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:PlaceHolder runat="server" ID="PnlProject">
                                                    <asp:HiddenField ID="IdProject" runat="server" />
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="litCodeProject" runat="server" /></strong></div>
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft"
                                                            OnTextChanged="TxtCodeProject_OnTextChanged" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf7">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_projectRight"
                                                                CssClassReadOnly="txt_ProjectRight_disabled" />
                                                        </div>
                                                    </div>
                                                  <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProject" TargetControlID="TxtDescriptionProject"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                                                        MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                                        OnClientPopulated="acePopulatedCod">
                                                    </uc1:AutoCompleteExtender>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- descrizione -->
                                    <div class="row">
                                        <asp:UpdatePanel ID="UpPnlDescription" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="colHalf">
                                                    <strong>
                                                        <asp:Literal ID="ProjectLitObject" runat="server"></asp:Literal></strong></div>
                                                <div class="colHalf2">
                                                    <cc1:CustomTextArea ID="TxtDescrizione" runat="server" CssClass="txt_input_full"
                                                        CssClassReadOnly="txt_input_full_disabled" /></div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- dd -->
                                    <asp:UpdatePanel ID="UpPnlGeneric" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="colHalf">
                                                    <strong>
                                                        <asp:Literal ID="litStatus" runat="server" /></strong></div>
                                                <div class="col">
                                                    <asp:DropDownList ID="ddlStatus" runat="server" Width="200">
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem id="opt_statusA" Value="A" />
                                                        <asp:ListItem id="opt_statusC" Value="C" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:PlaceHolder ID="plcType" runat="server" Visible="false">
                                                    <div class="col-no-margin">
                                                        <strong>
                                                            <asp:Literal ID="litType" runat="server" /></strong></div>
                                                    <div class="col-no-margin-top">
                                                        <asp:DropDownList ID="ddl_typeProject" runat="server" 
                                                            Width="150">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem id="opt_typeG" Value="G" />
                                                            <asp:ListItem id="opt_typeP" Value="P" Selected="True" />
                                                        </asp:DropDownList>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                            <div class="row">
                                                <div class="colHalf">
                                                    <strong>
                                                        <asp:Literal ID="litNum" runat="server" /></strong></div>
                                                <div class="col-no-margin">
                                                    <cc1:CustomTextArea ID="TxtNumProject" runat="server" CssClass="txt_textdata2"
                                                        CssClassReadOnly="txt_textdata_disabled2" />
                                                </div>
                                                <div class="col-no-margin">
                                                    <strong>
                                                        <asp:Literal ID="litYear" runat="server" /></strong></div>
                                                <div class="col-no-margin">
                                                    <cc1:CustomTextArea ID="TxtYear" runat="server" CssClass="txt_textdata2" CssClassReadOnly="txt_textdata_disabled2" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <asp:PlaceHolder runat="server" ID="PlcSubProject">
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="litSubset" runat="server" /></strong></div>
                                                    <div class="colHalf2">
                                                        <cc1:CustomTextArea ID="txt_subProject" runat="server" CssClass="txt_input_full"
                                                            CssClassReadOnly="txt_input_full_disabled" /></div>
                                                </asp:PlaceHolder>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                            <!-- intervals -->
                            <div class="row">
                                <fieldset>
                                    <h2 class="expand">
                                        <asp:Literal ID="litIntervals" runat="server" /></h2>
                                    <div class="collapse shown">
                                        <asp:UpdatePanel ID="upPnlIntervals" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <!-- Open -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaOpen" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col2">
                                                        <asp:DropDownList ID="ddl_dtaOpen" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaOpen_SelectedIndexChanged" >
                                                            <asp:ListItem id="dtaOpen_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaOpen_opt1" Value="1" />
                                                            <asp:ListItem id="dtaOpen_opt2" Value="2" />
                                                            <asp:ListItem id="dtaOpen_opt3" Value="3" />
                                                            <asp:ListItem id="dtaOpen_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaOpenFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaOpen_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaOpenTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaOpen_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Close -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaClose" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col2">
                                                        <asp:DropDownList ID="ddl_dtaClose" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaClose_SelectedIndexChanged" >
                                                            <asp:ListItem id="dtaClose_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaClose_opt1" Value="1" />
                                                            <asp:ListItem id="dtaClose_opt2" Value="2" />
                                                            <asp:ListItem id="dtaClose_opt3" Value="3" />
                                                            <asp:ListItem id="dtaClose_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCloseFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaClose_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCloseTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaClose_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Create -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaCreate" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col2">
                                                        <asp:DropDownList ID="ddl_dtaCreate" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaCreate_SelectedIndexChanged" >
                                                            <asp:ListItem id="dtaCreate_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaCreate_opt1" Value="1" />
                                                            <asp:ListItem id="dtaCreate_opt2" Value="2" />
                                                            <asp:ListItem id="dtaCreate_opt3" Value="3" />
                                                            <asp:ListItem id="dtaCreate_opt4" Value="4" />
                                                            <asp:ListItem id="dtaCreate_opt5" Value="5" />
                                                            <asp:ListItem id="dtaCreate_opt6" Value="6" />
                                                            <asp:ListItem id="dtaCreate_opt7" Value="7" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCreateFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaCreate_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCreateTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaCreate_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Expire -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaExpire" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col2">
                                                        <asp:DropDownList ID="ddl_dtaExpire" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaExpire_SelectedIndexChanged" >
                                                            <asp:ListItem id="dtaExpire_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaExpire_opt1" Value="1" />
                                                            <asp:ListItem id="dtaExpire_opt2" Value="2" />
                                                            <asp:ListItem id="dtaExpire_opt3" Value="3" />
                                                            <asp:ListItem id="dtaExpire_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaExpireFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaExpire_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaExpireTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaExpire_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                            </div>
                            <div class="row">
                                <!-- creeator/owner -->
                                <fieldset class="basic">
                                    <h2 class="expand">
                                        <asp:Literal ID="litOwnerAuthor" runat="server" /></h2>
                                    <div class="collapse" id="PnlCreator" runat="server">
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
                                                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
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
                                                        OnClientPopulated="creatorePopulated" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                                <div class="row">
                                                    <div class="col-right">
                                                        <asp:CheckBox ID="chkCreatoreExtendHistoricized" runat="server" Checked="true" />
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
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
                                                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
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
                                                        OnClientPopulated="proprietarioPopulated" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                            </div>
                            <div class="row">
                                <!-- note -->
                                <fieldset class="basic">
                                    <asp:UpdatePanel ID="UpPnlNote" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:RadioButtonList ID="rblFilterNote" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList" OnSelectedIndexChanged="RblTypeNote_SelectedIndexChanged" AutoPostBack="True">
                                                        <asp:ListItem id="optNoteAny" runat="server" Value="Q" Selected="True" />
                                                        <asp:ListItem id="optNoteAll" runat="server" Value="T" />
                                                        <asp:ListItem id="optNoteRole" runat="server" Value="R" />
                                                        <asp:ListItem id="optNoteRF" runat="server" Value="F" />
                                                        <asp:ListItem id="optNotePersonal" runat="server" Value="P" />
                                                    </asp:RadioButtonList>
                                                </div>
                                                <div class="col-right-no-margin-alLeft">
                                                    <asp:DropDownList Visible="false" ID="ddlNoteRF" runat="server" 
                                                        Width="150px">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="colHalf">
                                                    <strong>
                                                        <asp:Literal ID="litNotes" runat="server" /></strong></div>
                                                <div class="colHalf2">
                                                    <cc1:CustomTextArea ID="TxtNoteProject" runat="server" CssClass="txt_input_full"
                                                        CssClassReadOnly="txt_input_full_disabled" ClientIDMode="Static" Width="99%"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
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
                                <fieldset>
                                    <h2 class="expand">
                                        <asp:Literal ID="litCollocation" runat="server" /></h2>
                                    <div class="collapse" runat="server" id="PnlCollocation">
                                        <asp:UpdatePanel ID="upPnlCollocationAddr" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <asp:HiddenField ID="idCollocationAddr" runat="server" />
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="litCollocationAddr" runat="server" /></strong></div>
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="txtCodiceCollocazione" runat="server" CssClass="txt_addressBookLeft"
                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                            AutoCompleteType="Disabled">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf7">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="txtDescrizioneCollocazione" runat="server" CssClass="txt_projectRight"
                                                                CssClassReadOnly="txt_ProjectRight_disabled" Style="width: 90%; float: left;">
                                                            </cc1:CustomTextArea>
                                                            <cc1:CustomImageButton runat="server" ID="ImgCollocazioneAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnClick="ImgCollocazioneAddressBook_Click" />
                                                        </div>
                                                    </div>
                                                    <uc1:AutoCompleteExtender runat="server" ID="RapidCollocazione" TargetControlID="txtDescrizioneCollocazione"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="collocazioneSelected" BehaviorID="AutoCompleteExIngressoCollocazione"
                                                        OnClientPopulated="collocazionePopulated" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel ID="upPnlCollocation" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <!-- Open -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaCollocation" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col2">
                                                        <asp:DropDownList ID="ddl_dtaCollocation" runat="server" Width="130" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaCollocation_SelectedIndexChanged" >
                                                            <asp:ListItem id="dtaCollocation_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaCollocation_opt1" Value="1" />
                                                            <asp:ListItem id="dtaCollocation_opt2" Value="2" />
                                                            <asp:ListItem id="dtaCollocation_opt3" Value="3" />
                                                            <asp:ListItem id="dtaCollocation_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCollocationFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaCollocation_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCollocationTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaCollocation_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                            </div>
                            <asp:PlaceHolder runat="server" ID="phConservation">
                            <div class="row">
                                <fieldset>
                                    <asp:UpdatePanel ID="UpPnlConservation" runat="server" UpdateMode="Conditional" class="row">
                                        <ContentTemplate>
                                            <asp:PlaceHolder runat="server" ID="PlcPreservation">
                                                <asp:CheckBox ID="chkConservation" runat="server" OnCheckedChanged="chkConservation_CheckedChanged"
                                                    AutoPostBack="true" />
                                                <asp:CheckBox ID="chkConservationNo" runat="server" OnCheckedChanged="chkConservation_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                            </asp:PlaceHolder>
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
                            <div class="row">
                                <fieldset>
                                    <asp:UpdatePanel ID="UpPnlViewAll" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="col">
                                                <asp:Literal ID="litViewAll" runat="server" /></div>
                                            <div class="col">
                                                <asp:RadioButton ID="rbViewAllYes" runat="server" OnCheckedChanged="rbViewAll_CheckedChanged"
                                                    AutoPostBack="true" Checked="true" />
                                                <asp:RadioButton ID="rbViewAllNo" runat="server" OnCheckedChanged="rbViewAll_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
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
                                                        <asp:Literal runat="server" ID="SearchProjectLitNomeGriglia" />
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
            <cc1:CustomButton ID="SearchProjectSearch" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectSearch_Click" />
            <cc1:CustomButton ID="SearchProjectSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectSave_Click" />
            <cc1:CustomButton ID="SearchProjectEdit" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Enabled="false" OnClick="SearchProjectEdit_Click" />
            <cc1:CustomButton ID="SearchProjectRemove" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Enabled="false" OnClick="SearchProjectRemove_Click" />
            <cc1:CustomButton ID="SearchProjectRemoveFilters" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectRemoveFilters_Click" />
            <cc1:CustomButton ID="SearchProjectNewProject" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectNewProject_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnCbSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="addAll_Click" />
            <asp:HiddenField ID="HiddenRemoveUsedSearch" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenProjectsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenProjectsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenProjectsAll" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });
        });
    </script>
</asp:Content>
