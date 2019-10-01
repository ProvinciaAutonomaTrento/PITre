<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFilterDocInstanceAccess.aspx.cs" Inherits="NttDataWA.Popup.AddFilterDocInstanceAccess" MasterPageFile="~/MasterPages/Popup.Master"%>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript">

            function mittDestPopulated(sender, e) {
                var behavior = $find('AutoCompleteExIngressoMittDest');
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

            function mittDestSelected(sender, e) {
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

                var searchText = $get('<%=txtDescrizioneMittDest.ClientID %>').value;
                searchText = searchText.replace('null', '');
                var testo = value;
                var indiceFineCodice = testo.lastIndexOf(')');
                document.getElementById("<%=this.txtDescrizioneMittDest.ClientID%>").focus();
                document.getElementById("<%=this.txtDescrizioneMittDest.ClientID%>").value = "";
                var indiceDescrizione = testo.lastIndexOf('(');
                var descrizione = testo.substr(0, indiceDescrizione - 1);
                var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                document.getElementById("<%=this.txtCodiceMittDest.ClientID%>").value = codice;
                document.getElementById("<%=txtDescrizioneMittDest.ClientID%>").value = descrizione;

                __doPostBack('<%=this.txtCodiceMittDest.ClientID%>', '');
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

            function closeObjectPopup() {
                $('#btnObjectPostback').click();
            }

            function closeAddressBookPopup() {
                $('#btnAddressBookPostback').click();
            }

            function closeTitolarioPopup() {
                $('#btnTitolarioPostback').click();
            }

            function closeSearchProjectPopup() {
                $('#btnSearchProjectPostback').click();
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
    </script>
    <style type="text/css">
        
        #containerFilter fieldset
        {
            margin: 5px 0 5px 0;
            background-color: #e4f1f6;
            border: 0px;
            padding-left: 5px;
            padding-right: 5px;
            padding-bottom: 5px;
            width: 97%;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
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
            margin-top:4px;
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
            margin-top:8px;
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
        
        .col
        {
            float: left;
            margin: 0px 30px 0px 0px;
            text-align: left;
        }
        
        .row
        {
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
        
        .col-right
        {
            float: right;
            margin: 0 10px 0 30px;
            text-align: right;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">
    <div id="containerFilter" style=" padding-left:5px; width:99%">
        <%-- ************** TIPO ******************** --%>
        <div class="row">
            <fieldset>
                <asp:UpdatePanel runat="server" ID="UpPnlType" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-marginSx2">
                                <p>
                                    <span class="weight">
                                        <asp:Literal runat="server" ID="InstanceAccessTypeDocument"></asp:Literal></span></p>
                            </div>
                            <div class="col">
                                <asp:CheckBoxList ID="cbl_archDoc_E" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="A" Selected="True" runat="server" id="opArr"></asp:ListItem>
                                    <asp:ListItem Value="P" Selected="True" runat="server" id="opPart"></asp:ListItem>
                                    <asp:ListItem Value="I" Selected="True" runat="server" id="opInt"></asp:ListItem>
                                    <asp:ListItem Value="G" Selected="True" runat="server" id="opGrigio"></asp:ListItem>
                                    <asp:ListItem Value="ALL" Selected="False" runat="server" id="opAll"></asp:ListItem>
                                    <asp:ListItem Value="Pr" Selected="False" id="opPredisposed" runat="server"></asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                        <asp:Panel runat="server" ID="PnlAtt" ClientIDMode="Static" CssClass="hidden">
                            <div class="row">
                                <div class="col">
                                    <asp:RadioButtonList ID="rblFiltriAllegati" runat="server" CssClass="testo_grigio"
                                        RepeatDirection="Horizontal">
                                        <asp:ListItem Value="0" Text="Tutti" />
                                        <asp:ListItem Value="2" Text="PEC" />
                                        <asp:ListItem Value="1" Text="Utente" Selected="True" />
                                        <asp:ListItem Value="4" Text="Sist. esterni" />
                                        <asp:ListItem Value="3" runat="server" id="rbOpIS" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </asp:Panel>
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
            <fieldset>
                <div class="row">
                    <%-- ************** NUMERO PROTOCOLLO ******************** --%>
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
                <div class="row">
                     <%-- ************** MITTENTE/DESTINATARIO ******************** --%>
                    <asp:UpdatePanel ID="upPnlMittDest" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal ID="litMittDest" runat="server" /></span>
                                    </p>
                                </div>
                                <div class="col-right-no-margin">
                                    <cc1:CustomImageButton runat="server" ID="ImgMittDestAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                        OnClick="ImgMittDestAddressBook_Click" />
                                </div>
                            </div>
                            <div class="row">
                                <asp:HiddenField ID="idMittDest" runat="server" />
                                <div class="colHalf">
                                    <cc1:CustomTextArea ID="txtCodiceMittDest" runat="server" CssClass="txt_addressBookLeft"
                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                        AutoCompleteType="Disabled" onchange="disallowOp('ContentPlaceHolderContent');">
                                    </cc1:CustomTextArea>
                                </div>
                                <div class="colHalf2">
                                    <div class="colHalf3">
                                        <cc1:CustomTextArea ID="txtDescrizioneMittDest" runat="server" CssClass="txt_projectRight"
                                            CssClassReadOnly="txt_ProjectRight_disabled">
                                        </cc1:CustomTextArea>
                                    </div>
                                </div>
                                <uc1:AutoCompleteExtender runat="server" ID="RapidMittDest" TargetControlID="txtDescrizioneMittDest"
                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                    MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="mittDestSelected" BehaviorID="AutoCompleteExIngressoMittDest"
                                    OnClientPopulated="mittDestPopulated ">
                                </uc1:AutoCompleteExtender>
                            </div>
                            <div class="row">
                                <div class="col-right">
                                    <asp:CheckBox ID="chkMittDestExtendHistoricized" runat="server" Checked="true"  AutoPostBack="true" OnCheckedChanged="chkMittDestExtendHistoricized_Click"  />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </fieldset>
            <fieldset>
                <div class="row">
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
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </fieldset>
            <%--******************* REGISTRO ***************--%>
            <asp:UpdatePanel runat="server" ID="UpPnlRegistro" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:PlaceHolder ID="plcRegistro" runat="server">
                        <div class="row">
                            <fieldset>
                                <div class="col">
                                    <strong>
                                        <asp:Literal runat="server" ID="LtlIdReg"></asp:Literal></strong>
                                </div>
                                <div class="row">
                                    <div class="col">
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
            <fieldset>
                <div class="row">
                    <%-- ************** REPERTORIO ******************** --%>
                    <asp:UpdatePanel runat="server" ID="UpPnRepertorio" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col">
                                    <span class="weight">
                                        <asp:Literal runat="server" ID="LtlRepertorio"></asp:Literal>
                                    </span>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col2">
                                    <asp:DropDownList ID="ddl_RegRep" runat="server" Width="200px" CssClass="chzn-select-deselect">
                                    </asp:DropDownList>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="txtRepertorio" runat="server" CssClass="txt_addressBookLeft"
                                        CssClassReadOnly="txt_addressBookLeft_disabled" >
                                    </cc1:CustomTextArea>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </fieldset>
            <%-- ************** NUMERO ALLEGATI ******************** --%>
            <fieldset>
                <asp:UpdatePanel runat="server" ID="UpPnlNumberAttach" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row">
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
                                    <asp:DropDownList ID="ddl_op_allegati"  runat="server"
                                        Width="50px" CssClass="chzn-select-deselect">
                                        <asp:ListItem Text="<" Value="<" />
                                        <asp:ListItem Text="=" Value="=" />
                                        <asp:ListItem Text=">" Value=">" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col">
                                    <cc1:CustomTextArea ID="txt_allegati" runat="server" Columns="3"  CssClass="txt_input_full onlynumbers"
                                        CssClassReadOnly="txt_input_full_disabled" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </fieldset>
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
                                     CssClass="clickable" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"
                                     OnClientClick="return parent.ajaxModalPopupOpenTitolario();" />
                                <cc1:CustomImageButton runat="server" ID="SearchProjectImg" ImageUrl="../Images/Icons/search_projects.png"
                                    OnMouseOutImage="../Images/Icons/search_projects.png" OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/search_projects_disabled.png"
                                    OnClientClick="return parent.ajaxModalPopupSearchProject();" />
                            </div>
                            <div class="row">
                                <asp:HiddenField ID="IdProject" runat="server" />
                                <div class="colHalf">
                                    <cc1:CustomTextArea ID="txt_CodFascicolo" runat="server" CssClass="txt_addressBookLeft" onchange="disallowOp('ContentPlaceHolderContent');"
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
                                    MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                    OnClientPopulated="acePopulatedCod">
                                </uc1:AutoCompleteExtender>
                            </div>
                        </fieldset>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <%-- ************** RICHIESTA ******************** --%>
            <fieldset>
                <asp:UpdatePanel runat="server" ID="UpPnlRequest" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-marginSx2">
                                <p>
                                    <span class="weight">
                                        <asp:Literal runat="server" ID="InstanceAccessRequest"></asp:Literal></span></p>
                            </div>
                            <div class="col">
                                <asp:CheckBoxList ID="cbl_request" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="C" runat="server" id="opConf"></asp:ListItem>
                                    <asp:ListItem Value="A" runat="server" id="opAut"></asp:ListItem>
                                    <asp:ListItem Value="E" runat="server" id="opEstr"></asp:ListItem>
                                    <asp:ListItem Value="D" runat="server" id="opDup"></asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </fieldset>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="AddFilterBtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterBtnConfirm_Click" />
            <cc1:CustomButton ID="AddFilterBtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterBtnCancel_Click" />
            <asp:Button ID="btnObjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnObjectPostback_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <asp:Button ID="btnTitolarioPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnTitolarioPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <asp:Button ID="btnSearchProjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnSearchProjectPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />     
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>