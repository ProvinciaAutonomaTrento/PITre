<%@ Page language="c#" Codebehind="datagridRubrica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.datagridRubrica" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<!--<script language="javascript">
		function MyShowModal(descCorr)
		{
			var args = new Object;
			args.window = window;
		
			rtnValue=window.showModalDialog('scegliUoUtente.aspx?win=rubrica&rubr='+descCorr,args,'dialogWidth:615px;dialogHeight:380px;status:no;resizable:no;scroll:no;dialogLeft:100;dialogTop:100;center:no;help:no;');
			if(rtnValue)
			{
				window.close();
			}
		}

		</script>-->
	</HEAD>
	<body bottomMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<!--table cellSpacing="0" cellPadding="0" width="100%" border="0">
			<tr>
				<td-->
		<form id="datagridRubrica" method="post" runat="server">
			<table class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center"
				border="0">
				<tr height="20">
					<td class="titolo_grigio" style="HEIGHT: 17px" align="center">Risultato della 
						ricerca</td>
				</tr>
				<tr width="100%">
					<td width="99%"><asp:label id="Label5" runat="server" Font-Size="Smaller" Width="472px" CssClass="titolo_grigio"
							Visible="False">Label</asp:label></td>
					<td width="1%"><INPUT id="hiddenField" type="hidden" name="hiddenField" runat="server">
						<INPUT id="hiddenField2" type="hidden" name="hiddenField" runat="server"></td>
				</tr>
				<tr>
					<td align="center" colSpan="2"><asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" Width="95%" BorderColor="Gray" AutoGenerateColumns="False"
							BorderWidth="1px" PageSize="6" CellPadding="1" AllowSorting="True">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
							<FooterStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></FooterStyle>
							<Columns>
								<asp:TemplateColumn>
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label8 runat="server" Text=' <%# GetCheckBoxLabel((string) DataBinder.Eval(Container, "DataItem.codice"), (int) DataBinder.Eval(Container, "DataItem.chiave"),(string)" " )%> '>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										&nbsp;
										<asp:Label id=Label2 runat="server" Text="<%# DataBinder.Eval(Container, &quot;DataItem.codice&quot;, &quot;<input type='radio' name='dt' value='{0}' class='testo_grigio' >&quot;) %>">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="chiave" FooterText="chiave">
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="tipo" FooterText="tipo">
									<HeaderStyle Width="10%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo", "{0}") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="descrizione" FooterText="descrizione">
									<HeaderStyle Width="70%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label6 runat="server" Text='<%# getDescrizioneECodice((string)DataBinder.Eval(Container, "DataItem.descrizione"),(string)DataBinder.Eval(Container, "DataItem.codice")) %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox5 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="codice" FooterText="codice">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>' ID="Label4">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>' ID="Textbox3">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Dettagli" FooterText="Dettagli">
									<HeaderStyle Width="10%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton1" runat="server" BorderColor="DimGray" BorderWidth="1px" CommandName="Select"
											ImageUrl="../images/proto/dettaglio.gif"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton3" runat="server" CssClass="Pulsante" ImageUrl="../images/rtl/plus.gif"
											CommandName="exp"></asp:ImageButton>
										<asp:Image id="Image1" runat="server" Visible="False" ImageUrl="../images/rtl/minus.gif"></asp:Image>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False">
									<ItemTemplate>
										<asp:Button id="Button2" runat="server" CssClass="pulsante" Text="collapse" CommandName="clp"
											CausesValidation="false"></asp:Button>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="idcont">
									<ItemTemplate>
										<asp:Label id="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idcont") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idcont") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="systemId">
									<ItemTemplate>
										<asp:Label id="Label9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="tipoIE" FooterText="tipoIE">
									<ItemTemplate>
										<asp:Label id="Label10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoIE") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="TextBox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoIE") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
				<tr>
					<td height="5"></td>
				</tr>
			</table>
		</form>
		<!--/td>
			</tr>
		</table-->
	</body>
</HTML>
