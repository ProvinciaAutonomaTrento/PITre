<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MainMenu.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.MainMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%%>
<div id="navbar">
	<ul id="menu">
		<li id="containerMenuHomepage" runat="server" title="Homepage">
			<input type="submit" id="btnHomepage" runat="server" value="Homepage" NAME="btnHomepage">
		</li>
		<li id="containerMenuDocumenti" runat="server" title="Documenti">
			<input type="submit" id="btnDocumenti" runat="server" value="Documenti" NAME="btnDocumenti">
			<ul id="submenuDocumenti" runat="server" class="submenu docs" title="Elenco voci sottomenu documenti">
				<li id="menuNuovoProtocollo" runat="server">
					<input type="submit" id="btnNuovoProtocollo" runat="server" value="Nuovo protocollo" NAME="btnNuovoProtocollo">
				</li>
				<li id="menuNuovoProfilo" runat="server">
					<input type="submit" id="btnNuovoProfilo" runat="server" value="Nuovo profilo" NAME="btnNuovoProfilo">
				</li>
			</ul>
		</li>
		<li id="containerMenuRicerche" runat="server" title="Ricerche">
			<input type="submit" id="btnRicerche" runat="server" value="Ricerche" NAME="btnRicerche">
			<ul id="submenuRicerche" runat="server" class="submenu search" title="Elenco voci sottomenu ricerca">
				<li id="menuRicercaDocumenti" runat="server">
					<input type="submit" id="btnRicercaDocumenti" runat="server" value="Documenti" NAME="btnRicercaDocumenti">
				</li>
				<li id="menuRicercaFascicoli" runat="server">
					<input type="submit" id="btnRicercaFascicoli" runat="server" value="Fascicoli" NAME="btnRicercaFascicoli">
				</li>
				<li id="menuRicercaTrasmissioni" runat="server">
					<input type="submit" id="btnRicercaTrasmissioni" runat="server" value="Trasmissioni" NAME="btnRicercaTrasmissioni">
				</li>
			</ul>
		</li>
		<li id="containerMenuEsci" runat="server" title="Esci">
			<input type="submit" id="btnEsci" runat="server" value="Esci" NAME="btnEsci">
		</li>
	</ul>
</div>