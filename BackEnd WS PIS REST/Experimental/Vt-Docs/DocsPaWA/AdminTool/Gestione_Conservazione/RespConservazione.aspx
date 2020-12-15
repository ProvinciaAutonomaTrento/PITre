<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RespConservazione.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.RespConservazione" %>

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
    <title>Conservazione - Policy Documenti</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script src="../../LIBRERIE/rubrica.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/javascript">

        function OpenAddressBook() {
            
            var r = new Rubrica();
            
            r.MoreParams = "ajaxPage";
            r.CallType = r.CALLTYPE_RUOLO_RESP_REPERTORI;
            r.CorrType = r.Interni;
            
            var res = r.Apri();
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
    <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Conservazione > Resp. Conservazione" />
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
                Conservazione -> Selezione ruolo responsabile
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
                                <asp:UpdatePanel runat="server" ID="pnlRuoloResp" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="80%">
                                            <tr style="height: 25px; margin: 10px;">
                                                <td style="width: 100%; text-align: center" class="testo">
                                                    <asp:TextBox ID="txtCodRuolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30" OnTextChanged="txtCodRuolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                    <asp:TextBox ID="txtDescRuolo" runat="server" CssClass="testo_grigio" Width="390" MaxLength="30" Enabled="false"></asp:TextBox>
                                                    <cc1:ImageButton ID="btnApriRubrica" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica" OnClick="btnRubricaRuoloResp_Click"
                                                            ImageUrl="../../images/proto/rubrica.gif" runat="server" Height="20px" DisabledUrl="../../images/proto/rubrica.gif"></cc1:ImageButton>      
                                                    <asp:HiddenField ID="id_corr" runat="server" />
                                                </td>
                                            </tr>
                                            <tr style="height: 25px;">
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100%; text-align: center" class="testo">
                                                    <asp:Label runat="server" Text="Responsabile versamento: "></asp:Label>
                                                    <asp:DropDownList runat="server" ID="ddl_user" Enabled="false" CssClass="testo" Width="200px"></asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div id="button_new">
                                    <asp:Button ID="btn_salva_resp_cons" runat="server" CssClass="cbtn" Text="Salva" OnClick="btnSalvaRespCons_Click"/>
                                </div>
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
