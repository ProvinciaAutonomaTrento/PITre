<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="sceltaRuolo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.sceltaRuolo" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > Homepage_DPA</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="CSS/DocsPA.css" type="text/css" rel="stylesheet">
		<script>
		function Body_OnMouseOver()
		{
			try
			{
				if(top.frames[0].document!=null)
					if(top.frames[0].document.Script!=null)
					if(top.frames[0].document.Script["closeIt"]!=null)
						top.frames[0].document.Script.closeIt();
			}
			catch(exc)
			{;}	
		}
		</script>
</HEAD>
	<body onmouseover="Body_OnMouseOver()" bgColor="#f2f2f2" MS_POSITIONING="GridLayout" language=javascript>
		<form id="Home_DPA" action="Home_DPA.aspx" method="post" runat="server">
			<table height="100%" cellSpacing="1" cellPadding="1" width="100%" align="left" bgColor="#f2f2f2" border="0">
				<!--  tabella creata da noi   -->
				<tr>
					<td vAlign="top" width="40%" height="10">
						<table cellSpacing="1" cellPadding="1" border="0">
							<tr vAlign="bottom">
								<td class="testoBlue" align="left" width="79"><asp:label id="lbl_ruoli" runat="server" Height="16px" Width="41px" CssClass="testo_grigio">Ruoli</asp:label></td>
								<td class="testoBlue" width="80%"><asp:dropdownlist id="chklst_ruoli" runat="server" Width="249px" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist></td>
							</tr>
							<tr vAlign="bottom" height="20">
								<td align="left"><asp:label id="lbl_cod" Height="16px" CssClass="testo_grigio" Runat="server">Codice</asp:label></td>
								<td><asp:textbox id="Cod_ruolo" runat="server" Width="100px" CssClass="testo_grigio" MaxLength="16" ReadOnly="True"></asp:textbox></td>
							</tr>
							<!--
							<tr vAlign="top">
								<td vAlign="center" align="left"><asp:label id="lbl_livello" Height="10px" CssClass="testo_grigio" Runat="server">Livello</asp:label></td>
								<td><asp:textbox id="livello_ruolo" runat="server" Width="77px" CssClass="testo_grigio" MaxLength="2" ReadOnly="True"></asp:textbox></td>
							</tr>
							-->
							<tr>
								<td align="left"><asp:label id="lbl_descr" CssClass="testo_grigio" Runat="server">Descrizione</asp:label></td>
								<td><asp:textbox id="descr_ruolo" runat="server" Width="250px" CssClass="testo_grigio" MaxLength="80" ReadOnly="True" Rows="1"></asp:textbox></td>
							</tr>
							<tr>
								<td colSpan="2"></td>
							</tr>
						</table>
					</td>
					<td vAlign="top">
						<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" align="left" border="0">
							<TR>
								<TD class="menu_1_bianco" align="middle" bgColor="#4b4b4b" height="14">Struttura 
									di appartenenza</TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff" height="3">
									<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TBODY>
											<TR>
												<TD background="images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
											</TR>
										</TBODY>
									</TABLE>
								</TD>
							</TR>
							<TR vAlign="top">
								<TD><asp:table id="TableUO" runat="server" CssClass="testo_grigio"></asp:table></TD>
							</TR>
							<tr>
								<td colSpan="2"></td>
							</tr>
							<TR>
								<TD class="menu_1_bianco" align="middle" bgColor="#4b4b4b" colSpan="2" height="14">cose 
									da fare</TD>
							</TR>
							<TR>
								<TD bgColor="#ffffff" colSpan="2" height="3">
									<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TBODY>
											<TR>
												<TD background="images/proto/tratteggio_gri_o.gif"><IMG height="1" src="../images/proto/tratteggio_gri_o.gif" width="6" border="0"></TD>
											</TR>
										</TBODY>
									</TABLE>
								</TD>
							</TR>
							<TR>
								<td>
									<table>
										<tr>
											<TD class="testo_grigio">OGGETTO TRASMESSO
											</TD>
											<td><asp:dropdownlist id="DDLOggettoTab1" runat="server" Height="33" Width="147px" CssClass="testo_grigio" AutoPostBack="True">
													<asp:ListItem Value="" Selected="True"></asp:ListItem>
													<asp:ListItem Value="D">Documento</asp:ListItem>
													<asp:ListItem Value="P">Fascicolo</asp:ListItem>
												</asp:dropdownlist></td>
											<td></td>
										</tr>
									</table>
								</td>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td bgColor="#f2f2f2">
						<cf1:iframewebcontrol id="iFrame_portale" runat="server" iWidth="100%" iHeight="100%" Marginwidth="0" Marginheight="0" Frameborder="0" Scrolling="no" NavigateTo="blank_page.htm"></cf1:iframewebcontrol></td>
					<td>
						<cf1:iframewebcontrol id="iFrame_sx" runat="server" Scrolling="auto" Frameborder="0" Marginheight="0" Marginwidth="0" iHeight="100%" iWidth="100%"></cf1:iframewebcontrol></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
