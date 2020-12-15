using System;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using System.Data;
using System.Collections;
using System.Web;
using log4net;
using System.Collections.Generic;
using System.Linq;

//Andrea
using DocsPAWA.utils;
//End Andrea

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for TrasmManager.
	/// </summary>
	public class TrasmManager
	{
		private static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
        private static ILog logger = LogManager.GetLogger(typeof(TrasmManager));
        public static bool executeAccRif(Page page, TrasmissioneUtente trasmissioneUtente, string idTrasmissione, out string errore)
		{
            logger.Info("BEGIN");
            bool result = true;
            errore = string.Empty;

			try 
			{
                if (docsPaWS.TrasmissioneExecuteAccRif(trasmissioneUtente, idTrasmissione, UserManager.getRuolo(), UserManager.getInfoUtente(), out errore))
				{
					// se è un rifiuto...
					if(trasmissioneUtente.tipoRisposta.ToString().ToUpper().Equals("RIFIUTO"))
					{
                        InfoUtente infoUtente = UserManager.getInfoUtente();

						// ...allora la trasmissione torna al mittente
                        if (!docsPaWS.RitornaAlMittTrasmUt(trasmissioneUtente, infoUtente))
						{
							//throw new Exception();
                            result = false;
						}
					}
				}
				else
				{
                    //errore = "In questo momento non è possibile accettare/rifiutare la trasmissione.";
                    result = false;
				}
			} 			
			catch(Exception es) 
			{
                // ErrorManager.redirect(page, es);
                result = false;
			}
            logger.Info("END");
            return result;
		}
        
        public static int getGrdTodolistPageSize(Page page)
        {
            int num = 8;
            string val = System.Configuration.ConfigurationManager.AppSettings["GRD_TODOLIST_PAGE_SIZE"];
            return (val != null && Int32.TryParse(val, out num)) ? num : 8;
        }
		
		public static DocsPAWA.DocsPaWR.Trasmissione getTrasmRispostadaUt(Page page,DocsPaWR.TrasmissioneUtente trasmUt)
		{
			
			try 
			{				
				DocsPaWR.Trasmissione result = docsPaWS.TrasmissioneGetRisposta(trasmUt);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
		public static DocsPAWA.DocsPaWR.Trasmissione getTrasmRispostadaSing(Page page,DocsPaWR.TrasmissioneSingola trasmSing)
		{
			
			try 
			{				
				DocsPaWR.Trasmissione result = docsPaWS.TrasmissioneGetInRispostaA(trasmSing);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

        public static Trasmissione saveExecuteTrasm(Page page, Trasmissione trasmissione, DocsPAWA.DocsPaWR.InfoUtente infoUtente)
        {
            logger.Info("BEGIN");
            Trasmissione result = null;
            try
            {
                string server_path = Utils.getHttpFullPath(page);
                result = docsPaWS.TrasmissioneSaveExecuteTrasm(server_path, trasmissione, infoUtente);
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            logger.Info("END");
            return result;
        }

        public static Trasmissione saveExecuteTrasmAM(Page page, Trasmissione trasmissione, DocsPAWA.DocsPaWR.InfoUtente infoUtente)
        {
            logger.Info("BEGIN");
            Trasmissione result = null;
            try
            {
                string server_path = Utils.getHttpFullPath(page);
                result = docsPaWS.TrasmissioneSaveExecuteTrasmAM(server_path, trasmissione, infoUtente);
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            logger.Info("END");
            return result;
        }

        public static Trasmissione saveTrasm(Page page, Trasmissione trasmissione)
		{
            logger.Info("BEGIN");
			Trasmissione result = null;

			try 
			{				
				result = docsPaWS.TrasmissioneSaveTrasm(trasmissione);

				if(result == null)
				    throw new Exception();
			} 				
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}
            logger.Info("END");
			return result;
		}

        public static bool deleteTrasm(Trasmissione trasmissione)
        {
            bool result;

            try
            {
                result = docsPaWS.TrasmissioniDeleteTrasmissione(trasmissione);
            }
            catch
            {
                result = false;
            }

            return result;
        }

		public static Trasmissione[] getQueryEffettuate (Page page, TrasmissioneOggettoTrasm oggTrasm,Utente user,Ruolo ruolo,FiltroRicerca[] filterRic) 
		{
			try 
			{	
				Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuate(oggTrasm,filterRic,user,ruolo);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
        
		public static Trasmissione[] getQueryEffettuatePaging (Page page, TrasmissioneOggettoTrasm oggTrasm,Utente user,Ruolo ruolo,FiltroRicerca[] filterRic,int pageNumber, out int totalPageNumber, out int recordCount) 
		{
			totalPageNumber=0;
			recordCount=0;

			try 
			{				
				Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuatePaging(oggTrasm,filterRic,user,ruolo,pageNumber,out totalPageNumber,out recordCount);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
	
		public static Trasmissione[] getQueryEffettuateDocumentoPaging (Page page, TrasmissioneOggettoTrasm oggTrasm,Utente user,Ruolo ruolo,FiltroRicerca[] filterRic,int pageNumber, out int totalPageNumber, out int recordCount) 
		{
			totalPageNumber=0;
			recordCount=0;

			try 
			{				
				Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuateDocPaging(oggTrasm,filterRic,user,ruolo,pageNumber,out totalPageNumber,out recordCount);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
        
		public static Trasmissione[] getQueryRicevute (Page page, TrasmissioneOggettoTrasm oggTrasm,Utente user,Ruolo ruolo,FiltroRicerca[] filterRic) 
		{
			try 
			{				
				Trasmissione[] result = docsPaWS.TrasmissioneGetQueryRicevute(oggTrasm, filterRic, user, ruolo);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		public static Trasmissione[] getQueryRicevutePaging (
								Page page, 
								TrasmissioneOggettoTrasm oggTrasm,
								Utente user,
								Ruolo ruolo,
								FiltroRicerca[] filterRic,
								int pageNumber, 
								out int totalPageNumber, 
								out int recordCount)
		{
			totalPageNumber=0;
			recordCount=0;

			try 
			{				
				Trasmissione[] result = docsPaWS.TrasmissioneGetQueryRicevutePaging(oggTrasm, filterRic, user, ruolo, pageNumber, out totalPageNumber, out recordCount);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
	
		public static Trasmissione executeTrasm(Page page, Trasmissione myTrasm) 
		{
            logger.Info("BEGIN");
			try 
			{	
				string server_path = Utils.getHttpFullPath(page);
                
				//Celeste
                Trasmissione result = docsPaWS.TrasmissioneExecuteTrasm(server_path, myTrasm, UserManager.getInfoUtente());
				//InfoUtente infoUtente = UserManager.getInfoUtente(page);
				//Trasmissione result = docsPaWS.TrasmissioneExecuteTrasm(infoUtente,server_path, myTrasm);
				//Fine
				if(result == null)
				{
					throw new Exception();
				}
//				else if(result.ErrorSendingEmails)
//				{
//					throw new Exception("Non e' stato possibile inoltrare una o piu' e-mail. Contattare l'amministratore per risolvere il problema.");
//				}
                logger.Info("END");
				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;			
		}

		public static TrasmissioneUtente[] addTrasmissioneUtente(TrasmissioneUtente[] array, TrasmissioneUtente nuovoElemento) 
		{
			TrasmissioneUtente[] nuovaLista;
			if (array!=null)
			{
				int len = array.Length;
				nuovaLista = new TrasmissioneUtente[len +1];
				array.CopyTo(nuovaLista,0);
				nuovaLista[len] = nuovoElemento;
				return nuovaLista;
			}
			else
			{
				nuovaLista=new TrasmissioneUtente[1];
				nuovaLista[0] = nuovoElemento;
				return nuovaLista;
			}

		}
	
		public static TrasmissioneSingola[] addTrasmissioneSingola(TrasmissioneSingola[] array, TrasmissioneSingola nuovoElemento) 
		{
			TrasmissioneSingola[] nuovaLista;
			if (array!=null)
			{
				int len = array.Length;
				nuovaLista = new TrasmissioneSingola[len +1];
				array.CopyTo(nuovaLista,0);
				nuovaLista[len] = nuovoElemento;
				return nuovaLista;
			}
			else
			{
				nuovaLista=new TrasmissioneSingola[1];
				nuovaLista[0] = nuovoElemento;
				return nuovaLista;
			}

		}	

		public static void setTxUtAsViste (Page page) 
		{
			try 
			{				
				docsPaWS.trasmissioneSetTxUtAsViste(UserManager.getInfoUtente(page));
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			
		}

		#region Gestione Sessione: Risultato Query Trasm Eff.
		public static Trasmissione[] getDocTrasmQueryEff(Page page) 
		{
			return getDocTrasmQueryEff();
		}

		public static void setDocTrasmQueryEff(Page page, Trasmissione[] query) 
		{
			setDocTrasmQueryEff(query);
		}

		public static void removeDocTrasmQueryEff(Page page) 
		{
            removeDocTrasmQueryEff();
		}

        public static Trasmissione[] getDocTrasmQueryEff()
        {
            return (Trasmissione[])HttpContext.Current.Session["docTrasmissioni.trasmQueryEff"];
        }

        public static void setDocTrasmQueryEff(Trasmissione[] query)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["docTrasmissioni.trasmQueryEff"] = query;
        }

        public static void removeDocTrasmQueryEff()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("docTrasmissioni.trasmQueryEff");
        }
        #  endregion  

		#region gestione variabile sessione "m_hashTableRagioneTrasmissione"
		public static Hashtable getHashRagioneTrasmissione(Page page) 
		{
			return (Hashtable)page.Session["m_hashTableRagioneTrasmissione"];
		}

		public static void setHashRagioneTrasmissione(Page page, Hashtable hash) 
		{
			page.Session["m_hashTableRagioneTrasmissione"] = hash;
		}

		public static void removeHashRagioneTrasmissione(Page page) 
		{
			page.Session.Remove("m_hashTableRagioneTrasmissione");
		}
		#  endregion  

		#region Gestione Sessione: HashTable InfoDOC/InfoFasc OGG Trasmesso.

		public static Hashtable getHashTrasmOggTrasm(Page page) 
		{
			return (Hashtable) GetSessionValue("tabRisultatiRicTrasm.hashInfoOgg");
		}

		public static void setHashTrasmOggTrasm(Page page, Hashtable hash) 
		{
			SetSessionValue("tabRisultatiRicTrasm.hashInfoOgg",hash);
		}

		public static void removeHashTrasmOggTrasm(Page page) 
		{
			RemoveSessionValue("tabRisultatiRicTrasm.hashInfoOgg");
		}

		#  endregion  

		#region Gestione Sessione: Risultato Query Trasm Ric.
		public static Trasmissione[] getDocTrasmQueryRic(Page page) 
		{
			return getDocTrasmQueryRic();
		}

		public static void setDocTrasmQueryRic(Page page, Trasmissione[] query) 
		{
			setDocTrasmQueryRic(query);
		}

		public static void removeDocTrasmQueryRic(Page page) 
		{
            removeDocTrasmQueryRic();
		}

        public static Trasmissione[] getDocTrasmQueryRic()
        {
            return (Trasmissione[]) HttpContext.Current.Session["docTrasmissioni.trasmQueryRic"];
        }

        public static void setDocTrasmQueryRic(Trasmissione[] query)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["docTrasmissioni.trasmQueryRic"] = query;
        }

        public static void removeDocTrasmQueryRic()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("docTrasmissioni.trasmQueryRic");
        }
        #  endregion

		#region Gestione Sessione:  Trasm Risp Selezionata.
		public static Trasmissione getrasmRispSel(Page page) 
		{
			return (Trasmissione)page.Session["tabRisultatiRicTrasm.trasmRisp"];
		}

		public static void setTrasmRispSel(Page page, Trasmissione trasm) 
		{
			page.Session["tabRisultatiRicTrasm.trasmRisp"] = trasm;
		}

		public static void removeTrasmRispSel(Page page) 
		{
			page.Session.Remove("tabRisultatiRicTrasm.trasmRisp");
		}
		#  endregion

		#region Gestione Sessione: Trasm (Eff/Ric) Selezionata su docTrasmssioni 
		public static Trasmissione getDocTrasmSel(Page page) 
		{
			return getDocTrasmSel();
		}

		public static void setDocTrasmSel(Page page, Trasmissione trasm) 
		{
			setDocTrasmSel(trasm);
		}

		public static void removeDocTrasmSel(Page page) 
		{
			removeDocTrasmSel();
		}

        public static Trasmissione getDocTrasmSel()
        {
            return (Trasmissione)HttpContext.Current.Session["docTrasmissioni.trasmSel"];
        }

        public static void setDocTrasmSel(Trasmissione trasm)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["docTrasmissioni.trasmSel"] = trasm;
        }

        public static void removeDocTrasmSel()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("docTrasmissioni.trasmSel");
        }
        #  endregion	

		#region Gestione Sessione: Trasm (Eff/Ric) DataTableEff
		public static DataTable getDataTableEff(Page page) 
		{
            return getDataTableEff();
		}

		public static void setDataTableEff(Page page, DataTable tbl) 
		{
			setDataTableEff(tbl);
		}

		public static void removeDataTableEff(Page page) 
		{
			removeDataTableEff();
		}
        
        public static DataTable getDataTableEff()
        {
            return (DataTable)HttpContext.Current.Session["tabRisultatiRicTrasm.datatableEff"];
        }

        public static void setDataTableEff(DataTable tbl)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["tabRisultatiRicTrasm.datatableEff"] = tbl;
        }

        public static void removeDataTableEff()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("tabRisultatiRicTrasm.datatableEff");
        }
        #  endregion	

		#region Gestione Sessione: Trasm (Eff/Ric) DataTableRic
		public static DataTable getDataTableRic(Page page) 
		{
			return getDataTableRic();
		}

		public static void setDataTableRic(Page page, DataTable tbl) 
		{
			setDataTableRic(tbl);
		}

		public static void removeDataTableRic(Page page) 
		{
			removeDataTableRic();
		}

        public static DataTable getDataTableRic()
        {
            return (DataTable)HttpContext.Current.Session["tabRisultatiRicTrasm.datatableRic"];
        }

        public static void setDataTableRic(DataTable tbl)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["tabRisultatiRicTrasm.datatableRic"] = tbl;
        }

        public static void removeDataTableRic()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("tabRisultatiRicTrasm.datatableRic");
        }
        #  endregion	

		#region gestione trasmissione

		public static RagioneTrasmissione[] getListaRagioni(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, bool flgDaRicercaTrasm)
		{
			try
			{
				DocsPaWR.TrasmissioneDiritti diritti = new DocsPAWA.DocsPaWR.TrasmissioneDiritti();
				if(schedaDocumento != null)
					diritti.accessRights = schedaDocumento.accessRights;

				diritti.idAmministrazione = UserManager.getUtente(page).idAmministrazione;
				RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti,flgDaRicercaTrasm);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return null;
		}

		public static RagioneTrasmissione[] getListaRagioniFasc(Page page)
		{
			try
			{
				
				DocsPaWR.TrasmissioneDiritti diritti = new DocsPAWA.DocsPaWR.TrasmissioneDiritti();
				
				diritti.accessRights = "-1";  //INDICA CHE LE RAGIONI DA PRENDERE SONO TUTTE
				diritti.idAmministrazione = UserManager.getUtente(page).idAmministrazione;
				//flgDaRicercaTrasm=true : vengo da ricerca trasmissioni cerco tutte le ragioni
				//flgDaRicercaTrasm=false : ricerca trasm con cha_vis='1'
				bool flgDaRicercaTrasm=false;
				
				RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti,flgDaRicercaTrasm);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return null;
		}


		public static RagioneTrasmissione[] getListaRagioniFasc(Page page,Fascicolo fasc)
		{
			try
			{
				DocsPaWR.TrasmissioneDiritti diritti = new DocsPAWA.DocsPaWR.TrasmissioneDiritti();
				if(fasc!=null)
					if(fasc.stato=="C")
					{
						diritti.accessRights =  "45";		//lettura
					}
					else
					{
						diritti.accessRights = fasc.accessRights;
					}
				diritti.idAmministrazione = UserManager.getUtente(page).idAmministrazione;
				//flgDaRicercaTrasm=true : vengo da ricerca trasmissioni cerco tutte le ragioni
				//flgDaRicercaTrasm=false : ricerca trasm con cha_vis='1'
				bool flgDaRicercaTrasm=false;
				RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti,flgDaRicercaTrasm);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return null;
		}

		//trasmissione

		public static void setGestioneTrasmissione(Page page, Trasmissione trasmissione)
		{
			page.Session["gestioneTrasm.GestTrasm"] = trasmissione;
		}

		public static Trasmissione getGestioneTrasmissione(Page page)
		{
			return (Trasmissione) page.Session["gestioneTrasm.GestTrasm"];
		}

		public static void removeGestioneTrasmissione(Page page)
		{
			page.Session.Remove("gestioneTrasm.GestTrasm");
		}


		//ragione
		public static void setRagioneSel(Page page, RagioneTrasmissione ragione)
		{
			page.Session["gestioneNuovaTrasm.setRagioneSel"] = ragione;
		}

		public static RagioneTrasmissione getRagioneSel(Page page)
		{
			return (RagioneTrasmissione) page.Session["gestioneNuovaTrasm.setRagioneSel"];
		}

		public static void removeRagioneSel(Page page)
		{
			page.Session.Remove("gestioneNuovaTrasm.setRagioneSel");
		}
	

		public static Trasmissione removeTrasmSingola(Trasmissione trasmissione, int i, bool setDaEliminare)
		{
            ArrayList lista = new ArrayList();
			try 
			{				
				//if (trasmissione.trasmissioniSingole[i].systemId != null && !trasmissione.trasmissioniSingole[i].systemId.Equals("")) 
                if(setDaEliminare)
					trasmissione.trasmissioniSingole[i].daEliminare = true;
				else 
				{
                    DocsPaWR.TrasmissioneSingola[] listaTrasmSingole = new DocsPAWA.DocsPaWR.TrasmissioneSingola[trasmissione.trasmissioniSingole.Length - 1];
                    int indice = 0;
                    for (int j = 0; j < trasmissione.trasmissioniSingole.Length; j++)
                    {
                        if (!j.Equals(i))
                        {
                            listaTrasmSingole[indice] = trasmissione.trasmissioniSingole[j];
                            indice++;
                        }
                    }
                    trasmissione.trasmissioniSingole = listaTrasmSingole;	                   
				}
			}
			catch (Exception) {}
			return trasmissione;
		}

		#endregion 

		#region gestione variabile sessione "m_hashTableRuoliSup"   "m_hashTableUtenti"
		public static Hashtable getHashRuoliSup(Page page) 
		{
			return (Hashtable)page.Session["nuovaTrasm_hashTableRuoliSup"];
		}

		public static void setHashRuoliSup(Page page, Hashtable hash) 
		{
			page.Session["nuovaTrasm_hashTableRuoliSup"] = hash;
		}

		public static void removeHashHashRuoliSup(Page page) 
		{
			page.Session.Remove("nuovaTrasm_hashTableRuoliSup");
		}
		
		// utenti
		public static Hashtable getHashUtenti(Page page) 
		{
			return (Hashtable)page.Session["nuovaTrasm_hashTableUtenti"];
		}

		public static void setHashUtenti(Page page, Hashtable hash) 
		{
			page.Session["nuovaTrasm_hashTableUtenti"] = hash;
		}

		public static void removeHashUtenti(Page page) 
		{
			page.Session.Remove("nuovaTrasm_hashTableUtenti");
		}
		#  endregion  

		#region gestione template trasmissione

		public static TemplateTrasmissione saveTemplateTrasm(Page page,TemplateTrasmissione template)
		{
			try 
			{	
				TemplateTrasmissione result = docsPaWS.TrasmissioneAddTemplate(template);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
				return null;
			}
		}

		public static TemplateTrasmissione[] getListaTemplate (Page page,Utente user,Ruolo ruolo, string tipoOggetto) 
		{
			//tipo Oggetto: D=documento, F=fascicolo
			
			try 
			{	
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				

				TemplateTrasmissione[] result = docsPaWS.TrasmissioneGetListaTemplate(infoUtente.idPeople ,infoUtente.idCorrGlobali, tipoOggetto);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		public static void updateTemplate (Page page, DocsPAWA.DocsPaWR.TemplateTrasmissione template) 
		{
			try 
			{	
				bool result = docsPaWS.TrasmissioniUpdateTemplate(template);

				if(!result)
				{
					throw new Exception();
				}
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return;
		}

		public static void deleteTemplate (Page page, DocsPAWA.DocsPaWR.TemplateTrasmissione template) 
		{
			try 
			{	
				bool result = docsPaWS.TrasmissioniDeleteTemplate(template);

				if(!result)
				{
					throw new Exception();
				}
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return;
		}

		protected static DocsPAWA.DocsPaWR.TemplateTrasmissione[] addInListaTemplate(DocsPAWA.DocsPaWR.TemplateTrasmissione[] listaTemplate, DocsPAWA.DocsPaWR.TemplateTrasmissione oggetto) 
		{
			DocsPaWR.TemplateTrasmissione[] nuovaLista;
			
			if (listaTemplate != null) 
			{
				int len = listaTemplate.Length;
				nuovaLista = new DocsPAWA.DocsPaWR.TemplateTrasmissione[len + 1];
				listaTemplate.CopyTo(nuovaLista,0);
				nuovaLista[len] = oggetto;
			}
			else 
			{
				nuovaLista=new DocsPAWA.DocsPaWR.TemplateTrasmissione[1];
				nuovaLista[0] = oggetto;
			}
			listaTemplate =  nuovaLista;
			return listaTemplate;
		}
	

		public static DocsPAWA.DocsPaWR.TemplateTrasmissione[] rimuoviTemplate(DocsPAWA.DocsPaWR.TemplateTrasmissione[] listaTemplate, int index) 
		{
			if(listaTemplate == null || listaTemplate.Length < index)
				return listaTemplate;
			
			if(listaTemplate.Length == 1)
				return null;

			ArrayList nuovaArrayListaTemplate = new ArrayList();
			if (listaTemplate != null && listaTemplate.Length > 0) 
			{	
				for(int i = 0; i < listaTemplate.Length; i++) 
				{
					if (i != index)
						nuovaArrayListaTemplate.Add(listaTemplate[i]); 
					
					//= addInListaTemplate(nuovaArrayListaTemplate, listaTemplate[i]);
				}
			}

			DocsPaWR.TemplateTrasmissione[] nuovaListaTemplate = new DocsPAWA.DocsPaWR.TemplateTrasmissione[nuovaArrayListaTemplate.Count];
			for (int j=0; j < nuovaArrayListaTemplate.Count; j++)
				nuovaListaTemplate[j] = (DocsPAWA.DocsPaWR.TemplateTrasmissione) nuovaArrayListaTemplate[j];
			return nuovaListaTemplate;
		}


		//dataGrid ListaTemplate
		public static ArrayList getDataGridListaTemplate(Page page) 
		{
			return (ArrayList)page.Session["templateTrasmissioni.DataGridTemplate"];
		}

		public static void setDataGridListaTemplate(Page page, ArrayList template) 
		{
			page.Session["templateTrasmissioni.DataGridTemplate"] = template;
		}

		public static void removeDataGridListaTemplate(Page page) 
		{
			page.Session.Remove("templateTrasmissioni.DataGridTemplate");
		}


		//templateSel
		public static void setTemplateSel(Page page, DocsPAWA.DocsPaWR.TemplateTrasmissione template)
		{
			page.Session["templateTrasmissione.TemplateSel"] = template;
		}

		public static DocsPAWA.DocsPaWR.TemplateTrasmissione getTemplateSel(Page page)
		{
			return (DocsPAWA.DocsPaWR.TemplateTrasmissione) page.Session["templateTrasmissione.TemplateSel"];
		}

		public static void removeTemplateSel(Page page)
		{
			page.Session.Remove("templateTrasmissione.TemplateSel");
		}


		public static Trasmissione addTrasmDaTemplate(Page page,InfoDocumento infoDocumento, DocsPAWA.DocsPaWR.TemplateTrasmissione template, InfoUtente infoUtente)
		{
			try 
			{				
				DocsPaWR.Utente utente = UserManager.getUtente(page);
				DocsPaWR.Ruolo ruolo = UserManager.getRuolo(page);
				UserManager.getRuolo(page);
				Trasmissione result;
				result = docsPaWS.TrasmissioneAddDaTempl(infoDocumento, template, utente, ruolo);
				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}


		public static Trasmissione addTrasmFascicoloDaTemplate(Page page,InfoFascicolo infoFascicolo, DocsPAWA.DocsPaWR.TemplateTrasmissione template, InfoUtente infoUtente)
		{
			try 
			{				
				DocsPaWR.Utente utente = UserManager.getUtente(page);
				DocsPaWR.Ruolo ruolo = UserManager.getRuolo(page);
				UserManager.getRuolo(page);
				Trasmissione result;
				result = docsPaWS.TrasmissioneFascicoloAddDaTempl(infoFascicolo, template, utente, ruolo);
				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

		#endregion

		#region Gestione Report Trasmissioni
		public static FileDocumento getReportTrasm(Page page, DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm)
		{
			try 
			{
               
				FileDocumento fileDoc=null;

                DocsPAWA.DocsPaWR.InfoUtente infoUt = null;
                infoUt = UserManager.getInfoUtente(page);
               // UserManager.getUtente(

				int res = docsPaWS.ReportTrasmissioniDocFascUtente(oggettoTrasm,infoUt ,out fileDoc);

				if(res != 0)
				{
					throw new Exception("Si è verificato un errore nella creazione del report");
				} 
				else
					if(res == 0 && fileDoc == null)
					{
						page.Response.Write("<script>alert('Trasmissioni non trovate')</script>");
						return null;
					}
				return fileDoc;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
		
		public static FileDocumento getReportTrasmUO(Page page, DocsPAWA.DocsPaWR.FiltroRicerca[] filtriTrasm, string UO)
		{
			try 
			{
				FileDocumento fileDoc;
				int res = docsPaWS.ReportTrasmissioniUO(filtriTrasm, UO, UserManager.getInfoUtente(), out fileDoc);

				if(res != 0)
				{
					throw new Exception("Si è verificato un errore nella creazione del report");
				} 
				else
					if(res == 0 && fileDoc == null)
				{
					page.Response.Write("<script>alert('Trasmissioni non trovate')</script>");
					return null;
				}
				return fileDoc;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		#endregion

		#region gestione tasto back

		/// <summary>
		/// Impostazione valore in sessione
		/// </summary>
		/// <param name="sessionKey"></param>
		/// <param name="sessionValue"></param>
		private static void SetSessionValue(string sessionKey,object sessionValue)
		{	
			System.Web.HttpContext.Current.Session[sessionKey]=sessionValue;
		}

		/// <summary>
		/// Reperimento valore da sessione
		/// </summary>
		/// <param name="sessionKey"></param>
		/// <returns></returns>
		private static Object GetSessionValue(string sessionKey)
		{
			return System.Web.HttpContext.Current.Session[sessionKey];
		}

		/// <summary>
		/// Rimozione chiave di sessione
		/// </summary>
		/// <param name="sessionKey"></param>
		private static void RemoveSessionValue(string sessionKey)
		{
			System.Web.HttpContext.Current.Session.Remove(sessionKey);
		} 

		//numPag: numero della pagina dalla quale si è partiti nel datagrid
//		public static string getMemoriaNumPag(Page page) 
//		{
//			return (string) GetSessionValue("MemoriaNumPag");
//		}
//
//		public static void setMemoriaNumPag(Page page, string numPag) 
//		{
//			SetSessionValue("MemoriaNumPag",numPag);
//		}
//
//		public static void removeMemoriaNumPag(Page page) 
//		{
//			RemoveSessionValue("MemoriaNumPag");
//		}

		//Tipo trasmissione
		public static string getMemoriaTipoRic(Page page)
		{
			return (string) GetSessionValue("MemoriaTipoRic");
		}
		
		public static void setMemoriaTipoRic(Page page, string TipoRic)
		{
			SetSessionValue("MemoriaTipoRic",TipoRic);
		}

		public static void removeMemoriaTipoRic(Page page) 
		{
			RemoveSessionValue("MemoriaTipoRic");
		}

		//Tab: tab di ricerca di riferimento 
		public static string getMemoriaTab(Page page) 
		{
			return (string) GetSessionValue("MemoriaTabT");
		}

		public static void setMemoriaTab(Page page, string nomeTab) 
		{
			SetSessionValue("MemoriaTabT",nomeTab);
		}

		public static void removeMemoriaTab(Page page) 
		{
			RemoveSessionValue("MemoriaTabT");
		}
		//numPag: numero della pagina dalla quale si è partiti nel datagrid
		public static string getMemoriaNumPag(Page page) 
		{
			return (string) GetSessionValue("MemoriaTrasmNumPag");
		}

		public static void setMemoriaNumPag(Page page, string numPag) 
		{
			SetSessionValue("MemoriaTrasmNumPag",numPag);
		}

		public static void removeMemoriaNumPag(Page page) 
		{
			RemoveSessionValue("MemoriaTrasmNumPag");
		}
		//FiltriRicDoc: tab di ricerca di riferimento 
		public static DocsPAWA.DocsPaWR.FiltroRicerca[] getMemoriaFiltriRicTrasm(Page page) 
		{
			return (DocsPAWA.DocsPaWR.FiltroRicerca[]) GetSessionValue("MemoriaFiltriRicTrasm");
		}

		public static void setMemoriaFiltriRicTrasm(Page page, DocsPAWA.DocsPaWR.FiltroRicerca[] listaFiltri) 
		{
			SetSessionValue("MemoriaFiltriRicTrasm",listaFiltri);

		}

		public static void removeMemoriaFiltriRicTrasm(Page page) 
		{
			RemoveSessionValue("MemoriaFiltriRicTrasm");
		}
		
		public static void setMemoriaVisualizzaBack(Page page)
		{
			setMemoriaVisualizzaBack();
		}

		public static string getMemoriaVisualizzaBack(Page page)
		{
			return (string) getMemoriaVisualizzaBack();
		}

		public static void RemoveMemoriaVisualizzaBack(Page page)
		{
			RemoveMemoriaVisualizzaBack();
		}

        public static void setMemoriaVisualizzaBack()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["MemoriaVisualizzaBackT"] = "true";
        }

        public static string getMemoriaVisualizzaBack()
        {
            return (string)HttpContext.Current.Session["MemoriaVisualizzaBackT"];
        }

        public static void RemoveMemoriaVisualizzaBack()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("MemoriaVisualizzaBackT");
        }

		#endregion

        #region Modelli di Trasmissione
		/// <summary>
		/// Passando il parametro idRegistro = null, viene automaticamente assegnato il primo registro
		/// utile del ruolo, con cui si sta creando il modello di tramissione.
		/// </summary>
		/// <param name="trasmissione"></param>
		/// <param name="idRegistro"></param>
		/// <returns></returns>
		public static DocsPAWA.DocsPaWR.ModelloTrasmissione getModelloTrasmNuovo(DocsPAWA.DocsPaWR.Trasmissione trasmissione, string idRegistro)
		{
			DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
				
			//MODELLO
			if(trasmissione.tipoOggetto.ToString() == "DOCUMENTO")
				modello.CHA_TIPO_OGGETTO = "D";
			if(trasmissione.tipoOggetto.ToString() == "FASCICOLO")
				modello.CHA_TIPO_OGGETTO = "F";

			modello.ID_AMM = trasmissione.ruolo.idAmministrazione;
			modello.ID_PEOPLE = trasmissione.utente.idPeople;
			modello.ID_REGISTRO = idRegistro;
			
			//Controllo che il registro non sia indefinito, 
			//in questo caso associo il primo registro utile del ruolo in questione
			if(modello.ID_REGISTRO == null)
				modello.ID_REGISTRO = ((DocsPAWA.DocsPaWR.Registro) (UserManager.GetRegistriByRuolo(null,UserManager.getRuolo().systemId))[0]).systemId;
					
			modello.NOME = trasmissione.noteGenerali;
			modello.SINGLE = "0";
			//modello.SYSTEM_ID
			modello.VAR_NOTE_GENERALI = trasmissione.noteGenerali;

            //gestione della cessione dei diritti
            if (trasmissione.cessione != null && trasmissione.cessione.docCeduto)
            {
                modello.CEDE_DIRITTI = "1";
                if(trasmissione.cessione.idPeopleNewPropr != null && trasmissione.cessione.idPeopleNewPropr != "")
                    modello.ID_PEOPLE_NEW_OWNER = trasmissione.cessione.idPeopleNewPropr;
                if (trasmissione.cessione.idRuoloNewPropr != null && trasmissione.cessione.idRuoloNewPropr != "")
                    modello.ID_GROUP_NEW_OWNER = trasmissione.cessione.idRuoloNewPropr;
            }
            else
                modello.CEDE_DIRITTI = "0";

			
			//MITTENTE MODELLO
			modello.MITTENTE = new DocsPAWA.DocsPaWR.MittDest[1];
            modello.MITTENTE[0] = new MittDest();
			modello.MITTENTE[0].CHA_TIPO_MITT_DEST = "M";
			//modello.MITTENTE.CHA_TIPO_TRASM
			modello.MITTENTE[0].CHA_TIPO_URP = "P";
			modello.MITTENTE[0].DESCRIZIONE = trasmissione.ruolo.descrizione;
			modello.MITTENTE[0].ID_CORR_GLOBALI = Convert.ToInt32(trasmissione.ruolo.systemId);
			//modello.MITTENTE.ID_MODELLO
			//modello.MITTENTE.ID_RAGIONE
			//modello.MITTENTE.SYSTEM_ID
			modello.MITTENTE[0].VAR_COD_RUBRICA = trasmissione.ruolo.codiceRubrica;
			//modello.MITTENTE.VAR_NOTE_SING

			//RAGIONI DESTINATARI MODELLO
			DocsPaWR.RagioneDest ragioneDest_1 = new DocsPAWA.DocsPaWR.RagioneDest();
			ArrayList trasmSingole = new ArrayList(trasmissione.trasmissioniSingole);

			if(trasmSingole.Count != 0)
			{
				//Aggiungo la prima trasmissione singola
				DocsPaWR.TrasmissioneSingola trasmSingola_1 = (DocsPAWA.DocsPaWR.TrasmissioneSingola) trasmSingole[0];
				ragioneDest_1.CHA_TIPO_RAGIONE = trasmSingola_1.ragione.tipo;
				ragioneDest_1.RAGIONE = trasmSingola_1.ragione.descrizione;
				
				//Aggiungo il primo destinatario
				DocsPaWR.MittDest dest_1 = new DocsPAWA.DocsPaWR.MittDest();                
				
				dest_1.CHA_TIPO_MITT_DEST = "D";
				dest_1.CHA_TIPO_TRASM = trasmSingola_1.tipoTrasm;
				if(trasmSingola_1.tipoDest.ToString() == "RUOLO")
					dest_1.CHA_TIPO_URP = "R";
				if(trasmSingola_1.tipoDest.ToString() == "UTENTE")
					dest_1.CHA_TIPO_URP = "U";
				dest_1.DESCRIZIONE = trasmSingola_1.corrispondenteInterno.descrizione;
				dest_1.ID_CORR_GLOBALI = Convert.ToInt32(trasmSingola_1.corrispondenteInterno.systemId);
				//dest.ID_MODELLO
				dest_1.ID_RAGIONE = Convert.ToInt32(trasmSingola_1.ragione.systemId);
				//dest.SYSTEM_ID
				dest_1.VAR_COD_RUBRICA = trasmSingola_1.corrispondenteInterno.codiceRubrica;
				dest_1.VAR_NOTE_SING = trasmSingola_1.noteSingole;

                // flag nascondi versioni precedenti
                dest_1.NASCONDI_VERSIONI_PRECEDENTI = trasmSingola_1.hideDocumentPreviousVersions;

                ////gestione utenti con notifica
                dest_1.UTENTI_NOTIFICA = addNotificheUtenti(trasmSingola_1);                

                ragioneDest_1.DESTINATARI = Utils.addToArrayMittDest(ragioneDest_1.DESTINATARI, dest_1);
                modello.RAGIONI_DESTINATARI = Utils.addToArrayRagioneDest(modello.RAGIONI_DESTINATARI, ragioneDest_1);
				
				for(int i=1; i<trasmissione.trasmissioniSingole.Length; i++)
				{
					DocsPaWR.TrasmissioneSingola trasmSingola_2 = (DocsPAWA.DocsPaWR.TrasmissioneSingola) trasmSingole[i];
					int ragioneDestDaModificare = -1;

					for(int j=0; j<modello.RAGIONI_DESTINATARI.Length; j++)
					{
						DocsPaWR.RagioneDest ragioneDest_2 = (DocsPAWA.DocsPaWR.RagioneDest) modello.RAGIONI_DESTINATARI[j];
						if(ragioneDest_2.RAGIONE == trasmSingola_2.ragione.descrizione)
						{
							ragioneDestDaModificare = j;
							break;
						}
					}
					
					if(ragioneDestDaModificare != -1)
					{
						//Aggiungo un destinatario ad una ragioneDest esistente
						DocsPaWR.MittDest dest_2 = new DocsPAWA.DocsPaWR.MittDest();
			
						dest_2.CHA_TIPO_MITT_DEST = "D";
						dest_2.CHA_TIPO_TRASM = trasmSingola_2.tipoTrasm;
						if(trasmSingola_2.tipoDest.ToString() == "RUOLO")
							dest_2.CHA_TIPO_URP = "R";
						if(trasmSingola_2.tipoDest.ToString() == "UTENTE")
							dest_2.CHA_TIPO_URP = "U";
						dest_2.DESCRIZIONE = trasmSingola_2.corrispondenteInterno.descrizione;
						dest_2.ID_CORR_GLOBALI = Convert.ToInt32(trasmSingola_2.corrispondenteInterno.systemId);
						//dest.ID_MODELLO
						dest_2.ID_RAGIONE = Convert.ToInt32(trasmSingola_2.ragione.systemId);
						//dest.SYSTEM_ID
						dest_2.VAR_COD_RUBRICA = trasmSingola_2.corrispondenteInterno.codiceRubrica;
						dest_2.VAR_NOTE_SING = trasmSingola_2.noteSingole;

                        // flag nascondi versioni precedenti
                        dest_1.NASCONDI_VERSIONI_PRECEDENTI = trasmSingola_2.hideDocumentPreviousVersions;

                        ////gestione utenti con notifica
                        dest_2.UTENTI_NOTIFICA = addNotificheUtenti(trasmSingola_2);
                        
						((DocsPAWA.DocsPaWR.RagioneDest) modello.RAGIONI_DESTINATARI[ragioneDestDaModificare]).DESTINATARI = Utils.addToArrayMittDest(((DocsPAWA.DocsPaWR.RagioneDest) modello.RAGIONI_DESTINATARI[ragioneDestDaModificare]).DESTINATARI,dest_2);                        
					}
					else
					{
						//Aggiungo una nuova ragioneDest
						DocsPaWR.RagioneDest ragioneDest_3 = new DocsPAWA.DocsPaWR.RagioneDest();
						
						ragioneDest_3.CHA_TIPO_RAGIONE = trasmSingola_2.ragione.tipo;
						ragioneDest_3.RAGIONE = trasmSingola_2.ragione.descrizione;
						
						DocsPaWR.MittDest dest_3 = new DocsPAWA.DocsPaWR.MittDest();
			
						dest_3.CHA_TIPO_MITT_DEST = "D";
						dest_3.CHA_TIPO_TRASM = trasmSingola_2.tipoTrasm;
						if(trasmSingola_2.tipoDest.ToString() == "RUOLO")
							dest_3.CHA_TIPO_URP = "R";
						if(trasmSingola_2.tipoDest.ToString() == "UTENTE")
							dest_3.CHA_TIPO_URP = "U";
						dest_3.DESCRIZIONE = trasmSingola_2.corrispondenteInterno.descrizione;
						dest_3.ID_CORR_GLOBALI = Convert.ToInt32(trasmSingola_2.corrispondenteInterno.systemId);
						//dest.ID_MODELLO
						dest_3.ID_RAGIONE = Convert.ToInt32(trasmSingola_2.ragione.systemId);
						//dest.SYSTEM_ID
						dest_3.VAR_COD_RUBRICA = trasmSingola_2.corrispondenteInterno.codiceRubrica;
						dest_3.VAR_NOTE_SING = trasmSingola_2.noteSingole;

                        // flag nascondi versioni precedenti
                        dest_3.NASCONDI_VERSIONI_PRECEDENTI = trasmSingola_2.hideDocumentPreviousVersions;

                        ////gestione utenti con notifica
                        dest_3.UTENTI_NOTIFICA = addNotificheUtenti(trasmSingola_2);                        
                        
						ragioneDest_3.DESTINATARI   = Utils.addToArrayMittDest(ragioneDest_3.DESTINATARI,dest_3);
						modello.RAGIONI_DESTINATARI = Utils.addToArrayRagioneDest(modello.RAGIONI_DESTINATARI,ragioneDest_3);
					}
					
				}
			}            
            
			return modello;
		}

        private static DocsPaWR.UtentiConNotificaTrasm[] addNotificheUtenti(DocsPaWR.TrasmissioneSingola trasmSingola)
        {
            int contaU = 0;
           
            DocsPaWR.UtentiConNotificaTrasm[] listaUtentiNotifica = new UtentiConNotificaTrasm[trasmSingola.trasmissioneUtente.Length];                               
            
            foreach (DocsPaWR.TrasmissioneUtente trasmU in trasmSingola.trasmissioneUtente)
            {
                listaUtentiNotifica[contaU] = new UtentiConNotificaTrasm();
                listaUtentiNotifica[contaU].ID_PEOPLE = trasmU.utente.idPeople;
                listaUtentiNotifica[contaU].CODICE_UTENTE = trasmU.utente.codiceRubrica;                    
                if (trasmU.daNotificare)
                    listaUtentiNotifica[contaU].FLAG_NOTIFICA = "1";
                else
                    listaUtentiNotifica[contaU].FLAG_NOTIFICA = "0";
                contaU++;
            }

            return listaUtentiNotifica;
        }

		#endregion Modelli di Trasmissione

        /// <summary>
        /// Ritorna il valore della chiav edi web.config, che gestisce
        /// se gli utenti di un ruolo destinatario di una trasmissione devono comparire di default
        /// flaggati o no. nome = UT_TX_RUOLO_CHECKED. Se la chiave non esiste ritorna TRUE condizione di default in DocsPA, quindi TUTTI
        /// gli utenti compaioni checked.
        /// </summary>
        /// <returns></returns>
        public static bool getTxRuoloUtentiChecked()
        {

            string value=System.Configuration.ConfigurationManager.AppSettings["UT_TX_RUOLO_CHECKED"];
            if(value!=null && value.ToLower()=="false")
                return false;
            else
                return true;



        }

        #region Nuovi Metodi per todolist


	/// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="registri"></param>
        /// <param name="type"></param>
        /// <param name="filter"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        public static DocsPAWA.DocsPaWR.infoToDoList[] getMyTodolist(string docOrFasc, string registri,
            DocsPAWA.DocsPaWR.FiltroRicerca [] filter,
            int requestedPage, int pageSize, out int totalRecordCount)
        {
            totalRecordCount = 0;

            try
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                return docsPaWS.getMyTodoList(infoUtente, docOrFasc, registri, filter, requestedPage, pageSize, out totalRecordCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DocsPAWA.DocsPaWR.infoToDoList[] getMyNewTodolist(string registri,
           DocsPAWA.DocsPaWR.FiltroRicerca[] filter,
           int requestedPage, int pageSize, out int totalRecordCount, out int totalTrasmNonViste)
        {
            totalRecordCount = 0;
            totalTrasmNonViste = 0;

            try
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                return docsPaWS.getMyNewTodoList(infoUtente, registri, filter, requestedPage, pageSize, out totalRecordCount, out totalTrasmNonViste);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        
        // old method
        //public static DocsPAWA.DocsPaWR.infoToDoList[] getMyTodolist(string idPeople, string idGruppo, string registri, string type, DocsPAWA.DocsPaWR.FiltroRicerca [] filter)
        //{
        //    try
        //    {
        //        return docsPaWS.getMyTodoList(idPeople, idGruppo, registri, type, filter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static DocsPAWA.DocsPaWR.infoToDoList[] rimuoviToDoListACL(string numProto, string idPeopleDest, string idRuoloDest, string registri, DocsPAWA.DocsPaWR.FiltroRicerca[] filter, string tipoObj, DocsPaWR.InfoUtente infoUtente)
        public static bool rimuoviToDoListACL(string idTrasmUtente, string idTrasmSingola, string idPeople)
        {
            try
            {
                return docsPaWS.RimuoviToDoListACL(idTrasmUtente, idTrasmSingola, idPeople);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
       



        /// <summary>
        /// Metodo recupero oggetto Trasmissione partendo da oggetto InfoTodoList
        /// </summary>
        /// <param name="ut"></param>
        /// <param name="role"></param>
        /// <param name="todo"></param>
        /// <returns></returns>
        public static DocsPAWA.DocsPaWR.Trasmissione [] trasmGetDettaglioTrasmissione(DocsPAWA.DocsPaWR.Utente ut, DocsPAWA.DocsPaWR.Ruolo role, DocsPAWA.DocsPaWR.infoToDoList todo)
        {
            try
            {
                return docsPaWS.trasmGetDettaglioTrasmissione(ut,role,todo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool getAllTodoList(DocsPaWR.Utente userHome, Ruolo ruoloCorrente)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.getAllTodoList(userHome, ruoloCorrente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retValue;
        }

        public static bool getPredInTodoList(DocsPaWR.Utente userHome, Ruolo ruoloCorrente)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.getPredInTodoList(userHome, ruoloCorrente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retValue;
        }

        public static DocsPaWR.AllToDoList[] getDettagliAllTodoList(DocsPaWR.Utente utente)
        {
            DocsPaWR.AllToDoList[] lista = null;
            bool retValue = false;
            try
            {
                lista = docsPaWS.getDettagliAllTodoList(utente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }
        #endregion

        #region InfoTrasm

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFascicolo"></param>        
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaWR.InfoTrasmissione[] GetInfoTrasmissioniEffettuate(string idDocOrFasc, string docOrFasc, ref DocsPaWR.SearchPagingContext pagingContext)
        {
            DocsPaWR.InfoTrasmissione[] retValue = null;

            try
            {
                DocsPaWebService ws = new DocsPaWebService();

                retValue = ws.GetInfoTrasmissioniEffettuate(UserManager.getInfoUtente(), idDocOrFasc, docOrFasc, ref pagingContext);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore nella ricerca delle trasmissioni effettuate");
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaWR.InfoTrasmissione[] GetInfoTrasmissioniRicevute(string idDocOrFasc, string docOrFasc, ref DocsPaWR.SearchPagingContext pagingContext)
        {
            DocsPaWR.InfoTrasmissione[] retValue = null;

            try
            {
                DocsPaWebService ws = new DocsPaWebService();
                
                retValue = ws.GetInfoTrasmissioniRicevute(UserManager.getInfoUtente(), idDocOrFasc, docOrFasc, ref pagingContext);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore nella ricerca delle trasmissioni ricevute");
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento delle informazioni di stato relative ad una trasmissione utente
        /// </summary>
        /// <param name="idTrasmissioneUtente"></param>
        /// <returns></returns>
        public static DocsPaWR.StatoTrasmissioneUtente getStatoTrasmissioneUtente(string idTrasmissioneUtente)
        {
            try
            {
                using (DocsPaWebService ws = new DocsPaWebService())
                    return ws.GetStatoTrasmissioneUtente(UserManager.getInfoUtente(), idTrasmissioneUtente);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore nel reperimento dello stato della trasmissione utente");
            }
        }

        #endregion

        #region Metodi per Modelli di Trasmissione
        /// <summary>
        /// Tramissione di un fascicolo usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneFascDaModello(DocsPAWA.DocsPaWR.ModelloTrasmissione modello, string idStato, Fascicolo fascicolo, Page page)
        {
            logger.Info("BEGIN");
            Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            
            trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
            trasmissione.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fascicolo, page);
            trasmissione.utente = UserManager.getUtente(page);
            trasmissione.ruolo = UserManager.getRuolo(page);
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;  
            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
          
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    DocsPaWR.Corrispondente corr = new Corrispondente();
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, null, fascicolo, page);  
                    }
                    if (corr != null)
                    {
                        DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, page);
                    }
                }
            }
            trasmissione = TrasmManager.impostaNotificheUtentiDaModello(trasmissione, modello);
            trasmissione = TrasmManager.saveTrasm(page, trasmissione);
            TrasmManager.executeTrasm(page, trasmissione);
            if(idStato != null && idStato != "")
                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, fascicolo.systemID, idStato,page);

            logger.Info("END");
        }

        /// <summary>
        /// Tramissione di un fascicolo usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneFascDaModello(DocsPAWA.DocsPaWR.ModelloTrasmissione modello, string idStato, InfoFascicolo infoFascicolo, Page page)
        {
            logger.Info("BEGIN");
            Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            
            trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
            trasmissione.infoFascicolo = infoFascicolo;
            trasmissione.utente = UserManager.getUtente(page);
            trasmissione.ruolo = UserManager.getRuolo(page);
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
         
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    DocsPaWR.Corrispondente corr = new Corrispondente();
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        Fascicolo fascicolo = FascicoliManager.getFascicoloById(page, infoFascicolo.idFascicolo);
                        corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, null, fascicolo, page);
                    }
                    DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                    trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, page);
                }
            }
            trasmissione = TrasmManager.impostaNotificheUtentiDaModello(trasmissione, modello);
            trasmissione = TrasmManager.saveTrasm(page, trasmissione);
            TrasmManager.executeTrasm(page, trasmissione);
            if (idStato != null && idStato != "")
                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, infoFascicolo.idFascicolo, idStato,page);

            logger.Info("END");
        }
        
        /// <summary>
        /// Tramissione di un documento usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneDocDaModello(DocsPAWA.DocsPaWR.ModelloTrasmissione modello, string idStato, SchedaDocumento schedaDocumento, Page page)
        {
            logger.Info("BEGIN");
            Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

            trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
            trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
            trasmissione.utente = UserManager.getUtente(page);
            trasmissione.ruolo = UserManager.getRuolo(page);
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
             
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.Corrispondente corr = new Corrispondente();
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDocumento, null, page); ;
                    }
                    if (corr != null)
                    {
                        DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, page);
                    }
                }
            }
            trasmissione = TrasmManager.impostaNotificheUtentiDaModello(trasmissione, modello);

            if (trasmissione != null && modello.CEDE_DIRITTI.Equals("1"))
            {

                if (trasmissione.cessione == null)
                {
                    DocsPaWR.CessioneDocumento cessione = new DocsPAWA.DocsPaWR.CessioneDocumento();
                    cessione.docCeduto = true;
                    cessione.idPeople = UserManager.getInfoUtente(page).idPeople;
                    cessione.idRuolo = UserManager.getInfoUtente(page).idGruppo;
                    cessione.userId = UserManager.getInfoUtente(page).userId;
                    cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                    cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                    trasmissione.cessione = cessione;
                }
            }

            trasmissione = TrasmManager.saveTrasm(page, trasmissione);
            TrasmManager.executeTrasm(page, trasmissione);
            if (idStato != null && idStato != "")
                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, schedaDocumento.docNumber, idStato,page);

            logger.Info("END");
        }

        /// <summary>
        /// Tramissione di un documento usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneDocDaModello(DocsPAWA.DocsPaWR.ModelloTrasmissione modello, string idStato, InfoDocumento infoDocumento, Page page)
        {
            logger.Info("BEGIN");
            Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

            trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
            trasmissione.infoDocumento = infoDocumento;
            trasmissione.utente = UserManager.getUtente(page);
            trasmissione.ruolo = UserManager.getRuolo(page);
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
              
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    DocsPaWR.Corrispondente corr = new Corrispondente();
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaIE(page, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(page, infoDocumento.idProfile, infoDocumento.docNumber);
                        corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDoc, null, page);
                    }
                    if (corr != null)
                    {
                        DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, page);
                    }
                }
            }
            trasmissione = TrasmManager.impostaNotificheUtentiDaModello(trasmissione, modello);
            trasmissione = TrasmManager.saveTrasm(page, trasmissione);
            TrasmManager.executeTrasm(page, trasmissione);
            if (idStato != null && idStato != "")
                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, infoDocumento.docNumber, idStato,page);

            logger.Info("END");
        }

        private static DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr, Page page)
        {
            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = UserManager.getInfoUtente(page).idAmministrazione;
            qco.fineValidita = true;
            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
            return UserManager.getListaCorrispondenti(page, qco);
        }

        public static DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti, Page page)
        {
            //Quando la ragione di trasmissione ha mantieni lettura, anche la trasmissione deve mantenere la lettura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniLettura = true;
                }
            }

            //
            // Mev Cessione Diritti - Mantieni Scrittura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniScrittura = true;
                }
            }
            // End Mev
            //

            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;
            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = dataScadenza;
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPAWA.DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr, page);
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;

                    //Andrea
                    throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                    //End Andrea
                }
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPAWA.DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;
                
                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                    
                }
                //End Andrea
                else
                trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.getRuolo();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(page, qca, theUo);
                
                //Andrea
                if (ruoli==null || ruoli.Length == 0)
                {
                    throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti, page);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

            return trasmissione;
        }

        public static DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, Page page)
        {
            return addTrasmissioneSingola(trasmissione, corr, ragione, note, tipoTrasm, scadenza, false, page);
        }

        public static Corrispondente getCorrispondenti(string tipo_destinatario, SchedaDocumento schedaDocumento, Fascicolo fascicolo,Page page)
        {
            DocsPaWR.Corrispondente corr = new Corrispondente();
            //se la il modello di trasmissione ha come destinatario l'utente proprietario del documento
            if (schedaDocumento != null)
            {
                if (tipo_destinatario == "UT_P")
                {
                    string utenteProprietario = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                        }
                        else utenteProprietario = schedaDocumento.protocollatore.utente_idPeople;
                       
                    }
                    else
                    {
                        utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                    }
                    corr = UserManager.getCorrispondenteByIdPeople(page, utenteProprietario, DocsPaWR.AddressbookTipoUtente.INTERNO);
                }
                //ruolo proprietario del documento
                if (tipo_destinatario == "R_P")
                {
                    string idCorrGlobaliRuolo = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        else
                        idCorrGlobaliRuolo = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                    }
                    else
                    {
                        idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                    }
                    // corr = UserManager.getCorrispondenteBySystemID(page, idCorrGlobaliRuolo);
                    corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuolo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                }
                //trasmissione a UO del proprietario
                if (tipo_destinatario == "UO_P")
                {
                    string idCorrGlobaliUo = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        }
                        else
                        idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                    }
                    else
                    {
                        idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                    }
                    corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliUo, DocsPaWR.AddressbookTipoUtente.INTERNO);

                }//RUOLO Responsabile UO proprietario
                if (tipo_destinatario == "RSP_P")
                {
                    string idCorrGlobaliUo = string.Empty;
                    string idCorr = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                         //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                            //idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                        }
                    }
                    else
                    {
                        idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                    }
                    idCorr = UserManager.getRuolo(page).systemId;
                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //Ruolo segretario UO PROPRIETARIO
                if (tipo_destinatario == "R_S")
                {
                    string idCorrGlobaliUo = string.Empty;
                    string idCorr = String.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                           // idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                           // idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                        }
                    }
                    else
                    {
                        idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                    }
                    idCorr = UserManager.getRuolo(page).systemId;
                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //ruolo responsabile uo mittente
                if (tipo_destinatario == "RSP_M")
                {
                    string idCorrGlobaliUo = UserManager.getRuolo(page).uo.systemId;
                    string idCorr = UserManager.getRuolo(page).systemId;

                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }

                //ruolo segretario uo mittente
                if (tipo_destinatario == "S_M")
                {
                    string idCorrGlobaliUo = UserManager.getRuolo(page).uo.systemId;
                    string idCorr = UserManager.getRuolo(page).systemId;

                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }
            }
            else
            {
                //trsmissione a utente proprietario del documento
                if (tipo_destinatario == "UT_P")
                {
                    string utenteProprietario = string.Empty;
                    utenteProprietario = fascicolo.creatoreFascicolo.idPeople;
                    corr = UserManager.getCorrispondenteByIdPeople(page, utenteProprietario, DocsPaWR.AddressbookTipoUtente.INTERNO);
                }
                //ruolo proprietario
                if (tipo_destinatario == "R_P")
                {
                    string idCorrGlobaliRuolo = string.Empty;
                    idCorrGlobaliRuolo = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;                    
                    corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuolo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                }
                //uo proprietaria
                if (tipo_destinatario == "UO_P")
                {
                    string idCorrGlobaliUo = string.Empty;
                    idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                    corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                }
                //responsabile uo proprietario
                if (tipo_destinatario == "RSP_P")
                {
                    string idCorrGlobaliUo = string.Empty;
                    string idCorr = string.Empty;

                    idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                    idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;               
                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //ruolo segretario uo del proprietario
                if (tipo_destinatario == "R_S")
                {
                    string idCorrGlobaliUo = string.Empty;
                    string idCorr = string.Empty;

                    idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                    idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;

                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //ruolo responsabile uo del mittente
                if (tipo_destinatario == "RSP_M")
                {
                    string idCorrGlobaliUo = UserManager.getRuolo(page).uo.systemId;
                    string idCorr = UserManager.getRuolo(page).systemId;

                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //RUOLO SEGRETARIO UO DEL MITTENTE
                if (tipo_destinatario == "S_M")
                {
                    string idCorrGlobaliUo = UserManager.getRuolo(page).uo.systemId;
                    string idCorr = UserManager.getRuolo(page).systemId;

                    string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = null;
                    }
                }

            }
            return corr;
        }

        private static DocsPAWA.DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPAWA.DocsPaWR.Trasmissione objTrasm, DocsPAWA.DocsPaWR.ModelloTrasmissione modello)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = TrasmManager.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId, modello);
                        }
                    }
                }
            }
            return objTrasm;
        }

        private static bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, DocsPAWA.DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = true;

            //DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                            {
                                if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        #endregion Metodi per Modelli di Trasmissione

        public static Trasmissione[] getQueryRicevuteLite(
                                Page page,
                                TrasmissioneOggettoTrasm oggTrasm,
                                Utente user,
                                Ruolo ruolo,
                                FiltroRicerca[] filterRic,
                                int pageNumber,
                                bool excel,
                                int pageSize,
                                out int totalPageNumber,
                                out int recordCount)
        {
            totalPageNumber = 0;
            recordCount = 0;

            try
            {
 
                Trasmissione[] result = docsPaWS.TrasmissioneGetQueryRicevuteLite(oggTrasm, filterRic, user, ruolo, pageNumber,excel,pageSize, out totalPageNumber, out recordCount);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static Trasmissione[] getQueryEffettuatePagingLite(Page page, TrasmissioneOggettoTrasm oggTrasm, Utente user, Ruolo ruolo, FiltroRicerca[] filterRic, int pageNumber, bool excel, int pageSize, out int totalPageNumber, out int recordCount)
        {
            totalPageNumber = 0;
            recordCount = 0;

            try
            {
                Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuatePagingLite(oggTrasm, filterRic, user, ruolo, pageNumber, excel,pageSize, out totalPageNumber, out recordCount);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			}
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static TrasmissioneSingola RoleTransmissionWithHistoricized(List<TrasmissioneSingola> singleTransmissions, String roleCorrGlob)
        {
            TrasmissioneSingola retVal = null;

            // Se la lista di trasmissioni singole contiene un ruolo con id pari a quello passato, è stato
            // richiesto il dettaglio della trasmissione da un ruolo attivo, altrimenti bisogna restituire la
            // prima trasmissione utente che contiene uno dei ruoli padri del ruolo attuale.
            // Questo controllo andrebbe in realtà fatto da backend ma attualmente tutte le trasmissioni vengono gestite da
            // frontend
            retVal = singleTransmissions.Where(tu => tu.corrispondenteInterno.systemId == roleCorrGlob).FirstOrDefault();
            if (retVal == null)
            {
                RoleChainResponse roleChain = docsPaWS.GetRoleChainsId(new RoleChainRequest() { IdCorrGlobRole = roleCorrGlob });
                foreach (var id in roleChain.RoleChain)
                {
                    retVal = singleTransmissions.Where(tu => tu.corrispondenteInterno.systemId == id).FirstOrDefault();
                    if (retVal != null) break;
                }
 
            }

            return retVal;
        }

        /// <summary>
        /// Restituisce la ragione di trasmissione NOTIFICA
        /// </summary>
        /// <param name="idAmm"> Id dell'amministrazione corrente</param>
        /// <returns></returns>
        public static RagioneTrasmissione GetRagioneNotifica(string idAmm)
        {
            RagioneTrasmissione ragione = docsPaWS.GetRagioneNotifica(idAmm);
            return ragione;
        }

	
    }
}
