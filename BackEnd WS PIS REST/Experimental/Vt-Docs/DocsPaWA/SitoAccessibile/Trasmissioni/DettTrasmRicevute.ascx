<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DettTrasmRicevute.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Trasmissioni.DettTrasmRicevute" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<fieldset id="dettTrasmRicevuta" class="trasmissioni">
	<legend>Dettagli trasmissione ricevuta</legend>
	<div id="detailsContainer" class="fullContainer" runat="server"></div>
</fieldset>
<div id="pnlAccettazioneRifiuto" runat="server">
	<label for="txtNoteAccRif">Note aggiuntive</label>
	<asp:textbox id="txtNoteAccRif" CssClass="textField" runat="server" Width="30%"></asp:textbox>
	<asp:button id="btnAccetta" CssClass="button" runat="server" Text="Accetta"></asp:button>
	<asp:button id="btnRifiuta" CssClass="button" runat="server" Text="Rifiuta"></asp:button>
</div>
