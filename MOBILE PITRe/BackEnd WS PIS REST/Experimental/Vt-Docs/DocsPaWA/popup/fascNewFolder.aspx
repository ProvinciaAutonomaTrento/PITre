<%@ Page language="c#" Codebehind="fascNewFolder.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.fascNewFolder" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function window_onload() {
				document.all('txt_descFolder').focus();
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" onload="return window_onload()">
		<form id="insertNewFolder" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Nuova cartella" />
			<table border="0" class="info" align="center" style="WIDTH: 373px; HEIGHT: 131px">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:Label id="Label1" runat="server" CssClass="menu_grigio_popup">Nuovo sotto fascicolo</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<td><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
						<asp:Label id="lbl_note" runat="server" CssClass="titolo_scheda" Height="36px">Descrizione&nbsp;</asp:Label>
						<asp:TextBox id="txt_descFolder" runat="server" TextMode="MultiLine" Width="282px" Height="38px"
							CssClass="testo_grigio"></asp:TextBox>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<td align="center" height="30">
						<asp:button id="btn_salva" runat="server" CssClass="pulsante" Text="SALVA"></asp:button>&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="pulsante" Text="ANNULLA"></asp:button>
					</td>
				</TR>
			</table>
		</form>
	</body>
</HTML>
