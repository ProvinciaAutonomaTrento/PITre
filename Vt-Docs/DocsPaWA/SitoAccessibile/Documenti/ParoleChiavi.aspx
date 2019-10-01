<%@ Register TagPrefix="uc1" TagName="ListaParoleChiavi" Src="ListaParoleChiavi.ascx" %>
<%@ Page language="c#" Codebehind="ParoleChiavi.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.ParoleChiavi" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > Parole chiave</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="../Css/main.css" type="text/css" media="screen" title="default">
			<link rel="alternate stylesheet" href="../Css/highcontrast.css" type="text/css" title="Alto contrasto">
				<link rel="stylesheet" href="../Css/menu.css" type="text/css" media="screen">
	</HEAD>
	<body>
		<form id="frmParoleChiavi" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
				<div id="headerLogin">
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				</div>
				<div id="content">
					<uc1:ListaParoleChiavi id="listaParoleChiavi" runat="server"></uc1:ListaParoleChiavi>
					<p class="centerButtons">
						<asp:button id="btnOK" CssClass="button" runat="server" Text="OK"></asp:button>
						<asp:button id="btnBack" CssClass="button" runat="server" Text="Indietro"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
