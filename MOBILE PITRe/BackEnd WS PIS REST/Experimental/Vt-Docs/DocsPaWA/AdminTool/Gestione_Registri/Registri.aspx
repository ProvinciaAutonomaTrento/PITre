<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>

<%@ Page Language="c#" CodeBehind="Registri.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Registri.Registri" %>

<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Src="../UserControl/InteroperabilitySettings.ascx" TagName="InteroperabilitySettings"
    TagPrefix="uc3" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
    <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
    <script src="../../LIBRERIE/date.js" language="javascript" type="text/javascript"></script>
    <script language="JavaScript">

        var cambiapass;
        var hlp;

        function apriPopup() {
            window.open('../help.aspx?from=RG', '', 'width=450,height=500,scrollbars=YES');
        }
        function cambiaPwd() {
            window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
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
        function apriUOSmistamento(idRegistro) {
            var myUrl = "UOSmistamento.aspx?idRegistro=" + idRegistro;
            window.showModalDialog(myUrl, "", "dialogWidth:620px;dialogHeight:670px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }

        function _ApriRubricaRuolo() {
            var r = new Rubrica();
            r.CallType = r.CALLTYPE_RUOLO_REG_NOMAIL;
            var res = r.Apri();
        }

        function _ApriRubricaRuoloResp() {
            var r = new Rubrica();
            r.CallType = r.CALLTYPE_RUOLO_RESP_REG;
            var res = r.Apri();
        }

        function _ApriRubricaUtente() {
            var r = new Rubrica();
            r.CallType = r.CALLTYPE_UTENTE_REG_NOMAIL;
            var res = r.Apri();
        }

    </script>
    <script language="javascript" id="btn_test_Click" event="onclick()" for="btn_test">
			window.document.body.style.cursor='wait';
    </script>
    <script language="JavaScript">

        function ShowValidationMessage(message, warning) {
            if (warning) {
                if (window.confirm(message + "\n\nContinuare?")) {
                    Form1.txtCommandPending.value = 'DELETE';
                    Form1.submit();
                }
                else {
                    Form1.txtCommandPending.value = '';
                }
            }
            else {
                alert(message);
            }
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
					
    </script>
    <script type="text/javascript" language="javascript">
        function OpenAddressBook() {

            var r = new Rubrica();

            // Impostazione del tipo di corrispondenti da ricercare (Solo interni)
            r.CorrType = r.Interni;

            // Impostazione del calltype
            r.CallType = r.CALLTYPE_RUOLO_RESP_REPERTORI;

            r.MoreParams = "ajaxPage";

            // Apertura della rubrica
            var res = r.Apri();
        }
    </script>
    <style type="text/css">
        .style1
        {
            font-weight: bold;
            font-size: 12px;
            color: #4b4b4b;
            font-family: Verdana;
            width: 136%;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 20%;
        }
        .style3
        {
            width: 295px;
        }
    </style>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" onunload="ClosePopUp()">
    <form id="Form1" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Registri" />
    <input id="txtCommandPending" type="hidden" runat="server">
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
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" width="100%" bgcolor="#e0e0e0" style="height: 20px">
                Registri
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <asp:UpdatePanel ID="UpdatePanelRegistri" runat="server">
                    <ContentTemplate>
                        <asp:UpdateProgress ID="updProgressCasella" runat="server">
                            <ProgressTemplate>
                                <div style="width: 80%; height: 80%; color: black; position: absolute; top: 38%;
                                    left: 10%; right: 10%; bottom: 0%; vertical-align: middle; padding-top: 25%;">
                                    <asp:Label ID="lbl_wait" runat="server" Font-Bold="true" Font-Size="Medium">Attendere...</asp:Label>
                                    <asp:Image runat="server" ID="img_load_caselle" ImageUrl="~/images/loading.gif" ImageAlign="Middle" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <table cellspacing="0" cellpadding="0" align="center" border="0" width="820px">
                            <tr>
                                <td align="center" height="25">
                                </td>
                            </tr>
                            <tr>
                                <td class="pulsanti" align="center">
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="titolo" align="left">
                                                <asp:Label ID="lbl_tit" CssClass="titolo" runat="server">Lista Registri</asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btn_nuova" runat="server" CssClass="testo_btn" Text="Nuovo Registro">
                                                </asp:Button>&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td height="5">
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <div id="DivDGList" style="overflow: auto; width: 100%; height: 133px">
                                        <asp:DataGrid ID="dg_Registri" runat="server" Width="100%" BorderWidth="1px" CellPadding="1"
                                            BorderColor="Gray" AutoGenerateColumns="False">
                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID">
                                                    <HeaderStyle Width="0%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Codice" HeaderText="Codice">
                                                    <HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
                                                    <HeaderStyle HorizontalAlign="Center" Width="50px"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Email" HeaderText="Email">
                                                    <HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Disabilitato" HeaderText="Disabilitato" Visible="false">
                                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;"
                                                    CommandName="Select">
                                                    <HeaderStyle Width="5%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:ButtonColumn>
                                                <asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;"
                                                    CommandName="Delete">
                                                    <HeaderStyle Width="5%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:ButtonColumn>
                                            </Columns>
                                        </asp:DataGrid></div>
                                </td>
                            </tr>
                            <tr>
                                <td height="10">
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <asp:Panel ID="pnl_info" runat="server" Visible="False">
                                        <table class="contenitore" width="100%" border="0">
                                            <tr>
                                                <td>
                                                    <table cellspacing="0" cellpadding="0" width="100%">
                                                        <tr>
                                                            <td class="titolo_pnl" align="left">
                                                                Dettagli registro
                                                            </td>
                                                            <td class="titolo_pnl" align="right">
                                                                <asp:ImageButton ID="btn_chiudiPnlInfo" runat="server" ImageUrl="../Images/cancella.gif"
                                                                    ToolTip="Chiudi"></asp:ImageButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr valign="top">
                                                <td>
                                                    <table cellspacing="0" cellpadding="0" border="0">
                                                        <tr width="100%">
                                                            <td class="testo_grigio_scuro" align="right" width="100">
                                                                Codice *&nbsp;
                                                            </td>
                                                            <td>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txt_codice" runat="server" CssClass="testo" Width="100px" MaxLength="128"></asp:TextBox>
                                                                            <asp:Label ID="lbl_cod" runat="server" CssClass="testo"></asp:Label>
                                                                        </td>
                                                                        <td class="testo_grigio_scuro" align="right" width="100">
                                                                            Descrizione *&nbsp;
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txt_descrizione" runat="server" CssClass="testo" Width="150px" MaxLength="128"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td align="right">                                                                
                                                                <asp:CheckBox ID="ChkAutomatico" runat="server" CssClass="testo_grigio_scuro" Text="Apertura automatica">
                                                                </asp:CheckBox>
                                                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="ChkSospeso" runat="server" CssClass="testo_grigio_scuro"
                                                                    Text="Sospeso" />
                                                            </td>
                                                        </tr>
                                                        <asp:UpdatePanel runat="server" ID="pnl_pregresso" Visible="false" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <tr>
                                                                    <td colspan="2">
                                                                        <table cellpadding="0" cellspacing="0">
                                                                            <tr>
                                                                                <td style="padding-left: 45px; padding-right: 46px; width: 137px;">
                                                                                    <asp:CheckBox ID="chk_pregresso" Text="Import pregressi" runat="server" CssClass="testo_grigio_scuro" OnCheckedChanged="chkPregresso_Click" AutoPostBack="true" />
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" style="padding-right:10px;">
                                                                                    Anno
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox runat="server" ID="txt_anno_reg_pre" CssClass="testo" ReadOnly="true" MaxLength="4"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        <tr>
                                                            <td width="100" align="right">
                                                                <asp:Label runat="server" CssClass="testo_grigio_scuro" ID="lbl_ruoloRespReg" Text="Ruolo Resp.le"></asp:Label>&nbsp;
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="ruoloRespReg" runat="server" CssClass="testo" Width="260px" BackColor="#E0E0E0"
                                                                    ReadOnly="True"></asp:TextBox>
                                                                &nbsp;<asp:ImageButton ID="btn_RubricaRuoloResp" runat="server" Visible="true" ImageUrl="../../images/proto/rubrica.gif"
                                                                    OnClick="btn_RubricaRuoloResp_Click" ToolTip="Rubrica"></asp:ImageButton>
                                                                &nbsp;<asp:ImageButton ID="img_delRuoloResp" runat="server" ImageUrl="~/AdminTool/Images/cestino.gif"
                                                                    OnClick="img_delRuoloResp_Click" ToolTip="Elimina" />
                                                                <asp:TextBox ID="codRuoloRespReg" runat="server" Visible="false" Text=""></asp:TextBox>
                                                            </td>
                                                            <td align="left">
                                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" ID="lblDirittoResp"
                                                                    Text="Diritto per Acquisizione" CssClass="testo_grigio_scuro"></asp:Label>
                                                                <asp:DropDownList ID="ddl_DirittoResp" runat="server" CssClass="testo" AutoPostBack="false">
                                                                    <asp:ListItem Text="Seleziona..." Value="0"></asp:ListItem>
                                                                    <asp:ListItem Text="Lettura" Value="45"></asp:ListItem>
                                                                    <asp:ListItem Text="Scrittura" Value="63"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <asp:Panel ID="panel_RuoloResp" runat="server" Visible="true">
                                                <tr>
                                                    <td class="titolo_pnl" colspan="2">
                                                        <asp:Label runat="server" ID="lblTitle" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    Ruolo Creatore&nbsp;
                                                                </td>
                                                                <td width="*">
                                                                    <asp:TextBox ID="txt_RuoloInteropNoMail" runat="server" CssClass="testo" Width="260px"
                                                                        MaxLength="128" BackColor="#E0E0E0" ReadOnly="True"></asp:TextBox>
                                                                    &nbsp;<asp:ImageButton ID="brn_RubricaRuolo" runat="server" Visible="true" ImageUrl="../../images/proto/rubrica.gif"
                                                                        OnClick="brn_RubricaRuolo_Click" ToolTip="Rubrica"></asp:ImageButton>
                                                                    &nbsp;<asp:ImageButton ID="img_delRuoloNoMail" runat="server" ImageUrl="~/AdminTool/Images/cestino.gif"
                                                                        OnClick="img_delRuoloNoMail_Click" ToolTip="Elimina" />
                                                                    <asp:TextBox ID="txt_IdHiddenUtenteNomail" runat="server" Visible="false"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </asp:Panel>
                                            <asp:Panel ID="panel_InteropNoMail" runat="server" Visible="false">
                                                <tr>
                                                    <td colspan="2">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100" style="height: 24px">
                                                                    Utente Creatore&nbsp;
                                                                </td>
                                                                <td style="height: 24px">
                                                                    <asp:DropDownList ID="ddl_UtInteropNoMail" runat="server" CssClass="testo" AutoPostBack="true"
                                                                        OnSelectedIndexChanged="ddl_UtInteropNoMail_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txt_IdHiddenRuoloNomail" runat="server" Visible="false"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td align="right" width="100" class="testo_grigio_scuro">
                                                                    Gestione&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_automatico" runat="server" CssClass="testo" AutoPostBack="true"
                                                                        OnSelectedIndexChanged="ddl_automatico_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0">Manuale</asp:ListItem>
                                                                        <asp:ListItem Value="1">Semi-automatica</asp:ListItem>
                                                                        <asp:ListItem Value="2">Automatica</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    &nbsp;<asp:Label runat="server" CssClass="testo" ID="lblDescAutoInterop"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </asp:Panel>
                                            <tr>
                                                <td class="titolo_pnl">
                                                    Interoperabilita' attraverso la posta elettronica
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                        <tr>
                                                            <td class="style1" align="left" width="100">
                                                                <asp:UpdatePanel ID="updPanelCasella" runat="server">
                                                                    <ContentTemplate>
                                                                        <asp:Label>Casella di posta</asp:Label>
                                                                        <asp:TextBox ID="txt_indirizzo" runat="server" CssClass="testo" Width="200px" MaxLength="128"></asp:TextBox>
                                                                        &nbsp;&nbsp;&nbsp;
                                                                        <asp:ImageButton AlternateText="Aggiungi casella di posta" ImageAlign="AbsMiddle"
                                                                            ID="img_aggiungiCasella" runat="server" ToolTip="Aggiungi casella di posta" ImageUrl="~/images/Indent.ico" />
                                                                        &nbsp;&nbsp;&nbsp;
                                                                        <asp:Label>Elenco caselle</asp:Label>
                                                                        <asp:DropDownList runat="server" ID="ddl_caselle" CssClass="testo" Width="200" AutoPostBack="true">
                                                                        </asp:DropDownList>
                                                                        <asp:CheckBox ID="cbx_casellaPrincipale" runat="server" CssClass="testo_grigio_scuro"
                                                                            Text="Principale" AutoPostBack="true"></asp:CheckBox>
                                                                        <asp:ImageButton AlternateText="Elimina casella di posta" ImageAlign="AbsMiddle"
                                                                            ID="img_eliminaCasella" runat="server" ToolTip="Elimina casella di posta" ImageUrl="../images/cestino.gif" />
                                                                    </ContentTemplate>
                                                                    <Triggers>
                                                                        <asp:AsyncPostBackTrigger ControlID="dg_Registri" EventName="SelectedIndexChanged" />
                                                                    </Triggers>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td height="2">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:UpdatePanel ID="updPanelSMTP" runat="server">
                                                                    <ContentTemplate>
                                                                        <table class="contenitore" border="0">
                                                                            <tr>
                                                                                <td class="style1" colspan="4">
                                                                                    Posta In Uscita
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right">
                                                                                    Server (SMTP)
                                                                                </td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txt_smtp" runat="server" CssClass="testo" Width="120px" MaxLength="64"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="center" colspan="2">
                                                                                    <table border="0">
                                                                                        <tr width="100%">
                                                                                            <td align="left" class="style2">
                                                                                                Porta&nbsp;<asp:TextBox ID="txt_portasmtp" runat="server" CssClass="testo" Width="35px"
                                                                                                    MaxLength="10"></asp:TextBox>
                                                                                            </td>
                                                                                            <td class="testo_grigio_scuro" align="center" width="33%">
                                                                                                <asp:CheckBox ID="ChkBoxsmtp" runat="server" CssClass="testo" Width="3"></asp:CheckBox>&nbsp;SSL
                                                                                                su SMTP&nbsp;&nbsp;&nbsp;
                                                                                            </td>
                                                                                            <td class="testo_grigio_scuro" align="left" width="34%">
                                                                                                <asp:CheckBox ID="ChkBoxsmtpSTA" runat="server" CssClass="testo" Width="3"></asp:CheckBox>&nbsp;STA
                                                                                                su SMTP&nbsp;&nbsp;&nbsp;
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right">
                                                                                    User Id (SMTP)
                                                                                </td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txt_userid_smtp" runat="server" CssClass="testo" Width="120px" MaxLength="128"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro">
                                                                                    Password&nbsp;<asp:TextBox ID="txt_pwd_smtp" runat="server" CssClass="testo" Width="100px"
                                                                                        MaxLength="128" TextMode="Password"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    Conferma&nbsp;<asp:TextBox ID="txt_conferma_pwd_smtp" runat="server" CssClass="testo"
                                                                                        Width="100px" MaxLength="128" TextMode="Password"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right">
                                                                                    Ricevuta PEC&nbsp;
                                                                                </td>
                                                                                <td class="testo_grigio_scuro">
                                                                                    <asp:DropDownList ID="ddl_ricevutaPec" runat="server" Width="120px" AutoPostBack="true"
                                                                                        CssClass="testo" Height="16px">
                                                                                        <asp:ListItem Value="-">Non Disponibile</asp:ListItem>
                                                                                        <asp:ListItem Value="C">Completa</asp:ListItem>
                                                                                        <asp:ListItem Value="B">Breve</asp:ListItem>
                                                                                        <asp:ListItem Value="S">Sintetica</asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                    <Triggers>
                                                                        <asp:AsyncPostBackTrigger ControlID="ddl_caselle" EventName="SelectedIndexChanged" />
                                                                    </Triggers>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td height="2">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:UpdatePanel ID="UpdatePanel1PostaIngresso" runat="server">
                                                                    <ContentTemplate>
                                                                        <table class="contenitore" border="0">
                                                                            <tr>
                                                                                <td class="style1" colspan="4">
                                                                                    Posta In Ingresso
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right" width="80">
                                                                                    Protocollo&nbsp;
                                                                                </td>
                                                                                <td>
                                                                                    <asp:DropDownList ID="ddl_posta" runat="server" Width="138px" AutoPostBack="true"
                                                                                        CssClass="testo" Height="16px">
                                                                                        <asp:ListItem></asp:ListItem>
                                                                                        <asp:ListItem>POP</asp:ListItem>
                                                                                        <asp:ListItem>IMAP</asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    <asp:CheckBox ID="ChkMailPec" runat="server" CssClass="testo" Width="3px" AutoPostBack="True"></asp:CheckBox>Protocolla
                                                                                    se PEC
                                                                                    <br /><asp:CheckBox ID="ChkMailRicevutePendenti" runat="server" CssClass="testo" Width="3px"></asp:CheckBox><asp:Label runat="server" id="ChkMailRicevutePendentiText">Mantieni mail ricevute come pendenti </asp:Label>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    <asp:CheckBox ID="cbx_invioAuto" runat="server" />&nbsp;Invio Ricevuta Automatico
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right" width="80">
                                                                                    User Id&nbsp;
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txt_utente" runat="server" CssClass="testo" Width="150px" MaxLength="128"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro">
                                                                                    Password&nbsp;<asp:TextBox ID="txt_password" runat="server" CssClass="testo" Width="100px"
                                                                                        MaxLength="32" TextMode="Password"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    Conferma&nbsp;<asp:TextBox ID="txt_conferma_pwd" runat="server" CssClass="testo"
                                                                                        MaxLength="32" TextMode="Password"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right" width="80">
                                                                                    POP&nbsp;
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txt_pop" runat="server" CssClass="testo" Width="150px" MaxLength="64"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro">
                                                                                    Porta&nbsp;<asp:TextBox ID="txt_portapop" runat="server" CssClass="testo" Width="35px"
                                                                                        MaxLength="10"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    <asp:CheckBox ID="ChkBoxPopSSl" runat="server" CssClass="testo" Width="3px"></asp:CheckBox>SSL
                                                                                    su POP
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right" width="80">
                                                                                    IMAP&nbsp;
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txt_imap" runat="server" CssClass="testo" Width="150px" MaxLength="64"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro">
                                                                                    Porta&nbsp;<asp:TextBox ID="txt_portaImap" runat="server" CssClass="testo" Width="35px"
                                                                                        MaxLength="10"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    <asp:CheckBox ID="Chk_sslImap" runat="server" CssClass="testo" Width="3px"></asp:CheckBox>SSL
                                                                                    su IMAP
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="right" width="80">
                                                                                    Inbox&nbsp;
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txt_inbox" runat="server" CssClass="testo" Width="150px" MaxLength="64"></asp:TextBox>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" colspan="2">
                                                                                    Cartella mail elaborate&nbsp;<asp:TextBox ID="txt_mailElaborate" runat="server" CssClass="testo"
                                                                                        Width="166px" MaxLength="64"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="left" colspan="4">
                                                                                    Cartella mail non elaborate&nbsp;&nbsp;
                                                                                    <asp:TextBox ID="txt_mailNonElaborate" runat="server" CssClass="testo" Width="150px"
                                                                                        MaxLength="64"></asp:TextBox>
                                                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                    <asp:Label ID="lbl_note">Note utilizzo casella&nbsp;&nbsp</asp:Label>
                                                                                    <asp:TextBox ID="txt_note" runat="server" CssClass="testo" Width="150px" MaxLength="20"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <triggers>
                                                                                <asp:AsyncPostBackTrigger ControlID="ddl_caselle" EventName="SelectedIndexChanged" />
                                                                            </triggers>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td height="2">
                                                            </td>
                                                        </tr>
                                                        <tr id="titoloIS" runat="server">
                                                            <td align="left">
                                                                <table class="contenitore" border="0" style="width: 100%">
                                                                    <tr>
                                                                        <td class="style1" colspan="4">
                                                                            Interoperabilità semplificata
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <uc3:InteroperabilitySettings ID="isRegistrySettings" IsRf="false" runat="server" />
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
                                                    <asp:Panel ID="pnl_ricevute" runat="server" Visible="true">
                                                        <tr>
                                                            <td colspan="2" class="titolo_pnl">
                                                                Modello di stampa ricevute
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <table border="0" cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:FileUpload ID="fileupload_ricevuta" CssClass="testo_btn" runat="server" />
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;&nbsp;&nbsp;<asp:Button ID="btn_upload_ricevuta" CssClass="testo_btn" runat="server"
                                                                                Text=" Carica " CauseValidation="false" />
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="lbl_ricevuta" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                                                                            <asp:ImageButton ID="btn_elimina_ricevuta" runat="server" ImageUrl="../Images/cestino.gif"
                                                                                ToolTip="Elimina" Visible="false" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <table cellspacing="0" cellpadding="0" width="100%">
                                                        <tr>
                                                            <td align="left" class="style3">
                                                                <asp:Label ID="lbl_msg" runat="server" CssClass="testo_rosso"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Button ID="btn_UOsmista" runat="server" CssClass="testo_btn" Text="Primo smistamento">
                                                                </asp:Button>&#160;&#160
                                                                <asp:Button ID="btn_test" runat="server" CssClass="testo_btn" Text="Test Connessione"
                                                                    Visible="False"></asp:Button>&#160;&#160;
                                                                <asp:Button ID="btn_aggiungi" runat="server" CssClass="testo_btn" Text="Aggiungi">
                                                                </asp:Button>&#160;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="pulsanti" align="left">
                                    <span class="titolo">Lista Registri di repertorio</span>
                                </td>
                            </tr>
                            <tr style="visibility: hidden">
                                <td style="height: 10;" />
                            </tr>
                            <tr>
                                <td align="center">
                                    <div id="divRepertori" style="overflow: auto; width: 100%; height: 133px; margin-bottom: 10px;">
                                        <asp:DataGrid ID="dgRepertori" runat="server" Width="100%" BorderWidth="1px" CellPadding="1"
                                            BorderColor="Gray" AutoGenerateColumns="False" OnItemCommand="dgRepertori_ItemCommand"
                                            DataSourceID="RegistriRepertorioManagerDataSource">
                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn Visible="False" DataField="CounterId" />
                                                <asp:TemplateColumn HeaderText="Descrizione">
                                                    <HeaderStyle HorizontalAlign="Center" Width="40%" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRegistryDescription" runat="server" Text="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).CounterDescription %>"
                                                            Font-Strikeout="<%# !((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).Enabled %>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Descrizione Tipologia">
                                                    <HeaderStyle HorizontalAlign="Center" Width="40%" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTypologyDescription" runat="server" Text="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).TipologyDescription %>"
                                                            Font-Strikeout="<%# !((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).Enabled %>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="LongTipology" HeaderText="Tipologia documentale">
                                                    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn>
                                                    <HeaderStyle Width="10%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    <ItemTemplate>
                                                        <asp:ImageButton CommandName="Select" runat="server" ID="imgDetails" ImageUrl="~/AdminTool/Images/lentePreview.gif"
                                                            ToolTip="Dettagli registro" Visible="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).Enabled %>" />
                                                        <asp:HiddenField ID="hfRegistryId" runat="server" Value="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).CounterId %>" />
                                                        <asp:HiddenField ID="hfTipology" runat="server" Value="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorio)Container.DataItem).Tipology %>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                        <asp:ObjectDataSource ID="RegistriRepertorioManagerDataSource" runat="server" SelectMethod="GetRegisteredRegistries"
                                            TypeName="DocsPAWA.utils.RegistriRepertorioUtils" OnSelecting="RegistriRepertorioManagerDataSource_Selecting">
                                            <SelectParameters>
                                                <asp:Parameter DefaultValue="0" Name="administrationId" Type="String" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btn_upload_ricevuta" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
