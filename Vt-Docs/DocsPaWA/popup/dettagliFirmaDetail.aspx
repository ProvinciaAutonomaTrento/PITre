<%@ Page language="c#" Codebehind="dettagliFirmaDetail.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.dettagliFirmaDetail" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>	
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:Table id="tblSignedDocument" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" Width="100%" BorderWidth="1px">
				<asp:TableRow BorderColor="DarkRed" BorderStyle="Solid" BackColor="DarkRed">
					<asp:TableCell BackColor="DarkRed" ForeColor="White" Font-Size="Smaller" Font-Names="Verdana" Font-Bold="True"
						Text="Parametro"></asp:TableCell>
					<asp:TableCell BackColor="DarkRed" ForeColor="White" Width="32px" Font-Size="Smaller" Font-Names="Verdana"
						Font-Bold="True" HorizontalAlign="Center"></asp:TableCell>
					<asp:TableCell BackColor="DarkRed" ForeColor="White" Font-Size="Smaller" Font-Names="Verdana" Font-Bold="True"
						Text="Valore"></asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</form>
	</body>
</HTML>
