<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Page language="c#" Codebehind="TipologieAtto.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_TipologieAtto.TipologieAtto" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<SCRIPT language="JavaScript">
		
			var cambiapass;
			var hlp;
			
			function apriPopup() {
				window.open('../help.aspx?from=TA','','width=450,height=500,scrollbars=YES');
			}			
			function cambiaPwd() {
				window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
			}			
		</SCRIPT>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
    		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Registri" />
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" width="100%" bgColor="#e0e0e0" height="20">Tipologie 
						atto</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- corpo centrale -->
						<table cellSpacing="0" cellPadding="0" align="center" border="0">
							<tr>
								<td align="center" height="25"></td>
							</tr>
							<tr>
								<td class="pulsanti" align="center" width="700">
									<table width="100%">
										<tr>
											<td class="titolo" align="left"><asp:label id="lbl_tit" Runat="server" CssClass="titolo">
												Lista Tipologie atto
											</asp:label></td>
											<td align="right"><asp:button id="btn_nuova" runat="server" CssClass="testo_btn" Text="Nuova tipologia"></asp:button>&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td style="HEIGHT: 157px" align="center">
									<DIV id="DivDGList" style="OVERFLOW: auto; WIDTH: 720px; HEIGHT: 151px"><asp:datagrid id="dg_Tipologie" runat="server" AutoGenerateColumns="False" BorderColor="Gray"
											CellPadding="1" BorderWidth="1px" Width="100%">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID">
													<HeaderStyle Width="0%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
													<HeaderStyle HorizontalAlign="Center" Width="90%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;" CommandName="Select">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" CommandName="Delete">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
										</asp:datagrid></DIV>
								</td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td vAlign="top"><asp:panel id="pnl_info" Runat="server" Visible="False">
										<TABLE class="contenitore" width="100%">
											<TR>
												<TD>
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD class="titolo_pnl" align="left">Dettaglio tipologia atto</TD>
															<TD class="titolo_pnl" align="right">
																<asp:ImageButton id="btn_chiudiPnlInfo" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif"></asp:ImageButton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD>
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD class="testo_grigio_scuro" align="right" width="100">Descrizione *&nbsp;</TD>
															<TD>
																<asp:TextBox id="txt_descrizione" Runat="server" CssClass="testo" Width="400px" MaxLength="64"></asp:TextBox></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD>
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD align="left" width="85%">
																<asp:Label id="lbl_msg" runat="server" CssClass="testo_rosso"></asp:Label></TD>
															<TD align="right" width="15%">
																<asp:Button id="btn_save" runat="server" CssClass="testo_btn" Text="Aggiungi"></asp:Button>&nbsp;
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</asp:panel></td>
							</tr>
						</table>
						<!-- fine corpo centrale --></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
