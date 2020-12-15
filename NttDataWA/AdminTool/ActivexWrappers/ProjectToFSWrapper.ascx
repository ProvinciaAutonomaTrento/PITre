<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectToFSWrapper.ascx.cs"
    Inherits="SAAdminTool.ActivexWrappers.ProjectToFSWrapper" %>
<% 
    if (SAAdminTool.ActivexWrappers.Configurations.ProjectToFSControl &&
       !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "projectToFSObject"))
    { 
%>

    <object visible="false"
        classid="CLSID:55F2EDCF-1ECE-4F99-9477-EE3D9B36D75D"
        codebase="../activex/DocsPa_ProjectToFS.CAB#version=3,11,0,0" 
        style="height: 0px; width: 0px;">
    </object>
<%
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "projectToFSObject", string.Empty);
   }
%>

<script id="projectToFSScript" type="text/javascript">
    
    function ExportPrj(outputFolder)
    {
        try {
        
            if ("<%=SAAdminTool.ActivexWrappers.Configurations.ProjectToFSControl%>" == "True") {
                var user = new ActiveXObject("DocsPa_ProjectToFS.UserInfo");
        
                user.UserId = "<%=this.InfoUtente.userId%>";
                user.IdCorrGlobali = "<%=this.InfoUtente.idCorrGlobali%>";
                user.IdPeople = "<%=this.InfoUtente.idPeople%>";
                user.IdGruppo = "<%=this.InfoUtente.idGruppo%>";
                user.IdAmministrazione = "<%=this.InfoUtente.idAmministrazione%>";
                user.Sede = "<%=this.InfoUtente.sede%>";
                user.UrlWA = "<%=this.InfoUtente.urlWA%>";
                user.Dst = "<%=this.InfoUtente.dst%>";
                
                var controller;
                controller = new ActiveXObject("DocsPa_ProjectToFS.Controller");
                
                controller.user = user;
                controller.IdFascicolo = "<%= this.IdFascicolo %>";
                controller.webServiceUrl = "<%= this.WebServiceUrl %>";
                return controller.ExportProject(outputFolder);
                
                controller = null;
                user = null;
            }
            else {
                alert("Esportazione fascicolo non supportata");
            }
        }
        catch (e) {
            alert(e.message.toString());
            
        }
    }

    
</script>

