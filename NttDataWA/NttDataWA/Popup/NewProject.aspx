<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="NewProject.aspx.cs" Inherits="NttDataWA.Popup.NewProject" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .row
        {
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
        #container
        {
            padding: 10px;
        }
        
        #container fieldset
        {
            border: 1px solid #c1c1c1;
            margin-top: 0px;
            margin-bottom: 0px;
            margin-left: 5px;
            margin-right: 5px;
            margin-top: 3px;
            padding-left: 5px;
            padding-right: 10px;
            padding-top: 5px;
            padding-bottom: 0px;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
        
        #container fieldset.azure
        {
            border: 1px solid #3399cc;
        }
        #centerContentAddressbook
        {
            border-top: 1px solid #2e82bc;
            border-bottom: 1px solid #2e82bc;
            border-left: 1px solid #2e82bc;
            border-right: 1px solid #2e82bc;
            background-color: #edf4f8;
            float: left;
            width: 100%;
        }
        .txt_textarea
        {
            width: 100%;
            border: 1px solid #cccccc;
            line-height: 18px;
            font-family: Verdana, Arial, Verdana, sans-serif;
            font-size: 13px;
            height: 50px;
            overflow: auto;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
    </style>
    <script type="text/javascript">
        function swapProjectPrivatePublic(id) {
            if (id == '<%=ProjectCheckPrivate.ClientID%>' && $get('<%=ProjectCheckPrivate.ClientID%>').checked) {
                $get('<%=ProjectCheckPublic.ClientID%>').checked = false;
            }
            else {
                $get('<%=ProjectCheckPrivate.ClientID%>').checked = false;
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
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="Note" runat="server" Url="../popup/Note.aspx?type=F" Width="800"
        Height="600" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) { __doPostBack('UpPnlNote', '');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx?from=newProject"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { $('#btnAddressBookPostback').click(); }" />
    <div id="centerContentAddressbook">
        <div style="padding-top: 10px; padding-left: 25px; width: 95%">
            <asp:UpdatePanel runat="server" ID="UpPnlRegistry">
                <ContentTemplate>
                    <div class="row">
                        <div id="PnlRegistryPrj">
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
                                        <asp:CheckBox ID="ProjectCheckPublic" runat="server" CssClass="clickableLeftN" Checked="false"
                                            Visible="false" onclick="swapProjectPrivatePublic(this.id)" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <fieldset>
                    <asp:PlaceHolder runat="server" ID="PnlDescription">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="weight">
                                        <asp:Label runat="server" ID="projectLblDescrizione"> </asp:Label></span><span class="little">*</span></p>
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
                                        CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
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
                                    CssClass="txt_textarea" CssClassReadOnly="txt_textdata_disabled" />
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
            <div class="row">
                <asp:UpdatePanel ID="UpPanelTipologiaFascicolo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <fieldset class="azure" style="width: 98%;">
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <span class="weight"><a name="anchorType">
                                            <asp:Literal ID="projectLblTipoFascicolo" runat="server"></asp:Literal></a></span></p>
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
                                                        AutoPostBack="True" CssClass="chzn-select-deselect" Width="80%" OnChange="disallowOp('ContentPlaceHolderContent')">
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
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <p>
                                                    <div class="styled-select_full">
                                                        <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" CssClass="chzn-select-deselect"
                                                            Width="80%">
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
                    <asp:UpdatePanel runat="server" ID="UpProjectPhisycCollocation" UpdateMode="Conditional"
                        ClientIDMode="static">
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
                                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
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
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="NewProjectSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="NewProjectSave_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <cc1:CustomButton ID="NewProjectClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="NewProjectClose_Click" ClientIDMode="Static"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
