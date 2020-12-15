// This initializes the menu
$(function() {
    $('ul.jd_menu').jdMenu();

    $('ul.jd_menu').trigger('jdMenu')
	    .parents('li:eq(0)')
                .addClass('jdm_no_active')
			.end()
				.find('> li')
                .addClass('jdm_no_active')
});

(function($){
	function addEvents(ul) {
		var settings = $.data( $(ul).parents().andSelf().filter('ul.jd_menu')[0], 'jdMenuSettings' );
  
		$('> li', ul)
			.bind('mouseenter.jdmenu', function(evt) {
                
            })
			.bind('mouseleave.jdmenu', function(evt) {
                if (GetIeVersion()>0 && $('.jdm_active').length==0) $('#frame').css("visibility", "visible");
            })
            .bind('mouseenter.jdmenu mouseleave.jdmenu', function(evt) {
				$(this).toggleClass('jdm_hover');
				var ul = $('> ul', this);
				if ( ul.length == 1 ) {
					clearTimeout( this.$jdTimer );
					var enter = ( evt.type == 'click' );
					//var fn = ( enter ? showMenu : hideMenu );
					this.$jdTimer = setTimeout(function() {
						//fn( ul[0], settings.onAnimate, settings.isVertical );
					}, enter ? settings.showDelay : settings.hideDelay );
				}
			})
			.bind('click.jdmenu', function(evt) {
                if (GetIeVersion()>0) $('#frame').css("visibility", "hidden");

				var ul = $('> ul', this);
				if ( ul.length == 1 && 
					( settings.disableLinks == true || $(this).hasClass('accessible') ) ) {
                    showMenu( ul, settings.onAnimate, settings.isVertical );
					return false;
				}

				
				// The user clicked the li and we need to trigger a click for the a
				if ( evt.target == this ) {
					var link = $('> a', evt.target).not('.accessible');
					if ( link.length > 0 ) {
						var a = link[0];
						if ( !a.onclick) {
							window.open( a.href, a.target || '_self' );
						} else if (a.onclick.toString().indexOf('disallowOp')>0) {
                            disallowOp('menuTop');
                            window.open( a.href, a.target || '_self' );
                        } else {
							$(a).trigger('click');
						}
					}
				}
				if ( (settings.disableLinks && $(this).attr('id')!='LiMenuHome') || 
					( !settings.disableLinks && !$(this).parent().hasClass('jd_menu')) ) {
                    //$(this).parent().jdMenuHide();
					evt.stopPropagation();
				}
			})
			.find('> a')
            
				.bind('focus.jdmenu blur.jdmenu', function(evt) {
					var p = $(this).parents('li:eq(0)');
					if ( evt.type == 'focus' ) {
						p.addClass('jdm_hover');
                        p.removeClass('jdm_no_active');
					} else { 
						p.removeClass('jdm_hover');
                         p.addClass('jdm_no_active');
					}
				})
				.filter('.accessible')
					.bind('click.jdmenu', function(evt) {
						evt.preventDefault();
					});
	}

	function showMenu(ul, animate, vertical) {
		var ul = $(ul);
		if ( ul.is(':visible') ) {
			return;
		}
		//ul.bgiframe();
		var li = ul.parent();
		ul	.trigger('jdMenuShow')
			.positionBy({ 	target: 	li[0], 
							targetPos: 	( vertical === true || !li.parent().hasClass('jd_menu') ? 1 : 3 ), 
							elementPos: 0,
							hideAfterPosition: true
							});
		if ( !ul.hasClass('jdm_events') ) {
			ul.addClass('jdm_events');
			addEvents(ul);
		}
		li	.addClass('jdm_active')
            .removeClass('jdm_no_active')
			// Hide any adjacent menus
			.siblings('li').find('> ul:eq(0):visible')
				.each(function(){
					hideMenu( this ); 
				});
		if ( animate === undefined ) {
			ul.show();
		} else {
			animate.apply( ul[0], [true] );
		}
	}
	
	function hideMenu(ul, animate) {
		var ul = $(ul);
//		$('.bgiframe', ul).remove();
		ul	.filter(':not(.jd_menu)')
			.find('> li > ul:eq(0):visible')
				.each(function() {
					hideMenu( this );
				})
			.end();
		if ( animate === undefined ) {
			ul.hide();
            if (GetIeVersion()>0) $('#frame').css("visibility", "visible");
		} else {
			animate.apply( ul[0], [false] );
		}

		ul	.trigger('jdMenuHide')
			.parents('li:eq(0)')
				.removeClass('jdm_active jdm_hover')
                .addClass('jdm_no_active')
			.end()
				.find('> li')
				.removeClass('jdm_active jdm_hover')
                .addClass('jdm_no_active')
	}
	
	// Public methods
	$.fn.jdMenu = function(settings) {
		// Future settings: activateDelay
		var settings = $.extend({	// Time in ms before menu shows
									showDelay: 		200,
									// Time in ms before menu hides
									hideDelay: 		500,
									// Should items that contain submenus not 
									// respond to clicks
									disableLinks:	true
									// This callback allows for you to animate menus
									//onAnimate:	null
									}, settings);
		if ( !$.isFunction( settings.onAnimate ) ) {
			settings.onAnimate = undefined;
		}
		return this.filter('ul.jd_menu').each(function() {
			$.data(	this, 
					'jdMenuSettings', 
					$.extend({ isVertical: $(this).hasClass('jd_menu_vertical') }, settings) 
					);
			addEvents(this);
		});
	};
	
//	$.fn.jdMenuUnbind = function() {
//		$('ul.jdm_events', this)
//			.unbind('.jdmenu')
//			.find('> a').unbind('.jdmenu');
//	};
	$.fn.jdMenuHide = function() {
		return this.filter('ul').each(function(){ 
			hideMenu( this );
		});
	};

	// Private methods and logic
	$(document)
		// Bind a click event to hide all visible menus when the document is clicked
		.bind('click.jdmenu', function(){
			$('ul.jd_menu ul:visible').jdMenuHide();
		});
})(jQuery);
