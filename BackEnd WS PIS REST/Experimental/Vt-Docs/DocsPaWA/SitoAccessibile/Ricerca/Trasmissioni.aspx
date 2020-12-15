<%@ Register TagPrefix="uc1" TagName="DateMask" Src="../WebControls/DateMask.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ValidationContainer" Src="../Validations/ValidationContainer.ascx" %>
<%@ Page language="c#" Codebehind="Trasmissioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Ricerca.Trasmissioni" %>
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
		     <%} %>  > Ricerca trasmissioni</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/docsMenu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="frmRicercaTrasmissioni" method="post" runat="server">
			<div id="container">
				<div class="skipLinks"><A href="#content">Vai al contenuto</A> <A href="#docsMenu">Vai 
						al menu Documento</A> <A href="#navbar">Vai al menu utente</A>
				</div>
				<uc1:usercontext id="UserContext" runat="server"></uc1:usercontext>
				<uc1:mainmenu id="MainMenu" runat="server"></uc1:mainmenu>
				<div id="content">
					<uc1:ValidationContainer id="validationContainer" runat="server"></uc1:ValidationContainer>
					<fieldset>
						<legend>
							Tipo Trasmissione</legend>
						<p class="labelFieldPair"><asp:radiobuttonlist id="listTipiTrasmissioni" RepeatLayout="Flow" TextAlign="Left" RepeatDirection="Horizontal"
								Runat="server">
								<asp:ListItem Value="Ricevute" Selected="True">Ricevute</asp:ListItem>
								<asp:ListItem Value="Effettuate">Effettuate</asp:ListItem>
							</asp:radiobuttonlist><asp:button id="btnSelectTipoTrasmissione" Runat="server" CssClass="button" Text="Seleziona"></asp:button></p>
					</fieldset>
					<fieldset>
						<legend>
							Opzioni Trasmissione</legend>
						<fieldset class="small"><legend>Oggetto trasmesso</legend>
							<p class="labelFieldPair"><asp:radiobuttonlist id="listTipiOggetto" RepeatLayout="Flow" TextAlign="Left" RepeatDirection="Horizontal"
									Runat="server">
									<asp:ListItem Value="Tutti" Selected="True">Tutti</asp:ListItem>
									<asp:ListItem Value="Protocollati">Protocollato</asp:ListItem>
									<asp:ListItem Value="ProtocollatiArrivo">Protocollato in Arrivo</asp:ListItem>
									<asp:ListItem Value="ProtocollatiPartenza">Protocollato in Partenza</asp:ListItem>
									<asp:ListItem Value="NonProtocollati">Non Protocollato</asp:ListItem>
									<asp:ListItem Value="Fascicoli">Fascicolo</asp:ListItem>
								</asp:radiobuttonlist></p>
						</fieldset>
						<br>
						<p class="labelFieldPair" id="frameMittente" runat="server">
							<label id="lblMittente" for="txtMittente" runat="server">Mittente</label>
							<asp:textbox id="txtMittente" runat="server" CssClass="textField" size="50"></asp:textbox>
							<asp:button id="btnSearchMittente" Runat="server" CssClass="button" Text="Rubrica"></asp:button>
						</p>
						<p class="labelFieldPair" id="frameDestinatario" runat="server">
							<label id="lblDestinatario" for="txtDestinatario" runat="server">Destinatario</label>
							<asp:textbox id="txtDestinatario" runat="server" CssClass="textField" size="50"></asp:textbox>
							<asp:button id="btnSearchDestinatario" Runat="server" CssClass="button" Text="Rubrica"></asp:button>
						</p>
						<br>
						<br>
						<p class="labelFieldPair" id="containerRagioneTrasmissione" runat="server">
							<label id="lblRagioniTrasmissione" runat="server" for="cboRagioniTrasmissione">Ragione 
								Trasmissione</label>
							<asp:dropdownlist id="cboRagioniTrasmissione" Runat="server"></asp:dropdownlist>
						</p>
						<br>
						<br>
						<fieldset class="small">
							<legend>
								Data Trasmissione</legend>
							<p class="labelFieldPair">
								<label id="lblDataTrasmissioneFrom" runat="server" for="txtDataTrasmissioneFrom">Iniziale</label>
								<uc1:DateMask id="txtDataTrasmissioneFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
								<label id="lblDataTrasmissioneTo" runat="server" for="txtDataTrasmissioneTo">finale</label>
								<uc1:DateMask id="txtDataTrasmissioneTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
							</p>
						</fieldset>
						<fieldset class="small">
							<legend>
								Data Acc./Rif.</legend>
							<p class="labelFieldPair">
								<label id="lblDataAccRifFrom" runat="server" for="txtDataAccRifFrom">Iniziale</label>
								<uc1:DateMask id="txtDataAccRifFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
								<label id="lblDataAccRifTo" runat="server" for="txtDataTrasmissioneTo">finale</label>
								<uc1:DateMask id="txtDataAccRifTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
							</p>
						</fieldset>
						<fieldset class="small">
							<legend>
								Data risposta</legend>
							<p class="labelFieldPair">
								<label id="lblDataRispostaFrom" runat="server" for="txtDataRispostaFrom">Iniziale</label>
								<uc1:DateMask id="txtDataRispostaFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
								<label id="lblDataRispostaTo" runat="server" for="txtDataTrasmissioneTo">finale</label>
								<uc1:DateMask id="txtDataRispostaTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
							</p>
						</fieldset>
						<fieldset class="small">
							<legend>
								Note</legend>
							<p class="labelFieldPair" id="containerNoteIndividuali" runat="server">
								<label id="lblNoteIndividuali" runat="server" for="txtNoteIndividuali">Individuali</label>
								<asp:textbox id="txtNoteIndividuali" runat="server" CssClass="textField" Width="30%"></asp:textbox>
								<label id="lblNoteGenerali" runat="server" for="txtNoteGenerali">Generali</label>
								<asp:textbox id="txtNoteGenerali" runat="server" CssClass="textField" Width="30%"></asp:textbox>
							</p>
						</fieldset>
						<br>
						<asp:checkbox id="chkVisualizzaTrasmSottoposti" TextAlign="Left" Runat="server" Text="Visualizza trasmissioni sottoposti"></asp:checkbox>
					</fieldset>
					<p class="centerButtons">
						<asp:button id="btnSearch" Runat="server" CssClass="button" Text="Avvia ricerca"></asp:button>
						<asp:button id="btnClearFilters" Runat="server" CssClass="button" Text="Pulisci modulo"></asp:button>
					</p>
				</div>
			</div>
		</form>
	</body>
</HTML>
