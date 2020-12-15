<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="areaConservazioneValidation.aspx.cs" Inherits="DocsPAWA.popup.areaConservazioneValidation" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <base target="_self" />
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script type="text/javascript">
        function closeMask(retValue) {
            window.returnValue = retValue;
            window.close();
        }
    </script>
</head>
<body style="height: 100%">
    <form id="frmAreaConservazioneValidation" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Area di Conservazione" />
        <table class="info" width="100%" align="center" border="1" style="height: 100%;">
            <tr>
                <td class="item_editbox" align="center">
                    <asp:TextBox ID="txtReport" runat="server" ReadOnly="true" TextMode="MultiLine" Rows="25" Wrap="false" Width="99%"></asp:TextBox>
                </td>
            </tr>
            <tr style="height: 10%">
                <td class="item_editbox" height="20" align="center">
                    <asp:Button ID="btnClose" runat="server" Text="Chiudi" OnClientClick="closeMask(false)" />
                    <asp:Button ID="btnSend" runat="server" Text="Invia comunque" OnClientClick="closeMask(true)"/>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
