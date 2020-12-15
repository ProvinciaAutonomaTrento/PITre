<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="Project.aspx.cs" Inherits="NttDataWA.Project.Project" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/SelectAllCheck.ascx" TagPrefix="uc" TagName="SelectAllCheck" %>
<%@ Register Src="~/UserControls/HeaderProject.ascx" TagPrefix="uc1" TagName="HeaderProject" %>
<%@ Register Src="~/UserControls/ProjectTabs.ascx" TagPrefix="uc2" TagName="ProjectTabs" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <link href="../Css/bootstrap.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
    <style type="text/css">
        .jstree-last a, .jstree-leaf a
        {
            /*width: 400px; display: block; overflow: hidden; text-overflow: ellipsis;*/
        }
        
        .gridViewResult, .gridViewResult th:last-child, .gridViewResult td:last-child
        {
            border-right: 0;
        }
        
        .jstree-draggable
        {
            cursor: pointer;
        }
        
        .tbl_rounded tr.jstree-draggable-selected td, .tbl_rounded tr.jstree-draggable-selected td, .tbl_rounded tr.jstree-draggable-selected:hover td, .tbl_rounded tr.jstree-draggable-selected:hover td
        {
            background: #FFFC52;
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
        
        #tree
        {
            overflow: auto;
        }
        #tree a
        {
            max-width: 400px;
            text-overflow: ellipsis;
        }
        
        #contentDx
        {
            float: right;
            width: 60%;
            height: 100%;
            overflow: hidden;
            background-color: #eeeeee;
        }
        
        
        .containerGridView
        {
            height: 80%;
            overflow: auto;
        }
    </style>
    <script type="text/javascript">

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

        function noteSelected(sender, e) {
            var value = e.get_value();
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


            var searchText = $get('<%=TxtNote.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceDescrizione = testo.lastIndexOf('[');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            document.getElementById("<%=this.TxtNote.ClientID%>").focus();
            document.getElementById("<%=TxtNote.ClientID%>").value = descrizione;
            document.getElementById("<%=txtNoteAutoComplete.ClientID %>").value = '';

            var tmp = document.getElementById("<%=TxtNote_chars.ClientID %>").getAttribute("rel");
            charsLeft(tmp.split('_')[0], tmp.split('_')[1], tmp.split('_')[2]);

            var codiceRF = testo.substring(testo.lastIndexOf(" ["));
            codiceRF = codiceRF.substring(2, codiceRF.lastIndexOf("]"))
            if (codiceRF == "TUTTI") {
                $('ul.RblTypeNote input')[$('ul.RblTypeNote input').length - 1].checked = true;
                __doPostBack('<%=this.RblTypeNote.ClientID%>', '');
            }
            else {
                $('ul.RblTypeNote input')[$('ul.RblTypeNote input').length - 2].checked = true;
            }
        }


        function collPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoBISColl');
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

        function collSelected(sender, e) {
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

            var searchText = $get('<%=projectTxtDescrizioneCollocazione.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.projectTxtDescrizioneCollocazione.ClientID%>").focus();
            document.getElementById("<%=this.projectTxtDescrizioneCollocazione.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.projectTxtCodiceCollocazione.ClientID%>").value = codice;
            document.getElementById("<%=projectTxtDescrizioneCollocazione.ClientID%>").value = descrizione;

            __doPostBack('<%=this.projectTxtCodiceCollocazione.ClientID%>', '');
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
        function swapProjectPrivatePublic(id) {
            if (id == '<%=ProjectCheckPrivate.ClientID%>' && $get('<%=ProjectCheckPrivate.ClientID%>').checked) {
                $get('<%=ProjectCheckPublic.ClientID%>').checked = false;
            }
            else {
                $get('<%=ProjectCheckPrivate.ClientID%>').checked = false;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="MassiveSignatureHSM" runat="server" Url="../Popup/MassiveHSM_Signature.aspx?objType=D"
        Width="700" Height="400" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('UpContainerProjectTab', ''); }" />
    <uc:ajaxpopup2 Id="Note" runat="server" Url="../popup/Note.aspx?type=F" Width="800"
        Height="600" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) { __doPostBack('UpPnlNote', '');}" />
    <uc:ajaxpopup2 Id="OpenAddDoc" runat="server" Url="../popup/AddDocInProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpContainerProjectTab', '');}" />
    <uc:SelectAllCheck ID="SelectAllCheck" runat="server" Grid="gridViewResult" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzata" runat="server" Url="../popup/GridPersonalization.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GrigliaPersonalizzataSave" runat="server" Url="../popup/GridPersonalizationSave.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="GridPersonalizationPreferred" runat="server" Url="../popup/GridPersonalizationPreferred.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="800" Height="600"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="AddFilterProject" runat="server" Url="../popup/AddFilterProject.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="SearchSubset" runat="server" Url="../popup/ProjectSearchSubset.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="CreateNode" runat="server" Url="../popup/ProjectDataentryNode.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="350"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ModifyNode" runat="server" Url="../popup/ProjectDataentryNode.aspx?t=modify"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="HistoryProject" runat="server" Url="../Popup/HistoryProject.aspx"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="1000" Height="800"
        CloseFunction="function (event, ui) {__doPostBack('UpContainerProjectTab', '');}" />
    <uc:ajaxpopup2 Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup2 Id="MassiveAddAdlUser" runat="server" Url="../Popup/MassiveAddAdlUser.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlUser" runat="server" Url="../Popup/MassiveRemoveAdlUser.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveAddAdlRole" runat="server" Url="../Popup/MassiveAddAdlRole.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlRole" runat="server" Url="../Popup/MassiveRemoveAdlRole.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConservation" runat="server" Url="../Popup/MassiveConservation.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveVersPARER" runat="server" Url="../Popup/MassiveVers.aspx?isParer=true"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveTransmission" runat="server" Url="../Popup/MassiveTransmission.aspx?objType=D&docOrFasc=docInfasc"
        IsFullScreen="true" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConversion" runat="server" Url="../Popup/MassivePdfConversion.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveTimestamp" runat="server" Url="../Popup/MassiveTimestamp.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConsolidation" runat="server" Url="../Popup/MassiveConsolidation.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveConsolidationMetadati" runat="server" Url="../Popup/MassiveConsolidation.aspx?objType=D&metadati=true&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveForward" runat="server" Url="../Popup/MassiveForward.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveCollate" runat="server" Url="../Popup/MassiveCollate.aspx?docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveVersions" runat="server" Url="../Popup/MassiveRemoveVersions.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignature" runat="server" Url="../Popup/MassiveDigitalSignature.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignatureApplet" runat="server" Url="../Popup/MassiveDigitalSignature_applet.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignatureSocket" runat="server" Url="../Popup/MassiveDigitalSignature_socket.aspx?objType=D&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=doc&fromMassiveOperation=1&docOrFasc=docInfasc"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolarioMassive" runat="server" Url="../Popup/ClassificationScheme.aspx?from=search&massive=true&docOrFasc=docInfasc"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="SearchProjectMassive" runat="server" Url="../popup/SearchProject.aspx?caller=search"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ExportDocument" runat="server" Url="ImportExport/ExportDocumentActiveX.aspx"
        Width="610" Height="380" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'up'); }" />
    <uc:ajaxpopup2 Id="ImportDocument" runat="server" Url="ImportExport/ImportDocumentActiveX.aspx"
        Width="600" Height="400" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeImportDocument'); }" />
    <uc:ajaxpopup2 Id="ExportDocumentApplet" runat="server" Url="ImportExport/ExportDocumentApplet.aspx"
        Width="650" Height="400" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'up'); }" />
    <uc:ajaxpopup2 Id="ImportDocumentApplet" runat="server" Url="ImportExport/ImportDocumentApplet.aspx"
        Width="650" Height="450" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeImportDocument'); }" />
    <uc:ajaxpopup2 Id="ExportDocumentSocket" runat="server" Url="ImportExport/ExportDocumentSocket.aspx"
        Width="650" Height="400" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'up'); }" />
    <uc:ajaxpopup2 Id="ImportDocumentSocket" runat="server" Url="ImportExport/ImportDocumentSocket.aspx"
        Width="650" Height="450" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeImportDocument'); }" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="OpenAddDocCustom" runat="server" Url="../popup/AddDocInProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui)  {$('#btnLinkCustom').click();}" />
    <uc:ajaxpopup2 Id="SearchProjectCustom" runat="server" Url="../popup/SearchProject.aspx?caller=custom"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui)  {$('#btnLinkCustom').click();}" />
    <uc:ajaxpopup2 Id="ReportFrame" runat="server" Url="ImportExport/Import/visPdfReportFrame.aspx"
        IsFullScreen="true" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui)  {}" />
    <uc:ajaxpopup2 Id="Prints" runat="server" Url="../popup/visualReport_iframe.aspx"
        Width="400" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ChooseCorrespondent" runat="server" Url="../popup/ChooseCorrespondent.aspx"
        Width="700" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="StartProcessSignature" runat="server" Url="../popup/StartProcessSignature.aspx?from=SearchDocument"
        Width="1200" Height="800" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="InfoSignatureProcessesStarted" runat="server" Url="../popup/InfoSignatureProcessesStarted.aspx"
        PermitClose="false" PermitScroll="false" Width="600" Height="300" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="CreateNewDocument" runat="server" Url="../popup/CreateNewDocument.aspx"
        PermitClose="false" PermitScroll="false" Width="650" Height="400" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closePopupCreateNewDocument');}" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closePopupObject');}" />
    <uc:ajaxpopup2 Id="DetailsLFAutomaticMode" runat="server" Url="../popup/DetailsLFAutomaticMode.aspx"
        Width="750" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'SignatureProcessConcluted');}" />
    <uc:ajaxpopup2 Id="MassiveReportDragAndDrop" runat="server" Url="../popup/MassiveReportDragAndDrop.aspx"
        Width="700" Height="500" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'POPUP_DRAG_AND_DROP');}" />
    <uc:ajaxpopup2 Id="DescriptionProjectList" runat="server" Url="../Popup/DescriptionProjectList.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="MassiveTransmissionAccept" runat="server" Url="../popup/MassiveTransmissionAccept.aspx?type=f"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="1000" Height="900"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <uc1:HeaderProject ID="HeaderProject" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerProjectTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:ProjectTabs ID="ProjectTabs" runat="server" PageCaller="PROJECT" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <div class="colImg">
                                <cc1:CustomImageButton ID="projectImgAddDoc" ImageUrl="../Images/Icons/add_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/add_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/add_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/add_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupOpenAddDoc();" />
                                <cc1:CustomImageButton ID="projectImgNewDocument" ImageUrl="../Images/Icons/create_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/create_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/create_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/create_doc_in_project_disabled.png" Enabled="true"
                                    OnClientClick="return ajaxModalPopupCreateNewDocument();" />
                                <cc1:CustomImageButton ID="projectImgImportDoc" ImageUrl="../Images/Icons/import_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/import_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/import_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/import_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupImportDocument();" />
                                <cc1:CustomImageButton ID="projectImgExportDoc" ImageUrl="../Images/Icons/export_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/export_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/export_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/export_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupExportDocument();" />
                                <cc1:CustomImageButton ID="projectImgImportDocApplet" ImageUrl="../Images/Icons/import_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/import_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/import_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/import_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupImportDocumentApplet();" />
                                <cc1:CustomImageButton ID="projectImgExportDocApplet" ImageUrl="../Images/Icons/export_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/export_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/export_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/export_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupExportDocumentApplet();" />
                                <cc1:CustomImageButton ID="projectImgImportDocSocket" ImageUrl="../Images/Icons/import_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/import_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/import_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/import_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupImportDocumentSocket();" />
                                <cc1:CustomImageButton ID="projectImgExportDocSocket" ImageUrl="../Images/Icons/export_doc_in_project.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/export_doc_in_project_hover.png"
                                    OnMouseOutImage="../Images/Icons/export_doc_in_project.png" CssClass="clickable"
                                    ImageUrlDisabled="../Images/Icons/export_doc_in_project_disabled.png" Enabled="false"
                                    OnClientClick="return ajaxModalPopupExportDocumentSocket();" />
                                <cc1:CustomImageButton ID="projectImgAddFilter" ImageUrl="../Images/Icons/add_filters.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/add_filters_hover.png" OnMouseOutImage="../Images/Icons/add_filters.png"
                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/add_filters_disabled.png"
                                    Enabled="false" OnClientClick="return ajaxModalPopupAddFilterProject();" />
                                <cc1:CustomImageButton ID="projectImgRemoveFilter" ImageUrl="../Images/Icons/remove_filters.png"
                                    runat="server" OnMouseOverImage="../Images/Icons/remove_filters_hover.png" OnMouseOutImage="../Images/Icons/remove_filters.png"
                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/remove_filters_disabled.png"
                                    Enabled="false" OnClick="projectImgRemoveFilter_Click" />
                            </div>
                        </div>
                    </div>
                    <div id="containerProjectTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerProject" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <a name="anchorTop"></a>
                        <div class="box_inside">
                            <asp:UpdatePanel runat="server" ID="UpPnlRegistry">
                                <ContentTemplate>
                                    <div class="row">
                                        <div id="PnlRegistryPrj">
                                            <div class="col1Prj">
                                                <strong>
                                                    <asp:Label runat="server" ID="projectLblRegistro"></asp:Label></strong>
                                            </div>
                                            <div class="col">
                                                <asp:DropDownList runat="server" ID="projectDdlRegistro" Width="200" AutoPostBack="true"
                                                    CssClass="chzn-select-deselect" OnSelectedIndexChanged="projectDdlRegistro_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-right-no-margin2">
                                                <asp:UpdatePanel runat="server" ID="UpProjectPrivate" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:CheckBox ID="ProjectCheckPrivate" runat="server" CssClass="clickableLeftN" onclick="swapProjectPrivatePublic(this.id)" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <div class="col-right-no-margin2">
                                                <asp:UpdatePanel runat="server" ID="UpProjectPublic" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:CheckBox ID="ProjectCheckPublic" runat="server" CssClass="clickableLeftN" Checked="false" Visible="false" onclick="swapProjectPrivatePublic(this.id)" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpPnlDateProject">
                                <ContentTemplate>
                                    <div class="col14">   
                                            <asp:UpdatePanel ID="UpDirittiFascicolo" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Panel ID="PnlDirittiFascicolo" Visible="false" runat="server">
                                                        <span class="weight">
                                                            <asp:Label ID="LblDiritti" runat="server"></asp:Label></span>
                                                        <asp:Label ID="LblTipoDiritto" runat="server"></asp:Label>
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>                        
                                    </div>
                                    <div class="row">
                                        <div id="dateProject">
                                            <div class="row">
                                                <div class="col">
                                                    <strong>
                                                        <asp:Label runat="server" ID="projectLblDataApertura"></asp:Label></strong><asp:Label
                                                            runat="server" ID="projectLblDataAperturaGenerata"></asp:Label>
                                                </div>
                                                <div class="col4-right">
                                                    <strong>
                                                        <asp:Label runat="server" ID="projectlblDataChiusura"></asp:Label></strong><asp:Label
                                                            runat="server" ID="projectlblDataChiusuraGenerata"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="upnBtnTitolario" UpdateMode="Conditional" ClientIDMode="Static">
                                    <ContentTemplate>
                                        <fieldset>
                                            <asp:PlaceHolder runat="server" ID="PhlTitolario">
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Label runat="server" ID="projectlblCodiceClassificazione"></asp:Label></span>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                            OnMouseOutImage="../Images/Icons/classification_scheme.png" OnClientClick="return ajaxModalPopupOpenTitolario();"
                                                            CssClass="clickable" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <asp:HiddenField runat="server" ID="IdProject" />
                                                        <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft"
                                                            OnTextChanged="TxtCodeProject_OnTextChanged" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf2">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_addressBookRight"
                                                                CssClassReadOnly="txt_addressBookRight_disabled"></cc1:CustomTextArea>
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
                                                </div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" ID="PnlDescription">
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Label runat="server" ID="projectLblDescrizione"> </asp:Label></span><span class="little">*</span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="ProjectImgObjectHistory" ImageUrl="../Images/Icons/obj_history_big.png"
                                                            OnMouseOutImage="../Images/Icons/obj_history_big.png" OnMouseOverImage="../Images/Icons/obj_history_big_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_history_big_disabled.png"
                                                            OnClick="ProjectImgObjectHistory_Click" Visible="false" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <cc1:CustomTextArea ID="projectTxtDescrizione" runat="server" TextMode="MultiLine"
                                                        CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static"
                                                        Columns="5000"></cc1:CustomTextArea>
                                                </div>
                                                <div class="row">
                                                    <div class="col-right-no-margin">
                                                        <span class="charactersAvailable">
                                                            <asp:Literal ID="projectLtrDescrizione" runat="server" ClientIDMode="Static"> </asp:Literal>
                                                            <span id="projectTxtDescrizione_chars" clientidmode="Static" runat="server"></span>
                                                        </span>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </fieldset>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <!-- tree -->
                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="upnlStruttura" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder runat="server" ID="PlcStructur" Visible="false">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:UpdatePanel ID="upPnlStruttura" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal ID="litStruttura" runat="server" />
                                                                        <!--<asp:Literal ID="docsInFolderCount" runat="server" />-->
                                                                    </span>
                                                                </p>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <div class="linkTree">
                                                            <a href="#expand" id="expand_all">
                                                                <asp:Literal ID="litTreeExpandAll" runat="server" /></a> <a href="#collapse" id="collapse_all">
                                                                    <asp:Literal ID="litTreeCollapseAll" runat="server" /></a>
                                                        </div>
                                                            <cc1:CustomImageButton ID="ImgFolderSearch" runat="server" ClientIDMode="Static"
                                                                ImageUrl="../Images/Icons/search_projects.png" CssClass="clickable" OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                                                OnMouseOutImage="../Images/Icons/search_projects.png" OnClientClick="return ajaxModalPopupSearchSubset();" />
                                                            <cc1:CustomImageButton ID="ImgFolderAdd" runat="server" ClientIDMode="Static" ImageUrl="../Images/Icons/add_sub_folder.png"
                                                                CssClass="clickable" OnMouseOverImage="../Images/Icons/add_sub_folder_hover.png"
                                                                OnMouseOutImage="../Images/Icons/add_sub_folder.png" ImageUrlDisabled="../Images/Icons/add_sub_folder_disabeld.png" />
                                                            <cc1:CustomImageButton ID="ImgFolderModify" runat="server" ClientIDMode="Static"
                                                                ImageUrl="../Images/Icons/edit_project.png" CssClass="clickable" OnMouseOverImage="../Images/Icons/edit_project_hover.png"
                                                                OnMouseOutImage="../Images/Icons/edit_project.png" ImageUrlDisabled="../Images/Icons/edit_project_disabeld.png" />
                                                            <cc1:CustomImageButton ID="ImgFolderRemove" runat="server" ClientIDMode="Static"
                                                                ImageUrl="../Images/Icons/remove_sub_folder.png" CssClass="clickable" OnMouseOverImage="../Images/Icons/remove_sub_folder_hover.png"
                                                                OnMouseOutImage="../Images/Icons/remove_sub_folder.png" ImageUrlDisabled="../Images/Icons/remove_sub_folder_disabeld.png" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div id="tree">
                                                    </div>
                                                    <p id="log2">
                                                    </p>
                                                    <asp:HiddenField ID="treenode_sel" runat="server" ClientIDMode="Static" />
                                                    <asp:Button ID="BtnChangeSelectedFolder" runat="server" CssClass="hidden" OnClick="BtnChangeSelectedFolder_Click"
                                                        ClientIDMode="Static" />
                                                    <asp:Button ID="BtnRebindGrid" runat="server" CssClass="hidden" OnClick="BtnRebindGrid_Click"
                                                        ClientIDMode="Static" />
                                                </div>
                                            </fieldset>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <!-- Note -->
                            <div class="row">
                                <fieldset>
                                    <asp:UpdatePanel runat="server" ID="UpPnlNote" UpdateMode="Conditional" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col" style="width: 77%;">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="projectLitVisibleNotes" /><asp:Literal runat="server"
                                                                ID="projectLitNoteAuthor" /></span></p>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <cc1:CustomImageButton runat="server" ID="projectImgNotedetails" ImageUrl="../Images/Icons/ico_objects.png"
                                                        OnMouseOutImage="../Images/Icons/ico_objects.png" OnMouseOverImage="../Images/Icons/ico_objects.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
                                                        OnClientClick="return ajaxModalPopupNote();" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:RadioButtonList ID="RblTypeNote" runat="server" AutoPostBack="true" CssClass="rblHorizontal RblTypeNote"
                                                        OnSelectedIndexChanged="RblTypeNote_SelectedIndexChanged" RepeatLayout="UnorderedList">
                                                        <asp:ListItem Value="Personale" id="projectItemNotePersonal"></asp:ListItem>
                                                        <asp:ListItem Value="Ruolo" id="projectItemNoteRole"></asp:ListItem>
                                                        <asp:ListItem Value="RF" id="projectItemNoteRF"></asp:ListItem>
                                                        <asp:ListItem Value="Tutti" id="projectItemNoteAll" Selected="true"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                                <div class="col-right-no-margin-alLeft">
                                                    <asp:DropDownList Visible="false" ID="ddlNoteRF" runat="server" CssClass="chzn-select-deselect"
                                                        Width="150px" AutoPostBack="true" OnSelectedIndexChanged="ddlNoteRF_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <cc1:CustomTextArea ID="txtNoteAutoComplete" Visible="false" Width="100%" runat="server"
                                                    CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                                                <asp:HiddenField ID="isTutti" runat="server" Value="" />
                                                <uc1:AutoCompleteExtender runat="server" ID="autoComplete1" TargetControlID="txtNoteAutoComplete"
                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaNote"
                                                    MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                    OnClientItemSelected="noteSelected">
                                                </uc1:AutoCompleteExtender>
                                            </div>
                                            <div class="row">
                                                <div class="col-full">
                                                    <cc1:CustomTextArea ID="TxtNote" runat="server" TextMode="MultiLine" ClientIDMode="static"
                                                        CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-right-no-margin">
                                                    <span class="charactersAvailable">
                                                        <asp:Literal ID="projectLitVisibleNotesChars" runat="server" ClientIDMode="static"></asp:Literal>
                                                        <span id="TxtNote_chars" runat="server" clientidmode="static"></span></span>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                            <asp:PlaceHolder runat="server" ID="PnlRapidTransmission">
                                <div class="row">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="ProjectLitTransmRapid" runat="server"></asp:Literal></span></p>
                                            </div>
                                        </div>
                                        <asp:UpdatePanel runat="server" ID="UpPnlTransmissionsModel" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <p>
                                                            <div class="styled-select_full">
                                                                <asp:DropDownList ID="ProjectDdlTransmissionsModel" runat="server" CssClass="chzn-select-deselect"
                                                                    Width="100%">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </p>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                            </asp:PlaceHolder>
                            <div class="row">
                                <asp:UpdatePanel ID="UpPanelTipologiaFascicolo" runat="server">
                                    <ContentTemplate>
                                        <fieldset class="azure">
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight"><a name="anchorType">
                                                            <asp:Literal ID="projectLblTipoFascicolo" runat="server"></asp:Literal></a></span></p>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <cc1:CustomImageButton runat="server" ID="ProjectImgHistoryTipology" ImageUrl="../Images/Icons/obj_history.png"
                                                        OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                        OnClick="ProjectImgHistory_Click" Visible="false" />
                                                </div>
                                                <asp:UpdatePanel runat="server" ID="UpConfirmStateDiagram" UpdateMode="Conditional"
                                                    ClientIDMode="Static">
                                                    <ContentTemplate>
                                                        <asp:HiddenField runat="server" ID="HiddenAutomaticState" ClientIDMode="Static" />
                                                        <asp:HiddenField runat="server" ID="HiddenFinalState" ClientIDMode="Static" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <p>
                                                                <div class="styled-select_full">
                                                                    <asp:DropDownList ID="projectDdlTipologiafascicolo" runat="server" OnSelectedIndexChanged="ProjectDdlTypeDocument_OnSelectedIndexChanged"
                                                                        AutoPostBack="True" CssClass="chzn-select-deselect" Width="100%">
                                                                        <asp:ListItem Text=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal ID="DocumentLitStateDiagram" runat="server"></asp:Literal>&nbsp;
                                                                        <asp:Literal ID="LitActualStateDiagram" runat="server"></asp:Literal></span></p>
                                                            </div>
                                                            <div class="col-right-no-margin">
                                                                <cc1:CustomImageButton runat="server" ID="ProjectImgHistoryState" ImageUrl="../Images/Icons/obj_history.png"
                                                                    OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                                    OnClick="ProjectImgHistory_Click" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <p>
                                                                    <div class="styled-select_full">
                                                                        <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" CssClass="chzn-select-deselect"
                                                                            Width="100%">
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <asp:UpdatePanel runat="server" ID="UpPnlScadenza" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <asp:PlaceHolder runat="server" ID="PnlDocumentStateDiagramDate" Visible="false">
                                                                        <div class="col">
                                                                            <span class="weight">
                                                                                <asp:Literal ID="DocumentDateStateDiagram" runat="server"></asp:Literal>
                                                                            </span>
                                                                        </div>
                                                                        <div class="col">
                                                                            <asp:Panel runat="server" ID="PnlScadenza" Visible="false" ClientIDMode="Static">
                                                                                <cc1:CustomTextArea runat="server" ID="DocumentStateDiagramDataValue" CssClass="txt_textdata datepicker"
                                                                                    CssClassReadOnly="txt_textdata_disabled">
                                                                                </cc1:CustomTextArea>
                                                                            </asp:Panel>
                                                                        </div>
                                                                    </asp:PlaceHolder>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PnlTypeDocument" runat="server"></asp:PlaceHolder>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </fieldset>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <!--fine tipologia fascicolo-->
                            <div class="row">
                                <fieldset>
                                    <asp:UpdatePanel runat="server" ID="UpProjectPhisycCollocation" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Panel runat="server" ID="PnlPhisyCollocation">
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Label runat="server" ID="projectLblCollocazioneFisica"></asp:Label></span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgCollocationAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                            OnClick="DocumentImgCollocationAddressBook_Click" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <asp:HiddenField runat="server" ID="idProjectCollocation" />
                                                        <cc1:CustomTextArea ID="projectTxtCodiceCollocazione" runat="server" CssClass="txt_addressBookLeft"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true" OnTextChanged="TxtCodeCollocation_OnTextChanged"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf2">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="projectTxtDescrizioneCollocazione" runat="server" CssClass="txt_addressBookRight"
                                                                CssClassReadOnly="txt_addressBookRight_disabled"></cc1:CustomTextArea>
                                                            <uc1:AutoCompleteExtender runat="server" ID="RapidCollocazione" TargetControlID="projectTxtDescrizioneCollocazione"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="collSelected" BehaviorID="AutoCompleteExIngressoBISColl"
                                                                OnClientPopulated="collPopulated ">
                                                            </uc1:AutoCompleteExtender>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Label runat="server" ID="projectLblDataCollocazioneFisica"></asp:Label></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <cc1:CustomTextArea ID="projectTxtdata" runat="server" CssClass="txt_textdata datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col-right">
                                                        <asp:CheckBox ID="projectCkCartaceo" runat="server" />
                                                        <asp:Label runat="server" ID="projectLblCartaceo"></asp:Label>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                                <br />
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <div id="contentDxTopProject">
                            <%--Azioni massive --%>
                            <asp:UpdatePanel ID="UpnlFasiDiagrammaStato" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlFasiDiagrammaStato" runat="server" Visible="false">
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="rowMassiveOperation">
                                        <div class="col">
                                            <asp:DropDownList runat="server" ID="SearchDocumentDdlMassiveOperation" Width="400"
                                                AutoPostBack="true" CssClass="chzn-select-deselect" OnSelectedIndexChanged="SearchDocumentDdlMassiveOperation_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-right-no-margin">
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
                            <%--Label numero di elementi --%>
                            <asp:UpdatePanel runat="server" ID="UpnlNumerodocumenti" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <strong>
                                                    <asp:Literal runat="server" ID="projectLitNomeGriglia" />
                                                    <asp:Label runat="server" ID="projectLblDocumentiFascicoliCount"></asp:Label></strong>
                                                <asp:Label runat="server" ID="projectLblNumeroDocumenti"></asp:Label></p>
                                        </div>
                                    </div>
                                    <br />
                                    <br />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="containerGridView">
                            <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                                <ContentTemplate>
                                    <asp:GridView ID="gridViewResult" runat="server" AllowSorting="true" AllowPaging="false"
                                        AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                        HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                        OnRowDataBound="gridViewResult_RowDataBound" OnSorting="gridViewResult_Sorting"
                                        OnPreRender="gridViewResult_PreRender" OnRowCreated="gridViewResult_ItemCreated"
                                        OnRowCommand="GridView_RowCommand">
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
                        <span style="position: fixed"></span>
                    </div>
                    <!-- end of container -->
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="UploadLiveuploads"  runat="server" Visible="false">
        <div class="upload-dialog" id="upload-liveuploads" data-bind="template: { name: 'template-uploads' }"></div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="projectBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="projectBtnSave_Click" />
            <cc1:CustomButton ID="projectBtnSaveAndAcceptDocument" runat="server" CssClass="btnEnable clickableRight"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="projectBtnSaveAndAcceptDocument_Click"
                Visible="false" />
            <cc1:CustomButton ID="projectBntChiudiFascicolo" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="projectBntChiudiFascicolo_Click" />
            <cc1:CustomButton ID="projectBntCancellaFascicolo" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="projectBntCancellaFascicolo_Click" />
            <cc1:CustomButton ID="projectBtnAdL" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="projectBtnAdL_Click" />
            <cc1:CustomButton ID="projectBtnAdlRole" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="projectBtnAdLRole_Click" />
            <%--<cc1:CustomButton ID="ProjectBtnAccept" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick="return ajaxModalPopupMassiveTransmissionAccept();" Visible="false" />--%>
            <cc1:CustomButton ID="ProjectBtnAccept" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="ProjectBtnAccept_Click"  Visible="false" />
            <cc1:CustomButton ID="ProjectBtnView" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="ProjectBtnView_Click"  Visible="false" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:HiddenField runat="server" ID="HiddenRemoveNode" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="HiddenProjectLoaded" ClientIDMode="Static" />
            <asp:Button ID="btnCbSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="addAll_Click" />
            <asp:Button ID="btnLinkCustom" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnLinkCustom_Click" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsAll" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenControlPrivate" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenStateOfFolder" runat="server" ClientIDMode="Static" />
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
    <script type="text/javascript" data-main="<%=Page.ResolveClientUrl("~/Scripts/ProjectDragAndDrop.js") %>"
        src="<%=Page.ResolveClientUrl("~/Scripts/require.js") %>"></script>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
    <script type="text/javascript">
        function JsTree() {
            $(function () {
                //$("#tree").empty();

                $("#tree")
                .jstree({
                    "core": {
                        "strings": {
                            loading: "Caricamento in corso...",
                            new_node: "Nuovo fascicolo"
                        }
                    },
                    "html_data": {
                        "ajax": {
                            "url": "Project_getFolders.aspx?d" + new Date().getTime(),
                            "data": function (n) {
                                return { id: n.attr ? n.attr("id") : 0 };
                            }
                        }
                    },
                    "themes": {
                        "theme": "classic",
                        "dots": true,
                        "icons": true
                    },
                    "types": {
                        "valid_children": "all",
                        "types": {
                            "default": {
                                "valid_children": ["default", "nosons"]
                            }
                        }
                    },
                    "ui": {
                        "select_multiple_modifier": false
                    },
                    "crrm": {
                        "move": {
                            "check_move": function (m) {
                                if (m.o.hasClass('jstree-unamovable')) {
                                    return false;
                                }
                                else if (m.p == "before" || m.p == "after") {
                                    return false;
                                }
                                else {
                                    return true;
                                }
                            }
                        }
                    },
                    "dnd": {
                        "drag_check": function (data) {
                            if (data.r.attr("rel") == "nosons") {
                                return false;
                            }
                            return {
                                after: false,
                                before: false,
                                inside: true
                            };
                        },
                        "drag_finish": function (data) {
                            var iDs = "";
                            for (var i = 0; i < $('.jstree-draggable-selected').length; i++) {
                                if ($('.jstree-draggable-selected')[i].id != '') {
                                    if (iDs.length > 0) iDs += ",";
                                    iDs += $('.jstree-draggable-selected')[i].id;
                                }
                            }

                            var titles = "";
                            $(".jstree-draggable-selected").each(function () {
                                if ($(this).attr("title") != undefined) titles += "\n" + $(this).attr("title");
                            });
                            if ($('.jstree-draggable-selected').length > 0 && document.getElementById("HiddenStateOfFolder").value == 'C') {
                                alert("Attenzione! L'operazione richiesta non può essere effettuata poichè il fascicolo risulta essere chiuso.");
                                return false;
                            };
                            if ($('.jstree-draggable-selected').length > 0 && confirm('<asp:Literal id="litConfirmMoveDocuments" runat="server" />'.replace('##', titles.replace('\'', '\\\'')))) {
                                disallowOp('content');
                                $.ajax({
                                    'async': true,
                                    'timeout': 2160000,
                                    'type': 'POST',
                                    'url': "Project_getFolders.aspx",
                                    'data': {
                                        "operation": "drag_documents",
                                        "ids": iDs,
                                        "id": data.o.id,
                                        "ref": data.r[0].id
                                    },
                                    'success': function (r) {
                                        $('#log2').html(r);
                                    },
                                    'error': function (jqXHR, textStatus, errorThrown) {
                                        $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                                    },
                                    'complete': function (r) {
                                        $('.jstree-draggable').removeClass('jstree-draggable-selected');
                                        reallowOp();
                                    }
                                });
                            }
                        }
                    },
                    "plugins": ["themes", "html_data", "dnd", "crrm", "types", "ui"]
                })
                .bind("before.jstree", function (e, data) {
                    if (data.func == "move_node"
                        && data.args[1] == false
                        && data.plugin == "core"
                        && data.args[0].o != undefined
                    ) {
                        var selfDrop = false;
                        if (data.args[0].r != undefined && data.args[0].o[0].id == data.args[0].r[0].id) selfDrop = true;

                        var permitDrop = true;
                        if (data.args[0].ot._get_type(data.args[0].o) == 'root' && data.args[0].ot._get_type(data.args[0].r) != 'root') permitDrop = false;
                        if (data.args[0].cr === -1) permitDrop = false;
                        if (data.args[0].np[0].id == data.args[0].op[0].id) permitDrop = false;
                        if (!$(data.args[0].o[0]).hasClass('jstree-unamovable') && !selfDrop && permitDrop) {
                            if (document.getElementById("HiddenStateOfFolder").value == 'C') {
                                alert("Attenzione! L'operazione richiesta non può essere effettuata poichè il fascicolo risulta essere chiuso.");
                                return false;
                            };
                            if (!confirm('<asp:Literal id="litConfirmMoveFolder" runat="server" />'.replace('##', $(data.args[0].o[0]).attr('data-title').replace('\'', '\\\'')))) {
                                e.stopImmediatePropagation();
                                return false;
                            }
                        }
                        else {
                            if (!$(data.args[0].o[0]).hasClass('jstree-unamovable')) alert('<asp:Literal id="litTreeAlertOperationNotAllowed" runat="server" />');
                            e.stopImmediatePropagation();
                            return false;
                        }
                    }
                })
	            .bind("move_node.jstree", function (e, data) {
	                data.rslt.o.each(function (i) {
	                    /*
	                    data.rslt
	                    .o - the node being moved
	                    .r - the reference node in the move
	                    .ot - the origin tree instance
	                    .rt - the reference tree instance
	                    .p - the position to move to (may be a string - "last", "first", etc)
	                    .cp - the calculated position to move to (always a number)
	                    .np - the new parent
	                    .oc - the original node (if there was a copy)
	                    .cy - boolen indicating if the move was a copy
	                    .cr - same as np, but if a root node is created this is -1
	                    .op - the former parent
	                    .or - the node that was previously in the position of the moved node 
	                    */

	                    var iDs = "";
	                    for (var i = 0; i < data.rslt.o.length; i++) {
	                        if (iDs.length > 0) iDs += ",";
	                        iDs += data.rslt.o[i].id;
	                    }

	                    if (data.rslt.cr === -1 || data.rslt.np.attr("id") == data.rslt.op.attr("id")) {
	                        e.stopImmediatePropagation();
	                        JsTree();
	                        return false;
	                    }
	                    else {
	                        disallowOp('content');
	                        $.ajax({
	                            'async': true,
	                            'type': 'POST',
	                            'timeout': 2160000,
	                            'url': "Project_getFolders.aspx",
	                            'data': {
	                                "operation": "move_node",
	                                "ids": iDs,
	                                "id": $(this).attr("id"),
	                                "ref": data.rslt.r.attr("id"),
	                                "parent": data.rslt.cr === -1 ? 1 : data.rslt.np.attr("id"),
	                                "position": data.rslt.p
	                            },
	                            'success': function (r) {
	                                $('#log2').html(r);
	                            },
	                            'error': function (jqXHR, textStatus, errorThrown) {
	                                $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
	                            },
	                            'complete': function () {
	                                reallowOp();
	                            }
	                        });
	                    }
	                });
	            })
                .bind("select_node.jstree", function (event, data) {
                    if ($.jstree._focused().get_selected().attr('id') != $('#treenode_sel').val()) {
                        $('#treenode_sel').val($.jstree._focused().get_selected().attr('id'));
                        $('#tree .jstree-search').removeClass("jstree-search");
                        $('#BtnChangeSelectedFolder').click();
                    }
                })
                .bind("loaded.jstree", function (e, data) {
                    $('#tree').jstree('open_all');
                    if ($('#treenode_sel').val().length > 0) $("#tree").jstree("select_node", '#' + $('#treenode_sel').val())

                    // assign tooltip and css class to a
                    /*
                    $('.jstree-leaf a').addClass('clickableRight');
                    $('.jstree-last a').addClass('clickableRight');
                    $('.jstree-leaf a').each(function (i) {
                    var $e = $(this);
                    $e.attr('title', $e.text());
                    });
                    $('.jstree-last a').each(function (i) {
                    var $e = $(this);
                    $e.attr('title', $e.text());
                    });
                    */
                    tooltipTipsy();
                    $('.jstree-leaf').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-last').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-open').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-closed').each(function (i) {
                        $(this).attr('title', '');
                    });

                    var qry = $('.retvalSearchSubset input').get(0).value;
                    if (qry.length > 0) searchTree(qry);
                });

                // resolve bug, see https://github.com/vakata/jstree/issues/174
                $("#jstree-marker-line").remove();

                $('.jstree-draggable')
		        .click(function (e) {
		            if (e.ctrlKey) {
		                var row = $('.tbl_rounded tr').index($(this));
		                var index = row + 1;

		                $(this).toggleClass('jstree-draggable-selected');
		                $($(".tbl_rounded tr").get(index)).toggleClass("jstree-draggable-selected");
		            }
		            else if (e.shiftKey) {
		                var iStart = $('.jstree-draggable-selected:first').index();
		                var iCurrent = $('.tbl_rounded tr').index($(this));
		                var index = iCurrent + 1;

		                if (iStart == -1) {
		                    $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                    $(this).addClass('jstree-draggable-selected');
		                    $($(".tbl_rounded tr").get(index)).addClass("jstree-draggable-selected");
		                }
		                else if (iStart < iCurrent) {
		                    $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iStart - 1) + ')').addClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iCurrent + 1) + ')').removeClass('jstree-draggable-selected');
		                }
		                else if (iCurrent < iStart) {
		                    $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iCurrent - 1) + ')').addClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iStart + 1) + ')').removeClass('jstree-draggable-selected');
		                }
		                else if (iStart == iCurrent) {
		                    $(this).toggleClass('jstree-draggable-selected');
		                    $($(".tbl_rounded tr").get(index)).toggleClass("jstree-draggable-selected");
		                }
		            }
		            else {
		                var row = $('.tbl_rounded tr').index($(this));
		                var index = row + 1;

		                $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                $(this).addClass('jstree-draggable-selected');
		                $($(".tbl_rounded tr").get(index)).addClass("jstree-draggable-selected");
		            }
		        });


                // collapse/expand all noded
                $("#collapse_all").click(function (e) {
                    e.stopImmediatePropagation();
                    $('#tree').jstree('close_all');
                    return false;
                });
                $("#expand_all").click(function (e) {
                    e.stopImmediatePropagation();
                    $('#tree').jstree('open_all');
                    return false;
                });


                // dataentry operation 
                $("#ImgFolderAdd").click(function (e) {
                    e.stopImmediatePropagation();
                    if ($('#tree .jstree-clicked').length > 0) {
                        //$('#tree').jstree('create');

                        $.post(
			                "Project_getFolders.aspx",
			                {
			                    "operation": "create_node",
			                    "id": $('#treenode_sel').val(),
			                    "type": 'default'
			                },
			                function (r) {
			                    $('#log2').html(r);
			                }
		                );
                    }
                    return false;
                });
                $("#ImgFolderModify").click(function (e) {
                    e.stopImmediatePropagation();
                    if ($('#treenode_sel').val().indexOf('root_') >= 0) return false;

                    if ($('#tree .jstree-clicked').length > 0) {
                        $.post(
			        "Project_getFolders.aspx",
			        {
			            "operation": "rename_node",
			            "id": $('#treenode_sel').val(),
			            "type": 'default'
			        },
			        function (r) {
			            $('#log2').html(r);
			        }
		        );
                    }
                    return false;
                });
                $("#ImgFolderRemove").click(function (e) {
                    e.stopImmediatePropagation();
                    if ($('#treenode_sel').val().indexOf('root_') >= 0) return false;

                    if ($('#tree .jstree-clicked').length > 0) {
                        $.ajax({
                            async: false,
                            type: 'POST',
                            timeout: 2160000,
                            url: "Project_getFolders.aspx",
                            data: {
                                "operation": "remove_node",
                                "id": $('#treenode_sel').val()
                            },
                            success: function (r) {
                                $('#log2').html(r);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                            }
                        });
                    }
                    return false;
                });

                var nodes;
                function searchTree(qry) {
                    // set all nodes color back to normal
                    $("#tree").find('.jstree-search').removeClass('jstree-search');
                    $("#tree").find('.jstree-clicked').removeClass('jstree-clicked');

                    $.getJSON('Project_getFolders.aspx', { 'q': qry }, function (data) {
                        if (data != null) {
                            // server returns a list of object that is { string id, bool isResult }.  
                            // the results contains any elements in the search and all of the parents of that element.  
                            // all these elements are necessary to expand all the branches. 
                            // only search results return a true value for isResult
                            nodes = data;
                            var i = 0;

                            /*
                            $('#tree').bind('open_node.jstree', function (e, d) {
                            var index = -1;
                            for (var j = 0; j < data.length; j++) if (data[j].id == d.rslt.obj[0].id) index = j;
                            searchTreeFromIndex(index);
                            });
                            */
                            if (data.length == 1 && false) {// delete false condition to autoselect folder when result is one
                                $("#tree").jstree("select_node", "#" + data[0].id).trigger("select_node.jstree");
                            }
                            else {
                                while (i < data.length) {
                                    var node = '#' + data[i].id;
                                    if (data[i].isResult == 'true') {
                                        $(node).find('a').addClass('jstree-search');
                                        i++;
                                    }
                                    else {
                                        $('#tree').jstree('open_node', $(node));
                                        i++;
                                    }
                                }
                            }
                        }
                    })
                    .error(function (jqXHR, textStatus, errorThrown) {
                        $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                    });

                    // open the tree
                    $("#tree").jstree('open_all');

                    $('.retvalSearchSubset input').get(0).value = '';
                    //                    }
                }

                function searchTreeFromIndex(index) {
                    for (var j = index; j < nodes.length; j++) {
                        if (nodes[j]) {
                            var node = '#' + nodes[j].id;
                            if (nodes[j].isResult == 'true') {
                                $(node).find('a').addClass('jstree-search');
                            }
                            else {
                                $('#tree').jstree('open_node', $(node));
                            }
                        }
                    }
                }

                function DocumentsToggle(o) {
                    $('#documents tbody input').attr('checked', o.checked);
                }

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
            });
        };
    </script>
</asp:Content>
