<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuLog" Src="../UserControl/MenuLog.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="AbilitaLog.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Logs.AbilitaLog" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<script language="C#" runat="server">
			public bool getCheckBox(object abilita)
			{			
				string abil = abilita.ToString();
				if(abil == "true")
				{
					return true;
				}
				else
				{
					return false;
				}
			}

            public string getVal(object value)
            {
                return value.ToString();
            }    
		</script>
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
				hlp = window.open('../help.aspx?from=GLA','','width=450,height=500,scrollbars=YES');
			}			
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}
			function CheckAllDataGridCheckBoxes(aspCheckBoxID, checkVal) 
			{
				//re = new RegExp(':' + aspCheckBoxID + '$')  
				for(i = 0; i < document.forms[0].elements.length; i++) 
				{
					elm = document.forms[0].elements[i]
					
					if (elm.type == 'checkbox') 
					{
					    
						//if (re.test(elm.name)) 
						//{
							elm.checked = checkVal
						//}						
					}
				}
			}
			function Modificato()
			{
				document.forms[0].hd_modificato.value = "S";
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
		</SCRIPT>
		<script language="javascript" id="btn_modifica_click" event="onclick()" for="btn_modifica">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">		
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Abilitazioni sui log" />
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
					<td class="titolo" align="center" width="100%" bgColor="#e0e0e0" height="20">Attivazione 
						Log</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table height="100" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr height="100%" width="100%">
								<td width="120" height="100%"><uc2:menulog id="MenuLog" runat="server"></uc2:menulog></td>
								<td align="center" width="100%" height="100%"><br>
									<table cellSpacing="0" cellPadding="0" align="center" border="0">
										<tr>
											<td class="pulsanti" width="700" align="center">
												<table width="100%">
													<tr>
														<td align="left"><asp:label id="lbl_tit" CssClass="titolo" Runat="server"></asp:label></td>
														<td align="right"><asp:button id="btn_modifica" runat="server" CssClass="testo_btn" Text="Modifica"></asp:button>&nbsp;</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td align="right">
												<br>
												<asp:CheckBox id="chk_all" Checked="False" runat="server" Text="Seleziona / deseleziona tutti"
													TextAlign="left" CssClass="testo_grigio_scuro"></asp:CheckBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
												<DIV id="DivDGList" style="OVERFLOW: auto; HEIGHT: 465px">
													<asp:datagrid id="dg_AbilitaLog" runat="server" BorderWidth="1px" CellPadding="1" BorderColor="Gray"
														AutoGenerateColumns="False" Width="700px" OnItemDataBound="dg_AbilitaLog_DataBound">
														<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
														<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
														<ItemStyle CssClass="bg_grigioN"></ItemStyle>
														<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
														<Columns>
															<asp:BoundColumn DataField="oggetto" HeaderText="Oggetto">
																<HeaderStyle Width="20%"></HeaderStyle>
															</asp:BoundColumn>
															<asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Descrizione">
																<HeaderStyle Width="52%"></HeaderStyle>
															</asp:BoundColumn>
															<asp:TemplateColumn HeaderText="Attivo">
																<HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
																<ItemStyle HorizontalAlign="Center"></ItemStyle>
																<ItemTemplate>
																	<asp:CheckBox ID="Chk" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.attivo")) %>' runat="server" />
																</ItemTemplate>
															</asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="Notifica">
																<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
																<ItemStyle HorizontalAlign="Center"></ItemStyle>
																<ItemTemplate>
																	<asp:DropDownList ID="ddlNotify" runat="server"  />
                                                                    <asp:HiddenField ID="hdnNotify" Value='<%# getVal(DataBinder.Eval(Container, "DataItem.notify")) %>' runat="server" />
                                                                    <asp:HiddenField ID="hdnNotification" Value='<%# getVal(DataBinder.Eval(Container, "DataItem.notification")) %>' runat="server" />
                                                                    <asp:HiddenField ID="hdnConfigurable" Value='<%# getVal(DataBinder.Eval(Container, "DataItem.configurable")) %>' runat="server" />
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:BoundColumn Visible="False" DataField="codice" HeaderText="Codice"></asp:BoundColumn>
														</Columns>
													</asp:datagrid>
												</DIV>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<!-- FINE CORPO CENTRALE --></td>
					</TD></tr>
			</table>
		</form>
	</body>
</HTML>
