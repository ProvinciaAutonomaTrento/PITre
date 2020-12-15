<%@ Page language="c#" Codebehind="docSpedisci.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.docSpedisci" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
		function ChiudiOK()
		{
			this.returnValue=listaCertificati.selectCert.options[listaCertificati.selectCert.selectedIndex].Value;
			this.close();
		}

		function ChiudiKO()
		{
			this.returnValue="0";
			this.close();
		}
		</script>
		<script id="clientEventHandlersJS" language="javascript">
<!--

function selectCert_ondblclick() {
	if(listaCertificati.selectCert.selectedIndex>=0)
	{
		var idx = listaCertificati.selectCert.options[listaCertificati.selectCert.selectedIndex].Value;
		var store = new ActiveXObject("CAPICOM.Store");	
		store.Open(2, "MY", 0);
		var cert = store.certificates(idx);
		cert.Display();
	}
}

//-->
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" bottommargin="0" leftmargin="0" rightmargin="0" topmargin="0" onload="CaricaCertificati(listaCertificati.selectCert)">
		<form id="listaCertificati" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Spedisci" />
			<table align="center" border="0" class="info">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">Modalita' di spedizione</P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<td>
						<asp:radiobuttonlist id="rb_modSped_E" runat="server" CssClass="testo_grigio_scuro" Width="207px" TextAlign="Right" Height="10px">
							<asp:ListItem Value="I" Selected="True">Procedura di interoperabilit&#224;</asp:ListItem>
							<asp:ListItem Value="F">Fax</asp:ListItem>
						</asp:radiobuttonlist>&nbsp;
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<td class="pulsante" align="middle" height="7">
						<asp:Button id="btn_ok" runat="server" Text="Ok" CssClass="pulsante"></asp:Button>&nbsp;
						<asp:Button id="btn_annulla" runat="server" Text="Annulla" CssClass="pulsante"></asp:Button>
					</td>
				</tr>
				<tr>
					<td height="5"></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
