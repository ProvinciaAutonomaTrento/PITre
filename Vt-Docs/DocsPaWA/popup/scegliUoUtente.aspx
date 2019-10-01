<%@ Page language="c#" Codebehind="scegliUoUtente.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.scegliUoUtente" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="rubricaSemplice" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Seleziona Unita' Organizzativa" />
			<TABLE class="info" id="Table1" align="center" border="0">
				<TR>
					<td class="item_editbox" colSpan="2">
						<P class="boxform_item"><asp:label id="Label1" runat="server"> Seleziona l'Unità Organizzativa per il mittente </asp:label></P>
					</td>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<tr>
					<td align="center" height="25"><asp:label id="lbl_cod_rubr" runat="server" Visible="True" CssClass="testo_grigio_scuro" Font-Size="13px"></asp:label></td>
				</tr>
				<tr>
					<td height="3"></td>
				</tr>
				<TR>
					<TD align="center"><asp:listbox id="ListaUoUtente" runat="server" CssClass="testo_grigio" Width="530px" Height="150px"
							AutoPostBack="True"></asp:listbox></TD>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="center" height="30">&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="PULSANTE" Width="61px" Height="19px" Text="ANNULLA"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
