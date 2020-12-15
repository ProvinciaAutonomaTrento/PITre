var CHIUDI = "CLOSE";
/*var fsoURL = 'ws://localhost:4444/fso';
var scannURL = 'ws://localhost:4444/scann';
var signURL = 'ws://localhost:4444/sign';
var printURL = 'ws://localhost:4444/print';
*/

var fsoURL = 'ws://localhost:4444';
var scannURL = 'ws://localhost:4444';
var signURL = 'ws://localhost:4444';
var printURL = 'ws://localhost:4444';
var modelURL = 'ws://localhost:4444';

var TIPO_APPLET = {

    FSO: 0,
    SCANNER: 1,
    PRINTER: 2,
    SIGNER: 3,
    MODEL: 4
};

var FSO_METHOD = {

    TEST_CHIAMATA: 0,
    SELECT_FOLDER: 1,
    FOLDER_EXISTS: 2,
    FILE_EXISTS: 3,
    CREATE_FOLDER: 4,
    GET_FILES: 5,
    GET_FOLDERS: 6,
    GET_UNIQUE_FILE_NAME: 7,
    GET_TEMP_NAME: 8,
    GET_SPECIAL_FOLDER: 9,
    GET_EXTENSION_NAME: 10,
    DELETE_FILE: 11,
    CREATE_EMPTY_FILE: 12,
    CREATE_TEXT_FILE: 13,
    COPY_TO_CLIP_BOARD: 14,
    OPEN_FILE: 15,
    SAVE_FILE: 16,
    SAVE_FILE_FROM_URL: 17,
    SEND_FILE: 18,
    SEND_FILE_TO_URL: 19,
    GET_ENCODED_CONTENT: 20,
    PROJECT_TO_FS: 21,
    GET_FILE_FROM_PATH : 22

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
    PROCESS_MODEL: 1,
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

//File system applet method

function testChiamataFSO(callback) {

    try{
        var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] });
		if(!callback){
			callback = function(msg,connection){connection.close();};
		}
	    var wsFSO = new WebSocket(fsoURL);
	    invokeMethod(message,callback,wsFSO);
    } catch (ex) {
        throw ex;
    }
}

function selectFolder(choosertitle, startPath, callback) {


    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SELECT_FOLDER, "Parametri": [choosertitle, startPath] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message, callback, wsFSO);

}

function folderExists(path,callback) {

    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.FOLDER_EXISTS, "Parametri": [path] });
   if(!callback){
		callback = function(msg,connection){connection.close();};
   }
   var wsFSO = new WebSocket(fsoURL);
	invokeMethod(message,callback,wsFSO);

}

function fileExists(path,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.FILE_EXISTS, "Parametri": [path] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function createFolder(path,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.CREATE_FOLDER, "Parametri": [path] });
    if(!callback){
		callback = function(msg,connection){connection.close();};
    }
    var wsFSO = new WebSocket(fsoURL);
	invokeMethod(message,callback,wsFSO);
}

function getFiles(path,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_FILES, "Parametri": [path] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function getFolders(path,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_FOLDERS, "Parametri": [path] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function getUniqueFileName(prefix, extension) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_UNIQUE_FILE_NAME, "Parametri": [prefix, extension] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function getTempName(path,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_TEMP_NAME, "Parametri": [null] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function getSpecialFolder(callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_SPECIAL_FOLDER, "Parametri": [null] });
    var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function getExtensionName(path,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_EXTENSION_NAME, "Parametri": [path] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function deleteFile(path, force, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.DELETE_FILE, "Parametri": [path, force] });
    var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function createEmptyFile(path, overwrite, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.CREATE_EMPTY_FILE, "Parametri": [path, overwrite] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function createTextFile(path, overwrite, text, callback) {

    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.CREATE_TEXT_FILE, "Parametri": [path, overwrite, text] });
    var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}


function copyToClipBoard(text,callback) {

    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.COPY_TO_CLIP_BOARD, "Parametri": [text] });
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    var wsFSO = new WebSocket(fsoURL);
    invokeMethod(message, callback, wsFSO);
}

function openFile(path, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.OPEN_FILE, "Parametri": [path] });
    var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function saveFile(path, encodedText, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SAVE_FILE, "Parametri": [path, encodedText] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function saveFileFromURL(path, url, parameters, sessId, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SAVE_FILE_FROM_URL, "Parametri": [path, url, parameters, sessId] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function sendFile(path, url, sessionID, callback) {
    try {
        var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SEND_FILE, "Parametri": [path, url, sessionID] });
        var wsFSO = new WebSocket(fsoURL);
        invokeMethod(message, callback, wsFSO);
    } catch (ex) {
        throw ex;
    }
}

function sendFiletoURL(path, url, callback) {
    try{
        var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SEND_FILE_TO_URL, "Parametri": [path, url] });
        var wsFSO = new WebSocket(fsoURL);
        invokeMethod(message,callback,wsFSO);
    } catch (ex) {
        throw ex;
    }
}

function getEncodedContent(path, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_ENCODED_CONTENT, "Parametri": [path] });
	var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}

function projectToFS(path, xmlProject, urlDoc, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.PROJECT_TO_FS, "Parametri": [path, xmlProject, urlDoc] });
    var wsFSO = new WebSocket(fsoURL);
	if(!callback){
		callback = function(msg,connection){connection.close();};
	}
	invokeMethod(message,callback,wsFSO);
}


function getFileFromPath(fileName, url, callback) {
    try {
        var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_FILE_FROM_PATH, "Parametri": [fileName, url] });
        var wsFSO = new WebSocket(fsoURL);
        invokeMethod(message, callback, wsFSO);
    } catch (ex) {
        throw ex;
    }
}



//Scanner Applet method

function testChiamataSCANNER() {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] });
    var wsScann = new WebSocket(scannURL);
        if(!callback){
            callback = function(msg,connection){connection.close();};
        }
        invokeMethod(message,callback,wsScann);
}

function initScanningDialog(callback) {

    try{
        var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.INIT_SCANNING_DIALOG, "Parametri": [null] });
        var wsScann = new WebSocket(scannURL);
        if(!callback){
            callback = function(msg,connection){connection.close();};
        }
        invokeMethod(message,callback,wsScann);
    } catch (ex) {
        console.log(ex);
    }
}

function setFileType(type,callback) {

    try{
            var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.SET_FILE_TYPE, "Parametri": [type] });
            
	        var wsScann = new WebSocket(scannURL);
	        if(!callback){
	        	callback = function(msg,connection){connection.close();};
	        }
	        invokeMethod(message, callback, wsScann);
        } catch (ex) {
            throw ex;
        }
}

function setQuality(quality, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.SET_QUALITY, "Parametri": [quality] });
    var wsScann = new WebSocket(scannURL);
        if(!callback){
            callback = function(msg,connection){connection.close();};
        }
        invokeMethod(message,callback,wsScann);

}

//Printer Applet method

function testChiamataPRINTER() {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.PRINTER, "Metodo": PRINTER_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] });
     var wsPrint = new WebSocket(printURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsPrint);
}

function printA(printdata, printerFilter, callback) {

    //printerFilter = 'ZDesigner TLP 3844-Z';
    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.PRINTER, "Metodo": PRINTER_METHOD.PRINT, "Parametri": [printdata, printerFilter] });
    var wsPrint = new WebSocket(printURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsPrint);
}


//Signer Applet method

function testChiamataSIGNER() {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function setCheckCertificate(b,callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SET_CHECK_CERTIFICATE, "Parametri": [b] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function close() {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.CLOSE, "Parametri": [null] });
   var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function killApplet() {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.KILL_APPLET, "Parametri": [null] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function getCertificateListAsJsonFormat(storeLocation, storeName, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.GET_CERTIFICATE_LIST_AS_JSON_FORMAT, "Parametri": [storeLocation, storeName] });
	var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function signDataFromPath(phatfile, serialNumber, applyCosign, storeLocation, storeName, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_DATA_FROM_PATH, "Parametri": [phatfile, serialNumber, applyCosign, storeLocation, storeName]});
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function signData(contentEnc, certIndex, applyCosign, storeLocation, storeName, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_DATA_INDEX, "Parametri": [contentEnc, certIndex, applyCosign, storeLocation, storeName] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function signData(contentEnc, serialNumber, applyCosign, storeLocation, storeName, callback) {

    var message =  JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_DATA_SERIAL, "Parametri": [contentEnc, serialNumber, applyCosign, storeLocation, storeName] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function signHash(hashEnc, varcertSerial, applyCosign, storeLocation, storeName, callback) {

    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_HASH, "Parametri": [hashEnc, varcertSerial, applyCosign, storeLocation, storeName] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

//Model Applet method

function testChiamataMODEL() {

    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.MODEL, "Metodo": SIGNER_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] });
    var wsSign = new WebSocket(signURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsSign);
}

function processModel(m_documentId, m_modelType, xmlResponse, outputFilePath, pathNoName, callback) {

    var message = JSON.stringify({ "GUID": guid(), "Classe": TIPO_APPLET.MODEL, "Metodo": SIGNER_METHOD.SET_CHECK_CERTIFICATE, "Parametri": [m_documentId, m_modelType, xmlResponse, outputFilePath, pathNoName]});
    var wsModel = new WebSocket(modelURL);
    if (!callback) {
        callback = function (msg, connection) { connection.close(); };
    }
    invokeMethod(message, callback, wsModel);
}

function disconnect() {
    console.log("Disconnect");
    var message =  JSON.stringify({ "GUID": guid(), "Classe": -1, "Metodo": -1, "Parametri": [CHIUDI] });
    //invokeMethod(message); return JSON.stringify(message)
    port.disconnect();
    port = null;
}

function invokeMethod(message, callback, connection) {

    try{
        connection.onopen = function () { /*alert('Connected!');*/ connection.send(message); };
        connection.onclose = function () {
            //alert('Lost connection');
            if (connection.CLOSED === connection.readyState) {
                //reallowOp();
            }
        };
        connection.onmessage = function(event) { 
	
            var msg = JSON.parse(event.data);
            switch (msg.STATUS) {
                case 'SUCCESS': callback(msg.MESSAGE, connection);
                    break;
                case 'ERROR': throw msg.MESSAGE;
            }
        };
    } catch (Err) {
        throw Err;

    }
}