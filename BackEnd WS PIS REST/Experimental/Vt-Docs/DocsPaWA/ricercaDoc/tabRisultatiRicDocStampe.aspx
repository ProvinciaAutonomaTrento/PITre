<%@ Page language="c#" Codebehind="tabRisultatiRicDocStampe.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.tabRisultatiRicDocStampe" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>DOCSPA > tabRisultatiRicDocStampe</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
        
        <script language="javascript">
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";
				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="tabRisultatiRicDocStampe" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" align="center">
				<tr>
					<td height="1"><uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait></td>
				</tr>
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
								<td id="Td2" align="left" height="90%" runat="server"><asp:label id="titolo" CssClass="titolo" Runat="server"></asp:label></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr id="trBody" runat="server">
					<TD vAlign="middle"><asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" Width="100%" PageSize="20" AllowPaging="True" HorizontalAlign="Center"
							BorderColor="Gray" BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False" BorderStyle="Inset" AllowSorting="True"
							AllowCustomPaging="True" OnItemCreated="DataGrid1_ItemCreated">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Doc.">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.NumDoc") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.NumDoc") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Data">
									<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Oggetto">
									<HeaderStyle HorizontalAlign="Center" Width="95%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Oggetto") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Oggetto") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="chiave">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Registro" HeaderText="Registro">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Dett.">
									<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton1" runat="server" BorderColor="Gray" BorderWidth="1px" BorderStyle="Solid"
											ImageUrl="../images/proto/dettaglio.gif" CommandName="Select"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid></TD>
				</tr>
			</table>
		</form>
	</body>
</HTML>
