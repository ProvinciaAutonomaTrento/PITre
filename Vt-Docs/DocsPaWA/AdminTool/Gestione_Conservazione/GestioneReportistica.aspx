<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneReportistica.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.GestioneReportistica" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuConservazione" Src="../UserControl/MenuConservazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Conservazione - Gestione Reportistica</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script src="../../LIBRERIE/rubrica.js" type="text/javascript"></script>
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
    <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Conservazione > Gestione Reportistica" />
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
                Conservazione -> Gestione reportistica
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
                                <asp:UpdatePanel runat="server" ID="pnlRecipients" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="80%">
                                            <tr style="height: 25px; margin: 10px;">
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr style="height: 25px; margin: 10px;">
                                                <td style="width: 20%; text-align: left" valign="top" class="testo_grigio_scuro">
                                                    <asp:Label runat="server" Text="Destinatari report errori:<br />(è possibile inserire più indirizzi separati da ';') "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txt_mail_struttura" CssClass="inp2" Width="600px" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="pnlRecipientsPolicy" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="80%">
                                            <tr style="height: 25px; margin: 10px;">
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr style="height: 25px; margin: 10px;">
                                                <td style="width: 20%; text-align: left"  valign="top" class="testo_grigio_scuro">
                                                    <asp:Label runat="server" Text="Destinatari alert policy:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txt_mail_policy" CssClass="inp2" Width="600px" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div style="margin-top: 30px">
                                <asp:UpdatePanel runat="server" ID="pnlMailbox" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="55%" valign="middle" align="center">
                                            <tr align="center" style="margin-bottom:10px;">
                                                <td colspan="3" class="testo_grigio_scuro_grande">Configurazione casella</td>
                                            </tr>
                                            <tr align="left">
                                                <td class="testo_grigio_scuro" colspan="2">
                                                    <asp:label runat="server" ID="lbl_smtp" Text="Server SMTP"></asp:label>
                                                    <asp:TextBox runat="server" ID="txt_smtp" CssClass="testo" Width="350px"></asp:TextBox>
                                                </td>
                                                <td class="testo_grigio_scuro">
                                                    <asp:Literal ID="lbl_port" runat="server">Porta</asp:Literal>
                                                    <asp:TextBox runat="server" ID="txt_port" CssClass="testo" Width="50" MaxLength="5"></asp:TextBox>&nbsp;&nbsp;
                                                    <asp:CheckBox runat="server" ID="chk_ssl" Text="SSL" TextAlign="Left" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td class="testo_grigio_scuro">
                                                    <asp:label runat="server" ID="lbl_user" Text="Username"></asp:label>
                                                    <asp:TextBox runat="server" ID="txt_username" CssClass="testo" Width="100px"></asp:TextBox>
                                                </td>
                                                <td class="testo_grigio_scuro">
                                                    <asp:Label runat="server" ID="lbl_pass" Text="Password"></asp:Label>
                                                    <asp:TextBox runat="server" ID="txt_password" CssClass="testo" TextMode="Password" Width="100px"></asp:TextBox>
                                                </td>
                                                <td class="testo_grigio_scuro">
                                                    <asp:Label runat="server" ID="lbl_conf_pass" Text="Conferma"></asp:Label>
                                                    <asp:TextBox runat="server" ID="txt_conf_pass" CssClass="testo" TextMode="Password" Width="100px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td class="testo_grigio_scuro" colspan="3">
                                                    <asp:label runat="server" ID="lbl_from" Text="Indirizzo e-mail From:"></asp:label>
                                                    <asp:TextBox runat="server" ID="txt_from" CssClass="testo" Width="250px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                    </div>
                                <div id="button_new">
                                    <asp:Button ID="btn_save" runat="server" CssClass="cbtn" Text="Salva" OnClick="btnSalva_Click" />
                                </div>
                            </div>
                        </td>
                    </tr>
                <!-- FINE CORPO CENTRALE -->
                </table>
            </td>
        </tr>
        </table>
        </form>
</body>
</html>
