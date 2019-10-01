<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>

<%@ Page Language="c#" CodeBehind="Home.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Homepage.Home" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="JavaScript">

        var cambiapass;
        var hlp;

        function Init() {

        }
        function apriPopup() {
            hlp = window.open('../help.aspx?from=HP', '', 'width=450,height=500,scrollbars=YES');
        }
        function cambiaPwd() {
            cambiapass = window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
        }
        function ClosePopUp() {
            if (typeof (cambiapass) != 'undefined') {
                if (!cambiapass.closed)
                    cambiapass.close();
            }
            if (typeof (hlp) != 'undefined') {
                if (!hlp.closed)
                    hlp.close();
            }
        }

    </script>
    <script language="javascript" id="btn_test_Click" event="onclick()" for="btn_test">
			window.document.body.style.cursor='wait';
    </script>
    <script language="javascript" id="btn_elimina_click" event="onclick()" for="btn_elimina">
			window.document.body.style.cursor='wait';
    </script>
    <script language="javascript" id="btn_salva_click" event="onclick()" for="btn_salva">
			window.document.body.style.cursor='wait';
    </script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" onunload="Init();ClosePopUp()">
    <form id="Form1" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > HomePage" />
    <input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server">
    <!-- Gestione del menu a tendina -->
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
            <!-- STRISCIA DEL TITOLO DELLA PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Amministrazioni
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                <table cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td align="center" colspan="2" height="25">
                            <asp:Label ID="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" width="650">
                            <table width="100%" border="0">
                                <tr>
                                    <td class="pulsanti" height="30">
                                        <asp:Panel ID="pnl_ddlAmm" runat="server" Visible="True">
                                            <table cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td class="testo_grigio_scuro" id="lbl_ammPresenti" align="left" width="60%" runat="server">
                                                        Amministrazioni:&nbsp;&nbsp;
                                                        <asp:DropDownList ID="ddl_amministrazioni" runat="server" CssClass="testo" OnSelectedIndexChanged="ddl_amministrazioni_SelectedIndexChanged"
                                                            AutoPostBack="True" Width="270">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="right" width="40%">
                                                        <asp:Button ID="btn_nuova" runat="server" CssClass="testo_btn" Width="150" Text="Nuova Amministrazione">
                                                        </asp:Button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Panel ID="pnl_info" runat="server" Visible="True">
                                            <table class="contenitore" cellspacing="4" cellpadding="0" width="100%">
                                                <tr>
                                                    <td colspan="2">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="titolo_pnl" align="left">
                                                                    <asp:Label ID="lbl_titolo_pnl" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        Codice *
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbl_cod" CssClass="testo" runat="server"></asp:Label>
                                                        <asp:TextBox ID="txt_codice" runat="server" CssClass="testo" Width="200px" MaxLength="16"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        Descrizione *
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_descrizione" runat="server" CssClass="testo" Width="420px" MaxLength="500"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right" colspan="2">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        <asp:Label ID="lbl_etichette" CssClass="testo" runat="server">Etichette</asp:Label>
                                                    </td>
                                                    <td class="testo_grigio_scuro" align="left">
                                                        <table>
                                                            <tr>
                                                                <td width="393px;">
                                                                </td>
                                                                <td>
                                                                    <asp:ImageButton ID="btn_etichette" runat="server" ToolTip="Seleziona Opzioni" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                                        ImageAlign="AbsBottom"></asp:ImageButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <asp:Panel ID="pnlFascObbligatoria" runat="server" Visible="false">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" align="right">
                                                            <asp:Label ID="lbl_FascObbligatoria" CssClass="testo" runat="server">Classificazione obbligatoria</asp:Label>
                                                        </td>
                                                        <td class="testo_grigio_scuro" align="left">
                                                            <table>
                                                                <tr>
                                                                    <td width="393px;">
                                                                    </td>
                                                                    <td>
                                                                        <asp:ImageButton ID="btn_fascObbligatoria" runat="server" ToolTip="Seleziona Opzioni" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                                            ImageAlign="AbsBottom"></asp:ImageButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </asp:Panel>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        <asp:Label ID="lbl_segnatura" CssClass="testo" runat="server">Segnatura *</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_segnatura" runat="server" CssClass="testo" Width="392px" MaxLength="255"
                                                            OnTextChanged="txt_segnatura_TextChanged"></asp:TextBox>&nbsp;
                                                        <asp:ImageButton ID="btn_segn" runat="server" ToolTip="Seleziona Opzioni" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                            ImageAlign="AbsBottom"></asp:ImageButton>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        <asp:Label ID="lbl_timbro_su_pdf" CssClass="testo" runat="server">Timbro su pdf *</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_timbro_su_pdf" runat="server" CssClass="testo" Width="392px"
                                                            MaxLength="255" OnTextChanged="txt_timbro_su_pdf_TextChanged"></asp:TextBox>&nbsp;
                                                        <asp:ImageButton ID="btn_timbro_pdf" runat="server" ToolTip="Seleziona Opzioni" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                            ImageAlign="AbsBottom" OnClick="btn_timbro_pdf_Click"></asp:ImageButton>
                                                    </td>
                                                </tr>
                                                <%--Dettaglio firma--%> 
                                                <asp:Panel ID="pnlDettFirma" runat="server" >
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        <asp:Label ID="lbl_dettFirma" runat="server">Didascalia Firma:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_dettFirma" runat="server" CssClass="testo" Width="392px" MaxLength="255"></asp:TextBox>
                                                    </td>                                        
                                                </tr>
                                                </asp:Panel>
                                               
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        <asp:Label ID="lbl_fascicolatura" CssClass="testo" runat="server">Fascicolatura *</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_fascicolatura" runat="server" CssClass="testo" Width="392px"
                                                            MaxLength="255" OnTextChanged="txt_fascicolatura_TextChanged"></asp:TextBox>&nbsp;
                                                        <asp:ImageButton ID="btn_fasc" runat="server" ToolTip="Seleziona Opzioni" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                            ImageAlign="AbsBottom"></asp:ImageButton>
                                                    </td>
                                                </tr>
                                                <asp:Panel ID="pnl_protocolloTit_Riscontro" runat="server" Visible="false">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" align="right">
                                                            <asp:Label ID="lbl_protocolloTit_Riscontro" CssClass="testo" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txt_protocolloTit_Riscontro" runat="server" CssClass="testo" Width="392px"
                                                                MaxLength="255" OnTextChanged="txt_protocolloTit_TextChanged"></asp:TextBox>&nbsp;
                                                            <asp:ImageButton ID="btn_protocolloTit_Riscontro" runat="server" ToolTip="Seleziona Opzioni"
                                                                ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif" ImageAlign="AbsBottom"
                                                                OnClick="btn_protocolloTit_Riscontro_Click"></asp:ImageButton>
                                                        </td>
                                                    </tr>
                                                </asp:Panel>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        <asp:Label ID="lblClientModelProcessor" runat="server">Word processor:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="cboClientModelProcessor" runat="server" CssClass="testo" Width="323px">
                                                            <asp:ListItem Value="0">Seleziona...</asp:ListItem>
                                                            <asp:ListItem Value="0">----------------------------------</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right" colspan="2">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        Server SMTP
                                                    </td>
                                                    <td class="testo_grigio_scuro">
                                                        <asp:TextBox ID="txt_smtp" runat="server" CssClass="testo" Width="323px" MaxLength="128"></asp:TextBox>&nbsp;&nbsp;&nbsp;Porta&nbsp;
                                                        <asp:TextBox ID="txt_portasmtp" runat="server" CssClass="testo" Width="40px" MaxLength="10"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        UserID SMTP
                                                    </td>
                                                    <td class="testo_grigio_scuro">
                                                        <asp:TextBox ID="txt_userid_smtp" runat="server" CssClass="testo" Width="100px" MaxLength="128"></asp:TextBox>&nbsp;&nbsp;Password
                                                        SMTP&nbsp;
                                                        <asp:TextBox ID="txt_pwd_smtp" runat="server" CssClass="testo" Width="100px" MaxLength="128"
                                                            TextMode="Password"></asp:TextBox>&nbsp;&nbsp;Conferma
                                                        <asp:TextBox ID="txt_conferma_pwd_smtp" runat="server" CssClass="testo" Width="100px"
                                                            MaxLength="128" TextMode="Password"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right">
                                                        SSL su SMTP
                                                    </td>
                                                    <td class="testo_grigio_scuro">
                                                        <asp:CheckBox ID="chkSSLSMTP" ToolTip="Abilita uso di connessione SSL su protocollo SMTP"
                                                            runat="server" CssClass="testo" Width="3px"></asp:CheckBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" colspan="2">
                                                        Email notifica: 'From' :
                                                        <asp:TextBox ID="txt_from_mail" runat="server" CssClass="testo" Width="200px" MaxLength="128"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="left">
                                                                    <asp:CheckBox ID="chkSpedizioneAutomaticaDocumento" runat="server" Text="Spedizione automatica" />
                                                                </td>
                                                                <td class="testo_grigio_scuro" align="left">
                                                                    <asp:CheckBox ID="chkTrasmissioneAutomaticaDocumento" runat="server" Text="Trasmissione automatica" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="left">
                                                                    <asp:CheckBox ID="chkAvvisaSuSpedizioneDocumento" runat="server" Text="Avvisa su spedizione senza documento elettronico" />
                                                                </td>
                                                                <td class="testo_grigio_scuro" align="left">
                                                                    <asp:CheckBox ID="chkTipologiaObblig" runat="server" Text="Tipologia documento obbligatoria" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right" colspan="2">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                            <tr>
                                                                <td class="titolo_pnl" align="center" colspan="3">
                                                                    Ragione per la trasmissione automatica:
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro">
                                                                    Ai destinatari (TO) *
                                                                </td>
                                                                <td width="10">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="testo_grigio_scuro">
                                                                    Ai destinatari in conoscenza (CC) *
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro">
                                                                    <asp:DropDownList ID="ddl_ragione_to" runat="server" CssClass="testo" Width="230">
                                                                        <asp:ListItem Value="0">Seleziona...</asp:ListItem>
                                                                        <asp:ListItem Value="0">----------------------------------</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td width="10">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="testo_grigio_scuro">
                                                                    <asp:DropDownList ID="ddl_ragione_cc" runat="server" CssClass="testo" Width="230">
                                                                        <asp:ListItem Value="0">Seleziona...</asp:ListItem>
                                                                        <asp:ListItem Value="0">----------------------------------</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center"><br />
                                                        <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                            <tr>
                                                                <td class="titolo_pnl" align="center" colspan="3">
                                                                    Ragione per la gestione del banner:
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro">
                                                                    Testo del messaggio banner
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td width="100%">
                                                                    <asp:TextBox ID="txt_banner" Name="txt_banner" CssClass="testo_grigio_scuro" runat="server"
                                                                                Width="100%" TextMode="MultiLine" Rows="4" MaxLength="4000"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table><br />
                                                    </td>
                                                </tr>
                                                <!-- sezione ragioni per smistamento -->
                                                <asp:Panel ID="pnlRagioniSmista" runat="server">
                                                    <tr>
                                                        <td colspan="2" align="center">
                                                            <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                                <tr>
                                                                    <td class="titolo_pnl" align="center" colspan="3">
                                                                        Ragione per lo smistamento:
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="testo_grigio_scuro">
                                                                        Ai destinatari in Competenza
                                                                    </td>
                                                                    <td width="10">
                                                                        &nbsp;
                                                                    </td>
                                                                    <td class="testo_grigio_scuro">
                                                                        Ai destinatari in Conoscenza
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="testo_grigio_scuro">
                                                                        <asp:DropDownList ID="ddl_ragione_COMPETENZA" runat="server" CssClass="testo" Width="230">
                                                                            <asp:ListItem Value="0">Seleziona...</asp:ListItem>
                                                                            <asp:ListItem Value="0">----------------------------------</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td width="10">
                                                                        &nbsp;
                                                                    </td>
                                                                    <td class="testo_grigio_scuro">
                                                                        <asp:DropDownList ID="ddl_ragione_CONOSCENZA" runat="server" CssClass="testo" Width="230">
                                                                            <asp:ListItem Value="0">Seleziona...</asp:ListItem>
                                                                            <asp:ListItem Value="0">----------------------------------</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <div style="margin-top: 5px;">
                                                                <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                                    <tr>
                                                                        <td class="testo_grigio_scuro">
                                                                            <asp:CheckBox ID="chk_avviso_todolist" runat="server"></asp:CheckBox>
                                                                            Avvisa gli utenti se la lista delle 'Cose da fare' contiene trasmissioni più vecchie
                                                                            di
                                                                            <asp:TextBox ID="txt_gg_perm_todolist" CssClass="testo_grigio_scuro" runat="server"
                                                                                Width="30px" MaxLength="3"></asp:TextBox>&nbsp;giorni
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </asp:Panel>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right" colspan="2">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                            <tr>
                                                                <td colspan="2" class="titolo_pnl" align="center" colspan="3">
                                                                    Configurazioni componenti:
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="left">
                                                                    <asp:RadioButton ID="rdbDisableAll" Text="ActiveX" runat="server" 
                                                                        GroupName="ClientType"  AutoPostBack="true" CssClass="testo_grigio_scuro"  
                                                                        OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" Checked="true" /><br />
                                                                    <asp:RadioButton ID="rdbIsEnabledSmartClient" Text="SmartClient" runat="server" 
                                                                        GroupName="ClientType" AutoPostBack="true" CssClass="testo_grigio_scuro" 
                                                                        OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" 
                                                                        ToolTip="Abilita l'utente all'utilizzo dei componenti SmartClient dalla propria postazione di lavoro" /><br />
                                                                    <asp:RadioButton ID="rdbIsEnabledJavaApplet" Text="JavaApplet" runat="server" 
                                                                        GroupName="ClientType"  AutoPostBack="true" CssClass="testo_grigio_scuro"  
                                                                        OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" /><br />
                                                                    <asp:RadioButton ID="rdbIsEnabledHTML5Socket" Text="HTML5Socket" runat="server" 
                                                                        GroupName="ClientType" AutoPostBack="true" CssClass="testo_grigio_scuro" 
                                                                        OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" />
                                                                    </td>
                                                                   <td class="testo_grigio_scuro" align="center">
                                                                    <asp:CheckBox ID="chkSmartClientConvertPdfOnScan" runat="server" CssClass="testo_grigio_scuro"
                                                                        TextAlign="Left" Text="Acquisisci da scanner in formato PDF" ToolTip="Acquisisce i documenti da scanner direttamente in formato PDF" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" style="width: 50%;">
                                                                    <asp:Label ID="Label6" runat="server">Tipo stampante etichette</asp:Label>
                                                                </td>
                                                                <td style="width: 50%;">
                                                                    <asp:DropDownList CssClass="testo" runat="server" ID="ddlLabelPrinterType">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="testo_grigio_scuro" align="right" colspan="2">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <table border="0" cellpadding="0" cellspacing="0" width="90%" align="center">
                                                            <tr>
                                                                <td class="titolo_pnl" align="center" colspan="2">
                                                                    Autenticazione di dominio:
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="5">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="testo_blu">
                                                                    Nota: prima di valorizzare il campo dominio, consultare l'Help
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right">
                                                                    Formato autent.&nbsp;&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txt_autenticazione" runat="server" CssClass="testo" Width="392px"
                                                                        MaxLength="255"></asp:TextBox>&nbsp;
                                                                    <asp:ImageButton ID="btn_aut_dominio" runat="server" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                                        ToolTip="Seleziona opzioni" ImageAlign="AbsBottom" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right">
                                                                    Dominio&nbsp;&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txt_dominio" runat="server" CssClass="testo" Width="392px" MaxLength="255"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="2">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td align="left" width="55%">
                                                                    <asp:Label ID="lbl_msg" runat="server" CssClass="testo_rosso"></asp:Label>
                                                                </td>
                                                                <td align="right">
                                                                    <asp:Button ID="btn_annulla" runat="server" CssClass="testo_btn" Visible="False"
                                                                        Width="60" Text="Annulla"></asp:Button>
                                                                    <asp:Button ID="btn_elimina" runat="server" CssClass="testo_btn" Visible="False"
                                                                        Width="60" Text="Elimina"></asp:Button>&#160;&#160;<asp:Button ID="btn_test" runat="server"
                                                                            CssClass="testo_btn" Text="Test Connessione" Visible="False" Width="115px">
                                                                    </asp:Button>&#160;
                                                                    <asp:Button ID="btn_salva" runat="server" CssClass="testo_btn" Width="60" Text="Modifica">
                                                                    </asp:Button>&nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td width="10">
                            &nbsp;
                        </td>
                        <td valign="top" width="220">
                            <asp:Panel ID="pnl_fasc_segn" runat="server" Visible="False">
                                <table class="info" width="100%">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl_fascSegn" runat="server" CssClass="titolo_piccolo"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="btn_chiudi" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif">
                                            </asp:ImageButton>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:DataGrid ID="dgFascSegn" runat="server" Visible="True" Width="100%" AutoGenerateColumns="False">
                                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                <ItemStyle Font-Size="XX-Small" Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2">
                                                </ItemStyle>
                                                <HeaderStyle CssClass="titolo_piccolo" BackColor="#E0E0E0"></HeaderStyle>
                                                <Columns>
                                                    <asp:ButtonColumn Text="&lt;img src=../Images/aggiungi.gif border=0 alt='Aggiungi'&gt;"
                                                        CommandName="Select">
                                                        <ItemStyle BackColor="#E0E0E0"></ItemStyle>
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="codice" HeaderText="Codice"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="descrizione" HeaderText="Descrizione"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid><br>
                                            <asp:DataGrid ID="dg_Separ" runat="server" Visible="True" Width="100%" AutoGenerateColumns="False">
                                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                <ItemStyle Font-Size="XX-Small" Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2">
                                                </ItemStyle>
                                                <HeaderStyle CssClass="titolo_piccolo" BackColor="#E0E0E0"></HeaderStyle>
                                                <Columns>
                                                    <asp:ButtonColumn Text="&lt;img src=../Images/aggiungi.gif border=0 alt='Aggiungi'&gt;"
                                                        CommandName="Select">
                                                        <ItemStyle BackColor="#E0E0E0"></ItemStyle>
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="codice" HeaderText="Separatore">
                                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="descrizione" HeaderText="Descrizione"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                            <br>
                                            <asp:Panel ID="pnl_timbro" runat="server" Visible="False">
                                                <table id="caratteristiche" width="100%" cellspacing="1" cellpadding="0" border="0">
                                                    <!--style="border-collapse: separate" border="1px">-->
                                                    <tr>
                                                        <td style="width: 40%">
                                                            <asp:Label ID="Carattere" runat="server" Text="Carattere" Font-Size="XX-Small" Font-Names="Verdana"
                                                                ForeColor="#666666" BackColor="#F2F2F2" Width="100%"></asp:Label>
                                                        </td>
                                                        <td style="width: 60%">
                                                            <asp:DropDownList ID="CarattereList" runat="server" Visible="true" Font-Size="XX-Small"
                                                                Font-Names="Verdana" ForeColor="#666666" Width="100%">
                                                                <asp:ListItem></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 40%">
                                                            <asp:Label ID="Colore" runat="server" Text="Colore" Font-Size="XX-Small" Font-Names="Verdana"
                                                                ForeColor="#666666" BackColor="#F2F2F2" Width="100%"></asp:Label>
                                                        </td>
                                                        <td style="width: 60%">
                                                            <asp:DropDownList ID="ColoreList" runat="server" Visible="true" Font-Size="XX-Small"
                                                                Font-Names="Verdana" ForeColor="#666666" Width="100%">
                                                                <asp:ListItem></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 40%">
                                                            <asp:Label ID="Posizione" runat="server" Text="Posizione" Font-Size="XX-Small" Font-Names="Verdana"
                                                                ForeColor="#666666" BackColor="#F2F2F2" Width="100%"></asp:Label>
                                                        </td>
                                                        <td style="width: 60%">
                                                            <asp:DropDownList ID="PosizioneList" runat="server" Visible="true" Font-Size="XX-Small"
                                                                Font-Names="Verdana" ForeColor="#666666" Width="100%">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem>upSx</asp:ListItem>
                                                                <asp:ListItem>upDx</asp:ListItem>
                                                                <asp:ListItem>downSx</asp:ListItem>
                                                                <asp:ListItem>downDx</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 40%">
                                                            <asp:Label ID="Rotazione" runat="server" Text="Rotazione" Font-Size="XX-Small" Font-Names="Verdana"
                                                                ForeColor="#666666" BackColor="#F2F2F2" Width="100%"></asp:Label>
                                                        </td>
                                                        <td style="width: 60%">
                                                            <asp:DropDownList ID="RotazioneList" runat="server" Visible="true" Font-Size="XX-Small"
                                                                Font-Names="Verdana" ForeColor="#666666" Width="100%" Enabled="False">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Selected="True">0</asp:ListItem>
                                                                <asp:ListItem>90</asp:ListItem>
                                                                <asp:ListItem>180</asp:ListItem>
                                                                <asp:ListItem>270</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 40%">
                                                            <asp:Label ID="Orientamento" runat="server" Text="Tipo Stampa" Font-Size="XX-Small"
                                                                Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2" Width="100%"></asp:Label>
                                                        </td>
                                                        <td style="width: 60%">
                                                            <asp:DropDownList ID="OrientamentoList" runat="server" Visible="true" Font-Size="XX-Small"
                                                                Font-Names="Verdana" ForeColor="#666666" Width="100%">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Value="false">Segn_orizzontale</asp:ListItem>
                                                                <asp:ListItem Value="orizzontale">Timbro_orizzontale</asp:ListItem>
                                                                <asp:ListItem Value="verticale">Timbro_verticale</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnl_autenticazione" runat="server" Visible="False">
                                <table class="info" width="100%">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl_tit_aut" runat="server" CssClass="titolo_piccolo"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="btn_chiudi_aut" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif">
                                            </asp:ImageButton>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:DataGrid ID="dg_aut" runat="server" Visible="True" Width="100%" AutoGenerateColumns="False">
                                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                <ItemStyle Font-Size="XX-Small" Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2">
                                                </ItemStyle>
                                                <HeaderStyle CssClass="titolo_piccolo" BackColor="#E0E0E0"></HeaderStyle>
                                                <Columns>
                                                    <asp:ButtonColumn Text="&lt;img src=../Images/aggiungi.gif border=0 alt='Aggiungi'&gt;"
                                                        CommandName="Select">
                                                        <ItemStyle BackColor="#E0E0E0"></ItemStyle>
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="codice" HeaderText="Codice"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                            <br>
                                            <asp:DataGrid ID="dg_sep" runat="server" Visible="True" Width="100%" AutoGenerateColumns="False">
                                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                <ItemStyle Font-Size="XX-Small" Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2">
                                                </ItemStyle>
                                                <HeaderStyle CssClass="titolo_piccolo" BackColor="#E0E0E0"></HeaderStyle>
                                                <Columns>
                                                    <asp:ButtonColumn Text="&lt;img src=../Images/aggiungi.gif border=0 alt='Aggiungi'&gt;"
                                                        CommandName="Select">
                                                        <ItemStyle BackColor="#E0E0E0"></ItemStyle>
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="codice" HeaderText="Separatore"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                            <br>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo">
                                            es: nome.cognome@dominio
                                            <br />
                                            es: dominio\userid
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnl_etichette" runat="server" Visible="False">
                                <table class="info" width="100%">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl_titolo_etichette" runat="server" CssClass="titolo_piccolo"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="btn_chiudi_etichette" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif">
                                            </asp:ImageButton>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <table width="100%" cellspacing="0" border="1" style="width: 100%; border-collapse: collapse;">
                                                <tr>
                                                    <th class="titolo_piccolo" style="background-color: #E0E0E0; text-align: left; font-weight: normal;
                                                        text-align: center;">
                                                        Tipo
                                                    </th>
                                                    <th class="titolo_piccolo" style="background-color: #E0E0E0; text-align: left; font-weight: normal;
                                                        text-align: center;">
                                                        Descrizione
                                                    </th>
                                                    <th class="titolo_piccolo" style="background-color: #E0E0E0; text-align: left; font-weight: normal;
                                                        text-align: center;">
                                                        Etichetta
                                                    </th>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="Label1" runat="server" Text="A"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:TextBox ID="Descrizione1" runat="server" CssClass="testo_grigio_scuro" Width="100px"
                                                            MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox ID="Text1" runat="server" CssClass="testo_grigio_scuro" Width="50px"
                                                            MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="Label2" runat="server" Text="P"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:TextBox ID="Descrizione2" runat="server" CssClass="testo_grigio_scuro" Width="100px"
                                                            MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td align="center" width="60px;">
                                                        <asp:TextBox ID="Text2" runat="server" CssClass="testo_grigio_scuro" Width="50px"
                                                            MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="Label3" runat="server" Text="I"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:TextBox ID="Descrizione3" runat="server" CssClass="testo_grigio_scuro" Width="100px"
                                                            MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox ID="Text3" runat="server" CssClass="testo_grigio_scuro" Width="50px"
                                                            MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="Label4" runat="server" Text="G"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:TextBox ID="Descrizione4" runat="server" CssClass="testo_grigio_scuro" Width="100px"
                                                            MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox ID="Text4" runat="server" CssClass="testo_grigio_scuro" Width="50px"
                                                            MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="Label5" runat="server" Text="ALL"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:TextBox ID="Descrizione5" runat="server" CssClass="testo_grigio_scuro" Width="100px"
                                                            MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox ID="Text5" runat="server" CssClass="testo_grigio_scuro" Width="50px"
                                                            MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlFascObbligatoriaTipiDoc" runat="server" Visible="False">
                                <table class="info" width="100%">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl_fascObbligatoriaTipiDoc" runat="server" CssClass="titolo_piccolo">Classificazione obbligatoria tipi doc.</asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="btn_chiudiFascObbligatoria" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif">
                                            </asp:ImageButton>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <table width="100%" cellspacing="0" border="1" style="width: 100%; border-collapse: collapse;">
                                                <tr>
                                                    <th class="titolo_piccolo" style="background-color: #E0E0E0; text-align: left; font-weight: normal;
                                                        text-align: center;">
                                                        Tipo
                                                    </th>
                                                    <th class="titolo_piccolo" style="background-color: #E0E0E0; text-align: left; font-weight: normal;
                                                        text-align: center;">
                                                        Class. obbligatoria
                                                    </th>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="lblTipoDoc1" runat="server" Text="Arrivo"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:CheckBox ID="cbxTipoDoc1" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="lblTipoDoc2" runat="server" Text="Partenza"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:CheckBox ID="cbxTipoDoc2" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="lblTipoDoc3" runat="server" Text="Interni"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:CheckBox ID="cbxTipoDoc3" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="lblTipoDoc4" runat="server" Text="Non protocollati"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:CheckBox ID="cbxTipoDoc4" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="lblTipoDoc5" runat="server" Text="Stampa dei registri di repertorio"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:CheckBox ID="cbxTipoDoc5" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr style="color: #666666; background-color: #F2F2F2; font-family: Verdana; font-size: XX-Small;">
                                                    <td align="center">
                                                        <asp:Label ID="lblTipoDoc6" runat="server" Text="Stampa dei registri di protocollo"></asp:Label>
                                                    </td>
                                                    <td align="center" width="105px;">
                                                        <asp:CheckBox ID="cbxTipoDoc6" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <br>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
