<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="annullaContatore.aspx.cs" Inherits="DocsPAWA.popup.annullaContatore" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head runat="server">
    <title></title>
   <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
   <meta content="C#" name="CODE_LANGUAGE">
   <meta content="JavaScript" name="vs_defaultClientScript">
   <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
   <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">   
   <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Annulla contatore" />
    
    <table class="info" width="95%" align="center" border="0">
        <tr>
            <td class="item_editbox">
                <p class="boxform_item"><asp:Label ID="Label" runat="server">Annullamento</asp:Label></p>
            </td>
        </tr>
        <tr>
            <td height="5"></td>
        </tr>
        <tr>
            <td class="titolo_scheda" style="padding-left:10px;">
                <asp:Label ID="lbl_messageCheckOut" Height="15px" runat="server" CssClass="titolo_scheda">Impossibile annullare il contatore di repertorio</asp:Label>
            </td>
        </tr>
        <tr>
            <td style="padding-left:12px; padding-top:0px;>
                <asp:Label ID="lbl_messageCheckOut_descrizione" runat="server" CssClass="testo_grigio" Width="98%"></asp:Label>
            </td>
        </tr>

        <tr>
            <td class="titolo_scheda" style="padding-left:10px;">
                <asp:Label ID="lblAnnullamento" Height="20px" runat="server" CssClass="titolo_scheda">Motivo dell'annullamento&nbsp;*&nbsp;</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txt_note_annullamento" runat="server" CssClass="testo_grigio" Height="45px" Columns="1" Rows="3" MaxLength="250" Width="98%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td valign="middle" align="center" style="height:30px">
                <asp:Button ID="btn_ok" runat="server" CssClass="pulsante" Text="Ok" Width="80px" onclick="btn_ok_Click"></asp:Button>&nbsp;
                <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante" Text="Chiudi" Width="80px" OnClientClick="window.close();"></asp:Button>
            </td>
        </tr>
        </table>
          
    </form>
</body>
</html>
