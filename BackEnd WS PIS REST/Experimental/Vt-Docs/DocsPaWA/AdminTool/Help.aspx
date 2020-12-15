<%@ Page language="c#" Codebehind="Help.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Help" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat = "server">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function body_onLoad()
			{
				var newLeft=0;
				var newTop=0;
				window.moveTo(newLeft,newTop);
			}
		</script>        
  </HEAD>
	<body leftmargin="5" topmargin="5" MS_POSITIONING="GridLayout" onload=body_onLoad();>
	    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Help" />
		<table border="0" cellpadding="0" cellspacing="0" width="420" align=center>
			<tr>
				<td height="10" align="right" class="testo_grigio_scuro">|&nbsp;&nbsp;&nbsp;<a href="javascript:self.close()">Chiudi</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
			</tr>
			<tr>
				<td height="48" align="left"><img src="Images/logo.gif" border="0" height="48" width="218"></td>
			</tr>
			<tr>
				<td height="15"></td>
			</tr>
			<tr>
				<td height="20" bgcolor="#c0c0c0" class="testo_grigio_scuro" align="middle">
					<asp:Label id="lbl_testa" runat="server"></asp:Label></td>
			</tr>
			<tr>
				<td height="15"></td>
			</tr>
			<tr>
				<td id="textArea" class="testo" height="100%" width="100%" runat="server">
				</td>
			</tr>
		</table>
	</body>
</HTML>
