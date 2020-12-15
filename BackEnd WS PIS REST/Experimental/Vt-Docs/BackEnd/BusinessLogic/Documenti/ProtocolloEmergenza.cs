using System;
using System.Collections;
using System.Configuration;
using System.Data;
using log4net;

namespace BusinessLogic.Documenti
{
	public class ProtocolloEmergenza
	{
        private static ILog logger = LogManager.GetLogger(typeof(ProtocolloEmergenza));

		//questo metodo crea un documento protocollato su docsPa a partire da un protocollo di emergenza
        public static DocsPaVO.documento.resultProtoEmergenza importaProtoEmergenzaGrigi(DocsPaVO.documento.ProtocolloEmergenzaGrigi documento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.documento.resultProtoEmergenza res = new DocsPaVO.documento.resultProtoEmergenza();
            try
            {
                DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
                DocsPaVO.documento.SchedaDocumento schedaDocumento;

                schedaDocumento = creaSchedaDocumentoGrigio(documento, infoUtente, ruolo);
                schedaDocumento = DocSave.addDocGrigia(schedaDocumento, infoUtente, ruolo);
                if (schedaDocumento != null)
                    res.isSaved = true;

                //si classifica il documento
                classificaDocumento(schedaDocumento, documento.codiceClassifica, ref res, infoUtente, ruolo);

                //si trasmette il documento
                trasmettiDocumento(schedaDocumento, documento.templateTrasmissione, ref res, infoUtente, ruolo, null);
            }
            catch (Exception e)
            {
                res.messaggio = res.messaggio + " - " + e.Message;
                logger.Debug(e.ToString());
                return res;
            }
            return res;
        }

		//questo metodo crea un documento protocollato su docsPa a partire da un protocollo di emergenza
		public static DocsPaVO.documento.resultProtoEmergenza importaProtoEmergenza(DocsPaVO.documento.ProtocolloEmergenza protoEmergenza, DocsPaVO.utente.InfoUtente infoUtente) 
		{	
			DocsPaVO.documento.resultProtoEmergenza res = new DocsPaVO.documento.resultProtoEmergenza();

			try 
			{
				DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
				DocsPaVO.documento.SchedaDocumento schedaDocumento;
				logger.Debug("add protocollo Emergenza");
				//si costruisce l'oggetto schedaDocumento
				schedaDocumento = creaSchedaDocumento(protoEmergenza, infoUtente, ruolo);
				//si protocolla
				DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione;
				schedaDocumento = ProtoManager.protocolla(schedaDocumento, ruolo, infoUtente, out risultatoProtocollazione);
				if (schedaDocumento != null)
					res.isProtocollato = true;
					
				//si classifica il documento
				classificaDocumento(schedaDocumento, protoEmergenza.codiceClassifica, ref res, infoUtente, ruolo);

				//eventualmente si annulla il documento
				if (protoEmergenza.dataAnnullamento != null && !protoEmergenza.dataAnnullamento.Equals(""))
					annullaProtocollo(schedaDocumento, protoEmergenza, ref res, infoUtente);
				else
					//si trasmette il documento
					trasmettiDocumento(schedaDocumento, protoEmergenza.templateTrasmissione, ref res, infoUtente, ruolo, protoEmergenza.idRegistro);
			}
			catch (Exception e) 
			{
				res.messaggio = res.messaggio + " - " + e.Message;
				logger.Debug(e.ToString());
				return res;
			}
			return res;
		}
	
	
		internal static DocsPaVO.documento.SchedaDocumento setProtoArrivo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloEmergenza protoEmergenza)
		{
			DocsPaVO.documento.ProtocolloEntrata proto= new DocsPaVO.documento.ProtocolloEntrata();
			//proto.daProtocollare = "1";
			if(protoEmergenza.dataProtocolloMittente!=null 
				&& protoEmergenza.dataProtocolloMittente!="")
			{
				protoEmergenza.dataProtocolloMittente=protoEmergenza.dataProtocolloMittente.Trim();
				if(protoEmergenza.dataProtocolloMittente.Length<10)
				{
					//anno di 2 cifre, non va su docspa:
					DateTime d=new DateTime();
					d = System.Convert.ToDateTime(protoEmergenza.dataProtocolloMittente);
					string day=d.Day.ToString();
					string month=d.Month.ToString();
					if(day!=null && day.Length==1)
						day="0"+day;
					if(month!=null && month.Length==1)
						month="0"+month;
					protoEmergenza.dataProtocolloMittente=day+"/"+month+"/"+d.Year.ToString();
				}
			}
			proto.dataProtocolloMittente = protoEmergenza.dataProtocolloMittente;
			proto.descrizioneProtocolloMittente = protoEmergenza.numeroProtocolloMittente;

			//mittente
			DocsPaVO.utente.Corrispondente mittente= new DocsPaVO.utente.Corrispondente();
			mittente.descrizione = protoEmergenza.mittenti[0].ToString();
			proto.mittente = mittente;
			schedaDocumento.protocollo = proto;

			return schedaDocumento;
		}


		internal static DocsPaVO.documento.SchedaDocumento setProtoPartenza(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloEmergenza protoEmergenza)
		{
			DocsPaVO.documento.ProtocolloUscita proto= new DocsPaVO.documento.ProtocolloUscita();
			//proto.daProtocollare = "1";
			//destinatari
			proto.destinatari = new ArrayList(protoEmergenza.destinatari.Count);
			for (int i=0; i < protoEmergenza.destinatari.Count; i++)
			{
				DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
				corr.descrizione = protoEmergenza.destinatari[i].ToString();
				
				proto.destinatari.Add(corr);
			}
			//destinatariCC
			proto.destinatariConoscenza = new ArrayList(protoEmergenza.destinatariCC.Count);
			for (int j=0; j < protoEmergenza.destinatariCC.Count; j++)
			{
				DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
				corr.descrizione = protoEmergenza.destinatariCC[j].ToString();
				proto.destinatariConoscenza.Add(corr);
			}
			schedaDocumento.protocollo = proto;
			return schedaDocumento;
		}

		internal static DocsPaVO.documento.SchedaDocumento setProtoInterno(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloEmergenza protoEmergenza)
		{
			DocsPaVO.documento.ProtocolloInterno proto= new DocsPaVO.documento.ProtocolloInterno();
			//proto.daProtocollare = "1";
			//destinatari
			proto.destinatari = new ArrayList(protoEmergenza.destinatari.Count);
			for (int i=0; i < protoEmergenza.destinatari.Count; i++)
			{
				DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
				corr.descrizione = protoEmergenza.destinatari[i].ToString();
				
				proto.destinatari.Add(corr);
			}
			//destinatariCC
			proto.destinatariConoscenza = new ArrayList(protoEmergenza.destinatariCC.Count);
			for (int j=0; j < protoEmergenza.destinatariCC.Count; j++)
			{
				DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
				corr.descrizione = protoEmergenza.destinatariCC[j].ToString();
				proto.destinatariConoscenza.Add(corr);
			}
			schedaDocumento.protocollo = proto;
			return schedaDocumento;
		}

        internal static DocsPaVO.documento.SchedaDocumento creaSchedaDocumentoGrigio(DocsPaVO.documento.ProtocolloEmergenzaGrigi documento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();

            schedaDocumento.systemId = null;
            schedaDocumento = setFusionFields(schedaDocumento, infoUtente);            
            //oggetto
            schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
            schedaDocumento.oggetto.descrizione = documento.oggetto;
            //protocollo emergenza
            schedaDocumento.datiEmergenza = new DocsPaVO.documento.DatiEmergenza();
            schedaDocumento.datiEmergenza.dataProtocollazioneEmergenza = documento.dataCreazione;
            schedaDocumento.datiEmergenza.protocolloEmergenza = documento.segnatura;

            return schedaDocumento;
        }

		internal static DocsPaVO.documento.SchedaDocumento creaSchedaDocumento(DocsPaVO.documento.ProtocolloEmergenza protoEmergenza, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
		{
			DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
			schedaDocumento = setFusionFields(schedaDocumento, infoUtente);
			schedaDocumento.tipoProto = protoEmergenza.tipoProtocollo;
			//registro
			DocsPaVO.utente.Registro registro;
			registro = Utenti.RegistriManager.getRegistro(protoEmergenza.idRegistro);
			schedaDocumento.registro = registro;
			logger.Debug("data apertura: "+ registro.dataApertura);
			logger.Debug("data chiusura: "+ registro.dataChiusura);
			logger.Debug("data ultimo prot: "+ registro.dataUltimoProtocollo);

			//oggetto
			schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
			schedaDocumento.oggetto.descrizione = protoEmergenza.oggetto;
			//protocollo
			if (schedaDocumento.tipoProto.Equals("A"))
				schedaDocumento = setProtoArrivo(schedaDocumento, protoEmergenza);
			else if (schedaDocumento.tipoProto.Equals("P"))
				schedaDocumento = setProtoPartenza(schedaDocumento, protoEmergenza);
			else if (schedaDocumento.tipoProto.Equals("I"))
				schedaDocumento = setProtoInterno(schedaDocumento, protoEmergenza);

			//protocollo emergenza
			schedaDocumento.datiEmergenza = new DocsPaVO.documento.DatiEmergenza();
			schedaDocumento.datiEmergenza.dataProtocollazioneEmergenza = protoEmergenza.dataProtocollazione;
			schedaDocumento.datiEmergenza.protocolloEmergenza = protoEmergenza.numero;
			//schedaDocumento.datiEmergenza.cognomeProtocollatoreEmergenza = ...?;?
			//schedaDocumento.datiEmergenza.nomeProtocollatoreEmergenza = ...?;
			schedaDocumento.datiEmergenza = new DocsPaVO.documento.DatiEmergenza();
			schedaDocumento.datiEmergenza.dataProtocollazioneEmergenza = protoEmergenza.dataProtocollazione;
			schedaDocumento.datiEmergenza.protocolloEmergenza = protoEmergenza.numero;
			//schedaDocumento.protocollo.datiEmergenza.cognomeProtocollatoreEmergenza = ...?;
			//schedaDocumento.protocollo.datiEmergenza.nomeProtocollatoreEmergenza = ...?;
			return schedaDocumento;
		} 

		protected static DocsPaVO.documento.SchedaDocumento setFusionFields(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente) 
		{
			// campi obbligatori per DocsFusion
			schedaDocumento.idPeople = infoUtente.idPeople;
			schedaDocumento.userId = infoUtente.userId;
			if(schedaDocumento.typeId == null)
				schedaDocumento.typeId = "LETTERA";			
			if(schedaDocumento.appId == null)
				schedaDocumento.appId = "ACROBAT";
			return schedaDocumento;
		}

		internal static void classificaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, string codiceFascicolo, ref DocsPaVO.documento.resultProtoEmergenza res, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
		{
			try
			{
				if (codiceFascicolo == null || codiceFascicolo.Equals("") )
					return;
				//ricerca il codice classifica
			
				bool outValue = false;
                string msg = string.Empty;
				DocsPaVO.fascicolazione.Fascicolo fascicolo = Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, codiceFascicolo, schedaDocumento.registro, false, false);
				if (fascicolo != null)  //classifica il doc
                    outValue = Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocumento.systemId, fascicolo.systemID, false, out msg);
				else  //fascicolo non trovato
				{
					res.messaggio = res.messaggio + " - " + "Fascicolo non trovato"; 
					return;
				}
				res.isClassificato = true;
			}
			catch (Exception e) 
			{
				res.messaggio = res.messaggio + " - " + e.Message;
				logger.Debug(e.ToString());
			}
		}

		internal static void annullaProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloEmergenza protoEme, ref DocsPaVO.documento.resultProtoEmergenza res, DocsPaVO.utente.InfoUtente infoUtente)
		{
			try 
			{
				DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato = new DocsPaVO.documento.ProtocolloAnnullato();
				//DocsPaVO.documento.InfoDocumento infoDocumento = Documenti.Protocollo.getInfoDocumento(schedaDocumento);
				protocolloAnnullato.dataAnnullamento = protoEme.dataAnnullamento;
				protocolloAnnullato.autorizzazione = protoEme.noteAnnullamento;	
				res.isAnnullato = ProtoManager.annullaProtocollo(infoUtente, ref schedaDocumento, protocolloAnnullato);
			}
			catch (Exception e) 
			{
				res.messaggio = res.messaggio + " - " + e.Message;
				logger.Debug(e.ToString());
			}

		}

		internal static void trasmettiDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, string nomeModello, ref DocsPaVO.documento.resultProtoEmergenza res, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string idRegistro)
		{
			try 
			{
                if (nomeModello == null || nomeModello.Equals(""))
					return;

                #region OLD
                ////ricerca il template di trasmissione
                //ArrayList listaTemplate = Trasmissioni.TemplateTrasmManager.getTemplateDaNome(infoUtente.idPeople, infoUtente.idCorrGlobali, "D", template);
                //if (listaTemplate != null && listaTemplate.Count > 0)
                //{
                //    //trasmette il doc
                //    DocsPaVO.trasmissione.TemplateTrasmissione templateTrasm = ((DocsPaVO.trasmissione.TemplateTrasmissione) listaTemplate[0]);
					
                //    trasmetti(schedaDocumento, templateTrasm, ref res, infoUtente, ruolo);
                //} 
                //else
                //    res.messaggio = res.messaggio + " - " + "Template non trovato"; 	
                #endregion

                ArrayList listaModelli = Trasmissioni.ModelliTrasmissioni.getModelliByName(infoUtente.idAmministrazione, nomeModello, "D", idRegistro, infoUtente.idPeople, ruolo.systemId);
                if (listaModelli != null && listaModelli.Count == 1)
                {
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)listaModelli[0];
                    trasmetti(schedaDocumento, modello, ref res, infoUtente, ruolo);
                }
                else
                    res.messaggio = res.messaggio + " - " + "Modello non trovato"; 
            }
			catch (Exception e) 
			{
				res.messaggio = res.messaggio + " - " + e.Message;
				logger.Debug(e.ToString());
			}	
		}

		internal static void trasmetti(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, ref DocsPaVO.documento.resultProtoEmergenza res, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
		{
			try 
			{
				//cerca trasmissione dal template
				//DocsPaVO.trasmissione.Trasmissione trasm =Trasmissioni.TemplateTrasmManager.creaTrasmDaTemplate(schedaDocumento, template, infoUtente, ruolo);
                DocsPaVO.trasmissione.Trasmissione trasm = creaTrasmDaModello(schedaDocumento, modello, infoUtente, ruolo);
				if (trasm == null)
				{
					res.messaggio = res.messaggio + " - " + " Trasmissione annullata!";
					return;
				}
                if (infoUtente.delegato != null)
                    trasm.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
                //trasm = Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
                //trasm = Trasmissioni.ExecTrasmManager.executeTrasmMethod(null, trasm);
                trasm = Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(null, trasm);
                res.isTrasmesso = true;
			}
			catch (Exception e) 
			{
				res.messaggio = res.messaggio + " - " + e.Message;
				logger.Debug(e.ToString());
			}	
		}

        private static DocsPaVO.trasmissione.Trasmissione creaTrasmDaModello(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            try
            {                
                DocsPaVO.documento.InfoDocumento infoDocumento = Documenti.DocManager.getInfoDocumento(schedaDocumento);
                trasmissione.infoDocumento = infoDocumento;

                DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasmissione.utente = utente;

                trasmissione.ruolo = ruolo;
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                        
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());

                        DocsPaVO.utente.Corrispondente corr = Utenti.UserManager.getCorrispondenteBySystemID(mittDest.ID_CORR_GLOBALI.ToString());

                        if (corr != null) //corr nullo se non esiste o se è stato disabilitato 
                        {                            
                            // Aggiungo la trasmissione singola
                            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
                            trasmissioneSingola.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                            trasmissioneSingola.corrispondenteInterno = corr;
                            trasmissioneSingola.ragione = ragione;
                            trasmissioneSingola.noteSingole = mittDest.VAR_NOTE_SING;

                            // RUOLO
                            if (corr is DocsPaVO.utente.Ruolo)
                            {
                                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                                ArrayList listaUtenti = queryUtenti(corr, modello.ID_AMM);
                                if (listaUtenti.Count == 0)
                                    trasmissioneSingola = null;

                                for (int ut = 0; ut < listaUtenti.Count; ut++)
                                {
                                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                                    
                                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[ut];
                                    trasmissioneUtente.daNotificare = true;
                                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                                }
                            }

                            // UTENTE
                            if (corr is DocsPaVO.utente.Utente)
                            {
                                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();

                                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                                trasmissioneUtente.daNotificare = true;
                                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                            }

                            if (trasmissioneSingola != null)
                                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
                        }
                    }
                }
            }
            catch
            {
                trasmissione = null;
            }

            return trasmissione;
        }

        private static ArrayList queryUtenti(DocsPaVO.utente.Corrispondente corr, string idAmm)
        {
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();            

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = idAmm;
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.fineValidita = true;

            return Utenti.addressBookManager.getListaCorrispondenti(qco);
        }
	}
}
