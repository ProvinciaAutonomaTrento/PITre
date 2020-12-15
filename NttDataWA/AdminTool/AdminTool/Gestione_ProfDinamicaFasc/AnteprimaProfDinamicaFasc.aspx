<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="AnteprimaProfDinamicaFasc.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc.AnteprimaProfDinamicaFasc" validateRequest="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/ETcalendar.js"></script>
		<script>
			function clearSelezioneEsclusiva(id, numeroDiScelte)
			{
				numeroDiScelte--;
				while(numeroDiScelte >= 0)
				{
					var elemento = id+"_"+numeroDiScelte;
					document.getElementById(elemento).checked = false;
					numeroDiScelte--;
				}
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Anteprima" />
			<table width="100%">
				<tr>
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20"><asp:label id="lbl_NomeModello" runat="server" Width="442"></asp:label></td>
					<td align="center"><asp:Button id="btn_chiudi" runat="server" CssClass="testo_btn_p" Text="Chiudi" OnClientClick="window.close();"></asp:Button></td>
				<tr>
					<td colspan="2" bgColor="#f6f4f4" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid"><asp:panel id="panel_Contenuto" runat="server"></asp:panel></td>				
				</tr>
			</table>
		</form>
	</body>
</HTML>
