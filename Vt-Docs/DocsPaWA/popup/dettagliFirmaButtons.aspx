<%@ Page language="c#" Codebehind="dettagliFirmaButtons.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.dettagliFirmaButtons" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>	
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
		function CloseMask()
		{
			this.close();
		}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgColor="buttonface">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table2" style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 0px" align="center"
				border="0" width="100%">
				<TR>
					<TD class="pulsante" align="center" height="30">
						<asp:Button id="btnVisualizza" runat="server" CssClass="pulsante" Text="Visualizza"></asp:Button>&nbsp;
						<asp:Button id="btnChiudi" runat="server" CssClass="pulsante" Text="Chiudi"></asp:Button>
					</TD>
				</TR>
			</TABLE>
			&nbsp;
		</form>
	</body>
</HTML>
