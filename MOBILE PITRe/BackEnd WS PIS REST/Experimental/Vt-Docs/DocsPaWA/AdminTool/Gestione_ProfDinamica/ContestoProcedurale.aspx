<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContestoProcedurale.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_ProfDinamica.ContestoProcedurale" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Associazione contesto procedurale</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <script type="text/javascript" language="javascript">

        function rbEnabled_onClick(chkEnabled) {
            // Recupero di tutti gli elementi di tipo input
            var inputElems = document.getElementsByTagName('input');
            for (i = 0; i < inputElems.length; i++) {
                if (inputElems[i].type === 'radio' && inputElems[i].checked && inputElems[i].id != chkEnabled) {
                    inputElems[i].checked = '';
                }
            }
        }

    </script>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <div id="div_listaContestoProcedurale" align="center" runat="server" style="overflow: auto;
        height: 310px; width: 100%; text-align: center">
        <asp:UpdatePanel ID="UpDgContestoProcedurale" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:DataGrid ID="dg_ContestoProcedurale" runat="server" AutoGenerateColumns="False"
                    Width="100%">
                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <asp:HiddenField ID="System_id" runat="server" Value="<%# ((DocsPAWA.DocsPaWR.ContestoProcedurale)Container.DataItem).SYSTEM_ID %>" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="TIPO_CONTESTO_PROCEDURALE" HeaderText="Tipo Contesto">
                            <HeaderStyle Width="20%"></HeaderStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="NOME" HeaderText="Nome">
                            <HeaderStyle Width="60%"></HeaderStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="FAMIGLIA" HeaderText="Famiglia">
                            <HeaderStyle Width="10%"></HeaderStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="VERSIONE" HeaderText="Versione">
                            <HeaderStyle Width="10%"></HeaderStyle>
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Selezione">
                            <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            <ItemTemplate>
                                <asp:RadioButton ID="rbSelezioneContesto" runat="server" onclick="rbEnabled_onClick(this.id);" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="padding: 10px;">
            <asp:UpdatePanel ID="UpNuovoContesto" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlNuovoContesto" runat="server" BorderWidth="1px" BorderColor="#810D06"
                        BorderStyle="Solid" Visible="false">
                        <table width="99%">
                            <tr>
                                <td class="titolo_pnl" colspan="2">
                                    <table cellspacing="0" cellpadding="0" width="100%">
                                        <tr>
                                            <td class="titolo_pnl">
                                                Nuovo contesto procedurale
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="BtnChiudiNuovoContesto" runat="server" ImageUrl="../Images/cancella.gif" OnClick="BtnChiudiNuovoContesto_Click">
                                                </asp:ImageButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table width="98%">
                            <tr>
                                <td class="testo_grigio_scuro" style="width: 100px" align="left" width="108">
                                    Tipo contesto procedurale *
                                </td>
                                <td style="padding-left: 5px" cssclass="testo" align="left">
                                    <asp:TextBox ID="txt_tipoContesto" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">
                                    Nome *
                                </td>
                                <td style="padding-left: 5px" cssclass="testo" align="left">
                                    <asp:TextBox ID="txt_Nome" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">
                                    Famiglia *
                                </td>
                                <td style="padding-left: 5px" cssclass="testo" align="left">
                                    <asp:TextBox ID="txt_famiglia" runat="server" CssClass="testo" Width="20"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">
                                    Versione *
                                </td>
                                <td style="padding-left: 5px" cssclass="testo" align="left">
                                    <asp:TextBox ID="txt_versione" runat="server" CssClass="testo" Width="20"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" colspan="2">
                                    <asp:Button ID="btn_AggiungiContesto" runat="server" CssClass="testo_btn_p" Text="Aggiungi" OnClick="btn_AggiungiContesto_Click">
                                    </asp:Button>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div style="text-align: center">
        <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button Text="Conferma" ID="btnConfirm" ToolTip="Conferma" CssClass="testo_btn_p"
                    runat="server" OnClick="btnConfirm_Click" />&nbsp;
                <asp:Button Text="Nuovo Contesto" ID="btnNuovoContesto" ToolTip="Nuovo contesto procedurale"
                    CssClass="testo_btn_p" runat="server" OnClick="btnNuovoContesto_Click" />&nbsp;
                <input type="button" title="Annulla" value="Annulla" class="testo_btn_p" onclick="window.close();" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
