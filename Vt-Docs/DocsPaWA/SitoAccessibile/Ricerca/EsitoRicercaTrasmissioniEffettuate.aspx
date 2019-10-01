<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Page language="c#" Codebehind="EsitoRicercaTrasmissioniEffettuate.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Ricerca.EsitoRicercaTrasmissioniEffettuate" %>
<%@ Register TagPrefix="uc1" TagName="TrasmissioniEffettuate" Src="TrasmissioniEffettuate.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
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
		     <%} %>  > Ricerca - Trasmissioni effettuate trovate <%=GetTitleExtraInformation() %></title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="../Css/main.css" type="text/css" media="screen" title="default">
			<link rel="stylesheet" href="../Css/menu.css" type="text/css" media="screen">
	</HEAD>
	<body>
		<form id="frmEsitoRicercaTrasmissioniEffettuate" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
				
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<div id="content">
					<p id="pnlMessage" runat="server" class="message">
					</p>
					<uc1:TrasmissioniEffettuate id="trasmissioniEffettuate" runat="server"></uc1:TrasmissioniEffettuate>
					<p class="centerButtons">
						<asp:Button id="btnBack" CssClass="button" Text="Torna alla ricerca" runat="server" />
					</p>
				</div>
			</div>
		</form>
	</body>
</HTML>
