<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisposalReport.aspx.cs"
    Inherits="NttDataWA.Popup.DisposalReport" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/jscript">

        function resizeTableDocDisposal() {
            if ($('.GrdDocResultDisposal').length > 0 && $('.GrdDocResultDisposal')[0].offsetTop) {
                var height = document.documentElement.clientHeight;
                var rowHeight = (height) / ($('.GrdDocResultDisposal')[0].rows.length);
                for (var i = 1; i < $('.GrdDocResultDisposal')[0].rows.length; i++)
                    $('.GrdDocResultDisposal')[0].rows[i].style.height = rowHeight + 'px';

                window.onresize = resizeIframe
            }
        };

        function resizeTableFascDisposal() {
            if ($('.GrdFascResultDisposal').length > 0 && $('.GrdFascResultDisposal')[0].offsetTop) {
                var height = document.documentElement.clientHeight;
                var rowHeight = (height) / ($('.GrdFascResultDisposal')[0].rows.length);
                for (var i = 1; i < $('.GrdFascResultDisposal')[0].rows.length; i++)
                    $('.GrdFascResultDisposal')[0].rows[i].style.height = rowHeight + 'px';

                window.onresize = resizeIframe
            }
        };

    </script>
    <script src="../Scripts/jquery.ui.multidraggable-1.8.8.js" type="text/javascript"></script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="container">
        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpdatePanelTop">
            <ContentTemplate>
                <div class="boxTopReport">
                    <fieldset class="filterAddressbook">
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="litIdScarto" runat="server"></asp:Literal></strong>
                            </div>
                            <div class="col">
                                <NttDL:CustomTextArea ID="txtIdScarto" runat="server" CssClass="txt_input_half" CssClassReadOnly="txt_addressBookLeft_disabled"
                                    Enabled="False">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="litDescrScarto" runat="server"></asp:Literal></strong>
                            </div>
                            <div class="colHalf_arch">
                                <NttDL:CustomTextArea ID="TxtDescrScarto" runat="server" CssClass="txt_input_half"
                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="contentAddressBook">
            <div id="topContentAddressBook">
                <asp:UpdatePanel ID="UpTypeResultDisp" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <ul>
                            <li class="addressTab" id="liReportDocumentiDisposal" runat="server">
                                <asp:LinkButton runat="server" ID="DocTabLinkList" OnClick="AddressBookLinkList_Click"></asp:LinkButton></li>
                            <li class="otherAddressTab" id="liReportFascicoliDisposal" runat="server">
                                <asp:LinkButton runat="server" ID="FascTabLinkList" OnClick="AddressBookLinkOrg_Click"
                                    OnClientClick="disallowOp('Content2')"></asp:LinkButton></li>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="centerContentAddressbook">
                <div class="marginLeft">
                    <asp:UpdatePanel ID="UpPnlGridDocResultDisp" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="lblerror" runat="server"></asp:Label>
                            <asp:Panel ID="PnlGridDocResult" runat="server">
                                <div class="row">
                                    <div class="colHalf">
                                        <strong>
                                            <asp:Literal ID="Literal1" Text="Numero documenti" runat="server"></asp:Literal></strong>
                                    </div>
                                    <div class="colHalf_arch">
                                        <asp:Literal ID="LitNumDoc" runat="server"></asp:Literal></strong>
                                    </div>
                                    <div class="col-right">
                                        <NttDL:CustomImageButton ID="cImgBtnExportDocDisp" ImageUrl="../Images/Icons/export excel-pdf.png"
                                            runat="server" OnMouseOverImage="../Images/Icons/export-excel-pdf_hover.png"
                                            OnMouseOutImage="../Images/Icons/export excel-pdf.png" CssClass="clickableLeft"
                                            OnClick="cImgBtnExportDocDisp_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:GridView ID="GrdDocResultDisposal" runat="server" Width="99%" AutoGenerateColumns="False"
                                        OnRowDataBound="GrdDocResultDisposal_RowDataBound" AllowPaging="True" CssClass="tbl_rounded_custom round_onlyextreme GrdDocResultDisposal"
                                        PageSize="9" BorderWidth="0" OnPageIndexChanging="GrdDocResultDisposal_PageIndexChanging"
                                        DataKeyNames="Profile_ID">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="" HeaderStyle-Width="1%">
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="cb_selectallDoc" runat="server" AutoPostBack="true" OnCheckedChanged="addAllDoc_Click"
                                                        OnClientClick="disallowOp('Content2')" Checked='<%#this.CheckAllDoc %>' />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="checkDocumento" runat="server" OnCheckedChanged="checkDocumento_CheckedChanged" />
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblRegistroDispDoc" Text='Registro' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="registroDispDoc" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblUODispDoc" Text='UO' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="UODispDoc" runat="server" Text='<%# Bind("UO") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblIdDocumentoDispDoc" Text='ID Doc.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="ID_ProfileDispDoc" runat="server" Text='<%# Bind("Profile_ID") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblprotoDispDoc" Text='Proto.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="ProtoDispDoc" runat="server" Text='<%# Bind("Proto") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblannoprotoDispDoc" Text='Anno Proto.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="ProtoDateDispDoc" runat="server" Text='<%# Bind("ProtoDate") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LbldatacreaDispDoc" Text='Data Creazione.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="CreateDateDispDoc" runat="server" Text='<%# Bind("CreateDate") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LbloggettoDispDoc" Text='Oggetto' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="oggettoDispDoc" runat="server" Text='<%# Bind("Oggetto") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LbltipoDispDoc" Text='Tipo' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="TipoDispDoc" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblMITTDestDispDoc" Text='Mitt/Dest' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="CorrDispDoc" runat="server" Text='<%# Bind("MittDest") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LbltipologiaDispDoc" Text='Tipologia' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="tipologiaDispDoc" runat="server" Text='<%# Bind("Tipologia") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblcodefascDispDoc" Text='Codice Fasc' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="ProjectCodeDispDoc" runat="server" Text='<%# Bind("ProjectCode") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpPnlGridFascResultDisp" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="PnlFascReport" Visible="false">
                                <div class="row">
                                    <div class="colHalf">
                                        <strong>
                                            <asp:Literal ID="Literal2" Text="Numero fascicoli" runat="server"></asp:Literal></strong>
                                    </div>
                                    <div class="colHalf_arch">
                                        <asp:Literal ID="LitNumFasc" runat="server"></asp:Literal></strong>
                                    </div>
                                    <div class="col-right">
                                        <NttDL:CustomImageButton ID="cImgBtnExportFascDisp" ImageUrl="../Images/Icons/export excel-pdf.png"
                                            runat="server" OnMouseOverImage="../Images/Icons/export-excel-pdf_hover.png"
                                            OnMouseOutImage="../Images/Icons/export excel-pdf.png" ImageUrlDisabled="../Images/Icons/export excel-pdf.png"
                                            CssClass="clickableLeft" OnClick="cImgBtnExportFascDisp_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:GridView ID="GrdFascResultDisposal" runat="server" Width="99%" AutoGenerateColumns="False"
                                        AllowPaging="True" CssClass="tbl_rounded_custom round_onlyextreme GrdFascResultDisposal"
                                        PageSize="9" DataKeyNames="Project_ID" BorderWidth="0" OnPageIndexChanging="GrdFascResultDisposal_PageIndexChanging"
                                        OnRowDataBound="GrdFascResultDisposal_RowDataBound">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="" HeaderStyle-Width="1%">
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="cb_selectallFasc" runat="server" AutoPostBack="true" OnCheckedChanged="addAllFasc_Click"
                                                        OnClientClick="disallowOp('Content2')" Checked='<%#this.CheckAllFasc %>' />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="checkFascicolo" runat="server" OnCheckedChanged="checkFascicolo_CheckedChanged" />
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblRegistroDispFasc" Text='Registro' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="registroDispFasc" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblUODispFasc" Text='UO' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="tipoDispFasc" runat="server" Text='<%# Bind("UO") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblIdDocumentoDispFasc" Text='ID fasc.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="iD_ProtocolloDispFasc" runat="server" Text='<%# Bind("Project_ID") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblDescDispFasc" Text='Descrizione' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="dataDispFasc" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblstartdateDispFasc" Text='data creazione' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="oggettoDispFasc" runat="server" Text='<%# Bind("startDate") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblclosedateDispFasc" Text='data chiusura' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="idProfileDispFasc" runat="server" Text='<%# Bind("closeDate") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LbltipologiaDispFasc" Text='tipologia' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="TipologiaDispFasc" runat="server" Text='<%# Bind("Tipologia") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnAggiorna" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnAggiorna_Click" OnClientClick="disallowOp('Content2')" />
            <NttDL:CustomButton ID="btnAnnulla" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnAnnulla_Click" OnClientClick="disallowOp('Content2')" />
            <NttDL:CustomButton ID="btnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnClose_Click" OnClientClick="disallowOp('Content2')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
