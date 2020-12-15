<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<%@ Page Language="c#" CodeBehind="ProspettiRiepilogativi_LF.aspx.cs" AutoEventWireup="false"
    Inherits="ProspettiRiepilogativi.ProspettiRiepilogativi_LF" %>

<%@ Register Src="~/UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Corrispondente.ascx" TagName="Corrispondente" TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>

<%@ Register Src="~/ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>

<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<%@ Register src="~/UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>DOCSPA > ProspettiRiepilogativi_LF</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <link href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="javascript">
        function OpenFile(typeFile) {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

            try {
                fso = FsoWrapper_CreateFsoObject();
                // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

                path = fso.GetSpecialFolder(2).Path;

                if (typeFile == "PDF") {
                    filePath = path + "\\exportDocspa.pdf";
                    applName = "Adobe Acrobat";
                }
                else if (typeFile == "XLS") {
                    filePath = path + "\\exportDocspa.xls";
                    applName = "Microsoft Excel";
                }
                else if (typeFile == "Model") {
                    filePath = path + "\\exportDocspa.xls";
                    applName = "Microsoft Excel";
                }
                else if (typeFile == "ODS") {
                    filePath = path + "\\exportDocspa.ods";
                    applName = "Open Office";
                }

                exportUrl = "..\\..\\exportDati\\exportDatiPage.aspx";
                http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", exportUrl, false);
                http.send();

                var content = http.responseBody;

                if (content != null) {
                    AdoStreamWrapper_SaveBinaryData(filePath, content);

                    ShellWrappers_Execute(filePath);

                    self.close();
                }
            }
            catch (ex) {
                alert(ex.message.toString());

                //	alert("Impossibile aprire il file generato!\n\nPossibili motivi:\n- il browser non è abilitato ad eseguire controlli ActiveX\n- il sito intranet DocsPA non compare tra i siti attendibili di questo computer;\n- estensione '"+typeFile+"' non associata all'applicazione "+applName+";\n- "+applName+" non installato su questo computer;\n- applicazione "+applName+" relativa ad esportazioni precedentente effettuate ancora attiva.");					
            }
        }

        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try { return new ActiveXObject(objectType); }
            catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
        }

        //RICERCA RUOLI SOTTOPOSTI
        function _ApriRubricaRicercaRuoliSottoposti() {
            var r = new Rubrica();
            r.MoreParams = "tipo_corr=" + "U";
            r.CallType = r.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI;
            var res = r.Apri();
        }
    </script>
    <script language="javascript" id="btnStampa" event="onclick()" for="btnStampa">
		top.principale.frames[1].location='waitingpage.htm';
    </script>
</head>
<body bottommargin="0" topmargin="0" leftmargin="0" rightmargin="0" ms_positioning="GridLayout">
    <form id="FormProspettiRiepilogativi_LF" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManagerProspetti" AsyncPostBackTimeout="360000" runat="server">
    </asp:ScriptManager>
    <table id="contenitore" style="z-index: 101; left: 8px; width: 417px; position: absolute;
        top: 0px; height: 264px" bordercolor="#000000" cellspacing="0" cellpadding="0"
        width="417" border="0">
        <tr>
            <td align="center">
                <table class="contenitore" id="contenitore_report" style="width: 408px; height: 239px"
                    cellspacing="0" cellpadding="0" width="408" border="0">
                    <tr>
                        <td align="center">
                            <asp:Label ID="lb_report" runat="server" Width="168px" CssClass="testo_grigio_scuro">PROSPETTI RIEPILOGATIVI</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 200px">
                            <asp:RadioButtonList ID="rb_prospettiRiepilogativi" runat="server" Width="408px"
                                CssClass="testo_grigio" AutoPostBack="True">
                                <asp:ListItem Value="reportAnnualeDoc" Selected="True">Report annuale sui documenti</asp:ListItem>
                                <asp:ListItem Value="reportDocClassificati">Report sui documenti classificati</asp:ListItem>
                                <asp:ListItem Value="reportAnnualeDocTrasmAmm">Report annuale sui documenti spediti ad altre amministrazioni</asp:ListItem>
                                <asp:ListItem Value="reportAnnualeDocTrasmAOO">Report annuale sui documenti spediti ad altre AOO</asp:ListItem>
                                <asp:ListItem Value="reportAnnualeFasc">Report annuale sui fascicoli su titolario attivo</asp:ListItem>
                                <asp:ListItem Value="reportAnnualeFascXTit">Report annuale sui fascicoli per voce di titolario</asp:ListItem>
                                <asp:ListItem Value="tempiMediLavFasc">Tempi medi di lavorazione fascicoli</asp:ListItem>
                                <asp:ListItem Value="ReportDocXSede">Report annuale sui documenti per sede</asp:ListItem>
                                <asp:ListItem Value="ReportDocXUo">Report annuale sui documenti protocollati per UO</asp:ListItem>
                                <asp:ListItem Value="reportNumFasc">Report conteggio fascicoli procedimentali</asp:ListItem>
                                <asp:ListItem Value="reportNumDocInFasc">Report fascicoli procedimentali e documenti contenuti</asp:ListItem>
                                <%--<asp:ListItem Value="stampaProtArma">Stampa protocollo Arma</asp:ListItem>
                                <asp:ListItem Value="stampaDettaglioPratica">Stampa Dettaglio Pratica</asp:ListItem>
                                <asp:ListItem Value="stampaGiornaleRiscontri">Stampa Giornale Riscontri</asp:ListItem>
                                <asp:ListItem Value="documentiSpediti">Documenti spediti</asp:ListItem>--%>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td height="3">
            </td>
        </tr>
        <tr>
            <td align="center" height="3">
                <table class="contenitore" id="Filtri" style="height: 41px" cellspacing="0" cellpadding="0"
                    width="410" border="0">
                    <asp:Panel ID="pnl_amm" runat="server" Visible="true">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;Amministrazione *&nbsp;&nbsp;
                            </td>
                            <td style="height: 21px">
                                <asp:DropDownList ID="ddl_amm" TabIndex="1" runat="server" Width="200px" CssClass="testo_grigio"
                                    AutoPostBack="True" Height="8px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="panel_reg" runat="server" Visible="true">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 3px">
                                &nbsp;Registro *
                            </td>
                            <td style="height: 3px">
                                <asp:DropDownList ID="ddl_registro" TabIndex="2" runat="server" Width="200px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="panel_tit_ann_fasc" runat="server" Visible="true">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 3px">
                                &nbsp;Titolario *
                            </td>
                            <td style="height: 3px">
                                <asp:DropDownList ID="ddl_titolari_report_annuale" TabIndex="2" runat="server" Width="200px"
                                    CssClass="testo_grigio" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="panel_sede" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 18px" height="18">
                                &nbsp;Sede
                            </td>
                            <td style="height: 18px">
                                <asp:DropDownList ID="ddl_sede" TabIndex="3" runat="server" CssClass="testo_grigio"
                                    Width="200px" AutoPostBack="True">
                                    <asp:ListItem Value="">Tutte le Sedi</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_anno" runat="server" Visible="true">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" height="3">
                                &nbsp;Anno *
                            </td>
                            <td style="height: 21px">
                                <asp:DropDownList ID="ddl_anno" TabIndex="4" runat="server" Width="88px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_mese" runat="server" Visible="true">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" height="3">
                                &nbsp;Mese
                            </td>
                            <td>
                                <asp:DropDownList ID="ddl_Mese" runat="server" CssClass="testo_grigio" Width="88px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_modalita" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" height="3">
                                &nbsp;Modalità
                            </td>
                            <td>
                                <asp:RadioButtonList ID="rb_modalita" runat="server" CssClass="testo_grigio" Width="168px"
                                    AutoPostBack="True" Height="24px" CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Compatta" Selected="True">Compatta</asp:ListItem>
                                    <asp:ListItem Value="Estesa">Estesa</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_regCC" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 3px">
                                &nbsp;Registro *
                            </td>
                            <td style="height: 3px">
                                <asp:DropDownList ID="ddl_regCC" TabIndex="2" runat="server" Width="200px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_protArma" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" height="3">
                                &nbsp;Titolario*
                            </td>
                            <td style="height: 21px">
                                <asp:DropDownList ID="ddl_et_titolario" TabIndex="4" runat="server" Width="88px"
                                    CssClass="testo_grigio" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_DettPratica" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" height="3">
                                &nbsp;Pratica*
                            </td>
                            <td>
                                <asp:TextBox ID="txt_NumPratica" runat="server" AutoPostBack="true" Width="40px"
                                    Height="18px" CssClass="testo_grigio"></asp:TextBox>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_DocSpediti" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 50px; height: 21px" height="3">
                                &nbsp;Data&nbsp;Spedizione*
                            </td>
                            <td>
                                <asp:Label ID="lbl_dataSpedDa" runat="server" CssClass="testo_grigio" Width="8px">Da</asp:Label>
                                <uc1:Calendario ID="txt_dataSpedDa" runat="server" Visible="true" PaginaChiamante="prospetti" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbl_dataSpedA" runat="server" CssClass="testo_grigio" Width="8px">a</asp:Label>
                                <uc1:Calendario ID="txt_dataSpedA" runat="server" Visible="true" PaginaChiamante="prospetti" />
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="width: 150px; height: 21px" height="3">
                                &nbsp;Conferma di protocollazione*
                            </td>
                            <td height="25">
                                <asp:RadioButtonList ID="rb_confermaSpedizione" runat="server" CssClass="testo_grigio"
                                    RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1">Sì&nbsp;&nbsp;</asp:ListItem>
                                    <asp:ListItem Value="0">No&nbsp;&nbsp;</asp:ListItem>
                                    <asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_sottoposto" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;Proprietario*
                            </td>
                            <td>
                                <input id="hd_systemIdCorrSott" type="hidden" size="1" name="hd_systemIdSott" runat="server">
                                <asp:RadioButtonList ID="rb_scelta_sott" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal"
                                    Style="height: 21px; float: left;" AutoPostBack="True">
                                    <asp:ListItem Value="UO" Selected="True">UO</asp:ListItem>
                                    <asp:ListItem Value="RF">RF</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:Image ID="btn_img_sott_rubr" runat="server" ToolTip="Seleziona una uo dalla rubrica"
                                    ImageUrl="../../images/proto/rubrica.gif" AlternateText="Seleziona una uo dalla rubrica"
                                    Style="float: right; margin-right: 20px;"></asp:Image>
                            </td>
                        </tr>
                        <asp:Panel ID="pnl_scelta_uo" runat="server" Visible="false">
                            <tr>
                                <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                    &nbsp;UO*&nbsp;
                                </td>
                                <td>
                                    <asp:TextBox ID="txt1_corr_sott" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                        Width="75px"></asp:TextBox>&nbsp;
                                    <asp:TextBox ID="txt2_corr_sott" runat="server" CssClass="testo_grigio" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                    &nbsp;
                                </td>
                                <td>
                                    <asp:CheckBox ID="chk_sottoposti" runat="server" AutoPostBack="True" Text="visualizza sottoposti"
                                        class="titolo_scheda" />
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnl_scelta_rf" runat="server" Visible="false">
                            <tr>
                                <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                    &nbsp;RF*
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddl_rf" TabIndex="4" runat="server" Width="232px" CssClass="testo_grigio"
                                        AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;Data creazione
                            </td>
                            <td class="testo_grigio">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;
                                <asp:DropDownList ID="ddl_tipo_data_creazione" runat="server" AutoPostBack="True"
                                    CssClass="testo_grigio" Width="105px">
                                    <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="testo_grigio">
                                <asp:Label ID="lbl_dal_apertura" runat="server" Visible="false">Dal</asp:Label>
                                <uc1:Calendario ID="cld_creazione_dal" runat="server" PaginaChiamante="prospetti" />
                                <asp:Label ID="lbl_al_apertura" runat="server" Visible="false">al</asp:Label>
                                <uc1:Calendario ID="cld_creazione_al" runat="server" PaginaChiamante="prospetti"
                                    Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;Data chiusura
                            </td>
                            <td class="testo_grigio">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;
                                <asp:DropDownList ID="ddl_tipo_data_chiusura" runat="server" AutoPostBack="True"
                                    CssClass="testo_grigio" Width="105px">
                                    <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="testo_grigio">
                                <asp:Label ID="lbl_dal_chiusura" runat="server" Visible="false">Dal</asp:Label>
                                <uc1:Calendario ID="cld_chiusura_dal" runat="server" PaginaChiamante="prospetti" />
                                <asp:Label ID="lbl_al_chiusura" runat="server" Visible="false">al</asp:Label>
                                <uc1:Calendario ID="cld_chiusura_al" runat="server" PaginaChiamante="prospetti" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 21px" align="left">
                                &nbsp;Titolario*
                            </td>
                            <td>
                                <asp:DropDownList ID="ddl_titolari" TabIndex="4" runat="server" Width="232px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_FiltriCDC" runat="server" Visible="false">
                        <tr>
                        </tr>
                        <asp:Panel ID="pnlContrData" runat="server">
                            <asp:Panel ID="pnlTipoControlloContrData" runat="server" Visible="false">
                                <tr>
                                    <td class="titolo_scheda">
                                        <asp:Label ID="Label2" runat="server" Text="Controllo" Style="padding-left: 3px;
                                            width: 40%;" />
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblTipoControlloContrData" runat="server" RepeatDirection="Horizontal"
                                            CssClass="testo_grigio" AutoPostBack="true">
                                            <asp:ListItem Value="1">Successivo</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="0">Preventivo</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </asp:Panel>
                            <tr>
                                <td class="titolo_scheda" style="padding-left: 3px; width: 20%;">
                                    <asp:Label ID="lbl_Data" runat="server" Text="Data *" />
                                </td>
                                <td>
                                    <asp:Label ID="lbl_DataDa" runat="server" Text="da" CssClass="testo_grigio" />
                                    <uc1:Calendario ID="dataDa" runat="server" PaginaChiamante="prospetti" />
                                    <asp:Label ID="lbl_DataA" runat="server" Text="a" CssClass="testo_grigio" />
                                    <uc1:Calendario ID="dataA" runat="server" PaginaChiamante="prospetti" />
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td class="titolo_scheda" style="padding-left: 3px; width: 20%;">
                                <asp:Label ID="lbl_Uffici" runat="server" Text="Uffici" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddl_uffici" runat="server" CssClass="testo_grigio" Width="98%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="padding-left: 3px; width: 20%;">
                                <asp:Label ID="lbl_Magistrato" runat="server" Text="Magistrato" />
                            </td>
                            <td>
                                <uc2:Corrispondente ID="corr_magistrato" runat="server" CSS_CODICE="testo_grigio"
                                    CSS_DESCRIZIONE="testo_grigio" Width="98%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="padding-left: 3px; width: 20%;">
                                <asp:Label ID="lbl_Revisore" runat="server" Text="Revisore" />
                            </td>
                            <td>
                                <uc2:Corrispondente ID="corr_revisore" runat="server" CSS_CODICE="testo_grigio" CSS_DESCRIZIONE="testo_grigio"
                                    Width="98%" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_FiltriCDCElencoDecreti" runat="server" Visible="false">
                        <asp:Panel ID="pnlControlloElencoDecreti" runat="server" Visible="false">
                            <tr>
                                <td class="titolo_scheda">
                                    <asp:Label ID="Label1" runat="server" Text="Controllo" Style="padding-left: 3px;
                                        width: 40%;" />
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="rblControlloElencoDecreti" runat="server" RepeatDirection="Horizontal"
                                        CssClass="testo_grigio" AutoPostBack="true">
                                        <asp:ListItem Value="1">Successivo</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="0">Preventivo</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td class="titolo_scheda" style="padding-left: 3px;">
                                <asp:Label ID="lbl_Elenco" runat="server" Text="Numero Elenco *" />
                            </td>
                            <td>
                                <asp:TextBox ID="txt_Elenco" runat="server" AutoPostBack="true" Width="100px" Height="18px"
                                    CssClass="testo_grigio"></asp:TextBox>
                            </td>
                        </tr>
                    </asp:Panel>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
        <tr>
            <td align="center" bgcolor="#ffffff">
                <table id="bottoniera" cellspacing="0" cellpadding="0" width="300" border="0">
                    <tr>
                        <td height="15">
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:ImageButton ID="btnStampa" TabIndex="5" runat="server" SkinID="stampa_Attivo"
                                ImageAlign="Baseline"></asp:ImageButton>
                            <asp:ImageButton ID="btn_zoom" TabIndex="6" runat="server" SkinID="btnZoom"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <iframe id="iframeVisUnificata" style="width: 0px; height: 0px;" scrolling="auto"
        frameborder="0" runat="server" visible="true"></iframe>
        <uc1:ShellWrapper ID="shellWrapper" runat="server" />
            <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
            <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    </form>
</body>
</html>
