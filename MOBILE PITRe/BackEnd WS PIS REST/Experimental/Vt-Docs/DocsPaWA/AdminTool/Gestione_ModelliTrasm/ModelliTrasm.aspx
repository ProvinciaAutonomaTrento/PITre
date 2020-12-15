<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Page language="c#" Codebehind="ModelliTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.AdminTool.Gestione_ModelliTrasm.ModelliTrasm" EnableEventValidation="false" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Src="~/UserControls/PannelloRicercaModelliTrasmissione.ascx" TagName="PannelloRicercaModelliTrasmissione" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet"></link>
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
		function reloadPage()
		{
		    document.forms["Form1"].submit();
		}

		function apriPopup() 
		{
				window.open('../help.aspx?from=MT','','width=450,height=500,scrollbars=YES');
		}	
		
		function _ApriRubrica()
		{
			var r = new Rubrica();
			r.CallType = r.CALLTYPE_MITT_MODELLO_TRASM;			
			var res = r.Apri(); 
			
		}
		
		function ApriRubricaAoo()
		{
			
			var r = new Rubrica();
			r.CallType = r.CALLTYPE_DEST_MODELLO_TRASM;			
			//r.CallType = r.CALLTYPE_PROTO_OUT_MITT;
			var res = r.Apri(); 
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
		
		function apriModaleNotifiche(mode)
		{
		    var args = new Object;
			args.window = window;
			
		    rtnValue = window.showModalDialog('../../popup/GestioneModelliTrasm_Notifiche.aspx?mode='+mode,args,'dialogWidth:600px;dialogHeight:550px;status:no;resizable:yes;scroll:yes;center:yes;help:no;');
		    Form1.hd_returnValueModal.value = rtnValue;
            window.document.Form1.submit();
		}	
		</script>
		<script language="javascript" id="btn_find_click" event="onclick()" for="btn_find">
			
			var pattern =/^[Mm]{1}[Tt1}\_[0-9]+$/;

            var ddl = document.getElementById("ddl_ricerca"); 
            var searchParam = ddl.options[ddl.selectedIndex].value; 

			if(searchParam == "Codice" && document.getElementById("txt_search").value!="" && document.getElementById("txt_search")!=null)
		        {
		            var codice=document.getElementById("txt_search").value;
		            if(!pattern.test(codice))
		            {
		                alert('attenzione: il formato del codice deve essere mt_<numero modello>');
		                return false;
		            }
		        }
		        window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0">
		<form id="Form1" method="post" runat="server">
		    <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Modelli di trasmissione" />
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<TBODY>
					<tr>
						<td>
							<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
					</tr>
					<tr>
						<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
					</tr>
					<tr>
						<!-- TITOLO PAGINA -->
						<td class="titolo" style="HEIGHT: 20px" align="center" bgColor="#e0e0e0" height="34">Modelli Trasmissione</td>
					</tr>
                    <!-- Autenticazione Sistemi Esterni: ritorno alla configurazione per i sistemi esterni -->
                    <tr id="tr_backToExtSys" runat="server">
                        <td valign="top" align="left" bgcolor="#f6f4f4" height="20">
                            <asp:Button ID="btn_toExtSys" runat="server" CssClass="testo_btn" UseSubmitBehavior="false"
                                Text="Ritorna a Sistemi Esterni" OnClientClick="location.href='../Gestione_SistemiEsterni/SistemiEsterni.aspx'; return false;">
                            </asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                            <!-- CORPO CENTRALE -->
                            <table cellspacing="0" cellpadding="0" align="center" border="0" width="90%">
                                <tr>
                                    <td align="center" height="25">
                                        <asp:Label ID="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
								</tr>
								<tr>
									<td class="pulsanti" align="center">
										<table width="99%">
											<!-- Nuovo modello -->
                                            <tr>
                                                <td align="right" colspan="4">
												    <cc1:messagebox id="msg_ConfirmCambioMitt" runat="server"></cc1:messagebox>
												    <cc1:messagebox id="msg_ConfirmDel" runat="server"></cc1:messagebox>
                                                    <asp:button id="btn_lista_modelli" runat="server" CssClass="testo_btn_p" Text="Lista Modelli" Visible="False"></asp:button>&nbsp;
													<asp:button id="btn_salvaModello" runat="server" CssClass="testo_btn_p" Text="Salva" Visible="False"></asp:button>&nbsp;
													<asp:button id="btn_nuovoModello" runat="server" CssClass="testo_btn_p" Text="Nuovo"></asp:button>
											    </td>
											</tr>
											<!-- Ricerca modello -->
                                            <tr>
                                                <td colspan="4" style="width:99%;">
                                                    <cc2:PannelloRicercaModelliTrasmissione ID="prmtPannelloRicerca" runat="server" ButtonCss="testo_btn_p" UserType="Administrator" SearchContext="ModelliTrasmissione" />
                                                </td>
                                            </tr>							
										</table>
									</td>
								</tr>
								<!-- INIZIO: PANNELLI -->
								<tr>
									<td align="center" height="5"><br>
										<!-- INIZIO : PANNELLO LISTA MODELLI --><asp:panel id="Panel_ListaModelli" runat="server">
											<DIV id="DivDGListaTemplates" align="center" runat="server">
												<asp:datagrid id="dt_listaModelli" runat="server" AllowCustomPaging="True" AllowPaging="True"
													PageSize="15" AutoGenerateColumns="False" CellPadding="1" Width="100%" BorderWidth="1px" BorderColor="Gray" OnItemCreated="dt_listaModelli_ItemCreated">
													<SelectedItemStyle BackColor="RosyBrown"></SelectedItemStyle>
													<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
													<ItemStyle CssClass="bg_grigioN"></ItemStyle>
													<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
													<Columns>
														<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SystemId"></asp:BoundColumn>
														<asp:BoundColumn DataField="CODICE" HeaderText="Codice">
															<HeaderStyle Width="6%"></HeaderStyle>
														</asp:BoundColumn>
														<asp:BoundColumn DataField="MODELLO" HeaderText="Modello">
															<HeaderStyle Width="22%"></HeaderStyle>
														</asp:BoundColumn>
														<asp:BoundColumn DataField="REGISTRO" HeaderText="REGISTRO">
															<HeaderStyle Width="22%"></HeaderStyle>
														</asp:BoundColumn>
														<asp:BoundColumn DataField="TIPO DI TRASM." HeaderText="TIPO DI TRASM.">
															<HeaderStyle Width="14%"></HeaderStyle>
														</asp:BoundColumn> 
														<asp:BoundColumn DataField="VISIBILITA'" HeaderText="VISIBILITA'">
															<HeaderStyle Width="21%"></HeaderStyle>
														</asp:BoundColumn>
                                                        <asp:BoundColumn DataField="COD. REG." HeaderText="COD. REG.">
															<HeaderStyle Width="9%"></HeaderStyle>
														</asp:BoundColumn>
														<asp:ButtonColumn Text="&lt;img src=../images/lentePreview.gif border=0 alt='Selezione'&gt;" HeaderText="Seleziona"
															CommandName="Select">
															<HeaderStyle Width="3%"></HeaderStyle>
															<ItemStyle HorizontalAlign="Center"></ItemStyle>
														</asp:ButtonColumn>
														<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;"
															HeaderText="Elimina" CommandName="Delete">
															<HeaderStyle Width="3%"></HeaderStyle>
															<ItemStyle HorizontalAlign="Center"></ItemStyle>
														</asp:ButtonColumn>
													</Columns>
													<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
												</asp:datagrid></DIV>
										</asp:panel>
										<!-- FINE : PANNELLO LISTA MODELLI -->
										<!-- INIZIO : PANNELLO NUOVO TEMPLATE --><asp:panel id="Panel_NuovoModello" runat="server" Visible="False" BorderWidth="1px" BorderColor="#810D06"
											BorderStyle="Solid">
											<TABLE width="99.5%" border="0">
												<TR>
													<TD class="titolo_pnl" colSpan="5"><asp:Label ID="lbl_stato" runat=server /> Modello di Trasmissione</TD>
												</TR>
												<TR>
													<TD class="testo_grigio_scuro" style="HEIGHT: 23px" align="left" width="130">Nome *</TD>
													<TD style="HEIGHT: 23px" align="left" width="295">
														<asp:TextBox id="txt_nomeModello" runat="server" CssClass="testo" Width="320px"></asp:TextBox></TD>
													<TD class="testo_grigio_scuro" style="HEIGHT: 23px" align="center" rowSpan="1">Note</TD>
													<TD align="left" width="300" rowSpan="2">
														<asp:TextBox id="txt_noteGenerali" runat="server" CssClass="testo" Width="350px" TextMode="MultiLine"
															Rows="3"></asp:TextBox></TD>
												</TR>
												<TR>
													<TD style="HEIGHT: 23px" align="left" width="130"><asp:Label ID="lbl_codice" runat="server" Visible="true" CssClass="testo_grigio_scuro" Text="Codice"></asp:Label></TD>
													<TD style="HEIGHT: 23px" align="left" width="295" colSpan="3">
														<asp:TextBox id="txt_codModello" runat="server" CssClass="testo" Width="80px"></asp:TextBox></TD>
												</TR>
												<TR>
													<TD class="testo_grigio_scuro" align="left">Tipo Trasmissione *</TD>
													<TD align="left">
														<asp:DropDownList id="ddl_tipoTrasmissione" runat="server" CssClass="testo" AutoPostBack="true" OnSelectedIndexChanged="ddl_tipoTrasmissione_OnSelectedIndexChanged">
															<asp:ListItem Value="D">Documento</asp:ListItem>
															<asp:ListItem Value="F">Fascicolo</asp:ListItem>
														</asp:DropDownList>
                                                    </TD>
													<TD class="testo_grigio_scuro" align="right" width="120">Registro *</TD>
													<TD style="HEIGHT: 20px" align="left">
														<asp:dropdownlist id="ddl_registri" CssClass="testo" Runat="server" AutoPostBack="True"></asp:dropdownlist></TD>
													<TD class="testo_grigio_scuro" align="left" width="120"></TD>
												</TR>
											</TABLE>
										</asp:panel><BR>
										<asp:panel id="Panel_mitt" runat="server" Visible="False" BorderWidth="1px" BorderColor="#810D06"
											BorderStyle="Solid">
											<TABLE cellSpacing="1" cellPadding="0" width="99.5%">
												<TR>
													<TD class="titolo_pnl" colSpan="2">Visibilità</TD>
												</TR>
												<TR>
													<TD class="testo" align="left" width="40%">
														<asp:RadioButtonList id="rb_tipo_mittente" runat="server" CssClass="testo" CellPadding="0" Width="392px"
															AutoPostBack="True" RepeatDirection="Horizontal" CellSpacing="0">
															<asp:ListItem Value="AOO">Tutta la AOO</asp:ListItem>
															<asp:ListItem Value="R" Selected="True">Seleziona un ruolo da rubrica</asp:ListItem>
														</asp:RadioButtonList></TD>
													<TD>
														<asp:ImageButton id="btn_Rubrica_mitt" runat="server" Visible="true" ImageUrl="../../images/proto/rubrica.gif"></asp:ImageButton></TD>
												<TR>
													<TD colSpan="2">
														<asp:DataGrid id="dt_mitt" runat="server" Visible="False" PageSize="1" AutoGenerateColumns="False"
															CellPadding="1" Width="100%" BorderColor="Gray" OnItemCreated="dt_mitt_ItemCreated">
															<SelectedItemStyle BackColor="RosyBrown"></SelectedItemStyle>
															<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
															<ItemStyle CssClass="bg_grigioN"></ItemStyle>
															<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
															<Columns>
																<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SystemMittDest"></asp:BoundColumn>
																<asp:BoundColumn DataField="VAR_DESC_CORR" HeaderText="Descrizione">
																	<HeaderStyle Width="90%"></HeaderStyle>
																</asp:BoundColumn>
																<asp:BoundColumn DataField="VAR_COD_RUBRICA" HeaderText="Codice" Visible="false">
																		<HeaderStyle Width="120px"></HeaderStyle>
																</asp:BoundColumn>
																<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="" HeaderText="Elimina" CancelText="" EditText="&lt;img src=../images/cestino.gif border=0 alt='Elimina'&gt; ">
																	<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
																	<ItemStyle HorizontalAlign="Center"></ItemStyle>
																</asp:EditCommandColumn>
															</Columns>
															<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
														</asp:DataGrid></TD>
												</TR>
											</TABLE>
										</asp:panel><BR>
										<asp:panel id="Panel_dest" runat="server" Visible="False" BorderWidth="1px" BorderStyle="None">
											<TABLE cellSpacing="1" cellPadding="0" width="99.5%">
												<TR>
													<TD class="testo_grigio_scuro" align="left" colspan="2">Seleziona la Ragione Trasmissione<asp:label ID="lbl_ragione" runat="server" Text=" *"></asp:label> 
														&nbsp;&nbsp;
														<asp:dropdownlist id="ddl_ragioni" CssClass="testo" Runat="server" AutoPostBack="True">
														</asp:dropdownlist>
														&nbsp;&nbsp;
                                                        Disattiva le notifiche nella lista delle cose da fare<asp:CheckBox ID="ckb_notify" runat="server" Checked ="false"/>																			
														</TD>
												</TR>
												<tr>
												
												    <td class="testo_grigio_scuro" style="HEIGHT: 14px; width:330px" align="left">Seleziona i 
														Destinatari *&nbsp;&nbsp;<asp:textbox id="txt_codDest" CssClass="testo" Width="95px" Runat="server"></asp:textbox>
														&nbsp;<asp:imagebutton id="ibtnMoveToA" runat="server" AlternateText="Selezione per codice inserito" ImageUrl="../../images/rubrica/b_arrow_right.gif"></asp:imagebutton>
														&nbsp;&nbsp;<asp:ImageButton id="btn_Rubrica_dest" runat="server" Visible="True" ImageUrl="../../images/proto/rubrica.gif"
															AlternateText="Seleziona i destinatari dalla rubrica"></asp:ImageButton>
											        </td>
											        <td>
												        <asp:Button ID="btn_ruolo_segretario" runat="server" CssClass="testo_btn_p" Text="Segretario UO proprietario" Width="164px"/>&nbsp;
												        <asp:Button ID="btn_resp_uo_mitt" runat="server" CssClass="testo_btn_p" Text="Responsabile UO mittente" Width="164px"/>&nbsp;
												        <asp:Button ID="btn_segr_uo_mitt" runat="server" CssClass="testo_btn_p" Text="Segretario UO mittente" Width="155px" />												    												    												
												    </td>
												</tr>												
												<tr>
												<td></td>
												<td>
												<asp:Button ID="btn_utente_proprietario" runat="server" CssClass="testo_btn_p" 
                                                            Text="Utente proprietario" Width="115px"/>&nbsp;
												        <asp:Button ID="btn_ruolo_prop" runat="server" CssClass="testo_btn_p" Text="Ruolo proprietario" Width="115px"/>&nbsp;
												        <asp:Button ID="btn_Uo_prop" runat="server" CssClass="testo_btn_p" Text="UO proprietaria" Width="115px"/>&nbsp;
												        <asp:Button ID="btn_resp_uo_prop" runat="server" CssClass="testo_btn_p" Text="Resp. UO proprietaria" Width="132px"/>
												</td>
												</tr>
												<TR>
													<TD colSpan="2">
														<DIV id="Div1" style="OVERFLOW: auto; WIDTH: 99%; HEIGHT: 210px" align="center" runat="server">
															<asp:DataGrid id="dt_dest" runat="server" Visible="False" PageSize="1" AutoGenerateColumns="False"
																CellPadding="1" Width="100%" BorderColor="Gray" OnItemCreated="dt_dest_ItemCreated">
																<SelectedItemStyle BackColor="RosyBrown"></SelectedItemStyle>
																<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
																<ItemStyle CssClass="bg_grigioN"></ItemStyle>
																<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
																<Columns>
																	<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SystemIdMittDest"></asp:BoundColumn>
																	<asp:BoundColumn DataField="RAGIONE" HeaderText="Ragione">
																		<HeaderStyle Width="90px"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:TemplateColumn>
																		<HeaderStyle Width="3%"></HeaderStyle>
																		<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
																		<ItemTemplate>
																			<asp:ImageButton id="Img_tipo_urp" runat="server"></asp:ImageButton>
                                                                        </ItemTemplate>
																	</asp:TemplateColumn>
																	<asp:BoundColumn DataField="VAR_COD_RUBRICA" HeaderText="Codice">
																		<HeaderStyle Width="120px"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:BoundColumn DataField="VAR_DESC_CORR" HeaderText="Descrizione">
																		<HeaderStyle Width="32%"></HeaderStyle>
																	</asp:BoundColumn>
																	<asp:TemplateColumn HeaderText="Tipo">
																		<HeaderStyle Width="80px"></HeaderStyle>
																		<ItemTemplate>
																			<asp:DropDownList id="DropDownList1" runat="server" CssClass="testo" Width="100%">
																				<asp:ListItem Value="S">Uno</asp:ListItem>
																				<asp:ListItem Value="T">Tutti</asp:ListItem>
																			</asp:DropDownList>
																		</ItemTemplate>
																	</asp:TemplateColumn>
																	<asp:TemplateColumn HeaderText="Note">
																		<ItemTemplate>
																			<asp:TextBox id="TextBox1" runat="server" CssClass="testo" Width="100%"></asp:TextBox>
																		</ItemTemplate>
																	</asp:TemplateColumn>
                                                                    <asp:TemplateColumn HeaderText="Scadenza">
                                                                        <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                            Font-Underline="False" HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="gg "></asp:Label>
                                                                            <asp:TextBox ID="txt_scadenza" runat="server" CssClass="testo" MaxLength="3" Width="30px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                            Font-Underline="False" HorizontalAlign="Center" Width="70px" />
                                                                    </asp:TemplateColumn>
																	<asp:BoundColumn Visible="False" DataField="ID_RAGIONE" HeaderText="id_ragione"></asp:BoundColumn>
                                                                    <asp:TemplateColumn HeaderText="Nasc. vers." HeaderStyle-HorizontalAlign="Center">
                                                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkNascondiVersioniPrecedentiDocumento" runat="server" 
                                                                            ToolTip="Ai destinatari della trasmissione saranno nascoste le versioni precedenti a quella corrente dei documenti consolidati" CssClass="testo" 
                                                                            Checked='<%#DataBinder.Eval(Container.DataItem, "NASCONDI_VERSIONI_PRECEDENTI")%>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateColumn>
																	<asp:EditCommandColumn HeaderText="Elimina" CancelText="Cancel" EditText="&lt;img src=../images/cestino.gif border=0  onclick=confirmDel(); alt='Elimina'&gt;">
																		<HeaderStyle HorizontalAlign="Center" Width="4%"></HeaderStyle>
																		<ItemStyle HorizontalAlign="Center"></ItemStyle>
																	</asp:EditCommandColumn>
																</Columns>
																<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
															</asp:DataGrid></DIV>
													</TD>
												</TR>
											</TABLE>
											<table width=100% cellpadding=0 cellspacing=0>
											    <tr><td align=right><asp:Button ID="btn_pp_notifica" Width="220px" 
                                                        runat="server" CssClass="testo_btn_p" Visible="False" 
                                                        onclick="btn_pp_notifica_Click" /></td></tr>
											</table>
										</asp:panel>
									    <br>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</TBODY>
			</table>
			<INPUT id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server"></input>
		</form>
	</body>
</HTML>
