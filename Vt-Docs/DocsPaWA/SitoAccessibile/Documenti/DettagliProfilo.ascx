<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DettagliProfilo.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Documenti.DettagliProfilo" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<fieldset class="dettaglioProfilo">
	<legend>
		Dettagli profilo
	</legend>
	<div>
		<div class="noborderBox" id="divPrivato" runat="server">
			<p class="labelFieldPair">
				<label id="lblPrivato" runat="server">Documento privato</label>
				<asp:checkbox id="chkPrivato" runat="server"></asp:checkbox></p>
		</div>
		<div class="noborderBox" id="divParoleChiave" runat="server">
			<p class="labelFieldPair">
				<label id="lblParoleChiave" runat="server">Parole chiave</label>
				<br>
				<asp:listbox id="lstParoleChiave" runat="server" Height="44" Width="300"></asp:listbox></p>
		</div>
	</div>
	<div>
		<div class="noborderBox" id="divNote" runat="server">
			<p class="labelFieldPair">
				<label id="lblNote" runat="server">Note</label>
				<br>
				<asp:textbox id="txtNote" runat="server" Rows="2" Cols="20" TextMode="MultiLine" cssclass="textField"></asp:textbox>
			</p>
		</div>
		<div class="noborderBox">
			<p class="labelFieldPair">
				<label id="lblTipoDoc" runat="server">Tipo di Documento</label>
				<br>
				<asp:dropdownlist id="ddlTipoDoc" runat="server"></asp:dropdownlist>
				<asp:textbox id="txtTipoDoc" Runat="server" CssClass="textField"></asp:textbox>
			</p>
		</div>
	</div>
</fieldset>
