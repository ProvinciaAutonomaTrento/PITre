<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PdfCapabilitiesSmartClient.ascx.cs" Inherits="ProtocollazioneIngresso.PdfCapabilitiesSmartClient" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<OBJECT id="pdfConverter" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTYzIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
	classid="http:../SmartClient/librerie/DPA.Web.dll#DPA.Web.Pdf.ConverterControl" VIEWASTEXT>
</OBJECT>
<SCRIPT language="javascript">

	function EnableCheckRecognizeTextOcr(isPostBack)
	{
		// Script per l'abilitazione / disabilitazione del check per
		// l'interpretazione del testo con ocr mediante i componenti smart client
		var chkConvertiPDF=window.parent.document.forms[0].item("chkConvertiPDF");
		var chkRecognizeText=window.parent.document.forms[0].item("chkRecognizeText");
		var txtOcrSupported=window.parent.document.forms[0].item("txtOcrSupported");
		
		var converter=document.pdfConverter.GetConverter("<%= Path%>/SmartClient/librerie/");
		var enabled=(chkConvertiPDF.checked && converter.OcrSupported && chkConvertiPDF.checked);
		chkRecognizeText.disabled=(!enabled);
		
		if (!chkRecognizeText.disabled && isPostBack=="false")
			chkRecognizeText.checked=true;
			
		txtOcrSupported.value=enabled;
	}
	
</SCRIPT>
