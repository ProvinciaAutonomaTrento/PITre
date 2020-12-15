/*
	POP UP DI ATTESA OPERAZIONE
*/

// Progress
var countProgressMsg = 0;
var msgProgressStack = new Array(50);
var wndProgress = null;
var wndProgressShow = false;
var wndProgressWidth = (screen.width < 700) ? screen.width : 700;
var wndProgressHeight = 86;
var wndProgressTop = (screen.width - wndProgressWidth) / 2;
var wndProgressLeft = (screen.height - wndProgressHeight) / 2;
var modeProgress = "OUT-POPUP";  // OFF - nessun messaggio di attesa, IN - In alto al posto del menu principale, OUT-POPUP - Come popup non modale, OUT-WIN - Come finestra non modale
// Msgbox
var wndMsgBox = null;
var wndMsgBoxShow = false;
var modeMsgBox = "OUT-POPUP";  // OFF - nessun messaggio di attesa, IN - In alto al posto del menu principale, OUT-POPUP - Come popup non modale, OUT-WIN - Come finestra non modale
var wndMsgBoxWidth = (screen.width < 700) ? screen.width : 700;
var wndMsgBoxHeight = 86;
var wndMsgBoxTop = (screen.width - wndMsgBoxWidth) / 2;
var wndMsgBoxLeft = (screen.height - wndMsgBoxHeight) / 2;


var __oldDoPostBack;
var _buttonClicked = "";
var _textOperation;
var _id = null;
var _multipleOperation = false; // lasciare a false per il momento, a true è instabile.
var _excludeGenericMsg = true;


/*

	PROGRESS
	
*/

function checkIsInProgress()
{
	if (countProgressMsg > 0) 
	{
		internalShowProgress(msgProgressStack[countProgressMsg-1]);
		return true;
	}
	else
		return false;
}

function internalShowProgress(msg)
{
		var rect;
		
		document.frames["progress"].setTitle(msg.title);
		document.frames["progress"].setSubTitle(msg.subTitle);
		if (modeProgress == "OUT-WIN")
		{
			if (!wndProgressShow)
			{
				wndProgress = window.open("",'_blank','fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');
			}
			wndProgress.document.body.innerHTML = document.frames["progress"].document.body.innerHTML;
			wndProgress.document.body.style.border = "solid #810d06 2px";
			rect = document.frames["progress"].document.all['container'].getBoundingClientRect();
			
			// Resize and locate
			wndProgressWidth = rect.right - rect.left;
			wndProgressHeight = rect.bottom - rect.top;
			wndProgressTop = (screen.width - wndProgressWidth) / 2;
			wndProgressLeft = (screen.height - wndProgressHeight) / 2;
			wndProgress.resizeTo(wndProgressWidth, wndProgressHeight);
			wndProgress.moveTo(wndProgressLeft, wndProgressTop);
			
		}
		if (modeProgress == "OUT-POPUP")
		{
			if (!wndProgressShow)
				wndProgress = window.createPopup();
			
			wndProgress.document.body.innerHTML = document.frames["progress"].document.body.innerHTML;
			wndProgress.document.body.style.border = "solid #810d06 2px";
			rect = document.frames["progress"].document.all['container'].getBoundingClientRect();
			
			// Resize and locate
			wndProgressWidth = rect.right - rect.left; 
			wndProgressHeight = rect.bottom - rect.top;
			wndProgressTop = (screen.width - wndProgressWidth) / 2;
			wndProgressLeft = (screen.height - wndProgressHeight) / 2;

			if (wndProgressShow)
			    wndProgress.hide();
		    
			wndProgress.show(wndProgressTop, wndProgressLeft, wndProgressWidth, wndProgressHeight);

		}
		if ( (modeProgress == "IN") && (countProgressMsg > 0) )
			document.all["topFrame"].rows = "86,0,0,*,17,0";

}

function showProgress(msg)
{	
	if ((modeProgress != "OFF") && (countProgressMsg < 50))
	{
		msgProgressStack[countProgressMsg] = msg;
		countProgressMsg++;
		internalShowProgress(msg);
	}	
}

function hideProgress(id)
{
	var i;
	var msg = null;

	if ( (modeProgress != "OFF") && (countProgressMsg > 0) )
	{
				
		if (id == null)
		{
			countProgressMsg = 0;
			for (i = 0; i < 50; i++);
				msgProgressStack[i] = null;
		}
		else
		{
			id = id.toUpperCase();
			if (msgProgressStack[countProgressMsg-1].id != id) 
			{
				// retrieve msg object
				for(i = 0; i < countProgressMsg-1; i++)
					if (msgProgressStack[i].id == id)
					{
						msgProgressStack[i] = null;
						//compress
						for (u =i + 1; u < countProgressMsg; u++)
							msgProgressStack[u-1] = msgProgressStack[u];
						countProgressMsg--;
						break;
					}
			}
			else
			{	
				countProgressMsg--;
				msgProgressStack[countProgressMsg] = null;
				if (countProgressMsg != 0)
				{
					msg = msgProgressStack[countProgressMsg-1];
					internalShowProgress(msg);
				}
			}
		}
				
		if ( ( (modeProgress == "OUT-WIN") || (modeProgress == "OUT-POPUP") ) && (countProgressMsg == 0))
		{
			if (wndProgress != null)
			{
				if (modeProgress == "OUT-POPUP") 
					try { wndProgress.hide(); } catch(e) {}
				else
					try { wndProgress.close(); } catch(e) {}
				wndProgress = null;
			}	
		}
		
		if ((modeProgress == "IN") && (countProgressMsg == 0))
			document.all["topFrame"].rows = "0,0,86,*,17,0";
	}
}

/*

	MSG BOX 
	
*/
/*
function internalShowMsgBox(msg)
{
	var rect

	document.frames["msgbox"].setTitle(msg.title);
	document.frames["msgbox"].setSubTitle(msg.subTitle);
	document.frames["msgbox"].setIcon(msg.icon);

	if (modeMsgBox == "OUT-WIN")
	{
		if (!wndMsgBoxShow)
		{
			wndMsgBox=window.open("",'_blank','fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');
		}
		wndMsgBox.document.body.innerHTML = document.frames["msgbox"].document.body.innerHTML;
		wndMsgBox.document.body.style.border = "solid #810d06 2px";
		rect = document.frames["msgbox"].document.all['container'].getBoundingClientRect();
		
		// Resize and locate
		wndMsgBoxWidth = rect.right - rect.left;
		wndMsgBoxHeight = rect.bottom - rect.top;
		wndMsgBoxTop = (screen.width - wndMsgBoxWidth) / 2;
		wndMsgBoxLeft = (screen.height - wndMsgBoxHeight) / 2;
		wndMsgBox.resizeTo(wndMsgBoxWidth, wndMsgBoxHeight);
		wndMsgBox.moveTo(wndMsgBoxLeft, wndMsgBoxTop);

	}
	if (modeMsgBox == "OUT-POPUP")
	{
		if (!wndMsgBoxShow)
			wndMsgBox = window.createPopup();
		wndMsgBox.document.body.innerHTML = document.frames["msgbox"].document.body.innerHTML;
		wndMsgBox.document.body.style.border = "solid #810d06 2px";
		rect = document.frames["msgbox"].document.all['container'].getBoundingClientRect();

		// Resize and locate
		wndMsgBoxWidth = rect.right - rect.left;
		wndMsgBoxHeight = rect.bottom - rect.top;
		wndMsgBoxTop = (screen.width - wndMsgBoxWidth) / 2;
		wndMsgBoxLeft = (screen.height - wndMsgBoxHeight) / 2;

		if (!wndMsgBoxShow)
			wndMsgBox.show(wndMsgBoxTop, wndMsgBoxLeft, wndMsgBoxWidth, wndMsgBoxHeight);
	}
	if (modeMsgBox == "IN") 
		document.all["topFrame"].rows = "0,86,0,*,17,0";

}

function showMsgBox(msg)
{	
	if (modeMsgBox != "OFF")
	{
		internalShowMsgBox(msg);
	}	
}

function hideMsgBox()
{
	if (modeMsgBox != "OFF") 
	{
				
		if ( (modeMsgBox == "OUT-WIN") || (modeMsgBox == "OUT-POPUP") )
		{
			if (wndMsgBox != null)
			{
				if (modeMsgBox == "OUT-POPUP") 
					try { wndMsgBox.hide(); } catch(e) {}
				else
					try { wndMsgBox.close(); } catch(e) {}
				wndMsgBox = null;
			}	
		}
		
		if (modeMsgBox == "IN") 
			document.all["topFrame"].rows = "0,0,86,*,17,0";
	}
}*/


//Routine di accesso a livello di singola pagina per l'attivazione del messeggaio di attesa.


function progressMsg(id, title, subTitle)
{
	this.id = id.toUpperCase();
	this.title = title
	this.subTitle = subTitle;
	this.exclude = false;
}

function msgBoxMsg(icon, title, subTitle)
{
	this.icon = icon;
	this.title = title;
	this.subTitle = subTitle;
}

function InitOperationMessage(attachOnSubmit)
{
	//hideWorkingInProgress();
	if (typeof(__doPostBack) != "undefined")
	{
		// save a reference to the original __doPostBack
		__oldDoPostBack = __doPostBack;
		// replace __doPostBack with another function
		__doPostBack = myDoPostBack;
	}
	
	if (attachOnSubmit)
		document.forms[0].onsubmit = beforeSubmit;
}

/*
function InitOperationMessageT(attachOnSubmit)
{
	hideWorkingInProgress("risultatiRic");
	//if (attachOnSubmit)
	//	document.forms[0].onsubmit = beforeSubmit;
	
}*/
function beforeSubmit()
{
    if ( !window.top.checkIsInProgress() || (_multipleOperation == true) )
	{
		var msg = getOpDescription(_buttonClicked);
		
		if (msg != null)
			if (!msg.exclude)
				showWorkingInProgress(msg);
		return true;
	}
	else
		return false;
}
function myDoPostBack(eventTarget, eventArgument)
{
	if ( !window.top.checkIsInProgress() || (_multipleOperation == true) )
	{
		var msg = getOpDescription(eventTarget);
	
		if (msg != null)
			if (!msg.exclude)
				showWorkingInProgress(msg);
		return __oldDoPostBack (eventTarget, eventArgument);
	}
	else
		return false;
}
function getOpDescription(opCode)
{
	var msg = new progressMsg(_id,"Operazione in corso...","L'operazione richiesta è in corso, attendere prego.");
	msg.exclude = _excludeGenericMsg;
	try
	{
		if ((typeof(_textOperation[opCode]) == 'undefined') || (_textOperation[opCode] == null)) 
			return msg;
		else
			return _textOperation[opCode];
	}
	catch(e)
	{
		return msg;
	}
}

function showWorkingInProgress(msg)
{
    if (msg == "")
    {
        msg = getOpDescription(_buttonClicked);
	}	
	window.top.showProgress(msg);
}

function hideWorkingInProgress(id)
{	

    if (typeof(id) == 'undefined')
		window.top.hideProgress(_id);
	else
		window.top.hideProgress(id);
}

function showWarning(msgText)
{
	if (window.top.modeMsgBox == "OFF")
		alert(msgText);
	else
		window.top.showMsgBox(new msgBoxMsg(1,"Attenzione!!!",msgText));
}
function showComplete(msgText)
{
	if (window.top.modeMsgBox == "OFF")
		alert(msgText);
	else
		window.top.showMsgBox(new msgBoxMsg(2,"Completato!!!",msgText));
}

function getQuestion(sQuestion)
{

	var args = new String(2);
	args[0] = sQuestion;

	if (window.opener != null)
		args[1] = window.opener.top.baseUrl;
	else
		args[1] = window.top.baseUrl;
		
	if (args[1] == null)
		args[1] = window.parent.document.frames[0].baseUrl;

	var res = window.showModalDialog(args[1] + "/Question.htm",args,"dialogHide:yes;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes");
	if (typeof(res) == 'undefined')
		return false;
	else
		return res;
}

function originWindow(oWnd)
{
	wnd = oWnd;
	if ( (wnd == null) || (typeof(wnd) == 'undefined') )
		return null;
	
	while ((wnd.top != null) && (wnd.top != wnd))
		wnd = wnd.top;
	
	while (wnd.opener != null)
		wnd = wnd.opener;
		
		
		//alert(wnd.location.href);
	return wnd;
	
}
