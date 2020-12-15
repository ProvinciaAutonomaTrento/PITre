<%@ Page language="c#" Codebehind="SessionAborted.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SessionAborted" %>
<%@ Register src="UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function body_onLoad________old()
			{
				var newWidth  = 570;
				var newHeight = 320;

				window.resizeTo(newWidth, newHeight);
				
				var newLeft = (screen.availWidth  - newWidth)  / 2;
				var newTop  = (screen.availHeight - newHeight) / 2;
				window.moveTo(newLeft, newTop);				
			}
			function body_onLoad()
			{
				window.resizeTo(screen.availWidth, screen.availHeight);	
				window.moveTo(0, 0);														
			}
		</script>
	</HEAD>
	<body onload="body_onLoad()">
		    <uc1:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Exit" />
		<TABLE cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
			<TR>
				<TD align="left" bgColor="#810d06" colSpan="2"><asp:image runat="server" ImageUrl="images/logo.gif" border="0" width="141" height="58" ID="img_logologin"></asp:image></TD>
			</TR>
			<TR>
				<TD width="100%" bgColor="#9e9e9e" colSpan="2" height="20"><IMG height="1" src="images/spacer.gif" width="1" border="0"></TD>
			</TR>
			<TR height="40">
				<TD colSpan="2"></TD>
			</TR>
			<TR>
				<TD align="center" colSpan="2"><asp:label id="errorLabel" Visible="false" BackColor="White" ForeColor="Maroon" Font-Name="verdana"
						Font-Size="10" Font-Bold="True" Width="100%" Runat="server"></asp:label></TD>
			</TR>
			<TR>
				<TD colSpan="2" height="70"></TD>
			</TR>
			<TR>
			<TR>
				<TD class="testo_grigio_scuro" align="center" colSpan="2">Seleziona 'Accedi' per 
					tornare alla pagina di login.
				</TD>
			</TR>
			<TR>
				<TD colSpan="2" height="10"></TD>
			</TR>
			<TR>
				<TD align="center" colSpan="2"><A class="testo_grigio_scuro" href="login.htm"><IMG src="images/Butt_Accedi.jpg" border="0"></A>
				</TD>
			</TR>
		</TABLE>
	</body>
</HTML>
