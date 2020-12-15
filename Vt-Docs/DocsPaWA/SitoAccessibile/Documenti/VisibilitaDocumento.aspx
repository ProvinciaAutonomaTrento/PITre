<%@ Register TagPrefix="uc1" TagName="DettagliVisibilitaDocumento" Src="DettagliVisibilitaDocumento.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Page language="c#" Codebehind="VisibilitaDocumento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.VisibilitaDocumento" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
  <HEAD>
		<title>DOCSPA > Visibilità documento</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="../Css/main.css" type="text/css" media="screen" title="default">
			<link rel="alternate stylesheet" href="../Css/highcontrast.css" type="text/css" title="Alto contrasto">
				<link rel="stylesheet" href="../Css/menu.css" type="text/css" media="screen">
  </HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
				<div id="headerLogin">
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				</div>
				<div id="contentLogin">
					<uc1:DettagliVisibilitaDocumento id="DettagliVisibilitaDocumento" runat="server"></uc1:DettagliVisibilitaDocumento>
					<p class="centerButtons">
						<asp:button id="btnBack" CssClass="button" runat="server" Text="Torna al documento"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
