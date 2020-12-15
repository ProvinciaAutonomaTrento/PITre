<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientController.ascx.cs" Inherits="DocsPAWA.AcrobatIntegration.ClientController" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc1" %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "acrobatClientControllerScript"))
  { %>
<script id="acrobatClientControllerScript" type="text/javascript">
    /// Verifica se effettuare l'acquisizione da scanner usando direttamente l'integrazione adobe acrobat
    function ScanWithAcrobatIntegration()
    {
        return ("<%=ScanWithAcrobatIntegration%>" == "True");
    }
    
    // Verifica se l'integrazione adobe acrobat è attiva 
    // e che la libreria client risulta installata
    function IsIntegrationActiveAndInstalled()
    {
        return (IsIntegrationActive() && IsInstalledAdobeAcrobatIntegration());
    }

    // Verifica se l'integrazione con adobe acrobat sdk è attiva o meno
    function IsIntegrationActive()
    {
        return ("<%=IsIntegrationActive%>" == "True");
    }    
    
    // Verifica se risulta installata la libreria client per l'integrazione con l'sdk di adobe acrobat
    function IsInstalledAdobeAcrobatIntegration()
    {
	    var retValue=false;

	    try
	    {
		    var acrobatInterop=new ActiveXObject("<%=AcrobatIntegrationClassId%>");
		    retValue=true;
	    }
	    catch (e)
	    {
	    }				

	    return retValue;
    }
    
    // Verifica se la libreria client per l'integrazione con l'sdk di adobe acrobat
    // supporti l'interpretazione del testo con ocr
    function IsEnabledRecognizeText()
    {
        return ("<%=IsEnabledRecognizeText%>" == "True");
    }
    
    // Metodo per la conversione in pdf
    // In ingresso viene fornito di un'array di byte, il formato del file e se 
    // effettuare ocr o meno
    function ConvertPdfStream(content, fileFormat, recognizeText)
    {
        try
        {
            if (IsIntegrationActiveAndInstalled())
            {
                var outputContent = null;
                
                var inputFile=GetTemporaryFolder() + "\inputFile";
                if (fileFormat != null && fileFormat != "")
                {   
                    if (fileFormat.charAt(0)=='.')
                        inputFile +=  fileFormat;
                    else
                        inputFile += "." + fileFormat;
                }
                
                var outputFile=GetTemporaryFolder() + "\\outputFile.pdf";
                
                // Save del file da convertire
				AdoStreamWrapper_SaveBinaryData(inputFile,content);                
                
                // Conversione pdf del file in input
                var acrobatInterop=CreateObject("<%=AcrobatIntegrationClassId%>");
	            if (acrobatInterop.ConvertFileToPDF(inputFile, outputFile, recognizeText))
	            {
                    // Se la conversione è andata a buon fine,
                    // viene effettuato il caricamento del file nell'array di byte
                    outputContent = AdoStreamWrapper_OpenBinaryData(outputFile);
                }
            }
                
            return outputContent;
        }
        catch (ex)
        {
            throw ("Errore nella conversione in PDF:\n" + ex.message.toString());
        }
    }

    // Conversione in pdf del file
    // Parametri:
    //  - inputFile:     il percorso del file da convertire
    //  - outputFile:    percorso del file convertito in pdf
    //  - recognizeText:        indica se effettuare ocr o meno
    function ConvertPdfFile(inputFile, outputFile, recognizeText)
    {
        try
        {
            var retValue = false;
            
            if (IsIntegrationActiveAndInstalled())
            {	
	            var acrobatInterop=CreateObject("<%=AcrobatIntegrationClassId%>");
	            retValue = acrobatInterop.ConvertFileToPDF(inputFile,
										            outputFile,
										            recognizeText);
	        }
    	    
	        return retValue;
	    }  
	    catch (ex)
        {
            throw("Errore nella conversione in PDF:\n" + ex.message.toString());
        } 
    }
    
    // Acquisizione di un documento da scanner
    function ScanPdf(recognizeText)
    {
    	var pdfFolder=GetTemporaryFolder();
		
		var fso=FsoWrapper_CreateFsoObject();
		var retValue = (pdfFolder + fso.GetTempName() + ".pdf");
		
		if (!ScanPdfFile(retValue, recognizeText))
		    retValue="";

		return retValue;
    }
        
    // Acquisizione di un documento da scanner
    function ScanPdfFile(outputFile, recognizeText)
    {	
	    try
	    {
	        var retValue = false;
	    
	        if (IsIntegrationActiveAndInstalled())
            {	
		        var acrobatInterop=CreateObject("<%=AcrobatIntegrationClassId%>");
		        
		        if ("<%=IsAcrobat6Interop%>" == "True")
		            retValue = acrobatInterop.AcquireFromScanner(outputFile, recognizeText);
		        else
		            retValue = acrobatInterop.AcquireFromScanner(outputFile);
		    }
		    
		    return retValue;
	    }
	    catch (e)
	    {				
		    throw("Errore nella scansione del documento.\n" + e.message.toString());
	    }
	    
	    return retValue;
    }
	
    // Costruzione del percorso del file PDF temporaneo
	function GetTemporaryFolder()
	{	
		var fso=FsoWrapper_CreateFsoObject();
		var pdfFolder=fso.GetSpecialFolder(2).Path + "\\DPAImage\\";
		
		if (!fso.FolderExists(pdfFolder))
			fso.CreateFolder(pdfFolder);

		return pdfFolder;
	}
	
    // Creazione oggetto activex con gestione errore
	function CreateObject(objectType)
	{
		try
		{
			return new ActiveXObject(objectType);
		}
		catch (ex)
		{
			alert("Oggetto '" + objectType + "' non istanziato");
		}	
	}
</script>
<% 
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "acrobatClientControllerScript", string.Empty); 
  } %>
<uc1:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
<uc2:FsoWrapper ID="fsoWrapper" runat="server" />