<%@ Page language="c#" Codebehind="listaCertificati.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.listaCertificati" %>
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
	<body MS_POSITIONING="GridLayout" onload="CaricaCertificati(listaCertificati.selectCert)">
		<form id="listaCertificati" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Lista certificati" />    
			<table align="center" border="0" class="info">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							<asp:Label id="Label1" runat="server">Lista certificati</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<td class="testo_grigio" align="center">
						<SELECT id="selectCert" name="selectCert" style="WIDTH: 300px; HEIGHT: 151px" size="9" language="javascript"
							ondblclick="return selectCert_ondblclick()" class="testo_grigio">
						</SELECT>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<td class="pulsante" align="center" height="30">
						<INPUT class="pulsante" type="button" value="OK" onclick="ChiudiOK()"> <INPUT class="pulsante" type="button" value="ANNULLA" onclick="ChiudiKO();">
					</td>
				</tr>
			</table>
			&nbsp;
		</form>
	</body>
</HTML>
