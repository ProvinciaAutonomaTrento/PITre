

function waitingManagerJS_startWaiting(windowInWaiting,windowInLoading)
{	
	try
	{		
		pf_cursorWaiting(windowInWaiting,windowInLoading);						
		//waitingManagerJS_wait();					
	}
	catch(exc)
	{
		waitingManagerJS_wait();					
	}
}

function pf_cursorWaiting(windowInWaiting,windowInLoading)
{
	if(window.top.waitingServerJS_waitingManager!=undefined)
	{
		
		var objWaitingManager=window.top.waitingServerJS_waitingManager;
		if(objWaitingManager.waitingWindow!=undefined)
		{
			objWaitingManager.waitingWindow.startWaiting(windowInWaiting);
		}
		
		if (objWaitingManager.loadingWindow!=undefined)
		{
			objWaitingManager.loadingWindow.startWaiting(windowInLoading);
		}
	}		
}
			
function waitingManagerJS_waitingComplete()
{
	try
	{
		//alertComplete();		
		var objWaitingManager=window.top.waitingServerJS_waitingManager;
		if(objWaitingManager!=undefined)
		{
			if(objWaitingManager.waitingWindow!=null)
			{
				objWaitingManager.waitingWindow.waitingComplete();					
			}
		}
				
		//alert('waitingManagerJS_waitWindow');
		if (window.top.waitingManagerJS_waitWindow!=null)
		{					
			window.top.waitingManagerJS_waitWindow.close();
		}
	}
	catch(exc)
	{
		;//waitingManagerJS_wait();					
	}
}

function waitingManagerJS_wait()
{
	var l_preferences="width=200,height=25,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no";
	
	window.top.waitingManagerJS_waitWindow=window.open('','',l_preferences);
	var l_document=window.top.waitingManagerJS_waitWindow.document;
		
	//var rowHTML="<body style='cursor:wait;FONT-WEIGHT: normal; FONT-SIZE: 14px; COLOR: #000000; TEXT-INDENT: 0px; FONT-FAMILY: Verdana, Arial, Helvetica, sans-serif'>";
	var rowHTML="<body style='cursor:wait;FONT-WEIGHT: normal; FONT-SIZE: 14px; COLOR: #000000; TEXT-INDENT: 0px; FONT-FAMILY: Verdana, Arial, Helvetica, sans-serif'>";
	l_document.writeln(rowHTML);
		
	var rowHTML="<table width='100%' height='100%'>";
	l_document.writeln(rowHTML);	
			
	var rowHTML="<tr valign='middle'>";
	l_document.writeln(rowHTML);
	
	var rowHTML="<td aling='center'>";
	l_document.writeln(rowHTML);
	
	var rowHTML="Attendere prego....";
	l_document.writeln(rowHTML);
	
	var rowHTML="</td></tr></table></body>";
	l_document.writeln(rowHTML);
	
	//window.setInterval("waitingManagerJS_closeWaitWindow()",3000);
}
