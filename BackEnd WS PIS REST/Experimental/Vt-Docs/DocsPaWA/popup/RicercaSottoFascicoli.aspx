<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaSottoFascicoli.aspx.cs"
    Inherits="DocsPAWA.popup.RicercaSottoFascicoli" EnableEventValidation="true" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>

    <link href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Ricerca Sotto Fascicoli" />
    <div>
        <table class="contenitore" height="100%" width="100%" align="center" border="0">
            <tr valign="top">
                <td valign="top">
                    <table class="info_grigio" cellspacing="3" cellpadding="0" width="95%" align="center"
                        border="0">
                        <tr>
                            <td>
                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                    <tr>
                                        <td class="item_editbox">
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label7" runat="server" class="testo_grigio_scuro">Sotto Fascicoli</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label1" runat="server" class="testo_grigio_scuro">Descrizione:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_descrizione" runat="server" Width="300px" CssClass="testo_grigio"
                                                            BackColor="White"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <cc1:ImageButton ID="btn_cerca" runat="server" DisabledUrl="../images/proto/RispProtocollo/lentePreview.gif"
                                                            ImageUrl="../images/proto/RispProtocollo/lentePreview.gif" OnClick="btn_cerca_Click">
                                                        </cc1:ImageButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" colspan="2">
                                            <mytree:TreeView ID="Folders" runat="server" CssClass="testo_grigio" Width="90%"
                                                SystemImagesPath="../images/alberi/left/" NAME="Treeview1" BorderWidth="0px"
                                                DefaultStyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color: #d9d9d9;"
                                                Height="280px" BorderStyle="Solid" ImageUrl="../images/alberi/folders/folder_chiusa.gif"
                                                SelectedImageUrl="../images/alberi/folders/folder_piena.gif" ExpandedImageUrl="../images/alberi/folders/folder_aperta.gif"
                                                HoverStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
                                                SelectedStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
                                                AutoPostBack="True" OnExpand="Folders_Expand" OnSelectedIndexChange="Folders_SelectedIndexChange"></mytree:TreeView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lbl_msg" runat="server" CssClass="testo_red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Panel ID="pnl_ric" Visible="False" runat="server" Width="100%">
                                                <table class="info_grigio" cellspacing="0" cellpadding="0" width="100%" align="center"
                                                    border="0">
                                                    <tr>
                                                        <td>
                                                            <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
                                                                <tr>
                                                                    <td class="menu_1_rosso" align="center" width="99%">
                                                                        Risultato della ricerca
                                                                    </td>
                                                                    <td width="1%">
                                                                        <asp:ImageButton ID="btn_chiudi_risultato" runat="server" AlternateText="Chiudi risultato ricerca"
                                                                            ImageUrl="../images/chiude.gif" OnClick="btn_chiudi_risultato_Click"></asp:ImageButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div id="DivList" style="overflow: auto; height: 100px">
                                                                <table cellspacing="3" cellpadding="1" width="100%" border="0">
                                                                    <tr bgcolor="#eaeaea">
                                                                        <td class="testo_grigio" align="center">
                                                                            &nbsp;
                                                                        </td>
                                                                        <td class="testo_grigio" align="center">
                                                                            Descrizione
                                                                        </td>
                                                                    </tr>
                                                                    <asp:Label ID="lbl_td" runat="server"></asp:Label>
                                                                </table>
                                                            </div>
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
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btn_ok" runat="server" CssClass="PULSANTE" Text="OK" OnClick="btn_ok_Click"
                                    Height="20px"></asp:Button>
                            </td>
                            <td>
                                <asp:Button ID="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI" OnClick="btn_chiudi_Click">
                                </asp:Button>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
