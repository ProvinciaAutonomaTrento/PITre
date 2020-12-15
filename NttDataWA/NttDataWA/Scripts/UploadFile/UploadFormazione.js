
function jquerizeUpload() {
    'use strict';

    // Documents
    if ($.browser.msie && $.browser.version <= 9) {
        // $(".ie9ShowedField").show();
        $(".ie9HiddenField").hide();
        return;
    }



    // $(".ie9ShowedField").hide();
    $(".ie9HiddenField").show();




    var _uploadDocument = document.getElementById("frmUploadDocumentiFormazione");

    if (!_uploadDocument) {
        var formUploadDocument = document.createElement('form');
        formUploadDocument.setAttribute('action', '../handler/UploadFileFormazioneHandler.ashx');
        formUploadDocument.setAttribute('method', 'POST');
        formUploadDocument.setAttribute('id', 'frmUploadDocumentiFormazione');
        formUploadDocument.style.display = "none";

        var inputSelectDocument = document.createElement('input');
        inputSelectDocument.setAttribute('type', 'file');
        inputSelectDocument.setAttribute('id', 'inputFileFormazioneToUpload');
        inputSelectDocument.setAttribute('class', 'file-upload');
        inputSelectDocument.setAttribute('multiple', '');
        formUploadDocument.appendChild(inputSelectDocument);
        $("body").append(formUploadDocument);
    }

    $("#inputFileFormazioneToUpload").unbind("change");

    $("#inputFileFormazioneToUpload").change(function (e) {
        try {
            console.log(e.target.files);
            if (e.target.files === null) { //undefined
                if (e.target.value) {
                    // ie < 10 non supporta multiple files
                    //var docFile = e.target.value;
                    //var fileNameIE8 = docFile.substring(docFile.lastIndexOf('\\') + 1);
                    // $("#_txtFileDocumentSelect").text(fileNameIE8);
                    // $("#btnStartUploadDocumentiFormazione").show();

                    //$("#btnUploadFileDocumentMassivo").button("disable");
                    //$("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                    //$("#btnUploadFileDocumentMassivo").off('click');
                }
            } else if (e.target.files.length > 0) {
                //var fileName = e.target.files[0].name;
                //$("#_txtFileDocumentSelect").text(fileName);
                //$("#btnStartUploadMassivo").show();

                //$("#btnUploadFileDocumentMassivo").button("disable");
                //$("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                //$("#btnUploadFileDocumentMassivo").off('click');

            }
        } catch (error) {
            console.log("Errrore in change file input");
            console.log(error);
        }
    });




    // Initialize the jQuery File Upload widget:
    $('#boxUploadDocumentiFormazione').fileupload({
        fileInput: $("#inputFileFormazioneToUpload"), /* input in base.master */
        maxFileSize: 50000000 // 50 Mb
    });

    $(document).off("click", "#btnUploadDocuments");

    $(document).on("click", "#btnUploadDocuments", function () {
        $("#inputFileFormazioneToUpload").click();
    });




    // ALLEGATI

    var _uploadAttachment = document.getElementById("frmUploadAllegatiFormazione");

    if (!_uploadAttachment) {
        var formUploadAttachment = document.createElement('form');
        formUploadAttachment.setAttribute('action', '../handler/UploadAllegatiFormazioneHandler.ashx');
        formUploadAttachment.setAttribute('method', 'POST');
        formUploadAttachment.setAttribute('id', 'frmUploadAllegatiFormazione');
        formUploadAttachment.style.display = "none";

        var inputSelectAttachment = document.createElement('input');
        inputSelectAttachment.setAttribute('type', 'file');
        inputSelectAttachment.setAttribute('id', 'inputFileAllegatiFormazioneToUpload');
        inputSelectAttachment.setAttribute('class', 'file-upload');
        inputSelectAttachment.setAttribute('multiple', '');
        formUploadAttachment.appendChild(inputSelectAttachment);
        $("body").append(formUploadAttachment);
    }


    $("#inputFileAllegatiFormazioneToUpload").unbind("change");

    $("#inputFileAllegatiFormazioneToUpload").change(function (e) {
        try {
            console.log(e.target.files);
            if (e.target.files === null) { //undefined
                if (e.target.value) {
                    // ie < 10 non supporta multiple files
                    //var docFile = e.target.value;
                    //var fileNameIE8 = docFile.substring(docFile.lastIndexOf('\\') + 1);
                    // $("#_txtFileDocumentSelect").text(fileNameIE8);
                    // $("#btnStartUploadDocumentiFormazione").show();

                    //$("#btnUploadFileDocumentMassivo").button("disable");
                    //$("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                    //$("#btnUploadFileDocumentMassivo").off('click');
                }
            } else if (e.target.files.length > 0) {
                //var fileName = e.target.files[0].name;
                //$("#_txtFileDocumentSelect").text(fileName);
                //$("#btnStartUploadMassivo").show();

                //$("#btnUploadFileDocumentMassivo").button("disable");
                //$("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                //$("#btnUploadFileDocumentMassivo").off('click');

            }
        } catch (error) {
            console.log("Errrore in change file input");
            console.log(error);
        }
    });


    $('#boxUploadAllegatiFormazione').fileupload({
        fileInput: $("#inputFileAllegatiFormazioneToUpload"), /* input in base.master */
        maxFileSize: 50000000 // 50 Mb
    });
    $(document).off("click", "#btnUploadAttachments");

    $(document).on("click", "#btnUploadAttachments", function () {
        $("#inputFileAllegatiFormazioneToUpload").click();
    });





    // Template Formazione

    var _uploadTemplate = document.getElementById("frmUploadTemplateFormazione");

    if (!_uploadTemplate) {
        var formUploadTemplate = document.createElement('form');
        formUploadTemplate.setAttribute('action', '../handler/UploadTemplateFormazioneHandler.ashx');
        formUploadTemplate.setAttribute('method', 'POST');
        formUploadTemplate.setAttribute('id', 'frmUploadTemplateFormazione');
        formUploadTemplate.style.display = "none";

        var inputSelectTemplate = document.createElement('input');
        inputSelectTemplate.setAttribute('type', 'file');
        inputSelectTemplate.setAttribute('id', 'inputTemplateFormazioneToUpload');
        inputSelectTemplate.setAttribute('class', 'file-upload');
        // inputSelectTemplate.setAttribute('multiple', '');
        formUploadTemplate.appendChild(inputSelectTemplate);
        $("body").append(formUploadTemplate);
    }

    $("#inputTemplateFormazioneToUpload").unbind("change");

    $("#inputTemplateFormazioneToUpload").change(function (e) {
        try {
            console.log(e.target.files);
            if (e.target.files === null) { //undefined
                if (e.target.value) {
                    // ie < 10 non supporta multiple files
                    // var docFile = e.target.value;
                    // var fileNameIE8 = docFile.substring(docFile.lastIndexOf('\\') + 1);
                    // $("#_txtFileDocumentSelect").text(fileNameIE8);
                    // $("#btnStartUploadDocumentiFormazione").show();

                    //$("#btnUploadFileDocumentMassivo").button("disable");
                    //$("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                    //$("#btnUploadFileDocumentMassivo").off('click');
                }
            } else if (e.target.files.length > 0) {
                //var fileName = e.target.files[0].name;
                //$("#_txtFileDocumentSelect").text(fileName);
                //$("#btnStartUploadMassivo").show();

                //$("#btnUploadFileDocumentMassivo").button("disable");
                //$("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                //$("#btnUploadFileDocumentMassivo").off('click');

            }
        } catch (error) {
            console.log("Errrore in change file input");
            console.log(error);
        }
    });


    $('#boxUploadTemplateFormazione').fileupload({
        fileInput: $("#inputTemplateFormazioneToUpload"), /* input in base.master */
        maxFileSize: 50000000, // 50 Mb
        done: function (e, data) { $("#txtStatusTemplate").text("Template caricato"); setSessionValue("TemplateCaricato", "1"); }
    });

    $(document).off("click", "#btnUploadTemplate");

    $(document).on("click", "#btnUploadTemplate", function () {
        $("#inputTemplateFormazioneToUpload").click();
    });


    


}

//function createDeleteButton() {

//    $(document).off("click", "#boxCleanDocuments");
//    $(document).on("click", "#boxCleanDocuments", function () {
//        console.log("delete");
//        $.ajax({
//            type: "POST",
//            url: "../handler/DeleteFileFormazione.ashx",
//            // DO NOT SET CONTENT TYPE to json
//            // contentType: "application/json; charset=utf-8", 
//            // DataType needs to stay, otherwise the response object
//            // will be treated as a single string
//            dataType: "json",
//            success: function (data, textStatus, jqXHR) {
//                $("#txtDocumentiEliminati").text("Documenti eliminati con successo");
//            } 
//        });
//    });
//}

function setSessionValue(key, value) {
    $.ajax({
        type: "POST",
        url: "../handler/SessionHandler.ashx",
        data: { sessionKey: key, sessionValue: value },
        // DO NOT SET CONTENT TYPE to json
        // contentType: "application/json; charset=utf-8", 
        // DataType needs to stay, otherwise the response object
        // will be treated as a single string
        dataType: "json"
    });
}

$(jquerizeUpload());