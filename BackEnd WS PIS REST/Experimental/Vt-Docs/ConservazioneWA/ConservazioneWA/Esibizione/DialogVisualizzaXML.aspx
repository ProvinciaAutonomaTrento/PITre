<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogVisualizzaXML.aspx.cs" Inherits="ConservazioneWA.Esibizione.DialogVisualizzaXML" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Visualizza Documento</title>
    <base target="_self" />
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        #header
        {
            background: url(../Img/bg_header.png) repeat-x scroll;
        }
        
        #menutop
        {
            background: url(../Img/bg_menutop.png) repeat-x scroll;
        }
        
        .altro a:link
        {
            background-image: url('../Img/bg_menutop_no_hover.png');
        }
        
        .altro a:visited
        {
            background-image: url('../Img/bg_menutop_no_hover.png');
        }
        
        .altro a:hover
        {
            background-image: url('../Img/bg_menutop_hover.png');
        }
        
        .sonoqui a:link
        {
            background-image: url('../Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:visited
        {
            background-image: url('../Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:hover
        {
            background-image: url('../Img/sono_qui_hover.png');
        }
        
        .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
        }
        
        .tab_istanze_header
        {
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('../Img/bg_content.jpg');
        }
        
        .menu_pager_grigio
        {
            background-image: url('../Img/bg_pager_table.jpg');
            background-repeat: repeat-x;
        }
        
        TD.pulsanti
        {
            background-color: #4885a4;
            color: #ffffff;
            font-size: 11px;
            width: 95%;
            padding: 5px;
            font-weight: bold;
            text-align: center;
            margin-top: 5px;
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
    </style>
    <script type="text/javascript">
        function ApplySign(tipoFirma) {
            if (SignDocument(tipoFirma) == true)
                CloseWindow(true);
        }

        function ShowDocument() {
            iFrameVisualizzaXML.location = 'VisualizzaXMLFromStore.aspx?idistanza=' + frmDialogVisualizzaXML.hd_idIstanza.value + '&idDoc=' + frmDialogVisualizzaXML.hd_idDoc.value + '&type=' + frmDialogVisualizzaXML.hd_type.value;
            return true;
        }

        function CloseWindow(retValue) {
            window.returnValue = retValue;
            window.close();
        }

    </script>
</head>
<body onload="ShowDocument();" style="background-color: #f2f2f2">
    <form id="frmDialogVisualizzaXML" method="post" runat="server">
    <input type="hidden" runat="server" id="hd_idIstanza" name="hd_idIstanza" />
    <input type="hidden" runat="server" id="hd_idDoc" name="hd_idDoc" />
    <input type="hidden" runat="server" id="hd_type" name="hd_type" />
    <table cellspacing="3" cellpadding="0" width="100%" align="center" border="0">
        <tr align="center">
            <td colspan="2">
                <iframe id="iFrameVisualizzaXML" runat="server" name="iFrameVisualizzaXML" marginwidth="0"
                    marginheight="0" scrolling="auto" width="100%" height="500px" borderstyle="None"
                    borderwidth="0px"></iframe>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnChiudi" runat="server" Text="Chiudi" CssClass="cbtn"></asp:Button>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
