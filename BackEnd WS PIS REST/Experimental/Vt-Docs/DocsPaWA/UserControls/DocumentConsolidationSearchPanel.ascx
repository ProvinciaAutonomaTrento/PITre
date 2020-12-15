<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentConsolidationSearchPanel.ascx.cs" Inherits="DocsPAWA.UserControls.DocumentConsolidationSearchPanel" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Corrispondente.ascx" TagName="Corrispondente" TagPrefix="uc2" %>
<table class="info_grigio" cellspacing="0" cellpadding="0" border="0" width="95%" align="center">
    <tr>
        <td>
            <IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
            <asp:Label ID="lblStatoConsolidamento" runat="server" Text="Stato consolidamento:" CssClass="testo_grigio_scuro"></asp:Label> 
            <asp:CheckBoxList ID="lstFiltriConsolidamento" runat="server" 
                CssClass="testo_grigio" RepeatDirection="Vertical"
                onselectedindexchanged="lstFiltriConsolidamento_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="0">Non consolidati</asp:ListItem>
                <asp:ListItem Value="1">Consolidato contenuto</asp:ListItem>
                <asp:ListItem Value="2">Consolidato contenuto e metadati</asp:ListItem>
           </asp:CheckBoxList>
        </td>
    </tr>
    <asp:Panel ID="pnl_data_cons" runat="server" Visible="false">
	<tr>
    	<td>
            <IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">            
            <asp:Label ID="lblDataConsolidamento" runat="server" Text="Data consolidamento:" CssClass="testo_grigio_scuro"></asp:Label> 
            <br />
            <IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">            
            <asp:dropdownlist id="cboDataConsolidamento" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="true" OnSelectedIndexChanged="cboDataConsolidamento_OnSelectedIndexChanged">
				<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
				<asp:ListItem Value="1">Intervallo</asp:ListItem>
				<asp:ListItem Value="2">Oggi</asp:ListItem>
				<asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
				<asp:ListItem Value="4">Mese Corrente</asp:ListItem>
			</asp:dropdownlist>
            <uc1:Calendario id="txtDataConsolidamento" runat="server" Visible="true" />
            <uc1:Calendario id="txtDataConsolidamentoFinale" runat="server" Visible="true" />
         </td>
	</tr>
    <tr>
        <td style="padding-left:10px;">            
            <asp:Label ID="lblUtenteRuoloConsolidante" runat="server" Text="Utente / Ruolo consolidante:" CssClass="testo_grigio_scuro"></asp:Label> 
            <IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">            
            <uc2:Corrispondente ID="ctlUtenteRuoloConsolidante" runat="server" DESCRIZIONE_READ_ONLY="true" CSS_CODICE="testo_grigio" CSS_DESCRIZIONE="testo_grigio" WIDTH_CODICE="90" WIDTH_DESCRIZIONE="200" />
        </td>
    </tr>
    </asp:Panel>
</table>