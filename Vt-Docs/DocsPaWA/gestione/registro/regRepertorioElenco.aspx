<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="regRepertorioElenco.aspx.cs" Inherits="DocsPAWA.gestione.registro.regRepertorioElenco" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet" >
		<script language="javascript" id="btn_stampaRegistro_click" for="btn_stampaRegistro" event="onclick()">
				window.document.body.style.cursor='wait';
			
			WndWaitStampaReg();
		</script>
		<script language="javascript" id="btn_cambiaStatoReg_click" for="btn_cambiaStatoReg" event="onclick()">
			window.document.body.style.cursor='wait';
		</script>
	    <style type="text/css">
            .style1
            {
                text-align: justify;
                margin-left: 5px;
                margin-right: 5px;
                border-width: 1px;
                border-color: Black;
                border-style: solid;
                padding-left: 5px;
                padding-right: 5px;
            }
        </style>
	</HEAD>
	<BODY leftMargin="1" MS_POSITIONING="GridLayout">
		<FORM id="regElenco" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione registri repertorio" />
			<TABLE id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="408" align="center"
				border="0">
				
				<tr valign="top" height="70%">
					<td valign=top>
						<TABLE width="100%" height=100% class="info" cellSpacing="1" cellPadding="1" align="center" border="0">
							<TR height="20px">
								<TD class="item_editbox"><P class="boxform_item">Gestione registri repertorio</P>
								</TD>
							</TR>
							<tr>
								<td height="5" valign=top></td>
							</tr>
							
							<TR>
								<TD align="center" width="412" valign=Top>
									<DIV id="DivDGList" style="OVERFLOW: auto; WIDTH: 98%; HEIGHT: 200px">
									<asp:datagrid id="DataGrid2" SkinID="datagrid" runat="server" Width="98%" 
                                            BorderColor="Gray" CellPadding="1" AllowPaging="False"
										BorderWidth="1px" AutoGenerateColumns="False" OnSelectedIndexChanged="DataGrid2_SelectedIndexChanged">
										<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#23415F" Font-Size="11px" />
                                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>


										<Columns>
										<asp:TemplateColumn HeaderText="Id contatore" Visible="false">
												<HeaderStyle></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblContatore runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ContatoreID") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="REGISTRO">
												<HeaderStyle Width="40%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblDescrTipologia runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DescrizioneTipologia") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
										<asp:TemplateColumn HeaderText="Tipo" Visible="false">
												<HeaderStyle></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblTipo runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipologia") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Id registro" Visible="false">
												<HeaderStyle></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblIdReg runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdReg") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Id rf" Visible="false">
												<HeaderStyle></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblIdRf runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdRf") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>

                                            <asp:TemplateColumn HeaderText="AOO/RF">
												<HeaderStyle Width="40%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblDescrizione runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DescrRegOrRF") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Stato">
												<HeaderStyle Width="15%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblStato runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
                                             <asp:TemplateColumn HeaderText="Data Ultima Stampa" Visible="false">
												<HeaderStyle Width="15%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblDtaLastPrint runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DateLastPrint") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Responsabile" Visible="false">
												<HeaderStyle></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=lblResponsabile runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Responsabile") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
										    <asp:TemplateColumn>
												    <HeaderStyle Width="5%"></HeaderStyle>
												    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												    <ItemTemplate>
													    <asp:ImageButton id="ImageButton1" runat="server" Width="18px" BorderWidth="1px" ImageUrl="../../images/proto/dettaglio.gif"
														    AlternateText="Dettaglio" BorderStyle="None" Height="16px" CommandName="Select"></asp:ImageButton>
												    </ItemTemplate>
									       </asp:TemplateColumn>
										</Columns>
											<PagerStyle Width="350px" VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#c2c2c2"
											CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
								   </asp:datagrid>
								   </DIV>
								</TD>
							</TR>
                            <tr>
                                <td align="center" width="412" valign="top">
                                    <asp:Panel ID="pnlAlert" runat="server" CssClass="style1">
                                    <span class="FS-NBK">
                                        <span class="testo_red">Attenzione!</span> Avviando la stampa, verranno creati:
                                        <asp:BulletedList ID="blDocList" runat="server" CssClass="FS-NBK">
                                    </asp:BulletedList>
                                    </span>
                                    </asp:Panel>
                                    
                                </td>
                            </tr>
						</TABLE>
					</td>
				</tr>
				<tr height="7%">
					<td align="center">
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" border="0" align="center">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
                                <td style="padding-right: 3px;">
                                    <asp:Button id="btn_stampaRepertori" Text="Stampa" runat="server" 
                                        onclick="btn_stampaRepertori_Click1" />
                                </td>
                                <td style="padding-left: 3px;">
                                    <asp:Button id="btn_cambiaStato" Text="Cambia stato" runat="server" 
                                        onclick="btn_cambiaStato_Click1" />
                                </td>
                            </TR>
						</TABLE> <!--FINE BOTTONIERA -->
					</td>
				</tr>
			</TABLE>
		</FORM>
	</BODY>
</HTML>
