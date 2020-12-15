<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="regDettagli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.gestione.registro.regDettagli" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body leftMargin="1" MS_POSITIONING="GridLayout">
		<form id="regDettagli" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Dettaglio registro selezionato" />
			<asp:panel id="panel_Det" runat="server" Visible="False">
				<TABLE class="info_grigio" id="tblTx" style="BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; BORDER-BOTTOM: 1px solid" cellSpacing="1" cellPadding="1" width="77%" align="center" border="0">
					<TR>
						<TD class="infoDT" id="TD1" align="middle" colSpan="2" height="20" runat="server">
							<asp:Label id="titolo" Runat="server" CssClass="titolo_rosso">
								Dettaglio registro selezionato</asp:Label></TD>
					</TR>
					<TR>
						<TD height="15"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Registro</TD>
						<TD class="testo_grigio_scuro" vAlign="center">
							<asp:Label id="lbl_registro" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<TD height="10"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Descrizione</TD>
						<TD class="testo_grigio_scuro">
							<asp:Label id="lbl_descrizione" runat="server"></asp:Label></TD>
					</TR>
                    <TR>
						<TD height="10"></TD>
					</TR>
                    <tr>
					    <td class="titolo_scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">E-mail</td>
						<td class="testo_grigio_scuro" vAlign="center">
						    <asp:DropDownList ID="ddl_Caselle" runat="server" AutoPostBack="true" DataTextField="Caselle di posta" 
                                    onselectedindexchanged="ddl_Caselle_SelectedIndexChanged"></asp:DropDownList>
                        </td>
					</tr>
					<TR>
						<TD height="10"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data 
							apertura</TD>
						<TD class="testo_grigio_scuro">
							<asp:Label id="lbl_dataApertura" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<TD height="10"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data 
							chiusura</TD>
						<TD class="testo_grigio_scuro">
							<asp:Label id="lbl_dataChiusura" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<TD height="10"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data 
							Ultimo protocollo</TD>
						<TD class="testo_grigio_scuro">
							<asp:Label id="lbl_dataUltProto" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<TD height="10"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Prossimo 
							protocollo</TD>
						<TD class="testo_grigio_scuro">
							<asp:Label id="lbl_prossimoProtocollo" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<TD height="5"></TD>
					</TR>
					<TR>
						<TD class="testo_grigio" align="middle" colSpan="2" height="30">(Numero di 
							protocollo che verrà assegnato al momento della protocollazione di un nuovo 
							documento relativo a questo registro)
						</TD>
					</TR>
					<TR>
						<TD height="15"></TD>
					</TR>
				</TABLE>
			</asp:panel></form>
	</body>
</HTML>
