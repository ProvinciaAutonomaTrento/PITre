<%@ Page language="c#" Codebehind="StatoConnUtenti.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Utenti.StatoConnUtenti" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<script language="javascript">
function body_onLoad()
{
	var newLeft=0;
	var newTop=0;
	window.moveTo(newLeft,newTop);
}
</script>
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	<script language="JavaScript">
<!--
function refresh()
{
    window.location.reload();
}
//-->
</script>

	</HEAD>
	<body leftMargin="0" topMargin="0" MS_POSITIONING="GridLayout" onload=body_onLoad();>
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Stato degli utenti connessi" />
			<table cellSpacing="8" cellPadding="0" width="480" align="center" border="0">
				<tr>
					<td><IMG height="48" src="../Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="center" bgColor="#e0e0e0">Stato degli utenti 
						connessi.</td>
				</tr>
				<tr>
					<td><asp:label id="lbl_msg" runat="server" CssClass="testo_rosso"></asp:label></td>
				</tr>
				<tr>
					<td height="10"></td>
				</tr>
				<tr>
					<td class="testo" id="outputArea" runat="server"></td>
				</tr>
				<tr>
					<td height="10"></td>
				</tr>
				<tr>
					<td align="center"><asp:button id="btn_chiudi" runat="server" CssClass="testo_btn" Text="Chiudi"></asp:button>
					&nbsp;
                    <asp:button id="btn_SconnettiTutti" runat="server" CssClass="testo_btn" Text="Sconnetti tutti"></asp:button>
					&nbsp;
					<input type="button" class="testo_btn" value="Aggiorna Pagina" onclick="refresh();" />
					</td>
				</tr>
				
			</table>
		</form>
	</body>
</HTML>
