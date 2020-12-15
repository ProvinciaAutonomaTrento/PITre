function folderExistsAsync(path, onSuccess, onError) {
    onSuccess = onSuccess || null;
    onError = onError || null;
    var message = JSON.stringify({ GUID: guid(), CLASS: TIPO_APPLET.FSO, METHOD: FSO_METHOD.FOLDER_EXISTS, PARAMETERS: [path] });
    connectionManagerAsync(message, onSuccess, onError);

}



function invokeMethodAsync() {
    return new Promise(function (resolve, reject) {
        var server = new WebSocket('ws://mysite:1234');
        server.onopen = function () {
            resolve(server);
        };
        server.onerror = function (err) {
            reject(err);
        };

    });
}


function connectionManagerAsync(request) {

    invokeMethodAsync().then(function (server) {
        console.log("Promise resolved");
        // server is ready here
        server.send(request);
        server.onmessage = function (event) {
            var data = JSON.parse(event.data);
            switch (data) {
                case "SUCCESS":
                    break;
                case "ERROR":
                    break;
                default:
            }
            server.close();
        };


    }).catch(function (err) {
        console.log("Promise rejected");
        console.error(err);
    });

}
