var tabletMinW = 599;
var smartMinW = 450;
var isTransmit = false;
var zoomScroll;
var isiOS = false;
var swipeBool = true;
var element;
var elFooter;
var elSm;
var elZoom;
var URL;
var isTrueFirma;
var isFirmato = false;
var msGesture = window.navigator && window.navigator.msPointerEnabled && window.MSGesture;
var touchSupport = (("ontouchstart" in window) || msGesture || window.DocumentTouch && document instanceof DocumentTouch);
var zoom = 1 / (screen.availWidth / window.innerWidth);

var ua = navigator.userAgent.toLowerCase();
var isAndroid = ua.indexOf("android") > -1;

$(window).resize(function (e) {
    setTimeout(function () {
        zoom = 1 / (screen.availWidth / window.innerWidth);
    }, 100);


    mobileclient.iPhoneGest();
    mobileclient.setitemposition('#ricerca_item > .item_list');
    mobileclient.setitemposition('#item_list');

    if (($(window).width() <= tabletMinW) && ($(window).width() > smartMinW)) {
        DEVICE = 'galaxy';
    } else if (($(window).width() <= smartMinW)) {
        DEVICE = 'smart';
    } else {
        DEVICE = 'ipad';
    }


    if ((mobileclient.model.TabShow.AREA_DI_LAVORO || mobileclient.model.TabShow.SMISTAMENTO) && (DEVICE == 'smart')) {
        mobileclient.showTab('TODO_LIST')
    }

    var testo = 'Doc. principale';
    if (DEVICE == 'smart') {
        testo = 'Indietro';
    }
    $('#docPrincipale > a').text(testo);

    $('.previewdocP').attr('src', mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/smistatore_pulsante_visualizzadoc.png');
    $('.infoUtenteP').attr('src', mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/ipad/info_utente.png');
    $('.arrow_azioni').attr('src', mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png');
    $('.rrw').attr('src', mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_rrw.png');
    $('.rw').attr('src', mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_rw.png');
    $('.gra0').attr('src', mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png');
    $('.fw').attr('src', mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_fw.png');
    $('.ffw').attr('src', mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_ffw.png');
    $('.ar_left,.ar_right').attr('src', mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/ipad/smistatore_paginatore_arrow.png');
    $('.img_nota_ruolo').attr('src', mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png');
    $('#previewdoc').attr('src', mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/smistatore_pulsante_visualizzadoc.png');
    $('.closeButton').attr('src', mobileclient.context.WAPath + '/Content/img/' + DEVICE + '/smistatore_closebutton.png');

    if ($('#RIC_SALVATE.on').size() > 0) {
        mobileclient.reposArrow('RIC_SALVATE');
    }

    checkHeight()

    //if ((!mobileclient.tipodelega)&&(DEVICE=='smart'))mobileclient.tipodelega = 1;

    //$($('#service_bar > .filter_ricerca > ul > li')[mobileclient.tipodelega]).trigger('click');


});


(function ($) {
    // Determine if we on iPhone or iPad

    var agent = navigator.userAgent.toLowerCase();
    DEVICE = 'ipad';
    if (($(window).width() <= tabletMinW) && ($(window).width() > smartMinW)) {
        DEVICE = 'galaxy';
    } else if (($(window).width() <= smartMinW)) {
        DEVICE = 'smart';
    } else {
        DEVICE = 'ipad';
    }


    if (agent.indexOf('iphone') >= 0 || agent.indexOf('ipad') >= 0) {
        isiOS = true;
        $('body').addClass('iOS');
    }


    $.fn.doubletap = function (onDoubleTapCallback, onTapCallback, delay) {
        var eventName, action;
        delay = delay == null ? 500 : delay;

        eventName = touchSupport == true ? 'touchend' : 'click';


        $(this).bind(eventName, function (event) {
            if (eventName == 'touchend') event.preventDefault();

            var now = new Date().getTime();
            var lastTouch = $(this).data('lastTouch') || now + 1 /** the first time this will make delta a negative number */;
            var delta = now - lastTouch;
            clearTimeout(action);
            if (delta < delay && delta > 0) {
                if (onDoubleTapCallback != null && typeof onDoubleTapCallback == 'function') {
                    onDoubleTapCallback(event);
                }
            } else {
                $(this).data('lastTouch', now);
                action = setTimeout(function (evt) {
                    if (onTapCallback != null && typeof onTapCallback == 'function') {
                        onTapCallback(evt);
                    }
                    clearTimeout(action);   // clear the timeout
                }, delay, [event]);
            }
            $(this).data('lastTouch', now);
        });
    };
})(jQuery);

contenType = 'normal';


headerH = '';
footerH = '';
wrapperH = '';

//if ($.browser.webkit) document.write('<script type="text/javascript" src="' + this.context.WAPath + '/Scripts/iscroll-min.js"></script>');
var myScroll;
var timer;


function setHeightZoom(h) {
    // document.getElementById('zoomWrapper').style.height = h + 'px';
    //if ($.browser.webkit) if (zoomScroll) { zoomScroll.refresh(); }
}
function checkHeightZoom() {
    var actHeaderh = $('#image_zoomed .green_bar').height();
    var h = window.innerHeight - actHeaderh;
    setHeightZoom(wrapperH);
}
function setHeight() {
    var pageHeight = Math.max($(document).height(), $(window).height());
    var infoH = document.getElementById('info_utente_bar').offsetHeight;
    /*wrapperH = pageHeight - infoH;
    $('#wrapper').css({'min-height': wrapperH + 'px'});*/
    var loadingHeight = Math.max($(document).height(), $(window).height());

    $('#loading').css({ 'height': loadingHeight + 'px' });


    var backPos = 'center ' + ($(window).height() / 2 - 20) + 'px';
    $('#loading').css({ 'background-position': backPos });

    //if ($.browser.webkit) if (myScroll) { myScroll.refresh(); }

    utility.checkOrientation(window.orientation);
}
function checkHeight() {
    if (window.pageYOffset > 0 && $.browser.webkit) {
        window.scrollTo(0, 0);
    }
    setHeight();
}

function loaded() {
    //checkHeight()

    timer = setInterval('setHeight()', 50);


}

var preventBehavior = function (e) {
    e.preventDefault();
};

// Enable fixed positioning
//document.addEventListener("touchmove", preventBehavior, false);



document.addEventListener('DOMContentLoaded', loaded, false);

if (isAndroid) {

    window.addEventListener('touchend', function (e) {
        //$('#footer').addClass('footerLoad');
        setTimeout(function () {
            zoomNew = 1 / (screen.availWidth / window.innerWidth) / zoom;
            $('#footer').css({ 'zoom': zoomNew });
            //$('#footer').removeClass('footerLoad');
        }, 100)


    }, false);

}
    $(document).ready(function () {

    window.addEventListener('onorientationchange' in window ? 'orientationchange' : 'resize', checkHeight, false);
    //window.addEventListener('onorientationchange' in window ? 'orientationchange' : 'resize', function(){utility.checkOrientation(window.orientation)}, false);

    //element = document.getElementById('item_list');
    element = document.getElementById('wrapper');
    elFooter = document.getElementById('footer');


    var hammertime = Hammer(element).on("pinch", function (event) {
        swipeBool = false;

        setTimeout(function () {
            swipeBool = true;
        }, 300);
    });

    mobileclient.init();
    mobileclient.abilitazioneLibroFirma();
});

var page = {
    loading: function (mode) {

        if ($('#loading').size() == 0) {
            $(document.body).append($('<div id="loading">loading...</div>'));
            setHeight();
            var backPos = 'center ' + ($(window).height() / 2 - 20) + 'px';

            $('#loading').css({ 'background-position': backPos });
            var hammertimel = Hammer(element, { prevent_default: true, transform_always_block: true }).on("swipeleft", function (event) {
                return false;
            });


            var hammertimer = Hammer(element, { prevent_default: true, transform_always_block: true }).on("swiperight", function (event) {
                return false;
            });
            if (isAndroid) {
                var hammertimelf = Hammer(elFooter, { prevent_default: true, transform_always_block: true }).on("swipeleft", function (event) {
                    return false;

                });

                var hammertimerf = Hammer(elFooter, { prevent_default: true, transform_always_block: true }).on("swiperight", function (event) {
                    return false;
                });

            }


        }
        ;

        if (mode == true) {

            $('#loading').show();
            $('SELECT').css({ 'visibility': 'hidden' })


        }
        else {
            $('#loading').hide();
            $('SELECT').css({ 'visibility': 'visible' })
            $(document.body).css('overflow', 'auto');
        }
    }
}


var mobileclient = {
    gestureOpt: {
        prevent_default: true,
        drag: true,
        drag_block_horizontal: true,
        drag_lock_min_distance: 20,
        hold: false,
        release: true,
        swipe: false,
        tap: false,
        touch: true,
        transform: false
    },
    init: function (idTab) {

        this.iPhoneGest();
        //setto variabili
        this.application = $('#scroller');
        this.idTab = idTab;
        this.mode_view = 'visualicon';
        this._item = '';
        this.contenType = 'normal';
        this.model = model;
        //Mev Navigazione Allegati
        this.modelAllegati = model;
        //end
        this.context = context;
        //setto impostazioni in base al device
        this.device = DEVICE; //non usato per ora


        this.dimImg = 'dimX=560&dimY=560'; //'dimX=400&dimY=560';
        this.dimImgSmista = 'dimX=490&dimY=490'; //'dimX=350&dimY=490';

        if (DEVICE == 'smart') {
            heightPDF = 1200;
            widthPDF = 1200; // 845;
            ratioPDF = widthPDF / heightPDF;
            widthIMG = $(window).width() + 10;
            heightIMG = Math.ceil((widthIMG * heightPDF) / widthPDF);
            this.dimImg = 'dimX=' + widthIMG + '&dimY=' + heightIMG;
            this.dimImgSmista = 'dimX=' + widthIMG + '&dimY=' + heightIMG;
        }

        if (DEVICE == 'ipad') {
            this.dimImg = ''; //è il parametro aggiuntivo per la dimensione delle preview
            this.dimImg = 'dimX=1200&dimY=1200';
            this.dimImgSmista = 'dimX=850&dimY=850' //'dimX=608&dimY=850';
        }


        //lancio funzioni
        this.setActions();
        this.update();

    },
    iPhoneGest: function () {
        if (DEVICE == 'smart') {
            $('.iPhoneDouble').attr('colspan', 2);
            $('.colspan7').attr('colspan', 7);
        } else {
            $('.iPhoneDouble').attr('colspan', 1);
            $('.colspan7').attr('colspan', 1);
        }

    },
    reset: function () {
        $('#nnotifiche').hide();
        $('.none').hide();
        $('#footer_bar').removeClass('showIphone').addClass('hideIphone');
        $('#sezione_documento > h2').removeClass('noIphone');
        $('#sezione_documento .side.left').removeClass('noIphone');
        page.loading(false)
        $('#header UL > LI').removeClass('on');
        $('#footer_bar .cont').html('');
        $('#filter_ruolo > SELECT').html('');
        $('.filter_ricerca > SELECT').html('');
        $('#paginatore,#paginatoreIphone').html('');
        $('#paginatore').hide();
        $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        $('#prev_doc').html('')
        $('#nnotifiche').html('');
        $('#service_bar').html('').removeClass('skinGrey').removeClass('hauto');
        $('#item_list').html('');
        $('#info_item').html('');
        $('.scelta_ricerca,.filter_ricerca,#todolist_control').removeAttr('style');
        $('#ricerca_item > .item_list').html('');
        $('#ricerca_item > .filter_ricerca').html('');
        $('#transm,.filter_trasm').remove();
        $('.control_info').removeClass('premuto')
        $('.control_azioni').removeClass('premuto')
        $('#box_azioni').remove();
        $('#info_trasm').addClass('none');
        $('#sezione_acttrasm').remove();
        $('#note_aggiuntive').html('');
        $('.box_ricerche').remove();
        $('#deleghe').html('');
        $('#smista_doc').html('');
        $('#barra_smistamento').remove();
        $('#wrapper').removeClass('solid');
        $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        $('#service_bar').removeClass('hideIphone').removeClass('showIphone');

        $('#actionIphone').addClass('hideIphone').removeClass('showIphone');

        $('#firma').addClass('hide');
        $('#firmaView').addClass('hide');
        $('#paginatoreIphone').html('');
        $('#actionIphone').html('');

        if (!this.mode_view) this.mode_view = 'visualicon';

    },
    reposArrow: function (id) {
        var newPos = ($('#' + id).offset().left + ($('#' + id).outerWidth() / 2)) - 27;
        var newWidth = ($('#' + id).offset().left) + $('#' + id).outerWidth() + 15;
        $('.arrow_azioni').css({ 'right': 'auto', 'left': newPos + 'px' });
        $('.box_ricerche').css({ 'min-width': newWidth + 'px' });

    },
    checkSession: function (data) {
        if (!data) {

            if (true) {
                if (mobileclient.model.SessionExpired == true) {
                    tooltip.init({ "titolo": 'La tua sessione &egrave; scaduta', 'confirm': 'Riconnetti', 'returntrue': function () {
                        document.location.href = mobileclient.context.WAPath + "/Login/Login";
                    }
                    });
                    return false
                }
                else {
                    return false
                }

            }
        }
        else {
            if (data.Errori) {
                tooltip.init({ "titolo": 'Errore di sistema', 'confirm': 'Riconnetti', 'returntrue': function () {
                    document.location.href = mobileclient.context.WAPath + "/Login/Login";
                }
                });
            }
            else if (data.TabModel.SessionExpired == true) {
                tooltip.init({ "titolo": 'La tua sessione &egrave; scaduta', 'confirm': 'Riconnetti', 'returntrue': function () {
                    document.location.href = mobileclient.context.WAPath + "/Login/Login";
                }
                });
                return false
            }
        }
    },
    setActions: function () {
        $('#header UL > LI').each(function (i, el) {
            if ($(el).attr('id') != 'LOGOUT' && $(el).attr('id') != 'TODO_LIST') {
                $(el).click(function () {
                    mobileclient.showTab($(el).attr('id'));
                    $('#header UL > LI').removeClass('on');

                })
            }
            else if ($(el).attr('id') == 'TODO_LIST') {
                $(el).click(function () {
                    mobileclient.showAndResetTab($(el).attr('id'));
                    $('#header UL > LI').removeClass('on');

                })
            }
            else if ($(el).attr('id') == 'LOGOUT') {

                var nome = mobileclient.model.DescrUtente
                if (mobileclient.model.DelegaEsercitata)
                    nome = mobileclient.model.DelegaEsercitata.Delegato

                $(el).click(function () {
                    tooltip.init({ "titolo": nome + ' vuole uscire?', "content": '', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () {
                        mobileclient.logout()
                    }
                    })
                });
            }

        });
    },
    formatDate: function (data) {
        if (data.indexOf('-') == -1) {
            var data = eval(data.replace(/\/Date\((\d+)\)\//gi, "new Date($1)")) + ' ';
            data = data.substr(0, 15);
            data = data.split(' ');
            //  Mon Jul 26 2010 11:19:21

            switch (data[1]) {
                case 'Jan':
                    mese = "01";
                    break;
                case 'Feb':
                    mese = "02";
                    break;
                case 'Mar':
                    mese = "03";
                    break;
                case 'Apr':
                    mese = "04";
                    break;
                case 'May':
                    mese = "05";
                    break;
                case 'Jun':
                    mese = "06";
                    break;
                case 'Jul':
                    mese = "07";
                    break;
                case 'Aug':
                    mese = "08";
                    break;
                case 'Sep':
                    mese = "09";
                    break;
                case 'Oct':
                    mese = "10";
                    break;
                case 'Nov':
                    mese = "11";
                    break;
                case 'Dec':
                    mese = "12";
                    break;
            }

            var data = data[2] + '.' + mese + '.' + data[3];
            return data;
        }
        else {
            return ''
        }
    },
    showTab: function (idTab) {
        $('#info_trasm').addClass('none');
        page.loading(true);
        $.post(this.context.WAPath + "/Home/ShowTab", { "idTab": idTab }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    showAndResetTab: function (idTab) {
        $('#info_trasm').addClass('none');
        page.loading(true);
        $.post(this.context.WAPath + "/Home/showAndResetTab", { "idTab": idTab }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    update: function () {

        this.checkSession();
        this.reset();
        checkHeight();

        //console.log(dump(this.model));
        if (this.checkError()) {
            //console.log($('.control_info').attr('class'))

            if (this.model.DelegaEsercitata) {
                $('#LISTA_DELEGHE').addClass('esercitata');
            }
            else {
                $('#LISTA_DELEGHE').removeClass('esercitata');
            }
            if (this.model.TabShow.TODO_LIST) {
                $('#TODO_LIST').addClass('on');
                this.updateToDoList();
            }
            else if (this.model.TabShow.DETTAGLIO_DF_TRASM) {
                this.loadPrevious();
            }
            else if (this.model.TabShow.DETTAGLIO_DOC) {
                //$('#TODO_LIST').addClass('on');
                this.updateDettaglioDoc();

            }
            else if (this.model.TabShow.RICERCA) {
                $('#RICERCA').addClass('on');
                this.updateRicerca();
            }
            else if (this.model.TabShow.TRASMISSIONE) {
                this.loadPrevious();
            }
            //nuovi
            else if (this.model.TabShow.LISTA_DELEGHE) {
                $('#LISTA_DELEGHE').addClass('on');

                this.updateDeleghe();
            }
            else if (this.model.TabShow.AREA_DI_LAVORO) {
                $('#AREA_DI_LAVORO').addClass('on');
                this.updateADL();
            }
            else if (this.model.TabShow.SMISTAMENTO) {
                $('#SMISTAMENTO').addClass('on');
                this.updateSmistamento();
            }

            else if (this.model.TabShow.LIBRO_FIRMA) {
                $('#LIBRO_FIRMA').addClass('on');
                this.updateLibroFirma();
            }

        }

    },
    checkError: function () {
        if ($(this.model.Errori).size() > 0) {

            tooltip.init({ "titolo": $(mobileclient.model.Errori)[0], 'confirm': 'ok', 'returntrue': function () {
                mobileclient.loadPrevious()
            }
            });

            return false;

        }
        else {
            return true;
        }

    },
    loadPrevious: function () {
        this.reset();
        this.model.TabModel = this.model.PreviousTabModel;
        if (this.model.PreviousTabName == 'DETTAGLIO_DOC' && this.model.PreviousTabName2 != 'DETTAGLIO_DOC') {
            if (this.model.PreviousTabName2 == 'RICERCA') {
                var PreviousTabModel = this.model.PreviousTabModel;
                var PreviousTabName = this.model.PreviousTabName;
                this.model.PreviousTabModel = this.model.PreviousTabModel2;
                this.model.PreviousTabName = this.model.PreviousTabName2;
                this.model.PreviousTabModel2 = PreviousTabModel;
                this.model.PreviousTabName2 = PreviousTabName;
            }
            else {
                this.model.PreviousTabModel = this.model.PreviousTabModel2;
                this.model.PreviousTabName = this.model.PreviousTabName2;
            }
        }

        if (this.model.PreviousTabModel != null) {
            if (this.model.PreviousTabModel.DelegheRicevute) {
                this.showTab('LISTA_DELEGHE')
            }
            else if (this.model.PreviousTabModel.RicercaInAdl) {
                this.showTab('AREA_DI_LAVORO');
            }
            else if (this.model.PreviousTabModel.LibroFirmaElements) {
                this.showTab('LIBRO_FIRMA');
            }
            else if (!this.model.PreviousTabModel.Ricerche) {
                //            var fascinfo = data.TabModel.FascInfo;
                //            var docinfo = data.TabModel.DocInfo;
                //            var infotrasm = data.TabModel.TrasmInfo;
                //            mobileclient.inFolder(fascinfo.IdFasc, fascinfo.Descrizione, infotrasm.IdTrasm);
                this.showTab('TODO_LIST');
            }
            else {
                this.showTab('RICERCA');
            }
        } else {
            this.showTab('RICERCA');
        }

    },
    updateToDoList: function () {
        //checking sulle deleghe assegnate per il popup
        if (this.model.NumDeleghe != 0 && mobileclient.mode_view != 'visualgrid') {
            tooltip.init({ "titolo": '', "content": '<div class="msgdeleghe">Ti sono state assegnate ' + this.model.NumDeleghe + ' deleghe<br>Vuoi controllarle?</div>', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () {
                mobileclient.showTab('LISTA_DELEGHE');
            }
            });
        }

        //metto la classe in base al tipo di visualizzazione
        $('#item_list').attr('class', 'none ' + this.mode_view);

        //prima notifiche

        $('#nnotifiche').html(this.model.TabModel.NumElements);
        $('#nnotifiche').show();

        //poi aggiungo alla service_bar l'indietro se si tratta di un folder e metto il nome

        if (this.model.TabModel.IdParent) {

            var indietro;
            if (this.model.TabShow.TODO_LIST)
                indietro = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
                    mobileclient.upFolder()
                });
            else
                indietro = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
                    mobileclient.loadPrevious()
                });
            $('#service_bar').append(indietro);

            //dato che ci sono piazzo anche la folder name
            $('#service_bar').append($('<div id="name_onservicebar"></div>'));
            $('#name_onservicebar').html(this.model.TabModel.NomeParent);
        }
        else {

            //implemento la select dei ruoli
            var ruoli = $(this.model.Ruoli);
            $('#service_bar').addClass('skinGrey').append('<div id="filter_ruolo"><img class="info_utente infoUtenteP" src="' + this.context.WAPath + '/content/' + this.context.SkinFolder + '/img/' + DEVICE + '/info_utente.png" /><span class="iPhoneFilter" id="iPhoneFilter"></span><select></select></div>');

            $('#filter_ruolo > #iPhoneFilter').click(function () {
                //if (DEVICE == 'smart')
                mobileclient.skinSelect($('#filter_ruolo > SELECT').eq(0));
            });


            $('#filter_ruolo > SELECT').change(function () {
                mobileclient.cambiaRuolo($('#filter_ruolo > SELECT').val())
            });
            ruoli.each(function (i, el) {
                if (el.Id == mobileclient.model.IdRuolo) {
                    selected = 'selected="selected"';
                    $('#iPhoneFilter').text(el.Descrizione);
                }
                else selected = '';

                var descrizione = el.Descrizione;
                if (mobileclient.model.DelegaEsercitata) {
                    descrizione = "Ruolo delegato: " + descrizione;
                }

                $('#filter_ruolo > SELECT').append($('<option value="' + el.Id + '" ' + selected + '>' + descrizione + '</option>'));
            });

            //e l'action dell'info utente
            $('#filter_ruolo > .info_utente').click(function () {
                if ($('#info_utente').is(':visible')) {
                    $('#info_utente').remove()
                    var newsrc = $('#filter_ruolo > .info_utente').attr('src').replace('info_utente_on.png', 'info_utente.png');
                    $('#filter_ruolo > .info_utente').attr('src', newsrc);

                } else {
                    var newsrc = $('#filter_ruolo > .info_utente').attr('src').replace('info_utente.png', 'info_utente_on.png');
                    $('#filter_ruolo > .info_utente').attr('src', newsrc);

                    if (mobileclient.model.DelegaEsercitata) {
                        $('#service_bar').append($('<div id="info_utente"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><h3>Informazioni Utente</h3><ul><li>Utente: <span></span> </li><li>Ruolo delegato: <span></span></li></ul></div>'));

                        $($('#info_utente > UL > LI > SPAN')[0]).html(mobileclient.model.DelegaEsercitata.Delegato + "<em> delegato da " + mobileclient.model.DescrUtente + '</em>')
                        $($('#info_utente > UL > LI > SPAN')[1]).html(mobileclient.model.DescrRuolo)

                    }
                    else {
                        $('#service_bar').append($('<div id="info_utente"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><h3>Informazioni Utente</h3><ul><li>Utente: <span></span> </li><li>Ruolo: <span></span></li></ul></div>'));

                        $($('#info_utente > UL > LI > SPAN')[0]).html(mobileclient.model.DescrUtente)
                        $($('#info_utente > UL > LI > SPAN')[1]).html(mobileclient.model.DescrRuolo)
                    }
                }

            });

            //mauro 18-12-13 - info utente in header bar
            $('#info_utente_bar > UL').remove();
            $('#info_utente_bar').append('<ul><li><strong>Utente:</strong>&nbsp;<span></span></li><li><strong>Ruolo:</strong>&nbsp;<span></span></li></ul>');
            $($('#info_utente_bar > UL > LI > SPAN')[0]).html(mobileclient.model.DescrUtente);
            $($('#info_utente_bar > UL > LI > SPAN')[1]).html(mobileclient.model.DescrRuolo);

        }

        //metto su i controlli della todolist

        $('#service_bar').append('<div id="todolist_control"></div>');

        var control_info = '<div class="control_pulsante control_info disabled">&nbsp;</div>';
        var control_visualgrid = '<div class="control_pulsante control_visualgrid">&nbsp;</div>';
        var control_visualicon = '<div class="control_pulsante control_visualicon" style="padding: 10px;">&nbsp;</div>';
        var control_azioni = '<div class="control_pulsante control_azioni disabled">&nbsp;</div>';

        $('#todolist_control').append(control_azioni);
        $('#todolist_control').append(control_info);
        $('#todolist_control').append(control_visualicon);
        $('#todolist_control').append(control_visualgrid);


        $('#service_bar').append('<div class="clear"><br class="clear"/></div>');

        $('#todolist_control > .control_visualgrid').click(function () {

            mobileclient.mode_view = 'visualgrid';
            mobileclient.update()
        });
        $('#todolist_control > .control_visualicon').click(function () {
            mobileclient.mode_view = 'visualicon';
            mobileclient.update()
        });
        //premo i pulsanti del caso
        $('.control_' + this.mode_view).addClass('premuto')


        $('#service_bar').removeClass('hideService');

        if (this.model.TabModel.NumTotPages != 0) {
            //controllo la presenza di elementi

            //buildo il paginatore
            this.initPaginatore();

            //e poi popolo gli elementi
            //qui è da fare lo switch tra i tipi di visualizzazione, non era sufficiente cambiare il css
            var elementi = this.model.TabModel.ToDoListElements;
            $(elementi).each(function (i, el) {

                if (el.Tipo.FASCICOLO) var icateg = 'folder';
                else if (el.Tipo.FOLDER) var icateg = 'subfolder';
                else {
                    switch (el.TipoProto) {

                        case 'A':
                            icateg = 'Documento_A';
                            break;
                        case 'E':
                            icateg = 'Documento_E';
                            break;
                        case 'I':
                            icateg = 'Documento_I';
                            break;
                        case 'P':
                            icateg = 'Documento_P';
                            break;
                        case 'U':
                            icateg = 'Documento_U';
                            break;
                        default:
                            icateg = 'file';
                            break;

                    }

                }

                var _item = $('<div class="item"></div>');

                if (mobileclient.mode_view == 'visualicon') {

                    var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                    var info = $('<div class="info_item"></div>');
                    var urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';

                    //controllo se siamo dentro un fascicolo così non mostro tutte le proprietà
                    if (!mobileclient.model.TabModel.IdParent) {
                        $(info).append('<div class="onlyIphone data_tipo">' + el.DataDoc + ' ' + el.Ragione + '</div>');
                        $(info).append('<div class="onlyIphone mittente">' + el.Mittente + '</div>');
                        $(info).append('<span class="noIphone"><span>Da: </span>' + el.Mittente + '<br/></span>');
                        $(info).append('<span class="noIphone"><span>Data: </span>' + el.DataDoc + '<br/></span>');
                        $(info).append('<span class="noIphone"><span>Evento: </span>' + el.Ragione + '<br/></span>');
                        //$(info).append('<span class="noIphone"><span>Estensione: </span>' + el.Extension + '<br/></span>');
                    }

                    //controllo se è un fascicolo/folder così switcho tra oggetto e descrizione
                    if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) {
                        $(info).append('<span class="noIphone"><span>Descrizione: </span>' + el.Oggetto + '<br/></span>');
                        urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
                    }
                    else {
                        $(info).append('<span class="noIphone"><span>Estensione: </span>' + el.Extension + '<br/></span>');
                        $(info).append('<span class="noIphone"><span>Anteprima: </span>' + el.Anteprima + '<br/></span>');
                        $(info).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
                        if (el.Segnatura) {

                            $(info).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');
                        } else {
                            $(info).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');
                        }
                    }

                    $(info).append('<div class="onlyIphone clear"></div>');

                    _item.append(ico);

                    _item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="' + urlImg + '"/></div>');

                    _item.append(info);


                }
                else {
                    var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                    var info_1 = $('<div class="info_item1"></div>');

                    var urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';

                    //controllo se siamo dentro un fascicolo così non mostro tutte le proprietà
                    if (!mobileclient.model.TabModel.IdParent) {
                        $(info_1).append('<div class="onlyIphone data_tipo">' + el.DataDoc + ' ' + el.Ragione + '</div>');
                        $(info_1).append('<div class="onlyIphone mittente">' + el.Mittente + '</div>');
                        $(info_1).append('<span class="noIphone"><span>Da: </span>' + el.Mittente + '<br/></span>');
                        $(info_1).append('<span class="noIphone"><span>Data: </span>' + el.DataDoc + '<br/></span>');
                        $(info_1).append('<span class="noIphone"><span>Ragione: </span>' + el.Ragione + '<br/></span>');

                        var info_2 = $('<div class="info_item2"></div>');
                    }
                    else var info_2 = $('<div class="info_item2 single"></div>');


                    //controllo se è un fascicolo/folder così switcho tra oggetto e descrizione
                    if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) {
                        $(info_2).append('<span class="noIphone"><span>Descrizione: </span>' + el.Oggetto + '<br/></span>');
                        urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';

                    }
                    else {
                        $(info_1).append('<span class="noIphone"><span>Estensione: </span>' + el.Extension + '<br/></span>');
                        $(info_1).append('<span class="noIphone"><span>Anteprima: </span>' + el.Anteprima + '<br/></span>');
                        $(info_2).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
                        if (el.Segnatura) {

                            $(info_2).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');
                        } else {
                            $(info_2).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');
                        }

                    }
                    $(info_1).append('<div class="onlyIphone clear"></div>');
                    $(info_2).append('<div class="onlyIphone clear"></div>');

                    _item.append(ico);
                    _item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="' + urlImg + '"/></div>');
                    _item.append(info_1);
                    _item.append(info_2);
                    _item.append($('<div class="clear"><br class="clear"/></div>'));


                }

                $(_item).find('.icon').doubletap(
                //funzione del doppio tap
                    function (event) {

                        mobileclient._item = el;
                        mobileclient.getItem(el);
                    },
                /** funzione del tap singolo*/
                        function (event) {
                            mobileclient._item = el;
                            mobileclient.chooseDoc(_item);
                        },
                /** tempo di timeout sul quale fare il controllo */
                    400
                );

                $(_item).find('.oggettoItem').click(function () {
                    mobileclient._item = el;
                    mobileclient.control_info()
                });

                $(_item).find('.apri').click(function () {
                    mobileclient._item = el;
                    mobileclient.getItemSmart(el);
                });


                $('#item_list').append(_item);


            });
        }
        else {
            if (this.model.TabModel.IdParent) {
                $('#item_list').html('<div class="no_todolist_fascicolo">&nbsp;</div>');
            } else {
                $('#item_list').html('<div class="no_todolist">&nbsp;</div>');
            }
        }
        $('#item_list').show()

        this.setitemposition('#item_list');

        checkHeight()
    },
    setitemposition: function (div) {
        //funzione di allineamento delle altezze non ricordo xk l'ho chiamata così:)
        var item = $(div + ' > .item');
        item.css({ 'height': 'auto' });
        var maxheight = 0;
        item.each(function (i, el) {
            if ($(el).height() > maxheight) {
                maxheight = $(el).height();
            }
        });

        //console.log(item);
        item.each(function (i, el) {
            $(el).height(maxheight);


        });

    },
    zoomDocument: function (IdDoc) {
        // if (DEVICE == 'ipad') { document.removeEventListener("touchmove", preventBehavior, false); }
        $('#box_azioni > UL > LI').removeClass('selected');
        $('#firma,#firmaView').addClass('hide');
        $('#barra_smistamento').hide();
        width = $(window).width() - 20;
        if (DEVICE == 'smart') {
            width = $(window).width() * 2;
        }
        var height = Math.round(width * 1.42);

        width = height;

        //alert(width+' * '+height);
        //width = 1200;
        //height = 1200;
        imageZoomed = $('<div id="image_zoomed"><div class="green_bar"><div class="pulsante_classic fll  paddingLeft"><a href="javascript:;" id="backZoom">Indietro</a></div></div><div id="zoomWrapper"><div id="zoomScroller" style="width:' + width + 'px;height:' + height + 'px"><img src="' + this.context.WAPath + '/Documento/Preview/' + IdDoc + '?dimX=' + width + '&dimY=' + height + '" /></div></div></div>');

        //buildo il paginatore
        $('#paginatore,#paginatoreIphone').html('');
        $('#paginatore').append('<div class="bg">&nbsp;</div>');
        $('#paginatore').append('<div class="cont doc"></div>');


        $('#paginatore > .count').remove();

        $('#paginatore > .cont').append('<img class="gra0" src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');

        $('#paginatore > .cont > img,#paginatoreIphone > .cont > img').each(function (i, el) {
            $(el).bind('click touchstart', function () {
                $('#loading').show();
                if (i == 0) {
                    $('#zoomScroller > img').attr('src', mobileclient.context.WAPath + '/Documento/Preview/' + IdDoc + '?dimX=' + width + '&dimY=' + height);
                }
                else {
                    var pageDoc = Number(i) + 1;
                    $('#zoomScroller > img').attr('src', mobileclient.context.WAPath + '/Documento/Preview/' + IdDoc + '?page=' + pageDoc + '&' + 'dimX=' + width + '&dimY=' + height);
                }
                $('#zoomScroller > img').load(function () {
                    $('#loading').hide();
                });
            });
        })

        $('#paginatore').show();
        $('body').append(imageZoomed);
        // if ($.browser.webkit) zoomScroll = new iScroll('zoomScroller', { desktopCompatibility: true });
        checkHeightZoom();
        checkHeight();

        elZoom = document.getElementById('zoomScroller');

        var hammertimezoom = Hammer(elZoom).on("pinch", function (event) {
            swipeBool = false;
            setTimeout(function () {
                swipeBool = true;
            }, 300);
        });

        $('#backZoom').doubletap(
            function (event) {
                mobileclient.closezoom();
            },
            function (event) {
                mobileclient.closezoom();
            },
            400
        );


        $('#image_zoomed').doubletap(
            function (event) {

                if (swipeBool) mobileclient.closezoom();
            },
            function (event) {

            },
            400
        );


    },

    closezoom: function () {

        //if (DEVICE == 'ipad') { document.addEventListener("touchmove", preventBehavior, false); }
        $('#image_zoomed').remove();
        if (this.model.TabShow.SMISTAMENTO) {
            $('#paginatore').hide();
        }
        else {
            $('#paginatore > .cont > img,#paginatoreIphone > .cont > img').each(function (i, el) {
                $(el).click(function () {
                    mobileclient.changePreview(i);
                });
            })
        }
        $('#barra_smistamento').show();
    },
    initPaginatore: function () {


        if (this.model.TabShow.TODO_LIST) {
            URL = '/ToDoList/ChangePage'
        } else if (this.model.TabShow.AREA_DI_LAVORO) {

            URL = '/Adl/ChangePage';
        } else if (this.model.TabShow.RICERCA) {
            URL = '/Ricerca/ChangePage';
        }
        else if (this.model.TabShow.LIBRO_FIRMA) {
            URL = '/LibroFirma/ChangePage';
        }
        $('#paginatore').append('<div class="bg">&nbsp;</div>');
        $('#paginatore,#paginatoreIphone').append('<div class="cont"></div>');

        $('#paginatore > .cont').append('<img class="rrw hide" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_rrw.png"/>');
        $('#paginatore > .cont,#paginatoreIphone  > .cont').append('<img class="rw hide" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_rw.png"/>');

        $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
        $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');

        var indice = this.model.TabModel.CurrentPage % 5 - 1;
        if (indice == -1) indice = 4;
        var newsrc = $($('#paginatore > .cont > .gra0')[indice]).attr('src').replace('paginatore_pallino', 'paginatore_pallino_on');
        $($('#paginatore > .cont > .gra0')[indice]).attr('src', newsrc);


        $('#paginatoreIphone > .cont').append('<p></p>');


        $('#paginatore > .cont,#paginatoreIphone  > .cont').append('<img class="fw hide" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_fw.png"/>');
        $('#paginatore > .cont').append('<img class="ffw hide" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_ffw.png"/>');



        var hammertimer = Hammer(element, mobileclient.gestureOpt).on("swiperight", function (event) {


            if (mobileclient.model.TabModel.CurrentPage > 1 && mobileclient.model.TabModel.NumTotPages != 0) {
                if (swipeBool) {
                    mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage - 1);
                    swipeBool = false;
                }
                setTimeout(function () {
                    swipeBool = true;
                }, 300);
            }
        });

        var hammertimel = Hammer(element, mobileclient.gestureOpt).on("swipeleft", function (event) {
            if (mobileclient.model.TabModel.CurrentPage < mobileclient.model.TabModel.NumTotPages) {
                if (swipeBool) {
                    mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage + 1);
                    swipeBool = false;
                }
                setTimeout(function () {
                    swipeBool = true;
                }, 300);
            }
        });
        if (isAndroid) {
            var hammertimerf = Hammer(elFooter, mobileclient.gestureOpt).on("swiperight", function (event) {
                if (mobileclient.model.TabModel.CurrentPage > 1 && mobileclient.model.TabModel.NumTotPages != 0) {
                    if (swipeBool) {
                        mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage - 1);
                        swipeBool = false;
                    }
                    setTimeout(function () {
                        swipeBool = true;
                    }, 300);
                }
            });

            var hammertimelf = Hammer(elFooter, mobileclient.gestureOpt).on("swipeleft", function (event) {
                if (mobileclient.model.TabModel.CurrentPage < mobileclient.model.TabModel.NumTotPages) {
                    if (swipeBool) {
                        mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage + 1);
                        swipeBool = false;
                    }
                    setTimeout(function () {
                        swipeBool = true;
                    }, 300);
                }
            });
        }

        if (this.model.TabModel.CurrentPage != 1 && this.model.TabModel.NumTotPages != 0) {
            if (this.model.TabModel.CurrentPage > 5) {
                $('#paginatore > .cont > .rrw').removeClass('hide');

                $('#paginatore > .cont > .rrw').bind('click touchstart', function () {
                    mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage - (4 + mobileclient.model.TabModel.CurrentPage % 5))
                });
            }
            $('#paginatore > .cont > .rw,#paginatoreIphone > .cont > .rw').removeClass('hide');

            $('#paginatore > .cont > .rw,#paginatoreIphone > .cont > .rw').bind('click touchstart', function () {
                mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage - 1)
            });


        }

        $('#paginatoreIphone > .cont > p').text(this.model.TabModel.CurrentPage + ' di ' + this.model.TabModel.NumTotPages);


        if (this.model.TabModel.CurrentPage != this.model.TabModel.NumTotPages) {

            $('#paginatore > .cont > .fw, #paginatoreIphone > .cont > .fw').removeClass('hide');

            $('#paginatore > .cont > .fw, #paginatoreIphone > .cont > .fw').bind('click touchstart', function () {

                mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage + 1)
            });

            if (this.model.TabModel.CurrentPage + 5 <= this.model.TabModel.NumTotPages) {
                $('#paginatore > .cont > .ffw').removeClass('hide')
                $('#paginatore > .cont > .ffw').bind('click touchstart', function () {
                    mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage + (6 - mobileclient.model.TabModel.CurrentPage % 5))
                });
            }
        }
        $('#paginatore > .cont').append('<div id="npagine">' + this.model.TabModel.CurrentPage + ' di ' + this.model.TabModel.NumTotPages + '</div>')

        $('#paginatore').show();
        $('#paginatoreIphone').addClass('showIphone').removeClass('hideIphone');


    },
    addData: function (data) {
        //se sono già in un documento visualizzo solo le info di profilo dunque vai diretto alla funzione altrimenti vado a caricarmi le info di trasmissione

        if (mobileclient.model.TabShow.DETTAGLIO_DOC && !data) {
            mobileclient.updateDettaglioTrasm(mobileclient.model);
        }
        else if (data) {//se cè data vuol dire che sto passando per una trasmissione veloce
            mobileclient.updateDettaglioTrasm(data);

            $('#transm,.filter_trasm').remove();
            $('#sezione_documento > h2');
            $('#sezione_documento .side');

            if ($('#sezione_documento').find('.filter_trasm').size() == 0) {
                $('#sezione_documento').prepend('<div class="filter_trasm onlyIphone"><span class="iPhoneFilter transm" id="iPhoneFilterT"></span></div>');
            }

            var hideTran = false;
            if (data.TabModel.DocInfo) {
                $('#info_trasm').append('<div id="sezione_acttrasm"><div class="side left"><h3>DATA </h3><span>' + mobileclient.formatDate(data.TabModel.DocInfo.DataDoc) + '</span><br>&nbsp;</div><div class="clear"></div>')
                if (data.TabModel.DocInfo.CanTransmit == false)
                    hideTran = true;
            }
            else {
                $('#info_trasm').append('<div id="sezione_acttrasm"><div class="side left"><h3>DATA APERTURA: </h3><span>' + mobileclient.formatDate(data.TabModel.FascInfo.DataApertura) + '</span><br>&nbsp;</div><div class="side"><h3>DATA CHIUSURA: </h3><span>' + mobileclient.formatDate(data.TabModel.FascInfo.DataChiusura) + '</span></div><div class="clear"><br class="clear"/></div></div>')
                if (data.TabModel.FascInfo.CanTransmit == false)
                    hideTran = true;
            }

            //quindi aggiungo la parte che riguarda la trasmissione compresa la data apertura e/o chiusura dell'elemento
            $('#sezione_acttrasm').append($('<div id="sezione_acttrasm_left" class="side left absText"> <h3>NOTE GENERALI DI TRASMISSIONE</h3><p><textarea onblur="checkHeight();"></textarea></p></div> <div id="sezione_acttrasm_right" class="side" style="padding-left: 10px;"><br><select class="noIphone"></select><br><div class="pulsante_classic flr  noIphone"><a href="javascript:;">Annulla</a></div><div class="pulsante_classic flr" id="transmAbs"><a href="javascript:;">Trasmetti</a></div></div>'));

            $(data.TabModel.ModelliTrasm).each(function (i, el) {
                $('#sezione_acttrasm_right > SELECT').append('<option id="' + el.Id + '" value="' + el.Id + '">' + el.Codice + '</option>')
            })

            $($('#sezione_acttrasm_right > .pulsante_classic')[0]).click(function () {
                mobileclient.control_info()
            });

            if (hideTran) {
                $('#sezione_documento > .filter_trasm > .puls_blue').addClass('disabled');
                $($('#sezione_acttrasm_right > .pulsante_classic')[1]).addClass('hide')
            }

            if ($('#sezione_acttrasm_right > SELECT > option').length != 0) {
                $($('#sezione_acttrasm_right > .pulsante_classic')[1]).click(function () {
                    mobileclient.eseguiTrasm(data, $('#sezione_acttrasm_right > SELECT').val(), $('#sezione_acttrasm_right').parent().find('#sezione_acttrasm_left > p > TEXTAREA').val())
                })

                $('#sezione_documento > .filter_trasm > .puls_blue').eq(0).click(function () {
                    $($('#sezione_acttrasm_right > .pulsante_classic')[1]).click();
                });
            }
            var value = $('#sezione_acttrasm_right > SELECT').eq(0).val();
            var newVal = $('#sezione_acttrasm_right > SELECT').eq(0).find('option[value=' + value + ']').text();
            $('#iPhoneFilterT').html(newVal);
            $('#iPhoneFilterT').click(function () {
                isTransmit = true;
                if (DEVICE == 'smart') mobileclient.skinSelect($('#sezione_acttrasm_right > SELECT').eq(0));
            });
        }
        else {
            mobileclient.getTrasmission(mobileclient._item)
        }


        /*var starthit = 0;
        $('#info_trasm').children('div').each(function () {
        starthit += $(this).height();
        });

        if (starthit > $('#info_trasm').height()) {
        $('#info_trasm').height(starthit + 30);
        }*/

    },

    control_info: function (data) {
        $('.control_info').toggleClass('premuto')
        $('.control_azioni').removeClass('premuto')
        $('#firma,#firmaView').addClass('hide');
        $('#box_azioni').remove();
        $('#info_utente').remove();
        $('#note_aggiuntive').html('');
        $('#footer_bar').removeClass('showIphone').addClass('hideIphone');
        if ($('#info_trasm').is(':visible') && !data) {
            //nel caso in cui avevo usato control_info per una trasmissione ripristino ciò che avevo nascosto ed elimino i nuovi div
            $('#name_onservicebarT').remove();
            $('#indietro_trasm').remove();
            $('#service_bar > form').removeClass('hideService');
            $('#service_bar > DIV').removeClass('hideService');
            if (mobileclient.model.TabShow.RICERCA) {
                $('#paginatoreIphone').addClass('showIphone').removeClass('hideIphone');
                $('#service_bar').removeClass('hideIphone').addClass('skinGrey').addClass('hauto');
                $('#service_bar .pulsante_classic.onlyIphone').remove();
                mobileclient.ricerca_filtri(true)

            }
            $('#sezione_acttrasm').remove();
            //poi chiudo


            if (DEVICE == 'smart') {
                $('#info_trasm').css({ 'height': '0px' });
                $('#info_trasm').addClass('none');
                $('#info_trasm').hide();

            } else {
                $('#info_trasm').animate({
                    height: '0px'
                }, 300, function () {
                    $('#info_trasm').addClass('none');
                    $('#info_trasm').hide();
                })
            }
        }
        else {
            $('#sezione_trasmissione').addClass('none');
            $('#sezione_documento').addClass('hide');

            $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
            $('#service_bar').addClass('hideIphone').removeClass('showIphone');
            $('#actionIphone').removeClass('hideIphone').addClass('showIphone');


            if (DEVICE == 'smart') {
                $('#info_trasm').removeClass('none');
                $('#info_trasm').css({ 'height': '100%' })
                $('#info_trasm').show();
                mobileclient.addData(data)
                window.scrollTo(0, 0);
            } else {


                $('#info_trasm').animate({
                    'height': '500px'
                }, 300, function () {
                    $('#info_trasm').removeClass('none');
                    $('#info_trasm').show();
                    mobileclient.addData(data)
                    window.scrollTo(0, 0);
                }); //.css('height','auto !important')
            }
        }
    },
    control_azioni: function () {
        $('.control_azioni').toggleClass('premuto')
        $('#box_azioni').remove();
        $('#info_utente').remove();
        if ($('.control_azioni').hasClass('premuto')) {
            $('#service_bar').append($('<div id="box_azioni"><img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Azioni</h3><ul></ul></div>'));

            if (this.model.TabShow.DETTAGLIO_DOC) { //siamo in un documento
                if (!this.model.PreviousTabModel) {
                    this.model.PreviousTabModel = this.model.TabModel;
                    this.model.PreviousTabName = 'DETTAGLIO_DOC';
                }

                if (!this.model.PreviousTabModel.Ricerche) { //apro il doc dalla todolist
                    if (this.model.TabModel.DocInfo.IdDocPrincipale == null) {
                        if (mobileclient._item.TrasmInfo != null) { //Trasminfo è presente, quindi sto nel dettaglio
                            if (mobileclient._item.TrasmInfo.HasWorkflow) //E' presente il workflow,
                            {
                                $('#box_azioni > UL').append('<li>Accetta</li><li>Accetta e Trasmetti</li><li>Accetta e ADL</li><li>Rifiuta</li>')
                            } else {    //non è presente il workflow
                                $('#box_azioni > UL').append('<li>Trasmetti</li><li>Inserisci in ADL</li>')
                            }
                        } else {
                            $('#box_azioni > UL').append('<li>Trasmetti</li>');


                            //#<mauro20140311
                            if (this.model.PreviousTabName != 'AREA_DI_LAVORO') {
                                $('#box_azioni > UL').append('<li>Inserisci in ADL</li>')
                            }
                            //#>

                            if (this.model.PreviousTabName == 'AREA_DI_LAVORO') {
                                $('#box_azioni > UL').append('<li>Rimuovi da ADL</li>')
                            }
                        }
                    }
                    if (this.model.TabModel.DocInfo.IsAcquisito) {//Il documento è Aquisito Faccio apparire scarica
                        //Scarica ultima opzione sempre presente in caso di IsAcquisito

                        $('#box_azioni > UL').append('<li>Scarica</li>')

                        //Se è presente la preview (solo per file PDF allora abilitalo zoom)
                        if (this.model.TabModel.DocInfo.HasPreview)
                            $('#box_azioni > UL').append('<li>Zoom</li>')


                        mobileclient.controllaFirma();

                    }

                }
                else if (this.model.PreviousTabModel.Ricerche) {
                    //apro il doc dalla ricerca        
                    if (this.model.TabModel.DocInfo.IdDocPrincipale == null) {

                        //Mev gestione ADL da maschera dettaglio documento
                        //start...
                        // $('#box_azioni > UL').append('<li>Trasmetti</li>')

                        if (this.model.TabModel.TrasmInfo != null) { //Trasminfo è presente, quindi sto nel dettaglio
                            if (this.model.TabModel.TrasmInfo.HasWorkflow) //E' presente il workflow,
                            {
                                $('#box_azioni > UL').append('<li>Accetta</li><li>Accetta e Trasmetti</li><li>Accetta e ADL</li><li>Rifiuta</li>')
                            } else {    //non è presente il workflow
                                $('#box_azioni > UL').append('<li>Trasmetti</li><li>Inserisci in ADL</li>')
                            }
                        } else {
                            $('#box_azioni > UL').append('<li>Trasmetti</li>')
                            $('#box_azioni > UL').append('<li>Inserisci in ADL</li>')

                            if (this.model.PreviousTabName == 'AREA_DI_LAVORO') {
                                $('#box_azioni > UL').append('<li>Rimuovi da ADL</li>')
                            }
                        }
                        // end ...
                    }
                    if (this.model.TabModel.DocInfo.IsAcquisito) {
                        $('#box_azioni > UL').append('<li>Scarica</li>')
                        if (this.model.TabModel.DocInfo.HasPreview) {
                            $('#box_azioni > UL').append('<li>Zoom</li>')
                        }
                        mobileclient.controllaFirma();
                    }
                }
            }
            else if (this.model.TabShow.TODO_LIST) {
                //siamo in todolist
                if (!this.model.TabModel.IdParent) {
                    //siamo nel primo livello NON dentro un fascicolo
                    if (mobileclient._item.DocInfo != null || mobileclient._item.FascInfo != null) {   //ho un trasminfo vuoldeire che ho fatto apri o ho preso i dettagli del doc
                        if (mobileclient._item.HasWorkflow) {//E' presente il workflow,
                            $('#box_azioni > UL').append('<li>Accetta</li><li>Accetta e Trasmetti</li><li>Accetta e ADL</li><li>Rifiuta</li>')
                        } else {    //non è presente il workflow
                            if (this.model.TabModel.SetDataVista)
                                $('#box_azioni > UL').append('<li>Visto</li>')
                            $('#box_azioni > UL').append('<li>Trasmetti</li><li>Inserisci in ADL</li>')
                        }

                    }
                    //non ha il workflow
                    $('#box_azioni > UL').append('<li>Apri</li>')
                }
                else {
                    //sono dentro ad un fascicolo
                    if (mobileclient._item.Tipo.FOLDER) {
                        //ho selezionato un sottofascicolo
                        $('#box_azioni > UL').append('<li>Apri</li>')
                    }
                    else if (mobileclient._item.Tipo.DOCUMENTO) {
                        //ho selezionato un documento
                        $('#box_azioni > UL').append('<li>Apri</li>')

                    }

                }
            }
            else if (this.model.TabShow.RICERCA) {
                if (!this.model.TabModel.IdParent) {
                    //siamo nel primo livello NON dentro un fascicolo
                    $('#box_azioni > UL').append('<li>Apri</li>')
                    $('#box_azioni > UL').append('<li>Trasmetti</li>')

                }
                else {
                    //sono dentro ad un fascicolo
                    if (mobileclient._item.Tipo.FOLDER) {
                        //ho selezionato un sottofascicolo
                        $('#box_azioni > UL').append('<li>Apri</li>')
                    }
                    else if (mobileclient._item.Tipo.DOCUMENTO) {
                        //ho selezionato un documento
                        $('#box_azioni > UL').append('<li>Apri</li>')

                    }

                }

            }
            else if (this.model.TabShow.AREA_DI_LAVORO) {

                if (!this.model.TabModel.IdParent) {
                    //siamo nel primo livello NON dentro un fascicolo
                    $('#box_azioni > UL').append('<li>Apri</li>')
                    $('#box_azioni > UL').append('<li>Trasmetti</li>')
                    $('#box_azioni > UL').append('<li>Rimuovi da ADL</li>')
                }
                else {
                    //sono dentro ad un fascicolo
                    if (mobileclient._item.Tipo.FOLDER) {
                        //ho selezionato un sottofascicolo
                        $('#box_azioni > UL').append('<li>Apri</li>')
                    }
                    else if (mobileclient._item.Tipo.DOCUMENTO) {
                        //ho selezionato un documento
                        $('#box_azioni > UL').append('<li>Apri</li>')

                    }

                }

            }
            else if (this.model.TabShow.SMISTAMENTO) {
                if (this.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].HasWorkflow) //E' presente il workflow,
                {
                    $('#box_azioni > UL').append('<li>Accetta</li><li>Rifiuta</li>')
                } else if (this.model.TabModel.SetDataVista) {    //non è presente il workflow, verifico se il tasto visto è visibile
                    $('#box_azioni > UL').append('<li>Visto</li>')
                }
                //$('#box_azioni > UL').append('<li>Zoom</li><li>Scarica</li><li class="nopadd">Agg. Nota Generale</li>')
                $('#box_azioni > UL').append('<li>Scarica</li><li class="nopadd">Agg. Nota Generale</li>')
            }

            //funzione al pulsante annulla
            $('#box_azioni > button').click(function () {
                $('.control_azioni').removeClass('premuto')
                $('#box_azioni').remove();
            })
            //do le funzioni
            $('#box_azioni > UL > LI').each(function (i, el) {

                switch ($(el).html()) {
                    case 'Accetta':
                        //accetta
                        if (mobileclient.model.TabShow.SMISTAMENTO) {
                            $(el).click(function () {
                                mobileclient.popAccettaTrasmSmista(this)
                            })
                        }
                        else {
                            $(el).click(function () {
                                mobileclient.popAccettaTrasm(this)
                            })
                        }
                        break;
                    case 'Rifiuta':
                        //Rifiuta
                        if (mobileclient.model.TabShow.SMISTAMENTO) {
                            $(el).click(function () {
                                mobileclient.popRifiutaTrasmSmista(this)
                            })
                        }
                        else {
                            $(el).click(function () {
                                mobileclient.popRifiutaTrasm(this)
                                //#< 20140325 - nasconde dettaglio doc se visibile
                                if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                                mobileclient.updateToDoList();
                                //#>
                            })
                        }
                        break;
                    case 'Visto':
                        if (mobileclient.model.TabShow.SMISTAMENTO) {
                            $(el).click(function () {
                                mobileclient.vistoTasmSmista(this)
                            })
                        }
                        else {
                            $(el).click(function () {
                                mobileclient.vistoTrasm(this)
                            })
                        }
                        break;
                    case 'Zoom':
                        //zoom
                        if (mobileclient.model.TabShow.SMISTAMENTO) {
                            $(el).click(function () {
                                mobileclient.zoomDocument(mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id)
                            })
                        }
                        else {
                            $(el).click(function () {
                                //#< 20140325 - nasconde dettaglio doc se visibile
                                if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                                mobileclient.zoomDocument(mobileclient.model.TabModel.DocInfo.IdDoc)
                            })
                        }
                        break;
                    case 'Scarica':
                        //scarica
                        if (mobileclient.model.TabShow.SMISTAMENTO) {
                            $(el).click(function () {
                                window.open(mobileclient.context.WAPath + '/Documento/File/' + mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id)
                            })
                        }
                        else {
                            $(el).click(function () {
                                $('#firma,#firmaView').addClass('hide');
                                $('#box_azioni > UL > LI').removeClass('selected');
                                //#< 20140325 - nasconde dettaglio doc se visibile
                                if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                                window.open(mobileclient.context.WAPath + '/Documento/File/' + mobileclient.model.TabModel.DocInfo.IdDoc)
                            })
                        }
                        break;

                    case 'Apri':
                        //apri
                        $(el).click(function () {
                            //#< 20140325 - nasconde dettaglio doc se visibile
                            if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                            mobileclient.getItem(mobileclient._item)
                        })
                        break;
                    case 'Trasmetti':
                        if (mobileclient.model.TabShow.DETTAGLIO_DOC) {
                            //trasmetto dall'anteprima di un doc
                            $(el).click(function () {
                                //20140325 - nasconde dettaglio doc se visibile
                                if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                                mobileclient.trasmissioneForm(mobileclient.model.TabModel.DocInfo, mobileclient.model.TabModel.DocInfo.IdDoc, "")
                            })
                        }
                        else {
                            //trasmetto da ricerca oppure todolist
                            $(el).click(function () {
                                //20140325 - nasconde dettaglio doc se visibile
                                if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                                mobileclient.trasmissioneForm(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, "")
                            })
                        }
                        break;
                    case 'Accetta e Trasmetti':
                        if (mobileclient._item) {
                            var idtr = mobileclient._item.IdTrasm
                        }
                        else {
                            var idtr = mobileclient.model.TabModel.IdTrasm
                        }

                        if (mobileclient.model.TabShow.DETTAGLIO_DOC) {
                            //trasmetto dall'anteprima di un doc
                            $(el).click(function () {
                                mobileclient.trasmissioneForm(mobileclient.model.TabModel.DocInfo, mobileclient.model.TabModel.DocInfo.IdDoc, idtr)
                            })
                        }
                        else {
                            //trasmetto da ricerca oppure todolist
                            $(el).click(function () {
                                mobileclient.trasmissioneForm(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, idtr)
                            })
                        }
                        break;
                    case 'Accetta e ADL':
                        $(el).click(function () {
                            mobileclient.popAccettaTrasm(this)

                        })
                        break;

                    case 'Inserisci in ADL':
                        $(el).click(function () {
                            mobileclient.inserisciInAdl(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, mobileclient._item.TipoProto)
                            //20140325 - nasconde dettaglio doc se visibile
                            if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                        })

                        break;
                    case 'Rimuovi da ADL':
                        $(el).click(function () {
                            mobileclient.rimuoviDaAdl(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, mobileclient._item.TipoProto)
                            //#< 20140310 - aggiornamento refresh dopo rimozione da adl
                            mobileclient.update();
                            //20140325 - nasconde dettaglio doc se visibile
                            if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                            //#>
                        })
                        break;
                    case 'Agg. Nota Generale':
                        $(el).click(function () {
                            mobileclient.smistamento_addNotaGen(this);
                        })
                        break;
                }

            })

        }

    },
    chooseDoc: function (_item) {

        $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        $('#service_bar').addClass('hideIphone').removeClass('showIphone');
        $('#actionIphone').removeClass('hideIphone').addClass('showIphone');
        //chiudo div sparsi
        $('.control_azioni').removeClass('premuto')
        $('#box_azioni').remove();
        $('#info_trasm').addClass('none');
        $('#info_trasm').hide();
        $('#info_trasm').css({ 'height': '0px' });
        $('.control_info').removeClass('premuto')

        //classe all'elemento
        $('#item_list > .selezionato').removeClass('selezionato');
        $('.item_list > .selezionato').removeClass('selezionato');
        _item.addClass('selezionato');

        //classe ai pulsanti
        $('.control_info').removeClass('disabled')
        $('.control_azioni').removeClass('disabled');


        //funzione al pulsante
        $('.control_info').unbind('click');
        $('.control_azioni').unbind('click');


        $('.control_info').click(function () {
            mobileclient.control_info();

        });

        $('.control_azioni').click(function () {
            mobileclient.control_azioni();

        });

        //se è un sottofascicolo tolgo la visual info PD
        if (mobileclient._item.Tipo.FOLDER) {
            $('.control_info').addClass('disabled');
            $('.control_info').unbind('click');
        }

    },
    getItemSmart: function (_item) {

        $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        $('#service_bar').addClass('hideIphone').removeClass('showIphone');
        $('#actionIphone').addClass('hideIphone').removeClass('showIphone');

        if (_item.Tipo.DOCUMENTO) {
            if (this.model.TabShow.RICERCA) this.dettaglioDoc(_item.Id);
            else this.dettaglioDoc(_item.Id, _item.IdTrasm);

        } else {
            if (this.model.TabShow.RICERCA) this.inRicFolder(_item.Id, _item.Oggetto)
            else this.inFolder(_item.Id, _item.Oggetto, _item.IdTrasm)

        }

    },
    getItem: function (_item) {
        if (_item.Tipo.DOCUMENTO) {
            if ((this.model.TabShow.RICERCA) || (this.model.TabShow.AREA_DI_LAVORO) || (this.model.TabShow.LIBRO_FIRMA)/* || (this.model.TabShow.TODO_LIST)*/) {
                this.dettaglioDoc(_item.Id);
            }
            else
                this.dettaglioDoc(_item.Id, _item.IdTrasm, _item.IdEvento);
        } else {

            if (this.model.TabShow.RICERCA) this.inRicFolder(_item.Id, _item.Oggetto)
            else if (this.model.TabShow.AREA_DI_LAVORO) this.inAdlFolder(_item.Id, _item.Oggetto)
            else this.inFolder(_item.Id, _item.Oggetto, _item.IdTrasm)

        }
    },
    updateDettaglioDoc: function () {

        //Mev Navigazione Allegati - inizializza hidden field contenente id doc principale
        // if $('#name_onservicebar > input').val() =='')
        $('#name_onservicebar').html('<input type="hidden" name="idDocPrincipale" value="">');
        //end

        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        var docinfo = this.model.TabModel.DocInfo;
        var infotrasm = this.model.TabModel.TrasmInfo;

        //alert('IdParent: ' + this.model.TabModel.IdParent);

        var indietro = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
            mobileclient.loadPrevious()
        })

        //alert(this.model.PreviousTabName + '\n' + this.model.PreviousTabName2);
        //        if (this.model.PreviousTabName == 'TODO_LIST' || (this.model.PreviousTabName == 'DETTAGLIO_DOC' && this.model.PreviousTabName2 == 'TODO_LIST')) {
        //            indietro = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Indietro5</a></div>').click(function () { mobileclient.upFolder() });
        //        }

        if (this.model.PreviousTabName == 'DETTAGLIO_DOC') {
            //indietro = $('<div></div>')
        }
        //        if (this.model.TabModel.TrasmInfo != null) {
        //            indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () {
        //                mobileclient.showTab('TODO_LIST')
        //            })
        //        } else {
        //            indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () {
        //                mobileclient.showTab('RICERCA')
        //            })
        //        }
        if (docinfo.IdDocPrincipale != null) {
            var testo = 'Doc. principale';
            if (DEVICE == 'smart') {
                testo = 'Indietro';
            }
            indietro = $('<div class="pulsante_classic fll  paddingLeft" id="docPrincipale"><a href="javascript:;">' + testo + '</a></div>').click(function () {

                if (infotrasm == null) {
                    mobileclient.dettaglioDoc(docinfo.IdDocPrincipale);
                } else {
                    mobileclient.dettaglioDoc(docinfo.IdDocPrincipale, infotrasm.IdTrasm);
                }
            })
        }

        $('#service_bar').append(indietro);
        $('#service_bar').append('<div id="todolist_control" style="width:24%" ></div>');

        var visualizza = $('<div class="onlyIphone paddingRight control_info2"></div>').click(function () {
            mobileclient.control_info();
        })

        $('#service_bar').append(visualizza);

        var control_info = '<div class="control_pulsante control_info paddingLeft">&nbsp;</div>';
        var control_azioni = '<div class="control_pulsante control_azioni paddingLeft">&nbsp;</div>';

        $('#todolist_control').append(control_azioni);
        $('#todolist_control').append(control_info);

        $('.control_info').click(function () {
            mobileclient.control_info();
        });

        $('.control_azioni').click(function () {
            mobileclient.control_azioni();
        });

        $('#service_bar').removeClass('hideService');


        $('#service_bar').append($('<div id="name_onservicebar" class="name_anteprima"></div>'));
        var allegati1 = this.model.TabModel.Allegati;
        //Mev Navigazione Allegati
        if (allegati1.length == 1) {
            $('#name_onservicebar').html('<span>Anteprima della prima pagina</span>');
        }
        if (allegati1.length > 1) {
            $('#name_onservicebar').html('<span>Anteprima della prima pagina - </span><select></select>');
            var numall = 0;
            $(allegati1).each(function (i, el) {
                var nomeDoc = el.Oggetto;
                if (nomeDoc.length > 128) {
                    nomeDoc = nomeDoc.substring(0, 125) + '...';
                }
                if (numall == 0) {
                    $('#name_onservicebar > SELECT').append('<option id="' + el.IdDoc + '" value="' + el.IdDoc + '">' + nomeDoc + ' - (Documento principale)</option>');
                } else {
                    $('#name_onservicebar > SELECT').append('<option id="' + el.IdDoc + '" value="' + el.IdDoc + '">' + nomeDoc + ' - (Allegato n.' + numall + ' )</option>');
                }
                numall++;
            });
            $('#name_onservicebar > SELECT').change(function () {
                $("select option:selected").each(function () {
                    if (infotrasm == null) {
                        mobileclient.dettaglioDoc(this.id);
                        //mauro20140210 - Visualizzazione dettaglio firma
                        //inizio
                        if ($("div").hasClass("control_pulsante control_info paddingLeft premuto")) {
                            mobileclient.control_info();
                        }
                        //fine
                    }
                    else {
                        mobileclient.dettaglioDoc(this.id, infotrasm.IdTrasm);
                        //mauro20140210 - Visualizzazione dettaglio firma
                        //inizio
                        if ($("div").hasClass("control_pulsante control_info paddingLeft premuto")) {
                            mobileclient.control_info();
                        } //fine
                    }
                });
            }

            )
        }
        else {
            if (docinfo.IdDocPrincipale != null)
                this.dettaglioDocAllegati(docinfo.IdDocPrincipale, docinfo.IdDoc);
        }

        $('#footer_bar > .cont').append($('<ul><li class="first"><a href="" target="_blank">Scarica Documento</a></li><li><a href="javascript:;">Zoom +\-</a></li></ul><div class="clear"><br class="clear" /></div>'));

        $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/ajax-loader.gif)' });
        $('#prev_doc').css({ 'background-size': '' });
        $('#prev_doc').show();

        //console.log($('#prev_doc').css('backgroundImage'));

        if (docinfo.HasPreview == true) {
            //se ho la preview mostro il documento

            var img = new Image();

            $(img).attr('src', this.context.WAPath + '/Documento/Preview/' + docinfo.IdDoc + '?' + mobileclient.dimImg).load(function () {
                $('#prev_doc').css({ 'backgroundImage': 'url(' + mobileclient.context.WAPath + '/Documento/Preview/' + docinfo.IdDoc + '?' + mobileclient.dimImg + ')' });
                $('#prev_doc').css({ 'background-size': 'contain' });
            });
            //buildo il paginatore
            $('#paginatore').append('<div class="bg">&nbsp;</div>');
            $('#paginatore').append('<div class="cont doc"></div>');

            $('#paginatore > .cont').append('<img class="gra0" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
            $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
            $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
            $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');

            $('#paginatore > .cont > img,#paginatoreIphone > .cont > img').each(function (i, el) {
                $(el).click(function () {
                    mobileclient.changePreview(i);
                });
            })
            $('#paginatore').show();
            //$('#paginatoreIphone').addClass('showIphone').removeClass('hideIphone');

            $($('#footer_bar > div > ul > li > a')[1]).click(function () {
                mobileclient.zoomDocument(mobileclient.model.TabModel.DocInfo.IdDoc)
            });
        }
        else {
            $($('#footer_bar > div > ul > li')[1]).addClass('disabled');

            //altrimenti mostro il file no_preview non metto link e disattivo graficamente lo zoom.
            if (DEVICE != 'smart') {
                mobileclient.control_info();
            }

            if (docinfo.IsAcquisito) {
                $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/no_preview.jpg)' });
                $('#prev_doc').append('<div id="link_doc"><p>Per vedere il documento <br />clicca il pulsante</p><div class="pulsante"><a href="" target="_blank">Scarica Documento</a></div></div>');
                $('#link_doc > div > a')[0].href = this.context.WAPath + '/Documento/File/' + docinfo.IdDoc;
                $('#link_doc').show();
            }
            else {
                $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/img/' + DEVICE + '/no_acquisito.jpg)' });
            }
        }

        window.scrollTo(0, 0);


        if (docinfo.IsAcquisito) {
            $('#footer_bar > div > ul > li > a')[0].href = this.context.WAPath + '/Documento/File/' + docinfo.IdDoc;
            $('#footer_bar').addClass('showIphone').removeClass('hideIphone');
            mobileclient.controllaFirmaInit();
        } else {
            $('#footer_bar').removeClass('showIphone').addClass('hideIphone');
            isFirmato = false;
        }
        setTimeout(function () {
            if ($('#info_trasm').hasClass('none')) {
                $('.control_info').removeClass('premuto');
            } else {
                $('.control_info').addClass('premuto');
            }
        }, 500);

    },
    changePreview: function (i) {
        $('#paginatore > .cont > img,#paginatoreIphone > .cont > img').addClass('off');
        $($('#paginatore > .cont > img,#paginatoreIphone > .cont > img')[i]).removeClass('off');
        $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/ajax-loader.gif)' });
        $('#prev_doc').css({ 'background-size': '' });
        if (i == 0) {
            $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Documento/Preview/' + this.model.TabModel.DocInfo.IdDoc + '?' + mobileclient.dimImg + ')' });
            $('#prev_doc').css({ 'background-size': 'contain' });
        }
        else {
            ind = Number(i) + 1;
            var img = $('<img src="' + this.context.WAPath + '/Documento/Preview/' + this.model.TabModel.DocInfo.IdDoc + '?page=' + ind + '&' + mobileclient.dimImg + '" />');
            img.load(function () {

                $('#prev_doc').css({ 'backgroundImage': 'url(' + $(img).attr('src') + ')' });
                $('#prev_doc').css({ 'background-size': 'contain' });
            });
        }
    },
    updateRicerca: function () {
        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        //metto la classe in base al tipo di visualizzazione
        $('#ricerca_item > .item_list').attr('class', 'item_list none ' + this.mode_view);

        if (this.model.TabModel.IdParent) {
            var indietro = $('<div class="pulsante_classic fll  paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
                mobileclient.upRicFolder()
            });
            $('#service_bar').append(indietro);

            //dato che ci sono piazzo anche la folder name
            $('#service_bar').append($('<div id="name_onservicebar"></div>'));

            $('#name_onservicebar').html(this.model.TabModel.NomeParent.substr(0, 29) + '...');
        }
        else {
            //if (this.model.TabModel.Testo == null) var ricercavalue = 'Ricerca'
            if (this.model.TabModel.Testo == null) var ricercavalue = ''
            else var ricercavalue = this.model.TabModel.Testo

            //div del filtro input_ricerca
            $('#service_bar').append('<div class="scelta_ricerca"><form><input type="search" class="input_ricerca" value="' + ricercavalue + '"/></form></div>');

            //azione su input_ricerca

            //            $('.input_ricerca').focus(function () {
            //                if ($(this).val() == 'Ricerca') {
            //                    $(this).val('');
            //                }
            //            });

            //ric_documento|ric_fascicolo|ric_doc_fasc
            $('.scelta_ricerca > form > input').attr("value", ricercavalue);

            //ho dovuto portare tutto su un metodo per poter gestire i diversi dispositivi e le diverse view
            this.ricerca_filtri();

        }


        $('#service_bar').append('<div id="todolist_control"></div>');
        var control_info = '<div class="control_pulsante control_info disabled">&nbsp;</div>';
        var control_visualgrid = '<div class="control_pulsante control_visualgrid">&nbsp;</div>';
        var control_visualicon = '<div class="control_pulsante control_visualicon" style="padding: 10px;">&nbsp;</div>';
        var control_azioni = '<div class="control_pulsante control_azioni disabled">&nbsp;</div>';


        $('#todolist_control').append(control_azioni);
        $('#todolist_control').append(control_info);
        $('#todolist_control').append(control_visualicon);
        $('#todolist_control').append(control_visualgrid);


        $('#service_bar').append('<div class="clear"><br class="clear"/></div>');

        $('#todolist_control > .control_visualgrid').click(function () {

            mobileclient.mode_view = 'visualgrid';
            mobileclient.update()
        });
        $('#todolist_control > .control_visualicon').click(function () {
            mobileclient.mode_view = 'visualicon';
            mobileclient.update()
        });
        //premo i pulsanti del caso
        $('.control_' + this.mode_view).addClass('premuto')

        $('#service_bar').removeClass('hideService');

        this.initPaginatore(); //vado col paginatore

        //e poi con gli elementi da popolare
        var elementi = this.model.TabModel.Risultati;
        $(elementi).each(function (i, el) {
            if (el.Tipo.FASCICOLO) var icateg = 'folder';
            else if (el.Tipo.FOLDER) var icateg = 'subfolder';
            else {
                switch (el.TipoProto) {

                    case 'A':
                        icateg = 'Documento_A';
                        break;
                    case 'E':
                        icateg = 'Documento_E';
                        break;
                    case 'I':
                        icateg = 'Documento_I';
                        break;
                    case 'P':
                        icateg = 'Documento_P';
                        break;
                    case 'U':
                        icateg = 'Documento_U';
                        break;
                    default:
                        icateg = 'file';
                        break;

                }

            }

            var _item = $('<div class="item"></div>');

            if (mobileclient.mode_view == 'visualicon') {

                var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                var info = $('<div class="info_item"></div>');
                var urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';

                //controllo se fascicolo/folder o documento per mostrare oggetto o descrizioni
                if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) {
                    $(info).append('<span>Descrizione: </span>' + el.Oggetto + '<br/>');
                    urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
                }
                else {
                    $(info).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
                    $(info).append('<span class="noIphone"><span>Estensione: </span>' + el.Extension + '<br/></span>');
                    $(info).append('<span class="noIphone"><span>Anteprima: </span>' + el.Anteprima + '<br/></span>');
                    if (el.Segnatura) {

                        $(info).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');


                    } else {
                        $(info).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');


                    }
                }

                _item.append(ico);

                _item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="' + urlImg + '"/></div>');

                _item.append(info);

            }
            else {

                var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                //var info_1 = $('<div class="info_item1"></div>');
                var info_2 = $('<div class="info_item2"></div>');

                var urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';

                if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) {
                    $(info_2).append('<span class="noIphone"><span>Descrizione: </span>' + el.Oggetto + '<br/></span>');
                    urlImg = mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
                }
                else {
                    $(info_2).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
                    $(info_2).append('<span class="noIphone"><span>Estensione: </span>' + el.Extension + '<br/></span>');
                    $(info_2).append('<span class="noIphone"><span>Anteprima: </span>' + el.Anteprima + '<br/></span>');

                    if (el.Segnatura) {

                        $(info_2).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');
                    } else {
                        $(info_2).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');

                    }
                }
                _item.append(ico);
                //_item.append(info_1);

                _item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="' + urlImg + '"/></div>');

                _item.append(info_2);
                _item.append($('<div class="clear"><br class="clear"/></div>'));

            }
            $(_item).find('.icon').doubletap(
            //funzione del doppio tap
                function (event) {
                    if (swipeBool) {
                        mobileclient._item = el;
                        mobileclient.getItem(el)
                    }

                },
            /** funzione del tap singolo*/
                    function (event) {
                        mobileclient._item = el;
                        mobileclient.chooseDoc(_item);
                    },
            /** tempo di timeout sul quale fare il controllo */
                400
            );

            $(_item).find('.oggettoItem').click(function () {
                mobileclient._item = el;
                mobileclient.control_info()
            });

            $(_item).find('.apri').click(function () {
                mobileclient._item = el;
                mobileclient.getItemSmart(el);
            });

            $('#ricerca_item > .item_list').append(_item);
        });
        if (this.model.TabModel.NumTotPages == 0) {
            if (this.model.TabModel.IdParent) {
                $('#ricerca_item > .item_list').html('<div class="no_todolist_fascicolo">&nbsp;</div>');
            } else {
                $('#ricerca_item > .item_list').html('<div class="no_ricerca">&nbsp;</div>');
            }
        }

        $('#ricerca_item').show()
        $('#ricerca_item > .item_list').show()
        this.setitemposition('#ricerca_item > .item_list');

    },
    updateLibroFirma: function () {
        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        mobileclient.mode_view = 'visualgrid';

        $('#ricerca_item > .item_list').attr('class', 'item_list none ' + this.mode_view);
        if (this.model.TabModel.Testo == null) var ricercavalue = ''
        else var ricercavalue = this.model.TabModel.Testo
        $('#service_bar').removeClass('hideService');
        //div del filtro input_ricerca
        $('#service_bar').append('<div class="scelta_ricerca"><form><input type="search" class="input_ricerca" value="' + ricercavalue + '"/></form></div>');

        //azione su input_ricerca

        $('.input_ricerca').focus(function () {
            if ($(this).val() == 'Ricerca') {
                $(this).val('');
            }
        });

        //ric_documento|ric_fascicolo|ric_doc_fasc
        $('.scelta_ricerca > form > input').attr("value", ricercavalue);

        //ho dovuto portare tutto su un metodo per poter gestire i diversi dispositivi e le diverse view
        this.libroFirma_filtri();

        $('#item_list').attr('class', 'none ' + this.mode_view);

        //azioni
        var control_azioni = $('<div class="control_pulsante control_azioni">&nbsp;</div>');
        control_azioni.click(function () {
            $('.control_azioni').toggleClass('premuto');

            if ($('.control_azioni').hasClass('premuto')) {
                $('#service_bar').append($('<div id="box_azioni"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Azioni</h3><ul><li>Firma tutti</li><li class="nopadd">Respingi tutti</li></ul></div>'));

                //                $($('#box_azioni > UL > LI')[0]).click(function () {
                //                    mobileclient.zoomDocument(mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id)
                //                });
                //                $($('#box_azioni > UL > LI')[1]).click(function () {
                //                    window.open(mobileclient.context.WAPath + '/Documento/File/' + mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id)
                //                })
                //                $($('#box_azioni > UL > LI')[2]).click(function () {
                //                    mobileclient.smistamento_addNotaGen(this)
                //                });
                $($('#box_azioni > UL > LI')[1]).click(function () {
                    mobileclient.respingiSelezionatiLf();
                })

                $($('#box_azioni > UL > LI')[0]).click(function () {
                    mobileclient.firmaSelezionatiLf('cades', false);
                })
                $('#box_azioni').find('.annulla_button').click(function () {
                    $('.control_azioni').toggleClass('premuto');
                    $('#box_azioni').remove();
                });
            }
            else {
                $('#box_azioni').remove();
            }
        });
        $('#service_bar').append(control_azioni);
        if (this.model.TabModel.NumTotPages != 0) {
            //controllo la presenza di elementi
            //buildo il paginatore
            this.initPaginatore();
            //e poi popolo gli elementi
            //qui è da fare lo switch tra i tipi di visualizzazione, non era sufficiente cambiare il css
            var elementi = this.model.TabModel.LibroFirmaElements;

            $(elementi).each(function (i, el) {

                switch (el.TipoFirma) {
                    case 'DOC_SIGNATURE_P':
                        if (el.IdUtenteTitolare == '') {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_Pades_Ruolo';
                            }
                            else {
                                icateg = 'Documento_Pades_Ruolo';
                            }
                        }
                        else {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_Pades_Utente';
                            }
                            else {
                                icateg = 'Documento_Pades_Utente';
                            }
                        }
                        break;
                    case 'DOC_SIGNATURE':
                        if (el.IdUtenteTitolare == '') {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_Cades_Ruolo';
                            }
                            else {
                                icateg = 'Documento_Cades_Ruolo';
                            }
                        }
                        else {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_Cades_Utente';
                            }
                            else {
                                icateg = 'Documento_Cades_Utente';
                            }
                        }
                        break;
                    case 'DOC_VERIFIED':
                        if (el.IdUtenteTitolare == '') {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_Verified_Ruolo';
                            }
                            else {
                                icateg = 'Documento_Verified_Ruolo';
                            }
                        }
                        else {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_Verified_Utente';
                            }
                            else {
                                icateg = 'Documento_Verified_Utente';
                            }
                        }
                        break;
                    case 'DOC_STEP_OVER':
                        if (el.IdUtenteTitolare == '') {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_AdvancementProcess_Ruolo';
                            }
                            else {
                                icateg = 'Documento_AdvancementProcess_Ruolo';
                            }
                        }
                        else {
                            if (el.IsAllegato) {
                                icateg = 'Allegato_AdvancementProcess_Utente';
                            }
                            else {
                                icateg = 'Documento_AdvancementProcess_Utente';
                            }
                        }
                        break;
                    default:
                        icateg = 'file';
                        break;

                }
                switch (el.StatoFirma) {
                    case 'PROPOSTO':
                        icoStato = 'Stato_Firma_Proposto';
                        break;
                    case 'DA_FIRMARE':
                        icoStato = 'Stato_Firma_Da_Firmare';
                        break;
                    case 'DA_RESPINGERE':
                        icoStato = 'Stato_Firma_Da_Respingere';
                        break;
                }

                var _item = $('<div class="item"></div>');
                var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                var ico2 = $('<div class="icon2 ' + icoStato + '">&nbsp;</div>');
                var info_1 = $('<div class="info_item1"></div>');
                var info_2 = $('<div class="info_item3"></div>');
                var stato = $('<div class="ico">&nbsp;</div>');

                $(stato).append('<div class="icon2 ' + icoStato + '">&nbsp;</div>');
                if (el.ErroreFirma == '') {
                }
                else {
                    $(stato).append('<div class="Errore">Errore</div>');
                }
                $(info_2).append('<span>Id: </span>' + el.InfoDocumento.IdDoc + '<br/>');
                $(info_2).append('<span>Oggetto: </span>' + el.InfoDocumento.Oggetto + '<br/>');
                $(info_2).append('<span>Proviene da: </span>' + el.UtenteProponente + ' (' + el.RuoloProponente + ') <br/>');
                $(info_2).append('<span>Proposto il: </span>' + el.DataInserimento + '<br/>');

                $(info_1).append('<span>Note: </span>' + el.Note + '<br/>');

                _item.append(ico);
                _item.append(info_2);
                _item.append(info_1);
                _item.append(stato);

                _item.append($('<div class="clear"><br class="clear"/></div>'));

                $(_item).find('.icon').doubletap(
                //funzione del doppio tap
                function (event) {
                    if (swipeBool) {
                        mobileclient._item = el;
                        mobileclient.getItem(el)
                    }

                },
                /** funzione del tap singolo*/
                    function (event) {
                        //                        mobileclient._item = el;
                        //                        mobileclient.chooseDoc(_item);
                    },
                /** tempo di timeout sul quale fare il controllo */
                400
            );


                $(_item).find('.icon2').doubletap(
                //funzione del doppio tap
                function (event) {
                    mobileclient._item = el;
                    mobileclient.cambiaStatoElementoLf(mobileclient._item);
                },
                /** funzione del tap singolo*/
                    function (event) {
                    },
                /** tempo di timeout sul quale fare il controllo */
                400
            );
                $('#item_list').append(_item);
            });

            $('#item_list').show()

            this.setitemposition('#item_list');

            checkHeight();
        }
    },

    updateADL: function () {

        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        //metto la classe in base al tipo di visualizzazione
        $('#ricerca_item > .item_list').attr('class', 'item_list none ' + this.mode_view);

        if (this.model.TabModel.IdParent) {
            var indietro = $('<div class="pulsante_classic fll  paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
                mobileclient.upAdlFolder()
            });
            $('#service_bar').append(indietro);

            //dato che ci sono piazzo anche la folder name
            $('#service_bar').append($('<div id="name_onservicebar"></div>'));

            $('#name_onservicebar').html(this.model.TabModel.NomeParent.substr(0, 29) + '...');
        }
        else {
            if (this.model.TabModel.Testo == null) var ricercavalue = 'Ricerca'
            else var ricercavalue = this.model.TabModel.Testo

            //div del filtro input_ricerca
            $('#service_bar').append('<div class="scelta_ricerca"><form><input type="search" class="input_ricerca" value="' + ricercavalue + '"/></form></div>');

            //azione su input_ricerca

            $('.input_ricerca').focus(function () {
                if ($(this).val() == 'Ricerca') {
                    $(this).val('');
                }
            });

            //ric_documento|ric_fascicolo|ric_doc_fasc
            $('.scelta_ricerca > form > input').attr("value", ricercavalue);

            //ho dovuto portare tutto su un metodo per poter gestire i diversi dispositivi e le diverse view
            this.adl_filtri();

        }


        $('#service_bar').append('<div id="todolist_control"></div>');
        var control_info = '<div class="control_pulsante control_info disabled">&nbsp;</div>';
        var control_visualgrid = '<div class="control_pulsante control_visualgrid">&nbsp;</div>';
        var control_visualicon = '<div class="control_pulsante control_visualicon" style="padding: 10px;">&nbsp;</div>';
        var control_azioni = '<div class="control_pulsante control_azioni disabled">&nbsp;</div>';


        $('#todolist_control').append(control_azioni);
        $('#todolist_control').append(control_info);
        $('#todolist_control').append(control_visualicon);
        $('#todolist_control').append(control_visualgrid);


        $('#service_bar').append('<div class="clear"><br class="clear"/></div>');

        $('#todolist_control > .control_visualgrid').click(function () {

            mobileclient.mode_view = 'visualgrid';
            mobileclient.update()
        });
        $('#todolist_control > .control_visualicon').click(function () {
            mobileclient.mode_view = 'visualicon';
            mobileclient.update()
        });
        //premo i pulsanti del caso
        $('.control_' + this.mode_view).addClass('premuto')

        $('#service_bar').removeClass('hideService');

        this.initPaginatore(); //vado col paginatore

        //e poi con gli elementi da popolare
        var elementi = this.model.TabModel.Risultati;
        $(elementi).each(function (i, el) {

            if (el.Tipo.FASCICOLO) var icateg = 'folder';
            else if (el.Tipo.FOLDER) var icateg = 'subfolder';
            else {
                switch (el.TipoProto) {

                    case 'A':
                        icateg = 'Documento_A';
                        break;
                    case 'E':
                        icateg = 'Documento_E';
                        break;
                    case 'I':
                        icateg = 'Documento_I';
                        break;
                    case 'P':
                        icateg = 'Documento_P';
                        break;
                    case 'U':
                        icateg = 'Documento_U';
                        break;
                    default:
                        icateg = 'file';
                        break;

                }

            }

            var _item = $('<div class="item"></div>');

            if (mobileclient.mode_view == 'visualicon') {

                var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                var info = $('<div class="info_item"></div>');

                //controllo se fascicolo/folder o documento per mostrare oggetto o descrizioni
                if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) $(info).append('<span>Descrizione: </span>' + el.Oggetto + '<br/>');
                else {
                    $(info).append('<span>Oggetto: </span>' + el.Oggetto + '<br/>');
                    $(info).append('<span>Estensione: </span>' + el.Extension + '<br/>');
                    $(info).append('<span>Anteprima: </span>' + el.Anteprima + '<br/>');
                    if (el.Segnatura) {

                        $(info).append('<span>Segnatura: </span>' + el.Segnatura + '<br/>');
                    } else {
                        $(info).append('<span>Id: </span>' + el.Id + '<br/>');
                    }
                }

                _item.append(ico);
                _item.append(info);

            }
            else {

                var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                //var info_1 = $('<div class="info_item1"></div>');
                var info_2 = $('<div class="info_item2"></div>');


                if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) $(info_2).append('<span>Descrizione: </span>' + el.Oggetto + '<br/>');
                else {
                    $(info_2).append('<span>Oggetto: </span>' + el.Oggetto + '<br/>');
                    if (el.Segnatura) {

                        $(info_2).append('<span>Segnatura: </span>' + el.Segnatura + '<br/>');
                    } else {
                        $(info_2).append('<span>Id: </span>' + el.Id + '<br/>');
                    }
                    $(info_2).append('<span>Estensione: </span>' + el.Extension + '<br/></span>');
                    $(info_2).append('<span>Anteprima: </span>' + el.Anteprima + '<br/></span>');
                }

                _item.append(ico);
                //_item.append(info_1);
                _item.append(info_2);
                _item.append($('<div class="clear"><br class="clear"/></div>'));

            }
            $(_item).find('.icon').doubletap(
            //funzione del doppio tap
                function (event) {
                    if (swipeBool) {
                        mobileclient._item = el;
                        mobileclient.getItem(el)
                    }

                },
            /** funzione del tap singolo*/
                    function (event) {
                        mobileclient._item = el;
                        mobileclient.chooseDoc(_item);
                    },
            /** tempo di timeout sul quale fare il controllo */
                400
            );

            $('#ricerca_item > .item_list').append(_item);
        });
        if (this.model.TabModel.NumTotPages == 0) {
            if (this.model.TabModel.IdParent) {
                $('#ricerca_item > .item_list').html('<div class="no_todolist_fascicolo">&nbsp;</div>');
            } else {
                $('#ricerca_item > .item_list').html('<div class="no_ricerca">&nbsp;</div>');
            }
        }

        $('#ricerca_item').show()
        $('#ricerca_item > .item_list').show()
        this.setitemposition('#ricerca_item > .item_list');

    },
    ricerca_filtri: function (isPrepend) {
        $('.filter_ricerca').remove();

        var valueFilter = '<div class="filter_ricerca"><h2 class="onlyIphone">Ricerca Rapida</h2><span class="iPhoneFilter search" id="iPhoneFilterS"></span><select class="onlyIphone"></select><div class="puls_cerca onlyIphone">Cerca</div><ul><li class="noGalaxy on" id="RIC_DOCUMENTO">Documenti</li><li  class="noGalaxy" id="RIC_FASCICOLO">Fascicoli</li><!--<li class="on noIpad firstGalaxy" id="RIC_LIBERA">Mostra</li>--><li id="RIC_SALVATE">Preferite</li></ul></div>';


        if (isPrepend) {
            $('#service_bar').addClass('skinGrey').addClass('hauto').find('.scelta_ricerca').after(valueFilter);

        } else {

            $('#service_bar').addClass('skinGrey').addClass('hauto').append(valueFilter);
        }


        if (mobileclient.model.TabModel.TypeRicerca.RIC_DOC_FASC) {
            $('#RIC_LIBERA').attr('val', 'RIC_DOC_FASC');
        }
        else if (mobileclient.model.TabModel.TypeRicerca.RIC_DOCUMENTO) {
            $('#RIC_LIBERA').attr('val', 'RIC_DOCUMENTO');
        }
        else if (mobileclient.model.TabModel.TypeRicerca.RIC_FASCICOLO) {
            $('#RIC_LIBERA').attr('val', 'RIC_FASCICOLO');
        }


        $('.filter_ricerca > #iPhoneFilterS').click(function () {
            if (DEVICE == 'smart') mobileclient.skinSelect($('.filter_ricerca > SELECT').eq(0));
        });


        $('.filter_ricerca > SELECT').change(function () {
            var type = $($(this).children()[this.selectedIndex]).attr('rel');
        });


        var ricerche = $(mobileclient.model.TabModel.Ricerche);

        if (ricerche[0]) {
            if (ricerche[0].Type.RIC_DOCUMENTO) {
                var tipo = 'RIC_DOCUMENTO';
                var sigla = 'D';
            }
            else if (ricerche[0].Type.RIC_FASCICOLO) {
                var tipo = 'RIC_FASCICOLO';
                var sigla = 'F';
            }

            var testoDefault = sigla + ' ' + ricerche[0].Descrizione;
            $('#iPhoneFilterS').text(testoDefault);

        } else {
            $('#iPhoneFilterS').remove();

        }

        ricerche.each(function (i, el) {

            if (el.Type.RIC_DOCUMENTO) {
                var tipo = 'RIC_DOCUMENTO';
                var sigla = 'D';
            }
            else if (el.Type.RIC_FASCICOLO) {
                var tipo = 'RIC_FASCICOLO';
                var sigla = 'F';
            }

            if (mobileclient.model.TabModel.IdRicercaSalvata == el.Id) {
                selected = 'selected="selected"';
                $('#iPhoneFilterS').text(sigla + ' ' + el.Descrizione);
            }
            else selected = '';

            var descrizione = el.Descrizione;

            $('.filter_ricerca > SELECT').append($('<option id="' + el.Id + '" value="' + el.Id + '|' + tipo + '" ' + selected + '>' + sigla + ' ' + el.Descrizione + '</option>'));

        });


        if ($('.filter_ricerca > SELECT').children().length != 0) {
            $('#trasmissione_doc > .filter_trasm > .puls_cerca').removeClass('disabled');

            $('.filter_ricerca > .puls_cerca').click(function () {
                if ($('.filter_ricerca').children('select').val()) {
                    var value = $('.filter_ricerca').children('select').val().split('|');
                }
                else {
                    var value = $('.filter_ricerca > div > select').val().split('|');
                }


                mobileclient.filtroRicerca(value[0], value[1]);
            });
        } else {
            $('#trasmissione_doc > .filter_trasm > .puls_cerca').addClass('disabled');
        }


        //carico metto funzioni a tutti gli elementi e faccio le varie casistiche
        //parte comune per tutti, solo all'interno aggiungerò un IF per RIC_LIBERA presente solo su galaxy in verticale
        $('#service_bar > .filter_ricerca > UL > LI').each(function (i, el) {

            $(el).click(function () {
                $('.scelta_ricerca > form > input').attr('value', '')

                $('#service_bar > .filter_ricerca > ul > li').removeClass('on');
                $(this).addClass('on');

                //in base al click mostro o la select o l'input
                if ($(this).attr('id') == 'RIC_SALVATE') {

                    if ($('#box_ricerchesalvate').is(':visible')) {
                        $('.box_ricerche').remove();
                    }
                    else {
                        $('.box_ricerche').remove(); //tolgo nel dubbio tutti i box di ricerca
                        //disattivo l'input
                        $('.scelta_ricerca > form > input').attr('readonly', "readonly");
                        $('.scelta_ricerca > form > input').addClass('readonly');

                        //aggiungo il box della ricerca salvata
                        $('#service_bar').append($('<div id="box_ricerchesalvate" class="box_ricerche"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Seleziona ricerca preferita</h3><ul></ul></div>'));
                        mobileclient.reposArrow('RIC_SALVATE');
                        $('#box_ricerchesalvate > button').click(function () {

                            $('#box_ricerchesalvate').remove()
                        })

                        //prima riempio la select
                        var ricerche = $(mobileclient.model.TabModel.Ricerche);

                        ricerche.each(function (i, el) {
                            //controllo se documento o fascicolo per passare il type del documento
                            if (el.Type.RIC_DOCUMENTO) {
                                var tipo = 'RIC_DOCUMENTO';
                                var sigla = 'D';
                            }
                            else if (el.Type.RIC_FASCICOLO) {
                                var tipo = 'RIC_FASCICOLO';
                                var sigla = 'F';
                            }

                            //nel caso in cui sono già dentro una ricerca salvata metto il selected nella option
                            if (mobileclient.model.TabModel.IdRicercaSalvata == el.Id) selected = 'class="on"';
                            else selected = '';


                            $('#box_ricerchesalvate > UL').append(
                                $('<li id="' + el.Id + '" ' + selected + '>' + sigla + ' ' + el.Descrizione + '</li>').click(function () {
                                    mobileclient.filtroRicerca(el.Id, tipo);
                                }));
                        });

                    }
                }
                else {
                    //se non è una ricerca salvata aggiungo gli input
                    $('.scelta_ricerca > form > input').removeAttr('readonly');
                    $('.scelta_ricerca > form > input').removeClass('readonly');
                    $('#box_ricerchesalvate').remove();


                    if ($(this).attr('id') == 'RIC_LIBERA') {


                        //se non è salvata ma è libera dunque sono in android modalità verticale
                        if ($('#box_ricerchesalvate').is(':visible')) {
                            $('.box_ricerche').remove();
                        }
                        else {
                            $('.box_ricerche').remove();
                            //aggiungo il box
                            //<li id="RIC_DOC_FASC">Tutti</li>
                            $('#service_bar').append($('<div id="box_ricerchelibere" class="box_ricerche noIpad"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Seleziona tipo ricerca</h3><ul><li id="RIC_DOCUMENTO">Documenti</li><li id="RIC_FASCICOLO">Fascicoli</li></ul></div>'));


                            //spengo tutte le voci, e se ho fatto una ricerca libera e accendo la sottovoce selezionata prima
                            $('#box_ricerchelibere > ul > li').removeClass('on');
                            $('#' + $('#RIC_LIBERA').attr('val')).addClass('on');


                            mobileclient.reposArrow('RIC_LIBERA');

                            //azione alle voci della ricerca libera
                            $('#box_ricerchelibere').find('li').each(function (i, el) {
                                $(el).click(function () {
                                    //salvo il valore nel LI
                                    $('#RIC_LIBERA').attr('val', $(this).attr('id'));

                                    $('.box_ricerche').remove();
                                });

                            });

                            //azione al pulsante annulla
                            $('#box_ricerchelibere > button').click(function () {
                                $('#box_ricerchelibere').remove()
                            })


                        }
                    }
                    if (DEVICE == 'ipad' || mobileclient.contenType != 'normal') {
                        mobileclient.ricerca($('.scelta_ricerca > form > input').attr('value'), $('#service_bar > .filter_ricerca > ul > li.on').attr('id'));
                        return false;

                    }
                }

            })

        });
        //verifico se c'è settato un tipo di ricerca
        if (mobileclient.model.TabModel.IdRicercaSalvata) {

            $('#service_bar > .filter_ricerca > ul > li').removeClass('on');
            $('#RIC_SALVATE').addClass('on');
            mobileclient.reposArrow('RIC_SALVATE');

            //disattivo l'input
            $('.scelta_ricerca > form > input').attr('readonly', "readonly");
            $('.scelta_ricerca > form > input').addClass('readonly');

        }
        else {

            //spengo i filtri di ricerca e accendo quello che mi serve, come fatto con il menu a tendina verticale android
            $('#service_bar > .filter_ricerca > ul > li').removeClass('on');

            if ($('#RIC_LIBERA').length > 0) {
                //se sono in ricerca libera, accendo questo, altrimenti vuol dire che ho tutte le voci x esteso
                mobileclient.reposArrow('RIC_LIBERA');
                $('#RIC_LIBERA').addClass('on');
            }
            else {
                if (this.model.TabModel.TypeRicerca.RIC_DOC_FASC) {
                    $('#RIC_DOC_FASC').addClass('on');
                }
                else if (this.model.TabModel.TypeRicerca.RIC_DOCUMENTO) {
                    $('#RIC_DOCUMENTO').addClass('on');
                }
                else if (this.model.TabModel.TypeRicerca.RIC_FASCICOLO) {
                    $('#RIC_FASCICOLO').addClass('on');
                }
            }

            //meccanismo ricerca
        }
        if (DEVICE == 'ipad' || mobileclient.contenType != 'normal') {
            // metto action per l'input
            $('.scelta_ricerca > form').submit(function () {
                mobileclient.ricerca($('.scelta_ricerca > form > input').attr('value'), $('#service_bar > .filter_ricerca > ul > li.on').attr('id'));
                return false;
            });
        }
        else {
            // metto action per l'input
            $('.scelta_ricerca > form').submit(function () {
                mobileclient.ricerca($('.scelta_ricerca > form > input').attr('value'), $('#RIC_LIBERA').attr('val'));
                return false;
            });

        }
    },
    libroFirma_filtri: function () {
        $('#service_bar').removeClass('skinGrey');

        $('#service_bar').append('<div class="filter_ricerca"><ul><li class="noGalaxy lastIpad" id="RIC_OGGETTO_LF">Oggetto</li><li class="noGalaxy lastIpad" id="RIC_NOTE_LF">Note</li><li class="noGalaxy lastIpad" id="RIC_PROPONENTE_LF">Proponente</li><!--<li class="on noIpad firstGalaxy" id="RIC_LIBERA">Mostra</li>--></ul></div>');

        if (mobileclient.model.TabModel.TypeRicerca.RIC_OGGETTO_LF) {
            $('#RIC_LIBERA').attr('val', 'RIC_OGGETTO_LF');
        }
        else if (mobileclient.model.TabModel.TypeRicerca.RIC_NOTE_LF) {
            $('#RIC_LIBERA').attr('val', 'RIC_NOTE_LF');
        }
        else if (mobileclient.model.TabModel.TypeRicerca.RIC_PROPONENTE_LF) {
            $('#RIC_LIBERA').attr('val', 'RIC_PROPONENTE_LF');
        }

        $('#service_bar > .filter_ricerca > UL > LI').each(function (i, el) {

            $(el).click(function () {

                if (this.className != 'on') {
                    $('.scelta_ricerca > form > input').val("");
                }
                $('#service_bar > .filter_ricerca > ul > li').removeClass('on');
                $(this).addClass('on');
            });

        });

        if (this.model.TabModel.TypeRicerca.RIC_OGGETTO_LF) {
            $('#RIC_OGGETTO_LF').addClass('on');
        }
        else if (this.model.TabModel.TypeRicerca.RIC_NOTE_LF) {
            $('#RIC_NOTE_LF').addClass('on');
        }
        else if (this.model.TabModel.TypeRicerca.RIC_PROPONENTE_LF) {
            $('#RIC_PROPONENTE_LF').addClass('on');
        }

        $('.scelta_ricerca > form').submit(function () {
            mobileclient.ricercaLF($('.scelta_ricerca > form > input').attr('value'), $('#service_bar > .filter_ricerca > ul > li.on').attr('id'));
            return false;
        });

    },
    adl_filtri: function () {
        $('.filter_ricerca').remove();
        $('#service_bar').removeClass('skinGrey');

        $('#service_bar').append('<div class="filter_ricerca"><ul><li class="noGalaxy on" id="RIC_DOCUMENTO_ADL">Documenti</li><li class="noGalaxy lastIpad" id="RIC_FASCICOLO_ADL">Fascicoli</li><!--<li class="on noIpad firstGalaxy" id="RIC_LIBERA">Mostra</li>--></ul></div>');

        if (mobileclient.model.TabModel.TypeRicerca.RIC_DOC_FASC_ADL) {
            $('#RIC_LIBERA').attr('val', 'RIC_DOC_FASC_ADL');
        }
        else if (mobileclient.model.TabModel.TypeRicerca.RIC_DOCUMENTO_ADL) {
            $('#RIC_LIBERA').attr('val', 'RIC_DOCUMENTO_ADL');
        }
        else if (mobileclient.model.TabModel.TypeRicerca.RIC_FASCICOLO_ADL) {
            $('#RIC_LIBERA').attr('val', 'RIC_FASCICOLO_ADL');
        }

        //carico metto funzioni a tutti gli elementi e faccio le varie casistiche
        //parte comune per tutti, solo all'interno aggiungerò un IF per RIC_LIBERA presente solo su galaxy in verticale
        $('#service_bar > .filter_ricerca > UL > LI').each(function (i, el) {

            $(el).click(function () {
                if (this.className != 'on') {
                    $('.scelta_ricerca > form > input').val("");
                }
                $('#service_bar > .filter_ricerca > ul > li').removeClass('on');
                $(this).addClass('on');


                //se non è una ricerca salvata aggiungo gli input
                $('.scelta_ricerca > form > input').removeAttr('readonly');
                $('.scelta_ricerca > form > input').removeClass('readonly');
                $('#box_ricerchesalvate').remove();


                if ($(this).attr('id') == 'RIC_LIBERA') {

                    //se non è salvata ma è libera dunque sono in android modalità verticale
                    if ($('#box_ricerchesalvate').is(':visible')) {
                        $('.box_ricerche').remove();
                    }
                    else {
                        $('.box_ricerche').remove();
                        //aggiungo il box
                        //<li id="RIC_DOC_FASC_ADL">Tutti</li>
                        $('#service_bar').append($('<div id="box_ricerchelibere" class="box_ricerche noIpad"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Seleziona tipo ricerca</h3><ul><li id="RIC_DOCUMENTO_ADL">Documenti</li><li id="RIC_FASCICOLO_ADL">Fascicoli</li></ul></div>'));

                        //spengo tutte le voci, e se ho fatto una ricerca libera e accendo la sottovoce selezionata prima
                        $('#box_ricerchelibere > ul > li').removeClass('on');
                        $('#' + $('#RIC_LIBERA').attr('val')).addClass('on');
                        mobileclient.reposArrow('RIC_LIBERA');

                        //azione alle voci della ricerca libera
                        $('#box_ricerchelibere').find('li').each(function (i, el) {
                            $(el).click(function () {
                                //salvo il valore nel LI
                                $('#RIC_LIBERA').attr('val', $(this).attr('id'));
                                mobileclient.ricercaAdl($('.scelta_ricerca > form > input').attr('value'), $('#RIC_LIBERA').attr('val'));
                                $('.box_ricerche').remove();
                            });

                        });

                        //azione al pulsante annulla
                        $('#box_ricerchelibere > button').click(function () {
                            $('#box_ricerchelibere').remove()
                        })


                    }
                }

                if (DEVICE == 'ipad' || mobileclient.contenType != 'normal') {
                    mobileclient.ricercaAdl($('.scelta_ricerca > form > input').attr('value'), $('#service_bar > .filter_ricerca > ul > li.on').attr('id'));
                    return false;

                }
                /*
                else {
                mobileclient.ricercaAdl($('.scelta_ricerca > form > input').attr('value'), $('#RIC_LIBERA').attr('val'));
                return false;
                }
                */

            })

        });
        //verifico se c'è settato un tipo di ricerca
        if (mobileclient.model.TabModel.IdRicercaSalvata) {

            $('#service_bar > .filter_ricerca > ul > li').removeClass('on');
            $('#RIC_SALVATE').addClass('on');
            mobileclient.reposArrow('RIC_SALVATE');

            //disattivo l'input
            $('.scelta_ricerca > form > input').attr('readonly', "readonly");
            $('.scelta_ricerca > form > input').addClass('readonly');

        }
        else {

            //spengo i filtri di ricerca e accendo quello che mi serve, come fatto con il menu a tendina verticale android
            $('#service_bar > .filter_ricerca > ul > li').removeClass('on');

            if ($('#RIC_LIBERA').length > 0) {
                //se sono in ricerca libera, accendo questo, altrimenti vuol dire che ho tutte le voci x esteso
                $('#RIC_LIBERA').addClass('on');
            }
            else {

                if (this.model.TabModel.TypeRicerca.RIC_DOC_FASC) {
                    $('#RIC_DOC_FASC_ADL').addClass('on');
                }
                else if (this.model.TabModel.TypeRicerca.RIC_DOCUMENTO_ADL) {
                    $('#RIC_DOCUMENTO_ADL').addClass('on');
                }
                else if (this.model.TabModel.TypeRicerca.RIC_FASCICOLO_ADL) {
                    $('#RIC_FASCICOLO_ADL').addClass('on');
                }
            }

            //meccanismo ricerca
        }
        if (DEVICE == 'ipad' || mobileclient.contenType != 'normal') {
            // metto action per l'input
            $('.scelta_ricerca > form').submit(function () {
                mobileclient.ricercaAdl($('.scelta_ricerca > form > input').attr('value'), $('#service_bar > .filter_ricerca > ul > li.on').attr('id'));
                return false;
            });
        }
        else {
            // metto action per l'input
            $('.scelta_ricerca > form').submit(function () {
                mobileclient.ricercaAdl($('.scelta_ricerca > form > input').attr('value'), $('#RIC_LIBERA').attr('val'));
                return false;
            });

        }


    },
    updateDettaglioTrasm: function (data) {
        //alert('updateDettaglioTrasm\n' + data.TabModel.TrasmInfo);

        this.checkSession(data);
        $('#actionIphone').html('');


        var fascinfo = data.TabModel.FascInfo;
        var docinfo = data.TabModel.DocInfo;
        var infotrasm = data.TabModel.TrasmInfo;


        //#< 20140305 
        if (DEVICE == 'smart') {
            var indietro = $('<div class="pulsante_classic fll  paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
                mobileclient.loadPrevious()
            });
            $('#actionIphone').append(indietro);
        }
        //#>

        if (infotrasm) {//se non è un documento carico le info di trasmissione
            //alert('carico le info di trasmissione: ');
            $($('.dett_trasm > table > tbody > tr > td')[0]).html(data.TabModel.TrasmInfo.Mittente)
            $($('.dett_trasm > table > tbody > tr > td')[1]).html(data.TabModel.TrasmInfo.Ragione)
            $($('.dett_trasm > table > tbody > tr > td')[2]).html(this.formatDate(data.TabModel.TrasmInfo.Data))

            $('#sezione_trasmissione > P').html(data.TabModel.TrasmInfo.NoteGenerali)
            $('#sezione_trasmissione').removeClass('none').removeClass('hide');

            //Aggiorno l'item con i dati ricevuti
            this._item.HasWorkflow = data.TabModel.TrasmInfo.HasWorkflow;
            this._item.TrasmInfo = data.TabModel.TrasmInfo;
            this._item.DocInfo = data.TabModel.DocInfo;
            this._item.FascInfo = data.TabModel.FascInfo;
            if (this._item.HasWorkflow == true) {
                var mesi = new Array('Gennaio', 'Febbraio', 'Marzo',
                    'Aprile', 'Maggio', 'Giugno', 'Luglio', 'Agosto', 'Settembre',
                    'Ottobre', 'Novembre', 'Dicembre');
                var date = new Date();
                var gg = date.getDate();
                var mese = date.getMonth();
                var yy = date.getYear();
                var yyyy = (yy < 1000) ? yy + 1900 : yy;

                var mess = '';
                if (this.model.DelegaEsercitata != undefined && this.model.DelegaEsercitata != null)
                    mess = 'in data ' + gg + " " + mesi[mese] + " " + yyyy + ' da ' + this.model.DelegaEsercitata.Delegato + ' (delegato da ' + this.model.DescrUtente + ') dal dispositivo mobile.';
                else
                    mess = 'in data ' + gg + " " + mesi[mese] + " " + yyyy + ' da ' + this.model.DescrUtente + ' dal dispositivo mobile.';

                var rifiuta = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Rifiuta</a></div>').click(function () {
                    tooltip.init({ "titolo": 'Inserisci note di rifiuto', "content": $('<textarea></textarea>').html('Rifiutata ' + mess), 'deny': 'Annulla', 'confirm': 'Rifiuta', 'returntrue': function () {
                        mobileclient.rifiutaTrasm()
                    }
                    });
                });
                var accetta = $('<div class="pulsante_classic fll"><a href="javascript:;">Accetta</a></div>').click(function () {
                    tooltip.init({ "titolo": 'Inserisci note di accettazione', "content": $('<textarea></textarea>').html('Accettata ' + mess), 'deny': 'Annulla', 'confirm': 'Accetta', 'returntrue': function () {
                        mobileclient.accettaTrasm()
                    }
                    });
                });
                //var trasmetti = $('<div class="pulsante_classic fll"><a href="javascript:;">Trasmetti</a></div>').click(function () { mobileclient.trasmissioneForm(docinfo,fascinfo) });


                $('#actionIphone').append(rifiuta);
                $('#actionIphone').append(accetta);
                $('#actionIphone').removeClass('hideIphone').addClass('showIphone');

                //$('#actionIphone').append(trasmetti);

            }


        }
        else {
            //alert('non carico le info di trasmissione');
            if (data.TabModel.IdEvento != null) {
                this._item.DocInfo = data.TabModel.DocInfo;
                this._item.FascInfo = data.TabModel.FascInfo;
            }
            $('#sezione_trasmissione').addClass('none').addClass('hide');

            var indietro = $('<div class="pulsante_classic fll paddingLeft onlyIphone"><a href="javascript:;">Indietro</a></div>').click(function () {
                mobileclient.showTab('RICERCA')
            });
            $('#service_bar').append(indietro);

            var trasmetti = $('<div class="pulsante_classic flr paddingRight onlyIphone" id="transm"><a href="javascript:;">Trasmetti</a></div>').click(function () {

                if (mobileclient.model.TabShow.DETTAGLIO_DOC) {
                    //trasmetto dall'anteprima di un doc
                    mobileclient.trasmissioneForm(mobileclient.model.TabModel.DocInfo, mobileclient.model.TabModel.DocInfo.IdDoc, "")
                }
                else {
                    //trasmetto da ricerca oppure todolist
                    mobileclient.trasmissioneForm(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, "")
                }
            });

            $('#service_bar').append(trasmetti);
            $('.filter_trasm').remove();

            $('#service_bar').removeClass('hideService');
            $('#service_bar').removeClass('skinGrey').removeClass('hauto');
            $('#service_bar .filter_ricerca').html('');
            $('#actionIphone').addClass('hideIphone').removeClass('showIphone');
            $('#sezione_trasmissione').addClass('none');
        }
        if (data.TabModel.DocInfo) {
            $('#note_aggiuntive').html('');

            // forzatura visualizzazione div info trasmissioni per risoluzione bug mancata visualizzazione dopo giro TODO_LIST » ADL » TODO_LIST
            if ($('#sezione_trasmissione').css('display') == 'none')
                $('#sezione_trasmissione').css({ 'display': '' });
            //            if (mobileclient.model.TabShow.TODO_LIST)
            //                $('#sezione_trasmissione').css({ 'display': 'block' });
            //            else if (mobileclient.model.TabShow.AREA_DI_LAVORO)
            //                $('#sezione_trasmissione').css({ 'display': 'block' });

            $($('#sezione_documento > H2')[0]).html('Informazioni del documento')

            $($('#sezione_documento > DIV > P')[0]).html(data.TabModel.DocInfo.Oggetto)
            $($('#sezione_documento > DIV > P')[1]).html(data.TabModel.DocInfo.Note)


            $('#note_aggiuntive').append($('<div><h3>Id Documento</h3><p>' + docinfo.IdDoc + '</p></div>'));


            if (docinfo.IsProtocollato) {
                $('#note_aggiuntive').append($('<div><h3>Segnatura</h3><p>' + docinfo.Segnatura + '</p></div>'));
                $('#note_aggiuntive').append($('<div><h3>Data protocollazione</h3><p>' + this.formatDate(docinfo.DataProto) + '</p></div>'));
                if (docinfo.TipoProto != 'P') $('#note_aggiuntive').append($('<div><h3>Mittente</h3><p>' + docinfo.Mittente + '</p></div>'));

                if (docinfo.TipoProto == "P") {
                    var stringa_destinatari = '';
                    var destinatari = docinfo.Destinatari;
                    for (var i = 0; i < destinatari.length; i++) {
                        stringa_destinatari += destinatari[i] + "\n";
                    }

                    $('#note_aggiuntive').append($('<div><h3>Destinatari protocollo</h3><p>' + stringa_destinatari + '</p></div>'));
                }
            }
            else {
                $('#note_aggiuntive').append($('<div><h3>Data creazione</h3><p>' + this.formatDate(docinfo.DataDoc) + '</p></div>'));
            }

            if (docinfo.OriginalFileName)
                $('#note_aggiuntive').append($('<div><h3>Nome Originale</h3><p>' + docinfo.OriginalFileName + '</p></div>'));

            //#< 20140311 aggiunta info tipo protocollo 
            if (docinfo.TipoProto) {
                var tipoProto;
                switch (docinfo.TipoProto) {
                    case 'A':
                        tipoProto = 'P. Arrivo';
                        break;
                    case 'I':
                        tipoProto = 'P. Interno';
                        break;
                    case 'P':
                        tipoProto = 'P. Partenza';
                        break;
                    case 'E':
                        tipoProto = 'P. Entrata';
                        break;
                    case 'U':
                        tipoProto = 'P. Uscita';
                        break;
                    default:
                        tipoProto = 'Non Protocollato';
                        break;
                }
                //20140324 info tipo doc allegato
                if (docinfo.IdDocPrincipale) tipoProto = "Allegato";
                $('#note_aggiuntive').append($('<div><h3>Tipo Documento</h3><p>' + tipoProto + '</p></div>'));
            }
            //#>

            var fascicoli = docinfo.Fascicoli;
            var stringa_fascicoli = '';
            for (var i = 0; i < fascicoli.length; i++) {
                stringa_fascicoli += fascicoli[i][0] + ' - ' + fascicoli[i][1] + '<br/>';
            }

            $('#note_aggiuntive').append($('<div><h3>Codice - Descrizione Fascicolo</h3><p>' + stringa_fascicoli + '</p></div>'));


        }
        else {
            $($('#sezione_documento > H2')[0]).html('Informazioni del fascicolo')

            $($('#sezione_documento > DIV > H3')[0]).html('Descrizione')

            $($('#sezione_documento > DIV > P')[0]).html(data.TabModel.FascInfo.Descrizione)
            $($('#sezione_documento > DIV > P')[1]).html(data.TabModel.FascInfo.Note)

            $('#note_aggiuntive').append($('<div><h3>Codice-Descrizione Fascicolo</h3><p>' + data.TabModel.FascInfo.Codice + '</p></div>'));

            //#< 20140312 Aggiunta info stato protocollo
            var dataChiusura = data.TabModel.FascInfo.DataChiusura.replace('/Date(', '').replace(')/', '');
            var dataChiusura = new Date(parseInt(dataChiusura));
            var dataChiusura = dataChiusura.format('dd/MM/yyyy');
            if (dataChiusura != '01/01/1') {
                $('#note_aggiuntive').append($('<div><h3>Stato del Fascicolo</h3><p>Chiuso in data ' + dataChiusura + '</p></div>'));
            }
            else
                $('#note_aggiuntive').append($('<div><h3>Stato del Fascicolo</h3><p>Aperto</p></div>'));
            //#>
        }
        $('#sezione_documento').removeClass('hide');
        $('#info_trasm').removeAttr('style');

        if (docinfo) {
            var visualizza = $('<div class="onlyIphone paddingRight control_info2 premuto"></div>').click(function () {
                //#< 20140305 nasconde il pannello info trasm se attivo
                if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
                //#>
                mobileclient.dettaglioDoc(docinfo.IdDoc)

            });
        }
        else {
            var visualizza = $('<div class="pulsante_classic flr paddingRight"><a href="javascript:;">Contenuto</a></div>').click(function () {


                mobileclient.inFolder(fascinfo.IdFasc, fascinfo.Descrizione, infotrasm.IdTrasm);
            });
        }

        $('#actionIphone').append(visualizza);
        $('#actionIphone').removeClass('hideIphone').addClass('showIphone');
    },
    updateTrasmissione: function (data) {
        this.checkSession(data);
        $('#transm').remove();
        $('#info_trasm').addClass('none');
        $('#info_trasm').hide();
        $('#sezione_acttrasm').remove();
        $('#note_aggiuntive').html('');
        $('.box_ricerche').remove();

        $('#service_bar > form').addClass('hideService');
        $('#service_bar > div').addClass('hideService');
        if (!data.TabModel.DocInfo) var name = 'fascicolo';
        else var name = 'documento';
        $('#service_bar').append('<div id="name_onservicebarT">Trasmissione ' + name + '</div>');

        var indietro = $('<div class="pulsante_classic fll  paddingLeft noIphone" id="indietro_trasm"><a href="javascript:;">Indietro</a></div>').click(function () {
            mobileclient.control_info()
        });
        $('#service_bar').append(indietro);

        this.control_azioni();
        this.control_info(data);


    },
    popAddFirma: function (el) {


        $('.pop').remove();
        $('#info_trasm').addClass('none').hide();
        $('.control_azioni,.control_info').removeClass('premuto')
        $('#box_azioni').remove();
        $('#firmaView').addClass('hide');
        // Gabriele Melini 02-10-2014
        // gestione abilitazione tasto Richiedi OTP
        // -----------
        //alert('here');
        $.ajax({
            url: mobileclient.context.WAPath + '/Firma/HSMIsAllowedOTP',
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                if (result.HSMIsAllowedOTP) {
                    $('#otpBut').show();
                }
                else {
                    $('#otpBut').hide();
                }
            },
            async: false
        });
        if ($(el).hasClass('selected')) {
            $(el).removeClass('selected');
            $('#firma').addClass('hide');
        } else {
            $('#box_azioni > UL > LI').removeClass('selected');

            $(el).addClass('selected');
            $('.control_azioni').removeClass('premuto')
            $('#box_azioni').remove();
            $('#firma').removeClass('hide');

            //blank valori in pin / otp
            $('#pin').val("");
            $('#otp').val("");
            $('#alias').val("");
            $('#dominio').val("");

            //mauro20140214 - Info Memento : popolamento campi Alias/Dominio con richiesta asincrona
            //INIZIO
            var idDoc = mobileclient.model.TabModel.DocInfo.IdDoc;
            $.ajax({
                url: this.context.WAPath + "/Firma/HSMGetMemento?idDoc=" + idDoc,
                type: "POST",
                data: null,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                success: function (result) {

                    if (result.HSMMemento) {
                        $('#alias').val(result.HSMMemento.Alias);
                        $('#dominio').val(result.HSMMemento.Dominio);
                    }
                }
            });
            //FINE


            //aggiunto set focus su pin
            $('#pin').focus();


            // Mauro aggiunto gestione pulsante di richiesta otp
            //start
            $('#otpBut').unbind('click').bind('click', function (e) {

                var continua = true;
                $('#firma').find('#alias,#dominio').each(function () {
                    if (($(this).val() == '') && continua) {
                        continua = false;
                        var label = $('#firma').find('label[for=' + $(this).attr('id') + ']').text().split('*')[0] + ' obbligatorio';
                        input = this;
                        $(this).addClass('error');
                        window.scrollTo(0, 0);
                        tooltip.init({ "titolo": label, 'confirm': 'Ok', 'returntrue': function () {
                            input.focus();
                            $('#tooltip').hide();
                        }
                        });

                    } else {
                        $(this).removeClass('error');
                    }
                })

                if (continua) {
                    var aliasValue = $('#alias').val();
                    var dominioValue = $('#dominio').val();
                    e.preventDefault();
                    var objToSend = { aliasCertificato: aliasValue, dominioCertificato: dominioValue };
                    $('#loading').show();

                    $.post(mobileclient.context.WAPath + '/Firma/HSMRequestOTP', objToSend, function (data) {
                        if (data.HSMRequestOTPResult == true) {
                            message = 'Invio della Richiesta OTP effetuata con successo';
                        }
                        else {
                            message = 'Errore nell\'invio della Richiesta OTP';
                        }
                        tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                            $('#loading').hide();
                        }
                        });

                    }, 'json');
                }
            });
            //end

            $('#closeBut').unbind('click').bind('click', function (e) {
                e.preventDefault();
                $('#firma').addClass('hide');
            });

            $('#firmaBut').unbind('click').bind('click', function (e) {
                e.preventDefault();

                var continua = true;
                var input;
                $('#firma').find('input[required]').each(function () {
                    if (($(this).val() == '') && continua) {
                        continua = false;
                        var label = $('#firma').find('label[for=' + $(this).attr('id') + ']').text().split('*')[0] + ' obbligatorio';
                        input = this;
                        $(this).addClass('error');
                        window.scrollTo(0, 0);
                        tooltip.init({ "titolo": label, 'confirm': 'Ok', 'returntrue': function () {
                            input.focus();
                            $('#tooltip').hide();
                        }
                        });

                    } else {
                        $(this).removeClass('error');
                    }
                })


                if (continua) {
                    var firma = {};
                    firma.iddoc = mobileclient.model.TabModel.DocInfo.IdDoc;
                    $('#firma').find('input').each(function () {
                        if ($(this).is(':radio')) {
                            if ($(this).is(':checked')) {
                                firma[$(this).attr('name')] = $(this).val();
                            }
                        }
                        else if ($(this).is(':checkbox')) {
                            firma[$(this).attr('name')] = $(this).is(':checked');
                        }

                        else {
                            firma[$(this).attr('name')] = $(this).val();
                        }
                    });
                    $('#loading').show();
                    var objToSend = { iddoc: firma.iddoc, cofirma: firma.cofirma, timestamp: firma.timestamp, tipoFirma: firma.tipoFirma, alias: firma.alias, dominio: firma.dominio, otp: firma.otp, pin: firma.pin, convert: firma.convert };

                    $.post(mobileclient.context.WAPath + '/Firma/HSMSign', objToSend, function (data) {
                        //console.log(data)
                        if (data.HSMSignResult == true) {
                            message = 'Documento firmato con successo';
                            isTrueFirma = true;
                            isFirmato = true;
                        }
                        else {
                            message = 'Errore durante l\'operazione di firma';
                            isTrueFirma = false;
                        }
                        tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                            $('#loading').hide();
                            if (isTrueFirma) {
                                $('#firma,#firmaView').addClass('hide');
                                $('#box_azioni > UL > LI').removeClass('selected');
                            }
                        }
                        });

                    }, 'json');

                }
            })

        }
    },

    controllaFirmaInit: function () {
        //isFirmato
        var objToSend = { iddoc: mobileclient.model.TabModel.DocInfo.IdDoc };
        $.post(mobileclient.context.WAPath + '/Firma/HSMVerifySign', objToSend, function (data) {

            if (data.HSMVerifySignResult) {
                isFirmato = true;

                if ($('#bfirma').size() == 0) {
                    mobileclient.controllaFirma();
                } else if ($('#bfirma').size() == 1) {
                    $('#bfirma').remove();
                    mobileclient.controllaFirma();
                }

            } else {
                isFirmato = false;

            }
        }, 'json');


    },

    popAddFirmaFromLf: function (isCades, isSignedCades) {
        $('#loading').hide();
        $('.pop').remove();
        $('#info_trasm').addClass('none').hide();
        $('.control_azioni,.control_info').removeClass('premuto')
        $('#box_azioni').remove();
        $('#firmaView').addClass('hide');

        $("#pdf").attr('disabled', true);
        $("#p7m").attr('disabled', true);

        if (isCades) {
            $("#pdf").attr('checked', false);
            $("#p7m").attr('checked', true);
        }
        else {
            $("#p7m").attr('checked', false);
            $("#pdf").attr('checked', true);
        }
        if (isSignedCades) {
            message = 'Attenzione!Ci sono file su cui apporre firma digitale PADES. E\' necessario reinserire l\'OTP';
            tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                input.focus();
                $('#tooltip').hide();
            }

            });
        }
        $("#cosign").addClass('hide');
        // Gabriele Melini 02-10-2014
        // gestione abilitazione tasto Richiedi OTP
        // -----------
        //alert('here');
        $.ajax({
            url: mobileclient.context.WAPath + '/Firma/HSMIsAllowedOTP',
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                if (result.HSMIsAllowedOTP) {
                    $('#otpBut').show();
                }
                else {
                    $('#otpBut').hide();
                }
            },
            async: false
        });

        $('#box_azioni > UL > LI').removeClass('selected');

        $('.control_azioni').removeClass('premuto')
        $('#box_azioni').remove();
        $('#firma').removeClass('hide');

        //blank valori in pin / otp
        $('#pin').val("");
        $('#otp').val("");
        $('#alias').val("");
        $('#dominio').val("");
        //mauro20140214 - Info Memento : popolamento campi Alias/Dominio con richiesta asincrona
        //INIZIO
        var idDoc = "0";
        $.ajax({
            url: this.context.WAPath + "/Firma/HSMGetMemento?idDoc=" + idDoc,
            type: "POST",
            data: null,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (result) {

                if (result.HSMMemento) {
                    $('#alias').val(result.HSMMemento.Alias);
                    $('#dominio').val(result.HSMMemento.Dominio);
                }
            }
        });
        //FINE
        // Mauro aggiunto gestione pulsante di richiesta otp
        //start
        $('#otpBut').unbind('click').bind('click', function (e) {

            var continua = true;
            $('#firma').find('#alias,#dominio').each(function () {
                if (($(this).val() == '') && continua) {
                    continua = false;
                    var label = $('#firma').find('label[for=' + $(this).attr('id') + ']').text().split('*')[0] + ' obbligatorio';
                    input = this;
                    $(this).addClass('error');
                    window.scrollTo(0, 0);
                    tooltip.init({ "titolo": label, 'confirm': 'Ok', 'returntrue': function () {
                        input.focus();
                    }
                    });

                } else {
                    $(this).removeClass('error');
                }
            })

            if (continua) {
                var aliasValue = $('#alias').val();
                var dominioValue = $('#dominio').val();
                e.preventDefault();
                var objToSend = { aliasCertificato: aliasValue, dominioCertificato: dominioValue };
                $('#loading').show();

                $.post(mobileclient.context.WAPath + '/Firma/HSMRequestOTP', objToSend, function (data) {
                    if (data.HSMRequestOTPResult == true) {
                        message = 'Invio della Richiesta OTP effetuata con successo';
                    }
                    else {
                        message = 'Errore nell\'invio della Richiesta OTP';
                    }
                    tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                        $('#loading').hide();
                    }
                    });

                }, 'json');
            }
        });
        //end

        $('#closeBut').unbind('click').bind('click', function (e) {
            e.preventDefault();
            $('#firma').addClass('hide');
            if (isCades) {
                mobileclient.firmaSelezionatiLf('pades', true);
            }
            else {
                mobileclient.firmaSelezionatiLf('elettronica', false);
            }
        });

        $('#firmaBut').unbind('click').bind('click', function (e) {
            e.preventDefault();

            var continua = true;
            var input;
            $('#firma').find('input[required]').each(function () {
                if (($(this).val() == '') && continua) {
                    continua = false;
                    var label = $('#firma').find('label[for=' + $(this).attr('id') + ']').text().split('*')[0] + ' obbligatorio';
                    input = this;
                    $(this).addClass('error');
                    window.scrollTo(0, 0);
                    tooltip.init({ "titolo": label, 'confirm': 'Ok', 'returntrue': function () {
                        input.focus();
                        $('#tooltip').hide();
                    }
                    });

                } else {
                    $(this).removeClass('error');
                }
            })

            if (continua) {
                var firma = {};
                $('#firma').find('input').each(function () {
                    if ($(this).is(':radio')) {
                        if ($(this).is(':checked')) {
                            firma[$(this).attr('name')] = $(this).val();
                        }
                    }
                    else if ($(this).is(':checkbox')) {
                        firma[$(this).attr('name')] = $(this).is(':checked');
                    }

                    else {
                        firma[$(this).attr('name')] = $(this).val();
                    }
                });
                $('#loading').show();
                var objToSend = { cofirma: firma.cofirma, timestamp: firma.timestamp, tipoFirma: firma.tipoFirma, alias: firma.alias, dominio: firma.dominio, otp: firma.otp, pin: firma.pin, convert: firma.convert };

                $.post(mobileclient.context.WAPath + '/Firma/HSMMultiSign', objToSend, function (data) {
                    //console.log(data)
                    if (data.HSMSignResult == true) {
                        message = 'Documento firmato con successo';
                        isTrueFirma = true;
                        isFirmato = true;
                    }
                    else {
                        message = 'Errore durante l\'operazione di firma';
                        isTrueFirma = false;
                    }
                                        tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                                            $('#loading').hide();
                                            if (isTrueFirma) {
                                                $('#firma,#firmaView').addClass('hide');
                                                $('#box_azioni > UL > LI').removeClass('selected');
                                            }
                                        }
                                        });
                    if (isCades) {
                        mobileclient.firmaSelezionatiLf('pades', true);
                    } else {
                        mobileclient.firmaSelezionatiLf('elettronica', false);
                    }
                }, 'json');

            }
        })
    },

    controllaFirma: function () {
        if (isFirmato) {
            $('#box_azioni > UL').append('<li id="bfirma">Aggiungi firma</li>')
            $('#box_azioni > UL').append('<li id="bvedi">Vedi firme</li>')
        } else {
            $('#box_azioni > UL').append('<li id="bfirma">Firma</li>')

        }
        $('#bfirma').click(function () {
            mobileclient.popAddFirma(this)
        })
        $('#bvedi').click(function () {
            mobileclient.popViewFirma(this)
        })
    },
    popViewFirma: function (el) {
        $('.pop').remove();
        $('#info_trasm').addClass('none').hide();
        $('.control_azioni,.control_info').removeClass('premuto')
        $('#box_azioni').remove();
        $('#firma').addClass('hide');


        if ($(el).hasClass('selected')) {
            $(el).removeClass('selected');
            $('#firmaView').addClass('hide');
        } else {
            $('#box_azioni > UL > LI').removeClass('selected');
            $(el).addClass('selected');
            $('#firmaView').removeClass('hide');
            $('#firmaView').find('#detFirme').remove();
            var objToSend = { iddoc: mobileclient.model.TabModel.DocInfo.IdDoc };

            $('#firmaBtnChiudi').click(function () {
                $('#firmaView').addClass('hide');
            })

            $.post(mobileclient.context.WAPath + '/Firma/HSMSignDetail', objToSend, function (data) {


                if (data.InfoDocFirme) {

                    for (var j = 0; j < data.InfoDocFirme.Firme.length; j++) {
                        var thisFirma = data.InfoDocFirme.Firme[j];

                        $('.firmaInt', '#firmaView').append('<div id="detFirme"></div>');


                        for (var varL = 0; varL < data.InfoDocFirme.Firme[0].ChildNodiFirma[0].ChildNodiFirma.length; varL++) {

                            //console.log(varL)
                            var childFirma2 = data.InfoDocFirme.Firme[0].ChildNodiFirma[0].ChildNodiFirma[varL];
                            if (childFirma2.Text.indexOf('Firme (') != -1) {
                                /* ha altre firme */

                                for (var subFirm = 0; subFirm < childFirma2.ChildNodiFirma.length; subFirm++) {
                                    // Gabriele Melini 18-02-2015
                                    // modifica per numero firme > 2
                                    mobileclient.addLivelloFirma(childFirma2.ChildNodiFirma[subFirm], j, varL);
                                    /*
                                    detFirm = childFirma2.ChildNodiFirma[subFirm].DetailFirma;

                                    
                                    labelText_01 = (detFirm.CertificatoFirmatario == null) ? '' : detFirm.CertificatoFirmatario;
                                    labelText_02 = (detFirm.VerificaStatoFirma == null) ? '' : detFirm.VerificaStatoFirma;
                                    labelText_03 = (detFirm.VerificaStatoCertificato == null) ? '' : detFirm.VerificaStatoCertificato;
                                    labelText_04 = (detFirm.VerificaCRL == null) ? '' : detFirm.VerificaCRL;
                                    labelText_05 = (detFirm.VerificaDataValiditaDocumento == null) ? '' : detFirm.VerificaDataValiditaDocumento.substring(0, 10);
                                    labelText_06 = (detFirm.CertificatoEnte == null) ? '' : detFirm.CertificatoEnte;
                                    labelText_07 = (detFirm.CertificatoSN == null) ? '' : detFirm.CertificatoSN;
                                    labelText_08 = (detFirm.CertificatoValidoDal == null) ? '' : detFirm.CertificatoValidoDal;
                                    labelText_09 = (detFirm.CertificatoValidoAl == null) ? '' : detFirm.CertificatoValidoAl;
                                    labelText_10 = (detFirm.CertificatoAlgoritmo == null) ? '' : detFirm.CertificatoAlgoritmo;
                                    labelText_11 = (detFirm.CertificatoFirmatario == null) ? '' : detFirm.CertificatoFirmatario;
                                    labelText_12 = (detFirm.CertificatoThumbprint == null) ? '' : detFirm.CertificatoThumbprint;
                                    labelText_13 = (detFirm.SoggettoNome == null) ? '' : detFirm.SoggettoNome;
                                    labelText_14 = (detFirm.SoggettoCognome == null) ? '' : detFirm.SoggettoCognome;
                                    labelText_15 = (detFirm.SoggettoCodiceFiscale == null) ? '' : detFirm.SoggettoCodiceFiscale;
                                    labelText_16 = (detFirm.SoggettoDataNascita == null) ? '' : detFirm.SoggettoDataNascita;
                                    labelText_17 = (detFirm.SoggettoOrganizzazione == null) ? '' : detFirm.SoggettoOrganizzazione;
                                    labelText_18 = (detFirm.SoggettoRuolo == null) ? '' : detFirm.SoggettoRuolo;
                                    labelText_19 = (detFirm.SoggettoPaese == null) ? '' : detFirm.SoggettoPaese;
                                    labelText_20 = (detFirm.SoggettoIdTitolare == null) ? '' : detFirm.SoggettoIdTitolare;
                                    labelText_21 = (detFirm.InfoFirmaAlgoritmo == null) ? '' : detFirm.InfoFirmaAlgoritmo;
                                    labelText_22 = (detFirm.InfoFirmaImpronta == null) ? '' : detFirm.InfoFirmaImpronta;
                                    labelText_23 = (detFirm.InfoFirmaControfirmatario == null) ? '' : detFirm.InfoFirmaControfirmatario;

                                    var html_all = $('<div class="sezFirma sub" id="firma' + j + '_' + varL + '_' + subFirm + '">' +
                                    '<h4><span class="pointer">&#9654;</span> <span class="text">' + labelText_01 + '</span></h4>' +
                                    '<div class="textFirma acHidden">' +
                                    '<div class="tit">Risultato verifica</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Stato della firma</strong></div>' +
                                    '<div class="half">' + labelText_02 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Stato del certificato</strong></div>' +
                                    '<div class="half">' + labelText_03 + '</div>' +
                                    '</div>' +
                                    ////                                          disabilitata label Verifica CRL  
                                    ////                                        '<div class="cl">' +
                                    ////                                        '<div class="half"><strong>Verifica CRL</strong></div>' +
                                    ////                                        '<div class="half">' + labelText_04 + '</div>' +
                                    ////                                        '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Data verifica CRL</strong></div>' +
                                    '<div class="half">' + labelText_05 + '</div>' +
                                    '</div>' +
                                    '<div class="tit">Certificato</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Ente certificatore</strong></div>' +
                                    '<div class="half">' + labelText_06 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>S.N. Certificato</strong></div>' +
                                    '<div class="half">' + labelText_07 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Valido dal:</strong></div>' +
                                    '<div class="half">' + labelText_08 + '<strong> al </strong>' + labelText_09 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Algoritmo firma certificato</strong></div>' +
                                    '<div class="half">' + labelText_10 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Firmatario</strong></div>' +
                                    '<div class="half">' + labelText_11 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Thumbprint certificato</strong></div>' +
                                    '<div class="half">' + labelText_12 + '</div>' +
                                    '</div>' +
                                    '<div class="tit">Soggetto</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Nome</strong></div>' +
                                    '<div class="half">' + labelText_13 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Cognome</strong></div>' +
                                    '<div class="half">' + labelText_14 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Codice fiscale</strong></div>' +
                                    '<div class="half">' + labelText_15 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Data di nascita</strong></div>' +
                                    '<div class="half">' + labelText_16 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Organizzazione</strong></div>' +
                                    '<div class="half">' + labelText_17 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Ruolo</strong></div>' +
                                    '<div class="half">' + labelText_18 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Paese</strong></div>' +
                                    '<div class="half">' + labelText_19 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>ID titolare</strong></div>' +
                                    '<div class="half">' + labelText_20 + '</div>' +
                                    '</div>' +
                                    '<div class="tit">Firma documento</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Algoritmo di firma documento</strong></div>' +
                                    '<div class="half">' + labelText_21 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Impronta documento</strong></div>' +
                                    '<div class="half">' + labelText_22 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Controllo firmatario</strong></div>' +
                                    '<div class="half">' + labelText_23 + '</div>' +
                                    '</div>' +
                                    '</div>' +
                                    '</div>');

                                    $('#detFirme').append(html_all);
                                    */
                                }


                            } else {
                                if (childFirma2.DetailFirma) {

                                    detFirm = childFirma2.DetailFirma;

                                    labelText_01 = (detFirm.CertificatoFirmatario == null) ? '' : detFirm.CertificatoFirmatario;
                                    labelText_02 = (detFirm.VerificaStatoFirma == null) ? '' : detFirm.VerificaStatoFirma;
                                    labelText_03 = (detFirm.VerificaStatoCertificato == null) ? '' : detFirm.VerificaStatoCertificato;
                                    labelText_04 = (detFirm.VerificaCRL == null) ? '' : detFirm.VerificaCRL;
                                    labelText_05 = (detFirm.VerificaDataValiditaDocumento == null) ? '' : detFirm.VerificaDataValiditaDocumento.substring(0, 10);
                                    labelText_06 = (detFirm.CertificatoEnte == null) ? '' : detFirm.CertificatoEnte;
                                    labelText_07 = (detFirm.CertificatoSN == null) ? '' : detFirm.CertificatoSN;
                                    labelText_08 = (detFirm.CertificatoValidoDal == null) ? '' : detFirm.CertificatoValidoDal;
                                    labelText_09 = (detFirm.CertificatoValidoAl == null) ? '' : detFirm.CertificatoValidoAl;
                                    labelText_10 = (detFirm.CertificatoAlgoritmo == null) ? '' : detFirm.CertificatoAlgoritmo;
                                    labelText_11 = (detFirm.CertificatoFirmatario == null) ? '' : detFirm.CertificatoFirmatario;
                                    labelText_12 = (detFirm.CertificatoThumbprint == null) ? '' : detFirm.CertificatoThumbprint;
                                    labelText_13 = (detFirm.SoggettoNome == null) ? '' : detFirm.SoggettoNome;
                                    labelText_14 = (detFirm.SoggettoCognome == null) ? '' : detFirm.SoggettoCognome;
                                    labelText_15 = (detFirm.SoggettoCodiceFiscale == null) ? '' : detFirm.SoggettoCodiceFiscale;
                                    labelText_16 = (detFirm.SoggettoDataNascita == null) ? '' : detFirm.SoggettoDataNascita;
                                    labelText_17 = (detFirm.SoggettoOrganizzazione == null) ? '' : detFirm.SoggettoOrganizzazione;
                                    labelText_18 = (detFirm.SoggettoRuolo == null) ? '' : detFirm.SoggettoRuolo;
                                    labelText_19 = (detFirm.SoggettoPaese == null) ? '' : detFirm.SoggettoPaese;
                                    labelText_20 = (detFirm.SoggettoIdTitolare == null) ? '' : detFirm.SoggettoIdTitolare;
                                    labelText_21 = (detFirm.InfoFirmaAlgoritmo == null) ? '' : detFirm.InfoFirmaAlgoritmo;
                                    labelText_22 = (detFirm.InfoFirmaImpronta == null) ? '' : detFirm.InfoFirmaImpronta;
                                    labelText_23 = (detFirm.InfoFirmaControfirmatario == null) ? '' : detFirm.InfoFirmaControfirmatario;
                                    labelText_24 = (detFirm.InfoFirmaData == null) ? '' : detFirm.InfoFirmaData;
                                    var html_all = $('<div class="sezFirma" id="firma' + j + '_' + varL + '">' +
                                    '<h4><span class="pointer">&#9654;</span> <span class="text">' + labelText_01 + '</span></h4>' +
                                    '<div class="textFirma acHidden">' +
                                    '<div class="tit">Risultato verifica</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Stato della firma</strong></div>' +
                                    '<div class="half">' + labelText_02 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Stato del certificato</strong></div>' +
                                    '<div class="half">' + labelText_03 + '</div>' +
                                    '</div>' +
                                    /////                                  disabilitata label Verifica CRL
                                    ////                                    '<div class="cl">' +
                                    ////                                    '<div class="half"><strong>Verifica CRL</strong></div>' +
                                    ////                                    '<div class="half">' + labelText_04 + '</div>' +
                                    ////                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Data verifica CRL</strong></div>' +
                                    '<div class="half">' + labelText_05 + '</div>' +
                                    '</div>' +
                                    '<div class="tit">Certificato</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Ente certificatore</strong></div>' +
                                    '<div class="half">' + labelText_06 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>S.N. Certificato</strong></div>' +
                                    '<div class="half">' + labelText_07 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Valido dal:</strong></div>' +
                                    '<div class="half">' + labelText_08 + '<strong> al </strong>' + labelText_09 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Algoritmo firma certificato</strong></div>' +
                                    '<div class="half">' + labelText_10 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Firmatario</strong></div>' +
                                    '<div class="half">' + labelText_11 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Thumbprint certificato</strong></div>' +
                                    '<div class="half">' + labelText_12 + '</div>' +
                                    '</div>' +
                                    '<div class="tit">Soggetto</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Nome</strong></div>' +
                                    '<div class="half">' + labelText_13 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Cognome</strong></div>' +
                                    '<div class="half">' + labelText_14 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Codice fiscale</strong></div>' +
                                    '<div class="half">' + labelText_15 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Data di nascita</strong></div>' +
                                    '<div class="half">' + labelText_16 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Organizzazione</strong></div>' +
                                    '<div class="half">' + labelText_17 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Ruolo</strong></div>' +
                                    '<div class="half">' + labelText_18 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Paese</strong></div>' +
                                    '<div class="half">' + labelText_19 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>ID titolare</strong></div>' +
                                    '<div class="half">' + labelText_20 + '</div>' +
                                    '</div>' +
                                    '<div class="tit">Firma documento</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Algoritmo di firma documento</strong></div>' +
                                    '<div class="half">' + labelText_21 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Impronta documento</strong></div>' +
                                    '<div class="half">' + labelText_22 + '</div>' +
                                    '</div>' +
                                    '<div class="cl">' +
                                    '<div class="half"><strong>Controfirmatario</strong></div>' +
                                    '<div class="half">' + labelText_23 + '</div>' +
                                    '</div>' +
									'<div class="cl">' +
									'<div class="half"><strong>Data di firma</strong></div>' +
									'<div class="half">' + labelText_24 + '</div>' +
									'</div>' +
                                    '</div>' +
                                    '</div>');

                                    $('#detFirme').append(html_all);
                                }
                            }





                            //}

                        }

                        $('.sezFirma').each(function (i, el) {
                            $('h4:eq(0)', el).unbind('click').bind('click', function (e) {
                                e.preventDefault();
                                if ($(this).next(".textFirma").hasClass('acHidden')) {
                                    $(this).next(".textFirma").removeClass("acHidden");
                                    $(this).addClass('open');
                                } else {
                                    $(this).next(".textFirma").addClass("acHidden");
                                    $(this).removeClass('open');
                                }

                            });
                        });


                    }


                }

            }, 'json');


        }
    },

    addLivelloFirma: function (el, j, varL) {

        if (el.Text.indexOf('Firme (') == -1) {
            // aggiungo informazioni sulla firma

            detFirm = el.DetailFirma;

            labelText_01 = (detFirm.CertificatoFirmatario == null) ? '' : detFirm.CertificatoFirmatario;
            labelText_02 = (detFirm.VerificaStatoFirma == null) ? '' : detFirm.VerificaStatoFirma;
            labelText_03 = (detFirm.VerificaStatoCertificato == null) ? '' : detFirm.VerificaStatoCertificato;
            labelText_04 = (detFirm.VerificaCRL == null) ? '' : detFirm.VerificaCRL;
            labelText_05 = (detFirm.VerificaDataValiditaDocumento == null) ? '' : detFirm.VerificaDataValiditaDocumento.substring(0, 10);
            labelText_06 = (detFirm.CertificatoEnte == null) ? '' : detFirm.CertificatoEnte;
            labelText_07 = (detFirm.CertificatoSN == null) ? '' : detFirm.CertificatoSN;
            labelText_08 = (detFirm.CertificatoValidoDal == null) ? '' : detFirm.CertificatoValidoDal;
            labelText_09 = (detFirm.CertificatoValidoAl == null) ? '' : detFirm.CertificatoValidoAl;
            labelText_10 = (detFirm.CertificatoAlgoritmo == null) ? '' : detFirm.CertificatoAlgoritmo;
            labelText_11 = (detFirm.CertificatoFirmatario == null) ? '' : detFirm.CertificatoFirmatario;
            labelText_12 = (detFirm.CertificatoThumbprint == null) ? '' : detFirm.CertificatoThumbprint;
            labelText_13 = (detFirm.SoggettoNome == null) ? '' : detFirm.SoggettoNome;
            labelText_14 = (detFirm.SoggettoCognome == null) ? '' : detFirm.SoggettoCognome;
            labelText_15 = (detFirm.SoggettoCodiceFiscale == null) ? '' : detFirm.SoggettoCodiceFiscale;
            labelText_16 = (detFirm.SoggettoDataNascita == null) ? '' : detFirm.SoggettoDataNascita;
            labelText_17 = (detFirm.SoggettoOrganizzazione == null) ? '' : detFirm.SoggettoOrganizzazione;
            labelText_18 = (detFirm.SoggettoRuolo == null) ? '' : detFirm.SoggettoRuolo;
            labelText_19 = (detFirm.SoggettoPaese == null) ? '' : detFirm.SoggettoPaese;
            labelText_20 = (detFirm.SoggettoIdTitolare == null) ? '' : detFirm.SoggettoIdTitolare;
            labelText_21 = (detFirm.InfoFirmaAlgoritmo == null) ? '' : detFirm.InfoFirmaAlgoritmo;
            labelText_22 = (detFirm.InfoFirmaImpronta == null) ? '' : detFirm.InfoFirmaImpronta;
            labelText_23 = (detFirm.InfoFirmaControfirmatario == null) ? '' : detFirm.InfoFirmaControfirmatario;
            labelText_24 = (detFirm.InfoFirmaData == null) ? '' : detFirm.InfoFirmaData;

            var html_all = $('<div class="sezFirma sub" id="firma' + j + '_' + varL + '_' + subFirm + '">' +
                                        '<h4><span class="pointer">&#9654;</span> <span class="text">' + labelText_01 + '</span></h4>' +
                                        '<div class="textFirma acHidden">' +
                                        '<div class="tit">Risultato verifica</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Stato della firma</strong></div>' +
                                        '<div class="half">' + labelText_02 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Stato del certificato</strong></div>' +
                                        '<div class="half">' + labelText_03 + '</div>' +
                                        '</div>' +
            ////                                          disabilitata label Verifica CRL  
            ////                                        '<div class="cl">' +
            ////                                        '<div class="half"><strong>Verifica CRL</strong></div>' +
            ////                                        '<div class="half">' + labelText_04 + '</div>' +
            ////                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Data verifica CRL</strong></div>' +
                                        '<div class="half">' + labelText_05 + '</div>' +
                                        '</div>' +
                                        '<div class="tit">Certificato</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Ente certificatore</strong></div>' +
                                        '<div class="half">' + labelText_06 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>S.N. Certificato</strong></div>' +
                                        '<div class="half">' + labelText_07 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Valido dal:</strong></div>' +
                                        '<div class="half">' + labelText_08 + '<strong> al </strong>' + labelText_09 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Algoritmo firma certificato</strong></div>' +
                                        '<div class="half">' + labelText_10 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Firmatario</strong></div>' +
                                        '<div class="half">' + labelText_11 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Thumbprint certificato</strong></div>' +
                                        '<div class="half">' + labelText_12 + '</div>' +
                                        '</div>' +
                                        '<div class="tit">Soggetto</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Nome</strong></div>' +
                                        '<div class="half">' + labelText_13 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Cognome</strong></div>' +
                                        '<div class="half">' + labelText_14 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Codice fiscale</strong></div>' +
                                        '<div class="half">' + labelText_15 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Data di nascita</strong></div>' +
                                        '<div class="half">' + labelText_16 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Organizzazione</strong></div>' +
                                        '<div class="half">' + labelText_17 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Ruolo</strong></div>' +
                                        '<div class="half">' + labelText_18 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Paese</strong></div>' +
                                        '<div class="half">' + labelText_19 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>ID titolare</strong></div>' +
                                        '<div class="half">' + labelText_20 + '</div>' +
                                        '</div>' +
                                        '<div class="tit">Firma documento</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Algoritmo di firma documento</strong></div>' +
                                        '<div class="half">' + labelText_21 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Impronta documento</strong></div>' +
                                        '<div class="half">' + labelText_22 + '</div>' +
                                        '</div>' +
                                        '<div class="cl">' +
                                        '<div class="half"><strong>Controfirmatario</strong></div>' +
                                        '<div class="half">' + labelText_23 + '</div>' +
                                        '</div>' +
										'<div class="cl">' +
										'<div class="half"><strong>Data di firma</strong></div>' +
										'<div class="half">' + labelText_24 + '</div>' +
										'</div>' +
                                        '</div>' +
                                        '</div>');

            $('#detFirme').append(html_all);
        }
        else {
            // ci sono firme aggiuntiva
            for (var subFirm = 0; subFirm < el.ChildNodiFirma.length; subFirm++) {
                var child = el.ChildNodiFirma[subFirm];
                mobileclient.addLivelloFirma(child, j, varL);
            }
        }

    },


    popAccettaTrasm: function (el) {
        this.pop(el);
        $('.pop > H4').html('Inserisci note di accettazione')
        $('.pop > TEXTAREA').html('Accettata ' + $('.pop > TEXTAREA').html());
        $('.pop > BUTTON').html('Accetta')
        var adl = null;
        if (el.innerText == "Accetta e ADL")
            adl = el.innerText

        $('.pop > BUTTON').click(function () {
            mobileclient.accettaTrasm(adl);
            //#< 20140312 - nascondi il dettaglio del documento
            if (!$('#info_trasm').is(':hidden')) $('#info_trasm').hide();
            //#>
        })
    },
    popRifiutaTrasm: function (el) {
        this.pop(el);
        $('.pop > H4').html('Inserisci note di rifiuto')
        $('.pop > TEXTAREA').html('Rifiutata ' + $('.pop > TEXTAREA').html());
        $('.pop > BUTTON').html('Rifiuta')

        $('.pop > BUTTON').click(function () {
            mobileclient.rifiutaTrasm()
        })
    },
    popAccettaTrasmSmista: function (el) {
        this.pop(el);
        $('.pop > H4').html('Inserisci note di accettazione')
        $('.pop > TEXTAREA').html('Accettata ' + $('.pop > TEXTAREA').html());
        $('.pop > BUTTON').html('Accetta')

        $('.pop > BUTTON').click(function () {
            mobileclient.accettaTrasmSmista();
        })
    },
    popRifiutaTrasmSmista: function (el) {
        this.pop(el);
        $('.pop > H4').html('Inserisci note di rifiuto')
        $('.pop > TEXTAREA').html('Rifiutata ' + $('.pop > TEXTAREA').html());
        $('.pop > BUTTON').html('Rifiuta')

        $('.pop > BUTTON').click(function () {
            mobileclient.rifiutaTrasmSmista()
        })
    },
    inoltra: function () {
        this.trasmissioneForm()
    },
    fixPopTrasm: function () {
        var el = $('#box_azioni > UL > LI.selected');

        if (mobileclient.contenType == 'normal') {
            $(el).after($('.pop'));
        }
        else {
            $(el).parent().parent().append($('.pop'));
        }

    },
    pop: function (el, msg) {
        $('#firma').addClass('hide');
        $('#firmaView').addClass('hide');
        $('.pop').remove()
        $('#box_azioni > UL > LI').removeClass('selected');
        var mesi = new Array('Gennaio', 'Febbraio', 'Marzo',
            'Aprile', 'Maggio', 'Giugno', 'Luglio', 'Agosto', 'Settembre',
            'Ottobre', 'Novembre', 'Dicembre');
        var date = new Date();
        var gg = date.getDate();
        var mese = date.getMonth();
        var yy = date.getYear();
        var yyyy = (yy < 1000) ? yy + 1900 : yy;

        var mess = '';
        if (this.model.DelegaEsercitata != undefined && this.model.DelegaEsercitata != null)
            mess = 'in data ' + gg + " " + mesi[mese] + " " + yyyy + ' da ' + this.model.DelegaEsercitata.Delegato + ' (delegato da ' + this.model.DescrUtente + ') dal dispositivo mobile.';
        else
            mess = 'in data ' + gg + " " + mesi[mese] + " " + yyyy + ' da ' + this.model.DescrUtente + ' dal dispositivo mobile.';

        $(el).addClass('selected');


        if (mobileclient.contenType == 'normal') $(el).after('<div class="pop"><h4></h4><textarea>' + mess + '</textarea><button class="pulsante_classic"></button></div>');
        else $(el).parent().parent().append('<div class="pop"><h4></h4><textarea>' + mess + '</textarea><button class="pulsante_classic"></button></div>');


    },
    nuovaDelega: function () {
        //prima notifiche
        $('#nnotifiche').html(this.model.TabModel.NumElements);
        $('#nnotifiche').show();

        this.checkSession();

        //qualsiasi sia la voce accesa in servicebar, io la spengo e tolgo le funzioni ai pulsanti
        $('#service_bar').find('.on').removeClass("on");
        $('.filter_ricerca > ul > li').unbind('click');


        $('#service_bar > .filter_ricerca > ul > li').each(function (i, el) {
            $(el).click(function () {
                mobileclient.tipodelega = i;
                mobileclient.update();

            })
        });

        //e accendo quella della nuova delega
        $('#service_bar').find('.flr').addClass("on");

        //e svuoto tutto
        $('#deleghe').html('');

        //carico le informazioni attuali comprese dei ruoli delegabili
        $('#deleghe').append("<div class='info_deleghe'></div>");

        $('#deleghe > .info_deleghe').append("<div class='spalla'></div>");


        $('#deleghe > .info_deleghe > .spalla').append("<div class='imgBig'>&nbsp;</div>");
        $('#deleghe > .info_deleghe > .spalla').append("<p><strong>" + this.model.DescrUtente + "</strong><div class='onlyIphone'><strong>" + this.model.DescrRuolo + "</strong></div></p>");
        $('#deleghe > .info_deleghe > .spalla').append("<p class='noIphone'>" + this.model.DescrRuolo + "</p>");


        $('#deleghe > .info_deleghe ').append("<div class='contenuto iphoneVis'><div class='testo ruoliIphone'></div><div class='elenco_ruoli'></div></div>");
        $('#deleghe > .info_deleghe > .contenuto > .testo').append("<p class='fll'><strong>RUOLI DELEGABILI:</strong> <span>(puoi sceglierne solo uno alla volta)</span></p>");
        $('#deleghe > .info_deleghe > .contenuto > .testo').append('<div class="clear"><br class="clear"/></div>');

        //stampo tutti i ruoli

        // creo un nuovo array ruoli con aggiunta voce 'tutti'
        var ruoli = this.model.Ruoli;
        if (ruoli[0].Id == 'Tutti')
            ruoli.splice(0, 1);
        ruoli.unshift(
            { Id: 'Tutti', Descrizione: 'Tutti' }
        );

        $(ruoli).each(function (i, el) {
            //data di oggi..nel dubbio serve sempre
            var oggi = new Date();
            oggi_anno = oggi.getFullYear();
            oggi_mese = oggi.getMonth();
            oggi_mese2 = Number(oggi_mese) + 1;
            oggi_giorno = oggi.getDate();


            $('#deleghe > .info_deleghe > .contenuto > .elenco_ruoli').append($("<p class='item'>" + el.Descrizione + "</p>").click(function () {

                //tolgo l'on da tutti i radio
                $('#deleghe > .info_deleghe > .contenuto > .elenco_ruoli > .item').removeClass('on');

                //e lo metto a quello che mi interessa
                $(this).addClass('on');

                //attivo il pannelo de sotto
                $($('.info_deleghe')[1]).removeClass('off');
                $('#deleghe').find('.button_action').addClass('on');
                $('#deleghe').find('.pulsante_classic_big.off').removeClass('off');
                var dove = $('.button_action').eq(0).offset().top - $('#header').height();
                if ($.browser.webkit && (DEVICE != 'smart')) window.scrollTo(0, -dove);

                //$('#ricercadelegato').focus();

                //dopo aver acceso il pulsante per la delega gli metto anche la funzione
                $('#deleghe').find('.pulsante_classic_big').unbind('click');
                $('#deleghe').find('.pulsante_classic_big').click(function () {
                    if (!$('#ricercadelegato').attr('valore') || !$('#ricercaruolo').attr('valore') || ($("#dataInizioD").val() == '')/* || ($("#dataFineD").val() == '')*/) {
                        tooltip.init({ "titolo": "tutti i campi sono obbligatori", 'confirm': 'ok' });
                    }
                    else {
                        var dataInizio = new Date($("#dataInizioD").val());
                        var dataFine = $("#dataFineD").val().length == 0 ? '' : new Date($("#dataFineD").val());
                        var dataOggi = new Date();
                        dataOggi.setHours(0, 0, 0, 0);

                        if (dataFine.length > 0 && dataFine < dataInizio) {
                            tooltip.init({ "titolo": "Verificare intervallo date!", 'confirm': 'ok' });
                        }
                        else if (dataInizio < dataOggi) {
                            tooltip.init({ "titolo": "La data di decorrenza non deve essere inferiore alla data corrente!", 'confirm': 'ok' });
                        }
                        else {
                            mobileclient.creaDelega();
                        }
                    }

                });


                $($('.info_deleghe')[1]).find('input').removeAttr('readonly');
                $($('.info_deleghe')[1]).find('select').removeAttr('disabled');


                //stampo il ruolo di sotto
                $('#ricercaruolo').html(el.Descrizione);
                $($('.info_deleghe')[1]).find('.spalla > :nth-child(3)').html(el.Descrizione);

                //metto il valore
                $('#ricercaruolo').attr('valore', el.Id)

            }));
        });


        $('#deleghe > .info_deleghe').append('<div class="clear"><br class="clear"/></div>');

        $('#deleghe').append($('<div class="button_action">&nbsp;</div>').click(function () {
        }));


        //carico la form da compilare per una nuova delega
        //intanto piazzo la classe off, che toglierò quando clicco su un ruolo in alto

        var infodeleghe2 = $("<div class='info_deleghe off noBG'></div>");

        infodeleghe2.append("<div class='spalla noIphone'></div>");
        infodeleghe2.append("<div class='contenuto iphoneVis'></div>");
        infodeleghe2.append('<div class="clear"><br class="clear"/></div>');

        $(infodeleghe2).find('.spalla').append("<div class='imgBig'>&nbsp;</div>");
        $(infodeleghe2).find('.spalla').append("<p></p>");
        $(infodeleghe2).find('.spalla').append("<p></p>");

        var today = new Date();
        var minD = today.toJSON().slice(0, 10);
        var year = 1000 * 60 * 60 * 24 * 365 * 5;
        var maxD = new Date(today.getTime() + year).toJSON().slice(0, 10);


        $(infodeleghe2).find('.contenuto').append('<div class="form_deleghe"> \
            <table> \
            <tr><th>Ruolo Attribuito:</th><td id="ricercaruolo"></td></tr>\
            <tr><th>Nome delegato:</th><td><div class="boxricercadelegato"><input type="search" readonly="true" id="ricercadelegato" /><div class="suggestion"></div></div></td></tr>\
            <tr><th>Data decorrenza:</th><td><input placeholder="gg/mm/aaaa" type="date" id="dataInizioD" readonly="true" value="' + minD + '" min="' + minD + '" max="' + maxD + '"/></td></tr>\
            <tr><th>Data fine:</th><td><input type="date" placeholder="gg/mm/aaaa" id="dataFineD" readonly="true" min="' + minD + '" max="' + maxD + '"/></td></tr>\
            </table></div>');


        //attracco tutto
        $('#deleghe').append(infodeleghe2);


        $('#deleghe').append($('<div class="center"><input type="button" class="pulsante_classic_big center off" value="ATTIVA DELEGA" /></div>'));

        $('#deleghe').show();


        // $('#ricercadelegato').focus();
        var request = '';
        $('#ricercadelegato').keyup(function () {
            var desc = $(this).attr('value');
            if (desc.length > 3) {

                if (request) request.abort();

                $('.suggestion').show();

                $('.suggestion').html('');
                $('.suggestion').addClass('loading');

                request = $.post(mobileclient.context.WAPath + '/Delega/RicercaUtenti', { "descrizione": desc }, function (data) {

                    //creo l'elenco delle suggestion

                    $('.suggestion').append('<ul></ul>');

                    $(data).each(function (i, el) {
                        //popolo il sugg
                        $('.suggestion').show();
                        if (i < 3) {
                            $('.suggestion > ul').append($('<li>' + el.Descrizione + '</li>').click(function () {
                                //attacco la funzione al click del suggestion element
                                $('#ricercadelegato').attr('value', el.Descrizione);
                                $('#ricercadelegato').attr('valore', el.IdPeople);

                                $('.suggestion').hide();


                                //aggiungo il nome scelto anche nella spalla
                                $($('.info_deleghe')[1]).find('.spalla > :nth-child(2)').html('<strong>' + el.Descrizione.toLowerCase() + '</strong>');
                            }));
                        }

                    });

                    $('.suggestion').removeClass('loading');

                    if (data == '') {
                        $('.suggestion > ul').append($('<li>Nessun risultato</li>'));
                    }
                });
            }
            else {
                $('.suggestion').hide();
            }


        })

    },
    updateDeleghe: function () {
        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        var delega = $('<div class="pulsante_classic flr paddingRight"><a href="javascript:;">Delega</a></div>').click(function () {
            mobileclient.nuovaDelega();
        });
        $('#service_bar').append(delega);

        $('#service_bar').removeClass('hideService');

        //info utente
        $('#deleghe').append("<div class='info_deleghe'></div>");

        if (!this.model.DelegaEsercitata) {

            //Mev Deleghe impostate - aggiunto pulsante deleghe impostate
            $('#service_bar').append('<div class="filter_ricerca deleghe"><ul><li class="noIphone">Tutte</li><li class="firstIphone paddingLeft">Ricevute</li><li>Impostate</li><li>Assegnate</li></ul></div>');


            $('#service_bar > .filter_ricerca > ul > li').each(function (i, el) {
                $(el).click(function () {

                    if (i == 0) {
                        //spengo il LI acceso e accendo quello cliccato
                        $('#service_bar > .filter_ricerca > ul > li.on').removeClass("on");
                        $(this).addClass("on");

                        //mostro tutte le tabelle
                        $(".table_deleghe").show();
                    }
                    else {
                        //recupero l'id da aprire
                        var div = "D" + $(this).html().toLowerCase();
                        //console.log(div);
                        //spengo il LI acceso e accendo quello cliccato
                        $('#service_bar > .filter_ricerca > ul > li.on').removeClass("on");
                        $(this).addClass("on");


                        //nascondo tutte le tabelle e mostro quella cliccata
                        $(".table_deleghe").hide();
                        //console.log($(".table_deleghe."+ div));
                        $(".table_deleghe." + div).show();
                    }
                });
            });


            $('#deleghe > .info_deleghe').append("<div class='spalla'></div>");

            $('#deleghe > .info_deleghe > .spalla').append("<div class='img'>&nbsp;</div>");
            $('#deleghe > .info_deleghe > .spalla').append("<p><strong>" + this.model.DescrUtente + "</strong></p>");
            $('#deleghe > .info_deleghe > .spalla').append("<p>" + this.model.DescrRuolo + "</p>");

            $('#deleghe > .info_deleghe').append("<div class='contenuto'><div class='testo'></div></div>");
            $('#deleghe > .info_deleghe > .contenuto > .testo').append("<p><strong>DELEGA ATTIVA:</strong> Nessuna delega attiva</p>");
            $('#deleghe > .info_deleghe > .contenuto > .testo').append("<p><strong>RICORDA:</strong><br>Puoi svolgere solamente una delega ricevuta alla volta. <br>Selezionala utilizzando i bottoni rossi che trovi a fianco del titolo della delega.");
            $('#deleghe > .info_deleghe').append("<div class='clear'><br class='clear'/></div>");

            //se non esercito una delega
            $('#deleghe').append("<div class='table_deleghe Dricevute'><table><tr class='noIphone'><th>&nbsp;</th><th colspan=\"2\">Deleghe ricevute</th><th>Decorrenza</th><th>Scadenza</th><th>Delegante</th><th>&nbsp;</th></tr></table></div>");

            var deleghe_ricevute = this.model.TabModel.DelegheRicevute;
            var deleghe_assegnate = this.model.TabModel.DelegheAssegnate;
            //Mev Deleghe impostate
            var deleghe_impostate = this.model.TabModel.DelegheImpostate;
            //end

            $(deleghe_ricevute).each(function (i, el) {
                var ruolo = el.RuoloDelegato;
                var decorrenza = mobileclient.formatDate(el.DataDecorrenza);
                var scadenza = mobileclient.formatDate(el.DataScadenza);
                var delegante = el.Delegante;

                $('#deleghe > .Dricevute > table').append('<tr><td class="noIphone">&nbsp;</td><td class="colspan7"><div class="inputradio" id="' + el.Id + '" value="' + el.Id + '|' + el.IdRuoloDelegante + '|' + el.CodiceDelegante + '"><label class="onlyIphone" for=""' + el.Id + '""><span class="codice">' + ruolo + '</span><span class="delegante">' + delegante + '</span></label></div></td><td class="noIphone">' + ruolo + '</td><td class="noIphone">' + decorrenza + '</td><td class="noIphone">' + scadenza + '</td><td class="noIphone">' + delegante + '</td><td>&nbsp;</td></tr>');

                $('[value=' + el.Id + '|' + el.IdRuoloDelegante + '|' + el.CodiceDelegante + ']').click(function () {
                    $('#deleghe > .Dricevute > table').find('div').removeClass('checked');
                    $(this).addClass('checked');
                });


            });
            $('#deleghe > .Dricevute').append($('<div class="center"><input type="button" class="pulsante_classic_big center" value="ATTIVA DELEGA" /></div>').click(function () {

                if ($('#deleghe > .Dricevute > table').find("div.checked").size() > 0) {
                    var Id = $('#deleghe > .Dricevute > table').find("div.checked").attr('value').split('|')[0];
                    var IdRuoloDelegante = $('#deleghe > .Dricevute > table').find("div.checked").attr('value').split('|')[1];
                    var CodiceDelegante = $('#deleghe > .Dricevute > table').find("div.checked").attr('value').split('|')[2];

                    mobileclient.accettaDelega(Id, IdRuoloDelegante, CodiceDelegante);
                }
            }))


            //Mev Deleghe impostate
            $('#deleghe').append("<div class='table_deleghe Dimpostate'><table><tr class='noIphone'><th>&nbsp;</th><th colspan=\"2\">Deleghe impostate</th><th>Decorrenza</th><th>Scadenza</th><th>Delegato</th><th>&nbsp;</th></tr></table></div>");
            $(deleghe_impostate).each(function (i, el) {
                var ruolo = el.RuoloDelegato;
                var decorrenza = mobileclient.formatDate(el.DataDecorrenza);
                var scadenza = mobileclient.formatDate(el.DataScadenza);
                var delegato = el.Delegato;

                $('#deleghe > .Dimpostate > table').append('<tr><td class="noIphone">&nbsp;</td><td class="colspan7"><div class="inputcheckbox" id="' + el.Id + '" value="' + el.Id + '|' + el.DataDecorrenza + '|' + el.DataScadenza + '"  ><label class="onlyIphone" for=""' + el.Id + '""><span class="codice">' + ruolo + '</span><span class="delegante">' + delegato + '</span></label></div></td><td class="noIphone">' + ruolo + '</td><td class="noIphone">' + decorrenza + '</td><td class="noIphone">' + scadenza + '</td><td class="noIphone">' + delegato + '</td><td class="noIphone">&nbsp;</td></tr>');

                $('[value=' + el.Id + '|' + el.DataDecorrenza + '|' + el.DataScadenza + ']').click(function () {

                    $(this).toggleClass('checked');
                });
            });
            if (deleghe_impostate.length == 0) $('#deleghe > .Dimpostate').remove();
            //end

            $('#deleghe').append("<div class='table_deleghe Dassegnate'><table><tr class='noIphone'><th>&nbsp;</th><th colspan=\"2\">Deleghe assegnate</th><th>Decorrenza</th><th>Scadenza</th><th>Delegato</th><th>&nbsp;</th></tr></table></div>");
            $(deleghe_assegnate).each(function (i, el) {
                var ruolo = el.RuoloDelegato;
                var decorrenza = mobileclient.formatDate(el.DataDecorrenza);
                var scadenza = mobileclient.formatDate(el.DataScadenza);
                var delegato = el.Delegato;

                $('#deleghe > .Dassegnate > table').append('<tr><td class="noIphone">&nbsp;</td><td class="colspan7"><div class="inputcheckbox" id="' + el.Id + '" value="' + el.Id + '|' + el.DataDecorrenza + '|' + el.DataScadenza + '"  ><label class="onlyIphone" for=""' + el.Id + '""><span class="codice">' + ruolo + '</span><span class="delegante">' + delegato + '</span></label></div></td><td class="noIphone">' + ruolo + '</td><td class="noIphone">' + decorrenza + '</td><td class="noIphone">' + scadenza + '</td><td class="noIphone">' + delegato + '</td><td class="noIphone">&nbsp;</td></tr>');

                $('[value=' + el.Id + '|' + el.DataDecorrenza + '|' + el.DataScadenza + ']').click(function () {

                    $(this).toggleClass('checked');
                });
            });
            //Mev Deleghe impostate
            if (deleghe_assegnate.length == 0) $('#deleghe > .Dassegnate').remove();
            //end
            mobileclient.iPhoneGest();

            $('#deleghe > .Dassegnate').append($('<div class="center"><input type="button" class="pulsante_classic_big revoca" value="REVOCA DELEGA" /></div>').click(function () {
                if ($('#deleghe > .Dassegnate > table').find("div.checked").size() > 0) {
                    var deleghe = new Array();

                    $('#deleghe > .Dassegnate > table').find("div.checked").each(function (i, el) {

                        var Id = $(el).attr('value').split('|')[0];
                        var DataDecorrenza = eval($(el).attr('value').split('|')[1].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                        var DataScadenza = new Date();
                        if ($(el).attr('value').split('|')[2].replace('/Date(', '').substring(0, 1) != '-') {
                            DataScadenza = eval($(el).attr('value').split('|')[2].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                        }
                        else {
                            DataScadenza.setDate(DataDecorrenza.getDate() - 1);
                        }
                        var delega = { "Id": Id, "DataDecorrenza": DataDecorrenza.toUTCString(), "DataScadenza": DataScadenza.toUTCString() };
                        deleghe.push(delega);
                    });

                    mobileclient.RevocaDeleghe(deleghe);
                }
            }))


            //Mev Deleghe impostate - aggiunto pulsante revoca delega
            $('#deleghe > .Dimpostate').append($('<div class="center"><input type="button" class="pulsante_classic_big revoca" value="REVOCA DELEGA" /></div>').click(function () {
                if ($('#deleghe > .Dimpostate > table').find("div.checked").size() > 0) {
                    var deleghe = new Array();

                    $('#deleghe > .Dimpostate > table').find("div.checked").each(function (i, el) {

                        var Id = $(el).attr('value').split('|')[0];
                        var DataDecorrenza = eval($(el).attr('value').split('|')[1].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                        var DataScadenza = new Date();
                        if ($(el).attr('value').split('|')[2].replace('/Date(', '').substring(0, 1) != '-') {
                            DataScadenza = eval($(el).attr('value').split('|')[2].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                        }
                        else {
                            DataScadenza.setDate(DataDecorrenza.getDate() - 1);
                        }
                        var delega = { "Id": Id, "DataDecorrenza": DataDecorrenza.toUTCString(), "DataScadenza": DataScadenza.toUTCString() };
                        deleghe.push(delega);
                    });

                    mobileclient.RevocaDeleghe(deleghe);
                }
            }))


            //accendo il tab scelto
            if (!mobileclient.tipodelega) mobileclient.tipodelega = 0;

            if ((!mobileclient.tipodelega) && (DEVICE == 'smart')) mobileclient.tipodelega = 1;

            $($('#service_bar > .filter_ricerca > ul > li')[mobileclient.tipodelega]).trigger('click');


        }
        else {
            //se sto esercitando una delega

            //prima metto il bottoncino della delega
            var attiva = $('<div class="pulsante_classic fll on paddingLeft"><a href="javascript:;">Attiva</a></div>').click(function () {
                mobileclient.update();

            });
            $('#service_bar').append(attiva);

            //poi man mano tutte le info
            $('#deleghe > .info_deleghe').append("<div class='spalla'></div>");

            $('#deleghe > .info_deleghe > .spalla').append("<div class='imgBig'>&nbsp;</div>");
            $('#deleghe > .info_deleghe > .spalla').append("<p><strong>" + this.model.DelegaEsercitata.Delegato + "</strong><div class='onlyIphone'>Ruolo delegato:<strong>" + this.model.DelegaEsercitata.RuoloDelegato + "</strong></div></p>");

            $('#deleghe > .info_deleghe ').append("<div class='contenuto'><div class='testo'></div><div class='info_ruolo'></div></div>");


            $('#deleghe').append('<div class="tableDelega onlyIphone"><table><tr><th>Nome delegante:</th><td>' + this.model.DelegaEsercitata.Delegante + '</td></tr><tr><th>Decorrenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataDecorrenza) + '</td></tr><tr><th>Scadenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataScadenza) + '</td></tr></table></div>');

            $('#deleghe .tableDelega').append('<div class="pulsante_classic_big">Dismetti Delega</div>');

            $('#deleghe > .info_deleghe  > .contenuto > .testo').append("<p class='fll'><strong>Delega attiva:</strong> " + this.model.DelegaEsercitata.RuoloDelegato + " </p><div class=\"pulsante_classic flr\">DISMETTI</div>");
            $('#deleghe > .info_deleghe  > .contenuto > .testo').append('<div class="clear"><br class="clear"/></div>');

            $('#deleghe > .info_deleghe  > .contenuto > .info_ruolo').append('<table><tr><th>Data di decorrenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataDecorrenza) + '</td></tr><tr><th>Data di scadenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataScadenza) + '</td></tr><tr><th>Delegante:</th><td>' + this.model.DelegaEsercitata.Delegante + '</td></tr></table>');

            $('#deleghe > .info_deleghe').append("<div class='clear'><br class='clear'/></div>");


            $('#deleghe > .info_deleghe > .contenuto').find('.pulsante_classic').click(function () {
                tooltip.init({ "titolo": 'Stai per Dismettere una delega. Sei sicuro?', "content": '', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () {
                    mobileclient.dismettiDelega(mobileclient.model.DelegaEsercitata.CodiceDelegante)
                }
                })
            })


            $('#deleghe > .tableDelega ').find('.pulsante_classic_big').click(function () {
                tooltip.init({ "titolo": 'Stai per Dismettere una delega. Sei sicuro?', "content": '', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () {
                    mobileclient.dismettiDelega(mobileclient.model.DelegaEsercitata.CodiceDelegante)
                }
                })
            })


        }
        $('#deleghe').show();
    },
    updateSmistamento: function () {
        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        $('#wrapper').addClass('solid');
        if (!this.documento_attuale) this.documento_attuale = 0;
        //prima la service bar
        //azioni
        var control_azioni = $('<div class="control_pulsante control_azioni">&nbsp;</div>');
        control_azioni.click(function () {
            mobileclient.control_azioni();
        });
        /*
        control_azioni.click(function () {
        $('.control_azioni').toggleClass('premuto');

        if ($('.control_azioni').hasClass('premuto')) {
        $('#service_bar').append($('<div id="box_azioni"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Azioni</h3><ul><li>Zoom</li><li>Scarica</li><li class="nopadd">Agg. Nota Generale</li></ul></div>'));


        $($('#box_azioni > UL > LI')[0]).click(function () {
        mobileclient.zoomDocument(mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id)
        });
        $($('#box_azioni > UL > LI')[1]).click(function () {
        window.open(mobileclient.context.WAPath + '/Documento/File/' + mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id)
        })
        $($('#box_azioni > UL > LI')[2]).click(function () {
        mobileclient.smistamento_addNotaGen(this)
        });


        $('#box_azioni').find('.annulla_button').click(function () {
        $('.control_azioni').toggleClass('premuto');
        $('#box_azioni').remove();
        });
        }
        else {
        $('#box_azioni').remove();
        }


        });
        */
        $('#service_bar').append(control_azioni);
        //numero elemento
        var paginatore = '<div class="smistatore_paginatore"><img class="ar_left" src="' + this.context.WAPath + '/content/' + this.context.SkinFolder + '/img/' + DEVICE + '/smistatore_paginatore_arrow.png" /><p></p><img src="' + this.context.WAPath + '/content/' + this.context.SkinFolder + '/img/' + DEVICE + '/smistatore_paginatore_arrow.png" class="ar_right" /> </div>';

        $('#service_bar').append(paginatore);


        $('.smistatore_paginatore').find('.ar_left').click(function () {
            if (mobileclient.documento_attuale != 0) {
                mobileclient.documento_attuale--;
                mobileclient.smistamento_loaddoc();
            }
            else if (mobileclient.model.TabModel.CurrentPage != 1) {
                mobileclient.documento_attuale = mobileclient.model.TabModel.NumResultsForPage - 1;
                mobileclient.smistamentoForm(mobileclient.model.TabModel.CurrentPage - 1)
            }
            $('.control_azioni').removeClass('premuto')
            $('#box_azioni').remove();
        });
        $('.smistatore_paginatore').find('.ar_right').click(function () {

            if (mobileclient.documento_attuale != mobileclient.model.TabModel.SmistamentoElements.length - 1) {
                mobileclient.documento_attuale++;
                mobileclient.smistamento_loaddoc();
            }
            else if (mobileclient.model.TabModel.CurrentPage != mobileclient.model.TabModel.NumTotPages) {
                mobileclient.documento_attuale = 0;
                mobileclient.smistamentoForm(mobileclient.model.TabModel.CurrentPage + 1)
            }
            $('.control_azioni').removeClass('premuto')
            $('#box_azioni').remove();

        });


        //frecce paginatore
        $('#service_bar').removeClass('hideService');

        //parte superiore
        $('#smista_doc').append('<div id="lista_documenti"></div>');
        this.smistamento_loaddoc();

        elSm = document.getElementById('lista_documenti');
        var hammertimesm = Hammer(elSm, { prevent_default: false, transform_always_block: true }).on("pinch", function (event) {
            swipeBool = false;

            setTimeout(function () {
                swipeBool = true;
            }, 300);
        });


        var hammertimesmr = Hammer(elSm, { prevent_default: false, transform_always_block: true }).on("swiperight", function (event) {
            if (swipeBool) {
                $('.smistatore_paginatore').find('.ar_left').click()

                swipeBool = false;
            }
            setTimeout(function () {
                swipeBool = true;
            }, 300);
        });

        var hammertimesml = Hammer(elSm, { prevent_default: true, transform_always_block: true }).on("swipeleft", function (event) {
            if (swipeBool) {
                $('.smistatore_paginatore').find('.ar_right').click()
                swipeBool = false;
            }
            setTimeout(function () {
                swipeBool = true;
            }, 300);
        });

        //parte inferiore
        // MEV SMISTAMENTO
        // aggiunta radio con ricerca per competenza/conoscenza
        //$('#smista_doc').append('<div id="smista_form"><div class="filtri"><select name="modello_trasm"><option value="none">Seleziona modello di tramissione</option></select> <input type="search" id="smistamento_ricerca" /><div class="suggestion"></div></div></div>');
        $('#smista_doc').append('<div id="smista_form"><div class="filtri"><select name="modello_trasm"><option value="none">Seleziona modello di trasmissione</option></select><input type="search" id="smistamento_ricerca" /><div class="suggestion"></div><input type="radio" id="smistamento_ragione" name="smistamento_ragione" value="con" title="Ricerca per conoscenza"><label for="smistamento_ragione">CON.</label></input><input type="radio" id="smistamento_ragione" name="smistamento_ragione" value="comp" title="Ricerca per competenza" checked><label for="smistamento_ragione">COMP.</label></input></div></div>');

        //popolo la select con i modelli di trasmissione
        $(this.model.TabModel.ModelliTrasm).each(function (i, el) {
            $('#smista_doc').find('select').append('<option value="' + el.Id + '">' + el.Codice + '</option>');
        });

        $('#smista_form').append('<div id="smista_ricerca"></div>');
        $('#smista_form').append('<div id="smista_main"></div>')

        //attacco la funzione all'input di ricerca
        this.smistamento_ricerca();

        //metto la parte della ricerca

        //$('#smista_form').append('<div class="label_tabella none"><p>La tua Ricerca</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');
        $('#smista_ricerca').append('<div class="label_tabella none"><p>La tua Ricerca</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');

        //$('#smista_form').append('<ul class="items_ricerca"></ul>');
        $('#smista_ricerca').append('<ul class="items_ricerca"></ul>');


        //$('#smista_form').append('<div class="label_tabella"><p>' + this.model.TabModel.Tree.UOAppartenenza.Descrizione + '</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');
        $('#smista_main').append('<div class="label_tabella"><p>' + this.model.TabModel.Tree.UOAppartenenza.Descrizione + '</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');

        //$('#smista_form').append('<ul class="items_elenco"><li class="uoa" val="' + this.model.TabModel.Tree.UOAppartenenza.Id + '"><div class="item"><p>' + this.model.TabModel.Tree.UOAppartenenza.Descrizione + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li></ul>');
        $('#smista_main').append('<ul class="items_elenco"><li class="uoa" val="' + this.model.TabModel.Tree.UOAppartenenza.Id + '"><div class="item"><p>' + this.model.TabModel.Tree.UOAppartenenza.Descrizione + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li></ul>');

        // ruoli collassati/espansi
        var collapseRuoli = this.model.TabModel.CollapseRuoli;
        $('#smista_form').data('collapseRuoli', collapseRuoli);


        if (this.model.TabModel.Tree.UOAppartenenza.Ruoli) {
            //$('#smista_form > ul.items_elenco > li').append('<ul></ul>');
            $('#smista_main > ul.items_elenco > li').append('<ul></ul>');

            $(this.model.TabModel.Tree.UOAppartenenza.Ruoli).each(function (i, el) {
                li = $('<li class="ruolo" val="' + el.Id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden img_nota_ruolo" name="img_nota_ruolo"/><div id="expand"><p>' + el.Descrizione + '</p></div><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');

                if (el.Utenti) {
                    ul = $('<div id="container_utenti"><ul></ul></div>');

                    $(el.Utenti).each(function (a, b) {
                        var li = $('<li class="persona" val="' + b.Id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" /><p>' + b.Descrizione + '</p> <div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                        ul.append(li);

                        li.find('img').click(function () {
                            mobileclient.smistamento_addnota(this);
                        })
                    });

                }
                li.append(ul);
                if (collapseRuoli == true) {
                    li.find('#container_utenti').hide();
                }
                li.find('#expand').click(function (e) {
                    //alert("ok");
                    $(this).parent().parent().find('#container_utenti').slideToggle("fast");
                    //alert("done");
                    //$("#container_utenti").hide();

                });

                li.find('img').click(function () {
                    mobileclient.smistamento_addnota(this);
                })
                //$('#smista_form > ul.items_elenco > li > ul').append(li);
                $('#smista_main > ul.items_elenco > li > ul').append(li);
            });
        }

        //MEV SMISTAMENTO - aggiunto pannello di visualizzazione uo di livello inferiore

        if (this.model.TabModel.Tree.AltreUO) {
            if (this.model.TabModel.Tree.AltreUO.length > 0) {
                //$('#smista_form').append('<div class="label_tabella"><p>Unità organizzative di livello inferiore</p><br clear="all" class="clear"></div>');
                $('#smista_main').append('<div class="label_tabella"><p>Unità organizzative di livello inferiore</p><br clear="all" class="clear"></div>');
                //$('#smista_form').append('<ul class="items_elencoUO"></ul>');
                $('#smista_main').append('<ul class="items_elencoUO"></ul>');
                $(this.model.TabModel.Tree.AltreUO).each(function (i, el) {
                    var li = $('<li class="uoa" val="' + el.Id + '"><div class="item"><img id="imgAddNota" src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden img_nota_ruolo" name="img_nota_ruolo"/><p>'
                        + el.Descrizione + '</p>'
                        + '<img id="imgSelectUpUO" src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistamento_down_arrow.png" title="Naviga UO Inferiore" />'
                        + '<div class="inputcheckbox" name="conoscenza">&nbsp;</div>'
                        + '<div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea>'
                        + '<div class="clear"><br class="clear"/></div></div></li>');
                    li.find('#imgAddNota').hide();
                    li.find('#imgAddNota').click(function () { mobileclient.smistamento_addnota(this); });
                    li.find('#imgSelectUpUO').click(function () {
                        var id_uo = el.Id;
                        page.loading(true);
                        mobileclient.smistamento_navigaUOinf(id_uo);
                    });
                    //$('#smista_form > .items_elencoUO').append(li);
                    $('#smista_main > .items_elencoUO').append(li);
                });
            }
        }

        $('#smista_form').find('.inputcheckbox').each(function (i, e) {
            $(e).bind('click', function () {
                mobileclient.smistamento_selectoptions(this);
            });
        });

        /*
        var visto = $('<div class="pulsante_classic_big">VISTO</div>')
        var accetta = $('<div class="pulsante_classic_big">ACCETTA</div>').bind('click touchend', function () {
        mobileclient.popAccettaTrasmSmista(this);
        });

        var rifiuta = $('<div class="pulsante_classic_big">RIFIUTA</div>').bind('click touchend', function () {
        mobileclient.popRifiutaTrasmSmista(this);
        });
        */
        var smista = $('<div class="pulsante_classic_big">SMISTA</div>').bind('click touchend', function () {
            if ($('#smista_form').find('.inputcheckbox').hasClass('checked') || $('[name="modello_trasm"]').val() != 'none') {
                mobileclient.eseguiSmistamento();
            }
            else {
                tooltip.init({ "titolo": "&Egrave; necessario selezionare almeno un'opzione", 'confirm': 'ok' });
            }
        });

        $('#footer').append('<div id="barra_smistamento"></div>');

        $('#barra_smistamento').append(smista);

        $('#smista_doc').show();

    },
    smistamento_addNotaGen: function (el) {

        this.pop(el);
        $('.pop > H4').html('Inserisci Nota')
        $('.pop > TEXTAREA').html($('textarea[name="note_generali"]').html());
        $('.pop > BUTTON').html('Aggiungi')

        $('.pop > BUTTON').click(function () {

            $('textarea[name="note_generali"]').html($('.pop > TEXTAREA').val());
            $('#box_azioni').remove();
            $('.control_azioni').toggleClass('premuto');

        })

    },
    smistamento_addnota: function (img) {
        $('.addnote').remove();
        var addnote = $('<div class="addnote"><div class="hdr"><button class="annulla_button">Annulla</button><span></span></div></p><textarea></textarea><div class="pulsante_classic flr">a</div></div>');

        //se premo annulla chiudo
        addnote.find('.annulla_button').click(function () { $('.addnote').remove() });

        //verifico se ho già del testo
        if ($(img).parent().find('textarea').html() != '') {
            addnote.find('span').html('Modifica nota trasmissione');
            addnote.find('textarea').html($(img).parent().find('textarea').val());
            addnote.find('.pulsante_classic').html('Aggiorna');
        }
        else {
            addnote.find('span').html('Aggiungi nota trasmissione');
            addnote.find('.pulsante_classic').html('Aggiungi');
        }

        addnote.find('.pulsante_classic').click(function () {
            $(img).parent().find('textarea').html(addnote.find('textarea').val());
            $('.addnote').remove();

            //se ho inserito un valore metto l'icona di nota aggiunta altrimenti rimetto l'icona di nota non aggiunta
            if (addnote.find('textarea').val() != '') $(img).attr('src', $(img).attr('src').replace('_add', '_do'));
            else $(img).attr('src', $(img).attr('src').replace('_do', '_add'));
        });


        $(img).parent().append(addnote);
    },
    smistamento_ricerca: function () {
        var request;
        $('#smistamento_ricerca').keyup(function () {
            var desc = $(this).attr('value');
            var rag = $("input:radio[name='smistamento_ragione']:checked").val();

            if (desc.length > 3) {

                if (request) request.abort();

                $('.suggestion').show();

                $('.suggestion').html('');
                $('.suggestion').addClass('loading');

                request = $.post(mobileclient.context.WAPath + '/Smistamento/RicercaElementi', { "descrizione": desc, "ragione": rag }, function (data) {

                    //creo l'elenco delle suggestion

                    $('.suggestion').append('<ul></ul>');
                    $('.suggestion').show();


                    $('.suggestion > ul').append($('<li class="label">Utenti</li>'));
                    $('.suggestion > ul').append($('<li class="label">Ruoli</li>'));
                    $('.suggestion > ul').append($('<li class="label">Unità organizzative</li>'));

                    $(data).each(function (i, el) {
                        //popolo il sugg
                        $.expr[":"].econtains = function (obj, index, meta, stack) {
                            return (obj.textContent || obj.innerText || $(obj).text() || "").toLowerCase() == meta[3].toLowerCase();
                        }

                        var lista_id = el.IdUtente + '-' + el.IdRuolo + '-' + el.IdUO;

                        if (el.Type.UTENTE) {
                            if ($('.suggestion > ul > li:econtains(\'' + el.DescrizioneUtente + '\')').size() == 0) {
                                if (!el.DescrizioneRuolo) {
                                    $($('.label')[0]).after($('<li>' + el.DescrizioneUtente + '</li>').click(function () {
                                        mobileclient.smistamento_additem('persona', lista_id, el.DescrizioneUtente);
                                    }));
                                }
                            }

                        }


                        if (el.Type.RUOLO) {

                            if ($('.suggestion > ul > li:econtains(\'' + el.DescrizioneRuolo + '\')').size() == 0) {
                                $($('.label')[1]).after($('<li>' + el.DescrizioneRuolo + '</li>').click(function () {
                                    mobileclient.smistamento_additem('ruolo', lista_id, el.DescrizioneRuolo);
                                }));
                            }
                        }


                        if (el.Type.UO) {

                            if ($('.suggestion > ul > li:econtains(\'' + el.DescrizioneUO + '\')').size() == 0) {
                                $($('.label')[2]).after($('<li>' + el.DescrizioneUO + '</li>').click(function () {
                                    mobileclient.smistamento_additem('uoa', lista_id, el.DescrizioneUO);
                                }));
                            }
                        }

                    });
                    if ($($('.label')[0]).next().hasClass('label')) {
                        $($('.label')[0]).after($('<li class="nores">Nessun risultato</li>'));
                    }
                    if ($($('.label')[1]).next().hasClass('label')) {
                        $($('.label')[1]).after($('<li class="nores">Nessun risultato</li>'));
                    }


                    if ($($('.label')[2]).is(':last-child')) {
                        $($('.label')[2]).after($('<li class="nores">Nessun risultato</li>'));
                    }


                    $('.suggestion').removeClass('loading');

                });
            }
            else {
                $('.suggestion').hide();
            }

        })
    },
    smistamento_additem: function (type, id, value) {
        $('.label_tabella').show();
        //var elenco = $('#smista_form > UL.items_ricerca');
        var elenco = $('#smista_ricerca > UL.items_ricerca');
        var rag = $("input:radio[name='smistamento_ragione']:checked").val();
        // MEV SMISTAMENTO
        // attivo la checkbox corrispondente alla ricerca effettuata (competenza/conoscenza)
        var isRicCompetenza = false;
        if (rag == "comp") isRicCompetenza = true;

        if (elenco.find('[val="' + id + '"]').size() == 0) {
            if (type == "persona") {
                if (isRicCompetenza)
                    var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" class="img_hidden" /><p>' + value + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox checked" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                else
                    var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" class="img_hidden" /><p>' + value + '</p><div class="inputcheckbox checked" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                li.find('img').click(function () { mobileclient.smistamento_addnota(this); })
            }
            else if (type == "ruolo") {
                var idRuolo = id.split('-')[1];
                if (isRicCompetenza)
                    var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" name="img_nota_ruolo" class="img_nota_ruolo" /><p>' + value + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox checked" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                else
                    var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" name="img_nota_ruolo" class="img_nota_ruolo" /><p>' + value + '</p><div class="inputcheckbox checked" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                li.find('img').click(function () { mobileclient.smistamento_addnota(this); })
                // MEV SMISTAMENTO
                // se aggiungo un ruolo devo aggiungere anche tutti gli utenti appartenenti ad esso
                var ul_ruoli = $('<ul></ul>');
                var request = $.post(mobileclient.context.WAPath + '/Smistamento/RicercaUtentiInRuolo', { "descrizione": idRuolo }, function (data) {
                    $(data).each(function (i, el) {
                        var lista_id = el.IdUtente + '-' + el.IdRuolo + '-' + el.IdUO;
                        if (isRicCompetenza)
                            var li_utente = $('<li class="persona" val="' + lista_id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" class="img_hidden" /><p>' + el.DescrizioneUtente + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox checked" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                        else
                            var li_utente = $('<li class="persona" val="' + lista_id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" class="img_hidden" /><p>' + el.DescrizioneUtente + '</p><div class="inputcheckbox checked" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                        li_utente.find('img').click(function () { mobileclient.smistamento_addnota(this); })
                        li_utente.find('.inputcheckbox').click(function () { mobileclient.smistamento_selectoptions(this); });
                        ul_ruoli.append(li_utente);
                    });
                });
                li.append(ul_ruoli);
            }
            else {
                if (isRicCompetenza)
                    var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><p>' + value + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox checked" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li>');
                else
                    var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><p>' + value + '</p><div class="inputcheckbox checked" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li>');
            }

            li.find('.inputcheckbox').click(function () { mobileclient.smistamento_selectoptions(this); });


            elenco.append(li);
            $('.suggestion').hide();
        }
        else {
            tooltip.init({ "titolo": "Voce già inserita", 'confirm': 'ok' });
        }

    },

    smistamento_navigaUOinf: function (idUO) {

        var request;
        // memorizzo gli id selezionati per il livello corrente
        var lista_com = $('#smista_form').data('lista_com');
        var lista_cc = $('#smista_form').data('lista_cc');
        var lista_note = $('#smista_form').data('lista_note');
        if (undefined == lista_com) lista_com = [];
        if (undefined == lista_cc) lista_cc = [];
        if (undefined == lista_note) lista_note = [];
        //alert(lista_com);
        //alert(lista_cc);
        //var NoteIndividuali = $(this).find('TEXTAREA').html();

        var collapseRuoli = $('#smista_form').data('collapseRuoli');

        $('#smista_main').find('li').each(function (i) {
            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            var id = $(this).attr('val');
            var tipo = $(this).attr('class');
            if (tipo == "persona") id = id + "_" + $(this).parent().parent().attr('val') + "_" + $(this).parent().parent().parent().parent().attr('val');
            if (tipo == "ruolo") id = id + "_" + $(this).parent().parent().attr('val');
            //alert(tipo + " - " + id);

            if (competenza) {

                if (!(lista_com.indexOf(id) > -1)) lista_com.push(id);
                lista_cc = $.grep(lista_cc, function (n, i) {
                    return (n != id);
                });
                if (tipo == "ruolo") {
                    lista_note[id] = $(this).find('TEXTAREA').html();
                }

            }
            else if (conoscenza) {

                if (!(lista_cc.indexOf(id) > -1)) lista_cc.push(id);
                lista_com = $.grep(lista_com, function (n, i) {
                    return (n != id);
                });
                if (tipo == "ruolo") {
                    lista_note[id] = $(this).find('TEXTAREA').html();
                }
            }
            else {
                lista_com = $.grep(lista_com, function (n, i) {
                    return (n != id);
                });
                lista_cc = $.grep(lista_cc, function (n, i) {
                    return (n != id);
                });
            }
        });
        lista_com = $.unique(lista_com);
        lista_cc = $.unique(lista_cc);
        //alert(lista_com);
        //alert(lista_cc);
        //alert(lista_note);
        $('#smista_form').data('lista_com', lista_com);
        $('#smista_form').data('lista_cc', lista_cc);
        $('#smista_form').data('lista_note', lista_note);

        // svuoto il form e lo ricostruisco
        $('#smista_main').empty();
        $('#smista_main').focus();

        $('body').scrollTop(0);

        page.loading(true);

        request = $.post(mobileclient.context.WAPath + '/Smistamento/NavigaUO', { "idUO": idUO }, function (data) {

            $('#smista_main').append('<div class="label_tabella"><p>' + data.UOAppartenenza.Descrizione + '</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');

            var li_uoa;

            // se posso accedere alla uo padre inserisco il pulsante di navigazione
            if (data.idParent) {
                li_uoa = '<li class="uoa" val="' + data.UOAppartenenza.Id + '"><div class="item"><p>' + data.UOAppartenenza.Descrizione + '</p><img id="imgSelectParentUO" src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistamento_up_arrow.png" title="Naviga UO Superiore" /><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li>';
            }
            else {
                li_uoa = '<li class="uoa" val="' + data.UOAppartenenza.Id + '"><div class="item"><p>' + data.UOAppartenenza.Descrizione + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li>'
            }

            $('#smista_main').append('<ul class="items_elenco">' + li_uoa + '</ul>');

            // ruoli
            if (data.UOAppartenenza.Ruoli) {

                $('#smista_main > ul.items_elenco > li').append('<ul></ul>');

                $(data.UOAppartenenza.Ruoli).each(function (i, el) {

                    li = $('<li class="ruolo" val="' + el.Id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden img_nota_ruolo" name="img_nota_ruolo"/><div id="expand"><p>' + el.Descrizione + '</p></div><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                    // utenti
                    if (el.Utenti) {
                        ul = $('<div id="container_utenti"><ul></ul></div>');

                        $(el.Utenti).each(function (a, b) {
                            var li = $('<li class="persona" val="' + b.Id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" /><p>' + b.Descrizione + '</p> <div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                            ul.append(li);

                            li.find('img').click(function () {
                                mobileclient.smistamento_addnota(this);
                            })
                        });

                    }

                    li.append(ul);
                    if (collapseRuoli == true) {
                        li.find('#container_utenti').hide();
                    }
                    li.find('#expand').click(function (e) {
                        //alert("ok");
                        $(this).parent().parent().find('#container_utenti').slideToggle("fast");
                        //alert("done");
                        //$("#container_utenti").hide();

                    });
                    li.find('img').click(function () {
                        mobileclient.smistamento_addnota(this);
                    });
                    $('#smista_main > ul.items_elenco > li > ul').append(li);
                });
            }

            $('#imgSelectParentUO').click(function () {
                mobileclient.smistamento_navigaUOinf(data.idParent);
            });

            //uo di livello inferiore

            if (data.AltreUO) {
                if (data.AltreUO.length > 0) {

                    $('#smista_main').append('<div class="label_tabella"><p>Unità organizzative di livello inferiore</p><br clear="all" class="clear"></div>');
                    $('#smista_main').append('<ul class="items_elencoUO"></ul>');
                    $(data.AltreUO).each(function (i, el) {
                        var li = $('<li class="uoa" val="' + el.Id + '"><div class="item"><img id="imgAddNota" src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden img_nota_ruolo" name="img_nota_ruolo"/><p>'
                            + el.Descrizione + '</p>'
                            + '<img id="imgSelectUpUO" src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistamento_down_arrow.png" title="Naviga UO Inferiore" />'
                            + '<div class="inputcheckbox" name="conoscenza">&nbsp;</div>'
                            + '<div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea>'
                            + '<div class="clear"><br class="clear"/></div></div></li>');
                        li.find('#imgAddNota').hide();
                        li.find('#imgAddNota').click(function () { mobileclient.smistamento_addnota(this); });

                        li.find('#imgSelectUpUO').click(function () {
                            var id_uo = el.Id;
                            mobileclient.smistamento_navigaUOinf(id_uo);
                        });

                        $('#smista_main > .items_elencoUO').append(li);
                    });
                }
            }



            // attivo i checkbox per gli elementi memorizzati
            $.each(lista_com, function (index, value) {
                $('#smista_main').find('li').each(function (i) {
                    var li_type = $(this).attr('class');
                    var li_value = $(this).attr('val');
                    if (li_type == "persona") li_value = li_value + "_" + $(this).parent().parent().attr('val') + "_" + $(this).parent().parent().parent().parent().attr('val');
                    if (li_type == "ruolo") li_value = li_value + "_" + $(this).parent().parent().attr('val');
                    if (li_value == value) {
                        // attivo SOLO il checkbox corrispondente (serve per evitare che in caso di ruoli
                        // vengano attivati anche tutti gli utenti nel ruolo)
                        $(this).find('div[name="competenza"]').each(function (j, v) {
                            if ($(this).parent().parent().attr('class') == li_type) $(this).addClass('checked');
                        });
                        // per i ruoli ho anche le eventuali note
                        if (li_type = "ruolo") {
                            //$(this).find('div[name="competenza"]')[0].addClass('checked');
                            $(this).find('img[name=img_nota_ruolo]').removeClass('img_hidden');
                            if (undefined != lista_note[value] && lista_note[value] != '') {
                                $(this).find('TEXTAREA').html(lista_note[value]);
                                $(this).find('img[name=img_nota_ruolo]').attr('src', $(this).find('img[name=img_nota_ruolo]').attr('src').replace('_add', '_do'));
                            }

                        }
                    }

                });
            });
            $.each(lista_cc, function (index, value) {
                $('#smista_main').find('li').each(function (i) {
                    var li_type = $(this).attr('class');
                    var li_value = $(this).attr('val');
                    if (li_type == "persona") li_value = li_value + "_" + $(this).parent().parent().attr('val') + "_" + $(this).parent().parent().parent().parent().attr('val');
                    if (li_type == "ruolo") li_value = li_value + "_" + $(this).parent().parent().attr('val');
                    if (li_value == value) {
                        // attivo SOLO il checkbox corrispondente (serve per evitare che in caso di ruoli
                        // vengano attivati anche tutti gli utenti nel ruolo)
                        $(this).find('div[name="conoscenza"]').each(function (j, v) {
                            if ($(this).parent().parent().attr('class') == li_type) $(this).addClass('checked');
                        });
                        //$(this).find('div[name="conoscenza"]').addClass('checked');
                        if (li_type = "ruolo") {
                            $(this).find('img[name=img_nota_ruolo]').removeClass('img_hidden');
                            if (undefined != lista_note[value]) {
                                $(this).find('TEXTAREA').html(lista_note[value]);
                                $(this).find('img[name=img_nota_ruolo]').attr('src', $(this).find('img[name=img_nota_ruolo]').attr('src').replace('_add', '_do'));
                            }
                        }
                    }

                });
            });
            $('#smista_main').find('.inputcheckbox').click(function () { mobileclient.smistamento_selectoptions(this); });
            page.loading(false);
        });


        // gestione pulsante 'smista'
        // devo permettere lo smistamento anche se non ci sono elementi selezionati per l'uo corrente
        // ma ne esistono per uo inf/sup
        // al click devo prima rimuovere dall'elenco gli eventuali elementi che ho deselezionato
        $('.pulsante_classic_big').unbind('click');
        $('.pulsante_classic_big').click(function () {

            $('#smista_main').find('li').each(function (i) {

                var id = $(this).attr('val');
                var tipo = $(this).attr('class');
                if (tipo == "persona") id = id + "_" + $(this).parent().parent().attr('val') + "_" + $(this).parent().parent().parent().parent().attr('val');
                if (tipo == "ruolo") id = id + "_" + $(this).parent().parent().attr('val');

                // rimuovo tutti gli elementi della uo corrente dalla lista dei corrispondenti delle uo sup/inf
                // tali elementi sono selezionati in seguito nella funzione eseguiSmistamento
                lista_com = $.grep(lista_com, function (n, i) {
                    return (n != id);
                });

                lista_cc = $.grep(lista_cc, function (n, i) {
                    return (n != id);
                });
            });

            $('#smista_form').data('lista_com', lista_com);
            $('#smista_form').data('lista_cc', lista_cc);

            //alert(lista_com[0]); //DEBUG
            //alert(lista_cc[0]); //DEBUG
            if ($('#smista_form').find('.inputcheckbox').hasClass('checked') || $('[name="modello_trasm"]').val() != 'none' || undefined != lista_com[0] || undefined != lista_cc[0]) {
                mobileclient.eseguiSmistamento();
            }
            else {
                tooltip.init({ "titolo": "&Egrave; necessario selezionare almeno un'opzione", 'confirm': 'ok' });
            }
        });


    },

    //Questa funzione Regola lo spegnimento /acccesione delle checkbox nel form smistamento
    smistamento_selectoptions: function (checkbox) {
        var name = $(checkbox).attr('name');
        var li = $(checkbox).parent().parent();
        tipo_li = li.attr('class');

        // MEV SMISTAMENTO - commento l'if per gestire anche la parte della ricerca
        //if (!$(li).parent().hasClass('items_ricerca')) {

        //se la checkbox è un elemento dell'albero sotto, faccio tutti i casi

        switch (tipo_li) {

            case 'persona':
                // MEV SMISTAMENTO
                // per uniformare il comportamento del mobile a quello della versione desktop
                // non posso smistare per conoscenza (comp) ad un utente se è selezionato lo
                // smistamento per competenza (con) al ruolo di appartenenza
                var li_ruolo = $(li).parent().parent();
                var name_up;
                if (name == "competenza")
                    name_up = "conoscenza";
                else
                    name_up = "competenza";
                if ($(li_ruolo.find('div[name=' + name_up + ']')).attr('class') != "inputcheckbox checked") {


                    if (!$(li.find('.item > div[name=' + name + ']')[0]).hasClass('checked')) {
                        //spengo
                        if ($(li.parent().parent().find('div[name="conoscenza"]')).attr('class') != "inputcheckbox checked" &&
                            $(li.parent().parent().find('div[name="competenza"]')).attr('class') != "inputcheckbox checked" &&
                            $(li.parent().parent().parent().parent().find('div[name="conoscenza"]')).attr('class') != "inputcheckbox checked" &&
                            $(li.parent().parent().parent().parent().find('div[name="competenza"]')).attr('class') != "inputcheckbox checked") {
                            //riaccendo
                            // li.find('img').removeClass('img_hidden'); // cliente ha richiesto nessuna nota ad utente
                        }
                        $(li.find('.inputcheckbox')).removeClass('checked');
                        li.find('div[name=' + name + ']').addClass('checked');
                    }
                    else {
                        li.find('div[name=' + name + ']').removeClass('checked');
                        li.find('img').addClass('img_hidden');

                        //se tolto il check a tutti gli utenti, tolgo il check al ruolo
                        if (($(li.parent().parent().find('div[name="conoscenza"]')).attr('class') == "inputcheckbox checked" ||
                            $(li.parent().parent().find('div[name="competenza"]')).attr('class') == "inputcheckbox checked") &&
                            ($(li.parent().find(".checked")).length == "0")) {
                            $(li.parent().parent().find('.inputcheckbox')).removeClass('checked');
                            li.parent().parent().find('img').addClass('img_hidden');
                        }

                        //se di conseguenza tolto il check a tutti i ruoli, tolgo il check alla uo
                        if (($(li.parent().parent().parent().parent().find('div[name="conoscenza"]')).attr('class') == "inputcheckbox checked" ||
                            $(li.parent().parent().parent().parent().find('div[name="competenza"]')).attr('class') == "inputcheckbox checked") &&
                            ($(li.parent().parent().parent().find(".checked")).length == "0")) {
                            $(li.parent().parent().parent().parent().find('.inputcheckbox')).removeClass('checked');
                            li.parent().parent().parent().parent().find('img').addClass('img_hidden');
                        }
                    }
                }
                break;

            case 'ruolo':
                if (!$(li.find('.item > div[name=' + name + ']')[0]).hasClass('checked')) {
                    //spengo
                    if ($(li.parent().parent().find('div[name="conoscenza"]')).attr('class') != "inputcheckbox checked" &&
                        $(li.parent().parent().find('div[name="competenza"]')).attr('class') != "inputcheckbox checked") {
                        //riaccendo
                        li.find('img').addClass('img_hidden');
                        li.find('img[name=img_nota_ruolo]').removeClass('img_hidden');
                    }
                    $(li.find('.inputcheckbox')).removeClass('checked');
                    li.find('div[name=' + name + ']').addClass('checked');
                }
                else {
                    // li.find('.inputcheckbox').removeClass('checked'); // il check va tolto solo agli elementi dello stesso tipo.
                    li.find('div[name=' + name + ']').removeClass('checked');
                    li.find('img').addClass('img_hidden');
                    //se tolto il check a tutti i ruoli, tolgo il check alla uo
                    if (($(li.parent().parent().find('div[name="conoscenza"]')).attr('class') == "inputcheckbox checked" ||
                        $(li.parent().parent().find('div[name="competenza"]')).attr('class') == "inputcheckbox checked") &&
                        ($(li.parent().find(".checked")).length == "0")) {
                        $(li.parent().parent().find('.inputcheckbox')).removeClass('checked');
                        li.parent().parent().find('img').addClass('img_hidden');
                    }
                }
                break;

            case 'uoa':
                if (!$(li.find('.item > div[name=' + name + ']')[0]).hasClass('checked')) {
                    //spengo
                    $(li.find('.inputcheckbox')).removeClass('checked');

                    //riaccendo
                    li.find('div[name=' + name + ']').addClass('checked');
                    li.find('img').addClass('img_hidden');
                    //                        li.find('img[name=img_nota_ruolo]').removeClass('img_hidden');

                }
                else {
                    // li.find('.inputcheckbox').removeClass('checked'); // il check va tolto solo agli elementi dello stesso tipo.
                    li.find('div[name=' + name + ']').removeClass('checked');
                    li.find('img').addClass('img_hidden');
                }
                break;
        }
        //}
        // MEV SMISTAMENTO - OLD CODE
        /*
        else {
        //altrimenti se sto passando le checkbox degli elementi di ricerca, accendo spengo quella che ho cliccato
        if (!$(li.find('.item > div[name=' + name + ']')[0]).hasClass('checked')) {
        $(li.find('.inputcheckbox')).removeClass('checked');
        li.find('img').removeClass('img_hidden');
        $(li.find('.item > div[name=' + name + ']')[0]).addClass('checked');
        }
        else {
        $(li.find('.inputcheckbox')).removeClass('checked');
        li.find('img').addClass('img_hidden');

        }
        }
        */
        // fine MEV SMISTAMENTO - OLD CODE
    },
    smistamento_loaddoc: function () {
        if (this.model.TabModel.NumElements == 0) {
            $('.smistatore_paginatore > P').html('0 di 0');
        }
        else {

            var indice = this.documento_attuale + 1;
            indice = this.model.TabModel.NumResultsForPage * (this.model.TabModel.CurrentPage - 1) + indice;

            $('.smistatore_paginatore > P').html(indice + ' di ' + this.model.TabModel.NumElements);


            var doc = this.model.TabModel.SmistamentoElements[this.documento_attuale];

            var lista_documenti = $('#lista_documenti');
            lista_documenti.html('');

            var info = $('<div class="info"></div>');

            if (doc.Extension.toUpperCase() == "PDF")
                var note = $('<div class="other"><table><tr><th>NOTE GENERALI</th><td>' + doc.NoteGenerali + '</td></tr></table><img name="previewdoc" class="pulsante previewdocP" src="' + this.context.WAPath + '/content/' + this.context.SkinFolder + '/img/' + DEVICE + '/smistatore_pulsante_visualizzadoc.png"/><textarea name="note_generali" class="none"></textarea></div>');
            else
                var note = $('<div class="other"><table><tr><th>NOTE GENERALI</th><td>' + doc.NoteGenerali + '</td></tr></table><textarea name="note_generali" class="none"></textarea></div>');

            note.find("[name='previewdoc']").click(function () {
                //versione light dello zoomdocument
                $('#loading').show();
                mobileclient.zoomDocument(mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id);

                $('#zoomScroller > img').load(function () {
                    $('#loading').hide();
                });

                //VECCHIA ANTEPRIMA SOSTITUITA CON LO ZOOM
                /*page.loading(true);

                imageZoomed = $('<div id="smistamento_preview"><div class="img"><img src="' + mobileclient.context.WAPath + '/Content/img/' + DEVICE + '/smistatore_closebutton.png" class="close closeButton" /></div>');

                var img = $('<img src="' + mobileclient.context.WAPath + '/Documento/Preview/' + doc.Id + '?' + mobileclient.dimImgSmista + '" />');

                img.load(function () {

                $('body').append(imageZoomed);

                $('#smistamento_preview > .img').css({ 'backgroundImage': 'url(' + mobileclient.context.WAPath + '/Documento/Preview/' + doc.Id + '?' + mobileclient.dimImgSmista + ')' });
                $('#smistamento_preview > .img').css({ 'background-size': 'contain' });

                $('#smistamento_preview > .img > .close').click(function () {
                $('#smistamento_preview').remove();
                page.loading(false);
                });
                });
                */
            });

            lista_documenti.append(info);
            lista_documenti.append(note);
            lista_documenti.append('<div class="clear"><br class="clear"/></div>');

            var tableinfo = $('<table></table>');
            if (doc.Segnatura.indexOf('IdDoc') == -1) {
                tableinfo.append('<tr><th>SEGNATURA</th><td>' + doc.Segnatura + '</td></tr>');
            }
            else {
                var iddoc = doc.Segnatura.replace('IdDoc:', '');
                tableinfo.append('<tr><th>ID DOCUMENTO</th><td>' + iddoc + '</td></tr>');
            }
            tableinfo.append('<tr><th>DATA</th><td>' + this.formatDate(doc.DataDoc) + '</td></tr>');

            tableinfo.append('<tr><th>MITTENTE</th><td>' + doc.Mittente + '</td></tr>');

            if (doc.Destinatario != null) tableinfo.append('<tr><th>DESTINATARIO</th><td>' + doc.Destinatario + '</td></tr>');

            tableinfo.append('<tr><th>RAGIONE TRASMISSIONE</th><td>' + doc.RagioneTrasmissione + '</td></tr>');

            tableinfo.append('<tr><th>OGGETTO</th><td>' + doc.Oggetto + '</td></tr>');

            info.append(tableinfo);

        }


    },
    getTrasmission: function (_item) {

        if (_item.Tipo.FASCICOLO) {
            $.post(this.context.WAPath + "/Fascicolo/DettaglioFascTrasm", { "idFasc": _item.Id, "idTrasm": _item.IdTrasm, "idEvento": _item.IdEvento }, function (data) {
                mobileclient.updateDettaglioTrasm(data)
            }, 'json');
        } else {
            $.post(this.context.WAPath + "/Documento/DettaglioDocTrasm", { "idDoc": _item.Id, "idTrasm": _item.IdTrasm, "idEvento": _item.IdEvento }, function (data) {
                mobileclient.updateDettaglioTrasm(data)
            }, 'json');

        }
    },
    getPage: function (URL, numPage) {
        page.loading(true);
        $.post(this.context.WAPath + URL, { "numPage": numPage }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');

    },
    abilitazioneLibroFirma: function () {
        $.ajax({
            url: mobileclient.context.WAPath + '/LibroFirma/LibroFirmaIsAutorized',
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                if (result.IsAutorizedLF) {
                    $('#LIBRO_FIRMA').show();
                    $('#header UL LI').width('14.28%');

                }
                else {
                    $('#LIBRO_FIRMA').hide();
                    $('#header UL LI').width('16.66%');
                }
            },
            async: false
        });
    },

    cambiaRuolo: function (idRuolo) {
        page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/ChangeRole", { "idRuolo": idRuolo }, function (data) {
            mobileclient.model = data;
            //#20140304 inizializza paginatore smistamento
            mobileclient.documento_attuale = 0;
            mobileclient.update();
            mobileclient.abilitazioneLibroFirma();
        }, 'json');
    },
    dettaglioDoc: function (idDoc, idTrasm, idEvento) {
        page.loading(true);
        $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        if (!idTrasm) var idTrasm = ''
        $.post(this.context.WAPath + "/Documento/DettaglioDoc", { "idDoc": idDoc, "idTrasm": idTrasm, "idEvento": idEvento }, function (data) {
            mobileclient.model = data;
            mobileclient.update();
        }, 'json');


    },

    //Mev Navigazione Allegati
    dettaglioDocAllegati: function (idDocPrincipale, idDocCurrent) {
        $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        $.ajax({
            url: this.context.WAPath + "/Documento/DettaglioDoc?idDoc=" + idDocPrincipale,
            type: "POST",
            data: null,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                var allegati = result.TabModel.Allegati;
                var numall = 0;

                $('#name_onservicebar').html('<span>Anteprima della prima pagina - </span><select></select>');
                $(allegati).each(function (i, el) {
                    var nomeDoc = el.Oggetto;
                    if (nomeDoc.length > 128) {
                        nomeDoc = nomeDoc.substring(0, 125) + '...';
                    }
                    if (numall == 0) {
                        $('#name_onservicebar > SELECT').append('<option id="' + el.IdDoc + '" value="' + el.IdDoc + '">' + nomeDoc + ' - (Documento principale)</option>');
                    } else {
                        $('#name_onservicebar > SELECT').append('<option id="' + el.IdDoc + '" value="' + el.IdDoc + '">' + nomeDoc + ' - (Allegato n.' + numall + ' )</option>');
                    }
                    numall++;
                });

                $('#name_onservicebar > SELECT').change(function () {
                    $("select option:selected").each(function () {
                        mobileclient.dettaglioDoc(this.id);
                        //mauro20140210 - Visualizzazione dettaglio firma
                        //inizio
                        if ($("div").hasClass("control_pulsante control_info paddingLeft premuto")) {
                            mobileclient.control_info();
                        } //fine
                    });
                });
                $('#name_onservicebar > SELECT').val(idDocCurrent);
            }
        });

    },

    upFolder: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/UpFolder", {}, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    upRicFolder: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/UpFolder", {}, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    upAdlFolder: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/Adl/UpFolder", {}, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    inFolder: function (idFasc, nameFasc, idTrasm) {
        page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc, "idTrasm": idTrasm }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    inRicFolder: function (idFasc, nameFasc) {
        page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    inAdlFolder: function (idFasc, nameFasc) {
        page.loading(true);
        $.post(this.context.WAPath + "/Adl/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    ricerca: function (testo, tipoRic) {
        page.loading(true);
        //ric_documento|ric_fascicolo|ric_doc_fasc
        if (tipoRic == null) tipoRic = "RIC_DOCUMENTO";
        $.post(this.context.WAPath + "/Ricerca/RicercaTesto", { "testo": testo, "tipoRic": tipoRic }, function (data) {
            mobileclient.model = data;
            mobileclient.update();
            if (testo && tipoRic == "RIC_DOCUMENTO")
                tooltip.init({ "titolo": 'L\'intervallo di ricerca è limitato agli ultimi 31 giorni', 'confirm': 'Ok' })
        }, 'json');

    },
    ricercaAdl: function (testo, tipoRic) {
        page.loading(true);
        //ric_documento|ric_fascicolo|ric_doc_fasc

        if (tipoRic == null) tipoRic = "RIC_DOCUMENTO_ADL";

        $.post(this.context.WAPath + "/Adl/RicercaTesto", { "testo": testo, "tipoRic": tipoRic }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');

    },
    ricercaLF: function (testo, tipoRic) {
        page.loading(true);
        //ric_documento|ric_fascicolo|ric_doc_fasc

        if (tipoRic == null) tipoRic = "RIC_OGGETTO_LF";

        $.post(this.context.WAPath + "/LibroFirma/RicercaTesto", { "testo": testo, "tipoRic": tipoRic }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');

    },
    filtroRicerca: function (idRicSalvata, tipoRicSalvata) {
        page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/Ricerca", { "idRicSalvata": idRicSalvata, "tipoRicSalvata": tipoRicSalvata }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    filtroRicercaAdl: function (idRicSalvata, tipoRicSalvata) {
        page.loading(true);
        $.post(this.context.WAPath + "/Adl/Ricerca", { "idRicSalvata": idRicSalvata, "tipoRicSalvata": tipoRicSalvata }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    accettaTrasmSmista: function () {
        page.loading(true);
        var id = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissione;
        $.post(this.context.WAPath + '/Trasmissione/AccettaTrasm', { idTrasm: id, note: $('.pop > TEXTAREA').val() }, function (data) {
            if ($(data.Errori).size() > 0) {
                tooltip.init({ "titolo": "Errore durante la procedura di accettazione", 'confirm': 'ok' });
            }

            else {
                tooltip.init({ "titolo": "Trasmissione accettata correttamente", 'confirm': 'ok', 'returntrue': function () {
                    mobileclient.documento_attuale = 0;
                    mobileclient.showTab('SMISTAMENTO');
                    window.scrollTo(0, 0);
                }
                })
            }
        }, 'json');
    },

    rifiutaTrasmSmista: function () {
        page.loading(true);
        var id = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissione;
        $.post(this.context.WAPath + '/Trasmissione/RifiutaTrasm', { idTrasm: id, note: $('.pop > TEXTAREA').val() }, function (data) {
            if ($(data.Errori).size() > 0) {
                tooltip.init({ "titolo": "Errore durante la procedura di rifiuto", 'confirm': 'ok' });
            }

            else {
                tooltip.init({ "titolo": "Trasmissione rifiutata correttamente", 'confirm': 'ok', 'returntrue': function () {
                    mobileclient.documento_attuale = 0;
                    mobileclient.showTab('SMISTAMENTO');
                    window.scrollTo(0, 0);
                }
                })
            }
        }, 'json');
    },
    vistoTasmSmista: function () {
        page.loading(true);
        var id = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissione;
        $.post(this.context.WAPath + '/Trasmissione/VistoTrasm', { idTrasm: id }, function (data) {
            if ($(data.Errori).size() > 0) {
                tooltip.init({ "titolo": "Errore dutante la procedura di visto", 'confirm': 'ok' });
            }

            else {
                tooltip.init({ "titolo": "Trasmissione vistata correttamente", 'confirm': 'ok', 'returntrue': function () {
                    mobileclient.documento_attuale = 0;
                    mobileclient.showTab('SMISTAMENTO');
                    window.scrollTo(0, 0);
                }
                })
            }
        }, 'json');
    },
    accettaTrasm: function (inAdl) {
        page.loading(true);
        if (mobileclient._item) {
            var id = mobileclient._item.IdTrasm
            var idDoc = mobileclient._item.Id
            var tipoProto = mobileclient._item.TipoProto
            var isfasc = mobileclient._item.Tipo.FASCICOLO
        }
        else {
            var id = this.model.TabModel.IdTrasm
            var idDoc = this.model.TabModel.Id
            var tipoProto = this.model.TabModel.TipoProto
            var isfasc = this.model.TabModel.Tipo.FASCICOLO
        }
        if (isfasc)
            tipoProto = "fasc"

        if (inAdl == null) {
            $.post(this.context.WAPath + '/Trasmissione/AccettaTrasm', { idTrasm: id, note: $('.pop > TEXTAREA').val() }, function (data) {
                mobileclient.model = data;
                //20140324 gestione ritorno in dettaglio
                mobileclient.dettaglioDoc(idDoc);
                if (!$('#info_trasm').is(':hidden')) mobileclient.control_info();
            }, 'json');
        }
        else {
            $.post(this.context.WAPath + '/Trasmissione/AccettaTrasmInAdl', { idTrasm: id, note: $('.pop > TEXTAREA').val(), idDoc: idDoc, TipoProto: tipoProto }, function (data) {
                mobileclient.model = data;
                //20140324 gestione ritorno in dettaglio
                mobileclient.dettaglioDoc(idDoc);
                if (!$('#info_trasm').is(':hidden')) mobileclient.control_info();
            }, 'json');
        }

    },
    vistoTrasm: function () {
        page.loading(true);
        if (mobileclient._item.IdTrasm) {
            var id = mobileclient._item.IdTrasm;
            $.post(this.context.WAPath + '/Trasmissione/VistoTrasm', { idTrasm: id }, function (data) {
                mobileclient.model = data;
                mobileclient.update()
            }, 'json');
        }
        else {
            var id = mobileclient._item.IdEvento;
            $.post(this.context.WAPath + '/ToDoList/RimuoviNotificaToDoList', { idEvento: id }, function (data) {
                mobileclient.model = data;
                mobileclient.update()
            }, 'json');
        }
    },
    rifiutaTrasm: function () {
        page.loading(true);
        if (mobileclient._item) {
            var id = mobileclient._item.IdTrasm;
        }
        else {
            var id = this.model.TabModel.IdTrasm
        }
        $.post(this.context.WAPath + '/Trasmissione/RifiutaTrasm', { idTrasm: id, note: $('.pop > TEXTAREA').val() }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    trasmissioneForm: function (doc, id, idTrasm) {
        $('#firma,#firmaView').addClass('hide');

        if (doc) {
            $.post(this.context.WAPath + "/Trasmissione/TrasmissioneFormDoc", { "idDoc": id, "idTrasmPerAcc": idTrasm }, function (data) {
                mobileclient.updateTrasmissione(data)
            }, 'json');
        }
        else {
            $.post(this.context.WAPath + "/Trasmissione/TrasmissioneFormFasc", { "idFasc": id, "idTrasmPerAcc": idTrasm }, function (data) {
                mobileclient.updateTrasmissione(data)
            }, 'json');
        }
    },

    inserisciInAdl: function (doc, id, tipoProto) {
        $('#firma,#firmaView').addClass('hide');
        if (doc) {
            //Mev gestione ADL da maschera ricerca
            $.post(this.context.WAPath + '/Adl/AddDocInAdl', { "Id": id, "TipoProto": tipoProto }, function (data) {
                mobileclient.model = data;
                //20140324 gestione ritorno in dettaglio
                mobileclient.dettaglioDoc(id);
                if (!$('#info_trasm').is(':hidden')) mobileclient.control_info();
            }, 'json');
        }
        else {
            $.post(this.context.WAPath + '/Adl/AddFascInAdl', { "Id": id, "TipoProto": tipoProto }, function (data) {
                mobileclient.model = data;
                mobileclient.update()
            }, 'json');
        }
    },
    rimuoviDaAdl: function (doc, id, tipoProto) {
        $('#firma,#firmaView').addClass('hide');
        if (doc) {
            $.post(this.context.WAPath + '/Adl/RemoveDocFromAdl', { "Id": id, "TipoProto": tipoProto }, function (data) {
                mobileclient.model = data;
                mobileclient.update()
            }, 'json');
        } else {
            $.post(this.context.WAPath + '/Adl/RemoFascFromAdl', { "Id": id, "TipoProto": tipoProto }, function (data) {
                mobileclient.model = data;
                mobileclient.update()
            }, 'json');
        }
    },
    eseguiTrasm: function (_item, idTrasmModel, note) {
        $('#firma,#firmaView').addClass('hide');
        page.loading(true);
        var hasTrans = 0;
        if (_item.TabModel.IdTrasmPerAcc) hasTrans = 1;

        if (_item.TabModel.DocInfo) {
            $.post(this.context.WAPath + '/Trasmissione/EseguiTrasmDoc', { "idTrasmModel": idTrasmModel, "idDoc": _item.TabModel.DocInfo.IdDoc, 'note': note, 'idTrasmPerAcc': _item.TabModel.IdTrasmPerAcc }, function (data) {
                if (data.Success == true) message = 'Trasmissione effettuata correttamente';
                else message = 'Errore di trasmissione';
                tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                    if (hasTrans == 0) mobileclient.control_info()
                }
                });
            }, 'json');
        }
        else {
            $.post(this.context.WAPath + '/Trasmissione/EseguiTrasmFasc', { "idTrasmModel": idTrasmModel, "idFasc": _item.TabModel.FascInfo.IdFasc, 'note': note, 'idTrasmPerAcc': _item.TabModel.IdTrasmPerAcc }, function (data) {
                if (data.Success == true) message = 'Trasmissione effettuata correttamente';
                else message = 'Errore di trsasmissione';
                tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                    if (hasTrans == 0) mobileclient.control_info()
                }
                });
            }, 'json');
        }
        if (hasTrans == 1) {

            mobileclient.showTab('TODO_LIST')
            mobileclient.control_info()
        }
    },
    accettaDelega: function (Id, IdRuoloDelegante, CodiceDelegante) {
        page.loading(true);
        $.post(this.context.WAPath + '/Delega/AccettaDelega', { "Id": Id, "IdRuoloDelegante": IdRuoloDelegante, "CodiceDelegante": CodiceDelegante }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    RevocaDeleghe: function (deleghe) {
        page.loading(true);

        $.post(this.context.WAPath + '/Delega/RevocaDeleghe', { "deleghe": utility.format2json(deleghe) }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');

    },
    dismettiDelega: function (idDelegante) {
        page.loading(true);
        $.post(this.context.WAPath + '/Delega/DismettiDelega', { "idDelegante": idDelegante }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    creaDelega: function () {

        page.loading(true);
        var idUtente = $('#ricercadelegato').attr('valore');
        var idRuolo = $('#ricercaruolo').attr('valore');


        var dataInizio = new Date($("#dataInizioD").val()).format("dd/MM/yyyy");
        var dataFine = new Date($("#dataFineD").val()).format("dd/MM/yyyy");

        if (dataInizio == 'aN/aN/NaN') {
            var a = $("#dataInizioD").val().split(/[^0-9]/);
            dataInizio = new Date(a[0], a[1] - 1, a[2]).format("dd/MM/yyyy");

            var b = $("#dataFineD").val().split(/[^0-9]/);
            dataFine = new Date(b[0], b[1] - 1, b[2]).format("dd/MM/yyyy");
        }

        if ($("#dataInizioD").val().length > 0 && new Date($("#dataInizioD").val().split(/[^0-9]/)[0], $("#dataInizioD").val().split(/[^0-9]/)[1] - 1, $("#dataInizioD").val().split(/[^0-9]/)[2], new Date().getHours(), new Date().getMinutes(), new Date().getSeconds(), new Date().getMilliseconds()) < new Date()) {
            //var dt = new Date($("#dataInizioD").val().split(/[^0-9]/)[0], $("#dataInizioD").val().split(/[^0-9]/)[1] - 1, $("#dataInizioD").val().split(/[^0-9]/)[2], new Date().getHours(), new Date().getMinutes(), new Date().getSeconds(), new Date().getMilliseconds());
            tooltip.init({ "titolo": 'La data di decorrenza non deve essere inferiore alla data corrente!', 'confirm': 'ok' });
        }
        else if ($("#dataFineD").val().length > 0 && new Date($("#dataFineD").val().split(/[^0-9]/)[0], $("#dataFineD").val().split(/[^0-9]/)[1] - 1, $("#dataFineD").val().split(/[^0-9]/)[2]) < new Date($("#dataInizioD").val().split(/[^0-9]/)[0], $("#dataInizioD").val().split(/[^0-9]/)[1] - 1, $("#dataInizioD").val().split(/[^0-9]/)[2])) {
            tooltip.init({ "titolo": 'Verificare intervallo date!', 'confirm': 'ok' });
        }
        else {
            $.post(this.context.WAPath + '/Delega/CreaDelega', { "idUtente": idUtente, "idRuolo": idRuolo, "dataInizio": dataInizio, "dataFine": dataFine }, function (data) {
                if (data.Success) {
                    tooltip.init({ "titolo": "Delega creata correttamente", 'confirm': 'ok', 'returntrue': function () {
                        mobileclient.showTab('LISTA_DELEGHE');
                    }
                    })
                }
                else if (data.Error) {
                    tooltip.init({ "titolo": data.Error, 'confirm': 'ok' });
                }

            }, 'json');
        }
    },
    cambiaStatoElementoLf: function (_item) {
        page.loading(true);
        var tempScrollTop = $(window).scrollTop();

        $.post(this.context.WAPath + "/LibroFirma/ChangeState", { "idElemento": _item.IdElemento, "statoCorrente": _item.StatoFirma, "dataAccettazione": _item.DataAccettazione, "motivoRespingimento": _item.MotivoRespingimento, "idIstanzaProcesso": _item.IdIstanzaProcesso }, function (data) {
            mobileclient.model = data;
            mobileclient.update();
            $(window).scrollTop(tempScrollTop);
        }, 'json');
    },

    respingiSelezionatiLf: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/LibroFirma/RespingiSelezionati", function (data) {
            mobileclient.model = data;
            mobileclient.update();
            if (data.Success == true) message = 'Trasmissione effettuata correttamente';
            else message = 'Errore di trasmissione';
        }, 'json');
    },

    firmaSelezionatiLf: function (typeSign, isSignedCades) {
        page.loading(true);

        //Verifico se esiste qualche elemento in LF da firmare CADES
        if (typeSign == 'cades') {
            $.ajax({
                url: mobileclient.context.WAPath + '/LibroFirma/ExistsElementWithSignCades',
                type: 'POST',
                dataType: 'json',
                success: function (result) {
                    if (result.TabModel.ExistsSignCades) {
                        mobileclient.popAddFirmaFromLf(true, false);
                    } else {
                        var signCades = false;
                        mobileclient.firmaSelezionatiLf('pades', signCades);
                    }
                    // $('#loading').hide();
                }
            });
        }
        //Verifico se esiste qualche elemento in LF da firmare PADES
        if (typeSign == 'pades') {
            $.ajax({
                url: mobileclient.context.WAPath + '/LibroFirma/ExistsElementWithSignPades',
                type: 'POST',
                dataType: 'json',
                success: function (result) {
                    if (result.TabModel.ExistsSignPades) {
                        mobileclient.popAddFirmaFromLf(false, isSignedCades);
                    }
                    else {
                        mobileclient.firmaSelezionatiLf('elettronica', false);
                    }
                    //$('#loading').hide();
                }
            });
        }

        if (typeSign == 'elettronica') {
            //sottoscrizione e avanzamento iter
            $.post(this.context.WAPath + "/LibroFirma/FirmaSelezionati", function (data) {
                mobileclient.model = data;
                mobileclient.update();
            }, 'json');
        }

    },

    eseguiSmistamento: function () {

        var elements = new Array();
        var fromUOInfSup = false;

        var lista_com = $('#smista_form').data('lista_com');
        var lista_cc = $('#smista_form').data('lista_cc');
        var lista_note = $('#smista_form').data('lista_note');
        if (!(undefined == lista_com && undefined == lista_cc)) {
            fromUOInfSup = true;
        }

        if (undefined == lista_com) lista_com = [];
        if (undefined == lista_cc) lista_cc = [];
        if (undefined == lista_note) lista_note = [];
        //alert("com: " + lista_com.length + " - " + lista_com); //DEBUG
        //alert(lista_cc); //DEBUG
        //alert(lista_note.length); //DEBUG

        //prima carico gli elementi dell'albero, successivamente la ricerca
        $('.items_elenco').find('.persona').each(function () {

            var IdUtente = $(this).attr('val');
            var idRuolo = $(this).parent().parent().attr('val');
            var IdUO = $(this).parent().parent().parent().parent().attr('val');
            // MEV SMISTAMENTO - per elementi provenienti dalla ricerca ajax
            var isFromRicerca = false;

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            var NoteIndividuali = $(this).find('TEXTAREA').html();

            if (conoscenza == true || competenza == true) {
                // MEV SMISTAMENTO
                //alert(IdUtente + ' - ' + idRuolo + ' - ' + IdUO + ' - ' + competenza + ' - ' + conoscenza + ' - ' + isFromRicerca); //DEBUG
                //elements.push({ "IdUtente": IdUtente, "IdRuolo": idRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali });
                elements.push({ "IdUtente": IdUtente, "IdRuolo": idRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali, "isRicerca": isFromRicerca });
            }
        });

        $('.items_elenco').find('.ruolo').each(function () {
            // if ($(this).find('ul').children().size() == 0) {
            var IdRuolo = $(this).attr('val');
            var IdUO = $(this).parent().parent().attr('val');
            // MEV SMISTAMENTO
            var isFromRicerca = false;

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            var NoteIndividuali = $(this).find('TEXTAREA').html();

            if (conoscenza == true || competenza == true) {
                // MEV SMISTAMENTO
                //alert(' - ' + idRuolo + ' - ' + IdUO + ' - ' + competenza + ' - ' + conoscenza + ' - ' + isFromRicerca); //DEBUG
                //elements.push({ "IdUtente": "", "IdRuolo": IdRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali });
                elements.push({ "IdUtente": "", "IdRuolo": IdRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali, "isRicerca": isFromRicerca });
            }
            // }
        });

        $('.items_elenco').find('.uoa').each(function () {
            if ($(this).find('ul').children().size() == 0) {
                var IdUO = $(this).attr('val');
                // MEV SMISTAMENTO
                var isFromRicerca = false;

                var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
                var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');
                if (conoscenza == true || competenza == true) {
                    elements.push({ "IdUtente": "", "IdRuolo": "", "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "isRicerca": isFromRicerca });
                }
            }
        });

        //alert("vista principale - elements: " + elements.length); //DEBUG

        // MEV SMISTAMENTO - carico gli elementi provenienti dalla ricerca ajax

        $('.items_ricerca').find('.persona').each(function () {

            var lista_id = $(this).attr('val').split('-');

            var IdUtente = lista_id[0];
            var IdRuolo = lista_id[1];
            var IdUO = lista_id[2];
            var isFromRicerca = true;

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            //var NoteIndividuali = $(this).find('TEXTAREA').html();
            // MEV SMISTAMENTO DEBUG
            //alert(IdUtente + ' - ' + IdRuolo + ' - ' + IdUO + ' - ' + competenza + ' - ' + conoscenza + ' - ' + isFromRicerca); DEBUG

            if (conoscenza == true || competenza == true) {
                // MEV SMISTAMENTO
                //alert("IdUtente:" + IdUtente + " IdRuolo:" + IdRuolo + " IdUO:" + IdUO + " Competenza:" + competenza + " Conoscenza:" + conoscenza + " NoteIndividuali:" + "isRicerca:" + isFromRicerca); DEBUG
                //elements.push({ "IdUtente": IdUtente, "IdRuolo": idRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali });
                elements.push({ "IdUtente": IdUtente, "IdRuolo": IdRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": "", "isRicerca": isFromRicerca });
            }
        });
        $('.items_ricerca').find('.ruolo').each(function () {

            var lista_id = $(this).attr('val').split('-');

            var IdUtente = lista_id[0];
            var IdRuolo = lista_id[1];
            var IdUO = lista_id[2];
            var isFromRicerca = true;

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            var NoteIndividuali = $(this).find('TEXTAREA').html();

            if (conoscenza == true || competenza == true) {
                // MEV SMISTAMENTO
                //alert("IdUtente:" + IdUtente + " IdRuolo:" + IdRuolo + " IdUO:" + IdUO + " Competenza:" + competenza + " Conoscenza:" + conoscenza + " NoteIndividuali:" + NoteIndividuali + " isRicerca:" + isFromRicerca); DEBUG
                elements.push({ "IdUtente": "", "IdRuolo": IdRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali, "isRicerca": isFromRicerca });
            }
        });
        $('.items_ricerca').find('.uoa').each(function () {

            //if ($(this).find('ul').children().size() == 0) {
            var lista_id = $(this).attr('val').split('-');
            var IdUO = lista_id[2];
            var isFromRicerca = true;

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            if (conoscenza == true || competenza == true) {
                elements.push({ "IdUtente": "", "IdRuolo": "", "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": "", "isRicerca": isFromRicerca });
            }
            //}
        });

        //alert("da ricerca - elements: " + elements.length); //DEBUG

        // ---- MEV SMISTAMENTO - UO inferiori e superiori ----

        // COMPETENZA
        $.each(lista_com, function (index, value) {
            var ids = value.split('_');
            // persona
            if (ids.length == 3) {
                elements.push({ "IdUtente": ids[0], "IdRuolo": ids[1], "IdUO": ids[2], "Competenza": true, "Conoscenza": false, "NoteIndividuali": "", "isRicerca": false });
                //alert(ids[0] + ids[1] + ids[2] + "Comp"); //DEBUG
            }
            // ruolo
            if (ids.length == 2) {
                elements.push({ "IdUtente": "", "IdRuolo": ids[0], "IdUO": ids[1], "Competenza": true, "Conoscenza": false, "NoteIndividuali": lista_note[value], "isRicerca": false });
                //alert(ids[0] + ids[1] + " Comp " + lista_note[value]); //DEBUG
            }
            // uo
            if (ids.length == 1) {
                elements.push({ "IdUtente": "", "IdRuolo": "", "IdUO": ids[0], "Competenza": true, "Conoscenza": false, "NoteIndividuali": "", "isRicerca": false });
                //alert(ids[0] + " Comp "); //DEBUG
            }
        });
        // CONOSCENZA
        $.each(lista_cc, function (index, value) {
            var ids = value.split('_');
            // persona
            if (ids.length == 3) {
                elements.push({ "IdUtente": ids[0], "IdRuolo": ids[1], "IdUO": ids[2], "Competenza": false, "Conoscenza": true, "NoteIndividuali": "", "isRicerca": false });
                //alert(ids[0] + ids[1] + ids[2] + "Comp"); //DEBUG
            }
            // ruolo
            if (ids.length == 2) {
                elements.push({ "IdUtente": "", "IdRuolo": ids[0], "IdUO": ids[1], "Competenza": false, "Conoscenza": true, "NoteIndividuali": lista_note[value], "isRicerca": false });
                //alert(ids[0] + ids[1] + " Comp " + lista_note[value]); //DEBUG
            }
            // uo
            if (ids.length == 1) {
                elements.push({ "IdUtente": "", "IdRuolo": "", "IdUO": ids[0], "Competenza": false, "Conoscenza": true, "NoteIndividuali": "", "isRicerca": false });
                //alert(ids[0] + " Comp "); //DEBUG
            }
        });

        //alert("fine - elements: " + elements.length); //DEBUG

        // ---- MEV SMISTAMENTO - OLD CODE ----

        //carico gli elementi della ricerca

        //        $('.items_ricerca > LI').each(function () {
        //            var lista_id = $(this).attr('val').split('-');

        //            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
        //            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');
        //            // MEV MOBILE DEBUG
        //            alert(lista_id[0] + ' - ' + lista_id[1] + ' - ' + lista_id[2] + ' - ' + competenza + ' - ' + conoscenza);
        //            if (conoscenza == true || competenza == true) {
        //                // MEV MOBILE DEBUG
        //                alert(lista_id[0] + ' - ' + lista_id[1] + ' - ' + lista_id[2] + ' - ' + competenza + ' - ' + conoscenza);
        //                elements.push({ "IdUtente": lista_id[0], "IdRuolo": lista_id[1], "IdUO": lista_id[2], "Competenza": competenza, "Conoscenza": conoscenza });
        //            }
        //        })

        // ---- fine MEV SMISTAMENTO - OLD CODE ----


        //parametri del doc
        var IdDoc = this.model.TabModel.SmistamentoElements[this.documento_attuale].Id;
        var idTrasm = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissione;
        var idTrasmUtente = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissioneUtente;
        var IdTrasmSingola = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissioneSingola;
        if ($('#smista_form').find('.inputcheckbox').hasClass('checked') || fromUOInfSup == true) {
            page.loading(true);

            $.post(this.context.WAPath + '/Smistamento/EseguiSmistamento', { "idDoc": IdDoc, "idTrasm": idTrasm, "idTrasmUtente": idTrasmUtente, "IdTrasmSingola": IdTrasmSingola, "note": $('textarea[name="note_generali"]').val(), "elements": utility.format2json(elements) }, function (data) {
                //prima smisto
                if (data.Success) {
                    tooltip.init({ "titolo": "Smistamento effettuato correttamente", 'confirm': 'ok', 'returntrue': function () {

                        //poi se ho selezionato un modello di trasmissione...trasmetto
                        if ($('[name="modello_trasm"]').val() != 'none') {

                            idTrasmModel = $('[name="modello_trasm"]').val();

                            $.post(mobileclient.context.WAPath + '/Trasmissione/EseguiTrasmDoc', { "idTrasmModel": idTrasmModel, "idDoc": IdDoc, 'note': ''/*, "idTrasmPerAcc": idTrasm*/ }, function (data2) {

                                if (data2.Success == true) message = 'Smistamento tramite modello di trasmissione effettuato correttamente';
                                else message = 'Errore nello smistamento tramite modello di trasmissione';

                                tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                                    mobileclient.documento_attuale = 0;
                                    mobileclient.showTab('SMISTAMENTO');
                                }
                                });
                            }, 'json');
                        }
                        else {
                            mobileclient.documento_attuale = 0;
                            mobileclient.showTab('SMISTAMENTO');
                        }

                        window.scrollTo(0, 0);
                    }
                    })
                }
                else if (data.Error) {
                    tooltip.init({ "titolo": "Smistamento non effettuato", 'confirm': 'ok' });
                }

            }, 'json');
        }
        else {
            if ($('[name="modello_trasm"]').val() != 'none') {
                page.loading(true);

                idTrasmModel = $('[name="modello_trasm"]').val();

                $.post(mobileclient.context.WAPath + '/Trasmissione/EseguiTrasmDoc', { "idTrasmModel": idTrasmModel, "idDoc": IdDoc, 'note': '', "idTrasmPerAcc": idTrasm }, function (data2) {

                    if (data2.Success == true) message = 'Smistamento tramite modello di trasmissione effettuato correttamente';
                    else message = 'Errore nello smistamento tramite modello di trasmissione';

                    tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                        mobileclient.documento_attuale = 0;
                        mobileclient.showTab('SMISTAMENTO');
                    }
                    });

                    window.scrollTo(0, 0);
                }, 'json');
            }
        }


    },
    smistamentoForm: function (numPage) {
        page.loading(true);
        $.post(this.context.WAPath + '/Smistamento/SmistamentoForm', { "numPage": numPage }, function (data) {
            mobileclient.model = data;
            mobileclient.update()
        }, 'json');
    },
    logout: function () {
        document.location.href = this.context.WAPath + '/Home/Logout';
    },
    skinSelect: function (elem) {

        textSelect = '';
        if ((this.model.TabShow.TODO_LIST == 0) && (!isTransmit)) {
            textSelect = 'Seleziona il ruolo';
        }
        else if ((this.model.TabShow.RICERCA == 4) && (!isTransmit)) {
            textSelect = 'Seleziona la ricerca salvata';
        }
        else if ((this.model.TabShow.TRASMISSIONE == 5) || (isTransmit)) {
            textSelect = 'Seleziona il modello di trasmissione';
        }

        if ($(elem).children().length != 0) {
            if (!$(elem).parent().hasClass('select')) {
                $(elem).hide();
                selectOriginale = elem;
                selOption(selectOriginale, textSelect);
            }
        } else {
            $(elem).hide();
        }
        isTransmit = false;


    },
    formatTrasmDate: function (data, sep) {
        var data = eval(data.replace(/\/Date\((\d+)\)\//gi, "new Date($1)")) + ' ';
        data = data.substr(0, 15);
        data = data.split(' ');
        //  Mon Jul 26 2010 11:19:21

        switch (data[1]) {
            case 'Jan':
                mese = "01";
                break;
            case 'Feb':
                mese = "02";
                break;
            case 'Mar':
                mese = "03";
                break;
            case 'Apr':
                mese = "04";
                break;
            case 'May':
                mese = "05";
                break;
            case 'Jun':
                mese = "06";
                break;
            case 'Jul':
                mese = "07";
                break;
            case 'Aug':
                mese = "08";
                break;
            case 'Sep':
                mese = "09";
                break;
            case 'Oct':
                mese = "10";
                break;
            case 'Nov':
                mese = "11";
                break;
            case 'Dec':
                mese = "12";
                break;
        }
        if (!sep) sep = '.';
        var data = data[2] + sep + mese + sep + data[3];
        return data;
    }

}
//funzioni utilità
var utility = {
    format2json: function (obj) {
        var tipo = typeof (obj);
        if (tipo != "object" || obj === null) {

            if (tipo == "string") obj = '"' + obj + '"';
            return String(obj);
        }
        else {

            var n, v, json = [], arr = (obj && obj.constructor == Array);
            for (n in obj) {
                v = obj[n];
                tipo = typeof (v);
                if (tipo == "string") v = '"' + v + '"';
                else if (tipo == "object" && v !== null) v = utility.format2json(v);
                json.push((arr ? "" : '"' + n + '":') + String(v));
            }
            return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
        }
    },
    var_dump: function (arr, level) {
        var dumped_text = "";
        if (!level) level = 0;

        //The padding given at the beginning of the line.
        var level_padding = "";
        for (var j = 0; j < level + 1; j++) level_padding += "    ";

        if (typeof (arr) == 'object') { //Array/Hashes/Objects
            for (var item in arr) {
                var value = arr[item];

                if (typeof (value) == 'object') { //If it is an array,
                    dumped_text += level_padding + "'" + item + "' ...\n";
                    dumped_text += dump(value, level + 1);
                } else {
                    dumped_text += level_padding + "'" + item + "' => \"" + value + "\"\n";
                }
            }
        } else { //Stings/Chars/Numbers etc.
            dumped_text = "===>" + arr + "<===(" + typeof (arr) + ")";
        }
        return dumped_text;
    },
    checkOrientation: function (orientation) {
        switch (orientation) {
            case 0:
                contenType = "normal";
                break;

            case -90:
                contenType = "right";
                break;

            case 90:
                contenType = "left";
                break;

            case 180:
                contenType = "normal";
                break;
        }

        if (mobileclient.contenType != contenType) {

            mobileclient.contenType = contenType;

            mobileclient.fixPopTrasm();

            if ((mobileclient.model.TabShow.RICERCA || mobileclient.model.TabShow.AREA_DI_LAVORO) && $(window).width() <= 600) {
                mobileclient.update();
            }

            if (DEVICE == 'ipad') {
                checkHeight();
            }
            ;
        }


    }
}

function selOption(e, titSelect) {
    contList = $('<div class="contList"><h2>' + titSelect + '</h2></div>');
    contScroll = $('<div class="contScroll"></div>');
    UList = $('<ul id="listSelect"></ul>');
    cont = $('<div class="conts"></div>');
    $(e).children().each(function (i, elem) {
        textOption = $(this).text();
        active = ($(this).attr('selected')) ? 'class="active"' : '';
        UList.append('<li ' + active + '>' + textOption + '</li>');
    });
    $(document.body).append(cont);
    $(contScroll).append(UList);
    $(contList).append(contScroll);
    $(document.body).append(contList);

    $("#listSelect").children().each(function (i, el) {
        $(el).click(function () {
            e.selectedIndex = i;
            $(contList).remove();
            $(cont).remove();
            attuale = $($(e).children()[e.selectedIndex]).html();
            var contentNode = $(e).children()[e.selectedIndex];
            $(e).prev().html(attuale);
            $('#iPhoneFilterT').html(attuale);
            $(e).val($(contentNode).val())
            $(e).trigger('change');

        });
    });
    window.scrollTo(0, 0);
}

Date.prototype.format = function (format) //author: meizz
{
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }

    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
            RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}
