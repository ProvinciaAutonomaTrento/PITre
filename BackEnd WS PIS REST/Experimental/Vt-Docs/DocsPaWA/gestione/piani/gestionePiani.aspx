<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestionePiani.aspx.cs" Inherits="DocsPAWA.gestione.piani.gestionePiani" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>gestionePiani</title>
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
				alert(e.message);
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
			alert(e.message);
			}		
		}
		</script>
	</HEAD>
<body MS_POSITIONING="GridLayout" onmouseover="Body_OnMouseOver()" onload="Body_OnLoad()">
		<form id="gestionePiani" method="post" runat="server">
			<table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				<tr valign="top" height="100%">
					<td width="316" valign="top" style="WIDTH: 300px">
						<cf1:IFrameWebControl id="iFrame_sx" runat="server" Marginwidth="0" Marginheight="2" iWidth="415"
							iHeight="100%" Frameborder="0" Scrolling="no" NavigateTo="elencoPiani.aspx" Width="415px"></cf1:IFrameWebControl>
					</td>
					<td width="1"><img border="0" src="../../images/spaziatore.gif" width="1"></td>
					<!--TD vAlign="top" width="1" background="../../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2" height="100%"><IMG height="6" src="../../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<td width="1"><img border="0" src="../../images/spaziatore.gif" width="1"></td>
					<td width="*">
						<cf1:IFrameWebControl id="iFrame_dx" runat="server" Marginwidth="10" Marginheight="2" iWidth="100%"
							iHeight="100%" Frameborder="0" Scrolling="no" NavigateTo="whitepage.htm"></cf1:IFrameWebControl>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
