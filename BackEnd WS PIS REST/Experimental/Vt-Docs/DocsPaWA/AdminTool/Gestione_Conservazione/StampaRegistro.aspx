<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StampaRegistro.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.StampaRegistro" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuConservazione" Src="../UserControl/MenuConservazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="HeadStampaRegistro" runat="server">
    <title>Conservazione -> Stampa Registro</title>
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
                Conservazione -> Stampa Registro
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
                                Impostazione della frequenza di stampa registro:<br />
                                <br />
                                &nbsp;<br />
                                <table width="20%">
                                    <tr style="height: 25px">
                                        <td style="width: 35%; text-align: left" class="testo">
                                            <asp:CheckBox ID="cbAttivazioneStampaReg" runat="server" AutoPostBack="true" Text="Abilitato"
                                                class="testo_grigio_scuro" OnCheckedChanged="cbAttivazioneStampaReg_CheckedChanged"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 35%; text-align: right" class="testo">
                                            <asp:Label ID="lblFreq" runat="server" class="testo_grigio_scuro" Text="Frequenza:"></asp:Label>
                                            <asp:DropDownList ID="ddlFreqStampa" runat="server" CssClass="testo">
                                                <asp:ListItem Value="10" Text="Giornaliera" Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="20" Text="Settimanale"></asp:ListItem>
                                                <asp:ListItem Value="30" Text="Mensile"></asp:ListItem>
                                                <asp:ListItem Value="40" Text="Annuale"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 35%; text-align: right" class="testo">
                                            <asp:Label ID="lblHH" runat="server" class="testo_grigio_scuro" Text="Ora di stampa:"></asp:Label>
                                            <asp:DropDownList ID="ddlOraStampa" runat="server" CssClass="testo"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="width: 100%; text-align: center" class="testo">
                                            <asp:Label ID="lblDate" runat="server" class="testo_grigio_scuro" ></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                &nbsp;
                                <div id="button_new">
                                    <asp:Button ID="btn_salva_stampa_registro" runat="server" CssClass="cbtn" Text="Salva" OnClick="btnSalvaStampaRegistri_Click"/>
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
