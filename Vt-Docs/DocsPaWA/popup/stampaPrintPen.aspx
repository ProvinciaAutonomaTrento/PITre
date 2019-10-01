<%@ Page language="c#" Codebehind="stampaPrintPen.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.stampaPrintPen" %>
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
		<script language="javascript">
			function stampaSegnatura() {
				var f_protocollo=window.opener.document.docProtocollo;
				if(f_protocollo.lbl_segnatura.value != "") {
					try {	
					    document.stampaPrintPen.ctrlPrintPen.Dispositivo = f_protocollo.hd_dispositivo.value;
						document.stampaPrintPen.ctrlPrintPen.Text = f_protocollo.lbl_segnatura.value;
						document.stampaPrintPen.ctrlPrintPen.Stampa();
					} catch(e) {
						alert("Errore.\n" + e.toString());
					}
				} else {
					alert('Numero di protocollo non assegnato!');
				}
				return false;
			}
		</script>
	</HEAD>
	<body onload="javascript:setSegnatura();" MS_POSITIONING="GridLayout">
		<form id="stampaPrintPen" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Procedura di stampa" />
			<table id="tbl_contenitore" class="info" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Procedura di stampa</asp:label></P>
					</td>
				</TR>
				<tr>
					<td align="middle" height="30">&nbsp;
						<asp:Label id="Label2" runat="server" CssClass="testo_grigio_scuro" Width="272px">Si vuole procedere con la stampa?</asp:Label></td>
				</tr>
				<tr>
					<td align="middle"><asp:label id="lbl_segnatura" runat="server" CssClass="testo_red" Width="40px" Visible="False"></asp:label><asp:button id="btn_ok" runat="server" CssClass="pulsante" Text="OK"></asp:button><asp:button id="btn_chiudi" runat="server" CssClass="pulsante" Text="CHIUDI"></asp:button></td>
				</tr>
				<TR height="0">
					<TD>
						<OBJECT id="ctrlPrintPen" codeBase="../activex/DocsPa_PrintPen.CAB#version=1,0,0,0" height="1" width="1" classid="CLSID:2860F27F-FC9F-4CDA-B0CB-55A5BD09C52E" VIEWASTEXT>
							<PARAM NAME="_ExtentX" VALUE="26">
							<PARAM NAME="_ExtentY" VALUE="26">
							<PARAM NAME="PortaCOM" VALUE="1">
							<PARAM NAME="Text" VALUE="DFPrintPen Test">
							<PARAM NAME="NumCopie" VALUE="1">
							<PARAM NAME="TimeOut" VALUE="60">
							<PARAM NAME="Dispositivo" VALUE="Penna">
							<PARAM NAME="Q1" VALUE="Q690">
							<PARAM NAME="Q2" VALUE="24+0">
							<PARAM NAME="P1" VALUE="A63">
							<PARAM NAME="P2" VALUE="696">
							<PARAM NAME="P3" VALUE="3">
							<PARAM NAME="P4" VALUE="4">
							<PARAM NAME="P5" VALUE="3">
							<PARAM NAME="P6" VALUE="1">
						</OBJECT>
					</TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
