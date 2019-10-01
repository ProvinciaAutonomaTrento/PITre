<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentsRemoved.aspx.cs"
    MasterPageFile="~/MasterPages/Base.Master" Inherits="NttDataWA.Management.DocumentsRemoved" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="../Popup/DocumentViewer.aspx" ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=docInCest"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
    
     <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx">
                </div>
                <div id="containerStandardTopCx">
                    <p>
                        <asp:Label ID="DocumentsRemovesLbl" runat="server"/>
                    </p>
                </div>
                <div id="containerStandardTopDx">
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
         <div id="containerDocumentTab" class="containerStandardTab">
            <div id="containerDocumentTabOrangeInternalSpace">
                <div id="containerDocumentTabOrangeDx">
                    <div class="row">
                        <div class="colMassiveOperationSx">
                            <asp:UpdatePanel runat="server" ID="UpPanelNumberDocumentsRemoved" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:Label ID="lblNumberDocumentsRemoved" runat="server"></asp:Label>
                                </ContentTemplate>
                             </asp:UpdatePanel>
                        </div>
                        
                        <div class="colMassiveOperationDx">
                        <asp:UpdatePanel runat="server" ID="UpPanelBtnTop" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                            <cc1:CustomImageButton ID="BtnRemoveAllDocuments" ImageUrl="../Images/Icons/home_delete.png" ToolTip='<%$ localizeByText:DocumentsRemovedRemoveAllToolTip%>'
                                runat="server" OnMouseOverImage="../Images/Icons/home_delete_hover.png" OnMouseOutImage="../Images/Icons/home_delete.png"
                                ImageUrlDisabled="../Images/Icons/home_delete_disabled.png" CssClass="clickableLeft" OnClick="BtnRemoveAllDocuments_Click" /> 
                            <cc1:CustomImageButton ID="BtnExportDocumentsRemoved" ImageUrl="../Images/Icons/home_export.png" ToolTip='<%$ localizeByText:BtnExportDocumentsRemovedToolTip%>'
                                runat="server" OnMouseOverImage="../Images/Icons/home_export_hover.png" OnMouseOutImage="../Images/Icons/home_export.png"
                                ImageUrlDisabled="../Images/Icons/home_export_disabled.png" CssClass="clickableLeft" OnClientClick="return ajaxModalPopupExportDati();" />
                                </ContentTemplate>
                             </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
            <div id="containerStandardTabDxBorder">
            </div>
        </div>
    </div>
    <div id="containerStandard2" runat="server" clientidmode="Static">
        <div id="content">
            <div id="contentSx">
                    <fieldset class="basic">
                        <asp:UpdatePanel runat="server" ID="UpPnlFilter" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <%-- ID DOCUMENTO--%>
                                <div class="row">
                                    <div class="col">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="DocumentsRemovedLblIdDocument"></asp:Literal></span></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_idDocumento_C" runat="server" Width="140px" AutoPostBack="true"
                                            CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_idDocumento_C_SelectedIndexChanged">
                                            <asp:ListItem id="ddlItemValueSingle" Value="0"></asp:ListItem>
                                            <asp:ListItem id="ddlItemInterval" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="DocumentsRemovedLtlDaIdDoc" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_initIdDoc_C" runat="server" Width="80px" CssClass="txt_input_full onlynumbers"
                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="DocumentsRemovedLtlAIdDoc" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_fineIdDoc_C" runat="server" Width="80px" Visible="false"
                                            CssClass="txt_input_full onlynumbers" CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                    </div>
                                </div>
                                <%-- DATA CREAZIONE --%>
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="DocumentsRemovedLtlDataCreazione"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_dataCreazione_E" runat="server" AutoPostBack="true" Width="140px"
                                            CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
                                            <asp:ListItem id="ddlDateItemValueSingle" Value="0"></asp:ListItem>
                                            <asp:ListItem id="ddlDateItemInterval" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlDaDataCreazione" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_initDataCreazione_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlADataCreazione" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_finedataCreazione_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled" Visible="false"></cc1:CustomTextArea>
                                    </div>
                                </div>
                                  <%-- ************** OGGETTO ******************** --%>
                                <div class="row">
                                    <div class="col-marginSx">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal ID="DocumentsRemovedLitObject" runat="server"></asp:Literal></span></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-marginSx-full">
                                        <div class="full_width">
                                            <cc1:CustomTextArea ID="TxtObject" Width="99%" runat="server" class="txt_input_full"
                                                CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                        </div>
                                    </div>
                                </div>
                                <%--******************* TIPO FILE ACQUISITO ***************--%>
                                <div class="row">
                                    <div class="col">
                                        <span class="weight">
                                            <asp:Literal ID="LtlTipoFileAcq" runat="server"></asp:Literal>
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col">
                                        <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="chzn-select-deselect"
                                            Width="140px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col">
                                        <asp:CheckBox ID="chkFirmato" runat="server" />
                                    </div>
                                    <div class="col">
                                        <asp:CheckBox ID="chkNonFirmato" runat="server" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                </div>
                <div id="contentDx">
                     <asp:UpdatePanel ID="UpPanelGrdDocumentsRemoved" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                                <asp:GridView ID="GrdDocumentsRemoved" runat="server" Width="97%" AutoGenerateColumns="false"
                                    AllowPaging="True" PageSize="20" CssClass="tbl_rounded round_onlyextreme" BorderWidth="0" onrowcommand="GrdDocumentsRemoved_RowCommand"
                                    OnPageIndexChanging="GrdDocumentsRemoved_PageIndexChanging" OnRowDataBound="GrdDocumentsRemoved_RowDataBound"
                                    ShowHeaderWhenEmpty="true">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="idDocument" runat="server" Text='<%# Bind("docNumber") %>' ></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedIDDoc%>'>
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_idDocDate" runat="server" Text='<%# Bind("docNumber") %>' ></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedDate%>'>
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_Date" runat="server" Text='<%# Bind("dataApertura") %>' ></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedObject%>'>
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_object" runat="server" Text='<%# Bind("oggetto") %>' ></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedReason%>'>
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_reason" runat="server" Text='<%# Bind("noteCestino") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedDetails%>' ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                 <cc1:CustomImageButton ID="DocumentsRemovedDetailsDocument" CommandName="ViewDetailsDocument" runat="server" AllignImage="Center"
                                                    ImageUrl="../Images/Icons/ico_previous_details.png" OnMouseOutImage="../Images/Icons/ico_previous_details.png" ToolTip='<%$ localizeByText:DocumentsRemovedDetailsToolTip%>'
                                                    OnMouseOverImage="../Images/Icons/ico_previous_details_hover.png" CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_previous_details_disabled.png" />       
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedView%>' ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                 <cc1:CustomImageButton ID="DocumentsRemovedViewDocument" CommandName="ViewImageDocument" runat="server" AllignImage="Center" ToolTip='<%$ localizeByText:DocumentsRemovedViewToolTip%>'
                                                    CssClass="clickable" /> 
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedRestore%>' ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                 <cc1:CustomImageButton ID="DocumentsRemovedRestore" CommandName="Restore" runat="server" AllignImage="Center"
                                                    ImageUrl="../Images/Icons/home_refresh.png" OnMouseOutImage="../Images/Icons/home_refresh.png" ToolTip='<%$ localizeByText:DocumentsRemovedRestoreToolTip%>'
                                                    OnMouseOverImage="../Images/Icons/home_refresh_hover.png" CssClass="clickable" ImageUrlDisabled="../Images/Icons/home_refresh_disabled.png" />       
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:DocumentsRemovedRemove%>' ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                 <cc1:CustomImageButton ID="DocumentsRemovedRemoveDocument" CommandName="RemoveDocument" runat="server"
                                                    ImageUrl="../Images/Icons/delete2.png" OnMouseOutImage="../Images/Icons/delete2.png" ToolTip='<%$ localizeByText:DocumentsRemovedRemoveToolTip%>'
                                                    OnMouseOverImage="../Images/Icons/delete2_hover.png" CssClass="clickable" ImageUrlDisabled="../Images/Icons/delete2_disabled.png" />       
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="HiddenDocumentRemoved" runat="server" ClientIDMode="Static" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnDocumentsRemovedSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="BtnDocumentsRemovedSearch_Click" />
            <cc1:CustomButton ID="BtnDocumentsRemovedRemoveFilters" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="BtnDocumentsRemovedRemoveFilters_Click" Enabled="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
        <script type="text/javascript">
            $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
            $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
        </script>
</asp:Content>
