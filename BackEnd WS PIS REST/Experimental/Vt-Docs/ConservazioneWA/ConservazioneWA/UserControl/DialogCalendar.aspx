<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogCalendar.aspx.cs" Inherits="ConservazioneWA.UserControl.DialogCalendar"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Calendario - Seleziona...</title>
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
				window.returnValue=document.frmDialogCalendario.txtSelectedDate.value;
				window.close();
			}
					
		</SCRIPT>
</head>
<body>
    <form id="frmDialogCalendario" method="post" runat="server">    
			<asp:Calendar id="ctlCalendario" runat="server"  SkinID="calendario"/>                                                  
			<INPUT id="txtSelectedDate" type="hidden" name="txtSelectedDate" runat="server">
		</form>
</body>
</html>
