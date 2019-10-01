<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchTransfer.aspx.cs"
    Inherits="NttDataWA.Search.SearchTransfer" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="MassiveAddAdlUser" runat="server" Url="../Popup/MassiveAddAdlUser.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveRemoveAdlUser" runat="server" Url="../Popup/MassiveRemoveAdlUser.aspx?objType=P"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="upTop" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitSearchArchive" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server" Visible="false">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="colMassiveOperationSx">
                                            <asp:DropDownList runat="server" ID="SearchDocumentDdlMassiveOperation" Width="400"
                                                AutoPostBack="true" CssClass="chzn-select-deselect" OnSelectedIndexChanged="SearchDocumentDdlMassiveOperation_SelectedIndexChanged" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <fieldset>
                                    <asp:UpdatePanel ID="upTextID_E_Descrizione" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LtlIdVersamentoSearch"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="ddl_idTransfer_C" runat="server" Width="140px" AutoPostBack="true"
                                                        CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_idTransfer_C_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlIdTransferDA"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initIdTransfer_C" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                        CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal runat="server" ID="LtlIdTransferA"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineIdTransfer_C" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                                        CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="LtlIdVersamentoSearchDescrizione"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <cc1:CustomTextArea ID="TxtDescrizioneVersamento" runat="server" CssClass="txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled">
                                                </cc1:CustomTextArea>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                            <!-- intervals DATE-->
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <asp:UpdatePanel ID="upPnlIntervals" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <!-- Creato -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaCreazione" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_dtaCreazione" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaCreazione_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaCreazione_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaCreazione_opt1" Value="1" />
                                                            <asp:ListItem id="dtaCreazione_opt2" Value="2" />
                                                            <asp:ListItem id="dtaCreazione_opt3" Value="3" />
                                                            <asp:ListItem id="dtaCreazione_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCreazioneFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaCreazione_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaCreazioneTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaCreazione_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Analisi completata -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaAnalisiCompletata" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_dtaAnalisiCompletata" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaAnalisiCompletata_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaAnalisiCompletata_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaAnalisiCompletata_opt1" Value="1" />
                                                            <asp:ListItem id="dtaAnalisiCompletata_opt2" Value="2" />
                                                            <asp:ListItem id="dtaAnalisiCompletata_opt3" Value="3" />
                                                            <asp:ListItem id="dtaAnalisiCompletata_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaAnalisiCompletataFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaAnalisiCompletata_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaAnalisiCompletataTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaAnalisiCompletata_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Proposto -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaProposto" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_dtaProposto" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaProposto_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaProposto_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaProposto_opt1" Value="1" />
                                                            <asp:ListItem id="dtaProposto_opt2" Value="2" />
                                                            <asp:ListItem id="dtaProposto_opt3" Value="3" />
                                                            <asp:ListItem id="dtaProposto_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaPropostoFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaProposto_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaPropostoTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaProposto_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Approvato -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaApprovato" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_dtaApprovato" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaApprovato_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaApprovato_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaApprovato_opt1" Value="1" />
                                                            <asp:ListItem id="dtaApprovato_opt2" Value="2" />
                                                            <asp:ListItem id="dtaApprovato_opt3" Value="3" />
                                                            <asp:ListItem id="dtaApprovato_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaApprovatoFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaApprovato_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaApprovatoTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaApprovato_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <!-- Eseguito -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="lit_dtaEseguito" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:DropDownList ID="ddl_dtaEseguito" runat="server" Width="150" AutoPostBack="True"
                                                            OnSelectedIndexChanged="ddl_dtaEseguito_SelectedIndexChanged" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaEseguito_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaEseguito_opt1" Value="1" />
                                                            <asp:ListItem id="dtaEseguito_opt2" Value="2" />
                                                            <asp:ListItem id="dtaEseguito_opt3" Value="3" />
                                                            <asp:ListItem id="dtaEseguito_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaEseguitoFrom" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaEseguito_TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="col2">
                                                        <asp:Label ID="lbl_dtaEseguitoTo" runat="server" Visible="False"></asp:Label>
                                                    </div>
                                                    <div class="col4">
                                                        <cc1:CustomTextArea ID="dtaEseguito_TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                            CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                            </div>
                            <!-- Radio button stati -->
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <asp:UpdatePanel ID="upRadioButtonStati" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <!-- Stati radiobutton -->
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal ID="LitStateType" runat="server" /></span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <asp:RadioButtonList ID="rblStateType" runat="server" AutoPostBack="True" CssClass="rblHorizontal"
                                                            RepeatLayout="UnorderedList">
                                                            <asp:ListItem id="optCompletati" runat="server" Value="C" Selected="True" />
                                                            <asp:ListItem id="optErrori" runat="server" Value="E" />
                                                            <asp:ListItem id="optTutti" runat="server" Value="T" />
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <div id="contentDxTopProject">
                            <%--Label numero di elementi --%>
                            <asp:UpdatePanel runat="server" ID="UpnlNumeroTransfer" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col">
                                            <div class="p-padding-left">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="SearchTransferLitNomeGriglia" />
                                                    </span>
                                                    <asp:Label runat="server" ID="transferLblNumeroVersamenti"></asp:Label></p>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                                        <ContentTemplate>
                                            <div class="contentListResult">
                                                <fieldset>
                                                    <asp:GridView ID="gvResultTransfer" runat="server" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                                        Width="99%" AutoGenerateColumns="False" AllowPaging="false" OnPreRender="gvResultTransfer_PreRender"
                                                        OnSelectedIndexChanging="gvResultTransfer_SelectedIndexChanging" DataKeyNames="System_id"
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
                                                                    <asp:Label ID="LblSystem_id_PolicylabelHeader" Text='ID Vers.' runat="server"></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblSystem_id_Versamento" runat="server" Text='<%# Bind("System_id") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:Label ID="LblDescription_PolicylabelHeader" Text='Descrizione' runat="server"></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblDescrizione" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:Label ID="LblNumeroDoc_PolicylabelHeader" Text='Stato' runat="server"></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblStato" runat="server" Text='<%# Bind("STATO") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:Label ID="LblNumeroFasc_PolicylabelHeader" Text='Data' runat="server"></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblDataEsecuzione" runat="server" Text='<%# Bind("DateTime") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:Label ID="LblNumeroFasc_PolicylabelHeader" Text='N.Doc' runat="server"></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblTotale_documenti" runat="server" Text='<%# SetTotDocTransfer(Eval("NUMERO_DOC_EFFETTIVI"),Eval("NUMERO_DOC_COPIE"))  %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:Label ID="LblNumeroFasc_PolicylabelHeader" Text='Eff./Copie' runat="server"></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblNum_documenti_copiatiNum_documenti_trasferiti" runat="server" Text='<%#  SetNumDocTransfer(Eval("NUMERO_DOC_EFFETTIVI"),Eval("NUMERO_DOC_COPIE"))  %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:Label ID="lblckb" runat="server" Text='Dett.'></asp:Label>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <cc1:CustomImageButton ID="btnTransferDetails" ImageUrl="../Images/Icons/search_response_documents.png"
                                                                        runat="server" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                                                                        OnMouseOutImage="../Images/Icons/search_response_documents.png" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                                                                        CssClass="clickableLeft" OnClick="btnTransferDetails_Click" />
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
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <uc:ajaxpopup2 Id="MassiveExportData" runat="server" Url="../Popup/MassiveAddAdlUser.aspx?objType=P"
                Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', ''); }" />
            <cc1:CustomButton ID="SearchTransferSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchTransferSearch_Click" />
            <cc1:CustomButton ID="SearchTransferRemoveFilters" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchTransferRemoveFilters_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        });     
    
    </script>
</asp:Content>
