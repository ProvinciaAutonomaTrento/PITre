<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="listaSpedizioni.aspx.cs" Inherits="DocsPAWA.popup.listaSpedizioni" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
		<title></title>
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	<%Response.Expires=-1;%>
	<base target="_self">
</head>
<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Lista spedizioni" />
    <TABLE class="contenitore" id="tbl_container" width="650" align="center" border="0" height="430">
		<TR>
			<td align="center" height="20"><label id="lbl_text" class="menu_1_rosso">Spedizioni Documento</label>
			</td>
		</TR>
		<TR>
			<TD align="center" style="HEIGHT: 14px"><asp:label id="lbl_message" runat="server" Height="8px" Visible="False" Width="329px" CssClass="testo_grigio_scuro"></asp:label></TD>
		<TR>
		<TR vAlign="top" align="center">
			<TD align="center"><DIV id="DivDataGrid" style="OVERFLOW: auto"><asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" AutoGenerateColumns="False" BorderWidth="1px" AllowPaging="True"
						CellPadding="1" BorderColor="Gray" AllowCustomPaging="True" OnSelectedIndexChanged="DataGrid1_SelectedIndexChanged">
				<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
				<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
				<ItemStyle CssClass="bg_grigioN"></ItemStyle>
				<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
				<Columns>
				<asp:TemplateColumn HeaderText="Data spedizione">
				<HeaderStyle Width="10px"></HeaderStyle>
				<ItemStyle HorizontalAlign="Center"></ItemStyle>
				<ItemTemplate>
					<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' >
					</asp:Label>
				</ItemTemplate>
				<EditItemTemplate>
					<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
					</asp:TextBox>
				</EditItemTemplate>
				</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Destinatario">
										<HeaderStyle Width="250px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.destinatario") %>' ID="Label3" NAME="Label3">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.destinatario") %>' ID="Textbox3" NAME="Textbox3">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Indirizzo">
										<HeaderStyle Width="250px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.indirizzo") %>' ID="Label4" NAME="Label4">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.indirizzo") %>' ID="Textbox4" NAME="Textbox4">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Dettagli">
										<HeaderStyle Width="350px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dettagli") %>' ID="Label4" NAME="Label4">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dettagli") %>' ID="Textbox4" NAME="Textbox4">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="chiave">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>' ID="Label7" NAME="Label7">
											</asp:Label>
										</ItemTemplate>
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
</html>
