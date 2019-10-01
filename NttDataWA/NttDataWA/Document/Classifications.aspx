<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Classifications.aspx.cs"
    Inherits="NttDataWA.Document.Classification" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/HeaderDocument.ascx" TagPrefix="uc2" TagName="HeaderDocument" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="DocumentButtons" %>
<%@ Register Src="~/UserControls/DocumentTabs.ascx" TagPrefix="uc4" TagName="DocumentTabs" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc5" TagName="ViewDocument" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function SingleSelect(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio' && elm != current && re.test(elm.name)) {
                    if (elm.disabled)
                        alert('Non è possibile cambiare la fascicolazione primaria su un fascicolo di cui non si possiedono i diritti')
                    else
                        elm.checked = false;
                }
            }
        }

        function acePopulated(sender, e) {
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
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="UplodadFile" runat="server" Title="Acquisisci un documento" Url="../popup/UploadFile.aspx?idDoc=<%=GetIdDocumento()%>"
        Width="600" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="RepositoryView" runat="server" Url="../Repository/RepositoryView.aspx" Width="850"
        Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="ActiveXScann" runat="server" Title="Acquisizione da scanner" Url="../popup/acquisizione.aspx" ShowLoading="false"
         Width="1000" Height="700" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalSignSelector" runat="server" Title="Firma documento" Url="../popup/DigitalSignSelector.aspx?TipoFirma=sign"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalCosignSelector" runat="server" Title="Firma documento" Url="../popup/DigitalSignSelector.aspx?TipoFirma=cosign&Caller=cosign"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup2 Id="Signature" runat="server" Url="../popup/Signature.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx" ForceDontClose="true"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="VersionAdd" runat="server" Url="../popup/version_add.aspx" Width="450"
        Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="ModifyVersion" runat="server" Url="../popup/version_add.aspx?modifyVersion=t"
        Width="450" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlRapidCollation', ''); }" />
    <uc:ajaxpopup2 Id="SaveDialog" runat="server" Url="../CheckInOutApplet/CheckInOutSaveLocal.aspx" IsFullScreen="false"
        PermitClose="true" PermitScroll="false" Width="500" Height="430" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="CheckOutDocument" runat="server" Url="../CheckInOutApplet/CheckOutDocument.aspx" IsFullScreen="false"
        PermitClose="true" PermitScroll="false" Width="500" Height="250" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />    
    <uc:ajaxpopup2 Id="UndoCheckOut" runat="server" Url="../CheckInOutApplet/UndoPendingChange.aspx" IsFullScreen="false"
        PermitClose="true" PermitScroll="false" Width="500" Height="300" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="CheckInDocument" runat="server" Url="../CheckInOutApplet/CheckInDocument.aspx" IsFullScreen="false"
        PermitClose="true" PermitScroll="false" Width="500" Height="300" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="OpenLocalCheckOutFile" runat="server" Url="../CheckInOutApplet/OpenLocalCheckOutFile.aspx" IsFullScreen="false"
        PermitClose="true" PermitScroll="false" Width="500" Height="300" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="ShowCheckOutStatus" runat="server" Url="../CheckInOutApplet/ShowCheckOutStatus.aspx" IsFullScreen="false"
        PermitClose="true" PermitScroll="false" Width="500" Height="400" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="CheckOutModelApplet" runat="server" Url="../CheckInOutApplet/CheckOutModel.aspx" IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="CheckOutModelActiveX" runat="server" Url="../CheckInOut/CheckOutModel.aspx" IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
        <!-- Apertura popup searchproject Laura 19 Marzo -->
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="../popup/SearchProject.aspx?caller=classifica"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" Parametri="" CloseFunction="function (event, ui) { __doPostBack('UpPnlRapidCollation', '');}" />
    <uc:ajaxpopup2 Id="HSMSignature" runat="server" Url="../popup/HSM_Signature.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="700" Height="400" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx"
        PermitClose="false" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="StartProcessSignature" runat="server" Url="../popup/StartProcessSignature.aspx"
        Width="1200" Height="800" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup2 Id="DetailsLFAutomaticMode" runat="server" Url="../popup/DetailsLFAutomaticMode.aspx"
        Width="750" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpDocumentButtons', 'SignatureProcessConcluted');}" />
    <uc:ajaxpopup2 Id="DigitalVisureSelector" runat="server" Title="Approva documento" Url="../popup/DigitalVisure.aspx"
        Width="600" Height="300" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
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
                                    <uc4:DocumentTabs runat="server" PageCaller="CLASSIFICATIONS" ID="DocumentTabs">
                                    </uc4:DocumentTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <uc3:DocumentButtons runat="server" ID="DocumentButtons" PageCaller="CLASSIFICATIONS" />
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
                                            <asp:Label ID="LblObject" runat="server"></asp:Label></span>
                                    </div>
                                </div>
                                <div id="row_object" class="row">
                                    <asp:Literal ID="litObject" runat="server" /></div>
                            </fieldset>
                        </div>
                        <asp:UpdatePanel ID="UpNFascicoli" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <span class="weight">
                                    <asp:Label ID="LblTitle" runat="server"></asp:Label></span>
                                <asp:Label ID="lblNFascicoli" runat="server"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="tab_classGrid">
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel ID="UpGrid" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>
                                            <asp:HiddenField ID="deleteDoc" runat="server" ClientIDMode="Static" />
                                            <asp:GridView ID="GridViewClassifications" runat="server" AutoGenerateColumns="false"
                                                Width="98%" CssClass="tbl_rounded_custom round_onlyextreme" OnRowDeleting="GridViewClassifications_RowDeleting"
                                                BorderWidth="0" AllowPaging="True" PageSize="5" OnPageIndexChanging="GridViewClassifications_PageIndexChanging"    OnRowCommand="GridViewClassifications_RowCommand" >
                                                <RowStyle CssClass="NormalRow"/>
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <PagerStyle CssClass="recordNavigator2" />
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:LinkButton  ID="lnkCodice" runat="server" CommandArgument='<%# Bind("systemId") %>' CommandName="aperturaProject"
                                                                CssClass="clickableRight" ToolTip='<%# Bind("Tooltip0")%>' Text='<%# Bind("Codice") %>'></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblDescrizione" Text='<%# Bind("Descrizione") %>' ToolTip='<%# Bind("Tooltip1") %>'
                                                                CssClass="clickable" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblRegistro" Text='<%# Bind("Registro") %>' ToolTip='<%# Bind("Tooltip2") %>'
                                                                CssClass="clickable" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderImageUrl="../Images/Icons/delete.png" HeaderStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="ibDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                Visible="false" CommandArgument='<%# Bind("Codice") %>' ToolTip='<%# Bind("Tooltip3") %>'
                                                                ImageUrl="../Images/Icons/delete.png" autopostback="true" CssClass="clickable" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false" HeaderStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:RadioButton ID="rbSel" runat="server" onclick="SingleSelect('rbS',this);" OnCheckedChanged="rbSel_CheckedChanged"
                                                                AutoPostBack="true" ToolTip='<%# Bind("Tooltip4") %>' CssClass="clickable" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="FasciaPrimaria" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lbl_fascPrima" Text='<%# Bind("fascPrimaria") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblstato" Text='<%# Bind("Stato") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblcodice" Text='<%# Bind("Codice") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblsysReg" Text='<%# Bind("sysReg") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblidTitolario" Text='<%# Bind("idTitolario") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblsystemId" Text='<%# Bind("systemId") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblTipoFasc" Text='<%# Bind("tipoFasc") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblSicurezzaUtente" Text='<%# Bind("sicurezzaUtente") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-full">
                                <asp:UpdatePanel ID="UpPnlRapidCollation" runat="server" UpdateMode="Conditional"
                                    ClientIDMode="Static">
                                    <ContentTemplate>
                                        <div class="row">
                                            <asp:Panel ID="PnlRegistries" runat="server" Visible="false">
                                                <fieldset>
                                                    <asp:UpdatePanel runat="server" ID="UpDdlRegistries" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col7">
                                                                    <span class="weight">
                                                                        <asp:Label runat="server" ID="ClassificationsLabRegistries"></asp:Label>
                                                                    </span>
                                                                </div>
                                                                <div>
                                                                    <asp:DropDownList runat="server" ID="DdlRegistries" CssClass="chzn-select-deselect"
                                                                        AutoPostBack="true" OnSelectedIndexChanged="DdlRegistries_SelectedIndexChanged"
                                                                        Width="300">
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </fieldset>
                                            </asp:Panel>
                                        </div>
                                        <div class="row">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Label ID="LblTitleClassification" runat="server" Visible="true"></asp:Label></span><asp:Label
                                                                    ID="LblClassRequired" CssClass="little" Visible="false" runat="server">*</asp:Label></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                            ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png" OnMouseOutImage="../Images/Icons/classification_scheme.png"
                                                            OnClientClick="return ajaxModalPopupOpenTitolario();" CssClass="clickable" />
                                                        <cc1:CustomImageButton ID="ClassificationSchemaSearchProject" ImageUrl="../Images/Icons/search_projects.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/search_projects_hover.png" OnMouseOutImage="../Images/Icons/search_projects.png"
                                                            ImageUrlDisabled="../Images/Icons/search_projects_disabled.png" CssClass="clickable" OnClientClick="return ajaxModalPopupSearchProject();" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:PlaceHolder runat="server" ID="PnlProject">
                                                                <asp:HiddenField ID="IdProject" runat="server" />
                                                                <div class="colHalf">
                                                                    <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft"
                                                                        autocomplete="off" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCodeProject_OnTextChanged"
                                                                        AutoPostBack="true"></cc1:CustomTextArea>
                                                                </div>
                                                                <div class="colHalf2">
                                                                    <div class="colHalf3">
                                                                        <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_addressBookRight"
                                                                            autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled"></cc1:CustomTextArea>
                                                                    </div>
                                                                </div>
                                                               <uc1:AutoCompleteExtender runat="server" ID="RapidSender" TargetControlID="TxtDescriptionProject"
                                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                                                                    MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                    UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteDescriptionProject"
                                                                    OnClientPopulated="acePopulated">
                                                                </uc1:AutoCompleteExtender>
                                                            </asp:PlaceHolder>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </fieldset>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpHiddenConfirm" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:HiddenField ID="HiddenControlPrivateClass" runat="server" ClientIDMode="Static" />
                                        <asp:HiddenField ID="HiddenPublicFolder" runat="server" ClientIDMode="Static" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <uc5:ViewDocument ID="ViewDocument" runat="server" PageCaller="CLASSIFICATIONS">
                        </uc5:ViewDocument>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel runat="server" ID="UpDocumentButtons" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="ClassificationBtnSave" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="ClassificationBtnSave_Click" />
            <cc1:CustomButton ID="ClassificationBntCreaFasc" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="ClassificationBntCreaFasc_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
