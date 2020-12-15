<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogFirma.aspx.cs" Inherits="ConservazioneWA.DialogFirma" %>

<%@ Register Src="SmartClient/DigitalSignWrapper.ascx" TagName="DigitalSignWrapper"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Firma documento</title>
    <base target="_self" />
    <link href="CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <script src="<%=Page.ResolveClientUrl("~/Script/jquery-1.10.2.min.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Script/webclientconnector.js") %>" type="text/javascript"></script>
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
        function ApplySign(tipoFirma) {
            if (SignDocument(tipoFirma) == true)
                CloseWindow(true);
        }

        function ShowDocument() {
            iFrameSignedDoc.location = 'VisualizzaDocFirmato.aspx?idistanza=' + frmDialogFirmaDigitale.hd_idIstanza.value;
            return true;
        }

        function CloseWindow(retValue) {
            window.returnValue = retValue;
            window.close();
        }

        function SignDocument(tipoFirma) {
            var retValue = true;
            iFrameSignedDoc.location = 'VisualizzaDocFirmato.aspx?idistanza=' + frmDialogFirmaDigitale.hd_idIstanza.value;

            var indexCert = document.getElementById('lstListaCertificati').selectedIndex;

            if (isNaN(indexCert) || indexCert == -1) {
                alert("Nessun certificato selezionato");
                retValue = false;
            }
            else {
                var selectedValue = document.getElementById('lstListaCertificati').options[indexCert].Value;

                var http = new ActiveXObject("MSXML2.XMLHTTP")
                http.Open("POST", "VisualizzaDocFirmato.aspx?idIstanza=" + frmDialogFirmaDigitale.hd_idIstanza.value, false);
                http.send();

                var content = http.responseBody;

                // Applicazione della firma digitale al documento
                var signedValue = DigitalSignWrapper_SignData(content, selectedValue, (tipoFirma == "cosign"), 2, "My");

                retValue = (signedValue != null);

                if (retValue) {
                    http.Open("POST", "SalvaSignDoc.aspx" + "?tipofirma=" + tipoFirma + "&idIstanza=" + frmDialogFirmaDigitale.hd_idIstanza.value, false);
                    http.send(signedValue);

                    if (http.status != 0 && http.status != 200) {
                        alert("Errore durante l\'invio del documento firmato.\n" + http.statusText + "\n" + http.responseText);
                        retValue = false;
                    }
                    else {
                        alert("Firma avvenuta con successo.");
                    }
                }
            }

            return retValue;
        }

        function FetchListaCertificati() {
            if (frmDialogFirmaDigitale.hd_componentType.value == '0') {
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
            else {
                var storeLocation = "Windows-MY";

                var storeName = "";

                var list = new Array();

                getCertificateListAsJsonFormat(storeLocation, storeName, function (retValue, connection) {
                    var jsonList = eval(retValue);
                    if (jsonList != null) {
                        for (var i = 0; i < jsonList.length; i++) {
                            list[i] = jsonList[i];
                        }
                    }
                    FetchListaCertificatiApplet(list);
                    connection.close();
                });
            }
        }

        function FetchListaCertificatiApplet(list) {
            if (list.length > 0) {

                $.each(list, function () {
                    var cert = this;

                    props = cert.SubjectName.split(",");

                    var optionText;
                    var optionValue;

                    for (j = 0; j < props.length; j++) {
                        if (props[j].substr(0, 1) == " ")
                            props[j] = props[j].substr(1);

                        if (props[j].substr(0, 3) == "CN=") {
                            optionText = props[j].substr(3);
                        }
                    }
                    optionValue = cert.SerialNumber;
                    $('#lstListaCertificati').append('<option value="' + optionValue + '">' + optionText + '</option>');
                });
            }
            else {
                alert("Nessuna firma valida trovata.");
            }
        }

        function ApplySignApplet(tipoFirma) {
            var urlPost = 'VisualizzaDocFirmato.aspx?idistanza=' + frmDialogFirmaDigitale.hd_idIstanza.value + '&applet=1';
            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
                //sendStatus(docStatus, docStatusDescription, idDocumento, callback);
            }
            else {
                

                var selectedValue = temp.options[indexCert].value;

                getSpecialFolder(function (path, connection) {

                    $.ajax({
                        type: 'POST',
                        url: urlPost,
                        data: {},
                        success: function (content, textStatus, jqXHR) {
                            alert("afetr call is success");
                            status = jqXHR.status;
                            
                            //var content = response.responseText;
                            if (status != 200) {
                                signedAsPdf = false;
                                content = null;
                            }

                            if (content) {
                                alert("Call is success");
                                var tempfilePath = path + 'tempsign_' + getUniqueId() + '.xml';

                                saveFile(tempfilePath, content, function (retVal, connection) {
                                    if (retVal == "true") {
                                        status = 200;
                                    }
                                    else {
                                        status = 100;
                                    }
                                    connection.close();
                                    _sign(selectedValue, tempfilePath, content, tipoFirma, null);

                                });
                            }
                        },
                        error: function () {

                        },
                        async: true
                    });

                    connection.close();
                });
            }
        }

        function _sign(selectedValue, tempfilePath, content, tipoFirma, callback) {
            var status;
            var docStatus = true;
            var docStatusDescription = "";

            signDataFromPath(tempfilePath, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "", function (signedValue, connection) {

                if (signedValue == null || signedValue == "KO") {
                    docStatus = false;
                    docStatusDescription = "Errore nella firma digitale del documento.";
                    //sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                }
                else 
                {
                    getFileFromPath(signedValue, "", function (getFile, connection) {
                        var status = 100;
                        var completeUrl = "SalvaSignDoc.aspx" + "?tipofirma=" + tipoFirma + "&idIstanza=" + frmDialogFirmaDigitale.hd_idIstanza.value + "&applet=1";

                        $.ajax({
                            type: 'POST',
                            url: completeUrl,
                            data: 'contentFile=' + getFile,
                            success: function () {
                                status = 200;
                                CloseWindow(true);
                            },
                            error: function () {
                                docStatus = false;
                                docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                                deleteFile(tempfilePath, true);
                            },
                            async: true
                        });
                        //sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                        connection.close();
                    });
                }
                 connection.close();
          });
        }

        function getUniqueId() {
            var dateObject = new Date();
            var uniqueId = dateObject.getFullYear() + '' + dateObject.getMonth() + '' + dateObject.getDate() + '' + dateObject.getTime();

            return uniqueId;
        }
    </script>
</head>
<body onload="FetchListaCertificati(); ShowDocument();" style="background-color: #f2f2f2">
    <form id="frmDialogFirmaDigitale" method="post" runat="server">
    <input type="hidden" runat="server" id="hd_idIstanza" name="hd_idIstanza" />
    <input type="hidden" runat="server" id="hd_componentType" name="hd_componentType" />
    <uc1:DigitalSignWrapper ID="digitalSignWrapper" runat="server" />
    <table cellspacing="2" cellpadding="0" width="100%" align="center" border="0">
        <tr>
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
                    Text="File Xml"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <iframe id="iFrameSignedDoc" runat="server" name="iFrameSignedDoc" marginwidth="0"
                    marginheight="0" scrolling="auto" width="100%" height="340px" borderstyle="None"
                    borderwidth="0px"></iframe>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnApplicaFirma" runat="server" Text="Applica firma" CssClass="cbtn">
                </asp:Button>&nbsp;
                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" CssClass="cbtn"></asp:Button>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
