<%@ Page language="c#" Codebehind="regElenco.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.gestione.registro.regElenco" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" id="btn_stampaRegistro_click" for="btn_stampaRegistro" event="onclick()">
				window.document.body.style.cursor='wait';
			
			WndWaitStampaReg();
		</script>
		<script language="javascript" id="btn_cambiaStatoReg_click" for="btn_cambiaStatoReg" event="onclick()">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<BODY leftMargin="1" MS_POSITIONING="GridLayout">
		<FORM id="regElenco" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione registri" />
			<TABLE id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="408" align="center"
				border="0">
				
				<tr valign="top" height="70%">
					<td valign=top>
						<TABLE width="100%" height=100% class="info" cellSpacing="1" cellPadding="1" align="center" border="0">
							<TR height="20px">
								<TD class="item_editbox"><P class="boxform_item">Gestione registri</P>
								</TD>
							</TR>
							<tr>
								<td height="5" valign=top></td>
							</tr>
							
							<TR>
								<TD align="center" width="412" valign=Top>
									<DIV id="DivDGList" style="OVERFLOW: auto; WIDTH: 98%; HEIGHT: 80px">
									<asp:datagrid id="DataGrid2" SkinID="datagrid" runat="server" Width="98%" BorderColor="Gray" CellPadding="1" AllowPaging="False"
										BorderWidth="1px" AutoGenerateColumns="False" OnItemCreated="Grid2_OnItemCreated"  OnSelectedIndexChanged="DataGrid2_SelectedIndexChanged">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
										<asp:TemplateColumn HeaderText="Descrizione">
												<HeaderStyle Width="280px"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=Textbox4 runat="server" Width="150px" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
										<asp:TemplateColumn HeaderText="Codice">
												<HeaderStyle Width="30px"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Stato">
												<HeaderStyle Width="25px"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
												<ItemTemplate>
													<asp:Label id=LabelStato runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.img_stato") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.img_stato") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
										<asp:TemplateColumn>
												<HeaderStyle Width="18px"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												<ItemTemplate>
													<asp:ImageButton id="ImageButton1" runat="server" Width="18px" BorderWidth="1px" ImageUrl="../../images/proto/dettaglio.gif"
														AlternateText="Dettaglio" BorderStyle="None" Height="16px" CommandName="Select"></asp:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										<asp:TemplateColumn Visible=False HeaderText="chiave">
												<ItemTemplate>
													<asp:Label id=lblSys runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="sys">
												<ItemTemplate>
													<asp:Label id=Label5 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="false" HeaderText="sospeso">
											    <ItemTemplate>
													<asp:Label id="idSosp" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.sospeso") %>'>
													</asp:Label>
											    </ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
											<PagerStyle Width="350px" VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#c2c2c2"
											CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
								   </asp:datagrid>
								   </DIV>
								</TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr height="7%">
					<td>
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" border="0" align="center">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<TD><cc1:imagebutton id="btn_cambiaStatoReg" DisabledUrl="~/App_Themes/ImgComuni/btn_cambiastato_nonattivo.gif"
										Runat="server" AlternateText="Cambia stato" Thema="btn_" SkinID="cambiastato_attivo" 
										Tipologia="GEST_REG_C_STATO"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_casellaIstituzionale" DisabledUrl="~/App_Themes/ImgComuni/btn_casella_nonattivo.gif"
										Runat="server" AlternateText="Elabora dati casella Istituzionale" Thema="btn_" SkinID="casella_attivo" 
										Tipologia="GEST_CASELLA_IST"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_modificReg" DisabledUrl="~/App_Themes/ImgComuni/btn_modifica_nonattivo.gif"
										Runat="server" AlternateText="Modifica dati registro" Thema="btn_" SkinID="modifica_attivo" 
										Tipologia="GEST_REG_MODIFICA"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_stampaRegistro" DisabledUrl="~/App_Themes/ImgComuni/btn_stampa_nonattivo.gif"
										Runat="server" AlternateText="Registro di protocollo" Thema="btn_" SkinID="stampa_Attivo" 
										Tipologia="GEST_REG_STAMPA"></cc1:imagebutton></TD>
							</TR>
							<!--TR>
								<TD width="100%" bgColor="#9e9e9e" colSpan="14"><IMG height="5" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
							</TR-->
						</TABLE> <!--FINE BOTTONIERA -->
					</td>
				</tr>
			</TABLE>
		</FORM>
	</BODY>
</HTML>
