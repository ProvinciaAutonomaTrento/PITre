<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneAllineamento.aspx.cs" Inherits="DocsPAWA.FascicolazioneCartacea.GestioneAllineamento" %>

<%@ Register Src="../waiting/WaitingPanel.ascx" TagName="WaitingPanel" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register Src="DocumentiFascicolazione.ascx" TagName="DocumentiFascicolazione"
    TagPrefix="uc1" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <base target="_self" />
    <script type="text/javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
	
	<script type="text/javascript">
	
	    // Visualizzazione maschera di ricerca
	    function ShowWaitingPage()
	    {
	        ShowWaitCursor();
	        
	        ShowWaitPanel("Ricerca in corso...");
	    }
	    	    
		// Visualizzazione clessidra
		function ShowWaitCursor()
		{
			window.document.body.style.cursor="wait";
        }
        
        // Richiesta cancellazione snapshot selezionata
        function ConfirmDeleteSnapshot()
        {
            var cboSnapshots = document.getElementById("cboSnapshots");
            if (cboSnapshots != null && cboSnapshots.value > -1)
                return confirm("Rimuovere la ricerca selezionata?")
            else
                return false;
        }
        
        // Event handler per combo snapshots
        function cboSnapshots_OnChange()
        {
            var cboSnapshots = document.getElementById("cboSnapshots");
            if (cboSnapshots != null && cboSnapshots.value > -1)
            {
                ShowWaitingPage();
            }
        }

	</script>

</head>
<body bottomMargin="1" leftMargin="1" topMargin="4" rightMargin="1">
    <form id="frmGestioneAllineamento" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Allineamento Archivio Cartaceo" />
        <table id="tlbContainer" width="100%">
            <tr valign="top">
                <td style="width: 30%">
                    <table class="contenitore" id="tlbFilters" cellSpacing="0" cellPadding="0" border="0" style="height: 550px; vertical-align: top; width: 100%">
                        <tr>
                            <td valign="top">
                                <br />
                                <table class="info" style="width: 95%;" align="center">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblRicercheSalvate" runat="server" CssClass="titolo_scheda" Text="Ricerche salvate:"></asp:Label>
                                            <br />
                                            <asp:DropDownList ID="cboSnapshots" onchange="cboSnapshots_OnChange();" runat="server" CssClass="testo_grigio" AutoPostBack="True" OnSelectedIndexChanged="cboSnapshots_SelectedIndexChanged" width="100%"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr align="right">
                                        <td>
                                            <asp:imagebutton id="btnRemoveSnapshot" ImageAlign="middle" ImageUrl="../images/proto/cancella.gif" Runat="server" ToolTip="Rimuovi ricerca selezionata" OnClick="btnRemoveSnapshot_Click" width="20px"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 100px">
                                            <asp:Label id="lblTipoDocumento" runat="server" CssClass="titolo_scheda">Tipo documento:</asp:Label>
                                            <br />
                                            <asp:CheckBox ID="chkDocumentiGrigi" runat="server" CssClass="titolo_scheda" Text="Documenti non protocollati" Checked="True" />
                                            <br />
                                            <asp:CheckBox ID="chkProtocolliIngresso" runat="server" CssClass="titolo_scheda" Checked="True" />
                                            <br />
                                            <asp:CheckBox ID="chkProtocolliPartenza" runat="server" CssClass="titolo_scheda" Checked="True" />
                                            <br />
                                            <asp:CheckBox ID="chkProtocolliInterno" runat="server" CssClass="titolo_scheda" Checked="True" />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 100px">
                                            <asp:label id="lblDataCreazione" runat="server" CssClass="titolo_scheda">Data creazione:</asp:label>
                                            <br />
                                            <uc3:Calendario id="txtInitDataCreazione" runat="server" Visible="true" />
                                            <asp:label id="lblEndDataCreazione" runat="server" CssClass="titolo_scheda" Visible="False">a:</asp:label>
                                            <uc3:Calendario id="txtEndDataCreazione" runat="server" Visible="false" />
                                            
                                        </td>
                                    </tr>                              
                                </table>
                            </td>
                        </tr>
                    </table>
                   <table align="center" style="position: static">
                        <tr>
                            <td style="height: 56px">
                                <asp:imagebutton id="btnSearch" ImageAlign="middle" SkinID="ricercaDoc" Runat="server" AlternateText="Ricerca" OnClick="btnSearch_Click" />
                                <asp:imagebutton id="btnSaveSearch" ImageAlign="middle" SkinID="salva" Runat="server" AlternateText="Salva ricerca" OnClick="btnSaveSearch_Click" />
                                <uc2:WaitingPanel id="WaitingPanel1" runat="server" />
                            </td>
                        </tr>
                   </table>
                </td>
                <td style="width: 70%">
                    <uc1:DocumentiFascicolazione id="DocumentiFascicolazione1" runat="server" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
