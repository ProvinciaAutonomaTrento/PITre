using System;
using DocsPAWA;
using DocsPAWA.DocsPaWR;
using System.Web;
using System.Collections;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.popup.RubricaDocsPA
{
	public class DPA3_ResultHandler : IResultHandler
	{
        
		private System.Web.UI.Page page;
		private ArrayList trasm_strutture_vuote;
        public ArrayList listaExceptionTrasmissioni1 = new ArrayList();

		public DPA3_ResultHandler(System.Web.UI.Page _page)
		{
			page = _page;
			trasm_strutture_vuote = new ArrayList();
		}
		#region IResultHandler Members


		public void execute(ElementoRubrica[] _A_recipients, ElementoRubrica[] _CC_recipients, DocsPAWA.DocsPaWR.RubricaCallType calltype)
		{
			DocsPaWR.Corrispondente corr;
			DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(page);
			DocsPaWR.ElementoRubrica er = null;
			DocsPaWR.ProtocolloUscita pu = null;
			DocsPaWR.ProtocolloEntrata pe = null;
			DocsPaWR.ProtocolloInterno pi = null;

            //Andrea
            string messError = string.Empty;
            //End Andrea

            
            if (CallContextStack.CurrentContext == null || CallContextStack.CurrentContext.ContextState["TransmissionReasons"] != null)
            {
                Trasmissione trasm = new Trasmissione();

                trasm = new Trasmissione();
                trasm.ruolo = UserManager.getRuolo();
                trasm.utente = UserManager.getUtente();

                trasm.tipoOggetto = DocsPAWA.utils.MassiveOperationUtils.ObjectType;

                // Porcata nececessaria in quanto la rubrica lavora con la sessione... che uso indiscriminato! :)
                TrasmManager.setGestioneTrasmissione(page, trasm);
            }

          //  if (CallContextStack.CurrentContext.ContextState["TransmissionReasons"] != null)
          //  {
          /*      Trasmissione trasm = new Trasmissione();

                trasm = new Trasmissione();
                trasm.ruolo = UserManager.getRuolo();
                trasm.utente = UserManager.getUtente();

                // Porcata nececessaria in quanto la rubrica lavora con la sessione... che uso indiscriminato! :)
                TrasmManager.setGestioneTrasmissione(page, trasm);*/
          //  }

			switch (calltype) 
			{
				case RubricaCallType.CALLTYPE_PROTO_IN:
					pe = (DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo;
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, er.interno ? AddressbookTipoUtente.INTERNO : AddressbookTipoUtente.ESTERNO );

                    if (pe != null && UserManager.esisteCorrispondente(pe.mittenti, corr))
                    {
                        gestisci_scheda_protocollo(schedaDoc);                        
                    }
                    else
                    {
                        pe.mittente = corr;
                        pe.daAggiornareMittente = true;
                        gestisci_scheda_protocollo(schedaDoc);
                        page.RegisterStartupScript("set_mittente_hack",
                            "<script language=\"javascript\">" +
                            "window.dialogArguments.top.principale.document.frames[0].document.frames[0].document.getElementById(\"txt_DescMit_P\").value = \"" + er.descrizione + "\"; " +
                            "window.dialogArguments.top.principale.document.frames[0].document.frames[0].document.getElementById(\"txt_CodMit_P\").value = \"" + er.codice + "\"; " +
                            "</script>");
                    }
					break;

				case RubricaCallType.CALLTYPE_PROTO_IN_INT:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubrica (page, er.codice);
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenteIntermedio = corr;
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittenteIntermedio = true;
					gestisci_scheda_protocollo(schedaDoc);
					break;

				case RubricaCallType.CALLTYPE_PROTO_OUT:
					pu = (DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo;
                    DocsPAWA.DocsPaWR.Corrispondente[] dest = pu.destinatari;
                    DocsPAWA.DocsPaWR.Corrispondente[] destCC = pu.destinatariConoscenza;
                    addDestinatari(ref dest, destCC, _A_recipients, schedaDoc, false);
                    addDestinatari(ref destCC, dest, _CC_recipients, schedaDoc, true);
					pu.daAggiornareDestinatari = true;
                    pu.destinatari = dest;
                    pu.destinatariConoscenza = destCC;
					gestisci_scheda_protocollo(schedaDoc);
					break;

				
				case RubricaCallType.CALLTYPE_TRASM_SUP:
				case RubricaCallType.CALLTYPE_TRASM_INF:
				case RubricaCallType.CALLTYPE_TRASM_ALL:
				case RubricaCallType.CALLTYPE_TRASM_PARILIVELLO:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}

					string t_avviso = "";
					

					DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(page);
					DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(page);
					foreach (DocsPAWA.DocsPaWR.ElementoRubrica e in _A_recipients) 
					{
						//Liste di distribuzione
						if(e.tipo == "L")
						{
							//Controlla se a tutti gli elementi della lista è possibile trasmettere
							//filtrandoli per la ragione di trasmissione selezionata 
							DocsPaWR.ParametriRicercaRubrica qr = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
							UserManager.setQueryRubricaCaller (ref qr);

							if(trasmissione.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
							{
								//se è un documento
								qr.ObjectType = schedaDoc.tipoProto;
							}
							else
							{
								// fascicolo
								qr.ObjectType = "F:";
								if(fasc!=null)
									qr.ObjectType = qr.ObjectType + fasc.idClassificazione;
							}

							DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this.page);
							string gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0,1);
					
							switch(gerarchia_trasm)
							{
								case "T":
									qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;							
									break;					
								case "I":
									qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF;							
									break;					
								case "S":
									qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP;							
									break;							
								case "P":
									qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO;
									break;		
							}
                            string idAmm = UserManager.getInfoUtente().idAmministrazione;
							ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(page,e.codice,idAmm);
							DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(page);
							ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
							for(int i=0; i<listaCorr.Count; i++)
							{
								DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
								DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente) listaCorr[i];
								er_1 = UserManager.getElementoRubrica(this.page,c.codiceRubrica);
                                if (!er_1.disabledTrasm)
                                    ers[i] = er_1;                                
							}
							int coutStartErs = ers.Length;
						//	ElementoRubrica[] ers_1 = UserManager.filtra_trasmissioniPerListe(this.page,qr,ers);
                            ElementoRubrica[] ers_1 = ers;
							for(int i=0; i<listaCorr.Count; i++)
							{
								DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente) listaCorr[i];
								for(int j=0; j<ers_1.Length; j++)
								{
									if(c.codiceRubrica == ers_1[j].codice)
									{
										trasmissioni = addTrasmissioneSingola(trasmissione, c);
										break;
									}
								}
							}

                            if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                            {
                                //string messError = string.Empty;
                                //Andrea
                                foreach (string s in listaExceptionTrasmissioni1)
                                {
                                    messError = messError + s + "\\n";
                                }
                                //HttpContext.Current.Response.Write("<script>alert('AVVISO: I seguenti destinatari saranno esclusi dalla Nuova Trasmissione: \\n" + messError + "\\n');</script>");
                            }    
							
							TrasmManager.setGestioneTrasmissione(page, trasmissioni);							
						}
						//Fine liste di distribuzione
						else
						{
                            // nel caso in cui si sia selezionato un RF da rubrica
                            if (e.tipo.ToUpper().Equals("F"))
                            {
                                //Controlla se a tutti gli elementi dell'RF è possibile trasmettere
                                //filtrandoli per la ragione di trasmissione selezionata 
                                DocsPaWR.ParametriRicercaRubrica qr = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
                                UserManager.setQueryRubricaCaller(ref qr);

                                if (trasmissione.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
                                {
                                    //se è un documento
                                    qr.ObjectType = schedaDoc.tipoProto;
                                }
                                else
                                {
                                    // fascicolo
                                    qr.ObjectType = "F:";
                                    if (fasc != null)
                                        qr.ObjectType = qr.ObjectType + fasc.idClassificazione;
                                }

                                DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this.page);
                                string gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);

                                switch (gerarchia_trasm)
                                {
                                    case "T":
                                        qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;
                                        break;
                                    case "I":
                                        qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF;
                                        break;
                                    case "S":
                                        qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP;
                                        break;
                                    case "P":
                                        qr.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO;
                                        break;
                                }
                                
                                string sysRF = e.systemId;

                                 ArrayList listaCorr = UserManager.getCorrispondentiByCodRF(page, e.codice);
                                 DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(page);
                                 ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
                                 for (int i = 0; i < listaCorr.Count; i++)
                                 {
                                     DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
                                     DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                                     er_1 = UserManager.getElementoRubrica(this.page, c.codiceRubrica);
                                     ers[i] = er_1;
                                 }
                                 int coutStartErs = ers.Length;
                                // ElementoRubrica[] ers_1 = UserManager.filtra_trasmissioniPerListe(this.page, qr, ers);
                                 ElementoRubrica[] ers_1 = ers;
                                 for (int i = 0; i < listaCorr.Count; i++)
                                 {
                                     DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                                     for (int j = 0; j < ers_1.Length; j++)
                                     {
                                         if (c.codiceRubrica == ers_1[j].codice)
                                         {
                                             trasmissioni = addTrasmissioneSingola(trasmissione, c);
                                             break;
                                         }
                                     }
                                 }

                                 if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                                 {
                                     //string messError = string.Empty;
                                     //Andrea
                                     foreach (string s in listaExceptionTrasmissioni1)
                                     {
                                         messError = messError + s + "\\n";
                                     }
                                     //HttpContext.Current.Response.Write("<script>alert('AVVISO: I seguenti destinatari saranno esclusi dalla Nuova Trasmissione: \\n" + messError + "\\n');</script>");
                                 }    

                                 TrasmManager.setGestioneTrasmissione(page, trasmissioni);
                            }
                            else
                            {
                                DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
                                qco.codiceRubrica = e.codice;
                                qco.getChildren = false;
                                qco.idAmministrazione = UserManager.getInfoUtente(page).idAmministrazione;
                                qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                                qco.fineValidita = true;

                                try
                                {
                                    corr = UserManager.getListaCorrispondenti(page, qco)[0];
                                    trasmissione = addTrasmissioneSingola(trasmissione, corr);
                                    if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                                    {
                                        //string messError = string.Empty;
                                        //Andrea
                                        foreach (string s in listaExceptionTrasmissioni1)
                                        {
                                            messError = messError + s + "\\n";
                                        }
                                        listaExceptionTrasmissioni1 = new ArrayList();
                                        //HttpContext.Current.Response.Write("<script>alert('AVVISO: I seguenti destinatari saranno esclusi dalla Nuova Trasmissione: \\n" + messError + "\\n');</script>");
                                    }    
                                }
                                catch (Exception ex)
                                {
                                        HttpContext.Current.Response.Write("<script>alert('" + qco.codiceRubrica + ": Destinatario non interno o senza ruolo associato!');</script>");
                                        Console.WriteLine(ex.Message);
                                }
                            }
						}						
					}
                    if (!string.IsNullOrEmpty(messError))
                    {
                        HttpContext.Current.Response.Write("<script>alert('AVVISO: I seguenti destinatari saranno esclusi dalla Nuova Trasmissione: \\n" + messError + "\\n');</script>");
                    }
                    TrasmManager.setGestioneTrasmissione(page, trasmissione);

					if (trasm_strutture_vuote.Count > 0) 
					{
						if(t_avviso == "")
						{
							foreach (string s in trasm_strutture_vuote)
								t_avviso += (" - " + s + "\\n");
						
                            t_avviso = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati:\\n{0}\");", t_avviso);
						}
					}

                    // Se si proviene dalla pagina delle trasmissioni massive, o dal popup di gestione repertori non bisogna immergere il codice seguente
                    // ma bisogna solo chiudere la finestra ed inserire nel callcontext la lista delle trasmissioni
                    // singole create
                    if (CallContextStack.CurrentContext.ContextState["TransmissionReasons"] == null && CallContextStack.CurrentContext.ContextState["RuoloResponsabile"] == null)
                        HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();" + t_avviso + "window.close();</script>");
                    else
                    {
                        if(CallContextStack.CurrentContext.ContextState["TransmissionReasons"] != null)
                            CallContextStack.CurrentContext.ContextState["RecivierFromAddressBook"] = TrasmManager.getGestioneTrasmissione(page).trasmissioniSingole;
                        if(!String.IsNullOrEmpty(t_avviso))
                            HttpContext.Current.Response.Write("<script>"+t_avviso+"</script>");
                        HttpContext.Current.Response.Write("<script>window.close();</script>");
                    }
					break;
				case RubricaCallType.CALLTYPE_MANAGE:
					((RubricaDocsPA) page).closeRubrica();
					break;

				case RubricaCallType.CALLTYPE_PROTO_OUT_MITT:
					pu = (DocsPAWA.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo;

					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
					pu.mittente = corr;
                    pu.daAggiornareMittente = true;
					//((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
					if(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
						&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1") && (corr.tipoIE!=null && corr.tipoIE.Equals("I")))
					{				
						//se il corrispondente è un ruolo allora metto nell'ufficio referente la sua UO
						if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
						{
							pu.ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
							gestisci_scheda_protocollo(schedaDoc);
						}
						if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
						{
							pu.ufficioReferente = corr;
							gestisci_scheda_protocollo(schedaDoc);
						}
						if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
						{
							((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.GetRuoliUtente(page,((DocsPAWA.DocsPaWR.Utente)corr).idPeople);
							((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.getRuoliFiltrati(((DocsPAWA.DocsPaWR.Utente)corr).ruoli);
							if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length == 0)
							{
								HttpContext.Current.Response.Write("<script>alert('l\\'utente "  +corr.descrizione + " non appartiene a nessuna UO\\n');</script>");	
								DocumentManager.setDocumentoInLavorazione(page, schedaDoc);
								((RubricaDocsPA) page).resetRicerca();

							}
							else
							{

								if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length == 1)
								{
									pu.ufficioReferente = ((DocsPAWA.DocsPaWR.Utente)corr).ruoli[0].uo;	
									gestisci_scheda_protocollo(schedaDoc);
								}
								else
								{
								
									HttpContext.Current.Response.Write("<script>alert('l\\'utente "  +corr.descrizione + " appartiene a più UO\\n selezionare quella di interesse');</script>");	
									//HttpContext.Current.Response.Write("<script>var win = window.open(\"../scegliUoUtente.aspx?win=rubricaV2&rubr="+corr.descrizione+"\",\"new\",\"width=550,height=350,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop)</script>");
									//HttpContext.Current.Response.Write("<script>var win = window.open(\"../scegliUoUtente.aspx?win=rubricaV2&rubr="+corr.descrizione+"\",\"window\",\"width=550,height=350,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop)</script>");
									HttpContext.Current.Response.Write("<script>rtnValue=window.showModalDialog(\"../scegliUoUtente.aspx?win=rubricaV2&rubr="+corr.descrizione+"\",\"window\",'dialogWidth:615px;dialogHeight:380px;status:no;resizable:no;scroll:no;dialogLeft:100;dialogTop:100;center:no;help:no;');if(rtnValue){window.close();}</script>");
							
									//HttpContext.Current.Response.Write("<script>var k=window.open('../../documento/docProtocollo.aspx','IframeTabs'); window.close();</script>");					
									DocumentManager.setDocumentoInLavorazione(page, schedaDoc);
									((RubricaDocsPA) page).resetRicerca();
								}
							}
						}
						
										
					}
					else
					{
						gestisci_scheda_protocollo(schedaDoc);
					}
					break;

				case RubricaCallType.CALLTYPE_PROTO_INT_MITT:
					pi = (DocsPAWA.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo;

					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
					pi.mittente = corr;

					
					//((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
					if(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
						&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"))
					{
										
						//se il corrispondente è un ruolo allora metto nell'ufficio referente la sua UO
						if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
						{
							pi.ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
							gestisci_scheda_protocollo(schedaDoc);
						}
						if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
						{
							pi.ufficioReferente = corr;
							gestisci_scheda_protocollo(schedaDoc);
						}
						if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
						{
							((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.GetRuoliUtente(page,((DocsPAWA.DocsPaWR.Utente)corr).idPeople);
							((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.getRuoliFiltrati(((DocsPAWA.DocsPaWR.Utente)corr).ruoli);
							
							if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length == 1)
							{
								pi.ufficioReferente = ((DocsPAWA.DocsPaWR.Utente)corr).ruoli[0].uo;	
								gestisci_scheda_protocollo(schedaDoc);
							}
							else
							{
								
								HttpContext.Current.Response.Write("<script>alert('l\\'utente "  +corr.descrizione + " appartiene a più UO\\n selezionare quella di interesse');</script>");	
								//HttpContext.Current.Response.Write("<script>var win = window.open(\"../scegliUoUtente.aspx?win=rubricaV2&rubr="+corr.descrizione+"\",\"new\",\"width=550,height=350,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop)</script>");
								//HttpContext.Current.Response.Write("<script>var win = window.open(\"../scegliUoUtente.aspx?win=rubricaV2&rubr="+corr.descrizione+"\",\"window\",\"width=550,height=350,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop)</script>");
								HttpContext.Current.Response.Write("<script>rtnValue=window.showModalDialog(\"../scegliUoUtente.aspx?win=rubricaV2&rubr="+corr.descrizione+"\",\"window\",'dialogWidth:615px;dialogHeight:380px;status:no;resizable:no;scroll:no;dialogLeft:100;dialogTop:100;center:no;help:no;');if(rtnValue){window.close();}</script>");
							
								//HttpContext.Current.Response.Write("<script>var k=window.open('../../documento/docProtocollo.aspx','IframeTabs'); window.close();</script>");					
								DocumentManager.setDocumentoInLavorazione(page, schedaDoc);
								((RubricaDocsPA) page).resetRicerca();
							}
						}
										
					}
					else
					{
						gestisci_scheda_protocollo(schedaDoc);
					}
					
					break;

				case RubricaCallType.CALLTYPE_PROTO_INGRESSO:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO );
					
					ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMngUscita=new ProtocollazioneIngresso.Protocollo.ProtocolloMng(page);
                    schedaDoc = protocolloMngUscita.GetDocumentoCorrente();

                    if (schedaDoc != null)
					{
                        if ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo != null && !UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, corr))
                        {
                            // Impostazione del mittente nel protocollo
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;   
                        }
                    }
					
					((RubricaDocsPA) page).closeRubrica();

					break;

                case RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO:
                    if (_A_recipients == null || _A_recipients.Length == 0)
                    {
                        ((RubricaDocsPA)page).closeRubrica();
                        break;
                    }
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);

                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloUscitaMng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(page);
                    schedaDoc = protocolloUscitaMng.GetDocumentoCorrente();

                    if (schedaDoc != null)
                    {
                        // Impostazione del mittente nel protocollo in uscita semplificato
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).mittente = corr;
                    }

                    ((RubricaDocsPA)page).closeRubrica();

                    break;

                case RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO:

                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(page);
                    schedaDoc = protocolloMng.GetDocumentoCorrente();

                    if (schedaDoc != null && schedaDoc.protocollo!=null)
                    {

                        DocsPAWA.DocsPaWR.Corrispondente[] destinatari = ((DocsPaWR.ProtocolloUscita)(schedaDoc.protocollo)).destinatari;
                        DocsPAWA.DocsPaWR.Corrispondente[] destinatariCC = ((DocsPaWR.ProtocolloUscita)(schedaDoc.protocollo)).destinatariConoscenza;

                        addDestinatari(ref destinatari, destinatariCC, _A_recipients, schedaDoc, false);
                        addDestinatari(ref destinatariCC, destinatari, _CC_recipients, schedaDoc, true);
                        
                        ((DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = destinatari;
                         ((DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = destinatariCC;
                        
                    }
                    ((RubricaDocsPA)page).closeRubrica();

                    break;

				case RubricaCallType.CALLTYPE_UFFREF_PROTO:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
					// Per l'ufficio referente possiamo assumere che il 
					// corrispondente sia interno
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, AddressbookTipoUtente.INTERNO);	
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).ufficioReferente = corr;
					gestisci_scheda_protocollo(schedaDoc);


				
					break;
				case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
					//Metto in sessione il corrispondente, questo mi serve solo per le stampe rapporti nello specifico per stampa documenti registro
					//La rimozione dalla sessione di quest'oggetto viene fatta nella pagina tabGestioneReport.aspx
					// Possiamo assumere che il corrispondente sia interno
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, AddressbookTipoUtente.INTERNO);	
					gestisci_ric_fasc_lf(corr);
					System.Web.HttpContext.Current.Session.Add("corrStampaUo",corr);
					
					break;

				case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
				case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
				case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
					// Possiamo assumere che il corrispondente sia interno
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, AddressbookTipoUtente.INTERNO);	
					gestisci_ric_fasc_lf(corr);
					



					break;

				case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
				case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
					// Possiamo assumere che il corrispondente sia interno
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, AddressbookTipoUtente.INTERNO);	
					gestisci_ric_fasc_uffref (corr, false);
					break;

				case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
					// Possiamo assumere che il corrispondente sia interno
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
					    corr = UserManager.getCorrispondenteByCodRubricaIE (page, er.codice, AddressbookTipoUtente.INTERNO);	
					gestisci_ric_fasc_uffref (corr, true);
					break;

				case RubricaCallType.CALLTYPE_RICERCA_MITTDEST:
				case RubricaCallType.CALLTYPE_RICERCA_ESTESA:
				case RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
					UserManager.setCorrispondenteSelezionato ((RubricaDocsPA) page, corr);
					HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");
					break;

				case RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
					UserManager.setCorrispondenteIntSelezionato ((RubricaDocsPA) page, corr);
					HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");
					break;

				case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
					UserManager.setCorrispondenteReferenteSelezionato((RubricaDocsPA) page, corr);
					HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");
					break;

				case RubricaCallType.CALLTYPE_PROTO_INT_DEST:
					pi = (DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo;
                    DocsPAWA.DocsPaWR.Corrispondente[] desti = pi.destinatari;
                    DocsPAWA.DocsPaWR.Corrispondente[] destiCC = pi.destinatariConoscenza;

                    addDestinatari(ref desti, destiCC, _A_recipients, schedaDoc, false);
                    addDestinatari(ref destiCC, desti, _CC_recipients, schedaDoc, true);
					pi.daAggiornareDestinatari = true;
                    pi.destinatari = desti;
                    pi.destinatariConoscenza = destiCC;
					gestisci_scheda_protocollo(schedaDoc);
					break;

				case RubricaCallType.CALLTYPE_RICERCA_TRASM:
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
					UserManager.setCorrispondenteSelezionato (page, corr);
					HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");
					break;

                case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    UserManager.setCorrispondenteSelezionatoSottoposto(page, corr);
                    HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");
                    break;

                case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    UserManager.setCreatoreSelezionato(page, corr);
                    HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");
                    break;

                case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
                    er = _A_recipients[0];
                    if (!String.IsNullOrEmpty(er.systemId))
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    else if (er.isRubricaComune)
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, AddressbookTipoUtente.INTERNO);

                    HttpContext.Current.Session["CorrForOwnerAuthor"] = corr;
                    HttpContext.Current.Response.Write("<script>window.dialogArguments.top.principale.frames[1].document.forms[0].submit();window.close();</script>");

                    break;

                case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
					Corrispondente[] ur_selection = new Corrispondente[1] { corr };
					HttpContext.Current.Session["rubrica.listaCorr"] = ur_selection;
					HttpContext.Current.Session["typeRequest"] = "fascuffref";
					HttpContext.Current.Response.Write("<script>window.close();</script>");
					break;

				case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:
					er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
					Corrispondente[] lf_selection = new Corrispondente[1] { corr };
					HttpContext.Current.Session["rubrica.listaCorr"] = lf_selection;
					HttpContext.Current.Session["typeRequest"] = "filtrifasclf";
					HttpContext.Current.Response.Write("<script>window.close();</script>");
					break;

				case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI:
				case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT:
					if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
					er=_A_recipients[0];

					// Reperimento oggetto corrispondente
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);

					// Impostazione corrispondente in sessione
					ricercaDoc.FiltriRicercaDocumenti.RubricaWrapper.SetCorrispondenteDaRubrica(corr);

					// Chiusura rubrica
					((RubricaDocsPA) page).closeRubrica();
					HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
					break;

                case RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE:

                    // inserimento Christian per ticket OC0000001489548: impossibilità creazione liste con corrispondenti di RC                
                    for (int i = 0; i < _A_recipients.Length; i++)
                    {
                        if (_A_recipients[i].isRubricaComune)
                        {
                            //DocsPaWR.ElementoRubrica es = new DocsPAWA.DocsPaWR.ElementoRubrica();
                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(page, _A_recipients[i].codice);
                            _A_recipients[i].systemId = corr.systemId;
                        }
                    }
                    HttpContext.Current.Session.Add("selCorrDaRubrica", _A_recipients);
                    //HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;

					
				case RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM:
                case RubricaCallType.CALLTYPE_REPLACE_ROLE:
                case RubricaCallType.CALLTYPE_FIND_ROLE:
					HttpContext.Current.Session.Add("selMittDaRubrica",_A_recipients);
					//HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
					HttpContext.Current.Response.Write("<script>window.close();</script>");
					break;

                
                case RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL:
                    HttpContext.Current.Session.Add("selUtenteRegNoMail", _A_recipients);
                    //HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;

                case RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL:
                    HttpContext.Current.Session.Add("selRuoloRegNoMail", _A_recipients);
                    //HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;

                case RubricaCallType.CALLTYPE_RUOLO_RESP_REG:
                    HttpContext.Current.Session.Add("selRuoloRespReg", _A_recipients);
                    //HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;
                case RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI:
                    HttpContext.Current.Session["RuoloResponsabile"] = _A_recipients;
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;

				case RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM:
				case RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP:
				case RubricaCallType.CALLTYPE_MODELLI_TRASM_INF:
				case RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL:
				case RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
					HttpContext.Current.Session.Add("selDestDaRubrica",_A_recipients);
					//HttpContext.Current.Response.Write("<script>window.dialogArguments.document.forms[0].submit(); window.close();</script>");
					HttpContext.Current.Response.Write("<script>window.close();</script>");
					break;
                case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:
                    // Ricerca trasmissioni da todolist
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    UserManager.setCorrispondenteSelezionato(page, corr);
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;
                case RubricaCallType.CALLTYPE_CORR_NO_FILTRI:
                case RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE);
                    HttpContext.Current.Session.Add("rubrica.campoCorrispondente", corr);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;
                case RubricaCallType.CALLTYPE_CORR_INT_EST:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE);
                    HttpContext.Current.Session.Add("rubrica.campoCorrispondente", corr);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;
                case RubricaCallType.CALLTYPE_CORR_EST:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
                    HttpContext.Current.Session.Add("rubrica.campoCorrispondente", corr);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;
                case RubricaCallType.CALLTYPE_CORR_INT:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    HttpContext.Current.Session.Add("rubrica.campoCorrispondente", corr);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;
                case RubricaCallType.CALLTYPE_MITT_MULTIPLI:
                    if (_A_recipients == null || _A_recipients.Length == 0) 
					{
						((RubricaDocsPA) page).closeRubrica();
						break;
					}
                    foreach (ElementoRubrica erMittMultiplo in _A_recipients)
                    {
                        if (erMittMultiplo.tipo != "L")
                        {
                            Corrispondente corrMittMultiplo = UserManager.getCorrispondenteBySystemID(page, erMittMultiplo.systemId);
                            if (corrMittMultiplo == null)
                            {
                                corrMittMultiplo = UserManager.getCorrispondenteByCodRubricaRubricaComune(page, erMittMultiplo.codice);
                            }
                            if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, corrMittMultiplo) &&
                                !UserManager.esisteCorrispondente(new Corrispondente[1] { ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente }, corrMittMultiplo)
                                )
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, corrMittMultiplo);
                                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittentiMultipli = true;
                            }
                        }
                        if (erMittMultiplo.tipo == "L")
                        {
                            ArrayList corrLista = UserManager.getCorrispondentiByCodLista(this.page, erMittMultiplo.codice, UserManager.getInfoUtente(this.page).idAmministrazione);
                            foreach (Corrispondente co in corrLista)
                            {
                                if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, co) &&
                                    !UserManager.esisteCorrispondente(new Corrispondente[1] { ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente }, co)
                                )
                                {
                                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, co);
                                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittentiMultipli = true;
                                }
                            }
                        }
                    }
                    gestisci_scheda_protocollo(schedaDoc);
                    //((RubricaDocsPA)page).closeRubrica();
                    break;
                case RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO:
                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMngSemplificato = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(page);
                    schedaDoc = protocolloMngSemplificato.GetDocumentoCorrente();

                    if (_A_recipients == null || _A_recipients.Length == 0)
                    {
                        ((RubricaDocsPA)page).closeRubrica();
                        break;
                    }
                    foreach (ElementoRubrica erMittMultiplo in _A_recipients)
                    {
                        if (erMittMultiplo.tipo != "L")
                        {
                            Corrispondente corrMittMultiplo = UserManager.getCorrispondenteBySystemID(page, erMittMultiplo.systemId);
                            if (corrMittMultiplo == null)
                            {
                                corrMittMultiplo = UserManager.getCorrispondenteByCodRubricaRubricaComune(page, erMittMultiplo.codice);
                            }
                            if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, corrMittMultiplo) &&
                                !UserManager.esisteCorrispondente(new Corrispondente[1] { ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente }, corrMittMultiplo)
                                )
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, corrMittMultiplo);
                            }
                        }
                        if (erMittMultiplo.tipo == "L")
                        {
                            ArrayList corrLista = UserManager.getCorrispondentiByCodLista(this.page, erMittMultiplo.codice, UserManager.getInfoUtente(this.page).idAmministrazione);
                            foreach (Corrispondente co in corrLista)
                            {
                                if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, co) &&
                                        !UserManager.esisteCorrispondente(new Corrispondente[1] { ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente }, co)
                                )
                                {
                                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, co);
                                }
                            }
                        }
                    }
                    //gestisci_scheda_protocollo(schedaDoc);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;

                case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    UserManager.setCorrispondenteSelezionatoRuoloSottoposto(page, corr);
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;

                case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    UserManager.setCorrispondenteSelezionatoRuoloAmministrazione(page, corr);
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;


                case RubricaCallType.CALLTYPE_TUTTE_UO:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    UserManager.setCorrispondenteSelezionatoUOAmministrazione(page, corr);
                    HttpContext.Current.Response.Write("<script>window.close();</script>");
                    break;

                case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
                    er = _A_recipients[0];
                    if (er.systemId != null && er.systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, er.systemId);
                    }
                    else if (er.isRubricaComune)
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.page, er.codice);
                    }
                    else
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, er.codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                    HttpContext.Current.Session.Add("rubrica.campoCorrispondente", corr);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;

                case RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI:
                    HttpContext.Current.Session.Add("SelDescFindModelli", _A_recipients);
                    ((RubricaDocsPA)page).closeRubrica();
                    break;
			}
		}

		private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					if (ts.corrispondenteInterno.systemId.Equals(corr.systemId)) 
					{
						if(ts.daEliminare) 
						{
							((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
							return trasmissione;
						}
						else
							return trasmissione;
					}
				}			
			}

            //prima di aggiungere la trasm.ne singola verifico le ragioni cessione
            if (this.esisteRagTrasmCessioneInTrasm(trasmissione))
            {               
                return trasmissione;
            }

			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(page);

            //// Imposta la cessione sulla ragione
            //if (UserManager.ruoloIsAutorized(page, "ABILITA_CEDI_DIRITTI_DOC"))
            //    trasmissioneSingola.ragione.cessioneImpostata = this.cbx_cediDiritti.Checked;

			// Aggiungo la lista di trasmissioniUtente
			if(corr is DocsPAWA.DocsPaWR.Ruolo) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
				if (listaUtenti.Length == 0) 
					trasmissioneSingola = null;

				//ciclo per utenti se dest è gruppo o ruolo
				for(int i= 0; i < listaUtenti.Length; i++) 
				{
					DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) listaUtenti[i];
                    //trasmissioneUtente.daNotificare = true;
                    trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
					trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
				}
			}

			if (corr is DocsPAWA.DocsPaWR.Utente) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
                //trasmissioneUtente.daNotificare = true;
                trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
			}

			if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa) 
			{
				DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa) corr;
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
				qca.ragione = trasmissioneSingola.ragione;
				qca.ruolo = UserManager.getRuolo();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(null, qca, theUo);

                //Andrea
                if (ruoli == null || ruoli.Length == 0)
                {
                    listaExceptionTrasmissioni1.Add("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r);
                }

				return trasmissione;
			}

			if (trasmissioneSingola != null)
				trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
			else
			{
				// In questo caso questa trasmissione non può avvenire perché la
				// struttura non ha utenti (TICKET #1608)
				trasm_strutture_vuote.Add (String.Format ("{0} ({1})", corr.descrizione, corr.codiceRubrica));

			}
			return trasmissione;

		}

		private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = corr.codiceRubrica;
			qco.getChildren = true;
		
			qco.idAmministrazione= UserManager.getInfoUtente (this.page).idAmministrazione;
            qco.fineValidita = true;
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			return UserManager.getListaCorrispondenti(page, qco);
		}

		private void addDestinatari ( ref DocsPAWA.DocsPaWR.Corrispondente[] corrs, DocsPAWA.DocsPaWR.Corrispondente[] corrsCC, DocsPAWA.DocsPaWR.ElementoRubrica[] ers, DocsPAWA.DocsPaWR.SchedaDocumento doc, bool is_cc)
		{
			if (ers == null || ers.Length == 0)
				return;

			if (corrs == null)
				corrs = new DocsPAWA.DocsPaWR.Corrispondente[0];

			//ArrayList corrTotali = new ArrayList();
			System.Object[] corrTotali=new System.Object[0]; 

			//corrs.CopyTo(corrTotali,0);

			for (int n = 0; n < ers.Length; n++) 
			{
                if(ers[n].tipo == "L")
                {
                    int corrListaInterni = 0;
                    DocsPaWR.Corrispondente corr = null;
                    corr = UserManager.getCorrispondenteByCodRubricaIE(page, ers[n].codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE);

                    //Liste distribuzione
                    //Ricerco i corrispondenti relativi alla lista di distribuzione
                    //Mi viene restituito un ArrayList di corrispondenti
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    ArrayList corrLista = UserManager.getCorrispondentiByCodLista(page, ers[n].codice,idAmm);			

                    for(int i=0; i<corrLista.Count; i++)
                    {
                        DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente) corrLista[i];
						
                        if (!UserManager.esisteCorrispondente(corrs,c) && (!UserManager.esisteCorrispondente(corrsCC,c))) 
                        {
                            if(doc.tipoProto == "I")
                            {
                                if(c.tipoIE == "I")
                                {
                                    if(c.tipoCorrispondente == "R")
                                    {									
                                        if(UserManager.VerificaAutorizzazioneRuoloSuRegistro(page,doc.registro.systemId,c.systemId))
                                        {
                                            corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
                                            corrListaInterni++;
                                        }
                                    }
                                }
                                if(c.tipoCorrispondente == "U")
                                {
                                    string[] codiciAooAutorizzate = UserManager.GetUoInterneAoo(page);
                                    for(int j=0; j<codiciAooAutorizzate.Length; j++)
                                    {
                                        if(c.codiceRubrica == codiciAooAutorizzate[j])
                                        {
                                            corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
                                            corrListaInterni++;
                                        }
                                    }
                                }
                                if((c.tipoCorrispondente == "P") &&(c.tipoIE == "I"))
                                {
                                    string[] systemIdRuoliAutorizzati = UserManager.getUtenteInternoAOO( ((Utente) c).idPeople, doc.registro.systemId, page);
                                    if(systemIdRuoliAutorizzati.Length != 0)
                                    {
                                        corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
                                        corrListaInterni++;
                                    }
                                }								
                            }
                            else
                                corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
					
                            if (is_cc)
                                DocumentManager.DO_AddDestinatrioCCModificato (doc, c.systemId);
                            else
                                DocumentManager.DO_AddDestinatrioModificato (doc, c.systemId);
                        }
                    }

                    if(doc.tipoProto == "I")
                    {
                        if(corrLista.Count != corrListaInterni)
                        {
                            HttpContext.Current.Response.Write("<script>alert('Spiacente ma la lista contiene destinatari non interni !');</script>");					
                            return;
                        }
                    }
                }

                if (ers[n].tipo == "F" && !ers[n].isRubricaComune)
                {
                    DocsPaWR.Corrispondente corr = null;
                    corr =  UserManager.getCorrispondenteByCodRubricaIE(page, ers[n].codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE);

                    ArrayList corrLista = UserManager.getCorrispondentiByCodRF(this.page, corr.codiceRubrica);
                    for(int i=0; i<corrLista.Count; i++)
                    {
                        DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente) corrLista[i];
						
                        if (!UserManager.esisteCorrispondente(corrs,c) && (!UserManager.esisteCorrispondente(corrsCC,c))) 
                        {
                            if(doc.tipoProto == "I")
                            {
                                if(c.tipoIE == "I")
                                {
                                    if(c.tipoCorrispondente == "R")
                                    {									
                                    
                                    }
                                    if(UserManager.VerificaAutorizzazioneRuoloSuRegistro(page,doc.registro.systemId,c.systemId))
                                    {
                                        corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
                                    }
                                }
                                //if(c.tipoCorrispondente == "U")
                                //{
                                //    string[] codiciAooAutorizzate = UserManager.GetUoInterneAoo(page);
                                //    for(int j=0; j<codiciAooAutorizzate.Length; j++)
                                //    {
                                //        if(c.codiceRubrica == codiciAooAutorizzate[j])
                                //            corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
                                //    }
                                //}
                                //if((c.tipoCorrispondente == "P") &&(c.tipoIE == "I"))
                                //{
                                //    string[] systemIdRuoliAutorizzati = UserManager.getUtenteInternoAOO( ((Utente) c).idPeople, doc.registro.systemId, page);
                                //    if(systemIdRuoliAutorizzati.Length != 0)
                                //        corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
                                //}								
                            }
                            else
                                corrTotali=DocsPAWA.Utils.addToArray(corrTotali, c);	
					
                            if (is_cc)
                                DocumentManager.DO_AddDestinatrioCCModificato (doc, c.systemId);
                            else
                                DocumentManager.DO_AddDestinatrioModificato (doc, c.systemId);
                        }
                    }
                }





                if (ers[n].tipo != "F" && ers[n].tipo != "L" || (ers[n].tipo == "F" && ers[n].isRubricaComune))
                {
                    DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
                    if (ers[n].systemId != null && ers[n].systemId != "")
                    {
                        corr = UserManager.getCorrispondenteBySystemID(page, ers[n].systemId);
                    }
                    else
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(page, ers[n].codice);                        
                    }

                    if (doc.tipoProto == "I")
                    {
                        if (corr.tipoIE == "I")
                            corrTotali = DocsPAWA.Utils.addToArray(corrTotali, corr);
                        else
                            HttpContext.Current.Response.Write("<script>alert('Destinatario non interno !');</script>");
                    }
                    else
                    {
                        if (!UserManager.esisteCorrispondente(corrs, corr) && (!UserManager.esisteCorrispondente(corrsCC, corr)))
                        {
                            corrTotali = DocsPAWA.Utils.addToArray(corrTotali, corr);
                        }
                    }
                    if (is_cc)
                        DocumentManager.DO_AddDestinatrioCCModificato(doc, corr.systemId);
                    else
                        DocumentManager.DO_AddDestinatrioModificato(doc, corr.systemId);
                }
			}
			DocsPaWR.Corrispondente[] dest = new DocsPAWA.DocsPaWR.Corrispondente[corrTotali.Length + corrs.Length];		
			corrs.CopyTo(dest,0);
			if(corrTotali!=null && corrTotali.Length>0)
			{
				corrTotali.CopyTo(dest,corrs.Length);
			}
			corrs = dest;

            DocumentManager.setDocumentoSelezionato(doc);
            DocumentManager.setDocumentoInLavorazione(doc);
		}

		private void gestisci_scheda_protocollo(DocsPAWA.DocsPaWR.SchedaDocumento sd)
		{
			DocumentManager.setDocumentoInLavorazione(page, sd);
            DocumentManager.setDocumentoSelezionato(sd);
			HttpContext.Current.Response.Write("<script>var k=window.open('../../documento/docProtocollo.aspx','IframeTabs'); window.close();</script>");					
		}
		
		private void gestisci_ric_fasc_lf (DocsPAWA.DocsPaWR.Corrispondente corr)
		{
			DocsPaWR.Fascicolo fascLF = FascicoliManager.getFascicoloSelezionato();
		
		
			if(fascLF!=null)
			{
				DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
				FascicoliManager.DO_SetFlagLF();
				LF.UO_ID=corr.systemId;
				LF.CodiceRubrica=corr.codiceRubrica;
				LF.Descrizione=corr.descrizione;
				FascicoliManager.DO_SetLocazioneFisica(LF);
			}
			else
			{
				DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
				FascicoliManager.DO_SetFlagLF();
				LF.UO_ID=corr.systemId;
				LF.CodiceRubrica=corr.codiceRubrica;
				LF.Descrizione=corr.descrizione;
				FascicoliManager.DO_SetLocazioneFisica(LF);		
			}
			//window.dialogArguments.top.principale.frames[1].document.forms[0]
			//HttpContext.Current.Response.Write("<script>var k=window.parent.opener.document.forms[0].submit(); </script>");
			HttpContext.Current.Response.Write("<script>window.close();</script>");
		}

		private void gestisci_ric_fasc_uffref(DocsPAWA.DocsPaWR.Corrispondente corr, bool fasc_modifica)
		{

			DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
				
				
			if(fasc!=null)
			{
				if (!fasc_modifica) 
				{
					if(corr!=null)
					{
						fasc.ufficioReferente = corr;
					}
					FascicoliManager.setFascicoloSelezionato(fasc);
					FascicoliManager.DO_SetFlagUR();
				}
				else
				{
					if(corr!=null)
					{
						FascicoliManager.setUoReferenteSelezionato(corr);
						FascicoliManager.DO_SetFlagUR();
					}
				}
			}
			else
			{
				FascicoliManager.setUoReferenteSelezionato(corr);
				FascicoliManager.DO_SetFlagUR();

			}
			//HttpContext.Current.Response.Write("<script>var k=window.parent.opener.document.forms[0].submit(); </script>");
			HttpContext.Current.Response.Write("<script>parent.window.close();</script>");
		}

		#endregion

        private bool esisteRagTrasmCessioneInTrasm(DocsPAWA.DocsPaWR.Trasmissione trasmissione)
        {
            bool retValue = false;

            // verifica solo se le trasm.ni singole sono più di una
            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                // conta quante trasm.ni singole hanno la ragione con la cessione impostata
                int trasmConCessione = 0;
                foreach (DocsPaWR.TrasmissioneSingola trasmS in trasmissione.trasmissioniSingole)
                    if (trasmS.ragione.cessioneImpostata)
                        trasmConCessione++;

                // è possibile inserire solo una ragione se questa ha la cessione, quindi...
                DocsPaWR.RagioneTrasmissione ragioneAttuale = TrasmManager.getRagioneSel(page);
                if (trasmConCessione > 0) //sono state già inserite ragioni con cessione 
                {
                    // non si può inserire nient'altro, avvisa...
                    HttpContext.Current.Response.Write("<script language=\"javascript\">alert(\"Attenzione! poichè la trasmissione creata prevede la cessione dei diritti, non è possibile più di un destinatario\");</script>");
                    retValue = true;
                }
                else
                {
                    // non esistono ragioni con cessione, se l'attuale è con cessione avvisa che non si può...
                    if (UserManager.ruoloIsAutorized(page, "ABILITA_CEDI_DIRITTI_DOC"))
                    {
                        if (ragioneAttuale.cessioneImpostata)
                        {
                            HttpContext.Current.Response.Write("<script language=\"javascript\">alert(\"Attenzione! non è possibile inserire ragioni di trasmissione con cessione insieme a ragioni senza cessione\");</script>");
                            retValue = true;
                        }
                    }
                }
            }

            return retValue;
        }       
	}
}
