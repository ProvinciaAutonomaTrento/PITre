<%@ Page language="c#" Codebehind="areaDiLavoroFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.areaDiLavoroFasc" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server" >
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
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
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Area di lavoro - Fascicoli" />
			<uc1:DataGridPagingWait id="DataGridPagingWait1" runat="server"></uc1:DataGridPagingWait>
			<TABLE class="info" id="Table1" width="618" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Area di lavoro - Fascicoli</asp:label></P>
					</td>
					<td align="right" class="item_editbox">
					<P class="boxform_item"><asp:ImageButton ID="help" runat="server" OnClientClick="OpenHelp('GestioneADL')" AlternateText="Aiuto?" SkinID="btnHelp" border="0" /></P></td>
				</TR>
				<tr>
					<td align="right" colspan="2"></td>
				</tr>
				<TR>
					<TD align="center" colspan="2"><asp:label id="lbl_message" runat="server" CssClass="testo_msg_grigio" Width="510px" Visible="True"
							Height="20px"></asp:label></TD>
				<TR>
				<tr>
					<TD width="48%" height="15" colspan="2"><asp:panel id="pnl_ADL" Visible="True" Runat="server">
							<TABLE>
								<TR>
									<TD class="titolo_scheda">
										<asp:Label id="lbl_message2" Runat="server">Elimina tutti i fascicoli da ADL</asp:Label></TD>
									<TD>
										<asp:ImageButton id="btn_deleteADL" Runat="server" ImageUrl="../images/proto/cancella.gif"></asp:ImageButton></TD>
								</TR>
							</TABLE>
						</asp:panel></TD>
				</tr>
				<TR vAlign="top">
					<td vAlign="top" align="center" colspan="2"><asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" Width="96%" AutoGenerateColumns="False" BorderWidth="1px"
							AllowPaging="True" CellPadding="1" BorderColor="Gray" OnItemCreated="DataGrid1_ItemCreated">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Stato">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Tipo">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>' ID="Label3">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>' ID="Textbox2">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Cod. Class.">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Label4">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Textbox3">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Codice">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' ID="Label8">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' ID="Textbox8">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Descrizione">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.Descrizione")) %>' ID="Label5">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>' ID="Textbox4">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Aperto">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>' ID="Label6">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>' ID="Textbox5">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Chiuso">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>' ID="Label7">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>' ID="Textbox6">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Chiave">
									<ItemTemplate>
										<asp:Label id=Label9 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox7 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Center" Width="25px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<cc1:ImageButton id="ImageButton1" runat="server" ImageUrl="../images/proto/ico_riga.gif" BorderWidth="0px"
											AlternateText="Seleziona" CommandName="Select"></cc1:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<asp:Label id="Label10" runat="server">DETTAGLI</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:ImageButton id="Imagebutton2" runat="server" BorderColor="#404040" BorderWidth="1px" AlternateText="Dettagli"
											ImageUrl="../images/proto/dettaglio.gif" CommandName="Update"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</TR>
				<TR height="30">
					<TD align="center" height="30" colspan="2">&nbsp;&nbsp;&nbsp;
						<asp:button id="btn_elimina" runat="server" CssClass="PULSANTE" Visible="False" ToolTip="Elimina il fascicolo selezionato"
							Text="ELIMINA"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
