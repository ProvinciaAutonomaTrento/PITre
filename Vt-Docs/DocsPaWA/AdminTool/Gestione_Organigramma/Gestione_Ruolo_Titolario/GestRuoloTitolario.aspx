<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestRuoloTitolario.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Organigramma.Gestione_Ruolo_Titolario.GestRuoloTitolario" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="~/UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <LINK href="../../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <base target=_self />
</head>
<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0">
    <form id="frm_GestRuoloTitolario" runat="server" method="post">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Organigramma > Visibilità del ruolo sui nodi di titolario" />
        <asp:HiddenField ID="hd_idCorrGlobRuolo" runat=server />
        <asp:HiddenField ID="hd_idGruppo" runat=server />
        <asp:HiddenField ID="hd_idAmm" runat=server />
        <asp:HiddenField ID="hd_codRuolo" runat=server />
        <!-- tabella generale -->
        <table border=0 cellpadding=0 cellspacing=0 align=center>
            <tr>
                <!-- cella lato SX ... al momento non visibile -->
                <asp:Panel ID="cellaSX" runat=server Visible=false>                
                <td valign="top" width="200">
                     <table width="99%" align="center">
						<tr>
							<td class="pulsanti" height=35>								
								<table cellSpacing="0" cellPadding="3" border="0">
									<tr>													
										<td class="testo_grigio_scuro">Visibilità attuale di:</td>													
									</tr>
									<tr>
									    <td><asp:Label ID="lbl_ruolo" runat=server CssClass="testo_grigio_scuro"></asp:Label></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>													
						    <td>
						        <!-- lista dei nodi di titolario visibili al ruolo -->
						        <DIV id="DivDGList" style="OVERFLOW: auto; WIDTH: 195px; HEIGHT: 560px">
						            <asp:datagrid id="dg_visibAttuale" runat="server" BorderColor="Gray" 
                                        CellPadding="1" BorderWidth="1px"
								        AutoGenerateColumns="False" onitemcommand="dg_visibAttuale_ItemCommand">
								        <SelectedItemStyle HorizontalAlign="Left" CssClass="bg_grigioS"></SelectedItemStyle>
								        <EditItemStyle HorizontalAlign="Left"></EditItemStyle>
								        <AlternatingItemStyle HorizontalAlign="Left" CssClass="bg_grigioA"></AlternatingItemStyle>
								        <ItemStyle HorizontalAlign="Left" CssClass="bg_grigioN"></ItemStyle>
								        <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								        <Columns>
									        <asp:BoundColumn Visible="False" DataField="ID_RECORD" HeaderText="ID_RECORD">
										        <HeaderStyle Width="0px"></HeaderStyle>
									        </asp:BoundColumn>												        
									        <asp:ButtonColumn Text="&lt;img src=../../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;" CommandName="Select">
										        <HeaderStyle Width="5%"></HeaderStyle>
										        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>										       
									        </asp:ButtonColumn>
									        <asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Cod. / Descrizione">
										        <HeaderStyle Width="95%" HorizontalAlign="Left"></HeaderStyle>
									        </asp:BoundColumn>
									        <asp:BoundColumn Visible="False" DataField="CODICE" HeaderText="CODICE">
										        <HeaderStyle Width="0px"></HeaderStyle>
									        </asp:BoundColumn>
									        <asp:BoundColumn Visible="False" DataField="LIVELLO" HeaderText="LIVELLO">
										        <HeaderStyle Width="0px"></HeaderStyle>
									        </asp:BoundColumn>
									        <asp:BoundColumn Visible="False" DataField="CODLIV" HeaderText="CODLIV">
										        <HeaderStyle Width="0px"></HeaderStyle>
									        </asp:BoundColumn>
									        <asp:BoundColumn Visible="False" DataField="ID_PARENT" HeaderText="ID_PARENT">
										        <HeaderStyle Width="0px"></HeaderStyle>
									        </asp:BoundColumn>
									    </Columns>
							        </asp:datagrid>
						        </DIV>
						    </td>
						</tr>
					</table>					
                </td>
                </asp:Panel>
                <!-- cella lato DX -->
                <td valign="top" width="800">
                    <table width="99%" align="center">
                        <tr>
						    <td class="testo_grigio_scuro">Visibilità sul titolario di: <asp:Label ID="lbl_ruolo_new" runat=server CssClass="testo_grigio_scuro"></asp:Label></td>
						</tr>
						<tr>
							<td class="pulsanti" height=35>
								<table cellSpacing="0" cellPadding="0" width="100%" border="0">
									<tr>
										<td align="left" width="35%">
											<table cellSpacing="0" cellPadding="3" border="0">
												<tr>
													<!-- Registro -->
													<td class="testo_blu" >Registri:</td>
													<td class="testo_piccoloB" ><asp:dropdownlist id="ddl_registri" 
                                                            CssClass="testo_blu" Runat="server" AutoPostBack="True" Width="200px" 
                                                            onselectedindexchanged="ddl_registri_SelectedIndexChanged"></asp:dropdownlist></td>
												</tr>
											</table>
										</td>										
										<td align="right" width="65%">
											<table cellSpacing="0" cellPadding="3" border="0">
												<tr>
													<!-- Ricerca nodo -->
													<td class="testo_piccoloB"><asp:label id="lblTipoRicerca" runat="server" CssClass="testo_grigio_scuro">Ricerca per:</asp:label></td>
													<td class="testo_piccoloB">
													    <asp:dropdownlist id="cboTipoRicerca" runat="server" CssClass="testo">								        
													    </asp:dropdownlist>
													</td>
													<td class="testo_piccoloB"><asp:textbox id="txtFieldRicerca" runat="server" CssClass="testo" Width="200"></asp:textbox></td>
													<td class="testo_piccoloB"><asp:button id="btnSearch" CssClass="testo_btn" 
                                                            Runat="server" Text="Cerca" onclick="btnSearch_Click"></asp:button></td>
												</tr>
											</table>
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td>
							    <!-- Risultato della ricerca -->
								<DIV id="DivList" style="OVERFLOW: auto; HEIGHT: 50px">
									<TABLE cellSpacing="3" cellPadding="1" width="96%" border="0">
										<TR bgColor="#eaeaea">
											<TD width="15%" class="testo_grigio_scuro" align="center">Codice</TD>
											<TD width="80%" class="testo_grigio_scuro" align="center">Descrizione</TD>
											<TD width="5%" class="testo_grigio_scuro" align="center">Livello</TD>
										</TR>
										<asp:label id="lbl_td" Runat="server"></asp:label>
									</TABLE>
								</DIV>
							</td>
						</tr>
						<tr>
							<td>
								<!-- TREEVIEW --><iewc:treeview id="trvNodiTitolario" runat="server" 
                                    AutoPostBack="True" DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"
									backcolor="antiquewhite" borderwidth="1px" borderstyle="solid" bordercolor="maroon" font="verdana" width="100%"
									height="450px" onexpand="trvNodiTitolario_Expand1" 
                                    onselectedindexchange="trvNodiTitolario_SelectedIndexChange"></iewc:treeview></td>
						</tr>
						<tr>
						    <td>
						        <table border=0 cellpadding=1 cellspacing=1 width=100%>
						            <tr>
						                <td width=85% align="left">	
						                    <table border=0 cellspacing=0 cellpadding=0 width=100%>
						                        <tr>
						                            <td class="testo_piccolo" align=center><asp:Label ID="lbl_agg_nodo" runat="server" Text="Aggiungi visibilità al nodo selezionato"></asp:Label></td>
						                            <td class="testo_piccolo" align=center><asp:Label ID="lbl_agg_nodo_figli" runat="server" Text="Aggiungi visibilità al nodo e a tutti i sotto-nodi"></asp:Label></td>
						                            <td class="testo_piccolo" align=center><asp:Label ID="lbl_elimina" runat="server" Text="Elimina visibilità"></asp:Label></td>
						                        </tr>
						                        <tr>
						                            <td align=center>
						                                <asp:Button ID="btn_add" runat=server CssClass="testo_btn" 
                                                            Text="Aggiungi" onclick="btn_add_Click" Enabled=false />
						                            </td>
						                            <td align=center>
						                                <asp:Button ID="btn_add_figli" runat=server CssClass="testo_btn" 
                                                            Text="Aggiungi" onclick="btn_add_figli_Click" Enabled=false  />
						                            </td>
						                            <td align=center>
						                                <asp:Button ID="btn_del" runat=server CssClass="testo_btn" 
                                                            Text="Elimina" onclick="btn_del_Click" Enabled=false />
						                            </td>
						                        </tr>
						                    </table>						                    				                    						                    						                    
						                </td>
						                <td width=15% align="right" class="testo_piccoloB" valign="bottom">
						                    <asp:Button ID="btn_chiudi" runat=server CssClass="testo_btn" Text="Chiudi" 
                                                onclick="btn_chiudi_Click" />
						                </td>
						            </tr>
						        </table>
						    </td>
						</tr>
					</table>					
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
