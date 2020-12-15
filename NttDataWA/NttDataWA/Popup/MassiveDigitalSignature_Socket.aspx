<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveDigitalSignature_Socket.aspx.cs" Inherits="NttDataWA.Popup.MassiveDigitalSignature_Socket" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var applet = undefined;
        var commandType = '<%=CommandType%>';
        var convpdfa = null;

        function signSmartClient() {
            disallowOp('Content1');
            //if (window.frames['uploadFrame'].ApplySign()) {
            window.frames['uploadFrame'].ViewResult();
            //}
            reallowOp();
            return false;
        }

        function signWithApplet(callback) {
            disallowOp('Content1');
            var docs = "<%=this.GetSelectedDocumentsJSON()%>";
            if (docs != null && docs != "") {
                return this.SignDocuments(docs.split("|"), function () {
                    callback();
                    reallowOp();
                });

            }
            else {
                alert('Nessun documento selezionato per la firma');
                reallowOp();
                return false;
            }
        }

        function openApplet() {
            if (commandType != 'close') {
                try {
                    disallowOp('Content1');
                    //if (applet == undefined) {
                    //    applet = window.document.plugins[0];
                    //}
                    //if (applet == undefined) {
                    //    applet = document.applets[0];
                    //}
                    //if (appletFso == undefined) {
                    //    appletFso = window.document.plugins[1];
                    //}
                    //if (appletFso == undefined) {
                    //    appletFso = document.applets[1];
                    //}

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

        function getUniqueId() {
            var dateObject = new Date();
            var uniqueId = dateObject.getFullYear() + '' + dateObject.getMonth() + '' + dateObject.getDate() + '' + dateObject.getTime();

            return uniqueId;
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
            CURRENT++;
            if (CURRENT === COUNT_TOTAL)
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
                urlPost = recoveryPath + '?isHash=true&idDocumento=' + idDocumento;
                statusURL = ajaxCall(urlPost);
                status = statusURL.status;
                content = statusURL.content;
            }
            catch (e) {
                alert(e.message.toString());
                retValue = false;
            }

            if (content != null && content != '') {
                var signedValue;
                var cc;
                //test promise
                signHashPromise(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "", function (sv, connection) {
                    signedValue = sv;
                    cc = connection;
                }
                ).then(function () {
                    cc.close();
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
                    });

                /*
                signHashPromise(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "", function (signedValue, connection) {
                    connection.close();
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
                });
                */
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
                    var completeUrl = "../SmartClient/SaveSignedFile.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf + "&iscontent=true" + "&issocket=true";
                    getFileFromPath(signedValue, completeUrl, function (getFile, connection) {
                        //alert(getFile);
                        connection.close();
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
            var recoveryPath = '../SmartClient/SignedRecordViewer.aspx';
            if (sighHash) {
                _signHash(selectedValue, content, recoveryPath, tipoFirma, pades, idDocumento, callback);
            } else {

                if (content && content !== '') {
                    _signDocument(selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback);
                } else {
                    try {
                        status = 0;
                        content = '';
                        urlPost = recoveryPath + '?type=applet&idDocumento=' + idDocumento;
                        statusURL = ajaxCall(urlPost);
                        status = statusURL.status;
                        content = statusURL.content;
                    }
                    catch (e) {
                        alert(e.message.toString());
                        retValue = false;
                    }
                    saveFile(tempfilePath, content, function (retVal, connection) {
                        connection.close();
                        //alert("SaveFileNoApplet 1 retVal" + retVal);
                        if (retVal == "true") {
                            status = 200;
                        }
                        else {
                            status = 100;
                        }


                        _signDocument(selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback);
                    });
                }
            }
        }

        var convertFile = function (convCentr, fileFormat, selectedValue, tempfilePath, tipoFirma, idDocumento, pades, sighHash, callback) {
            var docStatus = true;
            var docStatusDescription = "";
            var urlPost;
            // ...se l'utente vuole convertire il documento si procede con la conversione
            // prefirma
            var statusURL = null;
            var content = null;
            var status = null;
            var signedAsPdf = false;

            if (fileFormat.indexOf("P7M") == -1 && fileFormat.indexOf("PDF") == -1) {
                // Se è richiesta conversione pdf prefirma...
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
                                    connection.close();
                                    if (retVal == "true") {
                                        status = 200;
                                    }
                                    else {
                                        status = 100;
                                    }

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
            else {
                //alert('Il file di tipo ' + fileFormat + ' non sarà convertito.');
                _sign(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash);
            }
        }

        function Sign(tipoFirma, idDocumento, callback, sighHash, pades, signed, fileExtension, convCentr) {
            var docStatus = true;
            var docStatusDescription = "";
            var urlPost;
            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;
            var statusURL = null;
            var content = null;
            var status = null;
            var fileFormat = '';
            var errorText = null;
            var tempfilePath = null;
            if (fileExtension) {
                fileFormat = fileExtension.toUpperCase();
            }

            if (pades || (signed && (signed === "0"))) {
                tipoFirma = 'sign';
            }

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
                sendStatus(docStatus, docStatusDescription, idDocumento, callback);
            }
            else {
                getSpecialFolder(function (path, connection) {
                    connection.close();
                    var selectedValue = temp.options[indexCert].value;
                    if (idDocumento != null && idDocumento != "") {
                        tempfilePath = path + 'tempsign_' + getUniqueId() + fileFormat;
                        urlPost = "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento + "&tipoFirma=" + tipoFirma + "&pades=" + pades;
                        statusURL = ajaxCall(urlPost);
                        status = statusURL.status;
                        content = statusURL.content;
                        errorText = statusURL.errorText;

                        if (status != 200) {
                            sendStatus(status, errorText, idDocumento, callback);
                            return;
                        }
                    }

                    if (docStatus) {
                        var signedAsPdf = false;
                        var toConvert = false;
                        var convLoc = false; // ConvertLocally();


                        // Conversione del file da firmare in pdf
                        // Se il sistema è configurato per convertire il documento in pdf prima
                        // della firma...
                        if (convCentr || convLoc) {
                            tempfilePath = path + 'tempsign_' + getUniqueId() + '.pdf';
                            convertFile(convCentr, fileFormat, selectedValue, tempfilePath, tipoFirma, idDocumento, pades, sighHash, callback);
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

        function SignDocuments(docs, callback) {
            var retValue = true;
            var strPadesSign = "<%=this.FirmaPades%>";
            var padesSign = false;
            var documento;
            var idDocumento;
            var isSigned;
            var fileExtension;

            //ABBATANGELI - Nuova gestione Sign/Cosign
            var tipoFirma = "cosign";
            var optfirma = document.getElementById("optFirma");
            if (optfirma.checked == true) {
                tipoFirma = "sign";
            }

            var convCentr = false;
            COUNT_TOTAL = docs.length;
            CURRENT = 0;

            if (strPadesSign == 'True' || strPadesSign == 'true') {
                padesSign = true;
            }

            //voglio la firma pades cliccando radiobutton sulla maschera
            if (document.getElementById("optPades")) {
                padesCheck = document.getElementById("optPades").checked;
                if (padesCheck)
                    padesSign = true;
                else
                    padesSign = false;
            }

            <%if (ConvertPdfOnSign) {%>
            convCentr = true;
            <%} else {%>
            if (document.getElementById("chkConverti"))
                convCentr = document.getElementById("chkConverti").checked;
                        <%} %>

            (async function loop() {
                for (k = 0; k < docs.length; k++) {
                    await new Promise((resolve) => {
                        documento = JSON.parse(docs[k]);
                        idDocumento = documento.idDocumento;
                        isSigned = documento.isSigned;
                        fileExtension = documento.fileExtension;
                        //Se contiene P vuol dire che arriva da LF es è cades
                        if (idDocumento.indexOf("P") > -1) {
                            idDocumento = idDocumento.replace("P", "");
                   <%-- this.SignHash('<%=this.TipoFirma%>', idDocumento, true);--%>
                            SignPromise(tipoFirma, idDocumento, callback, true, true, isSigned, fileExtension, convCentr).then(function () { resolve(); });
                        } else if (idDocumento.indexOf("C") > -1) {
                            idDocumento = idDocumento.replace("C", "");
                    <%--this.SignHash('<%=this.TipoFirma%>', idDocumento, false);--%>
                            SignPromise(tipoFirma, idDocumento, callback, true, false, isSigned, fileExtension, convCentr).then(function () { resolve(); });
                        }
                        else {
                            if (padesSign) {
                                SignPromise(tipoFirma, idDocumento, callback, true, true, isSigned, fileExtension, convCentr).then(function () { resolve(); });
                            } else {
                                SignPromise(tipoFirma, idDocumento, callback, true, false, isSigned, fileExtension, convCentr).then(function () { resolve(); });
                            }
                        }
                        // setTimeout(resolve, Math.random() * 1000)
                    });
                   //console.log(k);
                }
            })();


          <%-- 
            for (k = 0; k < docs.length; k++) {
                documento = JSON.parse(docs[k]);
                idDocumento = documento.idDocumento;
                isSigned = documento.isSigned;
                fileExtension = documento.fileExtension;
                //Se contiene P vuol dire che arriva da LF es è cades
                if (idDocumento.indexOf("P") > -1) {
                    idDocumento = idDocumento.replace("P", ""); --%>
                   <%-- this.SignHash('<%=this.TipoFirma%>', idDocumento, true);--%>
                   <%--Sign(tipoFirma, idDocumento, callback, true, true, isSigned, fileExtension, convCentr);
                } else if (idDocumento.indexOf("C") > -1) {
                    idDocumento = idDocumento.replace("C", ""); --%>
                    <%--this.SignHash('<%=this.TipoFirma%>', idDocumento, false);--%>
                   <%-- Sign(tipoFirma, idDocumento, callback, true, false, isSigned, fileExtension, convCentr);
                }
                else {

                    if (padesSign) {
                        Sign(tipoFirma, idDocumento, callback, true, true, isSigned, fileExtension, convCentr);
                    } else {
                        Sign(tipoFirma, idDocumento, callback, true, false, isSigned, fileExtension, convCentr);
                    }
                }
        }--%>
    }

    // test promise
        var _signHashPromise = function (selectedValue, content, recoveryPath, tipoFirma, pades, idDocumento, callback) {
           //console.log(">>>> INIT - _signHashPromise");
            var status;
            var urlPost;
            var statusURL;
            var docStatus = true;
            var docStatusDescription = "";
            var _promise = new Promise(function (resolve, reject) {
               //console.log(">>>> START - _signHashPromise");
                try {
                status = 0;
                content = '';
                urlPost = recoveryPath + '?isHash=true&idDocumento=' + idDocumento;
                statusURL = ajaxCall(urlPost);
                status = statusURL.status;
                content = statusURL.content;
            }
            catch (e) {
                alert(e.message.toString());
                retValue = false;
            }
            if (content != null && content != '') {

                var signedValue;
                var cc;
                //test promise
                signHashPromise(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "", function (sv, connection) {
                    signedValue = sv;
                    cc = connection;
                }
                ).then(function () {
                    cc.close();
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
                   //console.log("### >>>>  _signHashPromise");
                    resolve();
                });

            }
            else {
                alert("Impossibile contattare il ws...");
                callback(false);
                reject();
                }
               //console.log(">>>> END - _signHashPromise");
            });
           //console.log(">>>> RETURN - _signHashPromise");
            return _promise;
        }
//////////////////////


// test Promise
        var _signPromise = function (selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash, signedAsPdf) {
           //console.log(">>>> INIT - _signPromise");
            var status;
            var urlPost;
            var statusURL;
            var docStatus = true;
            var docStatusDescription = "";
            var recoveryPath = '../SmartClient/SignedRecordViewer.aspx';
            var _promise = new Promise(function (resolve, reject) {
               //console.log(">>>> START - _signPromise");
                if (sighHash) {
                    _signHashPromise(selectedValue, content, recoveryPath, tipoFirma, pades, idDocumento, callback).then(function () {
                       //console.log("### >>>>  _signPromise");
                        resolve();
                    });
                } else {
                    if (content && content !== '') {
                        _signDocument(selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback);
                    } else {
                        try {
                            status = 0;
                            content = '';
                            urlPost = recoveryPath + '?type=applet&idDocumento=' + idDocumento;
                            statusURL = ajaxCall(urlPost);
                            status = statusURL.status;
                            content = statusURL.content;
                        }
                        catch (e) {
                            alert(e.message.toString());
                            retValue = false;
                        }
                        saveFile(tempfilePath, content, function (retVal, connection) {
                            connection.close();
                            //alert("SaveFileNoApplet 1 retVal" + retVal);
                            if (retVal == "true") {
                                status = 200;
                            }
                            else {
                                status = 100;
                            }

                            _signDocument(selectedValue, tempfilePath, tipoFirma, idDocumento, signedAsPdf, callback);
                        });
                    }
                   //console.log("### >>>>  _signPromise in else sign");
                    resolve();
                }
               //console.log(">>>> END - _signPromise");
            });
           //console.log(">>>> RETURN - _signPromise");
            return _promise;
        }

///////////////////


// test promise
        function SignPromise(tipoFirma, idDocumento, callback, sighHash, pades, signed, fileExtension, convCentr) {
           //console.log(">>>>  INIT - SignPromise");
            var docStatus = true;
            var docStatusDescription = "";
            var urlPost;
            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;
            var statusURL = null;
            var content = null;
            var status = null;
            var fileFormat = '';
            var errorText = null;
            var tempfilePath = null;
            if (fileExtension) {
                fileFormat = fileExtension.toUpperCase();
            }
            var _promise = new Promise(function (resolve, reject) {
               //console.log(">>>>  START - SignPromise");
                if (pades || (signed && (signed === "0"))) {
                    tipoFirma = 'sign';
                }
                if (isNaN(indexCert) || indexCert == -1) {
                    docStatus = false;
                    docStatusDescription = "Nessun certificato selezionato";
                    sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                }
                else {
                    getSpecialFolder(function (path, connection) {
                        connection.close();
                        var selectedValue = temp.options[indexCert].value;
                        if (idDocumento != null && idDocumento != "") {
                            tempfilePath = path + 'tempsign_' + getUniqueId() + fileFormat;
                            urlPost = "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento + "&tipoFirma=" + tipoFirma + "&pades=" + pades;
                            statusURL = ajaxCall(urlPost);
                            status = statusURL.status;
                            content = statusURL.content;
                            errorText = statusURL.errorText;
                            if (status != 200) {
                                sendStatus(status, errorText, idDocumento, callback);
                                resolve();
                                return;
                            }
                        }
                        if (docStatus) {
                            var signedAsPdf = false;
                            var toConvert = false;
                            var convLoc = false; // ConvertLocally();

                            // Conversione del file da firmare in pdf
                            // Se il sistema è configurato per convertire il documento in pdf prima
                            // della firma...
                            if (convCentr || convLoc) {
                                tempfilePath = path + 'tempsign_' + getUniqueId() + '.pdf';
                                convertFilePromise(convCentr, fileFormat, selectedValue, tempfilePath, tipoFirma, idDocumento, pades, sighHash, callback).then(function () {
                                   //console.log("### >>>>  SignPromise Resolve");
                                    resolve();
                                });
                            }
                            else {
                                _signPromise(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash, false).then(function () {
                                   //console.log("### >>>>  SignPromise Resolve");
                                    resolve();
                                });
                            }
                        } else {
                            sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                            resolve();
                        }
                    });
                }
               //console.log(">>>>  END - SignPromise");
            })
           //console.log(">>>>  RETURN - SignPromise");
            return _promise;
            // Invio informazioni sullo stato della firma  
        }


        var convertFilePromise = function (convCentr, fileFormat, selectedValue, tempfilePath, tipoFirma, idDocumento, pades, sighHash, callback) {
           //console.log(">>>>  INIT - convertFilePromise");
            var docStatus = true;
            var docStatusDescription = "";
            var urlPost;
            // ...se l'utente vuole convertire il documento si procede con la conversione
            // prefirma
            var statusURL = null;
            var content = null;
            var status = null;
            var signedAsPdf = false;
            var _promise = new Promise(function (resolve, reject) {
                if (fileFormat.indexOf("P7M") == -1 && fileFormat.indexOf("PDF") == -1) {
                    // Se è richiesta conversione pdf prefirma...
                    // ...se è richiesta la conversione centrale...
                    if (convCentr) {
                       //console.log(">>>> - convertFilePromise conversione");
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
                                        connection.close();
                                        if (retVal == "true") {
                                            status = 200;
                                        }
                                        else {
                                            status = 100;
                                        }

                                        _sign(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, false, signedAsPdf);
                                        resolve();
                                    });
                                }
                            },
                            error: function () {
                                docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
                                                                            "Operazione annullata, il file non verrà firmato.";
                                docStatus = false;
                                sendStatus(docStatus, docStatusDescription, idDocumento, callback);
                                resolve();
                            },
                            async: true
                        });

                    }
                    else {
                        // ...conversione locale eliminata
                        //content = ConvertPdfStream(http.responseBody, fileFormat, false);
                    }
                }
                else {
                    //alert('Il file di tipo ' + fileFormat + ' non sarà convertito.');
                   //console.log(">>>> - convertFilePromise no conversione");
                    //_sign(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash);
                    _signPromise(selectedValue, tempfilePath, content, tipoFirma, idDocumento, pades, callback, sighHash, false).then(function () {
                       //console.log("### >>>>  convertFilePromise Resolve");
                        resolve();
                    });
                }
            });
           //console.log(">>>> RETURN - convertFilePromise");
            return _promise;
        }


    function CadesChkChange() {
        if (convpdfa == null)
            if (document.getElementById("chkConverti"))
                convpdfa = document.getElementById("chkConverti").checked;

        document.getElementById("chkConverti").checked = convpdfa;

    }

    function PadesChkChange() {
        if (convpdfa == null)
            if (document.getElementById("chkConverti"))
                convpdfa = document.getElementById("chkConverti").checked;

        //per il pades non posso fare la conversione prima e check dopo
        document.getElementById("chkConverti").checked = false;

    }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
<div class="container">
    <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcMessage" runat="server">
                <div class="row">
                    <p><asp:Literal ID="litMessage" runat="server" /></p>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UnPnlSign" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcSign" runat="server">
                <div class="row">
                    <div class="col">
                        <asp:Literal ID="lblListaCertificati" runat="server" />
                    </div>
                    <div class="col">
                        <select language="javascript" id="lstListaCertificati" style="width: 350px;"
                            size="9" name="selectCert" runat="server" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="row">
                <div>
                    <asp:RadioButton ID="optFirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                    <asp:RadioButton ID="optCofirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                </div><br />
                    <asp:CheckBox ID="chkConverti" runat="server" Checked="false" ClientIDMode="Static" />  &nbsp;&nbsp;
                    <asp:RadioButton id="optCades" Text="Cades" Checked="True" GroupName="firmaType" runat="server" ClientIDMode="Static" onmousedown ="CadesChkChange()"/> &nbsp;&nbsp;
                    <asp:RadioButton id="optPades" Text="Pades" Checked="False" GroupName="firmaType" runat="server" ClientIDMode="Static" onmousedown ="PadesChkChange()"/>
                    <asp:Panel ID="pnlConversione" runat="server" ClientIDMode="Static" CssClass="hidden">
                        <asp:RadioButton ID="optLocale" GroupName="grpLocCentr" runat="server" ClientIDMode="Static" />&nbsp;&nbsp;
                        <asp:RadioButton ID="optCentrale" GroupName="grpLocCentr" runat="server" ClientIDMode="Static" Checked="true" />
                    </asp:Panel>
                </div>
                <div class="row">
                    <p align="center"><asp:Literal ID="lblDocumentCount" runat="server" /></p>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlReport" runat="server" CssClass="row" Visible="false">
                <asp:GridView id="grdReport" runat="server" Width="100%" AutoGenerateColumns="False" CssClass="tbl_rounded_custom round_onlyextreme">         
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
                        <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReport%>' DataField="ObjId">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Esito" DataField="Result">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Dettagli" DataField="Details">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return signWithApplet(function(){ $('#hdnConfirm').click(); });" />
            <asp:Button ID="hdnConfirm" runat="server" CssClass="hidden"  ClientIDMode="Static" OnClick="BtnConfirm_Click"/>
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="return CloseApplet();" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
