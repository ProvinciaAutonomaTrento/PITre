<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentViewerSummeries.aspx.cs" Inherits="NttDataWA.Popup.DocumentViewerSummeries" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent"  ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
 <asp:UpdatePanel ID="UpPnlDocumentData" runat="server" UpdateMode="Conditional" ClientIDMode="Static" Visible="false">
 <ContentTemplate>
    <div id="contentDocumentViewer" align="center">
       <iframe width="100%" height="700px"  frameborder="0" marginheight="0" marginwidth="0" id="frame" runat="server" clientidmode="Static" style="z-index: 0;"></iframe>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="DocumentViewerBtnClose" OnClick="DocumentViewerBtnClose_Click"  runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
</asp:Content>