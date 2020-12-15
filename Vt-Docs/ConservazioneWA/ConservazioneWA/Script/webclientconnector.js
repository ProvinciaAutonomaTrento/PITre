var CHIUDI = "CLOSE";
/*var fsoURL = 'ws://localhost:4444/fso';
var scannURL = 'ws://localhost:4444/scann';
var signURL = 'ws://localhost:4444/sign';
var printURL = 'ws://localhost:4444/print';
*/

var connectionCount = 0;
var MAX_CONNECTIONS = 29;

var fsoURL = 'wss://localhost:12345';
var scannURL = 'wss://localhost:12345';
var signURL = 'wss://localhost:12345';
var printURL = 'wss://localhost:12345';
var modelURL = 'wss://localhost:12345';
var url = 'wss://localhost:12345';
var ISMSIE = isMsie();

var TIPO_APPLET = {
    FSO: 0,
    SCANNER: 1,
    PRINTER: 2,
    SIGNER: 3,
    MODEL: 4,
    HELPER: 5
};

var FSO_METHOD = {

    TEST_CHIAMATA: 0,
    SELECT_FOLDER: 1,
    FOLDER_EXISTS: 2,
    FILE_EXISTS: 3,
    CREATE_FOLDER: 4,
    GET_FILES: 5,
    GET_FOLDERS: 6,
    GET_ELEMENT_COUNT: 7,
    GET_UNIQUE_FILE_NAME: 8,
    GET_TEMP_NAME: 9,
    GET_SPECIAL_FOLDER: 10,
    GET_EXTENSION_NAME: 11,
    DELETE_FILE: 12,
    CREATE_EMPTY_FILE: 13,
    CREATE_TEXT_FILE: 14,
    COPY_TO_CLIP_BOARD: 15,
    OPEN_FILE: 16,
    SAVE_FILE: 17,
    SAVE_FILE_FROM_URL: 18,
    SEND_FILE: 19,
    SEND_FILE_TO_URL: 20,
    GET_ENCODED_CONTENT: 21,
    PROJECT_TO_FS: 22,
    GET_FILE_FROM_PATH : 23

};

var SCANN_METHOD = {

    TEST_CHIAMATA: 0,
    INIT_SCANNING_DIALOG: 1,
    SET_FILE_TYPE: 2,
    SET_QUALITY: 3
};

var PRINTER_METHOD = {

    TEST_CHIAMATA: 0,
    PRINT: 1
};

var SIGNER_METHOD = {

    TEST_CHIAMATA: 0,
    SET_CHECK_CERTIFICATE: 1,
    CLOSE: 2,
    KILL_APPLET: 3,
    GET_CERTIFICATE_LIST_AS_JSON_FORMAT: 4,
    SIGN_DATA_FROM_PATH: 5,
    SIGN_DATA_INDEX: 6,
    SIGN_DATA_SERIAL: 7,
    SIGN_HASH: 8
};

var MODEL_METHOD = {

    TEST_CHIAMATA: 0,
    PROCESS_MODEL: 1
};

var HELPER_METHOD = {

    VERSION: 0,
    CLOSE: 1
};

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
      .toString(16)
      .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
    s4() + '-' + s4() + s4() + s4();
}
//var appInstalled = detectChromeExtension(editorExtensionId, 'manifest.json', myCallbackFunction);

//console.log("Installed : "+appInstalled);

/*if (response) {
console.log(txtArea);
txtArea.innerHTML = response;
}*/




function getVersion(onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.HELPER, METHOD: HELPER_METHOD.VERSION, PARAMETERS: [] });
    connectionManager(message, onSuccess, onError);

}

//File system applet method

function testChiamataFSO(onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    try{
        var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.TEST_CHIAMATA, PARAMETERS: ['prova'] });
        connectionManager(message, onSuccess, onError);
    } catch (ex) {
        throw ex;
    }
}

function selectFolder(choosertitle, startPath, onSuccess, onError) {

    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.SELECT_FOLDER, PARAMETERS: [choosertitle, startPath] });
    connectionManager(message, onSuccess, onError);

}

function folderExists(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.FOLDER_EXISTS, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);

}

function fileExists(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.FILE_EXISTS, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function createFolder(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.CREATE_FOLDER, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function getFiles(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_FILES, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function getFolders(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_FOLDERS, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function getElementCount(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_ELEMENT_COUNT, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function getUniqueFileName(prefix, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_UNIQUE_FILE_NAME, PARAMETERS: [prefix, extension] });
    connectionManager(message, onSuccess, onError);
}

function getTempName(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_TEMP_NAME, PARAMETERS: [null] });
    connectionManager(message, onSuccess, onError);
}

function getSpecialFolder(onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_SPECIAL_FOLDER, PARAMETERS: [null] });
    connectionManager(message, onSuccess, onError);
}


function getExtensionName(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_EXTENSION_NAME, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function deleteFile(path, force, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.DELETE_FILE, PARAMETERS: [path, force] });
    connectionManager(message, onSuccess, onError);
}

function createEmptyFile(path, overwrite, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.CREATE_EMPTY_FILE, PARAMETERS: [path, overwrite] });
    connectionManager(message, onSuccess, onError);
}

function createTextFile(path, overwrite, text, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.CREATE_TEXT_FILE, PARAMETERS: [path, overwrite, text] });
    connectionManager(message, onSuccess, onError);
}


function copyToClipBoard(text, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.COPY_TO_CLIP_BOARD, PARAMETERS: [text] });
    connectionManager(message, onSuccess, onError);
}

function openFile(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.OPEN_FILE, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function saveFile(path, encodedText, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.SAVE_FILE, PARAMETERS: [path, encodedText] });
    connectionManager(message, onSuccess, onError);
}

function saveFileFromURL(path, url, parameters, sessId, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.SAVE_FILE_FROM_URL, PARAMETERS: [path, url, parameters, sessId] });
    connectionManager(message, onSuccess, onError);
}

function sendFile(path, url, sessionID, onSuccess, onError) {
    try {
        onSuccess = onSuccess || null;
        onError = onError || null;
        var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.SEND_FILE, PARAMETERS: [path, url, sessionID] });
        connectionManager(message, onSuccess, onError);
    } catch (ex) {
        throw ex;
    }
}

function sendFiletoURL(path, url, onSuccess, onError) {
    try{
        onSuccess = onSuccess || null;
        onError = onError || null;
        var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.SEND_FILE_TO_URL, PARAMETERS: [path, url] });
        connectionManager(message, onSuccess, onError);
    } catch (ex) {
        throw ex;
    }
}

function getEncodedContent(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_ENCODED_CONTENT, PARAMETERS: [path] });
    connectionManager(message, onSuccess, onError);
}

function projectToFS(path, xmlProject, urlDoc, onSuccess, onError) {

    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.PROJECT_TO_FS, PARAMETERS: [path, xmlProject, urlDoc] });
    connectionManager(message, onSuccess, onError);
}


function getFileFromPath(fileName, url, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.GET_FILE_FROM_PATH, PARAMETERS: [fileName, url] });
    connectionManager(message, onSuccess, onError);

}



//Scanner Applet method

function testChiamataSCANNER() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SCANNER, METHOD: SCANN_METHOD.TEST_CHIAMATA, PARAMETERS: ['prova'] });
    connectionManager(message, onSuccess, onError);
}

function initScanningDialog(onSuccess, onError) {

    try{
        onSuccess = onSuccess || null;
        onError = onError || null;
        var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SCANNER, METHOD: SCANN_METHOD.INIT_SCANNING_DIALOG, PARAMETERS: [null] });
        connectionManager(message, onSuccess, onError);
    } catch (ex) {
        console.log(ex);
    }
}

function setFileType(type, onSuccess, onError) {

    try{
            onSuccess = onSuccess || null;
            onError = onError || null;
            var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SCANNER, METHOD: SCANN_METHOD.SET_FILE_TYPE, PARAMETERS: [type] });
            connectionManager(message, onSuccess, onError);
        } catch (ex) {
            throw ex;
        }
}

function setQuality(quality, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SCANNER, METHOD: SCANN_METHOD.SET_QUALITY, PARAMETERS: [quality] });
    connectionManager(message, onSuccess, onError);

}

//Printer Applet method

function testChiamataPRINTER() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.PRINTER, METHOD: PRINTER_METHOD.TEST_CHIAMATA, PARAMETERS: ['prova'] });
    connectionManager(message, onSuccess, onError);
}

function printA(printdata, printerFilter, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    //printerFilter = 'ZDesigner TLP 3844-Z';
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.PRINTER, METHOD: PRINTER_METHOD.PRINT, PARAMETERS: [printdata, printerFilter] });
    connectionManager(message, onSuccess, onError);
}


//Signer Applet method

function testChiamataSIGNER() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.TEST_CHIAMATA, PARAMETERS: ['prova'] });
    connectionManager(message, onSuccess, onError);
}

function setCheckCertificate(b, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.SET_CHECK_CERTIFICATE, PARAMETERS: [b] });
    connectionManager(message, onSuccess, onError);
}

function close() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.CLOSE, PARAMETERS: [null] });
    connectionManager(message, onSuccess, onError);
}

function killApplet() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.KILL_APPLET, PARAMETERS: [null] });
    connectionManager(message, onSuccess, onError);
}

function getCertificateListAsJsonFormat(storeLocation, storeName, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.GET_CERTIFICATE_LIST_AS_JSON_FORMAT, PARAMETERS: [storeLocation, storeName] });
    connectionManager(message, onSuccess, onError);
}

function signDataFromPath(phatfile, serialNumber, applyCosign, storeLocation, storeName, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.SIGN_DATA_FROM_PATH, PARAMETERS: [phatfile, serialNumber, applyCosign, storeLocation, storeName]});
    connectionManager(message, onSuccess, onError);
}

function signData(contentEnc, certIndex, applyCosign, storeLocation, storeName, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.SIGN_DATA_INDEX, PARAMETERS: [contentEnc, certIndex, applyCosign, storeLocation, storeName] });
    connectionManager(message, onSuccess, onError);
}

function signData(contentEnc, serialNumber, applyCosign, storeLocation, storeName, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message =  JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.SIGN_DATA_SERIAL, PARAMETERS: [contentEnc, serialNumber, applyCosign, storeLocation, storeName] });
    connectionManager(message, onSuccess, onError);
}

function signHash(hashEnc, varcertSerial, applyCosign, storeLocation, storeName, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.SIGNER, METHOD: SIGNER_METHOD.SIGN_HASH, PARAMETERS: [hashEnc, varcertSerial, applyCosign, storeLocation, storeName] });
    connectionManager(message, onSuccess, onError);
}

//Model Applet method

function testChiamataMODEL() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.MODEL, METHOD: SIGNER_METHOD.TEST_CHIAMATA, PARAMETERS: ['prova'] });
    connectionManager(message, onSuccess, onError);
}

function processModel(m_documentId, m_modelType, xmlResponse, outputFilePath, pathNoName, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.MODEL, METHOD: SIGNER_METHOD.SET_CHECK_CERTIFICATE, PARAMETERS: [m_documentId, m_modelType, xmlResponse, outputFilePath, pathNoName]});
    connectionManager(message, onSuccess, onError);
}


function disconnect() {
    onSuccess = onSuccess || null;
    onError = onError || null;
    console.log("Disconnect");
    var message =  JSON.stringify({ GUID: guid(), CLASS: -1, METHOD: -1, PARAMETERS: [CHIUDI] });
    //invokeMethod(message); return JSON.stringify(message)
    port.disconnect();
    port = null;
}

function invokeMethod(message, onSuccess, onError, connection) {

    try {
        connection.onopen = function () { /*alert('Connected!');*/ connection.send(message); };
        connection.onclose = function (event) {
            //alert('Lost connection');
            connectionCount--;
            if (connection.CLOSED === connection.readyState) {
                //reallowOp();
            }
            console.log('CLOSE - init connectionCount: ' + connectionCount);
        };
        connection.onerror = function OnSocketError(ev) {
            onError(ev, connection);
        }
        connection.onmessage = function(event) { 
            connection.close();
            var msg = JSON.parse(event.data);
            switch (msg.STATUS) {
                case 'SUCCESS': onSuccess(msg.MESSAGE, connection);
                    break;
                case 'ERROR': throw msg.MESSAGE;
            }
        };
    } catch (Err) {
        throw Err;

    }
}


function connectionManager(message, onSuccess, onError) {
    var connection = {};
    var maxConnections = MAX_CONNECTIONS;

    console.log('connectionManager - init connectionCount: ' + connectionCount + ' ISMSIE: ' + ISMSIE);
    if (ISMSIE)
        maxConnections = 1;
    if (connectionCount > maxConnections) {
        setTimeout(
            function () {
                connectionManager(message, onSuccess, onError)
            },
        100);
    } else {
        connectionCount++;
        connection = new WebSocket(url);
        if (!onSuccess) {
            onSuccess = function (msg, connection) { connection.close(); };
        }
        if (!onError) {
            onError = function (msg, connection) { connection.close(); };
        }
        invokeMethod(message, onSuccess, onError, connection);
    }
    console.log('connectionManager - end connectionCount: ' + connectionCount + ' ISMSIE: ' + ISMSIE);
}

function isMsie() {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    return (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
}



