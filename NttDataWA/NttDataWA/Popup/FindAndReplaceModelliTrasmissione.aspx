<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindAndReplaceModelliTrasmissione.aspx.cs"
    Inherits="NttDataWA.popup.FindAndReplaceModelliTrasmissione" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <link type="text/css" href="../CSS/docspa_30.css" rel="Stylesheet" />
    <style type="text/css">
        .wizard
        {
            width: 98%;
            margin-top: 8px;
        }
        
        .wizard_header
        {
            font-weight: bold;
            font-size: 8pt;
            color: White;
            text-align: center;
        }
        
        .wizard_navigation
        {
            font-family: Verdana;
            font-size: 8pt;
            color: #284E98;
        }
        
        .wizard_sidebar_button
        {
            font-family: Verdana;
            color: Black;
        }
        
        .wizard_navigation
        {
            border-style: none;
        }
        
        .content
        {
            float: left;
            margin-left: 5px;
        }
        
        .clear
        {
            clear: both;
        }
        
        .content_checkbox
        {
            float: left;
            vertical-align: bottom;
        }
    </style>
    <script type="text/javascript" language="javascript">
        /*
        * Cancellazione della descrizione di un corrispondente se il codice è vuoto
        */
        function clearCorrData(codTxt, descrTxtId) {
            if (codTxt.value == '')
                document.getElementById(descrTxtId).value = '';

        }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="true"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="ReportGenerator" runat="server" Url="../popup/ReportGenerator.aspx"
        PermitClose="true" PermitScroll="false" IsFullScreen="false" Width="745" Height="400"
        CloseFunction="function (event, ui)  {__doPostBack('upButtons','');}" />
    <div style="width: 99%;">
        <asp:UpdatePanel ID="upAllPage" runat="server">
            <ContentTemplate>
                <asp:Button ID="btnRefresh" runat="server" Width="0" Height="0" Visible="false" />
                <fieldset class="filterAddressbook">
                    <asp:UpdatePanel ID="upWizard" runat="server">
                        <ContentTemplate>
                            <asp:Wizard ID="wzWizard" runat="server" CssClass="wizard" OnNextButtonClick="wzWizard_NextButtonClick"
                                OnFinishButtonClick="wzWizard_FinishButtonClick" OnPreviousButtonClick="wzWizard_PreviousButtonClick"
                                CancelButtonText="Annulla" CancelButtonStyle-CssClass="btnEnable" FinishCompleteButtonText="Conferma"
                                FinishCompleteButtonStyle-CssClass="btnEnable" FinishPreviousButtonText="Precedente"
                                FinishPreviousButtonStyle-CssClass="btnEnable" StartNextButtonText="Continua"
                                StartNextButtonStyle-CssClass="btnEnable" StepNextButtonText="Successivo" StepNextButtonStyle-CssClass="btnEnable"
                                StepPreviousButtonText="Precedente" StepPreviousButtonStyle-CssClass="btnEnable"
                                ActiveStepIndex="0">
                                <CancelButtonStyle CssClass="btnEnable" />
                                <FinishCompleteButtonStyle CssClass="btnEnable" />
                                <FinishPreviousButtonStyle CssClass="btnEnable" />
                                <HeaderStyle CssClass="wizard_header" />
                                <StartNextButtonStyle CssClass="btnEnable" />
                                <StepNextButtonStyle CssClass="btnEnable" />
                                <StepPreviousButtonStyle CssClass="btnEnable" />
                                <SideBarButtonStyle CssClass="wizard_sidebar_button" />
                                <SideBarStyle CssClass="pulsanti" Width="20%" BackColor="#2B6A97" />
                                <StepStyle CssClass="wizard_step" />
                                <NavigationStyle CssClass="wizard_navigation" />
                                <NavigationButtonStyle CssClass="PULSANTE" />
                                <SideBarTemplate>
                                    <asp:DataList ID="SideBarList" runat="server" OnItemDataBound="SideBarList_ItemDataBound">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="SideBarButton" runat="server" ForeColor="White"></asp:LinkButton>
                                        </ItemTemplate>
                                        <SelectedItemStyle Font-Bold="true" />
                                    </asp:DataList>
                                </SideBarTemplate>
                                <WizardSteps>
                                    <asp:WizardStep StepType="Start" ID="wzStepFind" runat="server" Title="Cerca" AllowReturn="false">
                                        <div class="content">
                                            <asp:Panel runat="server" ID="pnlRegistry">
                                                <span class="weight">
                                                    <asp:Label ID="lblRegistroFind" runat="server"></asp:Label>
                                                </span>
                                                <br />
                                                <asp:DropDownList ID="ddlRegistry" CssClass="chzn-select-deselect" runat="server"
                                                    Width="400" />
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="pnlFind">
                                                <span class="weight">
                                                    <asp:Label ID="lblRuoloFind" runat="server"></asp:Label>
                                                </span>
                                                <br />
                                                <cc1:CustomTextArea ID="txtFindCodice" Width="20%" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled"
                                                    runat="server" />
                                                <cc1:CustomImageButton ID="imgSrcFind" runat="server" ImageUrl="../Images/Icons/view_response_documents.png"
                                                    OnMouseOutImage="../Images/Icons/view_response_documents.png" OnMouseOverImage="../Images/Icons/view_response_documents_hover.png"
                                                    OnClick="imgSrcFind_Click" CssClass="clickable" />
                                                <cc1:CustomTextArea ID="txtFindDescrizione" Width="50%" runat="server" ReadOnly="true"
                                                    CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" />
                                                <cc1:CustomImageButton runat="server" ID="imgRubricaFind" ImageUrl="../Images/Icons/address_book.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                    Visible="true" OnClick="imgRubricaFind_Click" /><br />
                                            </asp:Panel>
                                            <div class="clear" />
                                            <asp:Panel ID="pnlReplace" runat="server">
                                                <span class="weight">
                                                    <asp:Label ID="lblRuoloReplace" runat="server"></asp:Label>
                                                </span>
                                                <br />
                                                <cc1:CustomTextArea ID="txtReplaceCodice" Width="20%" runat="server" CssClass="txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled" />
                                                <cc1:CustomImageButton ID="imgSrcReplace" runat="server" ImageUrl="../Images/Icons/view_response_documents.png"
                                                    OnMouseOutImage="../Images/Icons/view_response_documents.png" OnMouseOverImage="../Images/Icons/view_response_documents_hover.png"
                                                    OnClick="imgSrcReplace_Click" CssClass="clickable" />
                                                <cc1:CustomTextArea ID="txtReplaceDescrizione" runat="server" CssClass="txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled" ReadOnly="true" Width="50%" />
                                                <cc1:CustomImageButton runat="server" ID="imgRubricaReplace" ImageUrl="../Images/Icons/address_book.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                    Visible="true" OnClick="imgRubricaReplace_Click" />
                                            </asp:Panel>
                                        </div>
                                        <br />
                                        <div class="clear" />
                                        <div class="content_checkbox">
                                            <asp:CheckBox ID="chkCopyNotes" runat="server" />
                                        </div>
                                    </asp:WizardStep>
                                    <asp:WizardStep ID="WizardStep2" runat="server" Title="Sostituisci" AllowReturn="false"
                                        StepType="Finish">
                                    </asp:WizardStep>
                                </WizardSteps>
                            </asp:Wizard>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
                <asp:UpdatePanel ID="upSummary" runat="server">
                    <ContentTemplate>
                        <asp:Literal ID="ltlNoModels" runat="server" Visible="false" />
                        <asp:GridView ID="dgResult" runat="server" GridLines="Vertical" SkinID="datagrid"
                            AutoGenerateColumns="false" CssClass="tbl_rounded round_onlyextreme" Width="100%">
                            <RowStyle CssClass="NormalRow" Height="50" />
                            <AlternatingRowStyle CssClass="AltRow" />
                            <PagerStyle CssClass="recordNavigator2" />
                            <Columns>
                                <asp:BoundField ItemStyle-Font-Size="Small" DataField="CodiceModello" HeaderText="Codice modello"
                                    ItemStyle-Width="20%" />
                                <asp:BoundField ItemStyle-Font-Size="Small" DataField="DescrizioneModello" HeaderText="Descrizione modello"
                                    ItemStyle-Width="30%" />
                                <asp:BoundField ItemStyle-Font-Size="Small" DataField="SyntheticResult" HeaderText="Esito"
                                    ItemStyle-Width="10%" />
                                <asp:BoundField ItemStyle-Font-Size="Small" DataField="Message" HeaderText="Messaggio"
                                    ItemStyle-Width="40%" />
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="btnExport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick=" return __doPostBack('upButtons');" OnClick="btnExport_Click" />
            <cc1:CustomButton ID="btnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick=" return __doPostBack('upButtons');" OnClick="btnClose_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({
            allow_single_deselect: true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
