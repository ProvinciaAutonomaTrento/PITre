<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Page language="c#" Codebehind="DiagrammiStato.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_DiagrammiStato.DiagrammiStato" EnableEventValidation="false" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
        <title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<script language="javascript">
		    var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-500)/2;
			var new_h = (h-400)/2;
		    
		    function apriPopupGestioneStato()
			{
			    //window.open('AssociaModelli.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=160,scrollbars=NO');				
				window.showModalDialog('ModalDiagrammiStato.aspx?Chiamante=GestioneStato.aspx','','dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
				Form1.submit();
            }

            function ApriPopupVisibilita() {
                //window.showModalDialog('ModalDiagrammiStato.aspx?Chiamante=VisibilitaStati.aspx', '', 'dialogWidth:800px;dialogHeight:650px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
                window.showModalDialog('VisibilitaStati.aspx', '', 'dialogWidth:800px;dialogHeight:650px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
            }
		</script> 
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0">
		<form id="Form1" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Diagrammi di stato" />
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" style="HEIGHT: 20px" align="center" bgColor="#e0e0e0" height="34">Diagrammi 
						di stato</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table width="75%" cellSpacing="0" cellPadding="0" align="center" border="0">
							<tr>
								<td align="center" height="25"><asp:label id="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
							</tr>
							<tr>
								<td class="pulsanti" align="center">
									<table width="100%">
										<tr>
											<td align="left"><asp:label id="lbl_titolo" runat="server" CssClass="titolo">Lista Diagrammi di stato</asp:label></td>
											<td align="right"><asp:button id="btn_nuovoDiagr" runat="server" CssClass="testo_btn_p" Text="Nuovo"></asp:button>&nbsp;
												<asp:button id="btn_salva" runat="server" CssClass="testo_btn_p" Text="Salva"></asp:button>&nbsp;
												<asp:button id="btn_listaDiagrammi" runat="server" CssClass="testo_btn_p" Text="Diagrammi"></asp:button></td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- INIZIO: PANNELLI -->
							<tr>
								<td align="center" height="5"><BR>
									<!-- INIZIO : PANNELLO LISTA DIAGRAMMI -->
									<asp:panel id="Panel_ListaDiagrammi" Runat="server">
										<DIV id="DivGgListaDiagrammi" align="center" runat="server">
											<asp:DataGrid id="dg_listaDiagrammi" runat="server" Width="100%" AutoGenerateColumns="False" PageSize="6"
												AllowPaging="True" Height="6px" OnItemCreated="dg_listaDiagrammi_ItemCreated">
												<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
												<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
												<ItemStyle CssClass="bg_grigioN"></ItemStyle>
												<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="System_id" HeaderText="System_ id"></asp:BoundColumn>
													<asp:BoundColumn DataField="Nome diagramma" HeaderText="Nome diagramma">
														<HeaderStyle Width="90%"></HeaderStyle>
													</asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="Visibilita" Text="&lt;img src=../Images/utenti.gif border=0 alt='Visibilità'&gt;" HeaderText="Visibilità">
                                                        <HeaderStyle Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    </asp:ButtonColumn>

													<asp:ButtonColumn Text="&lt;img src=../images/lentePreview.gif border=0 alt='Selezione'&gt;" HeaderText="Seleziona"
														CommandName="Select">
														<HeaderStyle Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
													<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" HeaderText="Elimina"
														CommandName="Delete">
														<HeaderStyle Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
												</Columns>
												<PagerStyle HorizontalAlign="Center" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
											</asp:DataGrid></DIV>
									</asp:panel>
									<!-- FINE : PANNELLO LISTA DIAGRAMMI --></td>
							</tr>
							<tr>
								<td align="center" height="5">
									<!-- INIZIO : PANNELLO GESTIONE STATI --><asp:panel id="Panel_GestioneStati" Runat="server">
										<TABLE width="99.5%">
											<TR>
												<TD class="testo_grigio_scuro" width="20%">Nome Diagramma *</TD>
												<TD colSpan="3">
													<asp:TextBox id="txt_descrizione" runat="server" CssClass="testo" Width="320px"></asp:TextBox>
										        </TD>												
											</TR>
											<TR>
												<TD class="testo_grigio_scuro">Stato Corrente *</TD>
												<TD width="20%">
                                                    <asp:DropDownList id="ddl_stati" runat="server" CssClass="testo" Width="320px" AutoPostBack="True"></asp:DropDownList></TD>
												<TD align="center" width="20%"></TD>
												<TD>&nbsp;
													<asp:Button id="btn_addStato" runat="server" CssClass="testo_btn_p" Text="Aggiungi Stato"></asp:Button>&nbsp;
													<asp:Button id="btn_modStato" runat="server" CssClass="testo_btn_p" Text="Modifica Stato"></asp:Button>&nbsp;
													<asp:Button id="btn_delStato" runat="server" CssClass="testo_btn_p" Text="Elimina Stato"></asp:Button></TD>
											</TR>
											<tr>
											    <td colspan="4"><div style="border-bottom-style:solid; border-bottom-color:#810D06; width:100%;"></div></td>
											</tr>
											<TR>
												<TD></TD>
												<TD class="testo_grigio_scuro" align="center"><br/>Stati Disponibili</TD>
												<TD align="center"></TD>
												<TD class="testo_grigio_scuro" align="center"><br/>Stato/i Successivi</TD>
											</TR>
											<TR>
												<TD class="testo_grigio_scuro"></TD>
												<TD class="testo_grigio_scuro">
												    <asp:ListBox id="lbox_stati1"  CssClass="testo" runat="server" Width="320px" SelectionMode="Multiple" Rows="7"></asp:ListBox>
                                                </TD>
												<TD align="center">
													<asp:Button id="btn_moveStato1" runat="server" CssClass="testo_btn_p" Text=">>"></asp:Button><BR>
													<BR>
													<asp:Button id="btn_moveStato2" runat="server" CssClass="testo_btn_p" Text="<<"></asp:Button>
												</TD>
												<TD class="testo_grigio_scuro">
													<asp:ListBox id="lbox_stati2" CssClass="testo" runat="server" Width="320px" SelectionMode="Multiple" Rows="7"></asp:ListBox>
												</TD>
											</TR>
											<TR>
												<TD></TD>
												<TD></TD>
												<TD></TD>
												<TD>
													<asp:Label id="lbl_statoAuto" runat="server" CssClass="testo_grigio_scuro" Width="110px">Stato Automatico</asp:Label>
													<asp:DropDownList id="ddl_statiAutomatici" runat="server" CssClass="testo" Width="205px"></asp:DropDownList>
												</TD>
											</TR>
											<tr>
											    <td colspan="4"><div style="border-bottom-style:solid; border-bottom-color:#810D06; width:100%;"></div></td>
											</tr>
											<TR>
												<TD><br/>
													<asp:Label id="Label1" runat="server" Font-Bold="True" Font-Size="10px" Font-Names="Verdana">Stato Iniziale</asp:Label>
												</TD>
												<TD colSpan="3"><br/>
													<asp:Label class="testo_grigio_scuro" id="lbl_statiIniziali" runat="server" Width="100%"></asp:Label>
												</TD>
											</TR>
											<TR>
												<TD>
													<asp:Label id="Label2" runat="server" Font-Bold="True" Font-Size="10px" Font-Names="Verdana">Stati Finali</asp:Label>
												</TD>
												<TD colSpan="3">
													<asp:Label class="testo_grigio_scuro" id="lbl_statiFinali" runat="server" Width="100%"></asp:Label>
												</TD>
											</TR>
											<tr>
											    <td colspan="4"><div style="border-bottom-style:solid; border-bottom-color:#810D06; width:100%;"></div></td>
											</tr>
											<tr>
											    <td colSpan="4" style="HEIGHT: 40px" vAlign="bottom">
													<asp:Button id="btn_addStep" runat="server" CssClass="testo_btn_p" Text="Aggiungi Passo"></asp:Button>
													<asp:Button id="btn_modPasso" runat="server" CssClass="testo_btn_p" Text="Modifica Passo" Visible="False"></asp:Button>
												</td>
											</tr>
										</TABLE>
									</asp:panel>
									<!-- FINE : PANNELLO GESTIONE STATI --></td>
							</tr>							
							<tr>
								<td align="center" height="5"><BR>
									<!-- INIZIO : PANNELLO LISTA PASSI --><asp:panel id="Panel_ListaPassi" Runat="server">
										<DIV id="DivDGListaPassi" align="center" runat="server">
											<asp:DataGrid id="dg_listaPassi" runat="server" Width="100%" AutoGenerateColumns="False" PageSize="5"
												AllowPaging="True" Height="6px" OnItemCreated="dg_listaPassi_ItemCreated">
												<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
												<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
												<ItemStyle CssClass="bg_grigioN"></ItemStyle>
												<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
												<Columns>
													<asp:BoundColumn DataField="Stato" HeaderText="Stato">
														<HeaderStyle Width="20%"></HeaderStyle>
													</asp:BoundColumn>
													<asp:BoundColumn DataField="Stati Successivi" HeaderText="Stati Successivi">
														<HeaderStyle Width="70%"></HeaderStyle>
													</asp:BoundColumn>
													<asp:ButtonColumn Text="&lt;img src=../images/lentePreview.gif border=0 alt='Selezione'&gt;" HeaderText="Seleziona"
														CommandName="Select">
														<HeaderStyle Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
													<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" HeaderText="Elimina"
														CommandName="Delete">
														<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
												</Columns>
												<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
											</asp:DataGrid></DIV>
										<DIV></DIV>
									</asp:panel>
									<!-- FINE : PANNELLO LISTA PASSI --></td>
							</tr>
							<!-- FINE: PANNELLI -->
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
