<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="gestioneRicFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicoli.gestFasc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>DOCSPA > gestioneRicFasc</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
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
		<form id="gestioneDoc" method="post" runat="server">
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr vAlign="top">
					<td vAlign="top" width="360"><cf1:iframewebcontrol id="iFrame_sx" runat="server" Scrolling="auto" Frameborder="0" iHeight="100%" iWidth="430"
							Marginheight="2" Marginwidth="0" name="iFrame_sx"></cf1:iframewebcontrol></td>
					<td width="1"><IMG src="../images/spaziatore.gif" width="1" border="0"></td>
					<!--TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<td width="1"><IMG src="../images/spaziatore.gif" width="1" border="0"></td>
					<td width="*"><cf1:iframewebcontrol id="iFrame_dx" runat="server" Scrolling="auto" Frameborder="0" iHeight="100%" iWidth="100%"
							Marginheight="2" Marginwidth="10" name="iFrame_dx"></cf1:iframewebcontrol></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
