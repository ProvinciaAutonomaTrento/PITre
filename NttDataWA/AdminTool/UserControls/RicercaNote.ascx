<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RicercaNote.ascx.cs"
    Inherits="SAAdminTool.UserControls.RicercaNote" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td height="25">
            <asp:RadioButtonList ID="rl_visibilita" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="DataChanged">
                <asp:ListItem Value="Q" Selected>Qualsiasi&nbsp;&nbsp;</asp:ListItem>
                <asp:ListItem Value="T">Tutti&nbsp;&nbsp;</asp:ListItem>
                <asp:ListItem Value="R">Ruolo&nbsp;&nbsp;</asp:ListItem>
                <asp:ListItem Value="F">RF&nbsp;&nbsp;</asp:ListItem>
                <asp:ListItem Value="P">Personali</asp:ListItem>
            </asp:RadioButtonList>
           
        </td>
    </tr>
    <tr valign="top">
        <td>
            &nbsp;&nbsp;
            <asp:TextBox ID="txt_note" runat="server" Width="350px" CssClass="testo_grigio" Height="40px"
                TextMode="MultiLine"></asp:TextBox>


        </td>
    </tr>
     <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
</table>
