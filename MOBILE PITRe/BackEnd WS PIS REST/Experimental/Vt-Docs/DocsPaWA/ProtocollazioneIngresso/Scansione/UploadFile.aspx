<%@ Page language="c#" Codebehind="UploadFile.aspx.cs" AutoEventWireup="false" Inherits="ProtocollazioneIngresso.Scansione.UploadFile" %>

<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" TagPrefix="uc2" %>
<%@ Register Src="../../AcrobatIntegration/ClientController.ascx" TagName="ClientController"
    TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > UploadFile</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script type="text/javascript">
		
		    function UploadFileConvertPdfServer(fileName,convertPDFServer,recognizeText,acrobatIntegration,RemoveLocalFile) 
			{	
                var returnValue=false;
                
                try
                {
				    var fileToUpload=fileName;
				    var canUpload=false;
    				
				    var canContinue = true;
		            // Validazione del formato file fornito in ingresso e della dimensione
		            canContinue = (SF_ValidateFileFormat(fileName) && SF_ValidateFileSize(fileName))
	
			    	if (canContinue) {
			    	    var uploader = new ActiveXObject("DocsPa_AcquisisciDoc.ctrlUploader");
			    	    var xmlDom = uploader.GetXMLRequest(fileToUpload, false, false);

			    	    if (uploader.ErrNumber != 0)
                        {
                            alert(uploader.ErrDescription);					        
                        }
                        else
                        {
					        canUpload=true;					       
                        }

                        if (canUpload)
                        {
					        // Url a cui viene eseguito l'upload
					        var uploadPath="UploadFile.aspx?convertPdfServer="+convertPDFServer;
        					
                            var http = new ActiveXObject("MSXML2.XMLHTTP")
                            http.Open("POST", uploadPath, false);
                            http.send(xmlDom);

                            returnValue=(http.status!=1);
                        }

				        if (RemoveLocalFile)
				        {
					        try
					        {
						        // Cancellazione del file di cui si è fatto l'upload
						        var fso=FsoWrapper_CreateFsoObject();
						        fso.DeleteFile(fileName);
					        }
					        catch (ex)
					        {
						        // Errore in cancellazione dei file temporanei in locale,
						        // non è necessario sollevare un'eccezione
					        }
				        }
				    }
				}
				catch (ex)
				{
				    alert("Si è verificato un errore nell'invio del documento");
				}

				window.returnValue=returnValue;
				window.close();
			}
			
    		function UploadFile(fileName,convertPDF,recognizeText,acrobatIntegration,RemoveLocalFile) 
			{	
                var returnValue=false;
                
                try
                {
				var fileToUpload=fileName;
				var pdfFileName="";
				var canUpload=false;
				
				var canContinue = true;
			    if (convertPDF)
			        // Validazione del file da convertire in formato pdf
                    canContinue = SF_ValidateFileFormat(fileName + ".pdf");
			    else
			        // Validazione del formato file fornito in ingresso e della dimensione
			        canContinue = (SF_ValidateFileFormat(fileName) && SF_ValidateFileSize(fileName))
	
			    if (canContinue)
			    {
	        	    var convertWithAcrobat=(convertPDF && acrobatIntegration && IsInstalledAdobeAcrobatIntegration());
				    if (convertWithAcrobat)
				    {	
					    // Conversione file pdf con adobe acrobat integration
					    pdfFileName=ConvertPdfWithAcrobat(fileName,recognizeText);
					    
                        // Validazione dimensione file convertito in pdf
					    canContinue=SF_ValidateFileSize(pdfFileName);							

					    fileToUpload=pdfFileName;
					    convertPDF=false;
				    }
    				
    				if (canContinue)
    				{
    				    var uploader = new ActiveXObject("DocsPa_AcquisisciDoc.ctrlUploader");
				        var xmlDom = uploader.GetXMLRequest(fileToUpload,convertPDF,false);

				        if (uploader.ErrNumber != 0)
                        {
					        if (convertPDF)
					        {
						        // In presenza di errore nella conversione in PDF, 
						        // è possibile fare l'upload del documento originale
						        if (confirm('Conversione in PDF non effettuata.\nImportare ugualmente il documento nel formato originale?'))
						        {
						            xmlDom = uploader.GetXMLRequest(documentsFolder + "\\" + fileName, false, false);
							        canUpload=true;
						        }
					        }
					        else
					        {
					            alert(uploader.ErrDescription);
					        }
                        }
                        else
                        {
					        if (convertPDF)
					        {
						        var xmlNode=xmlDom.selectSingleNode('//CurrentVersion');
        						
						        if (xmlNode!=null)
						        {
							        xmlNode.attributes[0].text=fileName + ".PDF";
							        xmlNode=null;
							        xmlDom.save(fileName);						
						        }
					        }
					        else
					        {
						        canUpload=true;
					        }
                        }

                        if (canUpload)
                        {
					        // Url a cui viene eseguito l'upload
					        var uploadPath="UploadFile.aspx"
        					
                            var http = new ActiveXObject("MSXML2.XMLHTTP")
                            http.Open("POST", uploadPath, false);
                            http.send(xmlDom);

                            returnValue=(http.status!=1);
                        }

				        if (RemoveLocalFile)
				        {
					        try
					        {
						        // Cancellazione del file di cui si è fatto l'upload
						        var fso=FsoWrapper_CreateFsoObject();
						        fso.DeleteFile(fileName);
        						
						        if (pdfFileName!="")
							        fso.DeleteFile(pdfFileName);
					        }
					        catch (ex)
					        {
						        // Errore in cancellazione dei file temporanei in locale,
						        // non è necessario sollevare un'eccezione
					        }
				        }
				    }
				}
				}
				catch (ex)
				{
				    alert("Si è verificato un errore nell'invio del documento");
				}

				window.returnValue=returnValue;
				window.close();
			}
			
            // Conversione di un file tiff in pdf con l'sdk di acrobat
			function ConvertPdfWithAcrobat(acquiredFilePath, recognizeText)
			{   
			    var retValue="";
				
				try
				{
				    // Se attiva integrazione con adobe acrobat,
				    // il file originale viene convertito in pdf e
				    // salvato nella cartella temporanea. 
				    // Viene quindi fatto l'upload e l'integrazione
				    // del file in docspa
				    retValue = GetTemporaryPDFFilePath(acquiredFilePath);

				    ConvertPdfFile(acquiredFilePath, retValue, recognizeText);
                }
				catch (e)
				{
					alert("Errore nella conversione in Pdf del documento.\n" + e.message.toString());
				}
				
				return retValue;
			}
			
			// Costruzione del percorso del file PDF temporaneo
			// creato mediante integrazione adobe acrobat 7
			function GetTemporaryPDFFilePath(acquiredFilePath)
			{
				var fso=FsoWrapper_CreateFsoObject();		
				var pdfFolder=fso.GetFile(acquiredFilePath).ParentFolder;
				
				if (!fso.FolderExists(pdfFolder))
					fso.CreateFolder(pdfFolder);
				
				return pdfFolder + "\\" + fso.GetFileName(acquiredFilePath) + ".pdf"; 	
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="frmUploadFile" method="post" runat="server">
			<OBJECT id="ctrlUploader" codeBase="../../activex/DocsPa_AcquisisciDoc.CAB#version=1,0,0,0"
				classid="CLSID:27AEF6CF-0C73-4772-B6CD-F904A469184D" VIEWASTEXT>
				<PARAM NAME="_ExtentX" VALUE="0">
				<PARAM NAME="_ExtentY" VALUE="0">
			</OBJECT>
            <uc1:ClientController ID="AcrobatClientController" runat="server" />
		    <uc2:SupportedFileTypeController id="supportedFileTypeController" runat="server" />
            <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
		</form>
	</body>
</HTML>