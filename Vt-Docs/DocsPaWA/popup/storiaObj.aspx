<%@ Page language="c#" Codebehind="storiaObj.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.storiaObj" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat = "server">
	    <title></title>
<meta content="Microsoft Visual Studio 7.0" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="../CSS/docspa_30.css" type=text/css rel=stylesheet >
  </HEAD>
<body onblur=self.focus() bottomMargin=0 leftMargin=5 topMargin=5 rightMargin=5 MS_POSITIONING="GridLayout">
<form id=storiaObj method=post runat="server">
<TABLE class=info id=Table1 width=572 align=center border=0>
  <TR>
    <td class=item_editbox>
      <P class=boxform_item><asp:label id=Label2 runat="server" Width="291px" CssClass="menu_grigio_popup">Storia delle modifiche</asp:label></P></TD></TR>
  <TR vAlign=bottom>
    <TD align=center height=15><asp:label id=lb_dettagli runat="server" CssClass="testo_grigio_scuro" Visible="False"></asp:label></TD></TR>
  <TR vAlign=top>
    <TD align=center>
      <div id=div_storicoOggetto 
      style="OVERFLOW: auto; WIDTH: 560px; HEIGHT: 255px" 
      runat="server"><asp:datagrid id=DataGrid1 runat="server" SkinID="datagrid" AutoGenerateColumns="False" PageSize="8" AllowPaging="True" HorizontalAlign="Center" CellPadding="1" BorderWidth="1px" BorderColor="Gray" WIDTH="98%">
<SelectedItemStyle CssClass="bg_grigioS">
</SelectedItemStyle>

<AlternatingItemStyle CssClass="bg_grigioA">
</AlternatingItemStyle>

<ItemStyle CssClass="bg_grigioN">
</ItemStyle>

<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon">
</HeaderStyle>

<Columns>
<asp:BoundColumn DataField="Data" HeaderText="Data">
<HeaderStyle Width="11%">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo">
<HeaderStyle Width="27%">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Utente" HeaderText="Utente">
<HeaderStyle Width="22%">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="StoriaObj" HeaderText="Modifica">
<HeaderStyle Width="40%">
</HeaderStyle>
</asp:BoundColumn>
</Columns>

<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages">
</PagerStyle>
</asp:datagrid></DIV></TD></TR>
  <TR>
    <TD align=center height=3>
      <div id=div_storicoCorr 
      style="OVERFLOW: auto; WIDTH: 560px; HEIGHT: 255px" 
      runat="server"><asp:datagrid id=dg_CorrSto runat="server" SkinID="datagrid" Width="98%" AutoGenerateColumns="False" PageSize="8" AllowPaging="True" HorizontalAlign="Center" CellPadding="1" BorderWidth="1px" BorderColor="Gray" OnItemCreated="dg_CorrSto_ItemCreated">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:BoundColumn DataField="DATA" HeaderText="Data"></asp:BoundColumn>
									<asp:BoundColumn DataField="RUOLO" HeaderText="Ruolo"></asp:BoundColumn>
									<asp:BoundColumn DataField="UTENTE" HeaderText="utente"></asp:BoundColumn>
									<asp:BoundColumn DataField="MODIFICA" HeaderText="Modifica"></asp:BoundColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
									Mode="NumericPages"></PagerStyle>
							</asp:datagrid></DIV></TD></TR>
  <TR>
    <TD align=center height=3>
      <div id=div_StoricoData 
      style="OVERFLOW: auto; WIDTH: 560px; HEIGHT: 255px" 
      runat="server"><asp:datagrid id=Datagrid2 runat="server" SkinID="datagrid" Width="98%" AutoGenerateColumns="False" PageSize="8" AllowPaging="True" HorizontalAlign="Center" CellPadding="1" BorderWidth="1px" BorderColor="Gray" OnItemCreated="dg_CorrSto_ItemCreated">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
                                    <asp:BoundColumn DataField="DATA MODIFICA" HeaderText="Data Azione"></asp:BoundColumn>
									<asp:BoundColumn DataField="RUOLO" HeaderText="Ruolo"></asp:BoundColumn>
									<asp:BoundColumn DataField="UTENTE" HeaderText="utente"></asp:BoundColumn>
									<asp:BoundColumn DataField="DATA ARRIVO" HeaderText="Modifica"></asp:BoundColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
									Mode="NumericPages"></PagerStyle>
							</asp:datagrid></DIV></TD></TR>

<!--  div storico campi profilati -->
<tr>
    <td align="center">
      <div id=div_StoricoCampiProf style="OVERFLOW: auto; WIDTH: 560px; HEIGHT: 255px" 
        runat="server">
        <asp:datagrid id="DataGridStoricoCampiProf" runat="server" SkinID="datagrid" AutoGenerateColumns="False" 
        PageSize="8" AllowPaging="True" HorizontalAlign="Center" CellPadding="1" BorderWidth="1px" 
        BorderColor="Gray" WIDTH="98%">
            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></HeaderStyle>
            <Columns>
                <asp:BoundColumn DataField="DATA MODIFICA" HeaderText="Data MODIFICA">
                    <HeaderStyle Width="10%"></HeaderStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="UTENTE" HeaderText="Utente">
                    <HeaderStyle Width="25%"></HeaderStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="RUOLO" HeaderText="Ruolo">
                    <HeaderStyle Width="20%"></HeaderStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="CAMPO" HeaderText="Campo">
                    <HeaderStyle Width="25%"></HeaderStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Modifica" HeaderText="Modifica">
                    <HeaderStyle Width="20%"></HeaderStyle>
                </asp:BoundColumn>
            </Columns>
            <PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages">
            </PagerStyle>
        </asp:datagrid>
        </div>
     </td>
  </tr>
  <!--  end div storico campi profilati -->

  <TR>
    <TD height=5></TD></TR>
  <TR>
    <TD align=center><asp:button id=btn_ok runat="server" CssClass="PULSANTE" Text="Chiudi"></asp:button></TD></TR>
  <TR>
    <TD height=5></TD></TR></TABLE></FORM>
	</body>
</HTML>
