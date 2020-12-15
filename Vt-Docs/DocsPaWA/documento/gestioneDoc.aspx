<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="gestioneDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.gestioneDoc" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
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
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Documento" />
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr vAlign="top" height="100%">
					<td vAlign="top" width="360">
						<cf1:iframewebcontrol id="iFrame_sx" runat="server" Width="415px" Scrolling="no" Frameborder="0" iHeight="100%"
							iWidth="415" Marginheight="0" Marginwidth="0" BorderWidth="1px" BackColor="Red"></cf1:iframewebcontrol>
					</td>
					<td width="1"><IMG src="../images/spaziatore.gif" width="1" border="0"></td>
					<!--TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" height="100%" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<td width="1"><IMG src="../images/spaziatore.gif" width="1" border="0"></td>
					<td width="*">
						<cf1:iframewebcontrol id="iFrame_dx" runat="server" Scrolling="auto" Frameborder="0" iHeight="100%" iWidth="100%"
							Marginheight="0" Marginwidth="10" name="iFrame_dx"></cf1:iframewebcontrol>
					</td>
				</tr>
			</table>
			<iframe id="please_wait" style="DISPLAY: none; POSITION: absolute;" src="../waitingSpedisci.htm"
				width="400" height="100"></iframe>
    	</form>
	</body>
</HTML>
