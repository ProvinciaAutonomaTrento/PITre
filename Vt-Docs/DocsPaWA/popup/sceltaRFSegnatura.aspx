<%@ Page language="c#" Codebehind="sceltaRFSegnatura.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.sceltaRFSegnatura" %>
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
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Seleziona RF" />
<TABLE  class="info" id="Table1" width="480px" align="center" border="0">
    <tr >
        <td class="item_editbox">
            <P class="boxform_item">
                <asp:label id="lb_title" runat="server" Visible="True"></asp:label>
            </P>
        </td>
    </tr>
    <tr><td class="testo_grigio" height="10">&nbsp;</td></tr>
    <tr>
        <td align="center">
            <div id="divRF" runat="server">
                <asp:label id="lbl_doc_rf" runat="server" CssClass="testo_red" Font-Size="X-Small" Visible="True"></asp:label>
                <br /><br />
                <asp:DropDownList ID="ddl_regRF" runat="server" OnSelectedIndexChanged="ddl_regRF_IndexChanged" Visible="true" CssClass="testo_grigio" Width="400px" AutoPostBack="true"></asp:DropDownList>
                <br />
            </div>
            <div id="divCaselle" runat="server">
                <asp:label id="lblMailAddress" runat="server" CssClass="testo_red" Font-Size="X-Small" Visible="True">Elenco caselle Registro/RF</asp:label>
                <br /><br />
                <asp:DropDownList ID="ddlCaselle" runat="server" Visible="true" CssClass="testo_grigio" Width="400px"></asp:DropDownList>
                <br />
            </div>
        </td>
    </tr>
    <tr ><td class="testo_grigio" height="60" >&nbsp;</td></tr>
	<tr>
		<td align="center"><asp:button id="Btn_ok" runat="server" CssClass="PULSANTE" Text="Conferma"></asp:button>&nbsp;&nbsp;
		<asp:button id="Btn_annulla" runat="server" CssClass="PULSANTE" Text="Annulla"></asp:button></td>
	</tr>
</TABLE>
</FORM>
	</body>
</HTML>
