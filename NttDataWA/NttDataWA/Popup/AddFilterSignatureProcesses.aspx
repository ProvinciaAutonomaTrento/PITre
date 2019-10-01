<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFilterSignatureProcesses.aspx.cs"
    Inherits="NttDataWA.Popup.AddFilterSignatureProcesses" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
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

        .col-marginSx2 {
            float: left;
            width:15%;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }

        .col-marginSx2 p {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 4px;
        }     
        .colonnasx5
        {
            float: left;
            width: 40%;
            margin-bottom: 3px;
        }

        .colonnadx5
        {
            float:left;
            width: 30%;
            margin-bottom: 5px;
        }

        .row {
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }

        .row3 {
            padding-top: 10px;
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
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

            var searchText = $get('<%=txtDescrizioneRuoloTitolare.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneRuoloTitolare.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneRuoloTitolare.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceRuoloTitolare.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneRuoloTitolare.ClientID%>").value = descrizione;

            document.getElementById("<%=btnRuoloTitolare.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

        function acePopulatedUser(sender, e) {
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

        function aceSelectedUser(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneUtenteTitolare.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneUtenteTitolare.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneUtenteTitolare.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceUtenteTitolare.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneUtenteTitolare.ClientID%>").value = descrizione;

            document.getElementById("<%=btnUtenteTitolare.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

        function closeAddressBookPopup() {
            $('#btnAddressBookPostback').click();
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="container3">
        <div class="row3">
            <%-- ************** ORDINAMENTO PROCESSI******************** --%>
            <asp:Panel ID="PnlOrdinamento" runat="server">
                <div class="row">
                    <div class="col-marginSx2">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="LtlOrdinaPer" runat="server" /></span>
                        </p>
                    </div>
                    <div class="colonnasx5">
                        <asp:DropDownList ID="ddlOrder" CssClass="chzn-select-deselect" runat="server" Width="90%">
                        </asp:DropDownList>
                    </div>
                    <div class="colonnadx5">
                        <asp:DropDownList ID="ddlAscDesc" runat="server" CssClass="chzn-select-deselect" Width="90%">
                            <asp:ListItem id="li_asc" Value="Asc" runat="server" />
                            <asp:ListItem id="li_desc" Value="Desc" runat="server" Selected="True"/>
                        </asp:DropDownList>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div class="row3">
            <%-- ************** RUOLO TITOLARE******************** --%>
            <asp:UpdatePanel ID="UpdPnlRuoloTitolare" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <p>
                                <span class="weight">
                                    <asp:Literal ID="ltlRuoloTitolare" runat="server" /></span>
                            </p>
                        </div>
                        <div class="col-right-no-margin">
                            <cc1:CustomImageButton runat="server" ID="ImgAddressBookRuoloTitolare" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                OnClick="BtnAddressBookRuoloTitolare_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:HiddenField ID="idRuoloTitolare" runat="server" />
                        <div class="colHalf">
                            <cc1:CustomTextArea ID="txtCodiceRuoloTitolare" runat="server" CssClass="txt_addressBookLeft"
                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoComplete="off"
                                onchange="disallowOp('ContentPlaceHolderContent');" OnTextChanged="TxtCode_OnTextChanged">
                            </cc1:CustomTextArea>
                        </div>
                        <div class="colHalf2">
                            <div class="colHalf3">
                                <cc1:CustomTextArea ID="txtDescrizioneRuoloTitolare" runat="server" CssClass="txt_addressBookRight"
                                    CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                            </div>
                        </div>
                    </div>
                    <asp:Button ID="btnRuoloTitolare" runat="server" Text="vai" Style="display: none;" />
                    <uc1:AutoCompleteExtender runat="server" ID="RapidRuoloTitolare" TargetControlID="txtDescrizioneRuoloTitolare"
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
        <div class="row3">
            <%-- ************** UTENTE TITOLARE******************** --%>
            <asp:UpdatePanel ID="UpdPnlUtenteTitolare" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <p>
                                <span class="weight">
                                    <asp:Literal ID="ltlUtenteTitolare" runat="server" /></span>
                            </p>
                        </div>
                        <div class="col-right-no-margin">
                            <cc1:CustomImageButton runat="server" ID="ImgAddressBookUtenteTitolare" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                OnClick="BtnAddressBookUtenteTitolare_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:HiddenField ID="idUtenteTitolare" runat="server" />
                        <div class="colHalf">
                            <cc1:CustomTextArea ID="txtCodiceUtenteTitolare" runat="server" CssClass="txt_addressBookLeft"
                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoComplete="off"
                                onchange="disallowOp('ContentPlaceHolderContent');" OnTextChanged="TxtCode_OnTextChanged">
                            </cc1:CustomTextArea>
                        </div>
                        <div class="colHalf2">
                            <div class="colHalf3">
                                <cc1:CustomTextArea ID="txtDescrizioneUtenteTitolare" runat="server" CssClass="txt_addressBookRight"
                                    CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled">
                                </cc1:CustomTextArea>
                            </div>
                        </div>
                    </div>
                    <asp:Button ID="btnUtenteTitolare" runat="server" Text="vai" Style="display: none;" />
                    <uc1:AutoCompleteExtender runat="server" ID="RapidUtenteTitolare" TargetControlID="txtDescrizioneUtenteTitolare"
                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                        UseContextKey="true" OnClientItemSelected="aceSelectedUser" BehaviorID="AutoCompleteExIngressoBIS"
                        OnClientPopulated="acePopulatedUser">
                    </uc1:AutoCompleteExtender>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="row3">
            <%-- ************** TIPO ******************** --%>
            <asp:Panel ID="pnlTipo" runat="server">
                <div class="row">
                    <div class="col-marginSx2">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="ltlTipo" runat="server" /></span>
                        </p>
                    </div>
                    <div class="col">
                        <asp:CheckBoxList ID="cbl_TipoProcesso" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                            <asp:ListItem Value="P" runat="server" id="opProcesso" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="M" runat="server" id="opModello" Selected="True"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div class="row3">
            <%-- ************** NOME ******************** --%>
            <asp:Panel ID="pnlNome" runat="server">
                <div class="row">
                    <div class="col">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="ltlNome" runat="server" /></span>
                        </p>
                    </div>
                    <div class="row">
                        <div class="col" style="width: 80%">
                            <cc1:CustomTextArea ID="txtNome" runat="server" CssClass="txt_projectRight defaultAction"
                                CssClassReadOnly="txt_ProjectRight_disabled">
                            </cc1:CustomTextArea>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div class="row3">
            <%-- ************** STATO ******************** --%>
            <asp:Panel ID="pnlStato" runat="server">
                <div class="row">
                    <div class="col-marginSx2">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="ltlStato" runat="server" /></span>
                        </p>
                    </div>
                    <div class="col">
                        <asp:CheckBoxList ID="cbl_StatoProcesso" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                            <asp:ListItem Value="V" runat="server" id="opValido" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="I" runat="server" id="opInvalido" Selected="True"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
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
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>

