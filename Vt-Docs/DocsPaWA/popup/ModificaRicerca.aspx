<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="modificaRicerca.aspx.cs"
    Inherits="DocsPAWA.popup.modificaRicerca" %>

<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="HEAD1" runat="server">
    <title>Modifica una ricerca salvata</title>
    <link href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
    <base target="_self">
    <script language="javascript">
        function saveOk() {
            window.returnValue = 'ok';
        }		
    </script>
</head>
<body ms_positioning="GridLayout" onload="document.all('txt_titolo').focus();">
    <form id="Form1" method="post" runat="server">
    <table class="info" id="Table1" style="z-index: 101; left: 40px; width: 408px; position: absolute;
        top: 8px; height: 208px" height="208" width="408" align="center" border="0">
        <tr>
            <td class="item_editbox" height="20" align="left">
                <p class="boxform_item">
                    <asp:Label ID="lblTitle" runat="server" Text="Modifica una ricerca salvata"></asp:Label></p>
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <table id="Table2" cellspacing="2" cellpadding="0" width="97%" border="0">
                    <tr>
                        <td class="titolo_scheda" style="height: 9px">
                            Titolo&nbsp;*
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_titolo" runat="server" CssClass="testo_grigio" Width="383px"
                                TabIndex="2" MaxLength="64"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td height="3">
                        </td>
                    </tr>
                    <tr>
                        <td class="titolo_scheda">
                            Rendi disponibile
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:RadioButtonList ID="rbl_share" runat="server" CssClass="testo_grigio" Width="368px"
                                TabIndex="3" AutoPostBack="true">
                                <asp:ListItem Value="usr">solo a me stesso (@usr@)</asp:ListItem>
                                <asp:ListItem Value="grp">a tutto il ruolo (@grp@)</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="titolo_scheda">
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_uff_ref" runat="server" Visible="false">
                    </asp:Panel>
                    <asp:Panel ID="pnl_griglie_custom" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda">
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="height: 9px">
                                Associa la ricerca ad una mia griglia
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddl_ric_griglie" runat="server" CssClass="testo_grigio" Width="383px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr align="center">
                        <td>
                            <table id="Table3">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btn_salva" runat="server" CssClass="pulsante_hand" Text="Salva" TabIndex="1" OnClientClick="saveOk();">
                                        </asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_annulla" runat="server" CssClass="pulsante_hand" Text="Chiudi">
                                        </asp:Button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <cc1:MessageBox ID="MessageBox1" Style="z-index: 102; left: 320px; position: absolute;
        top: 280px" runat="server"></cc1:MessageBox>
    </form>
</body>
</html>
