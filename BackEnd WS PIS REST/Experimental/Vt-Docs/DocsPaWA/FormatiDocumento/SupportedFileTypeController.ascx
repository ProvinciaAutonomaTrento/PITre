<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SupportedFileTypeController.ascx.cs" Inherits="DocsPAWA.FormatiDocumento.SupportedFilesController" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>
<%@ Register Src="../ActivexWrappers/CacheWrapper.ascx" TagName="CacheWrapper" TagPrefix="uc2" %>
<uc1:FsoWrapper ID="fsoWrapper" runat="server" />
<uc2:CacheWrapper ID="CacheWrapper1" runat="server" />
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "supportedFileTypeControllerScript")){ %>

<script id="supportedFileTypeControllerScript" type="text/javascript">
    function SF_ValidateSize(fileExtension, size)
    {
        var retValue = false;
        if ("<%=SupportedFileTypesEnabled%>" == "True")
        {
            var fileFormatsArray = "<%=SupportedFileFormats%>".split('|');
            if (fileFormatsArray != null)
            {
                for (var i = 0; i < fileFormatsArray.length; i++)
                {
                    var current = fileFormatsArray[i].split(':');
                    if (current[0].toUpperCase() ==  fileExtension.toUpperCase())
                    {
                        // Reperimento della dimensione massima del file
                        var fileFormatsMaxSizeArray = "<%=SupportedFileFormatsMaxSize%>".split('|');
                        var maxFileSize = parseInt(fileFormatsMaxSizeArray[i]);

                        if (maxFileSize > 0)
                        {
                            var retValue = ((size / 1024) < maxFileSize);
                            if (!retValue)
                            {
                                var alertMode = "<%=MaxFileSizeAlertMode%>".split("|")[i];
                                
                                if (alertMode == "None")
                                {   
                                 retValue = true; }
                                
                                else if (alertMode == "Warning")
                                { retValue = confirm("Il file selezionato supera la dimensione prestabilita di " + maxFileSize + "KB.\n" + 
                                                            "L'operazione potrebbe richiedere diversi secondi o qualche minuto a seconda della velocità della rete.\n" + 
                                                            "Continuare?");
                                }
                                else if (alertMode == "Error")
                                {   
                                    alert("Acquisizione non eseguita.\nPer questo formato sono ammessi documenti di dimensione minore a quella prestabilita di " + maxFileSize + " KB.");
                                    retValue = false;
                                }
                            }
                        }
                        else
                        {
                            // Se la dimensione massima del formato è di 0 la dim del file è valida
                            retValue = true;
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            retValue = ((size / 1024) < parseInt("<%=FileAcquireSizeMax%>"));
            if (!retValue)
                retValue = confirm("Il file selezionato supera la dimensione prestabilita di <%=FileAcquireSizeMax%> KB.\n" + 
                            "L'operazione potrebbe richiedere diversi secondi o qualche minuto a seconda della velocità della rete.\n" + 
                            "Continuare?");            
        }
        return retValue;
    }
    
    function SF_ValidateFileSize(file)
    {
        var retValue = false;
        var fso = FsoWrapper_CreateFsoObject();
        if (fso.FileExists(file))
            retValue = SF_ValidateSize(fso.GetExtensionName(file), fso.GetFile(file).Size);
        return retValue;
    }
    //modifica
    //funzione che determina se usare l'activex del cache o no
    function SF_UseTransfertCache(file, retValue)
    {
       if("<%=useCache%>" == "True"
           && retValue)
        {
            var risposta = "";
            var fso = FsoWrapper_CreateFsoObject();
            var dim = fso.GetFile(file).Size;
            if("<%=isCache%>" == "True")
            {
                if (!SF_ActiveTransfertCache(fso.GetExtensionName(file), dim))
                    risposta = executeCache(file)
                else
                    return false;
            }
           else
                return false;
                    
            if (risposta == "nospace")
                    alert('Spazio insufficiente. \n Non è stato possibile rimuovere nessun file dalla cache. \n Per acquisire un nuovo file occorre attendere il trasferimento dei file al Server Centrale');
            else
              if (risposta != "")
                    alert('errore: ' + risposta);

            return true;
        }
        return false;
    }
    
    //funzione che determina 
    //se attivare il trasferimento sul caching o no in base alla dimensione del file 
    //se restituisce false avvia il trasferimento se true no
    function SF_ActiveTransfertCache(fileExtension, size)
    {
        var retValue = false;
        if ("<%=SupportedFileTypesEnabled%>" == "True")
        {
            var fileFormatsArray = "<%=SupportedFileFormats%>".split('|');
            if (fileFormatsArray != null)
            {
                for (var i = 0; i < fileFormatsArray.length; i++)
                {
                    var current = fileFormatsArray[i].split(':');
                    if (current[0].toUpperCase() ==  fileExtension.toUpperCase())
                    {
                        // Reperimento della dimensione massima del file
                        var fileFormatsMaxSizeArray = "<%=SupportedFileFormatsMaxSize%>".split('|');
                        var maxFileSize = parseInt(fileFormatsMaxSizeArray[i]);
    
                        if (maxFileSize > 0)
                        {
                            var retValue = ((size / 1024) < maxFileSize);
                            if (!retValue)
                            {
                                var alertMode = "<%=MaxFileSizeAlertMode%>".split("|")[i];
                                
                                if (alertMode == "None")
                                {   
                                 retValue = true; }
                                
                                }
                                else if (alertMode == "Error")
                                {   
                                    alert("Acquisizione non eseguita.\nPer questo formato sono ammessi documenti di dimensione minore a quella prestabilita di " + maxFileSize + " KB.");
                                    retValue = false;
                                }
                            }
                        }
                        else
                        {
                            // Se la dimensione massima del formato è di 0 la dim del file è valida
                            retValue = true;
                        }
                        break;
                    }
                }
            }
            return retValue;
        }
    
    //fine modifica
    // Validazione del formato file fornito in ingresso
    function SF_ValidateFileFormat(file)
    {
        var formatValid = false;
        var formatValidForType = false;
        
        if ("<%=SupportedFileTypesEnabled%>" == "True")
        {
            var fso = FsoWrapper_CreateFsoObject();
            var fileExtension = fso.GetExtensionName(file);
            var tipoDocumento = "";
            var fileFormatsArray = "<%=SupportedFileFormats%>".split("|");

            // Se l'estensione non è valorizzata bisogna bypassare il controllo.
            // Serve per far funzionare i modelli M/Text
            if (fileExtension != null && fileExtension == '') {
                formatValid = true;
                formatValidForType = true;
            }

            if (fileFormatsArray != null && fileExtension != null && fileExtension != '')
            {
                for (var i = 0; i < fileFormatsArray.length; i++)
                {
                    var current = fileFormatsArray[i].split(':');

                    if (current[0].toUpperCase() ==  fileExtension.toUpperCase() && current[1] == "True")
                    {   
                        formatValid = true;

                        // Verifica se il formato file è valido per il tipo documento (grigio o protocollo)
                        tipoDocumento = "<%=TipoDocumento%>";
                        var fileValidFor = "<%=DocumentType%>".split("|")[i];
                        
                        if ((fileValidFor == "Grigio" && tipoDocumento == "P") || (fileValidFor == "Protocollo" && tipoDocumento == "G"))
                            formatValidForType = false;
                        else
                            formatValidForType = true;
                        break;
                    }
                }
            }
            
            if (!formatValid)
                alert("Formato documento '" + fileExtension + "' non supportato.\nPer il supporto di questo formato rivolgersi all'amministratore di sistema.");
            else if (!formatValidForType)
            {
                var msg="Formato '" + fileExtension + "' non valido per ";
                if (tipoDocumento == "P")
                    msg += "un documento protocollato";
                else 
                    msg += "un documento grigio";
                alert(msg);
            }
        }
        else
        {
            formatValid = true;
            formatValidForType = true;
        }
        
        return (formatValid && formatValidForType);
    }
    
    // Creazione oggetto activex con gestione errore
	function SF_CreateObject(objectType)
	{ try { return new ActiveXObject(objectType); } catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }}
</script>

<% 
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "supportedFileTypeControllerScript", string.Empty); 
  } %>

