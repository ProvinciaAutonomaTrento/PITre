<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DettagliProtocollo.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Documenti.DettagliProtocollo" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<fieldset class="dettagliProtocollo">
	<legend>
		Dettagli protocollo</legend>
	<div>
		<div class="noborderBox" id="divMittente" runat="server">
			<p class="labelFieldPair">
				<label id="lblCodMitt" runat="server">Codice e descrizione mittente</label>
				<br>
				<asp:TextBox ID="txtCodMitt" Runat="server" title="Codice del mittente" size="10" CssClass="textField"></asp:TextBox>
				<asp:TextBox id="txtDescMitt" Runat="server" title="Descrizione del mittente" size="37" CssClass="textField"></asp:TextBox>
				<asp:Button id="btnDetailsMittente" Runat="server" Text="Dettagli" CssClass="button"></asp:Button>
			</p>
		</div>
		<div class="noborderBox" id="divMittenteInterm" runat="server" Visible="false">
			<p class="labelFieldPair">
				<label id="lblCodMittInterm" runat="server">Codice e descrizione mittente 
					intermedio</label>
				<br>
				<asp:TextBox ID="txtCodMittInterm" runat="server" title="Codice del mittente intermedio" size="10"
					cssclass="textField"></asp:TextBox>
				<asp:TextBox ID="txtDescMittInterm" runat="server" title="Descrizione del mittente intermedio"
					size="37" cssclass="textField"></asp:TextBox>
			</p>
		</div>
	</div>
	<div>
		<div class="noborderBox" id="divDestinatario" runat="server">
			<p class="labelFieldPair">
				<label id="lblCodDest" runat="server">Destinatario</label>
				<br>
				<asp:TextBox ID="txtDescDest" Runat="server" title="Descrizione del destinatario" size="37" cssclass="textField"></asp:TextBox>
				<asp:listbox id="lstDestinatari" runat="server" Width="300px" Height="44"></asp:listbox>
				<asp:Button id="btnDetailsCorrispondente" Runat="server" Text="Dettagli" CssClass="button"></asp:Button>
			</p>
			<br>
			<p class="labelFieldPair">
				<label id="lblCodDestCC" runat="server">Destinatario per conoscenza</label>
				<br>
				<asp:TextBox ID="txtDescDestCC" Runat="server" title="Descrizione del destinatario" size="37"
					cssclass="textField"></asp:TextBox>
				<asp:listbox id="lstDestinatariCC" runat="server" Width="300" Height="44"></asp:listbox></p>
			<asp:Button id="btnDetailsCorrispondenteCC" Runat="server" Text="Dettagli" CssClass="button"></asp:Button>
		</div>
		<div class="noborderBox" id="divProtoMittente" runat="server">
			<p class="labelFieldPair">
				<label id="lblProtMitt" runat="server">Numero e data Protocollo Mittente</label>
				<br>
				<asp:TextBox ID="txtProtMitt" Runat="server" title="Numero del protocollo" size="30" cssclass="textField"></asp:TextBox>
				<asp:TextBox ID="txtDtaProtMitt" Runat="server" title="Data del protocollo" size="20" cssclass="textField"></asp:TextBox>
			</p>
			<br>
			<p class="labelFieldPair">
				<label id="lblDtaArrivo" runat="server">Data di arrivo</label>
				<br>
				<asp:TextBox ID="txtDtaArrivo" Runat="server" size="20" cssclass="textField"></asp:TextBox>
			</p>
		</div>
		<div class="noborderBox" id="divProtocolloAnnullato" runat="server">
			<p class="labelFieldPair">
				<label id="lblAnnullato" runat="server">Annullato</label>
				<br>
				<asp:TextBox id="txtDataAnnullamento" Runat="server" cssclass="textField" size="10"></asp:TextBox>
				<asp:TextBox id="txtNoteAnnullamento" Runat="server" cssclass="textField" size="40"></asp:TextBox>
			</p>
		</div>
	</div>
</fieldset>
