<%@ Page language="c#" Codebehind="modificaMittTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.modificaMittTrasm" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="modificaMittTrasm" method="post" runat="server">
    		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Modifica mittente trasmissioni" />
			<TABLE id="tbl_modificaFascicolo" class="info" width="380" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							<asp:Label id="Label1" runat="server">Modifica mittente trasmissioni</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:Label id="lbl_nome" runat="server" CssClass="titolo_scheda" Width="48px">Ruolo&nbsp;</asp:Label>
						<asp:dropdownlist id="ddl_ruolo" runat="server" CssClass="testo_grigio" Width="250px" AutoPostBack="True"></asp:dropdownlist></TD>
				</TR>
				<tr>
					<td height="3"></td>
				</tr>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:Label id="Label2" runat="server" CssClass="titolo_Scheda" Width="48px">Utente&nbsp;</asp:Label>
						<asp:dropdownlist id="ddl_utente" runat="server" CssClass="testo_grigio" Width="250px"></asp:dropdownlist></TD>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="middle" height="30">
						<asp:button id="btn_salva" runat="server" CssClass="pulsante" Text="SALVA"></asp:button>&nbsp;
						<INPUT class="PULSANTE" id="btn_annulla" style="WIDTH: 58px; HEIGHT: 20px" onclick="javascript:window.close()" type="button" value="ANNULLA" name="btn_annulla">
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
