<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="DettagliCorrispondente.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Rubrica.DettagliCorrispondente" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Dettagli corrispondente</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
			<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
				<LINK media="screen" href="../Css/docsMenu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="datiCorrispondente" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<A href="#content">Vai al contenuto</A>
				</div>
				<div id="headerLogin">
					<uc1:usercontext id="UserContext" runat="server"></uc1:usercontext>
				</div>
				<div id="contentLogin">
					<fieldset class="corrDetails">
						<legend>Dettagli del corrispondente </legend>
						<p class="labelFieldPair">
							<label for="txtUser">Denominazione</label>
							<asp:textbox id="txtUser" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtCodiceRubrica">Codice rubrica</label>
							<asp:textbox id="txtCodiceRubrica" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtIndirizzo">Indirizzo</label>
							<asp:textbox id="txtIndirizzo" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtCap">Codice postale</label>
							<asp:textbox id="txtCap" runat="server" size="10" CssClass="textField short"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtCitta">Citt&agrave;�</label>
							<asp:textbox id="txtCitta" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtProvincia">Provincia (sigla)</label>
							<asp:textbox id="txtProvincia" runat="server" size="5" CssClass="textField short"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtNazione">Nazione</label>
							<asp:textbox id="txtNazione" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtTelprinc">Telefono principale</label>
							<asp:textbox id="txtTelprinc" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtTelsecond">Telefono secondario</label>
							<asp:textbox id="txtTelsecond" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtFax">FAX</label>
							<asp:textbox id="txtFax" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtFiscale">Codice fiscale o partita IVA</label>
							<asp:textbox id="txtFiscale" runat="server" size="50" CssClass="textField"></asp:textbox>
						</p>
						<p class="labelFieldPair">
							<label for="txtEmail">Email</label>
							<asp:textbox id="txtEmail" runat="server" size="50" CssClass="textField"></asp:textbox></p>
						<p class="labelFieldPair">
							<label for="txtCodaoo">Codice <acronym title="Area Organizzativa Omogenea">AOO</acronym> </label>
							<asp:textbox id="txtCodaoo" runat="server" size="50" CssClass="textField"></asp:textbox></p>
						<p class="labelFieldPair">
							<label for="txtCodadmin">Codice amministrazione</label>
							<asp:textbox id="txtCodadmin" runat="server" size="50" CssClass="textField"></asp:textbox></p>
						<p class="labelFieldPair">
							<label for="txtNote">Note</label>
							<asp:textbox id="txtNote" runat="server" CssClass="textField" size="50"></asp:textbox>
						</p>
					</fieldset>
					<br>
					<div id="pnlRicevutaRitorno" runat="server">
						<fieldset class="corrDetails">
							<legend>Ricevuta di ritorno 
							</legend>
							<p class="labelFieldPair">
								<label for="txtRicRitDestinatario">Destinatario</label>
								<asp:textbox id="txtRicRitDestinatario" runat="server" size="50" CssClass="textField"></asp:textbox></p>
							<p class="labelFieldPair">
								<label for="txtRicRitCodiceAmm">Cod. amm.</label>
								<asp:textbox id="txtRicRitCodiceAmm" runat="server" size="50" CssClass="textField"></asp:textbox></p>
							<p class="labelFieldPair">
								<label for="txtRicRitCodiceAOO">Cod. AOO</label>
								<asp:textbox id="txtRicRitCodiceAOO" runat="server" size="50" CssClass="textField"></asp:textbox></p>
							<p class="labelFieldPair">
								<label for="txtRicRitDataProtocollo">Data protocollo</label>
								<asp:textbox id="txtRicRitDataProtocollo" runat="server" size="50" CssClass="textField"></asp:textbox></p>
							<p class="labelFieldPair">
								<label for="txtRicRitNumProtocollo">Num. protocollo</label>
								<asp:textbox id="txtRicRitNumProtocollo" runat="server" size="50" CssClass="textField"></asp:textbox></p>
							<p class="labelFieldPair">
								<label for="txtRicRitDataSpedizione">Data spedizione</label>
								<asp:textbox id="txtRicRitDataSpedizione" runat="server" size="50" CssClass="textField"></asp:textbox></p>
						</fieldset>
					</div>
					<p class="centerButtons">
						<asp:button id="btnBack" runat="server" CssClass="button" Text="Torna indietro"></asp:button>
						<asp:button id="btnOccasionale" CssClass="button" runat="server" Text="Occasionale" visible="false"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
