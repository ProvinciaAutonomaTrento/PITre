<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DettagliGenerali.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Documenti.DettagliGenerali" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<fieldset class="baseData">
	<legend>Dettagli di base</legend>
	<div class="left">
		<p id="containerNumProt" runat="server">
			<label id="lblNumProtocollo" runat="server">Numero protocollo</label>
			<asp:TextBox ID="txtNumProtocollo" CssClass="textField" Runat="server" width="55px" ></asp:TextBox>
		</p>
		<p id="containerRegistro" runat="server">
			<label id="lblRegistro" runat="server">Registro</label>
			<asp:TextBox id="txtRegistro" Runat="server" CssClass="textField"></asp:TextBox>
			<asp:DropDownList id="cboRegistri" Runat="server"></asp:DropDownList>
		</p>
		<p id="containerStatoRegistro" runat="server">
			<label id="lblStatoRegistro" runat="server">Stato registro</label> 
			<!-- le classi di css da usare sono regStatusOpen, regStatusYellow e regStatusClosed -->
			<asp:Label id="txtStatoRegistro" runat="server" CssClass="regStatusOpen"></asp:Label>
		</p>
		<p id="containerSegnatura" runat="server">
			<label id="lblSegnatura" runat="server">Segnatura</label>
			<asp:TextBox ID="txtSegnatura" CssClass="textField" Runat="server" size="30"></asp:TextBox>
		</p>
		<p id="containerIdDocumentoBase" runat="server">
			<label id="lblIdDocumentoBase" runat="server">Id documento</label>
			<asp:TextBox id="txtIdDocumentoBase" CssClass="textField" Runat="server" width="80px"></asp:TextBox>
		</p>
		<p id="containerDataCreazioneBase" runat="server">
			<label id="lblDtaCreazioneBase" runat="server">Data Creazione</label>
			<asp:TextBox id="txtDtaCreazioneBase" CssClass="textField" Runat="server" size="20"></asp:TextBox>
		</p>
	</div>
	<div class="right">
		<p id="containerDataProtocollo" runat="server">
			<label id="lblDtaProto" runat="server">Data Protocollo</label>
			<asp:TextBox ID="txtDtaProto" Runat="server" size="20" CssClass="textField"></asp:TextBox>
		</p>
		<p ID="containerOggetto" runat="server">
			<label id="lblOggetto" runat="server">Oggetto</label>
			<asp:TextBox ID="txtOggetto" TextMode="MultiLine" Rows="3" Columns="20" Runat="server"></asp:TextBox>
		</p>
	</div>
</fieldset>
<fieldset class="extraData">
	<legend>Altri dettagli</legend>
	<p id="containerTipologiaDocumento" runat="server">
		<label id="lblFiltroTipoDoc" runat="server">Tipologia</label>
		<asp:DropDownList id="cboTipologiaDocumento" Runat="server"></asp:DropDownList>
		<asp:TextBox id="txtTipologiaDocumento" CssClass="textField" Runat="server"></asp:TextBox>
		<asp:Button id="btnVisibilita" cssclass="showVisibility" runat="server" alt="Apri visibilità documento"
			Tooltip="Apri visibilità documento"></asp:Button>
	</p>
	<p id="containerIdDocumentoExtra" runat="server">
		<label id="lblIdDocumentoExtra" runat="server">Id documento</label>
		<asp:TextBox id="txtIdDocumentoExtra" Runat="server" CssClass="textField" size="7"></asp:TextBox>
	</p>
	<p id="containerDataCreazioneExtra" runat="server">
		<label id="lblDtaCreazioneExtra" runat="server">Data Creazione</label>
		<asp:TextBox ID="txtDtaCreazioneExtra" Runat="server" CssClass="textField" size="20"></asp:TextBox>
	</p>
</fieldset>
<p class="clear"><!-- --></p>
