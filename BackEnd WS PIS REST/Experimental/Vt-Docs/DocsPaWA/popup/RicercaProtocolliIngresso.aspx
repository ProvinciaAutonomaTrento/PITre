<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Page language="c#" Codebehind="RicercaProtocolliIngresso.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.RicercaProtocolliIngresso" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta name="vs_showGrid" content="False">
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
						
						var pnlCorrispondenti=document.getElementById("pnl_corr");
						if (pnlCorrispondenti!=null)
							pnlCorrispondenti.style.visibility = "hidden";								
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


					function OpenAvvisoModale(mitt, oggetto, isProto, diventaOccasionale) 
			{
				var newLeft=(screen.availWidth-510);
				var newTop=(screen.availHeight-470);

				var myUrl = "AvvisoRispProtocollo.aspx?MITT=" + mitt + "&OGG=" + oggetto + "&isProto=" + isProto + "&isOcc=" + diventaOccasionale;
				rtnValue = window.showModalDialog(myUrl,'','dialogWidth:450px;dialogHeight:330px;status:no;resizable:no;dialogLeft:'+newLeft+' ;dialogTop:'+newTop+' ;scroll:no;help:no;'); 					
				frm_rispostaProtoUscita.hd_returnValueModal.value = rtnValue;	
				window.document.frm_rispostaProtoUscita.submit();			
			}		
					
		</script>
		<script  language="C#" runat="server">
		
			void resetSelection(object sender, ImageClickEventArgs e) 
			{
            foreach (DataGridItem dgItem in this.DataGrid1.Items)
				{
					RadioButton optCorr = dgItem.Cells[4].FindControl("optCorr") as RadioButton;
					optCorr.Checked = false;			
				}	
			}
			
			void checkOPT(object sender,EventArgs e) 
			{
            foreach (DataGridItem dgItem in this.DataGrid1.Items)
				{		
					RadioButton optCorr=dgItem.Cells[4].FindControl("optCorr") as RadioButton;
					if (optCorr!=null)
						optCorr.Checked=optCorr.Equals(sender);
				}
			}
			
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
		<form id="frm_rispostaProtoUscita" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca protocolli in ingresso" />
			<input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<TABLE class="contenitore" id="tbl_rispostaProtoUscita" height="603" width="580" align="center"
				border="0">
				<TR vAlign="middle" height="20">
					<td class="menu_1_rosso" align="center">Ricerca protocolli in ingresso</td>
				</TR>
				<tr height="120">
					<td>
						<TABLE class="info_grigio" height="100" cellSpacing="0" cellPadding="0" width="98%" align="center"
							border="0">
							<TR>
								<td align="center">
									<table id="tblNumeroProtocollo" cellSpacing="0" cellPadding="0" width="98%" align="center"
										border="0">
										<TR>
											<td height="1"></td>
										</TR>
										<TR>
											<td></td>
											<td colSpan="2"><asp:checkbox id="chk_ADL" Runat="server" Cssclass="titolo_scheda" Text="Ricerca solo in ADL"
													AutoPostBack="True"></asp:checkbox></td>
										</TR>
                                         <asp:Panel id="pnl_catene_extra_aoo" runat="server" Visible="false">
                                        <tr>
                                        
                                            <td></td>
                                            <td class="titolo_scheda" width="41%" height="29">
                                              <asp:label id="lbl_registro" runat="server" Width="97px">Registro:</asp:label>
                                              <asp:dropdownlist id="ddl_reg" runat="server" AutoPostBack="True" Width="110px" CssClass="testo_grigio">
											   </asp:dropdownlist>
                                            </td>
                                            <td>&nbsp;</td>   
                                        </tr>
                                        </asp:Panel>
										<tr>
											<td></td>
											<TD class="titolo_scheda" width="41%" height="29"><asp:label id="lblNumeroProtocollo" runat="server" Width="100px">Num. protocollo:</asp:label><asp:dropdownlist id="ddl_numProto" runat="server" AutoPostBack="True" Width="110px" CssClass="testo_grigio">
													<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
												</asp:dropdownlist></TD>
											<TD class="testo_grigio" width="58%" height="29"><asp:label id="lblInitNumProto" runat="server" Width="25px" CssClass="titolo_scheda" Visible="False">Da:</asp:label><asp:textbox id="txtInitNumProto" runat="server" Width="80px" CssClass="testo_grigio"></asp:textbox><IMG height="2" src="../images/proto/spaziatore.gif" width="1" border="0">
												<asp:label id="lblEndNumProto" runat="server" Width="15px" CssClass="titolo_scheda" Visible="False">a:</asp:label><asp:textbox id="txtEndNumProto" runat="server" Width="80px" CssClass="testo_grigio" Visible="False"></asp:textbox><asp:label id="lbl_annoProto" runat="server" Width="20px" CssClass="titolo_scheda"><IMG height="2" src="../images/proto/spaziatore.gif" width="5px" border="0">anno:&nbsp;</asp:label><asp:textbox id="txt_annoProto" runat="server" Width="55px" CssClass="testo_grigio" maxlength="4"></asp:textbox></TD>
										</tr>
										<tr>
											<td></td>
											<TD class="titolo_scheda" width="41%" height="28"><asp:label id="lblDataProtocollo" runat="server" Width="100px">Data protocollo:</asp:label><asp:dropdownlist id="ddl_dtaProto" runat="server" AutoPostBack="True" Width="110px" CssClass="testo_grigio">
													<asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
													<asp:ListItem Value="1">Intervallo</asp:ListItem>
												</asp:dropdownlist></TD>
											<TD class="testo_grigio" width="58%" height="29">
											<asp:label id="lblInitDtaProto" runat="server" Width="15px" CssClass="titolo_scheda" Visible="False">Da:</asp:label>
											<uc3:Calendario id="txtInitDtaProto" runat="server" Visible="true" />
											<IMG height="2" src="../images/proto/spaziatore.gif" width="1" border="0">
											<asp:label id="lblEndDataProtocollo" runat="server" Width="15px" CssClass="titolo_scheda" Visible="False">a:</asp:label>
											<uc3:Calendario id="txtEndDataProtocollo" runat="server" Visible="false" />
											</TD>
										</tr>
										<tr borderColor="#00cc00">
											<TD align="center" colSpan="7" height="30"><!--<asp:imagebutton id="btn_find_old" runat="server" CssClass="ImgHand" AlternateText="Trova" ImageUrl="../images/proto/zoom.gif"></asp:imagebutton>-->
												<asp:button id="btn_find" runat="server" CssClass="PULSANTE_HAND" Width="55px" Text="CERCA"
													Height="19px"></asp:button>
											</TD>
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
						<DIV class="testo_grigio_scuro" id="LOADING" style="FONT-SIZE: 12px; LEFT: 250px; VISIBILITY: hidden; POSITION: absolute; TOP: 300px">Ricerca 
							in corso . . .</DIV>
						<DIV id="DivDataGrid" style="OVERFLOW: auto; WIDTH: 570px; HEIGHT: 295px"><asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" Width="98%" Visible="False" AutoGenerateColumns="False"
								BorderWidth="1px" AllowPaging="True" CellPadding="1" BorderColor="Gray" AllowCustomPaging="True">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Data">
										<HeaderStyle Wrap="False" HorizontalAlign="Center" Width="10px" VerticalAlign="Middle"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' ForeColor="Red" Font-Bold="True">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id=Textbox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Registro">
										<HeaderStyle Width="40px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>' ID="Label6">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>' ID="Textbox6">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Oggetto">
										<HeaderStyle Width="400px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>' ID="Label2">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>' ID="Textbox2">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="true" HeaderText="Mittente">
										<HeaderStyle Width="30px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittDest") %>' ID="Label3">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittDest") %>' ID="Textbox3">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderStyle HorizontalAlign="Center" Width="40px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<HeaderTemplate>
											<asp:Label id="Label7" runat="server" CssClass="menu_1_bianco_dg">Sel</asp:Label>
										</HeaderTemplate>
										<ItemTemplate>
											<asp:RadioButton id="OptCorr" runat="server" AutoPostBack="True" Visible="True" OnCheckedChanged="checkOPT"
												TextAlign="Right" Text=""></asp:RadioButton>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="chiave">
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<asp:Label id=Label8 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
									Mode="NumericPages"></PagerStyle>
							</asp:datagrid></DIV>
					</TD>
				</TR>
				<TR vAlign="middle" height="80">
					<TD>
					<asp:panel id="pnl_corr" Runat="server" Visible="false">
							<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="570" align="center" border="0">
								<TR>
									<TD class="menu_1_rosso" style="WIDTH: 523px" align="center" width="523" height="20">Seleziona 
										il mittente per il protocollo in ingresso
									</TD>
									<TD>
										<asp:imagebutton id="btn_clearFlag" onclick="resetSelection" runat="server" Visible="False" AlternateText="Elimina tutte le selezioni"
											ImageUrl="../images/proto/RispProtocollo/uncheked.gif"></asp:imagebutton></TD>
									<TD>
										<asp:ImageButton id="btn_chiudi_risultato" runat="server" Visible="False" AlternateText="Chiudi  il pannello  dei corrispondenti"
											ImageUrl="../images/proto/RispProtocollo/newclose.gif" ImageAlign="Right"></asp:ImageButton></TD>
								</TR>
								<TR height="70">
									<TD align="center" colSpan="3">
										<DIV id="div_corr" style="OVERFLOW: auto; HEIGHT: 65px">
											<asp:datagrid id="dg_lista_corr" runat="server" SkinID="datagrid" Width="96%" Visible="True" BorderColor="Gray"
												CellPadding="1" BorderWidth="1px" AutoGenerateColumns="False" PageSize="1">
												<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
												<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
												<ItemStyle CssClass="bg_grigioN"></ItemStyle>
												<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="SYSTEM_ID"></asp:BoundColumn>
													<asp:BoundColumn DataField="DESC_CORR" HeaderText="Descrizione">
														<HeaderStyle Width="490px"></HeaderStyle>
														<ItemStyle Font-Names="verdana"></ItemStyle>
													</asp:BoundColumn>
													<asp:BoundColumn Visible="False" DataField="TIPO_CORR" HeaderText="Tipo">
														<HeaderStyle Width="30px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
													</asp:BoundColumn>
													<asp:TemplateColumn>
														<HeaderStyle Width="30px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<asp:RadioButton id="OptCorr" runat="server" AutoPostBack="True" Visible="True" OnCheckedChanged="checkOPT" 
																TextAlign="Right" Text=""></asp:RadioButton>
														</ItemTemplate>
													</asp:TemplateColumn>
												</Columns>
												<PagerStyle VerticalAlign="Middle" BorderColor="White" HorizontalAlign="Center" BackColor="#C2C2C2"
													CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
											</asp:datagrid></DIV>
									</TD>
								</TR>
							</TABLE>
						</asp:panel>
						</TD>
				</TR>
				<TR height="30">
					<TD align="center" height="30"><asp:button id="btn_ok" runat="server" Text="OK" CssClass="PULSANTE"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" Text="CHIUDI" CssClass="PULSANTE"></asp:button></TD>
				</TR>
			</TABLE>
			</form>
	</body>
</HTML>
