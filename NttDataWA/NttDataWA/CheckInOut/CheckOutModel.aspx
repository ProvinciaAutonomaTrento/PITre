<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOutModel.aspx.cs" Inherits="NttDataWA.CheckInOut.CheckOutModel"  MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc10" TagName="CheckInOutController" Src="CheckInOutController.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutPanel" Src="CheckInOutPanel.ascx" %>
<%@ Register Src="../ActivexWrappers/ClientModelProcessor.ascx" TagName="ClientModelProcessor" TagPrefix="uc4" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .container {text-align: left; line-height: 2em; width: 400px; height: 130px; position: absolute; left: 40%; top: 70%; margin: -90px 0 0 -135px;}
    .container img {float: left; display: block; margin: 15px auto; margin: 0 10px 70px 0;}
</style>
<script type="text/javascript">
    var fsoApp;
    var finalfilePath = '';
    var idDocument = '<%= GetDocumentId()%>';
    var documentNumber = '<%= GetDocumentNum()%>';

    function confirmAction() {
        var retValue = false;

        if ("<%=HasModelProcessorSelected()%>" == "True") {
            var filePath = ShowSaveDialogBox("", "<%=GetModelProcessorSupportedExtensions()%>", "Blocca documento");

            if (filePath != "") {
                if (ClientModelProcessor_ProcessModel(filePath, idDocument, "<%=this.ModelloDocumentoCorrente%>")) {
                    CheckOutDocument(filePath, "", idDocument, documentNumber, false, true, false, true, false)
                    retValue = true;
                }
            }
        }
        else
            alert("Nessun word processor impostato");

        return retValue;
    }
</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >

<asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
        <uc4:ClientModelProcessor ID="clientModelProcessor" runat="server" />
        <uc10:CheckInOutController id="checkInOutController" runat="server"></uc10:CheckInOutController>
            <div class="container">
                <asp:Literal ID="litInformationMessage" runat="server"></asp:Literal>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
