<%@ Page Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="dataentry_project.aspx.cs" Inherits="NttDataWA.Transmission.dataentry_project"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/HeaderProject.ascx" TagPrefix="uc2" TagName="HeaderProject" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="ProjectButtons" %>
<%@ Register Src="~/UserControls/ProjectTabs.ascx" TagPrefix="uc4" TagName="ProjectTabs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
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

            var searchText = $get('<%=TxtDescriptionRecipient.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionRecipient.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionRecipient.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeRecipientTransmission.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionRecipient.ClientID%>").value = descrizione;

            document.getElementById("<%=btnRecipient.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

        function chkSelAll(obj) {
            $('input[id ^= "chkNotifica_' + obj.id.split('_')[1] + '_"]').attr('checked', obj.checked);
        }

        function chkNotifica(obj) {
            if (!obj.checked) $('input[id ^= "chkSelAll_' + obj.id.split('_')[1] + '_"]').attr('checked', false);
        }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="TemplateSave" runat="server" Url="../popup/TransmissionTemplate_save.aspx"
        Width="400" Height="300" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlTransmissionsModel', '');}" />
    <uc:ajaxpopup Id="TemplateSaveNewOwner" runat="server" Url="../popup/TransmissionTemplate_saveNewOwner.aspx"
        Width="500" Height="600" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlTransmissionsModel', '');}" />
    <uc:ajaxpopup Id="SaveNewOwner" runat="server" Url="../popup/Transmission_saveNewOwner.aspx"
        Width="500" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlTransmissionSaved', '');}" />
    <uc:ajaxpopup Id="TransmitNewOwner" runat="server" Url="../popup/Transmission_saveNewOwner.aspx"
        Width="500" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlTransmissionSaved', '');}" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <div id="containerTop">
        <asp:UpdatePanel ID="UpUserControlHeaderDocument" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <uc2:HeaderProject runat="server" ID="HeaderProject" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpcontainerDocumentTabLeftBorder" runat="server" UpdateMode="Conditional"
            ClientIDMode="static">
            <ContentTemplate>
                <div id="containerDocumentTab" runat="server" clientidmode="Static" class="containerProjectTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerProjectTabOrangeSx">
                            <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc4:ProjectTabs runat="server" PageCaller="TRANSMISSIONS" ID="ProjectTabs"></uc4:ProjectTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
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
                        <div class="box_inside">
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <span class="weight">
                                                <asp:Literal ID="litDescriptionText" runat="server" /></span>
                                        </div>
                                    </div>
                                    <div id="row_object" class="row">
                                        <asp:Literal ID="litDescription" runat="server" /></div>
                                </fieldset>
                            </div>
                            <asp:UpdatePanel runat="server" ID="upPnlGeneral" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <!-- TITLE -->
                                    <h1 id="pageTitle">
                                        <asp:Literal ID="litPageTitle" runat="server" /></h1>
                                    <!-- REASON -->
                                    <div class="row">
                                        <fieldset>
                                            <asp:UpdatePanel ID="uppnlReason" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal ID="TransmissionLitReason" runat="server" /></span>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <asp:Image ID="imgReason" runat="server" ImageUrl="~/Images/Icons/info_1.png" onmouseover="this.src = this.src.replace('.png', '_hover.png');"
                                                                onmouseout="this.src = this.src.replace('_hover.png', '.png');" AlternateText=""
                                                                ToolTip="" CssClass="clickable" />
                                                        </div>
                                                    </div>
                                                    <div class="row-full">
                                                        <div class="col-full">
                                                            <p>
                                                                <asp:DropDownList ID="ddlReason" runat="server" CssClass="chzn-select-deselect" Width="98%"
                                                                    onchange="__doPostBack('uppnlReason', '');">
                                                                    <asp:ListItem Text=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </p>
                                                        </div>
                                                        <div>
                                                            <p id="parTransferRights" runat="server">
                                                                <asp:CheckBox ID="chkTransferRights" runat="server" AutoPostBack="true" OnCheckedChanged="chkTransferRights_CheckedChanged" /></p>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </fieldset>
                                    </div>
                                    <!-- RECIPIENT -->
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="TransmissionLitRecipient" runat="server" /></span></p>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <asp:UpdatePanel runat="server" ID="UpPnlIconAddress" UpdateMode="Conditional" ClientIDMode="Static">
                                                        <ContentTemplate>
                                                            <cc1:CustomImageButton ID="dtEntryDocImgAddressBookUser" ImageUrl="../Images/Icons/address_book.png"
                                                                runat="server" OnMouseOverImage="../Images/Icons/address_book_hover.png" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnMouseOutImage="../Images/Icons/address_book.png" CssClass="clickable" OnClick="dtEntryDocImgAddressBookUser_Click" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <asp:UpdatePanel runat="server" ID="UpPnlRecipient" UpdateMode="Conditional" ClientIDMode="Static">
                                                    <ContentTemplate>
                                                        <asp:HiddenField ID="IdRecipient" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="TxtCodeRecipientTransmission" runat="server" CssClass="txt_addressBookLeft"
                                                                CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                autocomplete="off" AutoPostBack="true"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="TxtDescriptionRecipient" runat="server" CssClass="txt_addressBookRight"
                                                                    CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                        <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="TxtDescriptionRecipient"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                            OnClientPopulated="acePopulated ">
                                                        </uc1:AutoCompleteExtender>
                                                        <asp:Button ID="btnRecipient" runat="server" Text="vai" Style="display: none;" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </fieldset>
                                    </div>
                                    <!-- NOTE -->
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal ID="TransmissionLitNote" runat="server" /></span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <cc1:CustomTextArea ID="TxtNote" runat="server" TextMode="MultiLine" CssClass="txt_textarea" /></div>
                                        </fieldset>
                                    </div>
                                    <!-- TEMPLATES -->
                                    <asp:UpdatePanel ID="upPnlTransmissionTemplates" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:PlaceHolder runat="server" ID="PnlTransmissionTemplates">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="row">
                                                            <div class="col">
                                                                <span class="weight">
                                                                    <asp:Literal ID="TransmissionLitTemplates" runat="server" /></span>
                                                            </div>
                                                        </div>
                                                        <div class="row-full">
                                                            <div class="col-full">
                                                                <p>
                                                                    <asp:UpdatePanel ID="upPnlTransmissionsModel" runat="server" UpdateMode="Conditional"
                                                                        ClientIDMode="Static">
                                                                        <ContentTemplate>
                                                                            <asp:DropDownList ID="DdlTransmissionsModel" runat="server" CssClass="chzn-select-deselect"
                                                                                Width="98%" onchange="__doPostBack('upPnlTransmissionsModel', '');">
                                                                                <asp:ListItem Text=""></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </ContentTemplate>
                                                                    </asp:UpdatePanel>
                                                                </p>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="upPnlTransmissionBuilt" runat="server" UpdateMode="Conditional"
                            ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="plcTransmissions" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upPnlTransmissionSaved" runat="server" UpdateMode="Conditional"
                            ClientIDMode="Static">
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="panelButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <asp:HiddenField ID="extend_visibility" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="proceed_private" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="proceed_ownership" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="final_state" runat="server" ClientIDMode="Static" />
            <cc1:CustomButton ID="TransmissionsBtnTransmit" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnTransmit_Click"
                Enabled="false" />
            <cc1:CustomButton ID="TransmissionsBtnRemove" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnRemove_Click"
                Enabled="false" />
            <cc1:CustomButton ID="TransmissionsBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnSave_Click"
                Enabled="false" />
            <cc1:CustomButton ID="TransmissionsBtnSaveTemplate" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnSaveTemplate_Click"
                Enabled="false" />
            <asp:Button ID="btnGoTransmissions" runat="server" ClientIDMode="Static" OnClick="btnGoTransmissions_Click"
                Style="display: none" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
