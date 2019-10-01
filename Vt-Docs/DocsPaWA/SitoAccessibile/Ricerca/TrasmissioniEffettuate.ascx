<%@ Register TagPrefix="uc1" TagName="DettTrasmEffettuate" Src="../Trasmissioni/DettTrasmEffettuate.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TrasmissioniEffettuate.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Ricerca.TrasmissioniEffettuate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<p class="message" id="pnlMessage" runat="server">
</p>
<wcag:AccessibleDataGrid id="grdTrasmissioniEffettuate" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="True">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="IDProfile"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="DocNumber"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="IDFascicolo"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="IDRegistro"></asp:BoundColumn>
		<asp:BoundColumn DataField="DataInvio" HeaderText="Data invio / Ragione">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Mittente" HeaderText="Mittente">
			<HeaderStyle Width="15%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Oggetto" HeaderText="Oggetto">
			<HeaderStyle Width="25%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo">
			<HeaderStyle Width="15%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Documento" HeaderText="Documento">
			<HeaderStyle Width="15%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Fascicolo" HeaderText="Fascicolo">
			<HeaderStyle Width="15%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Visualizza">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnShowDetails" runat="server" Text="Dettagli" CommandName="SHOW_DETAILS" CssClass="gridButton"></asp:Button>
				<br />
				<br />
				<asp:Button id="btnShow" runat="server" CommandName="SHOW" CssClass="gridButton"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<br>
<uc1:ListPagingNavigationControls id="ListPagingNavigation" runat="server"></uc1:ListPagingNavigationControls>
<br>
<uc1:DettTrasmEffettuate id="DettTrasmEffettuate" runat="server" Visible="False"></uc1:DettTrasmEffettuate>
