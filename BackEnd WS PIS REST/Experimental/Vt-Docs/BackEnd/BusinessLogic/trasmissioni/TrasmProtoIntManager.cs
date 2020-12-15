using System;
using System.Collections;
using log4net;

namespace BusinessLogic.trasmissioni
{
	/// <summary>
	/// Oggetto per la gestione delle trasmissioni dei protocolli interni a tutti
	/// i destinatari (TO/CC)
	/// </summary>
	public class TrasmProtoIntManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(TrasmProtoIntManager));
		/// <summary>
		/// Eccezione che indica che le ragioni trasmissioni dedicate alla protocollazione 
		/// interna non sono state trovate sul DB.
		/// </summary>
		public class RagioniNotFoundException : Exception
		{
			public override string Message
			{
				get	{ return "Ragioni per protocollazione interna non trovate."; }
			}
		}

		/// <summary>
		/// Verifica l'esistenza delle ragioni di trasmissione per il protocollo interno
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione</param>
		/// <returns>bool</returns>
		public bool VerificaRagioni(string idAmm, bool isEnableRef)
		{
			bool result;
			DocsPaDB.Query_DocsPAWS.Trasmissione vtr = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			
			result = vtr.VerificaRagProtInt(idAmm, isEnableRef);

			return result;
		}
		
		public static bool VerificaRagioni(string idAmm, bool isEnableRef, string tipoProto, bool destTo, bool destCC, out string message)
		{
			bool result;
			DocsPaDB.Query_DocsPAWS.Trasmissione vtr = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			
			result = vtr.VerificaRagProtIntBis(idAmm, isEnableRef, tipoProto,destTo,destCC, out message);

			return result;
		}

		/// <summary>
		/// Controlla che un'unita' organizzativa abbia un ruolo di riferimento.
		/// </summary>
		/// <param name="uo"></param>
		/// <returns></returns>
		public bool UOHasReferenceRole(string idUO) 
		{
			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			bool result = trasmissione.UOHasReferenceRole(idUO);
			trasmissione.Dispose();

			return result;
		}

		/// <summary>
		/// Controlla i corrispondenti del protocollo (interno ed in uscita) e li mette nella trasmisione.
		/// </summary>
		/// <returns></returns>
        /// 
        public static bool utenteAutorizzatoSuRregistro(DocsPaVO.utente.Corrispondente c, string idRegistro)
        {

            bool IsAutorized = false;
            //si prendono i ruoli dell'utente destinatario del protocollo
            ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)c).idPeople);
            //si verifica se tali ruoli sono autorizzati sul registro corrente
            if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
            {
                foreach (object objRuolo in listaRuoliUtente)
                {
                    if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, idRegistro))
                    {
                        IsAutorized = true;
                        break;

                    }
                }

            }

            return IsAutorized;
        
        }




        public static DocsPaVO.trasmissione.Trasmissione SetCorrispondentiTrasmissioneAutomatica(DocsPaVO.documento.SchedaDocumento schedaDocArrivo, string idRegistro , DocsPaVO.documento.SchedaDocumento schedaDocUscita, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) 
		{
			DocsPaVO.trasmissione.Trasmissione trasmissione = null;
			DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestTO = null;
			DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestCC = null;
			try
            {
                trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                trasmissione.ruolo=ruolo;
                trasmissione.utente=BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasmissione.utente.dst = infoUtente.dst;
                trasmissione.infoFascicolo = null;
                trasmissione.utente.idAmministrazione = infoUtente.idAmministrazione;
                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDocArrivo);
                
                //creo l'oggetto qca in caso di trasmissioni a UO
				DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
				qco.fineValidita = true;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
								
				qca.ruolo = ruolo;
				qca.queryCorrispondente = qco;
                qca.idRegistro = idRegistro;

				// Acquisisci le ragioni per la trasmissione del protocollo in arrivo ai vari destinatari	
				ragTrasmDestTO = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO",ruolo.idAmministrazione);
				ragTrasmDestCC = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("CC",ruolo.idAmministrazione);
				
			    #region protocollo in uscita
						
                DocsPaVO.documento.ProtocolloUscita protoUscTO = (DocsPaVO.documento.ProtocolloUscita)schedaDocUscita.protocollo;
                if (protoUscTO != null && protoUscTO.destinatari != null)
                {
                    foreach (object destinatario in protoUscTO.destinatari)
                    {
                        trasmissione.noteGenerali = "";
                        qca.ragione = ragTrasmDestTO;
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;


                        if (corr.tipoIE == "I")
                        {
                            // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                            if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                //prendiamo i ruoli di riferimento autorizzati
                                ArrayList listaRuoliRiferimento = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                string id_reg_proto = ((DocsPaVO.utente.Registro)ruolo.registri[0]).codRegistro;

                                if (listaRuoliRiferimento != null && listaRuoliRiferimento.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoliRiferimento)
                                    {
                                        /*
                                         * modificato sabrina per eliminare il controllo che non consentiva di inviare trasmissioni ai ruoli autorizzati
                                         * il controllo sul codice del corrispondente che deve coincidere con il codice del registro è stato messo forse per qualche cliente particolare
                                         * ma non và bene!!!
                                         */
                                        //if (((DocsPaVO.utente.Ruolo)objRuolo).codiceCorrispondente.StartsWith(id_reg_proto))
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }

                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                            {
                                if (verificaRuoloAutorizzato(corr.systemId, idRegistro))
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }

                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                            {
                                bool IsAutorized = false;
                                //si prendono i ruoli dell'utente destinatario del protocollo
                                ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                //si verifica se tali ruoli sono autorizzati sul registro corrente
                                if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoliUtente)
                                    {
                                        if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, idRegistro))
                                        {
                                            IsAutorized = true;
                                            break;

                                        }
                                    }

                                }
                                //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                //si effettua la trasmissione.
                                if (IsAutorized)
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }
                            }

                        }

                    }
                }
                #endregion

                #region Creazione trasmissioni singole ai destinatari CC

                #region protocollo in uscita
                DocsPaVO.documento.ProtocolloUscita protoUscCC = (DocsPaVO.documento.ProtocolloUscita)schedaDocUscita.protocollo;

                if (protoUscCC != null && protoUscCC.destinatariConoscenza!=null)
                {
                    foreach (object destinatario in protoUscCC.destinatariConoscenza)
                    {
                        trasmissione.noteGenerali = "";
                        qca.ragione = ragTrasmDestCC;
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;

                        if (corr.tipoIE == "I")
                        {
                            if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                string id_reg_proto = ((DocsPaVO.utente.Registro)ruolo.registri[0]).codRegistro;


                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        /*
                                      * modificato sabrina per eliminare il controllo che non consentiva di inviare trasmissioni ai ruoli autorizzati
                                      * il controllo sul codice del corrispondente che deve coincidere con il codice del registro è stato messo forse per qualche cliente particolare
                                      * ma non và bene!!!
                                      */
                                      //  if (((DocsPaVO.utente.Ruolo)objRuolo).codiceCorrispondente.StartsWith(id_reg_proto))
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }
                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                            {
                                if (verificaRuoloAutorizzato(corr.systemId, idRegistro))
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }

                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                            {
                                bool IsAutorized = false;
                                //si prendono i ruoli dell'utente destinatario del protocollo
                                ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                //si verifica se tali ruoli sono autorizzati sul registro corrente
                                if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoliUtente)
                                    {
                                        if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, idRegistro))
                                        {
                                            IsAutorized = true;
                                            break;

                                        }
                                    }

                                }
                                //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                //si effettua la trasmissione.
                                if (IsAutorized)
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }
                            }
                        }

                    }
                }
						#endregion
					
				#endregion
			}
			catch(Exception)
			{
				trasmissione = null;
			}	

			return trasmissione;
		}


        public DocsPaVO.trasmissione.Trasmissione SetCorrispondentiTrasmissione(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo,DocsPaVO.utente.InfoUtente infoutente)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestTO = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestCC = null;
            try
            {
                // Inizializza trasmissione
                trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                trasmissione.ruolo = ruolo;
                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDoc);
                trasmissione.infoFascicolo = null;
                DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                utente.idPeople = schedaDoc.idPeople;
                DocsPaDB.Query_DocsPAWS.Utenti utenteMittente = new DocsPaDB.Query_DocsPAWS.Utenti();
                utente.email = utenteMittente.GetEmailUtente(utente.idPeople);
                utente.idAmministrazione = infoutente.idAmministrazione;
                utente.dst = infoutente.dst;
                utente.urlWA = infoutente.urlWA;
                trasmissione.utente = utente;
               
                //creo l'oggetto qca in caso di trasmissioni a UO
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.fineValidita = true;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = SetQCA(qco, schedaDoc);

                qca.ruolo = ruolo;
                qca.queryCorrispondente = qco;

                // Acquisisci le ragioni per la trasmissione del protocollo
                if (!schedaDoc.tipoProto.Equals("A"))
                {
                    ragTrasmDestTO = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO", ruolo.idAmministrazione);
                    ragTrasmDestCC = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("CC", ruolo.idAmministrazione);
                }

                #region Creazione trasmissioni singole ai destinatari TO e dove fosse specificato all'ufficio referente
                switch (schedaDoc.tipoProto)
                {
                    case "I":
                        #region protocollo interno
                        DocsPaVO.documento.ProtocolloInterno protoIntTO = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;

                        foreach (object destinatario in protoIntTO.destinatari)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestTO;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;

                            if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                        ArrayList listaRuoli = new ArrayList();
                                        listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoli != null && listaRuoli.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoli)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                    }
                                }
                            }
                        }

                        //trasmissione ai ruoli di riferimento della UO REFERENTE
                        if (protoIntTO.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoIntTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoIntTO.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }

                        }
                        
                        #endregion
                        break;
                    case "P":
                        #region protocollo in uscita
                        DocsPaVO.documento.ProtocolloUscita protoUscTO = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;

                        foreach (object destinatario in protoUscTO.destinatari)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestTO;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;

                            if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente == null || corr.systemId != ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        //prendiamo i ruoli di riferimento autorizzati
                                        ArrayList listaRuoliRiferimento = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoliRiferimento != null && listaRuoliRiferimento.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoliRiferimento)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                            }
                                        }
                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                                    {
                                        if (verificaRuoloAutorizzato(corr.systemId, schedaDoc.registro.systemId))
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                        }
                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                                    {
                                        bool IsAutorized = false;
                                        //si prendono i ruoli dell'utente destinatario del protocollo
                                        ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                        //si verifica se tali ruoli sono autorizzati sul registro corrente
                                        if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoliUtente)
                                            {
                                                if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, schedaDoc.registro.systemId))
                                                {
                                                    IsAutorized = true;
                                                    break;
                                                }
                                            }
                                        }
                                        //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                        //si effettua la trasmissione.
                                        if (IsAutorized)
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                    }
                                }
                                
                                if (corr.tipoCorrispondente == "F")
                                {
                                    ArrayList listaRuoliRF = BusinessLogic.Rubrica.RF.getCorrispondentiByDescRF(corr.descrizione);
                                    if (listaRuoliRF != null && listaRuoliRF.Count > 0)
                                    {
                                        foreach (object objRuolo in listaRuoliRF)
                                            trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }
                                if (corr.tipoCorrispondente == "L")
                                {
                                    ArrayList listaOggettiLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByDescLista(corr.descrizione);
                                    if (listaOggettiLista != null && listaOggettiLista.Count > 0)
                                    {
                                        foreach (DocsPaVO.utente.Corrispondente objRuolo in listaOggettiLista)
                                        {
                                            if (objRuolo.tipoIE == "I")
                                            {
                                                //trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Corrispondente)objRuolo, qca.ragione);
                                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                                if (objRuolo.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                                {
                                                    //prendiamo i ruoli di riferimento autorizzati
                                                    ArrayList listaRuoliRiferimento = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)objRuolo);
                                                    if (listaRuoliRiferimento != null && listaRuoliRiferimento.Count > 0)
                                                    {
                                                        foreach (object obj in listaRuoliRiferimento)
                                                            trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)obj, qca.ragione);
                                                    }
                                                }
                                                else if (objRuolo.GetType() == typeof(DocsPaVO.utente.Ruolo))
                                                {
                                                    if (verificaRuoloAutorizzato(objRuolo.systemId, schedaDoc.registro.systemId))
                                                        trasmissione = AddTrasmissioneSingola(trasmissione, objRuolo, qca.ragione);
                                                }
                                                else if (objRuolo.GetType() == typeof(DocsPaVO.utente.Utente))
                                                {
                                                    bool IsAutorized = false;
                                                    //si prendono i ruoli dell'utente destinatario del protocollo
                                                    ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)objRuolo).idPeople);
                                                    //si verifica se tali ruoli sono autorizzati sul registro corrente
                                                    if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                                    {
                                                        foreach (object obj in listaRuoliUtente)
                                                        {
                                                            if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)obj).systemId, schedaDoc.registro.systemId))
                                                            {
                                                                IsAutorized = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                                    //si effettua la trasmissione.
                                                    if (IsAutorized)
                                                        trasmissione = AddTrasmissioneSingola(trasmissione, objRuolo, qca.ragione);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //trasmissione ai ruoli di riferimento della UO REFERENTE
                        if (protoUscTO.ufficioReferente != null)
                        {
                            if (protoUscTO.ufficioReferente != null)
                            {
                                DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                                trasmissione.noteGenerali = "";
                                qca.ragione = ragTrasmRef;

                                if (protoUscTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                {
                                    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                    ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoUscTO.ufficioReferente);

                                    if (listaRuoli != null && listaRuoli.Count > 0)
                                    {
                                        foreach (object objRuolo in listaRuoli)
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                            }
                        }

                        #endregion
                        break;
                    case "A":
                        #region protocollo ENTRATA (SOLO PER UFFICIO REFERENTE)

                        DocsPaVO.documento.ProtocolloEntrata protoEntRef = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
                        trasmissione.noteGenerali = "";

                        //trasmissione ai ruoli di riferimento della UO REFERENTE
                        if (protoEntRef.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoEntRef.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoEntRef.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }

                            }

                        }
                        #endregion
                        break;
                }
                #endregion

                #region Creazione trasmissioni singole ai destinatari CC
                switch (schedaDoc.tipoProto)
                {
                    case "I":
                        #region protocollo interno
                        DocsPaVO.documento.ProtocolloInterno protoIntCC = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;

                        foreach (object destinatario in protoIntCC.destinatariConoscenza)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestCC;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;
                            //se mitt coincide con il dest a quel dest non trasmetto nulla
                            if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                        ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoli != null && listaRuoli.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoli)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                            }
                                        }
                                        //										else
                                        //										{
                                        //											throw new Exception();
                                        //										}
                                    }
                                    else
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                    }
                                }
                            }
                        }
                        #endregion
                        break;
                    case "P":
                        #region protocollo in uscita
                        DocsPaVO.documento.ProtocolloUscita protoUscCC = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;

                        foreach (object destinatario in protoUscCC.destinatariConoscenza)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestCC;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;
                            if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente == null || corr.systemId != ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                        ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoli != null && listaRuoli.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoli)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                            }
                                        }
                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                                    {
                                        if (verificaRuoloAutorizzato(corr.systemId, schedaDoc.registro.systemId))
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                        }

                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                                    {
                                        bool IsAutorized = false;
                                        //si prendono i ruoli dell'utente destinatario del protocollo
                                        ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                        //si verifica se tali ruoli sono autorizzati sul registro corrente
                                        if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoliUtente)
                                            {
                                                if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, schedaDoc.registro.systemId))
                                                {
                                                    IsAutorized = true;
                                                    break;

                                                }
                                            }

                                        }
                                        //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                        //si effettua la trasmissione.
                                        if (IsAutorized)
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        break;
                }
                #endregion
            }
            catch (Exception)
            {
                trasmissione = null;
            }

            return trasmissione;
        }

        public DocsPaVO.trasmissione.Trasmissione SetCorrispondentiTrasmissione(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoutente, out string message)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestTO = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestCC = null;
            message = string.Empty;
            ArrayList trasm_strutture_vuote = new ArrayList();
            try
            {
                // Inizializza trasmissione
                trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                trasmissione.ruolo = ruolo;
                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDoc);
                trasmissione.infoFascicolo = null;
                DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                utente.idPeople = schedaDoc.idPeople;
                DocsPaDB.Query_DocsPAWS.Utenti utenteMittente = new DocsPaDB.Query_DocsPAWS.Utenti();
                utente.email = utenteMittente.GetEmailUtente(utente.idPeople);
                utente.idAmministrazione = infoutente.idAmministrazione;
                utente.dst = infoutente.dst;
                utente.urlWA = infoutente.urlWA;
                trasmissione.utente = utente;

                //creo l'oggetto qca in caso di trasmissioni a UO
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.fineValidita = true;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = SetQCA(qco, schedaDoc);

                qca.ruolo = ruolo;
                qca.queryCorrispondente = qco;

                // Acquisisci le ragioni per la trasmissione del protocollo
                if (!schedaDoc.tipoProto.Equals("A"))
                {
                    ragTrasmDestTO = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO", ruolo.idAmministrazione);
                    ragTrasmDestCC = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("CC", ruolo.idAmministrazione);
                }

                #region Creazione trasmissioni singole ai destinatari TO e dove fosse specificato all'ufficio referente
                switch (schedaDoc.tipoProto)
                {
                    case "I":
                        #region protocollo interno
                        DocsPaVO.documento.ProtocolloInterno protoIntTO = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;

                        foreach (object destinatario in protoIntTO.destinatari)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestTO;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;

                            if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                        ArrayList listaRuoli = new ArrayList();
                                        listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoli != null && listaRuoli.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoli)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                            }
                                        }
                                        else
                                        {
                                            // se non ci sono ruoli autorizzati potrebbe essere che il corrispondente sia 
                                            // un corrispondente esterno. Per verificare questo, facciamo una nuova ricerca
                                            ArrayList listaRuoliRifEsterni = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoEsterni(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                            if (listaRuoliRifEsterni != null && listaRuoliRifEsterni.Count > 0)
                                            {
                                                //   trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", corr.descrizione, corr.codiceRubrica, "UO esterna al registro"));
                                            }
                                            else
                                                trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", corr.descrizione, corr.codiceRubrica, "UO priva di ruoli di riferimento"));
                                        }
                                    }
                                    else
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione, trasm_strutture_vuote);
                                    }
                                }
                            }
                            else
                                trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", corr.descrizione, corr.codiceRubrica, "Destinatario e mittente della trasmissione coincidono"));
                        }

                        //trasmissione ai ruoli di riferimento della UO REFERENTE
                     /*   if (protoIntTO.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoIntTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoIntTO.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                    }
                                }
                                else
                                {
                                    //throw new Exception();
                                    trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", protoIntTO.ufficioReferente.descrizione, protoIntTO.ufficioReferente.codiceRubrica, "Destinatario privo di ruoli di riferimento"));
                                }
                            }
                        }*/

                        if (trasm_strutture_vuote.Count > 0)
                        {
                            if (message == "")
                            {
                                foreach (string s in trasm_strutture_vuote)
                                    message += (" - " + s + "\\n");

                                //message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati o perchè mittente della trasmissione:\\n{0}\");", message);
                                message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture:\\n{0}\");", message);
                            }
                        }

                        #endregion
                        break;
                    case "P":
                        #region protocollo in uscita
                        DocsPaVO.documento.ProtocolloUscita protoUscTO = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;

                        foreach (object destinatario in protoUscTO.destinatari)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestTO;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;

                            if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente == null || corr.systemId != ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        logger.Debug("getRuoliRiferimentoAutorizzati inizio");
                                        //prendiamo i ruoli di riferimento autorizzati
                                        ArrayList listaRuoliRiferimento = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                        logger.Debug("getRuoliRiferimentoAutorizzati fine");
                                        if (listaRuoliRiferimento != null && listaRuoliRiferimento.Count > 0)
                                        {
                                            logger.Debug("listaRuoliRiferimento contiene n=" + listaRuoliRiferimento.Count);
                                            foreach (object objRuolo in listaRuoliRiferimento)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                            }
                                        }
                                        else
                                        {
                                            // se non ci sono ruoli autorizzati potrebbe essere che il corrispondente sia 
                                            // un corrispondente esterno. Per verificare questo, facciamo una nuova ricerca
                                            logger.Debug("getRuoliRiferimentoEsterni inizio");
                                            ArrayList listaRuoliRifEsterni = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoEsterni(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                            logger.Debug("getRuoliRiferimentoEsterni fine");
                                            if (listaRuoliRifEsterni != null && listaRuoliRifEsterni.Count > 0)
                                            {
                                                //   trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", corr.descrizione, corr.codiceRubrica, "UO esterna al registro"));
                                            }
                                            else
                                                trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", corr.descrizione, corr.codiceRubrica, "UO priva di ruoli di riferimento"));
                                        }
                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                                    {
                                        if (verificaRuoloAutorizzato(corr.systemId, schedaDoc.registro.systemId))
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione, trasm_strutture_vuote);
                                        }
                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                                    {
                                        bool IsAutorized = false;
                                        //si prendono i ruoli dell'utente destinatario del protocollo
                                        ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                        //si verifica se tali ruoli sono autorizzati sul registro corrente
                                        if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoliUtente)
                                            {
                                                if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, schedaDoc.registro.systemId))
                                                {
                                                    IsAutorized = true;
                                                    break;
                                                }
                                            }
                                        }
                                        //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                        //si effettua la trasmissione.
                                        if (IsAutorized)
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione, trasm_strutture_vuote);
                                        }
                                    }
                                }
                            }
                            else
                                trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", corr.descrizione, corr.codiceRubrica, "Destinatario e mittente della trasmissione coincidono")); 
                        }
                        //trasmissione ai ruoli di riferimento della UO REFERENTE
                     /*   if (protoUscTO.ufficioReferente != null)
                        {
                            if (protoUscTO.ufficioReferente != null)
                            {
                                DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                                trasmissione.noteGenerali = "";
                                qca.ragione = ragTrasmRef;

                                if (protoUscTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                {
                                    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                    ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoUscTO.ufficioReferente);

                                    if (listaRuoli != null && listaRuoli.Count > 0)
                                    {
                                        foreach (object objRuolo in listaRuoli)
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                        }
                                    }
                                    else
                                        trasm_strutture_vuote.Add(String.Format("{0} ({1}): {2}", protoUscTO.ufficioReferente.descrizione, protoUscTO.ufficioReferente.codiceRubrica, "Destinatario privo di ruoli di riferimento"));
                                }
                            }
                        }*/

                        if (trasm_strutture_vuote.Count > 0)
                        {
                            if (message == "")
                            {
                                foreach (string s in trasm_strutture_vuote)
                                    message += (" - " + s + "\\n");

                                //message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati o perchè mittente della trasmissione:\\n{0}\");", message);
                                message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture:\\n{0}\");", message);
                            }
                        }
                        #endregion
                        break;
                    case "A":
                        #region protocollo ENTRATA (SOLO PER UFFICIO REFERENTE)

                 /*       DocsPaVO.documento.ProtocolloEntrata protoEntRef = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
                        trasmissione.noteGenerali = "";

                        //trasmissione ai ruoli di riferimento della UO REFERENTE
                        if (protoEntRef.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoEntRef.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoEntRef.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                    }
                                    }
                                }

                            }
                        if (trasm_strutture_vuote.Count > 0)
                        {
                            if (message == "")
                            {
                                foreach (string s in trasm_strutture_vuote)
                                    message += (" - " + s + "\\n");

                                message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati o perchè mittente della trasmissione:\\n{0}\");", message);
                            }
                        }*/
                        #endregion
                        break;
                }
                #endregion

                #region Creazione trasmissioni singole ai destinatari CC
                switch (schedaDoc.tipoProto)
                {
                    case "I":
                        #region protocollo interno
                        DocsPaVO.documento.ProtocolloInterno protoIntCC = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;

                        foreach (object destinatario in protoIntCC.destinatariConoscenza)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestCC;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;
                            //se mitt coincide con il dest a quel dest non trasmetto nulla
                            if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                        ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoli != null && listaRuoli.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoli)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                            }
                                        }
                                        else
                                            trasm_strutture_vuote.Add(String.Format("{0} ({1})", corr.descrizione, corr.codiceRubrica));
                                        //										else
                                        //										{
                                        //											throw new Exception();
                                        //										}
                                    }
                                    else
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione, trasm_strutture_vuote);
                                    }
                                }
                            }
                        }
                        if (trasm_strutture_vuote.Count > 0)
                        {
                            if (message == "")
                            {
                                foreach (string s in trasm_strutture_vuote)
                                    message += (" - " + s + "\\n");

                                message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione in CC a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati o perchè mittente della trasmissione:\\n{0}\");", message);
                            }
                        }
                        #endregion
                        break;
                    case "P":
                        #region protocollo in uscita
                        DocsPaVO.documento.ProtocolloUscita protoUscCC = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;

                        foreach (object destinatario in protoUscCC.destinatariConoscenza)
                        {
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmDestCC;
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;
                            if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente == null || corr.systemId != ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.systemId)
                            {
                                if (corr.tipoIE == "I")
                                {
                                    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                    {
                                        // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                        ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                        if (listaRuoli != null && listaRuoli.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoli)
                                            {
                                                trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                            }
                                        }
                                        else
                                            trasm_strutture_vuote.Add(String.Format("{0} ({1})", corr.descrizione, corr.codiceRubrica));
                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                                    {
                                        if (verificaRuoloAutorizzato(corr.systemId, schedaDoc.registro.systemId))
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione, trasm_strutture_vuote);
                                        }

                                    }
                                    else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                                    {
                                        bool IsAutorized = false;
                                        //si prendono i ruoli dell'utente destinatario del protocollo
                                        ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                        //si verifica se tali ruoli sono autorizzati sul registro corrente
                                        if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                        {
                                            foreach (object objRuolo in listaRuoliUtente)
                                            {
                                                if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, schedaDoc.registro.systemId))
                                                {
                                                    IsAutorized = true;
                                                    break;

                                                }
                                            }

                                        }
                                        //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                        //si effettua la trasmissione.
                                        if (IsAutorized)
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione, trasm_strutture_vuote);
                                        }
                                    }
                                }
                            }
                        }
                        if (trasm_strutture_vuote.Count > 0)
                        {
                            if (message == "")
                            {
                                foreach (string s in trasm_strutture_vuote)
                                    message += (" - " + s + "\\n");

                                message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione in CC a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati o perchè mittente della trasmissione:\\n{0}\");", message);
                            }
                        }
                        #endregion
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.Debug("errore in SetCorrispondentiTrasmissione: " + ex.Message + " stack trace: " + ex.StackTrace);
                trasmissione = null;
            }

            return trasmissione;
        } 

		/// <summary>
		/// Controlla i corrispondenti del protocollo (interno ed in uscita) e li mette nella trasmisione.
		/// </summary>
		/// <returns></returns>
		public DocsPaVO.trasmissione.Trasmissione SetCorrModificatiTrasmissione(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo) 
		{
			DocsPaVO.trasmissione.Trasmissione trasmissione = null;
			DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestTO = null;
			DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestCC = null;
			try
			{
				// Inizializza trasmissione
				trasmissione = new DocsPaVO.trasmissione.Trasmissione();
				trasmissione.ruolo = ruolo;
				trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDoc);
				trasmissione.infoFascicolo = null;
				DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
				utente.idPeople = schedaDoc.idPeople;
				trasmissione.utente = utente;				

				// creo l'oggetto qca in caso di trasmissioni a UO
				DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
				qco.fineValidita = true;
				DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = SetQCA(qco, schedaDoc);
								
				qca.ruolo = ruolo;
				qca.queryCorrispondente = qco;	

				// Acquisisci le ragioni per la trasmissione del protocollo
				if(!schedaDoc.tipoProto.Equals("A"))
				{
					ragTrasmDestTO = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO",ruolo.idAmministrazione);
					ragTrasmDestCC = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("CC",ruolo.idAmministrazione);
				}

				#region Creazione trasmissioni singole ai destinatari TO e dove fosse specificato all'ufficio referente
				switch (schedaDoc.tipoProto)
				{
					case "I":
						#region protocollo interno
						DocsPaVO.documento.ProtocolloInterno protoIntTO = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
				
						for(int i = 0;i<schedaDoc.destinatariModificati.Count;i++)
						{
                            if (schedaDoc.destinatariModificati[i] != null)
                            {
							    trasmissione.noteGenerali = "";
							    qca.ragione = ragTrasmDestTO;
							    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(schedaDoc.destinatariModificati[i].ToString());
					
							    if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
							    {
								    if (corr.tipoIE == "I")
								    {
									    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
									    {
										    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
										    ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
						
										    if(listaRuoli != null && listaRuoli.Count > 0)
										    {
											    foreach(object objRuolo in listaRuoli)
											    {
												    trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
											    }
										    }
										    else
										    {
											    throw new Exception();
										    }
									    }
									    else
									    {
										    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
									    }
								    }
							    }
                            }
						}
						
						//trasmissione ai ruoli di riferimento della UO REFERENTE
						if (protoIntTO.ufficioReferente != null)
						{
							DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE",ruolo.idAmministrazione);
							trasmissione.noteGenerali = "";
							qca.ragione = ragTrasmRef;

							if (protoIntTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
							{
								// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
								ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoIntTO.ufficioReferente);
						
								if(listaRuoli != null && listaRuoli.Count > 0)
								{
									foreach(object objRuolo in listaRuoli)
									{
										trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
									}
								}
								else
								{
									throw new Exception();
								}
							}
						
						}
						#endregion
						break;
					case "P":
						#region protocollo in uscita
						DocsPaVO.documento.ProtocolloUscita protoUscTO = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;
				
						for(int i = 0;i<schedaDoc.destinatariModificati.Count;i++)
						{
                            if (schedaDoc.destinatariModificati[i] != null)
                            {
							    trasmissione.noteGenerali = "";
							    qca.ragione = ragTrasmDestTO;
							    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(schedaDoc.destinatariModificati[i].ToString());
					
							    if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente == null || corr.systemId != ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.systemId)
							    {
								    if (corr.tipoIE == "I")
								    {
									    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
									    {
										    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
										    ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
						
										    if(listaRuoli != null && listaRuoli.Count > 0)
										    {
											    foreach(object objRuolo in listaRuoli)
											    {
												    trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
											    }
										    }
									    }
									    else
									    {
										    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
									    }
								    }
							    }
                            }
						}
						//trasmissione ai ruoli di riferimento della UO REFERENTE
						if (protoUscTO.ufficioReferente != null)
						{
							if (protoUscTO.ufficioReferente != null)
							{
								DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE",ruolo.idAmministrazione);
								trasmissione.noteGenerali = "";
								qca.ragione = ragTrasmRef;

								if (protoUscTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
								{
									// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
									ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoUscTO.ufficioReferente);
						
									if(listaRuoli != null && listaRuoli.Count > 0)
									{
										foreach(object objRuolo in listaRuoli)
										{
											trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
										}
									}
									else
									{
										throw new Exception();
									}
								}
							}
						}

						#endregion
						break;
					case "A":
						#region protocollo ENTRATA (SOLO PER UFFICIO REFERENTE)
						
						DocsPaVO.documento.ProtocolloEntrata protoEntRef = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
						trasmissione.noteGenerali = "";
					
						//trasmissione ai ruoli di riferimento della UO REFERENTE
						if (protoEntRef.ufficioReferente != null)
						{
							DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE",ruolo.idAmministrazione);
							trasmissione.noteGenerali = "";
							qca.ragione = ragTrasmRef;

							if (protoEntRef.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
							{
								// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
								ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoEntRef.ufficioReferente);
						
								if(listaRuoli != null && listaRuoli.Count > 0)
								{
									foreach(object objRuolo in listaRuoli)
									{
										trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
									}
								}

							}
						
						}
						#endregion
						break;
				}
				#endregion

				#region Creazione trasmissioni singole ai destinatari CC
				switch (schedaDoc.tipoProto)
				{
					case "I":
						#region protocollo interno
						DocsPaVO.documento.ProtocolloInterno protoIntCC = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
				
						for(int i = 0;i<schedaDoc.destinatariCCModificati.Count;i++)
						{
                            if (schedaDoc.destinatariCCModificati[i] != null)
                            {
                                trasmissione.noteGenerali = "";
                                qca.ragione = ragTrasmDestCC;
                                //DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(schedaDoc.destinatariModificati[i].ToString());
                                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(schedaDoc.destinatariCCModificati[i].ToString());
                                //se mitt coincide con il dest a quel dest non trasmetto nulla
                                if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
                                {
                                    if (corr.tipoIE == "I")
                                    {
                                        if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                                        {
                                            // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                            ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);

                                            if (listaRuoli != null && listaRuoli.Count > 0)
                                            {
                                                foreach (object objRuolo in listaRuoli)
                                                {
                                                    trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception();
                                            }
                                        }
                                        else
                                        {
                                            trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                        }
                                    }
                                }
                            }
						}
						#endregion
						break;
					case "P":
						#region protocollo in uscita
						DocsPaVO.documento.ProtocolloUscita protoUscCC = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;
				
						for(int j = 0;j<schedaDoc.destinatariCCModificati.Count;j++)
						{
                            if (schedaDoc.destinatariCCModificati[j] != null)
                            {
							    trasmissione.noteGenerali = "";
							    qca.ragione = ragTrasmDestCC;
							    //DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(schedaDoc.destinatariModificati[j].ToString());
                                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(schedaDoc.destinatariCCModificati[j].ToString());
							    if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente == null || corr.systemId != ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.systemId)
							    {
								    if (corr.tipoIE == "I")
								    {
									    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
									    {
										    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
										    ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
						
										    if(listaRuoli != null && listaRuoli.Count > 0)
										    {
											    foreach(object objRuolo in listaRuoli)
											    {
												    trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
											    }
										    }
										    else
										    {
											    throw new Exception();
										    }
									    }
									    else
									    {
										    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
									    }
								    }
							    }
                            }
						}
						#endregion
						break;
				}
				#endregion
			}
			catch
			{
				trasmissione = null;
			}	

			return trasmissione;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codiceRubrica"></param>
		/// <returns></returns>
		private static ArrayList QueryUtenti(string codiceRubrica, string idAmministrazione) 
		{
			
			// costruzione oggetto queryCorrispondente
			DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

			qco.codiceRubrica = codiceRubrica;
			qco.getChildren = true;
			qco.fineValidita = true;
			
			qco.idAmministrazione = idAmministrazione;
			
			// corrispondenti interni
			qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;			
			
			ArrayList listaCorrispondenti = BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco);
			
			return listaCorrispondenti;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trasmissione"></param>
		/// <param name="corr"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.Trasmissione AddTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm) 
		{			
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				foreach(object objTrasm in trasmissione.trasmissioniSingole) 
				{
					DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm;
					
					if (ts!=null && ts.corrispondenteInterno.systemId == corr.systemId) 
					{
						if(ts.daEliminare) ts.daEliminare = false;					
						return trasmissione;
					}
				}			
			}

			// Aggiungo la trasmissione singola
			DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = ragTrasm;
			
			// Aggiungo la lista di trasmissioniUtente
			if(corr.GetType() == typeof(DocsPaVO.utente.Ruolo)) 
			{
				// Gestione Ruoli/UO
				trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
				if (corr.idAmministrazione == null) corr.idAmministrazione = ((DocsPaVO.utente.Corrispondente)(((DocsPaVO.utente.Ruolo)(corr)).uo)).idAmministrazione;
				ArrayList listaUtenti = QueryUtenti(corr.codiceRubrica, corr.idAmministrazione);
				
				//if (listaUtenti == null || listaUtenti.Count == 0) return trasmissione;
                if (listaUtenti.Count == 0)
                    trasmissioneSingola = null;
				
				// Ciclo per utenti se dest è gruppo o ruolo
				foreach(object objUtente in listaUtenti) 
				{
					DocsPaVO.utente.Corrispondente utente = (DocsPaVO.utente.Corrispondente)objUtente;

					DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPaVO.utente.Utente) utente;
					if(ragTrasm.descrizione == "RISPOSTA") trasmissioneUtente.idTrasmRispSing = trasmissioneSingola.systemId;
					trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
				}
			}
			else 
			{
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
			}

			trasmissione.trasmissioniSingole.Add(trasmissioneSingola);

			return trasmissione;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="corr"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione AddTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm, ArrayList trasm_strutture_vuote)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                foreach (object objTrasm in trasmissione.trasmissioniSingole)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm;

                    if (ts.corrispondenteInterno.systemId == corr.systemId)
                    {
                        if (ts.daEliminare) ts.daEliminare = false;
                        return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = "S";
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragTrasm;
            if(ragTrasm!=null && 
                !string.IsNullOrEmpty(ragTrasm.systemId))
               logger.Debug("ragTrasm.SYSTEM_ID= "+ragTrasm.systemId);
            else
                logger.Debug("Attenzione paramentro ragTrasm in  AddTrasmissioneSingola è NULL !!!");

            // Aggiungo la lista di trasmissioniUtente
            if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
            {
                // Gestione Ruoli/UO
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                 //il problema è qui :)
                //if (corr.idAmministrazione == null) corr.idAmministrazione = ((DocsPaVO.utente.Corrispondente)(((DocsPaVO.utente.Ruolo)(corr)).uo)).idAmministrazione;
                
               
                if (corr != null 
                    && !string.IsNullOrEmpty(corr.systemId) 
                    && corr.idAmministrazione == null)
                {
                    logger.Debug("CORR.IDamministrazione=null");
                    string keyQuery = "SELECT ID_AMM AS ID FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID="+ragTrasm.systemId;
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(keyQuery))
                        {
                            if (reader.Read())
                            {
                                corr.idAmministrazione = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                                logger.Debug("CORR.IDamministrazione= " + corr.idAmministrazione);
                            }
                        }
                    }
				

                }
                
                ArrayList listaUtenti = QueryUtenti(corr.codiceRubrica, corr.idAmministrazione);
                logger.Debug("QueryUtenti effettuata, trovati n.Ut= " + listaUtenti.Count);

                //if (listaUtenti == null || listaUtenti.Count == 0) return trasmissione;
                if (listaUtenti.Count == 0)
                    trasmissioneSingola = null;

                // Ciclo per utenti se dest è gruppo o ruolo
                foreach (object objUtente in listaUtenti)
                {
                    DocsPaVO.utente.Corrispondente utente = (DocsPaVO.utente.Corrispondente)objUtente;

                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)utente;
                    if (ragTrasm.descrizione == "RISPOSTA") trasmissioneUtente.idTrasmRispSing = trasmissioneSingola.systemId;
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }
            else
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
            else
            {
                // In questo caso questa trasmissione non può avvenire perché la
                // struttura non ha utenti (TICKET #1608)
                trasm_strutture_vuote.Add(String.Format("{0} ({1})", corr.descrizione, corr.codiceRubrica));
            }
            logger.Debug("FINE ADDTRASMSINGOLA");
            return trasmissione;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="qco"></param>
		/// <returns></returns>
		private static DocsPaVO.addressbook.QueryCorrispondenteAutorizzato SetQCA(DocsPaVO.addressbook.QueryCorrispondente qco, DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qcAut = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
			qcAut.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
			
			if(schedaDoc					!= null &&
				schedaDoc.protocollo		    != null &&
				schedaDoc.protocollo.numero  != null &&
				schedaDoc.protocollo.numero  != "")
			{
				qcAut.idRegistro = schedaDoc.registro.systemId; 
			}

			return qcAut;
		}

		public static bool verificaRuoloAutorizzato(string idcorr,string registro)
		{
			bool result=false;
			try
			{
				
				result = BusinessLogic.Utenti.addressBookManager.VerificaAutorizzazioneRuolo(idcorr,registro);
			}
			catch
			{

			}
			return result;
		}
        /// <summary>
        /// Trasmette il protocollo creato automaticamente su un registro automatico ai destinatari del protocollo
        /// in partenza
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="ruolo"></param>
        /// <param name="serverName"></param>
        /// <param name="isEnableRef"></param>
        /// <param name="RagioniVerificate"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TrasmissioneProtocolloAutomatico(DocsPaVO.documento.SchedaDocumento schedaDocArrivo, string idRegistro,DocsPaVO.documento.SchedaDocumento schedaDocPartenza, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, string serverName, bool isEnableRef, out bool RagioniVerificate, out string message)
        {
            bool result = true;
            message = "";
            RagioniVerificate = true;
            bool destTo = false;
            bool destCC = false;
            message = "";
            try
            {

                if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatari != null && ((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatari.Count > 0)
                {
                    destTo = true;
                    if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatariConoscenza != null && ((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatariConoscenza.Count > 0)
                    {
                        destCC = true;
                    }
                }
                
                if (!VerificaRagioni(ruolo.idAmministrazione, false, schedaDocPartenza.tipoProto, destTo, destCC, out message)) throw new RagioniNotFoundException();

                // Predispone le trasmissioni ai destinatari del documento in partenza
                DocsPaVO.trasmissione.Trasmissione trasmissione = SetCorrispondentiTrasmissioneAutomatica(schedaDocArrivo, idRegistro, schedaDocPartenza, ruolo, infoUtente);


                if (trasmissione == null) throw new Exception();

                trasmissione.infoDocumento.tipoProto = schedaDocArrivo.tipoProto; //"I";

                if (trasmissione.trasmissioniSingole.Count > 0)
                {
                    // Salvataggio e Esecuzione della trasmissione solamente se il destinatario != mittente
                    if (infoUtente.delegato != null)
                        trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
				
                    //BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
                    //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName, trasmissione);

                    BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasmissione);
                
                }
            }
            catch (RagioniNotFoundException)
            {
                RagioniVerificate = false;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

		#region elisa protocollo interno
		
		/// <summary>
		/// Controlla i corrispondenti del protocollo (interno ed in uscita)
		/// </summary>
		/// <returns></returns>
		public bool SetCorrispondentiProtoInt(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo) 
		{
			try
			{
				
				DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
				utente.idPeople = schedaDoc.idPeople;	

				// creo l'oggetto qca in caso di trasmissioni a UO
				DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
				qco.fineValidita = true;
				DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = SetQCA(qco, schedaDoc);
								
				qca.ruolo = ruolo;
				qca.queryCorrispondente = qco;	

				// Acquisisci le ragioni per la trasmissione di protocollo interno

				DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestTO = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO",ruolo.idAmministrazione);
				DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestCC = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("CC",ruolo.idAmministrazione);

				if(schedaDoc.tipoProto.Equals("I"))
				{
					DocsPaVO.documento.ProtocolloInterno protoIntTO = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
				
					foreach(object destinatario in protoIntTO.destinatari)
					{
							
						qca.ragione = ragTrasmDestTO;
						DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;
					
						if (corr.systemId != ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.systemId)
						{
							if (corr.tipoIE == "I")
							{
								if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
								{
									// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
									ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
						
									if(listaRuoli != null && listaRuoli.Count > 0)
									{
										return true;										}
								}
									
							}
								
						}
					}
				}
						
			}
			catch(Exception)
			{
				return false;
			}	

			return true;
		}
		#endregion

        public DocsPaVO.trasmissione.Trasmissione SetCorrispondentiTrasmissioneUfficioReferente(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoutente, out string message)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = null;
            message = string.Empty;
            ArrayList trasm_strutture_vuote = new ArrayList();
            try
            {
                // Inizializza trasmissione
                trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                trasmissione.ruolo = ruolo;
                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDoc);
                trasmissione.infoFascicolo = null;
                DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                utente.idPeople = schedaDoc.idPeople;
                DocsPaDB.Query_DocsPAWS.Utenti utenteMittente = new DocsPaDB.Query_DocsPAWS.Utenti();
                utente.email = utenteMittente.GetEmailUtente(utente.idPeople);
                utente.idAmministrazione = infoutente.idAmministrazione;
                utente.dst = infoutente.dst;
                utente.urlWA = infoutente.urlWA;
                trasmissione.utente = utente;

                //creo l'oggetto qca in caso di trasmissioni a UO
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.fineValidita = true;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = SetQCA(qco, schedaDoc);

                qca.ruolo = ruolo;
                qca.queryCorrispondente = qco;



                switch (schedaDoc.tipoProto)
                {
                    case "I":
                        DocsPaVO.documento.ProtocolloInterno protoIntTO = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
                        if (protoIntTO.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoIntTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoIntTO.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                    }
                                }
                            }

                        }
                        break;
                    case "P":
                        DocsPaVO.documento.ProtocolloUscita protoUscTO = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;
                        if (protoUscTO.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoUscTO.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoUscTO.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                    }
                                }
                            }

                        }
                        break;
                    case "A":
                        DocsPaVO.documento.ProtocolloEntrata protoEntRef = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
                        if (protoEntRef.ufficioReferente != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmRef = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("REFERENTE", ruolo.idAmministrazione);
                            trasmissione.noteGenerali = "";
                            qca.ragione = ragTrasmRef;

                            if (protoEntRef.ufficioReferente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)protoEntRef.ufficioReferente);

                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione, trasm_strutture_vuote);
                                    }
                                }
                            }

                        }
                        break;
                }
                trasmissione.noteGenerali = "";


                if (trasm_strutture_vuote.Count > 0)
                {
                    if (message == "")
                    {
                        foreach (string s in trasm_strutture_vuote)
                            message += (" - " + s + "\\n");

                        message = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a queste strutture perchè prive di utenti o ruoli di riferimento o ruoli autorizzati o perchè mittente della trasmissione:\\n{0}\");", message);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("errore in SetCorrispondentiTrasmissione: " + ex.Message + " stack trace: " + ex.StackTrace);
                trasmissione = null;
            }

            return trasmissione;
        }

	}
}
