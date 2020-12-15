<%@ Page language="c#" Codebehind="login.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Login.login" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<SCRIPT language="javascript">
			function body_onLoad()
			{
				var maxWidth=570;
				var maxHeight=320;

				window.resizeTo(maxWidth,maxHeight);
				
				var newLeft=(screen.availWidth-maxWidth)/2;
				var newTop=(screen.availHeight-maxHeight)/2;
				window.moveTo(newLeft,newTop);
				
			}
		</SCRIPT>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript" id="btn_accedi_click" event="onclick()" for="btn_accedi">
			window.document.body.style.cursor='wait';
		</script>
		<script language="javascript" id="btn_new_session_click" event="onclick()" for="btn_new_session">
			window.document.body.style.cursor='wait';   
		</script>
	</HEAD>
	<body leftMargin="0" topMargin="0" onload="body_onLoad()" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Login" />
			<input id="hd_userid" type="hidden" name="hd_userid" runat="server"> <input id="hd_pwd" type="hidden" name="hd_pwd" runat="server">
			<input id="hd_tipoAmm" type="hidden" name="hd_tipoAmm" runat="server">
			<table cellSpacing="0" cellPadding="0" width="400" align="center" border="0">
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="right" height="10">|&nbsp;&nbsp;&nbsp;<A href="javascript:self.close()">Chiudi</A>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><asp:image runat="server" ID="img_logo" height="48" ImageUrl="../Images/logo.gif" width="218" border="0"></asp:image></td>
				</tr>
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td bgColor="#b16f6f" height="6"></td>
				</tr>
				<tr>
					<td height="2">&nbsp;</td>
				</tr>
				<tr>
					<td align="center" height="15"><asp:label id="lbl_error" runat="server" CssClass="titolo"></asp:label></td>
				</tr>
				<tr>
					<td align="center">
						<!-- Pannello della login--><asp:panel id="pnl_login" Visible="True" Runat="server">
							<TABLE class="contenitore2" cellSpacing="0" cellPadding="5" border="0">
								<TR>
									<TD class="testo_grigio_scuro">UserID</TD>
									<TD class="testo_grigio_scuro">
										<asp:TextBox id="txt_userid" tabIndex="1" runat="server" CssClass="testo_grigio_scuro"></asp:TextBox></TD>
								</TR>
								<TR>
									<TD class="testo_grigio_scuro">Password</TD>
									<TD class="testo_grigio_scuro">
										<asp:TextBox id="txt_pwd" tabIndex="2" runat="server" CssClass="testo_grigio_scuro" TextMode="Password"></asp:TextBox></TD>
								</TR>
								<TR>
									<TD align="right" colSpan="2">
										<asp:Button id="btn_accedi" tabIndex="3" runat="server" CssClass="testo_btn" Text="Accedi"></asp:Button>&nbsp;</TD>
								</TR>
							</TABLE>
						</asp:panel><asp:panel id="pnl_exist_login" Visible="False" Runat="server">
							<TABLE class="contenitore2" cellSpacing="0" cellPadding="5" width="90%" border="0">
								<TR>
									<TD class="testo_grigio_scuro" tabIndex="4" align="center" colSpan="2">Attenzione,<BR>
										questo utente risulta già connesso</TD>
								</TR>
								<TR>
									<TD align="left">&nbsp;
										<asp:Button id="btn_annulla" tabIndex="6" runat="server" CssClass="testo_btn" Text="Annulla"></asp:Button></TD>
									<TD align="right">
										<asp:Button id="btn_new_session" tabIndex="5" runat="server" CssClass="testo_btn" Text="Nuova sessione"></asp:Button>&nbsp;</TD>
								</TR>
							</TABLE>
						</asp:panel></td>
				</tr>
				<tr>
					<td height="3">&nbsp;</td>
				</tr>
				<tr>
					<td bgColor="#b16f6f" height="6"></td>
				</tr>
				<tr>
					<td align="right"><asp:label id="lbl_version" CssClass="testo_versione" Runat="server" tabIndex="7"></asp:label></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
