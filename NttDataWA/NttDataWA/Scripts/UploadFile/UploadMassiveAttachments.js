$(document).ready(function () {
    'use strict';
    var _currentAttachIndex = 0;
    var _allegatiTotali = 0;
    var _uploadResult = "ko";
    //var formUploadAttchments = document.createElement('form');
    //formUploadAttchments.setAttribute('action', '../Handler/UploadMassiveAttachments.ashx');
    //formUploadAttchments.setAttribute('method', 'POST');
    //formUploadAttchments.setAttribute('id', 'frmUploadMassivoAttachments');
    //formUploadAttchments.style.display = "none";

    //var inputFileAttachments = document.createElement('input');
    //inputFileAttachments.setAttribute('type', 'file');
    //inputFileAttachments.setAttribute('id', 'inputFileAttachmentsToUploadMassivo');
    //inputFileAttachments.setAttribute('class', 'file-upload');

    //formUploadAttchments.appendChild(inputFileAttachments);

    $('#uploadAttachments').fileupload({
        url: "../Handler/UploadMassiveAttachments.ashx",
        fileInput: $("#inputFileMassiveAttachments"),
        type: "POST",
        sequentialUploads: true,
        filesContainer: $('#tableFiles'),
        add: function (e, data) {
            data.context = $("#btnStartUploadMassivoAttachments")
                .click(function () {
                    try {
                        
                        if (data.files.length !== 0) {
                           //console.log("Numero allegati effettivi: " + data.files.length);
                        } else {
                            consle.log('nessun allegato presente');
                        }

                        $("#infoUpload").show();
                        $("#btnClosePopUp").hide();
                        $("#btnSelectAttachments").hide();
                        $("#btnStartUploadMassivoAttachments").hide();
                        // $("#boxFilesDetail").hide();
                        $("#fileupload-content").show();
                        $("#boxFilesDetail tr").remove();
                        data.submit()
                            .success(function (result, textStatus, jqXHR) { /*alert("success");*/ })
                            .error(function (jqXHR, textStatus, errorThrown) {
                                $("#infoUpload").hide();
                                $("#btnClosePopUp").show();
                                $("#errorEndUpload").show();
                            })
                            .complete(function (result, textStatus, jqXHR) { /*alert("complete");*/ });
                    } catch (error) {
                       //console.log(error);
                    }
                });
        },
        done: function (e, data) {
            for (var i = 0; i < data.result.length; i++) {
                elencaUploadDetails(data.result[i]);
                _currentAttachIndex++;
            }
            
            if (_currentAttachIndex === _allegatiTotali) {
                setSessionValue("UploadMassivoAllegati", "ok");
                $("#infoUpload").hide();
                $("#infoEndUpload").show();
                $("#btnClosePopUp").show();
            }
        }
    });

    $('#uploadAttachments').fileupload({
        filesContainer: $('#tableFiles')
    });



    $("#btnSelectAttachments").click(function () {
        $('#inputFileMassiveAttachments').click();
    });


    var _attachmentsFilesToUpload = [];
    var _tempArray = [];
    var _fileAddedIndex = 0;
    $('#inputFileMassiveAttachments').change(function (e) {
        try {
            ////console.log("Selezionati " + e.target.files.length + " allegati");
            // var filesList = $('#inputFileMassiveAttachments').prop('files');
            ////console.log("FILE LIST:" + JSON.stringify(filesList[0]));
            // $('#uploadAttachments').fileupload('add', { files: filesList, url: "../Handler/UploadMassiveAttachments.ashx", type: "POST" });
                //.success(function (result, textStatus, jqXHR) {//console.log("success"); })
               // .error(function (jqXHR, textStatus, errorThrown) {//console.log("error"); })
                //.complete(function (result, textStatus, jqXHR) {//console.log("complete"); });
            if (e.target.files == null) { // undefined
                // ie < 10 non supporta multiple files
                if (e.target.value) {
                    $("#boxLegenda").hide();
                    $("#btnStartUploadMassivoAttachments").removeClass("btnDisable").addClass("btnEnable");
                    _allegatiTotali = 1;
                    _tempArray = [];
                    _tempArray.push(e.target.value);
                    _attachmentsFilesToUpload = _attachmentsFilesToUpload.concat(_tempArray);
                    _tempArray.forEach(elencaFileIE8);
                }
            } else if (e.target.files.length > 0) {
                $("#boxLegenda").hide();
                $("#btnStartUploadMassivoAttachments").removeClass("btnDisable").addClass("btnEnable");
                _allegatiTotali = e.target.files.length;
                // aggiunge file
                _tempArray = [];
                for (var i = 0; i < e.target.files.length; i++) {
                    _tempArray.push(e.target.files[i]);
                }
                _attachmentsFilesToUpload = _attachmentsFilesToUpload.concat(_tempArray);
                _tempArray.forEach(elencaFile);
            }
        } catch (error) {
           //console.log("Errore in change");
           //console.log(error);
        }
    });

    
    function elencaFile(item) {
        var row = $('<tr class="file-details fade">' +
            '<td><span class="index">' + (++_fileAddedIndex) +'</span></td>' +
            '<td><p class="name">' + item.name + '</p>' +
           // '<div class="error"></div>' +
            '</td>' +
            '<td><p class="info"></p>' +
            '<td><p class="error"></p>' +
            '</td>' +
            '</tr>');
        //row.find('.name').text(item.name);
        //row.find('.size').text(item.size);
        //rows = rows.add(row);
        $("#boxFilesDetail").append(row);
    }

    function elencaFileIE8(item) {
        var fileNameIE8 = item.substring(item.lastIndexOf('\\') + 1);
        var row = $('<tr class="file-details fade">' +
            '<td><span class="index">' + (++_fileAddedIndex) + '</span></td>' +
            '<td><p class="name">' + fileNameIE8 + '</p>' +
            // '<div class="error"></div>' +
            '</td>' +
            '<td><p class="info"></p>' +
            '<td><p class="error"></p>' +
            '</td>' +
            '</tr>');
        //row.find('.name').text(item.name);
        //row.find('.size').text(item.size);
        //rows = rows.add(row);
        $("#boxFilesDetail").append(row);
    }


    var _rowIndex = 1;
    function elencaUploadDetails(result) {
        var row = $('<tr class="file-details fade">' +
            '<td><span class="index">' + _rowIndex++ + '</span></td>' +
            '<td><p class="name">' + result.name + '</p>' +
            // '<div class="error"></div>' +
            '</td>' +
            '<td><p class="info">' + (result.error === "" ? "OK" : "errore") +  '</p>' +
            '<td><p class="error">' + (result.error !== "" ? "(" + result.error + ")" : "")+ '</p>' +
            '</td>' +
            '</tr>');
        //row.find('.name').text(item.name);
        //row.find('.size').text(item.size);
        //rows = rows.add(row);
        $("#boxFilesDetail").append(row);
    }



    $("#btnSelectAttachments").hover(
        function () {
            $(this).removeClass("btnEnable").addClass("btnHover");
        }, function () {
            $(this).removeClass("btnHover").addClass("btnEnable");
        }
    );
    $("#btnStartUploadMassivoAttachments").hover(
        function () {
            $(this).removeClass("btnEnable").addClass("btnHover");
        }, function () {
            $(this).removeClass("btnHover").addClass("btnEnable");
        }
    );
    $("#btnClosePopUp").hover(
        function () {
            $(this).removeClass("btnEnable").addClass("btnHover");
        }, function () {
            $(this).removeClass("btnHover").addClass("btnEnable");
        }
    );

    $("#btnClosePopUp").click(function () {
        parent.closeAjaxModal('AttachmentsUpload', 'ok');
    });

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

    $("#boxConversionOptions  input:eq(0)").change(function () {
        var value = $(this).is(":checked") ? "1" : "0";
        setSessionValue("UploadMassivoConversionePDF", value);
    });


    //$("#btnStartUploadMassivoAttachments").click(function () {
    //    $('#uploadAttachments').fileupload({
    //        add: function (e, data) {
    //            data.submit();
    //        }
    //    });
    //});

    //function _uploadFile(index) {
    //    if (_attachmentsFilesToUpload && index >= _attachmentsFilesToUpload.length) { return; }
        
    //}



    //$("#uploadAttachments").fileupload({
    //    //fileInput: $("#inputFileMassiveAttachments"),
    //    url: "../Handler/UploadMassiveAttachments.ashx",
    //    type: "POST",
    //    filesContainer: $("#tableFiles"),
    //    //add: function (e, data) {
    //    //    data.context = $("#btnStartUploadMassivoAttachments").click(function () {
    //    //       //console.log("START upload");
    //    //        data.submit();
    //    //    });
    //    //},
    //    done: function (e, data) {
    //       //console.log("END upload");
    //        $.each(data.result.files, function (index, file) {
    //            $('<p/>').text(file.name).appendTo("#boxResult");
    //        });
    //    }
    //});



});