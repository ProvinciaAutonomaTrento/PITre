<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="ReadOnly" CodeBehind="DigitalSignDialog.aspx.cs" Inherits="NttDataWA.Popup.DigitalSignDialog"  %>

<%@ Register Src="../SmartClient/DigitalSignWrapper.ascx" TagName="CapicomWrapper" TagPrefix="uc2" %>
<%@ Register Src="../AcrobatIntegration/ClientController.ascx" TagName="ClientController" TagPrefix="uc1" %>

<html>
<head>
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <link href="../Css/Left/popup.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/json2.js" type="text/javascript"></script>
    <base target="_self" />
    <script type="text/javascript">
        function ApplySign() {
            // Reperimento id documenti in modalità multiselezione
            var docs = "<%=this.GetSelectedDocumentsIds()%>";
            
            if (docs != null && docs != "") {
                return this.SignDocuments(docs.split("|"));
            }
            else {
                alert('Nessun documento selezionato per la firma');
                return false;
            }
        }

        function ViewResult() {
            $('.pulsante69').click();
        }

        function CloseWindow() {
            parent.closeAjaxModal('DigitalSignSelector', 'up'); ;
        }
        
        function dummy()
        { }



        function SignDocuments(docs) {
            var retValue = false;
            var signType = "<%=this.GetSignType()%>";
            //var signType = "0";
            //ABBATANGELI - Nuova gestione Sign/Cosign
            var sigCosig = "cosign";
            var optfirma = document.getElementById("optFirma");

            if (!optfirma) {
                var tipoFirma = document.getElementById("tipoFirmaH");
                optfirma = { checked: (tipoFirma.value == 'true') };
            }

            if (optfirma.checked == true) {
                sigCosig = "sign";
            }

            for (k = 0; k < docs.length; k++) {
                if (signType == "1") {
                    return this.SignHash(sigCosig, docs[k]);
                } else {
                    return this.SignDocument(sigCosig, docs[k]);
                }
            }

            return retValue;
        }


        function SignHash(tipoFirma, idDocumento) {
            var docStatus = true;
            var docStatusDescription = "";

            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;

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
                        //docStatusDescription = http.statusText;
                        if (!http.responseText)
                            docStatusDescription = "Formato file non supportato per la firma.";
                        else
                            docStatusDescription = http.responseText;
                    }
                }

                var content = null;

                if (docStatus) {
                    var url = "../SmartClient/SignedRecordViewer.aspx?idDocumento=" + idDocumento+"&isHash=true";

                    var http = new ActiveXObject("MSXML2.XMLHTTP");
                    http.Open("POST", url, false);
                    http.send();
                    //iFrameSignedDoc.location = url;

                    var signedAsPdf = false;
                    var toConvert = false;

                    var convLoc = ConvertLocally();
                    var convCentr = ConvertCentrally();

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
                                    var sincrona = new ActiveXObject("MSXML2.XMLHTTP");
                                    sincrona.Open("POST", "../DigitalSignature/ConvPDFSincrona.aspx", false);
                                    sincrona.send();

                                    if (sincrona.status == 200)
                                        content = sincrona.responseBody;
                                }
                                else {
                                    // ...altrimenti si procede con la conversione locale
                                    content = ConvertPdfStream(http.responseBody, fileFormat, false);
                                }

                                signedAsPdf = (content != null);
                            }
                        }

                        if (content == null)
                            content = http.responseBody;
                    }
                    else {
                        content = http.responseBody;
                    }

                    if ((!signedAsPdf) && toConvert) {
                        //docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
                        //                            "Il file verrà firmato nel suo formato originale.";

                        //verificare il funzionamento qui sotto riportato
                        docStatusDescription = "Non e\' stato possibile convertire il file in formato PDF.\n" +
                                                                "Operazione annullata, il file non verra\' firmato.";
                        docStatus = false;
                        // Emanuela                                    return (docStatus == 0);
                    }
                    else {
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
            }

            // Invio informazioni sullo stato della firma
            var http2 = new ActiveXObject("MSXML2.XMLHTTP");
            http2.Open("POST", "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocumento=" + idDocumento, false);
            http2.send();

            return (docStatus == 0 || docStatus == true);
        }


        function SignDocument(tipoFirma, idDocumento) {
            var docStatus = true;
            var docStatusDescription = "";

            var temp = document.getElementById("lstListaCertificati");
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
                    http.Open("POST", "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento, false);

                    http.send();
                    if (http.status != 200) {
                        // Si è verificato un errore, reperimento del messaggio
                        docStatus = false;
                        //docStatusDescription = http.statusText;
                        if (!http.responseText)
                            docStatusDescription = "Formato file non supportato per la firma.";
                        else
                            docStatusDescription = http.responseText;
                    }
                }


                if (docStatus) {
                    var url = "../SmartClient/SignedRecordViewer.aspx?idDocumento=" + idDocumento;

                    var http = new ActiveXObject("MSXML2.XMLHTTP");
                    http.Open("POST", url, false);
                    http.send();
                    //iFrameSignedDoc.location = url;

                    var content = null;
                    var signedAsPdf = false;
                    var toConvert = false;

                    var convLoc = ConvertLocally();
                    var convCentr = ConvertCentrally();

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
                                    var sincrona = new ActiveXObject("MSXML2.XMLHTTP");
                                    sincrona.Open("POST", "../DigitalSignature/ConvPDFSincrona.aspx", false);
                                    sincrona.send();

                                    if (sincrona.status == 200)
                                        content = sincrona.responseBody;
                                }
                                else {
                                    // ...altrimenti si procede con la conversione locale
                                    content = ConvertPdfStream(http.responseBody, fileFormat, false);
                                }

                                signedAsPdf = (content != null);
                            }
                        }

                        if (content == null)
                            content = http.responseBody;
                    }
                    else {
                        content = http.responseBody;
                        //alert(http.responseBody);
                    }

                    if ((!signedAsPdf) && toConvert) {
                        //docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
                        //                            "Il file verrà firmato nel suo formato originale.";

                        //verificare il funzionamento qui sotto riportato
                        docStatusDescription = "Non e\' stato possibile convertire il file in formato PDF.\n" +
                                                                "Operazione annullata, il file non verra\' firmato.";
                        docStatus = false;
                        // Emanuela                                    return (docStatus == 0);
                    }
                    else {
                        // Applicazione della firma digitale al documento
                        var signedValue = CapicomWrapper_SignData(content, selectedValue, (tipoFirma == "cosign"), 2, "My");

                        if (signedValue == null) {
                            docStatus = false;
                            docStatusDescription = "Errore nella firma digitale del documento";
                        }
                        else {
                            var httpSave = new ActiveXObject("MSXML2.XMLHTTP");
                            httpSave.Open("POST", "../SmartClient/SaveSignedFile.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf + "&iscontent=false", false);
                            httpSave.send(signedValue);

                            if (httpSave.status != 0 && httpSave.status != 200) {
                                docStatus = false;
                                docStatusDescription = "Errore durante l\'invio del documento firmato.\n" + httpSave.statusText + "\n" + httpSave.responseText;
                            }
                        }
                    }
                }
            }

            // Invio informazioni sullo stato della firma
            var http2 = new ActiveXObject("MSXML2.XMLHTTP");
            http2.Open("POST", "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocumento=" + idDocumento, false);
            http2.send();

            return (docStatus == 0 || docStatus == true);
        }

        // Funzione per l'inizializzazione dell'interfaccia grafica
        function Initialize() {
            // Recupero della configurazione relativa alla conversione pdf
            // locale e centrale e alla sua abilitazione
            var isEnabledConv = ("<%=ConvertPdfOnSign%>" == "True");
            var convPdfLocale = false;
            var convPdfCentrale = true;

            // Recupero dei riferimenti alla checkbox "Converti in pdf", ai due option button "Locale"
            // e "Centrale" e al pannello che contiene i due option button
            var chkConverti = document.getElementById("chkConverti");
            var optLocale = document.getElementById("optLocale");
            var optCentrale = document.getElementById("optCentrale");
            var pnlConversione = document.getElementById("pnlConversione");
            // Se il sistema non è configurato per convertire in pdf prima della
            // firma o se lo è ma nessuna delle due conversioni è operativa,
            // bisogna disabilitare la checkbox "Converti in PDF", togliere
            // il flag e nascondere le due opzioni di conversione
            if (!((isEnabledConv && convPdfLocale) || (isEnabledConv && convPdfCentrale))) {
                chkConverti.checked = false;
                chkConverti.disabled = true;
                pnlConversione.style.visibility = 'hidden';
            }

            // Fetch della lista dei certificati
            FetchListaCertificati();

            // Simula il click sul check box "Converti in PDF"
            OnClickConvertPDF();
        }

        function FetchListaCertificati() {
            var list = CapicomWrapper_GetCertificateList(2, "MY");
            if (list.length > 0) {
                var i=1;
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
                            frmDialogFirmaDigitale.lstListaCertificati.options.add(option);
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
                            frmDialogFirmaDigitale.lstListaCertificati.options.add(option);

                            alert('Non è stato possibile controllare la firma digitale.');
                            option.Value = optionValue;
                            option.innerText = optionText;
                            option.style.color = 'black';
                        } catch (ex) {
                            //console.log('Errore controllo firma', ex);
                            alert('Errore nella verifica del certificato');
                        }


                    };

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

        //function FetchListaCertificati() {
        //    try {
        //        var list = CapicomWrapper_GetCertificateList(2, "MY");

        //        var e = new Enumerator(list);
        //        var i = 1;
        //        for (; !e.atEnd() ; e.moveNext()) {
        //            var cert = e.item();

        //            var option = document.createElement("OPTION");
        //            frmDialogFirmaDigitale.lstListaCertificati.options.add(option);
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

        // Funzione per la gestione del click sulla check box "Converti in PDF"
        function OnClickConvertPDF() {
            // Recupero della configurazione relativa alla conversione pdf
            // locale e centrale e alla sua abilitazione
            var isEnabledConv = ("<%=ConvertPdfOnSign%>" == "True");
            var convPdfLocale = (isEnabledConv && IsIntegrationActiveAndInstalled());
            var convPdfCentrale = (isEnabledConv && "<%=IsEnabledConvPDFSincrona%>" == "True");

            // Recupero dei riferimenti alla checkbox "Converti in pdf", ai due option button "Locale"
            // e "Centrale" e al pannello che contiene i due option button
            var optLocale = document.getElementById("optLocale");
            var optCentrale = document.getElementById("optCentrale");
            var pnlConversione = document.getElementById("pnlConversione");


            // Recupero dello stato di checking della chkConverti
            var chkConverti = document.getElementById("chkConverti").checked;

            // Se chkConverti è flaggato bisogna abilitare il pannello altrimenti bisogna
            // disabilitarlo
            if (chkConverti)
                pnlConversione.disabled = false;
            else
                pnlConversione.disabled = true;

            optLocale.setAttribute("disabled", !(convPdfLocale && chkConverti));
            optCentrale.setAttribute("disabled", !(convPdfCentrale && chkConverti));

            // Se è attiva la conversione locale la si preferisce altrimenti si seleziona
            // la conversione locale
            if (convPdfLocale)
                optLocale.checked = true;
            else
                optCentrale.checked = true;

        }

        // Analizza l'interfaccia al fine di determinare se bisogna convertire
        // il documento localmente
        function ConvertLocally() {
            // Recupero del riferimento all'option button "Locale" e valutazione
            // dello stato di flagging dell'opzione "Converti in PDF"
            var optLocale = document.getElementById("optLocale");
            var convertInPDF = document.getElementById("chkConverti").checked;

            // Si restituisce l'and fra le due variabili
            return optLocale.checked && convertInPDF;
        }

        // Analizza l'interfaccia al fine di determinare se bisogna convertire
        // il documento centralmente
        function ConvertCentrally() {
            // Recupero del riferimento all'option button "Centrale" e valutazione
            // dello stato di flagging dell'opzione "Converti in PDF"
            var optCentrale = document.getElementById("optCentrale");
            var convertInPDF = document.getElementById("chkConverti").checked;

            // Si restituisce l'and fra le due variabili
            return optCentrale.checked && convertInPDF;

        }
			
    </script>
</head>
<body onload="Initialize();">
    <form id="frmDialogFirmaDigitale" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600"
        EnablePageMethods="true">
    </asp:ScriptManager>
    <uc1:ClientController ID="AcrobatClientController" runat="server" />
    <table width="100%" cellpadding="0" align="center">
        <tr>
            <td class="weight">
                <asp:Label ID="lblListaCertificati" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <select language="javascript" id="lstListaCertificati" 
                    size="9" name="selectCert" runat="server" class="txt_textarea">
                </select>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <asp:RadioButton ID="optFirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                    <asp:RadioButton ID="optCofirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                    <asp:HiddenField ID="tipoFirmaH" ClientIDMode="Static" runat="server" Value="false" />
                </div>
                <br />
                <div>
                    <asp:CheckBox ID="chkConverti" runat="server" Text="pdf" Checked="false" />
                    <asp:CheckBox ID="chkPades" runat="server" Text="Firma PADES" Checked="false" ClientIDMode="Static"/>
                </div>

                <asp:Panel ID="pnlConversione" runat="server" class="hidden">
                    <asp:RadioButton ID="optLocale" GroupName="grpLocCentr" runat="server" Text="Local" Checked="true" />
                    &nbsp;&nbsp;
                    <asp:RadioButton ID="optCentrale" GroupName="grpLocCentr" runat="server" Text="Central" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="titolo_scheda" align="center">
                <asp:Button ID="DigitalSignDialogBtnSign" runat="server" Text="Applica firma" CssClass="pulsante69 hidden" OnClick="DigitalSignDialogBtnSign_Click" OnClientClick="return ApplySign();"></asp:Button>
            </td>
        </tr>
        <tr>
            <td align="center">
                
                <%--<asp:Label ID="lblDocumentCount" runat="server"></asp:Label>--%>
                
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
            </td>
        </tr>
    </table>
    </form>
</body>
</html>