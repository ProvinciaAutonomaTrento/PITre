<%@ Page Language="c#" CodeBehind="GestUtenti.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Utenti.GestUtenti" %>

<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script type="text/javascript" src="../CSS/caricamenujs.js"></script>
    <script type="text/javascript">

        var cambiapass;
        var hlp;
        var stut;
        var elut;
        var perm;

        function ApriGestRegistriTitolario(idCorrGlob, idAmm) {
            var myUrl = "VisualizzaRegistriUtente.aspx?idCorrGlob=" + idCorrGlob + "&idAmm=" + idAmm;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:460px;dialogHeight:213px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            //	Form1.hd_returnValueModal.value = rtnValue;								
            //window.document.Form1.submit();	
        }

        function ApriGestVociMenu(idCorrGlob, idAmm) {
            var myUrl = "VisualizzaMenuUtente.aspx?idCorrGlob=" + idCorrGlob + "&idAmm=" + idAmm;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:460px;dialogHeight:650px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            Form1.hd_returnValueModal.value = rtnValue;
            //window.document.Form1.submit();	
        }
        function ApriVisRuoli(idPeople, nome) {
            var myUrl = "VisualizzaRuoliUtente.aspx?idPeople=" + idPeople + "&nome=" + nome;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:600px;dialogHeight:440px;status:no;resizable:no;scroll:no;dialogLeft:0;dialogTop:0;center:yes;help:no;");
        }
        function ApriImportaUtenti() {
            var myUrl = "ImportaUtenti.aspx";
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:430px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            //rtnValue = window.open(myUrl,"","dialogWidth:430px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
            Form1.submit();
        }

        function apriPopup() {
            hlp = window.open('../help.aspx?from=UT', '', 'width=450,height=500,scrollbars=YES');
        }
        function apriPopupUtConn() {
            stut = window.open('StatoConnUtenti.aspx', '', 'width=500,height=500,scrollbars=YES');
        }
        function DivHeight() {
            if (DivDGList.scrollHeight < 181)
                DivDGList.style.height = DivDGList.scrollHeight;
            else
                DivDGList.style.height = 181;
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
            /*
            if(typeof(stut) != 'undefined')
            {
            if(!stut.closed)
            stut.close();
            }
            */
            if (typeof (elut) != 'undefined') {
                if (!elut.closed)
                    elut.close();
            }
            if (typeof (perm) != 'undefined') {
                if (!perm.closed)
                    perm.close();
            }
        }

        function ShowLdapSyncronization() {
            var pageHeight = 650;
            var pageWidth = 750;

            return (window.showModalDialog('LdapSyncronization.aspx',
			                            null,
			                            'dialogWidth:' + pageWidth +
			                            'px;dialogHeight:' + pageHeight +
			                            'px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no') == 'true');
        }

        function ShowLdapUserSyncronization() {
            var pageHeight = 200;
            var pageWidth = 750;

            return (window.showModalDialog('LdapUserConfigurations.aspx?idUser=<%=this.CurrentIdUser%>',
			                            null,
			                            'dialogWidth:' + pageWidth +
			                            'px;dialogHeight:' + pageHeight +
			                            'px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no') == 'true');
        }
        function AvvisoUtenteConLF(tipoTitolare, idRuolo, idUtente, numProcessi, numIstanze) {
            var myUrl = "../Gestione_Organigramma/AvvisoRuoloConLF.aspx?tipoTitolare=" + tipoTitolare + "&idRuolo=" + idRuolo + "&idUtente=" + idUtente + "&numProcessi=" + numProcessi + "&numIstanze=" + numIstanze;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:450px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            Form1.hd_returnValueModalLF.value = rtnValue;
            window.document.Form1.submit();	
        }
    </script>
    <script language="javascript" id="btn_aggiungi_click" event="onclick()" for="btn_aggiungi">
			window.document.body.style.cursor='wait';
    </script>
    <script language="javascript" id="btn_find_click" event="onclick()" for="btn_find">
			window.document.body.style.cursor='wait';
    </script>
    <style type="text/css">
        .style1
        {
            width: 107px;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 25%;
        }
        .style3
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 24%;
        }
    </style>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" onunload="ClosePopUp()"
    ms_positioning="GridLayout">
    <form id="Form1" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Utenti" />
    <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
    <input id="hd_returnValueModalLF" type="hidden" name="hd_returnValueModalLF" runat="server">
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <table style="width: 100%; height: 100%" cellspacing="1" cellpadding="0" width="100%"
        border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" TabIndex="25" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Utenti
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <table cellspacing="0" cellpadding="0" border="0" style="width: 90%">
                    <tr>
                        <td align="center" style="width: 100%; height: 10px">
                            <asp:Label ID="lbl_avviso" TabIndex="23" runat="server" CssClass="testo_rosso"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trIntegrazioneLdap" runat="server">
                        <td align="center" style="width: 100%; height: 40px;">
                            <table cellspacing="1" cellpadding="0" width="100%" border="0">
                                <tr>
                                    <td class="pulsanti" style="width: 85%">
                                        <asp:Label ID="Label1" runat="server" CssClass="titolo" Text="Integrazione LDAP"></asp:Label>
                                    </td>
                                    <td align="right" class="pulsanti" style="width: 15%">
                                        <asp:Button ID="btnSyncronizeLdap" runat="server" Text="Sincronizzazione utenti"
                                            CssClass="testo_btn" OnClick="btnSyncronizeLdap_Click" OnClientClick="return ShowLdapSyncronization()"
                                            Width="100%" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="pulsanti" align="center" style="width: 100%">
                            <!-- RICERCA UTENTE -->
                            <table id="tblPulsanti" runat="server" cellspacing="1" cellpadding="0" width="100%"
                                border="0">
                                <tr>
                                    <td width="100%" colspan="3">
                                        <asp:Label ID="lbl_tit" CssClass="titolo" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="testo_grigio_scuro" width="10%">
                                        <asp:DropDownList ID="ddl_ricerca" TabIndex="1" CssClass="testo_grigio_scuro_grande"
                                            runat="server" AutoPostBack="True" Width="100%">
                                            <asp:ListItem Value="*">Tutti</asp:ListItem>
                                            <asp:ListItem Value="var_cod_rubrica">Codice</asp:ListItem>
                                            <asp:ListItem Value="var_cognome" Selected="True">Cognome</asp:ListItem>
                                            <asp:ListItem Value="var_nome">Nome</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td width="30%">
                                        <asp:TextBox ID="txt_ricerca" TabIndex="2" CssClass="testo_grigio_scuro_grande" runat="server"></asp:TextBox>
                                        <asp:Button ID="btn_find" TabIndex="3" CssClass="testo_btn" runat="server" Text="Cerca">
                                        </asp:Button>
                                    </td>
                                    <td width="55%" align="right">
                                        <asp:Button ID="btn_newUt" TabIndex="5" runat="server" CssClass="testo_btn" Text="Nuovo Utente">
                                        </asp:Button>
                                        <asp:Button ID="btn_UtConn" TabIndex="4" runat="server" CssClass="testo_btn" Text="Utenti connessi">
                                        </asp:Button>
                                        <asp:Button ID="btn_importa" runat="server" Text="Importa da Excel" CssClass="testo_btn"
                                            OnClick="btn_importa_Click"></asp:Button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td height="3" style="width: 100%">
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 100%">
                            <asp:Panel ID="pnlDgUtenti" runat="server" ScrollBars="Auto" Width="100%" Height="180px">
                                <asp:DataGrid ID="dg_Utenti" TabIndex="7" runat="server" Width="100%" PageSize="6"
                                    AllowPaging="True" BorderWidth="1px" CellPadding="1" BorderColor="Gray" OnDeleteCommand="deleteGrid"
                                    AutoGenerateColumns="False" OnItemCreated="dg_Utenti_ItemCreated">
                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                    <Columns>
                                        <asp:BoundColumn Visible="False" DataField="idCorrGlobale" ReadOnly="True" HeaderText="ID_CORR_GLOB">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="idPeople" ReadOnly="True" HeaderText="ID_PEOPLE">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="userid" ReadOnly="True" HeaderText="USERID">
                                            <HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="password" ReadOnly="True" HeaderText="PASSWORD">
                                            <HeaderStyle HorizontalAlign="Center" Width="0%"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="cognome" ReadOnly="True" HeaderText="COGNOME">
                                            <HeaderStyle HorizontalAlign="Center" Width="35%"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="nome" ReadOnly="True" HeaderText="NOME">
                                            <HeaderStyle HorizontalAlign="Center" Width="35%"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="codice" ReadOnly="True" HeaderText="CODICE">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="codiceRubrica" ReadOnly="True" HeaderText="COD_RUBRICA">
                                            <HeaderStyle HorizontalAlign="Center" Width="19%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="amministratore" ReadOnly="True" HeaderText="AMMINISTRATORE">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="email" ReadOnly="True" HeaderText="EMAIL">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="abilitato" ReadOnly="True" HeaderText="ABILITATO">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="dominio" ReadOnly="True" HeaderText="DOMINIO">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="sede" ReadOnly="True" HeaderText="SEDE">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="notifica" ReadOnly="True" HeaderText="NOTIFICA">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="idAmministrazione" ReadOnly="True" HeaderText="ID_AMMINISTRAZIONE">
                                        </asp:BoundColumn>
                                        <asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;"
                                            CommandName="Select">
                                            <HeaderStyle Width="5%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:ButtonColumn>
                                        <asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina' &gt;"
                                            CommandName="Delete">
                                            <HeaderStyle Width="5%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:ButtonColumn>
                                        <asp:BoundColumn DataField="nessunaScadenzaPassword" Visible="False" HeaderText="NESSUNA_SCADENZA_PASSWORD">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="fromEmail" ReadOnly="True" HeaderText="FROMEMAIL">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="SincronizzaLdap" HeaderText="SINCRONIZZALDAP" Visible="False">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="IdSincronizzazioneLdap" HeaderText="IDSINCRONIZZAZIONELDAP"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="IdClientSideModelProcessor" DataField="IdClientSideModelProcessor"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="IsEnabledSmartClient" DataField="IsEnabledSmartClient"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="ApplyPdfConvertionOnScan" DataField="ApplyPdfConvertionOnScan"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="AutenticatoLdap" DataField="AutenticatoLdap" Visible="False">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="DispositivoStampa" DataField="DispositivoStampa" Visible="False">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="AbilitatoCentroServizi" DataField="AbilitatoCentroServizi"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="matricola" ReadOnly="True" HeaderText="MATRICOLA">
                                        </asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="AbilitatoChiaviConfig" DataField="AbilitatoChiaviConfig"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="Cha_Tipo_Componenti" DataField="Cha_Tipo_Componenti"
                                            Visible="False"></asp:BoundColumn>
                                        <asp:BoundColumn HeaderText="AbilitatoEsibizione" DataField="AbilitatoEsibizione"
                                            Visible="False"></asp:BoundColumn>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" PageButtonCount="20" CssClass="bg_grigioN"
                                        Mode="NumericPages"></PagerStyle>
                                </asp:DataGrid>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td height="5" style="width: 100%">
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="width: 100%">
                            <asp:Panel ID="pnl_info" runat="server" Visible="False" Width="100%">
                                <table class="contenitore" width="100%">
                                    <tr>
                                        <td colspan="4">
                                            <table cellspacing="0" cellpadding="0" width="100%">
                                                <tr>
                                                    <td class="titolo_pnl" style="height: 14px">
                                                        Dettagli utente
                                                    </td>
                                                    <td class="titolo_pnl" align="right" style="height: 14px">
                                                        &nbsp;
                                                    </td>
                                                    <td class="titolo_pnl" align="right" style="height: 14px">
                                                        <asp:ImageButton ID="btn_chiudiPnlInfo" TabIndex="7" runat="server" ToolTip="Chiudi"
                                                            ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%">
                                            UserId *&nbsp;
                                        </td>
                                        <td align="left" style="width: 25%">
                                            <asp:TextBox ID="txt_userid" TabIndex="8" CssClass="testo" runat="server" Width="100%"
                                                MaxLength="32" AutoPostBack="True"></asp:TextBox>
                                        </td>
                                        <td class="style3" align="right">
                                            Abilitato&nbsp;
                                        </td>
                                        <td class="testo_grigio_scuro" align="left" style="width: 25%" valign="middle">
                                            <asp:CheckBox ID="cb_abilitato" TabIndex="9" CssClass="testo" runat="server" OnCheckedChanged="chkAbilitato_OnCheckedChanged">
                                            </asp:CheckBox>
                                            <asp:CheckBox ID="chkUserPasswordNeverExpire" runat="server" Text="Nessuna scadenza"
                                                CssClass="testo" TextAlign="Left" />
                                            <asp:CheckBox ID="chkUserAutomatic" runat="server" Text="Automatico"
                                                CssClass="testo" TextAlign="Left" />
                                            <asp:CheckBox ID="cb_abilitatoCentroServizi" CssClass="testo" runat="server" Text="Abilitato al Centro Servizi"
                                                TextAlign="Left"></asp:CheckBox>
                                            <!-- End Mev CS 1.4 - Esibizione: Flag utilizzato per definire se l'utente può fare Esibizione -->
                                            <asp:CheckBox ID="cb_abilitatoEsibizione" CssClass="testo" runat="server" Text="Abilitato Esibizione"
                                                TextAlign="Left"></asp:CheckBox>
                                            <!-- End Mev CS 1.4 - Esibizione -->
                                            <asp:CheckBox runat="server" ID="cbx_chiavi" CssClass="testo" Text="Abilita gestione chiavi conf."
                                                TextAlign="Left" />
                                        </td>
                                    </tr>
                                    <tr id="trMatricola" runat="server">
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%;">
                                            Matricola&nbsp;
                                        </td>
                                        <td style="width: 25%;">
                                            <asp:TextBox ID="txt_Matricola" TabIndex="10" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="255"></asp:TextBox>
                                        </td>
                                        <td class="style3" align="right">
                                        </td>
                                        <td class="testo_grigio_scuro" align="left" style="width: 25%;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%;">
                                            Dominio autent.&nbsp;
                                        </td>
                                        <td style="width: 25%;">
                                            <asp:TextBox ID="txt_dominio" TabIndex="10" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="255"></asp:TextBox>
                                        </td>
                                        <td class="style3" align="right">
                                            Password&nbsp;
                                        </td>
                                        <td class="testo_grigio_scuro" align="left" style="width: 25%;">
                                            <asp:Label ID="lbl_password" runat="server" Visible="False"></asp:Label>
                                            <asp:TextBox ID="txt_password" TabIndex="11" runat="server" CssClass="testo" Width="70px"
                                                MaxLength="30" TextMode="Password"></asp:TextBox>&nbsp; Conferma
                                            <asp:TextBox ID="txt_ConfPassword" TabIndex="12" runat="server" CssClass="testo"
                                                MaxLength="30" TextMode="Password" Width="70px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%;">
                                            Nome *&nbsp;
                                        </td>
                                        <td style="width: 25%;">
                                            <asp:TextBox ID="txt_nome" TabIndex="13" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="100"></asp:TextBox>
                                        </td>
                                        <td class="style3" align="right">
                                            Cognome *
                                        </td>
                                        <td style="width: 25%;">
                                            <asp:TextBox ID="txt_cognome" TabIndex="14" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="100"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%;">
                                            Cod. rubrica *&nbsp;
                                        </td>
                                        <td style="width: 25%;">
                                            <asp:TextBox ID="txt_rubrica" TabIndex="15" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="128"></asp:TextBox>
                                        </td>
                                        <td class="style3" align="right">
                                            Email&nbsp;&nbsp;
                                        </td>
                                        <td style="width: 25%;">
                                            <asp:TextBox ID="txt_email" TabIndex="16" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="100"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%;">
                                            Sede&nbsp;
                                        </td>
                                        <td style="width: 25%; height: 22px;">
                                            <asp:TextBox ID="txt_sede" TabIndex="17" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="64"></asp:TextBox>
                                        </td>
                                        <td class="style3" align="right">
                                            Notifica trasmis.ne
                                        </td>
                                        <td style="width: 25%; height: 22px;">
                                            <asp:DropDownList ID="ddl_notifica" TabIndex="18" runat="server" CssClass="testo">
                                                <asp:ListItem Value="null">nessuna notifica</asp:ListItem>
                                                <asp:ListItem Value="E" Selected="True">tramite email (link a scheda documento/fascicolo e, se documento, link a immagine)</asp:ListItem>
                                                <asp:ListItem Value="ED">tramite email (allegati e link a scheda documento/fascicolo e, se documento, link a immagine)</asp:ListItem>
                                                <asp:ListItem Value="EA">tramite email (solo allegati)</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%">
                                            Amministratore
                                        </td>
                                        <td class="testo_grigio_scuro" style="width: 25%">
                                            <asp:DropDownList ID="cb_amm" TabIndex="19" CssClass="testo" runat="server" AutoPostBack="True"
                                                Width="175px" />
                                            &nbsp;
                                            <asp:ImageButton ID="btn_vociMenu" runat="server" Visible="False" ImageUrl="../images/userAdmin_menu.gif"
                                                ImageAlign="AbsBottom" AlternateText="Associa voci di menù per la gestione dell'amministrazione">
                                            </asp:ImageButton>
                                        </td>
                                        <td class="style3" align="right">
                                            From mail di notifica
                                        </td>
                                        <td style="width: 25%">
                                            <asp:TextBox ID="txt_from_email" TabIndex="16" runat="server" CssClass="testo" Width="100%"
                                                MaxLength="100"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" align="right" style="width: 25%">
                                            <asp:Label ID="lblClientModelProcessor" runat="server">Word processor</asp:Label>
                                        </td>
                                        <td style="width: 25%">
                                            <asp:DropDownList ID="cboClientModelProcessor" runat="server" CssClass="testo" Width="300px">
                                                <asp:ListItem Value="0">Seleziona...</asp:ListItem>
                                                <asp:ListItem Value="0">----------------------------------</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style3" align="right">
                                            <asp:Label ID="Label2" runat="server">Utilizza componenti :</asp:Label>
                                        </td>
                                        <td align="left" style="width: 25%">
                                            <table>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:RadioButton ID="rdbDisableAll" Text="Default amm." runat="server" GroupName="ClientType"
                                                            AutoPostBack="true" CssClass="testo_grigio_scuro" OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" /><br />
                                                        <asp:RadioButton ID="rdbIsEnabledActiveX" Text="ActiveX" runat="server" GroupName="ClientType"
                                                            AutoPostBack="true" CssClass="testo_grigio_scuro" OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" /><br />
                                                        <asp:RadioButton ID="rdbIsEnabledSmartClient" Text="SmartClient" runat="server" GroupName="ClientType"
                                                            AutoPostBack="true" CssClass="testo_grigio_scuro" OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged"
                                                            ToolTip="Abilita l'utente all'utilizzo dei componenti SmartClient dalla propria postazione di lavoro" /><br />
                                                        <asp:RadioButton ID="rdbIsEnabledJavaApplet" Text="JavaApplet" runat="server" GroupName="ClientType"
                                                            AutoPostBack="true" CssClass="testo_grigio_scuro" OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" /><br />
                                                        <asp:RadioButton ID="rdbIsEnabledHTML5Socket" Text="HTML5Socket" runat="server" GroupName="ClientType"
                                                            AutoPostBack="true" CssClass="testo_grigio_scuro" OnCheckedChanged="chkIsEnabledSmartClient_OnCheckedChanged" />
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                        <asp:CheckBox ID="chkSmartClientConvertPdfOnScan" runat="server" CssClass="testo_grigio_scuro"
                                                            TextAlign="Left" Text="Acquisisci in formato PDF" ToolTip="Acquisisce i documenti da scanner direttamente in formato PDF" />
                                                    </td>
                                                    <td class="testo_grigio_scuro">
                                                        <asp:Label ID="Label3" runat="server">Tipo stampante etichette</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList CssClass="testo" runat="server" ID="ddlLabelPrinterType">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="trLdapIntegrationDetail" runat="server">
                                        <td align="right" class="testo_grigio_scuro" style="width: 25%">
                                            <asp:Label ID="lblSyncronizeLdap" runat="server" Text="Sincronizzato in LDAP"></asp:Label>
                                        </td>
                                        <td class="testo_grigio_scuro" colspan="3">
                                            <input type="hidden" id="hdIdSincronizzazioneLdap" runat="server" />
                                            <asp:CheckBox ID="chkSyncronizeLdap" runat="server" AutoPostBack="true" CssClass="testo" />
                                            <asp:CheckBox ID="chkAutenticatoLdap" runat="server" AutoPostBack="true" CssClass="testo"
                                                TextAlign="Left" Text="Autenticato in LDAP" OnCheckedChanged="chkAutenticatoLdap_OnCheckedChanged" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" colspan="4">
                                            <asp:Button ID="btn_ruoli" runat="server" CssClass="testo_btn" TabIndex="21" Text="Visualizza ruoli" />
                                            &nbsp;
                                            <asp:Button ID="btn_aggiungi" runat="server" CssClass="testo_btn" TabIndex="20" Text="Aggiungi" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <asp:Label ID="lbl_msg" runat="server" CssClass="testo_rosso" TabIndex="22"></asp:Label>
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
    <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_conferma" runat="server"
        OnGetMessageBoxResponse="msg_conferma_GetMessageBoxResponse" />
    <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_modifica" runat="server"
        OnGetMessageBoxResponse="msg_modifica_GetMessageBoxResponse" />
    </form>
</body>
</html>
