<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="Events.aspx.cs" Inherits="NttDataWA.Document.Events" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="DocumentButtons" %>
<%@ Register Src="~/UserControls/DocumentTabs.ascx" TagPrefix="uc4" TagName="DocumentTabs" %>
<%@ Register Src="~/UserControls/HeaderDocument.ascx" TagPrefix="uc2" TagName="HeaderDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 td
        {
            background: #eee;
            border: 0;
        }
        .recordNavigator2, .recordNavigator2 td
        {
            border: 0;
        }
    </style>
    <script type="text/javascript">
        function recipientPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoRecipient');
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

        function recipientSelected(sender, e) {
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

            var searchText = $get('<%=txt_descrAuthor_E.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_descrAuthor_E.ClientID%>").focus();
            document.getElementById("<%=this.txt_descrAuthor_E.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_codAuthor_E.ClientID%>").value = codice;
            document.getElementById("<%=txt_descrAuthor_E.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txt_codAuthor_E.ClientID%>', '');
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="VisibilityHistory" runat="server" Url="../popup/VisibilityHistory.aspx"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup Id="VisibilityRemove" runat="server" Url="../popup/VisibilityRemove.aspx"
        Width="600" Height="350" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('container', ''); }" />
    <uc:ajaxpopup Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
     <uc:ajaxpopup Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx"
        PermitClose="false" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', '');}" />
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
                                    <uc4:DocumentTabs runat="server" PageCaller="EVENTS" ID="DocumentTabs"></uc4:DocumentTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <uc3:DocumentButtons runat="server" ID="DocumentButtons" PageCaller="EVENTS" Visible="false" />
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
                        <div class="box_inside">
                            <fieldset>
                                <div class="row">
                                    <div class="col">
                                        <span class="weight">
                                            <asp:Literal ID="litSubject" runat="server" /></span>
                                    </div>
                                </div>
                                <div id="row_object" class="row">
                                    <asp:Literal ID="litObject" runat="server" /></div>
                            </fieldset>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal ID="EventsLblData" runat="server" /></span>
                                            </p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col3">
                                            <asp:DropDownList ID="DdlDate" runat="server" Width="150" AutoPostBack="True" OnSelectedIndexChanged="DdlDate_SelectedIndexChanged"
                                                CssClass="chzn-select-deselect">
                                                <asp:ListItem id="opt0" Selected="True" Value="0" />
                                                <asp:ListItem id="opt1" Value="1" />
                                                <asp:ListItem id="opt2" Value="2" />
                                                <asp:ListItem id="opt3" Value="3" />
                                                <asp:ListItem id="opt4" Value="4" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col2">
                                            <asp:Label ID="VisibilityFrom" runat="server"></asp:Label>
                                        </div>
                                        <div class="col4">
                                            <cc1:CustomTextArea ID="TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                        </div>
                                        <div class="col2">
                                            <asp:Label ID="VisibilityTo" runat="server" Visible="False"></asp:Label>
                                        </div>
                                        <div class="col4">
                                            <cc1:CustomTextArea ID="TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="PnlFilterAuthorAction" runat="server" Visible="false">
                                        <%-- AUTHOR --%>
                                        <asp:UpdatePanel ID="UpPnlAuthor" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlAuthor"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                    <div class="col">
                                                        <p>
                                                            <asp:RadioButtonList ID="rblOwnerType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                                                                <asp:ListItem id="optRole" runat="server" Value="R" Selected="True" />
                                                                <asp:ListItem id="optUser" runat="server" Value="P" />
                                                            </asp:RadioButtonList>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin1">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientAddressBookAuthor"
                                                            ImageUrl="../Images/Icons/address_book.png" OnMouseOutImage="../Images/Icons/address_book.png"
                                                            OnMouseOverImage="../Images/Icons/address_book_hover.png" CssClass="clickable"
                                                            ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="DocumentImgRecipientAddressBookAuthor_Click" />
                                                    </div>
                                                </div>
                                                <asp:HiddenField runat="server" ID="IdRecipient" />
                                                <asp:HiddenField runat="server" ID="RecipientTypeOfCorrespondent" />
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="txt_codAuthor_E" runat="server" CssClass="txt_addressBookLeft"
                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                            OnTextChanged="txt_author_E_TextChanged">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf2">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="txt_descrAuthor_E" runat="server" CssClass="txt_addressBookRight"
                                                                CssClassReadOnly="txt_addressBookRight" AutoCompleteType="Disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="txt_descrAuthor_E"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="recipientSelected" BehaviorID="AutoCompleteExIngressoRecipient"
                                                        OnClientPopulated="recipientPopulated" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <%-- EVENT --%>
                                        <asp:UpdatePanel ID="UpPnlEvent" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlEvent"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <asp:DropDownList ID="ddlEvent" Width="98%" runat="server" CssClass="chzn-select-deselect">
                                                    </asp:DropDownList>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpdPanelVisibility" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="GridEvents" runat="server" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                    Width="99%" AutoGenerateColumns="False" AllowPaging="True" ShowHeaderWhenEmpty="true"
                                    OnPageIndexChanging="GridEvents_PageIndexChanging" BorderWidth="0" PageSize="10">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="LblData" runat="server" Text='<%# Bind("dataAzione") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="LblOperatore" runat="server" Text='<%# Bind("userIdOperatore") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="LblAzione" runat="server" Text='<%# Bind("descrOggetto") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- end of container -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="panelButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ID="BtnFilter" OnClick="BtnFilter_Click" />
            <cc1:CustomButton runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ID="BtnRemoveFilter" OnClick="BtnRemoveFilter_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
