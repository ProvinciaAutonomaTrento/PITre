<%@ Import Namespace="DocsPAWA.DocsPaWR" %>
<%@ Import Namespace="DocsPAWA" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile.Rubrica" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="Dettagli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Rubrica.Dettagli" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Rubrica" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Dettagli del corrispondente</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<div id="container">
			<div class="skipLinks">
				<A href="#content">Vai al contenuto</A>
				<A href="#navbar">Vai al menu utente</A>
			</div>
			<div id="headerLogin">
				<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
			</div>
				<div id="contentLogin">
			<%
				if (ElementoRubrica!=null && ElementoRubrica.tipo=="U")
				{
			%>
					<!--#include virtual="Dettagli\_UDett.aspx"-->		
			<%	
				}	
				else if (ElementoRubrica!=null && ElementoRubrica.tipo=="R")
				{
			%>
					<!--#include virtual="Dettagli\_RDett.aspx"-->		
			<%	
				}
				else if (ElementoRubrica!=null && ElementoRubrica.tipo=="P")
				{
			%>
					<!--#include virtual="Dettagli\_PDett.aspx"-->		
			<%	
				}
				else if (ElementoRubrica!=null && ElementoRubrica.tipo=="L")
				{
			%>
					<!--#include virtual="Dettagli\_LDett.aspx"-->		
			<%	
				}
				else
				{
			%>
					<span>
						<label>Corrispondente non trovato</label>
					</span>		
			<%	
				
				}
			%>
			<%
				if (this.Context.Request.Params["er"]!=null)
				{
			%>
						<div class="riga">
							<a href="./Dettagli.aspx?cod=<%=this.Escape((string)this.Context.Request.Params["er"])%>">Indietro</a>
						</div>
			<%
				}
				else
				{
			%>
					<form id="frmBack" method="post" runat="server">
						<p class="centerButtons">
							<asp:Button ID="btnBack" Runat="server" Text="Torna alla rubrica" CssClass="button"></asp:Button>
						</p>
					</form>
			<%
				}
			%>		
				</div> <!-- end content -->
			</div> <!-- end container -->
	</body>
</html>
