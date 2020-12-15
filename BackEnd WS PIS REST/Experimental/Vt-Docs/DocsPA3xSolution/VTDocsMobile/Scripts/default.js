// JavaScript Document
(function($){ 
	// Determine if we on iPhone or iPad
	var isiOS = false;
	var agent = navigator.userAgent.toLowerCase();
	console.log(agent);
	if(agent.indexOf('iphone') >= 0 || agent.indexOf('ipad') >= 0 || agent.indexOf('android') >= 0){
	       isiOS = true;
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

if($.browser.webkit) {
	
    document.write('<script type="text/javascript" src="'+this.context.WAPath+'/Scripts/iscroll-min.js"></script>');
	var myScroll;
	
	function setHeight() {
		if($.browser.webkit){
			
			var headerH = document.getElementById('header').offsetHeight;
			var footerH = document.getElementById('footer').offsetHeight;
			
			var wrapperH = window.innerHeight - headerH - footerH;
			document.getElementById('wrapper').style.height = wrapperH + 'px';
			
			if(myScroll){
				myScroll.refresh()
			}
		}
		
	}
	
	function loaded() {
		setInterval('setHeight()',50);//trovare soluzione migliore, l'ideale è dopo il loading avere il refresh del myscroll
		
		myScroll = new iScroll('scroller', {desktopCompatibility:true });
		/* myScroll = myScroll.destroy(true);	// Completely destroy the iScroll */	
		
	}

	window.addEventListener('onorientationchange' in window ? 'orientationchange' : 'resize', setHeight, false);
	
	var preventBehavior = function(e) {
	  e.preventDefault();
	};
 
	// Enable fixed positioning
	document.addEventListener("touchmove", preventBehavior, false);
	
	document.addEventListener('DOMContentLoaded', loaded, false);
}

var ie6 = (navigator.appName == "Microsoft Internet Explorer" && parseInt(navigator.appVersion) == 4 && navigator.appVersion.indexOf("MSIE 6.0") != -1);

$(document).ready(function() {
	mobileclient.init();
});

var page = {
	loading : function(mode){		
		if ( mode == true){
			$(document.body).append('<div id="loading">loading...</div>');
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
		
        //setto variabili
        this.application = $('#scroller');
        this.idTab = idTab;

        this.model = model;
        this.context = context;

        //lancio funzioni
        this.setActions();
        this.update();
		
		
    },
    reset: function () {
        $('#nnotifiche').hide();
        $('.none').hide();
		$('#footer_bar').hide();
		
		page.loading(false)
		$('#header > UL > LI').removeClass('on');
        $('#footer_bar .cont').html('');
        $('#prev_doc').html('')
        $('#nnotifiche').html('');
        $('#service_bar').html('');
        $('#item_list').html('');
		$('#ricerca_item > .filter_ricerca').html('');
		$('div.select').before($('div.select > select'));
		$('div.select').remove();
		$('#trasmissione_doc > .filter_trasm').html('');
		$('#info_item').html('');
		$('#deleghe').html('');
       
    },
    checkSession: function () {
        if (this.model.SessionExpired == true) {
			tooltip.init({"titolo" : 'La tua sessione &egrave; scaduta','confirm': 'Riconnetti','returntrue':function(){ document.location.href=mobileclient.context.WAPath+"/Login/Login";}});
           
        }
    },
	skinSelect: function(){

	 textSelect='';
	 if(this.model.TabShow.TODO_LIST==0){
		 textSelect='Seleziona il ruolo';
	 }
	 else if(this.model.TabShow.RICERCA==4){
		 textSelect='Seleziona la ricerca salvata';
	 }
	 else if(this.model.TabShow.TRASMISSIONE==5){
		 textSelect='Seleziona il modello di trasmissione';
	 }
	 
		$("select.skin").each(function(ind,elem){
		
			if($(elem).children().length!=0){
				if(!$(elem).parent().hasClass('select'))
				{
					
					selectOriginale = elem;
					attuale = $($(elem).children()[elem.selectedIndex]).html();
					contSelect = $('<div class="select"></div>');
					
					$(contSelect).click(function(){
						selOption(selectOriginale,textSelect);
					});
					$(elem).wrap(contSelect);
					$(elem).hide();
					$(elem).before('<p>'+attuale+'</p>');	
				}
			}else{
				$(elem).hide();
			}
			
		});	
	
	},
    setActions: function () {
        $('#header > UL > LI').each(function (i, el) {
			if($(el).attr('id') != 'LOGOUT'){
				$(el).click(function () {
					mobileclient.showTab($(el).attr('id'));
					 $('#header > UL > LI').removeClass('on');
				   
				})
			}
			else if($(el).attr('id') == 'LOGOUT'){
              var nome = mobileclient.model.DescrUtente
                if (mobileclient.model.DelegaEsercitata)
                    nome = mobileclient.model.DelegaEsercitata.Delegato

				 $(el).click(function(){ tooltip.init({"titolo" : nome+' vuole uscire?', "content": '', 'deny': 'NO', 'confirm': 'SI', 'returntrue': function(){mobileclient.logout()}})});
			}
        });
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
	},
    showTab: function (idTab) {
		
		page.loading(true);
		$.post(this.context.WAPath + "/Home/ShowTab", { "idTab": idTab }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');

    },
    update: function () {
        this.checkSession();
        this.reset()
		//console.log(this.model);
        if (this.model.TabShow.TODO_LIST) {
			$('#TODO_LIST').addClass('on');
            this.updateToDoList();
        }else if (this.model.TabShow.DETTAGLIO_DF_TRASM) {
			//$('#TODO_LIST').addClass('on');
			
            this.updateDettaglioTrasm();
        }else if (this.model.TabShow.DETTAGLIO_DOC) {
			//$('#TODO_LIST').addClass('on');
            this.updateDettaglioDoc();
        }else if (this.model.TabShow.RICERCA) {
			$('#RICERCA').addClass('on');
            this.updateRicerca();
        }else if (this.model.TabShow.TRASMISSIONE) {
            this.updateTrasmissione();
        }
		else if (this.model.TabShow.LISTA_DELEGHE) {
			$('#LISTA_DELEGHE').addClass('on');
            this.updateDeleghe();
        }
		else if(this.model.TabShow.CREA_DELEGA){
			$('#LISTA_DELEGHE').addClass('on');
            this.nuovaDelega();			
		}
		if($.browser.webkit) this.skinSelect();
    },
	previous: function(prev){
		this.checkSession();
        this.reset();
		
		var prev = this.model.PreviousTabName;
		
		console.log(this.model.TabShow)
        if (prev=='TODO_LIST' || prev=='RICERCA') {
			 this.showTab(prev);
        }else if (prev=='DETTAGLIO_DF_TRASM') {
			var oldmodello = this.model.PreviousTabModel;
			
			if (oldmodello.FascInfo) {
				$.post(this.context.WAPath + "/Fascicolo/DettaglioFascTrasm", { "idFasc":oldmodello.FascInfo.IdFasc, "idTrasm": oldmodello.TrasmInfo.IdTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
			} else {
				$.post(this.context.WAPath + "/Documento/DettaglioDocTrasm", { "idDoc":oldmodello.DocInfo.IdDoc, "idTrasm": oldmodello.TrasmInfo.IdTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
			}
			
        }else if (prev=='DETTAGLIO_DOC') {
			//non c'è mai l'indietro che va ad un doc
        }else if (prev=='TRASMISSIONE') {
			//non c'è mai l'indietro che va ad una trasmissione
        }
		
		
	
	},
    updateToDoList: function () {
		//checking sulle deleghe assegnate per il popup
		if(this.model.NumDeleghe != 0){
			tooltip.init({"titolo" : '', "content": '<div class="msgdeleghe">Ti sono state assegnate '+this.model.NumDeleghe+' deleghe<br>Vuoi controllarle?</div>', 'deny': 'NO', 'confirm': 'SI', 'returntrue':function(){mobileclient.showTab('LISTA_DELEGHE');} });
		}
		
        //prima notifiche
		if($('#nnotifiche')){
			if(this.model.TabModel.NumElements>0){
				$('#nnotifiche').html(this.model.TabModel.NumElements);
				$('#nnotifiche').show();
			}else{
				$('#nnotifiche').hide();	
			}
		}
		
        //poi aggiungo alla service_bar l'indietro se si tratta di un folder e metto il nome
        if (this.model.TabModel.IdParent) {
            var indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.upFolder() });
            $('#service_bar').append(indietro);


            //dato che ci sono piazzo anche la folder name
            $('#folder_name > SPAN').html(this.model.TabModel.NomeParent);
            $('#folder_name').show();
        }

        //poi paginatore
        $('#service_bar').append($('<div id="paginatore"><img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/paginatore_left.gif" /> <p></p> <img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/paginatore_right.gif" /></div>'));
        $('#paginatore > P').html(this.model.TabModel.CurrentPage + ' di ' + this.model.TabModel.NumTotPages);

        if (this.model.TabModel.CurrentPage == 1) {
            //se currentpage == 1 spengo la voce di sinistra
            $($('#paginatore > IMG')[0]).addClass('disabled');
        }
        else {
            $($('#paginatore > IMG')[0]).click(function () { mobileclient.getPage('/ToDoList/ChangePage',mobileclient.model.TabModel.CurrentPage - 1) });
        }
        if (this.model.TabModel.CurrentPage == this.model.TabModel.NumTotPages) {
            //se currentpage == numtotpage allora spengo la freccia di destra

            $($('#paginatore > IMG')[1]).addClass('disabled');
        }
        else {
            $($('#paginatore > IMG')[1]).click(function () { mobileclient.getPage('/ToDoList/ChangePage',mobileclient.model.TabModel.CurrentPage + 1) });
        }
        //e poi select dei ruoli
		$('#filter_ruolo > SELECT').unbind('change') // metto qui l'elimininazione perchè nel reset ci sono i div di mezzo
		$('#filter_ruolo > SELECT').html('');
		
		//gestione errore:)
		$($('#filter_ruolo > SELECT')[1]).remove();
		
        var ruoli = $(this.model.Ruoli);
		$('#filter_ruolo > SELECT').change(function () { mobileclient.cambiaRuolo($(this).val()) });
        ruoli.each(function (i, el) {
			if (el.Id == mobileclient.model.IdRuolo) selected = 'selected="selected"';
			else selected = '';
			
            $('#filter_ruolo > SELECT').append($('<option value="' + el.Id + '" '+selected+'>' + el.Descrizione + '</option>'));
        });


        $('#service_bar').show();
        $('#filter_ruolo').show();
        if( this.model.TabModel.NumTotPages != 0){
			$('#paginatore').show();
			//e poi popolo gli elementi
			var elementi = this.model.TabModel.ToDoListElements;
			$(elementi).each(function (i, el) {
	
				if (el.Tipo.FOLDER || el.Tipo.FASCICOLO) var icateg = 'folder';
				else icateg = 'file';
				
				//se è dentro un fascicolo mostro solo le note
				if (mobileclient.model.TabModel.IdParent) {
	
					var data_tipo = $('<div class="data_tipo">' + el.Note + '</div>');
					var mittente = '';
				}
				else{
					var data_tipo = $('<div class="data_tipo">' + el.DataDoc + ' - ' + el.Ragione + '</div>');
					var mittente = $('<div class="mittente">' + el.Mittente + '</div>');
				}
	
				var categ = $('<img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/item_categ_' + icateg + '.gif" class="categ" />');
				var oggetto = $('<div class="oggetto"><a href="javascript:;">' + el.Oggetto + '</a></div>');
				if(el.Tipo.FASCICOLO || el.Tipo.DOCUMENTO) oggetto.click(function () { mobileclient.getTrasmission(el) });
				var ico_open = $('<img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_' + icateg + '.gif" class="ico_open" />').click(function () { mobileclient.getItem(el) });
	
				var _item = $('<div class="item"></div>');
				
				_item.append(categ);
				_item.append(oggetto);
				_item.append(ico_open);
				_item.append($('<div class="clear"><br class="clear" /></div>'));
				_item.append(data_tipo);
				_item.append(mittente);
				_item.append($('<div class="clear"><br class="clear" /></div>'));
				
				$('#item_list').append(_item);
				$('#item_list').append($('<div class="clear"><br class="clear" /></div>'));
	
			});
			$('#item_list').show();			
		}else{
			if(this.model.TabModel.IdParent){
				$('#item_list').html('<div class="no_todolist_fascicolo">&nbsp;</div>');
			}else{
				$('#item_list').html('<div class="no_todolist">&nbsp;</div>');
			}
			
			$('#item_list').show();	
		}
    },
    updateRicerca: function () {
		//prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();
		
		//aggiungo alla service_bar l'indietro se si tratta di un folder e metto il nome
		
        if (this.model.TabModel.IdParent) {
            var indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.upRicFolder() });
            $('#service_bar').append(indietro);
			
            //dato che ci sono piazzo anche la folder name
            $('#folder_name > SPAN').html(this.model.TabModel.NomeParent);
            $('#folder_name').show();
			
			$('#ricerca_item > .filter_ricerca').hide();
			$('#ricerca_item > h2').hide();
        }
		else{
			
			//poi aggiorno la select ma solo se non sono dentro un fascicolo/folder
			$('#ricerca_item > .filter_ricerca').html('<select class="skin"></select>');
			$('#ricerca_item > .filter_ricerca').show();
			var ricerche = $(this.model.TabModel.Ricerche);
			ricerche.each(function (i, el) {
				//controllo se documento o fascicolo per passare il type del documento
				if(el.Type.RIC_DOCUMENTO){	var tipo = 'RIC_DOCUMENTO'; var sigla = 'D';}
				else if(el.Type.RIC_FASCICOLO){ var tipo = 'RIC_FASCICOLO'; var sigla = 'F';}
				
				//nel caso in cui sono già dentro una ricerca salvata metto il selected nella option
				if(mobileclient.model.TabModel.IdRicercaSalvata == el.Id) selected = 'selected="selected"';
				else selected = '';
				
				
				$('#ricerca_item > .filter_ricerca > SELECT').append($('<option id="' + el.Id + '" value="' + el.Id + '|' + tipo + '" '+selected+'>'+sigla +' '+ el.Descrizione + '</option>'));
			});
			$('#ricerca_item > .filter_ricerca').append($('<div class="puls_cerca">Cerca</div>'));			
			
			if($('#ricerca_item > .filter_ricerca > SELECT').children().length!=0){
				$('#trasmissione_doc > .filter_trasm > .puls_cerca').removeClass('disabled');		
				$('#ricerca_item > .filter_ricerca > .puls_cerca').click(function(){
				
				if ($('#ricerca_item > .filter_ricerca').children('select').val()){
					var value = $('#ricerca_item > .filter_ricerca').children('select').val().split('|');
				}
				else{
					var value = $('#ricerca_item > .filter_ricerca > div > select').val().split('|');
				}
				
					mobileclient.ricerca(value[0],value[1]);																											
				});				
			}else{
				$('#trasmissione_doc > .filter_trasm > .puls_cerca').addClass('disabled');	
			}		
			
			$('#ricerca_item > .filter_ricerca').append($('<div class="clear"><br class="clear"/></div>'));
		}

        //poi paginatore
        $('#service_bar').append($('<div id="paginatore"><img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/paginatore_left.gif" /> <p></p> <img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/paginatore_right.gif" /></div>'));
        $('#paginatore > P').html(this.model.TabModel.CurrentPage + ' di ' + this.model.TabModel.NumTotPages);

        if (this.model.TabModel.CurrentPage == 1) {
            //se currentpage == 1 
	
            $($('#paginatore > IMG')[0]).addClass('disabled');
			
        }
        else {
            $($('#paginatore > IMG')[0]).click(function () { mobileclient.getPage('/Ricerca/ChangePage',mobileclient.model.TabModel.CurrentPage - 1) });
        }
		
        if (this.model.TabModel.CurrentPage == this.model.TabModel.NumTotPages) {
            //se currentpage == numtotpage allora spengo la freccia di destra
            $($('#paginatore > IMG')[1]).addClass('disabled');
        }
        else {
            $($('#paginatore > IMG')[1]).click(function () {mobileclient.getPage('/Ricerca/ChangePage',mobileclient.model.TabModel.CurrentPage + 1) });
        }
		
		$('#service_bar').show();
		
		//solo se il numero totale di pagine è superiore a zero mostro il paginatore
		if(this.model.TabModel.NumTotPages != 0) $('#paginatore').show();
		
		
		
		//e poi popolo gli elementi
        var elementi = this.model.TabModel.Risultati;
		$('#ricerca_item > .item_list').html('');
        $(elementi).each(function (i, el) {

            if (el.Tipo.FOLDER || el.Tipo.FASCICOLO) var icateg = 'folder';
            else icateg = 'file';
				
         	var data_tipo = $('<div class="data_tipo">' + el.Note + '</div>');
			
            var categ = $('<img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/item_categ_' + icateg + '.gif" class="categ" />');
            var oggetto = $('<div class="oggetto"><a href="javascript:;">' + el.Oggetto + '</a></div>');
			
            if(el.Tipo.FASCICOLO || el.Tipo.DOCUMENTO) oggetto.click(function () { mobileclient.getTrasmission(el) });
			
            var ico_open = $('<img src="' + mobileclient.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/item_ico_open_' + icateg + '.gif" class="ico_open" />').click(function () { mobileclient.getItem(el) });

            var _item = $('<div class="item"></div>');
          
        
            
            _item.append(categ);
            _item.append(oggetto);
            _item.append(ico_open);
			_item.append($('<div class="clear"><br class="clear" /></div>'));
			_item.append(data_tipo);
            _item.append($('<div class="clear"><br class="clear" /></div>'));

            $('#ricerca_item > .item_list').append(_item);
            $('#ricerca_item > .item_list').append($('<div class="clear"><br class="clear" /></div>'));

        });
		
		//gestisco l'eccezione della ricerca vuota su fascicolo o sul totale contenuti
		if(this.model.TabModel.NumTotPages==0){
			if(this.model.TabModel.IdParent){
				$('#ricerca_item > .item_list').html('<div class="no_todolist_fascicolo">&nbsp;</div>');
			}else{
				$('#ricerca_item > .item_list').html('<div class="no_ricerca">&nbsp;</div>');
			}
		}
		
		$('#ricerca_item').show();
		
		
    },
    updateDettaglioTrasm: function () {
	
		//prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();
		
        var fascinfo = this.model.TabModel.FascInfo;
        var docinfo = this.model.TabModel.DocInfo;
        var infotrasm = this.model.TabModel.TrasmInfo;
		
		
		//se ho la trasmissione la mostro altrimenti sto cercando solo le info per documento/fascicolo..cioè sto chiamando la funzione dalla ricerca
		if(infotrasm){
			//popolo la service bar
			if(infotrasm.HasWorkflow == true){
				var mesi = new Array('Gennaio','Febbraio','Marzo',
					'Aprile','Maggio','Giugno','Luglio','Agosto','Settembre',
					'Ottobre','Novembre','Dicembre');
				var date = new Date();
				var gg  = date.getDate();
				var mese = date.getMonth();
				var yy = date.getYear();
				var yyyy = (yy < 1000) ? yy + 1900 : yy;

				var mess = 'in data '+gg + " " + mesi[mese] + " " + yyyy+' da '+this.model.DescrUtente+' dal dispositivo mobile.';
				
				var rifiuta = $('<div class="pulsante_classic fll"><a href="javascript:;">Rifiuta</a></div>').click(function () {
					tooltip.init({"titolo" : 'Inserisci note di rifiuto', "content": $('<textarea></textarea').html('Rifiutata '+mess), 'deny': 'Annulla', 'confirm': 'Rifiuta', 'returntrue': function(){mobileclient.rifiutaTrasm()}});
																															  });
				var accetta = $('<div class="pulsante_classic fll"><a href="javascript:;">Accetta</a></div>').click(function () {
					tooltip.init({"titolo" : 'Inserisci note di accettazione', "content": $('<textarea></textarea').html('Accettata '+mess), 'deny': 'Annulla', 'confirm': 'Accetta', 'returntrue': function(){mobileclient.accettaTrasm()}});
																														  });
				//var trasmetti = $('<div class="pulsante_classic fll"><a href="javascript:;">Trasmetti</a></div>').click(function () { mobileclient.trasmissioneForm(docinfo,fascinfo) });
			

				$('#service_bar').append(rifiuta);
				$('#service_bar').append(accetta);
				//$('#service_bar').append(trasmetti);
			}
			if (docinfo) {
				var visualizza = $('<div class="pulsante_classic flr"><a href="javascript:;">Visualizza</a></div>').click(function () {
					mobileclient.dettaglioDoc(docinfo.IdDoc)
				});
			}
			else {
				var visualizza = $('<div class="pulsante_classic flr"><a href="javascript:;">Contenuto</a></div>').click(function () {
					mobileclient.inFolder(fascinfo.IdFasc, fascinfo.Descrizione, infotrasm.IdTrasm);
				});
	
			}
	
			
			$('#service_bar').append(visualizza);
	
			$('#service_bar').show();
		
			$($('.dett_trasm > table > tbody > tr > td')[0]).html(infotrasm.Mittente)
			$($('.dett_trasm > table > tbody > tr > td')[1]).html(infotrasm.Ragione)
			$($('.dett_trasm > table > tbody > tr > td')[2]).html(this.formatTrasmDate(infotrasm.Data))
	
			$($('#sezione_trasmissione > P')[0]).html(infotrasm.NoteGenerali)
			$($('#sezione_trasmissione > P')[1]).html(infotrasm.NoteIndividuali)
			$('#sezione_trasmissione').show();	
		}
		else{
			var indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.showTab('RICERCA') });
			$('#service_bar').append(indietro);
			
			var trasmetti = $('<div class="pulsante_classic flr"><a href="javascript:;">Trasmetti</a></div>').click(function () { mobileclient.trasmissioneForm(mobileclient.model.TabModel.DocInfo,mobileclient.model.TabModel.FascInfo) });
            $('#service_bar').append(trasmetti);
			
			$('#service_bar').show();
			
			$('#sezione_trasmissione').hide();	
		}
		
        if (docinfo) {
            $($('#sezione_documento > H2')[0]).html('Informazioni del documento')

			$($('#sezione_documento > H3')[0]).html('OGGETTO')
			
            $($('#sezione_documento > P')[0]).html(docinfo.Oggetto)
            $($('#sezione_documento > P')[1]).html(docinfo.Note)
        }
        else {
            $($('#sezione_documento > H2')[0]).html('Informazioni del fascicolo')
			
			 $($('#sezione_documento > H3')[0]).html('DESCRIZIONE')

            $($('#sezione_documento > P')[0]).html(fascinfo.Descrizione)
            $($('#sezione_documento > P')[1]).html(fascinfo.Note)
        }
        $('#info_trasm').show();
    },
    updateDettaglioDoc: function () {
		//prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();
		
        var docinfo = this.model.TabModel.DocInfo;
        var infotrasm = this.model.TabModel.TrasmInfo;

        var visualizza = $('<div class="pulsante_classic flr"><a href="javascript:;">Info di profilo</a></div>').click(function () {
            mobileclient.infoProfilo();
        })

        $('#service_bar').append(visualizza);
		
		var indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.previous() });
           if (this.model.PreviousTabName == 'DETTAGLIO_DOC') {
            indietro = $('<div></div>')
           
            if (docinfo.IdDocPrincipale != null) {
            indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () {
                mobileclient.dettaglioDoc(docinfo.IdDocPrincipale)
            })
        }
        }
         $('#service_bar').append(indietro);
			
        $('#service_bar').show();
        $('#service_bar').append($('<div id="name_onservicebar" class="name_anteprima"></div>'));
        
        var allegati1 = this.model.TabModel.Allegati;
        if (allegati1.length > 1) {
            $('#name_onservicebar').html('<select></select>');
            var numall = 0;
            $(allegati1).each(function (i, el) {
                var nomeDoc = el.Oggetto;
                if (nomeDoc.length > 128) {
                    nomeDoc = nomeDoc.substring(0, 125) + '...';
                }
                if (numall == 0) {
                    $('#name_onservicebar > SELECT').append('<option id="' + el.IdDoc + '" value="' + el.IdDoc + '">Doc.princ. - ' + nomeDoc + '</option>');
                } else {
                    $('#name_onservicebar > SELECT').append('<option id="' + el.IdDoc + '" value="' + el.IdDoc + '">All. n.' + numall + ' - ' + nomeDoc + '</option>');
                }
                numall++;
            });
            $('#name_onservicebar > SELECT').change(function () {
                $("select option:selected").each(function () { 
                if(this.id!='')
                { 
                mobileclient.dettaglioDoc(this.id);
                } 
                });                
            }

            );
        } else {
            $('#name_onservicebar').html('&nbsp;');
        }

        $('#footer_bar > .cont').append($('<ul><li class="first"><a href="" target="_blank">Scarica Documento</a></li><li><a href="javascript:;">Zoom +\-</a></li></ul><div class="clear"><br class="clear" /></div>'));

        if (docinfo.HasPreview == true) {
            //se ho la preview mostro il documento e attivo il link allo zoom
			
			/* todo */
			heightPDF = 1200;
			widthPDF =  845;
			ratioPDF = widthPDF/heightPDF;			
			widthIMG = $(window).width()+10;
			heightIMG = Math.ceil((widthIMG*heightPDF)/widthPDF);
				
            $('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Documento/Preview/' + docinfo.IdDoc + '?dimX='+widthIMG+'&dimY='+heightIMG+')' });
            $($('#footer_bar > div > ul > li > a')[1]).click(function () { mobileclient.zoomDocument(mobileclient.model.TabModel.DocInfo.IdDoc) });

        }
        else {
            //altrimenti mostro il file no_preview non metto link e disattivo graficamente lo zoom.
			if(docinfo.IsAcquisito){
            	$('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/no_preview.jpg)' });
			}else{
				$('#prev_doc').css({ 'backgroundImage': 'url(' + this.context.WAPath + '/Content/' + mobileclient.context.SkinFolder + '/img/no_acquisito.jpg)' });
				
			}
            if(docinfo.IsAcquisito){
			   $('#prev_doc').append('<div id="link_doc"><p>Per vedere il documento <br />clicca il pulsante</p><div class="pulsante"><a href="" target="_blank">Scarica Documento</a></div></div>');
				$('#link_doc > div > a')[0].href = this.context.WAPath + '/Documento/File/' + docinfo.IdDoc;
				$('#link_doc').show();
			}
			
            $($('#footer_bar > div > ul > li')[1]).addClass('disabled');
        }
        if(docinfo.IsAcquisito){
				$('#footer_bar > div > ul > li > a')[0].href = this.context.WAPath + '/Documento/File/' + docinfo.IdDoc;
				$('#footer_bar').show();
		}else{
			/*
			$($('#footer_bar > div > ul > li')[0]).addClass('disabled');
			$('#footer_bar > div > ul > li > a')[0].href = 'javascript:;';
			*/
			$('#footer_bar').hide();
		}
		$('#prev_doc').show();
    },

	updateTrasmissione : function(){
		//prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();
		
		//prima metto il titolo alla service bar
		$('#service_bar').append('<h2>TRASMISSIONE RAPIDA</h2>');
		$('#service_bar').show();
		
		//metto il tasto indietro
		var indietro = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () { mobileclient.previous() });
            $('#service_bar').append(indietro);
		
		//tiro fuori le vars importanti
		var modelli = $(this.model.TabModel.ModelliTrasm);
		var docinfo = this.model.TabModel.DocInfo;
		var fascinfo = this.model.TabModel.FascInfo;

		//buildo la select dei modelli di trasmissione
		$('#trasmissione_doc > .filter_trasm').html('<select class="skin"></select>');
		
		modelli.each(function (i, el) {		
			$('#trasmissione_doc > .filter_trasm > SELECT').append($('<option value="' + el.Id + '" >'+ el.Codice + '</option>'));
		});
		
		$('#trasmissione_doc > .filter_trasm').append($('<div class="puls_blue">Trasmetti</div>'));
		
		if($('#trasmissione_doc > .filter_trasm > SELECT').children().length!=0){
			$('#trasmissione_doc > .filter_trasm > .puls_blue').removeClass('disabled');		
			$('#trasmissione_doc > .filter_trasm > .puls_blue').click(function(){
				if ($('#trasmissione_doc > .filter_trasm').children('select').val()){
					var idTrasmModel = $('#trasmissione_doc > .filter_trasm').children('select').val();
				}
				else{
					var idTrasmModel = $('#trasmissione_doc > .filter_trasm > div > SELECT').val();
				}
				
				var note = $('#trasmissione_doc > P > TEXTAREA').val();			
				if(mobileclient.model.TabModel.DocInfo) var _item = mobileclient.model.TabModel.DocInfo;
				else var _item = mobileclient.model.TabModel.FascInfo;		
				mobileclient.eseguiTrasm(_item,idTrasmModel,note);
			});
		}else{
			$('#trasmissione_doc > .filter_trasm > .puls_blue').addClass('disabled');	
		}
		
		$($('#trasmissione_doc > P')[0]).html('<textarea></textarea>');
		$('#trasmissione_doc > P > TEXTAREA').html('');
		
		if (docinfo) {
            $($('#trasmissione_doc > H2')[0]).html('Informazioni di profilo del documento')

            $($('#trasmissione_doc > P')[1]).html(docinfo.Oggetto)
            $($('#trasmissione_doc > P')[2]).html(docinfo.Note)
        }
        else {
            $($('#trasmissione_doc > H2')[0]).html('Informazioni di profilo del fascicolo')

            $($('#trasmissione_doc > P')[1]).html(fascinfo.Oggetto)
            $($('#trasmissione_doc > P')[2]).html(fascinfo.Note)
        }
		
		$('#trasmissione_doc').show();
		
		
	},
    zoomDocument: function (IdDoc) {
		// Disable fixed positioning
		if($.browser.webkit){ document.removeEventListener("touchmove", preventBehavior, false);}
		
		imageZoomed = $('<div id="image_zoomed"><div class="green_bar"><div class="pulsante_classic fll"><a href="javascript:;" onclick="mobileclient.closezoom()">Indietro</a></div></div><img src="' + this.context.WAPath + '/Documento/Preview/' + IdDoc + '" width="' + $(window).width() * 2 + '" /></div>');
			
		$('body').append(imageZoomed);
		

			$('#image_zoomed').doubletap(
				function(event){
					mobileclient.closezoom();
				},
				function(event){
					
				},
				400
			);
		
		
			
    },
    closezoom: function () {
		
		if($.browser.webkit){ document.addEventListener("touchmove", preventBehavior, false);}

        $('#image_zoomed').remove();
    },
    infoProfilo: function () {
        this.reset();
        var docinfo = this.model.TabModel.DocInfo;

        var visualizza = $('<div class="pulsante_classic fll"><a href="javascript:;">Indietro</a></div>').click(function () {
            mobileclient.reset();
            mobileclient.updateDettaglioDoc();
			//per tornare indietro, richiamo direttamente la funzione del dettagliodoc
        })

        $('#service_bar').append(visualizza);
        $('#service_bar').show();

		$('#info_item').append($('<h2>Informazioni di profilo documento</h2>'));
		$('#info_item').append($('<h3>Oggetto</h3><p>'+docinfo.Oggetto+'</p><div class="sep">&nbsp;</div>'));
		$('#info_item').append($('<h3>Note</h3><p>'+docinfo.Note+'</p><div class="sep">&nbsp;</div>'));
       
        
		$('#info_item').append($('<h3>Id Documento</h3><p>'+docinfo.IdDoc+'</p><div class="sep">&nbsp;</div>'));
		if(docinfo.IsProtocollato){
			$('#info_item').append($('<h3>Segnatura</h3><p>'+docinfo.Segnatura+'</p><div class="sep">&nbsp;</div>'));
			$('#info_item').append($('<h3>Data protocollazione</h3><p>'+this.formatTrasmDate(docinfo.DataProto)+'</p><div class="sep">&nbsp;</div>'));
			$('#info_item').append($('<h3>Mittente protocollazione</h3><p>'+docinfo.Mittente+'</p><div class="sep">&nbsp;</div>'));
			
			if(docinfo.TipoProto == "p"){
				var stringa_destinatari = '';
				var destinatari = docinfo.Destinatari;
				for (var i=0;i<destinatari.length;i++){
					stringa_destinatari += destinatari[i]+"  - ";
				}
			
				$('#info_item').append($('<h3>Destinatari protocollo</h3><p>'+stringa_destinatari+'</p><div class="sep">&nbsp;</div>'));
			}
		}
		else{
			$('#info_item').append($('<h3>Data creazione</h3><p>'+this.formatTrasmDate(docinfo.DataDoc)+'</p><div class="sep">&nbsp;</div>'));
		}
		var fascicoli = docinfo.Fascicoli;
		var stringa_fascicoli = '';
		for (var i=0;i<fascicoli.length;i++){
		
		}
	
		$('#info_item').append($('<h3>Fascicoli</h3><p>'+fascicoli+'</p><div class="sep">&nbsp;</div>'));
			
		
		
        $('#info_item').show();
		
		
		//console.log(docinfo);
    },
	nuovaDelega : function(){
		//prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();
		
		var nuovadelega = $('<div class="pulsante_classic flr"><a href="javascript:;">Delega Rapida</a></div>').click(function () { mobileclient.CreaDelegaForm() });
		$('#service_bar').append(nuovadelega);
		
		//in base al vecchio model, metto i pulsante di servizio a sinistra		
		if(!this.model.DelegaEsercitata){
			var multiplebutton = $('<div class="multiple_button"><ul><li>Ricevute</li><li>Assegnate</li></div>');
			
			$('#service_bar').append(multiplebutton);
		
			$('#service_bar > .multiple_button > ul > li').each(function(i,el){
					$(el).click(function(){
						mobileclient.tipodelega = i;
						mobileclient.showTab('LISTA_DELEGHE');
					
					})
			});
		}
		else{
			
			
		}
		
		$('#service_bar').show();
		
		
		//accendo la voce della nuova delega
		$('#service_bar').find('.flr').addClass("on");
		
		//e svuoto tutto
		$('#deleghe').html('');
		
		
		//aggiungo le info sulla persona
		$('#deleghe').append('<div class="info_pers"></div>');

		$('#deleghe > .info_pers').append('<p>'+this.model.DescrUtente+'</p>');
		$('#deleghe > .info_pers').append('<p>'+this.model.DescrRuolo+'</p>');
		
		//e la form..
		
		$('#deleghe').append('<table class="table_deleghe"></table>');
		$('#deleghe > .table_deleghe').append('<tr><th>Modelli:</th><td><input type="text" name="delega_id" value="Modello di Delega" /><img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/deleghe_damodello_ico.gif" name="delega_modello"></td></tr>');
		$('#deleghe > .table_deleghe').append('<tr><th>Nome Delegato:</th><td name="delega_nomedelegato">&nbsp;</td></tr>');
		$('#deleghe > .table_deleghe').append('<tr><th>Ruolo Delegato:</th><td name="delega_ruolodelegato">&nbsp;</td></tr>');
		$('#deleghe > .table_deleghe').append('<tr><th>Decorrenza:</th><td><input type="text" name="delega_datadecorrenza" value="gg/mm/aaaa" size="10" /><img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/deleghe_calendario_ico.gif" class="none"></td></tr>');
		$('#deleghe > .table_deleghe').append('<tr><th>Scadenza:</th><td><input type="text" name="delega_datascadenza" value="gg/mm/aaaa" size="10" /><img src="' + this.context.WAPath + '/Content/' + this.context.SkinFolder + '/img/deleghe_calendario_ico.gif" class="none"></td></tr>');
		
		
		
		$("[name='delega_id']").click(function(){
			mobileclient.delega_modelli();
		})
		$("[name='delega_id']").next().click(function(){
			mobileclient.delega_modelli();
		})
		
		$("[name='delega_datadecorrenza']").next().click(function(){
			mobileclient.delega_calendario('delega_datadecorrenza');
		})
		
		$("[name='delega_datascadenza']").next().click(function(){
			mobileclient.delega_calendario('delega_datascadenza');
		})
		
		var attiva_delega = $('<div class="pulsante_classic_big">Attiva Delega</div>').click(function () {
			
			var expdata = /^(0[1-9]|[12][0-9]|3[01])[/](0[1-9]|1[012])[/](19|20)\d\d/;
			if( expdata.test($("[name='delega_datadecorrenza']").attr('value')) && expdata.test($("[name='delega_datadecorrenza']").attr('value'))) {
			
				mobileclient.CreaDelegaDaModello($("[name='delega_id']").attr('val'),$("[name='delega_datadecorrenza']").attr('value'),$("[name='delega_datascadenza']").attr('value'));
			}
			else{
				tooltip.init({"titolo" : "le date devono essere inserite nel formato dd/mm/aaaa",'confirm': 'ok'});	
				
			}
			
			})
		$('#deleghe').append(attiva_delega);
		$('#deleghe').show();
		
	},
	delega_modelli : function(){
		if($(this.model.TabModel.ModelliDelega).size()>0){
			//replico i funzionamenti del seloption
			cont = $('<div class="conts"></div>');
			
			contList = $('<div class="contList"><h2>Seleziona il modello</h2></div>');			
			contScroll = $('<div class="contScroll"></div>');	
			UList = $('<ul id="listSelect"></ul>');			
			
			
	
			$(this.model.TabModel.ModelliDelega).each(function(i,el){
				var li = $('<li>'+el.Nome+'</li>').click(function(){
					$('.contList').remove();
					$('.conts').remove();
					//console.log(el);
					$("[name='delega_id']").attr('value',el.Nome);
					$("[name='delega_id']").attr('val',el.Id);
					$("[name='delega_datadecorrenza']").attr('value',mobileclient.formatTrasmDate(el.DataInizioDelega,'/'));
					$("[name='delega_datascadenza']").attr('value',mobileclient.formatTrasmDate(el.DataFineDelega,'/'));
					
					//nome delegato
					$("[name='delega_nomedelegato']").html(el.DescrUtenteDelegato);
									
					//ruolo delegato
					
					$("[name='delega_ruolodelegato']").html(el.DescrRuoloDelegante);
	
					
				});
				UList.append(li);				
			});
			
			$(document.body).append(cont);
			$(contScroll).append(UList);
			$(contList).append(contScroll);
			$(document.body).append(contList);
				
			myScrollTooltip = new iScroll('listSelect', {desktopCompatibility:true });
		}
		else{
			tooltip.init({"titolo" : "non sono presenti modelli di delega",'confirm': 'ok'});	
		}
	},
	delega_calendario : function(){
			//alert(1)			
	},
	updateDeleghe : function(){
	
		//prima notifiche
        $('#nnotifiche').html(this.model.ToDoListTotalElements);
        $('#nnotifiche').show();
	
		var nuovadelega = $('<div class="pulsante_classic flr"><a href="javascript:;">Delega Rapida</a></div>').click(function () { mobileclient.CreaDelegaForm() });
		$('#service_bar').append(nuovadelega);
		
		$('#service_bar').show();
		
		///se non sto assumendo una delega
		if(!this.model.DelegaEsercitata){
		
			var multiplebutton = $('<div class="multiple_button"><ul><li class="on">Ricevute</li><li>Assegnate</li></div>');
			
			$('#service_bar').append(multiplebutton);
			//aggiungo le azioni ai LI
			$('#service_bar > .multiple_button > ul > li').each(function(i,el){
					$(el).click(function(){
						
						$('#deleghe').find('.lista').parent().addClass('none');
						$($('#deleghe').find('.lista').parent()[i]).removeClass('none');
											
						//spengo il LI acceso e accendo quello cliccato
						$(this).parent().children('li').removeClass("on");
						$(this).addClass("on");
					})
			})
			
			
			
			//aggiungo le info sulla persona
			$('#deleghe').append('<div class="info_pers"></div>');
	
			$('#deleghe > .info_pers').append('<p>'+this.model.DescrUtente+'</p>');
			$('#deleghe > .info_pers').append('<p>'+this.model.DescrRuolo+'</p>');
			
			//aggiungo la lista delle deleghe ricevute
			$('#deleghe').append('<div class="Dricevute"></div>');
			$('#deleghe > .Dricevute').append('<ul class="lista"></ul>');
			
			$(this.model.TabModel.DelegheRicevute).each(function(i,el){
				
				
				
				var li = $('<li><input type="radio" name="Dricevuta" id="DR'+el.Id+'" value="'+el.Id+'|'+el.IdRuoloDelegante+'|'+el.CodiceDelegante+'" /><label for="DR'+el.Id+'"><strong>'+el.RuoloDelegato+'</strong><br/><em>'+el.Delegante+'</em></label><div class="clear"><br class="clear"/></div></li>');
				
				$('#deleghe > .Dricevute > UL').append(li);
				
				
			});
			var attiva_delega = $('<div class="pulsante_classic_big">Attiva Delega</div>').click(function () {
				
				if($('#deleghe > .Dricevute > UL').find(":checked").size()>0){
					var Id = $('#deleghe > .Dricevute > UL').find(":checked").attr('value').split('|')[0];
					var IdRuoloDelegante = $('#deleghe > .Dricevute > UL').find(":checked").attr('value').split('|')[1];
					var CodiceDelegante = $('#deleghe > .Dricevute > UL').find(":checked").attr('value').split('|')[2];
					
					mobileclient.accettaDelega(Id,IdRuoloDelegante,CodiceDelegante);
				}
				
			});
			$('#deleghe > .Dricevute').append(attiva_delega);
			
			
			//aggiungo la lista delle deleghe assegnate
			$('#deleghe').append('<div class="Dassegnate none"></div>');
			$('#deleghe > .Dassegnate').append('<ul class="lista"></ul>');
			
			$(this.model.TabModel.DelegheAssegnate).each(function(i,el){
				
				
				
				var li = $('<li><input type="checkbox" name="Dassegnata" id="DA'+el.Id+'" value="'+el.Id+'" /><label for="DA'+el.Id+'"><strong>'+el.RuoloDelegato+'</strong><br/><em>'+el.Delegato+'</em></label><div class="clear"><br class="clear"/></div></</li>');
				
				$('#deleghe > .Dassegnate > UL').append(li);
	
				
				
			});
			
			var revoca_delega = $('<div class="pulsante_classic_big red">Revoca Delega</div>').click(function () { mobileclient.revocaDelega() });
			$('#deleghe > .Dassegnate').append(revoca_delega);
			
			
			//accendo il tab scelto
			if(!mobileclient.tipodelega) mobileclient.tipodelega = 0;
			
			$($('#service_bar > .multiple_button > ul > li')[mobileclient.tipodelega]).trigger('click');
		}
		else{
			//se sto esercitando una delega
			
			var attiva = $('<div class="pulsante_classic fll on"><a href="javascript:;">Attiva</a></div>');
			$('#service_bar').append(attiva);
			
			//aggiungo le info sulla persona
			$('#deleghe').append('<div class="info_pers"></div>');
	
			$('#deleghe > .info_pers').append('<p>'+this.model.DelegaEsercitata.Delegato+'</p>');
			//$('#deleghe > .info_pers').append('<p>&nbsp;</p>');
			
			//aggiungo le info sul ruolo
			$('#deleghe > .info_pers').append('<p class="info_ruolo"></p>');
	
			$('#deleghe > .info_pers > .info_ruolo').append('<strong>Ruolo delegato:</strong> '+this.model.DelegaEsercitata.RuoloDelegato);
			
			//aggiungo le info della delega
			$('#deleghe').append('<table class="table_deleghe"></table>');
			$('#deleghe > .table_deleghe').append('<tr><th>Nome Delegante:</th><td>'+this.model.DelegaEsercitata.Delegante+'</td></tr>');
			$('#deleghe > .table_deleghe').append('<tr><th>Decorrenza:</th><td>'+this.formatTrasmDate(this.model.DelegaEsercitata.DataDecorrenza)+'</td></tr>');
			$('#deleghe > .table_deleghe').append('<tr><th>Scadenza:</th><td>'+this.formatTrasmDate(this.model.DelegaEsercitata.DataScadenza)+'</td></tr>');
			
			var dismetti_delega = $('<div class="pulsante_classic_big">Dismetti Delega</div>').click(function(){
			tooltip.init({"titolo" : 'Stai per dismettere una delega. Sei sicuro?', "content": '', 'deny': 'NO', 'confirm': 'SI', 'returntrue': function(){mobileclient.dismettiDelega(mobileclient.model.DelegaEsercitata.CodiceDelegante)}})})
			$('#deleghe').append(dismetti_delega);
			
		}
		
		
		$('#deleghe').show();
		
	},
    upFolder: function () {
		page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/UpFolder", {}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    upRicFolder: function () {
		page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/UpFolder", {}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    inFolder: function (idFasc, nameFasc, idTrasm) {
		page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc, "idTrasm": idTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
	inRicFolder: function (idFasc, nameFasc) {
		page.loading(true);

        $.post(this.context.WAPath + "/Ricerca/EnterInFolder", { "idFolder": idFasc, "nameFolder": nameFasc}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    cambiaRuolo: function (idRuolo) {
		page.loading(true);
        $.post(this.context.WAPath + "/ToDoList/ChangeRole", { "idRuolo": idRuolo }, function (data) {mobileclient.model = data; mobileclient.update() }, 'json');
    },
    dettaglioDoc: function (idDoc) {
		page.loading(true);
        $.post(this.context.WAPath + "/Documento/DettaglioDoc", { "idDoc": idDoc }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
	ricerca: function (idRicSalvata,tipoRicSalvata) {
		page.loading(true);
        $.post(this.context.WAPath + "/Ricerca/Ricerca", { "idRicSalvata": idRicSalvata,"tipoRicSalvata": tipoRicSalvata  }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
    getItem: function (_item) {
		
		if (_item.Tipo.DOCUMENTO) {
			if(this.model.TabShow.RICERCA)	this.dettaglioDoc(_item.Id);
			else this.dettaglioDoc(_item.Id, _item.IdTrasm);
			
		} else {
			if(this.model.TabShow.RICERCA)	this.inRicFolder(_item.Id, _item.Oggetto)
			else this.inFolder(_item.Id, _item.Oggetto, _item.IdTrasm)
			
		}
	
    },
    getTrasmission: function (_item) {
		page.loading(true);
        if (_item.Tipo.FASCICOLO) {
            $.post(this.context.WAPath + "/Fascicolo/DettaglioFascTrasm", { "idFasc": _item.Id, "idTrasm": _item.IdTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        } else {
            $.post(this.context.WAPath + "/Documento/DettaglioDocTrasm", { "idDoc": _item.Id, "idTrasm": _item.IdTrasm }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
        }
    },
    getPage: function (URL,numPage) {
		page.loading(true);
        $.post(this.context.WAPath + URL, { "numPage": numPage }, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
    },
	trasmissioneForm : function(docinfo,fascinfo){
		page.loading(true);
		if(docinfo){
        	$.post(this.context.WAPath + "/Trasmissione/TrasmissioneFormDoc", { "idDoc": docinfo.IdDoc}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
		}
		else{
			$.post(this.context.WAPath + "/Trasmissione/TrasmissioneFormFasc", { "idFasc": fascinfo.IdFasc}, function (data) { mobileclient.model = data; mobileclient.update() }, 'json');
		}
		
	},
	eseguiTrasm : function(_item,idTrasmModel,note){
		page.loading(true);
		
		if(_item.IdDoc){
			$.post(this.context.WAPath + '/Trasmissione/EseguiTrasmDoc', {"idTrasmModel" : idTrasmModel, "idDoc": _item.IdDoc,'note': note}, function (data) {
				if(data.Success==true) message = 'Trasmissione effettuata correttamente';
				else message = 'Errore di trasmissione';
				tooltip.init({"titolo" : message,'confirm': 'Ok'});																																																 																																			}, 'json');
		}
		else{
			$.post(this.context.WAPath + '/Trasmissione/EseguiTrasmFasc', {"idTrasmModel" : idTrasmModel, "idFasc": _item.IdFasc,'note': note}, function (data) {
				if(data.Success==true) message = 'Trasmissione effettuata correttamente';
				else message = 'Errore di trasmissione';
				tooltip.init({"titolo" : message,'confirm': 'Ok'});																																																 																																			}, 'json');
		}
	},
	accettaTrasm : function(){
		$.post(this.context.WAPath + '/Trasmissione/AccettaTrasm', {idTrasm:mobileclient.model.TabModel.TrasmInfo.IdTrasm,idTrasmUtente:mobileclient.model.TabModel.TrasmInfo.IdTrasmUtente,note:$('#tooltip > .content > TEXTAREA').val()}, function (data) {mobileclient.model = data; mobileclient.update() }, 'json');
	},
	logout : function(){
		document.location.href=this.context.WAPath + '/Home/Logout';
	},
	 
	rifiutaTrasm : function(){
		$.post(this.context.WAPath + '/Trasmissione/RifiutaTrasm', {idTrasm:mobileclient.model.TabModel.TrasmInfo.IdTrasm,idTrasmUtente:mobileclient.model.TabModel.TrasmInfo.IdTrasmUtente,note:$('#tooltip > .content > TEXTAREA').val()}, function (data) {mobileclient.model = data; mobileclient.update() }, 'json');
	},
	accettaDelega : function(Id,IdRuoloDelegante,CodiceDelegante){
		page.loading(true);
		$.post(this.context.WAPath + '/Delega/AccettaDelega', {"Id": Id, "IdRuoloDelegante": IdRuoloDelegante, "CodiceDelegante": CodiceDelegante}, function (data) {mobileclient.model = data; mobileclient.update() }, 'json');
	},
	dismettiDelega : function(idDelegante){
		page.loading(true);
		$.post(this.context.WAPath + '/Delega/DismettiDelega', { "idDelegante": idDelegante },function (data) {mobileclient.model = data; mobileclient.update() }, 'json');
	},
	CreaDelegaForm :function(){
		page.loading(true);
		$.post(this.context.WAPath + '/Delega/CreaDelegaForm', { },function (data) {mobileclient.model = data; mobileclient.update() }, 'json');	
	},
	CreaDelegaDaModello : function(idModello,dataInizio,dataFine){
		page.loading(true);
		$.post(this.context.WAPath + '/Delega/CreaDelegaDaModello',{"idModello":idModello,"dataInizio":dataInizio,"dataFine" :dataFine},function (data) {
			
		
		//{"Success":true,"Error":null}
		if(data.Success){
			tooltip.init({"titolo" : "Delega creata correttamente",'confirm': 'ok', 'returntrue': function(){mobileclient.update();}})
		}
		else if(data.Error){
			tooltip.init({"titolo" : "Delega non creata",'confirm': 'ok'});
		}
		
		}, 'json');
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
			$(e).parent().children('p').html(attuale);
			$(e).trigger('change');
			
		});
	});		
	myScrollTooltip = new iScroll('listSelect', {desktopCompatibility:true });
}