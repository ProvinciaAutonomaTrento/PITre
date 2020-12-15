// vecchio, usato da ajaxpopup_old.ascx
function chiudiModale(id, retval) {
    if (arguments.length > 0) $('.popup_' + id + '_value').get(0).value = retval;
    $('.popup_' + id).dialog('close');
}

// funzionante
function closeAjaxModal(id, retval) { // chiude il popup modale [id] e imposta il valore di ritorno [retval] nel campo hidden
    if (arguments.length > 1) parent.$('.retval input').get(0).value = retval;
    parent.$('#' + id + '_panel').dialog('close');
}

function disallowOp(id) { // crea un div che inibisce le operazioni e per sicurezza disabilita tutti gli input contenuti nell'elemento [id]
    $('#' + id + ' *').prop('disabled', true);
    $('body').append("<div id=\"disallower\" class=\"disallower_" + id + "\" style=\"position: absolute; top: 0px; left: 0px; opacity: 0.5; filter: alpha(opacity=50); width: " + ($(window).width() - 1) + "px; height: " + ($(window).height() - 1) + "px; background: #fff;\"><p style=\"margin-top: " + ($(window).height() / 2 - 250) + "px; text-align: center;\"><img src=\"images/ajax-loader.gif\" alt=\"\" /></p></div>");
}

function reallowOp() { // ripristina le operazioni e riabilita tutti gli input contenuti nell'elemento [id]
    var id = $('#disallower').attr('class').replace('disallower_', '');
    $('#disallower').remove();
    $('#' + id + ' *').prop('disabled', false);
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