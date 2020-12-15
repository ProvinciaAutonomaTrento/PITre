<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CheckInOutPanel.ascx.cs" Inherits="NttDataWA.CheckInOutApplet.CheckInOutPanel" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc2" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutController" Src="CheckInOutController.ascx" %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "checkInOutPanelScript"))
  { %>
<script id="checkInOutPanelScript" type="text/javascript">	
	
	function SaveFile(defaultFilePath) {
	    return ajaxModalPopupSaveDialog();
	    //var retValue = SaveFileVersion(defaultFilePath, 
	      //              "<%=GetCurrentFileExtensionList()%>", 
	        //            true, true, false, true);
	                    
        //if (retValue)
          //  alert("Copia terminata");
            
        //return retValue;
	}
	
	// CheckOut del documento per cui è già disponibile il content
	function CheckOutDownloaded(defaultFilePath,content) {
	    var fileExt = document.getElementById('fileExt').value;
	    var IDDocument = document.getElementById('documentId').value;
	    var DocumentNumber = document.getElementById('documentNumber').value;
	    var retValue = CheckOutDocumentDownloaded(defaultFilePath,
											fileExt,
											IDDocument,
											DocumentNumber,
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
	function CheckOut(defaultFilePath) {
	    var fileExt = document.getElementById('fileExt').value;
	    var IDDocument = document.getElementById('documentId').value;
	    var DocumentNumber = document.getElementById('documentNumber').value;
	    var retValue = CheckOutDocument(defaultFilePath,
										fileExt,
										IDDocument,
										DocumentNumber,
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
        //this.fso = FsoWrapper_CreateFsoObject();
        if (this.fso == undefined) {
            this.fso = window.document.plugins[1];
        }

        if (this.fso == undefined) {
            this.fso = document.plugins[1];
        }
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
	    var percorso = document.getElementById("hdnFilePath").value;
	    var strIsSignedFile = document.getElementById("isSigned").value;
	    var IsSignedFile = false;

	    if (strIsSignedFile == 'True')
	        IsSignedFile = true;

	    var strategy = null;

        // Intercetta modello MTEXT
	    if (percorso.indexOf("mtext://") > -1) {
	        strategy = new MTextCheckInStrategy();
	    } else {
	        strategy = new FSCheckInStrategy();
	    }

	    var retValue = CheckInDocument(true, true, IsSignedFile, true, strategy);
			
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
	    var IDDocument = document.getElementById('documentId').value;
	    var DocumentNumber = document.getElementById('documentNumber').value;
	    ShowDialogCheckOutStatus(IDDocument, DocumentNumber);

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
<div style="float: left;">
<table cellSpacing="0" cellPadding="0" border="0">
	<tr>
		<td valign = "middle">
			<uc1:CheckInOutController id="checkInOutController" runat="server"></uc1:CheckInOutController>
			<input id="txtResponse" type="hidden" runat="server" NAME="txtResponse">
			<input id="txtExtension" type="hidden" runat="server" NAME="txtExtension">
		</td>
		<td vAlign="middle">
		    <cc2:CustomImageButton runat="server" ID="btnSave"  ImageUrl="../Images/Icons/save_local_file.png"
            OnMouseOutImage="../Images/Icons/save_local_file.png" OnMouseOverImage="../Images/Icons/save_local_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/save_local_file_disabled.png" OnClientClick = "return SaveFile('');" />
		</td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle">
		    <cc2:CustomImageButton ID="btnCheckOut" runat="server" ImageUrl="../Images/Icons/lock.png"
            OnMouseOutImage="../Images/Icons/lock.png" OnMouseOverImage="../Images/Icons/lock_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/lock_disabled.png" />
		</td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle">
            <cc2:CustomImageButton ID="btnCheckIn" runat="server" ImageUrl="../Images/Icons/unlock.png"
            OnMouseOutImage="../Images/Icons/unlock.png" OnMouseOverImage="../Images/Icons/unlock_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/unlock_disabled.png" />
        </td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle">
            <cc2:CustomImageButton ID="btnUndoCheckOut" runat="server" ImageUrl="../Images/Icons/lock_no_save.png"
            OnMouseOutImage="../Images/Icons/lock_no_save.png" OnMouseOverImage="../Images/Icons/lock_no_save_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/lock_no_save_disabled.png" />
        </td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle">
            <cc2:CustomImageButton ID="btnOpenCheckedOutFile" runat="server" ImageUrl="../Images/Icons/open_file.png"
            OnMouseOutImage="../Images/Icons/open_file.png" OnMouseOverImage="../Images/Icons/open_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/open_file_disabled.png"/>
        </td>
		<td width="8" vAlign="middle"></td>
		<td vAlign="middle">
            <cc2:CustomImageButton ID="btnShowCheckOutStatus" runat="server" ImageUrl="../Images/Icons/View_lock_status.png"
            OnMouseOutImage="../Images/Icons/View_lock_status.png" OnMouseOverImage="../Images/Icons/View_lock_status_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/View_lock_status_disabled.png"/>
        </td>
	</tr>
</table>
</div>
<asp:UpdatePanel ID="pnlFileExtValue" UpdateMode="Conditional" runat="server">
   <ContentTemplate>
        <asp:HiddenField ID="fileExt" ClientIDMode="Static" runat="server"/>
        <asp:HiddenField ID="hdnFilePath" ClientIDMode="Static" runat="server"/>
        <asp:HiddenField ID="documentId" ClientIDMode="Static" runat="server"/>
        <asp:HiddenField ID="documentNumber" ClientIDMode="Static" runat="server"/>
        <asp:HiddenField ID="isSigned" ClientIDMode="Static" runat="server"/>
    </ContentTemplate>
</asp:UpdatePanel>