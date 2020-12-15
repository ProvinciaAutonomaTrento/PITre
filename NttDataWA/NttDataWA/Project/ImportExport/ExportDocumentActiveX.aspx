<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportDocumentActiveX.aspx.cs" Inherits="NttDataWA.Project.ImportExport.ExportDocumentActiveX" %>

<%@ Register Src="../../ActivexWrappers/ProjectToFSWrapper.ascx" TagName="ProjectToFSWrapper" TagPrefix="uc1" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc2" %>
<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register src="ExportProject.ascx" tagname="ExportProject" tagprefix="uc4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../Css/DocsPA_30.css" type="text/css" rel="stylesheet" />

    <script src="<%=Page.ResolveClientUrl("~/Scripts/jquery-1.8.1.min.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/Functions.js") %>" type="text/javascript"></script>
    <script src="Utils.js" type="text/javascript"></script>

    <base target="_self" />
</head>
<body>
    <form id="frmExpFasc" runat="server">
    <div id="Div1" runat="server">
        <table width="100%" class="info">
            <tr class="testo_grigio_scuro">
                <td>
                    
                </td>
            </tr>
            <tr class="testo_grigio_scuro">
                <td>
                    <asp:Label ID="lblFolderPath" runat="server" CssClass="titolo_scheda">Cartella Destinazione: *</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;<asp:TextBox ID="txtFolderPath" runat="server" CssClass="testo_grigio" 
                        Width="322px" />
                    &nbsp;
                    <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." CssClass="pulsante"
                        OnClientClick="PerformSelectFolder();" ToolTip="Seleziona Cartella Destinazione.">
                    </asp:Button>
                    <input id="hdResult" type="hidden" runat="server" />
                </td>
            </tr>
            <tr>
               <td />
            </tr>
            <tr>
                <td>
                    <uc4:ExportProject ID="exportProject" runat="server" />
                </td>
            </tr>
            <tr class="testo_grigio_scuro">
                <td align="center">
                    <asp:Button ID="btnEsporta" runat="server" OnClientClick="esporta();" CssClass="pulsante"
                        Text="Invia" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="Chiudi" CssClass="pulsante" OnClientClick="parent.closeAjaxModal('ExportDocument','');">
                    </asp:Button>
                </td>
            </tr>
        </table>
    </div>
    <uc1:ProjectToFSWrapper ID="prjToFs" runat="server" />
    <uc2:ShellWrapper ID="ShellWrapper" runat="server" />
    <uc3:FsoWrapper ID="FsoWrapper" runat="server" />
    </form>
    <script type="text/javascript">
        showPopupContent();
    </script>
</body>
</html>

