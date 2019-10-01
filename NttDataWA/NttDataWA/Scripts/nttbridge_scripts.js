
    console.log("Init");
    //var sendMessage = document.getElementById("btnSendMessage");
    var editorExtensionId = "knldjmfmopnpolahpmmgbagdohdnhkik";
    var port = null;
    var bridge_response = null;

    var CHIUDI = "CLOSE";
   
    var TIPO_APPLET = {
                           FSO : 0,
                           SCANNER : 1, 
                           PRINTER : 2, 
                           SIGNER : 3
                       };


    var FSO_METHOD = {
                          TEST_CHIAMATA : 0, 
                          SELECT_FOLDER : 1, 
                          FOLDER_EXISTS : 2, 
                          FILE_EXISTS : 3, 
                          CREATE_FOLDER : 4, 
                          GET_FILES : 5,
                          GET_FOLDERS : 6, 
                          GET_UNIQUE_FILE_NAME : 7, 
                          GET_TEMP_NAME : 8, 
                          GET_SPECIAL_FOLDER : 9, 
                          GET_EXTENSION_NAME : 10,
                          DELETE_FILE : 11,
                          CREATE_EMPTY_FILE : 12, 
                          CREATE_TEXT_FILE : 13, 
                          COPY_TO_CLIP_BOARD : 14, 
                          OPEN_FILE : 15, 
                          SAVE_FILE : 16,
                          SAVE_FILE_FROM_URL : 17, 
                          SEND_FILE_TO_URL : 18, 
                          GET_ENCODED_CONTENT : 19, 
                          PROJECT_TO_FS : 20
                      };

    var SCANN_METHOD = {
                            INIT_SCANNING_DIALOG : 0, 
                            SET_FILE_TYPE : 1, 
                            SET_QUALITY : 2
                        };

    var PRINTER_METHOD = {
                             TEST_CHIAMATA: 0, 
                             PRINT : 1
                         };


   
   var SIGNER_METHOD = {
                            TEST_CHIAMATA : 0, 
                            SET_CHECK_CERTIFICATE : 1, 
                            CLOSE : 2, 
                            KILL_APPLET : 3, 
                            GET_CERTIFICATE_LIST_AS_JSON_FORMAT : 4, 
                            SIGN_DATA_FROM_PATH : 5,
                            SIGN_DATA_INDEX : 6, 
                            SIGN_DATA_SERIAL : 7, 
                            SIGN_HASH : 8
                       };
   
    function connect() {

        console.log("Connessione");

        port = chrome.runtime.connect(editorExtensionId);
        console.log(port);
        if (port) {
            port.onDisconnect.addListener(onDisconnected);
            port.onMessage.addListener(onMessage);
        }
    }

//File system applet method

    function testChiamataFSO() {

          var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] };
          invokeMethod(message);
    }


    function selectFolder(choosertitle, startPath) {


        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SELECT_FOLDER, "Parametri": [choosertitle, startPath] };
        invokeMethod(message);

    }

    function folderExists(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.FOLDER_EXISTS, "Parametri": [path] };
        invokeMethod(message);

    }

    function fileExists(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.FILE_EXISTS, "Parametri": [path] };
        invokeMethod(message);
    }

    function createFolder(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.CREATE_FOLDER, "Parametri": [path] };
        invokeMethod(message);
    }

    function getFiles(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_FILES, "Parametri": [path] };
        invokeMethod(message);
    }

    function getFolders(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_FOLDERS, "Parametri": [path] };
        invokeMethod(message);
    }

    function getUniqueFileName(prefix, extension) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_UNIQUE_FILE_NAME, "Parametri": [prefix, extension] };
        invokeMethod(message);
    }

    function getTempName(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_TEMP_NAME, "Parametri": [null] };
        invokeMethod(message);
    }

    function getSpecialFolder() {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_SPECIAL_FOLDER, "Parametri": [null] };
        invokeMethod(message);
    }

    function getExtensionName(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_EXTENSION_NAME, "Parametri": [path] };
        invokeMethod(message);
    }

    function deleteFile(path, force) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.DELETE_FILE, "Parametri": [path, force] };
        invokeMethod(message);
    }

    function createEmptyFile(path, overwrite) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.CREATE_EMPTY_FILE, "Parametri": [path, overwrite] };
        invokeMethod(message);
    }

    function createTextFile(path, overwrite, text) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.CREATE_TEXT_FILE, "Parametri": [path, overwrite, text] };
        invokeMethod(message);
    }

  
    function copyToClipboard(text) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.COPY_TO_CLIP_BOARD, "Parametri": [text] };
        invokeMethod(message);
    }

    function openFile(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.OPEN_FILE, "Parametri": [path] };
        invokeMethod(message);
    }

    function saveFile(path, encodedText) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SAVE_FILE, "Parametri": [path, encodedText] };
        invokeMethod(message);
    }

    function saveFileFromURL(path, url, parameters, sessId) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SAVE_FILE_FROM_URL, "Parametri": [path, url, parameters, sessId] };
        invokeMethod(message);
    }

    function sendFiletoURL(path, url) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.SEND_FILE_TO_URL, "Parametri": [path, url] };
        invokeMethod(message);
    }

    function getEncodedContent(path) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.GET_ENCODED_CONTENT, "Parametri": [path] };
        invokeMethod(message);
    }

    function projectToFS(path, xmlProject, urlDoc) {

        var message = { "Classe": TIPO_APPLET.FSO, "Metodo": FSO_METHOD.PROJECT_TO_FS, "Parametri": [path, xmlProject, urlDoc] };
        invokeMethod(message);
    }

    //Scanner Applet method

    function testChiamataSCANNER() {

        var message = { "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] };
        invokeMethod(message);
    }

    function initScanningDialog() {

        var message = { "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.INIT_SCANNING_DIALOG, "Parametri": [null] };
        invokeMethod(message);

    }

    function setFileType(type) {

        var message = { "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.SET_FILE_TYPE, "Parametri": [type] };
        invokeMethod(message);

    }

    function setQuality(quality) {

        var message = { "Classe": TIPO_APPLET.SCANNER, "Metodo": SCANN_METHOD.SET_QUALITY, "Parametri": [quality] };
        invokeMethod(message);

    }

    //Printer Applet method

    function testChiamataPRINTER() {

        var message = { "Classe": TIPO_APPLET.PRINTER, "Metodo": PRINTER_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] };
        invokeMethod(message);
    }

    function print(printdata, printerFilter) {

        var message = { "Classe": TIPO_APPLET.PRINTER, "Metodo": PRINTER_METHOD.PRINT, "Parametri": ['prova'] };
        invokeMethod(message);
    }


    //Signer Applet method

    function testChiamataSIGNER() {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.TEST_CHIAMATA, "Parametri": ['prova'] };
        invokeMethod(message);
    }

    function setCheckCertificate(b) {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SET_CHECK_CERTIFICATE, "Parametri": [b] };
        invokeMethod(message);
    }

    function close() {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.CLOSE, "Parametri": [null] };
        invokeMethod(message);
    }

    function killApplet() {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.KILL_APPLET, "Parametri": [null] };
        invokeMethod(message);
    }

    function getCertificateListAsJsonFormat(storeLocation, storeName) {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.GET_CERTIFICATE_LIST_AS_JSON_FORMAT, "Parametri": [storeLocation, storeName] };
        invokeMethod(message);
    }

    function signDataFromPath(phatfile, serialNumber, applyCosign, storeLocation, storeName) {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_DATA_FROM_PATH, "Parametri": [phatfile, serialNumber, applyCosign, storeLocation, storeName] };
        invokeMethod(message);
    }

    function signData(contentEnc, certIndex, applyCosign, storeLocation, storeName) {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_DATA_INDEX, "Parametri": [contentEnc, certIndex, applyCosign, storeLocation, storeName] };
        invokeMethod(message);
    }

    function signdata(contentEnc, serialNumber, applyCosign, storeLocation, storeName) {

        var message = { "Classe": TIPO_APPLET.SIGNER, "Metodo": SIGNER_METHOD.SIGN_DATA_SERIAL, "Parametri": [contentEnc, serialNumber, applyCosign, storeLocation, storeName] };
        invokeMethod(message);
    }

//LISTNER
    function onDisconnected() {
        console.log("Disconnected");
        port = null;
    }

    function onMessage(msg) {

        console.log('Messaggio ricevuto');
        console.log(msg);
        bridge_response = msg.Response;

    }

    function disconnect() {
        console.log("Disconnect");
        var message = { "Classe": -1, "Metodo": -1, "Parametri": [CHIUDI] };
        invokeMethod(message);
        port.disconnect();
        port = null;
    }


    function invokeMethod(message) {

        if (!port)
            connect();
        if (port) {
            
            console.log("Invio messaggio..");
            
            console.log(message);
            console.log(port);

            port.postMessage(message);

        } else {
            console.log('Connessione non riuscita');
        }
    }



  