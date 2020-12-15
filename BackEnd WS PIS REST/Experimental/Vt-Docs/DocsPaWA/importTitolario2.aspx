<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="importTitolario2.aspx.cs" Inherits="DocsPAWA.importTitolario2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Import Titolario2</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="CSS/DocsPA_30.css" type="text/css" rel="stylesheet">		
</head>
<body bgColor="#ffffff" MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
    <table width="100%" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
        <tr>
            <td class="titolo" align="center"><h3>Importazione titolario - DocsPa</h3></td> 
        </tr>        
    </table>
    <br />
    <table width="100%" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
        <tr>
            <td colspan="2" align="center"><br />
                <asp:Label ID="lbl_errore" runat="server" Text="" ForeColor="Red" Width="100%"></asp:Label></td>
        </tr>
        <tr>
            <td style="width:20%;"><br />
                &nbsp;&nbsp;<asp:Label ID="lbl_Path" runat="server" Text="Path File * :" Font-Bold="True" ForeColor="#404040"></asp:Label></td>
            <td style="width:80%"><br />
                <asp:TextBox ID="txt_Path" CssClass="testo_grigio" Width="300px" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><br /></td>
            <td><br /><asp:Button ID="btn_import" runat="server" Text="Importa" OnClick="btn_import_Click" Width="80px" />
                &nbsp;<asp:Button ID="btn_log" runat="server" OnClick="bnt_log_Click" Text="Log" Width="80px" /></td>
        </tr>        
    </table>
    <br />
    <asp:Panel ID="pnl_log" runat="server" Visible = "false" Width="100%">
        <table width="100%" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
            <tr>
                <td>
                    <asp:TextBox ID="txt_log" runat="server" Height="350px" TextMode="MultiLine" Width="100%"></asp:TextBox></td>
            </tr>
        </table>
    </asp:Panel>    
    </form>
</body>
</html>
