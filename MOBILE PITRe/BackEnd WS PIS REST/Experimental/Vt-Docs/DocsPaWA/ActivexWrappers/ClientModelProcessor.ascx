<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientModelProcessor.ascx.cs" Inherits="DocsPAWA.ActivexWrappers.ClientModelProcessor" %>
<% if (DocsPAWA.ActivexWrappers.Configurations.UserClientModelProcessorControl && 
       !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "clientModelProcessorObject")) 
   { %> 
<div visible="false"><OBJECT CLASSID="CLSID:8F13EC22-319C-4814-8267-FF9FEF480602" CODEBASE="../activex/DocsPa_Models.CAB#version=3,10,0,3" height="0" width="0"></OBJECT></div>
<%this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "clientModelProcessorObject", string.Empty);
   }%> 
<script id="clientModelProcessorScript" type="text/javascript">
    function ClientModelProcessor_ProcessModel(outputFilePath, documentId, modelType) 
    {
        var retValue = false;

        try {
            if ("<%=DocsPAWA.ActivexWrappers.Configurations.UserClientModelProcessorControl%>" == "True") {
                var request = new ActiveXObject("DocsPa_Models.ModelRequest");

                request.DocumentId = documentId;
                request.ModelType = modelType;
                request.userInfo.Userid = "<%=this.InfoUtente.userId%>";
                request.userInfo.Dst = "<%=this.InfoUtente.dst%>";
                request.userInfo.IdAmministrazione = "<%=this.InfoUtente.idAmministrazione%>";
                request.userInfo.IdCorrGlobali = "<%=this.InfoUtente.idCorrGlobali%>";
                request.userInfo.IdGruppo = "<%=this.InfoUtente.idGruppo%>";
                request.userInfo.IdPeople = "<%=this.InfoUtente.idPeople%>";
                request.userInfo.UrlWA = "<%=this.InfoUtente.urlWA%>";

                var processor = new ActiveXObject("DocsPa_Models.ModelProcessor");

                retValue = processor.ProcessModel(request, "<%=this.WebServiceUrl%>", outputFilePath);

                if (!retValue)
                    alert(processor.Exception);

                processor = null;
            }
            else {
                alert("Elaborazione modelli client non supportata");
                retValue = false;
            }
        }
        catch (e) {
            alert(e.message.toString());
            retValue = false;
        }

        return retValue;
    }
</script>