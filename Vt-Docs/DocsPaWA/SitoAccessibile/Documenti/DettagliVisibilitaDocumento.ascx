<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DettagliVisibilitaDocumento.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Documenti.DettagliVisibilitaDocumento" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<wcag:AccessibleDataGrid id="grdVisibilita" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="True"
	Width="100%" Caption="Elenco dei ruoli e degli utenti cui il documento è visibile" summary="Elenco visibilità documento">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="CodiceRubrica"></asp:BoundColumn>
		<asp:BoundColumn DataField="RuoloUtente" HeaderText="Ruolo/Utente">
			<HeaderStyle Width="60%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Diritto" HeaderText="Diritto">
			<HeaderStyle Width="20%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Tipo" HeaderText="Tipo">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Azioni">
			<HeaderStyle Width="10%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnShowDetails" runat="server" cssclass="gridButton" Text=" Vedi utenti" CommandName="SHOW_DETAILS"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<p id="pnlListUsers" class="usersInRole" runat="server">
	<asp:Label id="lblUsersPrefix" runat="server" visible="false">Utenti del ruolo selezionato: </asp:Label>
	<asp:Label id="lblUsers" runat="server" visible="false"></asp:Label>
</p>
