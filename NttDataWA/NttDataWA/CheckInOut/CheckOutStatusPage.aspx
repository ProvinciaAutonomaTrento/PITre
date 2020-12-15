<%@ Page language="c#" Codebehind="CheckOutStatusPage.aspx.cs" AutoEventWireup="false" Inherits="NttDataWA.CheckInOut.CheckOutStatusPage" %>
<%@ Register TagPrefix="uc1" TagName="CheckOutStatusPanel" Src="CheckOutStatusPanel.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Informazioni sul documento bloccato</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript">
		
			function ClosePage()
			{
				window.close();
			}
		
		</script>
	</HEAD>
	<body topmargin="2">
		<form id="frmCheckOutStatus" method="post" runat="server">
			<div align="center">
				<uc1:CheckOutStatusPanel id="checkOutStatusPanel" runat="server"></uc1:CheckOutStatusPanel>
			</div>
			<br>
			<div align="center">
				<asp:Button id="btnClose" Runat="server" class="PULSANTE" Text="Chiudi"></asp:Button>
			</div>
		</form>
	</body>
</HTML>
