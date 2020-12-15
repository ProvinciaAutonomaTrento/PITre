<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentViewer.aspx.cs" Inherits="NttDataWA.Popup.DocumentViewer" MasterPageFile="~/MasterPages/Popup.Master"%>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc" TagName="ViewDocument" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
   function UpdateControl() {
            document.getElementById('<%=onClose.ClientID %>').value = 'close';
   }
</script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="contentDocumentViewer" align="center">
        <uc:ViewDocument ID="ViewDocument" runat="server"></uc:ViewDocument>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="DocumentViewerBtnClose" OnClientClick="UpdateControl(); $('iframe').remove();" OnClick="DocumentViewerBtnClose_Click"  runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
    <asp:HiddenField ID="onClose" runat="server" />
</asp:Content>