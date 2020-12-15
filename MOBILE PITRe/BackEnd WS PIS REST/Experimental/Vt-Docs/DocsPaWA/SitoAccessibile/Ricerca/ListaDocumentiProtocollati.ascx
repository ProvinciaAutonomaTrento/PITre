<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ListaDocumentiProtocollati.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Ricerca.ListaDocumentiProtocollati" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<p class="message" id="pnlMessage" runat="server">
</p>
<wcag:AccessibleDataGrid id="grdDocumentiProtocollati" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="True">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="DocNumber"></asp:BoundColumn>
		<asp:BoundColumn DataField="Protocollo" HeaderText="Numero / Data">
			<HeaderStyle Width="20%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="MittenteDestinatario" HeaderText="Mittente / Destinatario">
			<HeaderStyle Width="20%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Registro" HeaderText="Registro">
			<HeaderStyle Width="10%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Oggetto" HeaderText="Oggetto">
			<HeaderStyle Width="35%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="DirezioneProtocollo" HeaderText="Tipo">
			<HeaderStyle Width="5%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Azione">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnShowDocument" runat="server" CommandName="SHOW_DOCUMENT" CssClass="gridButton"
					Text="Apri"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn Visible="False" DataField="DataAnnullamento"></asp:BoundColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<uc1:ListPagingNavigationControls id="listPagingNavigation" runat="server"></uc1:ListPagingNavigationControls>
