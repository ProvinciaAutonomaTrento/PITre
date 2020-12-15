<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc1" TagName="AclDocumento" Src="AclDocumento.ascx" %>

<%@ Page Language="c#" CodeBehind="docProfilo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.docProfilo" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Src="../Note/DettaglioNota.ascx" TagName="DettaglioNota" TagPrefix="uc2" %>
<%@ Register Src="Oggetto.ascx" TagName="Oggetto" TagPrefix="uc4" %>
<%@ Register Src="../UserControls/DocumentConsolidation.ascx" TagName="DocumentConsolidation"
    TagPrefix="uc6" %>
<!DOCTYPE HTML	PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" id="btn_salva_click" event="onclick()" for="btn_salva">
			if(document.getElementById("abilitaModaleVis").value == "true")
			{
			    AvvisoVisibilita();
			}
			document.getElementById('btn_salva').style.display='none';
			document.getElementById('btn_salva_disabled').style.display='';
			window.document.body.style.cursor='wait';
			if(top.principale.iFrame_dx.document.iFrameDoc.document.frames[0] != null && top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms.length != 0)
			{
			    top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms[0].submit();
			}			
    </script>
    <script language="javascript" id="wait_BackToFolder" event="onclick()" for="btn_BackToFolder">
			window.document.body.style.cursor='wait';			
			WndWait();
    </script>
    <script language="javascript">
        var w = window.screen.width;
        var h = window.screen.height;
        var new_w = (w - 100) / 2;
        var new_h = (h - 400) / 2;

        function apriPopupAnteprima() {
            //window.open('AnteprimaProfDinamica.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
            window.showModalDialog('AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx', '', 'dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function apriStoricoStati() {
            //window.open('../popup/StoricoStatiDocumento.aspx','','top = '+ new_h +' left = '+(new_w-25)+' width=550,height=320,scrollbars=YES');
            window.showModalDialog('AnteprimaProfDinModal.aspx?Chiamante=../popup/StoricoStati.aspx?tipo=D', '', 'dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:yes;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function nascondi() {
            document.getElementById('btn_salva_disabled').style.display = 'none';
        }

        //Inserimento nuova tipologia Atto
        function ApriFinestraTipologiaAtto() {

            var left = (screen.availWidth - 594);
            var top = (screen.availHeight - 545);

            //window.open(urlPar,"StoriaModifiche","toolbar=no,location=no,directories=no, status=no,scrollbars=no,resizable=no,copyhistory=no,top="+top+",left="+left+", width=580, height=350");		
            url = "../popup/insTipoAtto.aspx?wnd=docProfilo";
            win = window.open(url, "TipoAtto", "toolbar=no,location=no,directories=no, status=no,scrollbars=no,resizable=no,copyhistory=no,top=" + top + ",left=" + left + ",width=404,height=108");
            win.focus();

        }


        //Apre la PopUp Modale per la ricerca dei fascicoli
        //utile per la fascicolazione rapida
        function ApriRicercaFascicoli2(codiceClassifica, NodoMultiReg) {
            var newUrl;

            newUrl = "../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica + "&caller=profilo&NodoMultiReg=" + NodoMultiReg;

            var newLeft = (screen.availWidth - 615);
            var newTop = (screen.availHeight - 704);

            // apertura della ModalDialog
            rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:auto;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
            if (rtnValue == "N") {
                window.document.docProfilo.txt_CodFascicolo.value = "";
                window.document.docProfilo.txt_DescFascicolo.value = "";
            }
            if (rtnValue == "Y") {
                window.document.docProfilo.submit();
            }
        }

        function ApriRicercaFascicoli(codiceClassifica) {
            var newUrl;

            newUrl = "../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica + "&caller=profilo";

            var newLeft = (screen.availWidth - 615);
            var newTop = (screen.availHeight - 704);

            // apertura della ModalDialog
            rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:auto;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
            if (rtnValue == "Y") {
                window.document.docProfilo.submit();
            }
        }

        function ApriSceltaFascicolo() {
            var newUrl;

            newUrl = "../popup/scegliFascicoloFascRapida.aspx";

            var newLeft = (screen.availWidth - 602);
            var newTop = (screen.availHeight - 628);

            // apertura della ModalDialog
            rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:590px;dialogHeight:440px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");

            window.document.docProfilo.submit();

        }

        function ApriDescrizioneCampo(tipoCampo) {
            var newLeft = (screen.availWidth - 600);
            var newTop = (screen.availHeight - 622);
            var newUrl;

            newUrl = "../popup/dettaglioCampo.aspx?tipoCampo=" + tipoCampo;

            window.showModalDialog(newUrl, "", "dialogWidth:440px;dialogHeight:400px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");

        }

        function ApriFinestraDocumentiNonProtocollati() {
            var newLeft = (screen.availWidth - 602);
            var newTop = (screen.availHeight - 649);

            rtnValue = window.showModalDialog('../popup/RicercaDocNonProtocollati.aspx', '', 'dialogWidth:596px;dialogHeight:641px;status:no;dialogLeft:' + newLeft + '; dialogTop:' + newTop + ';center:no;resizable:no;scroll:yes;help:no;');
            window.document.docProfilo.submit();
        }

        function ShowDialogRispostaDocGrigio(sys, tipoProto) {
            var newLeft = (screen.availWidth - 602);
            var newTop = (screen.availHeight - 625);

            var retValue =
					window.showModalDialog('../popup/listaDocInRisposta.aspx?sys=' + sys + '&tipo=' + tipoProto, '',
											'dialogHeight:620px; dialogWidth:595px;status:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;');

            if (retValue == 'true') {
                window.open('../documento/gestionedoc.aspx?tab=profilo', 'principale');
            }
        }

        function VaiRispostaDocGrigio() {
            top.principale.document.location = "../documento/gestionedoc.aspx?tab=profilo";
        }

        /*Attenzione affinchè funzioni la mutua esclusione dei chek privato ed utente
        nel caso qualcuno voglia aggiungere un nuovo check non gli dia un nome che inizi 
        con chk*/
        function SingleSelect(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {

                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name)) {
                    elm.checked = false;
                }
            }
        }

        function ApriStoriaConservazione(idProfile) {
            var newLeft = (screen.availWidth - 600);
            var newTop = (screen.availHeight - 622);
            var newUrl;

            newUrl = "../popup/storiaDocConservato.aspx?idProfile=" + idProfile;

            window.showModalDialog(newUrl, "", "dialogWidth:650px;dialogHeight:350px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
        }

        function AvvisoVisibilita() {
            var newLeft = (screen.availWidth - 500);
            var newTop = (screen.availHeight - 500);
            var newUrl;

            newUrl = "../popup/estensioneVisibilita.aspx";

            if (IsCheckBoxRequired()) {
                retValue = window.showModalDialog(newUrl, "", "dialogWidth:306px;dialogHeight:188px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:yes;help:no;");

                if (retValue == 'NO') {
                    document.getElementById("estendiVisibilita").value = "true";
                }
                else {
                    document.getElementById("estendiVisibilita").value = "false";
                }
            }
        }

        function IsCheckBoxRequired() {
            if (document.getElementById("chkPrivato") != null &&
              document.getElementById("chkPrivato").checked == true) {
                return true;
            }
            else {
                return false;
            }
        }


        // Script per la gestione dialog RicercaProtocolli
        function ApriFinestraDocumenti() {
            var newLeft = (screen.availWidth - 602);
            var newTop = (screen.availHeight - 649);

            rtnValue = window.showModalDialog('../popup/RicercaProtocolli.aspx?tipo=NP&page=profilo', '', 'dialogWidth:596px;dialogHeight:641px;status:no;dialogLeft:' + newLeft + '; dialogTop:' + newTop + ';center:no;resizable:no;scroll:yes;help:no;');
            window.document.docProfilo.submit();
        }



    </script>
</head>
<body>
    <form id="docProfilo" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManager" AsyncPostBackTimeout="360000" runat="server">
    </asp:ScriptManager>
    <asp:HiddenField ID="abilitaModaleVis" runat="server" />
    <asp:HiddenField ID="estendiVisibilita" runat="server" />
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Profilo" />
    <input id="h_tipoAtto" style="z-index: 101; left: 32px; width: 24px; position: absolute;
        top: 976px; height: 18px" type="hidden" size="1" name="h_tipoAtto" runat="server">
    <!-- campi stampa etichetta -->
    <input id="hd_UrlIniFileDispositivo" type="hidden" name="hd_UrlIniFileDispositivo"
        runat="server" />
    <input id="hd_dispositivo" type="hidden" name="hd_dispositivo" runat="server" />
    <input id="hd_amministrazioneEtichetta" type="hidden" name="hd_amministrazioneEtichetta"
        runat="server" />
    <input id="hd_descrizioneAmministrazione" type="hidden" name="hd_descrizioneAmministrazione"
        runat="server" />
    <input id="hd_classifica" type="hidden" name="hd_classifica" runat="server" />
    <input id="hd_fascicolo" type="hidden" name="hd_fascicolo" runat="server" />
    <input id="hd_num_doc" type="hidden" name="hd_num_doc" runat="server" />
    <input id="hd_numero_allegati" type="hidden" name="hd_numero_allegati" runat="server" />
    <input id="hd_codiceUoCreatore" type="hidden" name="hd_codiceUoCreatore" runat="server" />
    <input id="hd_dataCreazione" type="hidden" name="hd_dataCreazione" runat="server" />
    <input id="isFascRequired" type="hidden" name="isFascRequired" runat="server" />
    <input id="hd_modello_dispositivo" type="hidden" name="hd_modello_dispositivo" runat="server" />
    <!-- fine campi stampa etichetta -->
    <uc1:AclDocumento ID="aclDocumento" runat="server"></uc1:AclDocumento>
    <div id="divScroll">
        <table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="99%"
            align="center" border="0">
            <tr valign="top">
                <td align="left">
                    <table class="contenitore" id="Table1" height="100%" cellspacing="0" cellpadding="0"
                        width="100%" border="0">
                        <tr height="2" align="center">
                            <td class="titolo_scheda" height="5" colspan="3">
                                <asp:Label ID="lblStatoConsolidamento" CssClass="testo_red" runat="server" Width="100%"
                                    Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <!-- INIZIO TABELLA DATA_CREAZIONE -->
                                <table class="info_grigio" id="tbl_dataCreazione" cellspacing="0" cellpadding="0"
                                    width="96%" align="center" border="0">
                                    <tr height="5" valign="bottom">
                                        <td class="titolo_scheda" height="5" width="50%" style="padding-left:10px;">
                                            &nbsp;
                                        </td>
                                        <td align="right" height="5"  width="50%" style="padding-left:10px;">
                                            <uc6:DocumentConsolidation ID="documentConsolidationCtrl" runat="server"></uc6:DocumentConsolidation>
                                            <cc1:ImageButton ImageAlign="Middle" ID="btn_storiaCons" Height="16" ImageUrl="../images/proto/conservazione_d.gif"
                                                runat="server" AlternateText="Visualizza storia conservazione documento" Width="18"
                                                Visible="false" ToolTip="Visualizza storia conservazione documento" />
                                            <cc1:ImageButton ImageAlign="Middle" ID="btn_inoltra" Height="16" ImageUrl="../images/proto/inoltra.gif"
                                                runat="server" AlternateText="Inoltra il documento" ToolTip="Inoltra il documento">
                                            </cc1:ImageButton>
                                            <cc1:ImageButton ImageAlign="Middle" ID="btn_aggiungi" Height="16" ImageUrl="../images/proto/ins_area.gif"
                                                runat="server" AlternateText="Inserisci documento in area di lavoro" DisabledUrl="../images/proto/ins_area.gif"
                                                Tipologia="DO_ADD_ADL" ToolTip="Inserisci documento in area di lavoro"></cc1:ImageButton>
                                            <cc1:ImageButton ImageAlign="Middle" ID="btn_log" runat="server" Width="19px" AlternateText="Mostra Storia Documento"
                                                DisabledUrl="../images/proto/storia.gif" Height="17px" ImageUrl="../images/proto/storia.gif"
                                                Visible="true" ToolTip="Mostra Storia Documento"></cc1:ImageButton>
                                            <cc1:ImageButton ImageAlign="Middle" ID="btn_visibilita" runat="server" Width="19px"
                                                AlternateText="Mostra visibilità" Tipologia="DO_PRO_VISIBILITA" ImageUrl="../images/proto/ico_visibilita2.gif"
                                                DisabledUrl="../images/proto/ico_visibilita2.gif" ToolTip="Mostra visibilità">
                                            </cc1:ImageButton>
                                            <cc1:ImageButton ImageAlign="Middle" ID="btn_stampaSegn" runat="server" Width="18"
                                                AlternateText="Stampa etichetta" DisabledUrl="../images/proto/stampa.gif" Tipologia="DO_DOC_SE_GRIGIO"
                                                ImageUrl="../images/proto/stampa.gif" OnClick="btn_stampaSegn_Click" ToolTip="Stampa etichetta">
                                            </cc1:ImageButton>
                                            &nbsp;
                                            <asp:Image ID="imgTipoAlleg" runat="server" Visible="false" Height="22" Width="22" ImageUrl="~/images/allegati_attivo.gif" />
                                        </td>
                                    </tr>
                                    <tr height="5" valign="bottom" style="padding-left:10px;">
                                        <td class="titolo_scheda" height="5" width="50%">
                                            Data creazione
                                        </td>
                                        <td class="titolo_scheda" height="5"  width="50%" style="padding-left:10px;">
                                            Id documento
                                        </td>
                                    </tr>
                                    <tr>
                                        <td  width="50%" style="padding-left:10px;">
                                            <asp:TextBox ID="lbl_dataCreazione" runat="server" MaxLength="16" CssClass="testo_grigio14_bold"
                                                ReadOnly="True" Width="170px" BackColor="#D9D9D9"></asp:TextBox>
                                        </td>
                                        <td width="50%" style="padding-left:10px;">
                                            <asp:TextBox ID="txt_docNumber" runat="server" CssClass="testo_grigio14_bold" ReadOnly="True"
                                                BackColor="#D9D9D9" Width="170px" BorderStyle="Inset"></asp:TextBox>
                                            <asp:Label ID="ldl_docNumber" runat="server" CssClass="testo_grigio" Visible="False"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <!--td><IMG height="1" src="../images/tratteggio_gri_o.gif" width="8" border="0"></td-->
                                        <td align="left" width="50%" style="padding-left:10px;">
                                            <asp:CheckBox ID="chkPrivato" name="chkPrivato" runat="server" CssClass="testo_grigio_scuro"
                                                Text="Privato" TextAlign="Left" onclick="SingleSelect('chk',this);" ToolTip="Documento creato con visibilità limitata al solo ruolo e utente proprietario">
                                            </asp:CheckBox>
                                        </td>
                                        <td width="50%" style="padding-left:10px;padding-bottom:5px;">
                                            <asp:CheckBox ID="chkUtente" name="chkUtente" runat="server" CssClass="testo_grigio_scuro"
                                                Text="" TextAlign="Left" onclick="SingleSelect('chk',this);" ToolTip="Documento creato con visibilità limitata al solo utente proprietario">
                                            </asp:CheckBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td height="2">
                            </td>
                        </tr>
                        <asp:Panel ID="pnl_sessoEta" runat="server">
                            <tr>
                                <td>
                                    <!-- INIZIO TABELLA Sesso-->
                                    <table class="info_grigio" id="tbl_etasesso" cellspacing="0" cellpadding="0" width="96%"
                                        align="center" border="0">
                                        <tr>
                                            <td class="titolo_scheda" valign="middle" colspan="2" height="8">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Sesso
                                                / Eta'
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="10">
                                                &nbsp;
                                                <asp:RadioButtonList ID="rbl_sesso" runat="server" Height="12px" CssClass="testo_grigio"
                                                    RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0">
                                                    <asp:ListItem Value="0">M&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="1">F</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td height="10">
                                                &nbsp;
                                                <asp:DropDownList ID="ddl_eta" runat="server" CssClass="testo_grigio">
                                                    <asp:ListItem Value="0">Non presente</asp:ListItem>
                                                    <asp:ListItem Value="1">0-20</asp:ListItem>
                                                    <asp:ListItem Value="2">21-45</asp:ListItem>
                                                    <asp:ListItem Value="3">46-65</asp:ListItem>
                                                    <asp:ListItem Value="4">oltre 66</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnl_protocolloTitolario" runat="server" Visible="false">
                            <tr>
                                <td>
                                    <!-- INIZIO TABELLA PROTOCOLLO TITOLARIO-->
                                    <table class="info_grigio" id="TABLE2" cellspacing="0" cellpadding="0" width="96%"
                                        align="center" border="0">
                                        <tr>
                                            <td style="padding-left: 8px;">
                                                <asp:Label ID="lbl_etProtTitolario" runat="server" CssClass="titolo_scheda" Width="20%"></asp:Label>
                                                <asp:Label ID="lbl_txtProtTitolario" runat="server" CssClass="testo_grigio" Width="70%"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td height="2">
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td>
                                <!-- INIZIO TABELLA OGGETTO-->
                                <table class="info_grigio" id="tbl_oggetto" cellspacing="0" cellpadding="0" width="96%"
                                    align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle">
                                            &nbsp;&nbsp;Oggetto&nbsp;*
                                        </td>
                                        <td valign="middle" align="right">
                                            <table id="Table4">
                                                <tr>
                                                    <!-- Aggiunta del bottone per il correttore ortografico -->
                                                    <td>
                                                        <cc1:ImageButton ID="btn_Correttore" runat="server" Width="19px" AlternateText="Correttore ortografico"
                                                            DisabledUrl="../images/proto/check_spell.gif" Height="17" Visible="False" ImageUrl="../images/proto/check_spell.gif"
                                                            Enabled="false" OnClick="btn_Correttore_Click"></cc1:ImageButton>
                                                    </td>
                                                    <!-- fine aggiunta -->
                                                    <td>
                                                        <cc1:ImageButton ID="imgDescOgg" runat="server" Width="19px" AlternateText="Descrizione campo oggetto"
                                                            DisabledUrl="../images/rubrica/l_exp_o_grigia.gif" Height="17" ImageUrl="../images/rubrica/l_exp_o_grigia.gif"
                                                            OnClick="imgDescOgg_Click" Enabled="false"></cc1:ImageButton>
                                                    </td>
                                                    <td>
                                                        <cc1:ImageButton ID="btn_oggettario" runat="server" Width="19px" AlternateText="Seleziona un  oggetto nell'oggettario"
                                                            Tipologia="DO_PRO_OGGETTARIO" Height="17" ImageUrl="../images/proto/ico_oggettario.gif"
                                                            DisabledUrl="../images/proto/ico_oggettario.gif"></cc1:ImageButton>
                                                    </td>
                                                    <td>
                                                        <cc1:ImageButton ID="btn_modificaOgget" runat="server" AlternateText="Modifica" Tipologia="DO_PROT_OG_MODIFICA"
                                                            Height="17" ImageUrl="../images/proto/matita.gif" DisabledUrl="../images/proto/matita.gif">
                                                        </cc1:ImageButton>
                                                    </td>
                                                    <td>
                                                        <cc1:ImageButton ID="btn_storiaOgg" runat="server" Width="18" AlternateText="Storia"
                                                            Tipologia="DO_PROT_OG_STORIA" Height="17" ImageUrl="../images/proto/storia.gif"
                                                            DisabledUrl="../images/proto/storia.gif"></cc1:ImageButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <uc4:Oggetto ID="ctrl_oggetto" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td height="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <!-- INIZIO TABELLA PAROLE CHIAVE -->
                                <table class="info_grigio" id="tbl_paroleChiave" cellspacing="0" cellpadding="0"
                                    width="96%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" width="86%" height="19">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Parole
                                            chiave&nbsp;
                                        </td>
                                        <td valign="middle" align="right">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0"><cc1:ImageButton
                                                ID="btn_selezionaParoleChiave" runat="server" Width="19px" AlternateText="Seleziona parole chiave"
                                                Tipologia="DO_PRO_SELEZIONA" Height="17px" ImageUrl="../images/proto/ico_parole.gif"
                                                DisabledUrl="../images/proto/ico_parole.gif"></cc1:ImageButton>
                                        </td>
                                        <td valign="middle" align="right">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0"><cc1:ImageButton
                                                ID="btn_eliminaParoleChiave" runat="server" Width="19px" AlternateText="Elimina parole chiave"
                                                Tipologia="DO_PRO_ELIMINAPAROLE" Height="17px" ImageUrl="../images/proto/cancella.gif"
                                                DisabledUrl="../images/proto/cancella.gif"></cc1:ImageButton>
                                        </td>
                                        <td>
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" height="25">
                                            &nbsp;
                                            <asp:ListBox ID="lbx_paroleChiave" runat="server" Width="350px" Height="44px" CssClass="testo_grigio">
                                            </asp:ListBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <img height="3" src="../images/proto/spaziatore.gif" width="8"
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td height="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table class="info_grigio" id="tbl_note" cellspacing="0" cellpadding="0" width="96%"
                                    align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" align="center">
                                            <uc2:DettaglioNota ID="dettaglioNota" runat="server" Width="95%" TipoOggetto="Documento"
                                                Rows="3" TextMode="MultiLine" PAGINA_CHIAMANTE="docProfilo" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td height="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <!-- INIZIO PANNELLO NUM OGGETTO E COMMISSIONE REFERENTE -->
                                <asp:Panel ID="panel_numOgg_commRef" runat="server" Visible="False">
                                    <table class="info_grigio" id="tbl_numOggeCommRef" height="30" cellspacing="0" cellpadding="0"
                                        width="96%" align="center" border="0">
                                        <tr>
                                            <td class="titolo_scheda" valign="middle" height="19">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Numero
                                                oggetto
                                            </td>
                                            <td class="titolo_scheda" valign="middle" nowrap height="19">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                                Commissione referente
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                            <td height="15">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                                <asp:TextBox ID="txt_numOggetto" runat="server" CssClass="testo_grigio"></asp:TextBox>
                                            </td>
                                            <td class="menu_1_grigio" valign="top" height="15">
                                                <img border="0" height="1" src="../images/proto/spaziatore.gif" width="8">
                                                    <asp:DropDownList ID="ddl_commRef" runat="server" CssClass="testo_grigio">
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem Value="I">I</asp:ListItem>
                                                        <asp:ListItem Value="II">II</asp:ListItem>
                                                        <asp:ListItem Value="III">III</asp:ListItem>
                                                        <asp:ListItem Value="IV">IV</asp:ListItem>
                                                        <asp:ListItem Value="V">V</asp:ListItem>
                                                    </asp:DropDownList>
                                                </img>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <img height="3" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr id="tr_tipologia" runat="server" visible="true">
                            <td>
                                <!-- INIZIO TABELLA TIPO_ATTO -->
                                <table class="info_grigio" id="tbl_tipoAtto" cellspacing="0" cellpadding="0" width="96%"
                                    align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" width="34%">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Tipologia
                                            documento
                                                    </td>
                                        <td valign="middle" class="testo_grigio" align="left">
                                            <asp:Panel ID="pnl_star" runat="server">
                                                *</asp:Panel>
                                        </td>
                                        <td style="height: 20px" valign="middle" align="right">
                                            <cc1:ImageButton ID="btn_addTipoAtto" runat="server" Width="18" AlternateText="Aggiungi"
                                                Height="17" ImageUrl="../images/proto/aggiungi.gif" DisabledUrl="../images/proto/aggiungi.gif"
                                                OnClick="btn_addTipoAtto_Click"></cc1:ImageButton>
                                        </td>
                                        <td style="height: 20px">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio" align="left" height="25" colspan="2" style="padding-left: 10px;">
                                            <asp:DropDownList ID="ddl_tipoAtto" runat="server" Width="300px" CssClass="testo_grigio"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                        <td valign="middle" align="right" colspan="2">
                                        <cc1:ImageButton class="ImgHand" ID="btn_delTipologyDoc" runat="server" AlternateText="Elimina la tipologia del documento"
                                                        DisabledUrl="../images/scan/rimuovi.gif" Tipologia="ELIMINA_TIPOLOGIA_DOC"
                                                        ImageUrl="../images/scan/rimuovi.gif" ToolTip="Elimina la tipologia del documento" Visible="false" style="text-align:right"/>
                                            <asp:ImageButton ID="btn_CampiPersonalizzati" Visible="true" runat="server" AlternateText="Visualizza campi tipologia"
                                                ImageUrl="../images/proto/ico_oggettario.gif" OnClick="btn_CampiPersonalizzati_Click" />
                                            
                                        </td>
                                    </tr>
                                    <asp:Panel ID="Panel_DiagrammiStato" runat="server" Visible="false">
                                        <tr>
                                            <td class="titolo_scheda" colspan="3" style="height: 20px" valign="middle" height="20"
                                                colspan="4">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Stato:<img
                                                    height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                                <asp:Label ID="lbl_statoAttuale" runat="server" CssClass="titolo_scheda" Visible="False"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="testo_grigio" align="right" height="25" colspan="2" style="padding-left: 10px;">
                                                <asp:DropDownList ID="ddl_statiSuccessivi" runat="server" Width="300px" CssClass="testo_grigio">
                                                </asp:DropDownList>
                                            </td>
                                            <td valign="middle" align="right" colspan="2">
                                                <cc1:ImageButton ID="img_btnStoriaDiagrammi" DisabledUrl="../images/proto/storia.gif"
                                                    ImageUrl="../images/proto/storia.gif" Height="17" AlternateText="Storia" Width="18"
                                                    runat="server"></cc1:ImageButton>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="Panel_DataScadenza" runat="server" Visible="false">
                                        <tr>
                                            <td class="titolo_scheda" style="height: 20px" valign="middle" height="20" colspan="3">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Data Scadenza
                                                :<img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                                <cc1:DateMask ID="txt_dataScadenza" runat="server" CssClass="testo_grigio" Width="75px"></cc1:DateMask>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td height="2">
                            </td>
                        </tr>
                        <tr id="trDocumentoPrincipale" runat="server">
                            <td>
                                <table class="info_grigio" id="tblDocumentoPrincipale" cellspacing="0" cellpadding="0"
                                    width="96%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" align="left" colspan="2">
                                            <img height="3" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            Documento principale
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <img height="3" src="../images/proto/spaziatore.gif" border="0">
                                            <asp:TextBox ID="txtDocumentoPrincipale" runat="server" CssClass="testo_grigio" Width="95%"
                                                ReadOnly="true"></asp:TextBox>
                                        </td>
                                        <td class="titolo_scheda">
                                            <cc1:ImageButton ID="btnGoToDocumentoPrincipale" runat="server" ImageUrl="../images/proto/goto.gif"
                                                ImageAlign="Middle" ToolTip="Vai al documento principale" OnClick="btnGoToDocumentoPrincipale_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <asp:Panel ID="panel_Firm" runat="server" Visible="False">
                            <tr>
                                <td height="2">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table class="info_grigio" id="tbl_dest" cellspacing="0" cellpadding="0" width="96%"
                                        align="center" border="0">
                                        <tr>
                                            <td class="titolo_scheda" valign="middle" width="90%" height="19">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Firmatario
                                            </td>
                                            <td valign="middle" align="right">
                                                <cc1:ImageButton ID="btn_aggiungiFirmatario" DisabledUrl="../images/proto/aggiungi.gif"
                                                    ImageUrl="../images/proto/aggiungi.gif" Height="17" AlternateText="Aggiungi"
                                                    Width="18" runat="server"></cc1:ImageButton>
                                            </td>
                                            <td>
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" height="25">
                                                &nbsp;
                                                <asp:Label ID="lbl_Nome_F" runat="server" CssClass="testo_grigio">Nome</asp:Label>
                                                <asp:TextBox ID="txt_NomeFirma" runat="server" CssClass="testo_grigio"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label1" runat="server" CssClass="testo_grigio">Cognome</asp:Label>
                                                <asp:TextBox ID="txt_CognomeFirma" runat="server" CssClass="testo_grigio"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <!-- listBox Firmatari-->
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table class="info_grigio" id="tbl_dest9" cellspacing="0" cellpadding="0" width="96%"
                                        align="center" border="0">
                                        <tr height="30">
                                            <td>
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
                                            <td>
                                                <asp:ListBox ID="lbx_firmatari" runat="server" Height="40" Width="330" CssClass="testo_grigio"
                                                    Rows="3"></asp:ListBox>
                                            </td>
                                            <td valign="middle" align="right" width="29" height="20">
                                                <cc1:ImageButton ID="btn_cancFirmatario" DisabledUrl="../images/proto/cancella.gif"
                                                    ImageUrl="../images/proto/cancella.gif" Height="16px" AlternateText="Cancella"
                                                    Width="18" runat="server"></cc1:ImageButton>
                                            </td>
                                            <td>
                                                <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="rispProtoPanelGrigio" CssClass="info_grigio" runat="server" Visible="true">
                            <tr>
                                <td>
                                    <!-- CATENE -->
                                    <table class="info_grigio" id="tbl_rispostaprotoGrigio" cellspacing="0" cellpadding="0"
                                        width="96%" align="center" border="0">
                                        <tr>
                                            <td class="titolo_scheda" style="height: 23px" valign="middle" align="center">
                                                <table style="width: 150px; height: 30px" align="left">
                                                    <tr>
                                                        <td class="titolo_scheda" align="left">
                                                            &nbsp;&nbsp;Risposta al documento
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="titolo_scheda" align="right">
                                                    <tr>
                                                        <td align="right" width="26" height="17">
                                                            <cc1:ImageButton ID="btn_risp_sx" BorderWidth="0" runat="server" DisabledUrl="../images/proto/RispProtocollo/freccina_sx.gif"
                                                                ImageUrl="../images/proto/RispProtocollo/freccina_sx.gif" OnClick="btn_risp_sx_Click">
                                                            </cc1:ImageButton><img height="1" src="../images/proto/spaziatore.gif" width="1"
                                                                border="0">
                                                        </td>
                                                        <td>
                                                            <cc1:ImageButton ID="btn_in_risposta_a" BorderWidth="0" Width="26px" runat="server"
                                                                AlternateText="Ricerca i documenti a cui poter rispondere" DisabledUrl="../images/proto/RispProtocollo/lentePreview.gif"
                                                                ImageUrl="../images/proto/RispProtocollo/lentePreview.gif" OnClick="btn_in_risposta_a_Click"
                                                                ToolTip="Ricerca i documenti a cui poter rispondere"></cc1:ImageButton>
                                                            <cc1:ImageButton ID="btn_Risp" BorderWidth="0" runat="server" AlternateText="Crea documento in risposta"
                                                                DisabledUrl="../images/proto/RispProtocollo/catena.gif" ImageUrl="../images/proto/RispProtocollo/catena.gif"
                                                                OnClick="btn_Risp_Click" ToolTip="Crea documento in risposta"></cc1:ImageButton>
                                                            <cc1:ImageButton ID="btn_risp_grigio" BorderWidth="0" runat="server" AlternateText="Crea protocollo in risposta"
                                                                DisabledUrl="../images/proto/RispProtocollo/catena_grigio.gif" ImageUrl="../images/proto/RispProtocollo/catena_grigio.gif"
                                                                OnClick="btn_risp_grigio_Click" Visible="false" ToolTip="Crea protocollo in risposta">
                                                            </cc1:ImageButton>
                                                        </td>
                                                        <td align="center" width="25" height="17">
                                                            <cc1:ImageButton ID="btn_risp_dx" BorderWidth="0" runat="server" AlternateText="visualizza elenco documenti in risposta"
                                                                DisabledUrl="../images/proto/RispProtocollo/freccina_dx.gif" ImageUrl="../images/proto/RispProtocollo/freccina_dx.gif"
                                                                OnClick="btn_risp_dx_Click" ToolTip="visualizza elenco documenti in risposta">
                                                            </cc1:ImageButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <asp:Panel ID="pnl_text_risposta" runat="server" Visible="False">
                                            <tr>
                                                <td align="center">
                                                    <asp:TextBox ID="txt_RispostaDocGrigio" Width="367px" runat="server" CssClass="testo_grigio"
                                                        ReadOnly="True"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                    </table>
                                    <!--FINE CATENE-->
                                </td>
                            </tr>
                            <tr>
                            <td height="2">
                            </td>
                        </tr>
                        </asp:Panel>
                        <tr>
                            <td>
                                <!-- Fascicolazione / Trasmissione Rapida -->
                                <table class="info_grigio" id="tbl_fasc_rapida" cellspacing="0" cellpadding="0" width="96%"
                                    align="center" border="0">
                                    <asp:Panel ID="pnl_fasc_rapida" runat="server" Visible="True">
                                        <tbody>
                                            <tr>
                                                <td class="titolo_scheda" valign="middle">
                                                    &nbsp;&nbsp;<asp:Label ID="labelFascRapid" runat="server" Width="80%"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <cc1:ImageButton ID="btn_titolario" runat="server" ImageUrl="../images/proto/ico_titolario_noattivo.gif"
                                                        AlternateText="Titolario" ToolTip="Titolario" Tipologia="DO_CLA_TITOLARIO" DisabledUrl="../images/proto/ico_titolario_noattivo.gif"
                                                        OnClick="btn_titolario_Click"></cc1:ImageButton>
                                                    <cc1:ImageButton class="ImgHand" ID="imgFasc" runat="server" AlternateText="Ricerca Fascicoli"
                                                        DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" Tipologia="DO_CLA_VIS_PROC"
                                                        ImageUrl="../images/proto/ico_fascicolo_noattivo.gif" ToolTip="Ricerca Fascicoli">
                                                    </cc1:ImageButton>
                                                    <cc1:ImageButton class="ImgHand" Visible="false" ID="imgNewFasc" runat="server" AlternateText="Nuovo Fascicolo"
                                                        DisabledUrl="../images/fasc/fasc_direct.gif" Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/fasc/fasc_direct.gif"
                                                        ToolTip="Nuovo Fascicolo"></cc1:ImageButton>
                                                </td>
                                                <td>
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="2" border="0" />
                                                </td>
                                            </tr>
                                            <asp:Panel ID="Panel1" runat="server" Visible="false">
                                                <tr>
                                                    <td>
                                                        &nbsp;&nbsp;<asp:Label ID="Label2" runat="server" CssClass="titolo_scheda"></asp:Label>
                                                    </td>
                                                </tr>
                                            </asp:Panel>
                            </td>
                        </tr>
                        <asp:Panel ID="pnl_fasc_Primaria" runat="server" Visible="false">
                            <tr>
                                <td colspan="2">
                                    <table cellpadding="3" border="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_fasc_Primaria" runat="server" CssClass="titolo_scheda"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td colspan="2" height="25">
                                &nbsp;
                                <asp:TextBox ID="txt_CodFascicolo" AutoPostBack="True" Width="75px" runat="server"
                                    CssClass="testo_grigio" ReadOnly="False"></asp:TextBox>
                                <asp:TextBox ID="txt_DescFascicolo" AutoComplete="off" Width="250px" runat="server"
                                    CssClass="testo_grigio" ReadOnly="True"></asp:TextBox>
                            </td>
                        </tr>
                        <asp:Panel ID="pnl_trasm_rapida" runat="server" Visible="True">
                            <tr>
                                <td class="titolo_scheda" valign="middle">
                                    &nbsp;&nbsp;Trasmissione Rapida
                                </td>
                                <td valign="middle" align="right">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" height="25">
                                    <table width="100%" border="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txt_codModello" runat="server" Width="75px" AutoPostBack="true"
                                                    CssClass="testo_grigio" Visible="false"></asp:TextBox>
                                            </td>
                                            <td>
                                                <div class="holder">
                                                    <asp:DropDownList CssClass="testo_grigio" ID="ddl_tmpl" TabIndex="420" runat="server"
                                                        AutoPostBack="True" Width="350px">
                                                    </asp:DropDownList>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </asp:Panel>
                    </table>
                </td>
            </tr>
            </asp:Panel>
            <tr>
                <td height="2">
                </td>
            </tr>
            <tr>
                <td height="100%">
                    <cc2:MessageBox ID="MessageBox1" runat="server"></cc2:MessageBox>
                    <cc2:MessageBox ID="MessageBox" runat="server"></cc2:MessageBox>
                    <cc2:MessageBox ID="msg_TrasmettiDoc" runat="server"></cc2:MessageBox>
                    <cc2:MessageBox ID="msg_PersonaleDoc" runat="server"></cc2:MessageBox>
                    <cc2:MessageBox ID="msg_copiaDoc" runat="server"></cc2:MessageBox>
                    <cc2:MessageBox ID="msg_rimuoviTipologia" runat="server"></cc2:MessageBox>
                </td>
            </tr>
        </table>
        </td> </tr>
        <tr height="1">
            <td>
                <iframe id="ifrPrintPen" src="<%=UrlStampaEtichettaPage%>" width="0" height="0">
                </iframe>
            </td>
        </tr>
        <tr>
            <td>
                <!-- BOTTONIERA -->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" align="center" border="0">
                    <tr>
                        <td valign="top" colspan="5" height="5">
                            <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:ImageButton ID="btn_salva_disabled" runat="server" AlternateText="Salva" Thema="btn_"
                                SkinID="salva_nonAttivo" DisabledUrl="../images/bottoniera/btn_salva_nonAttivo.gif"
                                EnableViewState="False" Enabled="False"></cc1:ImageButton><cc1:ImageButton ID="btn_salva"
                                    runat="server" AlternateText="Salva" Tipologia="DO_PRO_SALVA" Thema="btn_" SkinID="salva_Attivo"
                                    DisabledUrl="../images/bottoniera/btn_salva_nonAttivo.gif"></cc1:ImageButton>
                        </td>
                        <td>
                            <cc1:ImageButton ID="btn_prodisponiProtocollo" runat="server" AlternateText="Predisponi alla protocollazione"
                                Tipologia="DO_PRO_PREDISPONI" Thema="btn_" SkinID="predisponi_Attivo" DisabledUrl="../images/bottoniera/btn_predisponi_nonAttivo.gif">
                            </cc1:ImageButton>
                        </td>
                        <td>
                            <cc1:ImageButton ID="btn_stampa" runat="server" AlternateText="Stampa scheda riassuntiva"
                                Tipologia="DO_PRO_STAMPASCHEDA" Thema="btn_" SkinID="stampa_Attivo" DisabledUrl="../images/bottoniera/btn_stampa_nonAttivo.gif">
                            </cc1:ImageButton>
                        </td>
                        <td>
                            <cc1:ImageButton ID="btn_rimuovi" runat="server" AlternateText="Rimuovi" Tipologia="DO_PRO_RIMUOVI"
                                Thema="btn_" SkinID="rimuovi_Attivo" DisabledUrl="../images/bottoniera/btn_rimuovi_nonAttivo.gif">
                            </cc1:ImageButton>
                        </td>
                        <td>
                            <cc1:ImageButton ID="btn_riproponi" runat="server" AlternateText="Riproponi" Tipologia="DO_DOC_RIPROPONI"
                                Thema="btn_" SkinID="riproponi_Attivo" DisabledUrl="../images/bottoniera/btn_riproponi_nonAttivo.gif">
                            </cc1:ImageButton>
                        </td>
                    </tr>
                </table>
                <!--FINE BOTTONIERA -->
            </td>
        </tr>
        </table>
    </div>
    </form>
</body>
</html>
