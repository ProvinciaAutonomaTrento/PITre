<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="AddFilterProject.aspx.cs" Inherits="NttDataWA.Popup.AddFilterProject" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxConfirmPopUpModal.ascx" TagPrefix="uc" TagName="ajaxConfirmPopUpModal" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            float:left;
            width: 99%;
            margin: 0 auto;
        }
        p
        {
            text-align: left;
        }
        .col-right
        {
            float: right;
            font-size: 0.8em;
        }
        ul
        {
            float: left;
            list-style: none;
            margin: 0;
            padding: 0;
            width: 90%;
        }
        li
        {
            display: inline;
            margin: 0;
            padding: 0;
        }
        .label_fixed
        {
            width: 120px;
            float: left;
        }
        .chzn-container-single
        {
            margin-top: 5px;
        }
        .tbl td
        {
            cursor: default;
        }
        .tbl th
        {
            font-weight: bold;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function swapUserPrivateCheckboxs(id) {
            if ($get('<%=cb_nonFirmato.ClientID %>') && $get('<%=cb_firmato.ClientID %>')) {
                if (id == '<%=cb_nonFirmato.ClientID %>' && $get('<%=cb_nonFirmato.ClientID %>').checked) {
                    $get('<%=cb_firmato.ClientID %>').checked = false;
                }
                else if (id == '<%=cb_firmato.ClientID %>' && $get('<%=cb_firmato.ClientID %>').checked) {
                    $get('<%=cb_nonFirmato.ClientID %>').checked = false;
                }
            }
        }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxConfirmPopUpModal ID="ajaxConfirmPopUpModal" runat="server" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <div class="container">
        <div id="rowMessage" runat="server" />
        <asp:UpdatePanel ID="UplnRadioButton" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
            <ContentTemplate>
                <fieldset class="basic">
                    <div class="row nowrap">
                        <div class="col">
                            <p>
                                <asp:Literal runat="server" ID="AddFilerProjectLitType"></asp:Literal>
                            </p>
                        </div>
                        <div class="col">
                            <p>
                                <asp:RadioButtonList ID="rbl_TipoDoc" runat="server" RepeatLayout="UnorderedList"
                                    AutoPostBack="true" OnSelectedIndexChanged="rbl_TipoDoc_SelectedIndexChanged">
                                    <asp:ListItem Value="A" runat="server" id="Arrivo"></asp:ListItem>
                                    <asp:ListItem Value="P" runat="server" id="Partenza"></asp:ListItem>
                                    <asp:ListItem Value="I" runat="server" id="Interno"></asp:ListItem>
                                    <asp:ListItem Value="G" runat="server" id="Grigio"></asp:ListItem>
                                    <asp:ListItem Value="T" runat="server" id="Tutti" Selected="true"></asp:ListItem>
                                </asp:RadioButtonList>
                            </p>
                        </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UplnFiltri" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
            <ContentTemplate>
                <fieldset class="basic">
                    <asp:PlaceHolder ID="plh_documento" runat="server">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="label_fixed">
                                        <asp:Label ID="LblAddDocNumProtocol" runat="server"></asp:Label></span>
                                </p>
                            </div>
                            <div class="col">
                                <asp:DropDownList ID="ddl_numProto" runat="server" OnSelectedIndexChanged="ddl_numProto_SelectedIndexChanged"
                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="130">
                                </asp:DropDownList>
                            </div>
                            <div class="col">
                                <p>
                                    <asp:Label ID="LblAddDocDa" runat="server"></asp:Label>
                                    <cc1:CustomTextArea ID="txtAddDocDa" runat="server" Columns="10" MaxLength="10" CssClass="txt_date onlynumbers"
                                        CssClassReadOnly="txt_date_disabled" />
                                </p>
                            </div>
                            <div class="col">
                                <p>
                                    <asp:Label runat="server" ID="LblAddDocA"></asp:Label>
                                    <cc1:CustomTextArea ID="txtAddDocA" runat="server" Columns="10" MaxLength="10" ClientIDMode="Static"
                                        CssClass="txt_date onlynumbers" CssClassReadOnly="txt_date_disabled" />
                                </p>
                            </div>
                            <div class="col">
                                <p>
                                    <asp:Label runat="server" ID="LblAddDocAnno"></asp:Label>
                                    <cc1:CustomTextArea ID="txtAddDocAnno" runat="server" Columns="4" MaxLength="4" CssClass="txt_year onlynumbers"
                                        CssClassReadOnly="txt_year_disabled" />
                                </p>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="row">
                        <div class="col">
                            <p>
                                <span class="label_fixed">
                                    <asp:Label ID="LblAddDocData" runat="server" /></span>
                            </p>
                        </div>
                        <div class="col">
                            <p>
                                <asp:DropDownList ID="ddl_dtaProto" runat="server" OnSelectedIndexChanged="ddl_dtaProto_SelectedIndexChanged"
                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="130">
                                </asp:DropDownList>
                            </p>
                        </div>
                        <div class="col">
                            <p>
                                <asp:Label ID="LblAddDocDataDa" runat="server" />
                                <cc1:CustomTextArea ID="txtAddDocDataDA" runat="server" MaxLength="10" CssClass="txt_date datepicker"
                                    CssClassReadOnly="txt_date_disabled" />
                            </p>
                        </div>
                        <div class="col">
                            <p>
                                <asp:Label ID="LblAddDocDataA" runat="server" />
                                <cc1:CustomTextArea ID="txtAddDocDataA" runat="server" Columns="10" MaxLength="10"
                                    CssClass="txt_date datepicker" CssClassReadOnly="txt_date_disabled" />
                            </p>
                        </div>
                    </div>
                    <!--oggetto-->
                    <div class="row">
                        <div class="col">
                            <p>
                                <asp:Label runat="server" ID="LblAddDocOgetto" Text="Oggetto"></asp:Label>
                            </p>
                        </div>
                        <div class="col-right">
                            <cc1:CustomImageButton runat="server" ID="DocumentImgObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                                OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                OnClientClick="return ajaxModalPopupObject();" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                            <ContentTemplate>
                                <asp:Panel ID="PnlCodeObject" runat="server" Visible="false">
                                    <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                        CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true" OnTextChanged="TxtCodeObject_Click">
                                    </cc1:CustomTextArea>
                                </asp:Panel>
                                <asp:Panel ID="PnlCodeObject2" runat="server">
                                    <asp:Panel ID="PnlCodeObject3" runat="server">
                                        <cc1:CustomTextArea ID="TxtObject" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                            CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static" Height="99%" Width="99%">
                                        </cc1:CustomTextArea>
                                    </asp:Panel>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="row">
                        <div class="col-right-no-margin">
                            <span class="col-right"><span class="charactersAvailable">
                                <asp:Literal ID="projectLitVisibleObjectChars" runat="server" />: <span id="TxtObject_chars"
                                    runat="server" clientidmode="Static"></span></span></span>
                        </div>
                    </div>
                    <%-- rubrica --%>
                    <asp:UpdatePanel ID="UpPnlRecipient" runat="server" UpdateMode="Conditional" ClientIDMode="static"
                        EnableViewState="true">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="plh_rubrica">
                                <asp:HiddenField runat="server" ID="IdRecipient" />
                                <asp:HiddenField runat="server" ID="RecipientTypeOfCorrespondent" />
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <asp:Label runat="server" ID="l_rubrica"></asp:Label>
                                        </p>
                                    </div>
                                    <div class="col-right">
                                        <cc1:CustomImageButton runat="server" ID="AddFilte4rProjectImgAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                            OnClick="TrasmissionsImgAddressBook_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="colHalf">
                                        <cc1:CustomTextArea ID="TxtRecipientCode" runat="server" CssClass="txt_addressBookLeft"
                                            OnTextChanged="TxtCode_OnTextChanged" CssClassReadOnly="txt_addressBookLeft_disabled"
                                            AutoPostBack="true" AutoCompleteType="Disabled" onblur="disallowOp('Content2');"
                                            ClientIDMode="Static">
                                        </cc1:CustomTextArea>
                                    </div>
                                    <div class="colHalf2">
                                        <div class="colHalf3">
                                            <cc1:CustomTextArea ID="TxtRecipientDescription" runat="server" CssClass="txt_addressBookLeft"
                                                CssClassReadOnly="txt_addressBookLeft_disabled">
                                            </cc1:CustomTextArea>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="row">
                        <asp:UpdatePanel ID="UpPnlDocType" runat="server">
                            <ContentTemplate>
                                <fieldset class="fieldWhite">
                                    <div class="row">
                                        <asp:Literal ID="SearchDocumentLitTypology" runat="server"></asp:Literal>
                                    </div>
                                    <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col-full">
                                                    <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" OnSelectedIndexChanged="DocumentDdlTypeDocument_OnSelectedIndexChanged"
                                                        AutoPostBack="True" CssClass="chzn-select-deselect" Width="97%" onchange="disallowOp('Content2');">
                                                        <asp:ListItem Text=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                <div class="row">
                                                    <div class="col-full">
                                                        <asp:DropDownList ID="ddlStateCondition" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                            Width="100%">
                                                            <asp:ListItem Value="Equals">Uguale a</asp:ListItem>
                                                            <asp:ListItem Value="Unequals">Diverso da</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <div class="styled-select_full">
                                                            <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" CssClass="chzn-select-deselect"
                                                                Width="100%">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <asp:PlaceHolder runat="server" ID="PnlDocumentStateDiagramDate" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="DocumentDateStateDiagram" runat="server"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="PnlTypeDocument" runat="server"></asp:PlaceHolder>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="row">
                        <div class="col">
                            <p>
                                <asp:Literal ID="AddFilterProjectLitFile" runat="server"></asp:Literal>
                            </p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                Width="300">
                                <asp:ListItem Text=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col">
                            <p>
                                <asp:CheckBox ID="cb_firmato" runat="server" onclick="swapUserPrivateCheckboxs(this.id)" />
                            </p>
                        </div>
                        <div class="col">
                            <p>
                                <asp:CheckBox ID="cb_nonFirmato" runat="server" onclick="swapUserPrivateCheckboxs(this.id)" />
                            </p>
                        </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="AddFilterProjectBtnInserisci" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2')"
                OnClick="AddFilterProjectBtnOk_Click" />
            <cc1:CustomButton ID="AddFilterProjectClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterProjectClose_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
