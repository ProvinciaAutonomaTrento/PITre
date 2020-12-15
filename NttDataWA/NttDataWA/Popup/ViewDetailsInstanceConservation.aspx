<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDetailsInstanceConservation.aspx.cs" Inherits="NttDataWA.Popup.ViewDetailsInstanceConservation" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>

<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />

         <style type="text/css">
        .tabPopup td
        {
            padding: 5px;
            text-align: center;
            color: #666666;
            font-size: 1em;
        }

     </style>

</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">
 <uc:ajaxpopup Id="ConservationAreaValidation" runat="server" Url="../Popup/ConservationAreaValidation.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="1000" Height="700" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupConservationAreaValidation');}" />
  <uc:ajaxpopup Id="ReportFormatiConservazione" runat="server" Url="~/Popup/ReportFormatiIstanzaConservazione.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="650" Height="500" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupReportFormatiConservazione');}" /> 
    <div id="contentViewDetails">
        <div id="contentDx">
        <asp:UpdatePanel ID="UpPanelField" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
            <fieldset>
                <div class="col5">
                   <div class="colAddSx1">
                            <asp:Label id="lblEnterDescription" runat="server"></asp:Label>
                    </div>
                    <div class="colAddDx1">
                            <cc1:CustomTextArea runat="server" ID="TxtEnterDescription" CssClassReadOnly="txt_addressBookLeft_disabled" CssClass="txt_addressFilter defaultAction" width="50%"></cc1:CustomTextArea>
                    </div>
                </div>
                <div class="col5">
                    <div class="colAddSx1">
                            <asp:Label id="lblEnterNote" runat="server"></asp:Label>
                    </div>
                    <div class="colAddDx1">
                         <cc1:CustomTextArea runat="server" ID="TxtEnterNote" CssClassReadOnly="txt_addressBookLeft_disabled" CssClass="txt_addressFilter defaultAction" width="50%"></cc1:CustomTextArea>
                    </div>
                </div>
                <asp:UpdatePanel ID="UpPanelCbConsolidateDocuments" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                    <ContentTemplate>
                <div class="col5">
                    <div class="colAddSx1"
                        <asp:Label id="lblTypeConservation" runat="server"></asp:Label>
                    </div>
                    <div style=" float:left; padding-right:10px">
                         <asp:DropDownList runat="server" ID="ddl_typeConservation" CssClass="chzn-select-deselect" width="120%"
                         OnSelectedIndexChanged="ddl_typeConservation_OnChanged" AutoPostBack="true" onchange="disallowOp('ContentPlaceHolderContent');"></asp:DropDownList>
                    </div>
                    <div class="colAddDx1">
                         <asp:CheckBox ID="cbConsolidatesDocuments" CssClass="clickableLeftN" runat="server" />
                    </div>
                </div>
                </ContentTemplate>
                </asp:UpdatePanel>
                <div class="col5" style=" padding-bottom:10px">
                    <div class="colAddSx1">
                            <asp:Label id="lblValidInstance" runat="server"></asp:Label>
                    </div>
                    <div class="colAddDx1">
                         <asp:DropDownList runat="server" ID="ddl_validInstance" CssClass="chzn-select-deselect" width="50%" AutoPostBack="true"
                          OnSelectedIndexChanged="ddl_validInstance_OnChanged" onchange="disallowOp('ContentPlaceHolderContent');"></asp:DropDownList>
                    </div>
                </div>
                <asp:UpdatePanel ID="UpPanelLabelInfoNumberSizeDocument" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="col5" style=" padding-bottom:10px">
                            <div style=" float:left; padding-right:20px">
                                    <asp:Label id="lblTotalSize" runat="server"></asp:Label>
                                    <asp:Label id="txtTotalSize" runat="server"></asp:Label>
                            </div>
                            <div class="colAddDx1">
                                 <asp:Label id="lblNumberDocument" runat="server"></asp:Label>
                                 <asp:Label id="txtNumberDocument" runat="server"></asp:Label>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        </div>
        <div id="contentGridDoc" class="col5">
              <asp:UpdatePanel ID="UpPanelGridItemsConservation" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                    <ContentTemplate>
                            <asp:GridView ID="GrdItemsConservation" runat="server" Width="99%" AutoGenerateColumns="false"
                                AllowPaging="True" PageSize="5" CssClass="tabPopup tbl" BorderWidth="0"
                                OnPageIndexChanging="GrdItemsConservation_PageIndexChanging" OnRowDataBound="GrdItemsConservation_RowDataBound"
                                ShowHeaderWhenEmpty="true" OnRowCommand="GrdItemsConservation_RowCommand">
                                <RowStyle CssClass="NormalRow"/>
                                <AlternatingRowStyle CssClass="AltRow" />
                                <PagerStyle CssClass="recordNavigator2" />
                                <Headerstyle Wrap="false"/>
                                <Columns>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_docNumber" runat="server" Text='<%# Bind("DocNumber") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="System Id" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_systemId" runat="server" Text='<%# Bind("SystemID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ViewInstanceConservationTypeDoc%>'>
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_TypeDoc" runat="server" Text='<%#this.GetLabelTypeDoc((NttDataWA.DocsPaWR.ItemsConservazione) Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ViewInstanceConservationObject%>' >
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_desc_oggetto" runat="server" Text='<%#this.GetLabelObject((NttDataWA.DocsPaWR.ItemsConservazione) Container.DataItem) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>   
                                    <asp:BoundField DataField="CodFasc" HeaderText='<%$ localizeByText:ViewInstanceConservationCodeProj%>' />
                                    <asp:BoundField DataField="Data_Ins" HeaderText='<%$ localizeByText:ViewInstanceConservationInsertionDate%>' />
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ViewInstanceConservationIdSignatureDate%>'>
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_data_prot_or_crea" runat="server" Text='<%#this.GetLabeIdSignatureDate((NttDataWA.DocsPaWR.ItemsConservazione) Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="SizeItem" HeaderText='<%$ localizeByText:ViewInstanceConservationKB%>' />
                                    <asp:BoundField DataField="tipoFile" HeaderText='<%$ localizeByText:ViewInstanceConservationTypeFile%>' />
                                    <asp:BoundField DataField="numAllegati" HeaderText='<%$ localizeByText:ViewInstanceConservationNumberAttachments%>' />
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaDetails%>' ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                                <cc1:CustomImageButton ID="ViewDetailsConservationDetailsItem" CommandName="ViewDetailsItem" runat="server" AllignImage="Center"
                                              ToolTip='<%$ localizeByText:ViewDetailsInstanceDetailsToolTip%>' CssClass="clickable" OnClientClick="disallowOp('ContentPlaceHolderContent');" />       
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaRemove%>' ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                                <cc1:CustomImageButton ID="ViewDetailsConservationRemoveItem" CommandName="RemoveItem" runat="server" OnClientClick="disallowOp('ContentPlaceHolderContent');"
                                                ImageUrl="../Images/Icons/delete2.png" OnMouseOutImage="../Images/Icons/delete2.png" ToolTip='<%$ localizeByText:ViewDetailsInstanceRemoveToolTip%>'
                                                OnMouseOverImage="../Images/Icons/delete2_hover.png" CssClass="clickable" ImageUrlDisabled="../Images/Icons/delete2_disabled.png" />       
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                    </ContentTemplate>
                </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="ViewInstanceConservationCheckFormat" runat="server" CssClass="btnEnable clickableright" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationCheckFormat_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="ViewInstanceConservationViewReport" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationViewReport_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="ViewInstanceConservationConvertAndSendForConservation" runat="server" CssClass="btnEnable clickableright" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationConvertAndSendForConservation_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="ViewInstanceConservationSendForConservation" runat="server" CssClass="btnEnable clickableright" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationSendForConservation_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="ViewInstanceConservationEnableInstanceReject" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationEnableInstanceReject_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="ViewInstanceConservationSelectedAsDefault" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationSelectedAsDefault_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');"/>
             <cc1:CustomButton ID="ViewInstanceConservationRemoveAll" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationRemoveAll_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');"/>
            <cc1:CustomButton ID="ViewInstanceConservationRemoveNonCompliant" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationRemoveNonCompliant_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="ViewInstanceConservationClose" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ViewInstanceConservationClose_Click" />
            <script type="text/javascript">
                $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
                $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
