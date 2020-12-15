<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="paroleChiave.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.paroleChiave" %>
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
	<body MS_POSITIONING="GridLayout" bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" >
		<form id="paroleChiave" method="post" runat="server" action="">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Parole chiave" />
			<table style="WIDTH: 472px; HEIGHT: 372px" align="center" border="0" class="info">
				<TR>
					<td class="item_editbox" colspan=2>
						<P class="boxform_item"><asp:label id="Label1" runat="server">Parole chiave</asp:label>
						</P>
					</td>
				</TR>
				<TR>
					<TD height="5" colspan=2></TD>
				</TR>
				<tr>
					<td vAlign="middle"><IMG height="1" src="../images/proto/spaziatore.gif" width="3" border="0">
						<cc1:imagebutton id="btn_aggiungi" DisabledUrl="../images/proto/aggiungi.gif" AlternateText="Aggiungi parola chiave"
							ImageUrl="../images/proto/aggiungi.gif" Tipologia="DO_DOC_ADD_PAROLA" Runat="server" Visible="False"></cc1:imagebutton></td>
					<td>
						<INPUT id="h_aggiorna" style="WIDTH: 66px; HEIGHT: 22px" type="hidden" size="5" runat="server"></td>
				</tr>
				<TR>
					<td align="center" height="50" colspan=2><asp:listbox id="ListParoleChiave" runat="server" CssClass="testo_grigio" SelectionMode="Multiple"
							Width="450px" Height="260px"></asp:listbox></td>
				</TR>
				<TR>
					<TD height="10px" colspan=2></TD>
				</TR>
				<TR>
					<td align="center" height="30" colspan=2>
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Width="39px" Height="19px" Text="OK"></asp:button>&nbsp;
						<INPUT class="PULSANTE" id="btn_chiudi" style="WIDTH: 58px; HEIGHT: 20px" onclick="javascript:window.close()"
							type="button" value="CHIUDI">
					</td>
				</TR>
			</table>
		</form>
	</body>
</HTML>
