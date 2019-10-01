<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="templateTrasmissioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.templateTrasmissioni" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		function confirmDel()
		{
			var agree=confirm("Confermi la cancellazione ?");
			if (agree)
			{
				document.getElementById("txt_confirmDel").value = "si";
				return true ;
			}			
		}
		
		</script>
	</HEAD>
	<body>
		<form id="templateTrasmissioni" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Modelli di trasmissione" />
			<TABLE class="info" id="Table1" height="100%" width="570" align="center" border="0">
				<TR height="25">
					<td class="item_editbox">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Modelli di trasmissione</asp:label></P>
					</td>
				</TR>
				<TR>
					<TD align="center"><asp:label id="lbl_message" runat="server" Visible="False" CssClass="testo_msg_grigio"></asp:label></TD>
				<TR>
					<TD vAlign="top" align="center">
					<asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" PageSize="5" AutoGenerateColumns="False" AllowPaging="True"
							BorderColor="Gray" BorderWidth="1px" CellPadding="1" Width="99%" OnItemCreated="DataGrid1_ItemCreated">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Titolo">
									<HeaderStyle Width="84%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.titolo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.titolo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="chiave">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle Width="8%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<cc1:ImageButton id="ImageButton1" runat="server" BorderWidth="0px" AlternateText="Seleziona" ImageUrl="../images/proto/ico_riga.gif"
											CommandName="Select"></cc1:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle Width="8%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<HeaderTemplate>
										<asp:Label id="Label2" runat="server" CssClass="menu_1_bianco_dg">Dettagli</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:ImageButton id="Imagebutton2" runat="server" BorderWidth="1px" BorderColor="#404040" AlternateText="Dettagli"
											ImageUrl="../images/proto/dettaglio.gif" CommandName="Update"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
						<P>
							<asp:datagrid id="Datagrid2" runat="server" SkinID="datagrid" Width="99%" CellPadding="1" BorderWidth="1px" BorderColor="Gray"
								AllowPaging="True" AutoGenerateColumns="False" PageSize="5" OnItemCreated="Datagrid2_ItemCreated">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="system_id"></asp:BoundColumn>
									<asp:BoundColumn DataField="MODELLO" HeaderText="Modello"></asp:BoundColumn>
									<asp:TemplateColumn>
										<HeaderStyle Width="8%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<cc1:ImageButton id="ImageButton1" runat="server" BorderWidth="0px" AlternateText="Seleziona" ImageUrl="../images/proto/ico_riga.gif"
												CommandName="Select"></cc1:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderStyle Width="8%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<HeaderTemplate>
											<asp:Label id="Label2" runat="server" CssClass="menu_1_bianco_dg">Dettagli</asp:Label>
										</HeaderTemplate>
										<ItemTemplate>
											<asp:ImageButton id="Imagebutton2" runat="server" BorderWidth="1px" BorderColor="#404040" AlternateText="Dettagli"
												ImageUrl="../images/proto/dettaglio.gif" CommandName="Update"></asp:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
									Mode="NumericPages"></PagerStyle>
							</asp:datagrid></P>
					</TD>
				</TR>
				<!--<TR id="tr2" runat="server">
					<TD class="titolo_scheda" id="TD2" align="center" height="10" runat="server">&nbsp;
						<asp:label id="LabelMsg" runat="server" Visible="False">Label</asp:label></TD>
				</TR>-->
				<TR>
					<td valign="top" align="center" height="100%" bgcolor="#fafafa">
						<iframe id="iFrame_cn" name="iFrame_cn" runat="server" frameborder="0" scrolling="auto"
							height="100%" width="100%" class="info"></iframe>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="center" height="30"><asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;&nbsp;&nbsp;
						<asp:button id="btn_elimina" runat="server" CssClass="PULSANTE" Text="ELIMINA"></asp:button>&nbsp;&nbsp;&nbsp;
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button></TD>
				</TR>
			</TABLE>
			<INPUT id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server">
		</form>
	</body>
</HTML>
