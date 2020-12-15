<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="searchInControlledProject.aspx.cs"
    Inherits="DocsPAWA.popup.searchInControlledProject" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <title>Documenti da trasmettere con il fascicolo</title>
    <base target="_self" />
</head>
<body bottommargin="0" leftmargin="5" topmargin="5" rightmargin="5" ms_positioning="GridLayout">
    <form id="frmSearchInControlledProject" runat="server">
        <table class="contenitore" id="tbl_rispostaProtoUscita" height="500" width="100%" align="center" border="0">
            <tr valign="middle" height="20">
                <td class="menu_1_rosso" align="center">
                    Seleziona i documenti da trasmettere con il fascicolo
                </td>
            </tr>
            <tr valign="top">
                <td align="center">
                    <div id="DivDataGrid" style="overflow: auto; height: 400px" align="center">
                        <table>
                            <tr>
                                <td align="left">
                                    <asp:CheckBox ID="chkSelectDeselectAll" runat="server" AutoPostBack="True" CssClass="testo_grigio_scuro"
                                        Text="Seleziona / deseleziona tutti" Checked="False" Visible="False"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DataGrid ID="DataGrid1" SkinID="datagrid" runat="server" Width="98%" Visible="False"
                                        DataKeyField="idProfile" AutoGenerateColumns="False" BorderWidth="1px" AllowPaging="True"
                                        CellPadding="1" BorderColor="Gray" AllowCustomPaging="True" OnItemCreated="Grid_OnItemCreated">
                                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                        <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                        <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
                                        <Columns>
                                            <asp:TemplateColumn>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkFascicola" runat="server" Enabled='<%# DataBinder.Eval(Container, "DataItem.Fascicola") %>'>
                                                    </asp:CheckBox>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Doc. Data">
                                                <HeaderStyle Wrap="False" HorizontalAlign="Justify" Width="10px" VerticalAlign="Middle">
                                                </HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descDoc") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="Textbox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descDoc") %>'>
                                                    </asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Registro">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="Textbox10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
                                                    </asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Oggetto">
                                                <HeaderStyle Width="480px"></HeaderStyle>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>'
                                                        ID="Label2">
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'
                                                        ID="Textbox2">
                                                    </asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Tipo">
                                                <HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_tipo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txt_tipo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
                                                    </asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="False" HeaderText="Data Annullamento">
                                                <HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_dtaAnnullam" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dataAnnullamento") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txt_dtaAnnullam" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dataAnnullamento") %>'>
                                                    </asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="False">
                                                <HeaderStyle HorizontalAlign="Center" Width="10px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <cc1:ImageButton ID="img_select" runat="server" BorderWidth="0px" AlternateText="Seleziona il documento"
                                                        ImageUrl="../images/proto/ico_riga.gif" CommandName="Select" DisabledUrl="../images/proto/ico_riga.gif">
                                                    </cc1:ImageButton>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="False" HeaderText="chiave">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_key" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="False" HeaderText="idProfile">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_idProfile" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="False" HeaderText="Num Prot">
                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label7" runat="server" Text='<%# getNumProt((string) DataBinder.Eval(Container, "DataItem.numProt")) %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="Textbox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.numProt") %>'>
                                                    </asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                        </Columns>
                                        <PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
                                            Mode="NumericPages"></PagerStyle>
                                    </asp:DataGrid>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <table align="center">
        <tr>
                <td align="center">
                    &nbsp;
                    <asp:Button ID="btn_ok" runat="server" CssClass="pulsante_mini_3" Text="OK"></asp:Button>&nbsp;
                    <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante_mini_3" Text="CHIUDI"></asp:Button>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
