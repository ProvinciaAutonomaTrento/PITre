<%@ Page Language="c#" CodeBehind="acquisisciDocumento.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.popup.acquisisciDocumento" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController"
    TagPrefix="uc2" %>
<%@ Register Src="../AcrobatIntegration/ClientController.ascx" TagName="ClientController"
    TagPrefix="uc1" %>
<%@ Register src="../ActivexWrappers/CacheWrapper.ascx" tagname="CacheWrapper" tagprefix="uc5" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">

    <script type="text/javascript">
		    
		    // fileName: nome del file
		    // convertPdf: conversione pdf
		    // keepOriginal: mantieni originale
		    // removeLocalFile: rimuovi locale
		    // convertPdfLocally: se true locale altrimenti centrale
		    // convertPdfSync: se true sincrona, altrimenti asincrona
		    function InviaFileXmlUpload(fileName, convertPdf, keepOriginal, removeLocalFile, convertPdfLocally, convertPdfSync)
		    {
		        document.upload.fileName.value = fileName;
			    document.upload.convertiPDF.value = convertPdf;
			    
			    //Verifico se è abilitata la conversione pdf lato server e se è stata richiesta
			    //Ricordo che vince sempre la conversione pdf lato server
			    //var documentPdfConvertServer = <%=DocumentPdfConvertServer%>;
			    //if((documentPdfConvertServer || convertPdfSincrono) && convertPdf )
			    //{
			    //    document.upload.convertiPDF.value = !convertPdf;
                document.upload.convertiPDFServer.value = !convertPdfLocally;
                document.upload.pdfSincrono.value = convertPdfSync;
			    //}
			    //else
			    //{
			    //    document.upload.convertiPDFServer.value = "false";;
			    //   document.upload.pdfSincrono.value = "false";
			    //}
			    
                document.upload.keepOriginal.value = keepOriginal;
			    document.upload.removeLocalFile.value = removeLocalFile;
			    document.upload.cartaceo.value = document.acquisisciDocumento.chk_cartaceo.checked;
			    //modifica
			    var validatesize = SF_ValidateFileSize(fileName);
	        	var useTransfertCache = SF_UseTransfertCache(fileName, validatesize);
	        	document.acquisisciDocumento.cache.value = useTransfertCache;
                //fine modifica
			    document.upload.submit();
		    }
		    
		    
			function InviaFile(fileName) 
			{
	        	var validateformat = SF_ValidateFileFormat(fileName);
	        	var validatesize = SF_ValidateFileSize(fileName);
	        	//modifica
	        	var useTransfertCache = SF_UseTransfertCache(fileName, validatesize);
             	//fine modifica
	        	// Validazione del formato file fornito in ingresso
                if(validateformat && validatesize)
                {  
                   //modifica
                   document.acquisisciDocumento.cache.value = useTransfertCache;
                   //fine modifica
                    document.acquisisciDocumento.submit();
			}
			
			}
			
			// Scansione documenti
			function Scan()
			{
				// Se conversione in pdf con sdk di adobe acrobat
				if (IsCheckedConvertPdfWithAcrobat() && ScanWithAcrobatIntegration())
					ScanDocumentsWithAcrobat();
				else
					ScanDocuments();	
			}

			// Verifica se deve effettuare la conversione in PDF
			// mediante l'SDK di adobe acrobat
			function IsCheckedConvertPdfWithAcrobat()
			{
			    //Controllo se è stata richiesta la conversione PDF lato server
				//In caso affermativo disabilito la conversione lato client
				//Per default vince sempre quella lato server
				
				// DECOMMENTARE SE DEVO FAR VINCERE LA CONV CENTRALIZZATA
			    
			    return (document.acquisisciDocumento.chk_ConvertiPDF.checked && 
			        IsIntegrationActiveAndInstalled() && document.getElementById("optLocale").checked);
			        
			}

			// Scansione di documenti mediante docspa
			function ScanDocuments()
			{	
                var args = new Object;
				args.window = window;

                var fileName="";
                var segn = "<%=this.SegnaturaProtocollo%>";
                if ("<%=this.IsActiveSmartClient%>" == "True")
                    fileName=window.showModalDialog('<%=DocsPAWA.Utils.getHttpFullPath()%>/SmartClient/DocumentAcquisitionDlg.aspx?segnatura='+segn+'',
													args,
													'dialogWidth:500px;dialogHeight:100px;status:no;resizable:no;scroll:no;center:yes;help:no;');
				else
//                    return scanDirect('<%=this.StampaSegnaturaAbilitata%>', '<%=this.SegnaturaProtocollo%>');
				    fileName=window.showModalDialog('<%=DocsPAWA.Utils.getHttpFullPath()%>/popup/acquisizione.aspx',
													args,
													'dialogHeight: ' + window.screen.availHeight + 'px; dialogWidth: ' + window.screen.availWidth + 'px; resizable: yes;');

				if (fileName!=null && fileName!='')
				{
					var pdfAcrobat=(IsCheckedConvertPdfWithAcrobat());
					
					if (pdfAcrobat)
					{
						// Se conversione in pdf con sdk di adobe acrobat attiva,
						// viene fatta la conversione in pdf del file tiff
						// e inviato il file a docspa
						ConvertPdfWithAcrobat(fileName,document.acquisisciDocumento.chkRecognizeText.checked);
					
					}
					else
					{
						// Upload del file
						InviaFileXmlUpload(fileName, document.acquisisciDocumento.chk_ConvertiPDF.checked, true,
						    true, document.getElementById("optLocale").checked,
						    document.getElementById("optSincrona").checked);
					}
				}
			}
			
			// Conversione di un file in pdf con acrobat integration
			function ConvertPdfWithAcrobat(originalFilePath, recognizeText)
			{   
				var outputPDFFilePath=GetTemporaryPDFFilePath();
                
                // Viene avviato il processo si conversione con l'integrazione adobe				
				if (ConvertPdfFile(originalFilePath, outputPDFFilePath, recognizeText))
				    InviaFileXmlUpload(outputPDFFilePath, false, false, false, false, false);
				    
			}
			
			// Upload del file
			function Upload()
			{
				window.document.body.style.cursor='wait';
				
				//Controllo se è stata richiesta la conversione PDF lato server
				//In caso affermativo disabilito la conversione lato client
				//Per default vince sempre quella lato server
			    var documentPdfConvertServer = <%=DocumentPdfConvertServer%>;
			    // Verifico se è stata richiesta la conversione lato server sincrona:
			    // è quella che vince sempre
			    var documentSyncPdfConvertServer = <%= DocumentSyncPdfConvertServer %>;
				
				try
				{
				    // Se è richiesta scansione del documento...
					if (document.acquisisciDocumento.optAcquisizioneScanner.checked)
					{
					    // ...chiamata alla procedura di scansione documento
						Scan();
					}
					else
					{
					    // ...altrimenti se è richiesta conversione pdf...
					    if(document.acquisisciDocumento.chk_ConvertiPDF.checked)
					    {
					        // ...allora verifica se è locale o centralizzata
					        // Se è locale...
					        if(document.getElementById("optLocale").checked)
					        {
					            // ...se è installata l'integrazione adobe...
					            if (IsIntegrationActiveAndInstalled())
					            {	
					                // Conversione in pdf con l'integrazione acrobat
						            ConvertPdfWithAcrobat(document.acquisisciDocumento.uploadedFile.value, document.acquisisciDocumento.chkRecognizeText.checked);
					            }
					            else
					            {
					                // ...altrimenti invia file con conversione locale
					                InviaFileXmlUpload(document.acquisisciDocumento.uploadedFile.value, true, false, false, true, false);
					            }   
					        }
					        else
					        {
					            // ...altrimenti invia file con conversione centralizzata e sincrono/asincrono dipendente
					            // dalle impostazioni
					            InviaFileXmlUpload(document.acquisisciDocumento.uploadedFile.value, true, false,
					                false, false, document.getElementById("optSincrona").checked);
					        
					        }
					    }
					    else
					    {
					        // ...altrimenti invia direttamente il file senza convertirlo
					        InviaFile(document.acquisisciDocumento.uploadedFile.value);
					    }
					        
					}
				}
				catch (e)
				{
				    //alert(e.message.toString());
					alert("Errore nell'upload del documento.");
				}
				
				window.document.body.style.cursor='default';
			}
			
			// Scansione di documenti mediante adobe acrobat
			function ScanDocumentsWithAcrobat()
			{	
				// Scansione del documento in cartella temporanea
				var acquiredFilePath=GetTemporaryPDFFilePath();
				
				if (ScanPdfFile(acquiredFilePath, document.acquisisciDocumento.chkRecognizeText.checked))
				    InviaFileXmlUpload(acquiredFilePath, false, false, true, false, false);
			}
			
			// Costruzione del percorso del file PDF temporaneo creato mediante integrazione adobe acrobat
			function GetTemporaryPDFFilePath()
			{
				var fso=FsoWrapper_CreateFsoObject();
					 // OLD:  var pdfFolder=fso.GetSpecialFolderPath(2) + "\\DPAImage\\";
				var specialFolder="";
				try
				{
				
				    specialFolder=fso.GetSpecialFolderPath(2);
				}
				catch(e)
				{
				    
				    specialFolder=fso.GetSpecialFolder(2);
				}
		
			   var pdfFolder=specialFolder + "\\DPAImage\\"; 
				
				if (!fso.FolderExists(pdfFolder))
					fso.CreateFolder(pdfFolder);
					
				return pdfFolder + fso.GetTempName() + ".pdf"; 	
			}
			
			// Impostazione valore per il check "chkConvertiPDF"
			function BindCheckConvertPdf()
			{
			    // Variabile utilizzata per verificare se l'utente può modificare lo stato
			    // della check box "Converti in PDF"
			    var documentPdfConvertEnabled = <%=DocumentPdfConvertEnabled%>;		
			    // Variabile utilizzata per verificare se di default deve essere
			    // spuntato il flag per la conversione PDF
			    var documentPdfConvert = <%=DocumentPdfConvert%>;
			    // Variabile utilizzata per verificare se è attiva la conversione PDF centralizzata
			    var documentPdfConvertServer = <%=DocumentPdfConvertServer%>;
			    // Variabile utilizzata per verificare se è attiva la conversione 
			    // centralizzata sincrona
			    var documentSyncPdfConvertServer = <%= DocumentSyncPdfConvertServer %>;
			    			    
			    document.acquisisciDocumento.chk_ConvertiPDF.checked = documentPdfConvert;
                document.acquisisciDocumento.chk_ConvertiPDF.disabled = !documentPdfConvertEnabled;
                
                // Se la checkbox non è flaggata...
                //if(!document.acquisisciDocumento.chk_ConvertiPDF.checked)
                    // ...lo stato di flag dipende dal valore della proprietà documentPdfConvertServer || documentSyncPdfConvertServer 
                //    document.acquisisciDocumento.chk_ConvertiPDF.checked = documentPdfConvertServer || documentSyncPdfConvertServer;
                
                // Impostazione dell'interfaccia grafica in funzione della configurazione
                OnClickCheckConvertPdf();
                OnClickLocaleCentralizzata();
                
                // Se gli option button Locale - Centrale sono disabilitati...
                if(document.getElementById("optLocale").disabled && 
                    document.getElementById("optCentrale").disabled)
                {
                    // ...bisogna unflaggare la check box "Converti in PDF", rieseguire l'evento click
                    // e disabilitare la check box
                    document.acquisisciDocumento.chk_ConvertiPDF.checked = false;
                    
                    OnClickCheckConvertPdf();
                    
                    //document.acquisisciDocumento.chk_ConvertiPDF.disabled = true;
                    
                }

			}
			
			// Impostazione valori per il check "chkRecognizeText"
			function BindCheckRecognizeText()
			{
			    var adobeIntegration = IsIntegrationActiveAndInstalled();

				var	pnl=document.getElementById("pnlRecognizeText");
				
				//Verifico che la conversione lato server sia abilitata    
                //In questo caso la conversione OCR è sempre non attiva
                //var documentPdfConvertServer = <%=DocumentPdfConvertServer%>;
                // Variabile utilizzata per verificare se è attiva la conversione 
			    // centralizzata sincrona
			    //var documentSyncPdfConvertServer = <%= DocumentSyncPdfConvertServer %>;
                
                // Recupero degli option button Locale-Centrale
				var optLocale = document.getElementById("optLocale");
			    
			    // Recupero della checkbox "Converti in PDF"
			    var conversionePDFAttiva = document.acquisisciDocumento.chk_ConvertiPDF.checked;
			    
			    // Variabile utilizzata per verificare se si può procedere con le verifiche per 
			    // la visualizzazione della checkbox "Interpreta con OCR"
			    var canContinue = true;
			    
			    // Se è selezionata conversione pdf locale...
			    if(!(conversionePDFAttiva && optLocale.checked))
			        canContinue = false;			    
                
                if (canContinue && adobeIntegration && IsEnabledRecognizeText())
				{
					pnl.style.visibility = "visible";

					document.acquisisciDocumento.chkRecognizeText.disabled = (!document.acquisisciDocumento.chk_ConvertiPDF.checked);
					document.acquisisciDocumento.chkRecognizeText.checked = (!document.acquisisciDocumento.chkRecognizeText.disabled);
				}
				else
				{
				    pnl.style.visibility = "hidden";
				}
			}			
			
			// Gestione abilitazione / disabilitazione tabella per l'acquisizione da scanner
			function EnableTableAcquisizioneDaScanner(enabled)
			{
				lblAcquisisciDaScanner.disabled=!enabled;
			}
						
			// Gestione abilitazione / disabilitazione tabella per l'acquisizione dei file
			function EnableTableAcquisizioneFile(enabled)
			{
				lblAcquisisciDaFile.disabled=!enabled;
			}

			function OnClickCheckConvertPdf()
			{
			    // Variabile utilizzata per verificare se l'utente può modificare lo stato
			    // della check box "Converti in PDF"
			    var documentPdfConvertEnabled = <%=DocumentPdfConvertEnabled%>;		
			    // Variabile utilizzata per verificare se di default deve essere
			    // spuntato il flag per la conversione PDF
			    var documentPdfConvert = <%=DocumentPdfConvert%>;
			    // Variabile utilizzata per verificare se è attiva la conversione PDF centralizzata
			    var documentPdfConvertServer = <%=DocumentPdfConvertServer%>;
			    // Variabile utilizzata per verificare se è attiva la conversione 
			    // centralizzata sincrona
			    var documentSyncPdfConvertServer = <%= DocumentSyncPdfConvertServer %>;
			    
			    // Prelevamento dello stato del flag della checkbox "Converti in PDF"
			    var conversioneFlag = document.acquisisciDocumento.chk_ConvertiPDF.checked;
			    
			    // Recupero degli option button Locale-Centrale
				var optLocale = document.getElementById("optLocale");
				var optCentrale = document.getElementById("optCentrale");
				    
			    // Recupero degli option button Sincrona-Asincrona
			    var optSincrona = document.getElementById("optSincrona");
			    var optAsincrona = document.getElementById("optAsincrona");
			    
				// Se è attiva la converisone PDF centralizzata, bisogna preferirla
				if(documentPdfConvertServer || documentSyncPdfConvertServer)
				{
				    // Flagging dell'opzione "centralizzata"
				    optCentrale.setAttribute("checked", true);
				    optLocale.setAttribute("checked", false);
    				    
			        // Se è attiva la conversione centralizzata sincrona...
			        if(documentSyncPdfConvertServer)
			        {
			            // Flagging dell'opzione sincrona
			            optSincrona.setAttribute("checked", true);
			            optAsincrona.setAttribute("checked", false);
			        }
			        else
			        {
			            // Flagging dell'opzione asincrona
			            optSincrona.setAttribute("checked", false);
			            optAsincrona.setAttribute("checked", true);
			        }
			    
				}
				
				// Abilitazione/Disabilitazione del pulsante della lista di opzioni
				// Locale-Centrale
				optLocale.setAttribute("disabled", !(conversioneFlag && documentPdfConvertEnabled && IsIntegrationActiveAndInstalled()));
				optCentrale.setAttribute("disabled", !((documentPdfConvertServer || documentSyncPdfConvertServer) && conversioneFlag && documentPdfConvertEnabled));
				optSincrona.setAttribute("disabled", !(documentSyncPdfConvertServer && documentPdfConvertEnabled && conversioneFlag));
				optAsincrona.setAttribute("disabled", !(documentPdfConvertServer && documentPdfConvertEnabled && conversioneFlag && <%= IsDocumentSaved %>));
				
				if(conversioneFlag)
				{
				    document.getElementById("rigaLocaleCentrale").style.visibility = "";
				    document.getElementById("rigaSincronaAsincrona").style.visibility = "";
				}
				else
				{
				    document.getElementById("rigaLocaleCentrale").style.visibility = "hidden";
				    document.getElementById("rigaSincronaAsincrona").style.visibility = "hidden";
				}				
				
				// Se entrambi gli option button Sincrona - Asincrona sono disattivati...
				if(optSincrona.disabled && optAsincrona.disabled)
				{
				    // ...bisogna spostare il check su locale e disattivare l'opzione Centrale...
				    optCentrale.disabled = true;
				    optLocale.checked = true;
				    
				    // ...e nascondere la riga Sincrona - Asincrona
				    document.getElementById("rigaSincronaAsincrona").style.visibility = "hidden";
				    
				}
				
				// Assegnazione valori check recognizetext
				BindCheckRecognizeText();

			}
		
			
			// Funzione per la gestione del cambio di selezione nella lista di opzioni
			// Locale - Centralizzata
			function OnClickLocaleCentralizzata()
			{
			    // Variabile utilizzata per verificare se l'utente può modificare lo stato
			    // della check box "Converti in PDF"
			    var documentPdfConvertEnabled = <%=DocumentPdfConvertEnabled%>;		
			    // Variabile utilizzata per verificare se è attiva la conversione PDF centralizzata
			    var documentPdfConvertServer = <%=DocumentPdfConvertServer%>;
			    // Variabile utilizzata per verificare se è attiva la conversione 
			    // centralizzata sincrona
			    var documentSyncPdfConvertServer = <%= DocumentSyncPdfConvertServer %>;
			    
			    // Recupero dei due option button Locale-Centrale
			    var optLocale = document.getElementById("optLocale");
			    var optCentrale = document.getElementById("optCentrale");
			    
				// Recupero degli option button Sincrona - Asincrona
			    var optSincrona = document.getElementById("optSincrona");
			    var optAsincrona = document.getElementById("optAsincrona");
			    
			    // Se l'utente può modificare le opzioni...
			    if(documentPdfConvertEnabled)
			    {
			        // ...se è abilitata e selezionata la conversione centralizzata...
			        if((documentPdfConvertServer || documentSyncPdfConvertServer) && optCentrale.checked)
			        {
			            // ...si procede all'abilitazione o meno dei bottoni di opzione in base
			            // alle impostazioni. Inoltre il pulsante di opzione "Conversione asincrona"
			            // viene disattivato per default se il documento non è stato ancora salvato
			            optSincrona.disabled = !documentSyncPdfConvertServer;
			            optAsincrona.disabled = !(documentPdfConvertServer && <%= IsDocumentSaved %>);
			            
			        }
			        else
			        {  
			            // ...altrimenti si procede alla disabilitazione di entrambi i bottoni
			            optSincrona.disabled = true;
			            optAsincrona.disabled = true;
			            
			        }
			        
			        if(optCentrale.checked)
			        {
			            document.getElementById("rigaSincronaAsincrona").style.visibility = "";
			        }
			        else
			        {
			            document.getElementById("rigaSincronaAsincrona").style.visibility = "hidden";
			        }
			    }
			    
			    BindCheckRecognizeText();
			    
			}
			
			// Impostazione del radio button selezionato per default
			// (acquisizione da scanner o da file)
			function SetDefaultRadioButton()
			{
				document.acquisisciDocumento.optAcquisizioneScanner.checked=true;
				EnabledControls(true);
			}
			
			// Gestione abilitazione / disabilitazione controlli dipendenti
			// dai radio buttons (acquisizione da scanner o da file)
			function EnabledControls(acquisizioneScannerEnabled)
			{
				EnableTableAcquisizioneDaScanner(acquisizioneScannerEnabled);
				EnableTableAcquisizioneFile(!acquisizioneScannerEnabled);
				
				EnableCheckCartaceo(acquisizioneScannerEnabled);
		    }
			
			// Gestione abilitazione / disabilitazione controllo
			// checkbox documento cartaceo
			function EnableCheckCartaceo(acquisizioneScannerEnabled)
			{
                // Check documento cartaceo abilitato solo se il documento non è acquisito da scanner
				document.acquisisciDocumento.chk_cartaceo.disabled = acquisizioneScannerEnabled;

                if (acquisizioneScannerEnabled)
                    document.acquisisciDocumento.chk_cartaceo.checked = true;
			}
			
			// Handler evento click radio acquisizione da scanner
			function OnClickRadioAcquisizioneScanner()
			{
				var radio=document.acquisisciDocumento.optAcquisizioneScanner;
				if (radio!=null)
				{
					EnabledControls(radio.checked);
				}
			}

			// Handler evento click radio acquisizione da file
			function OnClickRadioAcquisizioneDaFile()
			{
				var radio=document.acquisisciDocumento.optAcquisisciDaFile;

				if (radio!=null)
				{
				    var checked = document.acquisisciDocumento.chk_cartaceo.checked;
				    if (document.acquisisciDocumento.chk_cartaceo.disabled)
				        checked = false;
				
					EnabledControls(!radio.checked);

					SetControlFocus(document.acquisisciDocumento.uploadedFile.id);
					
					document.acquisisciDocumento.chk_cartaceo.checked = checked;
				}
			}
			
			// Handler evento onFocus controllo uploadFile
			function OnFocusUploadFile()
			{
				var radio=document.acquisisciDocumento.optAcquisisciDaFile;
				if (radio!=null)
				{
					radio.checked=true;
					
					OnClickRadioAcquisizioneDaFile();
				}
			}
			
			// Impostazione del focus su un controllo
			function SetControlFocus(controlID)
			{	
				try
				{
					var control=document.getElementById(controlID);
					
					if (control!=null)
					{
						control.focus();
					}
				}
				catch (e)
				{
				
				}
			}

            //Funzione di acquisizione diretta
            function scanDirect(stampaSegnaturaAbilitata, segnatura) {
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
			    }
			
			    document.acquisizione.ctrlOptAcq.ScannerStart(); 
			    return false;
			}

			
    </script>

    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />

    <script type="text/javascript" language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
</head>
<body bottommargin="1" leftmargin="1" topmargin="6" rightmargin="1" ms_positioning="GridLayout">
    <form id="acquisisciDocumento" method="post" enctype="multipart/form-data" runat="server">
    <input id="cache" type="hidden" name="cache" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Acquisizione" />
  <uc5:CacheWrapper ID="CacheWrapper1" runat="server" />   
    <uc2:SupportedFileTypeController ID="supportedFileTypeController" runat="server" />
    <uc1:ClientController ID="AdobeClientController" runat="server" />
    <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    
    <table class="info" id="tblContainer" cellspacing="0" cellpadding="5" width="400"
        align="center" border="0" runat="server">
        <tr>
            <td class="item_editbox" colspan="3">
                <p class="boxform_item">
                    <asp:Label ID="Label3" runat="server">Acquisizione</asp:Label></p>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="3">
                <table cellspacing="0" cellpadding="3" border="0">
                    <tr>
                        <td valign="top" width="3%">
                            <asp:RadioButton ID="optAcquisizioneScanner" runat="server" CssClass="titolo_scheda"
                                GroupName="ACQUISIZIONE"></asp:RadioButton>
                        </td>
                        <td width="97%">
                            <table id="tblAcquisizioneDaScanner" cellspacing="0" cellpadding="3" border="0">
                                <tr>
                                    <td class="titolo_scheda">
                                        <asp:Label ID="lblAcquisisciDaScanner" runat="server" CssClass="titolo_scheda">Acquisisci da scanner</asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" height="10">
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" width="3%">
                            <asp:RadioButton ID="optAcquisisciDaFile" runat="server" CssClass="titolo_scheda"
                                GroupName="ACQUISIZIONE" Width="24px"></asp:RadioButton>
                        </td>
                        <td width="97%">
                            <table id="tblAcquisizioneDocumento" cellspacing="0" cellpadding="3" border="0">
                                <tr>
                                    <td class="titolo_scheda">
                                        <asp:Label ID="lblAcquisisciDaFile" runat="server">Acquisisci da file:</asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <input class="testo_grigio" id="uploadedFile" type="file" size="50" name="uploadedFile"
                                            runat="server" onfocus="OnFocusUploadFile();">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width: 50%" colspan="2">
                <asp:CheckBox ID="chk_ConvertiPDF" runat="server" CssClass="titolo_scheda" Text="Converti in PDF">
                </asp:CheckBox>
            </td>
            <td style="width: 50%">
                <asp:CheckBox ID="chk_cartaceo" runat="server" CssClass="titolo_scheda" Text="Cartaceo"
                    Checked="true" />
            </td>
        </tr>
        <tr id="rigaLocaleCentrale">
            <td style="width: 25%">
                <asp:RadioButton ID="optLocale" runat="server" GroupName="LocaleCentrale" Text="Locale"
                    Checked="true" CssClass="titolo_scheda" />
            </td>
            <td style="width: 25%">
                <asp:RadioButton ID="optCentrale" runat="server" GroupName="LocaleCentrale" Text="Centrale"
                    CssClass="titolo_scheda" />
            </td>
            <td style="width: 50%">
                <asp:Panel ID="pnlRecognizeText" runat="server">
                    <asp:CheckBox ID="chkRecognizeText" runat="server" CssClass="titolo_scheda" Text="Interpreta testo con OCR">
                    </asp:CheckBox>
                </asp:Panel>
            </td>
        </tr>
        <tr id="rigaSincronaAsincrona">
            <td style="width: 25%">
                <asp:RadioButton ID="optSincrona" runat="server" Text="Sincrona" Checked="true" GroupName="SincronaAsincrona"
                    CssClass="titolo_scheda" />
            </td>
            <td style="width: 25%">
                <asp:RadioButton ID="optAsincrona" runat="server" Text="Asincrona" GroupName="SincronaAsincrona"
                    CssClass="titolo_scheda" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center" height="30" colspan="3">
                <input class="PULSANTE" onclick="Upload(); window.returnValue=true;" runat="server"
                    type="button" value="INVIA" id="btnInvia" />
                <input class="PULSANTE" onclick="window.returnValue=false; window.close();" type="button"
                    value="CHIUDI" id="btnChiudi" />
            </td>
        </tr>
    </table>
    </form>
    <form name="upload" action="acquisizioneXML.aspx" method="post">
    <input id="fileName" type="hidden" name="fileName" />
    <input id="keepOriginal" type="hidden" name="keepOriginal" />
    <input id="convertiPDF" type="hidden" name="convertiPDF" />
    <input id="convertiPDFServer" type="hidden" name="convertiPDFServer" />
    <input id="removeLocalFile" type="hidden" name="removeLocalFile" />
    <input id="cartaceo" type="hidden" name="cartaceo" />
    <input id="pdfSincrono" type="hidden" name="pdfSincrono" />
    </form>
</body>
</html>
