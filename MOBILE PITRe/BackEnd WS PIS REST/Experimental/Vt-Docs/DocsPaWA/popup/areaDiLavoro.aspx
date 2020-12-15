<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="NavigationContext" Src="../SiteNavigation/NavigationContext.ascx" %>
<%@ Page language="c#" Codebehind="areaDiLavoro.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.areaDiLavoro" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
			// Script eseguito in fase di cambio pagina griglia
			function WaitGridPagingAction()
			{
				window.document.body.style.cursor="wait";
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
								
			}							
		</script>
	</HEAD>
	<body onblur="self.focus()" MS_POSITIONING="GridLayout">
		<form id="areaDiLavoro" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Area di lavoro - Documenti" />
		    <TABLE class="info" id="Table1" width="618" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Area di lavoro - Documenti</asp:label></P>
					</td>
					<td align="right" class="item_editbox">
					<P class="boxform_item"><asp:ImageButton ID="help" runat="server" AlternateText="Aiuto?" SkinID="btnHelp" OnClientClick="OpenHelp('GestioneADL')" /></P></td>
				</TR>
				<tr>
					<td align="right" colspan="2"></td>
				</tr>
				<TR>
					<TD align="center" colspan="2"><asp:label id="lbl_message" runat="server" Height="20px" Visible="False" Width="510px" CssClass="testo_msg_grigio"></asp:label></TD>
				<tr>
					<TD width="48%" height="15" colspan="2"><asp:panel id="pnl_ADL" Visible="True" Runat="server">
							<TABLE>
								<TR>
									<TD class="titolo_scheda"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Elimina 
										tutti Da Area Di Lavoro&nbsp;&nbsp;</TD>
									<TD>
										<asp:ImageButton id="btn_deleteAllADL" Runat="server" ImageUrl="../images/proto/cancella.gif"></asp:ImageButton></TD>
								</TR>
							</TABLE>
						</asp:panel></TD>
				</tr>
				<TR vAlign="top">
					<TD align="center" colspan="2"><asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" Width="96%" AutoGenerateColumns="False" BorderWidth="1px"
							AllowPaging="True" CellPadding="1" BorderColor="Gray" AllowCustomPaging="True" OnItemCreated="DataGrid1_ItemCreated1">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Data">
									<HeaderStyle HorizontalAlign="Center" Width="80px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Segnatura / ID Doc">
									<HeaderStyle HorizontalAlign="Center" Width="180px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Oggetto">
									<HeaderStyle HorizontalAlign="Center" Width="250px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Tipo">
									<HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Center" Width="25px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<cc1:ImageButton id="ImageButton1" runat="server" ImageUrl="../images/proto/ico_riga.gif" BorderWidth="0px"
											CommandName="Select" AlternateText="Seleziona"></cc1:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="chiave">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderTemplate>
										<asp:Label id="Label2" runat="server" CssClass="menu_1_bianco_dg">Dettagli</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:ImageButton id="Imagebutton2" runat="server" BorderColor="#404040" BorderWidth="1px" AlternateText="Dettagli"
											ImageUrl="../images/proto/dettaglio.gif" CommandName="Update"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="dataAnnullamento"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="VIS">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderTemplate>
										<asp:Label id="lbl_Vis" runat="server" CssClass="menu_1_bianco_dg">Vis.</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:ImageButton id=IMG_VIS runat="server" ImageUrl="../images/proto/dett_lente_doc.gif" CommandName="VisDoc" ToolTip="Visualizza immagine documento">
										</asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
						<P></P>
						<asp:label id="pageNumberLabel" runat="server" Visible="false"></asp:label>
						<P></P>
					</TD>
				</TR>
				<TR height="30">
					<TD align="center" height="30" colspan="2"><asp:button id="btn_elimina" runat="server" Visible="False" CssClass="PULSANTE" Text="ELIMINA"></asp:button>&nbsp;
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button></TD>
				</TR>
			</TABLE>
			<uc1:DataGridPagingWait id="DataGridPagingWait1" runat="server"></uc1:DataGridPagingWait>
			<asp:TextBox id="txtIdRegistro" style="Z-INDEX: 101; LEFT: 104px; POSITION: absolute; TOP: 544px"
				runat="server" Visible="False">contiene IDREGISTRO</asp:TextBox>
		</form>
	</body>
</HTML>
