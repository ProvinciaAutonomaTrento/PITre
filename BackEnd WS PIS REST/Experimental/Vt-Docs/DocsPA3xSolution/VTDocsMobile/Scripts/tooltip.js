// JavaScript Document
//tooltip({"titolo" : 'Inserisci note di accettazione', "content": $('<textarea></textarea'), 'deny': 'Annulla', 'confirm': 'Accetta', 'returntrue': function(){}, 'returnfalse': function(){}});
var tooltip = {
    init: function (options) {
	
		var tooltipt = $('<div id="tooltip"></div>');
		var titolo = $('<div class="titolo"></div>').html(options.titolo)
		var content = $('<div class="content"></div>').append(options.content);
		
		var deny = $('<div class="deny"></div>').html(options.deny);
		var confirm = $('<div class="confirm"></div>').html(options.confirm);
		
		if(deny)deny.click(function(){tooltip.close();})
		if(confirm){
			confirm.click(
				function(){
						if(options.returntrue){
							tooltip.close();
							options.returntrue();
						}
						else{
							tooltip.close();
						}
				})
		}
		
		tooltipt.append(titolo);
		if(options.content) tooltipt.append(content);
		
		
		if(!options.deny && options.confirm){
			tooltipt.append(confirm);
			confirm.addClass('center');
		}
		else if(!options.confirm && options.deny){
			tooltipt.append(deny);
			deny.addClass('center');
		}
		else{
			tooltipt.append($('<div class="half"></div>').append(deny));
			tooltipt.append($('<div class="half"></div>').append(confirm));
			tooltipt.append($('<div class="clear"><br class="clear"/></div>'));
		}
		
		//mostro
		page.loading(true)
		$(document.body).append(tooltipt);
		tooltipt.show();
		
		
		
		
		//metto al centro
		var realTooltipWidth = tooltipt.width()+20; /* width + padding css */
		var left = ($(document.body).width()-realTooltipWidth)/2;
		tooltipt.css({'left':Math.ceil(left)+'px'});
		var realTooltipHeight = tooltipt.height()+30; /* height + padding css */
		var top = ($(window).height()-realTooltipHeight)/2;
		
		if(DEVICE == 'galaxy'){
			top = top + $(window).scrollTop();	
		}
		
		tooltipt.css({'top':Math.ceil(top)+'px'});
	},
	close : function(){
		page.loading(false);
		$('#tooltip').remove();
	}
	
	
}