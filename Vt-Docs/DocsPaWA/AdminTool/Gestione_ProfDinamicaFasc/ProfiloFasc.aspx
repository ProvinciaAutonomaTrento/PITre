<%@ Page Language="c#" CodeBehind="ProfiloFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.AdminTool.Gestione_ProfDinamicaFasc.ProfiloFasc"
    ValidateRequest="false" %>

<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc1" TagName="IntegrationAdapter" Src="../../UserControls/IntegrationAdapter.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="HEAD1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script type="text/javascript" language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script type="text/javascript" language="javascript">
        var w = window.screen.width;
        var h = window.screen.height;
        var new_w = (w - 500) / 2;
        var new_h = (h - 400) / 2;

        function apriPopupAnteprima() {
            //window.open('AnteprimaProfDinamicaFasc.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');
            window.showModalDialog('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=AnteprimaProfDinamicaFasc.aspx',
                '',
                'dialogWidth:700px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function apriModelliTrasm() {
            //window.open('AssociaModelliTrasmFasc.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');				
            window.showModalDialog('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=AssociaModelliTrasmFasc.aspx',
                '',
                'dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function apriPopupVisibilita() {
            //window.open('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=Visibilita.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');				
            window.showModalDialog('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=VisibilitaFasc.aspx',
                '',
                'dialogWidth:800px;dialogHeight:650px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        /*
        function apriPopupAssociaRuolo(idOggettoCustom)
        {
        //window.open('ModalAnteprimaProfDinamica.aspx?Chiamante=Visibilita.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');				
        window.showModalDialog('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=AssociaRuolo.aspx&IdOggCustom=' + idOggettoCustom,'','dialogWidth:800px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);			
        }
        */

        function apriPopupModelli() {
            //window.open('AssociaModelliFasc.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=160,scrollbars=NO');				
            window.showModalDialog('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=AssociaModelliFasc.aspx',
                '',
                'dialogWidth:500px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
            window.Form1.submit();
        }

        function apriPopupCampiComuni() {
            //window.open('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=Visibilita.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');				
            window.showModalDialog('ModalAnteprimaProfDinamicaFasc.aspx?Chiamante=CampiComuni.aspx',
                '',
                'dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
            window.Form1.submit();
        }

        function confirmDel() {
            var agree = confirm("Confermi la cancellazione ?");
            if (agree) {
                document.getElementById("txt_confirmDel").value = "si";
                return true;
            }
        }

        // Permette di inserire solamente caratteri numerici
        function ValidateNumericKey() {
            var inputKey = event.keyCode;
            var returnCode = true;

            if (inputKey > 47 && inputKey < 58) {
                return;
            } else {
                returnCode = false;
                event.keyCode = 0;
            }

            event.returnValue = returnCode;
        }
    </script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0">
    <form id="Form1" method="post" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Tipi Fascicolo" />
        <!-- Gestione del menu a tendina -->
        <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
        <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
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
                <td class="titolo" style="height: 20px" align="center" bgcolor="#e0e0e0" height="34">Tipi Fascicolo
                </td>
            </tr>
            <!-- Autenticazione Sistemi Esterni: ritorno alla configurazione per i sistemi esterni -->
            <tr id="tr_backToExtSys" runat="server">
                <td valign="top" align="left" bgcolor="#f6f4f4" height="20">
                    <asp:Button ID="btn_toExtSys" runat="server" CssClass="testo_btn" UseSubmitBehavior="false"
                        Text="Ritorna a Sistemi Esterni" OnClientClick="location.href = '../Gestione_SistemiEsterni/SistemiEsterni.aspx';return false;"></asp:Button>
                </td>
            </tr>
            <tr>
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
                                            <asp:Label ID="lbl_titolo" runat="server" CssClass="titolo">Lista Tipi Fascicolo</asp:Label>
                                        </td>
                                        <td align="right">&nbsp;
                                            <asp:Button ID="btn_Storico" Text="Gestione storico" runat="server"  CssClass="testo_btn_p" Visible="false" />
                                            <asp:Button ID="btn_CampiComuni" runat="server" CssClass="testo_btn_p" Visible="False" Text="Campi Comuni" OnClick="btn_CampiComuni_Click"></asp:Button>&nbsp;
                                            <asp:Button ID="btn_inEsercizio" runat="server" CssClass="testo_btn_p" Visible="False" Text="In Esercizio"></asp:Button>&nbsp;
                                            <asp:Button ID="btn_salvaTemplate" runat="server" CssClass="testo_btn_p" Visible="False" Text="Salva"></asp:Button>&nbsp;
                                            <asp:Button ID="btn_anteprima" runat="server" CssClass="testo_btn_p" Visible="False" Text="Anteprima"></asp:Button>&nbsp;
                                            <asp:Button ID="btn_modelli" runat="server" CssClass="testo_btn_p" Visible="False" Text="Tipi Fascicolo"></asp:Button>&nbsp;
                                            <asp:Button ID="btn_nuovoModello" runat="server" CssClass="testo_btn_p" Text="Nuovo"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <!-- INIZIO: PANNELLI -->
                        <tr>
                            <td align="center" height="5">
                                <br>
                                <!-- INIZIO : PANNELLO LISTA TEMPLATES -->
                                <asp:Panel ID="Panel_ListaModelli" runat="server">
                                    <div id="DivDGListaTemplates" align="center" runat="server">
                                        <asp:DataGrid ID="dg_listaTemplates" runat="server" Visible="False" AutoGenerateColumns="False"
                                            CellPadding="1" Width="100%" BorderWidth="1px" BorderColor="Gray"
                                            OnItemCreated="dg_listaTemplates_ItemCreated">
                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Tipo" HeaderText="Tipo">
                                                    <HeaderStyle Width="31%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Diagrammi">
                                                    <HeaderStyle Width="31%"></HeaderStyle>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../Images/ico_scan_ruota.gif"
                                                            CommandName="Diagrammi"></asp:ImageButton>
                                                        <asp:Label ID="lbDiagrammi" runat="server" Height="17px">Label</asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="In Esercizio" HeaderText="ATTIVO">
                                                    <HeaderStyle HorizontalAlign="Center" Width="6%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Modelli">
                                                    <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                                    <ItemTemplate>
                                                        &nbsp;
                                                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="../Images/documento.gif"
                                                            CommandName="Modelli"></asp:ImageButton>
                                                        <asp:Image ID="img_mod1" runat="server" ImageUrl="../Images/spunta_1.gif"></asp:Image>
                                                        <asp:Image ID="img_mod2" runat="server" ImageUrl="../Images/spunta_2.gif"></asp:Image>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Template Struttura">
                                                    <HeaderStyle Width="8%" Font-Bold="True" HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btTemplateStruttura" runat="server" ImageUrl="../../images/proto/matita.gif"
                                                            CommandName="Struttura"></asp:ImageButton>&nbsp;
                                                        <asp:Label ID="lbTemplate" runat="server" Height="17px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Visibilit&#224;">
                                                    <HeaderStyle Width="8%" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                        Font-Underline="False" HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButton4" runat="server" CommandName="Visibilita" ImageUrl="~/AdminTool/Images/utenti.gif" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Privato">
                                                    <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center" Font-Bold="False"></ItemStyle>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButtonPriv" runat="server" ImageUrl="../../images/proto/matita.gif"
                                                            CommandName="Privato"></asp:ImageButton>
                                                        <asp:Label ID="lbl_privato" runat="server" Height="17px">Label</asp:Label>&nbsp;
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Mesi Cons.">
                                                    <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center" Font-Bold="False"></ItemStyle>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButtonMesiCons" runat="server" ImageUrl="../../images/proto/matita.gif"
                                                            CommandName="MesiCons"></asp:ImageButton>
                                                        <asp:Label ID="lbl_mesiCons" runat="server" Height="17px">Label</asp:Label>&nbsp;
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Selezione">
                                                    <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../Images/lentePreview.gif"
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
                                    </div>
                                </asp:Panel>
                                <br>
                                <!-- FINE : PANNELLO LISTA TEMPLATES -->
                                <!-- INIZIO : PANNELLO ASSOCIAZIONE DIAGRAMMA E TRASMISISONI -->
                                <asp:Panel ID="Panel_Diagrammi_Trasmissioni" runat="server" Visible="False" BorderWidth="1px"
                                    BorderColor="#810D06" BorderStyle="Solid">
                                    <table width="99.5%">
                                        <tr>
                                            <td class="titolo_pnl" colspan="3">
                                                <table cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td class="titolo_pnl">Associazione Diagramma di stato -
                                                            <asp:Label ID="lbl_tipoFasc" runat="server" CssClass="titolo_pnl"></asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:ImageButton ID="pulsante_chiudi_diagr_trasm" runat="server" ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="testo_grigio_scuro" style="height: 6px" width="10%">Diagramma :
                                            </td>
                                            <td style="height: 6px">
                                                <asp:DropDownList ID="ddl_Diagrammi" runat="server" CssClass="testo" Width="250px"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 6px" align="right">
                                                <asp:Button ID="btn_cambiaDiag" runat="server" CssClass="testo_btn_p" Text="Cambia Diag."
                                                    Visible="False"></asp:Button>
                                                <asp:Button ID="btn_modelliTrasm" runat="server" CssClass="testo_btn_p" Text="Mod. Trasm."></asp:Button>
                                                <asp:Button ID="btn_confermaDiagrTrasm" runat="server" CssClass="testo_btn_p" Text="Conferma"></asp:Button>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="testo_grigio_scuro">Scadenza (gg)
                                            </td>
                                            <td class="testo_grigio_scuro" colspan="2">
                                                <asp:TextBox ID="txt_scadenza" runat="server" CssClass="testo" MaxLength="3" Width="30px"></asp:TextBox>
                                                Notifica scadenza (gg)
                                                <asp:TextBox ID="txt_preScadenza" runat="server" CssClass="testo" MaxLength="3" Width="30px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:DataGrid ID="dg_Stati" runat="server" Visible="False" AutoGenerateColumns="False"
                                                    CellPadding="1" Width="100%" BorderWidth="1px" BorderColor="Gray" OnItemCreated="dg_Stati_ItemCreated">
                                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                                    <Columns>
                                                        <asp:BoundColumn Visible="False" DataField="ID_STATO" HeaderText="IdStato"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Descrizione">
                                                            <HeaderStyle Width="90%"></HeaderStyle>
                                                        </asp:BoundColumn>
                                                        <asp:ButtonColumn Text="&lt;img src=../Images/ico_scan_ruota.gif border=0 alt='Diagramma di stato'&gt;"
                                                            HeaderText="Mod. Trasm." CommandName="Select">
                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                        </asp:ButtonColumn>
                                                    </Columns>
                                                </asp:DataGrid>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <!-- FINE : PANNELLO ASSOCIAZIONE DIAGRAMMA E TRASMISISONI -->
                                <!-- INIZIO : PANNELLO NUOVO TEMPLATE -->
                                <asp:Panel ID="Panel_NuovoModello" runat="server" Visible="False" BorderWidth="1px"
                                    BorderColor="#810D06" BorderStyle="Solid">
                                    <table width="99.5%">
                                        <tr>
                                            <td class="titolo_pnl" colspan="5">Nuovo Tipo di Fascicolo
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="testo_grigio_scuro" align="left" width="20%">Tipo Fascicolo *
                                            </td>
                                            <td align="left" width="20%">
                                                <asp:TextBox ID="txt_TipoDocumento" runat="server" CssClass="testo" Width="90%"></asp:TextBox>&nbsp;
                                            </td>
                                            <td align="right" width="20%">
                                                <asp:Label ID="lblNumeroMesiConservazione" runat="server" CssClass="testo_grigio_scuro">Mesi conservazione</asp:Label>
                                            </td>
                                            <td align="left" width="5%">
                                                <asp:TextBox ID="txt_MesiConservazione" runat="server" CssClass="testo" MaxLength="6"
                                                    Width="100%">
                                                </asp:TextBox>
                                            </td>
                                            <td align="right">
                                                <!--td align=center-->
                                                <asp:CheckBox ID="cb_Privato" runat="server" CssClass="testo_grigio_scuro" Text="Privato"
                                                    TextAlign="Left" Checked="False"></asp:CheckBox>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <!-- FINE : PANELLLO NUOVO TEMPLATE -->
                                <!-- INIZIO : PANNELLO LISTA COMPONENTI -->
                                <asp:Panel ID="Panel_ListaComponenti" runat="server" Visible="False" BorderWidth="1px"
                                    BorderColor="#810D06" BorderStyle="Solid">
                                    <table width="99.5%" bgcolor="white">
                                        <tr>
                                            <td class="titolo" align="center" colspan="6">
                                                <strong>Clicca su un'immagine per aggiungere il tipo di campo desiderato</strong>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:ImageButton ID="CampoDiTesto" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgCampo di testo.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="CasellaDiSelezione" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgCasellaDiSelezione.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="MenuATendina" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgMenuATendina.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="SelezioneEsclusiva" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgSelezioneEsclusiva.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="Contatore" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgContatore.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="Data" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgData.bmp"></asp:ImageButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:ImageButton ID="Corrispondente" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgCorrispondente.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="Link" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgLink.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="Separatore" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgSeparatore.bmp"></asp:ImageButton>
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="OggettoEsterno" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                    BorderStyle="Solid" ImageUrl="../Images/imgOggEsterno.bmp"></asp:ImageButton>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Panel ID="Panel_Dg_ListaComponenti" runat="server">
                                    <br>
                                    <asp:Label ID="lbl_nameTypeFasc" CssClass="titolo" Width="100%" Font-Bold="True"
                                        runat="server">
                                    </asp:Label>
                                    <div id="DivDGListaComponenti" align="center" runat="server">
                                        <asp:DataGrid ID="dg_listaComponenti" runat="server" Visible="False" AutoGenerateColumns="False"
                                            CellPadding="1" Width="100%" BorderWidth="1px" BorderColor="Gray" OnItemCreated="dg_listaComponenti_ItemCreated">
                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Ordinamento" HeaderText="Ordinamento">
                                                    <HeaderStyle HorizontalAlign="Center" Width="100px"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Tipo" HeaderText="Tipo Campo"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Etichetta" HeaderText="Etichetta">
                                                    <HeaderStyle Width="400px"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;"
                                                    CommandName="Select">
                                                    <HeaderStyle Width="5%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:ButtonColumn>
                                                <asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0; onclick=confirmDel(); alt='Elimina'&gt;"
                                                    CommandName="Delete">
                                                    <HeaderStyle Width="5%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:ButtonColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </asp:Panel>
                                <!-- FINE : PANNELLO LISTA COMPONENTI -->
                                <br>
                                <asp:Panel ID="Panel_Personalizzazione" runat="server" Visible="False" BorderWidth="1px"
                                    BorderColor="#810D06" BorderStyle="Solid">
                                    <table width="99.5%">
                                        <tr>
                                            <td class="titolo_pnl" colspan="2">
                                                <table cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td class="titolo_pnl">Personalizzazione Campo
                                                        </td>
                                                        <td align="right">
                                                            <asp:ImageButton ID="pulsante_chiudi_pers" runat="server" ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="99.5%">
                                        <tr>
                                            <td colspan="4">
                                                <!-- INIZIO : PANNELLI PERS TEXTBOX - CHECKBOX - RADIOBUTTON - COMBOBOX - CONTATORE - CORRISPONDENTE -->
                                                <asp:Panel ID="Panel_PersCampoDiTesto" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaCampoDiTesto" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Multilinea
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Multilinea_CampoDiTesto" runat="server" Width="180px" AutoPostBack="True"></asp:CheckBox>
                                                            </td>
                                                            <td rowspan="3">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgCampo di testo.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Obbligatorio_CampoDiTesto" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">N. Caratteri
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_NumeroCaratteri_CampoDiTesto" runat="server" CssClass="testo"
                                                                    Width="40">
                                                                </asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Ricerca_CampoDiTesto" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">N. Linee
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_NumeroLinee_CampoDiTesto" runat="server" CssClass="testo" Width="40"
                                                                    BackColor="White">
                                                                </asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersCampoDiTesto" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_1" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_1" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersCasellaDiSelezione" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaCasellaDiSelezione" runat="server" CssClass="testo"
                                                                    Width="200">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Elenco valori
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_valoriCasellaSelezione" runat="server" CssClass="testo"
                                                                    Width="180px" AutoPostBack="True">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td rowspan="4">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgCasellaDiSelezione.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Valore *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_valoreCasellaSelezione" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Default
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Default_CasellaSelezione" runat="server" AutoPostBack="True"
                                                                    OnCheckedChanged="cb_Default_CasellaSelezione_CheckedChanged"></asp:CheckBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Obbligatorio_CasellaDiSelezione" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="left">Disabilitato
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Disabilitato_CasellaSelezione" runat="server" AutoPostBack="True"
                                                                    OnCheckedChanged="cb_Disabilitato_CasellaSelezione_CheckedChanged" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" align="left">Ricerca
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Ricerca_CasellaDiSelezione" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td></td>
                                                            <td cssclass="testo">
                                                                <asp:RadioButtonList CssClass="testo_grigio_scuro" ID="rd_VerOri_CasellaSelezione" runat="server"
                                                                    RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="Verticale" Selected="True">Verticale</asp:ListItem>
                                                                    <asp:ListItem Value="Orizzontale">Orizzontale</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersCasellaDiSelezione" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>&nbsp;
                                                                <asp:Button ID="btn_aggiungiValoreCasellaSelezione" runat="server" CssClass="testo_btn_p"
                                                                    Text="Aggiungi Valore"></asp:Button>&nbsp;
                                                                <asp:Button ID="btn_eliminaValoreCasellaSelezione" runat="server" CssClass="testo_btn_p"
                                                                    Text="Elimina Valore"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_2" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_2" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersSelezioneEsclusiva" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaSelezioneEsclusiva" runat="server" CssClass="testo"
                                                                    Width="200">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Elenco valori
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_valoriSelezioneEsclusiva" runat="server" CssClass="testo"
                                                                    Width="180px" AutoPostBack="True">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td rowspan="4">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgSelezioneEsclusiva.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="height: 25px; width: 108px;" align="left" width="108">Valore *
                                                            </td>
                                                            <td style="height: 25px; padding-left: 5px;" cssclass="testo">
                                                                <asp:TextBox ID="txt_valoreSelezioneEsclusiva" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="height: 25px; width: 80px;" align="right" width="80">Default
                                                            </td>
                                                            <td style="height: 25px">
                                                                <asp:CheckBox ID="cb_Default_SelezioneEsclusiva" runat="server" AutoPostBack="True"
                                                                    OnCheckedChanged="cb_Default_SelezioneEsclusiva_CheckedChanged"></asp:CheckBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Obbligatorio_SelezioneEsclusiva" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Disabilitato
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Disabilitato_SelezioneEsclusiva" runat="server" AutoPostBack="True"
                                                                    OnCheckedChanged="cb_Disabilitato_SelezioneEsclusiva_CheckedChanged" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Ricerca_SelezioneEsclusiva" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td></td>
                                                            <td cssclass="testo">
                                                                <asp:RadioButtonList CssClass="testo_grigio_scuro" ID="rd_VerOri_SelezioneEsclusiva"
                                                                    runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="Verticale" Selected="True">Verticale</asp:ListItem>
                                                                    <asp:ListItem Value="Orizzontale">Orizzontale</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersSelezioneEsclusiva" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>&nbsp;
                                                                <asp:Button ID="btn_aggiungiValoreSelezioneEsclusiva" runat="server" CssClass="testo_btn_p"
                                                                    Text="Aggiungi Valore"></asp:Button>&nbsp;
                                                                <asp:Button ID="btn_elimnaValoreSelezioneEsclusiva" runat="server" CssClass="testo_btn_p"
                                                                    Text="Elimina Valore"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_3" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_3" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersContatore" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaContatore" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Separatore
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_separatore" runat="server" CssClass="testo" AutoPostBack="True"
                                                                    OnSelectedIndexChanged="ddl_separatore_SelectedIndexChanged">
                                                                    <asp:ListItem></asp:ListItem>
                                                                    <asp:ListItem>|</asp:ListItem>
                                                                    <asp:ListItem>-</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td rowspan="3">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgContatore.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Ricerca_Contatore" runat="server" AutoPostBack="True" OnCheckedChanged="cb_Ricerca_Contatore_CheckedChanged"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="height: 25px; width: 80px;" align="right" width="80">Campi
                                                            </td>
                                                            <td style="height: 25px; padding-left: 5px">
                                                                <asp:DropDownList ID="ddl_campiContatore" runat="server" CssClass="testo" Enabled="false">
                                                                    <asp:ListItem></asp:ListItem>
                                                                    <asp:ListItem>RF</asp:ListItem>
                                                                    <asp:ListItem>AOO</asp:ListItem>
                                                                    <asp:ListItem>ANNO</asp:ListItem>
                                                                    <asp:ListItem>CONTATORE</asp:ListItem>
                                                                    <asp:ListItem>COD_AMM</asp:ListItem>
                                                                    <asp:ListItem>COD_UO</asp:ListItem>
                                                                    <asp:ListItem>gg/mm/aaaa hh:mm</asp:ListItem>
                                                                    <asp:ListItem>gg/mm/aaaa</asp:ListItem>
                                                                </asp:DropDownList>
                                                                <asp:ImageButton ID="img_btnAddCampoContatore" runat="server" ImageUrl="~/AdminTool/Images/aggiungi.gif"
                                                                    ToolTip="Aggiunge il  campo selezionato" OnClick="img_btnAddCampoContatore_Click" />
                                                                <asp:ImageButton ID="img_btnDelCampoContatore" runat="server" ImageUrl="~/AdminTool/Images/elimina.gif"
                                                                    ToolTip="Resetta formato contatore" OnClick="img_btnDelCampoContatore_Click" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Azzera inizio intervallo
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Azzera_Anno" runat="server" />
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="height: 25px; width: 80px;" align="right" width="80">Formato
                                                            </td>
                                                            <td style="height: 25px; padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_formatoContatore" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <%--<TD style="HEIGHT: 25px; PADDING-LEFT: 5px">
                                                                    <asp:Label ID="lbl_formatoContatore" runat="server" Font-Bold="True" ForeColor="#C00000" Font-Size="X-Small"></asp:Label></TD>	--%>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro">Contatore da ricercare
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Contatore_visibile" runat="server" CssClass="testo_grigio_scuro"
                                                                    Text="Visualizza colonna contatore da ricercare" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro">Tipo Contatore
                                                            </td>
                                                            <td colspan="1" valign="top">
                                                                <asp:RadioButtonList ID="rbl_tipoContatore" CssClass="testo_grigio_scuro" RepeatDirection="Horizontal"
                                                                    runat="server">
                                                                    <asp:ListItem Selected="True">Tipologia</asp:ListItem>
                                                                    <asp:ListItem>AOO</asp:ListItem>
                                                                    <asp:ListItem>RF</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                                <asp:RadioButtonList ID="RadioButtonContatore" runat="server" CssClass="testo_grigio_scuro"
                                                                    RepeatDirection="Vertical" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonContatore_SelectedIndexChanged">
                                                                    <asp:ListItem>Classico</asp:ListItem>
                                                                    <asp:ListItem>Custom</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="left" colspan="2" valign="top">
                                                                <br />
                                                                <br />
                                                                <br />
                                                                <br />
                                                                <br />
                                                                <asp:Label ID="lblDataInizio" runat="server" Text="Data iniziale custom" Visible="False"></asp:Label>
                                                                &nbsp;<asp:TextBox ID="DataInizio" runat="server" Visible="False" Width="85px" CssClass="testo_grigio_scuro">
                                                                </asp:TextBox>
                                                                <br />
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ForeColor="Red"
                                                                    ControlToValidate="DataInizio" ValidationGroup="check" ErrorMessage="La data dev'essere in gg/mm/aaaa"
                                                                    ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$">
                                                                </asp:RegularExpressionValidator>
                                                                <br />
                                                                <asp:Label ID="lblDataFine" runat="server" Text="Data finale custom" Visible="False"></asp:Label>
                                                                &nbsp;&nbsp;
                                                                <asp:TextBox ID="DataFine" runat="server" CssClass="testo_grigio_scuro" Visible="False"
                                                                    Width="85px">
                                                                </asp:TextBox>
                                                                <br />
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="DataFine"
                                                                    ErrorMessage="La data dev'essere in gg/mm/aaaa" ForeColor="Red" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$"
                                                                    ValidationGroup="check">
                                                                </asp:RegularExpressionValidator>
                                                                &nbsp;&nbsp;
                                                            </td>
                                                            <td class="testo_grigio_scuro" colspan="2">Incremento differito
                                                                <asp:CheckBox ID="cb_ContaDopo" runat="server" />
                                                                <asp:Panel ID="pnl_repertori" Visible="false" runat="server">
                                                                    <asp:Label ID="lbl_repertorio" runat="server">Repertorio</asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                    <asp:CheckBox ID="cb_Repertorio" runat="server" />
                                                                </asp:Panel>
                                                                <asp:HiddenField ID="hiddenIdOggetto" runat="server" />
                                                                <!--<asp:ImageButton ID="btn_associaRuolo" runat="server" CommandName="Visibilita" ImageUrl="~/AdminTool/Images/utenti.gif"/> -->
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersContatore" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_4" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_4" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersMenuATendina" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaMenuATendina" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Elenco valori
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_valoriMenuATendina" runat="server" CssClass="testo" Width="180px"
                                                                    AutoPostBack="True">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td rowspan="4">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgMenuATendina.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Valore *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_valoreMenuATendina" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 80px" align="right" width="80">Default
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Default_MenuATendina" runat="server" AutoPostBack="True" OnCheckedChanged="cb_Default_MenuATendina_CheckedChanged"></asp:CheckBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Obbligatorio_MenuATendina" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Disabilitato
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Disabilitato_MenuATendina" runat="server" AutoPostBack="True"
                                                                    OnCheckedChanged="cb_Disabilitato_MenuATendina_CheckedChanged" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_Ricerca_MenuATendina" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td align="center"></td>
                                                            <td class="testo_grigio_scuro">&nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersMenuATendina" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>&nbsp;
                                                                <asp:Button ID="btn_aggiungiValoreMenuATendina" runat="server" CssClass="testo_btn_p"
                                                                    Text="Aggiungi Valore"></asp:Button>&nbsp;
                                                                <asp:Button ID="btn_eliminaValoreMenuATendina" runat="server" CssClass="testo_btn_p"
                                                                    Text="Elimina Valore"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_5" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_5" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersData" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" class="testo">
                                                                <asp:TextBox ID="txt_etichettaData" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="right" width="108">Formato Ora
                                                            </td>
                                                            <td style="padding-left: 5px" class="testo">
                                                                <asp:DropDownList ID="ddl_formatoOra" runat="server" CssClass="testo" Width="200">
                                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                                    <asp:ListItem Value="hh:mm" Text="hh:mm"></asp:ListItem>
                                                                    <asp:ListItem Value="hh:mm:ss" Text="hh:mm:ss"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td rowspan="3">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgData.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                            </td>
                                                            <td class="testo" colspan="3">
                                                                <asp:CheckBox ID="cb_Obbligatorio_Data" runat="server"></asp:CheckBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                            </td>
                                                            <td class="testo" colspan="3">
                                                                <asp:CheckBox ID="cb_Ricerca_Data" runat="server"></asp:CheckBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersData" runat="server" CssClass="testo_btn_p" Text="Conferma"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_6" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_6" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersCorrispondente" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaCorr" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 100px" align="right" width="80">Ruolo predefinito
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_ruoloPredefinito" runat="server" CssClass="testo" Width="300px">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td rowspan="3">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgCorrispondente.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Obbligatorio_Corr" runat="server"></asp:CheckBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 100px" align="right" width="80">Tipo Ricerca
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_tipoRicerca" runat="server" CssClass="testo" Width="120px">
                                                                    <asp:ListItem></asp:ListItem>
                                                                    <asp:ListItem>INTERNI/ESTERNI</asp:ListItem>
                                                                    <asp:ListItem>INTERNI</asp:ListItem>
                                                                    <asp:ListItem>ESTERNI</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                            </td>
                                                            <td cssclass="testo">
                                                                <asp:CheckBox ID="cb_Ricerca_Corr" runat="server"></asp:CheckBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <asp:Button ID="btn_ConfermaPersCorrispondente" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_7" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_7" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersLink" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:TextBox ID="txt_etichettaLink" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                            </td>
                                                            <td class="testo_grigio_scuro" style="width: 100px" align="right" width="80">Tipo link
                                                            </td>
                                                            <td style="padding-left: 5px" cssclass="testo">
                                                                <asp:DropDownList ID="ddl_tipoLink" runat="server" CssClass="testo" AutoPostBack="True"
                                                                    Width="300px">
                                                                    <asp:ListItem Value="INTERNO" Text="Interno"></asp:ListItem>
                                                                    <asp:ListItem Value="ESTERNO" Text="Esterno"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgLink.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" class="testo_grigio_scuro" style="width: 108px" width="108">Obbligatorio
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cb_obbligatorioLink" runat="server" />
                                                            </td>
                                                            <td align="left" class="testo_grigio_scuro" style="width: 108px" width="108">Tipo oggetto target
                                                            </td>
                                                            <td cssclass="testo" style="padding-left: 5px">
                                                                <asp:DropDownList ID="ddl_tipoObjLink" runat="server" CssClass="testo" Width="300px">
                                                                    <asp:ListItem Text="Documento" Value="DOCUMENTO"></asp:ListItem>
                                                                    <asp:ListItem Text="Fascicolo" Value="FASCICOLO"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <tr>
                                                                <td align="left" colspan="4">
                                                                    <asp:Button ID="btn_ConfermaPersLink" runat="server" CssClass="testo_btn_p" Text="Conferma" />
                                                                </td>
                                                                <td align="right" class="testo_grigio_scuro">Ordinamento&nbsp;
                                                                    <asp:ImageButton ID="btn_up_8" runat="server" BorderColor="#810D06" BorderStyle="Solid"
                                                                        BorderWidth="1px" ImageUrl="../Images/up.bmp" />&nbsp;
                                                                    <asp:ImageButton ID="btn_down_8" runat="server" BorderColor="#810D06" BorderStyle="Solid"
                                                                        BorderWidth="1px" ImageUrl="../Images/down.bmp" />&nbsp;
                                                                </td>
                                                            </tr>
                                                    </table>
                                                </asp:Panel>
                                                <asp:Panel ID="Panel_PersSeparatore" runat="server">
                                                <table width="100%">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">
                                                            Etichetta *
                                                        </td>
                                                        <td style="padding-left: 5px" cssclass="testo">
                                                            <asp:TextBox ID="txt_etichettaSeparatore" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                        </td>
                                                        <td class="testo_grigio_scuro" style="width: 108px" align="right" width="108">
                                                        </td>
                                                        <td style="padding-left: 5px" cssclass="testo">
                                                        </td>
                                                        <td rowspan="3">
                                                            <img style="border-right: #810d06 1px solid; border-top: #810d06 1px solid; border-left: #810d06 1px solid;
                                                                border-bottom: #810d06 1px solid" src="../Images/imgSeparatore.bmp" align="right">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">
                                                        </td>
                                                        <td cssclass="testo" colspan="3">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">
                                                        </td>
                                                        <td cssclass="testo" colspan="3">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left" colspan="4">
                                                            <asp:Button ID="btn_ConfermaPersSeparatore" runat="server" CssClass="testo_btn_p"
                                                                Text="Conferma"></asp:Button>
                                                        </td>
                                                        <td class="testo_grigio_scuro" align="right">
                                                            Ordinamento&nbsp;
                                                            <asp:ImageButton ID="btn_up_10" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                            <asp:ImageButton ID="btn_down_10" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                                <asp:Panel ID="Panel_PersOggEsterno" runat="server">
                                                    <table width="100%">
                                                        <tr>
                                                            <td valign="top">
                                                                <table>
                                                                    <tr>
                                                                        <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Etichetta *
                                                                        </td>
                                                                        <td style="padding-left: 5px" cssclass="testo">
                                                                            <asp:TextBox ID="txt_etichettaOggEsterno" runat="server" CssClass="testo" Width="200"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Obbligatorio
                                                                        </td>
                                                                        <td cssclass="testo" colspan="3">
                                                                            <asp:CheckBox ID="cb_obbligatorioOggEsterno" runat="server"></asp:CheckBox>
                                                                        </td>
                                                                        <tr>
                                                                            <td class="testo_grigio_scuro" style="width: 108px" align="left" width="108">Ricerca
                                                                            </td>
                                                                            <td cssclass="testo" colspan="3">
                                                                                <asp:CheckBox ID="cb_ricercaOggEsterno" runat="server"></asp:CheckBox>
                                                                            </td>
                                                                        </tr>
                                                                </table>
                                                            </td>
                                                            <td valign="top">
                                                                <uc1:IntegrationAdapter ID="intAdapter_OggEsterno" CssClass="testo" runat="server"
                                                                    View="ADMIN" />
                                                            </td>
                                                            <td valign="top" align="right">
                                                                <img style="border-bottom: #810d06 1px solid; border-left: #810d06 1px solid; border-right: #810d06 1px solid; border-top: #810d06 1px solid;"
                                                                    src="../Images/imgOggEsterno.bmp" align="right">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2">
                                                                <asp:Button ID="btn_ConfermaPersOggEsterno" runat="server" CssClass="testo_btn_p"
                                                                    Text="Conferma"></asp:Button>
                                                            </td>
                                                            <td class="testo_grigio_scuro" align="right">Ordinamento&nbsp;
                                                                <asp:ImageButton ID="btn_up_OggEsterno" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/up.bmp"></asp:ImageButton>&nbsp;
                                                                <asp:ImageButton ID="btn_down_OggEsterno" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                                                    BorderStyle="Solid" ImageUrl="../Images/down.bmp"></asp:ImageButton>&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <!-- FINE : PANNELLI PERS TEXTBOX - CHECKBOX - RADIOBUTTON - COMBOBOX - CONTATORE -->
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <!-- FINE : PANNELLI PERS TEXTBOX - CHECKBOX - RADIOBUTTON - COMBOBOX - CONTATORE - CORRISPONDENTE - LINK-->
                            </td>
                        </tr>
                        <tr>
                            <td height="3"></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel_Privato" runat="server" Visible="False" BorderWidth="1px" BorderColor="#810D06"
                                    BorderStyle="Solid">
                                    <table width="99.5%">
                                        <tr>
                                            <td class="titolo_pnl">
                                                <table cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td class="titolo_pnl">Personalizzazione Tipo Documento&nbsp;-&nbsp;
                                                            <asp:Label ID="lbl_TipoFascPr" runat="server">Label</asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:ImageButton ID="pulsante_chiudi_privato" runat="server" ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="cb_ModPrivato" runat="server" CssClass="testo_grigio_scuro" Text="Privato"
                                                    TextAlign="Left" Checked="False"></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btn_confermaPrivato" runat="server" CssClass="testo_btn_p" Text="Conferma"></asp:Button>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <!-- Panel per mesi Conservazione -->
                            <td height="3"></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="pnlTemplateStruttura" runat="server" Visible="False" BorderWidth="1px" BorderColor="#810D06"
                                    BorderStyle="Solid">
                                    <table width="99.5%">
                                        <tr>
                                            <td class="titolo_pnl">
                                                <table cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td class="titolo_pnl">Personalizzazione Template Struttura&nbsp;-&nbsp;
                                                            <asp:Label ID="lbTipoFascicoloTemplate" runat="server"></asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:ImageButton ID="btAnnullaTemplate" runat="server" ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style="height: 25px">
                                                <asp:DropDownList ID="ddTemplateStruttura" CssClass="testo" Width="250px" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr></tr>
                                        <tr>
                                            <td align="center" style="height: 25px">
                                                <asp:Button ID="btConfermaTemplate" runat="server" CssClass="testo_btn_p" Text="Conferma"></asp:Button>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <!-- Panel per mesi Conservazione -->
                            <td height="3"></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel_MesiCons" runat="server" Visible="False" BorderWidth="1px" BorderColor="#810D06"
                                    BorderStyle="Solid">
                                    <table width="99.5%">
                                        <tr>
                                            <td class="titolo_pnl" colspan="2">
                                                <table cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td class="titolo_pnl">Personalizzazione Tipo Documento&nbsp;-&nbsp;
                                                            <asp:Label ID="LblTipoFascMesiCons" runat="server">Label</asp:Label>
                                                        </td>
                                                        <td></td>
                                                        <td align="right">
                                                            <asp:ImageButton ID="pulsante_chiudi_MesiCons" runat="server" ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" width="20%">
                                                <asp:Label ID="lblModMesiCons" runat="server" CssClass="testo_grigio_scuro" Width="100%">Mesi conservazione</asp:Label>
                                            </td>
                                            <td align="left" width="90%">
                                                <asp:TextBox ID="txt_ModMesiCons" runat="server" CssClass="testo" MaxLength="6" Width="10%"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btn_confermaMesiCons" runat="server" CssClass="testo_btn_p" Text="Conferma"></asp:Button>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <!-- FINE: PANNELLI -->
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <cc2:MessageBox ID="msg_Privato" runat="server"></cc2:MessageBox>
                </td>
            </tr>
        </table>
        <input id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server" />
    </form>
</body>
</html>
