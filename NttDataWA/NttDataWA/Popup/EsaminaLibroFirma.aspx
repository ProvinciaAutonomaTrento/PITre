<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsaminaLibroFirma.aspx.cs"
    Inherits="NttDataWA.Popup.EsaminaLibroFirma" MasterPageFile="~/MasterPages/Popup.Master"
    Title="prova" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc9" TagName="ViewDocument" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .TreeAddressBook
        {
            padding: 0;
            overflow: auto;
        }
        
        .TreeAddressBook td, .TreeAddressBook th, .TreeAddressBook tr
        {
            border: 0;
            padding: 0;
            margin: 0;
            height: 20px;
        }
        
        .TreeAddressBook table
        {
            padding: 0;
            margin: 0;
            height: 0;
            border: 0;
        }
        
        .col-marginSx2
        {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
            white-space: nowrap;
        }
        
        .col-marginSx2 p
        {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 4px;
        }
        
        .col-marginSx
        {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }
        
        .col-marginDx
        {
            float: left;
            margin-right: 20px;
            text-align: center;
        }
        
        .colNavigation
        {
            padding-top: 10px;
            text-align: center;
            vertical-align: top;
            padding-left: 15%;
        }
        
        .col-marginDx img
        {
            float: left;
        }
        
        .col-marginSx p
        {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 8px;
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
        
        .col-right-no-margin2
        {
            float: right;
            margin: 0px;
            padding: 0px;
            padding-left: 5px;
            padding-right: 12px;
            text-align: right;
        }
        .colHalf2
        {
            float: right;
            margin: 0px;
            text-align: right;
            width: 80%;
        }
        
        .colHalf3
        {
            margin-left: 6px;
            margin-right: 5px;
        }
        
        .row
        {
            clear: both;
            min-height: 20px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
        
        #container
        {
            position: fixed;
            left: 0px;
            bottom: 71px;
            right: 0px;
            background: #ffffff;
            text-align: left;
            padding: 2px;
            overflow: hidden;
            height: 100%;
        }
        
        #containerDetails
        {
            height: 100%;
            overflow: auto;
        }
        .containerDetailsInfoDoc
        {
            margin: 2px;
            border: 1px solid #cccccc;
            padding: 5px;
            padding-top: 10px;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
        
        .navigation
        {
            width: 100%;
            text-align: center;
        }
        .TreeAddressBook_node a:link, .TreeAddressBook_node a:visited, .TreeAddressBook_node a:hover
        {
            padding: 0 5px;
        }
        
        .TreeAddressBook_selected
        {
            background-color: #477FAF;
        }
        
        .TreeAddressBook_selected a:link, .TreeAddressBook_selected a:visited, .TreeAddressBook_selected a:hover
        {
            padding: 0 5px;
            background-color: transparent;
            color: #fff;
        }
        
        .TreeSignatureProcess
        {
            padding: 0;
            margin-right: 30px;
            color: #0f64a1;
            overflow: auto;
            position: relative;
        }
        
        
        .TreeSignatureProcess img
        {
            width: 20px;
            height: 20px;
        }
        
        .TreeSignatureProcess_node a:link, .TreeSignatureProcess_node a:visited, .TreeSignatureProcess_node a:hover
        {
            padding: 0 5px;
        }
        
        .TreeSignatureProcess_selected
        {
            background-color: #477FAF;
            color: #fff;
        }
        
        .TreeSignatureProcess_selected a:link, .TreeSignatureProcess_selected a:visited, .TreeSignatureProcess_selected a:hover
        {
            padding: 0 5px;
            background-color: transparent;
            color: #fff;
        }
        
        .containerDetails fieldset
        {
            margin: 2px;
            border: 1px solid #cccccc;
            padding: 5px;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
        
        .col12
        {
            margin: 0px;
            padding: 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="InterruptionSignatureProcess" runat="server" Url="../popup/InterruptionSignatureProcess.aspx?from=E"
        Width="500" Height="350" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', '');}" />
    <div class="container">
        <div class="contentPopUpSX">
            <div class="containerDetails">
                <asp:Panel ID="pnlDetails" runat="server" Height="520px" Width="100%" ScrollBars="Auto">
                    <div class="row">
                        <asp:Panel ID="pnlInfoDoc" runat="server" Width="100%">
                            <asp:UpdatePanel ID="upPnlInfoDoc" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <div class="containerDetailsInfoDoc">
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="ltlIdDocumento"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col">
                                                <asp:Label ID="lblIdDocumento" runat="server"></asp:Label>
                                            </div>
                                            <asp:Panel ID="pnlSegnatura" runat="server">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlSegnatura"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="col">
                                                    <asp:Label ID="lblSegnatura" runat="server"></asp:Label>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="LtlDataCreazione"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col">
                                                <asp:Label ID="lblDataCreazione" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="ltlOggetto"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col12" style="width: 100%">
                                                <div class="esmaninaTextOverflow">
                                                    <asp:Label ID="lblOggetto" CssClass="clickable" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <asp:Panel ID="pnlOggettoAllegato" runat="server">
                                            <div class="row">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlOggettoAllegato"></asp:Literal>
                                                    </span>
                                                    <div class="col12" style="width: 100%">
                                                        <div class="esmaninaTextOverflow">
                                                            <asp:Label ID="lblOggettoAllegato" CssClass="clickable" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="ltlFile"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col">
                                                <asp:Label ID="lblFile" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <asp:Panel ID="pnlTipologia" runat="server" ClientIDMode="Static">
                                            <div class="row">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlTipologia"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lblTipologia" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlDestinatario" runat="server" ClientIDMode="Static">
                                            <div class="row">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlDestinatario"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="col12" style="width: 100%">
                                                    <div class="esmaninaTextOverflow">
                                                        <asp:Label ID="lblDestinatario" runat="server"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </div>
                    <div class="row" style="padding-top: 10px">
                        <asp:Panel ID="pnlInfoElementoLF" runat="server" Width="100%">
                            <asp:UpdatePanel ID="upPnlInfoElementoLF" runat="server" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div class="containerDetailsInfoDoc">
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="ltlTipoFirma"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col">
                                                <asp:Image runat="server" ID="imgTypeSignature" Width="15px" CssClass="clickableRight" />
                                            </div>
                                            <div class="col2">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlStato"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="col4">
                                                    <asp:UpdatePanel ID="upPnlImgState" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <cc1:CustomImageButton runat="server" ID="imgStato" Width="20px" CssClass="clickableLeft"
                                                                OnClick="BtnStato_Click" OnClientClick="disallowOp('Content2');" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                        </div>
                                        <asp:Panel ID="pnlMotivoRespingimento" runat="server">
                                            <div class="row">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlMotivoRespingimento"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="colHalfEsamina">
                                                    <cc1:CustomTextArea ID="txtMotivoRespingimento" runat="server" CssClass="txt_input_full"
                                                        CssClassReadOnly="txt_input_full_disabled" ReadOnly="true" />
                                                </div>
                                                <div class="col-right-no-margin2">
                                                    <cc1:CustomImageButton ID="btnModifyMotivoRespingimento" runat="server" CssClass="clickableLeft"
                                                        OnClick="btnModifyMotivoRespingimento_Click" ImageUrl="../Images/Icons/edit_verion.png"
                                                        OnMouseOverImage="../Images/Icons/edit_verion.png" OnMouseOutImage="../Images/Icons/edit_verion.png"
                                                        ImageUrlDisabled="../Images/Icons/edit_verion_disabled.png" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlProponente" runat="server" Width="99%">
                                            <div class="row">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlProponente"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="col12" style="width: 100%">
                                                    <div class="esmaninaTextOverflow">
                                                        <asp:Label ID="lblProponente" runat="server" CssClass="clickable"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="ltlProvieneDa"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col12" style="width: 100%">
                                                <div class="esmaninaTextOverflow">
                                                    <asp:Label ID="lblProvieneDa" runat="server" CssClass="clickable"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="ltlDataInserimento"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="col">
                                                <asp:Label ID="lblDataInserimento" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <asp:Panel ID="pnlNote" runat="server">
                                            <div class="row">
                                                <div class="col-marginSx2">
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="ltlNote"></asp:Literal>
                                                    </span>
                                                </div>
                                                <div class="col12" style="width: 100%">
                                                    <div class="esmaninaTextOverflow">
                                                        <asp:Label ID="lblNote" CssClass="clickable" runat="server"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </div>
                    <div class="navigation">
                        <asp:Panel ID="pnlNavigationButtons" runat="server">
                            <asp:UpdatePanel ID="upPnlNavigationButtons" runat="server" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div class="colNavigation">
                                        <div class="col-marginDx">
                                            <cc1:CustomImageButton runat="server" ID="btn_first" ImageUrl="../Images/Icons/smistamento_left_left.png"
                                                CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_left_left.png"
                                                ImageUrlDisabled="../Images/Icons/smistamento_left_left_disabled.png" OnMouseOverImage="../Images/Icons/smistamento_left_left_hover.png"
                                                OnClick="btn_first_Click" OnClientClick="disallowOp('Content2');" />
                                        </div>
                                        <div class="col-marginDx">
                                            <cc1:CustomImageButton runat="server" ID="btn_previous" ImageUrl="~/Images/Icons/smistamento_left.png"
                                                CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_left.png" ImageUrlDisabled="../Images/Icons/smistamento_left_disabled.png"
                                                OnMouseOverImage="../Images/Icons/smistamento_left_hover.png" OnClick="btn_previous_Click"
                                                OnClientClick="disallowOp('Content2');" />
                                        </div>
                                        <div class="col-marginDx">
                                            <span class="weight">
                                                <asp:Label ID="lbl_contatore" runat="server" CssClass="blueNavigation"></asp:Label></span>
                                        </div>
                                        <div class="col-marginDx">
                                            <cc1:CustomImageButton runat="server" ID="btn_next" ImageUrl="../Images/Icons/smistamento_right.png"
                                                CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_right.png"
                                                ImageUrlDisabled="../Images/Icons/smistamento_right_disabled.png" OnMouseOverImage="../Images/Icons/smistamento_right_hover.png"
                                                OnClick="btn_next_Click" OnClientClick="disallowOp('Content2');" />
                                        </div>
                                        <div class="col-marginDx">
                                            <cc1:CustomImageButton runat="server" ID="btn_last" ImageUrl="../Images/Icons/smistamento_right_right.png"
                                                CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_right_right.png"
                                                ImageUrlDisabled="../Images/Icons/smistamento_right_right_disabled.png" OnMouseOverImage="../Images/Icons/smistamento_right_right_hover.png"
                                                OnClick="btn_last_Click" OnClientClick="disallowOp('Content2');" />
                                        </div>
                                        <div class="col-marginDx">
                                            <asp:CheckBox ID="chk_showDoc" runat="server" TextAlign="Right" CssClass="clickable"
                                                AutoPostBack="True" OnCheckedChanged="chk_showDoc_CheckedChanged" onclick="disallowOp('Content2');">
                                            </asp:CheckBox>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </div>
                    <asp:UpdatePanel ID="UpPnlDettagliFirma" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="pnlInfoFirma" runat="server">
                                <div class="row" style="padding-top: 10px">
                                    <div class="containerDetailsInfoDoc">
                                        <asp:TreeView ID="trvDettagliFirma" runat="server" ExpandLevel="10" CssClass="TreeAddressBook"
                                            ShowLines="true" NodeStyle-CssClass="TreeAddressBook_node" SelectedNodeStyle-CssClass="TreeAddressBook_selected"
                                            OnTreeNodeCollapsed="trvDettagliFirma_Collapsed" OnTreeNodeExpanded="trvDettagliFirma_Collapsed"
                                            OnSelectedNodeChanged="trvDettagliFirma_SelectedNodeChanged" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="row" style="padding-top: 10px">
                        <asp:UpdatePanel ID="UpPnlDetailsSignatureProcess" runat="server" ClientIDMode="Static"
                            UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="pnlDetailsSignatureProcess" runat="server">
                                    <div class="containerDetailsInfoDoc">
                                        <asp:TreeView ID="TreeSignatureProcess" runat="server" ExpandLevel="10" ShowLines="true"
                                            NodeStyle-CssClass="TreeSignatureProcess_node" SelectedNodeStyle-CssClass="TreeSignatureProcess_selected"
                                            OnTreeNodeCollapsed="TreeSignatureProcess_Collapsed" OnTreeNodeExpanded="TreeSignatureProcess_Collapsed"
                                            CssClass="TreeSignatureProcess" />
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <div class="contentPopUpDX">
            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpPnlViewer">
                <ContentTemplate>
                    <div id="contentDocumentViewer" align="center">
                        <uc9:ViewDocument runat="server" PageCaller="ESAMINA_UNO_A_UNO" ID="ViewDocument" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="EsaminaLFSelezionaPerFirma" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="EsaminaLFSelezionaPerFirma_Click" />
            <cc1:CustomButton ID="EsaminaLFSelezionaPerRespingimento" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="EsaminaLFSelezionaPerRespingimento_Click" />
            <cc1:CustomButton ID="EsaminaLFDeseleziona" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="EsaminaLFDeseleziona_Click" />
            <cc1:CustomButton ID="EsaminaLFNonDiCompetenza" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="EsaminaLFNonDiCompetenza_Click" />
            <cc1:CustomButton ID="EsaminaLFChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="EsaminaLFChiudi_Click" />
            <asp:Button ID="btnObjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
