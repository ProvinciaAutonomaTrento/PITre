<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="ListeDistr.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_ListeDistr.ListeDistr" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
		<script language="javascript">
		function _ApriRubrica()
		{
			var r = new Rubrica();
			r.CallType = r.CALLTYPE_LISTE_DISTRIBUZIONE;			
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
		
		function confirmMod()
		{
			if(document.getElementById("txt_confirmMod").value == "si")
			{
				var agree=confirm("Ci sono delle modifiche da salvare ! Si vuole procedere comunque ?");
				if (agree)
				{
					document.getElementById("txt_confirmMod").value = "";
					return true ;
				}
				else
				{
					document.getElementById("txt_confirmMod").value = "si";					
				}
			}			
		}
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Liste Distribuzione" />
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
					<td class="titolo" style="HEIGHT: 20px" align="center" bgColor="#e0e0e0" height="34">Liste 
						distribuzione</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table cellSpacing="0" cellPadding="0" align="center" border="0">
							<tr>
								<td align="center" height="25"><asp:label id="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
							</tr>
							<tr>
								<td class="pulsanti" align="center">
									<table width="900">
										<tr>
											<td align="left"><asp:label id="lbl_titolo" runat="server" CssClass="titolo">Liste distribuzione</asp:label></td>
											<td align="right">
												<asp:Button id="btn_salva" runat="server" Text="Salva" CssClass="testo_btn_p"></asp:Button>&nbsp;
												<asp:Button id="btn_nuova" runat="server" Text="Nuova" CssClass="testo_btn_p"></asp:Button>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<!-- INIZIO: PANNELLI -->
							<tr>
								<td><br>
									<asp:panel id="Panel1" runat="server">
										<DIV id="DivDGElencoListe" runat="server">
											<asp:DataGrid id="dg_1" runat="server" Width="100%" AutoGenerateColumns="False" PageSize="6" AllowPaging="True" OnItemCreated="dg_1_ItemCreated">
												<SelectedItemStyle BackColor="RosyBrown"></SelectedItemStyle>
												<AlternatingItemStyle BackColor="#F2F2F2"></AlternatingItemStyle>
												<ItemStyle Font-Size="8pt" Font-Names="Arial"></ItemStyle>
												<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="ID_LISTA"></asp:BoundColumn>
													<asp:BoundColumn DataField="VAR_DESC_CORR" HeaderText="Lista">
														<HeaderStyle Width="80%"></HeaderStyle>
													</asp:BoundColumn>
													<asp:ButtonColumn Text="&lt;img src=../images/lentePreview.gif border=0 alt='Selezione'&gt;" HeaderText="Seleziona"
														CommandName="Select">
														<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
													<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="" HeaderText="Elimina" CancelText="" EditText="&lt;img src=../images/cestino.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;">
														<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:EditCommandColumn>
												</Columns>
												<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
											</asp:DataGrid></DIV>
									</asp:panel>
								</td>
							</tr>
							<tr>
								<td><br>
									<asp:panel id="Panel2" runat="server">
										<TABLE width="100%">
											<TR>
												<TD width="15%">
													<asp:Label id="Label4" runat="server" CssClass="testo_grigio_scuro">Codice Lista *:</asp:Label></TD>
												<TD width="75%">
													<asp:TextBox id="txt_codiceLista" runat="server" CssClass="testo" Width="20%"></asp:TextBox>
													<asp:Label id="Label3" runat="server" CssClass="testo_grigio_scuro" Font-Bold="True">Nome Lista :</asp:Label>
													<asp:TextBox id="txt_nomeLista" runat="server" CssClass="testo" Width="67%"></asp:TextBox></TD>
												<TD width="10%"></TD>
											</TR>
											<TR>
												<TD style="HEIGHT: 20px" width="15%">
													<asp:Label id="Label2" runat="server" CssClass="testo_grigio_scuro" Font-Bold="True">Codice Corr *:</asp:Label></TD>
												<TD style="HEIGHT: 20px" width="75%">
													<asp:TextBox id="txt_codiceCorr" runat="server" CssClass="testo" Width="20%" AutoPostBack="True"></asp:TextBox>
													<asp:TextBox id="txt_descrizione" runat="server" CssClass="testo" Width="78%"></asp:TextBox></TD>
												<TD style="HEIGHT: 20px" width="10%">
													<asp:ImageButton id="imgBtn_addCorr" runat="server" ImageUrl="../Images/aggiungi.gif"></asp:ImageButton>
													<asp:ImageButton id="imgBtn_descrizione" runat="server" ImageUrl="../../images/proto/rubrica.gif"></asp:ImageButton></TD>
											</TR>
											<TR>
												<TD colSpan="3">
													<DIV id="DivDGCorrispondenti" runat="server"><BR>
														<asp:DataGrid id="dg_2" runat="server" Width="100%" AutoGenerateColumns="False" PageSize="6" AllowPaging="True" OnItemCreated="dg_2_ItemCreated">
															<AlternatingItemStyle BackColor="#F2F2F2"></AlternatingItemStyle>
															<ItemStyle Font-Size="8pt" Font-Names="Arial"></ItemStyle>
															<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
															<Columns>
																<asp:BoundColumn Visible="False" DataField="ID_DPA_CORR" HeaderText="SystemIdCorr"></asp:BoundColumn>
																<asp:TemplateColumn HeaderText="Descrizione">
                                                                    <HeaderStyle Width="90%"></HeaderStyle>
                                                                    <ItemTemplate>
                                                                    <asp:Label ID="lblText" runat="server" Text="<%# this.GetText((System.Data.DataRowView)Container.DataItem) %>" ForeColor="<%# this.GetForeColor((System.Data.DataRowView)Container.DataItem) %>" />
                                                                        </ItemTemplate>
                                                                </asp:TemplateColumn>
																<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="" HeaderText="Elimina" CancelText="" EditText="&lt;img src=../images/cestino.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;">
																	<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
																	<ItemStyle HorizontalAlign="Center"></ItemStyle>
																</asp:EditCommandColumn>
															</Columns>
															<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
														</asp:DataGrid></DIV>
												</TD>
											</TR>
										</TABLE>
									</asp:panel>
									<INPUT id="txt_confirmDel" type="hidden" runat="server" NAME="txt_confirmDel" value="">&nbsp;
									<INPUT id="txt_confirmMod" type="hidden" runat="server" NAME="txt_confirmMod" value="">
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
