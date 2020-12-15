var tabletMinW=700;
var smartMinW=450;
var isTransmit=false;
var zoomScroll;
var isiOS=false;
$(window).resize ( function (e) {

	mobileclient.iPhoneGest();
	mobileclient.setitemposition('#ricerca_item > .item_list');
	mobileclient.setitemposition('#item_list');

	if(($(window).width()<=tabletMinW)&&($(window).width()>smartMinW)){
		DEVICE='galaxy';
	}else if(($(window).width()<=smartMinW) ) {
		DEVICE='smart';
	} else {
		DEVICE='ipad';
	}
	

	
	if((mobileclient.model.TabShow.AREA_DI_LAVORO||mobileclient.model.TabShow.SMISTAMENTO)&&(DEVICE=='smart')){
		mobileclient.showTab('TODO_LIST')
	}
	
	var testo='Al documento principale';
	if(DEVICE=='smart'){
		testo='Indietro';
	}
   $('#docPrincipale > a').text(testo);
	
	$('.previewdocP').attr('src',mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/smistatore_pulsante_visualizzadoc.png');
	$('.infoUtenteP').attr('src',mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/info_utente.png');
	$('.arrow_azioni').attr('src',mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png');
	$('.rrw').attr('src',mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_rrw.png');
	$('.rw').attr('src',mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_rw.png');
	$('.gra0').attr('src',mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png');
	$('.fw').attr('src',mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_fw.png');
	$('.ffw').attr('src',mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/paginatore_ffw.png');
	$('.ar_left,.ar_right').attr('src',mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/smistatore_paginatore_arrow.png');
	$('.img_nota_ruolo').attr('src',mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png');
	 $('#previewdoc').attr('src',mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/smistatore_pulsante_visualizzadoc.png');
	$('.closeButton').attr('src',mobileclient.context.WAPath + '/Content/img/' + DEVICE + '/smistatore_closebutton.png');
	
	if($('#RIC_SALVATE.on').size()>0){
		mobileclient.reposArrow('RIC_SALVATE');
	}
	
	checkHeight();
	checkHeightZoom();
	
	var $elem=$('#wrapper');
    var orgDisplay=$elem.css('display');
    $elem.css('display','none');
    $elem.get(0).offsetHeight;
    $elem.css('display',orgDisplay);
	
	if(DEVICE=='smart'){
			heightPDF = 1200;
			widthPDF =  845;
			ratioPDF = widthPDF/heightPDF;			
			widthIMG = $(window).width()+10;
			heightIMG = Math.ceil((widthIMG*heightPDF)/widthPDF);
			$('#prev_doc').height(473).css({'background-size': '100%  auto'});
			//$('#prev_doc.loaded').height(heightIMG).css({'background-size': 'auto '+heightIMG+'px'});
		}
	 
	
	
});


(function($){
	// Determine if we on iPhone or iPad
	
	var agent = navigator.userAgent.toLowerCase();
	DEVICE='ipad';
	if(($(window).width()<=tabletMinW)&&($(window).width()>smartMinW)){
		DEVICE='galaxy';
	}else if(($(window).width()<=smartMinW) ) {
		DEVICE='smart';
	} else {
		DEVICE='ipad';
	}

	
	
	if(agent.indexOf('iphone') >= 0 || agent.indexOf('ipad') >= 0){
	       isiOS = true;
		   $('body').addClass('iOS');
	}
 
 	
 
	$.fn.doubletap = function(onDoubleTapCallback, onTapCallback, delay){
		var eventName, action;
		delay = delay == null? 500 : delay;
		eventName = isiOS == true? 'touchend' : 'click';
 
		$(this).bind(eventName, function(event){
			var now = new Date().getTime();
			var lastTouch = $(this).data('lastTouch') || now + 1 /** the first time this will make delta a negative number */;
			var delta = now - lastTouch;
			clearTimeout(action);
			if(delta<delay && delta>0){
				if(onDoubleTapCallback != null && typeof onDoubleTapCallback == 'function'){
					onDoubleTapCallback(event);
				}
			}else{
				$(this).data('lastTouch', now);
				action = setTimeout(function(evt){
					if(onTapCallback != null && typeof onTapCallback == 'function'){
						onTapCallback(evt);
					}
					clearTimeout(action);   // clear the timeout
				}, delay, [event]);
			}
			$(this).data('lastTouch', now);
		});
	};
})(jQuery);

contenType='normal';


	
	headerH = '';
	footerH = '';
	wrapperH = ''
	
	
	


  
  
	var myScroll;
	var timer;
	
	
	
	function checkHeightZoom(){
	
		 width = $(window).width() - 20;
		 var variation=0;
			if(DEVICE=='smart'){
					width = $(window).width() * 2;
					variation=10;
			}
			var height=width*1.42;
	
		$('#zoomScroller').css({'width':width+'px','height':height+'px'});
	
		$('#zoomWrapper').css({'width':$(window).width()+'px'});
		var actHeaderh=$('#image_zoomed .green_bar').height();
		var h = $(window).height() - actHeaderh-variation;
		$('#zoomWrapper').height(h);
	}
	function setHeight() {
		$('#wrapper').height(wrapperH);
		//if ($.browser.webkit) if(myScroll){	myScroll.refresh();	}
		//if(myScroll){	$('#wrapper').getNiceScroll().resize();}
		
		//utility.checkOrientation(window.orientation);
	}
	function checkHeight(){
		headerH = document.getElementById('header').offsetHeight;
		footerH = document.getElementById('footer').offsetHeight;
		wrapperH = window.innerHeight - headerH - footerH;
		var lowHeight=false;
		if(wrapperH<50){
			lowHeight=true;
		}
		
		if(isInput(getActiveElement())&&((isiOS)||(lowHeight))){
			var keyHeight= window.innerHeight * (window.innerHeight > window.innerWidth ? 0.66 : 0.45);
			wrapperH = window.innerHeight - headerH - footerH+keyHeight;
			
		}
		setHeight();
	}
	
	
	function isInput(el) {
    var tagName = el && el.tagName && el.tagName.toLowerCase();
    return (tagName == 'input' && el.type != 'button' && el.type != 'radio' && el.type != 'checkbox') || (tagName == 'textarea');
};

function getActiveElement() {
    try {
        return document.activeElement;  // can get exeption in IE8
    } catch(e) {
    }
};
	
	function loaded() {
		checkHeight()
		timer = setInterval('setHeight()',50);		
		//if ($.browser.webkit) myScroll = new iScroll('scroller', {desktopCompatibility:true });
		//myScroll=$('#wrapper').niceScroll();
		
	}
	
	var preventBehavior = function(e) {
	  e.preventDefault();
	};
 
	// Enable fixed positioning
	//document.addEventListener("touchmove", preventBehavior, false);
	//document.addEventListener('DOMContentLoaded', loaded, false);
	


$(document).ready(function() {

	//window.addEventListener('onorientationchange' in window ? 'orientationchange' : 'resize', checkHeight, false);
	//window.addEventListener('onorientationchange' in window ? 'orientationchange' : 'resize', function(){utility.checkOrientation(window.orientation)}, false);
	
	
	
	mobileclient.init();
	checkHeight();
	
	
	
});

var page = {
	loading : function(mode){
		
		
		if ( mode == true){
			$(document.body).append($('<div id="loading">loading...</div>'));
			$('#loading').show();
			$('SELECT').css({'visibility':'hidden'})
			
			
		}
		else{
			$('#loading').remove();
			$('SELECT').css({'visibility':'visible'})
		}
	}
}




var mobileclient = {
    init: function (idTab) {
	checkHeightZoom()
	this.iPhoneGest();
        //setto variabili
        this.application = $('#scroller');
        this.idTab = idTab;
        this.mode_view = 'visualicon';
        this._item = '';
        this.contenType = 'normal';
        this.model = model;
        this.context = context;
        //setto impostazioni in base al device
        this.device = DEVICE; //non usato per ora
		
		
		this.dimImg = 'dimX=400&dimY=560';
        this.dimImgSmista = 'dimX=350&dimY=490';
		
		
		
		if(DEVICE=='ipad'){
			this.dimImg = ''; //è il parametro aggiuntivo per la dimensione delle preview
            this.dimImgSmista = 'dimX=608&dimY=850';
		}
		
       

        //lancio funzioni
        this.setActions();
        this.update();

    },
	iPhoneGest: function () {
		if(DEVICE=='smart'){
			$('.iPhoneDouble').attr('colspan',2);
			$('.colspan7').attr('colspan',7);
		}else{
			$('.iPhoneDouble').attr('colspan',1);
			$('.colspan7').attr('colspan',1);
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
        $('#prev_doc').html('').removeClass('loaded');
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

		
		$('#paginatoreIphone').html('');
		$('#actionIphone').html('');
		
        if (!this.mode_view) this.mode_view = 'visualicon';

    },
	reposArrow: function (id){
		var newPos=($('#'+id).offset().left+($('#'+id).outerWidth()/2))-27;
		var newWidth=($('#'+id).offset().left)+$('#'+id).outerWidth()+15;
		$('.arrow_azioni').css({'right':'auto','left':newPos+'px'});
		$('.box_ricerche').css({'min-width':newWidth+'px'});

	},
    checkSession: function (data) {
        if (!data) {
            if (this.model.SessionExpired == true) {
                tooltip.init({ "titolo": 'La tua sessione &egrave; scaduta', 'confirm': 'Riconnetti', 'returntrue': function () { document.location.href = mobileclient.context.WAPath + "/Login/Login"; } });
                return false
            }
        }
        else {
            if (data.Errori) {
                tooltip.init({ "titolo": 'Errore di sistema', 'confirm': 'Riconnetti', 'returntrue': function () { document.location.href = mobileclient.context.WAPath + "/Login/Login"; } });
            }
            else if (data.TabModel.SessionExpired == true) {
                tooltip.init({ "titolo": 'La tua sessione &egrave; scaduta', 'confirm': 'Riconnetti', 'returntrue': function () { document.location.href = mobileclient.context.WAPath + "/Login/Login"; } });
                return false
            }
        }
    },
    setActions: function () {
        $('#header UL > LI').each(function (i, el) {
            if ($(el).attr('id') != 'LOGOUT') {
                $(el).click(function () {
                    mobileclient.showTab($(el).attr('id'));
                    $('#header UL > LI').removeClass('on');

                })
            }
            else if ($(el).attr('id') == 'LOGOUT') {

                var nome = mobileclient.model.DescrUtente
                if (mobileclient.model.DelegaEsercitata)
                    nome = mobileclient.model.DelegaEsercitata.Delegato

                $(el).click(function () { tooltip.init({ "titolo": nome + ' vuole uscire?', "content": '', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () { mobileclient.logout() } }) });
            }

        });
    },
    formatDate: function (data) {
        if (data.indexOf('-') == -1) {
            var data = eval(data.replace(/\/Date\((\d+)\)\//gi, "new Date($1)")) + ' ';
            data = data.substr(0, 15);
            data = data.split(' ');
            // 	Mon Jul 26 2010 11:19:21 

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

        page.loading(true);
        $.post(this.context.WAPath + "/Home/ShowTab", { "idTab": idTab }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');

    },
    update: function () {

        this.checkSession();
        this.reset();


        //console.log(dump(this.model));
        if (this.checkError()) {
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
                //alert("report");
                $('#AREA_DI_LAVORO').addClass('on');
                this.updateADL();
            }
            else if (this.model.TabShow.SMISTAMENTO) {
                $('#SMISTAMENTO').addClass('on');
                this.updateSmistamento();
            }


        }

    },
    checkError: function () {
        if ($(this.model.Errori).size() > 0) {

            tooltip.init({ "titolo": $(mobileclient.model.Errori)[0], 'confirm': 'ok', 'returntrue': function () { mobileclient.loadPrevious() } });

            return false;

        }
        else { return true; }

    },
    loadPrevious: function () {

        this.reset();
        this.model.TabModel = this.model.PreviousTabModel

        if (this.model.PreviousTabModel.DelegheRicevute) {
            this.showTab('LISTA_DELEGHE')

        }
        else if (this.model.PreviousTabModel.RicercaInAdl) {
            this.showTab('AREA_DI_LAVORO');

        }
        else if (!this.model.PreviousTabModel.Ricerche) {
            this.showTab('TODO_LIST');

        }
        else {
            this.showTab('RICERCA');

        }
    },
    updateToDoList: function () {
        //checking sulle deleghe assegnate per il popup
        if (this.model.NumDeleghe != 0 && mobileclient.mode_view != 'visualgrid') {
            tooltip.init({ "titolo": '', "content": '<div class="msgdeleghe">Ti sono state assegnate ' + this.model.NumDeleghe + ' deleghe<br>Vuoi controllarle?</div>', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () { mobileclient.showTab('LISTA_DELEGHE'); } });
        }

        //metto la classe in base al tipo di visualizzazione
        $('#item_list').attr('class', 'none ' + this.mode_view);


        //prima notifiche
        $('#nnotifiche').html(this.model.TabModel.NumElements);
        $('#nnotifiche').show();
		
		

        //poi aggiungo alla service_bar l'indietro se si tratta di un folder e metto il nome
        if (this.model.TabModel.IdParent) {
            var indietro = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.upFolder() });
            $('#service_bar').append(indietro);


            //dato che ci sono piazzo anche la folder name
            $('#service_bar').append($('<div id="name_onservicebar"></div>'));
            $('#name_onservicebar').html(this.model.TabModel.NomeParent);
        }
        else {

            //implemento la select dei ruoli
            var ruoli = $(this.model.Ruoli);
            $('#service_bar').addClass('skinGrey').append('<div id="filter_ruolo"><img class="info_utente infoUtenteP" src="' + this.context.WAPath + '/content/' + this.context.SkinFolder + '/img/' + DEVICE + '/info_utente.png" /><span class="iPhoneFilter" id="iPhoneFilter"></span><select></select></div>');

			$('#filter_ruolo > #iPhoneFilter').click(function(){
				if(DEVICE=='smart') mobileclient.skinSelect($('#filter_ruolo > SELECT').eq(0));
			});
			

            $('#filter_ruolo > SELECT').change(function () { mobileclient.cambiaRuolo($('#filter_ruolo > SELECT').val()) });
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
					var urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';
					
                    //controllo se siamo dentro un fascicolo così non mostro tutte le proprietà
                    if (!mobileclient.model.TabModel.IdParent) {
						$(info).append('<div class="onlyIphone data_tipo">' + el.DataDoc +' '+el.Ragione +'</div>');
						$(info).append('<div class="onlyIphone mittente">' + el.Mittente +'</div>');
                        $(info).append('<span class="noIphone"><span>Da: </span>' + el.Mittente + '<br/></span>');
                        $(info).append('<span class="noIphone"><span>Data: </span>' + el.DataDoc + '<br/></span>');
                        $(info).append('<span class="noIphone"><span>Ragione: </span>' + el.Ragione + '<br/></span>');
                    }

                    //controllo se è un fascicolo/folder così switcho tra oggetto e descrizione
                    if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) {$(info).append('<span class="noIphone"><span>Descrizione: </span>' + el.Oggetto + '<br/></span>');
						urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
					}
                    else {
                        $(info).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
                        if (el.Segnatura) {

                            $(info).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');
                        } else {
                            $(info).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');
                        }
                    }
					
					$(info).append('<div class="onlyIphone clear"></div>');

                    _item.append(ico);
					
					_item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="'+urlImg+'"/></div>');
					
                    _item.append(info);


                }
                else {
                    var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                    var info_1 = $('<div class="info_item1"></div>');

					var urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';
					
                    //controllo se siamo dentro un fascicolo così non mostro tutte le proprietà
                    if (!mobileclient.model.TabModel.IdParent) {
						$(info_1).append('<div class="onlyIphone data_tipo">' + el.DataDoc +' '+el.Ragione +'</div>');
						$(info_1).append('<div class="onlyIphone mittente">' + el.Mittente +'</div>');
                        $(info_1).append('<span class="noIphone"><span>Da: </span>' + el.Mittente + '<br/></span>');
                        $(info_1).append('<span class="noIphone"><span>Data: </span>' + el.DataDoc + '<br/></span>');
                        $(info_1).append('<span class="noIphone"><span>Ragione: </span>' + el.Ragione + '<br/></span>');
                        var info_2 = $('<div class="info_item2"></div>');
                    }
                    else var info_2 = $('<div class="info_item2 single"></div>');


                    //controllo se è un fascicolo/folder così switcho tra oggetto e descrizione
                    if (el.Tipo.FASCICOLO || el.Tipo.FOLDER) {$(info_2).append('<span class="noIphone"><span>Descrizione: </span>' + el.Oggetto + '<br/></span>');
					urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
					
					}
                    else {
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
					_item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="'+urlImg+'"/></div>');
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

				
				$(_item).find('.oggettoItem').click(function(){
					mobileclient._item = el;
					mobileclient.control_info()
				});
				 
				 $(_item).find('.apri').click(function(){
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
    },
    setitemposition: function (div) {
        //funzione di allineamento delle altezze non ricordo xk l'ho chiamata così:)
        var item = $(div + ' > .item');
		item.css({'height':'auto'});
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

      // if(DEVICE=='ipad'){document.removeEventListener("touchmove", preventBehavior, false);}
       

       
        imageZoomed = $('<div id="image_zoomed"><div class="green_bar"><div class="pulsante_classic fll  paddingLeft"><a href="javascript:;" onclick="mobileclient.closezoom()">Indietro</a></div></div><div id="zoomWrapper"><div id="zoomScroller"><img src="' + this.context.WAPath + '/Documento/Preview/' + IdDoc + '" width="' + width + '" /></div></div></div>');

        $('body').append(imageZoomed);

		
		//if ($.browser.webkit) zoomScroll = new iScroll('zoomScroller', {desktopCompatibility:true });
		//zoomScroll=$('#zoomScroller').niceScroll();
		
		
		checkHeightZoom();
		
        $('#image_zoomed').doubletap(
				function (event) {
				    mobileclient.closezoom();
				},
				function (event) {

				},
				400
			);

    },
    closezoom: function () {
		if(DEVICE=='ipad'){
			//document.addEventListener("touchmove", preventBehavior, false); 
			}


        $('#image_zoomed').remove();
    },
    initPaginatore: function () {

        if (this.model.TabShow.TODO_LIST) {
            var URL = '/ToDoList/ChangePage'
        } else if (this.model.TabShow.AREA_DI_LAVORO) {

            var URL = '/Adl/ChangePage';
        } else {
            var URL = '/Ricerca/ChangePage';
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


        if (this.model.TabModel.CurrentPage != 1 && this.model.TabModel.NumTotPages != 0) {
            if (this.model.TabModel.CurrentPage > 5) {
                $('#paginatore > .cont > .rrw').removeClass('hide');
                $('#paginatore > .cont > .rrw').click(function () { mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage - (4 + mobileclient.model.TabModel.CurrentPage % 5)) });
            }
            $('#paginatore > .cont > .rw,#paginatoreIphone > .cont > .rw').removeClass('hide')
            $('#paginatore > .cont > .rw,#paginatoreIphone > .cont > .rw').click(function () { mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage - 1) });
        }
		
		 $('#paginatoreIphone > .cont > p').text(this.model.TabModel.CurrentPage + ' di ' + this.model.TabModel.NumTotPages);

        if (this.model.TabModel.CurrentPage != this.model.TabModel.NumTotPages) {
            $('#paginatore > .cont > .fw,#paginatoreIphone > .cont > .fw').removeClass('hide')
            $('#paginatore > .cont > .fw,#paginatoreIphone > .cont > .fw').click(function () { mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage + 1) });
            if (this.model.TabModel.CurrentPage + 5 <= this.model.TabModel.NumTotPages) {
                $('#paginatore > .cont > .ffw').removeClass('hide')
                $('#paginatore > .cont > .ffw').click(function () { mobileclient.getPage(URL, mobileclient.model.TabModel.CurrentPage + (6 - mobileclient.model.TabModel.CurrentPage % 5)) });
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
					
					if($('#sezione_documento').find('.filter_trasm').size()==0){
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
                    $('#sezione_acttrasm').append($('<div id="sezione_acttrasm_left" class="side left absText"> <h3>NOTE GENERALI DI TRASMISSIONE</h3><p><textarea></textarea></p></div> <div id="sezione_acttrasm_right" class="side" style="padding-left: 10px;"><br><select class="noIphone"></select><br><div class="pulsante_classic flr  noIphone"><a href="javascript:;">Annulla</a></div><div class="pulsante_classic flr" id="transmAbs"><a href="javascript:;">Trasmetti</a></div></div>'));
					
                    $(data.TabModel.ModelliTrasm).each(function (i, el) { $('#sezione_acttrasm_right > SELECT').append('<option id="' + el.Id + '" value="' + el.Id + '">' + el.Codice + '</option>') })


                    $($('#sezione_acttrasm_right > .pulsante_classic')[0]).click(function () { mobileclient.control_info() });

					
                    if (hideTran) {
						$('#sezione_documento > .filter_trasm > .puls_blue').addClass('disabled');	
                        $($('#sezione_acttrasm_right > .pulsante_classic')[1]).addClass('hide')
                    }

                    if ($('#sezione_acttrasm_right > SELECT > option').length != 0) {$($('#sezione_acttrasm_right > .pulsante_classic')[1]).click(function () { mobileclient.eseguiTrasm(data, $('#sezione_acttrasm_right > SELECT').val(), $('#sezione_acttrasm_right').parent().find('#sezione_acttrasm_left > p > TEXTAREA').val()) })


						$('#sezione_documento > .filter_trasm > .puls_blue').eq(0).click(function () { $($('#sezione_acttrasm_right > .pulsante_classic')[1]).click(); });

}
					var value=$('#sezione_acttrasm_right > SELECT').eq(0).val();
					var newVal=$('#sezione_acttrasm_right > SELECT').eq(0).find('option[value='+value+']').text();
					$('#iPhoneFilterT').html(newVal);
					$('#iPhoneFilterT').click(function(){
							isTransmit=true;
							if(DEVICE=='smart') mobileclient.skinSelect($('#sezione_acttrasm_right > SELECT').eq(0));
						});	
	                }
                else {
                    mobileclient.getTrasmission(mobileclient._item)
                }
	
	
	},
	
    control_info: function (data) {
	

	
        $('.control_info').toggleClass('premuto')
        $('.control_azioni').removeClass('premuto')
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
			if(mobileclient.model.TabShow.RICERCA){
				$('#paginatoreIphone').addClass('showIphone').removeClass('hideIphone');
				$('#service_bar').removeClass('hideIphone').addClass('skinGrey').addClass('hauto');
				$('#service_bar .pulsante_classic.onlyIphone').remove();
				mobileclient.ricerca_filtri(true)				
				
			}
            $('#sezione_acttrasm').remove();
            //poi chiudo
			
			
			
			if(DEVICE=='smart'){
			 	$('#info_trasm').css({ 'display': 'none','height':'0px' });
				
				
			 }else{
			 	$('#info_trasm').animate({
	                height: '0px'
	            }, 300, function () {
	                 $('#info_trasm').css({ 'display': 'none' });
	            })
			 }
        }
        else {
            $('#sezione_trasmissione').addClass('none');
            $('#sezione_documento').addClass('hide');
			 
			 $('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
			$('#service_bar').addClass('hideIphone').removeClass('showIphone');
			$('#actionIphone').removeClass('hideIphone').addClass('showIphone');
			
			 
			 if(DEVICE=='smart'){
			 	$('#info_trasm').css({ 'display': 'block','height':'100%' })
				 mobileclient.addData(data)
				 $('#wrapper').scrollTop(0,200);
	         
			 }else{
			 	$('#info_trasm').css({ 'display': 'block' });
				  $('#info_trasm').animate({
	                height: '460px'
	            }, 300, function () {
					mobileclient.addData(data)
	              	$('#wrapper').scrollTop(0,200);
	            })
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
                if (!this.model.PreviousTabModel.Ricerche) { //apro il doc dalla todolist
                    if (this.model.TabModel.DocInfo.IdDocPrincipale == null) {
                        if (this.model.TabModel.TrasmInfo != null) { //Trasminfo è presente, quindi sto nel dettaglio
                            if (this.model.TabModel.TrasmInfo.HasWorkflow) //E' presente il workflow,
                            {
                                $('#box_azioni > UL').append('<li>Accetta</li><li>Accetta e Trasmetti</li><li>Accetta e ADL</li><li>Rifiuta</li>')
                            } else {    //non è presente il workflow
                                $('#box_azioni > UL').append('<li>Trasmetti</li><li>Inserisci in ADL</li>')
                            }
                        } else {

                            $('#box_azioni > UL').append('<li>Trasmetti</li>')
                        }
                    }
                    if (this.model.TabModel.DocInfo.IsAcquisito) {//Il documento è Aquisito Faccio apparire scarica 
                        //Scarica ultima opzione sempre presente in caso di IsAcquisito
                        $('#box_azioni > UL').append('<li>Scarica</li>')

                        //Se è presente la preview (solo per file PDF allora abilitalo zoom)
                        if (this.model.TabModel.DocInfo.HasPreview)
                            $('#box_azioni > UL').append('<li>Zoom</li>')
                    }

                }
                else if (this.model.PreviousTabModel.Ricerche) {
                    //apro il doc dalla ricerca
                    if (this.model.TabModel.DocInfo.IdDocPrincipale == null) {
                        $('#box_azioni > UL').append('<li>Trasmetti</li>')
                    }
                    if (this.model.TabModel.DocInfo.IsAcquisito) {
                        $('#box_azioni > UL').append('<li>Scarica</li>')
                        if (this.model.TabModel.DocInfo.HasPreview) {
                            $('#box_azioni > UL').append('<li>Zoom</li>')
                        }
                    }
                }
            }
            else if (this.model.TabShow.TODO_LIST) {
                //siamo in todolist
                if (!this.model.TabModel.IdParent) {
                    //siamo nel primo livello NON dentro un fascicolo
                    if (mobileclient._item.TrasmInfo != null) {   //ho un trasminfo vuoldeire che ho fatto apri o ho preso i dettagli del doc
                        if (mobileclient._item.HasWorkflow) {//E' presente il workflow,
                            $('#box_azioni > UL').append('<li>Accetta</li><li>Accetta e Trasmetti</li><li>Accetta e ADL</li><li>Rifiuta</li>')
                        } else {    //non è presente il workflow
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
                        $(el).click(function () { mobileclient.popAccettaTrasm(this) })
                        break;
                    case 'Rifiuta':
                        //accetta
                        $(el).click(function () { mobileclient.popRifiutaTrasm(this) })
                        break;
                    case 'Zoom':
                        //zoom
                        $(el).click(function () { mobileclient.zoomDocument(mobileclient.model.TabModel.DocInfo.IdDoc) })
                        break;
                    case 'Scarica':
                        //scarica
                        $(el).click(function () { window.open(mobileclient.context.WAPath + '/Documento/File/' + mobileclient.model.TabModel.DocInfo.IdDoc) })
                        break;
                    case 'Apri':
                        //apri
                        $(el).click(function () { mobileclient.getItem(mobileclient._item) })
                        break;
                    case 'Trasmetti':
                        if (mobileclient.model.TabShow.DETTAGLIO_DOC) {
                            //trasmetto dall'anteprima di un doc	
                            $(el).click(function () { mobileclient.trasmissioneForm(mobileclient.model.TabModel.DocInfo, mobileclient.model.TabModel.DocInfo.IdDoc, "") })
                        }
                        else {
                            //trasmetto da ricerca oppure todolist
                            $(el).click(function () { mobileclient.trasmissioneForm(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, "") })
                        }
                        break;
                    case 'Accetta e Trasmetti':
		                if (mobileclient._item) {
                            var idtr = mobileclient._item.IdTrasm
                        }
                        else {
                            var idtr = this.model.TabModel.IdTrasm
                        }

                        if (mobileclient.model.TabShow.DETTAGLIO_DOC) {
                            //trasmetto dall'anteprima di un doc	
                            $(el).click(function () { mobileclient.trasmissioneForm(mobileclient.model.TabModel.DocInfo, mobileclient.model.TabModel.DocInfo.IdDoc, idtr) })
                        }
                        else {
                            //trasmetto da ricerca oppure todolist
                            $(el).click(function () { mobileclient.trasmissioneForm(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, idtr) })
                        }
                        break;
                    case 'Accetta e ADL':
                        $(el).click(function () { mobileclient.popAccettaTrasm(this) })
                        break;

                    case 'Inserisci in ADL':
                        $(el).click(function () { mobileclient.inserisciInAdl(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, mobileclient._item.TipoProto) })

                        break;
                    case 'Rimuovi da ADL':
                        $(el).click(function () { mobileclient.rimuoviDaAdl(mobileclient._item.Tipo.DOCUMENTO, mobileclient._item.Id, mobileclient._item.TipoProto) })
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
        $('#info_trasm').css({ 'display': 'none' });
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
			if(this.model.TabShow.RICERCA)	this.dettaglioDoc(_item.Id);
			else this.dettaglioDoc(_item.Id, _item.IdTrasm);
			
		} else {
			if(this.model.TabShow.RICERCA)	this.inRicFolder(_item.Id, _item.Oggetto)
			else this.inFolder(_item.Id, _item.Oggetto, _item.IdTrasm)
			
		}
	
    },
    getItem: function (_item) {

        if (_item.Tipo.DOCUMENTO) {
            if ((this.model.TabShow.RICERCA) || (this.model.TabShow.AREA_DI_LAVORO)) this.dettaglioDoc(_item.Id);
            else this.dettaglioDoc(_item.Id, _item.IdTrasm);

        } else {

            if (this.model.TabShow.RICERCA) this.inRicFolder(_item.Id, _item.Oggetto)
            else if (this.model.TabShow.AREA_DI_LAVORO) this.inAdlFolder(_item.Id, _item.Oggetto)
            else this.inFolder(_item.Id, _item.Oggetto, _item.IdTrasm)

        }
    },
    updateDettaglioDoc: function () {

		
		
		
		
	
        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();


        var docinfo = this.model.TabModel.DocInfo;
        var infotrasm = this.model.TabModel.TrasmInfo;

		
        var indietro = $('<div class="pulsante_classic fll  paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () {
            mobileclient.loadPrevious()
        })
        if (this.model.PreviousTabName == 'DETTAGLIO_DOC') {
            indietro = $('<div></div>')
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
			var testo='Al documento principale';
			if(DEVICE=='smart'){
				testo='Indietro';
			}
            indietro = $('<div class="pulsante_classic fll  paddingLeft" id="docPrincipale"><a href="javascript:;">'+testo+'</a></div>').click(function () {
                mobileclient.dettaglioDoc(docinfo.IdDocPrincipale)
            })
        }


        $('#service_bar').append(indietro);

        $('#service_bar').append('<div id="todolist_control" style="width:20%" ></div>');
		
		
		
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
        if (allegati1.length > 1) {
            $('#name_onservicebar').html('<span>Anteprima del documento - </span><select></select>');
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
                $("select option:selected").each(function () { mobileclient.dettaglioDoc(this.id); });
            }

            )
        } else {
            $('#name_onservicebar').html('<span>Anteprima del documento</span>');
        }
		
		$('#footer_bar > .cont').append($('<ul><li class="first"><a href="" target="_blank">Scarica Documento</a></li><li><a href="javascript:;">Zoom +\-</a></li></ul><div class="clear"><br class="clear" /></div>'));

        $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/ajax-loader.gif)' });
        $('#prev_doc').show().removeClass('loaded');
	
        //console.log($('#prev_doc').css('backgroundImage'));

        if (docinfo.HasPreview == true) {
            //se ho la preview mostro il documento

            var img = new Image();

            $(img).attr('src', this.context.WAPath + '/Documento/Preview/' + docinfo.IdDoc + '?' + mobileclient.dimImg).load(function () {

                $('#prev_doc').css({ 'backgroundImage': 'url(' + mobileclient.context.WAPath + '/Documento/Preview/' + docinfo.IdDoc + '?' + mobileclient.dimImg + ')' }).addClass('loaded');

				

            });

            //buildo il paginatore
            $('#paginatore').append('<div class="bg">&nbsp;</div>');
            $('#paginatore').append('<div class="cont doc"></div>');


            $('#paginatore > .cont').append('<img class="gra0" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
            $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
            $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');
            $('#paginatore > .cont').append('<img class="gra0 off" src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/paginatore_pallino.png"/>');

            $('#paginatore > .cont > img,#paginatoreIphone > .cont > img').each(function (i, el) {
                $(el).click(function () { mobileclient.changePreview(i); });

            })
            $('#paginatore').show();
			//$('#paginatoreIphone').addClass('showIphone').removeClass('hideIphone');
			
			$($('#footer_bar > div > ul > li > a')[1]).click(function () { mobileclient.zoomDocument(mobileclient.model.TabModel.DocInfo.IdDoc) });
			
			
        }
        else {
		
			 $($('#footer_bar > div > ul > li')[1]).addClass('disabled');
		
            //altrimenti mostro il file no_preview non metto link e disattivo graficamente lo zoom.
			if(DEVICE!='smart'){
				mobileclient.control_info();
			}
			
       			
            if (docinfo.IsAcquisito) {
                $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/' + DEVICE + '/no_preview.jpg)' }).addClass('loaded');
                $('#prev_doc').append('<div id="link_doc"><p>Per vedere il documento <br />clicca il pulsante</p><div class="pulsante"><a href="" target="_blank">Scarica Documento</a></div></div>');
                $('#link_doc > div > a')[0].href = this.context.WAPath + '/Documento/File/' + docinfo.IdDoc;
                $('#link_doc').show();
				
            }
            else {
                $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/img/' + DEVICE + '/no_acquisito.jpg)' }).addClass('loaded');;

				
            }
        }
		
		if(docinfo.IsAcquisito){
			$('#footer_bar > div > ul > li > a')[0].href = this.context.WAPath + '/Documento/File/' + docinfo.IdDoc;
			$('#footer_bar').addClass('showIphone').removeClass('hideIphone');
		}else{
			$('#footer_bar').removeClass('showIphone').addClass('hideIphone');
		}
		

    },
    changePreview: function (i) {
        $('#paginatore > .cont > img,#paginatoreIphone > .cont > img').addClass('off');
        $($('#paginatore > .cont > img,#paginatoreIphone > .cont > img')[i]).removeClass('off');
        $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/ajax-loader.gif)' }).removeClass('loaded');
        if (i == 0) {
            $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Documento/Preview/' + this.model.TabModel.DocInfo.IdDoc + '?' + mobileclient.dimImg + ')' }).addClass('loaded');;
        }
        else {
            ind = Number(i) + 1;
            var img = $('<img src="' + this.context.WAPath + '/Documento/Preview/' + this.model.TabModel.DocInfo.IdDoc + '?page=' + ind + '&' + mobileclient.dimImg + '" />');
            img.load(function () {

                $('#prev_doc').css({ 'backgroundImage': 'url(' + $(img).attr('src') + ')' });
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
            var indietro = $('<div class="pulsante_classic fll  paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.upRicFolder() });
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
				var urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';
				
                //controllo se fascicolo/folder o documento per mostrare oggetto o descrizioni
                if (el.Tipo.FASCICOLO || el.Tipo.FOLDER){ $(info).append('<span>Descrizione: </span>' + el.Oggetto + '<br/>');
					urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
}
                else {
                    $(info).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
                    if (el.Segnatura) {

                        $(info).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');
						
						
						
                    } else {
                        $(info).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');
						
					
                    }
                }

                _item.append(ico);
				
				_item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="'+urlImg+'"/></div>');
				
                _item.append(info);

            }
            else {

                var ico = $('<div class="icon ' + icateg + '">&nbsp;</div>');
                //var info_1 = $('<div class="info_item1"></div>');
                var info_2 = $('<div class="info_item2"></div>');

				var urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_file.gif';
				
                if (el.Tipo.FASCICOLO || el.Tipo.FOLDER){ $(info_2).append('<span class="noIphone"><span>Descrizione: </span>' + el.Oggetto + '<br/></span>');
					urlImg=mobileclient.context.WAPath + '/content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_folder.gif';
}
                else {
                    $(info_2).append('<span class="noIphone"><span>Oggetto: </span>' + el.Oggetto + '<br/></span>');
					
					
                    if (el.Segnatura) {
						
                        $(info_2).append('<span class="noIphone"><span>Segnatura: </span>' + el.Segnatura + '<br/></span>');
                    } else {
                        $(info_2).append('<span class="noIphone"><span>Id: </span>' + el.Id + '<br/></span>');
						
                    }
                }
                _item.append(ico);
                //_item.append(info_1);
				
				_item.append('<div class="onlyIphone oggettoItem">' + el.Oggetto + '</div><div class="onlyIphone apri"><img src="'+urlImg+'"/></div>');
				
                _item.append(info_2);
                _item.append($('<div class="clear"><br class="clear"/></div>'));

            }
            $(_item).find('.icon').doubletap(
            //funzione del doppio tap
				function (event) {
				    mobileclient._item = el;
				    mobileclient.getItem(el)
				},
            /** funzione del tap singolo*/
				function (event) {
				    mobileclient._item = el;
				    mobileclient.chooseDoc(_item);
				},
            /** tempo di timeout sul quale fare il controllo */
				400
			);

			$(_item).find('.oggettoItem').click(function(){
				mobileclient._item = el;
				mobileclient.control_info()
			});
			 
			 $(_item).find('.apri').click(function(){
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
    updateADL: function () {

        //prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();

        //metto la classe in base al tipo di visualizzazione
        $('#ricerca_item > .item_list').attr('class', 'item_list none ' + this.mode_view);

        if (this.model.TabModel.IdParent) {
            var indietro = $('<div class="pulsante_classic fll  paddingLeft"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.upAdlFolder() });
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
                }

                _item.append(ico);
                //_item.append(info_1);
                _item.append(info_2);
                _item.append($('<div class="clear"><br class="clear"/></div>'));

            }
            $(_item).find('.icon').doubletap(
            //funzione del doppio tap
				function (event) {
				    mobileclient._item = el;
				    mobileclient.getItem(el)
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
		
		var valueFilter='<div class="filter_ricerca"><h2 class="onlyIphone">Ricerca Rapida</h2><span class="iPhoneFilter search" id="iPhoneFilterS"></span><select class="onlyIphone"></select><div class="puls_cerca onlyIphone">Cerca</div><ul><li class="noGalaxy" id="RIC_DOCUMENTO">Documenti</li><li  class="noGalaxy" id="RIC_FASCICOLO">Fascicoli</li><li class="on noIpad firstGalaxy" id="RIC_LIBERA">Mostra</li><li id="RIC_SALVATE">Preferite</li></ul></div>';


		if(isPrepend){
			$('#service_bar').addClass('skinGrey').addClass('hauto').find('.scelta_ricerca').after(valueFilter);
		
		}else{
		
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
			
			
			$('.filter_ricerca > #iPhoneFilterS').click(function(){
				if(DEVICE=='smart') mobileclient.skinSelect($('.filter_ricerca > SELECT').eq(0));
			});
			
			
			
			
        $('.filter_ricerca > SELECT').change(function () {
			var type=$($(this).children()[this.selectedIndex]).attr('rel');
		});
		
		
		
		
		
			var ricerche = $(mobileclient.model.TabModel.Ricerche);
			
			if(ricerche[0]){
				if (ricerche[0].Type.RIC_DOCUMENTO) { var tipo = 'RIC_DOCUMENTO'; var sigla = 'D'; }
	             else if (ricerche[0].Type.RIC_FASCICOLO) { var tipo = 'RIC_FASCICOLO'; var sigla = 'F'; }
				 
				var testoDefault=sigla+' '+ricerche[0].Descrizione;
				$('#iPhoneFilterS').text(testoDefault);
			
			}else{
				$('#iPhoneFilterS').remove();
			
			}
			
             ricerche.each(function (i, el) {
				
			 	  if (el.Type.RIC_DOCUMENTO) { var tipo = 'RIC_DOCUMENTO'; var sigla = 'D'; }
                  else if (el.Type.RIC_FASCICOLO) { var tipo = 'RIC_FASCICOLO'; var sigla = 'F'; }
			 
                if (mobileclient.model.TabModel.IdRicercaSalvata == el.Id) {
					selected = 'selected="selected"';
					$('#iPhoneFilterS').text(sigla+ ' '+el.Descrizione);
				}
                else selected = '';

                var descrizione = el.Descrizione;
                
                $('.filter_ricerca > SELECT').append($('<option id="' + el.Id + '" value="' + el.Id + '|' + tipo + '" '+selected+'>'+sigla +' '+ el.Descrizione + '</option>'));
		
            });	
			
		
		if($('.filter_ricerca > SELECT').children().length!=0){
				$('#trasmissione_doc > .filter_trasm > .puls_cerca').removeClass('disabled');		

				$('.filter_ricerca > .puls_cerca').click(function(){
				if ($('.filter_ricerca').children('select').val()){
					var value = $('.filter_ricerca').children('select').val().split('|');
				}
				else{
					var value = $('.filter_ricerca > div > select').val().split('|');
				}
					
					
					mobileclient.filtroRicerca(value[0],value[1]);																											
				});				
			}else{
				$('#trasmissione_doc > .filter_trasm > .puls_cerca').addClass('disabled');	
			}	
		

        //carico metto funzioni a tutti gli elementi e faccio le varie casistiche
        //parte comune per tutti, solo all'interno aggiungerò un IF per RIC_LIBERA presente solo su galaxy in verticale
        $('#service_bar > .filter_ricerca > UL > LI').each(function (i, el) {


            $(el).click(function () {
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
                            if (el.Type.RIC_DOCUMENTO) { var tipo = 'RIC_DOCUMENTO'; var sigla = 'D'; }
                            else if (el.Type.RIC_FASCICOLO) { var tipo = 'RIC_FASCICOLO'; var sigla = 'F'; }

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
        if (DEVICE=='ipad' || mobileclient.contenType != 'normal') {
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
    adl_filtri: function () {
        $('.filter_ricerca').remove();
		$('#service_bar').removeClass('skinGrey');
		
		 $('#service_bar').append('<div class="filter_ricerca"><ul><li class="noGalaxy" id="RIC_DOCUMENTO_ADL">Documenti</li><li class="noGalaxy  lastIpad" id="RIC_FASCICOLO_ADL">Fascicoli</li><li class="on noIpad firstGalaxy" id="RIC_LIBERA">Mostra</li></ul></div>');

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

                if (DEVICE=='ipad' || mobileclient.contenType != 'normal') {
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
        if (DEVICE=='ipad' || mobileclient.contenType != 'normal') {
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
        this.checkSession(data);
		$('#actionIphone').html('');	
		
		
        var fascinfo = data.TabModel.FascInfo;
        var docinfo = data.TabModel.DocInfo;
        var infotrasm = data.TabModel.TrasmInfo;

		
        if (infotrasm) {//se non è un documento carico le info di trasmissione
            $($('.dett_trasm > table > tbody > tr > td')[0]).html(data.TabModel.TrasmInfo.Mittente)
            $($('.dett_trasm > table > tbody > tr > td')[1]).html(data.TabModel.TrasmInfo.Ragione)
            $($('.dett_trasm > table > tbody > tr > td')[2]).html(this.formatDate(data.TabModel.TrasmInfo.Data))

            $('#sezione_trasmissione > P').html(data.TabModel.TrasmInfo.NoteGenerali)
            $('#sezione_trasmissione').removeClass('none').removeClass('hide');

            //Aggiorno l'item con i dati ricevuti
            this._item.HasWorkflow = data.TabModel.TrasmInfo.HasWorkflow;
            this._item.TrasmInfo = data.TabModel.TrasmInfo;

			if(this._item.HasWorkflow == true){
				var mesi = new Array('Gennaio','Febbraio','Marzo',
					'Aprile','Maggio','Giugno','Luglio','Agosto','Settembre',
					'Ottobre','Novembre','Dicembre');
				var date = new Date();
				var gg  = date.getDate();
				var mese = date.getMonth();
				var yy = date.getYear();
				var yyyy = (yy < 1000) ? yy + 1900 : yy;
				
				var mess = 'in data '+gg + " " + mesi[mese] + " " + yyyy+' da '+this.model.DescrUtente+' dal dispositivo mobile.';
				
				var rifiuta = $('<div class="pulsante_classic fll paddingLeft"><a href="javascript:;">Rifiuta</a></div>').click(function () {
					tooltip.init({"titolo" : 'Inserisci note di rifiuto', "content": $('<textarea></textarea').html('Rifiutata '+mess), 'deny': 'Annulla', 'confirm': 'Rifiuta', 'returntrue': function(){mobileclient.rifiutaTrasm()}});
																															  });
				var accetta = $('<div class="pulsante_classic fll"><a href="javascript:;">Accetta</a></div>').click(function () {
					tooltip.init({"titolo" : 'Inserisci note di accettazione', "content": $('<textarea></textarea').html('Accettata '+mess), 'deny': 'Annulla', 'confirm': 'Accetta', 'returntrue': function(){mobileclient.accettaTrasm()}});
																														  });
				//var trasmetti = $('<div class="pulsante_classic fll"><a href="javascript:;">Trasmetti</a></div>').click(function () { mobileclient.trasmissioneForm(docinfo,fascinfo) });
				
				
				$('#actionIphone').append(rifiuta);
				$('#actionIphone').append(accetta);
				$('#actionIphone').removeClass('hideIphone').addClass('showIphone');
			
				//$('#actionIphone').append(trasmetti);

			}


        }
        else {
		var indietro = $('<div class="pulsante_classic fll paddingLeft onlyIphone"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.showTab('RICERCA') });
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
        }
        $('#sezione_documento').removeClass('hide');
		
		if (docinfo) {
				var visualizza = $('<div class="onlyIphone paddingRight control_info2 premuto"></div>').click(function () {
		
					
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
        $('#sezione_acttrasm').remove();
        $('#note_aggiuntive').html('');
        $('.box_ricerche').remove();

        $('#service_bar > form').addClass('hideService');
        $('#service_bar > div').addClass('hideService');
        if (!data.TabModel.DocInfo) var name = 'fascicolo';
        else var name = 'documento';
        $('#service_bar').append('<div id="name_onservicebarT">Trasmissione ' + name + '</div>');

        var indietro = $('<div class="pulsante_classic fll  paddingLeft noIphone" id="indietro_trasm"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.control_info() });
        $('#service_bar').append(indietro);

        this.control_azioni();
        this.control_info(data);


    },
    popAccettaTrasm: function (el) {
        this.pop(el);
        $('.pop > H4').html('Inserisci note di accettazione')
        $('.pop > TEXTAREA').html('Accettata ' + $('.pop > TEXTAREA').html());
        $('.pop > BUTTON').html('Accetta')
        var adl = null;
        if (el.innerText == "Accetta e ADL")
            adl = el.innerText

        $('.pop > BUTTON').click(function () { mobileclient.accettaTrasm(adl) })
    },
    popRifiutaTrasm: function (el) {
        this.pop(el);
        $('.pop > H4').html('Inserisci note di rifiuto')
        $('.pop > TEXTAREA').html('Rifiutata ' + $('.pop > TEXTAREA').html());
        $('.pop > BUTTON').html('Rifiuta')

        $('.pop > BUTTON').click(function () { mobileclient.rifiutaTrasm() })
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
        ;

    },
    pop: function (el, msg) {

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

        var mess = 'in data ' + gg + " " + mesi[mese] + " " + yyyy + ' da ' + this.model.DescrUtente + ' dal dispositivo mobile'; // + mobileclient.device;

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
        $(this.model.Ruoli).each(function (i, el) {
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
				var dove=$('.button_action').eq(0).offset().top-$('#header').height();
				 if (DEVICE!='smart')   $('#wrapper').scrollTop(dove,200); 
				
				//$('#ricercadelegato').focus();

                //dopo aver acceso il pulsante per la delega gli metto anche la funzione
                $('#deleghe').find('.pulsante_classic_big').unbind('click');
                $('#deleghe').find('.pulsante_classic_big').click(function () {


	
				
                    if (!$('#ricercadelegato').attr('valore') || !$('#ricercaruolo').attr('valore') ||  ($("#dataInizioD").val()=='') || ($("#dataFineD").val()=='')) {
                        tooltip.init({ "titolo": "tutti i campi sono obbligatori", 'confirm': 'ok' });

                    }
                    else {


                        var dataInizio = new Date($("#dataInizioD").val());
                        var dataFine =  new Date($("#dataFineD").val());
                        var dataOggi = new Date();
						

                        if (dataFine < dataInizio) {
                            tooltip.init({ "titolo": "data fine deve essere successiva a data inizio", 'confirm': 'ok' });
                        }
                        else if (dataInizio < dataOggi) {
                            tooltip.init({ "titolo": "data inizio deve essere successiva a data di oggi", 'confirm': 'ok' });
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

        $('#deleghe').append($('<div class="button_action">&nbsp;</div>').click(function () { }));


        //carico la form da compilare per una nuova delega
        //intanto piazzo la classe off, che toglierò quando clicco su un ruolo in alto

        var infodeleghe2 = $("<div class='info_deleghe off noBG'></div>");

        infodeleghe2.append("<div class='spalla noIphone'></div>");
        infodeleghe2.append("<div class='contenuto iphoneVis'></div>");
        infodeleghe2.append('<div class="clear"><br class="clear"/></div>');

        $(infodeleghe2).find('.spalla').append("<div class='imgBig'>&nbsp;</div>");
        $(infodeleghe2).find('.spalla').append("<p></p>");
        $(infodeleghe2).find('.spalla').append("<p></p>");

		var today=new Date();
		var minD=today.toJSON().slice(0,10);
		var year=1000 * 60 * 60 *24 * 365*5;
		var maxD=new Date(today.getTime()+year).toJSON().slice(0,10);

		
        $(infodeleghe2).find('.contenuto').append('<div class="form_deleghe"> \
			<table> \
			<tr><th>Ruolo Attribuito:</th><td id="ricercaruolo"></td></tr>\
			<tr><th>Nome delegato:</th><td><div class="boxricercadelegato"><input type="search" readonly="true" id="ricercadelegato" /><div class="suggestion"></div></div></td></tr>\
			<tr><th>Data decorrenza:</th><td><input placeholder="gg/mm/aaaa" type="date" id="dataInizioD" readonly="true" min="'+minD+'" max="'+maxD+'"/></td></tr>\
			<tr><th>Data fine:</th><td><input type="date" placeholder="gg/mm/aaaa" id="dataFineD" readonly="true" min="'+minD+'" max="'+maxD+'"/></td></tr>\
			</table></div>');

       
        //attracco tutto
        $('#deleghe').append(infodeleghe2);
		

        $('#deleghe').append($('<div class="center"><input type="button" class="pulsante_classic_big center off" value="ATTIVA DELEGA" /></div>'));

        $('#deleghe').show();


       // $('#ricercadelegato').focus();
        var request = '';
		
		
        $('#ricercadelegato').keypress(function (e) {
		

	          var desc = $(this).attr('value');
			var key=(typeof event!='undefined')?window.event.keyCode:e.keyCode; 
			
			if ((desc.length >= 3)||(key==13)) {

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

            $('#service_bar').append('<div class="filter_ricerca deleghe"><ul><li class="noIphone">Tutte</li><li class="firstIphone paddingLeft">Ricevute</li><li>Assegnate</li></ul></div>');

			

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

            $(deleghe_ricevute).each(function (i, el) {
                var ruolo = el.RuoloDelegato;
                var decorrenza = mobileclient.formatDate(el.DataDecorrenza);
                var scadenza = mobileclient.formatDate(el.DataScadenza);
                var delegante = el.Delegante;

                $('#deleghe > .Dricevute > table').append('<tr><td class="noIphone">&nbsp;</td><td class="colspan7"><div class="inputradio" id="' + el.Id + '" value="' + el.Id + '|' + el.IdRuoloDelegante + '|' + el.CodiceDelegante + '"><label class="onlyIphone" for=""' + el.Id + '""><span class="codice">' + ruolo + '</span><span class="delegante">' + delegante + '</span></label></div></td><td class="noIphone">' + ruolo + '</td><td class="noIphone">' + decorrenza + '</td><td class="noIphone">' + scadenza + '</td><td class="noIphone">' + delegante + '</td><td>&nbsp;</td></tr>');


                $('.inputradio[value="' + el.Id + '|' + el.IdRuoloDelegante + '|' + el.CodiceDelegante + '"]').click(function () {
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
			
			mobileclient.iPhoneGest();
			
            $('#deleghe > .Dassegnate').append($('<div class="center"><input type="button" class="pulsante_classic_big revoca" value="REVOCA DELEGA" /></div>').click(function () {
                if ($('#deleghe > .Dassegnate > table').find("div.checked").size() > 0) {
                    var deleghe = new Array();

                    $('#deleghe > .Dassegnate > table').find("div.checked").each(function (i, el) {

                        var Id = $(el).attr('value').split('|')[0];
                        var DataDecorrenza = eval($(el).attr('value').split('|')[1].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                        var DataScadenza = eval($(el).attr('value').split('|')[2].replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
                        var delega = { "Id": Id, "DataDecorrenza": DataDecorrenza.toUTCString(), "DataScadenza": DataScadenza.toUTCString() };
                        deleghe.push(delega);
                    });



                    mobileclient.RevocaDeleghe(deleghe);
                }
            }))

			
            //accendo il tab scelto
            if (!mobileclient.tipodelega) mobileclient.tipodelega = 0;
			
			if ((!mobileclient.tipodelega)&&(DEVICE=='smart'))mobileclient.tipodelega = 1;
	
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
            $('#deleghe > .info_deleghe > .spalla').append("<p><strong>" + this.model.DelegaEsercitata.Delegato + "</strong><div class='onlyIphone'>Ruolo delegato:<strong>"+this.model.DelegaEsercitata.RuoloDelegato+"</strong></div></p>");

            $('#deleghe > .info_deleghe ').append("<div class='contenuto'><div class='testo'></div><div class='info_ruolo'></div></div>");


			$('#deleghe').append('<div class="tableDelega onlyIphone"><table><tr><th>Nome delegante:</th><td>' + this.model.DelegaEsercitata.Delegante + '</td></tr><tr><th>Decorrenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataDecorrenza) + '</td></tr><tr><th>Scadenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataScadenza) + '</td></tr></table></div>');
			
			$('#deleghe .tableDelega').append('<div class="pulsante_classic_big">Dismetti Delega</div>');

            $('#deleghe > .info_deleghe  > .contenuto > .testo').append("<p class='fll'><strong>Delega attiva:</strong> " + this.model.DelegaEsercitata.RuoloDelegato + " </p><div class=\"pulsante_classic flr\">DISMETTI</div>");
            $('#deleghe > .info_deleghe  > .contenuto > .testo').append('<div class="clear"><br class="clear"/></div>');

            $('#deleghe > .info_deleghe  > .contenuto > .info_ruolo').append('<table><tr><th>Data di decorrenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataDecorrenza) + '</td></tr><tr><th>Data di scadenza:</th><td>' + mobileclient.formatDate(this.model.DelegaEsercitata.DataScadenza) + '</td></tr><tr><th>Delegante:</th><td>' + this.model.DelegaEsercitata.Delegante + '</td></tr></table>');

            $('#deleghe > .info_deleghe').append("<div class='clear'><br class='clear'/></div>");


            $('#deleghe > .info_deleghe > .contenuto').find('.pulsante_classic').click(function () {
                tooltip.init({ "titolo": 'Stai per Dismettere una delega. Sei sicuro?', "content": '', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () { mobileclient.dismettiDelega(mobileclient.model.DelegaEsercitata.CodiceDelegante) } })
            })

			
			$('#deleghe > .tableDelega ').find('.pulsante_classic_big').click(function () {
                tooltip.init({ "titolo": 'Stai per Dismettere una delega. Sei sicuro?', "content": '', 'deny': '<span class="smallb">NO</span>', 'confirm': '<span class="smallb">SI</span>', 'returntrue': function () { mobileclient.dismettiDelega(mobileclient.model.DelegaEsercitata.CodiceDelegante) } })
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
            $('.control_azioni').toggleClass('premuto');

            if ($('.control_azioni').hasClass('premuto')) {
                $('#service_bar').append($('<div id="box_azioni"><img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/' + DEVICE + '/arrow_azioni.png" class="arrow_azioni"/><button class="annulla_button">Annulla</button><h3>Azioni</h3><ul><li>Zoom</li><li>Scarica</li><li class="nopadd">Agg. Nota Generale</li></ul></div>'));


                $($('#box_azioni > UL > LI')[0]).click(function () { mobileclient.zoomDocument(mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id) });
                $($('#box_azioni > UL > LI')[1]).click(function () { window.open(mobileclient.context.WAPath + '/Documento/File/' + mobileclient.model.TabModel.SmistamentoElements[mobileclient.documento_attuale].Id) })
                $($('#box_azioni > UL > LI')[2]).click(function () { mobileclient.smistamento_addNotaGen(this) });


                $('#box_azioni').find('.annulla_button').click(function () { $('.control_azioni').toggleClass('premuto'); $('#box_azioni').remove(); });
            }
            else {
                $('#box_azioni').remove();
            }


        });

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


        });

        //frecce paginatore
        $('#service_bar').removeClass('hideService');

        //parte superiore
        $('#smista_doc').append('<div id="lista_documenti"></div>');
        this.smistamento_loaddoc();

        //parte inferiore
        $('#smista_doc').append('<div id="smista_form"><div class="filtri"><select name="modello_trasm"><option value="none">Seleziona modello di tramissione</option></select> <input type="search" id="smistamento_ricerca" /><div class="suggestion"></div></div></div>');

        //popolo la select con i modelli di trasmissione
        $(this.model.TabModel.ModelliTrasm).each(function (i, el) {
            $('#smista_doc').find('select').append('<option value="' + el.Id + '">' + el.Codice + '</option>');
        });

        //attacco la funzione all'input di ricerca
        this.smistamento_ricerca();

        //metto la parte della ricerca

        $('#smista_form').append('<div class="label_tabella none"><p>La tua Ricerca</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');

        $('#smista_form').append('<ul class="items_ricerca"></ul>');



        $('#smista_form').append('<div class="label_tabella"><p>' + this.model.TabModel.Tree.UOAppartenenza.Descrizione + '</p><p>Conoscenza</p><p>Competenza</p><br clear="all" class="clear" /></div>');


        $('#smista_form').append('<ul class="items_elenco"><li class="uoa" val="' + this.model.TabModel.Tree.UOAppartenenza.Id + '"><div class="item"><p>' + this.model.TabModel.Tree.UOAppartenenza.Descrizione + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li></ul>');

        if (this.model.TabModel.Tree.UOAppartenenza.Ruoli) {
            $('#smista_form > ul.items_elenco > li').append('<ul></ul>');

            $(this.model.TabModel.Tree.UOAppartenenza.Ruoli).each(function (i, el) {
                li = $('<li class="ruolo" val="' + el.Id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden img_nota_ruolo" name="img_nota_ruolo"/><p>' + el.Descrizione + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');


                if (el.Utenti) {
                    ul = $('<ul></ul>');

                    $(el.Utenti).each(function (a, b) {
                        var li = $('<li class="persona" val="' + b.Id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" /><p>' + b.Descrizione + '</p> <div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                        ul.append(li);

                        li.find('img').click(function () {
                            mobileclient.smistamento_addnota(this);
                        })
                    });

                }
                li.append(ul);
                li.find('img').click(function () {
                    mobileclient.smistamento_addnota(this);
                })
                $('#smista_form > ul.items_elenco > li > ul').append(li);
            });
        }

        $('#smista_form').find('.inputcheckbox').click(function () { mobileclient.smistamento_selectoptions(this); });


        var smista = $('<div class="pulsante_classic_big">SMISTA</div>').click(function () {
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
        $('#smistamento_ricerca').keyup(function (e) {
			
            var desc = $(this).attr('value');
            if (desc.length > 3) {

                if (request) request.abort();

                $('.suggestion').show();

                $('.suggestion').html('');
                $('.suggestion').addClass('loading');

                request = $.post(mobileclient.context.WAPath + '/Smistamento/RicercaElementi', { "descrizione": desc }, function (data) {

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
                                $($('.label')[0]).after($('<li>' + el.DescrizioneUtente + '</li>').click(function () {
                                    mobileclient.smistamento_additem('persona', lista_id, el.DescrizioneUtente);
                                }));
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
        var elenco = $('#smista_form > UL.items_ricerca');

        if (elenco.find('[val="' + id + '"]').size() == 0) {
            if (type == "persona") {

                var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" class="img_hidden  img_nota_ruolo" class="img_hidden" /><p>' + value + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                li.find('img').click(function () { mobileclient.smistamento_addnota(this); })
            } else if (type == "ruolo") {

                var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><img src="' + mobileclient.context.WAPath + '/content/img/' + DEVICE + '/smistatore_nota_add.png" name="img_nota_ruolo" class="img_hidden  img_nota_ruolo" /><p>' + value + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><textarea class="none"></textarea><div class="clear"><br class="clear"/></div></div></li>');
                li.find('img').click(function () { mobileclient.smistamento_addnota(this); })
            }
            else {
                var li = $('<li class="' + type + '" val="' + id + '"><div class="item"><p>' + value + '</p><div class="inputcheckbox" name="conoscenza">&nbsp;</div> <div class="inputcheckbox" name="competenza">&nbsp;</div><div class="clear"><br class="clear"/></div></div></li>');
            }

            li.find('.inputcheckbox').click(function () { mobileclient.smistamento_selectoptions(this); });


            elenco.append(li);
            $('.suggestion').hide();
        }
        else {
            tooltip.init({ "titolo": "Voce già inserita", 'confirm': 'ok' });
        }

    },

    //Questa funzione Regola lo spegnimento /acccesione delle checkbox nel form smistamento
    smistamento_selectoptions: function (checkbox) {
        var name = $(checkbox).attr('name');
        var li = $(checkbox).parent().parent();
        tipo_li = li.attr('class');

        if (!$(li).parent().hasClass('items_ricerca')) {
            //se la checkbox è un elemento dell'albero sotto, faccio tutti i casi

            switch (tipo_li) {

                case 'persona':
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
        }
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
            var note = $('<div class="other"><table><tr><th>NOTE GENERALI</th><td>' + doc.NoteGenerali + '</td></tr></table><img name="previewdoc" class="pulsante previewdocP" src="' + this.context.WAPath + '/content/' + this.context.SkinFolder + '/img/' + DEVICE + '/smistatore_pulsante_visualizzadoc.png"/><textarea name="note_generali" class="none"></textarea></div>');

            note.find("[name='previewdoc']").click(function () {
                //versione light dello zoomdocument

                page.loading(true);

                imageZoomed = $('<div id="smistamento_preview"><div class="img"><img src="' + mobileclient.context.WAPath + '/Content/img/' + DEVICE + '/smistatore_closebutton.png" class="close closeButton" /></div>');

                var img = $('<img src="' + mobileclient.context.WAPath + '/Documento/Preview/' + doc.Id + '?' + mobileclient.dimImgSmista + '" />');
                img.load(function () {

                    $('body').append(imageZoomed);

                    $('#smistamento_preview > .img').css({ 'backgroundImage': 'url(' + mobileclient.context.WAPath + '/Documento/Preview/' + doc.Id + '?' + mobileclient.dimImgSmista + ')' });

                    $('#smistamento_preview > .img > .close').click(function () {
                        $('#smistamento_preview').remove();
                        page.loading(false);
                    });
                });
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
            $.post(this.context.WAPath + "/Fascicolo/DettaglioFascTrasm", { "idFasc": _item.Id, "idTrasm": _item.IdTrasm }, function (data) { mobileclient.updateDettaglioTrasm(data) }, 'json');
        } else {
            $.post(this.context.WAPath + "/Documento/DettaglioDocTrasm", { "idDoc": _item.Id, "idTrasm": _item.IdTrasm }, function (data) { mobileclient.updateDettaglioTrasm(data) }, 'json');

        }
    },
    getPage: function (URL, numPage) {
        page.loading(true);
        $.post(this.context.WAPath + URL, { "numPage": numPage }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    cambiaRuolo: function (idRuolo) {
        page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/ChangeRole", { "idRuolo": idRuolo }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    dettaglioDoc: function (idDoc, idTrasm) {
        page.loading(true);

		$('#paginatoreIphone').addClass('hideIphone').removeClass('showIphone');
        if (!idTrasm) var idTrasm = ''
        $.post(this.context.WAPath + "/Documento/DettaglioDoc", { "idDoc": idDoc, "idTrasm": idTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    upFolder: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/UpFolder", {}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    upRicFolder: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/UpFolder", {}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    upAdlFolder: function () {
        page.loading(true);
        $.post(this.context.WAPath + "/Adl/UpFolder", {}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    inFolder: function (idFasc, nameFasc, idTrasm) {
        page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc, "idTrasm": idTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    inRicFolder: function (idFasc, nameFasc) {
        page.loading(true);

        $.post(this.context.WAPath + "/Ricerca/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    inAdlFolder: function (idFasc, nameFasc) {
        page.loading(true);

        $.post(this.context.WAPath + "/Adl/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    ricerca: function (testo, tipoRic) {
        page.loading(true);
        //ric_documento|ric_fascicolo|ric_doc_fasc
        //alert(testo+'-'+tipoRic)
        //alert(tipoRic);
        if (tipoRic == null) tipoRic = "RIC_DOCUMENTO";
        $.post(this.context.WAPath + "/Ricerca/RicercaTesto", { "testo": testo, "tipoRic": tipoRic }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');

    },
    ricercaAdl: function (testo, tipoRic) {
        page.loading(true);
        //ric_documento|ric_fascicolo|ric_doc_fasc
        //alert(testo+'-'+tipoRic)
        //alert(tipoRic);
        if (tipoRic == null) tipoRic = "RIC_DOCUMENTO_ADL";
        $.post(this.context.WAPath + "/Adl/RicercaTesto", { "testo": testo, "tipoRic": tipoRic }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');

    },
    filtroRicerca: function (idRicSalvata, tipoRicSalvata) {
        page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/Ricerca", { "idRicSalvata": idRicSalvata, "tipoRicSalvata": tipoRicSalvata }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    filtroRicercaAdl: function (idRicSalvata, tipoRicSalvata) {
        page.loading(true);
        $.post(this.context.WAPath + "/Adl/Ricerca", { "idRicSalvata": idRicSalvata, "tipoRicSalvata": tipoRicSalvata }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
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
            $.post(this.context.WAPath + '/Trasmissione/AccettaTrasm', { idTrasm: id, note: $('.pop > TEXTAREA').val() }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        }
        else {
            $.post(this.context.WAPath + '/Trasmissione/AccettaTrasmInAdl', { idTrasm: id, note: $('.pop > TEXTAREA').val(), idDoc: idDoc, TipoProto: tipoProto }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
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
        $.post(this.context.WAPath + '/Trasmissione/RifiutaTrasm', { idTrasm: id, note: $('.pop > TEXTAREA').val() }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    trasmissioneForm: function (doc, id, idTrasm) {
		
		
        if (doc) {
            $.post(this.context.WAPath + "/Trasmissione/TrasmissioneFormDoc", { "idDoc": id, "idTrasmPerAcc": idTrasm }, function (data) { mobileclient.updateTrasmissione(data) }, 'json');
        }
        else {
            $.post(this.context.WAPath + "/Trasmissione/TrasmissioneFormFasc", { "idFasc": id, "idTrasmPerAcc": idTrasm }, function (data) { mobileclient.updateTrasmissione(data) }, 'json');
        }
    },

    inserisciInAdl: function (doc, id, tipoProto) {

        if (doc) {
            $.post(this.context.WAPath + '/Adl/AddDocInAdl', { "Id": id, "TipoProto": tipoProto }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        }
        else {
            $.post(this.context.WAPath + '/Adl/AddFascInAdl', { "Id": id, "TipoProto": tipoProto }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        }
    },
    rimuoviDaAdl: function (doc, id, tipoProto) {
        if (doc) {
            $.post(this.context.WAPath + '/Adl/RemoveDocFromAdl', { "Id": id, "TipoProto": tipoProto }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        } else {
            $.post(this.context.WAPath + '/Adl/RemoFascFromAdl', { "Id": id, "TipoProto": tipoProto }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        }
    },
    eseguiTrasm: function (_item, idTrasmModel, note) {
        page.loading(true);
        var hasTrans = 0;
        if (_item.TabModel.IdTrasmPerAcc) hasTrans = 1;

        if (_item.TabModel.DocInfo) {
            $.post(this.context.WAPath + '/Trasmissione/EseguiTrasmDoc', { "idTrasmModel": idTrasmModel, "idDoc": _item.TabModel.DocInfo.IdDoc, 'note': note, 'idTrasmPerAcc': _item.TabModel.IdTrasmPerAcc }, function (data) {
                if (data.Success == true) message = 'Trasmissione effettuata correttamente';
                else message = 'Errore di trasmissione';
                tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () { if (hasTrans == 0) mobileclient.control_info() } });
            }, 'json');
        }
        else {
            $.post(this.context.WAPath + '/Trasmissione/EseguiTrasmFasc', { "idTrasmModel": idTrasmModel, "idFasc": _item.TabModel.FascInfo.IdFasc, 'note': note, 'idTrasmPerAcc': _item.TabModel.IdTrasmPerAcc }, function (data) {
                if (data.Success == true) message = 'Trasmissione effettuata correttamente';
                else message = 'Errore di trsasmissione';
                tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () { if (hasTrans == 0) mobileclient.control_info() } });
            }, 'json');
        }
        if (hasTrans == 1) {

            mobileclient.showTab('TODO_LIST')
            mobileclient.control_info()
        }
    },
    accettaDelega: function (Id, IdRuoloDelegante, CodiceDelegante) {
        page.loading(true);
        $.post(this.context.WAPath + '/Delega/AccettaDelega', { "Id": Id, "IdRuoloDelegante": IdRuoloDelegante, "CodiceDelegante": CodiceDelegante }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    RevocaDeleghe: function (deleghe) {
        page.loading(true);

        $.post(this.context.WAPath + '/Delega/RevocaDeleghe', { "deleghe": utility.format2json(deleghe) }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');

    },
    dismettiDelega: function (idDelegante) {
        page.loading(true);
        $.post(this.context.WAPath + '/Delega/DismettiDelega', { "idDelegante": idDelegante }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    creaDelega: function () {

        page.loading(true);
        var idUtente = $('#ricercadelegato').attr('valore');
        var idRuolo = $('#ricercaruolo').attr('valore');

		
        var dataInizio = new Date($("#dataInizioD").val()).format("dd/MM/yyyy");
        var dataFine = new Date($("#dataFineD").val()).format("dd/MM/yyyy");

        $.post(this.context.WAPath + '/Delega/CreaDelega', { "idUtente": idUtente, "idRuolo": idRuolo, "dataInizio": dataInizio, "dataFine": dataFine }, function (data) {


            //{"Success":true,"Error":null}
            if (data.Success) {
                tooltip.init({ "titolo": "Delega creata correttamente", 'confirm': 'ok', 'returntrue': function () { mobileclient.showTab('LISTA_DELEGHE'); } })
            }
            else if (data.Error) {
                tooltip.init({ "titolo": "Delega non creata", 'confirm': 'ok' });
            }

        }, 'json');
    },
    eseguiSmistamento: function () {

        var elements = new Array();



        //prima carico gli elementi dell'albero, successivamente la ricerca
        $('.items_elenco').find('.persona').each(function () {

            var IdUtente = $(this).attr('val');
            var idRuolo = $(this).parent().parent().attr('val');
            var IdUO = $(this).parent().parent().parent().parent().attr('val');

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            var NoteIndividuali = $(this).find('TEXTAREA').html();

            if (conoscenza == true || competenza == true) {
                elements.push({ "IdUtente": IdUtente, "IdRuolo": idRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali });
            }
        });

        $('.items_elenco').find('.ruolo').each(function () {
            // if ($(this).find('ul').children().size() == 0) {
            var IdRuolo = $(this).attr('val');
            var IdUO = $(this).parent().parent().attr('val');

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');

            var NoteIndividuali = $(this).find('TEXTAREA').html();

            if (conoscenza == true || competenza == true) {
                elements.push({ "IdUtente": "", "IdRuolo": IdRuolo, "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza, "NoteIndividuali": NoteIndividuali });
            }
            // }
        });

        $('.items_elenco').find('.uoa').each(function () {
            if ($(this).find('ul').children().size() == 0) {
                var IdUO = $(this).attr('val');

                var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
                var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');
                if (conoscenza == true || competenza == true) {
                    elements.push({ "IdUtente": "", "IdRuolo": "", "IdUO": IdUO, "Competenza": competenza, "Conoscenza": conoscenza });
                }
            }
        });

        //carico gli elementi della ricerca

        $('.items_ricerca > LI').each(function () {
            var lista_id = $(this).attr('val').split('-');

            var conoscenza = $($(this).find('.inputcheckbox')[0]).hasClass('checked');
            var competenza = $($(this).find('.inputcheckbox')[1]).hasClass('checked');
            if (conoscenza == true || competenza == true) {
                elements.push({ "IdUtente": lista_id[0], "IdRuolo": lista_id[1], "IdUO": lista_id[2], "Competenza": competenza, "Conoscenza": conoscenza });
            }
        })





        //parametri del doc
        var IdDoc = this.model.TabModel.SmistamentoElements[this.documento_attuale].Id;
        var idTrasm = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissione;
        var idTrasmUtente = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissioneUtente;
        var IdTrasmSingola = this.model.TabModel.SmistamentoElements[this.documento_attuale].IdTrasmissioneSingola;
        if ($('#smista_form').find('.inputcheckbox').hasClass('checked')) {
            $.post(this.context.WAPath + '/Smistamento/EseguiSmistamento', { "idDoc": IdDoc, "idTrasm": idTrasm, "idTrasmUtente": idTrasmUtente, "IdTrasmSingola": IdTrasmSingola, "note": $('textarea[name="note_generali"]').val(), "elements": utility.format2json(elements) }, function (data) {
                //prima smisto			
                if (data.Success) {
                    tooltip.init({ "titolo": "Smistamento effettuato correttamente", 'confirm': 'ok', 'returntrue': function () {

                        page.loading(true);

                        //poi se ho selezionato un modello di trasmissione...trasmetto
                        if ($('[name="modello_trasm"]').val() != 'none') {

                            idTrasmModel = $('[name="modello_trasm"]').val();

                            $.post(mobileclient.context.WAPath + '/Trasmissione/EseguiTrasmDoc', { "idTrasmModel": idTrasmModel, "idDoc": IdDoc, 'note': '' }, function (data2) {

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

                idTrasmModel = $('[name="modello_trasm"]').val();

                $.post(mobileclient.context.WAPath + '/Trasmissione/EseguiTrasmDoc', { "idTrasmModel": idTrasmModel, "idDoc": IdDoc, 'note': '' }, function (data2) {

                    if (data2.Success == true) message = 'Smistamento tramite modello di trasmissione effettuato correttamente';
                    else message = 'Errore nello smistamento tramite modello di trasmissione';

                    tooltip.init({ "titolo": message, 'confirm': 'Ok', 'returntrue': function () {
                        mobileclient.documento_attuale = 0;
                        mobileclient.showTab('SMISTAMENTO');
                    }
                    });
                }, 'json');
            }
        }


    },
    smistamentoForm: function (numPage) {
        page.loading(true);
        $.post(this.context.WAPath + '/Smistamento/SmistamentoForm', { "numPage": numPage }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    logout: function () {
        document.location.href = this.context.WAPath + '/Home/Logout';
    },
	skinSelect: function(elem){

	 textSelect='';
	 if((this.model.TabShow.TODO_LIST==0)&&(!isTransmit)){
		 textSelect='Seleziona il ruolo';
	 }
	 else if((this.model.TabShow.RICERCA==4)&&(!isTransmit)){
		 textSelect='Seleziona la ricerca salvata';
	 }
	 else if((this.model.TabShow.TRASMISSIONE==5)||(isTransmit)){
		 textSelect='Seleziona il modello di trasmissione';
	 }

			if($(elem).children().length!=0){
				if(!$(elem).parent().hasClass('select'))
				{
					$(elem).hide();
					selectOriginale = elem;
					selOption(selectOriginale,textSelect);
				}
			}else{
				$(elem).hide();
			}
			isTransmit=false;

	
	},
	formatTrasmDate: function(data,sep){
		var data = eval(data.replace(/\/Date\((\d+)\)\//gi, "new Date($1)"))+' ';	
		data = data.substr(0,15);
		data = data.split(' ');
		// 	Mon Jul 26 2010 11:19:21 

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
		 if(!sep) sep = '.';
		 var data = data[2]+sep+mese+sep+data[3];
		 return data;
	}	

}
//funzioni utilità
var utility = {
    format2json: function (obj) {
		var tipo = typeof (obj);  
		if (tipo != "object" || obj === null) {  
			
			if (tipo == "string") obj = '"'+obj+'"';  
			return String(obj);  
		}  
		else {  
			
			var n, v, json = [], arr = (obj && obj.constructor == Array);  
			for (n in obj) {  
				v = obj[n];
				tipo = typeof(v);  
				if (tipo == "string") v = '"'+v+'"';  
				else if (tipo == "object" && v !== null) v = utility.format2json(v);  
				json.push((arr ? "" : '"' + n + '":') + String(v));  
			}  
			return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");  
		}  
	},
	var_dump : function(arr,level) {
		var dumped_text = "";
		if(!level) level = 0;
		
		//The padding given at the beginning of the line.
		var level_padding = "";
		for(var j=0;j<level+1;j++) level_padding += "    ";
		
		if(typeof(arr) == 'object') { //Array/Hashes/Objects 
			for(var item in arr) {
				var value = arr[item];
				
				if(typeof(value) == 'object') { //If it is an array,
					dumped_text += level_padding + "'" + item + "' ...\n";
					dumped_text += dump(value,level+1);
				} else {
					dumped_text += level_padding + "'" + item + "' => \"" + value + "\"\n";
				}
			}
		} else { //Stings/Chars/Numbers etc.
			dumped_text = "===>"+arr+"<===("+typeof(arr)+")";
		}
		return dumped_text;
	},
	checkOrientation : function(orientation){
		switch(orientation){  
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
	
		if(mobileclient.contenType != contenType) {
			
			mobileclient.contenType = contenType;
			
			mobileclient.fixPopTrasm();
			
			if((mobileclient.model.TabShow.RICERCA||mobileclient.model.TabShow.AREA_DI_LAVORO) && $(window).width()<=600){
				mobileclient.update();	
			}
			
			if(DEVICE=='ipad') checkHeight();
		}
		
		
		
	}
}

function selOption(e,titSelect){
	contList = $('<div class="contList"><h2>'+titSelect+'</h2></div>');			
	contScroll = $('<div class="contScroll"></div>');	
	UList = $('<ul id="listSelect"></ul>');			
	cont = $('<div class="conts"></div>');
	$(e).children().each(function(i,elem){
		textOption = $(this).text();				
		UList.append('<li>'+textOption+'</li>');				
	});
	$(document.body).append(cont);
	$(contScroll).append(UList);
	$(contList).append(contScroll);
	$(document.body).append(contList);
		
	$("#listSelect").children().each(function(i,el){
		$(el).click(function(){
			e.selectedIndex = i;
			$(contList).remove();
			$(cont).remove();
			attuale = $($(e).children()[e.selectedIndex]).html();
			var contentNode=$(e).children()[e.selectedIndex];
			$(e).prev().html(attuale);
			$('#iPhoneFilterT').html(attuale);
			$(e).val($(contentNode).val())
			$(e).trigger('change');
			
		});
	});		
	//if ($.browser.webkit) myScrollTooltip = new iScroll('listSelect', {desktopCompatibility:true });
	//myScrollTooltip=$('#listSelect').niceScroll();
}

Date.prototype.format = function(format) //author: meizz
{
  var o = {
    "M+" : this.getMonth()+1, //month
    "d+" : this.getDate(),    //day
    "h+" : this.getHours(),   //hour
    "m+" : this.getMinutes(), //minute
    "s+" : this.getSeconds(), //second
    "q+" : Math.floor((this.getMonth()+3)/3),  //quarter
    "S" : this.getMilliseconds() //millisecond
  }

  if(/(y+)/.test(format)) format=format.replace(RegExp.$1,
    (this.getFullYear()+"").substr(4 - RegExp.$1.length));
  for(var k in o)if(new RegExp("("+ k +")").test(format))
    format = format.replace(RegExp.$1,
      RegExp.$1.length==1 ? o[k] :
        ("00"+ o[k]).substr((""+ o[k]).length));
  return format;
}
