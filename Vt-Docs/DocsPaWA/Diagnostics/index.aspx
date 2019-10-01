<%@ Page language="c#" Codebehind="index.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.Diagnostics.index" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Diagnostica</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="index" method="post" runat="server">
			<P>Diagnostica di DocsPA 3.0</P>
			<P>Premi qui per iniziale il test:
				<asp:button id="StartButton" runat="server" Text="Inizia"></asp:button>&nbsp;<asp:button id="ResetButton" runat="server" Text="Reset"></asp:button></P>
			<P>Connessione al Database =
				<asp:label id="DatabaseConnectionTest" runat="server">Da Effettuare</asp:label></P>
			<P>
				<asp:TextBox id="ExceptionMessage1" runat="server" Width="353px" Height="86px" TextMode="MultiLine"></asp:TextBox></P>
			<P>Connessione a tabella&nbsp;DPA_Amministra =
				<asp:label id="DPA_Amministra" runat="server">Da Effettuare</asp:label></P>
			<P>
				<asp:TextBox id="ExceptionMessage2" runat="server" TextMode="MultiLine" Height="86px" Width="353px"></asp:TextBox></P>
		</form>
	</body>
</HTML>
