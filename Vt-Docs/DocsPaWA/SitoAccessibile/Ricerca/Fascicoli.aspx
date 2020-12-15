<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Page language="c#" Codebehind="Fascicoli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Ricerca.Fascicoli" %>
<%@ Register TagPrefix="uc1" TagName="ValidationContainer" Src="../Validations/ValidationContainer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DateMask" Src="../WebControls/DateMask.ascx" %>
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
		     <%} %>  > Ricerca fascicoli</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
		<LINK media="screen" href="../Css/docsMenu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="frmRicercaFascicoli" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<A href="#content">Vai al contenuto</A> <A href="#docsMenu">Vai al menu documento</A>
					<A href="#navbar">Vai al menu utente</A>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<div id="content">
					<uc1:ValidationContainer id="validationContainer" runat="server"></uc1:ValidationContainer>
					<fieldset>
						<legend>
							Opzioni fascicolo</legend>
						<div>
							<p class="labelFieldPair">
								<label id="lblCodiceTitolario" runat="server" for="txtCodiceTitolario">Codice 
									fascicolo </label>
								<asp:TextBox id="txtCodiceTitolario" CssClass="textField" runat="server" ToolTip="Inserire il codice gerarchico del titolario"
									Width="22%"></asp:TextBox>
							</p>
							<p class="labelFieldPair">
								<label for="cboRegistri">Registro</label>
								<asp:DropDownList id="cboRegistri" Width="25%" Runat="server"></asp:DropDownList>
							</p>
							<fieldset class="small">
								<legend>
									Data chiusura</legend>
								<p class="labelFieldPair">
									<label id="lblDataChiusuraFrom" runat="server" for="txtDataChiusuraFrom">Iniziale</label>
									<uc1:DateMask id="txtDataChiusuraFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
									<label id="lblDataChiusuraTo" runat="server" for="txtDataChiusuraTo">finale</label>
									<uc1:DateMask id="txtDataChiusuraTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
								</p>
							</fieldset>
							<fieldset class="small">
								<legend>
									Data collocazione</legend>
								<p class="labelFieldPair">
									<label id="lblDataCollocazioneFrom" runat="server" for="txtDataCollocazioneFrom">Iniziale</label>
									<uc1:DateMask id="txtDataCollocazioneFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
									<label id="lblDataCollocazioneTo" runat="server" for="txtDataCollocazioneTo">finale</label>
									<uc1:DateMask id="txtDataCollocazioneTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
								</p>
							</fieldset>
							<br>
							<p class="labelFieldPair">
								<label id="lblNumero" runat="server" for="txtNumero">Numero</label>
								<asp:TextBox id="txtNumero" CssClass="textField" runat="server" Width="100px"></asp:TextBox>&nbsp;
							</p>
							<p class="labelFieldPair">
								<label for="cboStato">Stato</label>
								<asp:DropDownList id="cboStato" Width="100px" Runat="server"></asp:DropDownList>
							</p>
							<br>
							<br>
							<p class="labelFieldPair">
								<label for="txtCollocazioneFisica">Collocazione fisica</label>
								<asp:TextBox id="txtCollocazioneFisica" CssClass="textField" runat="server"></asp:TextBox>&nbsp;
								<asp:Button id="btnShowRubrica" Runat="server" CssClass="button" Text="Rubrica"></asp:Button>
							</p>
						</div>
						<div>
							<p class="labelFieldPair">
								<label id="lblAnno" runat="server" for="txtAnno">Anno</label>
								<asp:TextBox id="txtAnno" CssClass="textField" runat="server" Width="100px"></asp:TextBox>&nbsp;
							</p>
							<p class="labelFieldPair">
								<label for="cboTipo">Tipo</label>
								<asp:DropDownList id="cboTipo" Width="100px" Runat="server"></asp:DropDownList>
							</p>
							<fieldset class="small">
								<legend>
									Data apertura</legend>
								<p class="labelFieldPair">
									<label id="lblDataAperturaFrom" runat="server" for="txtDataAperturaFrom">Iniziale</label>
									<uc1:DateMask id="txtDataAperturaFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
									<label id="lblDataAperturaTo" runat="server" for="txtDataAperturaTo">finale</label>
									<uc1:DateMask id="txtDataAperturaTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
								</p>
							</fieldset>
							<fieldset class="small">
								<legend>
									Data creazione</legend>
								<p class="labelFieldPair">
									<label id="lblDataCreazioneFrom" runat="server" for="txtDataCreazioneFrom">Iniziale</label>
									<uc1:DateMask id="txtDataCreazioneFrom" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>&nbsp;
									<label id="lblDataCreazioneTo" runat="server" for="txtDataCreazioneTo">finale</label>
									<uc1:DateMask id="txtDataCreazioneTo" runat="server" CssClass="textField" Width="100px"></uc1:DateMask>
								</p>
							</fieldset>
							<br>
							<p class="labelFieldPair">
								<label for="txtDescrizione">Descrizione</label>
								<asp:TextBox id="txtDescrizione" CssClass="textField" runat="server" Width="200px"></asp:TextBox>&nbsp;
							</p>
						</div>
					</fieldset>
					<br>
					<p class="centerButtons">
						<asp:Button CssClass="button" id="btnSearch" Text="Avvia ricerca" Runat="server" />
						<asp:Button CssClass="button" id="btnClearFilters" Text="Pulisci modulo" Runat="server" />
					</p>
				</div> <!-- End content -->
			</div> <!-- End container -->
		</form>
	</body>
</HTML>
