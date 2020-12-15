<%@ Page language="c#" Codebehind="AnteprimaProfDinRicerche.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.AnteprimaProfDinRicerche" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">		
		<script language="JavaScript" src="../CSS/ETcalendar.js"></script>
		<script language="javascript">
		function chiudiFinestra(){
			window.close();
		}
		</script>
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
        <asp:ScriptManager ID="ScriptManagerProfDinamRicerche" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Anteprima" />
			<table width="100%">
				<tr>
					<td class="titolo" 
						align="center" bgColor="#e0e0e0" height="20"><asp:label id="lbl_NomeModello" runat="server" Font-Bold="True" Width="190"></asp:label></td>
					<td class="titolo" 
						align="center" bgColor="#e0e0e0" height="20">
						<asp:Button id="btn_ConfermaProfilazione" runat="server" Text="Conferma" CssClass="pulsante"
							Width="80px"></asp:Button></td>
					<td class="titolo" 
						align="center" bgColor="#e0e0e0" height="20">
						
                            <asp:Button id="btn_resettaPopUp" runat="server" Text="Resetta" CssClass="pulsante" Width="80px"></asp:Button></td>
					<td class="titolo" 
						align="center" bgColor="#e0e0e0" height="20">
						<asp:Button id="btn_Chiudi" runat="server" Text="Chiudi" CssClass="pulsante" Width="80px"></asp:Button></td>
				</tr>
				<tr>
					<td align="center" colSpan="4"><asp:label id="Label_Avviso" runat="server" Font-Bold="True" Font-Size="12px" ForeColor="Red"
							Visible="False"></asp:label></td>
				</tr>
				<tr>
					<td class="td_profDinamica"
						colSpan="4"><asp:panel id="panel_Contenuto" runat="server"></asp:panel></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
