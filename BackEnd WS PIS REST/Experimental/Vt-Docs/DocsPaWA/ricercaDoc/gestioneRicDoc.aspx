<%@ Page language="c#" Codebehind="gestioneRicDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.GestioneRicDoc" %>
<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > gestioneRicDoc</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script>
		function Body_OnMouseOver()
		{
			try
			{
			
				if(top.superiore.document!=null)
					if(top.superiore.document.Script!=null)
					if(top.superiore.document.Script["closeIt"]!=null)
						top.superiore.document.Script.closeIt();
			}
			catch(e)
			{
				//alert(e.message);
			}
			
		}
		function Body_OnLoad()
		{
			try
			{								
					if(top.superiore.document!=null)
						if(top.superiore.document.Script!=null)
						if(top.superiore.document.Script["CheckTestataTastoSel"]!=null)
							top.superiore.document.Script.CheckTestataTastoSel();
			}
			catch(e)
			{
			//alert(e.message);
			}		
		}	
				
		</script>
	</HEAD>
	<body onmouseover="Body_OnMouseOver()" onload="Body_OnLoad()" MS_POSITIONING="GridLayout">
		<form id="gestioneRicDoc" method="post" runat="server">
			<table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				<tr valign="top">
					<td width="360" valign="top">
						<cf1:IFrameWebControl id="iFrame_sx" runat="server" Marginwidth="0" Marginheight="2" iWidth="415" iHeight="100%"
							Frameborder="0" Scrolling="no" Width="415px"></cf1:IFrameWebControl>
					</td>
					<td width="1"><img border="0" src="../images/spaziatore.gif" width="1"></td>
					<!--TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<td width="1"><img border="0" src="../images/spaziatore.gif" width="1"></td>
					<td width="*">
						<cf1:IFrameWebControl id="iFrame_dx" runat="server" Marginwidth="10" Marginheight="2" iWidth="100%" iHeight="100%"
							Frameborder="0" Scrolling="nos"></cf1:IFrameWebControl>
					</td>
				</tr>
			</table>
			&nbsp;
		</form>
	</body>
</HTML>
