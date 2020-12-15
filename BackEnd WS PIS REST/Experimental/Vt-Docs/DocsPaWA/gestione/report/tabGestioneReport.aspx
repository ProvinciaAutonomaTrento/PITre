<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<%@ Page Language="c#" CodeBehind="tabGestioneReport.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.gestione.report.tabGestioneReport" %>

<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper"
    TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Src="CalendarReport.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
    <link href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" id="btn_stampaRegistro_click" event="onclick()" for="btn_stampaRegistro">
			window.document.body.style.cursor='wait';
			//disabilito la pag
			//document.getElementById("rb_report").disabled=true;
			document.getElementById("ddl_report").disabled=true;
			document.getElementById("ddl_registri").disabled=true;
			document.getElementById("btn_stampaRegistro").style.display='none';
			document.getElementById("btn_stampaRegistroDisabled").style.display='';
			WndWaitStampaReg();
    </script>
    <script language="JavaScript">

        ns = window.navigator.appName == "Netscape"
        ie = window.navigator.appName == "Microsoft Internet Explorer"

        var w = window.screen.width;
        var h = window.screen.height;
        var new_w = (w - 100) / 2;
        var new_h = (h - 400) / 2;

        function openReportXls() {
            var pageHeight = window.screen.availHeight;
            var pageWidth = window.screen.availWidth;
            window.open('stampaReportXLS.aspx', '', 'width=' + pageWidth + ',height=' + pageHeight + ',toolbar=no,statusbar=no,menubar=no,resizable=yes,scrollbars=auto');
        }

        function openDesc() {

            try {
                if (ns) {
                    showbox = document.layers[1]
                    showbox.visibility = "show";
                    // showbox.top=63;

                    var items = 1;
                    for (i = 1; i <= items; i++) {
                        elopen = document.layers[i]
                        if (i != (1)) {
                            elopen.visibility = "hide"
                        }
                    }
                }

                if (ie) {
                    curEl = event.toElement
                    // curEl.style.background = "#C08682"   

                    showBox = document.all.descreg;
                    showBox.style.visibility = "visible";
                }
            }
            catch (e)
                    { return false; }
        }

        function closeDesc() {
            try {
                var items = 1
                for (i = 0; i < items; i++) {
                    if (ie) {
                        document.all.descreg.style.visibility = "hidden"
                    }
                    if (ns) {
                        document.layers[i].visibility = "hide"
                    }
                }
            }
            catch (e)
                    { return false; }
        }

        function nascondi() {
            document.getElementById('btn_stampaRegistroDisabled').style.display = 'none';
        }

        function _ApriRubrica(target) {
            var r = new Rubrica();

            switch (target) {
                case "ruo":
                    r.CallType = r.CALLTYPE_STAMPA_REG_UO;
                    break;
            }
            var res = r.Apri();

        }

        function OpenFileXLS() {
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

                filePath = path + "\\ReportAvanzatoDocsPA.xls";
                applName = "Microsoft Excel";
                exportUrl = "stampaReportXLS.aspx";

                http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", exportUrl, false);
                http.send();

                var content = http.responseBody;

                if (content != null) {
                    AdoStreamWrapper_SaveBinaryData(filePath, content);

                    ShellWrappers_Execute(filePath);
                }
            }
            catch (ex) {
                alert(ex.message.toString());
            }
        }

        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try { return new ActiveXObject(objectType); }
            catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
        }

        function apriPopupAnteprima() {
            window.showModalDialog('../../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicerche.aspx', '', 'dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function StampaRisultatoRicerca() {
            var args = new Object;
            args.window = window;
            //window.showModalDialog("../../exportDati/exportDatiSelection.aspx?export=Stampe_DG",args,"dialogWidth:450px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
            window.showModalDialog("../../exportDati/exportDatiSelection.aspx?export=doc", args, "dialogWidth:450px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }

        // Funzione per la creazione del file con le etichette da stampare
        function StampaFascette() {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

            try {
                // Creo l'oggetto FSO
                fso = FsoWrapper_CreateFsoObject();

                // Richiedo la windows folder
                path = fso.GetSpecialFolder(2).Path;

                // Imposto come nome del file etichetteDocsPa.rtf
                filePath = path + "etichetteDocsPa.rtf";
                applName = "Microsoft Word";

                exportUrl = "../../fascicolo/exportEtichetteFasc.aspx";
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
    </script>
</head>
<body>
    <form id="tabGestioneReport" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Gestione Report" />
    <uc1:ShellWrapper ID="shellWrapper" runat="server" />
    <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
    <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    <table height="90%" cellspacing="0" cellpadding="0" width="408">
        <tr height="5%" valign="middle">
            <td align="center">
                <table class="info" cellspacing="0" cellpadding="0" width="405" border="0">
                    <tr height="22" valign="middle">
                        <td class="testo_grigio_scuro" width="15%">
                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Registro
                        </td>
                        <td valign="middle" width="60%">
                            &nbsp;
                            <asp:DropDownList ID="ddl_registri" runat="server" CssClass="testo_grigio" AutoPostBack="True"
                                Width="134px" OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:Image ID="icoReg" onmouseover="openDesc()" onmouseout="closeDesc()" runat="server"
                                BorderColor="Gray" BorderStyle="Solid" BorderWidth="0px" ImageUrl="../../images/proto/ico_registro.gif"
                                ImageAlign="AbsMiddle"></asp:Image>
                        </td>
                        <td align="right" width="25%" valign="middle" class="testo_grigio_scuro">
                            <table cellpadding="0" width="100%" height="100%" cellspacing="0" border="0">
                                <tr>
                                    <td align="right" valign="middle" class="testo_grigio_scuro">
                                        <asp:Label ID="lbl_ET_stato" Text="Stato" runat="server"></asp:Label>
                                    </td>
                                    <td align="right" valign="middle" class="testo_grigio_scuro">
                                        <asp:Image ID="img_statoReg" runat="server" BorderWidth="1" ImageUrl="../../images/stato_giallo2.gif"
                                            Height="18" Width="52" ImageAlign="AbsMiddle"></asp:Image><img height="1" src="../../images/proto/spaziatore.gif"
                                                width="2" border="0">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td height="1">
            </td>
        </tr>
        <tr>
            <td valign="top" align="center">
                <table class="info" id="tbl_tab" height="100%" width="405" border="0">
                    <tr>
                        <td class="item_editbox" valign="middle" height="19">
                            <p class="boxform_item">
                                Selezionare il Report da stampare</p>
                        </td>
                    </tr>
                    <tr>
                        <td height="19" align="center">
                            <asp:DropDownList ID="ddl_report" runat="server" CssClass="testo_grigio" AutoPostBack="True"
                                Width="380" OnSelectedIndexChanged="ddl_report_SelectedIndexChanged">
                                <asp:ListItem Value="T" Selected="True">Titolario</asp:ListItem>
                                <asp:ListItem Value="E">Corrispondenti Esterni</asp:ListItem>
                                <asp:ListItem Value="TR">Trasmissioni UO</asp:ListItem>
                                <asp:ListItem Value="DR">Documenti Registro</asp:ListItem>
                                <asp:ListItem Value="DG">Documenti Non Protocollati</asp:ListItem>
                                <asp:ListItem Value="B">Buste</asp:ListItem>
                                <asp:ListItem Value="F">Fascette Fascicolo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Panel ID="pnlInput" Height="40px" Visible="False" runat="server">
                                <table class="info_grigio" style="width: 376px; height: 50px" cellspacing="0" cellpadding="0"
                                    width="376" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img border="0" height="1" src="../../images/proto/spaziatore.gif" width="8">
                                            <asp:Label ID="lblInput" runat="server" CssClass="testo_grigio_scuro">Codice Fascicolo</asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="6" border="0">
                                                <asp:TextBox ID="txtInput" runat="server" CssClass="testo_grigio"></asp:TextBox></p>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnl_trasmUO" Visible="False" runat="server">
                                <!-- tabella -->
                                <table class="info_grigio" style="width: 376px; height: 50px" cellspacing="0" cellpadding="0"
                                    width="376" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Oggetto
                                            trasmesso
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img border="0" height="1" src="../../images/proto/spaziatore.gif" width="8">
                                            <asp:DropDownList ID="DDLOggettoTab1" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                                Height="33">
                                                <asp:ListItem Selected="True" Value="D">Documento</asp:ListItem>
                                                <asp:ListItem Value="F">Fascicolo</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                &nbsp;
                                <table class="info_grigio" style="width: 376px; height: 50px" cellspacing="0" cellpadding="0"
                                    width="376" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data
                                            trasmissione
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="25">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">
                                            <asp:DropDownList ID="ddl_dataTrasm" runat="server" Width="110px" AutoPostBack="True"
                                                CssClass="testo_grigio">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                            <asp:Label ID="lbl_initdataTrasm" runat="server" Width="10px" CssClass="testo_grigio"
                                                Visible="False">Da</asp:Label>
                                            <uc4:Calendario ID="txt_initDataTrasm" runat="server" Visible="true" />
                                            &nbsp;
                                            <asp:Label ID="lbl_finedataTrasm" runat="server" Width="10px" CssClass="testo_grigio"
                                                Visible="False">A</asp:Label>
                                            <uc4:Calendario ID="txt_fineDataTrasm" runat="server" Visible="true" />
                                        </td>
                                    </tr>
                                </table>
                                &nbsp;
                                <table class="info_grigio" style="width: 376px; height: 58px" cellspacing="0" cellpadding="0"
                                    width="376" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Ragione
                                            trasmissione
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="25">
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">
                                                <asp:DropDownList ID="ddl_ragioni" runat="server" Width="200px" AutoPostBack="True"
                                                    CssClass="testo_grigio">
                                                </asp:DropDownList>
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <!--DOCUMENTI REGISTRO-->
                            <asp:Panel ID="pnl_DocumentiRegistro" Visible="False" runat="server">
                                <table class="info_grigio" id="Table1" style="height: 70px" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Numero
                                            protocollo
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td class="titolo_scheda">
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:DropDownList ID="ddl_numProt_E" runat="server" Width="110px" AutoPostBack="true"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp; &nbsp;
                                                <asp:Label ID="lblDAnumprot_E" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:Label>
                                                <asp:TextBox ID="txt_initNumProt_E" runat="server" Width="80px" CssClass="testo_grigio"></asp:TextBox>&nbsp;
                                                <asp:Label ID="lblAnumprot_E" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                <asp:TextBox ID="txt_fineNumProt_E" runat="server" Width="80px" CssClass="testo_grigio"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                                <br>
                                <table class="info_grigio" id="Table2" style="height: 57px" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" style="height: 19px" valign="middle" height="19">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data
                                            protocollo
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td>
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:DropDownList ID="ddl_dataProt_E" runat="server" Width="110px" AutoPostBack="true"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="lbl_initdataProt_E" runat="server" Width="10px" CssClass="testo_grigio">Da</asp:Label>
                                                <uc4:Calendario ID="txt_initDataProt_E" runat="server" Visible="true" />
                                                &nbsp;
                                                <asp:Label ID="lbl_finedataProt_E" runat="server" Width="10px" CssClass="testo_grigio">A</asp:Label>
                                                <uc4:Calendario ID="txt_fineDataProt_E" runat="server" Visible="true" />
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                                <!--tabella mittente/destinatario -->
                                <br>
                                    <table align="center" border="0" cellpadding="0" cellspacing="0" class="info_grigio"
                                        width="95%">
                                        <tr>
                                            <td class="titolo_scheda" height="19" style="width: 228px" valign="middle" width="228">
                                                <img border="0" height="1" src="../../images/proto/spaziatore.gif" width="8">Unità
                                                Organizzativa
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chk_uo" runat="server" CssClass="titolo_scheda" Text="Estendi alle UO Sottoposte"
                                                                Width="175px"></asp:CheckBox>
                                                        </td>
                                                        <td align="right" valign="middle" width="29">
                                                            <img id="btn_Rubrica_E" runat="server" alt="Seleziona una UO nella rubrica" height="19"
                                                                src="../../images/proto/rubrica.gif" style="cursor: hand" width="29">
                                                        </td>
                                                    </tr>
                                                </table>
                                                <input id="hd_systemIdUo" runat="server" class="titolo_scheda" name="hd_systemIdUO"
                                                    size="1" type="hidden">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" height="25">
                                                <img border="0" height="1" src="../../images/proto/spaziatore.gif" width="4">
                                                <asp:TextBox ID="txt_codUO" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                                    Width="80px"></asp:TextBox>&#160;
                                                <asp:TextBox ID="txt_descUO" runat="server" CssClass="testo_grigio" Width="243px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <br>
                                        <table class="info_grigio" cellspacing="0" cellpadding="0" width="95%" align="center"
                                            border="0">
                                            <tr>
                                                <td class="titolo_scheda" style="padding-bottom: 5px; padding-top: 5px" valign="middle"
                                                    width="35%" height="19">
                                                    <img border="0" height="1" src="../images/proto/spaziatore.gif" width="4">
                                                    Tipologia documento
                                                </td>
                                                <td style="padding-bottom: 5px; padding-top: 5px">
                                                    <asp:DropDownList ID="ddl_tipoAttoDR" runat="server" CssClass="testo_grigio" Width="210px"
                                                        AutoPostBack="True" Height="25px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="padding-top: 5px">
                                                    <asp:ImageButton ID="btn_CampiPersonalizzatiDR" runat="server" ImageUrl="../../images/proto/ico_oggettario.gif">
                                                    </asp:ImageButton>
                                                </td>
                                            </tr>
                                        </table>
                                        <br></br>
                                        <br></br>
                                    </br>
                                </br>
                            </asp:Panel>
                            <!--DOCUMENTI GRIGI-->
                            <asp:Panel ID="pnl_DocumentiGrigi" Visible="False" runat="server">
                                <table class="info_grigio" id="Table3" style="height: 70px" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Id. Documento
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td class="titolo_scheda">
                                            <p>
                                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:DropDownList ID="ddl_idDoc_E" runat="server" Width="110px" AutoPostBack="true"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp; &nbsp;
                                                <asp:Label ID="lbl_initIdDoc_E" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:Label>
                                                <asp:TextBox ID="txt_initIdDoc_E" runat="server" Width="80px" CssClass="testo_grigio"></asp:TextBox>&nbsp;
                                                <asp:Label ID="lbl_fineIdDoc_E" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                <asp:TextBox ID="txt_fineIdDoc_E" runat="server" Width="80px" CssClass="testo_grigio"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                                <br>
                                <table class="info_grigio" id="Table4" style="height: 70px" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Creazione
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td class="titolo_scheda">
                                            <p>
                                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:DropDownList ID="ddl_dataCreazioneG_E" runat="server" Width="110px" AutoPostBack="true"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp; &nbsp;
                                                <asp:Label ID="lbl_initDataCreazioneG_E" runat="server" Width="10px" CssClass="testo_grigio">Da</asp:Label>
                                                <uc4:Calendario ID="txt_initDataCreazioneG_E" runat="server" Visible="true" />
                                                &nbsp;
                                                <asp:Label ID="lbl_fineDataCreazioneG_E" runat="server" Width="10px" CssClass="testo_grigio">A</asp:Label>
                                                <uc4:Calendario ID="txt_fineDataCreazioneG_E" runat="server" Visible="true" />
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                                <br>
                                <table class="info_grigio" cellspacing="0" cellpadding="0" width="95%" align="center"
                                    border="0">
                                    <tr>
                                        <td class="titolo_scheda" style="padding-bottom: 5px; padding-top: 5px" valign="middle"
                                            width="35%" height="19">
                                            <img border="0" height="1" src="../images/proto/spaziatore.gif" width="4">Tipologia
                                            documento
                                        </td>
                                        <td style="padding-bottom: 5px; padding-top: 5px">
                                            <asp:DropDownList ID="ddl_tipoAttoDG" runat="server" CssClass="testo_grigio" Width="210px"
                                                AutoPostBack="True" Height="25px">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="padding-top: 5px">
                                            <asp:ImageButton ID="btn_CampiPersonalizzatiDG" runat="server" ImageUrl="../../images/proto/ico_oggettario.gif">
                                            </asp:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <!--STAMPA BUSTE-->
                            <asp:Panel ID="pnl_StampaBuste" Visible="False" runat="server">
                                <table class="info_grigio" style="height: 57px" cellspacing="0" cellpadding="0" width="95%"
                                    align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="15">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Anno
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td class="titolo_scheda">
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:TextBox ID="txt_Anno_B" Width="50px" CssClass="testo_grigio" runat="server"
                                                    MaxLength="4"></asp:TextBox></p>
                                        </td>
                                    </tr>
                                </table>
                                <br>
                                <table class="info_grigio" style="height: 57px" cellspacing="0" cellpadding="0" width="95%"
                                    align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="15">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Numero
                                            protocollo
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td class="titolo_scheda">
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:DropDownList ID="ddl_numProt_B" runat="server" Width="110px" AutoPostBack="true"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp; &nbsp;
                                                <asp:Label ID="lblDAnumprot_B" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:Label>
                                                <asp:TextBox ID="txt_initNumProt_B" runat="server" Width="80px" CssClass="testo_grigio"></asp:TextBox>&nbsp;
                                                <asp:Label ID="lblAnumprot_B" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                <asp:TextBox ID="txt_fineNumProt_B" runat="server" Width="80px" CssClass="testo_grigio"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                                <br>
                                <table class="info_grigio" style="height: 57px" cellspacing="0" cellpadding="0" width="95%"
                                    align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" style="height: 15px" valign="middle">
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data
                                            Protocollo
                                        </td>
                                    </tr>
                                    <tr height="30">
                                        <td>
                                            <p>
                                                <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                                <asp:DropDownList ID="ddl_dataProt_B" runat="server" Width="110px" AutoPostBack="true"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corr.</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="lbl_initdataProt_B" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:Label>
                                                <uc4:Calendario ID="txt_initDataProt_B" runat="server" Visible="true" />
                                                &nbsp;
                                                <asp:Label ID="lbl_finedataProt_B" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                <uc4:Calendario ID="txt_fineDataProt_B" runat="server" Visible="true" />
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <!-- REPORT AVANZATI -->
                            <asp:Panel ID="pnl_reportAvanzati" runat="server" Visible="false">
                                <table class="info_grigio" style="height: 57px" cellspacing="0" cellpadding="0" width="95%"
                                    align="center" border="0">
                                    <tr>
                                        <!-- DDL TIPO RUOLO -->
                                        <td class="titolo_scheda" valign="middle" height="15" id="td_rep_av_ruolo" runat="server">
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Tipo
                                            Ruolo<br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:DropDownList ID="ddl_rep_av_ruolo" runat="server" CssClass="testo_grigio">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <!-- DDL RAGIONI DI TRASMISSIONE -->
                                        <td class="titolo_scheda" valign="middle" height="15" id="td_rep_av_rag_trasm" runat="server">
                                            <br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Ragione
                                            di trasmissione<br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:DropDownList ID="ddl_rep_av_rag_trasm" runat="server" CssClass="testo_grigio"
                                                Width="200px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <!-- IACOZZILLI 14/03/2013 -->
                                    <tr>
                                        <!-- Numerico Up/down per il report PEC (giorni trascorsi i quali si è protocollata una PEC) -->
                                        <td class="titolo_scheda" valign="middle" height="15" width="300" id="tdtxt_giorni_trascorsi"
                                            runat="server">
                                            <br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Giorni
                                            trascorsi dalla ricezione *<br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:TextBox ID="txt_giorni_trascorsi" runat="server" Width="50px" Visible="true"
                                                TextMode="SingleLine">1</asp:TextBox>
                                            <br />
                                            <asp:RangeValidator ID="VRVtxt_giorni_trascorsi" ControlToValidate="txt_giorni_trascorsi"
                                                Type="Integer" MinimumValue="1" MaximumValue="7" ErrorMessage="Inserire un numero compreso tra 0 e 7.<br />"
                                                Display="Dynamic" runat="server" Width="150" />
                                            <br />
                                            <asp:RegularExpressionValidator ID="Regtxt_giorni_trascorsi" runat="server" ControlToValidate="txt_giorni_trascorsi"
                                                ErrorMessage="Inserire un numero valido" ValidationExpression="^\d*" Width="150"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <!-- FINE IACOZZILLI 14/03/2013 -->
                                    <tr>
                                        <!-- DDL DATA DI RIFERIMENTO -->
                                        <td class="titolo_scheda" style="height: 15px" valign="middle" id="td_rep_av_data"
                                            runat="server">
                                            <br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Data
                                            di riferimento&nbsp;*<br />
                                            <br />
                                            <img height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:DropDownList ID="ddl_rep_av_data" runat="server" Width="110px" AutoPostBack="true"
                                                CssClass="testo_grigio" OnSelectedIndexChanged="ddl_rep_av_data_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lbl_rep_av_da" runat="server" Width="18px" CssClass="testo_grigio">Da</asp:Label>
                                            <uc4:Calendario ID="txt_rep_av_initData" runat="server" Visible="true" />
                                            &nbsp;&nbsp;
                                            <asp:Label ID="lbl_rep_av_a" runat="server" Width="14px" CssClass="testo_grigio">A</asp:Label>
                                            <uc4:Calendario ID="txt_rep_av_fineData" runat="server" Visible="true" />
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top" height="10%">
                <!-- BOTTONIERA -->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" align="center" border="0">
                    <tr>
                        <td valign="top" height="5">
                            <img height="1" src="../../images/proto/spaziatore.gif" width="1" border="0">
                        </td>
                    </tr>
                    <tr align="center">
                        <td>
                            <cc1:ImageButton ID="btn_stampaRegistro" Thema="btn_" SkinID="stampa_Attivo" runat="server"
                                AlternateText="Stampa il report selezionato" Tipologia="GEST_REG_STAMPA" DisabledUrl="~/App_Themes/ImgComuni/btn_stampa_nonattivo.gif">
                            </cc1:ImageButton>
                            <cc1:ImageButton ID="btn_stampaRegistroDisabled" ImageUrl="~/App_Themes/ImgComuni/btn_stampa_nonattivo.gif"
                                runat="server" AlternateText="Stampa il report selezionato" DisabledUrl="~/App_Themes/ImgComuni/btn_stampa_nonattivo.gif"
                                EnableViewState="False"></cc1:ImageButton>
                        </td>
                    </tr>
                </table>
                <!--FINE BOTTONIERA -->
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
