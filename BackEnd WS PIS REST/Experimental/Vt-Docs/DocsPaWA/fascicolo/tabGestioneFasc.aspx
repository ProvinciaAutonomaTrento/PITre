<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="tabGestioneFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.tabGestioneFasc" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="javascript">
        ns = window.navigator.appName == "Netscape"
        ie = window.navigator.appName == "Microsoft Internet Explorer"


        function openDesc() {

            try {
                if (ns) {


                    showbox = document.layers[1]
                    showbox.visibility = "show";
                    // showbox.top=63;

                    var items = 1;
                    for (i = 1; i <= items; i++) {
                        elopen = document.layers[i]
                        if (i != (1)) {
                            elopen.visibility = "hide"
                        }
                    }
                }

                if (ie) {
                    curEl = event.toElement
                    // curEl.style.background = "#C08682"   

                    showBox = document.all.descreg;
                    showBox.style.visibility = "visible";

                }
            }
            catch (e)
   { return false; }
        }

        function closeDesc() {






            try {
                var items = 1
                for (i = 0; i < items; i++) {
                    if (ie) {
                        document.all.descreg.style.visibility = "hidden"



                    }
                    if (ns) { document.layers[i].visibility = "hide" }
                }
            }

            catch (e)
   { return false; }
        }













        /// <summary>
        /// Visualizzazione pagina di ricerca
        /// </summary>
        function ShowWaitingPage() {
            top.principale.iFrame_dx.location = '../waitingpage.htm';
        }

  
    </script>
</head>
<body leftmargin="1" topmargin="0" ms_positioning="GridLayout">
    <form id="tab_gestioneFasc" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Gestione Fascicoli" />
    <table cellspacing="0" cellpadding="0" width="415" height="100%">
        <tr>
            <td valign="top" align="center">
                <table class="info" cellspacing="0" cellpadding="0" width="99%" border="0">
                    <tr height="22">
                        <td align="left">
                            <asp:Panel ID="pnl_regStato" runat="server">
                                <td class="testo_grigio_scuro" width="50">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registro
                                </td>
                                <td valign="middle">
                                    &nbsp;
                                    <asp:Label ID="lbl_registri" runat="server" CssClass="testo_grigio"></asp:Label>&nbsp;
                                    <asp:Image ID="icoReg" onmouseover="openDesc()" onmouseout="closeDesc()" runat="server"
                                        BorderWidth="0px" BorderColor="Gray" ImageAlign="AbsMiddle" BorderStyle="Solid"
                                        ImageUrl="../images/proto/ico_registro.gif"></asp:Image>
                                </td>
                                <td align="right">
                                    <table cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td class="testo_grigio_scuro">
                                                Stato&nbsp;
                                            </td>
                                            <td valign="middle" align="center" height="1">
                                                <asp:Image ID="img_statoReg" runat="server" BorderWidth="1" ImageUrl="../images/stato_verde.gif"
                                                    Height="18px" Width="52px"></asp:Image><img height="1" src="../images/proto/spaziatore.gif"
                                                        width="2" border="0">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr height="100%">
            <td>
                <table id="tbl_tab" cellspacing="0" cellpadding="0" width="410" border="0" height="100%">
                    <tr valign="bottom" height="17">
                        <td width="1" style="height: 20px">
                        </td>
                        <td width="67" height="20" style="height: 20px">
                            <cc1:ImageButton ID="btn_documenti" runat="server" SkinID="documenti_attivo" DisabledUrl="../images/tab_disattivo.gif"
                                Width="67px" Height="19px" CssClass="menu_1_grigio"></cc1:ImageButton>
                        </td>
                        <td width="1" bgcolor="#ffffff" height="20" style="height: 20px">
                        </td>
                        <td width="67" height="20" style="height: 20px">
                            <cc1:ImageButton ID="btn_trasmissioni" runat="server" SkinID="trasmissioni_attivo"
                                DisabledUrl="../images/tab_disattivo.gif" Tipologia="FASC_TAB_TRASMISSIONI" Width="69px"
                                Height="19px"></cc1:ImageButton>
                        </td>
                        <!--td width="1" bgColor="#ffffff" height="17"></td>
								<td width="67" height="19">
									<cc1:ImageButton id="btn_procedimentali" runat="server" ImageUrl="../images/fasc/procedimentali_nonattivo.gif" DisabledUrl="../images/tab_disattivo.gif" Tipologia="FASC_TAB_PROCED" Width="67px" Height="19px"></cc1:ImageButton></td-->
                        <td width="300" bgcolor="#ffffff" height="20" style="height: 20px">
                        </td>
                        <!--td width="67" height="19"></td>
								<td width="1" bgColor="#ffffff" height="17"></td>
								<td width="67" height="19"></td>
								<td width="1" bgColor="#ffffff" height="17"></td>
								<td width="67" height="19"></td>
								<td width="1" bgColor="#ffffff" height="17"></td-->
                    </tr>
                     <tr valign="top" align="center">
                        <td valign="top" align="center" width="415" colspan="12">
                            <cc1:IFrameWebControl ID="IframeTabs" runat="server" iHeight="100%" iWidth="415" Align="center"
                                Marginheight="0" Marginwidth="0" Scrolling="auto" Frameborder="0" BorderWidth="0px">
                            </cc1:IFrameWebControl>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>


