<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentConsolidationSummary.aspx.cs" Inherits="DocsPAWA.MassiveOperation.DocumentConsolidationSummary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <base target="_self" />
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script type="text/javascript">
        function closeMask(retValue) {
            window.returnValue = retValue;
            window.close();
        }
        function openReport() {
            var report = window.open('../Import/ReportGenerator/ExportReport.aspx', 'report', null, null);
            report.focus();
        }
    </script>
</head>
<body style="height: 100%">
    <form id="frmDocumentConsolidationSummary" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Esito consolidamento documenti" />
        <table class="contenitore" width="100%" align="center" style="height: 100%;position:absolute; bottom:0">
            <tr style="height: 90%">
                <td class="info_grigio" align="center" valign="top">
                    <asp:Panel ID="pnlContainer" runat="server" ScrollBars="Auto" Height="400px">
                        <asp:DataGrid id="grdDocumentConsolidationSummary" runat="server" SkinID="datagrid" Width="95%" BorderWidth="1px" 
                            BorderColor="Gray" CellPadding="1"
                                AutoGenerateColumns="False" BorderStyle="Inset">
                                <HeaderStyle CssClass="bg_grigioS" />
                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                <ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>	
                            <Columns>
                                <asp:BoundColumn DataField="ObjId" HeaderText="ID Doc." HeaderStyle-Width="20%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Result" HeaderText="Esito" HeaderStyle-Width="5%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Details" HeaderText="Dettagli" HeaderStyle-Width="75%"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </asp:Panel>
                </td>
            </tr>
            <tr style="height: 10%">
                <td class="info_grigio" height="20" align="center">
                    <asp:Button ID="btnClose" runat="server" Text="Annulla" CssClass="testo_grigio" OnClientClick="closeMask(false)" />
                    <asp:Button ID="btnExportPdf" runat="server" CssClass="testo_grigio" Text="Esporta in PDF" OnClick="btnExportPdf_Click"/>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>