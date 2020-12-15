<%@ Page language="c#" Codebehind="confermaSpedizione.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.confermaSpedizione" %>
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
<body onblur="self.focus()" bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
<form id="storiaObj" method="post" runat="server">
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Conferma Spedizione" />
<TABLE class="info" id="Table1" width="450px" align="center" border="0">
    <tr>
        <td class="item_editbox">
            <P class="boxform_item"><asp:label id="Label2" runat="server" Width="430px" CssClass="menu_grigio_popup">Conferma spedizione</asp:label></P>
        </td>
    </tr>
    <asp:Panel ID="pnl_regRF" runat="server" Visible="false">
    <tr>
        <td align="center"><asp:label id="lbl_doc_rf" runat="server" CssClass="testo_grigio_scuro" Visible="False"></asp:label><br /><asp:DropDownList ID="ddl_regRF" runat="server" Visible="false" CssClass="testo_grigio" Width="400px"></asp:DropDownList></td>
    </tr>
    </asp:Panel>
    <tr vAlign="bottom">
        <td align="center" height="15"><asp:label id="lb_doc_spedito" runat="server" CssClass="testo_grigio_scuro" Visible="False"></asp:label></td>
    </tr>
    <tr vAlign="bottom">
        <td align="center" height="15"><asp:label id="lb_doc_con_documenti" runat="server" CssClass="testo_grigio_scuro" Visible="False"></asp:label></td>
    </tr>
	<tr>
		<td align="center"><asp:button id="Btn_ok" runat="server" CssClass="PULSANTE" Text="Conferma"></asp:button>&nbsp;&nbsp;
		<asp:button id="Btn_annulla" runat="server" CssClass="PULSANTE" Text="Annulla"></asp:button></td>
	</tr>
</TABLE>
</FORM>
	</body>
</HTML>
