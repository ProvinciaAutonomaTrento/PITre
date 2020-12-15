<%@ Page language="c#" Codebehind="DocumentAcquisition.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SmartClient.DocumentAcquisition" %>

<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" TagPrefix="uc2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > Acquisizione</title>
		<script type="text/javascript">
		
			// Visualizzatore immagini per la scansione
			function ShowImageViewer()
			{
				DocumentAcquisition.myControl.ImageViewer.AllowScan=true;
				DocumentAcquisition.myControl.ImageViewer.ShowOpenFile=false;
				DocumentAcquisition.myControl.ImageViewer.AllowSave=true;
				DocumentAcquisition.myControl.TitleSeparateWindow='Acquisisci documento';
				DocumentAcquisition.myControl.ShowOnSeparateWindow=true;

				DocumentAcquisition.myControl.SetSizeScreenPercentage(100,100);

				var ret=DocumentAcquisition.myControl.ShowImageViewer();

				// Restituzione del percorso del file acquisito
				var imagePath=DocumentAcquisition.myControl.ImageViewer.ImagePath;

				// Chiusura immagine correntemente visualizzata
				DocumentAcquisition.myControl.ImageViewer.CloseImage();
				
				if (!ret && imagePath!="")
				{
					var fso=FsoWrapper_CreateFsoObject();
					if (fso.FileExists(imagePath))
						fso.DeleteFile(imagePath);	
						
					imagePath="";			
				}
				
				return imagePath;
			}
			
			// Invio del contenuto del file a docspa
			function InviaFile(fileName,convertPdf,recognizeText,removeLocalFile,cartaceo)
			{
			    var canContinue = true;
			    var convertPdf = document.DocumentAcquisition.chk_ConvertiPDF.checked;
			    
                if (!convertPdf)
                    canContinue = SF_ValidateFileFormat(fileName) && SF_ValidateFileSize(fileName);
                else
                    canContinue = SF_ValidateFileFormat(fileName + ".pdf");

                if (canContinue)
                {
                    document.upload.fileName.value=fileName;
				    document.upload.convertiPDF.value=convertPdf;
				    document.upload.recognizeText.value=recognizeText;
				    document.upload.removeLocalFile.value=removeLocalFile;
				    document.upload.cartaceo.value=cartaceo;
				    document.upload.submit();				
                }
			}
			
			// Upload del file
			function Upload()
			{
				window.document.body.style.cursor='wait';
										
				try
				{
					var filePath="";
					var removeLocalFile=false;
					
					if (document.DocumentAcquisition.optAcquisizioneScanner.checked)
					{
						// Acquisizione documento da scanner
						filePath=ShowImageViewer();
						
						// Rimozione file locale
						removeLocalFile=true;
					}
					else
					{
						// Reperimento percorso file locale
						filePath=window.document.DocumentAcquisition.uploadedFile.value;
					}
					
					if (filePath!=null && filePath!='')
					{					
						// Upload del file scannerizzato
						InviaFile(filePath,
							    document.DocumentAcquisition.chk_ConvertiPDF.checked,
							    document.DocumentAcquisition.chkRecognizeText.checked,
							    removeLocalFile, document.DocumentAcquisition.chk_cartaceo.checked);
					}					
				}
				catch (e)
				{
				alert(e.message.toString());
					alert("Errore nell'upload del documento.");
				}
				
				window.document.body.style.cursor='default';
			}
			
			// Gestione abilitazione / disabilitazione 
			// checkbox riconoscimento ocr, abilitato
			// solo se integrazione con adobe acrobat attiva
			function EnableCheckRecognizeText()
			{
				var converter=DocumentAcquisition.pdfConverter.GetConverter("<%= path%>/SmartClient/librerie/");
				
				var	pnl=document.getElementById("pnlRecognizeText");
				
				// Se il modulo per la conversione in pdf supporta
				// l'ocr, viene abilitato il check relativo
				if (converter.OcrSupported)
				{
					pnl.style.visibility = "visible";
					
					document.DocumentAcquisition.chkRecognizeText.disabled=
						(!document.DocumentAcquisition.chk_ConvertiPDF.checked);
					
					if (document.DocumentAcquisition.chkRecognizeText.disabled)
						document.DocumentAcquisition.chkRecognizeText.checked=false;
					else
						document.DocumentAcquisition.chkRecognizeText.checked=true;
				}
				else
				{
					pnl.style.visibility = "hidden";
				}
			}
			
			// Impostazione valore di default per il check "chkConvertiPDF"
			function SetDefaultValueCheckConvertiPDF()
			{
				var pdfConvert=(document.DocumentAcquisition.txtPDFConvert.value=='true');
				
				document.DocumentAcquisition.chk_ConvertiPDF.checked=pdfConvert;
			}

			// Gestione abilitazione / disabilitazione check "chkConvertiPDF"
			function SetDefaultEnabledCheckConvertiPDF()
			{
				var pdfConvertEnabled=(document.DocumentAcquisition.txtPDFConvertEnabled.value=='true');
				
				document.DocumentAcquisition.chk_ConvertiPDF.disabled=!pdfConvertEnabled;
			}
						
			// Impostazione visibilità check interpretazione testo con ocr,
			// solamente se attivata l'integrazione con adobe acrobat
			function SetVisibilityCheckRecognize(isVisible)
			{
				var	pnl=document.getElementById("pnlRecognizeText");
				if (isVisible)
					pnl.style.visibility = "visible";
				else
					pnl.style.visibility = "hidden";
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

			function chkConvertiPDF_onClick()
			{
				// abilitazione / disabilitazione check OCR,
				// abilitato solo se il valore del check converti in pdf è true.
				EnableCheckRecognizeText();
			}
			
			// Impostazione del radio button selezionato per default
			// (acquisizione da scanner o da file)
			function SetDefaultRadioButton()
			{
				document.DocumentAcquisition.optAcquisizioneScanner.checked=true;
				EnabledRadioButtonsControls(true);
			}
			
			// Gestione abilitazione / disabilitazione controlli dipendenti
			// dai radio buttons (acquisizione da scanner o da file)
			function EnabledRadioButtonsControls(acquisizioneScannerEnabled)
			{
				EnableTableAcquisizioneDaScanner(acquisizioneScannerEnabled);
				EnableTableAcquisizioneFile(!acquisizioneScannerEnabled);
				
				EnableCheckCartaceo(acquisizioneScannerEnabled);
			}
			
			// Handler evento click radio acquisizione da scanner
			function OnClickRadioAcquisizioneScanner()
			{
				var radio=document.DocumentAcquisition.optAcquisizioneScanner;
				if (radio!=null)
				{
					EnabledRadioButtonsControls(radio.checked);
				}
			}
			
			// Handler evento click radio acquisizione da file
			function OnClickRadioAcquisizioneDaFile()
			{
				var radio=document.DocumentAcquisition.optAcquisisciDaFile;
				if (radio!=null)
				{
				    var checked = document.DocumentAcquisition.chk_cartaceo.checked;
				    if (document.DocumentAcquisition.chk_cartaceo.disabled)
				        checked = false;
				
					EnabledRadioButtonsControls(!radio.checked);
					
					SetControlFocus(document.DocumentAcquisition.uploadedFile.id);
					
					document.DocumentAcquisition.chk_cartaceo.checked = checked;
				}
			}
			
			// Handler evento onFocus controllo uploadFile
			function OnFocusUploadFile()
			{
				var radio=document.DocumentAcquisition.optAcquisisciDaFile;
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
					
            // Gestione abilitazione / disabilitazione controllo
			// checkbox documento cartaceo
			function EnableCheckCartaceo(acquisizioneScannerEnabled)
			{
                // Check documento cartaceo abilitato solo se il documento non è acquisito da scanner
				document.DocumentAcquisition.chk_cartaceo.disabled = acquisizioneScannerEnabled;

                if (acquisizioneScannerEnabled)
                    document.DocumentAcquisition.chk_cartaceo.checked = true;
			}

		</SCRIPT>
		<META content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<META content="C#" name="CODE_LANGUAGE">
		<META content="JavaScript" name="vs_defaultClientScript">
		<META content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<SCRIPT language="javascript" src="../LIBRERIE/DocsPA_Func.js"></SCRIPT>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<BASE target="_self">
	</HEAD>
	<BODY bottomMargin="1" leftMargin="1" topMargin="6" rightMargin="1" MS_POSITIONING="GridLayout">
		<FORM id="DocumentAcquisition" method="post" encType="multipart/form-data" runat="server">
		    <uc2:SupportedFileTypeController id="supportedFileTypeController" runat="server" />
		    <uc1:FsoWrapper ID="fsoWrapper" runat="server" />
			<INPUT id="txtPDFConvert" type="hidden" value="false" name="txtPDFConvert" runat="server">
			<INPUT id="txtPDFConvertEnabled" type="hidden" value="false" name="txtPDFConvertEnabled"
				runat="server">
			<TABLE class="info" id="tblContainer" cellSpacing="0" cellPadding="5" width="400" align="center"
				border="0" runat="server">
				<TR>
					<TD class="item_editbox" style="width: 512px">
						<P class="boxform_item">
							<asp:label id="Label3" runat="server">Acquisizione</asp:label></P>
					</TD>
				</TR>
				<TR>
					<TD align="left" style="width: 512px">
						<TABLE cellSpacing="0" cellPadding="3" border="0">
							<TR>
								<TD vAlign="top" width="3%">
									<asp:radiobutton id="optAcquisizioneScanner" runat="server" GroupName="ACQUISIZIONE" CssClass="titolo_scheda"></asp:radiobutton></TD>
								<TD width="97%">
									<TABLE id="tblAcquisizioneDaScanner" cellSpacing="0" cellPadding="3" border="0">
										<TR>
											<TD class="titolo_scheda">
												<asp:label id="lblAcquisisciDaScanner" runat="server" CssClass="titolo_scheda">Acquisisci da scanner</asp:label></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
							<TR>
								<TD colSpan="2" height="10"></TD>
							</TR>
							<TR>
								<TD vAlign="top" width="3%">
									<asp:radiobutton id="optAcquisisciDaFile" runat="server" GroupName="ACQUISIZIONE" CssClass="titolo_scheda"
										Width="24px"></asp:radiobutton></TD>
								<TD width="97%">
									<TABLE id="tblAcquisizioneDocumento" cellSpacing="0" cellPadding="3" border="0">
										<TR>
											<TD class="titolo_scheda">
												<asp:label id="lblAcquisisciDaFile" runat="server">Acquisisci da file:</asp:label></TD>
										</TR>
										<TR>
											<TD><INPUT class="testo_grigio" id="uploadedFile" onfocus="OnFocusUploadFile();" type="file"
													size="50" name="uploadedFile" runat="server">
											</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD style="width: 512px">
						<asp:checkbox id="chk_ConvertiPDF" runat="server" CssClass="titolo_scheda" Text="Converti in PDF"></asp:checkbox>
                        &nbsp;
                        <asp:CheckBox id="chk_cartaceo" runat="server" Checked="true" CssClass="titolo_scheda" Text="Cartaceo" />
                    </TD>
				</TR>
				<TR>
					<TD style="width: 512px">
						<asp:panel id="pnlRecognizeText" Runat="server">&nbsp;&nbsp;&nbsp; 
<asp:checkbox id="chkRecognizeText" runat="server" CssClass="titolo_scheda" Text="Interpreta testo con OCR"></asp:checkbox></asp:panel></TD>
				</TR>
				<TR>
					<TD align="center" height="30" style="width: 512px"><INPUT class="PULSANTE" id="btnInvia" onclick="Upload();" type="button" value="INVIA" runat="server"
							NAME="btnInvia"> <INPUT class="PULSANTE" id="btnChiudi" onclick="javascript:window.close();" type="button"
							value="CHIUDI">
					</TD>
				</TR>
				<TR height="0" width="0">
					<TD style="width: 512px">
						<OBJECT id="myControl" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
							classid="../SmartClient/librerie/DPA.Web.dll#DPA.Web.Imaging.ImageViewerContainerControl"
							VIEWASTEXT>
						</OBJECT>
					</TD>
					<td>
						<OBJECT id="pdfConverter" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
							classid="../SmartClient/librerie/DPA.Web.dll#DPA.Web.Pdf.ConverterControl" VIEWASTEXT>
						</OBJECT>
					</td>
				</TR>
			</TABLE>
		</FORM>
		<FORM name="upload" action="UploadDocument.aspx" method="post">
			<INPUT id="fileName" type="hidden" name="fileName"> <INPUT id="convertiPDF" type="hidden" name="convertiPDF">
			<INPUT id="recognizeText" type="hidden" name="recognizeText"> <INPUT id="removeLocalFile" type="hidden" name="removeLocalFile">
			<INPUT id="cartaceo" type="hidden" name="cartaceo"> 
		</FORM>
	</BODY>
</HTML>
