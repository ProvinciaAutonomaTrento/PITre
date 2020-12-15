<%@ Page language="c#" Codebehind="acquisizioneXML.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.acquisizioneXML" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" TagPrefix="uc2" %>
<%@ Register src="../ActivexWrappers/CacheWrapper.ascx" tagname="CacheWrapper" tagprefix="uc3" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<SCRIPT type="text/javascript">
		
			function InviaFile(fileName, convertiPDF, convertiPDFServer, keepOriginal, removeLocalFile, cartaceo, pdfSincrono)
			{
				if (!(fileName == null || fileName == "")) 
				{
				    var canContinue = true;
				    if (convertiPDF && !convertiPDFServer)
				        // Validazione del file da convertire in formato pdf
                        canContinue = SF_ValidateFileFormat(fileName + ".pdf");
				    else
				        // Validazione del formato file fornito in ingresso e della dimensione
				        canContinue = (SF_ValidateFileFormat(fileName) && SF_ValidateFileSize(fileName))

				    if (canContinue) {

				        var uploader = null;
				        var xml_dom = null;

				        try {
				            uploader = new ActiveXObject("DocsPa_AcquisisciDoc.ctrlUploader");
				            xml_dom = uploader.GetXMLRequest(fileName, convertiPDF && !convertiPDFServer, false);
				        } catch (ex) {
				            alert("Si è verificato un errore durante l'upload. " + ex.Description);
				            return;
                        }

				        

				        if (uploader.ErrNumber != 0)
					    {
					        alert(uploader.ErrDescription);
				        }
					    else {
                            var http = null;
                            try {
                                http = new ActiveXObject("MSXML2.XMLHTTP");
                                http.Open("POST", "<%= path%>/Upload.aspx?cartaceo=" + cartaceo + "&convertiPDF=" + convertiPDF + "&convertiPDFServer=" + convertiPDFServer + "&convertiPDFServerSincrono=" + pdfSincrono, false);
                                http.send(xml_dom);
                            } catch (ex) {
                                alert("Errore durante l'upload del file: " + ex.Message);
                                return;
                            }
    						
					        if (removeLocalFile)
					        {
						        // Cancellazione del file di cui si è fatto l'upload
						        var fso=FsoWrapper_CreateFsoObject();
						        fso.DeleteFile(fileName);
					        }
					    }
					}
				}

				window.close();	
			}	
		</SCRIPT>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" onload="javascript:InviaFile('<%=FileName%>', <%=ConvertiPDF%>, <%=ConvertiPDFServer%>, <%=KeepOriginal%>, <%=RemoveLocalFile%>, <%=Cartaceo%>, <%= PdfSincrono %>);">
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
		<form name="upload" action="<%=path%>/Upload.aspx" method="post">
			<OBJECT id="ctrlUploader" codeBase="../activex/DocsPa_AcquisisciDoc.CAB#version=1,0,0,0"
                classid="CLSID:27AEF6CF-0C73-4772-B6CD-F904A469184D" VIEWASTEXT>
				<PARAM NAME="_ExtentX" VALUE="0">
				<PARAM NAME="_ExtentY" VALUE="0">
			</OBJECT>
		</form>
	</body>
</HTML>
