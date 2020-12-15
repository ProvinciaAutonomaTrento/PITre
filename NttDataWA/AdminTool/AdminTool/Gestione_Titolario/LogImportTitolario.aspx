<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogImportTitolario.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Titolario.LogImportTitolario" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">		
	<base target="_self"/>
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Log Import Titolario" />
    <table width="100%" height ="100%">
        <tr>
            <td class="titolo" align="center" bgColor="#e0e0e0" height="20"><asp:label id="lbl_import" runat="server" Width="95%">Log importazione titolario.</asp:label></td>
        </tr>
        <tr>
            <td align="center"><asp:TextBox id="txt_area" TextMode="MultiLine" Width="95%" Height="400px" Wrap="true" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td align="center"><asp:Button ID="Button1" runat="server" Text="Chiudi" CssClass="testo_btn" Width="110px" OnClick="Button1_Click"/></td>
        </tr>
    </table>
    </form>
</body>
</html>
