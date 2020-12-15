<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Import Namespace="DocsPAWA.DocsPaWR" %>
<%@ Import Namespace="DocsPAWA" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile.Rubrica" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="Rubrica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Rubrica.Rubrica" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Rubrica" Assembly="DocsPAWA" %>
<%
	string addSkipLink = "";
	string confButton = "";
	string cancButton = "";
	string searchMsg = "";
	string noSelMsg = "";
	
	if (!(SearchResult.PageRecipient == null || SearchResult.PageRecipient.Length == 0))
	{
		
		if (!(Result.PageRecipient == null || Result.PageRecipient.Length == 0))
		{
			addSkipLink = "<a href=\"#tbl-c\">Vai ai corrispondenti selezionati</a>";
			confButton = "<input id=\"ok\" type=\"submit\" class=\"button\" value=\"Conferma\" /> ";
			cancButton = "<input id=\"canc\" type=\"submit\" class=\"button\" value=\"Annulla\" />";		
		} else {
			addSkipLink = "<a href=\"#tbl\">Vai ai risultati della ricerca</a>";
			noSelMsg = "<p class=\"message\">Nessuna selezione effettuata</p>";
		}			
	}
				
	if (!(SearchResult.PageRecipient == null))
	{
		if (SearchResult.PageRecipient.Length == 0)
		{
			searchMsg = "<p class=\"message\">Nessun corrispondente trovato</p>";
		}			
	}
%>
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
		     <%} %>  > Rubrica</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
		<meta http-equiv="Content-Language" content="it-IT" />
		<meta name="keywords" content="DocsPA, Protocollo informatico" />
		<meta name="description" content="Sistema per la gestione del protocollo informatico" />
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet" />
		<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet" />
	</HEAD>
	<body>
		<div id="container">
			<div class="skipLinks">
				<%=addSkipLink%>
				<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
			</div>
			<form id="frmUserControls" runat="server" class="topRubrica">
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
			</form>
			<div id="content">
				<%=searchMsg%>
				<!--#include virtual="../Includes/_RubricaTop.aspx"-->
				<!--#include virtual="../Includes/_RubricaMiddle.aspx"-->
				<%=noSelMsg%>
				<!--#include virtual="../Includes/_RubricaBottom.aspx"-->
				<form method="post" action="./Rubrica.aspx">
					<input type="hidden" id="confirm" name="action" value="confirm" />
					<p class="centerButtons">
						<%=confButton%>
					</p>
				</form>
				<form method="post" action="./Rubrica.aspx">
					<input type="hidden" id="cancel" name="action" value="cancel" />
					<p class="centerButtons">
						<%=cancButton%>
					</p>
				</form>
			</div>
		</div>
	</body>
</HTML>
