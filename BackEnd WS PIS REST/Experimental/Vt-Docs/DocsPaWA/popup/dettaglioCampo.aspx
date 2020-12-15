<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dettaglioCampo.aspx.cs" Inherits="DocsPAWA.popup.dettaglioCampo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<META HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">
<base target="_self" />

<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet" >
    <title></title>

<style type="text/css">
.control_text{
    width:423px;
    height:310px;
    font-weight: normal; 
    font-size: 10px; 
    COLOR: #000000; 
    text-indent: 0px; 
    font-family: Verdana;
}
</style>

<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

</head>
<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
    <form id="dettaglioCampo" runat="server" method="post">
    <asp:Panel ID="pnl_no_ogg" runat="server">
        <table class="info" height="100%" width="100%" id="TABLE1">
        <tr>
        <td class=item_editbox height=20>
          <P class=boxform_item><asp:Label ID="nomePagina" runat="server"></asp:Label></P></td></tr>
        <tr><td class=testo_grigio><DIV style="OVERFLOW: auto; HEIGHT: 307px; width: 420px">
            <asp:Label ID="lblDettaglioCampo" runat="Server"></asp:Label></div>
            </td></tr>
            <tr><td align=Center><input class="pulsante_hand" onclick="window.close();" type=button value="Chiudi" name="Chiudi"></td></tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnl_ogg" runat="server" Visible="false">
    <input type = "hidden" id = "txtReturnValue" runat = "server" value = "true" />
      <table class="info" width="100%" height="100%">
      <tr>
        <td class=item_editbox height=20>
          <p class="boxform_item">
          <asp:Label ID="nomePagina2" runat="server"></asp:Label>
          </p>
          </td>
       </tr>
        <tr>
            <td><asp:TextBox ID="txt_oggetto" runat="server" TextMode="MultiLine" CssClass="control_text"></asp:TextBox> </td>
        </tr>
        <tr>
		<td align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	    </tr>
        </table>
        <table width="100%" >
        <tr>
            <td width="50%" align="right"><asp:Button class="pulsante_mini_3" type="button" Text="Salva" id="btn_salva" runat="server"></asp:Button></td>
            <td width="50%" align="left"><input class="pulsante_mini_3" onclick="window.close();" type="button" value="Chiudi" name="Chiudi_Ogg" ></input></td>
        </tr>
        </table>
    </asp:Panel>
    
    </form>
</body>
</html>
