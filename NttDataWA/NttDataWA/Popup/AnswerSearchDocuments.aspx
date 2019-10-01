<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    EnableViewState="true" CodeBehind="AnswerSearchDocuments.aspx.cs" Inherits="NttDataWA.Popup.AnswerSearchDocuments" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        p
        {
            text-align: left;
        }
        .red
        {
            color: #f00;
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
        .row1
        {
            clear: both;
            min-height: 15px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .chzn-container-single
        {
            margin-top: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AvviseProtocol" runat="server" Url="AvviseAnswerProtocol.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <div class="container">
        <div id="rowMessage" runat="server" />
        <asp:UpdatePanel ID="upPnlFilters" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <fieldset class="basic">
                    <div class="row nowrap">
                        <p>
                            <div class="col">
                                <asp:Literal runat="server" ID="AnswerSearchDocumentsLitType"></asp:Literal>
                            </div>
                            <div class="col">
                                <asp:RadioButtonList ID="rbl_TipoDoc" OnSelectedIndexChanged="rbl_TipoDoc_SelectedIndexChanged"
                                    runat="server" RepeatLayout="UnorderedList" AutoPostBack="true">
                                    <asp:ListItem Value="Arrivo" runat="server" id="opArr"></asp:ListItem>
                                    <asp:ListItem Value="Partenza" runat="server" id="opPart"></asp:ListItem>
                                    <asp:ListItem Value="Interno" runat="server" id="opInt"></asp:ListItem>
                                    <asp:ListItem Value="Non Protocollato" id="nnprot" runat="server"></asp:ListItem>
                                    <asp:ListItem Value="Predisposti" id="pred" runat="server"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                            <p>
                            </p>
                        </p>
                    </div>
                </fieldset>
                <fieldset class="basic">
                    <div class="row">
                        <asp:CheckBox ID="chk_ADL" runat="server" AutoPostBack="True"></asp:CheckBox>
                    </div>
                    <div class="row" id="pnl_catene_extra_aoo" runat="server" visible="false">
                        <asp:Literal ID="litRegistry" runat="server"></asp:Literal>
                        <asp:DropDownList ID="ddl_reg" runat="server" AutoPostBack="True" />
                    </div>
                    <asp:PlaceHolder ID="plc_proto" runat="server">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="label_fixed">
                                        <asp:Literal ID="litNumProtocol" runat="server"></asp:Literal></span>
                                </p>
                            </div>
                            <div class="col">
                                <asp:DropDownList ID="ddl_numProto" runat="server" OnSelectedIndexChanged="ddl_numProto_SelectedIndexChanged"
                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="130">
                                </asp:DropDownList>
                            </div>
                            <div class="col">
                                <p>
                                    <asp:Literal ID="lblInitNumProto" runat="server" Text="Da " Visible="false" />
                                    <cc1:CustomTextArea ID="txtInitNumProto" runat="server" Columns="10" MaxLength="10"
                                        ClientIDMode="Static" CssClass="txt_date onlynumbers" CssClassReadOnly="txt_date_disabled" />
                                </p>
                            </div>
                            <div id="pnlEndNumProto" runat="server" class="col" visible="false">
                                <p>
                                    a
                                    <cc1:CustomTextArea ID="txtEndNumProto" runat="server" Columns="10" MaxLength="10"
                                        ClientIDMode="Static" CssClass="txt_date onlynumbers" CssClassReadOnly="txt_date_disabled" />
                                    anno
                                    <cc1:CustomTextArea ID="txt_annoProto" runat="server" Columns="4" MaxLength="4" ClientIDMode="Static"
                                        CssClass="txt_year onlynumbers" CssClassReadOnly="txt_year_disabled" />
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="label_fixed">
                                        <asp:Literal ID="litDateProtocol" runat="server" /></span>
                                </p>
                            </div>
                            <div class="col">
                                <asp:DropDownList ID="ddl_dtaProto" runat="server" OnSelectedIndexChanged="ddl_dtaProto_SelectedIndexChanged"
                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="130">
                                </asp:DropDownList>
                            </div>
                            <div class="col">
                                <asp:Literal ID="lblInitDtaProto" runat="server" Text="Da " Visible="false" />
                                <cc1:CustomTextArea ID="txtInitDtaProto" runat="server" MaxLength="10" ClientIDMode="Static"
                                    CssClass="txt_date datepicker" CssClassReadOnly="txt_date_disabled" />
                            </div>
                            <div id="pnlEndDataProtocollo" runat="server" class="col" visible="false">
                                -
                                <cc1:CustomTextArea ID="txtEndDataProtocollo" runat="server" Columns="10" MaxLength="10"
                                    ClientIDMode="Static" CssClass="txt_date datepicker" CssClassReadOnly="txt_date_disabled" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plc_doc" runat="server">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="label_fixed">
                                        <asp:Literal ID="litIdDocument" runat="server" /></span>
                                </p>
                            </div>
                            <div class="col">
                                <asp:DropDownList ID="ddl_numDoc" runat="server" OnSelectedIndexChanged="ddl_numDoc_SelectedIndexChanged"
                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="130">
                                </asp:DropDownList>
                            </div>
                            <div class="col">
                                <p>
                                    <asp:Literal ID="lblInitNumDoc" runat="server" Text="Da " Visible="false" />
                                    <cc1:CustomTextArea ID="txtInitDoc" runat="server" Columns="10" MaxLength="10" ClientIDMode="Static"
                                        CssClass="txt_date onlynumbers" CssClassReadOnly="txt_date_disabled" />
                                </p>
                            </div>
                            <div id="pnlEndNumDoc" runat="server" class="col" visible="false">
                                <p>
                                    -
                                    <cc1:CustomTextArea ID="txtEndNumDoc" runat="server" Columns="10" MaxLength="10"
                                        ClientIDMode="Static" CssClass="txt_date onlynumbers" CssClassReadOnly="txt_date_disabled" />
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="label_fixed">
                                        <asp:Literal ID="litDateDocument" runat="server" /></span>
                                </p>
                            </div>
                            <div class="col">
                                <asp:DropDownList ID="ddl_dtaDoc" runat="server" OnSelectedIndexChanged="ddl_dtaDoc_SelectedIndexChanged"
                                    AutoPostBack="True" CssClass="chzn-select-deselect" Width="130">
                                </asp:DropDownList>
                            </div>
                            <div class="col">
                                <asp:Literal ID="lblInitDtaDoc" runat="server" Text="Da " Visible="false" />
                                <cc1:CustomTextArea ID="txtInitDtaDoc" runat="server" ClientIDMode="Static" CssClass="txt_date datepicker"
                                    CssClassReadOnly="txt_date_disabled" />
                            </div>
                            <div id="pnlEndDataDoc" runat="server" class="col" visible="false">
                                -
                                <cc1:CustomTextArea ID="txtEndDataDoc" runat="server" Columns="10" MaxLength="10"
                                    ClientIDMode="Static" CssClass="txt_date datepicker" CssClassReadOnly="txt_date_disabled" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <!--oggetto-->
                    <div class="row">
                        <div class="col">
                            <p>
                                <asp:Label runat="server" ID="LblAddDocOgetto"></asp:Label>
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
                                <asp:Panel ID="PnlCodeObject" runat="server">
                                    <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                        CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true" onchange="disallowOp('Content2');"
                                         OnTextChanged="TxtCodeObject_Click">
                                    </cc1:CustomTextArea>
                                </asp:Panel>
                                <asp:Panel ID="PnlCodeObject2" runat="server">
                                    <asp:Panel ID="PnlCodeObject3" runat="server">
                                        <cc1:CustomTextArea ID="TxtObject" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                            CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static" Height="40">
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
                    <%-- ************** TIPOLOGIA ******************** --%>
                    <div class="row" style="padding-top: 10px">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row1">
                                    <div class="col">
                                        <%--<span class="weight">--%>
                                        <asp:Literal ID="SearchDocumentLitTypology" runat="server" /><%--</span>--%>
                                    </div>
                                </div>
                                <asp:UpdatePanel ID="UpPnlDocType" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="col-full">
                                                    <div class="styled-select_full">
                                                        <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" OnSelectedIndexChanged="DocumentDdlTypeDocument_OnSelectedIndexChanged"
                                                            AutoPostBack="True" CssClass="chzn-select-deselect" Width="60%" onchange="disallowOp('Content2');">
                                                            <asp:ListItem Text=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <asp:DropDownList ID="ddlStateCondition" runat="server" Width="100%">
                                                                <asp:ListItem id="opt_StateConditionEquals" Value="Equals" />
                                                                <asp:ListItem id="opt_StateConditionUnequals" Value="Unequals" />
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <div class="styled-select_full">
                                                                <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" Width="100%" />
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
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </fieldset>
                <asp:PlaceHolder ID="plc_countRecord" runat="server" Visible="false">
                    <div class="row">
                        <p>
                            <asp:Literal ID="lbl_countRecord" runat="server" /></p>
                    </div>
                </asp:PlaceHolder>
                <asp:UpdatePanel ID="upPnlGridList" runat="server" class="row" UpdateMode="Conditional"
                    ClientIDMode="Static">
                    <ContentTemplate>
                        <asp:GridView ID="grdList" runat="server" Width="98%" AutoGenerateColumns="False"
                            AllowPaging="True" CssClass="tbl">
                            <RowStyle CssClass="NormalRow" />
                            <AlternatingRowStyle CssClass="AltRow" />
                            <Columns>
                                <asp:TemplateField ItemStyle-CssClass="grdList_date">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="grdList_date">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container, "DataItem.registro")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="grdList_description">
                                    <ItemTemplate>
                                        <%# NttDataWA.Utils.utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="grdList_description">
                                    <ItemTemplate>
                                        <%# NttDataWA.Utils.utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.mittDest"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="grdList_date">
                                    <ItemTemplate>
                                        <cc1:CustomImageButton runat="server" ID="imgRecipients" AlternateText='<%# GetLabel("AnswerRecipients") %>'
                                            ImageUrl="../Images/Icons/recipient_details.png" ImageUrlDisabled="../Images/Icons/recipient_details_disabled.png"
                                            OnMouseOutImage="../Images/Icons/recipient_details.png" OnMouseOverImage="../Images/Icons/recipient_details_hover.png"
                                            CssClass="clickable" OnClick="DisplayRecipients" OnClientClick="disallowOp('Content2')" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="grdList_description">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container, "DataItem.mittDest")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="grdList_date">
                                    <ItemTemplate>
                                        <asp:RadioButton ID="OptCorr" runat="server" Text="" OnCheckedChanged="checkOPTDoc"
                                            onclick="__doPostBack('upPnlGridIndexes', '');"></asp:RadioButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Chiave" Visible="False">
                                    <ItemTemplate>
                                        <asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:PlaceHolder ID="plcNavigator" runat="server" />
                        <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:PlaceHolder ID="pnl_corr" runat="server" Visible="false">
                    <div class="row">
                        <div class="col">
                            <span class="weight">
                                <asp:Literal ID="litSelectRecipient" runat="server" /></span>
                        </div>
                        <div class="col">
                            <asp:ImageButton ID="Imagebutton1" OnClick="resetSelection" runat="server" Visible="False"
                                AlternateText="Elimina tutte le selezioni" ImageUrl="../Images/Common/icon-back.png">
                            </asp:ImageButton>
                        </div>
                        <div class="col">
                            <asp:ImageButton ID="ImageButton3" runat="server" Visible="False" AlternateText="Chiudi  il pannello  dei corrispondenti"
                                ImageUrl="../Images/Common/icon_delete.png" ImageAlign="Right"></asp:ImageButton>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="upPnlGridCorr" runat="server" class="row" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:GridView ID="grdCorr" runat="server" Width="98%" AutoGenerateColumns="False"
                                AllowPaging="True" CssClass="tbl" OnPageIndexChanging="grdCorr_PageIndexChanged">
                                <RowStyle CssClass="NormalRow" />
                                <AlternatingRowStyle CssClass="AltRow" />
                                <Columns>
                                    <asp:BoundField DataField="SYSTEM_ID" HeaderText="ID" HeaderStyle-CssClass="hidden"
                                        ItemStyle-CssClass="grdList_date hidden" />
                                    <asp:BoundField DataField="DESC_CORR" HeaderText="Descrizione" ItemStyle-CssClass="grdList_description"
                                        HtmlEncode="false" />
                                    <asp:BoundField DataField="TIPO_CORR" HeaderText="Tipo" HeaderStyle-CssClass="hidden"
                                        ItemStyle-CssClass="grdList_date hidden" />
                                    <asp:TemplateField ItemStyle-CssClass="grdList_date" ItemStyle-HorizontalAlign="center">
                                        <ItemTemplate>
                                            <asp:RadioButton ID="OptCorr" runat="server" Text="" OnCheckedChanged="checkOPT"
                                                onclick="__doPostBack('upPnlGridCorr', '');"></asp:RadioButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:UpdatePanel ID="upPnlIndexesCorr" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:HiddenField ID="grid_corrindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnOk_Click" OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="BtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnSearch_Click" OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
