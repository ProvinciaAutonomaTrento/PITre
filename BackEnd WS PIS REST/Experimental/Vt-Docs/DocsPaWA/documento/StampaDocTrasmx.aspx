<%@ Page language="c#" Codebehind="StampaDocTrasmx.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.StampaDocTrasmx" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Stampa Documenti Trasmissioni" />
			<asp:Table id="tblTrasmx" style="Z-INDEX: 103; LEFT: 64px; POSITION: absolute; TOP: 56px" runat="server"
				Width="584px" EnableViewState="False" BackColor="White" BorderColor="#810D06" Font-Bold="True"
				CaptionAlign="Top" GridLines="Both" CssClass=".titolo_rosso"></asp:Table></form>
	</body>
</HTML>
