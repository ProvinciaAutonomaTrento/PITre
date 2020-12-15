<%@ Page language="c#" Codebehind="visualizzaLog.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.visualizzaLog" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
	    <title></title>
<meta content="Microsoft Visual Studio 7.0" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="../CSS/docspa_30.css" type=text/css rel=stylesheet >
 <base target="_self" />
		<%Response.Expires=-1;%>
  </HEAD>

<body  bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
<form id="storiaObj" method="post" runat="server">
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Visibilità Log" />
<TABLE class="info" id="Table1" width="572" align="center" border="0">
    <TR>
        <td class="item_editbox">
            <P class="boxform_item"><asp:label id="Label2" runat="server" Width="291px" CssClass="menu_grigio_popup">Storia delle modifiche</asp:label></P>
        </TD>
    </TR>
    <tr>
        <td>
            <div style="float:right;">
                <asp:ImageButton ID="ibRoleHistory" runat="server" Width="19px" AlternateText="Mostra storia ruolo"
                    ImageUrl="../images/proto/storia.gif" />
            </div>
            <div style="clear:both;"/>
        </td>
    </tr>
    <TR vAlign="bottom">
        <TD align="center" height="15"><asp:label id="lb_dettagli" runat="server" CssClass="testo_grigio_scuro" Visible="False"></asp:label></TD>
    </TR>
    <TR vAlign="top">
        <TD align="center">
            <div id="div_storicoOggetto" style="OVERFLOW: auto; WIDTH: 560px; HEIGHT: 385px" runat="server">
            <asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" AutoGenerateColumns="False" PageSize="8" AllowPaging="True" HorizontalAlign="Center" CellPadding="1" BorderWidth="1px" BorderColor="Gray" WIDTH="98%" OnPageIndexChanged="DataGrid1_PageIndexChanged">
<SelectedItemStyle CssClass="bg_grigioS">
</SelectedItemStyle>

<AlternatingItemStyle CssClass="bg_grigioA">
</AlternatingItemStyle>

<ItemStyle CssClass="bg_grigioN">
</ItemStyle>

<HeaderStyle CssClass="menu_1_bianco_dg">
</HeaderStyle>

<Columns>
<asp:BoundColumn DataField="Data" HeaderText="Data">
<HeaderStyle Width="26%">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Utente" HeaderText="Operatore">
<HeaderStyle Width="30%">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="StoriaObj" HeaderText="Azione">
<HeaderStyle Width="44%">
</HeaderStyle>
</asp:BoundColumn>
</Columns>

<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages">
</PagerStyle>
</asp:datagrid></DIV>
        </TD>
    </TR>
	<TR>
		<TD height="2"></TD>
	</TR>
	<TR>
		<TD align="center"><asp:button id="Btn_ok" runat="server" CssClass="PULSANTE" Text="Chiudi"></asp:button></TD>
	</TR>
</TABLE>
</FORM>
	</body>
</HTML>
