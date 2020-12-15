<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstanceAccess.aspx.cs"
    Inherits="NttDataWA.Management.InstanceAccess" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tbl_rounded
        {
            width: 97%;
            border-collapse: collapse;
        }
        .tbl_rounded td
        {
            background: #fff;
            min-height: 1em;
            border: 1px solid #d4d4d4;
            border-top: 0;
            text-align: center;
        }
                .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
       .gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        .gridViewResult
        {
            min-width: 99%;
        }
    </style>
     <script type="text/javascript">
         function richiedentePopulated(sender, e) {
             var behavior = $find('AutoCompleteExIngressoRichiedente');
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

         function richiedenteSelected(sender, e) {
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

             var searchText = $get('<%=txtDescrizioneRichiedente.ClientID %>').value;
             searchText = searchText.replace('null', '');
             var testo = value;
             var indiceFineCodice = testo.lastIndexOf(')');
             document.getElementById("<%=this.txtDescrizioneRichiedente.ClientID%>").focus();
             document.getElementById("<%=this.txtDescrizioneRichiedente.ClientID%>").value = "";
             var indiceDescrizione = testo.lastIndexOf('(');
             var descrizione = testo.substr(0, indiceDescrizione - 1);
             var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
             document.getElementById("<%=this.txtCodiceRichiedente.ClientID%>").value = codice;
             document.getElementById("<%=txtDescrizioneRichiedente.ClientID%>").value = descrizione;

             __doPostBack('<%=this.txtCodiceRichiedente.ClientID%>', '');
         }
     </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup Id="OpenAddDocCustom" runat="server" Url="../popup/AddDocInProject.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddDocPostback').click();}" />
    <uc:ajaxpopup Id="ChooseCorrespondent" runat="server" Url="../popup/ChooseCorrespondent.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { __doPostBack('UpPnlButtons', '');}" />
    <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx">
                </div>
                <div id="containerStandardTopCx">
                    <p>
                        <asp:Label ID="LblInstanceAccessTitlePage" runat="server" />
                    </p>
                </div>
                <div id="containerStandardTopDx">
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
    </div>
    <div id="containerStandard">
        <div id="content">
            <div id="contentSxAccess">
                <div class="row">
                    <fieldset style="margin-top: 5px;">
                        <div class="row">
                            <asp:UpdatePanel runat="server" ID="UpPnlIdInstance" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <%-- ID ISTANZA--%>
                                    <div class="row">
                                        <div class="col">
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlIdDoc"></asp:Literal>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col2">
                                            <asp:DropDownList ID="ddl_idInstance" runat="server" Width="140px" AutoPostBack="true"
                                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_idInstance_SelectedIndexChanged">
                                                <asp:ListItem Value="0"></asp:ListItem>
                                                <asp:ListItem Value="1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col2">
                                            <asp:Literal runat="server" ID="LtlDaIdDoc"></asp:Literal>
                                        </div>
                                        <div class="col4">
                                            <cc1:CustomTextArea ID="txt_initIdDoc_C" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                        </div>
                                        <div class="col2">
                                            <asp:Literal runat="server" ID="LtlAIdDoc"></asp:Literal>
                                        </div>
                                        <div class="col4">
                                            <cc1:CustomTextArea ID="txt_fineIdDoc_C" runat="server" Width="80px" Visible="true"
                                                CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="row">
                            <asp:UpdatePanel ID="UpPnlDescription" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <%-- ************** OGGETTO ******************** --%>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal ID="DescriptionInstance" runat="server"></asp:Literal></span></p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-marginSx-full">
                                            <div class="full_width">
                                                <asp:UpdatePanel ID="UpdPnlDescription" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                                    <ContentTemplate>
                                                        <cc1:CustomTextArea ID="TxtObject" Width="99%" runat="server" class="txt_input_full"
                                                            CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="row">
                            <%-- DATA CREAZIONE --%>
                            <asp:UpdatePanel runat="server" ID="UpdateCreationDate" UpdateMode="Conditional">
                                <ContentTemplate>
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
                                            <asp:DropDownList ID="ddl_dataCreazione_E" runat="server" AutoPostBack="true" Width="130px"
                                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
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
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </fieldset>
                <%--        <fieldset style="margin-top:5px;">
                        
                        <!-- Close -->
                    <asp:UpdatePanel runat="server" ID="UpdateCloseDatePanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal ID="lit_dtaClose" runat="server" /></span></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_dtaClose" runat="server" Width="130px" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddl_dtaClose_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                            <asp:ListItem id="dtaClose_opt0" Selected="True" Value="0" />
                                            <asp:ListItem id="dtaClose_opt1" Value="1" />
                                            <asp:ListItem id="dtaClose_opt2" Value="2" />
                                            <asp:ListItem id="dtaClose_opt3" Value="3" />
                                            <asp:ListItem id="dtaClose_opt4" Value="4" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Label ID="lbl_dtaCloseFrom" runat="server"></asp:Label>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="dtaClose_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Label ID="lbl_dtaCloseTo" runat="server" Visible="False"></asp:Label>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="dtaClose_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>--%>
                    <div id="titleEstremiProcedimento" style=" padding-top:10px; padding-left:5px">
                        <span class="weight">
                            <asp:Literal ID="litEstremiProcedimento" runat="server" />
                        </span>
                    </div>
                    <fieldset style="margin-top:5px;">
                        <%-- RICHIEDENTE--%>
                        <asp:UpdatePanel ID="upPnlRichiedente" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal ID="litRichiedente" runat="server" />
                                            </span>
                                        </p>
                                        </div>
                                    <div class="col-right-no-margin">
                                        <cc1:CustomImageButton runat="server" ID="ImgRichiedenteAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                            OnClick="ImgRichiedenteAddressBook_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:HiddenField ID="idRichiedente" runat="server" />
                                    <div class="colHalf">
                                        <cc1:CustomTextArea ID="txtCodiceRichiedente" runat="server" CssClass="txt_addressBookLeft"
                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                            AutoCompleteType="Disabled">
                                        </cc1:CustomTextArea>
                                    </div>
                                    <div class="colHalf2">
                                        <div class="colHalf3">
                                            <cc1:CustomTextArea ID="txtDescrizioneRichiedente" runat="server" CssClass="txt_projectRight"
                                                CssClassReadOnly="txt_ProjectRight_disabled">
                                            </cc1:CustomTextArea>
                                        </div>
                                    </div>
                                    <uc1:AutoCompleteExtender runat="server" ID="RapidRichiedente" TargetControlID="txtDescrizioneRichiedente"
                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                        MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                        UseContextKey="true" OnClientItemSelected="richiedenteSelected" BehaviorID="AutoCompleteExIngressoRichiedente"
                                        OnClientPopulated="richiedentePopulated ">
                                    </uc1:AutoCompleteExtender>
                                </div>
                                <div class="row">
                                    <div class="col-right">
                                        <asp:CheckBox ID="chkRichiedenteExtendHistoricized" runat="server" Checked="true" OnCheckedChanged="chkRichiedenteExtendHistoricized_Click"/>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <!-- DATA RICHIESTA -->
                        <asp:UpdatePanel runat="server" ID="UpdateRequestDatePanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal ID="lit_dtaRequest" runat="server" /></span></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_dtaRequest" runat="server" Width="130px" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddl_dtaRequest_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                            <asp:ListItem id="dtaRequest_opt0" Selected="True" Value="0" />
                                            <asp:ListItem id="dtaRequest_opt1" Value="1" />
                                            <asp:ListItem id="dtaRequest_opt2" Value="2" />
                                            <asp:ListItem id="dtaRequest_opt3" Value="3" />
                                            <asp:ListItem id="dtaRequest_opt4" Value="4" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Label ID="lbl_dtaRequestFrom" runat="server"></asp:Label>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="dtaRequest_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Label ID="lbl_dtaRequestTo" runat="server" Visible="False"></asp:Label>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="dtaRequest_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="row">
                        <%-- ************** PROTOCOLLO RICHIESTA ******************** --%>
                            <asp:UpdatePanel ID="UpdPnlProtoRequest" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal ID="litProtoRequest" runat="server"></asp:Literal></span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                             <cc1:CustomImageButton runat="server" ID="InstanceAccessCercaProto" ImageUrl="../Images/Icons/search_response_documents.png"
                                                OnMouseOutImage="../Images/Icons/search_response_documents.png" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                 OnClick="InstanceAccessCercaProto_Click" />
                                            <cc1:CustomImageButton runat="server" ID="InstanceAccessResetProtoRequest" ImageUrl="../Images/Icons/delete.png"
                                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                                EnableViewState="true" OnClick="InstanceAccessResetProtoRequest_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-marginSx-full">
                                            <div class="full_width">
                                        
                                                        <cc1:CustomTextArea ID="TxtProtoRequest" Width="99%" runat="server" class="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                            
                                            </div>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="idProtoRequest" runat="server" />
                                    </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="row">
                            <%-- ************** NOTE ******************** --%>
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal ID="NoteInstance" runat="server"></asp:Literal></span></p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-marginSx-full">
                                    <div class="full_width">
                                        <asp:UpdatePanel ID="UpdPnlNote" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                            <ContentTemplate>
                                                <cc1:CustomTextArea ID="TxtNote" Width="99%" runat="server" class="txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
            <div id="contentDxAccess">
                <div id="contentDxTopProject">
                            <%--Label numero di elementi --%>
                            <asp:UpdatePanel runat="server" ID="UpnlNumeroIstanze" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col">
                                            <div class="p-padding-left">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label runat="server" ID="lblElencoIstanze"></asp:Label></span>
                                                    <asp:Label runat="server" ID="lblNumeroIstanze"></asp:Label></p>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel runat="server" ID="UpnlGridInstanceAccess" UpdateMode="Conditional" class="UpnlGrid">
                                        <ContentTemplate>
                                            <asp:GridView ID="gridInstanceAccess" runat="server"
                                                AllowPaging="true" PageSize="20" AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                                HorizontalAlign="Center" ShowHeader="true" ShowHeaderWhenEmpty="true" Style="cursor: pointer;" 
                                                OnPageIndexChanging="GridInstanceAccess_PageIndexChanging" OnRowDataBound="GridInstanceAccess_RowDataBound">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <PagerStyle CssClass="recordNavigator2" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceAccessId%>' >
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblIdInstanceAccess" Text='<%# Bind("ID_INSTANCE_ACCESS") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="DESCRIPTION" HeaderText='<%$ localizeByText:InstanceAccessDescription%>' />
                                                    <asp:TemplateField HeaderText='<%$ localizeByText:InstanceAccessPeopleRichiedente%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblRichiedenteInstanceAccess" Text='<%# Bind("RICHIEDENTE.descrizione") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText ='<%$ localizeByText:InstanceAccessCreationDate%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblCreationDate" Text='<%#this.GetLabelCreationDate((NttDataWA.DocsPaWR.InstanceAccess) Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:TemplateField HeaderText ='<%$ localizeByText:InstanceAccessCloseDate%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblCloseDate" Text='<%#this.GetLabelCloseDate((NttDataWA.DocsPaWR.InstanceAccess) Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                   
                                                    <%--<asp:TemplateField HeaderText ='<%$ localizeByText:InstanceAccessRequestDate%>'>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblRequestDate" Text='<%#this.GetLabelRequestDate((NttDataWA.DocsPaWR.InstanceAccess) Container.DataItem) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="ID_DOCUMENT_REQUEST" HeaderText='<%$ localizeByText:InstanceAccessDocumentRequest%>' />
                                                    <asp:BoundField DataField="NOTE" HeaderText='<%$ localizeByText:InstanceAccessNote%>' />--%>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="InstanceAccessSearch" runat="server" CssClass="btnEnable defaultAction"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceAccessSearch_Click" />
            <cc1:CustomButton ID="InstanceAccessNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceAccessNew_Click" />
            <cc1:CustomButton ID="InstanceRemoveFilter" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InstanceRemoveFilter_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnAddDocPostback" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="btnAddDocPostback_Click" />
            <asp:Button ID="btnGridInstanceAccessRow" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="btnGridInstanceAccessRow_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
