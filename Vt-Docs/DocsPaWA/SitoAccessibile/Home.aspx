<%@ Import Namespace="DocsPAWA.DocsPaWR" %>
<%@ Import Namespace="DocsPAWA" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="UserContext.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="Home.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Home" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="uc1" TagName="TrasmissioniRicevute" Src="Ricerca/TrasmissioniRicevute.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MainMenu" Src="MainMenu.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title><%
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
            {
             %>
                <%= titolo%>
             <%
                   }
            else
            {
             %>
                DOCSPA
		     <%} %> > Homepage</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="./Css/main.css" type="text/css" media="screen" title="default">
			<link rel="alternate stylesheet" href="./Css/highcontrast.css" type="text/css" title="Alto contrasto">
				<link rel="stylesheet" href="./Css/menu.css" type="text/css" media="screen">
	</HEAD>
	<body>
		<form id="frmHome" method="post" action="Home.aspx" runat="server">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
				<% if (existsLogoAmm)
       { %>
				<div id="headerLoginCustom">
				<%}
       else
       { %>
				<div id="header">
				<%} %>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				</div>
				<uc2:MainMenu id="mainMenu" runat="server"></uc2:MainMenu>
				<div id="content">
					<div class="selRole">
						<div class="currRole">
							<!-- <span>Ruolo attuale: -->
							<!-- </span> -->
						</div>
						<fieldset>
							<legend>Selezione del ruolo</legend>
							<p class="labelFieldPair">
								<label for="cboRuoli">Ruoli disponibili</label>
								<asp:DropDownList id="cboRuoli" Runat="server"></asp:DropDownList>
							</p>
							<asp:Button ID="btnSetRole" Runat="server" Text="Imposta" CssClass="button"></asp:Button>
						</fieldset>
					</div>
					<div class="currRole">
						<span>Cose da fare :
						<asp:DropDownList ID="cboSearchType" Runat="server" Width="100px"></asp:DropDownList>
						<asp:button id="btnSearch" runat="server" Text="Mostra" CssClass="button"></asp:button>
						</span>
					</div>
					<div id="todoListContainer" runat="server">
						<uc1:TrasmissioniRicevute id="trasmissioniRicevute" runat="server"></uc1:TrasmissioniRicevute>
					</div>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
