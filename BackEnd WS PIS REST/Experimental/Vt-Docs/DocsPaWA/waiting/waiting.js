function WaitingManager(webAppUrl)
{	
	//Proprietà
	//this.baseUrl=null;
	this.baseUrl=webAppUrl;
	
	this.waitingWindow=new Waiting();
	this.waitingWindow.parent=this;
	
	this.loadingWindow=new Waiting();
	this.loadingWindow.parent=this;
}

function Waiting()
{
	//Proprietà
	this.parent=null;
	this.window=null;
	this.error=null;
		
	//metodi
	this.startWaiting=pf_startWaiting;	
	this.waitingComplete=pf_waitingComplete;
	this.changeCursor=pf_changeForChilds;		
}

function pf_changeForChilds(win,cursor)
{	
	try
	{
		//cambia il cursore della finestra
		pf_changeCursor(win,cursor);		
			
		//Cicla sugli iFrame di primo livello
		if (win.document!=null)
		{
			for(var t_index_lev1=0;t_index_lev1<win.document.frames.length;t_index_lev1++)
			{		
				try
				{
					var newWin_Lev1=win.document.frames(t_index_lev1);		
					pf_changeCursor(newWin_Lev1,cursor);		
					
					//Cicla sugli iFrame di secondo livello
					if (newWin_Lev1.document!=null)
					{
						for(var t_index_lev2=0;t_index_lev2<newWin_Lev1.document.frames.length;t_index_lev2++)
						{		
							try
							{
								var newWin_Lev2=newWin_Lev1.document.frames(t_index_lev2);
								pf_changeCursor(newWin_Lev2,cursor);		
								
								//Cicla sugli iFrame di terzo livello
								if (newWin_Lev2.document!=null)
								{	
									for(var t_index_lev3=0;t_index_lev3<newWin_Lev2.document.frames.length;t_index_lev3++)
									{		
										try
										{
											var newWin_Lev3=newWin_Lev2.document.frames(t_index_lev3);
											pf_changeCursor(newWin_Lev3,cursor);			
										
											//Cicla sugli iFrame di quarto livello
											if (newWin_Lev3.document!=null)
											{
												for(var t_index_lev4=0;t_index_lev4<newWin_Lev3.document.frames.length;t_index_lev4++)
												{		
													try
													{
														var newWin_Lev4=newWin_Lev3.document.frames(t_index_lev4);
														pf_changeCursor(newWin_Lev4,cursor);			
														
														//Cicla sui Frame di quinto livello
														if (newWin_Lev4.document!=null)
														{
															for(var t_index_lev5=0;t_index_lev5<newWin_Lev4.document.frames.length;t_index_lev5++)
															{		
																try
																{
																	var newWin_Lev5=newWin_Lev4.document.frames(t_index_lev5);
																	pf_changeCursor(newWin_Lev5,cursor);			
																}
																catch(exc)
																{
																	continue;
																}
															}
														}
													}
													catch(exc)
													{
														continue;
													}	
												}
											}
										}
										catch(exc)
										{
											continue;
										}				
									}								
								}
							}
							catch(exc)
							{
								continue;
							}			
						}			
					}
				}
				catch(exc)
				{
					continue;
				}
			}
		}
		
		//Cicla sui Frame di primo livello
		for(var t_index_lev1=0;t_index_lev1<win.frames.length;t_index_lev1++)
		{		
			var newWin_Lev1=win.frames(t_index_lev1);		
			pf_changeCursor(newWin_Lev1,cursor);		
			
			//Cicla sui Frame di secondo livello
			for(var t_index_lev2=0;t_index_lev2<newWin_Lev1.frames.length;t_index_lev2++)
			{		
				var newWin_Lev2=newWin_Lev1.frames(t_index_lev2);
				pf_changeCursor(newWin_Lev2,cursor);		
				
				//Cicla sui Frame di terzo livello
				for(var t_index_lev3=0;t_index_lev3<newWin_Lev2.frames.length;t_index_lev3++)
				{		
					var newWin_Lev3=newWin_Lev2.frames(t_index_lev3);
					pf_changeCursor(newWin_Lev3,cursor);			
				
					//Cicla sui Frame di quarto livello
					for(var t_index_lev4=0;t_index_lev4<newWin_Lev3.frames.length;t_index_lev4++)
					{		
						var newWin_Lev4=newWin_Lev3.frames(t_index_lev4);
						pf_changeCursor(newWin_Lev4,cursor);			
						
						//Cicla sui Frame di quinto livello
						for(var t_index_lev5=0;t_index_lev5<newWin_Lev4.frames.length;t_index_lev5++)
						{		
							var newWin_Lev5=newWin_Lev4.frames(t_index_lev5);
							pf_changeCursor(newWin_Lev5,cursor);			
						}	
					}				
				}								
			}			
		}
	}
	catch(exc)
	{;} 
}

function pf_startWaiting(windowInWaiting)
{
	try
	{		
		this.window=windowInWaiting;

		if (windowInWaiting!=null)
		{	
			//gestori eventi
			try
			{
				this.window.detachEvent('onload',eventHandler_onload);
			}
			catch(exc)
			{;}
			
			this.window.attachEvent('onload',eventHandler_onload);					
		}
		
		this.changeCursor(this.window,this.parent.baseUrl+'/waiting/waiting.cur');
	}
	catch(exc)
	{;}
}


function pf_waitingComplete()
{
	try
	{
		this.changeCursor(this.window,'default');	
		this.window=null;
	}
	catch(exc)
	{;}
}

function pf_changeCursor(win,newCursor)
{
	try
	{		
		if (win!=null && win.document!=null && win.document.body!=null)
		{
			//alert('changeCursor in '+this.window.name);
			//alert(location.host+' '+location.href+' '+location.pathname);
			win.document.body.style.cursor=newCursor;
		}
	}
	catch(exc)
	{
		this.error=exc;
	}
}

//*******  INIZIO GESTORI DI EVENTO ******************************
function eventHandler_onload()
{
	//in questo punto, l'oggetto 'this' contiene il riferimento
	//alla finestra in cui è stato dichiarato l'oggetto
	
	try
	{
		//Quando l'oggetto intercetta l'evento 'onLoad' della pagina che è
		//stata caricata, viene invocato il metodo della pagina
		//chiamante 'waitingServerJS_waitingComplete'
		//alert('eventHandler_onload');
		this.waitingServerJS_waitingComplete();
	}
	catch(exc)
	{;}
}
//*******  FINE GESTORI DI EVENTO ******************************






//variabile responsabile della gestione del meccanismo di waiting tra le pagine:
//CONTIENE IL RIFERIMENTO GLOBALE AL MANAGER
var waitingManagerJS_waitWindow;
var waitingServerJS_waitingManager;

function waitingServerJS_waitingComplete()
{
	//Questa routine viene invocata dall'oggetto javascript 'WaitingManager'
	//in risposta all'evento 'onload' della pagina in loading:
	//NON MODIFICARNE ASSOLUTAMENTE LA SIGNATURE
	try
	{
		waitingServerJS_waitingManager.waitingWindow.waitingComplete();
		//alert('waitingComplete in '+waitingServerJS_waitingManager.waitingWindow.window.name);
	}
	catch(exc)
	{;}
}


function waitingServerJS_createWaitingManager(baseUrl)
{
	try
	{
		waitingServerJS_waitingManager=new WaitingManager(baseUrl);
	}
	catch(exc)
	{;}
}


