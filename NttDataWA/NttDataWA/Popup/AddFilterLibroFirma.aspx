<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFilterLibroFirma.aspx.cs"
    Inherits="NttDataWA.Popup.AddFilterLibroFirma" MasterPageFile="~/MasterPages/Popup.Master" %>

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
        function closeObjectPopup() {
            $('#btnObjectPostback').click();
        }

        function closeAddressBookPopup() {
            $('#btnAddressBookPostback').click();
        }

        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });

            $('#container3 input, #container3 textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#container3 select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        });

        function acePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoProp');
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

            var searchText = $get('<%=txtDescrizioneProponente.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneProponente.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneProponente.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceProponente.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneProponente.ClientID%>").value = descrizione;

            document.getElementById("<%=btnProponente.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }


        function acePopulatedDest(sender, e) {
            var tbPosition = $common.getLocation($get('<%=txtDescrizioneDest.ClientID %>'));
            var offset = $get('<%=pnlDestAuto.ClientID %>').offsetTop;
            $('<%=pnlDestAuto.ClientID %>').offsetHeight = tbPosition.y + offset;
            var behavior = $find('AutoCompleteExIngressoDest');
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

        function aceSelectedDest(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneDest.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneDest.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneDest.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceDest.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneDest.ClientID%>").value = descrizione;

            document.getElementById("<%=btnDest.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

    </script>
    <style type="text/css">
        #container fieldset
        {
            border: 1px solid #c1c1c1;
            margin-top: 0px;
            margin-bottom: 0px;
            margin-left: 5px;
            margin-right: 5px;
            padding-left: 10px;
            padding-right: 15px;
            padding-top: 5px;
            padding-bottom: 5px;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
            margin-bottom: 5px;
        }
        
        .col-marginSx2
        {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }
        
        .col-marginSx2 p
        {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 4px;
        }
        
        .col-marginSx
        {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }
        
        .col-marginSx p
        {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 8px;
        }
        
        .col-right-no-margin1
        {
            float: right;
            margin: 0px;
            padding: 0px;
            padding-right: 5px;
            margin-top: 5px;
            text-align: right;
        }
        .colHalf2
        {
            float: right;
            margin: 0px;
            text-align: right;
            width: 80%;
        }
        
        .colHalf3
        {
            margin-left: 6px;
            margin-right: 5px;
        }
        
        .row
        {
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
        
        .row3
        {
            padding-top: 10px;
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
        #container3
        {
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
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="container3">
        <div class="row3">
            <asp:UpdatePanel runat="server" ID="UpdatePanelTypeDoc" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <fieldset>
                        <%-- ************** TIPO DOCUMENTO ******************** --%>
                        <asp:UpdatePanel runat="server" ID="UpPnlType" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-marginSx2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlTypeDocument"></asp:Literal></span></p>
                                    </div>
                                    <div class="col">
                                        <asp:CheckBoxList ID="cbl_archDoc_E" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="A" runat="server" id="opArr"></asp:ListItem>
                                            <asp:ListItem Value="P" runat="server" id="opPart"></asp:ListItem>
                                            <asp:ListItem Value="I" runat="server" id="opInt"></asp:ListItem>
                                            <asp:ListItem Value="G" Selected="True" runat="server" id="opGrigio"></asp:ListItem>
                                            <asp:ListItem Value="ALL" Selected="True" runat="server" id="opAll"></asp:ListItem>
                                            <asp:ListItem Value="Pr" Selected="True" id="opPredisposed" runat="server"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- ************** OGGETTO ******************** --%>
                        <div class="row">
                            <div class="col-marginSx">
                                <p>
                                    <span class="weight">
                                        <asp:Literal ID="DocumentLitObject" runat="server"></asp:Literal></span></p>
                            </div>
                            <div class="col-right-no-margin1">
                                <cc1:CustomImageButton runat="server" ID="DocumentImgObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                                    OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                    OnClientClick="return parent.ajaxModalPopupObject();" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-marginSx-full">
                                <div class="full_width">
                                    <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                        <ContentTemplate>
                                            <asp:Panel ID="PnlCodeObject" runat="server" Visible="false">
                                                <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true">
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
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpdatePanelDocument" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <h2 class="expand">
                                <asp:Literal runat="server" ID="SearchDocumentLit"></asp:Literal>
                            </h2>
                            <div id="Div1" class="collapse shown" runat="server">
                                <asp:UpdatePanel runat="server" ID="UpDoc" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <%-- ************** ID DOCUMENTO ******************** --%>
                                        <asp:UpdatePanel runat="server" ID="UpPnlIdDoc" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="LtlIdDoc"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col2">
                                                        <asp:DropDownList ID="ddl_idDoc" runat="server" Width="140px" AutoPostBack="true"
                                                            CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_idDoc_SelectedIndexChanged">
                                                            <asp:ListItem Value="0"></asp:ListItem>
                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Literal runat="server" ID="LtlDaIdDoc"></asp:Literal>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="txt_initIdDoc" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Literal runat="server" ID="LtlAIdDoc"></asp:Literal>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="txt_fineIdDoc" runat="server" Width="80px" Visible="true"
                                                            CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                    </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
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
                                        <%-- ************** TIPOLOGIA ******************** --%>
                                        <div class="row3">
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal ID="SearchDocumentLitTypology" runat="server" /></span>
                                                </div>
                                            </div>
                                            <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" AutoPostBack="True"
                                                            CssClass="chzn-select-deselect" Width="97%">
                                                            <asp:ListItem Text=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpdatePanelElement" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <fieldset>
                        <asp:PlaceHolder ID="plcElement" runat="server">
                            <div class="row">
                                <%-- ************** PROPONENTE******************** --%>
                                <asp:UpdatePanel ID="UpdPnlProponente" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="ltlProponente" runat="server" /></span>
                                                </p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <cc1:CustomImageButton runat="server" ID="ImgAddressBookProponente" ImageUrl="../Images/Icons/address_book.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                    OnClick="ImgAddressBookProponente_Click" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <asp:HiddenField ID="idProponente" runat="server" />
                                            <div class="colHalf">
                                                <cc1:CustomTextArea ID="txtCodiceProponente" runat="server" CssClass="txt_addressBookLeft"
                                                    AutoPostBack="true" OnTextChanged="TxtCode_OnTextChanged" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                    AutoComplete="off" onchange="disallowOp('ContentPlaceHolderContent');">
                                                </cc1:CustomTextArea>
                                            </div>
                                            <div class="colHalf2">
                                                <div class="colHalf3">
                                                    <cc1:CustomTextArea ID="txtDescrizioneProponente" runat="server" CssClass="txt_projectRight defaultAction"
                                                        CssClassReadOnly="txt_ProjectRight_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </div>
                                        <asp:Button ID="btnProponente" runat="server" Text="vai" Style="display: none;" />
                                        <div class="row">
                                            <div class="col-right">
                                                <asp:CheckBox ID="chkProponenteExtendHistoricized" runat="server" Checked="true"
                                                    AutoPostBack="true" OnCheckedChanged="chkDestExtendHistoricized_Click" />
                                            </div>
                                        </div>
                                        <%--<uc1:AutoCompleteExtender runat="server" ID="RapidProponente" TargetControlID="txtDescrizioneProponente"
                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                            MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoProp"
                                            OnClientPopulated="acePopulated">
                                        </uc1:AutoCompleteExtender>--%>
                                        <uc1:AutoCompleteExtender runat="server" ID="RapidProponente" TargetControlID="txtDescrizioneProponente"
                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoProp"
                                            OnClientPopulated="acePopulated">
                                        </uc1:AutoCompleteExtender>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <%-- ************** DATA INSERIMENTO ******************** --%>
                            <asp:UpdatePanel ID="UpDataInserimento" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlDataInserimento"></asp:Literal>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col2">
                                                <asp:DropDownList ID="ddl_DataInserimento" runat="server" AutoPostBack="true" Width="140px"
                                                    CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_DataInserimento_SelectedIndexChanged"
                                                    onchange="disallowOp('Content2');">
                                                    <asp:ListItem Value="0"></asp:ListItem>
                                                    <asp:ListItem Value="1"></asp:ListItem>
                                                    <asp:ListItem Value="2"></asp:ListItem>
                                                    <asp:ListItem Value="3"></asp:ListItem>
                                                    <asp:ListItem Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col2">
                                                <asp:Literal runat="server" ID="ltlDaDataInserimento"></asp:Literal>
                                            </div>
                                            <div class="col4">
                                                <cc1:CustomTextArea ID="txt_Da_dataInserimento" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                            <div class="col2">
                                                <asp:Literal runat="server" ID="ltlADataInserimento"></asp:Literal>
                                            </div>
                                            <div class="col4">
                                                <cc1:CustomTextArea ID="txt_A_dataInserimento" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <%-- ************** TIPO FIRMA RICHIESTA ******************** --%>
                            <asp:UpdatePanel runat="server" ID="UpPnlTypeSignature" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-marginSx2">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="LtlTypeSignature"></asp:Literal></span></p>
                                        </div>
                                        <div class="col">
                                            <asp:CheckBoxList ID="cbxTypeSignature" runat="server" CssClass="testo_grigio" RepeatDirection="Vertical">
                                                <asp:ListItem Value="DC" Selected="True" runat="server" id="opDC"></asp:ListItem>
                                                <asp:ListItem Value="DP" Selected="True" runat="server" id="opDP"></asp:ListItem>
                                                <asp:ListItem Value="ES" Selected="True" runat="server" id="opES"></asp:ListItem>
                                                <asp:ListItem Value="EA" Selected="True" runat="server" id="opEA"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <%-- ************** NOTE ******************** --%>
                            <div class="row">
                                <div class="col-marginSx">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal ID="ltlNote" runat="server"></asp:Literal></span></p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-marginSx-full">
                                    <div class="full_width">
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                            <ContentTemplate>
                                                <cc1:CustomTextArea ID="TxtNote" Width="99%" runat="server" class="txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                            <%-- ************** MODALITA ******************** --%>
                            <div class="row">
                                <div class="col-marginSx2">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="LtlMode"></asp:Literal></span></p>
                                </div>
                                <div class="col">
                                    <asp:CheckBoxList ID="CbxMode" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="A" Selected="True" runat="server" id="CbxModeOpA"></asp:ListItem>
                                        <asp:ListItem Value="M" Selected="True" runat="server" id="CbxModeOpM"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                            <%-- ************** STATO ******************** --%>
                            <asp:UpdatePanel runat="server" ID="UpPnlState" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-marginSx2">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="LtlState"></asp:Literal></span></p>
                                        </div>
                                        <div class="col">
                                            <asp:CheckBoxList ID="CbxState" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="PROPOSTO" Selected="True" runat="server" id="cbxOpP"></asp:ListItem>
                                                <asp:ListItem Value="DA_FIRMARE" Selected="True" runat="server" id="cbxOpF"></asp:ListItem>
                                                <asp:ListItem Value="DA_RESPINGERE" Selected="True" runat="server" id="cbxOpR"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <%-- ************** CON ERRORI/SENZA ERRORI ******************** --%>
                            <asp:UpdatePanel runat="server" ID="UpPnlErrorState" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-marginSx2">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="LtlErrorState"></asp:Literal></span></p>
                                        </div>
                                        <div class="col">
                                            <asp:CheckBoxList ID="CbxErrorState" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="CON_ERRORI" Selected="True" runat="server" id="cbxOpE"></asp:ListItem>
                                                <asp:ListItem Value="SENZA_ERRORI" Selected="True" runat="server" id="cbxOpSE"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:PlaceHolder>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpdatePanelRecord" UpdateMode="Conditional" ClientIDMode="Static">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <h2 class="expand">
                                <asp:Literal runat="server" ID="protocolloLit"></asp:Literal>
                            </h2>
                            <div id="Div2" class="collapse" runat="server">
                                <asp:UpdatePanel runat="server" ID="UpProto" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <%-- ************** NUMERO PROTOCOLLO ******************** --%>
                                        <div class="row3">
                                            <asp:UpdatePanel runat="server" ID="UpPnlIdProto" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlIdProto"></asp:Literal>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col2">
                                                            <asp:DropDownList ID="ddl_idProto" runat="server" Width="140px" AutoPostBack="true"
                                                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_idProto_SelectedIndexChanged">
                                                                <asp:ListItem Value="0"></asp:ListItem>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlDaIdProto"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_initIdProto" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="col2">
                                                            <asp:Literal runat="server" ID="LtlAIdProto"></asp:Literal>
                                                        </div>
                                                        <div class="col4">
                                                            <cc1:CustomTextArea ID="txt_fineIdProto" runat="server" Width="80px" Visible="true"
                                                                CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <%-- ************** DATA PROTOCOLLO ******************** --%>
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
                                        <div class="row">
                                            <%-- ************** MITTENTE/DESTINATARIO ******************** --%>
                                            <asp:UpdatePanel ID="upPnlDest" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="litDest" runat="server" /></span>
                                                            </p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton runat="server" ID="ImgDestAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnClick="ImgDestAddressBook_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:HiddenField ID="idDest" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="txtCodiceDest" runat="server" CssClass="txt_addressBookLeft"
                                                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                AutoCompleteType="Disabled" onchange="disallowOp('ContentPlaceHolderContent');">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div id="pnlDestAuto" runat="server" style="position: relative">
                                                            <div class="colHalf2">
                                                                <div class="colHalf3">
                                                                    <cc1:CustomTextArea ID="txtDescrizioneDest" runat="server" CssClass="txt_addressBookRight"
                                                                        CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                            <uc1:AutoCompleteExtender runat="server" ID="RapidDest" TargetControlID="txtDescrizioneDest"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="aceSelectedDest" BehaviorID="AutoCompleteExIngressoDest"
                                                                OnClientPopulated="acePopulatedDest ">
                                                            </uc1:AutoCompleteExtender>
                                                            <asp:Button ID="btnDest" runat="server" Text="vai" Style="display: none;" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-right">
                                                            <asp:CheckBox ID="chkDestExtendHistoricized" runat="server" Checked="true" AutoPostBack="true"
                                                                OnCheckedChanged="chkDestExtendHistoricized_Click" />
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
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
            <asp:Button ID="btnObjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnObjectPostback_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
