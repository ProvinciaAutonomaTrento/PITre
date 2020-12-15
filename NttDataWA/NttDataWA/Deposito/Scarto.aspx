<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Base.Master"
    CodeBehind="Scarto.aspx.cs" Inherits="NttDataWA.Deposito.Scarto" %>

<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ACT" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/ScartiTab.ascx" TagPrefix="uc2" TagName="HeaderScarto" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.collapsed" });
        });
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="DisposalReport" runat="server" Url="../popup/DisposalReport.aspx" PermitClose="True"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnDisposalReportPostback').click();}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <contenttemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitScartoProject" runat="server"></asp:Literal></p>
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
                                <uc2:HeaderScarto ID="ScartiTab" runat="server" PageCaller="NEW" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        <div class="row">
                                <div class="colMassiveOperationDx">
                                    <NttDL:CustomButton ID="BtnDisposalReport" runat="server" CssClass="buttonAbort" CssClassDisabled="buttonAbortDisable"
                                        OnMouseOver="buttonAbortHover" OnClick="cImgBtnDisposalReport_Click"  />
                                </div>
                            </div>
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
            <asp:UpdatePanel runat="server" ID="upTextBoxDisposalDetails" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitScartoId" runat="server" /></strong></div>
                            <div class="colHalf">
                                <NttDL:CustomTextArea ID="TxtIdScarto" runat="server" CssClass="txt_addressBookLeft"
                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False"></NttDL:CustomTextArea>
                            </div>
                        </div>
                        <div class="row">
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitScartoDescrizione" runat="server" /></strong></div>
                            <div class="colHalf2">
                                <NttDL:CustomTextArea ID="TxtDescrizioneScarto" runat="server" CssClass="txt_input_full"
                                    CssClassReadOnly="txt_input_full_disabled">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                        <div class="row">
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitScartoNote" runat="server" /></strong></div>
                            <div class="colHalf2">
                                <NttDL:CustomTextArea ID="TxtNoteScarto" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="upTextBoxDisposalState" UpdateMode="Conditional"
                ChildernAsTriggers="True">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <!-- STATO VERSAMENTO -->
                            <h2 class="expand">
                                <asp:Literal ID="litExpandStatoScarto" runat="server" /></h2>
                            <div class="collapse shown">
                                <asp:Panel ID="upPnlStatoScarto" runat="server">
                                    <div id="LabelDescrizioneStato" class="colHalf">
                                        <strong>
                                            <asp:Literal ID="LitScartoDescrizioneStatoScarto" runat="server" /></strong>
                                    </div>
                                    <div id="ColonnaLabelStatiScarto" class="colHalf4">
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoInDefinizione" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_projectRight_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoAnalisiCompletata" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoProposto" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoApprovato" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoInEsecuzione" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoCompletato" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtStatoErrore" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                    </div>
                                    <div id="LabelDescrizioneData" class="col">
                                        <strong>
                                            <asp:Literal ID="LitScartoDescrizioneDataStatoScarto" runat="server" /></strong>
                                    </div>
                                    <div id="ColonnaTextScarto" class="colHalf4">
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtDATAStatoInDefinizione" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_projectRight_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtDATAStatoAnalisiCompletata" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtDATAStatoProposto" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtDATAStatoApprovato" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtDATAStatoInEsecuzione" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <NttDL:CustomTextArea ID="TxtDATAStatoCompletato" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <div>
                                                    <NttDL:CustomTextArea ID="TxtDATAStatoErrore" runat="server" CssClass="txt_addressBookLeft"
                                                        CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false">
                                                    </NttDL:CustomTextArea>
                                                    <div id="ArchiveReportErrore">
                                                        <NttDL:CustomImageButton ID="cImgBtnReportErrore" ImageUrl="../Images/Icons/ico_view_document.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/ico_view_document_hover.png"
                                                            OnMouseOutImage="../Images/Icons/ico_view_document.png" ImageUrlDisabled="../Images/Icons/ico_view_document_disabled.png"
                                                            CssClass="clickableLeft" OnClick="cImgBtnReportErrore_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                            <!-- FINE STATO Scarto -->
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="ContentCustomSx" class="ContentCustomSx">
            </div>
        </div>
        <div id="contentDx">
            <div id="contentListDocument" class="contentDxDoc">
                <asp:UpdatePanel runat="server" ID="upDisposalDocument" UpdateMode="Conditional"
                    ChildernAsTriggers="True">
                    <ContentTemplate>
                        <fieldset>
                            <div class="row">
                                <div class="col-full">
                                    <strong>
                                        <asp:Literal ID="LitNoDocumentInPolicyDisposal" runat="server" /></strong>
                                    <asp:UpdatePanel ID="SmallUpdate" UpdateMode="Always" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="gvDocumentInPolicyDisposal" runat="server" ClientIDMode="Static"
                                                CssClass="tbl_rounded_custom round_onlyextreme" Width="99%" AutoGenerateColumns="false"
                                                BorderWidth="0" AllowPaging="true" PageSize="9" OnPageIndexChanging="gvDocumentInPolicyDisposalPageIndexChanging_click">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <PagerStyle CssClass="recordNavigator2" />
                                                <Columns>
                                                   <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblRegistro_PolicyDocumentslabelHeader" Text='AOO' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblRegistro_PolicyDocuments" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblTitolario_PolicyDocumentslabelHeader" Text='Titolario' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblTitolario_PolicyDocuments" runat="server" Text='<%# Bind("Titolario") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblClassetitolario_PolicyDocumentslabelHeader" Text='Classe' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblClassetitolario_PolicyDocuments" runat="server" Text='<%# Bind("Classetitolario") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblTipologia_PolicyDocumentslabelHeader" Text='Tipologia' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblTipologia_PolicyDocuments" runat="server" Text='<%# Bind("Tipologia") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblAnnoCreazione_PolicylabelHeader" Text='Anno Creazione' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblAnnoCreazione_PolicyDocuments" runat="server" Text='<%# Bind("AnnoCreazione") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblTotale_PolicylabelHeader" Text='Totale' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblTotale_PolicyDocuments" runat="server" Text='<%# Bind("Totale") %>'></asp:Label>
                                                            <asp:Label ID="CountDistinct" runat="server" Text='<%# SetCountDocuments(Eval("CountDistinct"))%>'
                                                                ClientIDMode="Static" Visible="false" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="gvDocumentInPolicyDisposal" EventName="PageIndexChanging" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="contentListProject" class="contentDxFasc">
                <asp:UpdatePanel runat="server" ID="upDisposalProject" UpdateMode="Conditional" ChildernAsTriggers="True">
                    <ContentTemplate>
                        <fieldset>
                            <div class="row">
                                <div class="col-full">
                                    <strong>
                                        <asp:Literal ID="LitNoProjectInPolicyDisposal" runat="server" /></strong>
                                    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Always" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="gvProjectInPolicyDisposal" runat="server" ClientIDMode="Static"
                                                CssClass="tbl_rounded_custom round_onlyextreme" Width="99%" AutoGenerateColumns="false"
                                                BorderWidth="0" AllowPaging="true" PageSize="9" OnPageIndexChanging="gvProjectInPolicyDisposalPageIndexChanging_click"
                                                EnableViewState="true">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <PagerStyle CssClass="recordNavigator2" />
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblRegistro_PolicyDocumentslabelHeader" Text='AOO' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblRegistro_PolicyDocuments" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblTitolario_PolicyDocumentslabelHeader" Text='Titolario' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblTitolario_PolicyDocuments" runat="server" Text='<%# Bind("Titolario") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblClassetitolario_PolicyDocumentslabelHeader" Text='Classe' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblClassetitolario_PolicyDocuments" runat="server" Text='<%# Bind("Classetitolario") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblTipologia_PolicyDocumentslabelHeader" Text='Tipologia' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblTipologia_PolicyDocuments" runat="server" Text='<%# Bind("Tipologia") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblAnnoCreazione_PolicylabelHeader" Text='Anno Chiusura' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblAnnoCreazione_PolicyDocuments" runat="server" Text='<%# Bind("AnnoChiusura") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="LblTotale_PolicylabelHeader" Text='Totale' runat="server"></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblTotale_PolicyDocuments" runat="server" Text='<%# Bind("Totale") %>'></asp:Label>
                                                            <asp:Label ID="CountDistinct" runat="server" Text='<%# SetCountProjects(Eval("CountDistinct"))%>'
                                                                ClientIDMode="Static" Visible="false" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="gvProjectInPolicyDisposal" EventName="PageIndexChanging" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <!--    </contenttemplate>
             </asp:UpdatePanel> -->
        </div>
    </div>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpHiddenFieldERASE">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddeConfirmDeleteDisposal" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpHiddenFieldUPDATE">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddeConfirmUpdateDisposal" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="upHiddenFieldStatoProposto">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddeConfirmProponiDisposal" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="upHiddenFieldStatoApprovato">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddeConfirmApprovaDisposal" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="upHiddenFieldStatoInEsecuzione">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddeConfirmEseguiDisposal" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="upHiddenCameBackToDefinition">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddenConfirmBackToDefinition" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="upHiddenFieldStatoInAnalisiCompletata">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="HiddenConfirmAnalisiScarto" ClientIDMode="static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpTranfetButtons">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnScartoNuovo" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoNuovo_Click" />
            <NttDL:CustomButton ID="btnScartoAnalizza" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoAnalizza_Click" />
            <NttDL:CustomButton ID="btnScartoProponi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoProponi_Click" />
            <NttDL:CustomButton ID="btnScartoApprova" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoApprova_Click" />
            <NttDL:CustomButton ID="btnScartoEsegui" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoEsegui_Click" />
            <NttDL:CustomButton ID="btnScartoModifica" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoModifica_Click" />
            <NttDL:CustomButton ID="btnScartoElimina" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnScartoElimina_Click" />
            <asp:Button ID="btnPolicyPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnPolicyPostback_Click" />
            <asp:Button ID="btnDisposalReportPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnDisposalReportPostback_click" />   
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
