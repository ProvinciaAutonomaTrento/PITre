<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdoStreamWrapper.ascx.cs" Inherits="SAAdminTool.ActivexWrappers.AdoStreamWrapper" %>
<% if (this.UseActivexWrapper && !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "AdoStreamWrapperObject")) { %> 
<OBJECT CLASSID="CLSID:2A3CCC9B-4B1C-42A4-AFC8-D324180E67DB" 
    CODEBASE="../activex/DocsPa_ActivexWrappers.CAB#version=3,6,0,0" 
    style="height: 0px; width: 0px"></OBJECT> 
<% this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "AdoStreamWrapperObject", string.Empty);} %><%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "adoStreamWrapperControlScript")){%>
<script id="adoStreamWrapperControlScript" type="text/javascript">
    function AdoStreamWrapper_OpenBinaryData(filePath)
    {
        try
        {
            var data=null;
            
            if ("<%=UseActivexWrapper%>" == "True")
            {
                var adoStreamWrapper=new ActiveXObject("DocsPa_ActivexWrappers.AdoStreamWrapper");
                data=adoStreamWrapper.OpenBinaryData(filePath);
            }
		    else
		    {
                var adoStream=new ActiveXObject("ADODB.Stream");
		        adoStream.Type = 1;
		        adoStream.Open();
		        adoStream.LoadFromFile(filePath);
		        data=adoStream.Read(-1);
		        adoStream.Close();
		        adoStream=null;
            }
            
            return data;
		}
		catch(e)
		{
		    alert("Errore nell'\scrittura del file '" + filePath + "'");
		}
    }
    
    function AdoStreamWrapper_SaveBinaryData(filePath, content)
    {
        try
        {
            var data=null;
            
            if ("<%=UseActivexWrapper%>" == "True")
            {
                var adoStreamWrapper=new ActiveXObject("DocsPa_ActivexWrappers.AdoStreamWrapper");
                adoStreamWrapper.SaveBinaryData(filePath, content);
            }
		    else
		    {
    
 		        var adoStream=CreateObject("ADODB.Stream");
		        adoStream.Type = 1;
		        adoStream.Open();
		        adoStream.Write(content);
		        adoStream.SaveToFile(filePath, 2);
		        adoStream.Close();
		        adoStream=null;
            }
		}
		catch(e)
		{
		    alert("Errore nella scrittura del file '" + filePath + "'");
		}
    }
</script>
<% this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "adoStreamWrapperControlScript", string.Empty); } %>