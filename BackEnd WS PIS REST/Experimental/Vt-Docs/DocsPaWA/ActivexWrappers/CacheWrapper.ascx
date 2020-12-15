<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CacheWrapper.ascx.cs" Inherits="DocsPAWA.ActivexWrappers.CacheWrapper" %>
<% if (DocsPAWA.ActivexWrappers.Configurations.CacheControl && !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "cacheObject")) { %> 
    <object visible="false" classid="CLSID:A8362A63-CD64-45BE-9DFD-03AEEC7E3499" codebase="../activex/DocsPa_Cache.CAB#version=3,11,0,1" height="0" width="0"></object>
<% this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "cacheObject", string.Empty); }%>
   <script>
     function executeCache(file)
    {
        try{
                if ("<%=DocsPAWA.ActivexWrappers.Configurations.CacheControl%>" == "True" 
                    &&  "<%=isActiveCache%>" == "TRUE") 
                {
                    
                    var webServiceUrl = "<%=webServiceUrl%>";
                    var docNumber = "<%=docNumber%>";
                    var pathRepository = "<%=pathRepository%>";
                    var versionId = "<%=versionId%>";  
                    var stato = "<%=statoFileTrasferito%>";
                    var localita = "<%=localita%>";   
                    var fileName = file;          
                    var controller;
                    controller = new ActiveXObject("DocsPa_Cache.Controller");
                    var risposta =controller.exeCache(webServiceUrl,
                                        docNumber,
                                        pathRepository,
                                        versionId,
                                        fileName,
                                        stato,
                                        localita);
                   return risposta;
                }
                else                 
                    alert('non è possibile eseguire il cache del file selezionato');
}
catch(e)
    {
        alert(e);
    }                    
                //return "errore di collegamento al modulo di cache"
       return '';
 }
</script>