<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<%@ Page Language="c#" CodeBehind="GestioneRuolo.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.GestioneRuolo" %>

<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>DOCSPA > GestioneRuolo</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">

    <script language="javascript" type="text/javascript">
        function OpenDeleghe()
			{		
				var pageHeight=window.screen.availHeight;
				var pageWidth = window.screen.availWidth;
				window.showModalDialog('Deleghe/GestioneDeleghe.aspx','',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
			}	
    </script>

</head>
<body>
    <form id="gestioneRuolo" method="post" runat="server">
    <table cellpadding="0" cellspacing="0" width="95%" height="100%" border="0" align="center">
        <tr valign="top">
            <td height="125">
                <cf1:IFrameWebControl ID="iFrame_sx" runat="server" Marginwidth="0" Marginheight="0"
                    iWidth="100%" iHeight="130px" Frameborder="0" Scrolling="auto" Width="100%" Height="130px">
                </cf1:IFrameWebControl>
            </td>
            <td width="0"><cc2:MessageBox Width="0" Height="0" ID="msg_delega" runat="server"></cc2:MessageBox></td>
        </tr>
        <tr valign="top">
            <td height="*" colspan="2">
                <table cellspacing="0" cellpadding="0" width="100%" align="center" border="0" height="100%">
                    <tr>
                        <td>
                            <cf1:IFrameWebControl ID="iFrame_dx" runat="server" Marginwidth="0" Height="100%"
                                Width="100%" Marginheight="0" iWidth="100%" iHeight="100%" Frameborder="0" Scrolling="auto">
                            </cf1:IFrameWebControl>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
