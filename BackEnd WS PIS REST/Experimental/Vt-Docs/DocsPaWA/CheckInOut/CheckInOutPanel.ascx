<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CheckInOutPanel.ascx.cs" Inherits="DocsPAWA.CheckInOut.CheckInOutPanel" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutController" Src="CheckInOutController.ascx" %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "checkInOutPanelScript"))
  { %>
<script id="checkInOutPanelScript" type="text/javascript">	
	
	function SaveFile(defaultFilePath)
	{
	    var retValue = SaveFileVersion(defaultFilePath, 
	                    "<%=GetCurrentFileExtensionList()%>", 
	                    true, true, false, true);
	                    
        if (retValue)
            alert("Copia terminata");
            
        return retValue;
	}
	
	// CheckOut del documento per cui è già disponibile il content
	function CheckOutDownloaded(defaultFilePath,content)
	{			
		var retValue=CheckOutDocumentDownloaded(defaultFilePath,
											"<%=LastAcquiredDocumentExtension%>",
											"<%=IDDocument%>",
											"<%=DocumentNumber%>",
											content,
											true,
											true,
											true,
											true);
											
		if (retValue)
		{
			var ctl=document.getElementById("<%=ResponseControlID%>");
		
			if (ctl!=null)
				ctl.value=retValue;
		}
		
		return retValue;							
	}

	// CheckOut del documento
	function CheckOut(defaultFilePath)
	{	
		var retValue=CheckOutDocument(defaultFilePath,
										"<%=LastAcquiredDocumentExtension%>",
										"<%=IDDocument%>",
										"<%=DocumentNumber%>",
										true,
										true,
										true,
										true,
										true);
		
		if (retValue)
		{
			var ctl=document.getElementById("<%=ResponseControlID%>");
		
			if (ctl!=null)
				ctl.value=retValue;
		}
					
		return retValue;
    }

    /* MEV INPS - Integrazione M/TEXT
     * Definizione della strategia per il checkin
     */

     // Strategia MTEXT
    function MTextCheckInStrategy() {
        this.mtext = "mtext";
    }

    MTextCheckInStrategy.prototype.GetFilePath = function (filepath) {
        return filepath.substr(8);
    }

    MTextCheckInStrategy.prototype.FileExists = function (filepath) {
        return true;
    }

    MTextCheckInStrategy.prototype.ValidateFileSize = function (filepath) {
        return true;
    }

    MTextCheckInStrategy.prototype.GetContent = function (filepath) {
        return "";
    }

    MTextCheckInStrategy.prototype.GetCheckInPageUrl = function (base) {
        return base + "CheckInPage.aspx";
    }
    MTextCheckInStrategy.prototype.DeleteLocalFile = function (filepath) {
    }


    // Strategia FileSystem
    function FSCheckInStrategy() {
        this.fso = FsoWrapper_CreateFsoObject();
    }

    FSCheckInStrategy.prototype.GetFilePath = function (filepath) {
        return filepath;
    }

    FSCheckInStrategy.prototype.FileExists = function (filepath) {
        return this.fso.FileExists(filepath);
    }

    FSCheckInStrategy.prototype.ValidateFileSize = function (filepath) {
        return SF_ValidateFileSize(filepath);
    }

    FSCheckInStrategy.prototype.GetContent = function (filepath) {
        return AdoStreamWrapper_OpenBinaryData(filepath);
    }

    FSCheckInStrategy.prototype.GetCheckInPageUrl = function (base) {
        return base + "CheckInPage.aspx";
    }
    FSCheckInStrategy.prototype.DeleteLocalFile = function (filepath) {
        this.fso.DeleteFile(filepath, true);
    }

    /*
     * Fine implementazioni interfacce per il checkin
     */

    /*
     * Implementazione interfacce per lo show file
     */

    // Implementazione per file system
    function FSShowFile() {
        
    }

    // Visualizzazione del file
    FSShowFile.prototype.OpenCheckOutDocument = function (showWaitingPage) {
        OpenCheckOutDocument(showWaitingPage);
    }

    // Implementazione per M/Text
    function MTextShowFile() {

    }

    // Visualizzazione del file
    MTextShowFile.prototype.OpenCheckOutDocument = function (showWaitingPage) {
        OpenCheckOutMTextDocument(showWaitingPage)        
    }

    /*
    * Fine implementazione interfacce per lo show file
    */


    /* MEV INPS - Integrazione M/TEXT
    * Gestisci il checkin di un documento M/TEXT
    */

	// CheckIn del documento
	function CheckIn() {

	    var percorso = "<%=CheckOutFilePath%>";
	    var strategy = null;

        // Intercetta modello MTEXT
	    if (percorso.indexOf("mtext://") > -1) {
	        strategy = new MTextCheckInStrategy();
	    } else {
	        strategy = new FSCheckInStrategy();
	    }
	    
		var retValue=CheckInDocument(true,true,("<%=IsSignedFile%>"=="True"),true,strategy);
			
		var ctl=document.getElementById("<%=ResponseControlID%>");
		
		if (ctl!=null)
		    ctl.value = retValue;
		//elimino il file temporaneo dopo il rilascio del doc
		if (strategy != 'mtext' && retValue == true && percorso != '')
		    strategy.DeleteLocalFile(percorso);
	    return retValue;
	}
		
	// UndoCheckOut dle documento
	function UndoCheckOut()
	{
		var retValue=UndoCheckOutDocument(true,true,false);
		
		var ctl=document.getElementById("<%=ResponseControlID%>");

		if (ctl!=null)
			ctl.value=retValue;
			
		return retValue;
	}
	
	// Visualizzazione file in checkout
	function OpenFile() {

	    // Instanziazione della classe da utilizzare per effettuare la visualizzazione
	    var showFileImpl = new FSShowFile();
	    if ('<%=CheckOuFilePath %>'.indexOf("mtext://") > -1)
	        showFileImpl = new MTextShowFile();

	    // Invocazione apertura del documento
        showFileImpl.OpenCheckOutDocument(true);
		
		return false;
	}
	
	// Visualizzazione maschera stato del checkout
	function ShowCheckOutStatus()
	{
	    ShowDialogCheckOutStatus("<%=this.IDDocument%>", "<%=this.DocumentNumber%>");
	    
	    return false;
	}
		
	function SubmitForm(formID)
	{
		document.forms(formID).submit();
	}	
</script>
<% 
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "checkInOutPanelScript", string.Empty); 
  } %>
<table height="43" cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0">
	<tr>
		<td valign = "middle">
			<uc1:CheckInOutController id="checkInOutController" runat="server"></uc1:CheckInOutController>
			<input id="txtResponse" type="hidden" runat="server" NAME="txtResponse">
			<input id="txtExtension" type="hidden" runat="server" NAME="txtExtension">
		</td>
		<td vAlign="middle">
		    <cc2:imagebutton id="btnSave" runat="server" ImageAlign = "Middle"
		        ImageUrl="../images/tabDocImages/checkInOut/copia_locale_attivo.gif"
		        AlternateText="Copia file in locale" DisabledUrl="../images/tabDocImages/checkInOut/copia_locale_noattivo.gif" Height="30px" Width="29px" ToolTip="Copia file in locale" OnClientClick = "return SaveFile('');" />
		</td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle">
		    <cc2:imagebutton id="btnCheckOut" runat="server" ImageAlign = "Middle" ImageUrl="../images/tabDocImages/checkInOut/check_out_attivo.gif"
				AlternateText="Rilascia documento" DisabledUrl="../images/tabDocImages/checkInOut/check_out_noattivo.gif" Height="30px"
				Width="29px" ToolTip="Blocca">
			</cc2:imagebutton>
		</td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle"><cc2:imagebutton id="btnCheckIn" ImageAlign = "Middle" runat="server" ImageUrl="../images/tabDocImages/checkInOut/check_in_attivo.gif"
				AlternateText="Blocca documento" DisabledUrl="../images/tabDocImages/checkInOut/check_in_noattivo.gif" Height="30px"
				Width="29px" ToolTip="Rilascia"></cc2:imagebutton></td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle"><cc2:imagebutton id="btnUndoCheckOut" runat="server" ImageAlign = "Middle" ImageUrl="../images/tabDocImages/checkInOut/undo_check_out_attivo.gif"
				AlternateText="Annulla blocco e rilascia senza salvare" DisabledUrl="../images/tabDocImages/checkInOut/undo_check_out_noattivo.gif"
				Height="30px" Width="29px" ToolTip="Rilascia senza salvare"></cc2:imagebutton></td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle"><cc2:imagebutton id="btnOpenCheckedOutFile" runat="server" ImageAlign = "Middle" ImageUrl="../images/tabDocImages/checkInOut/open_file_attivo.gif"
				AlternateText="Apri documento bloccato" DisabledUrl="../images/tabDocImages/checkInOut/open_file_noattivo.gif"
				Height="30px" Width="29px" ToolTip="Apri"></cc2:imagebutton></td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle"><cc2:imagebutton id="btnShowCheckOutStatus" runat="server" ImageAlign = "Middle" ImageUrl="../images/tabDocImages/checkInOut/checked_out_document.gif"
				DisabledUrl="../images/tabDocImages/checkInOut/checked_out_document.gif" Height="16px" Width="14px"></cc2:imagebutton></td>
	</tr>
</table>
