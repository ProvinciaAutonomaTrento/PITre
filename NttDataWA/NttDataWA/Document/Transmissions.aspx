<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="Transmissions.aspx.cs" Inherits="NttDataWA.Document.Transmissions"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/HeaderDocument.ascx" TagPrefix="uc2" TagName="HeaderDocument" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="DocumentButtons" %>
<%@ Register Src="~/UserControls/DocumentTabs.ascx" TagPrefix="uc4" TagName="DocumentTabs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
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

            __doPostBack('<%=this.TxtCodeRecipientTransmission.ClientID%>', '');
        }

        function UpdateExpand() {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });

            $('#contentNoteTask input, #contentNoteTask textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentNoteTask select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        };
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

        function verifica() {
            var valReturn = false;
            if (document.getElementById("<%=this.TxtCodeProject.ClientID%>").value != '' && document.getElementById("<%=TxtDescriptionProject.ClientID%>").value != '') {
                valReturn = true;
            }
            else if (document.getElementById("<%=this.TxtCodeProject.ClientID%>").value == '') {
                valReturn = true;
            }

            return valReturn;
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

            var searchText = $get('<%=TxtDescriptionProject.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeProject.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionProject.ClientID%>").value = descrizione;
        }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .chzn-container-single
        {
            margin-top: 5px;
        }
        
        .tbl_rounded_custom th
        {
            white-space: nowrap;
        }
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 td
        {
            background: #eee;
            border: 0;
        }
        .recordNavigator2, .recordNavigator2 td
        {
            border: 0;
        }
        
        .tbl_rounded_custom td
        {
            cursor: pointer;
        }
        .expand img
        {
            float: right;
            border: 0px;
        }
        .expand
        {
            font-size:13px;
            font-style:italic;
            white-space:nowrap;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="TransmissionsPrint" runat="server" Title="Stampa trasmissioni"
        Url="../popup/transmissions_print_iframe.aspx" IsFullScreen="true" PermitClose="false"
        PermitScroll="true" />
    <uc:ajaxpopup Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx"
        PermitClose="false" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('panelButtons', ''); }" />
    <uc:ajaxpopup Id="SearchProject" runat="server" Url="../Popup/SearchProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="CompleteTask" runat="server" Url="../popup/CompleteTask.aspx"
        PermitClose="false" PermitScroll="false" Width="500" Height="350" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="ReopenTask" runat="server" Url="../popup/CompleteTask.aspx?from=ReopenTask"
        PermitClose="false" PermitScroll="false" Width="500" Height="400" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
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
                                    <uc4:DocumentTabs runat="server" PageCaller="TRANSMISSIONS" ID="DocumentTabs"></uc4:DocumentTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <uc3:DocumentButtons runat="server" ID="DocumentButtons" PageCaller="TRANSMISSIONS"
                                Visible="false" />
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
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <span class="weight">
                                                <asp:Literal ID="TransmissionLitSubject" runat="server" /></span>
                                        </div>
                                    </div>
                                    <div id="row_object" class="row">
                                        <asp:Literal ID="litObject" runat="server" /></div>
                                </fieldset>
                            </div>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <asp:CheckBox ID="chkReceived" runat="server" />
                                    </div>
                                    <div class="row">
                                        <asp:CheckBox ID="chkTransmittedFromRole" runat="server" />
                                        <asp:CheckBox ID="chkTransmittedFromRF" runat="server" />
                                        <asp:DropDownList ID="ddlRF" runat="server" CssClass="chzn-select-deselect" Width="110" />
                                    </div>
                                </fieldset>
                            </div>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <span class="weight">
                                                <asp:Literal ID="TransmissionLitSenderRecipient" runat="server" /></span>
                                        </div>
                                        <div class="col">
                                            <asp:RadioButtonList ID="rblRecipientType" runat="server" RepeatLayout="UnorderedList"
                                                ClientIDMode="Static" />
                                        </div>
                                        <div class="col-right-no-margin">
                                            <cc1:CustomImageButton runat="server" ID="TrasmissionsImgAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                OnClick="TrasmissionsImgAddressBook_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <asp:UpdatePanel runat="server" ID="UpPnlRecipient" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <asp:HiddenField ID="IdRecipient" runat="server" />
                                                <asp:HiddenField ID="RecipientTypeOfCorrespondent" runat="server" />
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
                                                <div class="row">
                                                    <div class="col-right">
                                                        <asp:CheckBox ID="chkCreatoreExtendHistoricized" runat="server" Checked="false" />
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                            </div>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div id="col-reason">
                                            <span class="weight">
                                                <asp:Literal ID="TransmissionLitReason" runat="server" /></span><br />
                                            <asp:DropDownList ID="ddlReason" runat="server" CssClass="chzn-select-deselect" data-placeholder="Scegli una ragione di trasmissione"
                                                Width="300" />
                                        </div>
                                        <div id="col-date">
                                            <span class="weight">
                                                <asp:Literal ID="TransmissionLitDate" runat="server" /></span><br />
                                            <cc1:CustomTextArea ID="txtDate" runat="server" Columns="10" MaxLength="10" ClientIDMode="Static"
                                                CssClass="txt_date datepicker" CssClassReadOnly="txt_date_disabled" />
                                        </div>
                                    </div>
                                    <div class="row" style="margin-top: 4em;">
                                        <div class="col">
                                            <asp:CheckBox ID="chkAccepted" runat="server" />
                                        </div>
                                        <div class="col">
                                            <asp:CheckBox ID="chkViewed" runat="server" />
                                        </div>
                                        <div class="col">
                                            <asp:CheckBox ID="chkPending" runat="server" />
                                        </div>
                                        <div class="col">
                                            <asp:CheckBox ID="chkRefused" runat="server" />
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                            <asp:UpdatePanel ID="upPnlGridList" runat="server" class="row" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:GridView ID="grdList" runat="server" Width="98%" AutoGenerateColumns="False"
                                        AllowPaging="True" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                        PageSize="5" OnRowDataBound="grdList_RowDataBound" OnSelectedIndexChanged="grdList_SelectedIndexChanged">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField ItemStyle-CssClass="grdList_date" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                HeaderStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container, "DataItem.DataInvio").ToString()%>
                                                    <asp:HiddenField ID="trasmId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.IdTrasmissione") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-CssClass="grdList_description" ItemStyle-HorizontalAlign="Left"
                                                HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <%# GetSenderName(DataBinder.Eval(Container, "DataItem.Utente").ToString(), DataBinder.Eval(Container, "DataItem.UtenteDelegato").ToString())%><br />
                                                    <em>
                                                        <%# DataBinder.Eval(Container, "DataItem.Ruolo")%></em>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:PlaceHolder ID="plcNavigator" runat="server" />
                                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="grdList" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <fieldset style="clear: both; float: left;">
                                <div class="row">
                                    <div class="col-full">
                                        <span class="weight">
                                            <asp:Literal ID="TransmissionLitRapidTransmission" runat="server" /></span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-full">
                                        <asp:UpdatePanel ID="upPnlTransmissionsModel" runat="server" UpdateMode="Conditional"
                                            ClientIDMode="Static">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="DdlTransmissionsModel" runat="server" CssClass="chzn-select-deselect"
                                                    Width="100%" OnSelectedIndexChanged="DdlTransmissionsModel_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Text=""></asp:ListItem>
                                                </asp:DropDownList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpPnlTransmission" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="plcTransmission" runat="server" Visible="false">
                                    <table class="tbl_rounded">
                                        <tr>
                                            <th class="first">
                                                <asp:Literal ID="TransmissionLitNote" runat="server" />
                                            </th>
                                        </tr>
                                        <tr>
                                            <td class="first">
                                                <asp:Literal ID="trasmNote" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                    <!-- template -->
                                    <asp:Table ID="tblDetails" runat="server" CssClass="tbl_rounded" Visible="false">
                                        <asp:TableRow>
                                            <asp:TableCell ColumnSpan="7" CssClass="th first">
                                                <asp:Literal ID="TransmissionLitDetailsRecipient" runat="server" />
                                            </asp:TableCell></asp:TableRow><asp:TableRow CssClass="header">
                                            <asp:TableCell CssClass="first" ColumnSpan="2">Destinatario</asp:TableCell><asp:TableCell
                                                CssClass="center trasmDetailReason">Ragione</asp:TableCell><asp:TableCell CssClass="center trasmDetailType"
                                                    ColumnSpan="2">Tipo</asp:TableCell><asp:TableCell CssClass="trasmDetailNote">Note</asp:TableCell><asp:TableCell
                                                        CssClass="center trasmDetailDate">Scade il</asp:TableCell></asp:TableRow><asp:TableRow>
                                            <asp:TableCell ID="trasmDetailsRecipient" runat="server" CssClass="first" ColumnSpan="2" />
                                            <asp:TableCell ID="trasmDetailsReason" runat="server" CssClass="center" />
                                            <asp:TableCell ID="trasmDetailsType" runat="server" CssClass="center" ColumnSpan="2" />
                                            <asp:TableCell ID="trasmDetailsNote" runat="server" />
                                            <asp:TableCell ID="trasmDetailsExpire" runat="server" CssClass="center" />
                                        </asp:TableRow>
                                        <asp:TableRow CssClass="header2">
                                            <asp:TableCell CssClass="first">Utente</asp:TableCell><asp:TableCell CssClass="center trasmDetailDate">Vista il</asp:TableCell><asp:TableCell
                                                CssClass="center trasmDetailDate">Acc. il</asp:TableCell><asp:TableCell CssClass="center trasmDetailDate">Rif. il</asp:TableCell><asp:TableCell
                                                    CssClass="center trasmDetailDate">Rimossa</asp:TableCell><asp:TableCell CssClass="center"
                                                        ColumnSpan="2">Info Acc./Info rif.</asp:TableCell></asp:TableRow><asp:TableRow ID="rowDetails" runat="server" CssClass="users">
                                            <asp:TableCell ID="trasmDetailsUser" runat="server" CssClass="first" />
                                            <asp:TableCell ID="trasmDetailsViewed" runat="server" CssClass="center" />
                                            <asp:TableCell ID="trasmDetailsAccepted" runat="server" CssClass="center" />
                                            <asp:TableCell ID="trasmDetailsRif" runat="server" CssClass="center" />
                                            <asp:TableCell ID="trasmDetailsRemoved" runat="server" CssClass="center" />
                                            <asp:TableCell ID="trasmDetailsInfo" runat="server" ColumnSpan="2" />
                                        </asp:TableRow>
                                    </asp:Table>
                                    <asp:HiddenField ID="idContributo" runat="server" ClientIDMode="Static"/>
                                    <asp:PlaceHolder ID="plcTransmissions" runat="server" />
                                    <asp:UpdatePanel ID="upPnlNoteAccRif" runat="server" UpdateMode="Conditional" Visible="false">
                                        <ContentTemplate>
                                            <fieldset id="fldset_noteAccRif" runat="server" clientidmode="Static">
                                                <div class="row">
                                                    <span class="weight">
                                                        <asp:Literal ID="TransmissionNoteAccRej" runat="server" /></span><br />
                                                    <cc1:CustomTextArea ID="txt_noteAccRif" runat="server" MaxLength="250"/>
                                                </div>
                                            </fieldset>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <!-- Fascicolazione -->
                                    <asp:UpdatePanel ID="upPnlFascRequired" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <asp:Panel ID="pnlFascRequired" runat="server" Width="98%" ClientIDMode="Static">
                                                <div class="row" style="padding-left: 15px">
                                                     <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="DocumentLitClassificationRapidTrasm"></asp:Literal></span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <asp:UpdatePanel ID="pPnlBUttonsProject" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                                    runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                                    OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                                    OnClick="DocumentImgOpenTitolario_Click" CssClass="clickableLeftN"
                                                                    ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"/>
                                                                <cc1:CustomImageButton ID="DocumentImgSearchProjects" ImageUrl="../Images/Icons/search_projects.png"
                                                                    runat="server" OnMouseOverImage="../Images/Icons/search_projects_hover.png" OnMouseOutImage="../Images/Icons/search_projects.png"
                                                                    ImageUrlDisabled="../Images/Icons/search_projects_disabled.png" alt="Titolario"
                                                                    CssClass="clickableLeftN" OnClick="DocumentImgSearchProjects_Click"  />
                                                                <cc1:CustomImageButton ID="ImgAddProjects" ImageUrl="../Images/Icons/add_sub_folder.png"
                                                                    runat="server" OnMouseOverImage="../Images/Icons/add_sub_folder_hover.png" OnMouseOutImage="../Images/Icons/add_sub_folder.png"
                                                                    ImageUrlDisabled="../Images/Icons/add_sub_folder_disabled.png" alt="Titolario"
                                                                    CssClass="clickableLeftN" OnClick="ImgAddProjects_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                                        <ContentTemplate>
                                                            <asp:PlaceHolder runat="server" ID="PnlProject">
                                                                <asp:HiddenField ID="IdProject" runat="server" />
                                                                <div class="colHalf">
                                                                    <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft" onchange="disallowOp('ContentPlaceHolderContent');"
                                                                        AutoComplete="off"  AutoPostBack="true" OnTextChanged="TxtCodeProject_OnTextChanged"
                                                                        CssClassReadOnly="txt_addressBookLeft_disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                                <div class="colHalf2">
                                                                    <div class="colHalf3">
                                                                        <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_addressBookRight"
                                                                            autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled">
                                                                        </cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                                 <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProject" TargetControlID="TxtDescriptionProject"
                                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                                                                    MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                    UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                                                    OnClientPopulated="acePopulatedCod">
                                                                </uc1:AutoCompleteExtender>
                                                            </asp:PlaceHolder>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                            </ContentTemplate>
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
            <asp:HiddenField ID="proceed_personal" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="proceed_private" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="proceed_ownership" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="final_state" runat="server" ClientIDMode="Static" />
            <cc1:CustomButton ID="TransmissionsBtnSearch" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnSearch_Click" />
            <cc1:CustomButton ID="TransmissionsBtnClear" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnClear_Click" />
            <cc1:CustomButton ID="TransmissionsBtnAdd" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Enabled="false" OnClick="TransmissionsBtnAdd_Click" />
            <cc1:CustomButton ID="TransmissionsBtnModify" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" Enabled="false"
                OnClick="TransmissionsBtnModify_Click" />
            <cc1:CustomButton ID="TransmissionsBtnTransmit" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnTransmit_Click"
                Enabled="false" />
            <cc1:CustomButton ID="TransmissionsBtnPrint" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupTransmissionsPrint();"
                Visible="false" />
            <asp:Image ID="imgSepFooter" runat="server" Visible="false" src="../Images/Common/footer-sep-bar.png"
                Style="vertical-align: top; padding: 0 0 0 20px;" />
            <cc1:CustomButton ID="TransmissionsBtnAccept" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnAccept_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnAcceptADL" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnAcceptADL_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnAcceptADLR" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnAcceptADLR_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnAcceptLF" runat="server" CssClass="btnEnable clickable"
                CssClassDisabled="btnDisable" OnClientClick="disallowOp('ContentPlaceHolderContent');"
                OnMouseOver="btnHover" OnClick="TransmissionsBtnAcceptLF_Click" />
            <cc1:CustomButton ID="TransmissionsBtnReject" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnReject_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnView" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnView_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnViewADL" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnViewADL_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnViewADLR" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionsBtnViewADLR_Click"
                Visible="false" />
            <cc1:CustomButton ID="TransmissionsBtnAcquireRights" runat="server" CssClass="btnEnable clickable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="TransmissionBtnAcquireRights_Click"
                Visible="false" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnImgViewContributo" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnImgViewContributo_Click" />
            <asp:Button ID="btnImgCreaContributo" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnImgCreaContributo_Click" />
            <asp:Button ID="btnImgCloseTask" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnImgCloseTask_Click" />
            <asp:Button ID="btnImgBlockTask" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnImgBlockTask_Click" />
            <asp:Button ID="btnImgRiapriLavorazione" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnImgRiapriLavorazione_Click" />
            <asp:Button ID="btnImgRemoveTask" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnImgRemoveTask_Click" />
            <asp:HiddenField ID="idTrasmSingola" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenRemoveTask" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenCancelTask" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
