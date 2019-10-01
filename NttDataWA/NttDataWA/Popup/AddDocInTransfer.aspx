<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="AddDocInTransfer.aspx.cs" Inherits="NttDataWA.Popup.AddDocInTransfer" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
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
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="1000" CloseFunction="function (event, ui) {__doPostBack('UpdPnlObject', '');}" />
    <div class="container">
        <div id="rowMessage" runat="server" />
        <asp:UpdatePanel ID="UplnRadioButton" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
            <ContentTemplate>
                <fieldset class="basic">
                    <div class="row nowrap">
                        <div class="col">
                            <asp:Literal runat="server" ID="AnswerSearchDocumentsLitType"></asp:Literal>
                        </div>
                        <div class="col">
                            <asp:RadioButtonList ID="rbl_TipoDoc" runat="server" RepeatLayout="UnorderedList"
                                AutoPostBack="true" OnSelectedIndexChanged="rbl_TipoDoc_SelectedIndexChanged">
                                <asp:ListItem Value="P" runat="server" id="DocProt" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="G" runat="server" id="DocNonProt"></asp:ListItem>
                                <%--  <asp:ListItem Value="PRED" runat="server" id="Pred" ></asp:ListItem>--%>
                                <asp:ListItem Value="ADL" id="ADL" runat="server"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <!--filtri-->
        <asp:UpdatePanel ID="UplnFiltri" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
            <ContentTemplate>
                <fieldset class="basic">
                    <asp:PlaceHolder ID="plh_filtri" runat="server">
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
                                    <asp:Label ID="LblAddDocDa" runat="server" Visible="false"></asp:Label>
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
                                    <asp:Label ID="LblAddDocDataDa" runat="server" Visible="false" />
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
                    </asp:PlaceHolder>
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
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <!-- griglia dei risultati-->
        <asp:UpdatePanel ID="UplnGrid" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder ID="resultsearch" runat="server" Visible="false">
                    <fieldset class="basic">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <asp:Label ID="lbl_countRecord" runat="server"></asp:Label>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-full">
                                <asp:UpdatePanel runat="server" ID="UpPnlGrid" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="AddDocInProjectGrid" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                            HorizontalAlign="Center" CssClass="tbl" Width="100%" OnRowDataBound="AddDocInProjectGrid_OnRowCreated">
                                            <RowStyle CssClass="NormalRow" />
                                            <AlternatingRowStyle CssClass="AltRow" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="1%">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="cb_selectall" runat="server" Enabled='<%# this.GetEnableDisableCheck() %>'
                                                            AutoPostBack="true" OnCheckedChanged="addAll_Click" OnClientClick="disallowOp('Content2')" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="checkDocumento" runat="server" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Documento" HeaderStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="idDoc" runat="server" Text='<%# Bind("IdDocumento") %>' CssClass='<%#this.GetColorCss((string)DataBinder.Eval(Container.DataItem, "TipoDocumento"))%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Data" HeaderStyle-Width="9%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="data" runat="server" Text='<%# Bind("Data") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Registro" ItemStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="registro" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Oggetto" ItemStyle-Width="65%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="oggetto" runat="server" Text='<%# Bind("Oggetto") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo" ItemStyle-Width="5%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="tipo" runat="server" Text='<%# Bind("TipoDocumento") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="idProfile" runat="server" Text='<%# Bind("idProfile") %>'></asp:Label>
                                                        <asp:Label ID="privato" runat="server" Text='<%# Bind("Privato") %>'></asp:Label>
                                                        <asp:Label ID="personale" runat="server" Text='<%# Bind("Personale") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:PlaceHolder ID="plcNavigator" runat="server" />
                                        <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                                <asp:HiddenField ID="HiddenIsPersonal" runat="server" ClientIDMode="Static" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </fieldset>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="btnAddDocInTransfer" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="AddDocInTransfer_Click" Enabled="false" />
            <cc1:CustomButton ID="AddDocInProjectBtnSearch" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddDocInProjectBtnSearch_Click"
                OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="AddDocInProjectClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddDocInProjectClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
