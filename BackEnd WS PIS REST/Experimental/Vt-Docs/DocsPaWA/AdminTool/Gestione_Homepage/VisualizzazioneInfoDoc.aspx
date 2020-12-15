<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizzazioneInfoDoc.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Homepage.VisualizzazioneInfoDoc" %>

<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptManager1" runat="server">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Visualizza informazioni documento" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <table height="550px" cellSpacing="1" cellPadding="0" width="100%" border="0">
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
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Visualizza informazioni documento
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <table cellspacing="0" cellpadding="0" align="center" border="0" width="60%">
                    <tr>
                        <td height="10">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="contenitore" width="100%">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="0" width="90%">
                                            <tr>
                                                <td class="testo_grigio_scuro" align="left" width="200" colspan="3">
                                                    <div>
                                                        Id documento:&nbsp;
                                                        <asp:TextBox ID="idDocumento" runat="server" Width="100px"></asp:TextBox>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="testo_grigio_scuro" align="left"  colspan="1" width="30%">
                                                    <div>
                                                        Num. protocollo:&nbsp;
                                                        <asp:TextBox ID="numProtocollo" runat="server" Width="50px"></asp:TextBox>
                                                    </div>
                                                </td>
                                                 <td class="testo_grigio_scuro" align="left"  colspan="1" width="30%">
                                                    <div>
                                                            Anno:&nbsp;
                                                        <asp:TextBox ID="anno" runat="server" Width="50px"></asp:TextBox>
                                                    </div>
                                                </td>
                                                <td class="testo_grigio_scuro" align="left" width="30%">
                                                    <div>
                                                        Codice registro:&nbsp;
                                                        <asp:TextBox ID="codiceRegistro" runat="server" Width="50px"></asp:TextBox>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td height="10">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="3">
                                                    <asp:Button ID="btnCerca" Text="Cerca" CssClass="testo_btn" runat="server" OnClick="btnCerca_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" align="left">
                                                    <asp:UpdatePanel ID="UpPnlResult" runat="server" Visible="true" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:Panel ID="pnlResult" Visible="false" runat="server">
                                                                <tr >
                                                                    <td height="15">
                                                                    </td>
                                                                </tr>
                                                                <tr align="left">
                                                                    <td align="left" class="testo_grigio_scuro" width="20%">
                                                                        Segnatura:
                                                                    </td>
                                                                    <td class="testo" class="testo_grigio" width="350px">
                                                                        <asp:Label ID="LblSegnatura" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <asp:Panel ID="PnlTipologia" runat="server">
                                                                    <tr>
                                                                        <td height="5">
                                                                        </td>
                                                                    </tr>
                                                                    <tr align="left">
                                                                        <td align="left" class="testo_grigio_scuro" width="20%">
                                                                            Tipologia:
                                                                        </td>
                                                                        <td class="testo" class="testo_grigio" width="350px">
                                                                            <asp:Label ID="LblTipologia" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </asp:Panel>
                                                                <tr>
                                                                    <td height="5">
                                                                    </td>
                                                                </tr>
                                                                <tr align="left">
                                                                    <td align="left" class="testo_grigio_scuro" width="20%">
                                                                        Struttura protocollatrice:
                                                                    </td>
                                                                    <td class="testo" width="350px">
                                                                        <asp:Label ID="LblUoProtocollazione" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                             
                                                                <tr>
                                                                    <td height="5">
                                                                    </td>
                                                                </tr>
                                                                <tr align="left">
                                                                    <td align="left" class="testo_grigio_scuro" width="20%">
                                                                        Ruolo protocollatore:
                                                                    </td>
                                                                    <td class="testo" class="testo_grigio" width="350px">
                                                                        <asp:Label ID="LblRuoloProtocollatore" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                 <tr>
                                                                    <td height="5">
                                                                    </td>
                                                                </tr>
                                                                <tr align="left">
                                                                    <td align="left" class="testo_grigio_scuro" width="20%">
                                                                        Utente protocollatore:
                                                                    </td>
                                                                    <td class="testo" class="testo_grigio" width="350px">
                                                                        <asp:Label ID="LblUtenteProtocollatore" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td height="15">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="testo_grigio_scuro" colspan="3">
                                                                        <asp:Label ID="LblFascicolo" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="testo" colspan="3">
                                                                        <asp:Label ID="LblFascicoliList" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td height="15">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="testo_grigio_scuro" colspan="3">
                                                                        <asp:Label ID="LblTrasmissione" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="testo" colspan="3">
                                                                        <asp:Label ID="LblTrasmissioniList" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td height="15">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="testo_grigio_scuro" colspan="3">
                                                                        <asp:Label ID="LblSpedizioni" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </asp:Panel>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
