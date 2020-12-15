<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="ProponiClassifica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.ProponiClassifica" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title></title>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="../CSS/docspa_30.css" type=text/css rel=stylesheet >
  </HEAD>
<body onblur=self.focus() MS_POSITIONING="GridLayout">
<form id=areaDiLavoro method=post runat="server">
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Proponi Classifica" />
<TABLE class=info id=Table1 width=553 align=center border=0 style="WIDTH: 553px; HEIGHT: 188px">
  <TR>
    <td class=item_editbox align=center>
      <P class=boxform_item><asp:datagrid id=DataGrid1 runat="server" SkinID="datagrid" BorderColor="Gray" BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False" Width="501px">
<SelectedItemStyle CssClass="bg_grigioS">
</SelectedItemStyle>

<AlternatingItemStyle CssClass="bg_grigioA">
</AlternatingItemStyle>

<ItemStyle CssClass="bg_grigioN">
</ItemStyle>

<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon">
</HeaderStyle>

<Columns>
<asp:TemplateColumn HeaderText="Classifica">
<HeaderStyle HorizontalAlign="Center" Width="80px">
</HeaderStyle>

<ItemTemplate>
<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.var_codice") %>'></asp:Label>
</ItemTemplate>

<EditItemTemplate>
<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.id") %>'>
											</asp:TextBox>
</EditItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn HeaderText="Descrizione">
<HeaderStyle HorizontalAlign="Center" Width="180px">
</HeaderStyle>

<ItemTemplate>
											<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.collName") %>'>
											</asp:Label>
										
</ItemTemplate>

<EditItemTemplate>
											<asp:TextBox id=TextBox4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.collName") %>'>
											</asp:TextBox>
										
</EditItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn HeaderText="Rilevanza">
<HeaderStyle HorizontalAlign="Center" Width="250px">
</HeaderStyle>

<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.docRel") %>' ID="Label1">
											</asp:Label>
										
</ItemTemplate>

<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.docRel") %>' ID="Textbox2">
											</asp:TextBox>
										
</EditItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn>
<HeaderStyle HorizontalAlign="Center" Width="25px">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center">
</ItemStyle>

<ItemTemplate>
<cc1:ImageButton id=ImageButton1 runat="server" BorderWidth="0px" CommandName="Select" AlternateText="Seleziona" ImageUrl="../images/proto/ico_riga.gif"></cc1:ImageButton>
</ItemTemplate>
</asp:TemplateColumn>
</Columns>
							</asp:datagrid></P><asp:label id=lb runat="server" Visible="False" Font-Names="Verdana" Font-Size="12pt" ForeColor="Red"></asp:label></td></TR>
  <TR>
  <TR height=30>
    <TD align=center height=30>&nbsp; <asp:button id=btn_ok runat="server" Visible="False" CssClass="PULSANTE" Text="OK" Width="59px"></asp:button>&nbsp; 
<asp:button id=btn_chiudi runat="server" CssClass="PULSANTE" Text="CHIUDI" Visible="False"></asp:button></TD></TR></TABLE></form>


	</body>
</HTML>
