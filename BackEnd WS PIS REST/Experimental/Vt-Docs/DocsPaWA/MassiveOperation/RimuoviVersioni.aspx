<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RimuoviVersioni.aspx.cs"
    Inherits="DocsPAWA.MassiveOperation.RimuoviVersioni" MasterPageFile="~/MassiveOperation/MassiveMasterPage.Master" %>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    
</asp:Content>

<asp:Content ID="Form" runat="server" ContentPlaceHolderID="Form">
                <table ID="tblForm" class="info_grigio" cellspacing="0" cellpadding="0" width="100%"
                    align="center" border="0" runat="server" visible="true">
                    <tr>
                        <td class="titolo_scheda" valign="middle" align="center">
                            Sei sicuro di voler rimuovere le vecchie versioni e quali versioni vuoi eliminare?
                        </td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="titolo_scheda" valign="middle" align="left">
                            <asp:RadioButtonList ID="rbl_versioni" runat="server">
                               <asp:listitem id="opt_no_last" runat="server" Value="opt_no_last" Selected="True">Tutte tranne l'ultima</asp:listitem>
                               <asp:listitem id="opt_no_last_two" runat="server" Value="opt_no_last_two">Tutte tranne l'ultima e la penultima</asp:listitem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
</asp:Content>

