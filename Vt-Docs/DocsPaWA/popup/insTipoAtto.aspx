<%@ Page language="c#" Codebehind="insTipoAtto.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.insTipoAtto" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat = "server">
		<title></title>  
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" leftMargin="2" topMargin="2" >
		<form id="insTipoAtto" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Inserimento Tipo Atto" />
			<TABLE id="tbl_insOggetto" class="info" width="400" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							<asp:label id="Label1" runat="server" Width="177px">Inserimento Tipo Atto</asp:label>
						</P>
					</td>
				</TR>
				<TR>
					<TD height="5"></TD>
				</TR>
				<TR height="22">
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
						<asp:label id="lbl_tipoAtto" runat="server" CssClass="titolo_scheda">Tipo atto&nbsp;*</asp:label>
						<asp:textbox id="txt_tipoAtto" runat="server" CssClass="testo_grigio" Width="290px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD height="10"></TD>
				</TR>
				<TR>
					<TD align="center" height="30">
						<asp:button id="btn_Insert" runat="server" CssClass="pulsante" Text="INSERISCI"></asp:button>
						<asp:button id="Button1" runat="server" CssClass="pulsante" Text="CHIUDI"></asp:button></TD>
				</TR>
				<TR>
					<TD height="3"></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
