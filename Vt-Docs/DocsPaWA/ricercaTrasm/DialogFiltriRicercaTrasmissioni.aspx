<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogFiltriRicercaTrasmissioni.aspx.cs" Inherits="DocsPAWA.ricercaTrasm.DialogFiltriRicercaTrasmissioni" %>
<%@ Register Src="FiltriRicercaTrasmissioni.ascx" TagName="FiltriRicercaTrasmissioni" TagPrefix="uc1" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<script type="text/javascript" language="javascript" src="../LIBRERIE/rubrica.js"></script>
	<script type="text/javascript" language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	<base target="_self">    
	<script language="javascript" type="text/javascript">
	    
	    function CloseWindow(returnValue)
	    {
	        window.returnValue = returnValue;
	        window.close();
	    }
	    
	</script>
</head>
<body topMargin="5">
    <form id="frmDialogFiltriRicercaTrasmissioni" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca Documenti" />
        <uc1:FiltriRicercaTrasmissioni id="filtriRicercaTrasmissioni" runat="server">
        </uc1:FiltriRicercaTrasmissioni>
        <table class="testo_grigio" id="tblButtonsContainer" height="35" cellSpacing="0" width="150"
	        align="center" border="0" runat="server">
	        <tr>
		        <td class="titolo_scheda" align="center">
		        <asp:button id="btnOK" runat="server" CssClass="pulsante69" Text="OK" OnClick="btnOK_Click"></asp:button></td>
		        <td class="titolo_scheda" align="center">
		        <asp:button id="btnCancel" runat="server" CssClass="pulsante69" Text="Chiudi" OnClick="btnCancel_Click"></asp:button></td>
	        </tr>
        </table>
    </form>
</body>
</html>
