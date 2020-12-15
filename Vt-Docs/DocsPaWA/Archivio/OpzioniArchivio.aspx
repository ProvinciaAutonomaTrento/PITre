<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpzioniArchivio.aspx.cs"
    Inherits="DocsPAWA.Archivio.OpzioniArchivio" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <link href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">

    <script language="JavaScript">
   	function btn_titolario_onClick(queryString)
		{		
			var retValue=true;
			if (retValue)
				{
					ApriTitolario(queryString,"gestArchivio");
				}
		
			return retValue;
		}
		
	</script>

</head>
<body leftmargin="1" ms_positioning="GridLayout">
    <form id="opzioniArchivio" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Trasferimento in deposito" />
    <table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="408"
        align="center" border="0">
        <tr valign="top" height="70%">
            <td valign="top">
                <table width="100%" height="100%" class="info" cellspacing="1" cellpadding="1" align="center"
                    border="0">
                    <tr height="20px">
                        <td class="item_editbox">
                            <p class="boxform_item">
                                <asp:Label ID="lbl_titolo" CssClass="titolo_scheda" Text="Trasferimento in deposito"
                                    runat="server"></asp:Label></p>
                        </td>
                    </tr>
                    <tr>
                        <td height="5" valign="top">
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:RadioButtonList ID="rb_opzioni" runat="server" Width="408px" CssClass="testo_grigio"
                                AutoPostBack="True">
                                <asp:ListItem Value="serie">Documenti repertoriati</asp:ListItem>
                                <asp:ListItem Value="fascGen">Ricerca fascicolo generale</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr height="5">
                        <td>
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_filtroSerie" runat="server" Visible="false">
                        <tr>
                            <td>
                                <table class="info" width="100%" border="0" align="center">
                                    <tr>
                                        <td height="25" class="titolo_rosso" align="center" valign="top">
                                            RICERCA DOCUMENTI REPERTORIATI
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="25">
                                            &nbsp;&nbsp;Tipologia documento &nbsp;<asp:DropDownList ID="ddl_tipologiaDoc" runat="server"
                                                Width="200px" CssClass="testo_grigio" AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left" valign="top">
                                                        <asp:Panel ID="panel_Contenuto" runat="server">
                                                        </asp:Panel>
                                                        &nbsp;
                                                    </td>
                                                    <td valign="top">
                                                        <asp:Panel ID="pnl_RFAOO" runat="server">
                                                            <asp:Label ID="lblAooRF" CssClass="titolo_scheda" Text="Anno" runat="server"></asp:Label>
                                                            <asp:DropDownList ID="ddlAooRF" runat="server" CssClass="testo_grigio" AutoPostBack="True">
                                                            </asp:DropDownList>
                                                        </asp:Panel>
                                                    </td>
                                                    &nbsp;&nbsp;
                                                    <td class="titolo_scheda" valign="top">
                                                        &nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="lblAnno" CssClass="titolo_scheda" Text="Anno *" runat="server"></asp:Label>
                                                        <asp:TextBox ID="TxtAnno" runat="server" CssClass="testo_grigio" Visible="false"
                                                            Width="40" MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_filtroFascG" runat="server" Visible="false">
                        <tr>
                            <td>
                                <table class="info" width="100%" border="0" align="center">
                                    <tr>
                                        <td colspan="3" height="25" class="titolo_rosso" align="center" valign="top">
                                            RICERCA FASCICOLO GENERALE
                                        </td>
                                    </tr>
                                    <tr height="25" valign="top">
                                        <td class="testo_grigio_scuro" valign="middle">
                                            Registro
                                            <asp:DropDownList ID="ddl_registri" runat="server" CssClass="testo_grigio" Width="134px"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="titolo_scheda" valign="top">
                                            <asp:Label ID="Llb_annoF" CssClass="titolo_scheda" Text="Anno *" runat="server"></asp:Label>&nbsp;&nbsp;
                                            <asp:TextBox ID="tbAnnoProt" runat="server" CssClass="testo_grigio" Width="40px"
                                                MaxLength="4"></asp:TextBox>&nbsp;&nbsp;
                                        </td>
                                        <td valign="top" align="right">
                                            <cc1:ImageButton ID="btn_titolario" ImageUrl="../images/proto/ico_titolario_noattivo.gif"
                                                Height="17px" runat="server" DisabledUrl="../images/proto/ico_titolario_noattivo.gif"
                                                AlternateText="Titolario"></cc1:ImageButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <div style="width: 100%; height: 170;">
                                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                    <tr>
                                                        <td class="testo_grigio_scuro">
                                                            <mytree:TreeView ID="Gerarchia" runat="server" CssClass="testo_grigio" Width="400px"
                                                                SystemImagesPath="../images/alberi/left/" name="Treeview1" BorderWidth="0px"
                                                                DefaultStyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana;background-color: #d9d9d9;"
                                                                Height="170px" BorderStyle="Solid"></mytree:TreeView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr height="100%">
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr height="7%">
            <td>
                <!-- BOTTONIERA -->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" border="0" align="center">
                    <tr>
                        <td valign="top" height="5">
                            <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:ImageButton ID="btn_ricerca" Visible="true" DisabledUrl="../App_Themes/ImgComuni/btn_ricerca_nonAttivo.gif"
                                runat="server" AlternateText="Ricerca" Thema="btn_" SkinID="ricerca_attivo">
                            </cc1:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
