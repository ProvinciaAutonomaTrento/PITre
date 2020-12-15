<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TrasmissioniRicevute.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Ricerca.TrasmissioniRicevute" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="DettTrasmRicevute" Src="../Trasmissioni/DettTrasmRicevute.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<p class="message" id="pnlMessage" runat="server"></p>
<wcag:accessibledatagrid id="grdTrasmissioniRicevute" runat="server" summary="Elenco delle trasmissioni ricevute"
	AutoGenerateColumns="False" UseAccessibleHeader="True">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="IDProfile"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="DocNumber"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="IDFascicolo"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="IDRegistro"></asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Data invio / Ragione">
			<HeaderStyle Width="15%"></HeaderStyle>
			<ItemTemplate>
				<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataInvio") %>'>
				</asp:Label>
				<asp:label runat="server" Text="<br />-------<br />"></asp:label>
				<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ragione") %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="Mittente" HeaderText="Mittente Trasmissione (Ruolo)">
			<HeaderStyle Width="20%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Oggetto / Mittente">
			<HeaderStyle Width="25%"></HeaderStyle>
			<ItemTemplate>
				<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Oggetto") %>' ID="Label1">
				</asp:Label>
				<asp:label runat="server" Text="<br />-------<br />" ID="Label2"></asp:label>
				<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MittenteDocumento") %>' ID="Label3">
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="Documento" HeaderText="Documento">
			<HeaderStyle Width="15%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Fascicolo" HeaderText="Fascicolo">
			<HeaderStyle Width="15%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="DataScadenza" HeaderText="Scadenza">
			<HeaderStyle Width="5%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Visualizza">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnDetails" runat="server" Text="Dettaglio trasmissione" CommandName="SHOW_DETAILS"
					cssclass="gridButton"></asp:Button>
				<BR>
				<BR>
				<asp:Button id="btnShow" runat="server" CommandName="SHOW" CssClass="gridButton"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</wcag:accessibledatagrid>
<uc1:listpagingnavigationcontrols id="ListPagingNavigation" runat="server"></uc1:listpagingnavigationcontrols>
<br>
<uc1:detttrasmricevute id="DettTrasmRicevute" runat="server" Visible="False"></uc1:detttrasmricevute>
