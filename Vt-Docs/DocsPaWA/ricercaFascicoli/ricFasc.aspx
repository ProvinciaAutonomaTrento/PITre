<%@ Page Language="c#" CodeBehind="ricFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicoli.browsingFasc"
    EnableViewStateMac="False" %>

<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="aof" TagName="AuthorOwnerFilter" Src="~/UserControls/AuthorOwnerFilter.ascx" %>
<%@ Import Namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register Src="../UserControls/RicercaNote.ascx" TagName="RicercaNote" TagPrefix="uc2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>DOCSPA > fascicoli_sx</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>
    <link id="styleLink" runat="server" href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" id="btn_ricFascicoli_Click" event="onclick()" for="btn_ricFascicoli">
			WaitSearch();
    </script>
    <script language="JavaScript">

        var w = window.screen.width;
        var h = window.screen.height;
        var new_w = (w - 100) / 2;
        var new_h = (h - 400) / 2;

        function apriPopupAnteprima() {
            //window.open('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicercheFasc.aspx', '', 'top = ' + new_h + ' left = ' + new_w + ' width=500,height=420,scrollbars=YES');
            window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicercheFasc.aspx', '', 'dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
        }

        function apriSalvaRicerca() {
            var retval = window.showModalDialog('../popup/salvaRicerca.aspx?tipo=F', window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
            if (retval != null) {
                top.principale.iFrame_dx.document.location = '../waitingpage.htm';
            }
        }

        function WaitSearch() {
            window.document.body.style.cursor = 'wait';

            WndWait();
        }

        ns = window.navigator.appName == "Netscape"
        ie = window.navigator.appName == "Microsoft Internet Explorer"

        function btn_titolario_onClick(queryString) {
            var retValue = true;
            if (retValue) {
                ApriTitolario(queryString, "gestFasc");
            }

            return retValue;
        }

        function createFascicolo() {
            var l_createFascicolo = '<%=this.l_createFascicolo%>';
            if (l_createFascicolo) {
                __doPostBack('', '');
                var dx_frame = window.parent.frames[1];
                //window.document.location = 'tabRisultatiRicFasc.aspx?newFasc=1';
            }
        }

        function openDesc() {
            try {
                if (ns) {
                    showbox = document.layers[1]
                    showbox.visibility = "show";
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
                    showBox = document.all.descreg;
                    showBox.style.visibility = "visible";
                }
            }
            catch (e) {
                return false;
            }
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
            catch (e) {
                return false;
            }
        }

        function _ApriRubrica(target) {
            var r = new Rubrica();

            switch (target) {
                // Gestione fascicoli - Locazione fisica    
                case "gf_locfisica":
                    r.CallType = r.CALLTYPE_GESTFASC_LOCFISICA;
                    break;

                // Gestione fascicoli - Ufficio referente    
                case "gf_uffref":
                    r.CallType = r.CALLTYPE_GESTFASC_UFFREF;
                    break;

                case "gf_proprietario":
                    r.CallType = r.CALLTYPE_RICERCA_CREATOR;
                    break;
            }
            var res = r.Apri();
        }

        function ApriSceltaTitolario(codClassifica) {
            //window.open('../popup/sceltaTitolari.aspx?CodClassifica='+codClassifica,'','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
            window.showModalDialog('../popup/sceltaTitolari.aspx?CodClassifica=' + codClassifica, '', 'dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
            fascicoli_sx.submit();

        }

        function ApriSceltaNodo(queryString) {
            //window.open('../popup/sceltaNodoTitolario.aspx?' + queryString, '', 'top = ' + new_h + ' left = ' + new_w + ' width=500,height=420,scrollbars=YES');
            window.showModalDialog('../popup/sceltaNodoTitolario.aspx?' + queryString, '', 'dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
            fascicoli_sx.submit();
        }

        function ShowDialogRubrica(txtTipoCorrispondente) {
            var w_width = screen.availWidth - 40;
            var w_height = screen.availHeight - 35;

            var navapp = navigator.appVersion.toUpperCase();
            if ((navapp.indexOf("WIN") != -1) && (navapp.indexOf("NT 5.1") != -1))
                w_height = w_height + 20;

            var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";

            var params = "calltype=" + Rubrica.prototype.CALLTYPE_RICERCA_CREATOR + "&tipo_corr=" + document.getElementById(txtTipoCorrispondente).value;

            var urlRubrica = "../popup/rubrica/Rubrica.aspx";
            var res = window.showModalDialog(urlRubrica + "?" + params, window, opts);
        }

        // Permette di inserire solamente caratteri numerici
        function ValidateNumericKey() {
            var inputKey = event.keyCode;
            var returnCode = true;

            if (inputKey > 47 && inputKey < 58) {
                return;
            }
            else {
                returnCode = false;
                event.keyCode = 0;
            }

            event.returnValue = returnCode;
        }

        function apriModificaRicerca(codiceRicerca) {
            var retval = window.showModalDialog("../popup/ModificaRicerca.aspx?idRicerca=" + codiceRicerca + "", window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
            if (retval != null) {
                top.principale.iFrame_dx.document.location = '../waitingpage.htm';
            }
        }

        function apriSalvaRicercaADL() {
            var retVal = window.showModalDialog('../popup/salvaRicerca.aspx?ricADL=1&tipo=F', window.self, 'dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
            if (retval != null) {
                top.principale.iFrame_dx.document.location = '../waitingpage.htm';
            }
        }

    </script>
    <script language="javascript" id="clientEventHandlersJS">
		<!--
        function window_onload() { }
		//-->
    </script>
</head>
<body language="javascript" leftmargin="0" topmargin="0" ms_positioning="GridLayout">
    <form id="fascicoli_sx" method="post" runat="server" defaultbutton="btn_ricFascicoli">
    <div id="divScroll">
        <input id="hd_systemIdLF" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
        <input id="hd_systemIdUffRef" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
        <table id="tbl_contenitore" cellspacing="0" cellpadding="0" align="center" border="0"
            width="410" height="100%" style="margin-top: 2px;">
            <tr>
                <td valign="top" align="center">
                    <table class="contenitore" cellspacing="0" cellpadding="0" width="100%" border="0">
                        <tr>
                            <td>
                                <table class="info_grigio" id="Table7" cellspacing="0" cellpadding="0" width="98%"
                                    align="center" border="0" style="margin-top: 2px;">
                                    <tr height="22" valign="middle">
                                        <td class="titolo_scheda" width="15%" valign="middle">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registro
                                        </td>
                                        <td valign="middle" width="60%">
                                            <asp:DropDownList ID="ddl_registri" runat="server" CssClass="testo_grigio" Width="134px"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                            &nbsp;
                                            <asp:Image ID="icoReg" onmouseover="openDesc()" onmouseout="closeDesc()" runat="server"
                                                ImageAlign="AbsMiddle" ImageUrl="../images/proto/ico_registro.gif" BorderWidth="0px"
                                                BorderStyle="Solid" BorderColor="Gray"></asp:Image>
                                        </td>
                                        <td align="right" width="25%" valign="middle" class="testo_grigio_scuro">
                                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td align="right" valign="middle" class="testo_grigio_scuro">
                                                        Stato
                                                    </td>
                                                    <td align="right" valign="middle" class="testo_grigio_scuro">
                                                        <asp:Image ID="img_statoReg" runat="server" BorderWidth="1" ImageUrl="../images/stato_giallo2.gif"
                                                            Height="18" Width="52" ImageAlign="AbsMiddle"></asp:Image>
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
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
                                <table class="info_grigio" id="Table8" cellspacing="0" cellpadding="0" width="98%"
                                    align="center" border="0" style="margin-top: 2px;">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />
                                            <asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <cc1:ImageButton ID="btn_clear_fields" ImageUrl="../images/ricerca/remove_search_filter.gif"
                                                runat="server" AlternateText="Pulisci i campi" ToolTip="Pulisci i campi" CssClass="clear_flag"
                                                OnClick="btnCleanUpField_Click"></cc1:ImageButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <font size="1">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" /></font>
                                            <asp:DropDownList ID="ddl_Ric_Salvate" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                                Width="344px">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:ImageButton ID="btn_Canc_Ric" ImageUrl="../images/proto/cancella.gif" Width="19px"
                                                Height="17px" runat="server" AlternateText="Rimuove la ricerca selezionata">
                                            </asp:ImageButton><font size="1"></font>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table id="Table9" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                    style="margin-top: 2px;">
                                    <tr>
                                        <td class="menu_1_rosso" valign="top" align="center">
                                            Nodo titolario
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0" class="info_grigio">
                                                <tr>
                                                    <td class="titolo_scheda">
                                                        <table align="center" cellspacing="0" cellpadding="0" border="0" width="95%">
                                                            <tr height="15">
                                                                <td width="75%" align="right">
                                                                    <asp:RadioButtonList ID="OptLst" runat="server" CssClass="testo_grigio_scuro" Width="199px"
                                                                        AutoPostBack="True" Height="20px" RepeatDirection="Horizontal">
                                                                        <asp:ListItem Value="Cod" Selected="True">Codice</asp:ListItem>
                                                                        <asp:ListItem Value="Liv">Livello</asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                                <td valign="middle" align="right">
                                                                    <cc1:ImageButton ID="btn_titolario" ImageUrl="../images/proto/ico_titolario_noattivo.gif"
                                                                        Height="17px" runat="server" Tipologia="DO_CLA_TITOLARIO" DisabledUrl="../images/proto/ico_titolario_noattivo.gif"
                                                                        AlternateText="Titolario"></cc1:ImageButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 20px">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_Titolari" runat="server" CssClass="testo_grigio" Width="22%">Titolario</asp:Label>
                                                        <asp:DropDownList ID="ddl_titolari" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        &nbsp;
                                                        <asp:Label ID="lbl_codClass" runat="server" CssClass="testo_grigio" Width="22%"><B>Codice</B></asp:Label>
                                                        <asp:TextBox ID="txt_codClass" runat="server" CssClass="testo_grigio" Width="24%"
                                                            AutoPostBack="True"></asp:TextBox>
                                                        <asp:Label ID="lbl_protoTitolario" runat="server" CssClass="testo_grigio" Width="23%"
                                                            Style="text-align: right" Visible="false"></asp:Label>
                                                        <asp:TextBox ID="txt_protoPratica" runat="server" Width="24%" MaxLength="5" CssClass="testo_grigio"
                                                            Visible="false" OnTextChanged="txt_protoPratica_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 20px">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_livello1" runat="server" CssClass="testo_grigio" Width="22%">Livello1</asp:Label>
                                                        <asp:DropDownList ID="ddl_livello1" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 20px">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_livello2" runat="server" CssClass="testo_grigio" Width="22%">Livello2</asp:Label>
                                                        <asp:DropDownList ID="ddl_livello2" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 20px">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_livello3" runat="server" CssClass="testo_grigio" Width="22%">Livello3</asp:Label>
                                                        <asp:DropDownList ID="ddl_livello3" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 20px">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_livello4" runat="server" CssClass="testo_grigio" Width="22%">Livello4</asp:Label>
                                                        <asp:DropDownList ID="ddl_livello4" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="20">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_livello5" runat="server" CssClass="testo_grigio" Width="22%">Livello5</asp:Label>
                                                        <asp:DropDownList ID="ddl_livello5" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="20">
                                                        &nbsp;
                                                        <asp:Label ID="lbl_livello6" runat="server" CssClass="testo_grigio" Width="22%">Livello6</asp:Label>
                                                        <asp:DropDownList ID="ddl_livello6" runat="server" CssClass="testo_grigio" Width="73%"
                                                            AutoPostBack="True" EnableViewState="True">
                                                        </asp:DropDownList>
                                                        <br>
                                                    </td>
                                                </tr>
                                                <!-- fine tabella classifica-->
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <table id="Table10" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                    style="margin-top: 2px;">
                                    <tr>
                                        <td height="5" class="menu_1_rosso" align="center">
                                            Filtri di Ricerca
                                        </td>
                                    </tr>
                                    <!-- inizio modifica 21/12/2004 -->
                                    <tr align="center">
                                        <td align="center">
                                            <table class="info_grigio" id="Table11" cellspacing="0" cellpadding="0" width="100%"
                                                align="center" border="0" style="margin-top: 2px;">
                                                <tr>
                                                    <td>
                                                        <!-- tabella Data APERTURA-->
                                                        <table id="Table1" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td class="titolo_scheda" valign="baseline" width="50px">
                                                                    Aperto&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_dataA" runat="server" CssClass="testo_grigio" AutoPostBack="true">
                                                                        <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                                        <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                                        <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                                        <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                                        <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_initdataA" runat="server" CssClass="testo_grigio" Visible="true">Da</asp:Label>
                                                                    <uc3:Calendario ID="txt_initDataA" runat="server" Visible="true" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_finedataA" runat="server" CssClass="testo_grigio" Visible="true">A</asp:Label>
                                                                    <uc3:Calendario ID="txt_fineDataA" runat="server" Visible="true" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <!-- tabella Data CHIUSURA-->
                                                        <table id="Table2" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td class="titolo_scheda" valign="baseline" width="50px">
                                                                    Chiuso&nbsp;&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_dataC" runat="server" CssClass="testo_grigio" AutoPostBack="true">
                                                                        <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                                        <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                                        <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                                        <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                                        <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_initdataC" runat="server" CssClass="testo_grigio" Visible="true">Da</asp:Label>
                                                                    <uc3:Calendario ID="txt_initDataC" runat="server" Visible="true" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_finedataC" runat="server" CssClass="testo_grigio" Visible="true">A</asp:Label>
                                                                    <uc3:Calendario ID="txt_fineDataC" runat="server" Visible="true" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <!-- tabella Data CREAZIONE-->
                                                        <table id="Table3" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td class="titolo_scheda" valign="baseline" width="50px">
                                                                    Creato&nbsp;&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_creaz" runat="server" CssClass="testo_grigio" AutoPostBack="true">
                                                                        <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                                        <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                                        <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                                        <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                                        <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_initCreaz" runat="server" CssClass="testo_grigio" Visible="true">Da</asp:Label>
                                                                    <uc3:Calendario ID="txt_initDataCrea" runat="server" Visible="true" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_finCreaz" runat="server" CssClass="testo_grigio" Visible="true">A</asp:Label>
                                                                    <uc3:Calendario ID="txt_fineDataCrea" runat="server" Visible="true" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table class="info_grigio" id="Table12" cellspacing="0" cellpadding="0" width="100%"
                                                align="center" border="0" style="margin-top: 2px; padding-top: 2px;">
                                                <tr>
                                                    <td valign="top">
                                                        <table cellspacing="0" cellpadding="0" width="98%" border="0">
                                                            <tr align="left">
                                                                <td valign="top">
                                                                    <img height="1" src="../images/proto/spaziatore.gif" width="2" border="0">
                                                                    <asp:Label ID="lbl_NumFasc" runat="server" CssClass="titolo_scheda" Visible="True">Num.</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtNumFasc" runat="server" CssClass="testo_grigio" Width="40px"
                                                                        BackColor="White"></asp:TextBox>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:Label ID="lblAnnoFasc" runat="server" CssClass="titolo_scheda" Width="13px"
                                                                        Visible="True">Anno</asp:Label>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:TextBox ID="txtAnnoFasc" runat="server" CssClass="testo_grigio" Width="40px"
                                                                        BackColor="White" MaxLength="4"></asp:TextBox>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:Label ID="lbl_Stato" runat="server" CssClass="titolo_scheda" Width="13px" Visible="True">Stato</asp:Label>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:DropDownList ID="ddlStato" runat="server" CssClass="testo_grigio" Width="65px">
                                                                        <asp:ListItem></asp:ListItem>
                                                                        <asp:ListItem Value="A">Aperto</asp:ListItem>
                                                                        <asp:ListItem Value="C">Chiuso</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:Label ID="lbl_tipo" runat="server" CssClass="titolo_scheda" Width="13px" Visible="True">Tipo</asp:Label>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:DropDownList ID="ddlTipo" runat="server" CssClass="testo_grigio" Width="75px" AutoPostBack="true">
                                                                        <asp:ListItem></asp:ListItem>
                                                                        <asp:ListItem Value="G">Generale</asp:ListItem>
                                                                        <asp:ListItem Value="P">Proced.le</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <table cellspacing="0" cellpadding="0" width="98%" border="0">
                                                            <tr align="left">
                                                                <td style="width: 65px">
                                                                    <img height="1" src="../images/proto/spaziatore.gif" width="2" border="0">
                                                                    <asp:Label ID="lbl_descr" runat="server" CssClass="titolo_scheda" Width="13px" Visible="True">Descriz.</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">&nbsp;
                                                                    <asp:TextBox ID="txtDescr" runat="server" CssClass="testo_grigio" Width="298px" BackColor="White"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table class="info_grigio" id="Table13" cellspacing="0" cellpadding="0" width="100%"
                                                align="center" border="0" style="margin-top: 2px;">
                                                <tr>
                                                    <div id="divNote" runat="server">
                                                        <td valign="top" height="20">
                                                            <table cellspacing="0" cellpadding="0" border="0" style="width: 100%">
                                                                <tr align="left">
                                                                    <td style="width: 65px">
                                                                        &nbsp;
                                                                        <asp:Label ID="lblNote" runat="server" CssClass="titolo_scheda" Width="13px" Visible="True">Note</asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <uc2:RicercaNote ID="rn_note" runat="server" TextMode="MultiLine" TextBoxWidth="320"
                                                                            DdlWidth="320" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                </tr>
                                                </div>
                                            </table>
                                            <table class="info_grigio" id="Table14" cellspacing="0" cellpadding="0" width="100%"
                                                align="center" border="0" style="margin-top: 2px;">
                                                <tr valign="top" height="20">
                                                    <td>
                                                        <!-- tabella Data SCADENZA-->
                                                        <table id="Table5" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td class="titolo_scheda" valign="baseline" width="50px">
                                                                    Scad.
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_dataScadenza" runat="server" CssClass="testo_grigio" AutoPostBack="true">
                                                                        <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                                        <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                                        <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                                        <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                                        <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_dataScadenza_DA" runat="server" CssClass="testo_grigio" Visible="true">Da</asp:Label>
                                                                    <uc3:Calendario ID="txt_dataScadenza_DA" runat="server" Visible="true" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_dataScadenza_A" runat="server" CssClass="testo_grigio" Visible="true">A</asp:Label>
                                                                    <uc3:Calendario ID="txt_dataScadenza_A" runat="server" Visible="true" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table class="info_grigio" id="Table19" cellspacing="0" cellpadding="0" width="100%"
                                                align="center" border="0" style="margin-top: 2px;">
                                                <tr valign="top">
                                                    <td>
                                                        <table id="Table4" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td class="titolo_scheda" valign="baseline" width="50px">
                                                                    Coll.
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_data_LF" runat="server" CssClass="testo_grigio" AutoPostBack="true">
                                                                        <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                                        <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                                        <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                                        <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                                        <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_dta_LF_DA" runat="server" CssClass="testo_grigio" Visible="true">Da</asp:Label>
                                                                    <uc3:Calendario ID="txt_dta_LF_DA" runat="server" Visible="true" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbl_dta_LF_A" runat="server" CssClass="testo_grigio" Visible="true">A</asp:Label>
                                                                    <uc3:Calendario ID="txt_dta_LF_A" runat="server" Visible="true" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr valign="top" height="20">
                                                    <td>
                                                        <table id="Table20" cellspacing="0" cellpadding="0" width="98%" align="center" border="0"
                                                            runat="server">
                                                            <tr align="left">
                                                                <td style="width: 80px">
                                                                    <asp:Label ID="Label3" runat="server" CssClass="titolo_scheda" Visible="True">Colloc. fisica</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                                    <asp:TextBox ID="txt_varCodRubrica_LF" runat="server" CssClass="testo_grigio" Width="70px"
                                                                        BackColor="White" AutoPostBack="True"></asp:TextBox>
                                                                    <asp:TextBox ID="txt_descr_LF" runat="server" CssClass="testo_grigio" Width="160px"
                                                                        BackColor="White" ReadOnly="True"></asp:TextBox>
                                                                    <asp:Image ID="btn_Rubrica" runat="server" ImageUrl="../images/proto/rubrica.gif">
                                                                    </asp:Image>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:Panel runat="server" ID="pnl_uffRef" Visible="False">
                                                <table class="info_grigio" id="Table15" cellspacing="0" cellpadding="0" width="100%"
                                                    align="center" border="0" style="margin-top: 2px;">
                                                    <tr valign="top">
                                                        <td>
                                                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                                <tr align="left">
                                                                    <td style="width: 80px">
                                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                                        <asp:Label ID="lbl_uffRef" runat="server" CssClass="titolo_scheda" Visible="True">Ufficio Ref.</asp:Label>
                                                                    </td>
                                                                    <td height="25">
                                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                                        <asp:TextBox ID="txt_cod_UffRef" runat="server" AutoPostBack="True" Width="70px"
                                                                            CssClass="testo_grigio" BackColor="White"></asp:TextBox>
                                                                        <asp:TextBox ID="txt_desc_uffRef" runat="server" Width="160px" CssClass="testo_grigio"
                                                                            BackColor="White" ReadOnly="True"></asp:TextBox>
                                                                        <asp:Image ID="btn_rubricaRef" runat="server" ImageUrl="../images/proto/rubrica.gif">
                                                                        </asp:Image>
                                                                    </td>
                                                                    <td valign="bottom" align="center">
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="pnl_profilazione" runat="server" Visible="false">
                                                <table class="info_grigio" id="Table16" cellspacing="0" cellpadding="0" width="100%"
                                                    align="center" border="0" style="margin-top: 2px;">
                                                    <tr>
                                                        <td>
                                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                            <asp:Label ID="lbl_tipoFascicolo" runat="server" CssClass="titolo_scheda" Width="73">Tipo fasc.</asp:Label>&nbsp;
                                                            <asp:DropDownList ID="ddl_tipoFasc" runat="server" AutoPostBack="true" CssClass="testo_grigio"
                                                                Width="255px">
                                                            </asp:DropDownList>
                                                            &nbsp;
                                                            <asp:ImageButton ID="img_dettagliProfilazione" ImageUrl="../images/proto/ico_oggettario.gif"
                                                                AlternateText="Ricerca per campi profilati" runat="server" Visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="pnl_Sottofascicoli" runat="server">
                                                <table class="info_grigio" id="Table17" cellspacing="0" cellpadding="0" width="100%"
                                                    align="center" border="0" style="margin-top: 2px;">
                                                    <tr height="22">
                                                        <td>
                                                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                                <tr align="left">
                                                                    <td style="width: 80px">
                                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                                        <asp:Label ID="lbl_sottofasc" runat="server" CssClass="titolo_scheda" Visible="True">Sottofasc.</asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                                        <asp:TextBox ID="txt_sottofascicolo" runat="server" CssClass="testo_grigio" Width="285px"
                                                                            BackColor="White"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <tr align="center">
							                    <td><aof:AuthorOwnerFilter ID="aofAuthor" runat="server" ControlType="Author" /></td>
                                            </tr>
                                            <tr>
								                <td height="2"></td>
							                </tr>
                                            <tr align="center">
                                                <td><aof:AuthorOwnerFilter ID="aofOwner" runat="server" ControlType="Owner" /></td>
                                            </tr>
                                            <asp:Panel ID="pnl_cons" runat="server">
                                                <table class="info_grigio" id="Table21" cellspacing="0" cellpadding="0" width="100%"
                                                    align="center" border="0" style="margin-top: 2px;">
                                                    <tr>
                                                        <tr>
                                                            <td class="titolo_scheda">
                                                                <asp:CheckBox ID="cb_Conservato" AutoPostBack="true" runat="server" />&nbsp;Conservato
                                                                &nbsp;&nbsp;
                                                                <asp:CheckBox ID="cb_NonConservato" AutoPostBack="true" runat="server" />&nbsp;Mai
                                                                Conservato
                                                            </td>
                                                        </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="Panel_StatiDocumento" runat="server" Visible="false">
                                                <table class="info_grigio" id="Table22" cellspacing="0" cellpadding="0" width="100%"
                                                    align="center" border="0" style="margin-top: 2px;">
                                                    <tr>
                                                        <tr>
                                                            <td>
                                                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                                <asp:Label ID="lbl_statiDoc" runat="server" CssClass="titolo_scheda" Width="19%">Stato fasc.</asp:Label>&nbsp;
                                                                <asp:DropDownList ID="ddl_statiDoc" runat="server" CssClass="testo_grigio" AutoPostBack="false"
                                                                    Width="254px">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                </table>
                                            </asp:Panel>
                                            <!-- Ordinamento -->
                                            <table class="info_grigio" cellspacing="0" cellpadding="0" width="100%" align="center" border="0" style="margin-top: 2px;">
										    <tr>
											    <td class="titolo_scheda" valign="middle" height="19" width="35%" style="padding-bottom: 5px; padding-top: 5px">
                                                    &nbsp;Ordinamento
                                                </td>
                                            </tr>
                                            <tr>
										        <td style="padding-bottom: 5px; padding-top: 5px" class="style1">
                                                    &nbsp;<asp:DropDownList ID="ddlOrder" runat="server" CssClass="testo_grigio" Width="270" />
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlOrderDirection" runat="server" CssClass="testo_grigio">
                                                        <asp:ListItem Text="Crescente" Value="ASC" />
                                                        <asp:ListItem Text="Decrescente" Value="DESC" Selected="True" />
                                                    </asp:DropDownList>
                                                </td>
									        </tr>
                                        </table>
                                        <tr>
                                            <td style="height: 3px" />
                                        </tr>
                                        <!--Visiblità Documenti-->
                                        <asp:Panel ID="pnl_visiblitaFasc" runat="server" Visible="false">
                                        <tr>
                                            <td class="titolo_scheda" style="padding-left:5px">
                                                <table width="100%">
                                                    <tr>
                                                        <td class="titolo_scheda">
                                                            <asp:Label ID="lbl_visibilita">Visibilità :</asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:RadioButtonList id="rbl_visibilita" runat="server" RepeatDirection="Horizontal" CssClass="testo_grigio" Width="80%">
                                                                <asp:ListItem Text="Tipica e Atipica" Value="T_A" Selected="True"/>
                                                                <asp:ListItem Text="Tipica" Value="T" />
                                                                <asp:ListItem Text="Atipica" Value="A" />                                                                                        
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 3px" />
                                        </tr>                                        
                                        </asp:Panel>        
                                </table>
                            </td>
                        </tr>
                          <tr>
                            <td valign="top">
                                <table>
                                <tr>
                                    <td class="testo_grigio" style="width: 223px">
                                        &nbsp;&nbsp;<asp:Label ID="lbl_mostraTuttiFascicoli" runat="server" CssClass="testo_grigio_scuro">Mostra tutti i fascicoli</asp:Label>
                                    </td>
                                    <td class="testo_grigio" valign="top">
                                        <asp:RadioButtonList ID="rbl_MostraTutti" runat="server" CssClass="testo_grigio"
                                            AutoPostBack="True" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0">
                                            <asp:ListItem Value="S">SI&nbsp;&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
                                        </asp:RadioButtonList>
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
                    <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" align="center" border="0"
                        style="margin-top: 5px;">
                        <tr>
                            <td>
                                <%--                                    <cc1:imagebutton id="btn_ricFascicoli" autopostback="False" Thema="btn_" SkinID="cerca_attivo" runat="server" disabledurl="../App_Themes/ImgComuni/btn_cerca_nonattivo.gif" alternatetext="Cerca fascicolo"></cc1:imagebutton>
                                --%>
                                <asp:Button ID="btn_ricFascicoli" CssClass="pulsante92" runat="server" Text="Cerca"
                                    ToolTip="Cerca fascicolo" />
                            </td>
                            <td>
                                <asp:Button ID="btn_salva" runat="server" Enabled="True" CssClass="pulsante92" Text="Salva"
                                    ToolTip="Salva ricerca" />
                            </td>
                            <td>
                                <asp:Button ID="btn_modifica" CssClass="pulsante79" runat="server" Text="Modifica"
                                    ToolTip="Modifica la ricerca salvata" OnClick="ModifyRapidSearch_Click" />
                            </td>
                            <td>
                                <%-- <cc1:imagebutton id="btn_new" autopostback="False" Thema="btn_" SkinID="nuovo_attivo" runat="server" disabledurl="../App_Themes/ImgComuni/btn_nuovo_nonattivo.gif" alternatetext="Nuovo fascicolo" enabled="False"></cc1:imagebutton> --%>
                                <asp:Button ID="btn_new" CssClass="pulsante92" Text="Nuovo" Enabled="false" ToolTip="Nuovo fascicolo"
                                    runat="server" />
                            </td>
                        </tr>
                    </table>
                    <table id="Table6" cellspacing="0" cellpadding="0" align="center" border="0" style="margin-top: 5px;">
                        <tr>
                            <td>
                                <asp:Button ID="btn_new_tit" runat="server" Enabled="False" CssClass="pulsante92"
                                    Text="Nodo Tit" ToolTip="Nuovo nodo di titolario" />
                            </td>
                            <td>
                                <cc1:ImageButton ID="btn_del" Thema="btn_" SkinID="rimuovi_attivo" runat="server"
                                    Visible="False" Tipologia="FASC_CANCELLA" DisabledUrl="../images/bottoniera/btn_rimuovi_nonattivo.gif"
                                    AlternateText="Cancella fascicolo" Enabled="False"></cc1:ImageButton>
                            </td>
                        </tr>
                    </table>
                    <!--FINE	BOTTONIERA -->
                </td>
            </tr>
        </table>
        </td> </tr> </table>
        <cc2:MessageBox ID="mb_ConfirmDelete" Style="z-index: 101; left: 576px; position: absolute;
            top: 24px" runat="server"></cc2:MessageBox>
    </div>
    <input id="txtSystemIdUtenteCreatore" type="hidden" runat="server" />
    <input id="txtTipoCorrispondente" type="hidden" runat="server" />
    <script language="javascript">
        esecuzioneScriptUtente();
    </script>
    </form>
</body>
</html>
