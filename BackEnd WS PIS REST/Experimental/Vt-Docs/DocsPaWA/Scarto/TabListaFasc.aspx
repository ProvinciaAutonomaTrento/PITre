<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="TabListaFasc.aspx.cs" Inherits="DocsPAWA.Scarto.TabListaFasc" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../waiting/WaitingPanel.ascx" tagname="WaitingPanel" tagprefix="uc2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > TabListaFasc</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";
				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
			
			function btn_scarto_clientClick()
            {
                ShowWaitPanel("<%=this.WaitingPanelTitle%>");
            }
		</script>
		
		
		</HEAD>
	<body text="#660066" bottomMargin="0" vLink="#ff3366" aLink="#cc0066" link="#660066" leftMargin="0"
		topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="TabListaFasc" method="post" runat="server">
		   <table cellSpacing="0" cellPadding="0" width="100%" align="center">
				<tr>
					<td height="1"><uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait></td>
				</tr>
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
								<td id="Td2" align="left" height="80%" runat="server">
								    <asp:label id="titolo" CssClass="titolo" Runat="server"></asp:label>
								</td>
								<td class="testo_grigio_scuro" style="height: 19px" valign="middle" align="right" width="5%">
                                    <asp:ImageButton ID="btn_scarto" runat="server" ImageUrl="../images/proto/btn_scartoALL.gif" AlternateText="Inserisci tutti i fascicoli in area di scarto" OnClick="btn_scarto_Click" OnClientClick="btn_scarto_clientClick()"></asp:ImageButton>
                                </td>
								
							</tr>
						</table>
					</td>
				</tr>
				<tr>
                    <td height="2"> <asp:Label class="testo_red" ID="lbl_messaggio" runat="server" Visible=false ></asp:Label>
                    </td>
                </tr>
				<tr id="trBody" runat="server">
					<td vAlign="middle">
					    <asp:datagrid id="dg_Fasc" runat="server" SkinID="datagrid" AllowCustomPaging="True" BorderStyle="Inset" AutoGenerateColumns="False"
							Width="100%" CellPadding="1" BorderWidth="1px" BorderColor="Gray" HorizontalAlign="Center" AllowPaging="True">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn Visible="False" SortExpression="Stato" HeaderText="Stato">
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Tipo" HeaderText="Tipo">
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="CodClass" HeaderText="CodClass">
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Label3">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Textbox3">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Codice" HeaderText="Codice">
									<HeaderStyle Width="10%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Descrizione" HeaderText="Descrizione">
									<HeaderStyle Width="50%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Chiusura">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Mesi cons.">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiCons") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiCons") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Mesi chiusura">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiChiusura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiChiusura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Dett">
									<HeaderStyle Width="5px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton1" runat="server" BorderWidth="1px" BorderColor="#404040" ImageUrl="../images/proto/dettaglio.gif"
											CommandName="Select" AlternateText="Dettaglio"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Area scarto">
								    <HeaderStyle Wrap="false" HorizontalAlign="center" Width="5px"/>
								    <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"/>
								    <ItemTemplate>
								        <cc1:ImageButton ID="btn_scarto" runat="server"  ImageUrl="../images/proto/btn_scarto.gif" 
								        CommandName="AreaScarto" AlternateText="Inserisci questo fascicolo in 'Area di scarto'"/> 
								    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="false" HeaderText="in scarto">
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="lbl_inScarto" runat="server"  Text='<%# DataBinder.Eval(Container, "DataItem.InScarto") %>'>
								        </asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="false" >
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="lbl_Chiave" runat="server"  Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
								        </asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
						<uc2:WaitingPanel ID="WaitingPanel1" runat="server" />
						</td>
				</tr>
				<tr>
					<td vAlign="middle" align="center"><input id="hd1" type="hidden" value="null" name="hd1" runat="server"></td>
				</tr>
			</table>
			<uc1:DataGridPagingWait ID="dg_Fasc_pagingWait" runat="server"></uc1:DataGridPagingWait>
        </form>
	</body>
</HTML>

