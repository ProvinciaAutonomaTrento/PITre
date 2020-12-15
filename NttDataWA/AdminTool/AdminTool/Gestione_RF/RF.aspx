<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<%@ Page Language="c#" CodeBehind="RF.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_RF.RF" %>

<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc3" TagName="RubricaComune" Src="~/AdminTool/UserControl/RubricaComune.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="HEAD1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
    <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
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

        function ShowContinueModifyRF(message) {

            if (window.confirm(message + "\n\nContinuare?")) {
                Form1.txtCommandPending.value = 'MODIFY';
                Form1.submit();
            }
            else {
                Form1.txtCommandPending.value = '';
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

        function ApriGestioneRuoliInRF(idAooColl, codRF, codAooCol, chaDisabled, idReg, idAmm) {


            var ddl = document.getElementById("ddl_registri");
            if (ddl.value != null && ddl.value != "" && ddl.value == idAooColl) {
                var myUrl = "GestioneRuoliInRF.aspx?codRF=" + codRF + "&codAooCol=" + codAooCol + "&chaDisabled=" + chaDisabled + "&idAooColl=" + idAooColl + "&idReg=" + idReg + "&idAmm=" + idAmm;
                rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:750px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:yes;help:no;");

            }
            else {
                alert('Per associare dei ruoli all\'RF è necessario prima salvare\nle modifiche relative alla AOO collegata');
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
            width: 369px;
        }
    </style>
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
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" onunload="ClosePopUp()">
    <form id="Form1" method="post" runat="server">
    <asp:ScriptManager ID="scrManager" runat="server">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Raggruppamenti funzionali" />
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
                Raggruppamenti funzionali
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <asp:UpdatePanel ID="UpdatePanelGridRF" runat="server">
                    <ContentTemplate>
                        <asp:UpdateProgress  ID="updProgressCasella" runat="server" >
                            <ProgressTemplate >
                                <div style="width:80%;height:80%;color:black;position:absolute; 
                                    top:38%; left:10%;right:10%;bottom:0%; vertical-align:middle;padding-top:30%;"> 
                                    <asp:Label ID="lbl_wait" runat="server" Font-Bold="true" Font-Size="Medium">Attendere...</asp:Label>
                                    <asp:Image runat="server" ID="img_load_caselle" ImageUrl="~/images/loading.gif" ImageAlign="Middle" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <table cellspacing="0" cellpadding="0" align="center" border="0">
                            <tbody>
                                <tr>
                                    <td align="center" height="25">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="pulsanti" align="center" width="700">
                                        <table width="100%">
                                            <tr>
                                                <td class="titolo" align="left">
                                                    <asp:Label ID="lbl_tit" CssClass="titolo" runat="server">
													Lista RF
                                                    </asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Button ID="btn_nuova" runat="server" CssClass="testo_btn" Text="Nuovo RF"></asp:Button>&nbsp;
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
                                    <td style="height: 157px" align="center">
                                        <div id="DivDGList" style="overflow: auto; width: 720px; height: 151px">
                                            <asp:DataGrid ID="dg_RF" runat="server" Width="100%" BorderWidth="1px" CellPadding="1"
                                                BorderColor="Gray" AutoGenerateColumns="False" OnItemDataBound="dg_RF_ItemDataBound">
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
                                                    <asp:BoundColumn Visible="False" DataField="Disabled" HeaderText="DISABLED">
                                                        <HeaderStyle Width="0%"></HeaderStyle>
                                                    </asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="10">
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <asp:Panel ID="pnl_info" runat="server" Visible="false">
                                            <table class="contenitore" width="100%">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="titolo_pnl" align="left">
                                                                    Dettagli RF
                                                                </td>
                                                                <td class="titolo_pnl" align="right">
                                                                    <!--<asp:ImageButton id="btn_chiudiPnlInfo" runat="server" ImageUrl="../Images/cancella.gif" ToolTip="Chiudi"></asp:ImageButton></TD>-->
                                                                    <asp:CheckBox ID="ChkDisabilitato" runat="server" CssClass="testo_grigio_scuro" Text="Disabilitato">
                                                                    </asp:CheckBox>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="30px">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    Codice *&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txt_codice" runat="server" CssClass="testo" Width="200px" MaxLength="16"></asp:TextBox>
                                                                    <asp:Label ID="lbl_cod" runat="server" CssClass="testo"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 30px">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    Descrizione *&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txt_descrizione" runat="server" CssClass="testo" Width="250px" MaxLength="128"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="30px">
                                                        <table cellspacing="0" cellpadding="0" width="100%" height="20px">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    AOO collegata *&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_registri" runat="server" AutoPostBack="True" Width="250px"
                                                                        CssClass="testo_grigio_scuro">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="titolo_pnl">
                                                        Interoperabilita' attraverso la posta elettronica
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
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
                                                                            <asp:AsyncPostBackTrigger ControlID="dg_RF" EventName="SelectedIndexChanged" />
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
                                                                                        <br /><asp:CheckBox ID="ChkMailRicevutePendenti" runat="server" CssClass="testo" Width="3px"></asp:CheckBox><asp:Label ID="ChkMailRicevutePendentiText" runat="server">Mantieni mail ricevute come pendenti </asp:Label>                                                                                 
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
                                                                                <tr align="justify">
                                                                                    <td class="testo_grigio_scuro" colspan="4" width="20%">
                                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                        <asp:Label ID="lbl_mailNonElaborate">Cartella mail non elaborate&nbsp;&nbsp;</asp:Label>
                                                                                        <asp:TextBox ID="txt_mailNonElaborate" runat="server" CssClass="testo" Width="150px"
                                                                                            MaxLength="64"></asp:TextBox>
                                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                        <asp:Label ID="lbl_note">Note utilizzo casella&nbsp;&nbsp</asp:Label>
                                                                                        <asp:TextBox ID="txt_note" runat="server" CssClass="testo" Width="150px" MaxLength="20"></asp:TextBox>
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
                                                        </table>
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
                                                                    <asp:Button ID="btn_GestRuoliInUo" runat="server" CssClass="testo_btn" Text="Gestione Ruoli in UO">
                                                                    </asp:Button>&nbsp;&nbsp;&nbsp;
                                                                    <asp:Button ID="btn_test" runat="server" CssClass="testo_btn" Text="Test Connessione"
                                                                        Visible="False"></asp:Button>&#160;&#160;
                                                                    <asp:Button ID="btn_aggiungi" runat="server" CssClass="testo_btn" Text="Aggiungi">
                                                                    </asp:Button>&nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <uc3:RubricaComune ID="RubricaComune1" runat="server" Tipo="RF" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cc2:MessageBox ID="msg_confirmModificaRF" runat="server"></cc2:MessageBox>
                                    </td>
                                </tr>
                        </table>
                        </td> </tr> </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
    </form>
</body>
</html>
