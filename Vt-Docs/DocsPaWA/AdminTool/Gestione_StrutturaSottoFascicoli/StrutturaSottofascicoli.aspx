<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StrutturaSottofascicoli.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_StrutturaSottofascicoli.StrutturaSottofascicoli" %>

<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc3" TagName="AppTitleProvider" Src="../../UserControls/AppTitleProvider.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../CSS/caricamenujs.js"></script>
    <script type="text/javascript">
        function confirmDel() {
            var agree = confirm("Confermi la cancellazione ?");
            if (agree) {
                document.getElementById("txt_confirmDel").value = "si";
                return true;
            }
        }
    </script>
    <style type="text/css">
        .style1
        {
            font-weight: bold;
            font-size: 12px;
            color: #4b4b4b;
            font-family: Verdana;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <uc3:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Struttura Sottofascicoli" />
        <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
        <table height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
            <tr>
                <td>
                    <!-- TESTATA CON MENU' -->
                    <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
                </td>
            </tr>
            <tr>
                <!-- STRISCIA SOTTO IL MENU -->
                <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                    <asp:Label ID="lbl_position" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <!-- TITOLO PAGINA -->
                <td class="titolo" align="center" width="100%" bgcolor="#e0e0e0" height="20">Struttura Fascicoli</td>
            </tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                <table cellspacing="0" cellpadding="0" align="center" border="0" width="80%">
                    <tr>
                        <td align="center" height="25">
                            <asp:Label ID="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="pulsanti" align="center">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lbl_titolo" runat="server" CssClass="titolo">Lista Strutture Fascicoli</asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Button ID="btNuovaStruttura" runat="server" CssClass="testo_btn_p" Text="Nuova" OnClick="btNuovaStruttura_Click"></asp:Button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" height="5">
                            <asp:DataGrid ID="dgStrutture" runat="server" AutoGenerateColumns="False"
                                CellPadding="1" Width="100%" BorderWidth="1px" BorderColor="Gray"
                                OnItemCreated="dgStrutture_ItemCreated"
                                OnDeleteCommand="dgStrutture_DeleteCommand"
                                OnSelectedIndexChanged="dgStrutture_SelectedIndexChanged">
                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                <Columns>
                                    <asp:BoundColumn DataField="System_Id" Visible="false"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Name" HeaderText="Nome">
                                        <HeaderStyle Width="29%"></HeaderStyle>
                                    </asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Selezione">
                                        <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btSelezione" runat="server" ImageUrl="../Images/lentePreview.gif"
                                                CommandName="Select"></asp:ImageButton>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 onclick=confirmDel(); alt='Elimina'&gt;"
                                        HeaderText="Elimina" CommandName="Delete">
                                        <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:ButtonColumn>
                                </Columns>
                            </asp:DataGrid>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pNuovaStruttura" runat="server" Visible="False" BorderWidth="1px"
                                BorderColor="#810D06" BorderStyle="Solid">
                                <table width="99.5%">
                                    <tr>
                                        <td class="titolo_pnl" colspan="2">Struttura Fascicoli
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="left" width="50%" colspan="2">
                                            Descrizione Struttura *&nbsp
                                            <asp:TextBox ID="tbNomeStruttura" runat="server" CssClass="testo" Width="50%"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvNome" runat="server"
                                                ValidationGroup="InsertStruttura"
                                                ControlToValidate="tbNomeStruttura">
                                                * Inserire una descrizione
                                            </asp:RequiredFieldValidator>
                                            <asp:HiddenField ID="hfStrutturaID" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="contenitore" border="0">
                                                <tr>
                                                    <td class="style1" colspan="2">Elementi Struttura</td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" style="vertical-align:middle; text-align:left; width:50%;" colspan="2">
                                                        Descrizione Elemento *&nbsp
                                                        <asp:TextBox ID="tbDesElemento" runat="server" CssClass="testo" Width="50%"></asp:TextBox>
                                                        <asp:ImageButton ID="btAdd" runat="server" ImageUrl="~/images/proto/aggiungi.gif"
                                                            ToolTip="Aggiungi elemento"
                                                            OnClick="OnAddRootElement" ValidationGroup="InsertElemento" />
                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:RequiredFieldValidator ID="rfvDesElemento" runat="server"
                                                            ValidationGroup="InsertElemento"
                                                            ControlToValidate="tbDesElemento">
                                                            * Inserire una descrizione
                                                        </asp:RequiredFieldValidator>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="left">
                                                        <asp:ListView ID="lvStruttura" runat="server" 
                                                            InsertItemPosition="None"
                                                            OnItemDataBound="lvStruttura_ItemDataBound"
                                                            OnItemCommand="lvStruttura_ItemCommand"
                                                            OnItemInserting="lvStruttura_ItemInserting"
                                                            OnItemEditing="lvStruttura_ItemEditing"
                                                            OnItemCanceling="lvStruttura_ItemCanceling"
                                                            OnItemDeleting="lvStruttura_ItemDeleting">
                                                            <LayoutTemplate>
                                                                <table style="width: 99.5%;">
                                                                    <tr>
                                                                        <td class="testo_grigio_scuro" style="width: 80%">Struttura *</td>
                                                                        <td class="testo_grigio_scuro" style="text-align: center; width: 20%">Operazioni</td>
                                                                    </tr>
                                                                    <tr runat="server" id="itemPlaceholder"></tr>
                                                                </table>
                                                            </LayoutTemplate>
                                                            <ItemTemplate>
                                                                <tr class="bg_grigioN">
                                                                    <td>
                                                                        <asp:Literal ID="litIndent" runat="server"></asp:Literal>
                                                                        <asp:Label ID="tbNodeName" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td style="text-align: center">
                                                                        <asp:ImageButton ID="btAdd" runat="server" ImageUrl="~/images/proto/aggiungi.gif"
                                                                            ToolTip="Inserisci elemento"
                                                                            CommandName="InsertNode" CommandArgument='<%# Eval("SYSTEM_ID") %>' />
                                                                        <asp:ImageButton ID="btUpdate" runat="server" ImageUrl="~/images/proto/matita.gif"
                                                                            ToolTip="Modifica elemento"
                                                                            CommandName="Edit" CommandArgument='<%# Eval("SYSTEM_ID") %>' />
                                                                        <asp:ImageButton ID="btRemove" runat="server" ImageUrl="~/images/proto/cancella.gif"
                                                                            ToolTip="Cancella elemento"
                                                                            CommandName="Delete" CommandArgument='<%# Eval("SYSTEM_ID") %>' OnClientClick="return confirm('Confermi la cancellazione?')" />
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                            <AlternatingItemTemplate>
                                                                <tr class="bg_grigioA">
                                                                    <td>
                                                                        <asp:Literal ID="litIndent" runat="server"></asp:Literal>
                                                                        <asp:Label ID="tbNodeName" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td style="text-align: center">
                                                                        <asp:ImageButton ID="btAdd" runat="server" ImageUrl="~/images/proto/aggiungi.gif"
                                                                            CommandName="InsertNode" CommandArgument='<%# Eval("SYSTEM_ID") %>' />
                                                                        <asp:ImageButton ID="btUpdate" runat="server" ImageUrl="~/images/proto/matita.gif"
                                                                            CommandName="Edit" CommandArgument='<%# Eval("SYSTEM_ID") %>' />
                                                                        <asp:ImageButton ID="btRemove" runat="server" ImageUrl="~/images/proto/cancella.gif"
                                                                            CommandName="Delete" CommandArgument='<%# Eval("SYSTEM_ID") %>' OnClientClick="return confirm('Confermi la cancellazione?')" />
                                                                    </td>
                                                                </tr>
                                                            </AlternatingItemTemplate>
                                                            <InsertItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="tbNodeName" runat="server" Text='<%# Bind("Name") %>' ValidationGroup="SingleNode"></asp:TextBox><br />
                                                                        <asp:RequiredFieldValidator runat="server" ID="rfvNodename" ControlToValidate="tbNodeName" ValidationGroup="SingleNode"> * Inserire un nome per il nodo</asp:RequiredFieldValidator>
                                                                    </td>
                                                                    <td style="text-align: center">
                                                                        <asp:ImageButton ID="btSalvaInsert" runat="server" ImageUrl="~/images/proto/salva.gif" ToolTip="Salva"
                                                                            ValidationGroup="SingleNode" CommandName="Insert" CommandArgument='<%# Bind("SYSTEM_ID") %>' />
                                                                        <asp:ImageButton ID="btAnnullaInsert" runat="server" ImageUrl="~/images/proto/btn_corrente.gif"
                                                                            CommandName="Cancel" CommandArgument='<%# Bind("SYSTEM_ID")  %>' ToolTip="Indietro"/>
                                                                    </td>
                                                                </tr>
                                                            </InsertItemTemplate>
                                                            <EditItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="tbNodeName" runat="server" Text='<%# Bind("Name") %>' ValidationGroup="SingleNode"></asp:TextBox>
                                                                    </td>
                                                                    <td style="text-align: center">
                                                                        <asp:ImageButton ID="btSalvaInsert" runat="server" ImageUrl="~/images/proto/salva.gif"
                                                                            ValidationGroup="SingleNode" CommandName="UpdateNode" CommandArgument='<%# Bind("SYSTEM_ID") %>' />
                                                                        <asp:ImageButton ID="btAnnullaInsert" runat="server" ImageUrl="~/images/proto/btn_corrente.gif"
                                                                            CommandName="Cancel" CommandArgument='<%# Bind("SYSTEM_ID") %>' />
                                                                    </td>
                                                                </tr>
                                                            </EditItemTemplate>
                                                            <EmptyDataTemplate>
                                                                <tr>
                                                                    <td class="testo">Nessun dato presente.
                                                                    </td>
                                                                </tr>
                                                            </EmptyDataTemplate>
                                                        </asp:ListView>
                                                        <asp:TextBox runat="server" ID="tbNumNodiStruttura" Text="<%# GetNodesNumber() %>" ValidationGroup="InsertStruttura" Style="display: none"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvNumStrutturaNodes"
                                                            ControlToValidate="tbNumNodiStruttura"
                                                            ValidationGroup="InsertStruttura">
                                                            * Inserire un elemento nella struttura
                                                        </asp:RequiredFieldValidator>
                                                        <asp:HiddenField ID="hfParentID" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: right">
                                            <asp:Button ID="btAnnulla" runat="server" CssClass="testo_btn_p"
                                                Text="Annulla" OnClick="btAnnulla_Click" CausesValidation="False" />
                                            <asp:Button ID="btSalvaStruttura" runat="server" CssClass="testo_btn_p" ValidationGroup="InsertStruttura"
                                                Text="Salva" OnClick="btSalvaStruttura_Click" CausesValidation="True" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                        </td>
                    </tr>
                </table>

            </td>
        </table>
        <input id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server" />
    </form>
</body>
</html>
