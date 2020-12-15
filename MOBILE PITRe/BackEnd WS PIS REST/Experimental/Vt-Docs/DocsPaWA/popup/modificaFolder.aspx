<%@ Page language="c#" Codebehind="modificaFolder.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.modificaFolder" %>
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
		<form id="modificaFolder" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Modifica dati folder" />
			<TABLE id="tbl_modificaFascicolo" class="info" width="380" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							<asp:Label id="Label1" runat="server">Modifica dati folder</asp:Label></P>
					</td>
				</TR>
				<TR>
					<TD height="40">
						<asp:Label id="lbl_nome" runat="server" CssClass="titolo_scheda" Width="18px" Height="15px">&nbsp;Nome&nbsp;</asp:Label>
						<asp:TextBox id="txt_nomeFolder" runat="server" Width="300px" CssClass="testo_grigio"></asp:TextBox></TD>
				</TR>
				<TR>
					<TD align="middle"  height="30">
						<asp:button id="btn_salva" runat="server" CssClass="pulsante" Text="SALVA"></asp:button>&nbsp;
						<INPUT class="PULSANTE" id="btn_annulla" style="WIDTH: 58px; HEIGHT: 20px" onclick="javascript:window.close()" type="button" value="ANNULLA" name="btn_annulla">
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
