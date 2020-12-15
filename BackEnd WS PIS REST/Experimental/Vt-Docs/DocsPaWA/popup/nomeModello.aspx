<%@ Page language="c#" Codebehind="nomeModello.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.nomeModello" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function chiudiPopup()
			{
				window.returnValue = document.getElementById("txt_nomeModello").value;
				window.close();
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="nomeModello" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Nome Modello" />
			<TABLE id="tbl_nome" class="info" width="400" align="center" border="0">
				<TR>
					<TD height="5"></TD>
				</TR>
				<TR height="22">
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
						<asp:label id="lbl_nome" runat="server" CssClass="titolo_scheda">Nome Modello&nbsp;</asp:label>
						<asp:textbox id="txt_nomeModello" runat="server" CssClass="testo_grigio" Width="290px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD height="10"></TD>
				</TR>
				<TR>
					<TD align="center" height="30">
						<INPUT type="button" id="btn_Insert" Class="pulsante" value="INSERISCI" onclick="chiudiPopup()">
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
