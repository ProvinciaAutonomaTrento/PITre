<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsportaLog.aspx.cs" Inherits="ConservazioneWA.PopUp.EsportaLog" %>

<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Esporta Log</title>
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
            background-image: url('Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }

    </style>
    <script type="text/javascript">
        function ShowDocument() {
            iFrameSignedDoc.location = 'docVisualizzaReport.aspx';
            return true;
        }
        function CloseWindow() {
            window.returnValue = null;
            window.close();
        }
        function DialogDownload() {

            var filePath = window.showModalDialog("SalvaLogDialog.aspx?nomedoc=" + form1.hd_nomeDoc.value, "", "dialogWidth:425px;dialogHeight:140px;status:no;resizable:no;scroll:no;center:yes;help:no");
            
            if ((filePath != 'annulla') && (filePath.slice(-4) == '.pdf')) {

                //richiesta http per contenuto file
                var http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", "docVisualizzaReport.aspx", false);
                http.send();
       

                if (http.status != 200 &&
        		    http.statusText != null && http.statusText != "") {
                    throw http.statusText;

                }
                else {
                    var content = http.responseBody;
                    retValue = (content != null);

                    if (retValue) {
                        try {
                            // Salvataggio del file in locale
                            AdoStreamWrapper_SaveBinaryData(filePath, content);
                            alert(filePath + ' salvato correttamente.');
                        }
                        catch (ex) {
                            throw "Impossibile salvare il file '" + filePath + "'.";
                        }

                        retValue = true;
                    }
                }

                CloseWindow();

            }

        }

        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try {
                return new ActiveXObject(objectType);
            }
            catch (ex) {
                alert("Oggetto '" + objectType + "' non istanziato");
            }
        }

    </script>
</head>
<body onload="ShowDocument();" style="background-color: #f2f2f2">
    <form id="form1" runat="server">
    <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
    <input type="hidden" runat="server" id="hd_nomeDoc" name="hd_nomedoc" />
    <div>
    <table cellspacing="2" cellpadding="0" width="100%" align="center" border="0">
        <tr>
            <td>
                <iframe id="iFrameSignedDoc" runat="server" name="iFrameSignedDoc" marginwidth="0"
                     marginheight="0" scrolling="auto" width="100%" height="500px" borderstyle="None"
                    borderwidth="0px" style="margin-bottom: 10px;" ></iframe>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSalva" runat="server" Text="Salva" CssClass="cbtn" Visible="false" />
                <asp:Button ID="btnChiudi" runat="server" Text="Chiudi" CssClass="cbtn" Visible="false" />
            </td>
        </tr>
        </table>
    </div>
    </form>
</body>
</html>
