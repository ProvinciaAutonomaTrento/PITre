<%@ Page language="c#" Codebehind="DialogCalendario.aspx.cs" AutoEventWireup="false" Inherits="ProtocollazioneIngresso.DialogCalendario" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > Seleziona...</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<SCRIPT language="JavaScript">						
			window.name="DialogCalendario";			
			var dialogRetValue=window.dialogArguments;	
			
			function CloseWindow()
			{
				window.returnValue=document.frmDialogCalendario.txtSelectedDate.value;
				window.close();
			}
					
		</SCRIPT>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="frmDialogCalendario" method="post" runat="server">
			<asp:Calendar id="ctlCalendario" class="info_grigio" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px"
				runat="server" Width="100%" Height="100%" FirstDayOfWeek="Monday" ShowGridLines="True"></asp:Calendar>
			<INPUT id="txtSelectedDate" type="hidden" name="txtSelectedDate" runat="server">
		</form>
	</body>
</HTML>
