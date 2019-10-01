<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneCampiComuni.aspx.cs" Inherits="DocsPAWA.RicercaCampiComuni.GestioneCampiComuni" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<head runat="server">
    <title></title>
</head>
<body MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Ricerche Campi Comuni" />
        <table cellpadding="0" cellspacing="0" width="100%" border="0" height="100%">
            <tr valign="top" height="100%">
                <td valign="top" width="25%" height="100%">
                    <cf1:IFrameWebControl id="iFrame_sx" runat="server" Marginwidth="0" Marginheight="2" iWidth="415" iHeight="100%" Frameborder="0" Scrolling="no" NavigateTo="RicercaCampiComuni.aspx" Width="415px"></cf1:IFrameWebControl>
				</td>
                <td valign="top" width="75%" height="100%">
                    <cf1:IFrameWebControl id="iFrame_dx" runat="server" Marginwidth="10" Marginheight="2" iWidth="100%" iHeight="100%" Frameborder="0" Scrolling="no" NavigateTo="RisultatoRicercaCampiComuni.aspx"></cf1:IFrameWebControl>
		        </td>
            </tr>
        </table>    
    </form>
</body>
</html>
