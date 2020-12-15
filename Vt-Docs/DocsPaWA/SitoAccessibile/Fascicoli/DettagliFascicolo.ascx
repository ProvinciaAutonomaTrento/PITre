<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DettagliFascicolo.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Fascicoli.DettagliFascicolo" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="TrasmissioniRicevute" Src="TrasmissioniRicevute.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TrasmissioniEffettuate" Src="TrasmissioniEffettuate.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TreeView" Src="../WebControls/TreeView.ascx" %>
<fieldset class="fascDetails">
	<legend>
		Dati Fascicolo</legend>
	<p>
		<label id="lblClassifica" runat="server">Classifica</label>
		<asp:TextBox id="txtClassifica" Runat="server" cssclass="textField"></asp:TextBox>
	</p>
	<p>
		<label id="lblCodice" runat="server">Codice</label>
		<asp:TextBox id="txtCodice" Runat="server" cssclass="textField"></asp:TextBox>
	</p>
	<p>
		<label id="lblTipo" runat="server">Tipo</label>
		<asp:TextBox id="txtTipo" Runat="server" cssclass="textField"></asp:TextBox>
	</p>
	<p>
		<label id="lblStato" runat="server">Stato</label>
		<asp:TextBox id="txtStato" Runat="server" cssclass="textField"></asp:TextBox>
	</p>
	<p>
		<label id="lblDescrizione" runat="server">Descrizione</label>
		<asp:TextBox id="txtDescrizione" Runat="server" cssclass="textField"></asp:TextBox>
	</p>
	<p>
		<label id="lblNote" runat="server">Note</label>
		<asp:TextBox id="txtNote" Runat="server" cssclass="textField"></asp:TextBox>
	</p>
</fieldset>
<br>
<br>
<fieldset class="filtroTrasmissioni">
	<legend>
		Trasmissioni</legend>
	<asp:radiobuttonlist id="rblSearchType" runat="server" TextAlign="Left" RepeatDirection="Horizontal"
		RepeatLayout="Flow">
		<asp:ListItem Value="Effettuate" Selected="True">Effettuate</asp:ListItem>
		<asp:ListItem Value="Ricevute">Ricevute</asp:ListItem>
	</asp:radiobuttonlist>
	<asp:button id="btnSearch" runat="server" Text="Ricerca" CssClass="button"></asp:button>
</fieldset>
<uc1:TreeView id="trvFolders" runat="server"></uc1:TreeView>
<div class="clear"><!-- --></div>
<br>
<uc1:TrasmissioniEffettuate id="trasmissioniEffettuate" runat="server" Visible="False"></uc1:TrasmissioniEffettuate>
<br>
<uc1:TrasmissioniRicevute id="trasmissioniRicevute" runat="server" Visible="False"></uc1:TrasmissioniRicevute>
