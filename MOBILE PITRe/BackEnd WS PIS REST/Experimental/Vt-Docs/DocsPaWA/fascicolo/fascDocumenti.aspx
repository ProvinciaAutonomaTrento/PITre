<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<%@ Page Language="c#" CodeBehind="fascDocumenti.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.fascicolo.fascDocumenti" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="AclFascicolo" Src="AclFascicolo.ascx" %>
<%@ Register Src="../Note/DettaglioNota.ascx" TagName="DettaglioNota" TagPrefix="uc5" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc6" %>
<%@ Register Src="../UserControls/Corrispondente.ascx" TagName="Corrispondente" TagPrefix="uc7" %>
<%@ Import Namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc4" %>
<%@ Register TagPrefix="uc11" TagName="StampaEtichetta" Src="StampaEtichetta.ascx" %>
<%@ Register Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc8" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="True" name="vs_showGrid">
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>
    <link href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../CSS/ETcalendar.js"></script>
    <script language="javascript">

        var w = window.screen.width;
        var h = window.screen.height;
        var new_w = (w - 100) / 2;
        var new_h = (h - 400) / 2;

        function ApriFinestraRicercaDocPerClassifica(parameters) {
            var newLeft = (screen.availWidth - 602);
            var newTop = (screen.availHeight - 689);
            var myUrl = "../popup/RicercaDocumentiPerClassifica.aspx?" + parameters;
            rtnValue = window.showModalDialog(myUrl, '', 'dialogWidth:595px;dialogHeight:643px;status:no;dialogLeft:' + newLeft + '; dialogTop:' + newTop + ';center:no;resizable:yes;scroll:no;help:no;');
            window.document.fascDocumenti.submit();
        }

        function ApriDettagliProfilazione() {
            window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamicaFasc.aspx', '', 'dialogWidth:700px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');
        }

        function ApriPopUpImportDoc() {
            var newLeft = (screen.availWidth - 850);
            var newTop = (screen.availHeight - 600);
            var cod = document.getElementById("txt_fascdesc").value;
            var myUrl = "../ImportMassivoDoc/importaDoc.aspx?codFasc=" + cod;
            var retValue = window.showModalDialog(myUrl, '', 'dialogWidth:700px;dialogHeight:380px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no;top:' + newTop + ';left:' + newLeft);
            if (retValue = 'chiuso') window.document.fascDocumenti.submit();

        }

        // Funzione per l'apertura di un poput per l'esportazione in locale
        // di un fascicolo
        // param: idFasc - l'id del fascicolo da esportare
        function OpenPopUpExportFasc(idFasc) {
            var newLeft = (screen.availWidth - 850);
            var newTop = (screen.availHeight - 600);
            var myUrl = "../EsportaFascicolo/esportaFasc.aspx?idFasc=" + idFasc;
            var retValue = window.showModalDialog(myUrl, '', 'dialogWidth:700px;dialogHeight:380px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + newTop + ';left:' + newLeft);
            if (retValue = 'chiuso') window.document.fascDocumenti.submit();
        }


        function apriStoricoStati() {
            //window.open('../popup/StoricoStatiDocumento.aspx','','top = '+ new_h +' left = '+(new_w-25)+' width=550,height=320,scrollbars=YES');
            window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=../popup/StoricoStati.aspx?tipo=F', '', 'dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:yes;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function ApriRicercaSottoFascicoli(idfascicolo, desc) {
            var newLeft = (screen.availWidth - 615);
            var newTop = (screen.availHeight - 710);
            // apertura della ModalDialog
            rtnValue = window.showModalDialog('../popup/RicercaSottoFascicoli.aspx?idfascicolo=' + idfascicolo + '&desc=' + desc, "", "dialogWidth:615px;dialogHeight:440px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");

            if (rtnValue == "Y") {
                document.getElementById("hd_returnValueModal").value = rtnValue;
                window.document.fascDocumenti.submit()
            }

        }

        function ApriStoriaConservazione(idProject) {
            var newLeft = (screen.availWidth - 600);
            var newTop = (screen.availHeight - 622);
            var newUrl;

            newUrl = "../popup/storiaDocConservato.aspx?idProject=" + idProject;

            window.showModalDialog(newUrl, "", "dialogWidth:750px;dialogHeight:350px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
        }

        // Funzione per la creazione del file con le etichette da stampare
        function OpenFile() {
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
                filePath = path + "\\etichetteDocsPa.rtf";
                applName = "Microsoft Word";

                exportUrl = "exportEtichetteFasc.aspx";
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


        function _ApriRubrica(target) {
            var r = new Rubrica();

            switch (target) {
                // Gestione fascicoli - Locazione fisica         
                case "ef_locfisica":
                    r.CallType = r.CALLTYPE_EDITFASC_LOCFISICA;

                    break;

                // Gestione fascicoli - Ufficio referente         
                case "ef_uffref":
                    r.CallType = r.CALLTYPE_EDITFASC_UFFREF;
                    break;

            }
            var res = r.Apri();
        }

        function UpdateCampiProfilati() {
            parent.parent.document.iFrame_dx.document.iFrameDoc.document.forms[0].elements('hd_Update').value = 'Y';
            parent.parent.document.iFrame_dx.document.iFrameDoc.document.forms[0].submit();
        }
        function AggiornaLocazione() {
            __doPostBack('updPanelCorrispondente', '');
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 42px;
        }
        .img_fasc img
        {
            float: left;
            width: 30px;
        }
    </style>
</head>
<body>
    <form id="fascDocumenti" name="fascDocumenti" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Documenti Fascicolo" />
    <uc1:AclFascicolo ID="aclFascicolo" runat="server"></uc1:AclFascicolo>
    <asp:ScriptManager ID="ScriptManager" AsyncPostBackTimeout="360000" runat="server">
    </asp:ScriptManager>
    <div id="divScroll">
        <table class="contenitore" id="Table8" height="100%" cellspacing="0" cellpadding="0"
            width="100%" border="0">
            <tr>
                <td align="center">
                    <div style="height: 15px;">
                        <table cellspacing="0" cellpadding="0" width="98%" border="0">
                            <tr>
                                <td>
                                    <asp:Label ID="lblTitolario" runat="server" CssClass="titolo_scheda"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblClassifica" runat="server" CssClass="titolo_scheda"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <!--dati fascicolo-->
                    <table class="info_grigio" id="tbl_oggetto1" cellspacing="0" cellpadding="0" width="98%"
                        border="0">
                        <%-- bottoni--%>
                        <tr>
                            <td class="titolo_scheda" align="right" valign="middle" style="padding-top: 2px;
                                padding-right: 2px; padding-right: 4px;">
                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Dati fascicolo
                                <cc1:ImageButton ID="btn_storiaFasc" Height="17px" ImageUrl="../images/proto/conservazione_d.gif"
                                    runat="server" AlternateText="Visualizza storia conservazione fascicolo" Width="19px"
                                    Visible="false"></cc1:ImageButton>
                                <cc1:ImageButton ID="btn_log" Height="17px" ImageUrl="../images/proto/storia.gif"
                                    runat="server" AlternateText="Mostra Storia Fascicolo" Width="19px" DisabledUrl="../images/proto/storia.gif">
                                </cc1:ImageButton>
                                <cc1:ImageButton ID="btn_visibilita" Height="17px" ImageUrl="../images/proto/ico_visibilita2.gif"
                                    runat="server" AlternateText="Mostra visibilità" Width="19px" DisabledUrl="../images/proto/ico_visibilita2.gif">
                                </cc1:ImageButton>
                                <cc1:ImageButton ID="btn_chiudiRiapri" Height="17" ImageUrl="../images/fasc/ico_apri-chiudi.gif"
                                    runat="server" AlternateText="chiudi/riapri fascicolo" Width="18" DisabledUrl="../images/fasc/ico_apri-chiudi.gif"
                                    Tipologia="FASC_APRICHIUDI"></cc1:ImageButton>
                                <cc1:ImageButton ID="btn_modifica" ImageUrl="../images/proto/matita.gif" runat="server"
                                    AlternateText="modifica fascicolo" Width="18" DisabledUrl="../images/proto/matita.gif"
                                    Tipologia="FASC_MOD_FASCICOLO" OnClick="btn_modifica_Click"></cc1:ImageButton>
                                <cc1:ImageButton ID="btn_addToAreaLavoro" Height="16" ImageUrl="../images/proto/ins_area.gif"
                                    runat="server" AlternateText="Inserisci fascicolo in area di lavoro" Width="18"
                                    DisabledUrl="../images/fasc/ico_fascicolo_area.gif" Tipologia="FASC_ADD_ADL">
                                </cc1:ImageButton>
                                <cc1:ImageButton ID="btn_stampaFascette" Height="16" DisabledUrl="../images/proto/stampa.gif"
                                    Width="18" AlternateText="Stampa fascette del fascicolo" runat="server" ImageUrl="../images/proto/stampa.gif"
                                    Tipologia=""></cc1:ImageButton>
                                <cc1:ImageButton ID="btnPrintSignature" Height="16" DisabledUrl="../images/fasc/stampa_etichetta.gif"
                                    Tipologia="" Width="18" AlternateText="Stampa etichetta" runat="server" ImageUrl="../images/fasc/stampa_etichetta.gif">
                                </cc1:ImageButton>
                            </td>
                        </tr>
                        <%-- fine bottoni--%>
                    </table>
                </td>
            </tr>
            <tr>
                <td height="2" class="testo_grigio_scuro">
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table class="info_grigio" id="tbl_oggetto" cellspacing="0" cellpadding="0" width="98%">
                        <%-- campi fascicolo--%>
                        <tr>
                            <td class="titolo_scheda" valign="middle" height="8" align="left">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" CssClass="testo_grigio">&nbsp;Classifica</asp:Label>
                                        </td>
                                        <td>
                                        <!--Mev Ospedale Maggiore Policlinico-->
                                        <asp:TextBox ID="txt_ClassFasc" runat="server" CssClass="testo_grigio" ReadOnly="True" AutoPostBack="true" 
                                                Width="95"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" CssClass="testo_grigio">Codice</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_fascdesc" runat="server" CssClass="testo_grigio" ReadOnly="True"
                                                Width="95"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbl_dataAp" runat="server" CssClass="testo_grigio" Width="85px">&nbsp;Data apertura</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_fascApertura" runat="server" CssClass="testo_grigio" ReadOnly="True"
                                                Width="95"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lbl_dataC" runat="server" CssClass="testo_grigio" Width="85px">Data chiusura</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_FascChiusura" runat="server" CssClass="testo_grigio" ReadOnly="True"
                                                Width="95"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label5" runat="server" CssClass="testo_grigio">&nbsp;Tipo</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_Fasctipo" runat="server" CssClass="testo_grigio" ReadOnly="True"
                                                Width="95"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" CssClass="testo_grigio">Stato</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_fascStato" runat="server" CssClass="testo_grigio" ReadOnly="True"
                                                Width="95"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Label ID="Label3" runat="server" CssClass="testo_grigio">&nbsp;Descrizione</asp:Label>
                                        </td>
                                        <td colspan="3">
                                            <asp:TextBox ID="txt_descrizione" runat="server" CssClass="testo_grigio" ReadOnly="True"
                                                Width="292px" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right" class="testo_grigio">
                                            caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"
                                                size="4" class="testo_grigio" readonly="readonly" />&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <%--fine campi fascicolo--%>
            </tr>
            <%-- checkboxes tipi fascicolo--%>
            <tr>
                <td height="2" class="testo_grigio_scuro">
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table class="info_grigio" id="Table1" cellspacing="0" cellpadding="0" width="98%"
                        align="center" border="0">
                        <tr>
                            <td class="titolo_scheda" valign="middle" colspan="2" height="8">
                                <asp:Label ID="lblFascicoloCartaceo" runat="server" CssClass="testo_grigio">&nbsp;Cartaceo&nbsp;</asp:Label>
                                <asp:CheckBox ID="chkFascicoloCartaceo" runat="server" CssClass="testo_grigio" Enabled="False">
                                </asp:CheckBox>&nbsp;&nbsp;
                                <asp:Label ID="lblFascicoloPrivato" runat="server" CssClass="testo_grigio">&nbsp;Privato&nbsp;</asp:Label>
                                <asp:CheckBox ID="chkFascicoloPrivato" runat="server" CssClass="testo_grigio" Enabled="False">
                                </asp:CheckBox>&nbsp;&nbsp;
                                <asp:Label ID="lblFascicoloControllato" runat="server" CssClass="testo_grigio">&nbsp;Impone visibilità&nbsp;</asp:Label>
                                <asp:CheckBox ID="chkFascicoloControllato" runat="server" CssClass="testo_grigio"
                                    Enabled="False"></asp:CheckBox>&nbsp;&nbsp;
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--fine checkboxes tipi fascicolo--%>
            <tr>
                <td height="2" class="testo_grigio_scuro">
                </td>
            </tr>
            <asp:Panel ID="pnlscheda" runat="server" Visible="True" BorderColor="#000000">
                <tr>
                    <%--locazione fisica--%>
                    <td align="left">
                        <table class="info_grigio" id="Table2" cellspacing="0" cellpadding="0" width="98%"
                            align="center" border="0">
                            <tr>
                                <td class="titolo_scheda" align="left" colspan="1" height="20px;">
                                    <asp:Panel ID="pnl_locFis" runat="server" Visible="True" BorderColor="#000000">
                                        <table>
                                            <tr>
                                                <td align="left" style="width: 30%; height: 20px;">
                                                    <asp:Label ID="ldb_locFis" runat="server" CssClass="testo_grigio">&nbsp;Collocaz. fisica</asp:Label>
                                                </td>
                                                <td align="left" style="width: 60%; height: 20px;">
                                                    <asp:TextBox ID="txt_LFCod" runat="server" CssClass="testo_grigio" Width="53px" AutoPostBack="true"
                                                        OnTextChanged="txt_LFCod_TextChanged" ReadOnly="True"></asp:TextBox>
                                                    <asp:TextBox ID="txt_LFDesc" runat="server" CssClass="testo_grigio" Width="160px"
                                                        ReadOnly="True" AutoPostBack="false"></asp:TextBox>
                                                </td>
                                                <td align="left">
                                                    <asp:ImageButton ID="btn_rubrica" runat="server" Height="20px" ImageUrl="../images/proto/rubrica.gif"
                                                        Visible="false" CausesValidation="false" CssClass="img_fasc" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="width: 30%">
                                                    <asp:Label ID="Label12" runat="server" CssClass="testo_grigio">&nbsp;Data collocaz.</asp:Label>
                                                </td>
                                                <td align="left" style="width: 60%">
                                                    <uc6:Calendario ID="txt_LFDTA" runat="server" Visible="true" EnableBtnCal="false" />
                                                </td>
                                                <td width="40px;" align="left">
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <tr>
                            <td height="2" class="testo_grigio_scuro">
                            </td>
                        </tr>
                    </td>
                </tr>
            </asp:Panel>
            <%--fine locazione fisica--%>
             <asp:Panel ID="pnl_uff_ref" runat="server" Visible="False" BorderColor="#000000">
                <tr>
                    <%--ufficio referente--%>
                    <td align="left">
                        <table class="info_grigio" id="Table7" cellspacing="0" cellpadding="0" width="98%"
                            align="center" border="0">
                            <tr>
                                <td class="titolo_scheda" align="left" colspan="1" height="20px;">
                                    <table>
                                        <tr>
                                            <td align="left" style="width: 30%; height: 20px;">
                                                <asp:Label ID="lbl_uffRef" runat="server" CssClass="testo_grigio">&nbsp;Ufficio Referente*</asp:Label>
                                            </td>
                                            <td align="left" style="width: 60%; height: 20px;">
                                                <asp:TextBox ID="txt_cod_uff_ref" runat="server" CssClass="testo_grigio" Width="53px"
                                                    AutoPostBack="true" OnTextChanged="txt_cod_uff_ref_TextChanged" ReadOnly="True"></asp:TextBox>
                                                <asp:TextBox ID="txt_desc_uff_ref" runat="server" CssClass="testo_grigio" Width="160px"
                                                    ReadOnly="True" AutoPostBack="false"></asp:TextBox>
                                            </td>
                                            <td align="left" style="width:40px;">
                                                <asp:ImageButton ID="btn_rubrica_ref" runat="server" Height="20px" ImageUrl="../images/proto/rubrica.gif"
                                                    Visible="false" CausesValidation="false" CssClass="img_fasc"  />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td height="2" class="testo_grigio_scuro">
                    </td>
                </tr>
            </asp:Panel>
            <%--ufficio referente--%>
            <%-- profilazione--%>
            <asp:Panel ID="pnl_profilazione" runat="server" Visible="false">
                <tr>
                    <td>
                        <table class="info_grigio" id="Table3" cellspacing="0" cellpadding="0" width="98%"
                            align="center" border="0">
                            <tr>
                                <td class="titolo_scheda" valign="middle" height="8">
                                    <asp:Label ID="lbl_tipoFasc" runat="server" CssClass="testo_grigio" Width="30%">&nbsp;Tipologia fascicolo</asp:Label>
                                    <asp:DropDownList ID="ddl_tipologiaFasc" runat="server" CssClass="testo_grigio" AutoPostBack="True"
                                        Width="240px">
                                    </asp:DropDownList>
                                    &nbsp;
                                    <asp:ImageButton ID="img_btnDettagliProf" AlternateText="Visualizza dettagli" runat="server"
                                        ImageUrl="../images/proto/ico_oggettario.gif" Width="18px" Height="17px"></asp:ImageButton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td height="2" class="testo_grigio_scuro">
                    </td>
                </tr>
            </asp:Panel>
            <%--fine profilazione--%>
            <%--diagrammi stato--%>
            <asp:Panel ID="Panel_DiagrammiStato" runat="server" Visible="false">
                <tr>
                    <td>
                        <table class="info_grigio" id="Table4" cellspacing="0" cellpadding="0" width="98%"
                            align="center" border="0">
                            <tr>
                                <td height="25" width="10%">
                                    <asp:Label ID="lbl_statoFasc" runat="server" CssClass="titolo_scheda">&nbsp;Stato :&nbsp;</asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lbl_statoAttuale" runat="server" CssClass="titolo_scheda" Text="&nbsp;"></asp:Label>
                                </td>
                                <td align="right" style="padding-right: 10px;">
                                    <cc1:ImageButton ID="img_btnStoriaDiagrammi" DisabledUrl="../images/proto/storia.gif"
                                        ImageUrl="../images/proto/storia.gif" Height="17" AlternateText="Storia" Width="18"
                                        runat="server"></cc1:ImageButton>
                                </td>
                            </tr>
                            <tr>
                                <td class="testo_grigio">
                                    &nbsp;Stati :&nbsp;
                                </td>
                                <td colspan="2">
                                    <asp:DropDownList ID="ddl_statiSuccessivi" runat="server" Width="90%" CssClass="testo_grigio"
                                        Enabled="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td height="2" class="testo_grigio_scuro">
                    </td>
                </tr>
            </asp:Panel>
            <%--fine diagrammi stato--%>
            <%--data scadenza--%>
            <asp:Panel ID="Panel_DataScadenza" runat="server" Visible="false">
                <tr>
                    <td>
                        <table class="info_grigio" id="Table5" cellspacing="0" cellpadding="0" width="98%"
                            align="center" border="0">
                            <tr>
                                <td class="titolo_scheda" valign="middle" height="8">
                                    <asp:Label ID="lbl_dataScadenza" runat="server" CssClass="testo_grigio" Width="27%">&nbsp;Data Scadenza &nbsp;</asp:Label>
                                    <cc1:DateMask ID="txt_dataScadenza" runat="server" CssClass="testo_grigio" Width="21%"></cc1:DateMask>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td height="2" class="testo_grigio_scuro">
                    </td>
                </tr>
            </asp:Panel>
            <%--fine data scadenza--%>
            <%-- dettaglio note  --%>
            <tr>
                <td>
                    <table class="info_grigio" id="Table6" cellspacing="0" cellpadding="0" width="98%"
                        align="center" border="0">
                        <tr>
                            <td class="titolo_scheda" valign="middle" colspan="1" align="center">
                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                <uc5:DettaglioNota ID="dettaglioNota" runat="server" TipoOggetto="Fascicolo" Rows="3"
                                    Width="95%" Enabled="False" TextMode="MultiLine" PAGINA_CHIAMANTE="fascDocumenti" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td height="2" class="testo_grigio_scuro">
                </td>
            </tr>
            <%--fine dettaglio note  --%>
            <tr>
                <td>
                    <table width="98%" cellspacing="0" cellpadding="0" border="0" class="info_grigio"
                        align="center">
                        <tr>
                            <td align="center">
                                <asp:Label ID="Label7" CssClass="testo_grigio_scuro_grande" runat="server">Sotto fascicoli</asp:Label>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="img_cercaSottoFasc" AlternateText="Ricerca sottofascicoli" runat="server"
                                    ImageUrl="../images/proto/ico_fascicolo_noattivo.gif" Width="18px" OnClick="img_cercaSottoFasc_Click">
                                </asp:ImageButton>&nbsp;
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!--fine dati fascicolo-->
            <tr>
                <td height="2" class="testo_grigio_scuro">
                </td>
            </tr>
            <tr>
                <td>
                    <table width="98%" cellspacing="0" cellpadding="0" border="0" class="info_grigio"
                        align="center">
                        <tr>
                            <td class="testo_grigio_scuro" style="padding-left: 2px; padding-top: 8px;">
                                <mytree:TreeView ID="Folders" runat="server" CssClass="testo_grigio" Width="380px"
                                    SystemImagesPath="../images/alberi/left/" NAME="Treeview1" BorderWidth="0px"
                                    DefaultStyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color: #d9d9d9;"
                                    Height="80px" BorderStyle="Solid" ImageUrl="../images/alberi/folders/folder_chiusa.gif"
                                    SelectedImageUrl="../images/alberi/folders/folder_piena.gif" ExpandedImageUrl="../images/alberi/folders/folder_aperta.gif"
                                    HoverStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
                                    SelectedStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
                                    OnSelectedIndexChange="Folders_SelectedIndexChange" AutoPostBack="True"></mytree:TreeView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td align="center">
                                            <cc1:ImageButton ID="btn_modFolder" Thema="btn_" SkinID="modifica_attivo" runat="server"
                                                AlternateText="Modifica sotto fascicolo" DisabledUrl="../images/bottoniera/btn_modifica_nonattivo.gif"
                                                Tipologia="FASC_MOD_FOLDER"></cc1:ImageButton>
                                        </td>
                                        <td align="center">
                                            <cc1:ImageButton ID="btn_aggiungi" Thema="btn_" SkinID="aggiungi_attivo" runat="server"
                                                AlternateText="Aggiungi sotto fascicolo" DisabledUrl="../images/bottoniera/btn_aggiungi_nonattivo.gif"
                                                Tipologia="FASC_NEW_FOLDER" autopostback="false"></cc1:ImageButton>
                                        </td>
                                        <td align="center">
                                            <cc1:ImageButton ID="btn_rimuovi" Thema="btn_" SkinID="rimuovi_attivo" runat="server"
                                                AlternateText="Rimuovi sotto fascicolo" DisabledUrl="../images/bottoniera/btn_rimuovi_nonattivo.gif"
                                                Tipologia="FASC_DEL_FOLDER"></cc1:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <!-- BOTTONIERA -->
                    <br />
                    <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" align="center" border="0">
                        <tr>
                            <td>
                                <!-- MEV Ospedale Maggiore Policlinico-->
                                <cc1:ImageButton ID="btn_Riclassifica" Thema="btn_" SkinID= "riclassifica_attivo" runat="server" 
                                    DisabledUrl="../images/bottoniera/btn_riclassifica_nonAttivo.gif"
                                    AlternateText="Riclassifica" ImageUrl= "../images/bottoniera/btn_riclassifica_attivo.gif"
                                    Tipologia="FASC_RICLASS" Enabled="false" Visible="false" >
                                </cc1:ImageButton>
                                <!-- END MEV Ospedale Maggiore Policlinico-->
                                <cc1:ImageButton ID="btn_Salva" Thema="btn_" SkinID="salva_Attivo" runat="server"
                                    AlternateText="Salva modifiche" DisabledUrl="../images/bottoniera/btn_salva_NoAttivo.gif"
                                    Tipologia="FASC_INS_DOC" Enabled="False" ImageUrl="../images/bottoniera/btn_salva_Attivo.gif">
                                </cc1:ImageButton>
                                <cc1:ImageButton ID="btn_AnnullaModifiche" Thema="btn_" SkinID="annulla_attivo" runat="server"
                                    AlternateText="Annulla modifiche" DisabledUrl="../images/bottoniera/btn_annulla_nonattivo.gif"
                                    Tipologia="FASC_INS_DOC" Enabled="False" OnClick="btn_AnnullaModifiche_Click"
                                    ImageUrl="../images/bottoniera/btn_annulla_attivo.gif"></cc1:ImageButton>
                                <cc2:MessageBox ID="msg_StatoAutomatico" Height="0" runat="server" OnGetMessageBoxResponse="msg_StatoAutomatico_GetMessageBoxResponse">
                                </cc2:MessageBox>
                                <cc2:MessageBox ID="msg_StatoFinale" Height="0" runat="server" OnGetMessageBoxResponse="msg_StatoFinale_GetMessageBoxResponse">
                                </cc2:MessageBox>
                            </td>
                        </tr>
                    </table>
                    <!--FINE	BOTTONIERA -->
                </td>
            </tr>
        </table>
        <input type="hidden" id="hd_returnValueModal" runat="server" />
        <uc2:AdoStreamWrapper ID="AdoStreamWrapper1" runat="server" />
        <uc3:FsoWrapper ID="FsoWrapper1" runat="server" />
        <asp:PlaceHolder runat="server" ID="ph_stampa"></asp:PlaceHolder>
        <%--<uc11:StampaEtichetta id="StampaEtichetta" runat="server"></uc11:StampaEtichetta>--%>
        <uc4:ShellWrapper ID="ShellWrapper1" runat="server" />
    </div>
    </form>
</body>
</html>
