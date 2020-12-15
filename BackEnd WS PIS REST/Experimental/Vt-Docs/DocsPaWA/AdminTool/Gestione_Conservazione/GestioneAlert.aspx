<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneAlert.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.GestioneAlert" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuConservazione" Src="../UserControl/MenuConservazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="HeadGestioneAlert" runat="server">
    <title>Conservazione -> Gestione Alert</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="JavaScript" type="text/javascript">

        var cambiapass;
        var hlp;

        function apriPopup() {
            hlp = window.open('../help.aspx?from=CON', '', 'width=450,height=500,scrollbars=YES');
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
        function controllo() {

        }
    </script>
    <style type="text/css">
        #button_new
        {
            text-align: center;
            margin: 0px;
            padding: 0px;
            margin-top: 20px;
            margin-bottom: 10px;
        }
        .cbtn
        {
            font-family: Verdana;
            font-size: 11px;
            margin: 0px;
            padding: 0px;
            padding: 2px;
            width: 120px;
            height: 25px;
            color: #ffffff;
            border: 1px solid #ffffff;
            background-image: url('../images/bg_button.gif');
        }
    </style>
</head>
<body onunload="ClosePopUp()" style="margin: 0px; padding: 0px;">
    <form id="frm_conservazione" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Conservazione" />
    <!-- Gestione del menu a tendina -->
    <uc3:MenuTendina ID="MenuTendina" runat="server"></uc3:MenuTendina>
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
                Conservazione -> Gestione Alert
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                <table height="100" cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tr>
                        <td width="120" height="100%">
                            <uc2:MenuConservazione ID="MenuConservazione" runat="server"></uc2:MenuConservazione>
                        </td>
                        <td width="100%" height="100%" valign="middle" align="center">
                            <div id="DivSel" style="overflow: auto; height: 551px" class="testo_grigio_scuro">
                                <br />
                                Gestione Alert Conservazione<br />
                                &nbsp;<br />
                                <table width="80%">
                                    <tr style="background-color: #810d06">
                                        <td style="width: 50%; text-align: left" class="menu_1_bianco_dg">Alert</td>
                                        <td style="width: 10%; text-align: center" class="menu_1_bianco_dg">Abilita</td>
                                        <td colspan="2" style="width: 20%; text-align: center" class="menu_1_bianco_dg">Parametri</td>
                                    </tr>
                                    <!-- VERIFICA LEGGIBILITA' ANTICIPATA -->
                                    <tr style="height: 50px; background-color: #d9d9d9">
                                        <td style="width: 50%; text-align: left" class="testo_grigio_scuro">
                                            Esecuzione anticipata della verifica periodica di leggibilità: 
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:CheckBox runat="server" ID="chkVerificaAnticipata" AutoPostBack="true" OnCheckedChanged="CheckedChanged" />
                                        </td>
                                        <td style="width: 20%; text-align: left" class="testo">
                                            <asp:Label runat="server" ID="lblVerAntScadenza" Text="Scadenza (giorni): <br />" ></asp:Label>
                                            <asp:Label runat="server" ID="lblVerAntTolleranza" Text="Tolleranza (giorni):" ></asp:Label>
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:TextBox runat="server" ID="txtVerAntScadenza" Width="50px" style="margin-bottom: 1px;" CssClass="testo" Enabled="false" ></asp:TextBox><br />   
                                            <asp:TextBox runat="server" ID="txtVerAntTolleranza" Width="50px" style="margin-top: 1px;" CssClass="testo" Enabled="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <!-- VERIFICA LEGGIBILITA' SU CAMPIONE MAGGIORE DEL CONSENSITO -->
                                    <tr style="height: 50px; background-color: #f2f2f2">
                                        <td style="width: 50%; text-align: left" class="testo_grigio_scuro">
                                            Esecuzione della verifica di leggibilità su un campione maggiore di quello consentito:
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:CheckBox runat="server" ID="chkVerificaDimensioni" AutoPostBack="true" 
                                            OnCheckedChanged="CheckedChanged" ToolTip="Abilita l'alert" />
                                        </td>
                                        <td style="width: 20%; text-align: left" class="testo">
                                            <asp:Label runat="server" ID="lblVerDimPercentuale" Text="Max documenti verificabili (%): " ></asp:Label>
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:TextBox runat="server" ID="txtVerDimPercentuale" Width="50px" CssClass="testo"></asp:TextBox>
                                        </td> 
                                    </tr>
                                    <!-- VERIFICA LEGGIBLITA' DOCUMENTO SINGOLO -->
                                    <tr style="height: 50px; background-color: #d9d9d9">
                                        <td style="width: 50%; text-align: left" class="testo_grigio_scuro">
                                            Frequenza utilizzo verifica leggibilità su singolo documento
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">                                            
                                            <asp:CheckBox runat="server" ID="chkVerLegDocumento" AutoPostBack="true" 
                                            OnCheckedChanged="CheckedChanged" ToolTip="Abilita l'alert" />
                                        </td>
                                        <td style="width: 20%; text-align: left" class="testo">
                                            <asp:Label runat="server" ID="lblVerLegDocumento_max" Text="Num. massimo operazioni: <br />" ></asp:Label>
                                            <asp:Label runat="server" ID="lblVerLegDocumento_per" Text="Periodo di monitoraggio (giorni): "></asp:Label>
                                        </td> 
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:TextBox runat="server" ID="txtVerLegDocumento_max" Width="50px" style="margin-bottom: 1px;" CssClass="testo"></asp:TextBox><br />                                            
                                            <asp:TextBox runat="server" ID="txtVerLegDocumento_per" Width="50px" style="margin-top: 1px;" CssClass="testo"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <!-- DOWNLOAD ISTANZA -->
                                    <tr style="height: 50px; background-color: #f2f2f2">
                                        <td style="width: 50%; text-align: left" class="testo_grigio_scuro">
                                            Frequenza utilizzo download istanza
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">                                            
                                            <asp:CheckBox runat="server" ID="chkDownloadIstanza" AutoPostBack="true" 
                                            OnCheckedChanged="CheckedChanged" ToolTip="Abilita l'alert" />
                                        </td>
                                        <td style="width: 20%; text-align: left" class="testo">
                                            <asp:Label runat="server" ID="lblDownloadIstanza_max" Text="Num. massimo operazioni: <br />"></asp:Label>
                                            <asp:Label runat="server" ID="lblDownloadIstanza_per" Text="Periodo di monitoraggio (giorni): "></asp:Label>
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:TextBox runat="server" ID="txtDownloadIstanza_max" Width="50px" style="margin-bottom: 1px;" CssClass="testo"></asp:TextBox><br />
                                            <asp:TextBox runat="server" ID="txtDownloadIstanza_per" Width="50px" style="margin-top: 1px;" CssClass="testo"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <!-- SFOGLIA ISTANZA -->
                                    <tr style="height: 50px; background-color: #d9d9d9">
                                        <td style="width: 50%; text-align: left" class="testo_grigio_scuro">
                                            Frequenza utilizzo sfoglia istanza
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">                                            
                                            <asp:CheckBox runat="server" ID="chkSfogliaIstanza" AutoPostBack="true" 
                                            OnCheckedChanged="CheckedChanged" ToolTip="Abilita l'alert" />
                                        </td>
                                        <td style="width: 20%; text-align: left" class="testo">
                                            <asp:Label runat="server" ID="lblSfogliaIstanza_max" Text="Num. massimo operazioni: <br />" ></asp:Label>
                                            <asp:Label runat="server" ID="lblSfogliaIstanza_per" Text="Periodo di monitoraggio (giorni): " ></asp:Label>
                                        </td>
                                        <td style="width: 10%; text-align: center" class="testo">
                                            <asp:TextBox runat="server" ID="txtSfogliaIstanza_max" Width="50px" style="margin-bottom: 1px;" CssClass="testo"></asp:TextBox><br />
                                            <asp:TextBox runat="server" ID="txtSfogliaIstanza_per" Width="50px" style="margin-top: 1px;" CssClass="testo"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                &nbsp; <br />
                                Configurazione Server SMTP <br />
                                &nbsp; <br />
                                <table width="60%" style="background-color: #d9d9d9; padding-left: 3px; padding-right: 3px;">
                                    <tr style="height: 30px">
                                        <td style="text-align: left" colspan="2" class="testo_grigio_scuro">
                                            Server:&nbsp;
                                            <asp:TextBox runat="server" ID="txtMailServer" CssClass="testo" Width="250px" ></asp:TextBox>
                                        </td>
                                        <td style="text-align: left" class="testo_grigio_scuro">
                                            Porta:&nbsp;
                                            <asp:TextBox runat="server" ID="txtMailPorta" CssClass="testo" Width="50px"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                            SSL:&nbsp;
                                            <asp:CheckBox runat="server" ID="chkMailSSL" ToolTip="Abilita l'uso di connessione SSL su protocollo SMTP" />
                                        </td>
                                    </tr>
                                    <tr style="height: 30px">
                                        <td style="text-align: left" class="testo_grigio_scuro">
                                            Userid:&nbsp;
                                            <asp:TextBox runat="server" ID="txtMailUserID" CssClass="testo" Width="100px" ></asp:TextBox>
                                        </td>
                                        <td style="text-align: left" class="testo_grigio_scuro">
                                            Password:&nbsp;
                                            <asp:TextBox runat="server" ID="txtMailPwd" CssClass="testo" Width="100px" TextMode="Password"></asp:TextBox>
                                        </td>
                                        <td style="text-align: left" class="testo_grigio_scuro">
                                            Conferma password:&nbsp;
                                            <asp:TextBox runat="server" ID="txtMailPwdConferma" CssClass="testo" Width="100px" TextMode="Password"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                Parametri E-Mail Notifica
                                &nbsp;<br />
                                <br />
                                <table width="60%" style="background-color: #d9d9d9; padding-left: 3px; padding-right: 3px;">
                                    <tr style="text-align: left; height: 30px">
                                        <td style="text-align: left" class="testo_grigio_scuro">
                                            Mittente:&nbsp;                                 
                                            <asp:TextBox runat="server" ID="txtMailFrom" CssClass="testo" Width="200px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: left" class="testo_grigio_scuro">
                                            Destinatario:&nbsp;
                                            <asp:TextBox runat="server" ID="txtMailTo" CssClass="testo" Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>


                                &nbsp;
                                <div id="button_new">
                                    <asp:Button ID="btn_salva_alert" runat="server" CssClass="cbtn" Text="Salva" OnClick="btn_salva_alert_Click" />
                                </div>
                                <br />
                            </div>
                        </td>
                    </tr>
                </table>
                <!-- FINE CORPO CENTRALE -->
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
