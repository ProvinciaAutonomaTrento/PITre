<%@ Page language="c#" Codebehind="UploadDocument.aspx.cs" AutoEventWireup="false" Inherits="NttDataWA.SmartClient.UploadDocument" %>

<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" TagPrefix="uc2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > Acquisizione File</title>
		<script type="text/javascript">
		
			function InviaFile(fileName,convertiPDF,recognizeText,RemoveLocalFile,cartaceo) 
			{	
				try
				{
					var fileToUpload=fileName;
					var pdfFileName="";
			
			        var canContinue = true;
			        if (convertiPDF)
			            canContinue = SF_ValidateFileFormat(fileName + ".pdf");
			        
			        if (canContinue)
			        {
         			    if (convertiPDF)
					    {	
						    var converter=upload.pdfConverter.GetConverter("<%= path%>/SmartClient/Librerie/");
    												
						    if (converter.ConvertPdf(fileName,recognizeText))
						    {
							    pdfFileName=converter.OutputPdfFilePath;
						        fileToUpload=pdfFileName;
						        canContinue = SF_ValidateFileSize(fileToUpload);
						    }
					    }
				    }
			    
				    if (canContinue)
				    {
                        // ByteArray contenente lo stream del file in formato XML
			            var fileUploader=upload.uploader.GetFileUploader(fileToUpload);
			            var content=fileUploader.GetContentXmlByteArray();
    					
			            var http = new ActiveXObject("MSXML2.XMLHTTP")
			            http.Open("POST","<%= path%>/SmartClient/UploadPageHandler.aspx?cartaceo=" + cartaceo, false);
			            http.send(content);

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
				catch (exception)
				{
					alert(exception.message.toString());
				}
				finally
				{
					window.close();	
				}
			}
			
		</script>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" onload="javascript:InviaFile('<%=FileName%>', <%=ConvertiPDF%>, <%=RecognizeText%>, <%=RemoveLocalFile%>, <%=Cartaceo%>);">
		<form id="acquisizioneXML" method="post" runat="server">
		    <uc2:SupportedFileTypeController id="supportedFileTypeController" runat="server" />
            <uc1:FsoWrapper ID="fsoWrapper" runat="server" />
			<table class="testo_msg_grigio" align="center" width="100%" height="100%">
				<tr align="center" valign="middle">
					<td><font size="3">Upload del file in corso...</font>
					</td>
				</tr>
			</table>
		</form>
		<form name="upload" action="UploadPageHandler.aspx" method="post">
			<OBJECT id="MSXML3" style="DISPLAY: none" codeBase="../activex/msxml3.cab#version=8,00,7820,0"
				type="application/x-oleobject" height="0" width="0" data="data:application/x-oleobject;base64,EQ/Z9nOc0xGzLgDAT5kLtA=="
				classid="clsid:f5078f32-c551-11d3-89b9-0000f81fe221" VIEWASTEXT>
			</OBJECT>
			<OBJECT id="pdfConverter" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
				classid="../SmartClient/Librerie/DPA.Web.dll#DPA.Web.Pdf.ConverterControl" VIEWASTEXT>
			</OBJECT>
			<OBJECT id="uploader" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
				classid="../SmartClient/Librerie/DPA.Web.dll#DPA.Web.FileUploaderControl" VIEWASTEXT>
			</OBJECT>
		</form>
	</body>
</HTML>
