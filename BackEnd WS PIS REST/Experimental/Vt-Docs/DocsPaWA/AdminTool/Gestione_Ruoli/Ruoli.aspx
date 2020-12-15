<%@ Page language="c#" Codebehind="Ruoli.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Ruoli.Ruoli" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
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
				hlp = window.open('../help.aspx?from=RU','','width=450,height=500,scrollbars=YES');
			}
			function DivHeight()
			{
				if (DivDGList.scrollHeight< 229) 
					DivDGList.style.height=DivDGList.scrollHeight;
				else
					DivDGList.style.height=229;
			}			
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
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
			function ApriGestioneUtenti(codAmm,codRuolo,descRuolo) 
			{			
				var myUrl = "Utenti_TipoRuolo.aspx?codAmm="+codAmm+"&codRuolo="+codRuolo+"&descRuolo="+descRuolo;				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:700px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
			}
		</SCRIPT>
		<SCRIPT language="JavaScript">
		
			function ShowValidationMessage(message,warning)
			{
				if (warning)
				{
					if (window.confirm(message + "\n\nContinuare?"))
					{
						Form1.submit();
					}
				}
				else
				{
					alert(message);
				}
			}
		
			// Permette di inserire solamente caratteri numerici
			function ValidateNumericKey()
			{
				var inputKey=event.keyCode;
				var returnCode=true;
				
				if(inputKey > 47 && inputKey < 58)
				{
					return;
				}
				else
				{
					returnCode=false; 
					event.keyCode=0;
				}
				
				event.returnValue = returnCode;
			}
					
		</SCRIPT>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript: DivHeight();"
		rightMargin="0" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Ruoli" />
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Tipi ruolo</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<table cellSpacing="0" cellPadding="0" align="center" border="0">
							<tr>
								<td align="center" height="25"></td>
							</tr>
							<tr>
								<td class="pulsanti" align="center" width="700">
									<table width="100%">
										<tr>
											<td align="left"><asp:label id="lbl_tit" CssClass="titolo" Runat="server">Lista tipi ruolo</asp:label></td>
											<td align="right"><asp:button id="btn_nuovoRuolo" runat="server" CssClass="testo_btn" Text="Nuovo tipo ruolo"></asp:button>&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td style="HEIGHT: 157px" align="center" valign="top">
									<DIV id="DivDGList" style="OVERFLOW: auto; WIDTH: 720px; HEIGHT: 150px">
										<asp:datagrid id="dg_ruoli" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
											AutoGenerateColumns="False" Width="100%">
											<SelectedItemStyle HorizontalAlign="Left" CssClass="bg_grigioS"></SelectedItemStyle>
											<EditItemStyle HorizontalAlign="Left"></EditItemStyle>
											<AlternatingItemStyle HorizontalAlign="Left" CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle HorizontalAlign="Left" CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID">
													<HeaderStyle Width="0px"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="CODICE" HeaderText="Codice">
													<HeaderStyle Width="30%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Descrizione">
													<HeaderStyle Width="50%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="LIVELLO" HeaderText="Livello">
													<HeaderStyle Width="10%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
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
										</asp:datagrid>
									</DIV>
								</td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td vAlign="top">
									<asp:panel id="pnl_info" Runat="server" Visible="False">
										<TABLE class="contenitore" width="100%">
											<TR>
												<TD colSpan="3">
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD class="titolo_pnl" align="left">Dettagli ruolo</TD>
															<TD class="titolo_pnl" align="right">
																<asp:ImageButton id="btn_chiudiPnlInfo" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif"></asp:ImageButton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD width="30%">
													<TABLE>
														<TR>
															<TD class="testo_grigio_scuro" vAlign="middle" align="left">Codice *&nbsp;</TD>
															<TD vAlign="top">
																<asp:TextBox id="txt_codice" Runat="server" CssClass="testo" Width="100px" MaxLength="20"></asp:TextBox>
																<asp:Label id="lbl_cod" runat="server" CssClass="testo"></asp:Label></TD>
														</TR>
													</TABLE>
												</TD> <!--/TR>
												<TR-->
												<TD width="50%">
													<TABLE>
														<TR>
															<TD class="testo_grigio_scuro" align="left">Descrizione *&nbsp;</TD>
															<TD>
																<asp:TextBox id="txt_descrizione" Runat="server" CssClass="testo" Width="250px" MaxLength="128"></asp:TextBox></TD>
														</TR>
													</TABLE>
												</TD> <!--/TR>
												<TR-->
												<TD width="20%">
													<TABLE>
														<TR>
															<TD class="testo_grigio_scuro" align="left" colSpan="3">Livello *&nbsp;</TD>
															<TD>
																<asp:TextBox id="txt_livello" Runat="server" CssClass="testo" Width="35px" MaxLength="3"></asp:TextBox></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD align="left" colSpan="3">
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD align="left" width="70%"></TD>
															<TD vAlign="bottom" align="right" width="30%">
																<asp:Button id="btn_utenti" runat="server" CssClass="testo_btn" Text="Utenti nel ruolo" Visible="False"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;
																<asp:Button id="btn_salva" runat="server" CssClass="testo_btn" Text="Salva"></asp:Button>&nbsp;
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</asp:panel>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
