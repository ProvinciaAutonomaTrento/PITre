<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FiltriDocumentiAvanzati.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Ricerca.FiltriDocumentiAvanzati" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="DateMask" Src="../WebControls/DateMask.ascx" %>
<fieldset>
	<legend>Opzioni avanzate</legend>
	<a href="#" id="advancedOpts"></a>
	<div>
		<p class="labelFieldPair">
			<label id="lblParoleChiave" runat="server">Parole chiavi</label>
			<br>
			<asp:ListBox id="listParoleChiavi" Runat="server" Rows="3"></asp:ListBox>
			<asp:Button ID="btnShowParoleChiavi" CssClass="button" Runat="server" Text="Seleziona"></asp:Button>
			<asp:Button ID="btnRemoveParolaChiave" CssClass="button" Runat="server" Text="Rimuovi"></asp:Button>
		</p>
		<br>
		<p class="labelFieldPair">
			<label id="lblNote" runat="server">Note</label>
			<br>
			<asp:TextBox id="txtNote" Runat="server" CssClass="textField" TextMode="MultiLine" Rows="2" Columns="40"></asp:TextBox>
		</p>
		<br />
		<p class="labelFieldPair" id="panelSegnatura" runat="server">
			<label id="lblSegnatura" runat="server">Segnatura</label>
			<br>
			<asp:TextBox id="txtSegnatura" Runat="server" CssClass="textField" size="50" MaxLength="50"></asp:TextBox>
		</p>
		<br />
		<p class="labelFieldPair" id="panelSegnaturaMittente" runat="server">
			<label id="lblSegnaturaMittente" runat="server">Segnatura mittente</label>
			<br>
			<asp:TextBox id="txtSegnaturaMittente" Runat="server" CssClass="textField" size="50" MaxLength="50"></asp:TextBox>
		</p>
		<br />
		<p class="labelFieldPair" id="panelMittenteIntermedio" runat="server">
			<label id="lblMittenteIntermedio" runat="server">Mittente intermedio</label>
			<br>
			<asp:TextBox id="txtMittenteIntermedio" Runat="server" CssClass="textField" size="50" MaxLength="50"></asp:TextBox>
			<asp:Button id="btnShowRubricaMittenteIntermedio" runat="server" CssClass="button" Text="Rubrica"></asp:Button>
		</p>
		<br />
		<p class="labelFieldPair" id="panelDataProtocolloEmergenza" runat="server">
			<label id="lblDataProtocolloEmergenza" runat="server">Data protocollo emergenza</label>
			<br />
			<uc1:DateMask id="txtDataProtocolloEmergenza" Runat="server" CssClass="textField" width="100px"></uc1:DateMask>
		</p>
		<br>
		<p class="labelFieldPair" id="panelProtocolloEmergenza" runat="server">
			<label id="lblProtocolloEmergenza" runat="server" for="txtProtocolloEmergenza">Protocollo emergenza</label>
			<br />
			<asp:TextBox id="txtProtocolloEmergenza" Runat="server" CssClass="textField" size="50" MaxLength="50"></asp:TextBox>
		</p>
	</div>
	<div>
		<fieldset class="small">
			<legend>
				Fascicoli</legend>
			<p class="labelFieldPair">
				<label id="lblCodiceFascicolo" runat="server">Codice</label>
				<asp:TextBox id="txtCodiceFascicolo" Runat="server" CssClass="textField" size="5" MaxLength="20"></asp:TextBox>
				<asp:Button id="btnSearchFascicolo" runat="server" CssClass="innerButton" Text="Cerca"></asp:Button>
			</p>
			<p class="labelFieldPair">
				<label id="lblDescrizioneFascicolo" runat="server">Descrizione</label>
				<asp:TextBox id="txtDescrizioneFascicolo" Runat="server" CssClass="textField" size="18" MaxLength="50"
					ReadOnly="True"></asp:TextBox>
			</p>
		</fieldset>
		<fieldset class="small" id="frameEvidenza" runat="server">
			<legend>
				Evidenza</legend>
			<p class="labelFieldPair">
				<asp:RadioButtonList id="listEvidenza" Runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
					<asp:ListItem Value="yes">Si</asp:ListItem>
					<asp:ListItem Value="no">No</asp:ListItem>
					<asp:ListItem Value="all" Selected="True">Tutti</asp:ListItem>
				</asp:RadioButtonList>
			</p>
		</fieldset>
		<fieldset class="small" id="frameStato" runat="server">
			<legend>
				Stato</legend>
			<p class="labelFieldPair">
				<asp:RadioButtonList id="listStato" Runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
					<asp:ListItem Value="invalid">Annullato</asp:ListItem>
					<asp:ListItem Value="valid">Non annullato</asp:ListItem>
					<asp:ListItem Value="all" Selected="True">Tutti</asp:ListItem>
				</asp:RadioButtonList>
			</p>
		</fieldset>
		<fieldset class="small" id="frameDataProtocolloMittente" runat="server">
			<legend>Data protocollo mittente</legend>
			<p class="labelFieldPair">
				<label id="lblDataProtocolloMittFrom" runat="server">Iniziale</label>
				<uc1:DateMask id="txtDataProtocolloMittFrom" Runat="server" CssClass="textField" width="100px"></uc1:DateMask>&nbsp;
				<label id="lblDataProtocolloMittTo" runat="server">finale</label>
				<uc1:DateMask id="txtDataProtocolloMittTo" Runat="server" CssClass="textField" width="100px"></uc1:DateMask>
			</p>
		</fieldset>
		<fieldset class="small" id="frameDataArrivo" runat="server">
			<legend>Data arrivo</legend>
			<p class="labelFieldPair">
				<label id="lblDataArrivoFrom" runat="server">Iniziale</label>
				<uc1:DateMask id="txtDataArrivoFrom" Runat="server" CssClass="textField" width="100px"></uc1:DateMask>&nbsp;
				<label id="lblDataArrivoTo" runat="server">finale</label>
				<uc1:DateMask id="txtDataArrivoTo" Runat="server" CssClass="textField" width="100px"></uc1:DateMask>
			</p>
		</fieldset>
		<fieldset class="small" id="framePriviDi" runat="server">
			<legend>Privi di</legend>
			<p class="labelFieldPair">
				<asp:CheckBox id="chkPriviAssegnatario" Runat="server" Text="Assegnatario" TextAlign="left"></asp:CheckBox>
				<asp:CheckBox id="chkPriviImmagine" Runat="server" Text="Immagine" TextAlign="left"></asp:CheckBox>
				<asp:CheckBox id="chkPriviFascicolazione" Runat="server" Text="Fascicolazione" TextAlign="left"></asp:CheckBox>
			</p>
		</fieldset>
	</div>
</fieldset>
