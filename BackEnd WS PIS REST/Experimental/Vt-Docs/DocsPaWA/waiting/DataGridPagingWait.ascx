<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DataGridPagingWait.ascx.cs" Inherits="DocsPAWA.waiting.DataGridPagingWait" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
		if (eventTarget!=null)
		{
			var doWait=false;

			var elements=eventTarget.split("$");
			
			if (elements.length>=3)
			{
				doWait=(elements[2].indexOf("ctl")>-1);
			}

			if (doWait)
			{
				var dataGridId="<% =DataGridID %>";

				if (eventTarget.indexOf(dataGridId) > -1)
				{
					// Se l'evento è stato generato dal datagrid registrato,
					// viene richiamato lo script di callback
					<% =WaitScriptCallback %>
				}
			}
		}
	}

</script>