<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Page language="c#" Codebehind="Fascicolo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Fascicoli.Fascicolo" %>
<%@ Register TagPrefix="uc1" TagName="DettagliFascicolo" Src="DettagliFascicolo.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Dettagli del fascicolo</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/docsMenu.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/treeView.css" type="text/css" rel="stylesheet">
		<script type="text/javascript" src="../WebControls/TreeView.js"></script>
	</HEAD>
	<body>
		<form id="frmFascicolo" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<A href="#content">Vai al contenuto</A>
					<A href="#navbar">Vai al menu utente</A>
				</div>
				<div id="header">
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				</div>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<div id="content">
					<uc1:DettagliFascicolo id="DettagliFascicolo" runat="server"></uc1:DettagliFascicolo>
					<p class="centerButtons">
						<asp:button id="btnBack" runat="server" Text="Torna ai risultati" CssClass="button"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
		<script type="text/javascript">
			<!--
			ddtreemenu.createTree("treemenu1", false)
			//-->
		</script>
	</body>
</HTML>
