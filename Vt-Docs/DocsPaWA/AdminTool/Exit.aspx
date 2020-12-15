<%@ Page language="c#" Codebehind="Exit.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Exit" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language=javascript>
			function Chiude()
			{		
				/*
				if(window.opener!=null)
				{
					alert("opener name=" + window.opener.name);
					window.opener.close();
				}	
				*/
				window.close(); 
			}
		</script>
	</HEAD>
	<body onload="Chiude();">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Logout amministrazione" />
		<p align="center" class="testo_grande">
			Logout amministrazione....
		</p>
	</body>
</HTML>
