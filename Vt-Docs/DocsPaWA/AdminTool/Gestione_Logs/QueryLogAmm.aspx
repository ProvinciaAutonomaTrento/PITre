<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Page language="c#" Codebehind="QueryLogAmm.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Logs.QueryLogAmm" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuLog" Src="../UserControl/MenuLogAmm.ascx" %>
<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="LogCalendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
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
				hlp = window.open('../help.aspx?from=GLQ','','width=450,height=500,scrollbars=YES');
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
				if(typeof(obj_calwindow) != 'undefined')
				{
					if(!obj_calwindow.closed)
						obj_calwindow.close();
				}
			}					
			
			function StampaRisultatoRicerca()
			{				
				var args=new Object;
				args.window=window;
				window.showModalDialog("exportLog.aspx?export=Amministrazione",
										args,
										"dialogWidth:450px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}
		</SCRIPT>
		<script language="javascript" id="btn_cerca_click" event="onclick()" for="btn_cerca">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Ricerca log Amministrazione" />
		<!-- Gestione del menu a tendina --><uc3:menutendina id="MenuTendina" runat="server"></uc3:menutendina>
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
					<td class="titolo" align="center" width="100%" bgColor="#e0e0e0" height="20">Ricerca log Amministrazione</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table height="100" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr height="100%" width="100%">
								<td width="120" height="100%"><uc2:menulog id="MenuLogAmm" runat="server"></uc2:menulog>
									<DIV id="DivSel" style="OVERFLOW: auto; HEIGHT: 388px"></DIV>
								</td>
								<td vAlign="top" align="center" width="100%" height="100%"><br>
									<table cellSpacing="0" cellPadding="0" align="center" border="0">
										<tr>
											<td class="pulsanti" align="center" width="840"><table width="100%">
													<tr>
														<td align="left"><asp:label id="lbl_tit" Runat="server" CssClass="titolo"></asp:label></td>
														<td align="right"><asp:button id="btn_cerca" runat="server" CssClass="testo_btn" Text="Ricerca"></asp:button>&nbsp;</td>
													</tr>
													<tr>
														<td colSpan="2">
															<table cellSpacing="0" cellPadding="2" width="100%" border="0">
																<tr>
																	<td class="testo_grigio_scuro" align="right" width="10%">Data da:</td>
																	<td class="testo_grigio_scuro" width="35%" valign="middle">
																	    <uc4:Calendario id="txt_data_da" runat="server" Visible="true" />&nbsp;<asp:ImageButton ID="eliminaDataDa" runat="server" ImageUrl="../Images/noData.gif" width="16" height="16" border="0" alt="Elimina la data DA" />
																	&nbsp;&nbsp;a:&nbsp;
																		<uc4:Calendario id="txt_data_a" runat="server" Visible="true" />&nbsp;<asp:ImageButton ID="eliminaDataA" runat="server" ImageUrl="../Images/noData.gif" width="16" height="16" border="0" alt="Elimina la data A" />
																	</td>
																	<td class="testo_grigio_scuro" align="right" width="15%">UserId utente:</td>
																	<td width="40%">
																		<table cellSpacing="0" cellPadding="0" width="100%" border="0">
																			<tr>
																				<td align="left"><asp:textbox id="txt_user" runat="server" CssClass="testo" Width="160px" MaxLength="20"></asp:textbox></td>
																				<td align="right" class="testo_grigio_scuro">Esito:&nbsp;<asp:dropdownlist id="ddl_esito" runat="server" CssClass="testo" Width="80px" AutoPostBack="False">
																						<asp:ListItem Value="null" Selected="True">Tutti</asp:ListItem>
																						<asp:ListItem Value="1">Positivo</asp:ListItem>
																						<asp:ListItem Value="0">Negativo</asp:ListItem>
																					</asp:dropdownlist></td>
																			</tr>
																		</table>
																	</td>
																</tr>
																<tr>
																    <td colspan="4" class="testo_grigio_scuro">Oggetto:&nbsp;<asp:dropdownlist id="ddl_oggetto" runat="server" CssClass="testo" Width="213px" AutoPostBack="True">
																			<asp:ListItem Value="null" Selected="True">Seleziona...</asp:ListItem>
																		</asp:dropdownlist>&nbsp;&nbsp;&nbsp;&nbsp;Azione:&nbsp;<asp:dropdownlist id="ddl_azione" runat="server" CssClass="testo" Width="355px"></asp:dropdownlist>
																		&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="ckb_Log" runat="server" Text="Log" CssClass="testo" Checked="true" />&nbsp;
																		            <asp:CheckBox ID="ckb_Storico" runat="server" Text="Storico" CssClass="testo" />
																		</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td align="center">
												<br>
												<asp:panel id="pnl_log" Visible="False" Runat="server">
												<table width="100%">
											        <tr>
											            <td align="right"><asp:imagebutton id="btn_stampa" Runat="server" ImageUrl="../../images/proto/export.gif" AlternateText="Esporta il risultato della ricerca"></asp:imagebutton></td>
											        </tr>
											        <tr>
											            <td>
								                        <DIV id="DivDGList" style="OVERFLOW: auto; HEIGHT: 430px">
														<asp:datagrid id="dg_Log" runat="server" Width="840px" Visible="True" AutoGenerateColumns="False"
															BorderColor="Gray" CellPadding="1" BorderWidth="1px">
															<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
															<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
															<ItemStyle CssClass="bg_grigioN"></ItemStyle>
															<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
															<Columns>
																<asp:BoundColumn DataField="utente" HeaderText="Utente"></asp:BoundColumn>
																<asp:BoundColumn DataField="data" ReadOnly="True" HeaderText="Data">
																	<ItemStyle HorizontalAlign="Center"></ItemStyle>
																</asp:BoundColumn>
																<asp:BoundColumn DataField="oggetto" HeaderText="Oggetto"></asp:BoundColumn>
																<asp:BoundColumn DataField="operazione" HeaderText="Operazione"></asp:BoundColumn>
																<asp:BoundColumn DataField="descrizione" HeaderText="Informazioni"></asp:BoundColumn>
																<asp:BoundColumn DataField="esito" HeaderText="Esito">
																	<ItemStyle HorizontalAlign="Center"></ItemStyle>
																</asp:BoundColumn>
															</Columns>
														</asp:datagrid></DIV>
											            </td>
											        </tr>
											    </table>
												</asp:panel>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<!-- FINE CORPO CENTRALE -->
					</td>				
				</tr>
			</table>
		</form>
	</body>
</HTML>
