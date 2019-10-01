  <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlliAutomatici.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.ControlliAutomatici" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuConservazione" Src="../UserControl/MenuConservazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc4" TagName="Time" Src="~/AdminTool/Gestione_Pubblicazioni/TimeControl.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Conservazione -> Controlli Automatici</title>
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
                Conservazione -> Controlli Automatici
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
                                Impostazione dello Scheduler per le verifiche automatiche:<br />
                                <br />
                                Modalità di avvio del servizio:<br />
                                <br />
                               &nbsp;<br />
                               <table width="70%">

                               <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo" >
            <asp:CheckBox ID="Enabled" runat="server" class="testo_grigio_scuro" 
                AutoPostBack="true" Text="Abilitato" 
                oncheckedchanged="Enabled_CheckedChanged"/>
        </td>
        <td style="width: 65%" class="testo">
        </td>
        </tr>
        <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group1" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true" />
        </td>
        <td style="width: 65%" class="testo">
            ogni 
            <asp:TextBox ID="txtGroup1Number" runat="server" Width="40px"  CssClass="testo"></asp:TextBox>
            <!--<asp:ListItem Value="BySecond" Text="Secondo" Selected="True"></asp:ListItem>-->
            <asp:DropDownList ID="cboGroup1ExecutionMode" runat="server"  CssClass="testo">
            
                <asp:ListItem Value="ByMinute" Text="Minuto"></asp:ListItem> 
                <asp:ListItem Value="Hourly" Text="Ora"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group2" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true"/>
        </td>
        <td style="width: 65%; text-align: left" class="testo">
            ogni giorno alle 
            <uc4:Time id="timeGroup2" runat="server"></uc4:Time>
        </td>
    </tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group3" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true" />
        </td>
        <td style="width: 65%; text-align: left" class="testo">
            ogni <asp:DropDownList ID="cboGroup3Days" runat="server" CssClass="testo">
                <asp:ListItem Value="1" Text="Lunedì" Selected="True"></asp:ListItem>
                <asp:ListItem Value="2" Text="Martedì"></asp:ListItem>
                <asp:ListItem Value="3" Text="Mercoledì"></asp:ListItem>
                <asp:ListItem Value="4" Text="Giovedì"></asp:ListItem>
                <asp:ListItem Value="5" Text="Venerdì"></asp:ListItem>
                <asp:ListItem Value="6" Text="Sabato"></asp:ListItem>
                <asp:ListItem Value="0" Text="Domenica"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="requiredCboGroup3Days" runat="server" ControlToValidate="cboGroup3Days" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
            alle 
            <uc4:Time id="timeGroup3" runat="server"></uc4:Time>
        </td>
    </tr>
                               </table>
&nbsp;
              <div id="button_new">
                                    <asp:Button ID="btn_salva_scheduler" runat="server" CssClass="cbtn" 
                                        Text="Salva" onclick="btn_salva_scheduler_Click" />
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