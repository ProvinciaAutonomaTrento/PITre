<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="SearchAutorizzazioniVersamento.aspx.cs" Inherits="NttDataWA.Search.SearchAutorizzazioniVersamento" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/AutorizzazioniTab.ascx" TagPrefix="uc2" TagName="HeaderAutorizzazioni" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        function UserInAuthPopulated(sender, e) {
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

        function UserInAuthSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneUserInAuth.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneUserInAuth.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneUserInAuth.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceUserInAuth.ClientID%>").value = codice;
            document.getElementById("<%=txtCodiceUserInAuth.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceUserInAuth.ClientID%>', '');
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderAutorizzazioni" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <contenttemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitAutorizzazioniVersamento" runat="server"></asp:Literal></p>
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </contenttemplate>
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
                                <%--   <uc2:SearchDocumentTabs ID="SearchDocumentTabs" runat="server" PageCaller="SIMPLE" />--%>
                                <uc2:HeaderAutorizzazioni ID="AutorizzazioniVersamentoTabs" runat="server" PageCaller="SEARCH" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="containerStandard2">
        <div id="contentSx">
            <asp:UpdatePanel runat="server" ID="upUtenteDetails" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitFiltriAuth" runat="server" />
                                </strong>
                            </div>
                        </div>
                        <div class="row">
                        </div>
                        <div class="row">
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <asp:Literal ID="LitAutorizzazioneId" runat="server" />
                                <br />
                            </div>
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="TxtIdAutorizzazione" runat="server" CssClass="txt_addressBookLeft"
                                    CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                            </div>
                            <div class="col-right-no-margin">
                                <cc1:CustomImageButton runat="server" ID="ImgUserInAuthAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                    OnClick="ImgUserInAuthAddressBook_Click" />
                            </div>
                        </div>
                        <div class="row">
                            <asp:HiddenField ID="idUserInAuth" runat="server" />
                            <div class="colHalf">
                                <asp:Literal ID="litUserInAuth" runat="server" /></div>
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="txtCodiceUserInAuth" runat="server" CssClass="txt_addressBookLeft"
                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                    AutoCompleteType="Disabled">
                                </cc1:CustomTextArea>
                            </div>
                            <div class="colHalf7">
                                <div class="colHalf3">
                                    <cc1:CustomTextArea ID="txtDescrizioneUserInAuth" runat="server" CssClass="txt_projectRight"
                                        CssClassReadOnly="txt_ProjectRight_disabled">
                                    </cc1:CustomTextArea>
                                </div>
                            </div>
                            <uc1:AutoCompleteExtender runat="server" ID="RapidUserInAuth" TargetControlID="txtDescrizioneUserInAuth"
                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                UseContextKey="true" OnClientItemSelected="UserInAuthSelected" BehaviorID="AutoCompleteUserInAuth"
                                OnClientPopulated="UserInAuthPopulated ">
                            </uc1:AutoCompleteExtender>
                        </div>
                        <div class="row">
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="upValidityTime" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <!--  -->
                            <strong>
                                <asp:Literal ID="litExpandUserInAuth" runat="server" /></strong>
                            <br />
                            <!-- Data Decorrenza -->
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <asp:Literal ID="lit_dtaDecorrenza" runat="server" /></p>
                                </div>
                            </div>
                            <div class="row">
                            </div>
                            <div class="row">
                            </div>
                            <div class="row">
                                <div class="col">
                                    <asp:DropDownList ID="ddl_dtaDecorrenza" runat="server" Width="150" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddl_dtaDecorrenza_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                        <asp:ListItem id="dtaDecorrenza_opt0" Selected="True" Value="0" />
                                        <asp:ListItem id="dtaDecorrenza_opt2" Value="2" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col2">
                                    <asp:Label ID="lbl_dtaDecorrenzaFrom" runat="server"></asp:Label>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="dtaDecorrenza_TxtFrom" runat="server" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                </div>
                                <div class="col2">
                                    <asp:Label ID="lbl_dtaDecorrenzaTo" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="dtaDecorrenza_TxtTo" runat="server" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                </div>
                            </div>
                            <!-- Data Scadenza -->
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <asp:Literal ID="lit_dtaScadenza" runat="server" /></p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <asp:DropDownList ID="ddl_dtaScadenza" runat="server" Width="150" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddl_dtaScadenza_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                        <asp:ListItem id="dtaScadenza_opt0" Selected="True" Value="0" />
                                        <asp:ListItem id="dtaScadenza_opt2" Value="2" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col2">
                                    <asp:Label ID="lbl_dtaScadenzaFrom" runat="server"></asp:Label>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="dtaScadenza_TxtFrom" runat="server" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                                </div>
                                <div class="col2">
                                    <asp:Label ID="lbl_dtaScadenzaTo" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="dtaScadenza_TxtTo" runat="server" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="upStateAuth" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <div class="row">
                                <strong>
                                    <asp:Literal ID="LitStateTypeAuth" runat="server" /></strong>
                            </div>
                            <div class="row">
                            </div>
                            <div class="row">
                            </div>
                            <div class="row">
                                <div class="col">
                                    <asp:RadioButtonList ID="rblStateType" runat="server" AutoPostBack="True" CssClass="rblHorizontal"
                                        RepeatLayout="UnorderedList">
                                        <asp:ListItem id="optAttiva" runat="server" Value="A" Selected="True" />
                                        <asp:ListItem id="optScaduta" runat="server" Value="S" />
                                        <asp:ListItem id="optTutti" runat="server" Value="T" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="ContentCustomSx" class="ContentCustomSx">
            </div>
        </div>
        <div id="contentDx">
            <div id="contentListDocument" class="contentDxDoc">
                <div class="row">
                    <strong>
                        <asp:Literal ID="LitAuthResult" runat="server" /></strong>
                </div>
                <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                    <ContentTemplate>
                        <div class="contentListResult">
                            <fieldset>
                                <asp:GridView ID="gvResultAUTH" runat="server" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                    Width="99%" AutoGenerateColumns="False" AllowPaging="false" DataKeyNames="System_ID"
                                    BorderWidth="0">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ckbSelected" runat="server" AutoPostBack="true" />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblSystem_id_PolicylabelHeader" Text='ID' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblSystem_id_Versamento" runat="server" Text='<%# Bind("System_id") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="LblDescription_PolicylabelHeader" Text='Utente' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblDescrizione" runat="server" Text='<%#  Bind("DescrizioneRubrica")  %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="LblNumeroDoc_PolicylabelHeader" Text='Decorrenza' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblStato" runat="server" Text='<%# Bind("dtaDecorrenza", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="LblNumeroFasc_PolicylabelHeader" Text='Scadenza' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblDataEsecuzione" runat="server" Text='<%# Bind("dtaScadenza", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblckb" runat="server" Text='Dett.'></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="btnAuthorizationDetails" ImageUrl="../Images/Icons/search_response_documents.png"
                                                    runat="server" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                    OnMouseOutImage="../Images/Icons/search_response_documents.png" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                    CssClass="clickableLeft" OnClick="btnAuthorizationDetails_Click" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:HiddenField ID="rowIndex" runat="server" ClientIDMode="Static" />
                            </fieldset>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpTranfetButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="SearchAuthorization" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchAuthorization_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <cc1:CustomButton ID="btnClearFilterUserInAuth" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnClearFilterUserInAuth_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
