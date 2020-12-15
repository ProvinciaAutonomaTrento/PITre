<%@ Register TagPrefix="uc1" TagName="DettTrasmEffettuate" Src="../Trasmissioni/DettTrasmEffettuate.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TrasmissioniEffettuate.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Fascicoli.TrasmissioniEffettuate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc2" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<p class="message" id="pnlMessage" runat="server">
</p>
<wcag:AccessibleDataGrid id="grdTrasmissioniEffettuate" runat="server" UseAccessibleHeader="True" AutoGenerateColumns="False">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
		<asp:BoundColumn DataField="DataInvio" HeaderText="Data invio">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Utente" HeaderText="Utente">
			<HeaderStyle Width="40%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo">
			<HeaderStyle Width="40%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Visualizza">
			<ItemTemplate>
				<asp:Button id="btnShowDetails" runat="server" Text="Dettagli" CommandName="SHOW_DETAILS" CssClass="gridButton"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<uc2:ListPagingNavigationControls id="pagingTrasmEffettuate" runat="server"></uc2:ListPagingNavigationControls>
<br />
<uc1:DettTrasmEffettuate id="dettTrasmEffettuate" runat="server"></uc1:DettTrasmEffettuate>
