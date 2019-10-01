<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="AnteprimaProfDinamicaFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.AnteprimaProfDinamicaFasc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<META HTTP-EQUIV="Pragma" CONTENT="no-cache">
        <META HTTP-EQUIV="Expires" CONTENT="-1">	
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/ETcalendar.js"></script>
		<script language="javascript">
			function chiudiFinestra(){
				window.close();
			}
		</script>		
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManagerProfDinam" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Anteprima" />
			<table width="100%">
				<tr>
					<td class="titolo" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid"
						align="center" Width="85%" bgColor="#e0e0e0" height="20">
						<asp:label id="lbl_NomeModello" runat="server" Font-Bold="True"></asp:label>
					</td>
					<td class="titolo"
						align="center" width="15%" bgColor="#e0e0e0" height="20">
						<asp:Button id="btn_Chiudi" runat="server" Text="Chiudi" CssClass="pulsante" Width="80px"></asp:Button>
					</td>
				</tr>
				<tr>
					<td class="td_profDinamica"
						colSpan="2"><asp:panel id="panel_Contenuto" runat="server"></asp:panel></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
