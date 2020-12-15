<%@ Page Title="Gestione Autorizzazioni" Language="C#" MasterPageFile="~/MasterPages/Base.Master"
    AutoEventWireup="true" CodeBehind="AutorizzazioniVersamento.aspx.cs" Inherits="NttDataWA.Deposito.AutorizzazioniVersamento" %>

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
    <uc:ajaxpopup2 Id="OpenAddDoc" runat="server" Url="../popup/AddDocInTransfer.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui)  {$('#btnDocumentPostback').click();}" />
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../popup/SearchProjectTransfer.aspx?caller=classifica"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui)  {$('#btnProjectPostback').click();}" />
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
        <%--   <asp:UpdatePanel runat="server" ID="UpContainerAutorizzazioniTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:HeaderAutorizzazioni ID="AutorizzazioniVersamentoTabs" runat="server" PageCaller="AUTORIZZAZIONE" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <div class="row">
                                <div class="colMassiveOperationSx">
                                </div>
                                <div class="colMassiveOperationDx">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>--%>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <%--   <uc2:SearchDocumentTabs ID="SearchDocumentTabs" runat="server" PageCaller="SIMPLE" />--%>
                                <uc2:HeaderAutorizzazioni ID="AutorizzazioniVersamentoTabs" runat="server" PageCaller="NEW" />
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
            <!-- Area Utente-Note -->
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
                            </div>
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="TxtIdAutorizzazione" runat="server" CssClass="txt_addressBookLeft"
                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False"></cc1:CustomTextArea>
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
                        <div class="row">
                            <div class="colHalf">
                                <asp:Literal ID="LitUserInAuthNote" runat="server" /></div>
                            <div class="colHalf2">
                                <cc1:CustomTextArea ID="TxtNoteUserInAuth" runat="server" CssClass="txt_textarea"
                                    CssClassReadOnly="txt_textarea_disabled">
                                </cc1:CustomTextArea>
                            </div>
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
                            <!-- Data Decorrenza -->
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <asp:Literal ID="lit_dtaDecorrenza" runat="server" /></p>
                                </div>
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
            <div id="ContentCustomSx" class="ContentCustomSx">
            </div>
        </div>
        <div id="contentDx">
            <div id="contentListDocument" class="contentDxDoc">
                <asp:UpdatePanel runat="server" ID="upDocumentInAUTH" UpdateMode="Conditional" ChildernAsTriggers="True">
                    <ContentTemplate>
                        <fieldset>
                            <div class="row">
                                <div class="col-full">
                                    <strong>
                                        <asp:Literal ID="LitDocumentInAuth" runat="server" /></strong>
                                    <asp:PlaceHolder ID="plcNavigatorDoc" runat="server" />
                                    <asp:GridView ID="gvDocumentInAUTH" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                        HorizontalAlign="Center" CssClass="tbl_rounded_custom round_onlyextreme" Width="100%"
                                        DataKeyNames="System_ID" PageSize="9" OnPageIndexChanging="gvDocumentInAUTHTransferPageIndexChanging_click">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblRegistrogvDocumentInAUTHHeader" Text='Registro' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="registro" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblTipogvDocumentInAUTHHeader" Text='Tipo' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="tipo" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='ID Prot.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="iD_Protocollo" runat="server" Text='<%# Bind("ID_Protocollo") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblTipogvDocumentInAUTHHeader" Text='Data' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="data" runat="server" Text='<%# Bind("Data") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Oggetto' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="oggetto" runat="server" Text='<%# Bind("Oggetto") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Mitt/Dest' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="idProfile" runat="server" Text='<%# Bind("Mittente_Destinatario") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblckb" runat="server" Text='Dett.'></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="btnDocDetails" ImageUrl="../Images/Icons/search_response_documents.png"
                                                        runat="server" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                        OnMouseOutImage="../Images/Icons/search_response_documents.png" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                        CssClass="clickableLeft" OnClick="btnDocDetails_Click" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblckb" runat="server" Text='Elimina'></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="btnDocRemove" ImageUrl="../Images/Icons/delete.png" runat="server"
                                                        OnMouseOverImage="../Images/Icons/delete_hover.png" OnMouseOutImage="../Images/Icons/delete.png"
                                                        ImageUrlDisabled="../Images/Icons/delete_disabled.png" CssClass="clickableLeft"
                                                        OnClick="btnDocRemove_Click" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:PlaceHolder ID="plcNavigator" runat="server" />
                                    <asp:HiddenField ID="grid_pageindexDoc" runat="server" ClientIDMode="Static" />
                                </div>
                            </div>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="contentListProject" class="contentDxFasc">
                <asp:UpdatePanel runat="server" ID="upProjectInAuth" UpdateMode="Conditional" ChildernAsTriggers="True">
                    <ContentTemplate>
                        <fieldset>
                            <div class="row">
                                <div class="col-full">
                                    <strong>
                                        <asp:Literal ID="LitProjectInAuth" runat="server" /></strong>
                                    <asp:PlaceHolder ID="plcNavigatorPrj" runat="server" />
                                    <asp:GridView ID="gvProjectInAUTH" runat="server" Width="100%" AutoGenerateColumns="False"
                                        AllowPaging="True" AllowCustomPaging="false" CssClass="tbl_rounded_custom round_onlyextreme"
                                        PageSize="9" BorderWidth="0" DataKeyNames="System_ID" OnPageIndexChanging="gvProjectInAUTHTransferPageIndexChanging_click">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblRegistro_gvProjectInAUTHHeader" Text='Registro' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Literal ID="litRegistro" runat="server" Text='<%# Bind("Registro") %>'></asp:Literal>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblDescrizione_gvProjectInAUTHHeader" Text='Descrizione' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <%# NttDataWA.Utils.utils.TruncateString(Eval("Descrizione"))%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblCodice_gvProjectInAUTHHeader" Text='Codice' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Literal ID="litFascCodice" runat="server" Text='<%# Bind("Codice") %>'></asp:Literal>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblDataApertura_gvProjectInAUTHHeader" Text='DataApertura' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Literal ID="litFascDataApertura" runat="server" Text='<%# Eval("DataApertura", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblDataChiusura_gvProjectInAUTHHeader" Text='DataChiusura' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="litFascDataChiusura" Text='<%# SetDataChiusura(Eval("DataChiusura"))  %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblckb" runat="server" Text='Dett.'></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="btnDocDetails" ImageUrl="../Images/Icons/search_response_documents.png"
                                                        runat="server" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                        OnMouseOutImage="../Images/Icons/search_response_documents.png" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                        CssClass="clickableLeft" OnClick="btnPrjDetails_Click" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblckb" runat="server" Text='Elimina'></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="btnPrjRemove" ImageUrl="../Images/Icons/delete.png" runat="server"
                                                        OnMouseOverImage="../Images/Icons/delete_hover.png" OnMouseOutImage="../Images/Icons/delete.png"
                                                        ImageUrlDisabled="../Images/Icons/delete_disabled.png" CssClass="clickableLeft"
                                                        OnClick="btnPrjRemove_Click" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="grid_pageindexPrj" runat="server" ClientIDMode="Static" />
                                </div>
                            </div>
                        </fieldset>
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
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnDocumentPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnDocumentPostback_Click" />
            <asp:Button ID="btnProjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnProjectPostback_Click" />
            <cc1:CustomButton ID="btnUserInAuthNuovo" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnUserInAuthNuovo_Click" />
            <cc1:CustomButton ID="btnSearchDoc" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnSearchDoc_Click" />
            <cc1:CustomButton ID="btnSearchPrj" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnSearchPrj_Click" />
            <cc1:CustomButton ID="btnEditUserInAuth" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnEditUserInAuth_Click" />
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
