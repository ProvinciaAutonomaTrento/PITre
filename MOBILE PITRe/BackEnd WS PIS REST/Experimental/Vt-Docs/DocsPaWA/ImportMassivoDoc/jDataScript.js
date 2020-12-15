// Impostazione del focus su un controllo
function SetControlFocus(controlID) {
    try {
        var control = document.getElementById(controlID);

        if (control != null) {
            control.focus();
        }
    }
    catch (e)
		{ }
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

// Validazione dati immessi
function Validate() {
    var retValue = true;
    var fso = FsoWrapper_CreateFsoObject();
    var validationMessage = "";

    var selectedFolder = frmImportaDoc.txtFolderPath.value;
    if (selectedFolder == "") {
        alert("Selezionare la cartella da importare");
        frmImportaDoc.txtFirstInvalidControlID.value = "txtFolderPath";
        retValue = false;
    }
    // caso import doppione stessa dir
    var lastDirSelect = frmImportaDoc.lastDirSelection.value;
    if (lastDirSelect != "") {
        if (selectedFolder == lastDirSelect) {
            if (!confirm("La cartella selezionata coincide con la precedente cartella già importata. Continuare?")) {
                retValue = false;
            }

        }

    }
    return retValue;
}

// Reperimento percorso correntemente selezionato
function GetSelectedFilePath() {
    var selectedPath = frmImportaDoc.txtFolderPath.value;

    if (selectedPath != "") {
        var fso = FsoWrapper_CreateFsoObject();

        var folder = fso.GetFolder(selectedPath);

        if (folder != null) {
            selectedPath = folder.Path;

            if (selectedPath.charAt(selectedPath.length - 1) != "\\")
                selectedPath = selectedPath + "\\";
        }
    }

    return selectedPath;
}

function PerformSelectFolder() {
    var folder = ShellWrappers_BrowseForFolder("Scegliere la cartella da importare");

    if (folder != "") {
        var fso = FsoWrapper_CreateFsoObject();
        if (fso.FolderExists(folder)) {
            frmImportaDoc.txtFolderPath.value = folder;
            frmImportaDoc.txtFirstInvalidControlID.value = "txtFileName";
        }
    } else {
        return;
    }
}

var wndAttendi;

function invia() {

    try {
        if (Validate() == false) return;
        else {
            document.getElementById('btnInvia').style.display = 'none';
            ShowWaitingPage("L'importazione può richiedere alcuni minuti...");
            listadir();
            frmImportaDoc.lastDirSelection.value = frmImportaDoc.txtFolderPath.value;
        }
    }
    catch (e) {
        alert(e.message.toString());
    }

    CloseWaitingPage();
}

function ShowWaitingPage(msg) {
    //wndAttendi = window.open('tempPage.aspx', '', 'toolbar=0,scrollbars=0,resizable=0,width=350,height=10,left=350,top=350');
    wndAttendi = window.open('tempPage.aspx', '_blank', "location=0,toolbar=0,scrollbars=0,resizable=0,width=350,height=10,left=350,top=350");
}

function CloseWaitingPage() {
    if (wndAttendi != null)
        wndAttendi.close();
}

//FUNZIONI RICORSIVE DI INDAGINE CARTELLE E FILE FSO
var PathArray = new Array();
var itr = 0;
function listadir() {

    var pathRootFolder = frmImportaDoc.txtFolderPath.value
    if (pathRootFolder == "") {
        alert('Cartella non selezionata.');
        return;
    }
    else {
        DeleteMetaFile();
        PathArray[1] = pathRootFolder;
        loadFolderAndFile(PathArray, 1, 2, pathRootFolder);
        SendMetaFile();
    }
}

function loadFolderAndFile(PathArray, itr, nextFree, SelectedFolder) {
    var newitr = nextFree;
    var fso = FsoWrapper_CreateFsoObject();
    if (fso.FolderExists(PathArray[itr])) {
        var fold = fso.GetFolder(PathArray[itr]);
    }
    else {
        return;
    }
    FileEnum = new Enumerator(FsoWrapper_GetFiles(fold));
    FolderEnum = new Enumerator(FsoWrapper_GetFolders(fold));
    try {

        //ciclo di scansione Files della root Folder
        if (FolderEnum.atEnd()) {
            if (!FileEnum.atEnd()) {
                for (; !FileEnum.atEnd(); FileEnum.moveNext()) {
                    //passo il content del file se esiste un file
                    var content = AdoStreamWrapper_OpenBinaryData(FileEnum.item());
                    pushContentAndMetadata(content, FileEnum.item(), SelectedFolder, "FILE");
                }
            }
        }
        //ciclo di scansione di sotto directory
        for (; !FolderEnum.atEnd(); FolderEnum.moveNext()) {
            if (!FileEnum.atEnd()) {
                //ciclo di scansione Files
                for (; !FileEnum.atEnd(); FileEnum.moveNext()) {
                    //content del file
                    var content = AdoStreamWrapper_OpenBinaryData(FileEnum.item());
                    pushContentAndMetadata(content, FileEnum.item(), SelectedFolder, "FILE");
                }
            }
            else {
                if (FolderEnum.item() != null)
                    pushContentAndMetadata(null, FolderEnum.item(), SelectedFolder, "DIR");
            }

            //stampa  directory
            PathArray[newitr] = FolderEnum.item();
            newitr++;
        }
        loadFolderAndFile(PathArray, itr + 1, newitr, SelectedFolder);
    }
    catch (e) {
        alert(e.message.toString());
        return;
    }
}

function pushContentAndMetadata(contentFile, metaData, name, type) {

    var http = new ActiveXObject("MSXML2.XMLHTTP");
    var content = contentFile;
    http.Open("POST", "importaDoc.aspx?Absolutepath=" + Url.encode(metaData) + "&codFasc=" + Url.encode(document.getElementById('codFasc').value) + "&foldName=" + Url.encode(name) + "&type=" + type, false);
    http.send(content);
    
    try {
        var message = http.statusText;
        AppendLine("TS=" + getTime() + "|Type=" + type + "|Absolutepath=" + metaData + "|ErrorMessage=" + message);
    }
    catch (ex) {
        alert(ex.message.toString());
    }
    
    /*
    try {
        var errorMessage = "";

        // Controllo esito
        if (http.status != 200 &&
        		    http.statusText != null && http.statusText != "") {
            // Si è verificato un errore, reperimento del messaggio
            errorMessage = http.statusText;
        }

        AppendLine("TS=" + getTime() + "|Type=" + type + "|Absolutepath=" + metaData + "|ErrorMessage=" + errorMessage);
    }
    catch (ex) {
        alert(ex.message.toString());
    }
    */
    
    return;
}
function GetMetaFilePath() {
    // Scrittura dati file mediante fsowrapper
    var fso = FsoWrapper_CreateFsoObject();
    return fso.GetSpecialFolder(2).Path + "\\importaDoc.txt";
}

function DeleteMetaFile() {
    // Scrittura dati file mediante fsowrapper
    var fso = FsoWrapper_CreateFsoObject();
    var path = GetMetaFilePath();
    if (fso.FileExists(path))
        fso.DeleteFile(path, true);
}

function AppendLine(value) {
    // Scrittura dati file mediante fsowrapper
    var fso = FsoWrapper_CreateFsoObject();

    var stream = null;

    var path = GetMetaFilePath();
    if (fso.FileExists(path))
        stream = fso.OpenTextFile(path, 8);
    else
        stream = fso.CreateTextFile(path);

    stream.WriteLine(value);

    FsoWrapper_CloseFsoStreamObject(stream);
}

function ReadMetaFile() {
    var content = "";

    var path = GetMetaFilePath();
    var fso = FsoWrapper_CreateFsoObject();

    if (fso.FileExists(path)) {
        // Lettura contenuto file di metadati
        var stream = fso.OpenTextFile(path, 1);
        content = stream.ReadAll();
        FsoWrapper_CloseFsoStreamObject(stream);
    }

    return content;
}

function SendMetaFile() {
    // Lettura contenuto file di metadati
    document.getElementById("hdMetaFileContent").value = ReadMetaFile();
    frmImportaDoc.submit();
}

function getTime() {
    var Digital = new Date();
    var ore = Digital.getHours();
    var minuti = Digital.getMinutes();
    var secondi = Digital.getSeconds();
    if (ore <= 9) ore = "0" + ore;
    if (minuti <= 9) minuti = "0" + minuti;
    if (secondi <= 9) secondi = "0" + secondi;
    var myclock = ore + ":" + minuti + ":" + secondi;
    return myclock;
}

//url encode dati
var Url = {

    // public method for url encoding
    encode: function(string) {
        return escape(this._utf8_encode(string));
    },


    // private method for UTF-8 encoding
    _utf8_encode: function(string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    }
}

function lanciaVisPdf() {
    var w = window.screen.availWidth;
    var h = window.screen.availHeight;
    var dimensionWindow = "width=" + w + ",height=" + h;
    window.showModalDialog('visPdfReportFrame.aspx', '', 'dialogWidth:' + w + ';dialogHeight:' + h + ';status:no;resizable:yes;scroll:no;center:no;help:no;close:no;top:' + 0 + ';left:' + 0);
}

function chiudiWin() {
    window.close();
    window.opener.document.fascDocumenti.submit();
}

		

