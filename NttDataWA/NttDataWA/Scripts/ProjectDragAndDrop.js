/*global require, alert*/
/*jslint browser:true*/

require.config({
    paths: {
        knockout: '../scripts/knockout-3.4.1'
    }
});

require(['src/html5Upload', 'domReady', 'knockout-models'], function (html5Upload, domReady, models) {
    'use strict';

    domReady(function () {
        if (html5Upload.fileApiSupported()) {

            var context = document.getElementById('upload-liveuploads'),
                uploadsModel = new models.UploadsViewModel(),
                progress = 0;

            html5Upload.onEnabled('../DragAndDrop/EnabledHandler.ashx', function () {

                html5Upload.initialize({
                    // URL that handles uploaded files
                    uploadUrl: '../DragAndDrop/DragAndDropHandler.ashx',

                    // HTML element on which files should be dropped (optional)
                    dropContainer: document.getElementById('contentDx'),

                    // HTML file input element that allows to select files (optional)

                    // Key for the file data (optional, default: 'file')
                    key: 'File',

                    // Additional data submitted with file (optional)
                    data: { CallerPage: 1 },

                    // Maximum number of simultaneous uploads
                    // Other uploads will be added to uploads queue (optional)
                    maxSimultaneousUploads: 10,

                    // Callback for each dropped or selected file
                    // It receives one argument, add callbacks 
                    // by passing events map object: file.on({ ... })
                    onFileAdded: function (file) {
                        var uploadManager = this || null;
                        var fileModel = new models.FileViewModel(file);
                        uploadsModel.uploads.push(fileModel);
                        uploadsModel.inUpload(true);
                        file.on({
                            // Called after received response from the server
                            onCompleted: function (response) {
                                if (response) {
                                    try {
                                        response = JSON.parse(response);
                                    } catch (ex) {
                                       //console.log(ex);
                                        response = {Success: false, Error:'Generic Error'};
                                    }

                                }
                                if (response.Success)
                                    fileModel.uploadSuccessful(true);
                                else {
                                   //console.log(response.Error);
                                    fileModel.uploadSuccessful(false);
                                }

                                fileModel.uploadCompleted(true);
                                if (uploadManager && uploadManager.activeUploads <= 0)
                                    window.location.href = "../Project/Project.aspx?closeDragAndDrop=true&back=1";
                            },
                            // Called during upload progress, first parameter
                            // is decimal value from 0 to 100.
                            onProgress: function (progress, fileSize, uploadedBytes) {
                                fileModel.uploadProgress(parseInt(progress, 10));
                            }
                        });
                    }
                });
                models.applyBindings(uploadsModel, context);
            });
        }
    });
});
