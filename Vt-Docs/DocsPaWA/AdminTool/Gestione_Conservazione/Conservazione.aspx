<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Conservazione.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.Conservazione" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuConservazione" Src="../UserControl/MenuConservazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Conservazione</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet"/>
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

        function ReportMonitoraggioPolicy() {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.open("MonitoraggioPolicy.aspx", "MonitoraggioPolicy", 'width=600,height=320,scrollbars=NO,top=' + newTop + ',left=' + newLeft);
            
        }

		</script>
    <style type="text/css">
        .contenitore_box
        {
            background-color: #e0e0e0;
            text-align: center;
            max-width: 70%;
            font-family: Tahoma, Arial, sans-serif;
            margin: 0 auto;
        }
        .contenitore_box table
        {
            margin-top: 20px;
            font-family: Verdana;
            font-size: 10px;
            font-weight: bold;
        }
        .contenitore_box table tr
        {
            text-align: left;
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
<body onunload="ClosePopUp()" style="margin:0px;padding:0px;">
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
                Conservazione
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
                               &nbsp;
                                <asp:Panel runat="server" ID="pnlAttivaCons" Visible="false" Width="70%">
                                <div class="contenitore_box">
                                        <table width="100%">
                                            <tr>
                                                <td width="35%">Stato conservazione per l'amministrazione: </td>
                                                <td width="45%"><asp:Label runat="server" ID="lbl_status"></asp:Label></td>
                                                <td rowspan="2" width="20%" align="center">
                                                    <asp:Button ID="btn_attiva" runat="server" CssClass="cbtn" Text="Attiva" OnClick="btn_attiva_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%">Responsabile della conservazione: </td>
                                                <td colspan="2" width="45%"><asp:Label runat="server" ID="lbl_resp_cons"></asp:Label></td>
                                            </tr>
                                        </table>      
                                </div>
                                    &nbsp;
                                <div class="contenitore_box">
                                    <table width="100%">
                                        <tr>
                                            <td width="35%">Policy documenti attive nell'amministrazione</td>
                                            <td width="40%"><asp:Label runat="server" ID="lbl_policyDoc"></asp:Label></td>
                                            <td rowspan="2" width="20%" align="center">
                                                <asp:Button ID="btn_monitoring" runat="server" CssClass="cbtn" Text="Monitoraggio policy" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="35%">Policy stampe attive nell'amministrazione</td>
                                            <td width="40%"><asp:Label runat="server" ID="lbl_policyStampe"></asp:Label></td>
                                        </tr>
                                    </table>
                                </div>
                                </asp:Panel>   
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
