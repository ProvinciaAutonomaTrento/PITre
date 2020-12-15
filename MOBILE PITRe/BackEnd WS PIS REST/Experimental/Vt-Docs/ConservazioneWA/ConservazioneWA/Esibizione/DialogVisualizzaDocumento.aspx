<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogVisualizzaDocumento.aspx.cs" Inherits="ConservazioneWA.Esibizione.DialogVisualizzaDocumento" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
            iFrameVisualizzaDoc.location = 'VisualizzaDocFromStore.aspx?idistanza=' + frmDialogVisualizzaDocumento.hd_idIstanza.value + '&idDoc=' + frmDialogVisualizzaDocumento.hd_idDocCorrente.value;
            return true;
        }

        function CaricaDoc(idDoc) {
            frmDialogVisualizzaDocumento.hd_idDocCorrente.value = idDoc;
            //iFrameVisualizzaDoc.location = 'VisualizzaDocFromStore.aspx?idistanza=' + frmDialogVisualizzaDocumento.hd_idIstanza.value + '&idDoc=' + idDoc;
            //return true;
        }

        function CloseWindow(retValue) {
            window.returnValue = retValue;
            window.close();
        }

    </script>
</head>
<body onload="ShowDocument();" style="background-color: #f2f2f2">
    <form id="frmDialogVisualizzaDocumento" method="post" runat="server">
    <input type="hidden" runat="server" id="hd_idIstanza" name="hd_idIstanza" />
    <input type="hidden" runat="server" id="hd_idDocPrincipale" name="hd_idDocPrincipale" />
    <input type="hidden" runat="server" id="hd_idDocCorrente" name="hd_idDocCorrente" />
    <table cellspacing="3" cellpadding="0" width="100%" align="center" border="0">
        <tr align="justify">
            <td colspan="2">
                <iframe id="iFrameVisualizzaDoc" runat="server" name="iFrameVisualizzaDoc" marginwidth="0"
                    marginheight="0" scrolling="auto" width="100%" height="500px" borderstyle="None"
                    borderwidth="0px"></iframe>
            </td>
            <td colspan="1" valign="top" runat="server" id="cell_btn" >
                <div runat="server" id="div_allegati" style="overflow-y: auto;">
                    <asp:ImageButton ID="ImgBtn_Principale" runat="server" ImageAlign="Top" BorderWidth="1px" ImageUrl="../Img/doc.gif" ToolTip="Documento principale" /> 
                    <br /> 
                </div>
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
