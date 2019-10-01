<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportDocumentActiveX.aspx.cs" Inherits="NttDataWA.Project.ImportExport.ImportDocumentActiveX" %>
<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc2" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc4" %>
<%@ Register src="Import/massiveImportDocumenti.ascx" tagname="massiveImportDocumenti" tagprefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../../Css/DocsPA_30.css" type="text/css" rel="stylesheet">
	<script type="text/javascript" language="javascript">
	    function chiudi() {
	        parent.closeAjaxModal('ImportDocument', 'up');
	    }
    </script>
    <script type="text/javascript" language="javascript" src="Import/jDataScript.js"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/jquery-1.8.1.min.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/Functions.js") %>" type="text/javascript"></script>
	<base target="_self" ></base>  
</head>
<body MS_POSITIONING="GridLayout">
    <form id="frmImportaDoc" runat="server">
		<input type="hidden" id="txtFirstInvalidControlID" runat="server">
		<input type="hidden" id="hdMetaFileContent" runat="server">
        <table id="principale"  class="info" width="100%">
        <tr class="testo_grigio_scuro">
            <td> </td>
        </tr>
        <tr class="testo_grigio_scuro">
		<TD>
			<asp:label id="lblFolderPath" runat="server" CssClass="titolo_scheda">Cartella Sorgente: *</asp:label>
		</TD>
		</tr>
				<tr>
					<TD>
						&nbsp;<asp:textbox id="txtFolderPath" runat="server" CssClass="testo_grigio" Width="322px"></asp:textbox>&nbsp;
						<asp:button id="btnBrowseForFolder" runat="server" Text="..." CssClass="pulsante" OnClientClick="PerformSelectFolder();" ToolTip="Seleziona Cartella Origine."></asp:button>
					    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
					</TD>
		</tr> 
			<tr style="display:none"> 
				<td>
				<input id="codFasc" type="text" runat="server"> 
				<input id="lastDirSelection" type="text" runat="server" /> 
				</td>
		</tr> 
            </table>
              <table width="100%" height="260px">
              <tr>
              <td valign="top">
                    <!-- custom control -->
                    <uc1:massiveImportDocumenti ID="massiveImportDocumenti" runat="server" />
                </td>
                </tr>
              </table>
              <table class="info" width="100%">
				<tr class="testo_grigio_scuro" >
					<td align="center">
						<asp:button id="btnInvia" runat="server" OnClientClick="invia('import');" CssClass="pulsante" Text = "Invia" />
					    &nbsp;&nbsp;&nbsp;&nbsp;
						<asp:button id="btnCancel" runat="server" Text="Chiudi" CssClass="pulsante" OnClientClick="chiudi();"></asp:button>
					</td>
				</tr> 						
             </table>
    	 <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    	 <uc2:ShellWrapper ID="shellWrapper" runat="server" />
		 <uc4:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
        
    </form>
    <script type="text/javascript">
        showPopupContent();
    </script>
</body>
</html>
