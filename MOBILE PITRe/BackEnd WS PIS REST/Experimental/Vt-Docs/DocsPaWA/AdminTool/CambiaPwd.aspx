<%@ Page language="c#" Codebehind="CambiaPwd.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.CambiaPwd" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function body_onLoad()
			{
				var newLeft=0;
				var newTop=0;
				window.moveTo(newLeft,newTop);
			}
		</script>
	</HEAD>
	<body leftMargin="0" topMargin="0" MS_POSITIONING="GridLayout" onload=body_onLoad();>
		<form id="Form1" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Cambia Password" />
			<table cellSpacing="0" cellPadding="0" width="400" align="center" border="0">
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="right" height="10">|&nbsp;&nbsp;&nbsp;<A href="javascript:self.close()">Chiudi</A>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><IMG height="48" src="Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td height="20" bgcolor="#c0c0c0" class="testo_grigio_scuro" align="center">Cambia 
						Password</td>
				</tr>
				<tr>
					<td class="titolo" align="center" height="20"><asp:Label id="lbl_msg" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td align="center">
						<!-- Pannello della login--><asp:panel id="pnl_login" Visible="True" Runat="server">
							<TABLE class="contenitore2" cellSpacing="0" cellPadding="5" border="0">
								<TR>
									<TD class="testo_grigio_scuro">UserID</TD>
									<TD class="testo_grigio_scuro">
										<asp:Label id="lbl_userid" runat="server"></asp:Label></TD>
								</TR>								
								<TR>
									<TD class="testo_grigio_scuro" colSpan="2">&nbsp;</TD>
								</TR>
								<TR>
									<TD class="testo_grigio_scuro">Nuova Password</TD>
									<TD class="testo_grigio_scuro">
										<asp:TextBox id="txt_newPwd" runat="server" CssClass="testo_grigio_scuro" TextMode="Password"></asp:TextBox></TD>
								</TR>
								<TR>
									<TD class="testo_grigio_scuro">Conferma</TD>
									<TD class="testo_grigio_scuro">
										<asp:TextBox id="txt_Conf_newPwd" runat="server" CssClass="testo_grigio_scuro" TextMode="Password"></asp:TextBox></TD>
								</TR>
								<TR>
									<TD align="right" colSpan="2">
										<asp:Button id="btn_cambia" runat="server" CssClass="testo_btn" Text="Cambia"></asp:Button>&nbsp;</TD>
								</TR>
							</TABLE>
						</asp:panel>
						<!-- Pannello dell'esito--><asp:panel id="pnl_esito" Visible="True" Runat="server"><BR>
							<BR>
							<asp:Label id="lbl_esito" Runat="server" CssClass="testo_grigio_scuro"></asp:Label>
						</asp:panel>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
