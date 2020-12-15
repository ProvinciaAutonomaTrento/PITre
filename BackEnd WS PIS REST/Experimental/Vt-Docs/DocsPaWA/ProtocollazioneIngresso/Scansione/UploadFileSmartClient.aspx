<%@ Page language="c#" Codebehind="UploadFileSmartClient.aspx.cs" AutoEventWireup="false" Inherits="ProtocollazioneIngresso.Scansione.UploadFileSmartClient" %>

<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>
<%@ Register Src="../../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" TagPrefix="uc2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > UploadFileSmartClient</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script type="text/javascript">
    		function UploadFile(fileName,convertiPDF,recognizeText,RemoveLocalFile) 
			{	
				var returnValue=false;
				
				try
				{
				var fileToUpload=fileName;
				var pdfFileName="";

			    var canContinue = true;
		
				if (convertiPDF)
				{
                    canContinue = SF_ValidateFileFormat(fileName + ".pdf");

					var converter=frmUploadFile.pdfConverter.GetConverter("<%= path%>/SmartClient/librerie/");
										
					if (converter.ConvertPdf(fileName,recognizeText))
						pdfFileName=converter.OutputPdfFilePath;
						
					fileToUpload=pdfFileName;
				}
				else
				    canContinue = SF_ValidateFileFormat(fileToUpload);
				    
				// Validazione dimensione file
				canContinue = SF_ValidateFileSize(fileToUpload);
				
				if (canContinue)
				{
				    // ByteArray contenente lo stream del file in formato XML
				    var fileUploader=frmUploadFile.uploader.GetFileUploader(fileToUpload);
				    var content=fileUploader.GetContentXmlByteArray();
    				
				    // Url a cui viene eseguito l'upload
				    var uploadPath="UploadFileSmartClient.aspx"				
                    var http = new ActiveXObject("MSXML2.XMLHTTP")
                    http.Open("POST",uploadPath,false);
                    http.send(content);
                    
                    returnValue=(http.status!=1);
    				
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
					    {}
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
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="frmUploadFile" method="post" runat="server">
			<OBJECT id="pdfConverter" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
				classid="http:../../SmartClient/librerie/DPA.Web.dll#DPA.Web.Pdf.ConverterControl" VIEWASTEXT>
			</OBJECT>
			<OBJECT id="uploader" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
				classid="http:../../SmartClient/librerie/DPA.Web.dll#DPA.Web.FileUploaderControl" VIEWASTEXT>
			</OBJECT>
			<uc2:SupportedFileTypeController id="supportedFileTypeController" runat="server" />
            <uc1:FsoWrapper ID="fsoWrapper" runat="server" />
		</form>
	</body>
</HTML>