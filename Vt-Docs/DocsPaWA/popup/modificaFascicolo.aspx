<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Page language="c#" Codebehind="modificaFascicolo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.modificaFascicolo" %>
<%@ Register src="../Note/DettaglioNota.ascx" tagname="DettaglioNota" tagprefix="uc1" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<base target=_self></base>
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<script language="JavaScript" src="../CSS/ETcalendar.js"></script>
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
		<script language="javascript">

		function _ApriRubrica(target)
		{
			var r = new Rubrica();
			
			switch (target) {
				// Gestione fascicoli - Locazione fisica
				case "ef_locfisica":
					r.CallType = r.CALLTYPE_EDITFASC_LOCFISICA;
					
					break;
					
				// Gestione fascicoli - Ufficio referente
				case "ef_uffref":
					r.CallType = r.CALLTYPE_EDITFASC_UFFREF;
					break;	
																
			}
			var res = r.Apri(); 		
		}
		
		function clearSelezioneEsclusiva(id, numeroDiScelte)
		{
			numeroDiScelte--;
			while(numeroDiScelte >= 0)
			{
				var elemento = id+"_"+numeroDiScelte;
				document.getElementById(elemento).checked = false;
				numeroDiScelte--;
			}
		}	
		</script>
			
  </HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="modificaFascicolo" method="post" runat="server">
		<asp:ScriptManager ID="ScriptManager" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Modifica Dati Fascicolo" />
			<table class="info" id="tbl_modificaFascicolo" height="330" width="100%" align="center" border="0">
				<tr>
					<td class="item_editbox">
						<p class="boxform_item"><asp:label id="Label1" runat="server">Modifica dati fascicolo</asp:label></p>
					</td>
				</tr>
				<tr>
					<td>
					    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
					    <asp:label id="lbl_descrizione" runat="server"  CssClass="titolo_scheda">Descrizione *&nbsp;</asp:label>
					</td>
				</tr>
				<tr>
					<td>
					    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
					    <asp:textbox id="txt_descFascicolo" runat="server" Height="60px" CssClass="testo_grigio" Width="95%" TextMode="MultiLine" Rows="2"></asp:textbox>
		            </td>
				</tr>
				<tr>
					<td>
					    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
					    <uc1:DettaglioNota ID="dettaglioNota" runat="server" TipoOggetto="Fascicolo" Rows = "3" width="95%" />
					</td>
				</tr>
	            <tr>
	                <td>
	                    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
	                    <asp:Label ID="lblFascicoloCartaceo" runat="server" Text="Cartaceo" CssClass="titolo_scheda" ></asp:Label>
	                    <asp:CheckBox ID="chkFascicoloCartaceo" runat="server" CssClass="testo_grigio" />&nbsp;&nbsp;
                        <asp:Label ID="lblFascicoloControllato" runat="server" CssClass="testo_grigio">&nbsp;Controllato&nbsp;</asp:Label>
                        <asp:CheckBox ID="chkFascicoloControllato" runat="server" CssClass="testo_grigio" Enabled="False" Checked="false"></asp:CheckBox>&nbsp;&nbsp;
	                </td>	                    
                </tr>
	            
				<asp:panel id="pnl_locFis" Visible="True" Runat="server">
                <tr>
                    <td class="titolo_scheda" colspan="2" height="20">
                    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                    Collocazione Fisica
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                        <asp:textbox id="txt_LFCod" runat="server" CssClass="testo_grigio" Width="70px" AutoPostBack="True"></asp:textbox>&nbsp;
                        <asp:textbox id="txt_LFDesc" runat="server" CssClass="testo_grigio" Width="255px" ReadOnly="True"></asp:textbox>
                        <asp:imagebutton id="btn_rubrica" runat="server" Height="20px" ImageUrl="../images/proto/rubrica.gif"></asp:imagebutton>
                    </td>                    
                </tr>
                <tr>
                    <td colspan="2">
                        <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                        <asp:label id="lbl_LFDTA" runat="server" CssClass="titolo_scheda" Width="75px">Data&nbsp;</asp:label>
                        <uc3:Calendario id="txt_LFDTA" runat="server" Visible="true" />
                        
                    </td>
                </tr>
	            </asp:panel>
				
                <asp:panel id="pnl_uff_ref" Visible="false" Runat="server">				
                <tr>
                    <td class="titolo_scheda" colspan="2" height="20">
                    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                    Ufficio Referente *
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                        <asp:textbox id="txt_cod_uff_ref" runat="server" CssClass="testo_grigio" Width="70px" AutoPostBack="True" ReadOnly="True"></asp:textbox>
                        <asp:textbox id="txt_desc_uff_ref" runat="server" CssClass="testo_grigio" Width="255px" ReadOnly="True"></asp:textbox>
                        <asp:imagebutton id="btn_rubrica_ref" runat="server" Height="20px" ImageUrl="../images/proto/rubrica.gif" Enabled="False"></asp:imagebutton>
                    </td>
                </tr>
				</asp:panel>
			</table>
				
			<asp:Panel id="panel_profDinamica" runat="server" Visible="false" BorderWidth="1" BorderColor="white">
                <table width="99.5%" style="margin-left:0px; border: #800000 1px solid; BACKGROUND-COLOR: #fafafa;">
                    <tr>
                        <td class="titolo_scheda">&nbsp;Tipologia fascicolo :&nbsp;&nbsp;</td>
                        <td style="padding-top:5px"><asp:DropDownList ID="ddl_tipologiaFasc" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="350px"></asp:DropDownList></td>
                    </tr>
                    <asp:panel id="Panel_DiagrammiStato" Runat="server" Visible="false">
		            <tr>
			            <td class="titolo_scheda">&nbsp;Stati :&nbsp;&nbsp;</td>
			            <td><asp:DropDownList id="ddl_statiSuccessivi" runat="server" Width="350px" CssClass="testo_grigio"></asp:DropDownList></td>				
		            </tr>			
		            </asp:panel>
                    <tr>
                        <td colspan="2"><asp:panel id="panel_Contenuto" runat="server" Width="100%" Height="245" ScrollBars="Auto"></asp:panel></td>
                    </tr>
                </table>
            </asp:Panel>
			
			<table width="100%">
				<tr>
					<td align="center">
					    <asp:button id="btn_salva" runat="server" CssClass="pulsante" Text="SALVA"></asp:button>&nbsp;
						<INPUT class="PULSANTE" id="btn_annulla" style="WIDTH: 58px; HEIGHT: 20px" type="button" value="ANNULLA" runat="server">
						<cc2:messagebox id="msg_StatoAutomatico"  Height="0" runat="server" 
                            ongetmessageboxresponse="msg_StatoAutomatico_GetMessageBoxResponse"></cc2:messagebox>
						<cc2:messagebox id="msg_StatoFinale" Height="0" runat="server" 
                            ongetmessageboxresponse="msg_StatoFinale_GetMessageBoxResponse"></cc2:messagebox>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
