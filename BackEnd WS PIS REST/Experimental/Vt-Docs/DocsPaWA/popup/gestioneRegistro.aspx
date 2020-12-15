<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="gestioneRegistro.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.gestioneregistro" %>
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
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="gestioneregistro" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione registro" />
			<TABLE id="tbl_gestReg" class="info" align="center" border="0" width="360">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:Label id="Label2" runat="server" CssClass="menu_grigio_popup">Gestione registro</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="LabelRegistro" runat="server" CssClass="titolo_scheda" Width="132px">Registro&nbsp;</asp:label>
						<asp:textbox id="txtRegistro" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" ReadOnly="True"></asp:textbox>
					</TD>
				</TR>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="LabelDescrizione" runat="server" CssClass="titolo_scheda" Width="132px">Descrizione&nbsp;</asp:label>
						<asp:textbox id="txtDescrizione" runat="server" CssClass="testo_grigio" Width="250px" Height="22px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="LabelEmail" runat="server" CssClass="titolo_scheda" Width="132px">E-mail&nbsp;</asp:label>
						<asp:textbox id="txtEmail" runat="server" CssClass="testo_grigio" Width="250px" Height="22px"></asp:textbox>
					</TD>
				</TR>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="LabelDataapertura" runat="server" CssClass="titolo_scheda" Width="132px">Data apertura&nbsp;</asp:label>
						<asp:textbox id="txtDataApertura" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" ReadOnly="True"></asp:textbox>
					</TD>
				</TR>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="LabelDataChiusura" runat="server" CssClass="titolo_scheda" Width="132px">Data chiusura&nbsp;</asp:label>
						<asp:textbox id="txtDataChiusura" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" ReadOnly="True"></asp:textbox>
					</TD>
				</TR>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="Label1" runat="server" CssClass="titolo_scheda" Width="132px">Data ultimo protocollo&nbsp;</asp:label>
						<asp:textbox id="txtDataUltimoProt" runat="server" Width="250px" CssClass="testo_grigio" ReadOnly="True" Height="22px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="LabelProssimoProtocollo" runat="server" CssClass="titolo_scheda" Width="132px">Prossimo protocollo&nbsp;</asp:label>
						<asp:textbox id="txtProssimoProtocollo" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" MaxLength="7" ReadOnly="True"></asp:textbox>
					</TD>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="middle">
						<asp:label id="LabelNota" runat="server" CssClass="testo_grigio" Width="400px">&nbsp;(Numero di protocollo che verrà assegnato al momento della protocollazione di un nuovo documento relativo a questo registro)&nbsp;</asp:label>
					</TD>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="middle" height="30">
						<asp:Button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:Button>&nbsp;
						<asp:Button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:Button>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
