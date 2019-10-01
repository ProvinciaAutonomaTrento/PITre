<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DocumentiFascicolo.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Fascicoli.DocumentiFascicolo" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<p id="pnlMessage" runat="server" class="message">
</p>
<wcag:AccessibleDataGrid id="grdDocumentiFascicolo" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="True"
	summary="Elenco dei documenti contenuti nel fascicolo">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="IDProfile"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="DocNumber"></asp:BoundColumn>
		<asp:BoundColumn DataField="Documento" HeaderText="Documento">
			<HeaderStyle Width="20%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Registro" HeaderText="Registro">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Oggetto" HeaderText="Oggetto">
			<HeaderStyle Width="40%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="MittenteDestinatari" HeaderText="Mittente / Destinatari">
			<HeaderStyle Width="20%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="TipoDocumento" HeaderText="Tipo">
			<HeaderStyle Width="5px"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Azioni">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnDocumentDetails" runat="server" CommandName="SHOW_DOCUMENT" Text="Apri" CssClass="gridButton"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn Visible="False" DataField="DataAnnullamento"></asp:BoundColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<uc1:ListPagingNavigationControls id="ListPagingNavigationControl" runat="server"></uc1:ListPagingNavigationControls>
