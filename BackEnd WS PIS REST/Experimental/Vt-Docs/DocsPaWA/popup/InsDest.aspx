<%@ Page language="c#" Codebehind="InsDest.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.InsDest" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function CheckCodici(f)
			{
				if (f.txt_codRub.value != "")
				{			
					var strCR = f.txt_codRub.value;						
					if (strCR.indexOf(" ") > -1)
					{
						alert("Attenzione! eliminare gli spazi vuoti dal CODICE RUBRICA");
						f.txt_codRub.focus();
						return false;
					}
				}
				if (f.txt_codCorr.value != "")
				{
					var strCC = f.txt_codCorr.value;						
					if (strCC.indexOf(" ") > -1)
					{
						alert("Attenzione! eliminare gli spazi vuoti dal CODICE CORRISPONDENTE");
						f.txt_codCorr.focus();
						return false;
					}
				}
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="InsDest" onsubmit="return CheckCodici(this);" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Inserimento Corrispondenti" />
			<TABLE class="info" id="tbl_dettagliCorrispondenti" height="250" width="414" align="center"
				border="0">
				<TR>
					<td class="item_editbox" colSpan="2">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Inserimento Corrispondenti</asp:label></P>
					</td>
				</TR>
				<TR>
					<TD style="WIDTH: 178px; HEIGHT: 38px" vAlign="top" align="right"><asp:label id="lbl_parent" runat="server" Width="178px" CssClass="titolo_rosso"></asp:label></TD>
					<td style="WIDTH: 351px; HEIGHT: 38px" vAlign="top" align="left"><asp:label id="lbl_parent_desc" runat="server" CssClass="titolo_rosso"></asp:label></td>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 18px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						Tipo Corrispondente
					</TD>
					<TD style="WIDTH: 351px; HEIGHT: 18px" align="center"><asp:dropdownlist id="ddl_tipoCorr" runat="server" Width="205px" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						Codice rubrica</TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_codRub" runat="server" Width="205" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						Codice corrispondente</TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_codCorr" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						Codice AMM</TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_codAmm" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						Codice AOO</TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_codAOO" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_canpref" runat="server">Canale preferenziale</asp:label></TD>
					<TD style="WIDTH: 351px" align="center"><asp:dropdownlist id="dd_canpref" runat="server" Width="205px" CssClass="testo_grigio"></asp:dropdownlist></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_email" runat="server">Email</asp:label></TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_email" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_desc_nome" runat="server">Descrizione *</asp:label></TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_descr" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_cognome" runat="server" Visible="False">Cognome  *</asp:label>&nbsp;</TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_cognome" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="50"
							Visible="False"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 17px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_indirizzo" runat="server">Indirizzo</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 17px" align="center"><asp:textbox id="txt_indirizzo" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_cap" runat="server">Cap</asp:label></TD>
					<TD style="WIDTH: 351px" align="center"><asp:textbox id="txt_cap" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="5"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 20px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_citta" runat="server">Citta'</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 20px" align="center"><asp:textbox id="txt_citta" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="64"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_provincia" runat="server">Provincia</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_provincia" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="2"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_local" runat="server">Località</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_local" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="2"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 22px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_nazione" runat="server">Nazione</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 22px" align="center"><asp:textbox id="txt_nazione" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="32"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_telefono1" runat="server">Telefono princ.</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_telefono1" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_telefono2" runat="server">Telefono sec.</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_telefono2" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_fax" runat="server">Fax</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_fax" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_codfisc" runat="server">Codice fiscale/Partita iva</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_codfisc" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" style="WIDTH: 178px; HEIGHT: 16px"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_note" runat="server">Note</asp:label></TD>
					<TD style="WIDTH: 351px; HEIGHT: 16px" align="center"><asp:textbox id="txt_note" runat="server" Width="205px" CssClass="testo_grigio" MaxLength="250"></asp:textbox></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 566px" align="center" colSpan="2" height="10"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 566px" align="center" colSpan="2" height="30"><asp:button id="btn_Insert" runat="server" CssClass="pulsante" Text="Inserisci"></asp:button>&nbsp;&nbsp;&nbsp;<asp:button id="btn_chiudi" runat="server" CssClass="pulsante" Text="Chiudi"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
