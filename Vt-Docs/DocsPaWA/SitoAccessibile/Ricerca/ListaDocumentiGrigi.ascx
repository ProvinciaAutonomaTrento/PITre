<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ListaDocumentiGrigi.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Ricerca.ListaDocumentiGrigi" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<p class="message" id="pnlMessage" runat="server">
</p>
<wcag:AccessibleDataGrid id="grdDocumentiGrigi" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="True">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="DocNumber"></asp:BoundColumn>
		<asp:BoundColumn DataField="Documento" HeaderText="Numero/Data Documento">
			<HeaderStyle Width="30%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Oggetto" HeaderText="Oggetto">
			<HeaderStyle Width="60%"></HeaderStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Azione">
			<HeaderStyle Width="10%"></HeaderStyle>
			<ItemTemplate>
				<asp:Button id="btnShowDocument" runat="server" Text="Apri" CssClass="gridButton" CommandName="SHOW_DOCUMENT"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</wcag:AccessibleDataGrid>
<uc1:ListPagingNavigationControls id="listPagingNavigation" runat="server"></uc1:ListPagingNavigationControls>
