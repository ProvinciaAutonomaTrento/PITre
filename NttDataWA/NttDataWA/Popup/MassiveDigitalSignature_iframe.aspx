<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MassiveDigitalSignature_iframe.aspx.cs" Inherits="NttDataWA.Popup.MassiveDigitalSignature_iframe" %>
<%@ Register Src="../SmartClient/DigitalSignWrapper.ascx" TagName="CapicomWrapper" TagPrefix="uc2" %>
<%@ Register Src="../AcrobatIntegration/ClientController.ascx" TagName="ClientController" TagPrefix="uc1" %>
<html>
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/json2.js" type="text/javascript"></script>
    <script type="text/javascript">
        //function FetchListaCertificati() {
        //    try {
        //        var list = CapicomWrapper_GetCertificateList(2, "MY");
        //        var e = new Enumerator(list);
        //        var i = 1;
        //        for (; !e.atEnd(); e.moveNext()) {
        //            var cert = e.item();

        //            var option = document.createElement("OPTION");
        //            parent.document.getElementById('lstListaCertificati').options.add(option);
        //            props = cert.SubjectName.split(",");

        //            for (j = 0; j < props.length; j++) {
        //                if (props[j].substr(0, 1) == " ")
        //                    props[j] = props[j].substr(1);
        //                if (props[j].substr(0, 3) == "CN=")
        //                    option.innerText = props[j].substr(3);
        //            }
        //            option.Value = i;
        //            i++;
        //        }
        //    }
        //    catch (err) {
                
        //    }
        //}


        function FetchListaCertificati() {
            var list = CapicomWrapper_GetCertificateList(2, "MY");
            if (list.length > 0) {
                var i = 1;
                var expired = '<%= lblExpired %>'
                var optionText;
                var optionValue;
                var option;
                var cert;
                var e = new Enumerator(list);
                var i = 1;
                for (; !e.atEnd() ; e.moveNext()) {
                    var cert = e.item();
                    //console.log('CERTIFICATO', cert);
                    props = cert.SubjectName.split(",");

                    for (j = 0; j < props.length; j++) {
                        if (props[j].substr(0, 1) == " ")
                            props[j] = props[j].substr(1);

                        if (props[j].substr(0, 3) == "CN=") {
                            optionText = props[j].substr(3);
                        }
                    }
                    optionValue = i;
                    i++;

                    var onSuccess = function (response) {
                        try{
                            var revocationDate = '';
                            var revocate = false;
                            option = document.createElement("OPTION");
                            parent.document.getElementById('lstListaCertificati').options.add(option);
                            if (response.revocationStatus == -1) {
                                alert('Non è stato possibile controllare la firma digitale.');
                                option.Value = optionValue;
                                option.innerText = optionText;
                                option.style.color = 'black';
                            } else {
                                if (response.revocationDate && (response.revocationStatus == 1 || response.revocationStatus == 4)) {
                                    revocationDate = response.revocationDate;
                                    revocate = true;
                                }
                                if (revocate) {
                                    optionText += ' ' + expired + ': ' + revocationDate;

                                    option.Value = optionValue;
                                    option.innerText = optionText;
                                    option.style.color = 'red';
                                } else {

                                    option.Value = optionValue;
                                    option.innerText = optionText;
                                    option.style.color = 'black';
                                }
                            }
                        }catch(ex){
                            alert('Non è stato possibile controllare la firma digitale.');
                            if (option) {
                                option.Value = optionValue;
                                option.innerText = optionText;
                                option.style.color = 'black';
                            }
                        }
                    };

                    var onError = function (response) {
                        try {
                            option = document.createElement("OPTION");
                            parent.document.getElementById('lstListaCertificati').options.add(option);

                            alert('Non è stato possibile controllare la firma digitale.');
                            option.Value = optionValue;
                            option.innerText = optionText;
                            option.style.color = 'black';
                        } catch (ex) {
                            //console.log('Errore controllo firma', ex);
                            alert('Errore nella verifica del certificato');
                        }
                    }

                    $.ajax({
                        type: "POST",
                        url: "../Utils/ValidateCertificateHandler.ashx",
                        data: JSON.stringify(cert),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: onSuccess,
                        error: onError,
                        async: false
                    });
                };
            }
            else {
                alert("Nessuna firma valida trovata.");
            }
        }

        function SignDocument(tipoFirma, idDocumento, conv, pades) {
            var docStatus = true;
            var docStatusDescription = "";

            var temp = parent.document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
            }
            else {
                var selectedValue = temp.options[indexCert].Value;

                if (idDocumento != null && idDocumento != "") {
                    // Nel caso in cui è stato fornito l'id di un documento, 
                    // modifica lo stato di sessione del sistema
                    //var http = new ActiveXObject("Msxml2.XMLHTTP.4.0");
                    var http = new ActiveXObject("MSXML2.XMLHTTP");
                    http.Open("POST", "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?idDocumento=" + idDocumento + "&tipoFirma=" + tipoFirma + "&pades=" + pades, false);

                    http.send();
                    if (http.status != 200) {
                        // Si è verificato un errore, reperimento del messaggio
                        docStatus = false;
                        docStatusDescription = http.responseText;
                    }
                }

                var http1 = new ActiveXObject("MSXML2.XMLHTTP");
                var url = "../SmartClient/SignedRecordViewer.aspx?idDocumento=" + idDocumento;

                if (docStatus) {
                    http1.Open("POST", url, false);
                    http1.send();
                    //iFrameSignedDoc.location = url;

                    var content = null;
                    var signedAsPdf = false;

                    var convLoc = parent.ConvertLocally();
                    var convCentr = parent.ConvertCentrally();

                    if (conv=="False") {
                        convLoc = false;
                        convCentr = false;
                        signedAsPdf = false;
                    }

                    // Conversione del file da firmare in pdf
                    // Se il sistema è configurato per convertire il documento in pdf prima
                    // della firma...
                    if (convCentr || convLoc) {
                        // ...se l'utente vuole convertire il documento si procede con la conversione
                        // prefirma
                        var fileFormat = "<%=GetFileExtension()%>";

                        if (fileFormat.indexOf(".p7m") == -1 && fileFormat.indexOf(".pdf") == -1) {
                            // Se è richiesta conversione pdf prefirma...
                            if (parent.document.getElementById("chkConverti").checked) {
                                // ...se è richiesta la conversione centrale...
                                if (convCentr) {
                                    // ...si procede con la  conversione sincrona
                                    var sincrona = new ActiveXObject("MSXML2.XMLHTTP");
                                    sincrona.Open("POST", "../DigitalSignature/ConvPDFSincrona.aspx", false);
                                    sincrona.send();

                                    if (sincrona.status == 200)
                                        content = sincrona.responseBody;
                                }
                                else {
                                    // ...altrimenti si procede con la conversione locale
                                    content = ConvertPdfStream(http1.responseBody, fileFormat, false);
                                }

                                signedAsPdf = (content != null);

                                if (!signedAsPdf) {
                                    docStatusDescription = "Non e' stato possibile convertire il file in formato PDF.\n" +
					                                       "Il file verra' firmato nel suo formato originale.";
                                }
                            }
                        }

                        if (content == null)
                            content = http1.responseBody;
                    }
                    else {
                        content = http1.responseBody;
                    }


                    // Applicazione della firma digitale al documento
                    var signedValue = CapicomWrapper_SignData(content, selectedValue, (tipoFirma == "cosign"), 2, "My");

                    if (signedValue == null) {
                        docStatus = false;
                        docStatusDescription = "Errore nella firma digitale del documento";
                    }
                    else {
                        if (!convLoc && !convCentr) {
                            signedAsPdf = false;
                        }
                       
                        var httpSave = new ActiveXObject("MSXML2.XMLHTTP");
                        httpSave.Open("POST", "../SmartClient/SaveSignedFile.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf, false);
                        httpSave.send(signedValue);

                        if (httpSave.status != 0 && httpSave.status != 200) {
                            docStatus = false;
                            docStatusDescription = "Errore durante l\'invio del documento firmato.\n" + httpSave.statusText + "\n" + httpSave.responseText;
                        }
                    }
                }
            }

            // Invio informazioni sullo stato della firma
            var http2 = new ActiveXObject("MSXML2.XMLHTTP");
            http2.Open("POST", "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento, false);
            http2.send();

            return (docStatus == 0);
        }

        function SignHash(tipoFirma, idDocumento, conv, pades) {
            var docStatus = true;
            var docStatusDescription = "";
            var temp = parent.document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;           

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
            }
            else {
                var selectedValue = temp.options[indexCert].Value;
                if (idDocumento != null && idDocumento != "") {
                    // Nel caso in cui è stato fornito l'id di un documento, 
                    // modifica lo stato di sessione del sistema
                    //var http = new ActiveXObject("Msxml2.XMLHTTP.4.0");
                    var http = new ActiveXObject("MSXML2.XMLHTTP");
                    http.Open("POST", "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento + "&tipoFirma=" + tipoFirma + "&pades=" + pades, false);

                    http.send();
                    if (http.status != 200) {
                        // Si è verificato un errore, reperimento del messaggio
                        docStatus = false;
                        docStatusDescription = http.http.statusText;

                        //docStatusDescription = "Formato file non supportato per la firma.";
                    }
                }

                if (docStatus) {
                    var content = null;
                    var url = "../SmartClient/SignedRecordViewer.aspx?idDocumento=" + idDocumento + "&isHash=true";

                    var http = new ActiveXObject("MSXML2.XMLHTTP");
                    http.Open("POST", url, false);
                    http.send();

                    if (http.status != 0 && http.status != 200) {
                        docStatus = false;
                        docStatusDescription = "Errore durante il calcolo hash.\n" + httpSave.statusText + "\n" + httpSave.responseText;
                    } else {
                        content = http.responseBody;
                    }
                    // Applicazione della firma digitale al documento
                    var signedValue = CapicomWrapper_SignHash(content, selectedValue, (tipoFirma == "cosign"), 2, "My");

                    if (signedValue == null) {
                        docStatus = false;
                        docStatusDescription = "Errore nella firma digitale del documento";
                    }
                    else {
                        var httpSave = new ActiveXObject("MSXML2.XMLHTTP");
                        httpSave.Open("POST", "../SmartClient/SaveSignedHashFile.aspx" + "?isPades=" + pades + "&idDocumento=" + idDocumento, false);
                        httpSave.send(signedValue);

                        if (httpSave.status != 0 && httpSave.status != 200) {
                            docStatus = false;
                            docStatusDescription = "Errore durante l\'invio del documento firmato.\n" + httpSave.statusText + "\n" + httpSave.responseText;
                        }
                    }
                    
                }
            }

            // Invio informazioni sullo stato della firma
            var http2 = new ActiveXObject("MSXML2.XMLHTTP");
            http2.Open("POST", "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento, false);
            http2.send();

            return (docStatus == 0);
        }
    </script>
</head>
<body >
    <uc1:ClientController ID="AcrobatClientController" runat="server" />
    <uc2:CapicomWrapper ID="CapicomWrapper" runat="server" />
    <form id="form1" runat="server">
    <div>
    <script type="text/javascript">
        $(function () {
            FetchListaCertificati();
        });
    </script>
    </div>
    </form>
</body>
</html>
