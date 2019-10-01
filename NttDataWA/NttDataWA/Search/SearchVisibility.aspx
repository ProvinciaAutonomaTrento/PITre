<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchVisibility.aspx.cs"
    MasterPageFile="../MasterPages/Base.Master" Inherits="NttDataWA.Search.SearchVisibility" %>

<%@ Register Src="~/UserControls/SearchProjectsTabs.ascx" TagPrefix="uc2" TagName="SearchProjectsTabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        .row
        {
            min-height: 25px;
        }
        .tbl_rounded_custom td
        {
            vertical-align: top;
            cursor: pointer;
        }
        .tbl_rounded_custom tr.nopointer td
        {
            cursor: default;
        }
        .tbl_rounded_custom th
        {
            white-space: nowrap;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#contentSx input, #contentSx textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentSx select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        });
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="VisibilityHistory" runat="server" Url="../popup/VisibilityHistory.aspx?tipoObj=D"
        Width="800" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('containerStandard2', ''); }" />
    <uc:ajaxpopup Id="VisibilityRemove" runat="server" Url="../popup/VisibilityRemove.aspx"
        Width="600" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('containerStandard2', ''); }" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitSearchProject" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
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
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <div id="containerDocumentTabOrangeSx">
                                    <ul>
                                        <li class="searchIAmSearch" id="LiSearchProject" runat="server">
                                            <asp:HyperLink ID="LinkSearchVisibility" runat="server" NavigateUrl="~/Search/SearchVisibility.aspx"
                                                CssClass="clickable"></asp:HyperLink></li>
                                    </ul>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
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
            <div id="containerStandard2" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <!-- filters -->
                                <fieldset class="basic">
                                    <!-- DDL Prot-NonProt-Tipologia -->
                                    <div class="row">
                                        <div class="col-full">
                                            <asp:DropDownList ID="SearchDocumentDdlIn" runat="server" CssClass="chzn-select-deselect"
                                                OnSelectedIndexChanged="SearchDocumentDdlIn_SelectedIndexChanged" AutoPostBack="true"
                                                Width="100%">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <!-- idDocumento -->
                                    <asp:PlaceHolder ID="PhlSearchIDDocument" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="SearchIDDocument"></asp:Literal> *</span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <cc1:CustomTextArea ID="TxtIDDocument" runat="server" ClientIDMode="Static" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled">
                                                    </cc1:CustomTextArea>
                                                </p>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!-- ricerca Protocolli -->
                                    <asp:PlaceHolder ID="PhlSearchProto" runat="server">
                                        <!-- registro -->
                                        <asp:PlaceHolder ID="plcRegistry" runat="server">
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="litRegistry" runat="server" /></span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList runat="server" ID="DdlRegistries" CssClass="chzn-select-deselect"
                                                        Width="90" AutoPostBack="false">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="litNum" runat="server" /> *</span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <cc1:CustomTextArea ID="TxtNumProto" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="litYear" runat="server" /> *</span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <cc1:CustomTextArea ID="TxtYear" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled"
                                                        MaxLength="4" />
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </asp:PlaceHolder>
                                    <!-- Tipologia -->
                                    <asp:PlaceHolder ID="phlSearchTypologyDoc" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="SearchDocumentLitTypology" runat="server"></asp:Literal></span></p>
                                            </div>
                                        </div>
                                        <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <div class="styled-select_full">
                                                            <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" OnSelectedIndexChanged="DocumentDdlTypeDocument_OnSelectedIndexChanged"
                                                                AutoPostBack="True" CssClass="chzn-select-deselect" Width="97%">
                                                                <asp:ListItem Text=""></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <!-- Tipologia attributi ricerca -->
                                        <br />
                                        <asp:UpdatePanel ID="pnlSearchTypologyAttr" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:PlaceHolder ID="phlSearchTypologyAttr" runat="server" Visible="false">
                                                    <table border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td align="left" valign="top">
                                                                <asp:Panel ID="panel_Contenuto" runat="server">
                                                                </asp:Panel>
                                                                &nbsp;
                                                            </td>
                                                            <td valign="top">
                                                                <asp:Panel ID="pnl_RFAOO" runat="server">
                                                                    <asp:Label ID="lblAooRF" Text="" runat="server"></asp:Label>
                                                                    <asp:DropDownList ID="ddlAooRF" runat="server" AutoPostBack="False" Visible="false"
                                                                        CssClass="chzn-select-deselect">
                                                                    </asp:DropDownList>
                                                                </asp:Panel>
                                                            </td>
                                                            <td class="titolo_scheda" valign="top">
                                                                <asp:Panel ID="pnlAnno" runat="server">
                                                                    <div class="row">
                                                                        <div class="col">
                                                                            <asp:Label ID="lblAnno" Text="Anno *" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="col">
                                                                            <asp:TextBox ID="TxtAnno" runat="server" Width="40" MaxLength="4" CssClass="txt_textdata"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </asp:Panel>
                                                            </td>
                                                            <td class="titolo_scheda" valign="top">
                                                                <asp:Panel ID="pnlNumero" runat="server">
                                                                    <div class="row">
                                                                        <div class="col">
                                                                            <asp:Label ID="lblNumero" Text="Numero *" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="col">
                                                                            <asp:TextBox ID="TxtNumero" runat="server" Width="40" CssClass="txt_textdata"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpdPanelVisibility" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="GridDocuments" runat="server" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                    Width="99%" AutoGenerateColumns="False" AllowPaging="True" OnRowCommand="GridDocuments_RowCommand"
                                    OnSelectedIndexChanging="GridDocuments_SelectedIndexChanging" OnPageIndexChanging="GridDocuments_PageIndexChanging"
                                    DataKeyNames="idCorr" OnPreRender="GridDocuments_PreRender" BorderWidth="0">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Image ID="imgTipo" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRole" runat="server" Text='<%# Bind("Ruolo") %>'></asp:Literal>
                                                <div id="divDetails" runat="server" visible="false">
                                                    <div>
                                                        <strong>
                                                            <asp:Literal ID="lblDetailsUser" runat="server" Visible="false" /></strong></div>
                                                    <asp:Literal ID="LblDetails" runat="server"></asp:Literal>
                                                    <asp:Literal ID="VisibilityLblDetails" runat="server" />
                                                    <asp:Literal ID="LblDetailsInfo" runat="server" />
                                                    <asp:HiddenField ID="hdnTipo" runat="server" Value='<%#Eval("Tipo") %>' />
                                                    <asp:HiddenField ID="hdnCodRubrica" runat="server" Value='<%#Eval("CodiceRubrica") %>' />
                                                    <asp:HiddenField ID="hdnDiSistema" runat="server" Value='<%#Eval("DiSistema") %>' />                                                
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ControlStyle Width="120px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Label ID="LblDiritto" runat="server" Text='<%# Bind("diritti") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="TipoDiritto" />
                                        <asp:BoundField DataField="DataInsSecurity" />
                                        <asp:TemplateField HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Label ID="LblEndDate" runat="server" Text='<%# Bind("DataFine") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="NoteSecurity" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="ImgDelete" runat="server" CommandName="Erase" ImageAlign="Middle"
                                                    ImageUrl="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                    OnMouseOutImage="../Images/Icons/delete.png" CssClass="clickableLeft" Visible="false"/>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <cc1:CustomImageButton ID="ImgHistory" runat="server" CommandName="History" ImageUrl="../Images/Icons/obj_history.png"
                                                    OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                     OnClick="ImgHistory_Click"
                                                     ToolTip='<%#  this.GetTitle()%>'
                                                    AlternateText='<%#  this.GetTitle()%>' CssClass="clickableLeft" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="LblRemoved" runat="server" Text='<%# Bind("Rimosso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Note" Visible="False" />
                                        <asp:BoundField HeaderText="idCorr" Visible="False" DataField="idCorr" />
                                        <asp:TemplateField Visible="False">
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="ImgDetails" runat="server" CommandName="Details" ImageAlign="Middle"
                                                    ImageUrl="~/Images/Icons/docDetails.png" OnMouseOverImage="~/Images/Icons/docDetails_hover.png"
                                                    OnMouseOutImage="~/Images/Icons/docDetails.png" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="GridDocuments" EventName="PageIndexChanging" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="rowIndex" runat="server" ClientIDMode="Static" />
                        <asp:Button ID="btnDetails" runat="server" ClientIDMode="Static" CssClass="hidden"
                            OnClick="GridDocuments_Details" />
                        <div class="row">
                            <asp:Label ID="VisibilityLblDelVis" runat="server" Visible="False"></asp:Label>
                            <br />
                            <asp:Label ID="lblResult" runat="server" ForeColor="Red" Visible="False"></asp:Label>
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
            <cc1:CustomButton ID="SearchProjectSearch" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectSearch_Click" />
            <cc1:CustomButton ID="SearchProjectRemove" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SearchProjectRemove_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
