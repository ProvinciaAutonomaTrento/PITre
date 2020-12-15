<%@ Page language="c#" Codebehind="newErrorPage.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.newErrorePage" %>
<%@ Register src="UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>
		
		<LINK href="CSS/DocsPA.css" type="text/css" rel="stylesheet">
  </HEAD>
	<body  MS_POSITIONING="GridLayout">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Errore" />
		<table cellSpacing="0" cellPadding="0" width="100%" border="0" >
			<tr vAlign="top">
				<td>
					<form id="form" method="post" runat="server">
						<table height="245" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0"
							bgColor="#f2f2f2">
							<tr>
								<td height="3"></td>
							</tr>
							<tr vAlign="top">
								<td>
									<table class="info" height="100%" cellSpacing="0" cellPadding="0" width="97%" align="center"
										border="0">
										<tr vAlign="top" height="10%" bgColor="#810d06">
											<td valign="middle" align="center" style="BORDER-RIGHT: #e2e2e2 1px solid; BORDER-TOP: #e2e2e2 1px solid; BORDER-LEFT: #e2e2e2 1px solid; BORDER-BOTTOM: #e2e2e2 1px solid">
												<!--<table width="97%" align="center" border="1">
													<tr bgColor="#810d06">
														<td align="center" vAlign="center">-->
												<asp:label id="Label1" Font-Bold="True" Font-Names="verdana;arial" Font-Size="X-Small" Runat="server"
													BorderColor="#F2F2F2" ForeColor="White" Width="120px" Height="12px"> Avviso di errore </asp:label>
												<!--</td>
													</tr>
												</table>-->
											</td>
										</tr>
										<TR height="20%" valign=bottom><td class="testo_grigio_scuro" align="center">ATTENZIONE</td></TR>
										<tr  height="40%">
											<td class="testo_grigio_scuro" align="center" vAlign="top">
											    <asp:label id="lbl_mgserrore" Runat="server">
											</asp:label></td>
										</tr>
										<tr height="30%" vAlign="top">
											<td class="testo_grigio_scuro" valign="middle" align="center"><asp:label id="proc" Runat="server" Visible="False"></asp:label></td>
										</tr>
										<TR>
											<td align="center" height="30">
												<INPUT class="PULSANTE" id="btn_chiudi" runat="server" type="button" value="CHIUDI">
											</td>
										</TR>
									</table>
								</td>
							</tr>
							<tr vAlign="top">
								<td height="3"></td>
							</tr>
						</table>
					</form>
				</td>
			</tr>
		</table>
	</body>
</HTML>
