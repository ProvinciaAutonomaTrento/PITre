<%@ Page language="c#" Codebehind="gestioneReport.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.gestione.report.gestioneReport" %>
<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
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
	<body MS_POSITIONING="GridLayout">
		<form id="gestioneReport" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Report" />
			<table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				<tr valign="top" height="100%">
					<td width="*">
						<cf1:IFrameWebControl id="iFrame_elenco" runat="server" Marginwidth="0" Marginheight="0" iWidth="433px"
							iHeight="100%" Frameborder="0" Scrolling="auto" NavigateTo="tabGestioneReport.aspx" Width="433px"></cf1:IFrameWebControl>
					</td>
					<td width="1"><img border="0" src="../../images/spaziatore.gif" width="1"></td>
					<!--TD vAlign="top" width="1" background="../../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2" height="100%"><IMG height="6" src="../../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<td width="1"><img border="0" src="../../images/spaziatore.gif" width="1"></td>
					<td width="*">
						<img border="0" src="../../images/spaziatore.gif" width="1">
						<cf1:IFrameWebControl id="iFrame_dettagli" runat="server" Marginwidth="0" Marginheight="0" iWidth="600"
							Frameborder="0" Scrolling="no" Width="100%" Height="138px"></cf1:IFrameWebControl>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
