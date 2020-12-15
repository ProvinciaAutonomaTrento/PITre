<%@ Page language="c#" Codebehind="DialogCalendar.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.UserControls.DialogCalendar" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		
		<base target="_self" />
		<SCRIPT language="JavaScript">						
			window.name="DialogCalendar";			
			var dialogRetValue=window.dialogArguments;	
			
			function CloseWindow()
			{
				window.returnValue=document.getElementById("txtSelectedDate").value;
				window.close();
			}
					
		</SCRIPT>
	</HEAD>
	<body>
		<form id="frmDialogCalendario" method="post" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Calendario"/>
			<asp:Calendar id="ctlCalendario" runat="server" SkinID="calendario"/>                                                  
			<INPUT id="txtSelectedDate" type="hidden" name="txtSelectedDate" runat="server">
		</form>
	</body>
</HTML>
