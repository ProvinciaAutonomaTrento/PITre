<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Creatore.ascx.cs" Inherits="DocsPAWA.UserControls.Creatore" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

<script language="javascript" type="text/javascript">

	// Gestione visualizzazione maschera rubrica
	function ShowDialogRubrica(txtTipoCorrispondente)
	{
		var w_width = screen.availWidth - 40;
		var w_height = screen.availHeight - 35;
		
		var navapp = navigator.appVersion.toUpperCase();
		if ((navapp .indexOf("WIN") != -1) && (navapp .indexOf("NT 5.1") != -1))
			w_height = w_height + 20;
		
		var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
		
		var params="calltype=" +  Rubrica.prototype.CALLTYPE_RICERCA_CREATOR + "&tipo_corr=" + document.getElementById(txtTipoCorrispondente).value;
		
		var urlRubrica="../popup/rubrica/Rubrica.aspx";
		var res=window.showModalDialog (urlRubrica + "?" + params,window,opts);				
	}	
		
</script>

<input id="txtTipoCorrispondente" type="hidden" runat="server" />
<table class="info_grigio" id="tblContainer" cellSpacing="0" cellPadding="0"
	 align="center" border="0" runat="server" width="97%">
	<tr>
		<td>
			<div id="pnlContainer" style="OVERFLOW: auto;" runat="server">
				<table class="testo_grigio" id="tblUtenteCreatore" cellSpacing="0" cellPadding="0"
					width="100%" align="center" border="0" runat="server">
					<tr>
					    <TD class="titolo_scheda" width="20%" style="height: 19px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="lblUtenteCreatore" runat="server">Proprietario:</asp:label></TD>
						<TD class="titolo_scheda" width="70%" style="height: 19px">
						    <asp:RadioButtonList ID="optListTipiCreatore" runat="server" CssClass="titolo_scheda" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="optListTipiCreatore_SelectedIndexChanged">
						        <asp:ListItem Value="U">UO</asp:ListItem>
								<asp:ListItem Value="R" Selected="True">Ruolo</asp:ListItem>
								<asp:ListItem Value="P">Persona</asp:ListItem>
						    </asp:RadioButtonList>
						</TD>
						<TD class="titolo_scheda" width="10%" style="height: 19px"><asp:ImageButton id="btnShowRubrica" runat="server" ImageUrl="../images/proto/rubrica.gif" Height="19px" Width="29px" OnClick="btnShowRubrica_Click" /></td>
					</tr>
					<tr>
						<TD class="titolo_scheda" colspan="3" style="height: 15px" valign="top">
						    <IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						    <input id="txtSystemIdUtenteCreatore" type="hidden" runat="server" />
						    <asp:TextBox ID="txtCodiceUtenteCreatore" runat="server" Width="80px" CssClass="testo_grigio" AutoPostBack="True" OnTextChanged="txtCodiceUtenteCreatore_TextChanged"></asp:TextBox>&nbsp;
						    <asp:TextBox ID="txtDescrizioneUtenteCreatore" runat="server" Width="280px" CssClass="testo_grigio"></asp:TextBox>
						</TD>
					</tr>
				</table>
			</div>
		</td>
	</tr>
</table>