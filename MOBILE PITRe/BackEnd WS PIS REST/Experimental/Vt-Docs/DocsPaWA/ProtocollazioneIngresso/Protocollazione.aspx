<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="SmistaUO" Src="Smistamento/SmistaUO.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="PdfCapabilities" Src="PdfCapabilitiesSmartClient.ascx" %>
<%@ Page language="c#" Codebehind="Protocollazione.aspx.cs" AutoEventWireup="false" Inherits="ProtocollazioneIngresso.Protocollazione" %>
<%@ Register src="../Note/DettaglioNota.ascx" tagname="DettaglioNota" tagprefix="uc4" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc3" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register TagPrefix="uc1" TagName="AccessoNegato" Src="AccessoNegato.ascx" %>
<%@ Register TagPrefix="uc3" TagName="StampaEtichetta" Src="StampaEtichetta.ascx" %>
<%@ Register TagPrefix="uc2" Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagName="RicercaVeloce" TagPrefix="ut8" Src="~/UserControls/RubricaVeloce.ascx" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc6" %>

<%@ Register src="../documento/Oggetto.ascx" tagname="Oggetto" tagprefix="uc5" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<SCRIPT language="JavaScript">						
			window.name="DialogProtocollazioneIngresso";			
			var dialogRetValue=window.dialogArguments;			
		</SCRIPT>
		<SCRIPT language="javascript">
            

			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-100)/2;
			var new_h = (h-400)/2;
			
			function apriPopupAnteprima() {
			    var sUrl = unescape(window.location.pathname);
				//window.open('AnteprimaProfDinamica.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
				window.showModalDialog('../documento/AnteprimaProfDinamica.aspx?ProtoSempl=yes&Chiamante=' + sUrl,'','dialogWidth:700px;dialogHeight:400px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
				//window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx?ProtoSempl=yes','','dialogWidth:700px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
				//window.open('../documento/AnteprimaProfDinamica.aspx?ProtoSempl=yes&Chiamante=' + sUrl,'','width=700px,height=400px,status=no,resizable=no,scrollbars=no,top=100,left=100');
			}
		
			// Gestione visualizzazione pulsante di protocollazione,
			// che  viene nascosto (e visualizzato al suo posto un pulsante disabilitato) 
			// per evitare che l'utente possa effettuare diverse protocollazioni contemporaneamente
			 function EnableButtonProtocollazione(isEnabled)
                  {
                        var btnProtocolla=GetButtonProtocolla();
                        var btnProtocollaDisabled=GetButtonProtocollaDisabled();
                        if (btnProtocolla != null && btnProtocollaDisabled != null)
                       {
                            if (isEnabled)
                            {
                                 btnProtocolla.style.display='';
                                  btnProtocollaDisabled.style.display='none';
                            }
                            else
                            {
                                  btnProtocolla.style.display='none';
                                  btnProtocollaDisabled.style.display='';
                            }
                        }
                  }

			// Impostazione del focus su un controllo
			function SetControlFocus(controlID)
			{	
				try
				{
					//var control=document.getElementById(controlID);
					
					//if (control!=null)
					//{
					//	control.focus();
					//}
				}
				catch (e)
				{
				
				}
			}
			
			// Reperimento imagebutton nuovo protocollo
			function GetButtonNuovoProtocollo()
			{
				return document.getElementById('btnNuovoProtocollo');
			}
			
			// Reperimento imagebutton pulizia dati
			function GetButtonClearData()
			{
				return document.getElementById('btnClearData');
			}
			
			// Reperimento imagebutton riproposizione dati
			function GetButtonRiproponi()
			{
				return document.getElementById('btnRiproponi');
			}
			
			// Reperimento imagebutton chiudi
			function GetButtonClose()
			{
				return document.getElementById('btnClose');
			}
			
			// Reperimento imagebutton protocolla
			function GetButtonProtocolla()
			{
				return document.getElementById('btnProtocolla');						
			}
			
			// Reperimento imagebutton protocolla disabilitato
			function GetButtonProtocollaDisabled()
			{
				return document.getElementById('btnProtocollaDisabled');						
			}			
			
			// Reperimento imagebutton acquisizione documento
			function GetButtonAcquireDocument()
			{
				return document.getElementById('btnAcquireDocument');
			}

			// Reperimento imagebutton acquisizione allegato
			function GetButtonAcquireAttach()
			{
				return document.getElementById('btnAcquireAttach');
			}			
						
			// Visualizzazione clessidra
			function ShowWaitCursor()
			{
				
				window.document.body.style.cursor="wait";
				tblContainer.style.cursor="wait";
				tblButtonsContainer.style.cursor="wait";
				panelUO.style.cursor="wait";			
					
				GetButtonProtocolla().style.cursor="wait";
				GetButtonNuovoProtocollo().style.cursor="wait";
				GetButtonRiproponi().style.cursor="wait";
				GetButtonClearData().style.cursor="wait";
				GetButtonAcquireDocument().style.cursor="wait";
				GetButtonAcquireAttach().style.cursor="wait";
				GetButtonClose().style.cursor="wait";
			}
			
			function CloseWindow()
			{
				window.close();
			}
			
			// Impostazione dello stato del protocollo come "ProtocollatoAcquisito"
			function SetStatoProtocollatoAcquisito()
			{
				document.frmProtIngresso.txtStatoDocumento.value="ProtocollatoAcquisito";
			}			
			
			function GetLabelDocumentiAcquisiti()
			{
				return document.getElementById("lblDescrizioneDocAcquisiti");;
			}
			
			function GetLabelAllegatiAcquisiti()
			{
				return document.getElementById("lblDescrizioneAllAcquisiti");;
			}
			
			// Viene incrementato il contatore dei documenti principali scannerizzati
			function IncrementCounterDocuments()
			{
				var countDocuments=0;
				
				if (document.frmProtIngresso.txtCountDocument.value!='')
					countDocuments=parseInt(document.frmProtIngresso.txtCountDocument.value,'');

				countDocuments++;
				
				document.frmProtIngresso.txtCountDocument.value = countDocuments;
				
				// Impostazione numero documenti acquisiti
				GetLabelDocumentiAcquisiti().innerHTML="N° documenti: " + countDocuments;
			}
						
			// Viene incrementato il contatore dei documenti allegati scannerizzati
			function IncrementCounterAttachments()
			{
				var countAttachments=0;
				
				if (document.frmProtIngresso.txtCountAttachment.value!='')
					countAttachments=parseInt(document.frmProtIngresso.txtCountAttachment.value,'');
					
				countAttachments++;
				
				document.frmProtIngresso.txtCountAttachment.value=countAttachments;
				
				// Impostazione numero allegati acquisiti
				GetLabelAllegatiAcquisiti().innerHTML="N° allegati: " + countAttachments;
			}
			
			// Restituzione del numero di documenti acquisiti
			// (il primo è il documento principale, quelli successivi sono nuove versioni)
			function GetCountDocuments()
			{
				return parseInt(document.frmProtIngresso.txtCountDocument.value);			
			}

			// Restituzione del numero di allegati acquisiti
			function GetCountAttachments()
			{
				return parseInt(document.frmProtIngresso.txtCountAttachment.value);			
			}

			// Verifica se è obbligatorio acquisire documenti
			/// <returns></returns>
			function IsAcquisizioneDocumentiObbligatoria()
			{
				return (document.frmProtIngresso.txtModAcquisizione.Value=="true");
			}
		
			// Scansione del singolo documento principale, riporta il path del documento acquisito
			function ScanSingleDocument()
			{
			    var retValue = false;
				
			    if (window.confirm("Si desidera scansionare il documento principale?"))
			    {
					var filepath = ShowDialogAcquisizioneDocumento(true);

					if (filepath != null && filepath != '')
				    {
                        // Upload del documento acquisito
                        retValue = UploadFile(filepath);
                            
                        if (retValue)
                        {
					        // Impostazione dello stato del protocollo come "ProtocollatoAcquisito"
					        SetStatoProtocollatoAcquisito();
                                // Se il documento è stato acquisito correttamente,viene incrementato il contatore del doc principale 					
                            IncrementCounterDocuments();
                        }
                        else
                        {
                                alert("Errore nell'acquisizione del documento principale");
                        }
				    }
                }
			
				return retValue;
			}
						
			// Scansione del singolo allegato
			function ScanSingleAttachment()
			{
			    var retValue = false;
				
				if (window.confirm("Si desidera scansionare un allegato?")) 
				{		
					var filepath = ShowDialogAcquisizioneDocumento(false);				
					
                    // Upload del documento acquisito
					if (filepath != null && filepath != '')
					{
					    retValue = UploadFile(filepath);
					    
					    if (retValue)
					{
                        // Se il documento è stato acquisito correttamente,
					    // viene incrementato il contatore del doc allegato
					    IncrementCounterAttachments();
                    }
					    else
					    {
					        alert("Errore nell'acquisizione dell'allegato");
					    }
                    }
				}
			
				return retValue;
			}
			
			// Flusso di scansione del documento principale e degli allegati
			function ScanDocuments()
			{
				var retValue = ScanSingleDocument();

				if (retValue)
					// Scansione degli allegati (solo se la scansione del doc principale è stata effettuata)
					ScanAttachments();
				
				if (GetCountDocuments()>0)
					document.frmProtIngresso.submit();
			}

			// Scansione in batch degli allegati
			function ScanAttachments()
			{	
				do
				{
					// Abilitazione / disabilitazoine pulsante acquisizione
					// allegati in base al numero di documenti acquisiti
					EnableImageButton(GetButtonAcquireAttach(),
										(GetCountAttachments()==0),
										'Images/btn_acquisisci_alleg.gif',
										'Images/btn_acquisisci_alleg_dis.gif');
					
					if (!ScanSingleAttachment(true))
					    break;
				} while (true);		
			}			
			
			// Creazione messaggio in caso di errore nell'invio del documento
			function GetUploadErrorMessage(fileName)
			{
				var msg="Si è verificato un errore nell'invio";
   				if (fileName.indexOf("DOC")>-1)
   					msg += " della versione 1";
   				else
   				{
   					var items=fileName.split("_");
					if (items[0]=="VER")
   						msg += " della versione " + items[1];
   					else if (items[0]=="ALL")
   						msg += " dell'allegato " + items[1];
   				}
   				msg+=".\nPotrebbe essere necessario acquisire nuovamente il documento.";
   				return msg;
			}
			
			// Upload del file acquisito
            function UploadFile(filePath)
			{

			    // Verifica se convertire il documento in PDF
			    if (document.frmProtIngresso.chkConvertiPDF != null)
			        var convertPDF = document.frmProtIngresso.chkConvertiPDF.checked;
			    else 
                    var convertPDF = false;
			    // Verifica se interpretare il testo del documento con ocr (se attivo)
			    if (document.frmProtIngresso.chkRecognizeTex != null)
			        var recognizeText = (!document.frmProtIngresso.chkRecognizeText.disabled && document.frmProtIngresso.chkRecognizeText.checked);
			    else 
                    var recognizeText = false;
				// Creazione querystring:
				// - filePath:			percorso del file da inviare
				// - convertPdf:		bool, se convertire o meno il doc in pdf
				// - recognizeText:		bool, se interpretare o meno testo del doc con ocr
				// - removeLocalFile:	bool, rimozione file da percorso di scansione
				var args='filePath=' + filePath + '&convertPdf=' + convertPDF + '&recognizeText=' + recognizeText + '&removeLocalFile=' + true;
				var returnValue=false;
				
				if (frmProtIngresso.txtSmartClientActivation.value=="true")
					// Modale contenente la logica per l'upload del file tramite componenti smartclient
					returnValue=window.showModalDialog('Scansione/UploadFileSmartClient.aspx?' + args,'','');
				else
					// Modale contenente la logica per l'upload del file
					returnValue=window.showModalDialog('Scansione/UploadFile.aspx?' + args,'','');
	
				return returnValue;
            }

			// Scansione di un documento.
			// Se acquisito correttamente, viene incrementato il 
			// contatore del numero dei documenti o degli allegati
			function ShowDialogAcquisizioneDocumento(isDocumentoPrincipale)
			{	
				var filePath="";
				var screenHeight=window.screen.availHeight;
				var percent=((screenHeight * 13) / 100);
				screenHeight=screenHeight - percent;

				var acquirePageUrl;
				if (document.frmProtIngresso.txtSmartClientActivation.value=='true')
					// filePath=window.showModalDialog('Scansione/AcquisizioneSmartClient.aspx?DocumentoPrincipale=' + isDocumentoPrincipale, '', 'dialogHeight:0px; dialogWidth:0px; dialogTop=0px; dialogLeft=0px');
					filePath=ShowImageViewer("<%=this.txtSegnatura.Text %>");
				else
					filePath=window.showModalDialog('Scansione/Acquisizione.aspx?DocumentoPrincipale=' + isDocumentoPrincipale, '', 'dialogHeight: ' + screenHeight + 'px; dialogWidth: ' + window.screen.availWidth + 'px; dialogTop: ' + percent + 'px; resizable: yes;');
				
				if (filePath!=null && filePath!='')
					filePath=CopyAcquiredFile(filePath,isDocumentoPrincipale);	
				return filePath;
			}
						
			function SetDocumentsFolder(documentsFolder)
			{
				document.frmProtIngresso.txtDocumentsFolder.value=documentsFolder;				
			}
			
			function GetDocumentsFolder()
			{
				return document.frmProtIngresso.txtDocumentsFolder.value;
			}
			
			// Reperimento dell'id del protocollo, composto da:
			// USERID_IDAMM_IDREGISTRO_NUMPROTO.
			// Utilizzato per la creazione della cartella temporanea
			// necessaria per la memorizzazione delle immagini scansionate.
			function GetDocumentID()
			{
				return document.frmProtIngresso.txtDocumentID.value;
			}
			
			// Impostazione valore check "chkConvertiPDF" nel campo nascosto "txtPDFConvert" 
			// per poter ripristinarne il valore corrente per ogni postback
			function OnCheckConvertPDF()
			{			
				EnableCheckRecognizeText();
				
				document.frmProtIngresso.txtPDFConvert.value=document.frmProtIngresso.chkConvertiPDF.checked;
			}
			
			// Abilitazione / disabilitazione check per il riconoscimento ocr
			function EnableCheckRecognizeText()
			{
    			
	    		//Verifico che la conversione lato server sia abilitata    
                //In questo caso la checkbox OCR è sempre inattiva
                var pdfConvertServer = (document.frmProtIngresso.txtPDFConvertServer.value=='true');
                
                if (document.frmProtIngresso.txtOcrSupported.value=="true" && !pdfConvertServer)
				{
					if (!document.frmProtIngresso.chkConvertiPDF.disabled)
					{
						document.frmProtIngresso.chkRecognizeText.disabled=
								!document.frmProtIngresso.chkConvertiPDF.checked;
								
						if (!document.frmProtIngresso.chkConvertiPDF.checked)
							document.frmProtIngresso.chkRecognizeText.checked=false;
						else
							document.frmProtIngresso.chkRecognizeText.checked=true;
					}				
				}
				else
				{
					document.frmProtIngresso.chkRecognizeText.disabled=true;
				}
			}
			
			// Impostazione valore di default per il check "chkConvertiPDF"
			function SetValueCheckConvertiPDF()
			{
				var	pdfConvert=(document.frmProtIngresso.txtPDFConvert.value=='true');
				
				document.frmProtIngresso.chkConvertiPDF.checked=pdfConvert;
			}
			
			// Gestione abilitazione / disabilitazione check "chkConvertiPDF"
			function SetEnabledCheckConvertiPDF()
			{
				var	pdfConvertEnabled=(document.frmProtIngresso.txtPDFConvertEnabled.value=='true');
				document.frmProtIngresso.chkConvertiPDF.disabled=!pdfConvertEnabled;
				
				//Verifico che la conversione lato server sia abilitata    
                //In questo caso la checkbox di conversione pdf è attiva
                var pdfConvertServer = (document.frmProtIngresso.txtPDFConvertServer.value=='true');
                if(pdfConvertServer)
				{
				    document.frmProtIngresso.chkConvertiPDF.disabled=!pdfConvertServer;        
				}
			}
			
			// Copia del file scannerizzato in una sottocartella a quella di scansione
			// (il cui nome è l'id del protocollo). Nella cartella vengono salvati
			// tutti i file del documento corrente per poter fare successivamente l'upload.
			function CopyAcquiredFile(filePath, isDocumentoPrincipale) {
			    var fso = FsoWrapper_CreateFsoObject();

				// Composizione cartella temporanea in cui vengono copiati
				// i file scannerizzati
				var documentsFolder = fso.GetParentFolderName(filePath) + "\\" + GetDocumentID();

				SetDocumentsFolder(documentsFolder);
				if (!fso.FolderExists(documentsFolder))
					fso.CreateFolder(documentsFolder);				

				var destinationFileName;
				// Creazione del nome del file:
				// - DOC		--> documento principale
				// - VER_###	--> altra versione del documento principale
				// - ALL_###	--> allegato al documeno principale
				if (isDocumentoPrincipale)
				{	
					if (GetCountDocuments() > 0)
						destinationFileName="VER_" + GetCountDocuments();
					else
						destinationFileName="DOC";
				}
				else
					destinationFileName="ALL_" + GetCountAttachments();
			
				// Creazione del nome del file di destinazione	
				destinationFileName=documentsFolder + "\\" + destinationFileName + "." + fso.GetExtensionName(filePath);
				
				// Copia del file
				fso.CopyFile(filePath,destinationFileName,true);
				if (fso.FileExists(filePath))
					// Cancellazione del file originale
					fso.DeleteFile(filePath,true);
			    return destinationFileName;
			}
			
			// Gestione abilitazione / disabilitazione pulsante "Protocolla".
			// Risulta attivo solo se inseriti:
			// - oggetto
			// - mittente
			// - selezionata almeno una UO per lo smistamento
			// - tipologia documento (se richiesta da web config)
			// - Selezionato almeno un modello trasmissione (se obbligatorio)
			function RefreshButtonProtocollaEnabled()
			{
			    var tipoAttoRequired = "<%=this.IsRequiredTipologiaAtto()%>";
			    var tipoAttoSelected = "<%=this.IsTipologiaDocOk()%>";
			    var UORequired = "<%=this.IsUORequired() %>";

			    var isChanged2 = false;
			    var isChanged = true; 
			    var isTipoAttoOk = false;
			    var isChanged3=false;
                
                if(tipoAttoRequired.toLowerCase() == "false" || (tipoAttoRequired.toLowerCase() == "true" && tipoAttoSelected.toLowerCase() == "true"))
			    {
			        isTipoAttoOk = true;    
			    }			    
                
                if(document.frmProtIngresso.isFascRequired.value=='true')
                {
                    if(document.frmProtIngresso.txt_CodFascicolo.value.length >0)
                    {
                        isChanged2 = true;
                    }
                    
                    if(document.frmProtIngresso.isTrasmRapidaRequired.value=='true')
                    {
                        if(document.getElementById("ddl_trasm_rapida").selectedIndex > 0)
                        {
                            isChanged3=true;
                        }
                    }

                    //Originale
                    /*isChanged =
					    (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					     document.frmProtIngresso.textOggetto.value != '' &&
					     document.frmProtIngresso.txtDescrMittente.value != '' &&
					     (AlmostOneRadioChecked() || isChanged3) && isChanged2 && isTipoAttoOk
					    );*/

                    //se è richiesta la UO
                    if (UORequired == "True") {
                        isChanged =
					    (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					     document.frmProtIngresso.textOggetto.value != '' &&
					     document.frmProtIngresso.txtDescrMittente.value != '' &&
					     (AlmostOneRadioChecked() || isChanged3) && isChanged2 && isTipoAttoOk
					    );
                    }
                    else {
                        //Se non è richiesta la UO e neanche la trasmRapida
                        if (UORequired == "False" && isChanged2 == false ) {
                            isChanged =
					        (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         document.frmProtIngresso.txtDescrMittente.value != '' && isChanged2
					        );
                        }
                        //se la trasmissione rapida è richiesta
                        else isChanged =
					       (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         document.frmProtIngresso.txtDescrMittente.value != '' && isChanged2);
                    }
                }
                else
                {
                    if(document.frmProtIngresso.isTrasmRapidaRequired.value=='true')
                    {
                        if(document.getElementById("ddl_trasm_rapida").selectedIndex > 0)
                        {
                            isChanged3=true;
                        }
                    }

                    if (UORequired == "True") {
                        isChanged =
					    (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					     document.frmProtIngresso.textOggetto.value != '' &&
					     document.frmProtIngresso.txtDescrMittente.value != '' &&
					     (AlmostOneRadioChecked() || isChanged3) && isTipoAttoOk
					);
                    }

                    else {

                        if (UORequired == "False" && isChanged3 == false) {
                            isChanged =
					       (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         document.frmProtIngresso.txtDescrMittente.value != '' &&
					         isTipoAttoOk
                         );
                        }
                        else isChanged =
					       (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         document.frmProtIngresso.txtDescrMittente.value != '' &&
					         isChanged3 && isTipoAttoOk
                             );
                    }
				}

				// Gestine abilitazione / disabilitazione pulsante protocollazione				
				EnableButtonProtocollazione(isChanged);
			}
			
			// Gestione abilitazione / disabilitazione pulsante "Protocolla" in uscita.
			// Risulta attivo solo se inseriti:
			// - oggetto
			// 
			// - selezionata almeno una UO per lo smistamento
			function RefreshButtonProtocollaUscitaEnabled()
			{
			    var tipoAttoRequired = "<%=this.IsRequiredTipologiaAtto()%>";
			    var tipoAttoSelected = "<%=this.IsTipologiaDocOk()%>";
			    var UORequired = "<%=this.IsUORequired() %>";
			    var isChanged2 = false;
			    var isChanged = true;
			    var isTipoAttoOk = false;
			    var isChanged3 = false;
                
                if(tipoAttoRequired.toLowerCase() == "false" || (tipoAttoRequired.toLowerCase() == "true" && tipoAttoSelected.toLowerCase() == "true"))
			    {
			        isTipoAttoOk = true;    
			    }			    

                if(document.frmProtIngresso.isFascRequired.value=='true')
                {
                    if(document.frmProtIngresso.txt_CodFascicolo.value.length >0)
                    {
                        isChanged2 = true;
                    }
                    
                    if(document.frmProtIngresso.isTrasmRapidaRequired.value=='true')
                    {      
                        if(document.getElementById("ddl_trasm_rapida").selectedIndex > 0)
                        {
                            isChanged3 = true;
                        }
                    }
                    //Modificato
				    /*isChanged=
					(document.frmProtIngresso.cboRegistriDisponibili.value!='' &&
					 document.frmProtIngresso.textOggetto.value!='' &&
					 AlmostOneDestinatarioIsSelect() &&
					 (AlmostOneRadioChecked() || isChanged3) && isChanged2 && isTipoAttoOk
					);*/
                    //se è richiesta la UO
                    if (UORequired == "True") {
                        isChanged =
					    (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					     document.frmProtIngresso.textOggetto.value != '' &&
					     AlmostOneDestinatarioIsSelect() &&
					     (AlmostOneRadioChecked() || isChanged3) && isChanged2 && isTipoAttoOk
					    );
                    }
                    else {
                        //Se non è richiesta la UO e neanche la trasmRapida
                        if (UORequired == "False" && isChanged2 == false) {
                            isChanged =
					        (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         AlmostOneDestinatarioIsSelect() && isTipoAttoOk && isChanged2
					        );
                        }
                        //se la trasmissione rapida è richiesta
                        else isChanged =
					       (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         AlmostOneDestinatarioIsSelect() && isTipoAttoOk && isChanged2
                             );
                    }
                }
                else
                {
                    if(document.frmProtIngresso.isTrasmRapidaRequired.value=='true')
                    {
                        if(document.getElementById("ddl_trasm_rapida").selectedIndex > 0)
                        {
                            isChanged3 = true;
                        }
                    }

                    if (UORequired == "True") {
                        isChanged =
					    (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					     document.frmProtIngresso.textOggetto.value != '' &&
					     AlmostOneDestinatarioIsSelect() &&
					     (AlmostOneRadioChecked() || isChanged3) && isTipoAttoOk
					    );
                    }
                    else {

                        if (UORequired == "False" && isChanged3 == false) {
                            isChanged =
					       (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         AlmostOneDestinatarioIsSelect() &&
					         isTipoAttoOk
                         );
                        }
                        else isChanged =
					       (document.frmProtIngresso.cboRegistriDisponibili.value != '' &&
					         document.frmProtIngresso.textOggetto.value != '' &&
					         AlmostOneDestinatarioIsSelect() &&
					         isChanged3 && isTipoAttoOk
                             );
                    }
                }
                
				// Gestine abilitazione / disabilitazione pulsante protocollazione				
				EnableButtonProtocollazione(isChanged);
			}
			
			function  AlmostOneDestinatarioIsSelect()
			{
			   var retValue=false;
			   var listDest = document.getElementById('lbx_dest');
			   if(listDest!=null)
			    {
			        if(listDest.length>0)
			            retValue = true;		            
			    }
			    return retValue;
			}
			
			// Gestione abilitazione/disabilitazione di un image button.
			// NB: da utilizzare, soprattutto per effettuare il refresh dell'immagine,
			// solo per gli imagebutton che vengono abilitati/disabilitati a livello di javascript 
			function EnableImageButton(imgButton,isDisabled,enabledImageUrl,disabledImageUrl)
			{	
				if (isDisabled)
				{
					imgButton.src=disabledImageUrl;
					imgButton.style.cursor="Default";
				}
				else
				{
					imgButton.src=enabledImageUrl;
					imgButton.style.cursor="Hand";
				}
				
				imgButton.disabled=isDisabled;
			}

			// Gestione visualizzazione maschera rubrica
			function ShowDialogRubrica()
			{
				var w_width = screen.availWidth - 40;
				var w_height = screen.availHeight - 35;
				
				var navapp = navigator.appVersion.toUpperCase();
				if ((navapp .indexOf("WIN") != -1) && (navapp .indexOf("NT 5.1") != -1))
					w_height = w_height + 20;
				
				var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
				var params = "calltype=" + Rubrica.prototype.CALLTYPE_PROTO_INGRESSO;
				
				var urlRubrica="../popup/rubrica/Rubrica.aspx"
				var res=window.showModalDialog (urlRubrica + "?" + params,'',opts);				
			}
			
		
            // Gestione visualizzazione maschera rubrica
			function ShowDialogRubricaDest()
			{
				var w_width = screen.availWidth - 40;
				var w_height = screen.availHeight - 35;
				
				var navapp = navigator.appVersion.toUpperCase();
				if ((navapp .indexOf("WIN") != -1) && (navapp .indexOf("NT 5.1") != -1))
					w_height = w_height + 20;
				
				var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
				var params = "calltype=" + Rubrica.prototype.CALLTYPE_PROTO_USCITA_SEMPLIFICATO;
				
				var urlRubrica="../popup/rubrica/Rubrica.aspx"
				var res=window.showModalDialog (urlRubrica + "?" + params,'',opts);				
			}

            // Gestione visualizzazione maschera rubrica
			function ShowDialogRubricaMittUsc()
			{
				var w_width = screen.availWidth - 40;
				var w_height = screen.availHeight - 35;
				
				var navapp = navigator.appVersion.toUpperCase();
				if ((navapp .indexOf("WIN") != -1) && (navapp .indexOf("NT 5.1") != -1))
					w_height = w_height + 20;
				
				var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
				var params = "calltype=" + Rubrica.prototype.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO;
				
				var urlRubrica="../popup/rubrica/Rubrica.aspx"
				var res=window.showModalDialog (urlRubrica + "?" + params,'',opts);
            }

            function ShowDialogRubricaMittMultipli() 
            {
                var w_width = screen.availWidth - 40;
                var w_height = screen.availHeight - 35;

                var navapp = navigator.appVersion.toUpperCase();
                if ((navapp.indexOf("WIN") != -1) && (navapp.indexOf("NT 5.1") != -1))
                    w_height = w_height + 20;

                var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
                var params = "calltype=" + Rubrica.prototype.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO;

                var urlRubrica = "../popup/rubrica/Rubrica.aspx"
                var res = window.showModalDialog(urlRubrica + "?" + params, '', opts);
            }
		
			function ShowCalendar()
			{
				return window.showModalDialog('DialogCalendario.aspx',
								'',
								'dialogWidth:230px;dialogHeight:230px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
			}

			function ShowCalendarDataProtMitt()
			{
				var retValue=ShowCalendar();
				
				if (retValue!=null && retValue!='')
				{
					document.frmProtIngresso.txtDataProtocolloMittente.value=retValue;
				}
			}
			
			function ShowCalendarDataArrivoProt()
			{
				var retValue=ShowCalendar();
				
				if (retValue!=null && retValue!='')
				{
					document.frmProtIngresso.txtDataArrivoProt.value=retValue;
				}
			}
			
			// In creazione di un nuovo protocollo, se i dati immessi
			// relativi al protocollo mittente sono già presenti,
			// viene richiesto all'utente se proseguire o meno con l'inserimento.
			// In caso affermativo, il campo nascosto "txtNewProtocolloPending"
			// viene valorizzato a true e viene fatta una postback
			function ConfirmInsertProtocollo()
			{
				var alertMsg=document.frmProtIngresso.txtMessageProtocolloEsistente.value;
				document.frmProtIngresso.txtMessageProtocolloEsistente.value='';
				
				if (confirm(alertMsg))
				{	
					document.frmProtIngresso.txtInsertProtocolloPending.value="true";
					document.frmProtIngresso.submit();
				}
			}
			
		    function InsertProtocollo()
			{
				var alertMsg=document.frmProtIngresso.txtMessageProtocolloEsistente.value;
				document.frmProtIngresso.txtMessageProtocolloEsistente.value='';
			    document.frmProtIngresso.txtInsertProtocolloPending.value="true";
			    document.frmProtIngresso.submit();
			}

		    function CloseProtocollo()
			{
				var alertMsg=document.frmProtIngresso.txtMessageProtocolloEsistente.value;
				document.frmProtIngresso.txtMessageProtocolloEsistente.value='';
			    document.frmProtIngresso.txtCloseAnteprimaProf.value="true";
			    document.frmProtIngresso.submit();
			}

		    //Apre la PopUp Modale per la ricerca dei fascicoli
		    //utile per la fascicolazione rapida
		    function ApriRicercaFascicoli(codiceClassifica) 
		    {
			    var newUrl;
    		    newUrl="../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica+"&caller=protocollo";
    		    var newLeft=(screen.availWidth-615);
			    var newTop=(screen.availHeight-710);	
    	        // apertura della ModalDialog
			    rtnValue = window.showModalDialog(newUrl,"","dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
			    if (rtnValue == "Y")
			    {
				    window.document.frmProtIngresso.submit();
			    }
		    }
		    
		    
			// INIZIO --------------------------------
			// GESTIONE NECESSARIA PER INTERCETTARE LA CHIUSURA 
			// DELLA PAGINA CON LA "X" DEL BROWSER
/*
			function AttachDocumentEvents()
			{
				// Viene fatto l'attach degli eventi principali 
				// relativi al documento stesso alla funzione "OnDocumentEvent"
				document.onclick=OnDocumentEvent;
				document.onkeydown=OnDocumentEvent;
				document.onkeypress=OnDocumentEvent;
			}
			
			function OnDocumentEvent()
			{			
				// Ogni volta che viene generato un evento del documento,
				// nel campo hiddentext "txtClientEventTarget" viene memorizzato
				// il valore "OnDocumentEvent"
				document.frmProtIngresso.txtClientEventTarget.value="OnDocumentEvent";
			}
						
			function OnUnload()
			{ 
				// Alla deallocazione dell'oggetto documento, viene
				// verificato il valore del campo hidden "txtClientEventTarget".
				// Se null, significa che la pagina è stata chiusa mediante
				// la "X" del browser, quindi viene fatta la submit della pagina
				// impostando il valore "true" nel campo nascosto "txtUnloadMode".
				if(document.frmProtIngresso.txtClientEventTarget.value=='')
				{
					document.frmProtIngresso.txtUnloadMode.value="true";
					document.frmProtIngresso.submit();
				}
			}
*/
			// FINE --------------------------------
            
            function AvvisoVisibilita()
            {
                var newLeft=(screen.availWidth-500);
		        var newTop=(screen.availHeight-500);
		        var newUrl;
		        newUrl="../popup/estensioneVisibilita.aspx";
    		    
	            if(IsCheckBoxRequired() && document.getElementById("abilitaModaleVis").value == "true")
	            {
	                retValue=window.showModalDialog(newUrl,"","dialogWidth:306px;dialogHeight:188px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:yes;help:no;");
    	            
	                if(retValue == 'NO')
	                {
	                    document.getElementById("estendiVisibilita").value = "true";
	                }
	                else
	                {
	                    document.getElementById("estendiVisibilita").value = "false";
	                }
	            }
            }

            function IsCheckBoxRequired() {
                if (document.getElementById("chkPrivato") != null && document.getElementById("chkPrivato").checked == true) {
                    return true;
                }
                else {
                    return false;
                }
            }
            
            function ChiudiPag()
            {
                document.getElementById("isInterno").value = "chiuso";
                return true;
            }
            function ApriTitolario2(qrstr,isFasc) 
            {
	            var newUrl;
            	
	            newUrl="../popup/titolario.aspx?"+qrstr+"&isFasc="+isFasc;
            	
	            var newLeft=(screen.availWidth-650);
	            var newTop=(screen.availHeight-740);	
            	
	            // apertura della ModalDialog
	            rtnValue = window.showModalDialog(newUrl,"","dialogWidth:650px;dialogHeight:720px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
	            if (rtnValue != "N")
	            {
	             window.document.frmProtIngresso.submit();   
	            }
	        }

	     // Visualizzazione maschera per l'acquisizione dei documenti			
	        function ShowAcquire() {
	            var ret = ApriAcquisisciDocumento();

	         if (Boolean(ret)) {
	             frmProtIngresso.submit();
	             
             }
	         return Boolean(ret);
	     }

	     //Acquisizione documento 
	     function AcquireDocument() {
	         
	         var retValue = false;
	         retValue = ShowAcquire();
	         if (retValue) {
	             // Impostazione dello stato del protocollo come "ProtocollatoAcquisito"
	             SetStatoProtocollatoAcquisito();
	             // Se il documento è stato acquisito correttamente,viene incrementato il contatore del doc principale 					
	             IncrementCounterDocuments();
	             
	             
	         }

	         return retValue;

	     }

         function AcquireAttach(){
             var retValue = false;
             retValue = ShowAcquire();
             if (retValue) {
                 IncrementCounterAttachments();
             }

             return retValue;

         }

         function onClickNuovoAllegato() {
             var oldcursor = window.document.body.style.cursor;
             window.document.body.style.cursor = 'wait';

             var ret = showMaskAllegato("");

             if (!ret)
                 window.document.body.style.cursor = oldcursor;

             return ret;
         }


         function showMaskAllegato(versionId) {
             var url = "../popup/gestioneAllegato.aspx";
             if (versionId != "")
                 url = url + "?versionId=" + versionId;

             versionId = window.showModalDialog(url, null, 'dialogWidth:408px;dialogHeight:180px;status:no;resizable:no;scroll:no;help:no;close:no');
             //document.getElementById("txtSavedVersionId").value = versionId;
             return (versionId != null);
         }

         function ApriFinestraMultiCorrispondenti() {
             var rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M&page=ricerca&corrId=protIngr', '', 'dialogWidth:730px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
             window.document.frmProtIngresso.submit();
         }

		</script>

        <link href="../CSS/docspa_30.css" rel="stylesheet" type="text/css" />
        <style>

        .box_item{
            background-color:#fafafa;
            padding:2px;
        }
        </style>

        <style type="text/css">
    .autocomplete_completionListElementbis
    {
        height: 280px;
        list-style-type: none;
        margin: 0px;
        padding: 0px;
        font-size: 10px;
        color: #333333;
        line-height: 18px;
        border: 1px solid #333333;
        overflow: auto;
        padding-left: 5px;
        background-color: #ffffff;
        font-family: Verdana, Arial, sans-serif;
        z-index : 1004;
    }
    
    .single_itembis
    {
        border-bottom: 1px dashed #cccccc;
        padding-top: 2px;
        padding-bottom: 2px;
    }
    
    .single_item_hoverbis
    {
        border-bottom: 1px dashed #cccccc;
        background-color: #9d9e9c;
        color: #000000;
        padding-top: 2px;
        padding-bottom: 2px;
    }

    .selectedWord{
        font-weight:bold;
        color:#000000;
        text-decoration:underline;
    }

</style>
	</HEAD>
	<body bottomMargin="1" leftMargin="1" topMargin="1" rightMargin="1" MS_POSITIONING="GridLayout">
		<form id="frmProtIngresso" method="post" target="DialogProtocollazioneIngresso" runat="server">
        <asp:ScriptManager ID="ScriptManager" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="abilitaModaleVis" runat="server" />
        <asp:HiddenField ID="estendiVisibilita" runat="server" />
        <asp:HiddenField ID="isInterno" runat="server" />
        <asp:HiddenField ID="appoIdMod" runat="server" />
        <asp:HiddenField ID="appoIdAmm" runat="server" />
        <!-- TextBox di appoggio allo user control Oggetto per poter gestire il controllo sulla protocollazione -->
        <asp:TextBox ID="textOggetto" runat="server" Visible="true" Width="0" Height="0"></asp:TextBox>
		<!-- Fine aggiunta per user control ctrl_oggetto ********************************************* -->
		    <uc2:SupportedFileTypeController id="supportedFileTypeController" runat="server" />
			<INPUT id="txtCountDocument" type="hidden" size="1" value="0" name="txtCountDocument" runat="server">
			<input id="isFascRequired" type="hidden" name="isFascRequired" runat="server" />
			<INPUT id="txtCountAttachment" type="hidden" size="1" value="0" name="txtCountAttatchment" runat="server"> 
			<INPUT id="txtStatoDocumento" type="hidden" size="1" name="txtStatoDocumento" runat="server">
			<INPUT id="txtModAcquisizione" type="hidden" size="1" name="txtModAcquisizione" runat="server">
			<input id="txtDocumentsFolder" style="WIDTH: 40px; HEIGHT: 22px" type="hidden" size="1" name="convertiPDF" runat="server">&nbsp; 
			<INPUT id="txtUnloadMode" style="WIDTH: 32px; HEIGHT: 22px" type="hidden" size="1" name="txtUnloadMode" runat="server">
			<uc1:accessonegato id="AccessoNegato" runat="server" Visible="False"></uc1:accessonegato>
			<INPUT id="txtDocumentID" style="WIDTH: 40px; HEIGHT: 22px" type="hidden" size="1" name="txtDocumentID" runat="server">&nbsp; 
			<INPUT id="txtPDFConvert" style="WIDTH: 37px; HEIGHT: 22px" type="hidden" size="1" value="false" name="txtPDFConvert" runat="server">
			<INPUT id="txtPDFConvertEnabled" style="WIDTH: 37px; HEIGHT: 22px" type="hidden" size="1" value="false" name="txtPDFConvertEnabled" runat="server"> 
			<INPUT id="txtPDFConvertServer" style="WIDTH: 37px; HEIGHT: 22px" type="hidden" size="1" value="false" name="txtPDFConvertServer" runat="server"> 
			<INPUT id="txtInsertProtocolloPending" style="WIDTH: 37px; HEIGHT: 22px" type="hidden" size="1" value="false" name="txtInsertProtocolloPending" runat="server">&nbsp;
			<INPUT id="txtCloseAnteprimaProf" style="WIDTH: 37px; HEIGHT: 22px" type="hidden" size="1" value="false" name="txtInsertProtocolloPending" runat="server">&nbsp;
			<INPUT id="txtMessageProtocolloEsistente" style="WIDTH: 48px; HEIGHT: 22px" type="hidden" size="1" name="txtMessageProtocolloEsistente" runat="server"> 
			<INPUT id="txtSmartClientActivation" type="hidden" size="1" value="false" name="txtSmartClientActivation" runat="server"> 
			<INPUT id="txtOcrSupported" type="hidden" size="1" value="false" name="txtOcrSupported" runat="server">
			<input id="isTrasmRapidaRequired" type="hidden" name="isTrasmRapidaRequired" runat="server" />
			<uc6:FsoWrapper id="fsoWrapper" runat="server" />
            <uc3:StampaEtichetta id="StampaEtichetta" runat="server"></uc3:StampaEtichetta>
            

			<table bgcolor="#eaeaea" id="tblContainer" height="160" cellSpacing="0" cellPadding="4" width="750" align="center" runat="server" style="border:1px solid #990000">
				<tr>
					<td class="titolo_scheda" valign="top">
						<table id="tblRegistri" cellSpacing="0" cellPadding="0" width="100%" align="center">							
                            <tr class="box_item">
								<td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblRegistriDisponibili" runat="server" Width="100%">Registro</asp:label></td>
								<td class="testo_grigio" width="37%" style="border-bottom:2px solid #eaeaea"><asp:dropdownlist id="cboRegistriDisponibili" runat="server" Width="100%" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist></td>
								<td class="titolo_scheda" align="right" width="5%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblStato" runat="server" Width="100%">Stato:</asp:label></td>
								<td class="testo_grigio" width="7%" style="border-bottom:2px solid #eaeaea"><asp:Image id="imgStatoRegistro" runat="server"></asp:Image></td>
								<td class="titolo_scheda" align="right" width="6%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblNumProtocollo" runat="server" Width="100%" style="white-space: nowrap;">Num. Prot.</asp:label></td>
								<td class="testo_grigio" align="left" width="12%" style="border-bottom:2px solid #eaeaea"><asp:textbox id="txtNumProtocollo" runat="server" Width="100%" CssClass="testo_grigio" ReadOnly="True"></asp:textbox></td>
								<td class="titolo_scheda" align="right" width="6%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblDataProtocollo" runat="server" Width="100%" style="white-space: nowrap;">Data Prot.</asp:label></td>
								<td class="testo_grigio" align="left" style="width: 12%" style="border-bottom:2px solid #eaeaea"><cc2:datemask id="txtDataProtocollo" runat="server" Width="100%" CssClass="testo_grigio" ReadOnly="True"></cc2:datemask></td>
                                <td style="border-bottom:2px solid #eaeaea"><asp:CheckBox id="chkPrivato" Text="Privato" textAlign="Left" runat="server" CssClass="testo_grigio_scuro" Checked="False" tooltip="Documento creato con visibilità limitata al solo ruolo e utente proprietario" AutoPostBack="True" /></td>
                            </tr>
							<tr class="box_item">
								<td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblSegnatura" runat="server" width="100%">Segnatura</asp:label></td>
								<td class="testo_grigio" colSpan="6" style="border-bottom:2px solid #eaeaea"><asp:textbox id="txtSegnatura" runat="server" Width="100%" CssClass="testo_segnatura" ReadOnly="True"></asp:textbox></td>
                                <td class="testo_grigio" style="border-bottom:2px solid #eaeaea"><asp:textbox id="txt_num_stampe" runat="server" Width="100%" CssClass="testo_grigio" MaxLength="3" Text="1"></asp:textbox></td>
								<td class="testo_grigio" style="border-bottom:2px solid #eaeaea"><cc1:imagebutton id="btnPrintSignature" Width="16px" Runat="server" AlternateText="Stampa" DisabledUrl="../images/proto/stampa.gif" Tipologia="DO_PROT_SE_STAMPA" ImageUrl="../images/proto/stampa.gif" height="16px"></cc1:imagebutton></td>
							</tr>
							<tr class="box_item">
								<td class="titolo_scheda" width="13%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblOggetto" runat="server" width="100%">Oggetto *</asp:label></td>
								<td class="testo_grigio" colSpan="7" align="left" style="border-bottom:2px solid #eaeaea">
								    <uc5:Oggetto ID="ctrl_oggetto" runat="server"/>
								</td>
								<td align="center" style="border-bottom:2px solid #eaeaea">
									<table id="TABLE1" cellSpacing="0" cellPadding="2" width="100%" align="center" border="0" >
							            <tr>
										    <!-- Aggiunta del bottone per il correttore ortografico -->
						                    <td><cc1:imagebutton ID="btn_Correttore" runat="server" Width="19px" AlternateText="Correttore ortografico"
						                                DisabledUrl="../images/proto/check_spell.gif"  Height="17" Visible="false"
						                                ImageUrl="../images/proto/check_spell.gif" Enabled="false" onclick="btn_Correttore_Click" ></cc1:imagebutton></td>
						                    <!-- fine aggiunta -->
										</tr>
										<tr>
										    <td><cc1:imagebutton id="btn_RubrOgget_P" Runat="server" Width="19px" AlternateText="Seleziona un  oggetto nell'oggettario"
														        DisabledUrl="../images/proto/ico_oggettario.gif" Tipologia="DO_PROT_OG_OGGETTARIO" Height="17"
																ImageUrl="../images/proto/ico_oggettario.gif" ></cc1:imagebutton></td>
										</tr>
									</table>
								</td>
							</tr>
							<asp:Panel ID="pnlProtoIngressoSemplificato" runat="server">
                            <ut8:RicercaVeloce id="rubrica_veloce" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged" CALLTYPE_RUBRICA_VELOCE="CALLTYPE_PROTO_INGRESSO"></ut8:RicercaVeloce>
							<tr class="box_item">
								<td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea">
                                <asp:label id="lblMittente" runat="server" width="100%">Mittente *</asp:label>
                                </td>
								<td class="testo_grigio" colSpan="7" valign="middle" style="border-bottom:2px solid #eaeaea">
								    <asp:textbox id="txtCodMittente" runat="server" Width="15%" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>
								    <asp:textbox id="txtDescrMittente" runat="server" Width="491px" CssClass="testo_grigio"></asp:textbox>
								     <asp:Panel ID="pnl_new_mittente_semplificato_ingresso_veloce" runat="server" Visible="false">
                                     <script type="text/javascript">
                                         function acePopulated(sender, e) {
                                             var behavior = $find('AutoCompleteExIngresso');
                                             var target = behavior.get_completionList();
                                             if (behavior._currentPrefix != null) {
                                                 var prefix = behavior._currentPrefix.toLowerCase();
                                                 var i;
                                                 for (i = 0; i < target.childNodes.length; i++) {
                                                     var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                                     if (sValue.indexOf(prefix) != -1) {
                                                         var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                                         var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                                         var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                                         target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                                     }
                                                 }
                                             }
                                         }

                                         function aceSelected(sender, e) {
                                             var value = e.get_value();

                                             if (!value) {

                                                 if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                                     value = e._item.parentElement.attributes["_value"].value;

                                                 else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                                     value = e._item.parentElement.parentElement.attributes["_value"].value;

                                                 else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                                     value = e._item.parentNode._value;

                                                 else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                                     value = e._item.parentNode.parentNode._value;

                                                 else value = "";

                                             }

                                             var searchText = $get('<%=txtDescrMittente.ClientID %>').value;
                                             searchText = searchText.replace('null', '');
                                             var testo = value;
                                             var indiceFineCodice = testo.lastIndexOf(')');
                                             document.getElementById("<%=txtDescrMittente.ClientID%>").focus();
                                             document.getElementById("<%=txtDescrMittente.ClientID%>").value = "";
                                             var indiceDescrizione = testo.lastIndexOf('(');
                                             var descrizione = testo.substr(0, indiceDescrizione - 1);
                                             var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                                             document.getElementById('txtCodMittente').value = codice;
                                             document.getElementById('txtDescrMittente').value = descrizione;
                                             setTimeout('__doPostBack(\'txtCodMittente\',\'\')', 0);
                                         } 
                                 </script>
                                <cc3:AutoCompleteExtender runat="server" ID="new_mitt_veloce_sempl_ing" TargetControlID="txtDescrMittente"
                                    CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                                    CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaCorrispondentiVeloce"
                                    MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngresso" OnClientPopulated="acePopulated">
                                </cc3:AutoCompleteExtender>
                                    
                                    
                                    
                                    </asp:Panel>
								</td>
								<td width="16px" style="border-bottom:2px solid #eaeaea"><cc1:imagebutton id="btnShowRubrica" Width="29px" Runat="server" AlternateText="Rubrica" DisabledUrl="Images/rubrica.gif" Tipologia="" ImageUrl="Images/rubrica.gif" height="19px"></cc1:imagebutton></td>
							</tr>
                            <!--Mittenti Multipli-->
                            <asp:Panel id="panel_DettaglioMittenti" Runat="server" Visible="false">
                            <ut8:RicercaVeloce id="rubrica_mitt_multiplo_semplificato" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged" CALLTYPE_RUBRICA_VELOCE="CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO"></ut8:RicercaVeloce>
                                <asp:HiddenField ID="txt_cod_mitt_mult_nascosto" runat="server"/>
                                <asp:HiddenField ID="txt_desc_mitt_mult_nascosto" runat="server" />
                                <asp:Button id="btn_nascosto_mitt_multipli" runat="server" style="display: none;"/>
                                <tr class="box_item">
                                    <td class="titolo_scheda" width="14%">Mitt. Multipli</td>
                                    <td class="testo_grigio" colSpan="7" align="center">
                                        <%--<asp:textbox id="txt_codMittMultiplo" AutoPostBack="True" Width="15%" Runat="server" CssClass="testo_grigio"></asp:textbox>
									    <asp:textbox id="txt_descMittMultiplo" AutoComplete="off" Width="79%" Runat="server" CssClass="testo_grigio"></asp:textbox>--%>
                                        <cc1:ImageButton id="btn_downMittente" runat="server" AlternateText="Inserisci Mittente multiplo" DisabledUrl="../images/proto/ico_freccia_giu.gif" ImageUrl="../images/proto/ico_freccia_giu.gif"></cc1:ImageButton>
                                        &nbsp;
									    <cc1:ImageButton id="btn_upMittente" runat="server" AlternateText="Inserisci Mittente" DisabledUrl="../images/proto/ico_freccia_su.gif" ImageUrl="../images/proto/ico_freccia_su.gif"></cc1:ImageButton>
                                    </td>
                                    <td>
                                        <%--<cc1:ImageButton id="btn_insMittMultiplo" runat="server" AlternateText="Inserisci mittente" DisabledUrl="../images/proto/aggiungi.gif" ImageUrl="../images/proto/aggiungi.gif"></cc1:ImageButton>--%>
                                    </td>
                                </tr>
                                <tr class="box_item">
                                    <td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea"></td>   
                                    <td class="testo_grigio" colSpan="7" valign="middle" style="border-bottom:2px solid #eaeaea">
                                        <asp:ListBox id="lbx_mittMultiplo" runat="server" CssClass="testo_grigio" width="100%" Rows="2"></asp:ListBox>
                                    </td>
                                    <td align="left" style="border-bottom:2px solid #eaeaea">
                                        <cc1:imagebutton id="btn_RubrMittMultiplo" height="19px" alt="Seleziona mittente nella rubrica" src="../images/proto/rubrica.gif" Width="29px" runat="server"></cc1:imagebutton>
                                        <cc1:imagebutton id="btn_CancMittMultiplo" Width="19" Runat="server" AlternateText="Cancella Mittente" DisabledUrl="../images/proto/cancella.gif" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton>
                                    </td>                                                                         
                                </tr>
							</asp:Panel>
                            <tr class="box_item">
								<td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea"><asp:label id="lblDescrProtMittente" runat="server" width="100%" style="white-space: nowrap;">Prot. Mitt.</asp:label></td>
								<td class="testo_grigio" colspan="2" style="border-bottom:2px solid #eaeaea"><asp:textbox id="txtDescrProtMittente" runat="server" Width="100%" CssClass="testo_grigio"></asp:textbox></td>
								<td class="titolo_scheda" vAlign="middle" width="6%" colspan="3" style="border-bottom:2px solid #eaeaea"><asp:label id="lblDataProtocolloMittente" runat="server">Data Prot.</asp:label><uc3:Calendario id="txtDataProtocolloMittente" runat="server" /></td>
								<TD class="titolo_scheda" vAlign="middle" colspan="3" style="border-bottom:2px solid #eaeaea"><asp:label id="lblDataArrivoProt" runat="server">Data Arrivo</asp:label><uc3:Calendario id="txtDataArrivoProt" runat="server"></uc3:Calendario></td>
							</tr>
                            </asp:Panel>
							<asp:Panel ID="pnlProtoUscitaSemplificato" runat="server">
                            <ut8:RicercaVeloce id="rubrica_veloce_destinatario_mittente_uscita" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged" CALLTYPE_RUBRICA_VELOCE="CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO"></ut8:RicercaVeloce> 
							<tr class="box_item">
								<td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea">
                                    <asp:label id="lblMittUsc" runat="server" width="100%">Mittente:</asp:label>
                                </td>
								<td class="testo_grigio" colSpan="7" valign="middle" style="border-bottom:2px solid #eaeaea">
                                     <asp:textbox id="txtCodMittUsc" runat="server" Width="15%" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>
								    <asp:textbox id="txtDescMittUsc" runat="server" Width="491px" CssClass="testo_grigio" ReadOnly="true"></asp:textbox>
                                    <asp:Panel runat="server" ID="pnl_new_mittente_uscita_semplificato" Visible="false">
                                     <script type="text/javascript">
                                         function acePopulated(sender, e) {
                                             var behavior = $find('AutoCompleteExIngresso');
                                             var target = behavior.get_completionList();
                                             if (behavior._currentPrefix != null) {
                                                 var prefix = behavior._currentPrefix.toLowerCase();
                                                 var i;
                                                 for (i = 0; i < target.childNodes.length; i++) {
                                                     var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                                     if (sValue.indexOf(prefix) != -1) {
                                                         var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                                         var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                                         var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                                         target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                                     }
                                                 }
                                             }
                                         }

                                         function aceSelected(sender, e) {
                                             var value = e.get_value();

                                             if (!value) {

                                                 if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                                     value = e._item.parentElement.attributes["_value"].value;

                                                 else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                                     value = e._item.parentElement.parentElement.attributes["_value"].value;

                                                 else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                                     value = e._item.parentNode._value;

                                                 else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                                     value = e._item.parentNode.parentNode._value;

                                                 else value = "";

                                             }

                                             var searchText = $get('<%=txtDescMittUsc.ClientID %>').value;
                                             searchText = searchText.replace('null', '');
                                             var testo = value;
                                             var indiceFineCodice = testo.lastIndexOf(')');
                                             document.getElementById("<%=txtDescMittUsc.ClientID%>").focus();
                                             document.getElementById("<%=txtDescMittUsc.ClientID%>").value = "";
                                             var indiceDescrizione = testo.lastIndexOf('(');
                                             var descrizione = testo.substr(0, indiceDescrizione - 1);
                                             var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                                             document.getElementById('txtCodMittUsc').value = codice;
                                             document.getElementById('txtDescMittUsc').value = descrizione;
                                             setTimeout('__doPostBack(\'txtCodMittUsc\',\'\')', 0);

                                         } 


            </script>
                <cc3:AutoCompleteExtender runat="server" ID="new_mitt_veloce_sempl_usc" TargetControlID="txtDescMittUsc"
                    CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                    CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaCorrispondentiVeloce"
                    MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                    UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngresso" OnClientPopulated="acePopulated">
                </cc3:AutoCompleteExtender>
            </asp:Panel>
								</td>
								<td align="left" style="border-bottom:2px solid #eaeaea">
                                <cc1:imagebutton id="btnShowRubricaMittUsc" Width="29px" Runat="server" AlternateText="Rubrica" DisabledUrl="Images/rubrica.gif"
										Tipologia="" ImageUrl="Images/rubrica.gif" height="19px"></cc1:imagebutton>
								</td>
							</tr>   
                            <ut8:RicercaVeloce id="rubrica_veloce_destinatario" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged" CALLTYPE_RUBRICA_VELOCE="CALLTYPE_PROTO_USCITA_SEMPLIFICATO"></ut8:RicercaVeloce>                        
                            <tr class="box_item">
								<td class="titolo_scheda" width="14%">
                                     <asp:label id="lblDest" runat="server" width="100%">Destinatari: *</asp:label></td>
								<td class="testo_grigio" colSpan="7">
                                    <asp:textbox id="txtCodDest" runat="server" Width="15%" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>
								    <asp:textbox id="txtDescrDest" runat="server" Width="491px" CssClass="testo_grigio"></asp:textbox>
                                    <script type="text/javascript">
                                        function ItemSelectedDestinatario(sender, e) {
                                            var value = e.get_value();

                                            if (!value) {

                                                if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                                    value = e._item.parentElement.attributes["_value"].value;

                                                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                                    value = e._item.parentElement.parentElement.attributes["_value"].value;

                                                else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                                    value = e._item.parentNode._value;

                                                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                                    value = e._item.parentNode.parentNode._value;

                                                else value = "";

                                            }

                                            var searchText = $get('<%=txtDescrDest.ClientID %>').value;
                                            searchText = searchText.replace('null', '');
                                            var testo = value;
                                            var indiceFineCodice = testo.lastIndexOf(')');
                                            document.getElementById("<%=txtDescrDest.ClientID%>").focus();
                                            document.getElementById("<%=txtDescrDest.ClientID%>").value = "";
                                            var indiceDescrizione = testo.lastIndexOf('(');
                                            var descrizione = testo.substr(0, indiceDescrizione - 1);
                                            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                                            document.getElementById('txtCodDest').value = codice;
                                            document.getElementById('txtDescrDest').value = descrizione;
                                            document.all("btn_aggiungiDest_P").click();
                                        }

                                        function acePopulatedDest(sender, e) {
                                            var behavior = $find('AutoCompleteExDestinatari');
                                            var target = behavior.get_completionList();
                                            if (behavior._currentPrefix != null) {
                                                var prefix = behavior._currentPrefix.toLowerCase();
                                                var i;
                                                for (i = 0; i < target.childNodes.length; i++) {
                                                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                                    if (sValue.indexOf(prefix) != -1) {
                                                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                                    }
                                                }
                                            }
                                        }
            </script>
                        <asp:Panel ID="pnl_new_destinatario_uscita_semplificato" Visible="false" runat="server">
                            <cc3:AutoCompleteExtender runat="server" ID="new_dest_veloce_sempl_usc" TargetControlID="txtDescrDest"
                                CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                                CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaCorrispondentiVeloce"
                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                UseContextKey="true" OnClientItemSelected="ItemSelectedDestinatario" BehaviorID="AutoCompleteExDestinatari" OnClientPopulated="acePopulatedDest">
                            </cc3:AutoCompleteExtender>
                        </asp:Panel>
                               </td>
								<td align="left">
                                    <cc1:imagebutton id="btnShowRubricaDest" Width="29px" Runat="server" AlternateText="Rubrica" DisabledUrl="Images/rubrica.gif"
										Tipologia="" ImageUrl="Images/rubrica.gif" height="19px"></cc1:imagebutton>
                                    <cc1:imagebutton id="btn_aggiungiDest_P" Width="18" Runat="server" AlternateText="Aggiungi" DisabledUrl="../images/proto/aggiungi.gif"
								    Tipologia="" Height="17" ImageUrl="../images/proto/aggiungi.gif" ></cc1:imagebutton>
								</td>
							</tr>
							<tr class="box_item">
								<td class="titolo_scheda" width="14%"></td>
								<td  colSpan="7"><asp:ListBox id="lbx_dest" runat="server" Width="100%" CssClass="testo_grigio" Rows="2"></asp:ListBox></td>
								<td align="left">
									<cc1:imagebutton id="btn_cancDest" Width="19" Runat="server" AlternateText="Cancella" DisabledUrl="../images/proto/cancella.gif"
										Tipologia="DO_OUT_DES_ELIMINA" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton>
								</td>
							</tr>
							<tr class="box_item">
        					    <td class="testo_grigio" align=center colspan=9>
							        <cc1:ImageButton id="btn_insDestCC" runat="server" AlternateText="Inserisci tra i destinatari per Conoscenza"
								        DisabledUrl="../images/proto/ico_freccia_giu.gif" ImageUrl="../images/proto/ico_freccia_giu.gif"></cc1:ImageButton>&nbsp;
							        <cc1:ImageButton id="btn_insDest" runat="server" AlternateText="Inserisci tra i destinatari" DisabledUrl="../images/proto/ico_freccia_su.gif"
								        ImageUrl="../images/proto/ico_freccia_su.gif"></cc1:ImageButton>
								</td>
					        </tr>
							<tr class="box_item">
								<td class="titolo_scheda" width="14%" style="border-bottom:2px solid #eaeaea">
                                     <asp:label id="lblDescCC" runat="server" width="100%">Destinatari CC:</asp:label></td>
						        <td  colSpan="7" style="border-bottom:2px solid #eaeaea">
                                     <asp:ListBox id="lbx_destCC" runat="server" Width="100%" CssClass="testo_grigio" Rows="2"></asp:ListBox></td>
								<td align="left" style="border-bottom:2px solid #eaeaea">
									<cc1:imagebutton id="btn_cancDestCC" Width="19" Runat="server" AlternateText="Cancella" DisabledUrl="../images/proto/cancella.gif"
										Tipologia="DO_OUT_DES_ELIMINA" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton>
								</td>
							</tr>
							</asp:Panel>
							<asp:panel id="pnl_fasc_rapida" Runat="server" Visible="True">
							<tr class="box_item">
								<td class="titolo_scheda" width="13%" style="border-bottom:2px solid #eaeaea">
                                <asp:label id="labelFascRapid" runat="server" width="100%" style="white-space: nowrap;"></asp:label></td>
								<td class="testo_grigio" colSpan="7" valign="middle" style="border-bottom:2px solid #eaeaea">
								    <asp:textbox id="txt_CodFascicolo" runat="server" Width="15%" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>
								    <asp:textbox id="txt_DescFascicolo" runat="server" Width="491px" CssClass="testo_grigio"></asp:textbox>
								</td>
								<td align="left" style="border-bottom:2px solid #eaeaea">
								    <cc1:imagebutton id="btn_titolario" Runat="server" ImageUrl="../images/proto/ico_titolario_noattivo.gif" AlternateText="Titolario" Tipologia="DO_CLA_TITOLARIO" DisabledUrl="../images/proto/ico_titolario_noattivo.gif" OnClick="btn_titolario_Click"></cc1:imagebutton>
								    <cc1:imagebutton id="imgFasc" Runat="server" AlternateText="Ricerca Fascicoli" DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/proto/ico_fascicolo_noattivo.gif"></cc1:imagebutton>
								     <cc1:imagebutton id="imgFascNew" Runat="server" Visible="false" AlternateText="Nuovo Fascicolo" DisabledUrl="../images/fasc/fasc_direct.gif" Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/fasc/fasc_direct.gif"></cc1:imagebutton>
                                </td>								    
							</tr>
                            </asp:panel>
						    <asp:Panel ID="pnl_trasm_rapida" runat="server" Visible="true">
						    <tr class="box_item">
						        <td class="titolo_scheda" width="13%" style="border-bottom:2px solid #eaeaea">
						        <asp:Label ID="lbl_tras_rapida" runat="server" style="white-space: nowrap;">Trasm. Rapida:</asp:Label>
						        </td>
						        <td class="testo_grigio" colspan="8" style="border-bottom:2px solid #eaeaea">
                                    <asp:DropDownList CssClass="testo_grigio" id="ddl_trasm_rapida" tabIndex="420" runat="server" AutoPostBack="True" Width="350px"></asp:DropDownList>
						        </td>
						    </tr>
						    </asp:Panel>
						    <!-- pannello tipologia documento -->
						    <asp:Panel ID="pnl_tipologia_doc" runat="server" Visible="false">
						    <tr class="box_item">
								<td class="titolo_scheda" valign="middle" width="13%" style="border-bottom:2px solid #eaeaea">Tipologia:&nbsp;<asp:Label ID="star" runat="server" Visible="false" Text="*"></asp:Label></td>
								<td class="titolo_scheda" valign="middle" height="25" colspan="8" style="border-bottom:2px solid #eaeaea">
								    <table border="0" class="titolo_scheda" cellSpacing="0" cellPadding="0">
                                        <tr>
                                            <td align="left">
									            <asp:dropdownlist id="ddl_tipoAtto" runat="server" Width="200px" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist>
									            <asp:Label ID="unicaTipoAtto" runat="server" Visible="false"></asp:Label>
									            <asp:HiddenField ID="unicoCodTipoAtto" runat="server" Visible="false" />
								                <asp:imagebutton id="btn_CampiPersonalizzati" runat="server" Width="18px" AlternateText="Visualizza campi tipologia" Height="17px" ImageUrl="../images/proto/ico_oggettario.gif" OnClick="btn_CampiPersonalizzati_Click"></asp:imagebutton>
								            </td>
								            <td>
										        <asp:panel id="pnl_DiagrammiStato" Runat="server" Visible="false">
											        <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0"/>Stato :<img height="1" src="../images/proto/spaziatore.gif" width="8" border="0"/>
											        <asp:Label id="lbl_statoAttuale" runat="server" CssClass="titolo_scheda" Visible="False"></asp:Label>
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
											        <asp:DropDownList id="ddl_statiSuccessivi" runat="server" Width="150px" CssClass="testo_grigio"></asp:DropDownList>
										        </asp:panel>
								            </td>
								            <td>
										        <asp:panel id="pnl_DataScadenza" Runat="server" Visible="false" >
										            <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"/>Data Scadenza :
										            <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"/>
											        <cc1:DateMask id="txt_dataScadenza" runat="server" CssClass="testo_grigio" Width="75px"></cc1:DateMask>						   								    									    									
										        </asp:panel>
								            </td>
                                        </tr>
                                    </table>
							    </td>
						    </tr>
						    </asp:Panel>
						    <asp:Panel ID="pnl_note" runat="server" Visible="false" >
						    <tr class="box_item">
						        <td colspan="9"><uc4:DettaglioNota ID="dettaglioNota" runat="server" width="95%" TipoOggetto = "Documento" ContainerSessionKey = "ProtocollazioneIngresso.ProtocolloCorrente" Rows="3" TextMode="MultiLine" PAGINA_CHIAMANTE="Protocollazione"/></td>
						    </tr>
						    </asp:Panel>
						</table>
					</td>
				</tr>
				<tr class="box_item">
					<td class="testo_grigio_scuro" align="center">
						<table id="tblInsertButtonsContainer" cellSpacing="0" cellPadding="2" width="40%" align="center" border="0" runat="server">
						    <tr>
                                 <td colspan="7">
                                 <img height="2" src="../images/proto/spaziatore.gif" width="8" border="0">
                                </td>
                            </tr>
							<tr vAlign="middle">
								<td class="testo_grigio_scuro" vAlign="middle" align="center">Nuovo protocollo</TD>
								<td class="testo_grigio_scuro" vAlign="middle" align="center">
								    <cc1:imagebutton id="btnNuovoProtocollo" Width="20px" Runat="server" AlternateText="Nuovo protocollo"
										DisabledUrl="../App_Themes/ImgComuni/new_proto_disabled.gif" Tipologia="" Thema="new_" SkinID="proto" height="18px"></cc1:imagebutton></TD>
								<td class="testo_grigio_scuro" vAlign="middle" align="center">Riproponi</TD>
								<td class="testo_grigio_scuro" vAlign="middle" align="center">
								    <cc1:imagebutton id="btnRiproponi" Width="20px" Runat="server" AlternateText="Riproponi" DisabledUrl="../App_Themes/ImgComuni/new_proto_rip_disabled.gif"
										Tipologia="" Thema="new_" SkinID="proto_rip" height="18px"></cc1:imagebutton></td>
								<td class="testo_grigio_scuro" vAlign="middle" align="center">Pulisci</TD>
								<td class="testo_grigio_scuro" vAlign="middle" align="center">
								    <cc1:imagebutton id="btnClearData" Width="20px" Runat="server" AlternateText="Pulisci" DisabledUrl="../App_Themes/ImgComuni/clearFlag_disabled.gif"
										Tipologia="" Thema="clear" SkinID="Flag" height="18px"></cc1:imagebutton></td>
							</tr>
						    <tr><td colspan="7"><img height="2" src="../images/proto/spaziatore.gif" width="8" border="0"></td></tr>
						</table>
					</td>
				</tr>
				<tr class="box_item">
					<td class="titolo_scheda" align="center" colSpan="7">
						<DIV id="panelUO" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 100px" align="center" runat="server"><uc1:smistauo id="SmistaUO" runat="server"></uc1:smistauo></DIV>
					</td>
				</tr>
				<tr class="box_item">
					<td class="titolo_scheda">
						<table id="tblButtonsContainer" cellSpacing="0" cellPadding="2" width="550" align="center" border="0" runat="server">
							<tr>
								<td class="titolo_scheda" align="center" width="25%">
								    <asp:imagebutton id="btnProtocolla" runat="server" AlternateText="Protocolla" SkinID="protocolla_PS" Enabled="true" OnClientClick="AvvisoVisibilita();"></asp:imagebutton>
									<asp:imagebutton id="btnProtocollaDisabled" style="CURSOR: default" runat="server" AlternateText="Protocolla" ImageUrl="../App_Themes/ImgComuni/btn_PS_protocolla_dis.gif" Enabled="False"></asp:imagebutton></td>
								<td class="titolo_scheda" align="center" width="25%">
								    <asp:imagebutton id="btnAcquireDocument" runat="server" AlternateText="Acquisisci documento principale" SkinID="acquisisci_PS" Enabled="False"></asp:imagebutton></td>
								<td class="titolo_scheda" align="center" width="25%">
								    <asp:imagebutton id="btnAcquireAttach" runat="server" AlternateText="Acquisisci allegato" SkinID="acquisisci_alleg_PS" Enabled="False" ></asp:imagebutton></td>
								<td class="titolo_scheda" align="center" width="25%">
								    <asp:imagebutton id="btnClose" runat="server" AlternateText="Chiudi" SkinID="chiudi_PS" Enabled="False" OnClientClick="ChiudiPag();"></asp:imagebutton></td>
							</tr>
							<tr>
								<td class="titolo_scheda" align="center" width="25%"></td>
								<td class="titolo_scheda" align="center" width="25%"><asp:label id="lblDescrizioneDocAcquisiti" runat="server">N° documenti:</asp:label></td>
								<td class="titolo_scheda" align="center" width="25%"><asp:label id="lblDescrizioneAllAcquisiti" runat="server">N° allegati:</asp:label></td>
								<td class="titolo_scheda" align="center" width="25%"></td>
							</tr>
							
                            <tr>
								<td class="titolo_scheda" align="left" width="25%" colSpan="2"><div id="checkConverti" runat="server"><asp:checkbox id="chkConvertiPDF" runat="server" Text="Converti in PDF"></asp:checkbox></div></td>
								<td class="titolo_scheda" align="left" width="25%" colSpan="2"><div id="checkRecognize" runat="server"><asp:checkbox id="chkRecognizeText" runat="server" Text="Interpreta testo con OCR"></asp:checkbox></div></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
            
		</form>
        
	</body>
</HTML>
