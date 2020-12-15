<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_News.News" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <META HTTP-EQUIV="Expires" CONTENT="-1">	
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" bgcolor="#f6f4f4" height="100%"  >
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione News" />
         <uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
        <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td>
                    <!-- TESTATA CON MENU' -->
                    <uc1:testata id="Testata" runat="server"></uc1:testata>
                </td>
            </tr>
            <tr><td height="10px"></td></tr>
            <tr>
                <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                    <asp:label id="lbl_position" runat="server"></asp:label>
                </td>
            </tr>
            <tr><td height="10px"></td></tr>
            <tr>
                <!-- TITOLO PAGINA -->
                <td class="titolo" style="height: 20px" align="center" bgcolor="#e0e0e0" height="34">Gestione News</td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#f6f4f4" height="100%" width="100%">
                    <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                    <table cellspacing="0" cellpadding="0" align="center" border="0">
                        <tr>
                            <td align="center"><asp:Label ID="risultato_mod" runat="server" Visible="false" CssClass="testo_rosso"></asp:Label></td>
                        </tr>
                    
                        <tr>
                            <td align="center">
                            <table width="70%" border="0"><tr><td width="70%" align="center">
                                <asp:TextBox ID="news" runat="server" CssClass="testo_grigio_scuro" Width="300px"></asp:TextBox>
                                </td>
                                <td width="10%" align="center">
                                <asp:CheckBox ID="enable_news" runat="server" Text="Abilitato" CssClass="testo_grigio_scuro" />
                                </td>
                                <td width="20%" align="center">
                                <asp:Button ID="btn_refresh" runat="server" Text="Aggiorna" CssClass="testo_btn" OnClick="Aggiorna" />
                            </td></tr></table>
                            </td>
                        </tr>
                 </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
