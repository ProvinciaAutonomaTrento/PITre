<%@ Register TagPrefix="uc1" TagName="ValidationContainer" Src="../Validations/ValidationContainer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="Documenti.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Ricerca.Documenti" %>
<%@ Register TagPrefix="uc1" TagName="FiltriDocumenti" Src="FiltriDocumenti.ascx" %>
<%@ Register TagPrefix="uc1" TagName="FiltriDocumentiAvanzati" Src="FiltriDocumentiAvanzati.ascx" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
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
		     <%} %>  > Ricerca Documenti</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="../Css/main.css" type="text/css" media="screen" title="default">
			<link rel="stylesheet" href="../Css/menu.css" type="text/css" media="screen">
	</HEAD>
	<body>
		<form ID="frmDocumenti" method="post" runat="server">
			<div id="container">
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<div id="content">
					<uc1:ValidationContainer id="validationContainer" runat="server"></uc1:ValidationContainer>
					<fieldset>
						<legend>Tipo di profilo</legend>
						<asp:CheckBox ID="chkProtocolloArrivo" Text="Protocollo in arrivo" Runat="server"></asp:CheckBox>&nbsp;
						<asp:CheckBox ID="chkProtocolloPartenza" Text="Protocollo in partenza" Runat="server"></asp:CheckBox>&nbsp;
						<asp:CheckBox ID="chkProtocolloInterno" Text="Protocollo interno" Runat="server"></asp:CheckBox>&nbsp;
						<asp:CheckBox ID="chkDocumentoGrigio" Text="Documento grigio" Runat="server"></asp:CheckBox>&nbsp;
						<asp:Button ID="btnSelectTipoDocumento" Runat="server" CssClass="button" Text="Seleziona" title="Seleziona il profilo da visualizzare"></asp:Button>
					</fieldset>
					<uc1:FiltriDocumenti id="filtriDocumenti" runat="server"></uc1:FiltriDocumenti>
					<uc1:FiltriDocumentiAvanzati id="filtriDocumentiAvanzati" runat="server"></uc1:FiltriDocumentiAvanzati>
					<p class="centerButtons">
						<asp:Button id="btnSearch" CssClass="button" Runat="server" Text="Avvia Ricerca"></asp:Button>
						<asp:Button id="btnClearFilters" CssClass="button" Runat="server" Text="Pulisci modulo"></asp:Button>
						<asp:Button id="btnAdvancedFilters" CssClass="button" Runat="server"></asp:Button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
