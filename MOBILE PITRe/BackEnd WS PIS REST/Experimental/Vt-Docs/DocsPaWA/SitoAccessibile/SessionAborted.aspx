<%@ Page language="c#" Codebehind="SessionAborted.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.SessionAborted" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
	<head>
		<title>DOCSPA > Sessione scaduta</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
		<meta http-equiv="Content-Language" content="it-IT" />
		<meta name="keywords" content="DocsPA, Protocollo informatico" />
		<meta name="description" content="Sistema per la gestione del protocollo informatico" />
		<link rel="stylesheet" href="./Css/main.css" type="text/css" media="screen" title="default" />
	</head>
	<body>
		<form id="frmSessionAborted" method="post" runat="server">
			<div id="container">
				<div class="skipLinks"><a href="#content">Vai al contenuto</a> <a href="#docsMenu">Vai 
						al menu documento</a> <a href="#navbar">Vai al menu utente</a>
				</div>
				<div id="headerLogin"><!-- --></div>
				<div id="contentLogin">
					<p id="pnlMessage" runat="server" class="message">
					<pre>
					</pre>
					</p>
					<p class="centerButtons">
						<asp:Button ID="btnLogin" Text="Accedi" Runat="server" CssClass="button"></asp:Button>
					</p>
				</div>
			</div>
		</form>
	</body>
</html>
