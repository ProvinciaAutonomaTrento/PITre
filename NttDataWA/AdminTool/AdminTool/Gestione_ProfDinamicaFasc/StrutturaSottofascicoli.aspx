<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StrutturaSottofascicoli.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc.StrutturaSottofascicoli"
    ValidateRequest="false" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <base target="_self" />
</head>
<body ms_positioning="GridLayout">
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Struttura sottofascicoli" />
    <table width="100%">
        <tr>
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                <asp:Label ID="lbl_titolo" runat="server"></asp:Label>
            </td>
            <td align="center" width="38%" bgcolor="#e0e0e0">
                <asp:Button ID="btn_chiudi" runat="server" Text="Chiudi" CssClass="testo_btn_p" OnClick="btn_chiudi_Click" />&nbsp;
                <asp:Button ID="btn_conferma" runat="server" Text="Conferma" CssClass="testo_btn_p"
                    OnClick="btn_conferma_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="testo_piccoloB" height="40">
                Descrizione&nbsp;&nbsp;
                <asp:TextBox ID="txt_descrizione" CssClass="testo_grigio_scuro_grande" runat="server"
                    Width="340px"></asp:TextBox>&nbsp;
                <asp:Button ID="btn_aggiungi" CssClass="testo_btn" runat="server" ToolTip="Aggiungi sottofascicolo" Text="Aggiungi" OnClick="btn_aggiungi_Click">
                </asp:Button>&nbsp;&nbsp;
                <asp:CheckBox ID="ck_modifica" Checked="false" runat="server" Text="Modifica" TextAlign="Right">
                </asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" colspan="2">
                <mytree:treeview id="trvSottoFasc" runat="server" cssclass="testo_grigio" width="410px"
                    systemimagespath="../images/alberi/left/" name="Treeview1" borderwidth="0px"
                    defaultstyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color: #d9d9d9;"
                    height="100px" borderstyle="Solid" imageurl="../images/alberi/folders/folder_chiusa.gif"
                    selectedimageurl="../images/alberi/folders/folder_piena.gif" expandedimageurl="../images/alberi/folders/folder_aperta.gif"
                    hoverstyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
                    selectedstyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"></mytree:treeview>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
