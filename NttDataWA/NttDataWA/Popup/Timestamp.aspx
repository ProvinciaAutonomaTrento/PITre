<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Timestamp.aspx.cs" Inherits="NttDataWA.Popup.Timestamp" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>

<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">
    <div id="contentTimestamp">
        <div id="containerStandardTopCx">
            <p>
                <asp:Label ID="TimestampsLbl" runat="server" />
            </p>
        </div>
        <div style=" padding-top:10px; padding-left:10px; width:97%">
         <asp:UpdatePanel ID="UpdatePanelGrdTimestamp" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate> 
                    <asp:GridView ID="GrdTimestamp" runat="server" AutoGenerateColumns="false" AllowPaging="True" PageSize="5"
                        HeaderStyle-CssClass="tableHeaderPopup" CssClass="tabPopup tbl" OnRowDataBound="GrdTimestamp_RowDataBound"
                        Style="cursor: pointer;" OnPageIndexChanging="GrdTimestamp_PageIndexChanging">
                        <SelectedRowStyle CssClass="selectedrow" />
                            <HeaderStyle CssClass="tableHeaderPopup" />
                            <RowStyle CssClass="NormalRow" />
                            <AlternatingRowStyle CssClass="AltRow" />
                            <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:TemplateField HeaderText='<%$ localizeByText:TimestampLblNumberSeries%>'>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNumberSeries" Text='<%# Bind("NUM_SERIE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText='<%$ localizeByText:TimestampLblDateStartValue%>'>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNumberDateStartValue" Text='<%# Bind("DTA_CREAZIONE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText='<%$ localizeByText:TimestampLblDateEndValue%>'>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDateEndValue" Text='<%#this.GetDataFineValiditaMarca((NttDataWA.DocsPaWR.TimestampDoc) Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>               
                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                 <div id="containerDetailsTimestamp" runat="server" style=" padding-top:5px;" Visible="false">
                    <fieldset>
                        <legend>
                            <strong>
                                <asp:Label ID="TimestampLblDetails" runat="server" style=" font-size:1.2em"></asp:Label></strong>
                        </legend>
                        <div id="containerDetails" style=" padding-left:10px;">
                            <div class="row">
                                <span class="weight">
                                    <asp:Label ID="TimestampLblSubject" runat="server"></asp:Label></span>
                                <asp:Label ID="TimestampLblTxtSubject" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <span class="weight">
                                    <asp:Label ID="TimestampLblCountry" runat="server"></asp:Label></span>
                                <asp:Label ID="TimestampLblTxtCountry" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <span class="weight">
                                    <asp:Label ID="TimestampLblCertified" runat="server"></asp:Label></span>
                                <asp:Label ID="TimestampLblTxtCertified" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <span class="weight">
                                    <asp:Label ID="TimestampLblAlgorithm" runat="server"></asp:Label></span>
                                <asp:Label ID="TimestampLblTxtAlgorithm" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <span class="weight">
                                    <asp:Label ID="TimestampLblSeries" runat="server"></asp:Label></span>
                                <asp:Label ID="TimestampLblTxtSeries" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <div style=" float:left; margin-right:10px">
                                    <span class="weight">
                                        <asp:Literal runat="server" ID="TimestampLblTipoValiditaMarca"></asp:Literal></span>
                                </div>
                                <div style=" padding-bottom:0">
                                    <asp:Image ID="imgCheckTimestampTipoValiditaMarca" runat="server" />
                                </div>
                            </div>
                            <asp:Panel ID="PnlExpiredCertificate" runat="server" Visible="true">
                                <div class="row">
                                    <div>
                                        <span class="weight">
                                            <asp:Label ID="TimestampLblExpiredCertificate" runat="server"></asp:Label></span>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>  
                    </fieldset>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel ID="UpdatePanelFrame" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <div id="divFrame" class="row" runat="server" >
                    <iframe frameborder="0" marginheight="0" marginwidth="0" id="frame" height="0px"
                        runat="server" clientidmode="Static"></iframe>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnTimestampAssignsTSR" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" Enabled="false"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnTimestampAssignsTSR_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="BtnTimestampSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnTimestampSave_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="BtnTimestampCreatesTSD" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Enabled="false" OnClick="BtnTimestampCreatesTSD_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
            <cc1:CustomButton ID="BtnTimestampCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnTimestampCancel_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
