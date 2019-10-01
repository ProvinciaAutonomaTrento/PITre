<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisibilitaFasc.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_ProfDinamicaFasc.VisibilitaFasc" validateRequest="false"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 

<html>
<head runat = "server">
        <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	<base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="3600"></asp:ScriptManager>
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Visibilita' Tipologia Fascicoli" />
        
        <script language="javascript" type="text/javascript">
            // This Script is used to maintain Grid Scroll on Partial Postback
            var scrollTopListaCampi;
            var scrollTopListaRuoli;
            //Register Begin Request and End Request 
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
            //Get The Div Scroll Position
            function BeginRequestHandler(sender, args) {
               var divListaCampi = document.getElementById('div_listaCampi');
               var divListaRuoli = document.getElementById('div_listaRuoli');
               if (divListaCampi != null) {
                   scrollTopListaCampi = divListaCampi.scrollTop;
               }
               if (divListaRuoli != null) {
                   scrollTopListaRuoli = divListaRuoli.scrollTop;
               }
            }
            //Set The Div Scroll Position
            function EndRequestHandler(sender, args) {
               var divListaCampi = document.getElementById('div_listaCampi');
               var divListaRuoli = document.getElementById('div_listaRuoli');
               if (divListaCampi != null) {
                   divListaCampi.scrollTop = scrollTopListaCampi;
               }
               if (divListaRuoli != null) {
                   divListaRuoli.scrollTop = scrollTopListaRuoli;
               }

               hideWait();
           }        
           
           function showWait() {
               this._popup = $find('mdlPopupWait');
               this._popup.show();
           }

           function hideWait() {
               this._popup = $find('mdlPopupWait');
               this._popup.hide();                  
           }
           </script>            
           
        <table width="100%">
				<tr>
					<td class="titolo"  align="left" bgColor="#e0e0e0" height="20">
						<asp:Label id="lbl_titolo" runat="server"></asp:Label>
				    </td>
					<td align="right" bgColor="#e0e0e0" width="30%" style="padding-right:13px;">
			            <asp:Button id="btnChiudi" runat="server" Text="Chiudi" CssClass="testo_btn_p" OnClientClick="window.close();"></asp:Button>             
			        </td>
				</tr>
				<tr>
		            <td colspan="2" style="height:10px; border-bottom:2px #810D06 solid;"></td>
		        </tr>
				<!-- Ricerca ruoli -->
				<tr>
				    <td class="testo_grigio_scuro_grande">Ricerca ruoli :
					    <asp:DropDownList id="ddl_ricTipo" Runat="server" CssClass="testo_grigio_scuro" 
					        AutoPostBack="true" Width="22%" onselectedindexchanged="ddl_ricTipo_SelectedIndexChanged">
                            <asp:ListItem Value="SEL" Text="Seleziona ..."></asp:ListItem>
						    <asp:ListItem Value="T" Text="Tutti"></asp:ListItem>
						    <asp:ListItem Value="COD_RUOLO" Text="Codice Ruolo"></asp:ListItem>
					        <asp:ListItem Value="DES_RUOLO" Text="Descr. Ruolo"></asp:ListItem>
					        <asp:ListItem Value="COD_RF" Text="Codice RF"></asp:ListItem>
					        <asp:ListItem Value="DES_RF" Text="Descr. RF"></asp:ListItem>
					        <asp:ListItem Value="COD_AOO" Text="Codice AOO"></asp:ListItem>
					        <asp:ListItem Value="DES_AOO" Text="Descr. AOO"></asp:ListItem>
					        <asp:ListItem Value="COD_UO" Text="Codice UO"></asp:ListItem>
					        <asp:ListItem Value="DES_UO" Text="Descr. UO"></asp:ListItem>	
					        <asp:ListItem Value="COD_TIPO" Text="Codice TIPO"></asp:ListItem>
					        <asp:ListItem Value="DES_TIPO" Text="Descr. TIPO"></asp:ListItem>				
						</asp:DropDownList>
					    <asp:textbox id="txt_ricerca"  CssClass="testo_grigio_scuro_grande" Runat="server" Width="55%" ReadOnly="true" BackColor="Gainsboro"></asp:textbox>
					</td>
					<td align="right" width="33%" style="padding-right:13px;">
					    <asp:button id="btn_find"  CssClass="testo_btn_p_large" Runat="server" Text="Cerca" OnClick="btn_find_Click"></asp:button>
					    <asp:Button id="btn_conferma" runat="server" Text="Conferma" CssClass="testo_btn_p_large" OnClick="btn_conferma_Click" OnClientClick="showWait();"></asp:Button>
					</td>
				</tr>
				<tr>
		            <td></td>
		            <td align="right" width="30%" style="padding-right:13px;">
		                <asp:Button id="btn_estendiACampi" runat="server" Text="Estendi a Campi" ToolTip="Estende la configurazione attuale dei diritti sui ruoli, a tutti i campi presenti nella lista." CssClass="testo_btn_p_large" onclick="btn_estendiACampi_Click" OnClientClick="showWait();"></asp:Button>                
				        <cc1:ConfirmButtonExtender ID="ConfirmButtonEstendiACampi" runat="server" ConfirmText="Gli attuali diritti sui ruoli verranno estesi a tutti i campi presenti nella lista, sovrascrivendo i vecchi diritti. Confermare ?" TargetControlID="btn_estendiACampi">
                        </cc1:ConfirmButtonExtender>      
		            </td>
		        </tr>
			    <tr>
			        <td colspan="2" style="height:10px; text-align:center;">
			            <asp:label id="lbl_ricercaRuoli" runat="server" CssClass="titolo"></asp:label>
					</td>
			    </tr>
				<tr>
					<td colspan="2">
					<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
					<DIV id="div_listaRuoli" align="center" runat="server" style="OVERFLOW: auto; HEIGHT: 225px; width:100%">
					<asp:DataGrid id="dg_Ruoli" runat="server" AutoGenerateColumns="False" Width="100%" Visible="true" onitemcommand="dg_Ruoli_ItemCommand">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
							<Columns>
								<asp:BoundColumn Visible="False" DataField="ID_GRUPPO" HeaderText="id_gruppo"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CODICE RUOLO"  HeaderText="Cod. Ruolo" ItemStyle-HorizontalAlign="Left">
							        <HeaderStyle Width="12%"></HeaderStyle>
						            <ItemStyle HorizontalAlign="Left" />
						        </asp:BoundColumn>
								<asp:BoundColumn DataField="DESCRIZIONE RUOLO"  HeaderText="Descrizione Ruolo" ItemStyle-HorizontalAlign="Left">
									<HeaderStyle Width="43%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left" />
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Creazione">
									<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
									<HeaderTemplate>
						                <table>
						                <tr>
    						                <td><asp:CheckBox ID="CheckAllCreazioneTipologia" runat="server" 
                                                    AutoPostBack="true" 
                                                    oncheckedchanged="CheckAllCreazioneTipologia_CheckedChanged" ToolTip="Seleziona tutti"/></td>
    						                <td>Creazione</td>
						                </tr>
						                </table>
						            </HeaderTemplate>  
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox id="CheckBox1" runat="server" AutoPostBack="True"></asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Ricerca">
									<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
									<HeaderTemplate>
						                <table>
						                    <tr>
						                        <td><asp:CheckBox ID="CheckAllRicercaTipologia" runat="server" AutoPostBack="true" 
                                                        oncheckedchanged="CheckAllRicercaTipologia_CheckedChanged" ToolTip="Seleziona tutti"/></td>
						                        <td>Ricerca</td>
						                    </tr>
                                        </table>						                
                                    </HeaderTemplate>      
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox id="CheckBox2" runat="server" AutoPostBack="True"></asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Vis. Campi">
							        <HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
							        <ItemStyle HorizontalAlign="Center"></ItemStyle>
							        <ItemTemplate>
								        <asp:ImageButton ID="imgBtn_visCampi" runat="server" CommandName="VisibilitaCampi" ImageUrl="~/AdminTool/Images/utenti.gif" />
							        </ItemTemplate>
						        </asp:TemplateColumn>
							</Columns>
						</asp:DataGrid>
						</DIV>
						</ContentTemplate>
			            </asp:UpdatePanel>
					</td>
				</tr>
	</table> 
	
	<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Panel ID="panel_listaCampi" Visible="false" runat="server">
	<table width="100%">
	    <tr>
		    <td colspan="2" style="height:10px; border-bottom:2px #810D06 solid;"></td>
		</tr>
		<tr>
			<td class="titolo"  align="left" bgColor="#e0e0e0" height="20">
				<asp:Label id="lbl_ruolo" runat="server"></asp:Label>
			</td>
			<td align="right" width="30%" bgColor="#e0e0e0" style="padding-right:15px;">
                <asp:Button id="btn_estendiALista" runat="server" Text="Estendi a Ruoli" ToolTip="Estende la configurazione attuale dei diritti sui campi, a tutti i ruoli presenti nella lista."                 
                    CssClass="testo_btn_p_large" onclick="btn_estendiALista_Click" OnClientClick="showWait();"></asp:Button>
                <cc1:ConfirmButtonExtender ID="ConfirmButtonEstendiALista" runat="server" ConfirmText="Gli attuali diritti sui campi verranno estesi a tutti i ruoli presenti nella lista, sovrascrivendo i vecchi diritti. Confermare ?" TargetControlID="btn_estendiALista">
                </cc1:ConfirmButtonExtender>   
            </td>
		</tr>
		<tr>
	        <td colspan="2" style="height:10px;"></td>
	    </tr>	
		<!-- Lista Campi Tipologia -->
		<tr>
			<td colspan="2">
			    <DIV id="div_listaCampi" align="center" runat="server" style="OVERFLOW: auto; HEIGHT: 225px; width:100%">
			    <asp:DataGrid id="dg_Campi" runat="server" AutoGenerateColumns="False" Width="100%" Visible="true" onitemcommand="dg_Campi_ItemCommand">
                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
			        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					<ItemStyle CssClass="bg_grigioN"></ItemStyle>
					<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
					<Columns>
						<asp:BoundColumn Visible="False" DataField="ID_CAMPO" HeaderText="id_campo"></asp:BoundColumn>
						<asp:BoundColumn DataField="DESCRIZIONE"  HeaderText="Descrizione" ItemStyle-HorizontalAlign="Left">
							<HeaderStyle Width="55%"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Modifica">
							<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
							<HeaderTemplate>
						        <table>
						            <tr>
    						            <td><asp:CheckBox ID="CheckAllModificaCampi" runat="server" 
                                                AutoPostBack="true" 
                                                ToolTip="Seleziona tutti" 
                                                oncheckedchanged="CheckAllModificaCampi_CheckedChanged"/></td>
    						            <td>Modifica</td>
						            </tr>
						        </table>
						    </HeaderTemplate>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox id="cb_insListaCampi" runat="server" AutoPostBack="True" oncheckedchanged="cb_insListaCampi_CheckedChanged"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>						
						<asp:TemplateColumn HeaderText="Visibilità">
							<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
							<HeaderTemplate>
						        <table>
						            <tr>
    						            <td><asp:CheckBox ID="CheckAllVisibilitaCampi" runat="server" 
                                                AutoPostBack="true" 
                                                ToolTip="Seleziona tutti" 
                                                oncheckedchanged="CheckAllVisibilitaCampi_CheckedChanged"/></td>
    						            <td>Visibilità</td>
						            </tr>
						        </table>
						    </HeaderTemplate>	
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox id="cb_visListaCampi" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>						
						<asp:TemplateColumn HeaderText="Ruoli">
							<ItemTemplate>
								<asp:ImageButton ID="imgBtn_visRuoliCampo" runat="server" CommandName="VisibilitaRuoliCampo" ImageUrl="~/AdminTool/Images/utenti.gif" OnClientClick="showWait();" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>
				</DIV>
			</td>
		</tr>		
	</table>   	
	</asp:Panel>
	</ContentTemplate>
	<Triggers>
	    <asp:AsyncPostBackTrigger ControlID="dg_Ruoli" />
	</Triggers>
	</asp:UpdatePanel>
	
	<!-- PopUp Wait-->
    <cc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait" BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display:none; font-weight:bold; font-size:xx-large; font-family:Arial;">Attendere prego ...</div>
     
    </form>    
</body>
</html>