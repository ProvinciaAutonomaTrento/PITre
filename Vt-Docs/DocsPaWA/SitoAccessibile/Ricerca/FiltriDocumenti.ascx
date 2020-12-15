<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FiltriDocumenti.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Ricerca.FiltriDocumenti" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="DateMask" Src="../WebControls/DateMask.ascx" %>
<fieldset>
	<legend>Opzioni documento</legend>
	<div>
		<fieldset class="small" id="frameDataProtocollo" runat="server">
			<legend>Data protocollo</legend>
			<p class="labelFieldPair">
				<label id="lblDataProtocolloFrom" runat="server">Iniziale</label>
				<uc1:DateMask id="txtDataProtocolloFrom" Runat="server" CssClass="textField"></uc1:DateMask>&nbsp;				
				<label id="lblDataProtocolloTo" runat="server">finale</label>
				<uc1:DateMask id="txtDataProtocolloTo" Runat="server" CssClass="textField"></uc1:DateMask>&nbsp;
				<label id="lblAnnoProtocollo" runat="server">Anno</label>
				<asp:textbox id="txtAnnoProtocollo" Runat="server" cssclass="textField" width="35px" MaxLength="4"></asp:textbox>
			</p>
		</fieldset>
		<fieldset class="small" id="frameNumeroProtocollo" runat="server">
			<legend>Numero protocollo</legend>
			<p class="labelFieldPair">
				<label id="lblNumProtocolloFrom" runat="server">Iniziale</label>
				<asp:textbox id="txtNumProtocolloFrom" Runat="server" cssclass="textField"></asp:textbox>&nbsp;
				<label id="lblNumProtocolloTo" runat="server">finale</label>
				<asp:textbox id="txtNumProtocolloTo" Runat="server" cssclass="textField"></asp:textbox></p>
		</fieldset>
		<p class="labelFieldPair">
			<label id="lblOggetto" runat="server">Oggetto</label>
			<br />
			<asp:textbox id="txtOggetto" Runat="server" Columns="40" Rows="3" TextMode="MultiLine"></asp:textbox>
		</p>
		<p class="labelFieldPair" id="pnlMittenteDestinatario" runat="server">
			<label id="lblMittenteDestinatario" runat="server">Mittente/Destinatario</label>
			<br />
			<asp:textbox id="txtMittenteDestinatario" Runat="server" cssclass="textField buttoned" MaxLength="50"
				size="50"></asp:textbox>
			<asp:Button ID="btnShowRubricaMittDest" Runat="server" Text="Rubrica" CssClass="button"></asp:Button>
		</p>
	</div>
	<div>
		<fieldset class="small" id="frameDataCreazione" runat="server">
			<legend>Data creazione</legend>
			<p class="labelFieldPair">
				<label id="lblDataCreazioneFrom" runat="server">Iniziale</label>
				<uc1:DateMask id="txtDataCreazioneFrom" Runat="server" CssClass="textField"></uc1:DateMask>&nbsp;
				<label id="lblDataCreazioneTo" runat="server">finale</label>
				<uc1:DateMask id="txtDataCreazioneTo" Runat="server" CssClass="textField"></uc1:DateMask>
			</p>
		</fieldset>
		<fieldset class="small" id="frameIdDocumento" runat="server">
			<legend>Id Documento</legend>
			<p class="labelFieldPair">
				<label id="lblIdDocumentoFrom" runat="server">Iniziale</label>
				<asp:textbox id="txtIdDocumentoFrom" Runat="server" cssclass="textField"></asp:textbox>&nbsp;
				<label id="lblIdDocumentoTo" runat="server">finale</label>
				<asp:textbox id="txtIdDocumentoTo" Runat="server" cssclass="textField"></asp:textbox></p>
		</fieldset>
		<fieldset class="small" id="frameRegistri" runat="server">
			<legend>Registro</legend>
			<p class="labelFieldPair">
				<asp:Button ID="btnSelectAllRegistri" Runat="server" Text="Tutti" CssClass="button"></asp:Button>&nbsp;
				<asp:Button ID="btnUnSelectAllRegistri" Runat="server" Text="Nessuno" CssClass="button"></asp:Button>				
				<br /><br />
				<asp:ListBox id="lstRegistri" runat="server" Rows="3" SelectionMode="Multiple"></asp:ListBox>
			</p>
		</fieldset>
		<br />
		<p class="labelFieldPair">
			<label id="lblTipologiaDocumento" runat="server">Tipologia documento</label>
			<br />
			<asp:dropdownlist id="cboTipologiaDocumento" Runat="server"></asp:dropdownlist>
		</p>
	</div>
</fieldset>