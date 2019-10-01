<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Page language="c#" Codebehind="ricercaDocumenti.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.RicercaDocumenti" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta content="False" name="vs_showGrid">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" id="btn_find_Click" event="onclick()" for="btn_find">
					window.document.body.style.cursor='wait';
					SetPanelsVisibility()
		</script>
		<script language="javascript">
					
					// Impostazione visibilità pannelli per ricerca
					function SetPanelsVisibility()
					{
						document.getElementById("LOADING").style.visibility = "Visible";
						document.getElementById("DivDataGrid").style.visibility = "hidden";
						
						var lbl_count=document.getElementById("lbl_countRecord");
						if(lbl_count!=null)
							lbl_count.style.visibility = "hidden";							
					}

					function CloseWindow(retValue)
					{
						window.returnValue=retValue;
						window.close();
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
					
					function ApriModalOggettarioClassifica(wnd) 
               {
	               var left=(screen.availWidth-595);
	               var top=(screen.availHeight-620);
	               var args=new Object;
	               args.window=window;

	               rtnValue = window.showModalDialog('../popup/oggettario.aspx?wnd='+wnd,	args, 'dialogWidth:575px;dialogHeight:420px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no'); 	
	               
	               if(rtnValue!=undefined)
	               {
	            
	                  document.getElementById("txt_oggetto").value = rtnValue;
	               }
               }

					
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
		<form id="frm_rispostaProtoUscita" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca documenti" />
			<input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<TABLE class="contenitore" id="tbl_rispostaProtoUscita" height="603" width="580" align="center"
				border="0">
				<TR vAlign="middle" height="20">
					<td class="menu_1_rosso" align="center">Ricerca Documenti</td>
				</TR>
				<tr height="120">
					<td>
						<TABLE class="info_grigio" id="tbl_cont" height="100" cellSpacing="0" cellPadding="0" width="98%"
							align="center" border="1">
							<TR>
								<td vAlign="top" align="center">
									<table id="tblNumeroProtocollo" cellSpacing="0" cellPadding="0" width="98%" align="center"
										border="0">
										<TR height="20">
											<td></td>
											<td align="center" colSpan="2"><asp:radiobuttonlist id="rbl_sceltaRicerca" Runat="server" AutoPostBack="True" Width="336px" CssClass="testo_grigio_scuro"
													RepeatDirection="Horizontal">
													<asp:ListItem Value="P" Selected="True">Doc. protocollati</asp:ListItem>
													<asp:ListItem Value="G">Doc. </asp:ListItem>
													<asp:ListItem Value="PRED">Pred. </asp:ListItem>
													<asp:ListItem Value="ADL">Doc. in ADL</asp:ListItem>
												</asp:radiobuttonlist></td>
										</TR>
										<tr height="80">
											<td colSpan="3">
												<table id="tbl_filtri_prot" cellSpacing="0" cellPadding="0" width="100%" align="center">
													<asp:panel id="pnl_filtri_prot" Runat="server">
														<TBODY>
															<TR>
																<TD class="titolo_scheda" height="29"><LABEL id="lblNumeroProtocollo" style="WIDTH: 100px">Num. 
																		protocollo:</LABEL>
																	<asp:dropdownlist id="ddl_numProto" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="True">
																		<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
																		<asp:ListItem Value="1">Intervallo</asp:ListItem>
																	</asp:dropdownlist></TD>
																<TD class="testo_grigio" width="59%" height="29">
																	<asp:label id="lblInitNumProto" runat="server" CssClass="titolo_scheda" Width="25px" Visible="False">Da:</asp:label>
																	<asp:textbox id="txtInitNumProto" tabIndex="0" runat="server" CssClass="testo_grigio" Width="80px"></asp:textbox><IMG height="2" src="../images/proto/spaziatore.gif" width="1" border="0">
																	<asp:label id="lblEndNumProto" runat="server" CssClass="titolo_scheda" Width="15px" Visible="False">a:</asp:label>
																	<asp:textbox id="txtEndNumProto" runat="server" CssClass="testo_grigio" Width="80px" Visible="False"></asp:textbox>
																	<asp:label id="lbl_annoProto" runat="server" CssClass="titolo_scheda" Width="20px"><IMG height="2" src="../images/proto/spaziatore.gif" width="5px" border="0">anno:&nbsp;</asp:label>
																	<asp:textbox id="txt_annoProto" runat="server" CssClass="testo_grigio" Width="55px" maxlength="4"></asp:textbox></TD>
															</TR>
															<TR>
																<TD class="titolo_scheda" height="29"><LABEL id="lblDataProtocollo" style="WIDTH: 100px">Data 
																		protocollo:</LABEL>
																	<asp:dropdownlist id="ddl_dtaProto" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="True">
																		<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
																		<asp:ListItem Value="1">Intervallo</asp:ListItem>
																	</asp:dropdownlist></TD>
																<TD class="testo_grigio" width="59%" height="29">
																	<asp:label id="lblInitDtaProto" runat="server" CssClass="titolo_scheda" Width="10px" Visible="False">Da:</asp:label>
																	<uc3:Calendario id="txtInitDtaProto" runat="server" Visible="true" />
																	<IMG height="2" src="../images/proto/spaziatore.gif" width="1" border="0">
																	<asp:label id="lblEndDataProtocollo" runat="server" CssClass="titolo_scheda" Width="10px" Visible="False">a:</asp:label>
																	<uc3:Calendario id="txtEndDataProtocollo" runat="server" Visible="false" />
																	</TD>
															</TR>
													</asp:panel><asp:panel id="pnl_filtri_grigi" Runat="server">
														<TR>
															<TD class="titolo_scheda" height="29"><LABEL id="lblIdDocumento" style="WIDTH: 100px">Id. 
																	documento:</LABEL>
																<asp:dropdownlist id="ddl_idDocumento" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="True">
																	<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
																	<asp:ListItem Value="1">Intervallo</asp:ListItem>
																</asp:dropdownlist></TD>
															<TD class="testo_grigio" width="59%" height="29">
																<asp:label id="lblDAidDoc" runat="server" CssClass="titolo_scheda" Width="25px" Visible="False">Da:</asp:label>
																<asp:textbox id="txt_initIdDoc" runat="server" CssClass="testo_grigio" Width="80px"></asp:textbox><IMG height="2" src="../images/proto/spaziatore.gif" width="1" border="0">
																<asp:label id="lblAidDoc" runat="server" CssClass="titolo_scheda" Width="15px" Visible="False">a:</asp:label>
																<asp:textbox id="txt_fineIdDoc" runat="server" CssClass="testo_grigio" Width="80px" Visible="False"></asp:textbox></TD>
														</TR>
														<TR>
															<TD class="titolo_scheda" height="29"><LABEL id="lblDtaCreazione" style="WIDTH: 100px">Data 
																	creazione:</LABEL>
																<asp:dropdownlist id="ddl_dataCreazione" runat="server" CssClass="testo_grigio" Width="110px" AutoPostBack="True">
																	<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
																	<asp:ListItem Value="1">Intervallo</asp:ListItem>
																</asp:dropdownlist></TD>
															<TD class="testo_grigio" width="59%" height="29">
																<asp:label id="lblDAdataCreaz" runat="server" CssClass="titolo_scheda" Width="10px" Visible="False">Da:</asp:label>
																<uc3:Calendario id="txt_initDataCreaz" runat="server" Visible="true" />
																<IMG height="2" src="../images/proto/spaziatore.gif" width="1" border="0">
																<asp:label id="lblAdataCreaz" runat="server" CssClass="titolo_scheda" Width="10px" Visible="False">a:</asp:label>
																<uc3:Calendario id="txt_fineDataCreaz" runat="server" Visible="false" />
																</TD>
														</TR>
													</asp:panel>
												</table>
											</td>
										</tr>
										<!--OGGETTARIO-->
										<tr height="65">
										   <td colSpan="3">
                                    <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
                                       <tr>
                                          <td class="titolo_scheda" valign="middle" width="87%" height="19">Oggetto</td>
                                          <td align="right" width="19px"><asp:imagebutton id="btn_RubrOgget_E" ImageUrl="../images/proto/ico_oggettario.gif" Width="19px"
                                          Height="17px" Runat="server" AlternateText="Seleziona un oggetto nell'oggettario"></asp:imagebutton></td>
                                          </tr>
                                          <tr>
                                          <td colspan="2">
                                          <asp:textbox id="txt_oggetto" runat="server" CssClass="testo_grigio" Width="90%" Height="32px"
                                          TextMode="MultiLine"></asp:textbox></td>
                                       </tr>

                                    </table>
										   </td>
										</tr>
										<!--FINE OGGETTARIO-->
										
										<tr borderColor="#00cc00">
											<TD align="center" colSpan="7" height="30"><asp:button id="btn_find" runat="server" Width="55px" CssClass="PULSANTE_HAND" Text="CERCA"
													Height="19px"></asp:button></TD>
										</tr>
									</table>
								</td>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr vAlign="top" height="20">
					<td class="countRecord" vAlign="middle" align="center" width="100%" height="20"><asp:label id="lbl_countRecord" Runat="server" CssClass="titolo_rosso" Visible="False">Documenti Tot:</asp:label></td>
				</tr>
				<TR vAlign="top" height="295">
					<TD align="center">
						<DIV class="testo_grigio_scuro" id="LOADING" style="FONT-SIZE: 12px; LEFT: 250px; VISIBILITY: hidden; POSITION: absolute; TOP: 300px"
							align="center">Ricerca in corso . . .</DIV>
						<DIV id="DivDataGrid" style="OVERFLOW: auto; WIDTH: 570px; HEIGHT: 270px" align="center">
							<table>
								<tr>
									<td>
										<asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" Width="98%" Visible="False" DataKeyField="idProfile"
											AutoGenerateColumns="False" BorderWidth="1px" AllowPaging="True" CellPadding="1" BorderColor="Gray"
											AllowCustomPaging="True" OnItemCreated="Grid_OnItemCreated">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
											<Columns>
												<asp:TemplateColumn>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
                                                        <asp:RadioButton ID="OptDoc" runat="server" AutoPostBack="True" Visible="True" Text=""
                                                             OnCheckedChanged="checkOPT" TextAlign="Right"></asp:RadioButton>
                                                    </ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Doc. Data">
													<HeaderStyle Wrap="False" HorizontalAlign="Justify" Width="10px" VerticalAlign="Middle"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<asp:Label id="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descDoc") %>'>
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox id="Textbox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descDoc") %>'>
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Registro">
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<asp:Label id=Label10 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox id=Textbox10 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Oggetto">
													<HeaderStyle Width="480px"></HeaderStyle>
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>' ID="Label2">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>' ID="Textbox2">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Tipo">
													<HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<asp:Label id="lbl_tipo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox id="txt_tipo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="Data Annullamento">
													<HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<asp:Label id="lbl_dtaAnnullam" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dataAnnullamento") %>'>
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox id="txt_dtaAnnullam" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dataAnnullamento") %>'>
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False">
													<HeaderStyle HorizontalAlign="Center" Width="10px"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<cc1:ImageButton id="img_select" runat="server" BorderWidth="0px" AlternateText="Seleziona il documento"
															ImageUrl="../images/proto/ico_riga.gif" CommandName="Select" DisabledUrl="../images/proto/ico_riga.gif"></cc1:ImageButton>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="chiave">
													<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
													<ItemTemplate>
														<asp:Label id="lbl_key" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
														</asp:Label>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="idProfile">
													<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
													<ItemTemplate>
														<asp:Label id="lbl_idProfile" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'>
														</asp:Label>
													</ItemTemplate>
												</asp:TemplateColumn>
													<asp:TemplateColumn Visible="False" HeaderText="System Id">
									                    <HeaderStyle Wrap="False"></HeaderStyle>
									                    <ItemTemplate>
										                    <asp:Label id="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
										                    </asp:Label>
									                    </ItemTemplate>
									                    <EditItemTemplate>
										                    <asp:TextBox id="Textbox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
										                    </asp:TextBox>
									                    </EditItemTemplate>
								                </asp:TemplateColumn>
                                                <asp:TemplateColumn Visible="False">
									                    <HeaderStyle Wrap="False"></HeaderStyle>
									                    <ItemTemplate>
										                    <asp:Label id="lbl_colore" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.protocollo") %>'>
										                    </asp:Label>
									                    </ItemTemplate>			
								                </asp:TemplateColumn>
											</Columns>
											<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
												Mode="NumericPages"></PagerStyle>
										</asp:datagrid></td>
								</tr>	
							</table>
						</DIV>
					</TD>
				</TR>
				<TR height="30">
					<TD align="center" height="30">&nbsp;
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button></TD>
				</TR>
				</TABLE>
			</form>
	</body>
</HTML>
