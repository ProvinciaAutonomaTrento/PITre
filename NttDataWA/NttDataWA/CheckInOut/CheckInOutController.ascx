<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="CheckInOutController.ascx.cs"
    Inherits="NttDataWA.CheckInOut.CheckInOutController" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc4" %>
<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc3" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController" TagPrefix="uc1" %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "checkInOutControllerScript"))
  { %>

<script id="checkInOutControllerScript" type="text/javascript">
    var wndAttendi;
    var wndOldMousePointer = window.document.body.style.cursor;
    //var msgWait = "Attendere prego...";
    //var msgCheckIn = "Rilascio del documento in corso...";
    //var msgCheckOut = "Blocco del documento in corso...";
    //var msgUndoCheckOut = "Annullamento del blocco in corso...";
    //var msgOpenCheckOut = "Apertura documento bloccato in corso...";
    //var msgSaveFileVersion = "Copia locale in corso...";
    var msgWait = "<%=MsgWait%>";
    var msgCheckIn = "<%=MsgCheckIn %>";
    var msgCheckOut = "<%=MsgCheckOut %>";
    var msgUndoCheckOut = "<%=MsgUndoCheckOut %>";
    var msgOpenCheckOut = "<%=MsgOpenCheckOut %>";
    var msgSaveFileVersion = "<%=MsgSaveFileVersion %>";
    var _tmpDownloadedFile = "";

    function ApreAttendi(msg) {
        wndOldMousePointer = window.document.body.style.cursor;
        window.document.body.style.cursor = 'wait';

        wndAttendi = window.open("", "Attendere", "width=240,height=20,left=380,top=350,resizable=no,scrollbars=no,menubar=no,status=no,toolbar=no");
        wndAttendi.document.write("<HTML><HEAD><TITLE>" + msgWait + "</TITLE></HEAD><BOBY BGCOLOR='#F79C25'><P align='center'><font size=3 face=Verdana>" + msg + "</font></P></BODY></HTML>");
    }

    function ChiudeAttendi() {
        if (typeof (wndAttendi) != 'undefined') {
            if (!wndAttendi.closed)
                wndAttendi.close();

            window.document.body.style.cursor = wndOldMousePointer;
        }
    }

    function SaveFileVersion(defaultFilePath, fileType, showSaveDialog, showErrorMessage, showFile, showWaitingPage) {
        var _log = "LOG ";
        var retValue = false;

        var filePath = defaultFilePath;
        //console.log("1 - FilePath: " + filePath);
        if (showSaveDialog)
            filePath = ShowSaveDialogBox(defaultFilePath, fileType, "Copia locale", "1");

        if (filePath != null && filePath != "") {
            try {
                // Visualizzazione finestra di attesa
                if (showWaitingPage)
                    ApreAttendi(msgSaveFileVersion);

                var encodedFilePath = EncodeHtml(filePath);
                //console.log("2 - Encoded FilePath: " + encodedFilePath);
                //console.log("3 - URL: " + "<%=SaveFilePageUrl%>?filePath=" + encodedFilePath);
                var http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", "<%=SaveFilePageUrl%>?filePath=" + encodedFilePath, false);
                http.send();

                if (http.status != 200 && http.statusText != null && http.statusText != "") {
                    //console.log("4 - Status: " + http.status);
                    // Si è verificato un errore, reperimento del messaggio
                    throw http.statusText;
                }
                else {
                    //console.log("4 - Status: " + http.status);
                    var content = http.responseBody;
                    retValue = (content != null);
                    //console.log("5 - Ret Value: " + retValue);
                    if (retValue) {
                        try {
                            // Salvataggio del file in locale
                            AdoStreamWrapper_SaveBinaryData(filePath, content);
                        }
                        catch (ex) {
                            //console.log("5 - ERROR: " + ex);
                            throw "Impossibile salvare il file '" + filePath + "'.";
                        }

                        retValue = true;
                    }

                    if (retValue && showFile) {
                        //console.log("7 - Visualizza File: ");
                        // Visualizzazione del file
                        ShowFileDocument(filePath);
                    }
                    
                }
            }
            catch (ex) {
                alert("Errore nel download del documento:\n" + ex.toString());

                retValue = false;
            }
            finally {
                if (showWaitingPage)
                    ChiudeAttendi();
            }
        }

        return retValue;
    }

    function CheckOutDocumentDownloaded(defaultFilePath, fileType, idDocument, documentNumber, content, showSaveDialog, showErrorMessage, showFile, showWaitingPage) {
        var retValue = false;

        if (content != null) {
            var filePath = defaultFilePath;

            if (showSaveDialog)
                filePath = ShowSaveDialogBox(defaultFilePath, fileType, '<%=CheckOutPageTitle %>');

            if (filePath != null && filePath != "") {
                // Salvataggio del file in locale
                AdoStreamWrapper_SaveBinaryData(filePath, content);

                // CheckOut del documento,
                // senza effettuare il download e visualizzando il file
                retValue = CheckOutDocument(filePath,
											fileType,
											idDocument,
											documentNumber,
											false,
											showErrorMessage,
											false,
											showFile,
											showWaitingPage);
            }
        }

        return retValue;
    }

    function CheckOutDocument(defaultFilePath, fileType, idDocument, documentNumber, showSaveDialog, showErrorMessage, downloadFile, showFile, showWaitingPage) {
        return CheckOutDocument(defaultFilePath, fileType, idDocument, documentNumber, showSaveDialog, showErrorMessage, downloadFile, showFile, showWaitingPage, false);
    }

    function CheckOutDocument(defaultFilePath, fileType, idDocument, documentNumber, showSaveDialog, showErrorMessage, downloadFile, showFile, showWaitingPage, mtext) {
        var retValue = false;

        var filePath = defaultFilePath;

        if (showSaveDialog)
            filePath = ShowSaveDialogBox(defaultFilePath, fileType, '<%=CheckOutPageTitle %>');

        if (filePath != null && filePath != "") {
            try {
                // Validazione del formato file fornito in ingresso
                if (SF_ValidateFileFormat(filePath)) {
                    // Visualizzazione finestra di attesa
                    if (showWaitingPage)
                        ApreAttendi(msgCheckOut);

                    var encodedFilePath = EncodeHtml(filePath);

                    var http = CreateObject("MSXML2.XMLHTTP");
                    http.Open("POST", "<%=CheckOutPageUrl%>?location=" + encodedFilePath + "&idDocument=" + idDocument + "&documentNumber=" + documentNumber + "&machineName=<%=MachineName%>&downloadFile=" + downloadFile, false);
                    http.send();

                    var response = http.responseText;

                    retValue = (response == "");

                    if (!retValue && showErrorMessage) {
                        // Visualizzazione messaggio di errore nel checkout
                        alert(response);
                    }

                    var downloaded = false;

                    if (retValue && downloadFile) {
                        // Download del file
                        downloaded = DownloadCheckedOutDocument(filePath);

                        if (!downloaded)
                            throw new Error(0, "Impossibile scaricare il file '" + filePath + "'.\nIl file potrebbe non essere stato acquisito oppure potrebbe non essere presente alcun modello predefinito.");
                        else
                            _tmpDownloadedFile = filePath;
                    }

                    // Validazione della dimensione del file fornito in ingresso

			        if (!retValue && showErrorMessage)
			        {
				        // Visualizzazione messaggio di errore nel checkout
				        alert(response);
			        }

                    if (!SF_ValidateFileSize(filePath) && !mtext) {
                        // Nel caso il formato file non fosse valido, viene annullato il blocco
                        UndoCheckOutDocument(false, false, false);
                        retValue = false;
                    }
                    else {
                        if (retValue && showFile) {
                            // Visualizzazione del file
                            ShowFileDocument(filePath);
                        }
                    }
                }
            }
            catch (ex) {
                alert("Errore nel blocco del documento:\n" + ex.message.toString());

                // Nel caso l'operazione non è andata a buon fine, 
                // il blocco sul documento viene annullato (senza visualizzare messaggi)
                UndoCheckOutDocument(false, false, false);
                retValue = false;
            }
            finally {
                if (showWaitingPage)
                    ChiudeAttendi();
            }
        }

        return retValue;
    }

    // Download del file nel percorso richiesto per
    // il documento correntemente checkedout
    function DownloadCheckedOutDocument(filePath) {
        var retValue = false;

        var http = CreateObject("MSXML2.XMLHTTP");
        http.Open("POST", "<%=DownloadCheckOutPageUrl%>?filePath=" + filePath, false);
        http.send();

        var fso = FsoWrapper_CreateFsoObject();
        var fileExt = fso.GetExtensionName(filePath);

        var content = http.responseBody;
        retValue = (content != null);

        if (retValue) {
            try {
                // Salvataggio del file in locale
                AdoStreamWrapper_SaveBinaryData(filePath, content);
            }
            catch (ex) {
                ex.message = "Impossibile salvare il file '" + filePath + "'.";

                throw ex;
            }

            retValue = true;
        }
        else if (!retValue && fileExt.toLowerCase() == "txt") {
            // Nel caso in cui nessun content è stato scaricato e
            // solamente per i file .txt, è necessario creare un file 
            // di testo di lunghezza 0 bytes
            var stream = fso.CreateTextFile(filePath, true);
            FsoWrapper_CloseFsoStreamObject(stream);

            retValue = true;
        }

        return retValue;
    }

    /*
    Apertura del file (fuori da docspa) con l'applicazione proprietaria. 
    Nel caso il file non è presente, viene visualizzato un messaggio 
    di notifica all’utente.
    */
    function OpenCheckOutDocument(showWaitingPage) {
        try {
            // Visualizzazione maschera di attesa
            if (showWaitingPage) {
                //var waitingPage=ShowWaitingPage("<%=OpenCheckOutWaitingPageUrl%>");
                ApreAttendi(msgOpenCheckOut);
            }

            var checkOutFilePath = document.getElementById('OutFilePathController').value;
            ShowFileDocument(checkOutFilePath);
        }
        catch (ex) {
            alert("Errore nella visualizzazione del documento bloccato:\n" + ex.message.toString());
        }
        finally {
            //if (showWaitingPage && waitingPage!=null)
            //	waitingPage.close();
            if (showWaitingPage)
                ChiudeAttendi();
        }
    }

    /*
     * Indirizzamento della richiesta di apertura file in checkout alla pagina M/Text
    */
    function OpenCheckOutMTextDocument(showWaitingPage) {
        try {
            // Visualizzazione maschera di attesa
            if (showWaitingPage) {
                ApreAttendi(msgOpenCheckOut);
            }

            var http = new ActiveXObject("MSXML2.XMLHTTP");
            http.Open("POST", '<%=MTextShowDocumentUrl%>', false);
            http.send();

            var response = http.responseText;

            var retValue = (http.status == 200);

            if (!retValue) {
                // Visualizzazione dell'eventuale messaggio di errore
                alert(response.toString().split("|")[1]);
            }
            else {
                var url = response.toString().split("|")[1];
                // apertura di una finestra per la visualizzazione dell'applet M/Text
                window.open(url, '_blank', 'status=0,toolbar=0,location=0,menubar=0');
            }

        }
        catch (ex) {
            alert("Errore nella visualizzazione del documento bloccato:\n" + ex.message.toString());
        }
        finally {
            if (showWaitingPage)
                ChiudeAttendi();
        }
    }

    /*
    Visualizzazione del file in CheckOut con l'applicazione proprietaria. 
    */
    function ShowFileDocument(filePath) {
        var fso = FsoWrapper_CreateFsoObject();

        if (fso.FileExists(filePath)) {
            ShellWrappers_Execute(filePath);
        }
        else {
            alert("File '" + filePath + "' non trovato");
        }
    }

    // Visualizzazione maschera impostazione commenti per il checkin del documento
    function ShowDialogComments() {
        var args = new Object;
        args.window = window;

        var retValue = window.showModalDialog("<%=CheckInOutFolderUrl%>CommentsPage.aspx",
											args,
											'dialogWidth:425px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
        return retValue;
    }

    /* 
    * MEV INPS - Integrazione con M/TEXT
    * Aggiunta del nuovo metodo CheckInDocument che accetta come parametro la strategia da utilizzare.
    * Al momento la strategia può essere FileSystem (Tradizionale) o MTEXT
    */
    // Checkin del file per il documento checkedOut
    function CheckInDocument(showCommentsPage, showErrorMessage, isSignedFile, showWaitingPage, strategy) {
        if (strategy == null) strategy = new FSCheckInStrategy();
        var retValue = false;

        var canContinue = true;

        var fileContent;

        var filePath = document.getElementById('OutFilePathController').value;
        var checkOutFilePath = strategy.GetFilePath(filePath);
        // Verifica esistenza del file
        if (!strategy.FileExists(checkOutFilePath)) {
            canContinue = confirm("Impossibile trovare il file '" + checkOutFilePath +
	            "'.\nRilasciare comunque il documento?");
        }
        else {
            // Verifica della sola dimensione del file, che deve essere valida rispetto al formato
            canContinue = strategy.ValidateFileSize(checkOutFilePath);

            try {
                // Reperimento contenuto del file
                fileContent = strategy.GetContent(checkOutFilePath);
                if (fileContent == null) {
                    alert("Il file correntemente bloccato potrebbe essere aperto in visualizzazione.\n" +
	                "E' necessario chiudere il file e ripetere l'operazione.");
                    canContinue = false;
                }
            }
            catch (ex) {
                alert("Il file correntemente bloccato potrebbe essere aperto in visualizzazione.\n" +
	                "E' necessario chiudere il file e ripetere l'operazione.");

                canContinue = false;
            }
        }

        if (isSignedFile) {
            canContinue = confirm("Attenzione!\nIl file correntemente bloccato è firmato digitalmente.\n" +
	            "Rilasciando il documento verrà creata una nuova versione non firmata.\n" +
	            "Si desidera continuare?");
        }

        if (canContinue && showCommentsPage) {
            // Visualizzazione maschera impostazione commenti per il checkin del documento
            canContinue = ShowDialogComments();
        }

        if (canContinue) {
            try {
                // Visualizzazione maschera di attesa
                if (showWaitingPage) {
                    //var waitingPage=ShowWaitingPage("<%=CheckInWaitingPageUrl%>");
                    ApreAttendi(msgCheckIn);
                }

                var http = new ActiveXObject("MSXML2.XMLHTTP");
                http.Open("POST", strategy.GetCheckInPageUrl("<%=CheckInOutFolderUrl%>"), false);
                http.send(fileContent);

                var response = http.responseText;

                retValue = (response == "");

                if (!retValue && showErrorMessage) {
                    // Visualizzazione dell'eventuale messaggio di errore
                    alert(response);
                }
                else if (strategy.FileExists(checkOutFilePath)) {
                    try {
                        // Rimozione file locale, solo se checkin andato a buon fine
                        strategy.DeleteFile(checkOutFilePath, true);
                    }
                    catch (ex) {
                        // Impossibile rimuovere il file locale, potrebbe essere aperto
                    }
                }
            }
            catch (ex) {
                alert("Errore nel rilascio del documento:\n" + ex.message.toString());
            }
            finally {
                if (showWaitingPage)
                    ChiudeAttendi();
            }
        }

        return retValue;
    }

    /*
    Rimozione del blocco impostato dall'utente stesso che lo ha effettuato 
    ed annulla le modiche effettuate (rimozione del file copiato localmente).
    */
    function UndoCheckOutDocument(showConfirm, showErrorMessage, showWaitingPage) {
        var retValue = false;
        var canContinue = true;

        var checkOutFilePath = document.getElementById('OutFilePathController').value;
        if (checkOutFilePath == "")
            checkOutFilePath = _tmpDownloadedFile;

        if (showConfirm) {
            // Visualizzazione messaggio di conferma annullamento
            canContinue = confirm('<%=CheckOutUndoConfirm %>');
        }

        if (canContinue) {
            try {
                // Visualizzazione maschera di attesa
                if (showWaitingPage)
                    ApreAttendi(msgUndoCheckOut);

                var http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", "<%=UndoCheckOutPageUrl%>", false);
                http.send();

                var response = http.responseText;

                retValue = (response == "");

                if (!retValue && showErrorMessage) {
                    // Visualizzazione messaggio di errore nell'undocheckout
                    alert(response);
                }
                else {
                    var fso = FsoWrapper_CreateFsoObject()

                    if (fso.FileExists(checkOutFilePath)) {
                        try {
                            // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                            fso.DeleteFile(checkOutFilePath, true);
                        }
                        catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                        }
                    }
                }
            }
            catch (ex) {
                alert("Errore nell'annullamento del blocco del documento:\n" + ex.message.toString());
            }
            finally {
                if (showWaitingPage)
                    ChiudeAttendi();
            }
        }

        return retValue;
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

    // Codifica stringa javascript
    function EncodeHtml(value) {
        value = escape(value);
        value = value.replace(/\//g, "%2F");
        value = value.replace(/\?/g, "%3F");
        value = value.replace(/=/g, "%3D");
        value = value.replace(/&/g, "%26");
        value = value.replace(/@/g, "%40");
        // Gabriele Melini 02-04-2014
        // bug lettere accentate
        value = value.replace(/%E0/g, "%C3%A0"); // à
        value = value.replace(/%E8/g, "%C3%A8"); // è
        value = value.replace(/%E9/g, "%C3%A9"); // é
        value = value.replace(/%EC/g, "%C3%AC"); // ì
        value = value.replace(/%F2/g, "%C3%B2"); // ò
        value = value.replace(/%F9/g, "%C3%B9"); // ù
        value = value.replace(/%A3/g, "%C2%A3"); // £
        value = value.replace(/%B0/g, "%C2%B0"); // °
        value = value.replace(/%A7/g, "%C2%A7"); // §

        return value;
    }

    // Visualizzazione informazioni di stato
    // sul documento checkedout
    function ShowDialogCheckOutStatus(idDocument, documentNumber) {
        var args = new Object;
        args.window = window;

        var retValue = window.showModalDialog("<%=CheckInOutFolderUrl%>CheckOutStatusPage.aspx?idDocument=" + idDocument + "&documentNumber=" + documentNumber,
											args,
											'dialogWidth:425px;dialogHeight:210px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
        return retValue;
    }

    // Selezione del percorso del file in cui fare checkout
    function ShowSaveDialogBox(defaultFilePath, fileType, title, visOpt) {
        var args = new Object;
        args.window = window;

        if (defaultFilePath != "") {
            // Encoding del path di default del file, se specificato
            defaultFilePath = EncodeHtml(defaultFilePath);
        }

        // Variabile utilizzata per indicare se devo visualizzare le
        // tre opzioni di salvataggio. Per default non visualizzo le
        // opzioni.
        var visualizzaOpzioni = "0";
        // Variabile utilizzata per determinare l'altezza della finestra
        // Per default l'altezza è quella con le opzioni non visualizzate
        var height = "200px";

        // Se è stato passato il parametro visOpt...
        if (visOpt != null) {
            // ...la visualizzazione delle opzioni di salvataggio
            // dipende dal valore di vis opt
            visualizzaOpzioni = visOpt;
        }

        // Se devo visualizzare le opzioni di salvataggio...
        if (visualizzaOpzioni == "1") {
            // ...l'altezza della finestra
            height = "450px";
        }

        var retValue = window.showModalDialog("<%=CheckInOutFolderUrl%>CheckInOutSaveDialog.aspx?visOpt=" + visualizzaOpzioni + "&fileName=" + defaultFilePath + "&fileType=" + fileType + "&title=" + title,
											args,
											"dialogWidth:380px;dialogHeight:" + height + ";status:no;resizable:no;scroll:no;center:yes;help:no;close:no");

        if (retValue == null)
            retValue = "";

        return retValue;
    }



    // Visualizzazione pagina di attesa per le operazioni di checkInCheckOut
    function ShowWaitingPage(waitingPageUrl) {
        var top = (screen.availHeight * 40) / 100
        var left = (screen.availWidth * 30) / 100

        return window.open(waitingPageUrl,
							"",
							"height=100,width=300,top=" + top + ",left=" + left);
    }
</script>
<uc1:SupportedFileTypeController ID="supportedFileTypeController" runat="server" />
<uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
<uc3:ShellWrapper ID="shellWrapper" runat="server" />
<uc4:FsoWrapper ID="fsoWrapper" runat="server" />
<% 
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "checkInOutControllerScript", string.Empty);
  } %>
<asp:UpdatePanel ID="pnlTempValue" UpdateMode="Conditional" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="OutFilePathController" ClientIDMode="Static" runat="server"/>
    </ContentTemplate>
</asp:UpdatePanel>