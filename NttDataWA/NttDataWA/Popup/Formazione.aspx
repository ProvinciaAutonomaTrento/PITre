<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Formazione.aspx.cs" Inherits="NttDataWA.Popup.Formazione"
    MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        function closePopupAddressBook() {
            $('#btnAddressBookPostback').click();
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

            var searchText = $get('<%=txtDescrizioneUO.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneUO.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneUO.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceUO.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneUO.ClientID%>").value = descrizione;

            document.getElementById("<%=btnUO.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }


    </script>
    <style type="text/css">
        #container {
            padding: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div class="contentAddressBook">
        <div id="topContentPopupSearch">
            <asp:UpdatePanel ID="UpnlFormazioneTab" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                <ContentTemplate>
                    <ul>
                        <li class="addressTab" id="liPulisciUO" runat="server">
                            <asp:LinkButton runat="server" ID="LnkPulisciUO" OnClick="LnkPulisciUO_Click"
                                OnClientClick="disallowOp('ContentPlaceHolderContent')"></asp:LinkButton></li>
                        <li class="otherAddressTab" id="liPopolaUO" runat="server">
                            <asp:LinkButton runat="server" ID="LnkPopolaUO" OnClick="LnkPopolaUO_Click"
                                OnClientClick="disallowOp('ContentPlaceHolderContent')"></asp:LinkButton></li>
                    </ul>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="centerContentAddressbook">
            <div id="contentTab">
                <div class="row">
                    <asp:UpdatePanel ID="UpnlMessagge" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                        <ContentTemplate>
                            <p style="text-align:right;" >
                                 <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="../xml/importDocumentiUO.xls" Target="_blank" Text="Scarica Foglio Excel" />

                            </p>
                            <p>
                                <span class="weight">
                                <asp:Label ID="LblMessage" runat="server"></asp:Label></span>
                            </p>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpnlUO" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:Panel ID="PnlUO" runat="server">
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal ID="ltlUO" runat="server" /></span>
                                        </p>
                                    </div>
                                    <div class="col-right-no-margin">
                                        <cc1:CustomImageButton runat="server" ID="ImgAddressBookUO" ImageUrl="../Images/Icons/address_book.png"
                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="ImgAddressBook_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:HiddenField ID="idUO" runat="server" />
                                    <div class="colHalf">

                                        <cc1:CustomTextArea ID="txtCodiceUO" runat="server" CssClass="txt_addressBookLeft"
                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCodiceUO_OnTextChanged"
                                            AutoComplete="off" onchange="disallowOp('ContentPlaceHolderContent');">
                                        </cc1:CustomTextArea>
                                    </div>
                                    <div class="colHalf2">
                                        <div class="colHalf3">
                                            <cc1:CustomTextArea ID="txtDescrizioneUO" runat="server" CssClass="txt_projectRight defaultAction"
                                                CssClassReadOnly="txt_ProjectRight_disabled">
                                            </cc1:CustomTextArea>
                                        </div>
                                    </div>
                                </div>

                                


                                <asp:Button ID="btnUO" runat="server" Text="vai" Style="display: none;" />
                                <uc1:AutoCompleteExtender runat="server" ID="RapidUO" TargetControlID="txtDescrizioneUO"
                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                    CompletionListHighlightedItemCssClass="singl    e_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                    OnClientPopulated="acePopulated">
                                </uc1:AutoCompleteExtender>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="updateTest" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <%--<div id="boxCleanDocuments" runat="server" clientidmode="Static" class="btnEnable" style="background-position-y: -3px; background-size: cover; width: 145px; border-radius: 5px; cursor: pointer;">
                                <span>Elimina Documenti</span>
                            </div>
                            <p id="txtDocumentiEliminati" runat="server" clientidmode="Static"></p>--%>

                            <!-- Upload File class="ie9HiddenField" -->
                            <div id="boxShowHide" runat="server" clientidmode="Static" class="ie9HiddenField" >
                                 <!-- Template -->
                                <div style="width: 30%; float: left;">
                                    <div  style="margin: auto;">

                                    <div id="boxUploadTemplateFormazione"><!-- box per il widget -->
                                        <!-- teplate upload rispettare classi -->
                                        <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important; text-align:center;">
                                            <label id="btnUploadTemplate" class="fileinput-button">
                                                <span>Carica Foglio Excel</span>
                                            </label>
                                            <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                        </div>
                                        <p id="txtStatusTemplate" style="text-align: center;">non caricato</p>
                                    </div>
                                        
                                    </div>
                                </div>

                                 <!-- Documenti -->
                                <div style="width: 35%; float:left;">
                                    <div style="margin: auto;">
                                    <div id="boxUploadDocumentiFormazione"><!-- box per il widget -->
                                        <!-- teplate upload rispettare classi -->
                                        <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important; text-align:center;">
                                            <label id="btnUploadDocuments" class="fileinput-button">
                                                <span>Carica Documenti</span>
                                            </label>
                                            <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                        </div>
                                        <div class="fileupload-content" style="margin: 0 7px 0 5px;">
                                            <table class="files"></table>
                                        </div>
                                    </div>
                                        </div>
                                </div>

                                <!-- Allegati -->
                                <div style="width: 34%; float: left;">
                                    <div  style="margin: auto;">
                                    <div id="boxUploadAllegatiFormazione"><!-- box per il widget -->
                                        <!-- teplate upload rispettare classi -->
                                        <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important; text-align:center;">
                                            <label id="btnUploadAttachments" class="fileinput-button">
                                                <span>Carica Allegati</span>
                                            </label>
                                            <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                        </div>
                                        <div class="fileupload-content" style="margin: 0 7px 0 5px;">
                                            <table class="files"></table>
                                        </div>

                                    </div>
                                        </div>
                                </div>

                               


                                <div style="clear: left;"></div>

                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
    
                    

                </div>
            </div>
        </div>
    </div>
    


    

    

     <script id="template-upload" type="text/x-jquery-tmpl">
                    <tr class="template-upload{{if error}} ui-state-error{{/if}}">
                        <td class="name">${name}</td>
                        <td class="size">${sizef}</td>
                        {{if error}}
                        <td class="error" colspan="2">Error:
                            {{if error === 'maxFileSize'}}File is too big
                            {{else error === 'minFileSize'}}File is too small
                            {{else error === 'acceptFileTypes'}}Filetype not allowed
                            {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                            {{else}}${error}
                            {{/if}}
                        </td>
                        {{else}}
                        <td class="progress">
                            <div></div>
                        </td>
                        <td class="start">
                            <button>Start</button></td>
                        {{/if}}
                    <td class="cancel">
                        <button>Cancel</button></td>
                    </tr>
                </script>
                <script id="template-download" type="text/x-jquery-tmpl">
                     <tr class="template-download{{if error}} ui-state-error{{/if}}">
                        {{if error}}
                            <td></td>
                            <td class="name">${name}</td>
                            <td class="size">${sizef}</td>
                            <td class="error" colspan="2">Error:
                                {{if error === 1}}File exceeds upload_max_filesize (php.ini directive)
                                {{else error === 2}}File exceeds MAX_FILE_SIZE (HTML form directive)
                                {{else error === 3}}File was only partially uploaded
                                {{else error === 4}}No File was uploaded
                                {{else error === 5}}Missing a temporary folder
                                {{else error === 6}}Failed to write file to disk
                                {{else error === 7}}File upload stopped by extension
                                {{else error === 'maxFileSize'}}File is too big
                                {{else error === 'minFileSize'}}File is too small
                                {{else error === 'acceptFileTypes'}}Filetype not allowed
                                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                                {{else error === 'uploadedBytes'}}Uploaded bytes exceed file size
                                {{else error === 'emptyResult'}}Empty file upload result
                                {{else}}${error}
                                {{/if}}
                            </td>
                        {{else}}
                            <%--<td class="preview">
                                {{if thumbnail_url}}
                                    <!--<a href=".${url}" target="_blank">-->
                                        <img src=".${thumbnail_url}" style="height: 25px;width: 25px;">

                                    <!--</a>-->
                                {{/if}}
                            </td>--%>
                            <td class="name">
                                ${name}
                                <!-- <a href=".${url}"{{if thumbnail_url}} target="_blank"{{/if}}>${name}</a> -->
                            </td>
                            <td class="size">${sizef}</td>
                            <td colspan="2"></td>
                        {{/if}}
                        <%--<td class="delete">
                            <button data-type="${delete_type}" data-url="${delete_url}">Delete</button>
                        </td>--%>
                    </tr>
                </script>   

    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.tmpl.js?v=3") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.tmplPlus.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.iframe-transport.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.fileupload.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.fileupload-ui.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/uploadFormazione.js?v=25") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="FormazioneBtnConfirm" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="FormazioneBtnConfirm_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <cc1:CustomButton ID="FormazioneBtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="FormazioneBtnCancel_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />


        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>

