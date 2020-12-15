<%@ Page language="c#" Codebehind="acquisizione.aspx.cs" AutoEventWireup="false" Inherits="NttDataWA.Popup.acquisizione" %>
<?xml version="1.0" encoding="UTF-8" ?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" id="Html" runat="server">
<head id="Head1" runat="server">
<title></title>
<script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
	function inviaScansione() {
		if (document.acquisizione.ctrlOptAcq != undefined) {
		    var retValue = document.acquisizione.ctrlOptAcq.SavedFile;
		    var error = document.acquisizione.ctrlOptAcq.ErrNumber; 
		    InviaFileXmlUpload(retValue, false, false, true, true, false);
		    parent.closeAjaxModal('ActiveXScann', 'up');
		}
		else {
		    alert('OCX <<DocsPa_AcquisisciDoc>> not found!');
            parent.closeAjaxModal('ActiveXScann', '');
		}
	}


	function InviaFileXmlUpload(fileName, convertiPDF, keepOriginal, removeLocalFile, convertiPDFServer, convertPdfSync) {
		var uploader = null;
		var xml_dom = null;
		var cartaceo = true;
		var retval = false;

		try {
			uploader = new ActiveXObject("DocsPa_AcquisisciDoc.ctrlUploader");
			xml_dom = uploader.GetXMLRequest(fileName, convertiPDF && !convertiPDFServer, false);
		} catch (ex) {
			alert("Si è verificato un errore durante l'upload. " + ex.Description);
			retval = false;
			return retval;
		}

		if (uploader.ErrNumber != 0) {
			alert(uploader.ErrDescription);
			retval = false;
		}
		else {
			var http = null;
			try {
			    http = new ActiveXObject("MSXML2.XMLHTTP");
			    http.Open("POST", "<%= fullpath%>/Upload.aspx?cartaceo=" + cartaceo + "&convertiPDF=" + convertiPDF + "&convertiPDFServer=" + convertiPDFServer + "&convertiPDFServerSincrono=" + convertPdfSync, false);
			    http.send(xml_dom);
			    retval = true;
			} catch (ex) {
			    alert("Errore durante l'upload del file: " + ex.Message);
			    retval = false;
			    return retval;
			}
		}

		return retval;
	}

	function scan(stampaSegnaturaAbilitata, segnatura) {
		try {
			// A partire dalla versione 3.5.15, DocsPa_AcquisisciDoc.ocx
			// gestisce l'impostazione del parametro per la stampa della
			// segnatura direttamente tramite gli scanner REI.
			// E' gestita un'eccezione nel caso in cui la versione del componente
			// è precedente alla suddetta e non gestisce la stringa di segnatura
			document.acquisizione.ctrlOptAcq.PrintSegnatureEnabled = stampaSegnaturaAbilitata;
			document.acquisizione.ctrlOptAcq.PrintSegnature = segnatura;
		}
		catch (e) {
			alert(e);
		}
			
		document.acquisizione.ctrlOptAcq.ScannerStart(); 
		return false;
	}

	$(function () {
	    var height = document.documentElement.clientHeight;
	    height -= document.getElementById('ctrlOptAcq').offsetTop; // not sure how to get this dynamically
	    height -= 50; /* whatever you set your body bottom margin/padding to be */
	    document.getElementById('ctrlOptAcq').style.height = height + "px";

        if ('<%=this.IsEnabledAutomaticScan %>' == 1) {
            return scan('<%=this.StampaSegnaturaAbilitata%>', '<%=this.SegnaturaProtocollo%>');
        }
    });
	</script>
    <style type="text/css">
    .row
    {
        float: left;
        min-height: 1px;
        margin: 0 0 1px 0;
        text-align: left;
        vertical-align: top;
        background: #d9d9d9;
        width: 100%;
    }
    
    .col
    {
        float: left;
        margin: 0;
        text-align: center;
        width: 16%;
    }
    
    .white 
    {
        background: #fff;
    }
    </style>
</head>
<body id="IdMasterBody" runat="server">
<form id="acquisizione" name="acquisizione">
    <div class="row">
        <div class="col"><input type="image" src="../Images/Scan/ico_scan_avvia.gif" onclick="return scan('<%=this.StampaSegnaturaAbilitata%>', '<%=this.SegnaturaProtocollo%>');" alt="Avvia scansione" /></div>
        <div class="col"><input type="image" src="../Images/Scan/ico_scan_elimina.gif" onclick="document.acquisizione.ctrlOptAcq.ImgDelete(); return false;" alt="Elimina pagina" /></div>
        <div class="col"><input type="image" src="../Images/Scan/ico_scan_precedente.gif" onclick="document.acquisizione.ctrlOptAcq.ImgPrev(); return false;" alt="Pagina precedente" /></div>
        <div class="col"><input type="image" src="../Images/Scan/ico_scan_successiva.gif" onclick="document.acquisizione.ctrlOptAcq.ImgNext(); return false;" alt="Pagina successiva" /></div>
        <div class="col"><input type="image" src="../Images/Scan/ico_scan_ruota.gif" onclick="document.acquisizione.ctrlOptAcq.ImgRotation(); return false;" alt="Ruota immagine" /></div>
        <div class="col"><input type="image" src="../Images/Scan/ico_scan_salva.gif" onclick="document.acquisizione.ctrlOptAcq.SaveScanner();inviaScansione(); return false;" alt="Invia File" /></div>
    </div>
    <div class="row white">
        <object id="ctrlOptAcq" codebase="../activex/DocsPa_AcquisisciDoc.CAB#version=1,0,0,0" height="100%"
			width="100%" classid="CLSID:00E3AC1D-2F39-465E-BD2C-24A4E1F1E83D" viewastext>
			<param name="_ExtentX" value="17357" />
			<param name="_ExtentY" value="7646" />
		</object>
    </div>
</form>
</body>
</html>
