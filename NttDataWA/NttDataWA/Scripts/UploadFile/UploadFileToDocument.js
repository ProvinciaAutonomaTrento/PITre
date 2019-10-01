$(document).ready(function () {
    'use strict';

    var _uploadDocument = document.getElementById("frmUploadMassivo");
    var _formUploadAllegati = document.getElementById("frmUploadAttMassivo");

    if (!_uploadDocument) {
        var formUploadDocument = document.createElement('form');
        formUploadDocument.setAttribute('action', 'UploadDocumentHandler.ashx');
        formUploadDocument.setAttribute('method', 'POST');
        formUploadDocument.setAttribute('id', 'frmUploadMassivo');
        formUploadDocument.style.display = "none";

        var inputSelectDocument = document.createElement('input');
        inputSelectDocument.setAttribute('type', 'file');
        inputSelectDocument.setAttribute('id', 'inputFileDocumentToUploadMassivo');
        inputSelectDocument.setAttribute('class', 'file-upload');

        formUploadDocument.appendChild(inputSelectDocument);
        $("body").append(formUploadDocument);
    }

    if (!_formUploadAllegati) {
        var formUploadAttachments = document.createElement('form');
        formUploadAttachments.setAttribute('action', 'UploadAttachmentsHandler.ashx');
        formUploadAttachments.setAttribute('method', 'POST');
        formUploadAttachments.setAttribute('id', 'frmUploadAttMassivo');
        formUploadAttachments.style.display = "none";

        var inputSelectAttachment = document.createElement('input');
        inputSelectAttachment.setAttribute('type', 'file');
        inputSelectAttachment.setAttribute('id', 'inputAttachmentsToUploadMassivo');
        inputSelectAttachment.setAttribute('class', 'file-upload');
        inputSelectAttachment.setAttribute('multiple', 'multiple');

        formUploadAttachments.appendChild(inputSelectAttachment);


        $("body").append(formUploadAttachments);
    }
    
    $('#inputFileDocumentToUploadMassivo').change( function (e) {
        try {
           //console.log(e.target.files);
            if (e.target.files == null) { //undefined
                if (e.target.value) {
                    // ie < 10 non supporta multiple files
                    var docFile = e.target.value;
                    var fileNameIE8 = docFile.substring(docFile.lastIndexOf('\\') + 1);
                    $("#_txtFileDocumentSelect").text(fileNameIE8);
                    $("#btnStartUploadMassivo").show();

                    $("#btnUploadFileDocumentMassivo").button("disable");
                    $("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                    $("#btnUploadFileDocumentMassivo").off('click');
                }
            } else if (e.target.files.length > 0) {
                var fileName = e.target.files[0].name;
                $("#_txtFileDocumentSelect").text(fileName);
                $("#btnStartUploadMassivo").show();

                $("#btnUploadFileDocumentMassivo").button("disable");
                $("#btnUploadFileDocumentMassivo").removeClass("btnEnable").addClass("btnDisable");
                $("#btnUploadFileDocumentMassivo").off('click');

            }
        } catch (error) {
           //console.log("Errrore in change file input");
           //console.log(error);
        }
    });

    
    var _allegatiPresenti = false;
    var _numeroAllegati = 0;
    var _allegatiCaricati = 0;
    var _alegatiNonCaricati = 0;

    $('#inputAttachmentsToUploadMassivo').change(function (e) {
        try {
            if (e.target.files == null) { //undefined
                if (e.target.value) {
                    _allegatiPresenti = true;
                    _numeroAllegati = 1;
                    $("#_txtFileAttachmentsSelect").text("Numero Allegati: " + 1);

                    $('#btnAttachmentstMassivo').button("disable");
                    $('#btnAttachmentstMassivo').removeClass("btnEnable").addClass("btnDisable");
                    $('#btnAttachmentstMassivo').off('click');
                }
            } else if (e.target.files.length > 0) {
                _allegatiPresenti = true;
                _numeroAllegati = e.target.files.length;
                $("#_txtFileAttachmentsSelect").text("Numero Allegati: " + e.target.files.length);

                $('#btnAttachmentstMassivo').button("disable");
                $('#btnAttachmentstMassivo').removeClass("btnEnable").addClass("btnDisable");
                $('#btnAttachmentstMassivo').off('click');
            }
        } catch (error) {
            _allegatiPresenti = false;
           //console.log(error);
        }
    });

    
    
    // Documents
    if ($.browser.msie) {
        $(".ie9ShowedField").show();
        $(".ie9HiddenField").hide();
        $("#accordion").accordion({ active: 1 });
    } else {
        $(".ie9ShowedField").hide();
        $(".ie9HiddenField").show();
    }
    


    $("#accordion").parent().height("85%");

    $("#accordion").accordion({
        fillSpace: true,
        change: function (event, ui) {
            // alert('activate ' + ui.newHeader.text());
            if (ui.options.active === 1) {
                // scanner selezionato
                $("#boxConversionOptions  input:eq(1)").prop('checked', true).attr("disabled", true);
                setSessionValue("UploadMassivoCartaceo", "1");
            } else {
                $("#boxConversionOptions  input:eq(1)").prop('checked', false).attr("disabled", false);
                setSessionValue("UploadMassivoCartaceo", "0");
            }
        }
    });




    var _btnAvviaAllegati = $('#btnStartUploadAttachmentMassivo');
    function avviaUploadAllegati() {
        _btnAvviaAllegati.click();
    }

    function terminaProcedura() {
        parent.closeAjaxModal('UplodadFile', 'UploadMassivoOk');
    }





    // Initialize the jQuery File Upload widget:
    $('#uploadFileDocumentMassivo_box').fileupload({
        fileInput: $("#inputFileDocumentToUploadMassivo"),
        singleFileUploads: true,
        maxFileSize: 50000000, // 50 Mb
        add: function (e, data) {
            data.context = $('#btnStartUploadMassivo')
                .click(function () {
                    try {
                        if (data.files.length !== 0) {
                            $("#btnStartUploadMassivo").hide();
                            data.submit();
                        }
                    } catch (error) {
                       //console.log(error);
                    }
                });
        },
        done: function (e, data) {
            try {
               //console.log('avvio upload allegati');

                var errore = data.result[0].error;
                if (errore === "") {
                    $("#_txtFileDocumentSelect").text("Caricato");
                    if (_allegatiPresenti) {
                        avviaUploadAllegati();
                    } else {
                        $("#btnClosePanel").show();
                    }
                } else {
                    $("#_txtFileDocumentSelect").text(errore);
                }
  
            } catch (error) {
               //console.log(error);
            }
        }
    }).queue(function () {
        $("#uploadFileDocumentMassivo_box").find(".fileupload-buttonbar").removeClass("ui-widget-header");
        $("#uploadFileDocumentMassivo_box").find(".fileupload-content").first().removeClass("ui-widget-content");

        $("#btnUploadFileDocumentMassivo").find(".ui-icon").first().remove();
        $("#btnUploadFileDocumentMassivo").removeClass().addClass("btnEnable").addClass("customEnableButton").addClass("fileinput-button").dequeue();
        // $("#btnUploadFileDocumentMassivo").removeClass("ui-button-text-icon-primary").addClass("ui-button-text-only").dequeue();

    });

    //$('#btnStartUploadMassivo').click(function () {
    //    try {
    //        var filesList = $("#inputFileDocumentToUploadMassivo").prop('files');
    //        $('#uploadFileDocumentMassivo_box').fileupload('send', { files: filesList });

    //    } catch (error) {
    //       //console.log(error);
    //    }
    //});

    $("#btnClosePanel").click(function () {
        terminaProcedura();
    });

    $(document).on("click", "#btnUploadFileDocumentMassivo", function () {
        $("#inputFileDocumentToUploadMassivo").click();
    });

    $('#uploadAttachmentsMassivo_box').fileupload({
        fileInput: $("#inputAttachmentsToUploadMassivo"),
        maxFileSize: 50000000, // 50 Mb
        add: function (e, data) {
            // non viene eseguito se non vengono caricati file
            data.context = _btnAvviaAllegati
                .click(function () {
                    try {
                       //console.log('inizio upload allegati');
                        if (data.files.length !== 0) {
                           //console.log("Numero allegati effettivi: " + data.files.length);
                            data.submit();
                        } else {
                            consle.log('nessun allegato presente');
                            $("#btnClosePanel").show();
                        }
                    } catch (error) {
                       //console.log(error);
                    }
                });
        },
        done: function (e, data) {
           //console.log("DONE ALLEGATI");
            try {
                for (var i = 0; i < data.result.length; i++) {
                    var errore = data.result[0].error;
                    if (errore === "") {
                        _allegatiCaricati++;
                        $("#_txtFileAttachmentsSelect").text("Caricati " + _allegatiCaricati + " di " + _numeroAllegati);
                    } else {
                        _alegatiNonCaricati++;
                        var errorParagraph = $("<p style='color:red; font-weight: bold;'>" + data.result[0].name + "</p>");
                        $("#boxUploadErrorLog").append(errorParagraph);
                        var errorDetailParagraph = $("<p style='color:red; font-size: 10px;'> (" + data.result[0].error + " )</p>");
                        $("#boxUploadErrorLog").append(errorDetailParagraph);
                    }
                }
                

            } catch (error) {
               //console.log(error);
            }

           //console.log("Fine Caricamento allegati");
           //console.log("Allegati caricati: " + _allegatiCaricati);
           //console.log("Allegati NON caricati: " + _alegatiNonCaricati);
           //console.log("Allegati TOTALI: " + _numeroAllegati);
           //console.log("----------------");

            if (_allegatiCaricati + _alegatiNonCaricati === _numeroAllegati) {
               //console.log("Procedure terminata");
                $("#btnClosePanel").show();
            }
        }
    }).queue(function () {
        $("#uploadAttachmentsMassivo_box").find(".fileupload-buttonbar").removeClass("ui-widget-header");
        $("#uploadAttachmentsMassivo_box").find(".fileupload-content").first().removeClass("ui-widget-content");

        $("#btnAttachmentstMassivo").find(".ui-icon").first().remove();
        $("#btnAttachmentstMassivo").removeClass().addClass("btnEnable").addClass("customEnableButton").addClass("fileinput-button").dequeue();
        //$("#btnAttachmentstMassivo").removeClass("ui-button-text-icon-primary").addClass("ui-button-text-only").dequeue();


    });

    $(document).on("click", "#btnAttachmentstMassivo", function () {
        $("#inputAttachmentsToUploadMassivo").click();
    });




    $("#btnUploadFileDocumentMassivo").hover(
        function () {
            $(this).removeClass("ui-state-hover").removeClass("btnEnable").addClass("btnHover");
        }, function () {
            $(this).removeClass("ui-state-hover").removeClass("btnHover").addClass("btnEnable");
        }
    );
    
    $("#btnAttachmentstMassivo").hover(
        function () {
            $(this).removeClass("ui-state-hover").removeClass("btnEnable").addClass("btnHover");
        }, function () {
            $(this).removeClass("ui-state-hover").removeClass("btnHover").addClass("btnEnable");
        }
    );
    
    



    //$("#uploadBox").on("click", "input:radio", function () {

    //    if( $(this).attr("id") === "optUploadMassivo") {
    //        $("#rowUploadDocumentAndAttachment").show();
    //    } else {
    //        $("#rowUploadDocumentAndAttachment").hide();
    //    }

    //});

    $("#cbxUploadConvertiPdf").change(function () {
        var value = $(this).is(":checked") ? "1" : "0";
        setSessionValue("UploadMassivoConversionePDF", value);
    });

    $("#cbxUploadCartaceo").change(function () {
        var value = $(this).is(":checked") ? "1" : "0";
        setSessionValue("UploadMassivoCartaceo", value);
    });

    $("#cbxScannerConvertiPdf").change(function () {
        var value = $(this).is(":checked") ? "1" : "0";
        setSessionValue("UploadMassivoConversionePDF", value);
    });

   



    $("input[name*='rdbAcquire']").change(function () {
        if (this.value === 'optUploadMassivo') {
            $("#rowUploadDocumentAndAttachment").show();
        }
        else {
            $("#rowUploadDocumentAndAttachment").hide();
        }
    });


    $("#boxConversionOptions  input:eq(0)").change(function () {
        var value = $(this).is(":checked") ? "1" : "0";
        setSessionValue("UploadMassivoConversionePDF", value);
    });

    $("#boxConversionOptions  input:eq(1)").change(function () {
        var value = $(this).is(":checked") ? "1" : "0";
        setSessionValue("UploadMassivoCartaceo", value);
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




});










//var imageUpload = {
//    init: function (selector, context, options) {

//        selector = selector || '.file-upload';
//        context = context || $('#frmUploadMassivo');

//        var filesList = [],
//            paramNames = [];
//        var url = site_url + '/doUpload';

//        $(selector, context).fileupload(options || {
//            url: url,
//            type: 'POST',
//            dataType: 'json',
//            autoUpload: false,
//            singleFileUploads: false,
//            add: function (e, data) {
//                for (var i = 0; i < data.files.length; i++) {
//                    filesList.push(data.files[i]);
//                    paramNames.push(e.delegatedEvent.target.name);
//                }

//                return false;
//            },
//            change: function (e, data) {

//            },
//            done: function (e, data) {

//            }

//        });
//        $('#save_btn').click(function (e) {
//            e.preventDefault();

//            $(selector, context).fileupload('send', { files: filesList, paramName: paramNames });
//        });
//    }
//};



