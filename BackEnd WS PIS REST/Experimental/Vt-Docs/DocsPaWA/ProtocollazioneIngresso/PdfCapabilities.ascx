<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PdfCapabilities.ascx.cs" Inherits="ProtocollazioneIngresso.PdfCapabilities" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register Src="../AcrobatIntegration/ClientController.ascx" TagName="ClientController" TagPrefix="uc1" %>
<script type="text/javascript">
	
	function EnableCheckRecognizeTextOcr(isPostBack)
	{	
		// Script per l'abilitazione / disabilitazione del check per
		// l'interpretazione del testo con ocr.
		// Abilitato solo se risulta attiva la configurazione "adobeAcrobatIntegration"
		// e se è installato il setup per l'integrazione con adobe acrobat 7		
		var chkConvertiPDF=window.parent.document.forms[0].item("chkConvertiPDF");
		var chkRecognizeText=window.parent.document.forms[0].item("chkRecognizeText");
		var txtOcrSupported=window.parent.document.forms[0].item("txtOcrSupported");

		var ocrSupported=(IsIntegrationActiveAndInstalled() && IsEnabledRecognizeText());
		
		var enabled=(chkConvertiPDF.checked && ocrSupported);

		chkRecognizeText.disabled=(!enabled);
		
		if (!chkRecognizeText.disabled && isPostBack=="false")
			chkRecognizeText.checked=true;
			
		txtOcrSupported.value=ocrSupported;
	}
	
</script>
<uc1:ClientController ID="AcrobatClientController" runat="server" />