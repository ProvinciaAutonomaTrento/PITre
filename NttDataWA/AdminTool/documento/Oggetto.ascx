<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Oggetto.ascx.cs" Inherits="SAAdminTool.documento.Oggetto" %>
<%@ Register src="../ActivexWrappers/SpellWrapper.ascx" tagname="SpellWrapper" tagprefix="uc1" %>
<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
<table id="tbl_oggettario" cellSpacing="0" cellPadding="0" width="100%" border="0">
    <tr>
        <td align="left"><%=blankSpace%>
            <asp:TextBox ID="txt_cod_oggetto" CssClass="testo_grigio" runat="server" 
                Width="75px" ontextchanged="txt_cod_oggetto_TextChanged"></asp:TextBox>
            <asp:textbox id="txt_oggetto" CssClass="testo_grigio" Runat="server" 
                Width="265px" Height="30px" TextMode="MultiLine" ontextchanged="txt_oggetto_TextChanged"></asp:textbox>
            <uc1:SpellWrapper ID="SpellWrapper1" runat="server" />
        </td>
    </tr>
    <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
</table>
<script type="text/javascript" language="javascript">
    //funzione per invocare il controllo ortografico sulla textbox dell'Oggetto
    //la funzione restituirà come output il testo con le relative correzioni
    //ortografiche nella textbox Oggetto
    function SpellCheck(myText)
    {
        var correct = SpellWrapper_Spell(myText);
        document.all.<%=txt_oggetto.ClientID%>.value = correct;
    }
</script>
