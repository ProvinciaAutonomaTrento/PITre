<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NotificaErrori.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.WebControls.ValidazioneErrori" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div class=notifyErrors id=errorsContainer runat="server">
	<h2><label id="lblHeader" runat="server"></label></h2>
	<ul>
		<li>Il campo Codice fascicolo non può essere vuoto.</li>
		<li>Il campo Anno non può essere vuoto.</li>
	</ul>
	<p>Per favore <a href="#firstID">correggi i seguenti errori (id del primo campo da correggere)</a> ed invia nuovamente il modulo
	</p>
</div>
