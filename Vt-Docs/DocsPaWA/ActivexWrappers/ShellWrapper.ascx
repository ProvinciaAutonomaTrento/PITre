<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShellWrapper.ascx.cs" Inherits="DocsPAWA.ActivexWrappers.ShellWrapper" %>
<% if (this.UseActivexWrapper && !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "ShellWrapperObject")) 
   { %> 
<OBJECT CLASSID="CLSID:C50FED4F-D464-4695-93E1-05C5B346C2D8" 
    CODEBASE="../activex/DocsPa_ActivexWrappers.CAB#version=3,6,0,0" 
    style="height: 0px; width: 0px"></OBJECT> 
<% 
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "ShellWrapperObject", string.Empty);
   } %><%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "shellWrapperControlScript")){%>
<script id="shellWrapperControlScript" type="text/javascript">
    function ShellWrappers_Execute(filePath)
    {
        try
        {
            if ("<%=UseActivexWrapper%>" == "True")
            {
                var shellWrapper=new ActiveXObject("DocsPa_ActivexWrappers.ShellWrapper");
                shellWrapper.ShellExecute(filePath);
            }
		    else
		    {
                var shellObj=new ActiveXObject("Shell.Application");
                shellObj.ShellExecute(filePath);
            }
		}
		catch(e)
		{
		    alert("Errore in apertura del file richiesto");
		}
	}		
	function ShellWrappers_BrowseForFolder(description)
	{
	    var retValue = "";
	    
	    try
	    {   
	        if ("<%=UseActivexWrapper%>" == "True")
	        {
	            var shellWrapper=new ActiveXObject("DocsPa_ActivexWrappers.ShellWrapper");
	            retValue=shellWrapper.BrowseForFolder(description);
	        }
	        else
	        {
	            var shell=new ActiveXObject("Shell.Application");
			    var folder=new Object;

			    folder=shell.BrowseForFolder(0,description,0);

			    if (folder!=null && folder.Items()!=null && folder.Items().Item()!=null)
			    {
				    var folderItem=folder.Items().Item();
				    retValue=folderItem.Path;
			    }
	        }
	    }
	    catch(e)
	    {
	        alert("Errore nella visualizzazione dello sfoglia per cartelle");
	    }
	    
        return retValue;
	}
</script>
<% this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "shellWrapperControlScript", string.Empty); } %>