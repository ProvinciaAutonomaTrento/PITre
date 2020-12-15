<%@ Page language="c#" Codebehind="StampaTrasmissioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaTrasm.StampaTrasmissioni" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Stampa Trasmissioni" />
			<P>
				<asp:Label id="lbStampa" runat="server" ForeColor="DarkSlateBlue" Font-Bold="True" Font-Italic="True"
					Font-Size="Larger"></asp:Label></P>
			<P>
				<asp:Label id="lbRuoloUtente" runat="server" Font-Size="Larger" Font-Italic="True" Font-Bold="True"
					ForeColor="DarkSlateBlue"></asp:Label></P>
			<P>
				<asp:Label id="lbTipoOggetto" runat="server" Font-Size="Larger" Font-Italic="True" Font-Bold="True"
					ForeColor="DarkSlateBlue"></asp:Label></P>
		</form>
		<asp:DataGrid id="grigliaEffettuate" runat="server" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
			BackColor="White" CellPadding="3" GridLines="Vertical" AutoGenerateColumns="False" EnableViewState="False">
			<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
			<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
			<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
			<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
			<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
			<Columns>
				<asp:BoundColumn DataField="Data" HeaderText="Data invio" FooterText="Data invio"></asp:BoundColumn>
				<asp:BoundColumn DataField="Utente" HeaderText="Mittente" FooterText="Mittente"></asp:BoundColumn>
				<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo" FooterText="Ruolo"></asp:BoundColumn>
				<asp:BoundColumn Visible="False" DataField="Chiave" HeaderText="Chiave" FooterText="Chiave"></asp:BoundColumn>
				<asp:BoundColumn DataField="InfoOggTrasm" HeaderText="Descrizione" FooterText="Descrizione"></asp:BoundColumn>
				<asp:BoundColumn DataField="segnData" HeaderText="Segnatura" FooterText="Segnatura"></asp:BoundColumn>
			</Columns>
			<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
		</asp:DataGrid>
		<asp:DataGrid id="DataGrid1" runat="server"></asp:DataGrid>
		<asp:DataGrid id="grigliaRicevute" runat="server" AutoGenerateColumns="False" GridLines="Vertical"
			CellPadding="3" BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#999999"
			EnableViewState="False" Visible="False">
			<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
			<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
			<AlternatingItemStyle BackColor="#DCDCDC"></AlternatingItemStyle>
			<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
			<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
			<Columns>
				<asp:BoundColumn DataField="Data" HeaderText="Data" FooterText="Data"></asp:BoundColumn>
				<asp:BoundColumn DataField="Utente" HeaderText="Utente" FooterText="Utente"></asp:BoundColumn>
				<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo" FooterText="Ruolo"></asp:BoundColumn>
				<asp:BoundColumn DataField="Ragione" HeaderText="Ragione" FooterText="Ragione"></asp:BoundColumn>
				<asp:BoundColumn DataField="DataScad" HeaderText="Data scadenza" FooterText="Data scadenza"></asp:BoundColumn>
				<asp:BoundColumn DataField="Chiave" HeaderText="Chiave" FooterText="Chiave"></asp:BoundColumn>
				<asp:BoundColumn DataField="InfoOggTrasm" HeaderText="Informazioni" FooterText="Informazioni"></asp:BoundColumn>
				<asp:BoundColumn DataField="segnData" HeaderText="Data segnatura" FooterText="Data segnatura"></asp:BoundColumn>
			</Columns>
			<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
		</asp:DataGrid>
	</body>
</HTML>
