<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selezioneSmistamento.aspx.cs" Inherits="DocsPAWA.smistaDoc.selezioneSmistamento" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<base target="_self" />
<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
    <title>Selezioni smistamento</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table id="content" runat="server" width="100%" style="height:100%" >
        <tr>
            <td>      
                    <IEWC:TREEVIEW id="treeViewSelezioni" runat="server" AutoPostBack="True" 
                        height="540px" width="580px"
														font="verdana" bordercolor="maroon" borderstyle="solid" borderwidth="1px" backcolor="antiquewhite"
														
                        DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;" 
                        onselectedindexchange="treeViewSelezioni_SelectedIndexChange"></IEWC:TREEVIEW>
            </td>
        </tr>
        <tr>
            <td>
            <table id="pulsanti" runat="server" width="100%">
                <tr> 
                    <td align="center">
                        <asp:Button ID="btn_indietro" runat="server" Text="Chiudi" CssClass="pulsante" onclick="btn_indietro_Click" />                              
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
