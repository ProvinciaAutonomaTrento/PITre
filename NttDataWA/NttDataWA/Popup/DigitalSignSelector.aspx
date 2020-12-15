<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DigitalSignSelector.aspx.cs" Inherits="NttDataWA.Popup.DigitalSignSelector" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/json2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var applet = undefined;
        var appletFso = undefined;
        var commandType = '<%=CommandType%>';
        var tempfilePath = '';

        function signSmartClient() {
            disallowOp('Content1');
            window.frames['uploadFrame'].ViewResult();
            reallowOp();
            return false;
        }

        function signWithApplet() {
            disallowOp('Content1');
            var docs = "<%=this.GetSelectedDocumentsIds()%>";
            if (docs != null && docs != "") {
                return this.SignDocuments_Applet(docs.split("|"));
            }
            else {
                alert('Nessun documento selezionato per la firma');
                return false;
            }
        }

        function signWithSocket(callback) {
            disallowOp('Content1');
            var docs = "<%=this.GetSelectedDocumentsIds()%>";
            if (docs != null && docs != "") {

                this.SignDocuments(docs.split("|"), callback);
            }
            else {
                alert('Nessun documento selezionato per la firma');
            }
        }

        function openSocket() {
            if (commandType != 'close') {
                try {
                    disallowOp('Content1');

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
                        FetchListaCertificati(list);
                        connection.close();
                    });
                }
                catch (err) {
                    alert("Error:" + err.description);
                    reallowOp();
                }

                reallowOp();
                return false;
            }
            else {
                return true;
            }
        }

        function openApplet() {
            if (commandType != 'close') {
                try {
                    disallowOp('Content1');
                    if (applet == undefined) {
                        applet = window.document.plugins[0];
                    }
                    if (applet == undefined) {
                        applet = document.applets[0];
                    }
                    if (appletFso == undefined) {
                        appletFso = window.document.plugins[1];
                    }
                    if (appletFso == undefined) {
                        appletFso = document.applets[1];
                    }

                    var storeLocation = "Windows-MY";

                    var storeName = "";

                    var list = new Array();

                    if (applet != undefined) {
                        var retValue = applet.getCertificateListAsJsonFormat(storeLocation, storeName);
                        var jsonList = eval(retValue);
                        if (jsonList != null) {
                            for (var i = 0; i < jsonList.length; i++) {
                                list[i] = jsonList[i];
                            }
                        }
                    }
                    else {
                        alert("Impossibile caricare Applet.");
                    }

                    FetchListaCertificati(list);
                }
                catch (err) {
                    alert("Error:" + err.description);
                    reallowOp();
                }

                reallowOp();
                return false;
            }
            else {
                return true;
            }
        }

        function FetchListaCertificati(list) {
            if (list.length > 0) {

                $.each(list, function () {
                    var cert = this;
                    //console.log('Certificato', cert);
                    props = cert.SubjectName.split(",");
                    var expired = '<%= lblExpired %>'
                    var optionText;
                    var optionValue;
                    var option;
                    for (j = 0; j < props.length; j++) {
                        if (props[j].substr(0, 1) == " ")
                            props[j] = props[j].substr(1);

                        if (props[j].substr(0, 3) == "CN=") {
                            optionText = props[j].substr(3);
                        }
                    }

                    optionValue = cert.SerialNumber;
                    //console.log('SerialNumber', cert.SerialNumber);




                    var onSuccess = function (response) {
                        try {
                            var revocationDate = '';
                            var revocate = false;
                            if (response.revocationStatus == -1) {
                                alert('Non è stato possibile controllare la firma digitale.');
                                if (!isMsie()) {
                                    option = new Option(optionText, optionValue);
                                    $('#lstListaCertificati').append(option);
                                    option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                    option.css("color", "black");
                                } else {
                                    option = document.createElement("OPTION");
                                    $('#lstListaCertificati').append(option);
                                    option.value = optionValue;
                                    option.innerText = optionText;
                                    option.style.color = 'black';
                                }
                            } else {
                                if (response.revocationDate && (response.revocationStatus == 1 || response.revocationStatus == 4)) {
                                    revocationDate = response.revocationDate;
                                    revocate = true;
                                }
                                if (revocate) {
                                    optionText += ' - ' + expired + ': ' + revocationDate;

                                    if (!isMsie()) {
                                        option = new Option(optionText, optionValue);
                                        $('#lstListaCertificati').append(option);
                                        option = $("#lstListaCertificati option[value='" + optionValue + "']").attr("disabled", "disabled");
                                        option.css("color", "red");
                                    } else {
                                        option = document.createElement("OPTION");
                                        $('#lstListaCertificati').append(option);
                                        option.value = optionValue;
                                        option.innerText = optionText;
                                        option.style.color = 'red';
                                    }
                                } else {

                                    if (!isMsie()) {
                                        option = new Option(optionText, optionValue);
                                        $('#lstListaCertificati').append(option);
                                        option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                        option.css("color", "black");
                                    } else {
                                        option = document.createElement("OPTION");
                                        $('#lstListaCertificati').append(option);
                                        option.value = optionValue;
                                        option.innerText = optionText;
                                        option.style.color = 'black';
                                    }
                                }
                            }
                        } catch (ex) {
                            alert('Non è stato possibile controllare la firma digitale.');
                            
                           
                            if (!isMsie()) {
                                option = new Option(optionText, optionValue);
                                $('#lstListaCertificati').append(option);
                                option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                option.css("color", "black");
                            } else {
                                option = document.createElement("OPTION");
                                $('#lstListaCertificati').append(option);
                                option.value = optionValue;
                                option.innerText = optionText;
                                option.style.color = 'black';
                            }
                        }
                    };

                    var onError = function (response) {
                        try {
                            alert('Non è stato possibile controllare la firma digitale.');
                            if (!isMsie()) {
                                option = new Option(optionText, optionValue);
                                $('#lstListaCertificati').append(option);
                                option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                option.css("color", "black");
                            } else {
                                option = document.createElement("OPTION");
                                $('#lstListaCertificati').append(option);
                                option.value = optionValue;
                                option.innerText = optionText;
                                option.style.color = 'black';
                            }
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
                        error: onError
                    });
                });
            }
            else {
                alert("Nessuna firma valida trovata.");
            }
        }

        function SignDocuments_Applet(docs) {
            var retValue = false;
            var signType = "<%=this.GetSignType()%>";
            //ABBATANGELI - Nuova gestione Sign/Cosign
            var sigCosig = "cosign";
            var optfirma = document.getElementById("optFirma");

            if (!optfirma) {
                var tipoFirma = document.getElementById("tipoFirmaH");
                optfirma = { checked : (tipoFirma.value == 'true') };
            }

            if (optfirma.checked == true)
            {
                sigCosig = "sign";
            }

            for (k = 0; k < docs.length; k++) 
            {
                if (signType == "1") {
                    return this.SignHash(sigCosig, docs[k]);
                } else {
                    return this.SignDocument(sigCosig, docs[k]);
                }
            }

            return retValue;
        }

        function SignDocument(tipoFirma, idDocumento) {
            var docStatus = true;
            var docStatusDescription = "";

            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;

            var content = null;
            var status = null;

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
            }
            else {
                var selectedValue = temp.options[indexCert].value;
                if (idDocumento != null && idDocumento != "") {
                    var fileFormat = "<%=GetFileExtension()%>";
                    tempfilePath = appletFso.GetSpecialFolder() + 'tempsign_' + getUniqueId() + fileFormat;
                    $.ajax({
                        type: 'POST',
                        url: "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento,
                        success: function (data, textStatus, jqXHR) {
                            status = jqXHR.status;
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            content = textStatus;
                        },
                        async: false
                    });

                    if (status != 200) {
                        docStatus = false;
                        docStatusDescription = content;
                    }
                }

                if (docStatus) {
                    var signedAsPdf = false;
                    var toConvert = false;
                    var convLoc = false; // ConvertLocally();
                    var convCentr = document.getElementById("chkConverti").checked;

                    // Conversione del file da firmare in pdf
                    // Se il sistema è configurato per convertire il documento in pdf prima
                    // della firma...
                    if (convCentr || convLoc) {
                        // ...se l'utente vuole convertire il documento si procede con la conversione
                        // prefirma
                        var fileFormat = "<%=GetFileExtension()%>";
                        tempfilePath = appletFso.GetSpecialFolder() + 'temp__123_firm.' + fileFormat;

                        if (fileFormat.indexOf(".p7m") == -1 && fileFormat.indexOf(".pdf") == -1) {
                            // Se è richiesta conversione pdf prefirma...
                            if (document.getElementById("chkConverti").checked) {
                                // ...se è richiesta la conversione centrale...
                                if (convCentr) {
                                    // ...si procede con la  conversione sincrona
                                    toConvert = true;
                                    var urlPost = '<%=httpFullPath%>' + '/DigitalSignature/ConvPDFSincrona.aspx';
                                    var paramPost = '';

                                    if (appletFso.saveFileFromURL(tempfilePath, urlPost, paramPost)) {
                                        status = 200;
                                        content = true;
                                    } else {
                                        status = 100;
                                    }
                                }
                                else {
                                    // ...conversione locale eliminata
                                    //content = ConvertPdfStream(http.responseBody, fileFormat, false);
                                }

                                if (status != 200) {
                                    signedAsPdf = false;
                                    content = null;
                                }
                                else {
                                    signedAsPdf = (content != null);
                                }
                            }
                        }
                    }

                    if ((!signedAsPdf) && toConvert) {
                        docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
					                                            "Operazione annullata, il file non verrà firmato.";
                        docStatus = false;
                    }
                    else {
                        if (content == null) 
                        {
                            var urlPost = '<%=httpFullPath%>' + '/SmartClient/SignedRecordViewer.aspx';
                            var paramPost = "<<idDocumento:" + idDocumento + ">>";

                            if (appletFso.saveFileFromURL(tempfilePath, urlPost, paramPost)) {
                                status = 200;
                                content = true;
                            } else {
                                status = 100;
                            }
                        }


                        // Applicazione della firma digitale al documento
                        var signedValue = null;
                        if (status == 200 || status == 0) {
                            signedValue = applet.signDataFromPath(tempfilePath, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "");
                            /*signedValue = applet.signData(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "");*/
                        }

                        if (signedValue == null || signedValue == "KO") {
                            docStatus = false;
                            docStatusDescription = "Errore nella firma digitale del documento.";
                        }
                        else {
                            var status = 100;
                            var completeUrl = "<%=httpFullPath%>" + "/SmartClient/SaveSignedFile.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf + "&iscontent=true";
                            if (appletFso.sendFiletoURL(signedValue, completeUrl)) {
                                status = 200;
                                appletFso.deleteFile(signedValue, true);
                            }
                            else {
                                docStatus = false;
                                docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                            }
                        }
                    }
                }
            }

            // Invio informazioni sullo stato della firma
            var status = 100;
            $.ajax({
                type: 'POST',
                url: "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento,
                success: function (data, textStatus, jqXHR) {
                    status = jqXHR.status;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    status = textStatus;
                    content = null;
                },
                async: false
            });
            return (docStatus == 0 || docStatus == 1);
        }

        function SignHash(tipoFirma, idDocumento) {
            var docStatus = true;
            var docStatusDescription = "";

            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;

            var content = null;
            var status = null;
            var isSigned = false;
            var errorText = '';

            var pades = false;
             if (document.getElementById("chkPades"))
                 pades = document.getElementById("chkPades").checked;
             var fileFormat = "<%=GetFileExtension()%>";
             var isSigned = ("<%=IsSigned()%>" === "1");

             if (pades)
                 tipoFirma = 'sign';


            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
            }
            else {
                var selectedValue = temp.options[indexCert].value;
                if (idDocumento != null && idDocumento != "") {
                    var fileFormat = "<%=GetFileExtension()%>";
                    tempfilePath = appletFso.GetSpecialFolder() + 'tempsign_' + getUniqueId() + fileFormat;
                    $.ajax({
                        type: 'POST',
                        url: "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento + "&tipoFirma=" + tipoFirma + "&pades=" + pades,
                        success: function (data, textStatus, jqXHR) {
                            status = jqXHR.status;
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            content = textStatus;
                            if (jqXHR != null)
                                errorText = jqXHR.responseText;
                        },
                        async: false
                    });

                    if (status != 200) {
                        docStatus = false;
                        docStatusDescription = errorText ? errorText : (content ? content : 'Errore generico');
                    }
                }

                if (docStatus) {
                    var signedAsPdf = false;
                    var toConvert = false;
                    var convLoc = false; // ConvertLocally();
                    var convCentr = document.getElementById("chkConverti").checked;

                    // Conversione del file da firmare in pdf
                    // Se il sistema è configurato per convertire il documento in pdf prima
                    // della firma...
                    if (convCentr || convLoc) {
                        // ...se l'utente vuole convertire il documento si procede con la conversione
                        // prefirma
                        var fileFormat = "<%=GetFileExtension()%>";

                        if (fileFormat.indexOf(".p7m") == -1 && fileFormat.indexOf(".pdf") == -1) {
                            // Se è richiesta conversione pdf prefirma...
                            if (document.getElementById("chkConverti").checked) {
                                // ...se è richiesta la conversione centrale...
                                if (convCentr) {
                                    // ...si procede con la  conversione sincrona
                                    toConvert = true;
//ABBATANGELI GIANLUIGI
// ERRORE !!! - E' NECESSARIO AVERE IN RISPOSTA HASH DEL FILE E NON TUTTO IL FILE !!!
// MODIFICARE  ConvPDFSincrona.aspx.cs CON IN PIU' PARAMETRO ISHASH CHE RESTITUISCE IL SOLO HASH DEL FILE CONVERTITO IN PDF !!!
var urlPost = '<%=httpFullPath%>' + '/DigitalSignature/ConvPDFSincrona.aspx';
// FINE ERRORE !!!
                                    var paramPost = '';

                                    if (appletFso.saveFileFromURL(tempfilePath, urlPost, paramPost)) {
                                        status = 200;
                                        content = true;
                                    }
                                    else {
                                        status = 100;
                                    }
                                }
                                else {
                                    // ...conversione locale eliminata
                                    //content = ConvertPdfStream(http.responseBody, fileFormat, false);
                                }

                                if (status != 200) {
                                    signedAsPdf = false;
                                    content = null;
                                }
                                else {
                                    signedAsPdf = (content != null);
                                }
                            }
                        }
                    }

                    if ((!signedAsPdf) && toConvert) {
                        docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
					                                                "Operazione annullata, il file non verrà firmato.";
                        docStatus = false;
                    }
                    else 
                    {
                        if (content == null) 
                        {
                            try {
                                var status = 0;
                                var content = '';
                                $.ajax({
                                    type: 'POST',
                                    cache: false,
                                    processData: false,
                                    url: '<%=httpFullPath%>' + '/SmartClient/SignedRecordViewer.aspx?isHash=true&idDocumento=' + idDocumento,
                                    success: function (data, textStatus, jqXHR) {
                                        status = jqXHR.status;
                                        content = jqXHR.responseText;
                                    },
                                    error: function (jqXHR, textStatus, errorThrown) {
                                        status = textStatus;
                                        content = null;
                                    },
                                    async: false
                                });
                            }
                            catch (e) {
                                alert(e.message.toString());
                                retValue = false;
                            }

                            var signedValue = null;
                            if (content != null && content != '') //if (status == 200 || status == 0) {
                            {
                                signedValue = applet.signHash(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "");
                            }
                            else {
                                alert("Impossibile contattare il ws...");
                            }

                            if (signedValue == null || signedValue == "KO") {
                                docStatus = false;
                                docStatusDescription = "Errore nella firma digitale del documento.";
                            }
                            else {
                                var status = 100;
                                $.ajax({
                                    type: 'POST',
                                    url: "<%=httpFullPath%>" + "/SmartClient/SaveSignedHashFile.aspx?idDocumento=" + idDocumento + "&isPades=" + pades,
                                    data: { 'signedDoc': signedValue },
                                    success: function (data, textStatus, jqXHR) {
                                        status = jqXHR.status;
                                    },
                                    error: function (jqXHR, textStatus, errorThrown) {
                                        content = textStatus;
                                    },
                                    async: false
                                });

                                if (status != 200) {
                                    docStatus = false;
                                    docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                                }
                            }
                        }
                        else {
                            signedValue = applet.signDataFromPath(tempfilePath, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "");

                            if (signedValue == null || signedValue == "KO") {
                                docStatus = false;
                                docStatusDescription = "Errore nella firma digitale del documento.";
                            }
                            else {
                                var status = 100;
                                var completeUrl = "<%=httpFullPath%>" + "/SmartClient/SaveSignedFile.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf + "&iscontent=true";
                                if (appletFso.sendFiletoURL(signedValue, completeUrl)) {
                                    status = 200;
                                    appletFso.deleteFile(signedValue, true);
                                }
                                else {
                                    docStatus = false;
                                    docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                                }
                            }
                        }
                    }
                }
            }

            // Invio informazioni sullo stato della firma
            var status = 100;
            $.ajax({
                type: 'POST',
                url: "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento,
                success: function (data, textStatus, jqXHR) {
                    status = jqXHR.status;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    status = textStatus;
                    content = null;
                },
                async: false
            });
            return (docStatus == 0 || docStatus == 1);
        }

        function SignDocuments(docs, callback) {
            var retValue = false;
            var signType = "<%=this.GetSignType()%>";

            //ABBATANGELI - Nuova gestione Sign/Cosign
            var sigCosig = "cosign"
            var optfirma = document.getElementById("optFirma");

            if (!optfirma) {
                var tipoFirma = document.getElementById("tipoFirmaH");
                optfirma = { checked: (tipoFirma.value == 'true') };
            }

            if (optfirma.checked == true)
            {
                sigCosig = "sign";
            }

            for (k = 0; k < docs.length; k++) {
                Sign(sigCosig, docs[k], callback, (signType == "1"));
            
            }

            return retValue;
        }

        var ajaxCall = function (urlPost, data) {
            var status;
            var content;
            var ret = {};
            $.ajax({
                type: 'POST',
                url: urlPost,
                data: data,
                success: function (data, textStatus, jqXHR) {
                    ret.status = jqXHR.status;
                    ret.content = jqXHR.responseText;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    ret.status = textStatus;
                    ret.content = null;
                    if (jqXHR != null)
                        ret.errorText = jqXHR.responseText;
                },
                async: false
            });
            return ret;
        };

        var sendStatus = function (docStatus, docStatusDescription, idDocumento, callback) {

            ajaxCall("../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento);
            reallowOp();
            if(callback)
                callback();

        };

        var _signHash = function (selectedValue, content, recoveryPath, tipoFirma, pades, idDocumento, callback) {
            var status;
            var urlPost;
            var statusURL;
            var docStatus = true;
            var docStatusDescription = "";

            try {
                status = 0;
                content = '';
                urlPost = recoveryPath + '&isHash=true';
                statusURL = ajaxCall(urlPost);
                status = statusURL.status;
                content = statusURL.content;
            }
            catch (e) {
                alert(e.message.toString());
                retValue = false;
            }

            if (content != null && content != '') {
                signHash(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "", function (signedValue, connection) {
                    if (signedValue == null || signedValue == "KO") {
                        docStatus = false;
                        docStatusDescription = "Errore nella firma digitale del documento.";
                    }
                    else {
                        var status = 100;
                        var urlPost = "../SmartClient/SaveSignedHashFile.aspx?isPades=" + pades + "&idDocumento=" + idDocumento;
                        var data = { 'signedDoc': signedValue };
                        var statusURL = ajaxCall(urlPost, data);
                        status = statusURL.status;
                        content = statusURL.content;
                        if (status != 200) {
                            docStatus = false;
                            docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                        }
                    }
                    sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                    connection.close();
                });
            }
            else {
                alert("Impossibile contattare il ws...");
                callback(false);
            }

        };

        var _signDocument = function (selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback) {
            var status;
            var urlPost;
            var statusURL;
            var docStatus = true;
            var docStatusDescription = "";

            signDataFromPath(tempfilePath, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "", function (signedValue, connection) {

                if (signedValue == null || signedValue == "KO") {
                    docStatus = false;
                    docStatusDescription = "Errore nella firma digitale del documento.";
                    sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                }
                else {
                    var status = 100;
                    var completeUrl = "../SmartClient/SaveSignedFile.aspx" + "?tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf + "&iscontent=true" + "&issocket=true" + "&idDocumento=" + idDocumento;
                    getFileFromPath(signedValue, completeUrl, function (getFile, connection) {
                        //alert(getFile);

                        function sendError() {
                            alert('<asp:Literal id="litSendError" runat="server" />');
                        }

                        $.ajax({
                            type: 'POST',
                            url: completeUrl,
                            data: 'contentFile=' + getFile,
                            success: function () {
                                status = 200;
                                sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                            },
                            error: function () {
                                docStatus = false;
                                docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                                deleteFile(signedValue, true);
                                sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                            },
                            async: true
                        });

                        connection.close();
                    });
                }
                connection.close();
            });
        };

        var _sign = function (selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash, signedAsPdf) {
            var status;
            var urlPost;
            var statusURL;
            var docStatus = true;
            var docStatusDescription = "";
            var recoveryPath = '../SmartClient/SignedRecordViewer.aspx?idDocumento=' + idDocumento;
            if (sighHash) {
                _signHash(selectedValue, content, recoveryPath, tipoFirma, pades, idDocumento, callback);
            } else {

                if (content && content !== '') {
                    _signDocument(selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback);
                } else {
                    try {
                        status = 0;
                        content = '';
                        urlPost = recoveryPath + '&type=applet';
                        statusURL = ajaxCall(urlPost);
                        status = statusURL.status;
                        content = statusURL.content;
                    }
                    catch (e) {
                        alert(e.message.toString());
                        retValue = false;
                    }
                    saveFile(tempfilePath, content, function (retVal, connection) {
                        //alert("SaveFileNoApplet 1 retVal" + retVal);
                        if (retVal == "true") {
                            status = 200;
                        }
                        else {
                            status = 100;
                        }
                        connection.close();

                        _signDocument(selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback);
                    });
                }
            }
        }

        var convertFile = function (convCentr, fileFormat, selectedValue, tempfilePath, tipoFirma, idDocumento, pades, callback) {
            var docStatus = true;
            var docStatusDescription = "";
            var urlPost;
            // ...se l'utente vuole convertire il documento si procede con la conversione
            // prefirma
            var statusURL = null;
            var content = null;
            var status = null;
            var signedAsPdf = false;
            if (fileFormat.indexOf(".p7m") == -1 && fileFormat.indexOf(".pdf") == -1) {
                // Se è richiesta conversione pdf prefirma...
                if (document.getElementById("chkConverti").checked) {
                    // ...se è richiesta la conversione centrale...
                    if (convCentr) {
                        // ...si procede con la  conversione sincrona
                        toConvert = true;
                        //ABBATANGELI GIANLUIGI
                        // ERRORE !!! - E' NECESSARIO AVERE IN RISPOSTA HASH DEL FILE E NON TUTTO IL FILE !!!
                        // MODIFICARE  ConvPDFSincrona.aspx.cs CON IN PIU' PARAMETRO ISHASH CHE RESTITUISCE IL SOLO HASH DEL FILE CONVERTITO IN PDF !!!
                        urlPost = '../DigitalSignature/ConvPDFSincrona.aspx?applet=true';
                        // FINE ERRORE !!!
                        var paramPost = '';
                        $.ajax({
                            type: 'POST',
                            url: urlPost,
                            data: {},
                            success: function (content, textStatus, jqXHR) {
                                status = jqXHR.status;
                                //var content = response.responseText;
                                if (status != 200) {
                                    signedAsPdf = false;
                                    content = null;
                                }
                                else {
                                    signedAsPdf = (content != null);
                                }

                                if ((!signedAsPdf) && toConvert) {
                                    docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
                                                                                "Operazione annullata, il file non verrà firmato.";
                                    docStatus = false;
                                    sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                                } else {
                                    saveFile(tempfilePath, content, function (retVal, connection) {
                                        //alert("SaveFileNoApplet 1 retVal" + retVal);
                                        if (retVal == "true") {
                                            status = 200;
                                        }
                                        else {
                                            status = 100;
                                        }
                                        connection.close();
                                        _sign(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, false, signedAsPdf);

                                    });
                                }
                            },
                            error: function () {
                                docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
                                                                            "Operazione annullata, il file non verrà firmato.";
                                docStatus = false;
                                sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                            },
                            async: true
                        });

                    }
                    else {
                        // ...conversione locale eliminata
                        //content = ConvertPdfStream(http.responseBody, fileFormat, false);
                    }

                }
            }
        }

        function Sign(tipoFirma, idDocumento, callback, sighHash) {
            var docStatus = true;
            var docStatusDescription = "";
            var urlPost;
            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;
            var statusURL = null;
            var content = null;
            var status = null;
            var isSigned = false;
            var fileFormat = "<%=GetFileExtension()%>".toUpperCase();
            var errorText = null;
            var pades = false;
            var tempfilePath = null;

            if (document.getElementById("chkPades"))
                pades = document.getElementById("chkPades").checked;
           //console.log('isSigned', "<%=IsSigned()%>"); 
            isSigned = ("<%=IsSigned()%>" === "1");

            if (pades)
                tipoFirma = 'sign';

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
                sendStatus(docStatus, docStatusDescription, idDocumento, callback);
            }
            else {
                getSpecialFolder(function (path, connection) {

                    var selectedValue = temp.options[indexCert].value;
                    if (idDocumento != null && idDocumento != "") {
                        tempfilePath = path + 'tempsign_' + getUniqueId() + fileFormat;
                        urlPost = "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&tipoFirma=" + tipoFirma + "&pades=" + pades+"&idDocumento=" + idDocumento;
                        statusURL = ajaxCall(urlPost);
                        status = statusURL.status;
                        content = statusURL.content;
                        errorText = statusURL.errorText;

                        if (status != 200) {
                            sendStatus(status, errorText, idDocumento,
                            function () {
                                reallowOp();
                                callback();
                            });
                            return;
                        }
                    }

                    if (docStatus) {
                        var signedAsPdf = false;
                        var toConvert = false;
                        var convLoc = false; // ConvertLocally();
                        var convCentr = document.getElementById("chkConverti").checked;

                        // Conversione del file da firmare in pdf
                        // Se il sistema è configurato per convertire il documento in pdf prima
                        // della firma...
                        if (convCentr || convLoc) {
                            tempfilePath = path + 'tempsign_' + getUniqueId() + '.pdf';
                            convertFile(convCentr, fileFormat, selectedValue, tempfilePath, tipoFirma, idDocumento, pades, callback);
                        }
                        else {

                            _sign(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash, false);
                        }
                    } else {
                        sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                    }
                });
            }

            // Invio informazioni sullo stato della firma  
        }
        /*NO APLLET END*/

        function getUniqueId() {
            var dateObject = new Date();
            var uniqueId = dateObject.getFullYear() + '' + dateObject.getMonth() + '' + dateObject.getDate() + '' + dateObject.getTime();

            return uniqueId;
        }

        function CloseApplet() {
            disallowOp('Content1');
            try {
                if (applet) {
                    applet.close();
                    commandType = 'close';
                }
                //applet.killApplet();
                return true;
            }
            catch (err) {
                return true;
                //alert(err.Description);
            }
        }

        function EncodeHtml(value) {
            value = escape(value);
            value = value.replace(/\//g, "%2F");
            value = value.replace(/\?/g, "%3F");
            value = value.replace(/=/g, "%3D");
            value = value.replace(/&/g, "%26");
            value = value.replace(/@/g, "%40");
            return value;
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
  
         <%if (componentType != NttDataWA.Utils.Constans.TYPE_APPLET && componentType != NttDataWA.Utils.Constans.TYPE_SOCKET)
           { %>
            <iframe id="uploadFrame" frameborder="0" height="300px" width="100%" scrolling="no" src="DigitalSignDialog.aspx?TipoFirma=<%=this.TipoFirma%>"></iframe>
         <%}
           else if (CommandType != "close" && componentType != NttDataWA.Utils.Constans.TYPE_SOCKET)
           {%>
             <applet id='signApplet' 
                    code= 'com.nttdata.signapplet.gui.SignApplet' 
                    codebase= '<%=Page.ResolveClientUrl("~/Libraries/")%>';
                    archive='SignApplet.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>junit-3.8.1.jar'
		            width= '10'   height = '9'>
                <param name="java_arguments" value="-Xms128m" />
                <param name="java_arguments" value="-Xmx256m" />
            </applet>
            <applet id='fsoApplet' 
                code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
                codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                archive='FsoApplet.jar'
		        width = '10'   height = '9'>
                <param name="java_arguments" value="-Xms128m" />
                <param name="java_arguments" value="-Xmx512m" />
            </applet>
         <%} %>

    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
            <div>
                <asp:Label ID="lblListaCertificati" runat="server"></asp:Label>
           </div>
           <div>
                <select language="javascript" id="lstListaCertificati" 
                    size="9" name="selectCert" runat="server" class="txt_textarea" ClientIDMode="Static">
                </select>
           </div>
           <br />
           <div>
                <asp:RadioButton ID="optFirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                <asp:RadioButton ID="optCofirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                <asp:HiddenField ID="tipoFirmaH" ClientIDMode="Static" runat="server" Value="false" />
            </div>
            <br />
           <div>
                <asp:CheckBox ID="chkPades" runat="server" Text="Firma PADES" Checked="false" ClientIDMode="Static"/><br />
                <asp:CheckBox ID="chkConverti" runat="server" Text="pdf" Checked="false" ClientIDMode="Static"/>
           </div>
           <div>
                <asp:Label ID="lblDocumentCount" runat="server"></asp:Label>
                <br /><br />
                <asp:UpdatePanel ID="pnlGridResultContainer" UpdateMode="Conditional" ScrollBars="Auto" runat="server">
                    <ContentTemplate>
                        <asp:GridView id="grdResult" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded_custom round_onlyextreme">
		                    <RowStyle CssClass="NormalRow" />
		                    <AlternatingRowStyle CssClass="AltRow" />
                            <PagerStyle CssClass="recordNavigator2" />
                            <Columns>							  
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdDocumento" runat="server" Text='<%#(DataBinder.Eval(Container, "DataItem.IdDocument"))%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ControlStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderStyle HorizontalAlign="Center" Width="70%"/>
                                    <ItemTemplate>
                                        <asp:Label ID="lblEsito" runat="server" Font-Bold="true" Text='<%#(((bool) DataBinder.Eval(Container, "DataItem.Status")) ? "OK" : "KO")%>'></asp:Label>
                                        <br />
                                        <asp:Label ID="lblDescrizioneEsito" runat="server" Text='<%#(DataBinder.Eval(Container, "DataItem.StatusDescription"))%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
           </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="DigitalSignDialogBtnSign" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Firma" ClientIDMode="Static" OnClick="DigitalSignDialogBtnSign_Click"/>
            <cc1:CustomButton ID="DigitalSignDialogBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="DigitalSignDialogBtnClose_Click" />
			<asp:Button ID="hdnDigitalSignDialogBtnSign" runat="server" CssClass="hidden"  ClientIDMode="Static" OnClick="DigitalSignDialogBtnSign_Click"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
