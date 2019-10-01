<%@ Page language="c#" Codebehind="ListeDistr.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.RubricaDocsPA.ListeDistr" validateRequest="false"%>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<LINK href="../CSS/rubrica.css" type="text/css" rel="stylesheet">
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
		</script>
	</HEAD>
	<body bgColor="#eaeaea">
		<form id="Form1" method="post" runat="server">
    		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Liste" />
			<table width="100%">
				<tr>
					<td><asp:panel id="Panel1" runat="server">
							<TABLE width="100%">
								<TR>
									<TD width="50%">
										<asp:Label id="Label1" runat="server" CssClass="testo_grigio_scuro" Font-Bold="True">Liste di distribuzione</asp:Label></TD>
									<td align="right"><asp:ImageButton ID="help" runat="server" AlternateText="Aiuto?" SkinID="btnHelp" OnClientClick="OpenHelp('GestioneListe')" /></td>
								</TR>
								<TR>
									<TD colspan="2">
										<DIV id="DivDGElencoListe" style="BORDER-TOP: black 2px solid; OVERFLOW: auto; HEIGHT: 184px"
											runat="server"><BR>
											<asp:DataGrid id="dg_1" SkinID="datagrid" runat="server" Width="100%" AutoGenerateColumns="False" PageSize="6" AllowPaging="True">
												<SelectedItemStyle BackColor="RosyBrown"></SelectedItemStyle>
												<AlternatingItemStyle BackColor="#F2F2F2"></AlternatingItemStyle>
												<ItemStyle Font-Size="8pt" Font-Names="Arial"></ItemStyle>
												<HeaderStyle CssClass="menu_1_bianco"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="ID_LISTA"></asp:BoundColumn>
													<asp:BoundColumn DataField="VAR_DESC_CORR" HeaderText="Lista">
														<HeaderStyle Width="80%"></HeaderStyle>
													</asp:BoundColumn>
													<asp:ButtonColumn Text="&lt;img src=../images/proto/lentePreview.gif border=0 alt='Selezione'&gt;"
														HeaderText="Seleziona" CommandName="Select">
														<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
													<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="" HeaderText="Elimina" CancelText="" EditText="&lt;img src=../images/proto/b_elimina.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;">
														<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:EditCommandColumn>
												</Columns>
												<PagerStyle Font-Size="XX-Small" Font-Bold="True" HorizontalAlign="Center" ForeColor="Black"
													Mode="NumericPages"></PagerStyle>
											</asp:DataGrid></DIV>
									</TD>
								</TR>
							</TABLE>
						</asp:panel></td>
				</tr>
				<tr>
					<td><asp:panel id="Panel2" runat="server">
							<TABLE width="100%">
								<TR>
									<TD width="15%">
										<asp:Label id="Label4" runat="server" CssClass="testo_grigio_scuro">Codice Lista :</asp:Label></TD>
									<TD width="75%" colspan="2">
										<asp:TextBox id="txt_codiceLista" runat="server" CssClass="testo_grigio" Width="20%"></asp:TextBox>
										<asp:Label id="Label3" runat="server" CssClass="testo_grigio_scuro" Font-Bold="True">Nome Lista :</asp:Label>
										<asp:TextBox id="txt_nomeLista" runat="server" CssClass="testo_grigio" Width="60%"></asp:TextBox></TD>
								</TR>
								<TR>
									<TD style="HEIGHT: 20px" width="15%">
										<asp:Label id="Label2" runat="server" CssClass="testo_grigio_scuro" Font-Bold="True">Codice Corr :</asp:Label></TD>
									<TD style="HEIGHT: 20px" width="75%">
										<asp:TextBox id="txt_codiceCorr" runat="server" CssClass="testo_grigio" Width="20%" AutoPostBack="True"></asp:TextBox>
										<asp:TextBox id="txt_descrizione" runat="server" CssClass="testo_grigio" Width="78%"></asp:TextBox></TD>
									<TD style="HEIGHT: 20px" width="10%">
										<asp:ImageButton id="imgBtn_addCorr" runat="server" ImageUrl="../images/proto/aggiungi.gif"></asp:ImageButton>
										<asp:ImageButton id="imgBtn_descrizione" runat="server" ImageUrl="../images/proto/rubrica.gif"></asp:ImageButton></TD>
								</TR>
								<TR>
									<TD colSpan="3">
										<DIV id="DivDGCorrispondenti" style="BORDER-TOP: black 2px solid; OVERFLOW: auto; HEIGHT: 184px"
											runat="server"><BR>
											<asp:DataGrid id="dg_2" SkinID="datagrid" runat="server" Width="100%" AutoGenerateColumns="False" PageSize="6" AllowPaging="True">
												<AlternatingItemStyle BackColor="#F2F2F2"></AlternatingItemStyle>
												<ItemStyle Font-Size="8pt" Font-Names="Arial"></ItemStyle>
												<HeaderStyle CssClass="menu_1_bianco"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="ID_DPA_CORR" HeaderText="SystemIdCorr"></asp:BoundColumn>
													<asp:TemplateColumn HeaderText="Descrizione">
                                                        <HeaderStyle Width="90%"></HeaderStyle>
                                                        <ItemTemplate>
                                                        <asp:Label ID="lblText" runat="server" Text="<%# this.GetText((System.Data.DataRowView)Container.DataItem) %>" ForeColor="<%# this.GetForeColor((System.Data.DataRowView)Container.DataItem) %>" />
                                                            </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:EditCommandColumn ButtonType="LinkButton" UpdateText="" HeaderText="Elimina" CancelText="" EditText="&lt;img src=../images/proto/b_elimina.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;">
														<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:EditCommandColumn>
												</Columns>
												<PagerStyle Font-Size="XX-Small" Font-Bold="True" HorizontalAlign="Center" ForeColor="Black"
													Mode="NumericPages"></PagerStyle>
											</asp:DataGrid></DIV>
									</TD>
								</TR>
							</TABLE>
						</asp:panel>
						<INPUT id="txt_confirmDel" type="hidden" runat="server" NAME="txt_confirmDel">&nbsp;
						<INPUT id="txt_confirmMod" type="hidden" runat="server" NAME="txt_confirmMod">
					</td>
				</tr>
				
			</table>
				<table width="100%">
				    <asp:Panel ID="Panel3" runat="server">
				    <div id="Div1" style="BORDER-TOP: black 2px solid; OVERFLOW: auto; HEIGHT: 3px" runat="server">
    			    <tr>
					    <td><asp:Label id="Label5" runat="server" CssClass="testo_grigio_scuro" Font-Bold="True">Rendi Disponibile</asp:Label>
					        <asp:radiobuttonlist id="rbl_share" RepeatColumns="2" RepeatDirection="Horizontal" tabIndex="3" runat="server" Width="100%" CssClass="testo_grigio" Height="100%">
					        <asp:ListItem Value="usr" Selected="True">solo a me stesso (@usr@)</asp:ListItem>
					        <asp:ListItem Value="grp">a tutto il ruolo (@grp@)</asp:ListItem>
					        </asp:radiobuttonlist>
					    </td>
				    </tr>
			        </div>
				    </asp:Panel>				    
					<tr>
						<td align="right">
						    <br />
							<asp:ImageButton id="btn_nuova" runat="server" SkinID="nuova_Attivo"></asp:ImageButton>
							<asp:ImageButton id="btn_salva" runat="server" SkinID="salvaL"></asp:ImageButton>
							<asp:ImageButton id="btn_annulla" runat="server" SkinID="btnAnnulla"></asp:ImageButton>&nbsp;
						</td>
					</tr>
				</table>
		</form>
	</body>
</HTML>
