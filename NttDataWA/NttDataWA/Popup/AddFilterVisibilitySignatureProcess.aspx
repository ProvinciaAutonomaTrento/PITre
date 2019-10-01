<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFilterVisibilitySignatureProcess.aspx.cs" 
    Inherits="NttDataWA.Popup.AddFilterVisibilitySignatureProcess"  MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    #container3 {
        position: fixed;
        top: 1px;
        left: 0px;
        bottom: 71px;
        right: 0px;
        overflow: auto;
        background: #eeeeee;
        text-align: left;
        padding: 2px;
        height: 85%;
        margin: 5px;
        padding: 10px;
    }
</style>
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

            var searchText = $get('<%=txtDescrizioneRuolo.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneRuolo.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneRuolo.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceRuolo.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneRuolo.ClientID%>").value = descrizione;

            document.getElementById("<%=btnRuolo.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

    function closeAddressBookPopup() {
        $('#btnAddressBookPostback').click();
    }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="container3">
        <div class="row3">
            <asp:UpdatePanel ID="UpdPnlRuolo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <p>
                                <span class="weight">
                                    <asp:Literal ID="ltlRuolo" runat="server" /></span>
                            </p>
                        </div>
                        <div class="col-right-no-margin">
                            <cc1:CustomImageButton runat="server" ID="ImgAddressBookRuolo" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                OnClick="BtnAddressBookRuolo_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:HiddenField ID="idRuolo" runat="server" />
                        <div class="colHalf">
                            <cc1:CustomTextArea ID="txtCodiceRuolo" runat="server" CssClass="txt_addressBookLeft"
                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoComplete="off"
                                onchange="disallowOp('ContentPlaceHolderContent');" OnTextChanged="TxtCode_OnTextChanged">
                            </cc1:CustomTextArea>
                        </div>
                        <div class="colHalf2">
                            <div class="colHalf3">
                                <cc1:CustomTextArea ID="txtDescrizioneRuolo" runat="server" CssClass="txt_addressBookRight"
                                    CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                            </div>
                        </div>
                    </div>
                    <asp:Button ID="btnRuolo" runat="server" Text="vai" Style="display: none;" />
                    <uc1:AutoCompleteExtender runat="server" ID="RapidRuolo" TargetControlID="txtDescrizioneRuolo"
                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                        UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                        OnClientPopulated="acePopulated">
                    </uc1:AutoCompleteExtender>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
        <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="AddFilterBtnConfirm" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterBtnConfirm_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <cc1:CustomButton ID="AddFilterBtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterBtnCancel_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>