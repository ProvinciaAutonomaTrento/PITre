<%@ Page language="c#" Codebehind="InfoRubrica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.InfoRubrica" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta name="vs_showGrid" content="True">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>
	</HEAD>
	<body rightmargin="0" topmargin="0" bottommargin="0" leftMargin="0" MS_POSITIONING="GridLayout">
		<form id="InfoRubrica" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Info rubrica" />
			<TABLE  id="Table1" align="center" cellSpacing="0" cellPadding="0" width="95%" border="0" class="info_grigio">
				<tr height="20">
					<td align="center" class="titolo_grigio">Dettagli Corrispondente: 
						<asp:Label id="lbl_nomeCorr"  runat="server"></asp:Label>
					</TD>
				</TR>
				<!--TR>
					<TD class="menu_1_bianco" bgColor="#c08682" colSpan="5"></TD>
				</TR-->
				<TR>
					<TD align="middle">
						<asp:Label id="lbl_DettCorr" runat="server" cssclass=testo_grigio
						></asp:Label></TD>
				</TR>
				<!--TR>
					<TD class="menu_1_bianco" bgColor="#c08682" colSpan="5"></TD>
				</TR-->
			</TABLE>
		</form>
	</body>
</HTML>
