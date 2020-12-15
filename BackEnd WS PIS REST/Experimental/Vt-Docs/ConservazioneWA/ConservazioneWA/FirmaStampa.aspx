<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirmaStampa.aspx.cs" Inherits="ConservazioneWA.FirmaStampa" %>

<%@ Register Src="SmartClient/DigitalSignWrapper.ascx" TagName="DigitalSignWrapper" TagPrefix="uc1" %>
<%@ Register Src="ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Firma registro di conservazione</title>
    <base target="_self" />
    <script src="Script/jquery-1.10.2.min.js"
        type="text/javascript"></script>
    <link href="CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        #header
        {
            background: url(Img/bg_header.png) repeat-x scroll;
        }
        
        #menutop
        {
            background: url(Img/bg_menutop.png) repeat-x scroll;
        }
        
        .altro a:link
        {
            background-image: url('Img/bg_menutop_no_hover.png');
        }
        
        .altro a:visited
        {
            background-image: url('Img/bg_menutop_no_hover.png');
        }
        
        .altro a:hover
        {
            background-image: url('Img/bg_menutop_hover.png');
        }
        
        .sonoqui a:link
        {
            background-image: url('Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:visited
        {
            background-image: url('Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:hover
        {
            background-image: url('Img/sono_qui_hover.png');
        }
        
        .cbtn
        {
            background-image: url('Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('Img/bg_button_hover.jpg');
        }
        
        .tab_istanze_header
        {
            background-image: url('Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('Img/bg_content.jpg');
        }
        
        .menu_pager_grigio
        {
            background-image: url('Img/bg_pager_table.jpg');
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

        function ApplySign(tipoFirma) 
        {
            if (SignDocument(tipoFirma) == true)
                CloseWindow('reload');
        }

        function DialogDownload() {

            //percorso locale di salvataggio
            var filePath = window.showModalDialog("Popup/SalvaStampaConservazione.aspx?idDocumento=" + frmDialogFirmaDigitale.hd_iddocumento.value, "", "dialogWidth:425px;dialogHeight:140px;status:no;resizable:no;scroll:no;center:yes;help:no");

            if ((filePath != 'annulla') && (filePath.slice(-4) == '.p7m')) {

                //richiesta http per contenuto file
                var http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", "VisualizzaDocumento.aspx?idDocumento=" + frmDialogFirmaDigitale.hd_iddocumento.value + "&indiceAllegato=0&downloadAsAttachment=true", false);
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

            }
        }

        function SignDocument(tipoFirma) {
            var retValue = true;
            iFrameSignedDoc.location = 'VisualizzaDocumento.aspx?idDocumento=' + frmDialogFirmaDigitale.hd_iddocumento.value + '&indiceAllegato=0';

            var indexCert = document.getElementById('lstListaCertificati').selectedIndex;

            if (isNaN(indexCert) || indexCert == -1) {
                alert("Nessun certificato selezionato");
                retValue = false;
            }
            else {
                var selectedValue = document.getElementById('lstListaCertificati').options[indexCert].Value;

                var http = new ActiveXObject("MSXML2.XMLHTTP")
                http.Open("POST", "VisualizzaDocumento.aspx?idDocumento=" + frmDialogFirmaDigitale.hd_iddocumento.value + "&indiceAllegato=0", false);
                http.send();

                var content = http.responseBody;

                // Applicazione della firma digitale al documento
                var signedValue = DigitalSignWrapper_SignData(content, selectedValue, (tipoFirma == "cosign"), 2, "My");

                retValue = (signedValue != null);

                if (retValue) {
                    http.Open("POST", "SalvaSignStampaConservazione.aspx" + "?tipofirma=" + tipoFirma + "&idDocumento=" + frmDialogFirmaDigitale.hd_iddocumento.value, false);
                    http.send(signedValue);

                    if (http.status != 0 && http.status != 200) {
                        alert("Errore durante il salvataggio del documento firmato.\n" + http.statusText + "\n" + http.responseText);
                        retValue = false;
                    }
                    else {
                        alert("Firma avvenuta con successo.");
                    }
                }
            }

            return retValue;
        }
 
        function FetchListaCertificati() 
        {
            var list = DigitalSignWrapper_GetCertificateList(2, "MY");

            var e = new Enumerator(list);
            var i = 1;
            for (; !e.atEnd(); e.moveNext()) {
                var cert = e.item();

                var option = document.createElement("OPTION");

                document.getElementById('lstListaCertificati').options.add(option);

                props = cert.SubjectName.split(",");

                for (j = 0; j < props.length; j++) {
                    if (props[j].substr(0, 1) == " ")
                        props[j] = props[j].substr(1);
                    if (props[j].substr(0, 3) == "CN=")
                        option.innerText = props[j].substr(3);
                }

                option.Value = i;
                i++;
            }

        }


        function ShowDocument() 
        {
            iFrameSignedDoc.location = 'VisualizzaDocumento.aspx?idDocumento=' + frmDialogFirmaDigitale.hd_iddocumento.value + '&indiceAllegato=0';
            return true;
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

        function CloseWindow(retValue) 
        {
            window.returnValue = retValue;
            window.close();
            if (window.opener && !window.opener.closed) {
                window.opener.location.reload();
            }
        }

    </script>
</head>
<body onload="ShowDocument();" style="background-color: #f2f2f2">
    <form id="frmDialogFirmaDigitale" method="post" runat="server">
    <input type="hidden" runat="server" id="hd_iddocumento" name="hd_iddocumento" />
    <uc1:DigitalSignWrapper ID="digitalSignWrapper" runat="server" />
    <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
    <table cellspacing="2" cellpadding="0" width="100%" align="center" border="0">
        <tr id="rowListaCertificati" runat="server">
            <td align="center" class="pulsanti">
                <asp:Label ID="lblListaCertificati" runat="server" CssClass="testo_grigio_scuro">Lista certificati</asp:Label>
            </td>    
        </tr>
        <tr>
            <td>
                <select language="javascript" class="testo_grigio" id="lstListaCertificati" size="9"
                    name="selectCert" runat="server" style="width: 100%; height: 100px; overflow-x: auto;
                    overflow-y: auto;">
                </select>
            </td>
        </tr>
        <tr>
            <td align="center" class="pulsanti">
                <asp:Label ID="lb_intestazione_xml" runat="server" CssClass="testo_grigio_scuro" 
                    Text="File di stampa del registro"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                 <iframe id="iFrameSignedDoc" runat="server" name="iFrameSignedDoc" marginwidth="0"
                 marginheight="0" scrolling="auto" width="100%" height="480px" borderstyle="None"
                 borderwidth="0px" style="margin-bottom: 10px;" ></iframe>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnApplicaFirma" runat="server" Text="Applica firma" CssClass="cbtn">
                </asp:Button>&nbsp;
                <asp:Button ID="btnSalvaDoc" runat="server" Text="Esporta documento" CssClass="cbtn"
                Visible="false" />
                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" CssClass="cbtn"></asp:Button>                
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
