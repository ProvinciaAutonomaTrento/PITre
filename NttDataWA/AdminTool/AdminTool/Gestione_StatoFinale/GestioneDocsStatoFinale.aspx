<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneDocsStatoFinale.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_StatoFinale.GestioneDocsStatoFinale" %>

<%@ Register Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../CSS/caricamenujs.js"></script>
    <title></title>
    <script type="text/javascript">
        function showDialogVisibilitaStatoFinale(idDocument) {
            window.document.body.style.cursor = "wait";

            var pageHeight = 379;
            var pageWidth = 526;

            var refresh = window.showModalDialog('VisibilitaDocsStatoFinale.aspx?IdDoc=' + idDocument,
                    null,
                    'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');

            if (refresh == '1') {

                document.getElementById("hdRefreshGrid").value = refresh;

            }
        }



        function reload() {

            window.document.forms[0].submit();
            //location.reload(true);
        }

    </script>
    <style type="text/css">
        .style1
        {
            height: 4px;
        }
    </style>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" ms_positioning="GridLayout">
    <form id="frmDocsStatoFinale" method="post" runat="server">
    <input id="hdRefreshGrid" type="hidden" runat="server" onpropertychange="reload();" />
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Sblocca documenti" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <table cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Documenti allo stato finale
            </td>
        </tr>
        <tr>
            <td height="5">
            </td>
        </tr>
        <tr>
            <td height="5" align="right" width="80%">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td height="5" align="center">
                <asp:Label ID="lblMessage" runat="server" CssClass="testo_rosso"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table id="table_filtri" width="100%" border="0">
                    <tr id="filtriGrigiProt">
                        <td class="info">
                                <table>
                            <tr>
                            <td>
                                   <asp:RadioButtonList ID="rblTipo" runat="server" CssClass="testo_grigio_scuro" RepeatDirection="Horizontal"
                                            OnSelectedIndexChanged="rblTipo_SelectedIndexChanged" AutoPostBack="True">
                                            <asp:ListItem Selected="True" Value="P">Protocollati</asp:ListItem>
                                            <asp:ListItem Value="NP">Non Protocollati</asp:ListItem>
                                        </asp:RadioButtonList>
                        </td>
                                        <td class="info">
                                <asp:CheckBox ID="cbSbloccati" runat="server" Text="Sbloccati"  
                                       CssClass="testo_grigio_scuro" TextAlign="Right"/>
                                       </td>
                                </tr>
                                </table>
                        

                        
                    </tr>
                    <tr>
                        <asp:Panel ID="pnlProt" runat="server">
                          
                                    <table border="0" cellpadding="0" cellspacing="0" width="90%">
                                        <tr>
                                           
                                                &nbsp;<asp:Label ID="lbl_registri" runat="server" Text="Registro:" CssClass="testo_grigio_scuro"></asp:Label>
                                                &nbsp;
                                                <asp:DropDownList ID="ddl_registri" runat="server" CssClass="testo" 
                                                    OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                <asp:Label ID="lbl_anno" runat="server" Text="Anno:" CssClass="testo_grigio_scuro"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="tbx_anno" runat="server" CssClass="testo"  ></asp:TextBox>
                                            <cc2:FilteredTextBoxExtender ID="flt_tbx_anno" runat="server" FilterType="Numbers"
                                                TargetControlID="tbx_anno">
                                            </cc2:FilteredTextBoxExtender>
                                                <asp:Label ID="lbl_numProto" runat="server" Text="Nr. Protocollo:" CssClass="testo_grigio_scuro"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="tbx_numProto" runat="server" CssClass="testo" ></asp:TextBox>
                                            <cc2:FilteredTextBoxExtender ID="flt_tbx_numProto" runat="server" FilterType="Numbers"
                                                TargetControlID="tbx_numProto">
                                            </cc2:FilteredTextBoxExtender>
                                         
                                        </tr>
                                    </table>
                          
                        </asp:Panel>
                   
                        <asp:Panel ID="pnlDOC" runat="server" Visible="False">
                            <tr id="Documenti" class="info">
                                <td>
                                    &nbsp;<asp:Label ID="lblDOc" runat="server" Text="ID Documento:" CssClass="testo_grigio_scuro"></asp:Label>
                                    &nbsp;
                                    <asp:TextBox ID="tbxDoc" runat="server" CssClass="testo"></asp:TextBox>
                                    <cc2:FilteredTextBoxExtender ID="flt_tbxDoc" runat="server" FilterType="Numbers"
                                        TargetControlID="tbxDoc">
                                    </cc2:FilteredTextBoxExtender>
                            </tr>
                        </asp:Panel>
                    
                        <asp:Panel ID="pnlTipologia" runat="server" Visible="False">
                                <tr>
                                <td>
                                 <asp:Label ID="l_tipologia" runat="server" CssClass="testo_grigio_scuro" Text="Tipologia documento:"></asp:Label>&nbsp;
                                 <asp:DropDownList ID="ddl_tipologiaDoc" runat="server"
                                        CssClass="testo" AutoPostBack="True" OnSelectedIndexChanged="ddl_tipologiaDoc_SelectedIndexChanged">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                  </asp:DropDownList>
                                 </td>
                                 </tr>
                                </asp:Panel>

                            </tr>
                            <tr>
                                <td>
                                    <table border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Panel ID="panel_Contenuto" runat="server" CssClass="testo">
                                                </asp:Panel>
                                                
                                            </td>
                                            <td>
                                                <asp:Panel ID="pnl_RFAOO" runat="server">
                                                    <asp:Label ID="lblAooRF" CssClass="testo_grigio_scuro" Text="" runat="server"></asp:Label>
                                                    <asp:DropDownList ID="ddlAooRF" runat="server" CssClass="testo" AutoPostBack="False"
                                                        Visible="false">
                                                    </asp:DropDownList>
                                                </asp:Panel>
                                            </td>
                                            <td class="titolo_scheda" valign="top">
                                                <asp:Panel ID="pnlAnno" runat="server" Visible = "false">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                &nbsp;&nbsp;<asp:Label ID="lblAnno" CssClass="testo_grigio_scuro" Text="Anno *" runat="server" Visible="false" ></asp:Label>
                                                                <asp:TextBox ID="txt_anno" runat="server" CssClass="testo" MaxLength="4" Visible="false"></asp:TextBox>
                                                                <cc2:FilteredTextBoxExtender ID="flt_TxtAnnoDa" runat="server" FilterType="Numbers"
                                                                    TargetControlID="txt_anno">
                                                                </cc2:FilteredTextBoxExtender>
                                                        
                                                            </td>
                                                            <td>
                                                                &nbsp;&nbsp;<asp:Label ID="lblNumero" CssClass="testo_grigio_scuro" Text="Numero *" runat="server" Visible="false"></asp:Label>
                                                                <asp:TextBox ID="txt_numero" runat="server" CssClass="testo" Visible="false"></asp:TextBox>
                                                                <cc2:FilteredTextBoxExtender ID="flt_txt_Numero" runat="server" FilterType="Numbers"
                                                                    TargetControlID="txt_numero">
                                                                </cc2:FilteredTextBoxExtender>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                            <td class="titolo_scheda" valign="top">
                                                <asp:Panel ID="pnlNumero" runat="server" Visible="false">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                &nbsp;&nbsp;<asp:Label ID="lblNumeroDa" CssClass="titolo_scheda" Text="Da Numero *"
                                                                    runat="server" Visible="false"></asp:Label>
                                                                <asp:TextBox ID="TxtNumeroDa" runat="server" CssClass="testo_grigio" Width="40" MaxLength="4" Visible="false"></asp:TextBox>
                                                                <cc2:FilteredTextBoxExtender ID="flt_TxtNumeroDa" runat="server" FilterType="Numbers"
                                                                    TargetControlID="TxtNumeroDa">
                                                                </cc2:FilteredTextBoxExtender>
                                                            </td>
                                                            <td>
                                                                &nbsp;&nbsp;<asp:Label ID="Label1" CssClass="titolo_scheda" Text="A Numero *" runat="server" Visible="false"></asp:Label>
                                                                <asp:TextBox ID="TxtNumeroA" runat="server" CssClass="testo_grigio" Width="40" MaxLength="4" Visible="false"></asp:TextBox>
                                                                <cc2:FilteredTextBoxExtender ID="flt_TxtNumeroA" runat="server" FilterType="Numbers"
                                                                    TargetControlID="TxtNumeroA">
                                                                </cc2:FilteredTextBoxExtender>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </asp:Panel>
                    </tr>
                    <tr>
                        <td align="center" height="26" valign="middle" class="pulsanti">
                            <asp:Button ID="btn_cerca" runat="server" CssClass="testo_btn" 
                                Text="CERCA" OnClick="btn_cerca_Click"></asp:Button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" width="80%">
                <div id="divGrdDocuments" style="overflow: auto; width: 100%; height: 357px" visible="false"
                    align="center">
                    <asp:DataGrid ID="grdDocuments" runat="server" AutoGenerateColumns="False" BorderColor="Gray"
                        CellPadding="1" BorderWidth="1px" Width="100%" 
                        OnItemDataBound="grdDocuments_ItemDataBound">
                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                        <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                        <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:Label ID="lblIdDoc" runat="server" Text='<%#((SAAdminTool.DocsPaWR.DocumentoStatoFinale) Container.DataItem).IdDocumento%>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="DocName" HeaderText="ID Doc. - Segnatura">
                                <HeaderStyle HorizontalAlign="Center" Width="25%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                    Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Oggetto" HeaderText="Oggetto">
                                <HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="MittDest" HeaderText="Mitt/Dest">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Tipologia" HeaderText="Tipologia Documento">
                                <HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Mod. Diritti">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnSblocca" runat="server" Width="20px" CommandName="SBLOCCA"
                                        ImageUrl="../Images/utenti.gif" Height="20px" ToolTip="Sblocca"></asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
