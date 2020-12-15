<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridViewPagingWait.ascx.cs" Inherits="DocsPAWA.waiting.GridViewPagingWait" %>
<script language="javascript">
	
	try
	{
		var f = __doPostBack;
		__doPostBack = function(eventTarget, eventArgument)
		{
			f(eventTarget, eventArgument);
				DoWait(eventTarget, eventArgument);
		};
	}
	catch (ex)
	{
		// funzione "__doPostBack" ancora non disponibile
	}
	
	function DoWait(eventTarget,eventArgument)
	{               
	    var dataGridId="<% =DataGridID %>";
	   
		if (eventTarget == dataGridId)
		{
            // Se l'evento è stato generato dal datagrid registrato,
            // viene richiamato lo script di callback
            <% =WaitScriptCallback %>
		}
	}
</script>