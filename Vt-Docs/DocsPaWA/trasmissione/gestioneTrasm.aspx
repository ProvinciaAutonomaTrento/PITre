<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="gestioneTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.trasmissione.gestioneTrasm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > gestioneTrasm</title>
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
	<body onmouseover="Body_OnMouseOver()" onload="Body_OnLoad()" MS_POSITIONING="GridLayout"
		topmargin="1">
		<!--<body MS_POSITIONING="GridLayout" topmargin="1">-->
		<form id="gestioneTrasm" method="post" runat="server">
			<TABLE id="Table1" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TR vAlign="top" height="100%">
					<TD vAlign="top" width="360">
						<cf1:IFrameWebControl id="iFrame_sx" runat="server" Marginwidth="0" Marginheight="2" iWidth="415" iHeight="100%"
							Frameborder="0" Scrolling="auto" Width="415px"></cf1:IFrameWebControl></TD>
					<TD width="1"><IMG src="../images/spaziatore.gif" width="1" border="0"></TD>
					<!--TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" height="100%" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<TD width="1"><IMG src="../images/spaziatore.gif" width="1" border="0"></TD>
					<TD width="*">
						<cf1:IFrameWebControl id="iFrame_dx" runat="server" Marginwidth="10" Marginheight="2" iWidth="100%" iHeight="100%"
							Frameborder="0" Scrolling="auto" name="iFrame_dx"></cf1:IFrameWebControl></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
