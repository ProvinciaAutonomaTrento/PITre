<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="listaDocInRisposta.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.listaDocInRisposta" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<%Response.Expires=-1;%>
		<base target="_self">
	</HEAD>
	<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
		<form id="rispAlProto" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" />
			<TABLE class="contenitore" id="tbl_container" width="578" align="center" border="0" height="580">
				<TR>
					<td align="center" height="20">
					    <asp:Label id="lbl_text" class="menu_1_rosso" runat="server"></asp:Label>
					</td>
				</TR>
				<tr>
					<td align="center" height="19" style="HEIGHT: 19px"><asp:Label ID="lbl_titolo" Runat="server" CssClass="menu_1_rosso"></asp:Label></td>
				</tr>
				<TR>
					<TD align="center" style="HEIGHT: 14px"><asp:label id="lbl_message" runat="server" Height="8px" Visible="False" Width="329px" CssClass="testo_grigio_scuro"></asp:label></TD>
				<TR>
				<TR vAlign="top">
					<TD align="center"><DIV id="DivDataGrid" style="OVERFLOW: auto; WIDTH: 560px; HEIGHT: 470px">
                    <asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" AutoGenerateColumns="False" BorderWidth="1px" AllowPaging="True"
								CellPadding="1" BorderColor="Gray" AllowCustomPaging="True">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Doc. Data">
										<HeaderStyle Width="10px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' Font-Bold="True" ForeColor="Red">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Registro">
										<HeaderStyle Width="30px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>' >
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Oggetto">
										<HeaderStyle Width="250px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Destinatario">
										<HeaderStyle Width="30px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittDest") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittDest") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Tipo">
										<HeaderStyle Width="30px"></HeaderStyle>
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
										<HeaderStyle HorizontalAlign="Center" Width="50px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<HeaderTemplate>
											<asp:Label id="Label2" runat="server" CssClass="menu_1_bianco_dg">Dett</asp:Label>
										</HeaderTemplate>
										<ItemTemplate>
											<asp:ImageButton id="Imagebutton2" runat="server" BorderColor="#404040" BorderWidth="1px" AlternateText="Dettagli"
												ImageUrl="../images/proto/dettaglio.gif" CommandName="Select"></asp:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="chiave">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:ImageButton id="img_canc" runat="server" BorderColor="#404040" ToolTip="Elimina il collegamento"
												CommandName="delete" ImageUrl="../images/proto/RispProtocollo/cestino.gif"></asp:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn Visible="False" HeaderText="Segnatura">
									<ItemTemplate>
										<asp:Label id=Label8 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox8 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
									Mode="NumericPages"></PagerStyle>
							</asp:datagrid></DIV>
					</TD>
				</TR>
				<TR height="30">
					<TD align="center" height="30"><asp:button id="btn_chiudi" runat="server" Visible="True" CssClass="PULSANTE" Text="CHIUDI"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
