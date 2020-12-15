<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TrasmissioniRicevute.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Fascicoli.TrasmissioniRicevute" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="DettTrasmRicevute" Src="../Trasmissioni/DettTrasmRicevute.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<p class="message" id="pnlMessage" runat="server">
</p>
<wcag:AccessibleDataGrid id="grdTrasmissioniRicevute" runat="server" UseAccessibleHeader="True" AutoGenerateColumns="False">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
		<asp:BoundColumn DataField="DataInvio" HeaderText="Data invio">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Mittente" HeaderText="Mittente">
			<HeaderStyle Width="30%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo">
			<HeaderStyle Width="30%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Ragione" HeaderText="Ragione">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="DataScadenza" HeaderText="Data scadenza">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Visualizza">
			<HeaderStyle Width="10%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnShowDetails" runat="server" CssClass="gridButton" CommandName="SHOW_DETAILS"
					Text="Dettagli"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<br>
<uc1:DettTrasmRicevute id="dettTrasmRicevute" runat="server"></uc1:DettTrasmRicevute>
<br />
<uc2:ListPagingNavigationControls id="pagingTrasmRicevute" runat="server"></uc2:ListPagingNavigationControls>