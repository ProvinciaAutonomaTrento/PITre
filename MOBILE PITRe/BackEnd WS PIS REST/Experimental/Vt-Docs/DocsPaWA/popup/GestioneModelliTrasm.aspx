<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="GestioneModelliTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.GestioneModelliTrasm" %>
<%@ Register TagPrefix="cc2" Src="~/UserControls/PannelloRicercaModelliTrasmissione.ascx" TagName="PannelloRicercaModelliTrasmissione" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/rubrica.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		function reloadPage()
		{
		    document.forms["Form1"].submit();
		}
		
		function ApriRubricaTrasm (ragione, tipo_oggetto)
		{
			
				var tipoDest = "<%=gerarchia_trasm%>";
				
				var r = new Rubrica();
				r.CorrType = r.Interni;
				switch (tipoDest) {
					case 'T':
						r.CallType = r.CALLTYPE_MODELLI_TRASM_ALL;
						break;
						
					case 'I':
						r.CallType = r.CALLTYPE_MODELLI_TRASM_INF;
						break;
						
					case 'S':
						r.CallType = r.CALLTYPE_MODELLI_TRASM_SUP;
						break;		
						
					case 'P':
						r.CallType = r.CALLTYPE_MODELLI_TRASM_PARILIVELLO;
						break;													
				}
				if (tipo_oggetto != null)
					r.MoreParams = "objtype=" + tipo_oggetto;
					
				var res = r.Apri(); 
		}		
		
		function confirmDel()
		{
			var agree=confirm("Confermi la cancellazione ?");
			if (agree)
			{
				document.getElementById("txt_confirmDel").value = "si";
				return true ;
			}			
		}

			function OpenHelp(from) 
			{		
				var pageHeight= 600;
				var pageWidth = 800;
				//alert(from);
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				var newwin = window.showModalDialog('../Help/Manuale.aspx?from=' + from,
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
								
				//newwin.location.hash=from;
				//return false;
			}				    
		    
		    function apriModaleNotifiche(mode)
		    {
		        var args = new Object;
			    args.window = window;
    			
		        rtnValue = window.showModalDialog('GestioneModelliTrasm_Notifiche.aspx?mode='+mode,args,'dialogWidth:600px;dialogHeight:550px;status:no;resizable:yes;scroll:yes;center:yes;help:no;');
		        Form1.hd_returnValueModal.value = rtnValue;
                window.document.Form1.submit();
            }

            function ApriRubricaAoo() {

                var r = new Rubrica();
                r.CallType = r.CALLTYPE_DEST_MODELLO_TRASM;
                //r.CallType = r.CALLTYPE_PROTO_OUT_MITT;
                var res = r.Apri();
            }

            function _ApriRubrica() {
                var r = new Rubrica();
                r.CallType = r.CALLTYPE_MITT_MODELLO_TRASM;
                var res = r.Apri();

            }  		
		</script>
		<script language="javascript" id="btn_ricerca_click" event="onclick()" for="btn_ricerca">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
		    <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">		    		    
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione modelli trasmissione" />
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TBODY>
					<tr>
						<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
							<table cellSpacing="0" cellPadding="0" width="90%" align="center" border="0">
								<tr>
									<td align="center" colSpan="2" height="25"><asp:label id="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
								</tr>
								<tr>
									<td>
										<table cellSpacing="0" cellPadding="0" width="100%">
											<!-- Nuovo modello -->
                                            <tr>
												<td class="pulsanti" align="left" height="20"><asp:label id="lbl_titolo" runat="server" CssClass="testo_grigio_scuro">Lista Modelli</asp:label></td>
												<td class="pulsanti" align="right" height="20" valign="bottom">
                                                    <cc1:messagebox id="msg_ConfirmDel" runat="server"></cc1:messagebox>&nbsp;
													<asp:button id="btn_lista_modelli" runat="server" CssClass="PULSANTE" Visible="False" Text="Lista Modelli" Width="80"></asp:button>&nbsp;
													<asp:button id="btn_salvaModello" runat="server" CssClass="PULSANTE" Visible="False" Text="Salva" Width="80"></asp:button>&nbsp;
													<asp:button id="btn_nuovoModello" runat="server" CssClass="PULSANTE" Text="Nuovo" Width="80"></asp:button>&nbsp;
													<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="Chiudi" Width="80"></asp:button>&nbsp;
													<asp:ImageButton ID="help" runat="server" AlternateText="Aiuto?" SkinID="btnHelp" OnClientClick="OpenHelp('GestioneModelliTrasm')" />
												</td>
											</tr>
											<!-- Ricerca modello -->
											<tr>
                                                <td colspan="2">
                                                    <table width="100%">
                                                        <tr>
                                                            <td colspan="4">
                                                                <cc2:PannelloRicercaModelliTrasmissione ID="prmtRicerca" runat="server" ButtonCss="PULSANTE" Search="btn_ricerca_Click" SearchContext="ModelliTrasmissioneUtente" UserType="User" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                           
											<!-- INIZIO: PANNELLI -->
											<tr>
												<td align="center" colSpan="2" height="5">&nbsp;
												</td>
											</tr>
											<tr>
												<td align="center" colSpan="2" height="5">
													<!-- INIZIO : PANNELLO LISTA MODELLI --><asp:panel id="Panel_ListaModelli" runat="server">
														<DIV id="DivDGListaTemplates" align="center" runat="server">
															<asp:datagrid id="dt_listaModelli" SkinID="datagrid" runat="server" AutoGenerateColumns="False" CellPadding="1"
																Width="100%" BorderWidth="1px" BorderColor="Gray" AllowPaging="True" OnItemCreated="Grid_OnItemCreated">
																<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
																<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
																<ItemStyle CssClass="bg_grigioN"></ItemStyle>
																<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
																<Columns>
																	<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SystemId"></asp:BoundColumn>
																	<asp:BoundColumn DataField="CODICE" HeaderText="Codice">
																		<HeaderStyle Width="8%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:BoundColumn DataField="MODELLO" HeaderText="Modello">
																		<HeaderStyle Width="24%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:BoundColumn DataField="REGISTRO" HeaderText="REGISTRO">
																		<HeaderStyle Width="24%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:BoundColumn DataField="TIPO DI TRASM." HeaderText="TIPO DI TRASM.">
																		<HeaderStyle Width="18%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:BoundColumn DataField="VISIBILITA'" HeaderText="VISIBILITA'">
																		<HeaderStyle Width="20%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:ButtonColumn Text="&lt;img src=../images/proto/lentePreview.gif border=0 alt='Selezione'&gt;"
																		HeaderText="Seleziona" CommandName="Select">
																		<HeaderStyle Width="3%"></HeaderStyle>
																		<ItemStyle HorizontalAlign="Center"></ItemStyle>
																	</asp:ButtonColumn>
																	<asp:ButtonColumn Text="&lt;img src=../images/proto/cancella.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;"
																		HeaderText="Elimina" CommandName="Delete">
																		<HeaderStyle Width="3%"></HeaderStyle>
																		<ItemStyle HorizontalAlign="Center"></ItemStyle>
																	</asp:ButtonColumn>
																</Columns>
																<PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
															</asp:datagrid></DIV>
													</asp:panel>
													<!-- FINE : PANNELLO LISTA MODELLI -->
													<!-- INIZIO : PANNELLO NUOVO TEMPLATE -->
													<asp:panel id="Panel_NuovoModello" runat="server" Visible="False" BorderWidth="1px" BorderColor="#810D06"
														BorderStyle="Solid">
														<TABLE width="99%" border="0">
															<TBODY>
																<TR>
																	<TD class="testo_grigio_scuro" align="center" colSpan="4" height="25"><asp:Label ID="lbl_stato" runat=server CssClass="testo_grigio_scuro" /> Modello di Trasmissione</TD>
																</TR>
																<TR>
																	<TD class="testo_grigio_scuro" style="HEIGHT: 23px" align="left" width="120">Nome *</TD>
																	<TD style="WIDTH: 265px; HEIGHT: 23px" align="left" width="265">
																		<asp:TextBox id="txt_nomeModello" runat="server" CssClass="testo_grigio" Width="300px"></asp:TextBox></TD>
																	<TD class="testo_grigio_scuro" style="HEIGHT: 23px" align="left" width="6%" rowSpan="1">Note
																	</TD>
																	<TD align="left" width="300" rowSpan="2">
																		<asp:TextBox id="txt_noteGenerali" runat="server" CssClass="testo_grigio" Width="300px" Rows="3"
																			TextMode="MultiLine"></asp:TextBox></TD>
																</TR>
																<TR>
																	<TD style="HEIGHT: 23px" align="left" width="120"><asp:Label ID="lbl_codice" runat="server" Visible="true" Text="Codice" CssClass="testo_grigio_scuro"></asp:Label></TD>
																	<TD style="WIDTH: 265px; HEIGHT: 23px" align="left" width="265">
																		<asp:TextBox id="txt_codModello" runat="server" CssClass="testo_grigio" Width="80px"></asp:TextBox></TD>
																</TR>
																<TR>
																	<TD class="testo_grigio_scuro" align="left">Tipo Trasmissione *</TD>
																	<TD align="left" colspan="2" class="testo_grigio_scuro">
																		<asp:DropDownList id="ddl_tipoTrasmissione" runat="server" CssClass="testo_grigio" AutoPostBack="true" OnSelectedIndexChanged="ddl_tipoTrasmissione_OnSelectedIndexChanged">
																			<asp:ListItem Value="D">Documento</asp:ListItem>
																			<asp:ListItem Value="F">Fascicolo</asp:ListItem>
																		</asp:DropDownList>
																		&nbsp;
                                                                        <asp:Label ID="lbl_registro_obb" runat="server" CssClass="testo_grigio_scuro" Visible="true" Text="Registro *"></asp:Label>
                                                                        &nbsp;
																		<asp:dropdownlist id="ddl_registri" CssClass="testo_grigio" AutoPostBack="True" Runat="server"></asp:dropdownlist>
																		<asp:Label ID="lbl_registri" runat="server" CssClass="testo_grigio" Visible="false"></asp:Label></TD>
												
											                    </tr>
											                    <TR>
												                    <TD class="testo_grigio_scuro" align="left">Rendi disponibile</TD>
												                    <TD colSpan="3">
													                    <asp:radiobuttonlist id="rbl_share" tabIndex="3" runat="server" CssClass="testo_grigio" Width="250px"
														                    BackColor="Transparent" RepeatDirection="Horizontal">
														                    <asp:ListItem Value="usr" Selected="True">solo a me stesso</asp:ListItem>
														                    <asp:ListItem Value="grp">a tutto il ruolo</asp:ListItem>
													                    </asp:radiobuttonlist></TD>
											                    </TR>
										                    </table>
										</asp:panel><BR>
										<asp:panel id="Panel_dest" runat="server" Visible="False" BorderWidth="1px" BorderStyle="None">
											<TABLE cellSpacing="1" cellPadding="0" width="100%">
												<TR>
													<TD class="testo_grigio_scuro" align="left">Seleziona la Ragione Trasmissione<asp:label ID="lbl_ragione" runat="server" Text=" *"></asp:label>&nbsp;&nbsp;
														<asp:dropdownlist id="ddl_ragioni" CssClass="testo_grigio" Runat="server" AutoPostBack="True"></asp:dropdownlist></TD>
													<TD class="testo_grigio_scuro" style="HEIGHT: 14px" align="left">Seleziona i 
														Destinatari *&nbsp;&nbsp;<asp:textbox id="txt_codDest" CssClass="testo_grigio" Width="95px" Runat="server"></asp:textbox>
														&nbsp;<asp:imagebutton id="ibtnMoveToA" runat="server" AlternateText="Selezione per codice inserito" ImageUrl="../images/rubrica/b_arrow_right.gif"></asp:imagebutton>
														&nbsp;&nbsp;&nbsp;<asp:ImageButton id="btn_Rubrica_dest" runat="server" Visible="True" ImageUrl="../images/proto/rubrica.gif"
															AlternateText="Seleziona i destinatari dalla rubrica"></asp:ImageButton></TD>
												</TR>
												<TR>
													<TD colSpan="2">
														<DIV id="Div1" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 210px" align="center" runat="server">
															<asp:DataGrid id="dt_dest" SkinID="datagrid" runat="server" Visible="False" 
                                                    AutoGenerateColumns="False" CellPadding="1"
																Width="100%" BorderColor="Gray" PageSize="1" onitemcreated="dt_dest_ItemCreated">
																<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
																<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
																<ItemStyle CssClass="bg_grigioN"></ItemStyle>
																<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
																<Columns>
																	<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SystemIdMittDest"></asp:BoundColumn>
																	<asp:BoundColumn DataField="RAGIONE" HeaderText="Ragione">
																		<HeaderStyle Width="10%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:TemplateColumn>
																		<HeaderStyle Width="2%"></HeaderStyle>
																		<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
																		<ItemTemplate>
																			<asp:ImageButton id="Img_tipo_urp" runat="server"></asp:ImageButton>
																		</ItemTemplate>
																	</asp:TemplateColumn>
																	<asp:BoundColumn DataField="VAR_COD_RUBRICA" HeaderText="Codice">
																		<HeaderStyle Width="10%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:BoundColumn DataField="VAR_DESC_CORR" HeaderText="Descrizione">
																		<HeaderStyle Width="35%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:TemplateColumn HeaderText="Tipo">
																		<HeaderStyle Width="60px"></HeaderStyle>
																		<ItemTemplate>
																			<asp:DropDownList id="DropDownList1" runat="server" CssClass="testo_grigio">
																				<asp:ListItem Value="S">Uno</asp:ListItem>
																				<asp:ListItem Value="T">Tutti</asp:ListItem>
																			</asp:DropDownList>
																		</ItemTemplate>
																	</asp:TemplateColumn>
																	<asp:TemplateColumn HeaderText="Note">
																		<HeaderStyle Width="30%"></HeaderStyle>
																		<ItemTemplate>
																			<asp:TextBox id="TextBox1" runat="server" CssClass="testo_grigio" Width="100%"></asp:TextBox>
																		</ItemTemplate>
																	</asp:TemplateColumn>
                                                                    <asp:TemplateColumn HeaderText="SCADENZA">
                                                                        <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                            Font-Underline="False" HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="gg" Width="20px"></asp:Label>
                                                                            <asp:TextBox ID="txt_scadenza" runat="server" CssClass="testo_grigio" MaxLength="3"
                                                                                Width="30px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                            Font-Underline="False" HorizontalAlign="Center" Width="70px" />
                                                                    </asp:TemplateColumn>
                                                                    <asp:TemplateColumn HeaderText="Nasc. vers." HeaderStyle-HorizontalAlign="Center">
                                                                        <ItemStyle Width="5px" HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkNascondiVersioniPrecedentiDocumento" runat="server" 
                                                                            ToolTip="Ai destinatari della trasmissione saranno nascoste le versioni precedenti a quella corrente dei documenti consolidati" CssClass="testo" 
                                                                            Checked='<%#DataBinder.Eval(Container.DataItem, "NASCONDI_VERSIONI_PRECEDENTI")%>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateColumn>
																	<asp:EditCommandColumn HeaderText="Elimina" CancelText="Cancel" EditText="&lt;img src=../images/proto/cancella.gif border=0  onclick=confirmDel(); alt='Elimina'&gt;">
																		<HeaderStyle HorizontalAlign="Center" Width="3%"></HeaderStyle>
																		<ItemStyle HorizontalAlign="Center"></ItemStyle>
																	</asp:EditCommandColumn>
																	<asp:BoundColumn Visible="False" DataField="ID_RAGIONE" HeaderText="id_ragione"></asp:BoundColumn>
																</Columns>
																<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
															</asp:DataGrid></DIV>
													</TD>
												</TR>
											</TABLE>
											<table width=100% cellpadding=0 cellspacing=0>
											    <tr><td align=right><asp:Button ID="btn_pp_notifica" 
                                                        runat="server" CssClass="PULSANTE" Visible="False" Text="" 
                                                        onclick="btn_pp_notifica_Click" /></td></tr>
											</table>
										</asp:panel><BR>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					</table></td></tr>
				</TBODY>
			</table>
			<INPUT id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server"/>                                    
           
            
		</form>
	</body>
</HTML>
