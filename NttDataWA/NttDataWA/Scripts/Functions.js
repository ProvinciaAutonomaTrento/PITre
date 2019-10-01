function beginRequest(sender, args) {
    $find("mdlWait").show();
}

function endRequest(sender, args) {
    $find("mdlWait").hide();
}

function RefreshExpand() {
 $("h2.expand").toggler({ initShow: "div.collapsed" });
}

function changeLanguageDirection(dir) {
    $(function () {
        $('Html').attr('dir', dir);
    });
    $(document).ready(function () {
        if (dir == 'rtl') {
            $("link[rel=stylesheet]").attr('href', 'Css/Right/Login.css');
        }
        else {
            $("link[rel=stylesheet]").attr('href', 'Css/Left/Login.css');
        }
    });
}

function forceLogin(ipaddress, messageStart, messageEnd) {
    var message;
    message = messageStart + " " + ipaddress + " " + messageEnd;
    var result = window.confirm(message);
    if (result) {
        document.getElementById('HiForcerLogin').value = "1";
        document.getElementById("FrmLogin").submit();

    }
}

function closeAjaxModal(id, retval) { // chiude il popup modale [id] e imposta il valore di ritorno [retval] nel campo hidden
    var p = parent.fra_main;

    if (arguments.length > 2 && arguments[2] != null) {
        p = arguments[2];
    }
    else {
        try {
            var e = p.$('iframe').get(0);
            if (e.id != 'ifrm_' + id) {
                p = e.contentWindow;
                e = p.$('iframe').get(0);

                if (e.id != 'ifrm_' + id) {
                    p = e.contentWindow;
                    e = p.$('iframe').get(0);
                }
            }
        }
        catch (err) {
            try {
                p = parent.parent.fra_main;
            }
            catch (err2) {
                p = parent;
            }
        }
    }

    if (arguments.length > 1) {
        p.$('.retval' + id + ' input').val(retval);
    }
    p.$('#' + id + '_panel').dialog('close');
}

function showPopupContent() {
    try {
        var frameId = window.frameElement.id.replace('ifrm_', '');
        $('#' + frameId + '_panel>#ifrm2_' + frameId, parent.document).hide();
        $('#' + frameId + '_panel', parent.document).find('#ifrm2_' + frameId).hide(); // x IE8...
        $('#' + frameId + '_panel>#ifrm_' + frameId, parent.document).show();
        $('#' + frameId + '_panel', parent.document).find('#ifrm_' + frameId).show(); // x IE8...
    }
    catch (err) { }
}

function disallowOp(id) { // crea un div che inibisce le operazioni e per sicurezza disabilita tutti gli input contenuti nell'elemento [id]
    var href = window.location.href.split('/');
    var path = '../';
    if (href[href.length - 1].toLowerCase() == 'index.aspx') path = '';
    $('body').append("<div id=\"disallower\" style=\"position: absolute; top: 0px; left: 0px; opacity: 0.5; filter: alpha(opacity=50); width: " + ($(window).width() - 1) + "px; height: " + ($(window).height() - 1) + "px; background: #fff; z-index: 110;\"><p style=\"margin-top: " + ($(window).height() / 2 - 25) + "px; text-align: center;\"><img src=\"" + path + "Images/common/loading.gif\" alt=\"\" /></p></div>");
}

function disallowOpHome(id) { // crea un div che inibisce le operazioni e per sicurezza disabilita tutti gli input contenuti nell'elemento [id]
    var href = window.location.href.split('/');
    var path = '../';
    if (href[href.length - 1].toLowerCase() == 'index.aspx') path = '';
    $('body').append("<div id=\"disallower\" style=\"position: absolute; top: 0px; left: 0px; opacity: 0.5; filter: alpha(opacity=50); width: " + ($(window).width() - 1) + "px; height: " + ($(window).height() - 1) + "px; background: #fff; z-index: 110;\"><p style=\"margin-top: " + ($(window).height() / 2 - 25) + "px; text-align: center;\"><img src=\"Images/common/loading.gif\" alt=\"\" /></p></div>");    
}


function reallowOp() { // ripristina le operazioni e riabilita tutti gli input contenuti nell'elemento [id]
    $('#disallower').remove();
}

function objToString(obj) { // visualizza un oggetto JSon come stringa
    var str = '';
    for (var p in obj) {
        if (obj.hasOwnProperty(p)) {
            str += p + '::' + obj[p] + '\n';
        }
    }
    return str;
}

function resizeIframe() {
    if (document.getElementById('frame') && document.getElementById('frame').offsetTop) {
        var height = document.documentElement.clientHeight;
        height -= document.getElementById('frame').offsetTop; // not sure how to get this dynamically
        height -= 280; /* whatever you set your body bottom margin/padding to be */
        document.getElementById('frame').style.height = height + "px";
        window.onresize = resizeIframe
    }
};

function resizeIframeViewerSign() {
    if (document.getElementById('frame')) {
        var height = document.documentElement.clientHeight;
        height -= 210; /* whatever you set your body bottom margin/padding to be */
        document.getElementById('frame').style.height = height + "px";
    }
};

function resizeIframeViewer() {
    if (document.getElementById('frame')) {
        var height = document.documentElement.clientHeight;
        height -= 150; /* whatever you set your body bottom margin/padding to be */
        document.getElementById('frame').style.height = height + "px";
    }
};

function resizeIframeViewerSmistamento() {
    if (document.getElementById('frame')) {
        var height = document.documentElement.clientHeight;
        height -= 110; /* whatever you set your body bottom margin/padding to be */
        document.getElementById('frame').style.height = height + "px";
    }
};

function resizeIframeViewerFirma() {
    if (document.getElementById('frame')) {
        var height = document.documentElement.clientHeight;
        //height -= 110; /* whatever you set your body bottom margin/padding to be */
        document.getElementById('frame').style.height = height + "px";
        alert('height: '+height);
    }
    else
        alert('no frame');
};


function resizeDocAcquisition() {
    if (document.getElementById('docAcquisition') && document.getElementById('docAcquisition').offsetTop) {
        var height = document.documentElement.clientHeight;
        height -= document.getElementById('docAcquisition').offsetTop; // not sure how to get this dynamically
        height -= 280; /* whatever you set your body bottom margin/padding to be */
        document.getElementById('docAcquisition').style.height = height + "px";
        window.onresize = resizeIframe
    }
};

function tooltipTipsy() {
    $(".tipsy").remove();
    $('.tooltip').tipsy();
    $('.clickable').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
    $('.clickableLeft').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.clickableLargeLeft').tipsy({ className: 'clickable-large-left', gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
    $('.redStrike').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.clickableUnderline').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.clickable').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.clickableLarge').tipsy({ className: 'clickable-large', gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
    $('.referenceCode').tipsy({ className: 'reference-tip', gravity: 'n', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.clickableLeftN').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.clickableRight').tipsy({ gravity: 'w', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
    $('.clickableRightNoHtml').tipsy({ gravity: 'w', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: false });
    $('.clickableNE').tipsy({ gravity: 'ne', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.btnEnable').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
    $('.chzn-select-deselect').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
}

function refreshSelect() {
    $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
    $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
}

function permitOnlyNumbers(id) {
    $("#" + id).keydown(function (event) {
        // Allow: backspace, delete, tab, escape, and enter
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
        // Allow: Ctrl+V
                (event.keyCode == 86 && event.ctrlKey === true) ||
        // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
        // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (!event.shiftKey) {
                if ((event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
            else {
                return false;
            }
        }
    });
}

function _FormatDate(id, event) {
    var dateControl = $('#' + id);
    var inputKey = event.keyCode;
    if (inputKey == 37 || inputKey == 39 || inputKey == 8 || inputKey == 46) { return; } // left arrow, right arrow, del, backspace
    var textValue = dateControl.val();

    var contentArray = textValue.split('/');
    var innerValue = contentArray.join('');
    if (innerValue.length == 7 || parseInt(contentArray.length) >= 3) {
        //
    }
    else if (innerValue.length > 3) {
        dateControl.val(innerValue.substr(0, 2) + '/' + innerValue.substr(2, 2) + '/' + innerValue.substr(4));
    }
    else if (innerValue.length > 1) {
        dateControl.val(innerValue.substr(0, 2) + '/' + innerValue.substr(2));
    }

    if (dateControl.val().length >= 10) {
        var t = dateControl.val().substr(0, 10);
        dateControl.val(t);
    }
    if (dateControl.val().length == 10) {
        if (!IsDate(dateControl.val())) {
            if (parent.fra_main) { parent.fra_main.ajaxDialogModal('ErrorDateInvalid', 'warning', ''); } else { parent.ajaxDialogModal('ErrorDateInvalid', 'warning', ''); }
        }
    }
}

function FormatDate(id) {
    $("#" + id).keyup(function (event) {
        _FormatDate(id, event);
    });
    $("#" + id).keydown(function (event) {
        _FormatDate(id, event);
    });
}

function _FormatHour(id) {
    $("#" + id).keyup(function (event) {
        var hourControl = $('#' + id);
        var inputKey = event.keyCode;
        if (inputKey == 37 || inputKey == 39 || inputKey == 8 || inputKey == 46) { return; } // left arrow, right arrow, del, backspace
        var textValue = hourControl.val();
        var contentArray = textValue.split('.');
        var innerValue = contentArray.join('');
        if (innerValue.length == 5 || parseInt(contentArray.length) >= 3) {
            //
        }
        else if (innerValue.length > 3) {
            hourControl.val(innerValue.substr(0, 2) + '.' + innerValue.substr(2, 2) + '.' + innerValue.substr(4));
        }
        else if (innerValue.length > 1) {
            hourControl.val(innerValue.substr(0, 2) + '.' + innerValue.substr(2));
        }

        if (hourControl.val().length == 6) {
            var t2 = hourControl.val() + '00';
            hourControl.val(t2);
        }
        else if (hourControl.val().length >= 8) {
            var t = hourControl.val().substr(0, 8);
            hourControl.val(t);
        }
        if (hourControl.val().length == 8) {
            if (!IsHour(hourControl.val())) {
                if (parent.fra_main) { parent.fra_main.ajaxDialogModal('ErrorHourInvalid', 'warning', ''); } else { parent.ajaxDialogModal('ErrorHourInvalid', 'warning', ''); }
            }
        }
    });
}

function IsHour(testValue) {
    if (testValue.length == 8) {
        if (parseInt(testValue.substr(0, 2)) < 24 && parseInt(testValue.substr(3, 2)) < 60 && parseInt(testValue.substr(6, 2)) < 60) return true;
    }
    return false;
}

function SetRetValue(id, retval) {
    $('.retval' + id + ' input').val(retval);
}

function _charsLeft(id, maxLength, title) {
    if ($("#" + id).val() != null) {
        var actLength2 = $("#" + id).val().length;
        var actLength3 = ($("#" + id).val().split('à').length - 1) * 9 + ($("#" + id).val().split('è').length - 1) * 9 + ($("#" + id).val().split('é').length - 1) * 9 + ($("#" + id).val().split('ì').length - 1) * 9 + ($("#" + id).val().split('ò').length - 1) * 9 + ($("#" + id).val().split('ù').length - 1) * 9;
        var actLength = actLength2 + actLength3;
        if (actLength > maxLength) {
            var warning = "Lunghezza " + title + " eccessiva: " + actLength + " caratteri, massimo " + maxLength + ". <br />I caratteri in eccesso saranno rimossi.";
            if (!($('#dialog_modal').dialog('isOpen') == true) && !dialogModalAlreadyOpened) {
                dialogModalAlreadyOpened = true;
                if (parent.fra_main) { parent.fra_main.ajaxDialogModal('WarningDocumentExcessiveCharacters', 'warning', '', warning); } else { parent.ajaxDialogModal('WarningDocumentExcessiveCharacters', 'warning', '', warning); }
            }
            actLength3 = ($("#" + id).val().split('à').length - 1) * 9 + ($("#" + id).val().split('è').length - 1) * 9 + ($("#" + id).val().split('é').length - 1) * 9 + ($("#" + id).val().split('ì').length - 1) * 9 + ($("#" + id).val().split('ò').length - 1) * 9 + ($("#" + id).val().split('ù').length - 1) * 9;
            $("#" + id).val($("#" + id).val().substring(0, maxLength - actLength3 - 1));
            actLength2 = $("#" + id).val().length;
            actLength3 = ($("#" + id).val().split('à').length - 1) * 9 + ($("#" + id).val().split('è').length - 1) * 9 + ($("#" + id).val().split('é').length - 1) * 9 + ($("#" + id).val().split('ì').length - 1) * 9 + ($("#" + id).val().split('ò').length - 1) * 9 + ($("#" + id).val().split('ù').length - 1) * 9;
            actLength = actLength2 + actLength3;
            $("#" + id + "_chars").text(maxLength - actLength);
        }
        else {
            $("#" + id + "_chars").text(maxLength - actLength);
        }
    }
}

function charsLeft(id, maxLength, title) {// inserire un element html, anche span, con suffisso _chars per visualizzare il numero di caratteri rimanenti
    $("#" + id).keyup(function (event) {
        _charsLeft(id, maxLength, title);
    });
    $("#" + id).change(function (event) {
        _charsLeft(id, maxLength, title);
    });

    _charsLeft(id, maxLength, title);
}

function IsDate(testValue) {
    var returnValue = true;
    try {
        $.datepicker.parseDate("dd/mm/yy", testValue)
    }
    catch (e) {
        returnValue = false;
    }
    return returnValue;
}

function FormatHour() {
    $('input.hour').each(function (index, e) {
        permitOnlyNumbers(e.id)
        _FormatHour(e.id);
    });
}

function DatePicker(lang) {

    $.datepicker.regional['it'] = {
        closeText: 'Chiudi',
        prevText: '&#x3c;Prec',
        nextText: 'Succ&#x3e;',
        currentText: 'Torna ad oggi',
        gotoCurrent: false,
        monthNames: ['Gennaio', 'Febbraio', 'Marzo', 'Aprile', 'Maggio', 'Giugno',
			'Luglio', 'Agosto', 'Settembre', 'Ottobre', 'Novembre', 'Dicembre'],
        monthNamesShort: ['Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu',
			'Lug', 'Ago', 'Set', 'Ott', 'Nov', 'Dic'],
        dayNames: ['Domenica', 'Luned&#236', 'Marted&#236', 'Mercoled&#236', 'Gioved&#236', 'Venerd&#236', 'Sabato'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mer', 'Gio', 'Ven', 'Sab'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Me', 'Gi', 'Ve', 'Sa'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: '',
        yearRange: '1900:' + (new Date().getFullYear() + 10)
    };
    $.datepicker.setDefaults($.datepicker.regional[lang]);

    $('input.datepicker').datepicker({
        showOn: "both",
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true,
        buttonImage: "../Images/Icons/calendar.png",
        buttonImageOnly: true,
        dateFormat: 'dd/mm/yy',
        showOtherMonths: true,
        selectOtherMonths: true
    });
    
    $('input.datepicker').datepicker('option', $.datepicker.regional[lang]);
    $('input.datepicker').each(function (index, e) {
        permitOnlyNumbers(e.id)
        FormatDate(e.id);
    });
}

function EnableOnlyPreviousToday() {
    $('input.datepicker').datepicker({
        showOn: "both",
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true,
        buttonImage: "../Images/Icons/calendar.png",
        buttonImageOnly: true,
        dateFormat: 'dd/mm/yy',
        showOtherMonths: true,
        selectOtherMonths: true,
        maxDate: new Date()
    });
}

function OnlyNumbers() {
    $('input.onlynumbers').each(function (index, e) {
        permitOnlyNumbers(e.id);
    });
    $('textarea.onlynumbers').each(function (index, e) {
        permitOnlyNumbers(e.id);
    });
}

function ValidateNumKey() {
    var inputKey = event.keyCode;
    var returnCode = true;
    if (inputKey > 47 && inputKey < 58) { return; } else
    { returnCode = false; event.keyCode = 0; }
    event.returnValue = returnCode;
}


function GetIeVersion() {
    var ie = (function () {
        var undef,
        v = 3,
        div = document.createElement('div'),
        all = div.getElementsByTagName('i');
        while (
            div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->',
            all[0]
        );
        return v >= 4 ? v : 0;
    } ());

    return ie;
}

