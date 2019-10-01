<%@ Page language="c#" Codebehind="Exit.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.Exit" %>
<%@ Register src="UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</head>
	
	<body onload="on_load()" MS_POSITIONING="GridLayout">
		<script language=javascript>
			function on_load()
			{
				window.close();			
			}
		</script>
		    <uc1:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Exit" />
		<table width="100%" height="100%" ID="Table1">
			<tr align="center" valign="middle">
				<td class="testo_grigio">
					<label id="lbl_msg" Class="testo_grigio" style="FONT-SIZE: medium;  COLOR: black;  FONT-FAMILY: verdana;  Arial: ; tahoma: ">
						Chiusura in corso...</label></td>
			</tr>
		</table>
	</body>	
</html>
