<%@ Page language="c#" Codebehind="scegliFascicoloFascRapida.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.scegliFascicoloFascRapida" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<%Response.Expires=-1;%>
		<base target="_self">
		<script language="C#" runat="server">		
			void checkOPT(object sender,EventArgs e) 
			{	
				foreach (DataGridItem dgItem in this.DgListaFasc.Items)
				{		
					RadioButton optFasc=dgItem.Cells[0].FindControl("OptFasc") as RadioButton;
					if (optFasc!=null)
						optFasc.Checked=optFasc.Equals(sender);
				}
			}
			
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Fascicolazione rapida - scelta fascicolo" />
			<input type="hidden" id="hd_focus" runat="server">
			<table class="contenitore" height="400" width="575" align="center" border="0">
				<tr vAlign="middle" height="35">
					<td class="menu_1_rosso" align="center">Lista fascicoli per fascicolazione rapida</td>
				</tr>
				<tr>
					<td vAlign="top">
						<DIV id="DivDataGrid" style="OVERFLOW: auto; WIDTH: 565px; HEIGHT: 320px"><asp:datagrid id="DgListaFasc" runat="server" SkinID="datagrid" Width="97%" BorderStyle="Inset" AutoGenerateColumns="False"
								CellPadding="1" BorderWidth="1px" BorderColor="Gray" HorizontalAlign="Center" AllowPaging="True">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
								<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn>
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<asp:RadioButton id="OptFasc" runat="server" AutoPostBack="True" Visible="True" Text="" OnCheckedChanged="checkOPT"
												TextAlign="Right"></asp:RadioButton>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn SortExpression="Codice" HeaderText="Codice">
										<HeaderStyle Width="20%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Left"></ItemStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' ID="Label6">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' ID="Textbox4">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Registro">
										<HeaderStyle Width="10%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="lbl_descRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Registro") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="txt_descRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Registro") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn SortExpression="Descrizione" HeaderText="Descrizione">
										<HeaderStyle Width="45%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>' ID="Label7">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>' ID="Textbox5">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn SortExpression="Tipo" HeaderText="Tipo">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>' ID="Label4">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>' ID="Textbox2" >
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Stato">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="stato" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Apertura">
										<HeaderStyle Width="10%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>' ID="Label8" >
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>' ID="Textbox6" >
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Chiusura">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>' ID="Label9">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>' ID="Textbox7">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="Chiave">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>' ID="Label10">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>' ID="Textbox8">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
							</asp:datagrid></DIV>
					</td>
				</tr>
				<TR vAlign="top">
					<td align="center">
						<table>
							<tr vAlign="top">
								<TD align="center"><asp:panel id="pnlButtonOk" Runat="server" Visible="True">
										<asp:button id="btn_ok" runat="server" Text="OK" CssClass="PULSANTE"></asp:button>
									</asp:panel></TD>
								<td><asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button></td>
							</tr>
						</table>
					</td>
				</TR>
			</table>
		</form>
	</body>
</HTML>
