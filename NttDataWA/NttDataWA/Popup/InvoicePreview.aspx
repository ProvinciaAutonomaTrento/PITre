<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoicePreview.aspx.cs" Inherits="NttDataWA.Popup.InvoicePreview" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="contentDocumentViewer" align="center">
                <div id="contentDxReport" runat="server" clientidmode="Static">
                    <asp:UpdatePanel ID="UpPnlDocumentData" runat="server" UpdateMode="Conditional" ClientIDMode="Static" Visible="false">
                        <ContentTemplate>
                            <fieldset>
                                <iframe width="100%" height="600px" frameborder="0" marginheight="0" marginwidth="0" id="frame"
                                      runat="server" clientidmode="Static" style="z-index: 999999999;"></iframe>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="InvoicePreviewBtnClose" runat="server" OnClick="InvoicePreviewBtnClose_Click" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
        </ContentTemplate>
    </asp:UpdatePanel>    
</asp:Content>



