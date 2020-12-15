<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="disservizio.aspx.cs" Inherits="DocsPAWA.disservizio" %>
<%@ Register Src="UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <title></title>
    <link href="CSS/DocsPA.css" type="text/css" rel="stylesheet"/>
    <LINK href="AdminTool/CSS/AmmStyle.css" type="text/css" rel="stylesheet"/>
</head>
<body style="background-color:#f6f4f4;margin-top:100px;">
    <script language=javascript>
        function chiudi() {
            window.close();
        }
    </script>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Indisponibilità" />
    <table width="100%" cellSpacing="0" cellPadding="0" border="0" height="100%">
        <tr valign="middle" align="center" >
            <td vAlign="top" align="center" bgColor="#f6f4f4" border="3" >  
                <form id="form" method="post" runat="server">
                <div >
                <table class="contenitore" border="0">
                    <tr>
                        <td width="100%"  >
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td valign="middle" align="middle" height="40" width="600" class="pulsanti" style="background-color:#810d06">
                            <asp:Label ID="Label1" runat="server" class="testo_bianco" Font-Size="12"> Indisponibilità del sistema </asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr valign="middle">
                        <td align="center" class="testo_grigio" >
                            <asp:Label runat="server" ID="lbl_mgserrore" Height="20px" Width="100%"
                                class="testo_grigio_scuro" text="Sistema non disponibile.">Sistema non disponibile.</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="100%" height="100">
                            &nbsp;
                        </td>
                    </tr>
                    <tr valign="top" >
                        <td align="middle" class="testo_grigio" valign="center" height="30">
                            <input class="pulsante" type="button" value="Chiudi" onclick="chiudi();"/>
                        </td>
                    </tr>
                    <tr>
                        <td width="100%" >
                            &nbsp;
                        </td>
                    </tr>
                </table>
                </div>

                </form>
            </td>
        </tr>
    </table>
</body>
</html>
