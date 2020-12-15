<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Document.aspx.cs" Inherits="NttDataWA.Document.Document"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/HeaderDocument.ascx" TagPrefix="uc2" TagName="HeaderDocument" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="DocumentButtons" %>
<%@ Register Src="~/UserControls/DocumentTabs.ascx" TagPrefix="uc4" TagName="DocumentTabs" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc5" TagName="ViewDocument" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <link href="../Css/bootstrap.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/require.js"></script>
    <script type="text/javascript">

        function escape_xml(string) {
            return string
			.toString()
			.replace('&amp;', '&' )
			.replace('&lt;', '<')
			.replace('&gt;','>' )
        };

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

        function verifica() {
            var valReturn = false;
            if (document.getElementById("<%=this.TxtCodeProject.ClientID%>").value != '' && document.getElementById("<%=TxtDescriptionProject.ClientID%>").value != '') {
                valReturn = true;
            }
            else if (document.getElementById("<%=this.TxtCodeProject.ClientID%>").value == '') {
                valReturn = true;
            }

            return valReturn;
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




        function acePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoBIS');
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

        function aceSelected(sender, e) {
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


            var searchText = $get('<%=TxtDescriptionSender.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionSender.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionSender.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeSender.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionSender.ClientID%>").value = escape_xml(descrizione)

        }

        function ItemSelectedDestinatario(sender, e) {
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


            var searchText = $get('<%=TxtRecipientDescription.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtRecipientDescription.ClientID%>").focus();
            document.getElementById("<%=this.TxtRecipientDescription.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtRecipientCode.ClientID%>").value = codice;
            document.getElementById("<%=TxtRecipientDescription.ClientID%>").value = descrizione;
            __doPostBack('<%=this.DocumentImgAddRecipient.ClientID%>', '');
        }

        function acePopulatedDest(sender, e) {
            var behavior = $find('AutoCompleteExDestinatari');
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

        function swapUserPrivateCheckboxs(id) {
            if ($get('<%=DocumentChekUser.ClientID %>') && $get('<%=DocumentCheckPrivate.ClientID %>')) {
                if (id == '<%=DocumentChekUser.ClientID %>' && $get('<%=DocumentChekUser.ClientID %>').checked) {
                    $get('<%=DocumentCheckPrivate.ClientID %>').checked = false;
                }
                else if (id == '<%=DocumentCheckPrivate.ClientID %>' && $get('<%=DocumentCheckPrivate.ClientID %>').checked) {
                    $get('<%=DocumentChekUser.ClientID %>').checked = false;
                }
            }
        }

        function UpdateTooltipTxtProtocolSender() {
            document.getElementById("<%=TxtProtocolSender.ClientID%>").setAttribute('title','');
            var text = document.getElementById("<%=TxtProtocolSender.ClientID%>").value;
            document.getElementById("<%=TxtProtocolSender.ClientID%>").setAttribute('title',text);
        }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="senderpopup" runat="server" Url="../Popup/Sender.aspx" IsFullScreen="true"
        PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui)  {$('#btnSenderPostback').click();}" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
        
    <uc:ajaxpopup2 Id="UplodadFile" runat="server" Url="" Width="600"
        Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="RepositoryView" runat="server" Url="../Repository/RepositoryView.aspx" Width="850"
        Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="Signature" runat="server" Url="../popup/Signature.aspx" PermitClose="false"
        PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="DigitalSignSelector" runat="server" Title="Firma documento" Url="../popup/DigitalSignSelector.aspx?TipoFirma=sign"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalCosignSelector" runat="server" Title="Firma documento"
        Url="../popup/DigitalSignSelector.aspx?TipoFirma=cosign&Caller=cosign" Width="600" Height="500"
        PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalVisureSelector" runat="server" Title="Approva documento" Url="../popup/DigitalVisure.aspx"
        Width="600" Height="300" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />   
    <uc:ajaxpopup2 Id="ViewObject" runat="server" Url="../popup/ViewObject.aspx" Width="600"
        Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ChooseCorrespondent" runat="server" Url="../popup/ChooseCorrespondent.aspx"
        Width="700" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="AnswerSearchDocuments" runat="server" Url="../popup/AnswerSearchDocuments.aspx"
        Width="800" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlResponseProtocol', '');}" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlResponseProtocol', ''); }" />
    <uc:ajaxpopup2 Id="AnswerChooseRecipient" runat="server" Url="../popup/AnswerChooseRecipient.aspx"
        Width="800" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlResponseProtocol', '');}" />
    <uc:ajaxpopup2 Id="AnswerShowAnswers" runat="server" Url="../popup/AnswerShowAnswers.aspx"
        Width="800" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlResponseProtocol', '');}" />
    <uc:ajaxpopup2 Id="Note" runat="server" Url="../popup/Note.aspx?type=D" Width="800"
        Height="600" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) { __doPostBack('UpPnlNote', '');}" />
    <uc:ajaxpopup2 Id="AbortCounter" runat="server" Url="../popup/AbortCounter.aspx"
        Width="600" Height="450" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="SendingReportDocument" runat="server" Url="~/Popup/ReportSpedizioni.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {$('#btnReportSpedizioniPostback').click();}" />
    <%--    <uc:ajaxpopup2 Id="ReceivedSending" runat="server" Url="../popup/ReceivedSending.aspx"
        IsFullScreen="true" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    --%>
    <uc:ajaxpopup2 Id="CorrespondentDetails" runat="server" Url="../popup/CorrespondentDetails.aspx"
        Width="680" Height="700" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {$('#btnUpdateCorrespondentsDetails').click();}" />
    <uc:ajaxpopup2 Id="RecipientsDetails" runat="server" Url="../popup/RecipientsDetails.aspx"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup2 Id="RecipientsCCDetails" runat="server" Url="../popup/RecipientsDetails.aspx?type=cc"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup2 Id="ChoiceTypeDelivery" runat="server" Url="../popup/ChoiceTypeDelivery.aspx"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="AbortRecord" runat="server" Url="../popup/AbortRecord.aspx" Width="500"
        Height="350" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="SelectRF" runat="server" Url="../popup/SelectRF.aspx" Width="600"
        Height="380" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="Prints" runat="server" Url="../popup/visualReport_iframe.aspx"
        Width="400" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('container', '');}" />
    <uc:ajaxpopup2 Id="Consolidation" runat="server" Url="../popup/Consolidation.aspx"
        Width="600" Height="370" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="RemoveProfile" runat="server" Url="../popup/RemoveProfile.aspx"
        Width="500" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('container', '');}" />
    <uc:ajaxpopup2 Id="VerifyPrevious" runat="server" Url="../popup/VerifyPrevious.aspx"
        Width="600" Height="550" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="VerifyPreviousViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {$('BtnVerifyPreviousViewer').click();}" />
    <uc:ajaxpopup2 Id="SelectKeyword" runat="server" Url="../popup/SelectKeyword.aspx"
        Width="700" Height="550" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="Visibility" runat="server" Url="../popup/Visibility.aspx" IsFullScreen="true"
        PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup2 Id="VersionAdd" runat="server" Url="../popup/version_add.aspx" Width="450"
        Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ModifyVersion" runat="server" Url="../popup/version_add.aspx?modifyVersion=t"
        Width="450" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="History" runat="server" Url="../Popup/History.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="1000" Height="800" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ChooseRFSegnature" runat="server" Url="../popup/ChooseRFSegnature.aspx"
        Width="500" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ChooseRFSegnatureFromRecord" runat="server" Url="../popup/ChooseRFSegnature.aspx?fromRecord=true"
        Width="500" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ActiveXScann" runat="server" Url="../popup/acquisizione.aspx"
        ShowLoading="false" Width="1000" Height="700" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <!--IsFullScreen="true"-->
    <uc:ajaxpopup2 Id="PrintReceiptPdf" runat="server" Url="../popup/PrintReceiptPdf.aspx"
        PermitClose="true" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="AddressBook_New" runat="server" Url="../popup/AddressBook_new.aspx"
        PermitClose="true" PermitScroll="false" Width="680" Height="700" CloseFunction="function (event, ui) { __doPostBack('UpPnlSender', 'NewSenderFromAddressBook');}" />
    <uc:ajaxpopup2 Id="DetailsSenderK1" runat="server" Url="../Popup/DetailsSenderK1K2.aspx"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="750" Height="700"
        ForceDontClose="true" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', 'ReturnFromK1_K2');}" />
    <uc:ajaxpopup2 Id="DetailsSenderK2" runat="server" Url="../Popup/DetailsSenderK1K2.aspx"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="1200" Height="700"
        ForceDontClose="true" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', 'ReturnFromK1_K2');}" />
    <uc:ajaxpopup2 Id="SaveDialog" runat="server" Url="../CheckInOutApplet/CheckInOutSaveLocal.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="430"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="CheckOutDocument" runat="server" Url="../CheckInOutApplet/CheckOutDocument.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="250"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="UndoCheckOut" runat="server" Url="../CheckInOutApplet/UndoPendingChange.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="CheckInDocument" runat="server" Url="../CheckInOutApplet/CheckInDocument.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="330"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="OpenLocalCheckOutFile" runat="server" Url="../CheckInOutApplet/OpenLocalCheckOutFile.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ShowCheckOutStatus" runat="server" Url="../CheckInOutApplet/ShowCheckOutStatus.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="CheckOutModelApplet" runat="server" Url="../CheckInOutApplet/CheckOutModel.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="CheckOutModelActiveX" runat="server" Url="../CheckInOut/CheckOutModel.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />

    <uc:ajaxpopup2 Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx"
        PermitClose="false" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../popup/SearchProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="OpenAddDocCustom" runat="server" Url="../popup/AddDocInProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui)  {$('#btnLinkCustom').click();}" />
    <uc:ajaxpopup2 Id="SearchProjectCustom" runat="server" Url="../popup/SearchProject.aspx?caller=custom"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui)  {$('#btnLinkCustom').click();}" />
    <uc:ajaxpopup2 Id="HierarchyVisibility" runat="server" Url="../popup/HierarchyVisibilityTransmission.aspx"
        IsFullScreen="false" Width="500" Height="300" PermitClose="false" ForceDontClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="HSMSignature" runat="server" Url="../popup/HSM_Signature.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="700" Height="400" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="StartProcessSignature" runat="server" Url="../popup/StartProcessSignature.aspx"
        Width="1200" Height="800" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', 'CloseStartProcessSignature');}" />
    <uc:ajaxpopup2 Id="DetailsLFAutomaticMode" runat="server" Url="../popup/DetailsLFAutomaticMode.aspx"
        Width="750" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', 'SignatureProcessConcluted');}" />
     <uc:ajaxpopup2 Id="MassiveReportDragAndDrop" runat="server" Url="../popup/MassiveReportDragAndDrop.aspx"
        Width="700" Height="500" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'POPUP_DRAG_AND_DROP');}" />
     <uc:ajaxpopup2 Id="NewProject" runat="server" Url="../popup/NewProject.aspx"
       IsFullScreen="true" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
     <uc:ajaxpopup2 Id="FlussoAutomatico" runat="server" Url="../popup/FlussoAutomatico.aspx"
       Width="1200" Height="500" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="AddressBookFromPopup" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui){__doPostBack('UpdPnlObject', 'closePopupAddressBook');}" />
    <uc:ajaxpopup2 Id="InvoicePreview" runat="server" Url="../popup/InvoicePreview.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="500" Height="800"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="MassiveTransmissionAccept" runat="server" Url="../popup/MassiveTransmissionAccept.aspx?type=d"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="1000" Height="900"
        CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="SelectAttForPublication" runat="server" Url="../popup/SelectAttForPublication.aspx"
        Width="1000" Height="600" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="InformazioniFile" runat="server" Url="../popup/InformazioniFile.aspx" PermitClose="true" PermitScroll="false" 
        IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <div id="containerTop">
        <asp:UpdatePanel ID="UpUserControlHeaderDocument" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <uc2:HeaderDocument runat="server" ID="HeaderDocument" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpcontainerDocumentTabLeftBorder" runat="server" UpdateMode="Conditional"
            ClientIDMode="static">
            <ContentTemplate>
                <div id="containerDocumentTab" runat="server" clientidmode="Static">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <asp:UpdatePanel runat="server" ID="UpContainerDocumentTab" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc4:DocumentTabs runat="server" PageCaller="DOCUMENT" ID="DocumentTabs"></uc4:DocumentTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <uc3:DocumentButtons runat="server" ID="DocumentButtons" PageCaller="DOCUMENT" />
                        </div>
                    </div>
                    <asp:UpdatePanel runat="server" ID="UpcontainerDocumentTabDxBorder" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerDocumentTabDxBorder" runat="server" clientidmode="Static">
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="container" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <a name="anchorTop"></a>
                        <div class="box_inside">
                            <div class="row">
                                <div class="col">
                                    <div id="colBottom">
                                        <asp:UpdatePanel runat="server" ID="UpTypeProtocol" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:RadioButtonList runat="server" ID="RblTypeProtocol" OnSelectedIndexChanged="RblTypeProtocol_OnSelectedIndexChanged"
                                                    AutoPostBack="true" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="A" id="rbIn" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="P" id="rbOut"></asp:ListItem>
                                                    <asp:ListItem Value="I" id="rbOwn"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                                <div class="col2-right-no-margin-right">
                                    <asp:UpdatePanel ID="UpPnlRegistry" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="col3-right2">
                                                <asp:Label ID="DocumentLblRegistry" runat="server">
                                                    <asp:Literal runat="server" ID="DocumentLitRegistries"></asp:Literal></asp:Label>
                                            </div>
                                            <div class="col3-right">
                                                <asp:Panel ID="PnlRegistry" runat="server">
                                                    <asp:DropDownList runat="server" ID="DdlRegistries" CssClass="chzn-select-deselect"
                                                        Width="90" OnSelectedIndexChanged="DdlRegistries_SelectedIndexChanged" AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </asp:Panel>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div  class="row">
                                    <div class="col">
                                        <div id="colBottom4">
                                            <asp:UpdatePanel runat="server" ID="UpDirittiDocumento" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Panel ID="PnlDirittiDocumento" Visible="false" runat="server">
                                                        <span class="weight">
                                                            <asp:Label ID="LblDiritti" runat="server"></asp:Label></span>
                                                        <asp:Label ID="LblTipoDiritto" runat="server"></asp:Label>
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                    <div  class="col2-right-no-margin-right">
                                        <div class="col3-right">
                                            <asp:UpdatePanel runat="server" ID="UpDocumentPrivate" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:PlaceHolder runat="server" ID="PnlCheckUser">
                                                        <div class="col2-right">
                                                            <span id="DisabledDocumentChekUser" runat="server">
                                                                <asp:CheckBox ID="DocumentChekUser" runat="server" onclick="swapUserPrivateCheckboxs(this.id)"
                                                                    CssClass="clickableLeftN" />
                                                            </span>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <div class="col2-right">
                                                        <span id="DisabledDocumentCheckPrivate" runat="server">
                                                            <asp:CheckBox ID="DocumentCheckPrivate" runat="server" onclick="swapUserPrivateCheckboxs(this.id)"
                                                                CssClass="clickableLeftN" />
                                                        </span>
                                                    </div>
                                                    <asp:HiddenField runat="server" ID="HiddenISPrivate" ClientIDMode="Static" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        </div>
                                    </div>
                            </div>
                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="UpPnlDataDocument" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlDataDocument" runat="server" Visible="false">
                                            <fieldset>
                                                <div class="col">
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Label ID="DocumentLblCreationDate" runat="server" Visible="false"></asp:Label></span>
                                                            <asp:Label ID="LblCreationDate" runat="server" Visible="false"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Label ID="DocumentLblRecordDate" runat="server" Visible="false"></asp:Label></span>
                                                                <asp:Label ID="LblRecordDate" runat="server" Visible="false"></asp:Label>
                                                            </p>
                                                        </div>
                                                    </div>

                                                    <asp:PlaceHolder ID="plcEmergency" runat="server" Visible="false">
                                                        <div class="row">
                                                            <div class="col">
                                                                <p><span class="weight"><asp:Label ID="DocumentLblEmergencyNum" runat="server"></asp:Label></span>
                                                                    <asp:Label ID="LblEmergencyNum" runat="server"></asp:Label>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col">
                                                                <p><span class="weight"><asp:Label ID="DocumentLblEmergencyDate" runat="server"></asp:Label></span>
                                                                    <asp:Label ID="LblEmergencyDate" runat="server"></asp:Label>
                                                                </p>
                                                            </div>
                                                        </div>
                                                    </asp:PlaceHolder>                                            
                                                </div>
                                                <asp:PlaceHolder ID="PlcConforme" runat="server"  Visible="false">
                                                    <div class="col-right-no-margin-no-top">
                                                        <div class="col-no-margin-top">
                                                            <span class="weight">
                                                               <asp:Label ID="DcoumentLblConforme" runat="server"></asp:Label></span>
                                                         </div>
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgInfoFile" ImageUrl="../Images/Icons/info_file_ok.png"
                                                        OnMouseOutImage="../Images/Icons/info_file_ok.png" OnMouseOverImage="../Images/Icons/info_file_ok.png"
                                                        CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/info_file_ok.png" OnClientClick="return ajaxModalPopupInformazioniFile();"/>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </fieldset>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpAbortedAnnulled" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:HiddenField runat="server" ID="HiddenAbortPre" ClientIDMode="Static" />
                                        <asp:PlaceHolder runat="server" ID="PnlAbortRecord" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="redWeight">
                                                                <asp:Literal ID="LitDocumentAbortRecord" runat="server"></asp:Literal></span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="redWeight">
                                                                <asp:Literal ID="LitDocumentAbortRecordData" runat="server"></asp:Literal></span>
                                                            <span class="red">
                                                                <asp:Label ID="LblDocumentAborted" runat="server"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="redWeight">
                                                                <asp:Literal ID="LitDocumentAbortRecordReason" runat="server"></asp:Literal></span>
                                                            <span class="red">
                                                                <asp:Label ID="LblDocumentAbortText" runat="server"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpConsolidate" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder runat="server" ID="PnlConsolidate" Visible="false">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="redWeight">
                                                                <asp:Literal ID="LitDocumentConsolidate" runat="server"></asp:Literal></span>
                                                            <span class="red">
                                                                <asp:Label ID="LblDocumentConsolidateType" runat="server"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="DocumentLitObject" runat="server"></asp:Literal></span><span class="little">*</span></p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <asp:UpdatePanel ID="UpPnlIconObjects" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgDesriptionObject" ImageUrl="../Images/Icons/obj_description.png"
                                                            OnMouseOutImage="../Images/Icons/obj_description.png" OnMouseOverImage="../Images/Icons/obj_description_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_description_disabled.png"
                                                            OnClick="DocumentImgDesriptionObject_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                                                            OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                                            OnClientClick="return ajaxModalPopupObject();" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgObjectHistory" ImageUrl="../Images/Icons/obj_history_big.png"
                                                            OnMouseOutImage="../Images/Icons/obj_history_big.png" OnMouseOverImage="../Images/Icons/obj_history_big_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_big_disabled.png"
                                                            OnClick="DocumentImgHistory_Click" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                        <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <asp:Panel ID="PnlCodeObject" runat="server" Visible="false">
                                                        <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                                            autocomplete="off" CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true"
                                                            OnTextChanged="TxtCodeObject_Click">
                                                        </cc1:CustomTextArea>
                                                    </asp:Panel>
                                                    <asp:Panel ID="PnlCodeObject2" runat="server">
                                                        <asp:Panel ID="PnlCodeObject3" runat="server">
                                                            <cc1:CustomTextArea ID="TxtObject" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                                                CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static">
                                                            </cc1:CustomTextArea>
                                                        </asp:Panel>
                                                    </asp:Panel>
                                                </div>
                                                <div class="row">
                                                    <div class="col-right-no-margin">
                                                        <span class="charactersAvailable">
                                                            <asp:Literal ID="DocumentLitObjectChAv" runat="server"></asp:Literal>
                                                            <span id="TxtObject_chars"></span></span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                    </div>
                                                </div>
                                                <asp:Panel ID="PnlDdlOggettario" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <div class="styled-select_full">
                                                                <asp:DropDownList ID="DdlOggettario" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                                    Width="100%" OnSelectedIndexChanged="DdlOggettario_SelectedIndexChanged">
                                                                    <asp:ListItem Text=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <asp:Panel ID="PnlSenders" runat="server">
                                    <fieldset>
                                        <div class="row">
                                            <asp:UpdatePanel runat="server" ID="UpPnlImgSender" UpdateMode="Conditional" ClientIDMode="Static">
                                                <ContentTemplate>
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="DocumentLblSender" runat="server"></asp:Literal></span><span class="little">*</span>
                                                        </p>
                                                    </div>
                                                    <div class="col">
                                                        <asp:Image ID="DocumentImgSenderInt" runat="server" ImageUrl="../Images/Icons/flag_ok.png"
                                                            Visible="False" CssClass="clickable" ImageAlign="Middle" />
                                                        <cc1:CustomImageButton ID="DocumentImgAddNewCorrispondent" runat="server" ImageUrl="../Images/Icons/add_version.png"
                                                            OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png"
                                                            CssClass="clickable" Visible="False" OnClick="DocumentImgAddNewCorrispondent_Click"
                                                            ClientIDMode="Static" ImageAlign="Middle" />
                                                        <asp:Image ID="DocumentImgSenderWarning" runat="server" Height="24" Width="22" ImageUrl="../Images/Common/messager_warning.png"
                                                            Visible="False" CssClass="clickable" ImageAlign="Middle" />
                                                        <asp:Image ID="DocumentImgMailSender" runat="server" ImageUrl="~/Images/Icons/mittente_mail.png"
                                                            Visible="false" CssClass="clickable" ImageAlign="Middle" />
                                                        <asp:Image ID="DocumentImgPecSender" runat="server" ImageUrl="~/Images/Icons/mittente_pec.png"
                                                            Visible="false" CssClass="clickable" ImageAlign="Middle" />
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgSenderAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                            OnClick="DocumentImgSenderAddressBook_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgSenderDetails" ImageUrl="../Images/Icons/address_book_details.png"
                                                            OnMouseOutImage="../Images/Icons/address_book_details.png" OnMouseOverImage="../Images/Icons/address_book_details_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_details_disabled.png"
                                                            OnClick="DocumentImgSenderDetails_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgHistorySender" ImageUrl="../Images/Icons/obj_history.png"
                                                            OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                            OnClick="DocumentImgHistory_Click" />
                                                        <asp:Button ID="btnUpdateCorrespondentsDetails" runat="server" ClientIDMode="Static"
                                                            Style="display: none;" OnClick="btnUpdateCorrespondentsDetails_Click" />
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <div class="row">
                                            <asp:UpdatePanel runat="server" ID="UpPnlSender" UpdateMode="Conditional" ClientIDMode="static">
                                                <ContentTemplate>
                                                    <asp:HiddenField ID="IdSender" runat="server" />
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="TxtCodeSender" runat="server" CssClass="txt_addressBookLeft"
                                                            autocomplete="off" OnTextChanged="TxtCode_OnTextChanged" AutoPostBack="true"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf2">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="TxtDescriptionSender" runat="server" CssClass="txt_addressBookRight" MaxLength="500"
                                                                autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled" AutoCompleteType="Disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                  <uc1:AutoCompleteExtender runat="server" ID="RapidSender" TargetControlID="TxtDescriptionSender"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                        OnClientPopulated="acePopulated">
                                                    </uc1:AutoCompleteExtender>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <asp:UpdatePanel ID="UpPnlMultipleSender" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel ID="PnlMultipleSender" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="DocumentLitMultipleSender" runat="server"></asp:Literal></span></p>
                                                        </div>
                                                        <div class="col">
                                                            <p>
                                                                <cc1:CustomImageButton runat="server" ID="DocumentImgDownSender" ImageUrl="../Images/Icons/down_arrow.png"
                                                                    OnMouseOutImage="../Images/Icons/down_arrow.png" OnMouseOverImage="../Images/Icons/down_arrow_hover.png"
                                                                    CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/down_arrow_disabled.png"
                                                                    OnClick="AddSenderToMultipleSender_Click" />
                                                                <cc1:CustomImageButton runat="server" ID="DocumentImgUpSender" ImageUrl="../Images/Icons/up_arrow.png"
                                                                    OnMouseOutImage="../Images/Icons/up_arrow.png" OnMouseOverImage="../Images/Icons/up_arrow_hover.png"
                                                                    CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/up_arrow_disabled.png"
                                                                    OnClick="AddMultipleSenderToSender_Click" />
                                                            </p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgMultipleSenderAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnClick="DocumentImgMultipleSenderAddressBook_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgMultipleSenderDetails" ImageUrl="../Images/Icons/address_book_details.png"
                                                                OnMouseOutImage="../Images/Icons/address_book_details.png" OnMouseOverImage="../Images/Icons/address_book_details_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_details_disabled.png"
                                                                OnClick="DocumentImgMultipleSenderDetails_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgMultipleSenderDelete" ImageUrl="../Images/Icons/delete.png"
                                                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                                OnClick="DeleteMultipleSender_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgHistoryMultipleSender" ImageUrl="../Images/Icons/obj_history.png"
                                                                OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                                OnClick="DocumentImgHistory_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:ListBox ID="ListBoxMultipleSender" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled">
                                                        </asp:ListBox>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel ID="UpPnlRecipients" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel ID="PnlRecipients" runat="server">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="DocumentLitRecipient" runat="server"></asp:Literal></span></p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgPrintEnvelopesRecipients" ImageUrl="../Images/Icons/print_letter_recipients.png"
                                                                OnMouseOutImage="../Images/Icons/print_letter_recipients.png" OnMouseOverImage="../Images/Icons/print_letter_recipients_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/print_letter_recipients_disabled.png"
                                                                OnClick="DocumentImgPrintEnvelopesRecipients_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnClick="DocumentImgRecipientAddressBook_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgAddRecipient" ImageUrl="../Images/Icons/add_version.png"
                                                                OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/add_version_disabled.png"
                                                                OnClick="AddRecipient_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgHistoryRecipient" ImageUrl="../Images/Icons/obj_history.png"
                                                                OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                                OnClick="DocumentImgHistory_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:HiddenField ID="IdRecipient" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="TxtRecipientCode" runat="server" CssClass="txt_addressBookLeft"
                                                                autocomplete="off" OnTextChanged="TxtCode_OnTextChanged" AutoPostBack="true"
                                                                CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="TxtRecipientDescription" runat="server" CssClass="txt_addressBookRight" MaxLength="500"
                                                                    autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled" AutoCompleteType="Disabled">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                         <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="TxtRecipientDescription"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="ItemSelectedDestinatario" BehaviorID="AutoCompleteExDestinatari"
                                                            OnClientPopulated="acePopulatedDest">
                                                        </uc1:AutoCompleteExtender>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="DocumentLitRecipients" runat="server"></asp:Literal></span><span
                                                                        class="little">*</span></p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgSending" ImageUrl="../Images/Icons/sending.png"
                                                                OnMouseOutImage="../Images/Icons/sending.png" OnMouseOverImage="../Images/Icons/sending_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/sending_disabled.png"
                                                                OnClick="DocumentImgSending_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgReportSpedizioni" ImageUrl="../Images/Icons/received_sending.png"
                                                                OnMouseOutImage="../Images/Icons/received_sending.png" OnMouseOverImage="../Images/Icons/received_sending_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/received_sending_disabled.png"
                                                                OnClick="DocumentImgReportSpedizioni_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgFlussoProcedurale" ImageUrl="../Images/Icons/workflow.png" Visible="false"
                                                                OnMouseOutImage="../Images/Icons/workflow.png" OnMouseOverImage="../Images/Icons/workflow_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/workflow.png"
                                                                OnClientClick="return ajaxModalPopupFlussoAutomatico();" />
                                                            <%--                                                       <cc1:CustomImageButton runat="server" ID="DocumentImgReceivedSending" ImageUrl="../Images/Icons/received_sending.png"
                                                                OnMouseOutImage="../Images/Icons/received_sending.png" OnMouseOverImage="../Images/Icons/received_sending_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/received_sending_disabled.png"
                                                                OnClick="DocumentImgReceivedSending_Click" />--%>
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientsDetails" ImageUrl="../Images/Icons/recipient_details.png"
                                                                OnMouseOutImage="../Images/Icons/recipient_details.png" OnMouseOverImage="../Images/Icons/recipient_details_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/recipient_details_disabled.png"
                                                                OnClientClick="return ajaxModalPopupRecipientsDetails();" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientDetails" ImageUrl="../Images/Icons/recipients_details.png"
                                                                OnMouseOutImage="../Images/Icons/recipients_details.png" OnMouseOverImage="../Images/Icons/recipients_details_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/recipients_details_disabled.png"
                                                                OnClick="DocumentImgRecipientDetails_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgDeleteRecipient" ImageUrl="../Images/Icons/delete.png"
                                                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                                OnClick="DeleteRecipient_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:ListBox ID="ListBoxRecipient" runat="server" CssClass="txt_textarea"></asp:ListBox>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="DocumentLitRecipientsCC" runat="server"></asp:Literal></span></p>
                                                        </div>
                                                        <div class="col">
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgAddRecipientToCC" ImageUrl="../Images/Icons/down_arrow.png"
                                                                OnMouseOutImage="../Images/Icons/down_arrow.png" OnMouseOverImage="../Images/Icons/down_arrow_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/down_arrow_disabled.png"
                                                                OnClick="AddRecipientToCC" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgAddRecipientFromCC" ImageUrl="../Images/Icons/up_arrow.png"
                                                                OnMouseOutImage="../Images/Icons/up_arrow.png" OnMouseOverImage="../Images/Icons/up_arrow_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/up_arrow_disabled.png"
                                                                OnClick="AddFromCCToRecipient" />
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <!--<cc1:CustomImageButton runat="server" ID="DocumentImgReceivedSendingCC" ImageUrl="../Images/Icons/received_sending.png"
                                                                OnMouseOutImage="../Images/Icons/received_sending.png" OnMouseOverImage="../Images/Icons/received_sending_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/received_sending_disabled.png"
                                                                OnClick="DocumentImgReceivedSendingCC_Click" />-->
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientsDetailsCC" ImageUrl="../Images/Icons/recipient_details.png"
                                                                OnMouseOutImage="../Images/Icons/recipient_details.png" OnMouseOverImage="../Images/Icons/recipient_details_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/recipient_details_disabled.png"
                                                                OnClientClick="return ajaxModalPopupRecipientsCCDetails();" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientDetailsCC" ImageUrl="../Images/Icons/recipients_details.png"
                                                                OnMouseOutImage="../Images/Icons/recipients_details.png" OnMouseOverImage="../Images/Icons/recipients_details_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/recipients_details_disabled.png"
                                                                OnClick="DocumentImgRecipientDetailsCC_Click" />
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgDeleteRecipientCC" ImageUrl="../Images/Icons/delete.png"
                                                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                                OnClick="DeleteRecipientCC_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:ListBox ID="ListBoxRecipientCC" runat="server" CssClass="txt_textarea"></asp:ListBox>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </asp:Panel>
                                <asp:UpdatePanel ID="UpPnlMeansSender" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlMeansSender" runat="server" Visible="false">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col5">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="DocumentLitMeansSender" runat="server"></asp:Literal></span></p>
                                                    </div>
                                                    <div class="col">
                                                        <asp:DropDownList ID="DdlMeansSending" runat="server" CssClass="chzn-select-deselect"
                                                            Width="260">
                                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </fieldset>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="UpPnlSenderProtocol" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlSenderProtocol" runat="server">
                                            <fieldset>
                                                <div class="boxDataDocument">
                                                    <div class="boxDoubleDocumentSx">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="DocumentLitProtocolSender"></asp:Literal></span>
                                                        </p>
                                                    </div>
                                                    <div class="boxDoubleDocumentDx">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="DocumentLitDateProtocolSender"></asp:Literal></span>
                                                        </p>
                                                    </div>
                                                </div>
                                                <div class="boxDataDocument">
                                                    <div class="boxDoubleDocumentSx">
                                                        <cc1:CustomTextArea ID="TxtProtocolSender" runat="server" CssClass="txt_textdata clickable" 
                                                            CssClassReadOnly="txt_textdata_disabled" Width="90%" onchange="UpdateTooltipTxtProtocolSender()">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div class="boxDoubleDocumentDx">
                                                        <cc1:CustomTextArea ID="TxtDateProtocol" runat="server" CssClass="txt_textdata datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled" ClientIDMode="Static">
                                                        </cc1:CustomTextArea>
                                                        &nbsp;
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgViewDocumentExists" ImageUrl="../Images/Icons/verify_prevoius.png"
                                                            OnMouseOutImage="../Images/Icons/verify_prevoius.png" OnMouseOverImage="../Images/Icons/verify_prevoius_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/verify_prevoius_disabled.png"
                                                            OnClick="DocumentImgViewDocumentExists_Click" />
                                                    </div>
                                                </div>
                                            </fieldset>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="UpPnlArrivaleDate" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlArrivaleDate" runat="server">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="boxDataDocument">
                                                        <div class="boxDoubleDocumentSx">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="DocumentLitArrivalDate"></asp:Literal></span>
                                                            </p>
                                                        </div>
                                                        <div class="boxDoubleDocumentDx">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="DocumentLitTimeOfArrival"></asp:Literal></span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="boxDataDocument">
                                                        <div class="boxDoubleDocumentSx">
                                                            <cc1:CustomTextArea ID="TxtArrivalDate" runat="server" CssClass="txt_textdata datepicker"
                                                                CssClassReadOnly="txt_textdata_disabled" ClientIDMode="Static">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="boxDoubleDocumentDx">
                                                            <cc1:CustomTextArea ID="TxtTimeOfArrival" runat="server" CssClass="txt_textdata hour"
                                                                CssClassReadOnly="txt_textdata_disabled">
                                                            </cc1:CustomTextArea>
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgHistoryDate" ImageUrl="../Images/Icons/obj_history.png"
                                                                OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                                OnClick="DocumentImgHistory_Click" />
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row">
                                    <fieldset>
                                        <asp:UpdatePanel runat="server" ID="UpPnlNote" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col" style="width: 77%;">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="DocumentLitVisibleNotes" /><asp:Literal runat="server"
                                                                    ID="DocumentLitNoteAuthor" /></span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgNotedetails" ImageUrl="../Images/Icons/ico_objects.png"
                                                            OnMouseOutImage="../Images/Icons/ico_objects.png" OnMouseOverImage="../Images/Icons/ico_objects.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
                                                            OnClientClick="return ajaxModalPopupNote();" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:RadioButtonList ID="RblTypeNote" runat="server" AutoPostBack="true" CssClass="rblHorizontal RblTypeNote"
                                                            OnSelectedIndexChanged="RblTypeNote_SelectedIndexChanged" RepeatLayout="UnorderedList">
                                                            <asp:ListItem Value="Personale" id="DocumentItemNotePersonal"></asp:ListItem>
                                                            <asp:ListItem Value="Ruolo" id="DocumentItemNoteRole"></asp:ListItem>
                                                            <asp:ListItem Value="RF" id="DocumentItemNoteRF"></asp:ListItem>
                                                            <asp:ListItem Value="Tutti" id="DocumentItemNoteAll" Selected="true"></asp:ListItem>
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
                                                        autocomplete="off" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                                                    <asp:HiddenField ID="isTutti" runat="server" Value="" />
                                                 <uc1:AutoCompleteExtender runat="server" ID="autoComplete1" TargetControlID="txtNoteAutoComplete"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaNote"
                                                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        OnClientItemSelected="noteSelected">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <cc1:CustomTextArea ID="TxtNote" runat="server" TextMode="MultiLine" ClientIDMode="static"
                                                            autocomplete="off" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-right-no-margin">
                                                        <span class="charactersAvailable">
                                                            <asp:Literal ID="DocumentLitVisibleNotesChars" runat="server" ClientIDMode="static"></asp:Literal>
                                                            <span id="TxtNote_chars" runat="server" clientidmode="static"></span></span>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <asp:UpdatePanel ID="UpPnlResponseProtocol" runat="server" UpdateMode="Conditional"
                                        ClientIDMode="Static">
                                        <ContentTemplate>
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Label runat="server" ID="DocumentLblAnswer" CssClass="clickableRight"></asp:Label></span>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgGoAnswer" ImageUrl="../Images/Icons/view_answer_documents.png"
                                                            OnMouseOutImage="../Images/Icons/view_answer_documents.png" OnMouseOverImage="../Images/Icons/view_answer_documents_hover.png"
                                                            CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/view_answer_documents_disabled.png"
                                                            Visible="false" OnClick="DocumentImgGoAnswer_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgAnswerSearch" ImageUrl="../Images/Icons/search_response_documents.png"
                                                            OnMouseOutImage="../Images/Icons/search_response_documents.png" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                            OnClick="DocumentImgAnswerSearch_Click" />
                                                    </div>
                                                    <asp:PlaceHolder ID="plcAnswerRif" runat="server" Visible="false">
                                                        <div class="row">
                                                            <span class="col-right-no-margin">
                                                                <cc1:CustomImageButton runat="server" ID="DocumentImgDelAnswer" ImageUrl="../Images/Icons/delete.png"
                                                                    ImageAlign="AbsMiddle" OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                                    CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                                    OnClick="DocumentImgDelAnswer_Click" />
                                                            </span>
                                                            <div class="col">
                                                                <p>
                                                                    <asp:Literal ID="litAnswerRif" runat="server" /></p>
                                                            </div>
                                                        </div>
                                                    </asp:PlaceHolder>
                                            </fieldset>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div class="row">
                                    <asp:UpdatePanel ID="UpPnlResponse" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <fieldset>
                                                <asp:PlaceHolder ID="plcViewResponse" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Label runat="server" ID="LblViewResponse" CssClass="clickableRight"></asp:Label></span>
                                                            </p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgViewAnswers" ImageUrl="../Images/Icons/view_response_documents.png"
                                                                OnMouseOutImage="../Images/Icons/view_response_documents.png" OnMouseOverImage="../Images/Icons/view_response_documents_hover.png"
                                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/view_response_documents_disabled.png"
                                                                OnClick="DocumentImgViewAnswers_Click" />
                                                            <div class="col">
                                                                <p>
                                                                    <asp:Literal ID="Literal1" runat="server" /></p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlCreateResponse"></asp:Literal></span>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgAnswerWithProtocol" ImageUrl="../Images/Icons/response_protocol.png"
                                                            OnMouseOutImage="../Images/Icons/response_protocol.png" OnMouseOverImage="../Images/Icons/response_protocol_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/response_protocol_disabled.png"
                                                            OnClick="DocumentImgAnswerWithProtocol_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgAnswerWithPredisposed" ImageUrl="../Images/Icons/response_predisposed.png"
                                                            OnMouseOutImage="../Images/Icons/response_predisposed.png" OnMouseOverImage="../Images/Icons/response_predisposed_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/response_predisposed_disabled.png"
                                                            OnClick="DocumentImgAnswerWithPredisposed_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgAnswerWithDocument" ImageUrl="../Images/Icons/response_document.png"
                                                            OnMouseOutImage="../Images/Icons/response_document.png" OnMouseOverImage="../Images/Icons/response_document_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/response_document_disabled.png"
                                                            OnClick="DocumentImgAnswerWithDocument_Click" />
                                                    </div>
                                            </fieldset>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <asp:Panel CssClass="row" runat="server" ID="pnlPjct">
                                    <fieldset>
                                        <asp:UpdatePanel runat="server" ID="UpPnlPrimaryClassification" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:PlaceHolder ID="pnl_fasc_Primaria" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="ltlPrimaryFascText"></asp:Literal></span>&nbsp;<asp:Label
                                                                        ID="lblPrimaryFascDescr" runat="server">*</asp:Label></p>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="DocumentLitClassificationRapid"></asp:Literal></span><asp:Label
                                                            ID="LblClassRequired" CssClass="little" Visible="false" runat="server">*</asp:Label></p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <asp:UpdatePanel ID="pPnlBUttonsProject" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                            OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                            OnClientClick="return ajaxModalPopupOpenTitolario();" CssClass="clickableLeftN"
                                                            ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png" />
                                                        <cc1:CustomImageButton ID="DocumentImgSearchProjects" ImageUrl="../Images/Icons/search_projects.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/search_projects_hover.png" OnMouseOutImage="../Images/Icons/search_projects.png"
                                                            ImageUrlDisabled="../Images/Icons/search_projects_disabled.png" alt="Titolario"
                                                            CssClass="clickableLeftN" OnClick="DocumentImgSearchProjects_Click" />
                                                        <cc1:CustomImageButton ID="DocumentImgNewProject" ImageUrl="../Images/Icons/add_sub_folder.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/add_sub_folder_hover.png" OnMouseOutImage="../Images/Icons/add_sub_folder.png"
                                                            ImageUrlDisabled="../Images/Icons/add_sub_folder_disabled.png" alt="Titolario"
                                                            CssClass="clickableLeftN" OnClick="DocumentImgNewProject_Click" Visible="false" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:PlaceHolder runat="server" ID="PnlProject">
                                                        <asp:HiddenField ID="IdProject" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft"
                                                                AutoComplete="off" OnTextChanged="TxtCodeProject_OnTextChanged" AutoPostBack="true"
                                                                CssClassReadOnly="txt_addressBookLeft_disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_addressBookRight"
                                                                    autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled">
                                                                </cc1:CustomTextArea>
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
                                                    <asp:HiddenField ID="HiddenPublicFolderTypeOperation" runat="server" ClientIDMode="Static" />
                                                    <asp:HiddenField ID="HiddenPublicFolder" runat="server" ClientIDMode="Static" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <asp:PlaceHolder runat="server" ID="PnlRapidTransmission">
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="DocumentLitTransmRapid" runat="server"></asp:Literal></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <asp:UpdatePanel runat="server" ID="UpPnlTransmissionsModel" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div class="col-full">
                                                            <p>
                                                                <asp:DropDownList ID="DocumentDdlTransmissionsModel" runat="server" CssClass="chzn-select-deselect"
                                                                    Width="100%">
                                                                </asp:DropDownList>
                                                            </p>
                                                        </div>
                                                        <asp:HiddenField runat="server" ID="HiddenControlPrivateTrans" ClientIDMode="Static" />
                                                        <asp:HiddenField runat="server" ID="HiddenControlPrivateClass" ClientIDMode="Static" />
                                                        <asp:HiddenField runat="server" ID="HiddenControlPrivateTypeOperation" ClientIDMode="Static" />
                                                        <asp:HiddenField runat="server" ID="HiddenControlPrivateHierarchyTransmission" ClientIDMode="Static" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </asp:PlaceHolder>
                                    </fieldset>
                                </asp:Panel>
                                <asp:UpdatePanel ID="UpPnlDocType" runat="server">
                                    <ContentTemplate>
                                        <fieldset class="azure">
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight"><a name="anchorType">
                                                            <asp:Literal ID="DocumentLitTypeDocument" runat="server"></asp:Literal></a></span><asp:Label ID="lblTypeDocRequired" CssClass="little" runat="server" Visible="false">*</asp:Label></p>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <cc1:CustomImageButton runat="server" ID="DocumentImgHistoryTipology" ImageUrl="../Images/Icons/obj_history.png"
                                                        OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                        CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                        OnClick="DocumentImgHistory_Click" Visible="false" />
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
                                                                    <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" OnSelectedIndexChanged="DocumentDdlTypeDocument_OnSelectedIndexChanged"
                                                                        AutoPostBack="True" CssClass="chzn-select-deselect" Width="100%">
                                                                        <asp:ListItem Text=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </p>
                                                        </div>
                                                    </div>                                                    
                                                    <asp:PlaceHolder ID="PnlTypeDocument" runat="server"></asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                        <div class="row">
                                                            <div class="col">
                                                                <p>
                                                                    <span class="weight">
                                                                        <asp:Literal ID="DocumentLitStateDiagram" runat="server"></asp:Literal>&nbsp;
                                                                        <asp:Literal ID="LitActualStateDiagram" runat="server"></asp:Literal></span></p>
                                                            </div>
                                                            <div class="col-right-no-margin">
                                                                <cc1:CustomImageButton runat="server" ID="DocumentImgHistoryState" ImageUrl="../Images/Icons/obj_history.png"
                                                                    OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                                    CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_history_disabled.png"
                                                                    OnClick="DocumentImgHistory_Click" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <p>
                                                                    <div class="styled-select_full">
                                                                        <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" CssClass="chzn-select-deselect"
                                                                            OnSelectedIndexChanged="DocumentDdlStateDiagram_SelectedIndexChanged" AutoPostBack="true" Width="100%">
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <asp:PlaceHolder runat="server" ID="PnlDocumentStateDiagramDate" Visible="false">
                                                            <div class="row">
                                                                <asp:UpdatePanel runat="server" ID="UpPnlScadenza" UpdateMode="Conditional">
                                                                    <ContentTemplate>
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
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </fieldset>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="UpPnlKeywords" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlKeywpords" runat="server" Visible="false">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="DocumentLitKeyword" runat="server"></asp:Literal></span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton ID="DocumentImgSelectKeyword" ImageUrl="../Images/Icons/obj_objects.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/obj_objects_hover.png" OnMouseOutImage="../Images/Icons/obj_objects.png"
                                                            ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png" CssClass="clickableLeftN"
                                                            OnClick="DocumentImgSelectKeyword_Click" />
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgDeleteKeyword" ImageUrl="../Images/Icons/delete.png"
                                                            OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                            OnClick="DocumentImgDeleteKeyword_Click" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <asp:ListBox ID="ListKeywords" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled">
                                                    </asp:ListBox>
                                                </div>
                                            </fieldset>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="UpPnlMainDoc" ClientIDMode="Static" runat="server" UpdateMode="Conditional"
                                    Visible="false">
                                    <ContentTemplate>
                                        <fieldset>
                                             <div class="row">
                                                <asp:Label ID="lblMainDoc" runat="server" Style="float: left; line-height: 20px;" />
                                                <div style="padding-left: 10px; float: left; line-height: 20px;">
                                                    <cc1:CustomImageButton runat="server" ID="imgMainDoc" ImageUrl="../Images/Icons/view_response_documents.png"
                                                                OnMouseOutImage="../Images/Icons/view_response_documents.png" OnMouseOverImage="../Images/Icons/view_response_documents_hover.png"
                                                                ImageUrlDisabled="../Images/Icons/view_response_documents_disabled.png"
                                                        OnClick="imgMainDoc_Click" CssClass="clickableLeftN" />
                                                </div>
                                            </div>
                                        </fieldset>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <uc5:ViewDocument ID="ViewDocument" runat="server" PageCaller="DOCUMENT"></uc5:ViewDocument>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpHiddenField">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddenSendNoFile" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- end of container -->
    <div id="UploadLiveuploads"  runat="server" Visible="false">
        <div class="upload-dialog" id="upload-liveuploads" data-bind="template: { name: 'template-uploads' }"></div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel runat="server" ID="UpDocumentButtons" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="DocumentBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnSave_Click" />
            <cc1:CustomButton ID="DocumentBtnCreateDocument" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="DocumentBtnCreateDocument_Click" />
            <cc1:CustomButton ID="DocumentBntRecord" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBntRecord_Click" OnClientClick="return verifica();" />
            <cc1:CustomButton ID="DocumentBtnRepeat" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnRepeat_Click" />
            <cc1:CustomButton ID="DocumentBtnForward" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnForward_Click" />
            <cc1:CustomButton ID="DocumentBtnSend" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover"  OnClick="DocumentBtnSend_Click" />
            <cc1:CustomButton ID="DocumentBtnTransmit" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnTransmit_Click" />
            <cc1:CustomButton ID="DocumentBtnAdL" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnAdL_Click" />
            <cc1:CustomButton ID="DocumentBtnAdLRole" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnAdLRole_Click" />
            <cc1:CustomButton ID="DocumentBtnPrepared" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnPrepared_Click" />
            <cc1:CustomButton ID="DocumentBtnPrint" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnPrint_Click" />
            <cc1:CustomButton ID="DocumentBtnRemove" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnRemove_Click" />
            <cc1:CustomButton ID="DocumentBtnUndo" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnUndo_Click" />
            <cc1:CustomButton ID="DocumentBtnConsolid" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="DocumentBtnConsolid_Click" />
            <%--<cc1:CustomButton ID="DocumentBtnAccept" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick="return ajaxModalPopupMassiveTransmissionAccept();" Visible ="false" />--%>
            <cc1:CustomButton ID="DocumentBtnAccept" runat="server" CssClass="btnEnable1" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover1" OnClick="DocumentBtnAccept_Click" Visible ="false" />
            <cc1:CustomButton ID="DocumentBtnView" runat="server" CssClass="btnEnable1" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover1" OnClick="DocumentBtnView_Click" Visible="false" />
            <asp:Button ID="btnSenderPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnSenderPostback_Click" />
            <asp:Button ID="BtnVerifyPreviousViewer" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="BtnVerifyPreviousViewer_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnLinkCustom" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnLinkCustom_Click" />
            <asp:Button ID="btnReportSpedizioniPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnReportSpedizioniPostback_Click" />
            <asp:Button ID="btnChangeTabNewDocument" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnChangeTabNewDocument_Click" />
            <asp:HiddenField runat="server" ID="HiddenControlRepeatCopyDocuments" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="HiddenControlRepeatCopyDocuments2" />
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
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({
            allow_single_deselect: true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
    <script type="text/javascript">
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function (s, e) {
            DocumentDragAndDropMain();
        });
        </script>
</asp:Content>
