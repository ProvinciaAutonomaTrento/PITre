// JavaScript Document

var myScroll;
var timer;

var page = {
    loading: function (mode) {
        $(document.body).append('<div id="loading">loading...</div>');
        if (mode == true) $('#loading').show();
        else $('#loading').remove();
    }
}
var tabletMinW=700;
var smartMinW=450;
var headerH;
var wrapperH;

var preventBehavior = function(e) {
	  e.preventDefault();
	};
	
	function loaded() {
		//checkHeight()
		//timer = setInterval('setHeight()',50);		
		//myScroll = new iScroll('scrollerLogin', {desktopCompatibility:true });
		
	}
 
	// Enable fixed positioning
	document.addEventListener("touchmove", preventBehavior, false);
	document.addEventListener('DOMContentLoaded', loaded, false);

function setHeight() {
		document.getElementById('wrapperLogin').style.height = wrapperH + 'px';
		//if(myScroll){	myScroll.refresh();	}
	}
	
function checkHeight(){
		headerH = document.getElementById('header_login').offsetHeight;
		wrapperH = window.innerHeight - headerH ;
	
	}

$(window).resize ( function (e) {
	if(($(window).width()<=tabletMinW)&&($(window).width()>smartMinW)){
		DEVICE='galaxy';
	}else if(($(window).width()<=smartMinW) ) {
		DEVICE='smart';
	} else {
		DEVICE='ipad';
	}
	mobileclient.iPhoneGest();
	//checkHeight()
})


var init = function () {
	
	// Determine if we on iPhone or iPad
	var isiOS = false;
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
	
	mobileclient.iPhoneGest();

    var password = getCookie("VTDOCS_MOBILE_PASSWORD");
    var username = getCookie("VTDOCS_MOBILE_USERNAME");
    if (password) $('#password').val(password);
    if (username) $('#username').val(username);
	
	//myScroll = new iScroll('scrollerLogin', {desktopCompatibility:true });
	
	
}

var mobileclient = {
	iPhoneGest: function () {
		if(DEVICE=='smart'){
			$('.iPhoneDouble').attr('colspan',2);
		}else{
			$('.iPhoneDouble').attr('colspan',1);
		}
	
	}
}

var submit_form = function () {

    //check dei campi
    var form = $('#vtportal_login');

    var username = $('#username').attr('value');
    var password = $('#password').attr('value');

    if (username == '' && password == '') {
        tooltip.init({ "titolo": 'username e password devono essere compilati', 'confirm': 'OK' });
        return false;
    }
    else if (username == '') {
        tooltip.init({ "titolo": 'l\'username deve essere compilato', 'confirm': 'OK' });
        return false;
    }
    else if (password == '') {
        tooltip.init({ "titolo": 'la password deve essere compilata', 'confirm': 'OK' });
        return false;
    }
    else {
        var remPass = $("#remember_password:checked");
        if (remPass.val() != null) {
            createCookie("VTDOCS_MOBILE_PASSWORD", password);
            createCookie("VTDOCS_MOBILE_USERNAME", username);
        }
        document.getElementById('vtportal_login').submit();
    }
}

function createCookie(cookieName, cookieValue) {
    var theDate = new Date();
    theDate.setFullYear(theDate.getFullYear() + 1);
    cookieName = escape(cookieName);
    cookieValue = escape(cookieValue);
    if (document.cookie != document.cookie)
    { index = document.cookie.indexOf(cookieName); }
    else
    { index = -1; }

    if (index == -1) {
        document.cookie = cookieName + "=" + cookieValue + "; expires=" + theDate;
    }
}

function getCookie(name) {
    name = escape(name)
    if (document.cookie) {
        index = document.cookie.indexOf("; " + name + "=");
        if (index < 0 && document.cookie.indexOf(name + "=") == 0) index = -2;
        else if (index < 0) return false;
        index += 2;
        if (index != -1) {
            cookieNameStart = (document.cookie.indexOf("=", index) + 1);
            cookieNameEnd = document.cookie.indexOf(";", index);
            if (cookieNameEnd == -1) { cookieNameEnd = document.cookie.length; }
            return unescape(document.cookie.substring(cookieNameStart, cookieNameEnd));
        }
    }
}


//blocco lo scroll e carico le immagini
if($.browser.webkit) {
	var preventBehavior = function(e) {
	  e.preventDefault();
	};
	// Enable fixed positioning
	document.addEventListener("touchmove", preventBehavior, false);
}
$(document).ready(function() {
if(Immagini.length!=0){
	var contatore = 0;
	for(var i=0;i<Immagini.length;i++){
		var img = $('<img src=""/>');
		img.attr('src',Immagini[i]);
		$('#load_image').append(img);
		$(img).load(function(){
			
			contatore++;
	
			if(contatore==Immagini.length-1){
				$('#splash').hide();	
				//checkHeight()
			}
		});
		
		
	}	
}
else{
	$('#splash').hide();	
	//checkHeight()
}
});
