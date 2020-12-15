<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="storiaVisibilitaDocumento.aspx.cs" Inherits="DocsPAWA.popup.storiaVisibilitaDocumento" EnableEventValidation="false" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <HEAD runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self" />
		<%Response.Expires=-1;%>

	</HEAD>
    <body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="storiaVisibilitaDocumento" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Storia visibilita' " />
			<TABLE id="Table1" class="info" width="100%" align="center" border="0">
				<TR>
					<TD class="item_editbox">
                        <P class="boxform_item">
							<asp:label id="LblTitolo" runat="server">Storia visibilità </asp:label></P>
					</TD>
				</TR>
				<TR>
					<TD height="2"></TD>
				</TR>
				<TR>
					<TD align="center" valign="top">
					    <div class="div_Visibilita">
						<asp:datagrid id="DGStoria" SkinID="datagrid" Width="100%" runat="server" BorderColor="Gray" BorderWidth="1px"
							CellPadding="1" HorizontalAlign="Center" AutoGenerateColumns="False"  >
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Utente">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.utente") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" CssClass="testo_grigio" Text='<%# DataBinder.Eval(Container, "DataItem.utente") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Ruolo">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ruolo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox2 runat="server" CssClass="testo_grigio" Text='<%# DataBinder.Eval(Container, "DataItem.ruolo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								
								<asp:TemplateColumn HeaderText="Data">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Cod.Operazione">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codOperazione") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codOperazione") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Descrizione">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid>
						</div>
					</TD>
				</TR>
				<TR>
					<TD align="center" id="dettaglio" runat="server">
							<asp:label id="LblDettagli" runat="server" CssClass="testo_grigio" Visible="False"></asp:label>
				    </TD>
				</TR>
				<TR>
					<TD height="2"></TD>
				</TR>
				<TR>
					<TD align="center"><asp:button id="Btn_ok" runat="server" CssClass="PULSANTE" Text="Chiudi"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</html>
