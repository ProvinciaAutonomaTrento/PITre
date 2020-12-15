using System;
using System.Collections;
using System.Collections.Generic;
using DocsPaDB;
using System.Configuration;
using log4net;

namespace BusinessLogic.Documenti 
{
	/// <summary>
	/// </summary>
	public class ProtoManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(ProtoManager));
		/// <summary>
		/// Metodo che restituisce la lista degli oggetti possibili per un dato registro e una data amministrazione
		/// </summary>
		public static System.Collections.ArrayList getListaOggetti(DocsPaVO.documento.QueryOggetto objQueryOggetto)
		{
			#region Codice Commentato
			/*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			db.openConnection();

			// Creo l'oggetto che dovrà essere popolato dalla funzione
			System.Collections.ArrayList listaObj = new System.Collections.ArrayList();

			// Query sul database
			string queryString =
				"SELECT SYSTEM_ID, VAR_DESC_OGGETTO FROM DPA_OGGETTARIO " +
				"WHERE CHA_OCCASIONALE='0' ";
				
			//condizioni sul registro		
			for(int i=0;i<objQueryOggetto.idRegistri.Count;i++)
			{
				if(i==0)
					queryString=queryString+" AND (";
				queryString=queryString+"ID_REGISTRO='" + objQueryOggetto.idRegistri[i].ToString() +"' ";
				if(i<objQueryOggetto.idRegistri.Count-1) 
					queryString=queryString+" OR ";
				else
					queryString=queryString+" OR ID_REGISTRO IS NULL) ";
			}
			queryString=queryString+" AND ID_AMM='" + objQueryOggetto.idAmministrazione + "'";
			if (objQueryOggetto.queryDescrizione != null && !objQueryOggetto.queryDescrizione.Equals("")) 
				queryString = queryString + " AND VAR_DESC_OGGETTO LIKE '%" + objQueryOggetto.queryDescrizione.Replace("'","''") + "%'";
			
			queryString += " ORDER BY VAR_DESC_OGGETTO";
			logger.Debug("Ricerca oggetti: " + queryString);
			DataSet dataSet = new DataSet();
			db.fillTable(queryString, dataSet, "DPA_OGGETTARIO");

			//creazione della lista oggetti
			foreach(DataRow dataRow in dataSet.Tables["DPA_OGGETTARIO"].Rows) {
				listaObj.Add(new DocsPaVO.documento.Oggetto(dataRow["SYSTEM_ID"].ToString(),dataRow["VAR_DESC_OGGETTO"].ToString()));
			}  
			dataSet.Dispose();
			db.closeConnection();*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			System.Collections.ArrayList list = doc.GetListaOggetti(objQueryOggetto);
			return list;
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="infoDoc"></param>
		/// <param name="protAnn"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static bool annullaProtocollo(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protAnn) 
		{
            // Verifica stato di consolidamento del documento
            DocumentConsolidation.CanExecuteAction(infoUtente, schedaDocumento.systemId, DocumentConsolidation.ConsolidationActionsDeniedEnum.CancelProtocol, true);

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            
            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = documentManager.AnnullaProtocollo(ref schedaDocumento, protAnn);

                if (retValue)
                {
                    //AS400
                    AS400.AS400.setAs400(infoUtente, "", schedaDocumento.systemId, DocsPaAS400.Constants.DELETE_OPERATION);
                }

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
		}
/// <summary>
/// ricerca schedadocumento by num_proto,num_anno,id_reg
/// </summary>
/// <param name="numProto"></param>
/// <param name="anno"></param>
/// <param name="idReg"></param>
/// <param name="infoutente"></param>
/// <returns></returns>
		public static DocsPaVO.documento.SchedaDocumento ricercaScheda(string numProto,string anno,string idReg,DocsPaVO.utente.InfoUtente infoutente)
		{
			DocsPaVO.documento.SchedaDocumento sch=null;
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			sch=doc.ricercaProto(numProto,anno,idReg,infoutente);
			if(sch==null)
			{
				throw new Exception();
			}
			return sch;
		}
		/// <summary>
		/// ricerca protocollo by segnatura
		/// </summary>
		/// <param name="segnatura"></param>
		/// <param name="infoutente"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.SchedaDocumento ricercaScheda(string segnatura,DocsPaVO.utente.InfoUtente infoutente)
		{
			DocsPaVO.documento.SchedaDocumento sch=null;
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			sch=doc.ricercaProto(segnatura,infoutente);
			if(sch==null)
			{
				throw new Exception();
			}
			return sch;
		}
		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="objRuolo"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.SchedaDocumento protocolla(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objSicurezza, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione) 
		{
            logger.Info("BEGIN");
            //controllo se ruolo abilitato sul registro.
            bool  associato = false;
            //controllo se il Flag WSPIA è attivo.
            string flagWspia = "1";

            //SOSTITUISCO IL CARATTERE SPECIALE
            if (schedaDoc.oggetto.descrizione.Contains("–"))
                schedaDoc.oggetto.descrizione = schedaDoc.oggetto.descrizione.Replace("–", "-");

            for (int i = 0; i < objRuolo.registri.Count;i++ )
            {
                DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro )objRuolo.registri[i];
                if (reg.systemId == schedaDoc.registro.systemId)
                {
                    associato = true;
                    if (reg.FlagWspia == "1")
                        flagWspia = "1";
                    else
                        flagWspia = "0";
                   
                    break;
                }
            }
            
            if (!associato)
                throw new Exception("il ruolo " + objRuolo.descrizione + " non è associato al registro" + schedaDoc.registro.descrizione);


            #region Controlli su Mancanza Mitt/Dest
            //controllo se presente Mitt/Dest dati obligatori.
            if (schedaDoc != null && schedaDoc.protocollo != null)
            {
                if (schedaDoc.tipoProto == "A")
                {
                    DocsPaVO.documento.ProtocolloEntrata schEnt = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
                    if(!(schEnt.mittente!=null 
                        && schEnt.mittente.descrizione!=null 
                        &&  !string.IsNullOrEmpty(schEnt.mittente.descrizione.Trim())))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.MITTENTE_MANCANTE;
                        throw new Exception();
                    }
                }
                else if (schedaDoc.tipoProto == "P")
                {
                    DocsPaVO.documento.ProtocolloUscita schUsc = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;
                    if (!(schUsc.destinatari != null
                        && schUsc.destinatari.Count > 0
                        && schUsc.destinatari[0]!=null
                        && !String.IsNullOrEmpty(((DocsPaVO.utente.Corrispondente)schUsc.destinatari[0]).descrizione.Trim())
                        && ((DocsPaVO.utente.Corrispondente)schUsc.destinatari[0]).systemId != "0"
                        ))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DESTINATARIO_MANCANTE;
                        throw new Exception();
                    }
                
                }
                else if (schedaDoc.tipoProto == "I")
                {
                    DocsPaVO.documento.ProtocolloInterno schInt = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
                    if (!(schInt.mittente != null
                        && schInt.mittente.descrizione != null
                        && !string.IsNullOrEmpty(schInt.mittente.descrizione.Trim())))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.MITTENTE_MANCANTE;
                        throw new Exception();
                    }

                    if (!
                        (schInt.destinatari != null
                        && schInt.destinatari.Count > 0
                        && schInt.destinatari[0] != null
                        && ((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).descrizione != null
                        && !string.IsNullOrEmpty(((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).descrizione.Trim())
                        && ((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).systemId != "0"))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DESTINATARIO_MANCANTE;
                        throw new Exception();
                    }

                
                }




            }
            #endregion

            // Avvio del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                //Import pregressi
                if (!schedaDoc.pregresso)
                {
                    schedaDoc = getDataProtocollo(schedaDoc);
                }
                schedaDoc.predisponiProtocollazione = false;
                DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                if (DocsPaDB.Query_Utils.Utils.getStatoRegistro(schedaDoc.registro) != "G")
                {
                    schedaDoc.oraCreazione = obj.SelectDBTime();
                }
                risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;

                //add massimo digregorio carica dati protocollatore in schedaProtocollo
                schedaDoc = getDatiProtocollatore(schedaDoc, objRuolo, objSicurezza);
                //DATI CREATORE
                schedaDoc = getDatiCreatore(schedaDoc, objRuolo, objSicurezza);

                if (!string.IsNullOrEmpty(schedaDoc.systemId))
                {
                    if (!doc.CheckProto(schedaDoc))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                        throw new Exception();
                    }

                    schedaDoc = protocollaDocProntoProtocollazione(objSicurezza, objRuolo, schedaDoc);
                }
                else
                {
                    DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza, flagWspia);

                    List<DocsPaVO.documento.FileRequest> versions = new List<DocsPaVO.documento.FileRequest>();

                    
                if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                    {
                        versions = new List<DocsPaVO.documento.FileRequest>(
                            (DocsPaVO.documento.FileRequest[])
                                schedaDoc.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest)));

                        // Ordinamento versioni
                        versions.Sort(
                                delegate(DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
                                {
                                    int versionX, versionY;
                                    Int32.TryParse(x.version, out versionX);
                                    Int32.TryParse(y.version, out versionY);

                                    return (versionX.CompareTo(versionY));
                                }
                            );
                    }
                    
                    DocsPaVO.utente.Ruolo[] ruoliSuperiori;

                    if (!documentManager.CreateProtocollo(schedaDoc, objRuolo, out risultatoProtocollazione, out ruoliSuperiori))
                    {
                        throw new Exception();
                    }
                    else
                    {
                        // Notifica creazione del protocollo
                        DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(objSicurezza);

                        eventsNotification.DocumentoCreatoEventHandler(schedaDoc, objRuolo, ruoliSuperiori);

                        // Sincronizzazione repository
                        if (schedaDoc.repositoryContext != null)
                        {
                            SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(schedaDoc.repositoryContext);
                            
                            // In fase di inserimento di un repository temporaneo, 
                            // possono essere creati:
                            // - la prima versione del documento e, qualora sia stato acquisito un file
                            //   e firmato, anche la seconda versione firmata del documento
                            // - la prima versione di n allegati
                            // La prima versione del documento è creata automaticamente con la creazione del documento stesso.
                            // In caso di seconda versione firmata del documento, è necessario procedre alla creazione.
                            foreach (DocsPaVO.documento.FileRequest v in versions)
                            {
                                int version;
                                Int32.TryParse(v.version, out version);

                                DocsPaVO.documento.FileRequest savedVersion = null;

                                if (version > 1)
                                {
                                    // Seconda versione firmata del documento,
                                    // impostazione dell'id del documento di appartenenza
                                    v.docNumber = schedaDoc.docNumber;
                                    
                                    // Inserimento delle versioni del documento,
                                    // acquisite oltre alla versione principale
                                    if (!documentManager.AddVersion(v, false))
                                        throw new ApplicationException(string.Format("Errore nella creazione della versione {0} del documento con id {1}", version, schedaDoc.systemId));

                                    savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, v);

                                    // Inserimento della nuova versione come primo elemento della lista documenti
                                    schedaDoc.documenti.Insert(0, savedVersion);
                                }
                                else
                                {
                                    // La versione principale del documento è già stata creata al momento dell'inserimento,
                                    // pertanto è necessario copiare solamente il file acquisito nel repository
                                    savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, (DocsPaVO.documento.FileRequest) schedaDoc.documenti[0]);

                                    // Aggiornamento istanza documento principale
                                    schedaDoc.documenti[0] = savedVersion;
                                }
                            }

                            if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                            {
                                // Gli allegati e le rispettive versioni andranno create manualmente
                                foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                                {
                                    string oldVersionLabel = allegato.versionLabel;

                                    // Impostazione del docnumber del documento principale
                                    // cui sarà associato l'allegato
                                    allegato.docNumber = schedaDoc.docNumber;

                                    if (!documentManager.AddAttachment(allegato, "N"))
                                        throw new ApplicationException(string.Format("Errore nella creazione dell'allegato {0} del documento con id {1}", allegato.position, schedaDoc.systemId));

                                    allegato.versionLabel = oldVersionLabel;
                                }

                                DocsPaVO.documento.FileRequest[] allegati = (DocsPaVO.documento.FileRequest[]) schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato));

                                if (allegati.Length > 0)
                                    schedaDoc.allegati = new ArrayList(SessionRepositorySyncronizer.CopyToRepository(fileManager, allegati));
                            }

                            // Se è presente un repository temporaneo, viene effettuato l'inserimento del file nel repository del documentale
                            // Imposta il repository come scaduto
                            fileManager.Delete();
                            
                            schedaDoc.repositoryContext = null;
                        }
                    }
                }

                //Richiamo il metodo per il calcolo della atipicità del documento
                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                schedaDoc.InfoAtipicita = documentale.CalcolaAtipicita(objSicurezza, schedaDoc.docNumber, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);

                if (risultatoProtocollazione == DocsPaVO.documento.ResultProtocollazione.OK)
                {
                    // Impostazione della transazione come completata
                    transactionContext.Complete();
                }
            }

            logger.Info("END");

			return schedaDoc;
		}

        public static DocsPaVO.documento.SchedaDocumento protocollaPerAggiorna(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objSicurezza, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            //controllo se ruolo abilitato sul registro.
            bool associato = false;

            for (int i = 0; i < objRuolo.registri.Count; i++)
            {
                DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)objRuolo.registri[i];
                if (reg.systemId == schedaDoc.registro.systemId)
                {
                    associato = true;
                    break;
                }
            }

            if (!associato)
                throw new Exception("il ruolo " + objRuolo.descrizione + " non è associato al registro" + schedaDoc.registro.descrizione);


            #region Controlli su Mancanza Mitt/Dest
            //controllo se presente Mitt/Dest dati obligatori.
            if (schedaDoc != null && schedaDoc.protocollo != null)
            {
                if (schedaDoc.tipoProto == "A")
                {
                    DocsPaVO.documento.ProtocolloEntrata schEnt = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
                    if (!(schEnt.mittente != null
                        && schEnt.mittente.descrizione != null
                        && !string.IsNullOrEmpty(schEnt.mittente.descrizione.Trim())))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.MITTENTE_MANCANTE;
                        throw new Exception();
                    }
                }
                else if (schedaDoc.tipoProto == "P")
                {
                    DocsPaVO.documento.ProtocolloUscita schUsc = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;
                    if (!(schUsc.destinatari != null
                        && schUsc.destinatari.Count > 0
                        && schUsc.destinatari[0] != null
                        && !String.IsNullOrEmpty(((DocsPaVO.utente.Corrispondente)schUsc.destinatari[0]).descrizione.Trim())))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DESTINATARIO_MANCANTE;
                        throw new Exception();
                    }

                }
                else if (schedaDoc.tipoProto == "I")
                {
                    DocsPaVO.documento.ProtocolloInterno schInt = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
                    if (!(schInt.mittente != null
                        && schInt.mittente.descrizione != null
                        && !string.IsNullOrEmpty(schInt.mittente.descrizione.Trim())))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.MITTENTE_MANCANTE;
                        throw new Exception();
                    }

                    if (!(schInt.destinatari != null
                        && schInt.destinatari.Count > 0 != null
                        && schInt.destinatari[0] != null
                        && ((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).descrizione != null)
                        && !string.IsNullOrEmpty(((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).descrizione.Trim()))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DESTINATARIO_MANCANTE;
                        throw new Exception();
                    }


                }




            }
            #endregion

            // Avvio del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                schedaDoc = getDataProtocollo(schedaDoc);
                schedaDoc.predisponiProtocollazione = false;
                DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                bool protocollazioneLibera = bool.Parse(ConfigurationSettings.AppSettings["PROTOCOLLAZIONE_LIBERA"].ToLower());


                ArrayList funz = objRuolo.funzioni;
                foreach (DocsPaVO.utente.Funzione f in funz)//verifico se attiva la microfunz IMP_DOC_MASSIVA_PREG
                {
                    if (f.codice.Equals("IMP_DOC_MASSIVA_PREG"))
                        protocollazioneLibera= true;
                }



                /*if (DocsPaDB.Query_Utils.Utils.getStatoRegistro(schedaDoc.registro) != "G")
                {
                    schedaDoc.oraCreazione = obj.SelectDBTime();
                }*/
                if (protocollazioneLibera)
                {
                    if (DocsPaDB.Query_Utils.Utils.getStatoRegistro(schedaDoc.registro) != "G" && objSicurezza.urlWA != null)
                    {
                        schedaDoc.oraCreazione = obj.SelectDBTime();
                    }

                }
                else
                {
                    if (DocsPaDB.Query_Utils.Utils.getStatoRegistro(schedaDoc.registro) != "G")
                    {
                        schedaDoc.oraCreazione = obj.SelectDBTime();
                    }

                }

                risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;

                //add massimo digregorio carica dati protocollatore in schedaProtocollo
                schedaDoc = getDatiProtocollatore(schedaDoc, objRuolo, objSicurezza);
                //DATI CREATORE
                schedaDoc = getDatiCreatore(schedaDoc, objRuolo, objSicurezza);

                if (!string.IsNullOrEmpty(schedaDoc.systemId))
                {
                    if (!doc.CheckProto(schedaDoc))
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                        throw new Exception();
                    }

                    schedaDoc = protocollaDocProntoProtocollazione(objSicurezza, objRuolo, schedaDoc);
                }
                //else
                //{
                //    DocsPaDocumentale_GFD.Documentale.DocumentManager documentManager = new DocsPaDocumentale_GFD.Documentale.DocumentManager(objSicurezza);


                //    List<DocsPaVO.documento.FileRequest> versions = new List<DocsPaVO.documento.FileRequest>();


                //    if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                //    {
                //        versions = new List<DocsPaVO.documento.FileRequest>(
                //            (DocsPaVO.documento.FileRequest[])
                //                schedaDoc.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest)));

                //        // Ordinamento versioni
                //        versions.Sort(
                //                delegate(DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
                //                {
                //                    int versionX, versionY;
                //                    Int32.TryParse(x.version, out versionX);
                //                    Int32.TryParse(y.version, out versionY);

                //                    return (versionX.CompareTo(versionY));
                //                }
                //            );
                //    }

                //    DocsPaVO.utente.Ruolo[] ruoliSuperiori;

                //    if (!documentManager.AggiornaProtocollo(schedaDoc, objRuolo, out risultatoProtocollazione, out ruoliSuperiori))
                //    {
                //        throw new Exception();
                //    }
                //    else
                //    {
                //        // Notifica creazione del protocollo
                //        DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(objSicurezza);

                //        eventsNotification.DocumentoCreatoEventHandler(schedaDoc, objRuolo, ruoliSuperiori);

                //        // Sincronizzazione repository
                //        if (schedaDoc.repositoryContext != null)
                //        {
                //            SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(schedaDoc.repositoryContext);

                //            // In fase di inserimento di un repository temporaneo, 
                //            // possono essere creati:
                //            // - la prima versione del documento e, qualora sia stato acquisito un file
                //            //   e firmato, anche la seconda versione firmata del documento
                //            // - la prima versione di n allegati
                //            // La prima versione del documento è creata automaticamente con la creazione del documento stesso.
                //            // In caso di seconda versione firmata del documento, è necessario procedre alla creazione.
                //            foreach (DocsPaVO.documento.FileRequest v in versions)
                //            {
                //                int version;
                //                Int32.TryParse(v.version, out version);

                //                DocsPaVO.documento.FileRequest savedVersion = null;

                //                if (version > 1)
                //                {
                //                    // Seconda versione firmata del documento,
                //                    // impostazione dell'id del documento di appartenenza
                //                    v.docNumber = schedaDoc.docNumber;

                //                    // Inserimento delle versioni del documento,
                //                    // acquisite oltre alla versione principale
                //                    if (!documentManager.AddVersion(v, false))
                //                        throw new ApplicationException(string.Format("Errore nella creazione della versione {0} del documento con id {1}", version, schedaDoc.systemId));

                //                    savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, v);

                //                    // Inserimento della nuova versione come primo elemento della lista documenti
                //                    schedaDoc.documenti.Insert(0, savedVersion);
                //                }
                //                else
                //                {
                //                    // La versione principale del documento è già stata creata al momento dell'inserimento,
                //                    // pertanto è necessario copiare solamente il file acquisito nel repository
                //                    savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0]);

                //                    // Aggiornamento istanza documento principale
                //                    schedaDoc.documenti[0] = savedVersion;
                //                }
                //            }

                //            if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                //            {
                //                // Gli allegati e le rispettive versioni andranno create manualmente
                //                foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                //                {
                //                    string oldVersionLabel = allegato.versionLabel;

                //                    // Impostazione del docnumber del documento principale
                //                    // cui sarà associato l'allegato
                //                    allegato.docNumber = schedaDoc.docNumber;

                //                    if (!documentManager.AddAttachment(allegato, "N"))
                //                        throw new ApplicationException(string.Format("Errore nella creazione dell'allegato {0} del documento con id {1}", allegato.position, schedaDoc.systemId));

                //                    allegato.versionLabel = oldVersionLabel;
                //                }

                //                DocsPaVO.documento.FileRequest[] allegati = (DocsPaVO.documento.FileRequest[])schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato));

                //                if (allegati.Length > 0)
                //                    schedaDoc.allegati = new ArrayList(SessionRepositorySyncronizer.CopyToRepository(fileManager, allegati));
                //            }

                //            // Se è presente un repository temporaneo, viene effettuato l'inserimento del file nel repository del documentale
                //            // Imposta il repository come scaduto
                //            fileManager.Delete();

                //            schedaDoc.repositoryContext = null;
                //        }
                //    }
                //}

                if (risultatoProtocollazione == DocsPaVO.documento.ResultProtocollazione.OK)
                {
                    // Impostazione della transazione come completata
                    transactionContext.Complete();
                }
            }



            return schedaDoc;
        }


/// <summary>
/// add massimo digregorio gestione protocollatore
/// </summary>
/// <param name="schedaDoc"></param>
/// <param name="objRuolo"></param>
/// <param name="objUtente"></param>
/// <returns></returns>
		internal static DocsPaVO.documento.SchedaDocumento getDatiProtocollatore(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente) 
		{ 
			if (schedaDoc.protocollatore == null || schedaDoc.protocollatore.utente_idPeople.Equals(String.Empty))
			{
				schedaDoc.protocollatore = new DocsPaVO.documento.Protocollatore(objUtente, objRuolo);
			} 
			return schedaDoc;
		}
		internal static DocsPaVO.documento.SchedaDocumento getDatiCreatore(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente) 
		{ 
			if (schedaDoc.creatoreDocumento == null || schedaDoc.creatoreDocumento.idPeople.Equals(String.Empty))
			{
				schedaDoc.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(objUtente,objRuolo);
			} 
			return schedaDoc;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        internal static DocsPaVO.documento.SchedaDocumento protocollaDocProntoProtocollazione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
            string flag = "1";

            for (int i = 0; i < objRuolo.registri.Count; i++)
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg = (DocsPaVO.utente.Registro)objRuolo.registri[i];
                if (schedaDoc.registro.codRegistro == reg.codRegistro)
                {                    
                        flag = reg.FlagWspia;                   
                break;
            }
                ///MODIFICA AFIORDI
              
            }

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente, flag);

            DocsPaVO.documento.ResultProtocollazione result;
            documentManager.ProtocollaDocumentoPredisposto(schedaDoc, objRuolo, out result);

            // Se il risultato della protocollazione è positivo, il documento è stato ricevuto per 
            // inteoperabilità semplificata, viene inviata al mittente la ricevuta di conferma di
            // ricezione
            if(schedaDoc.typeId == BusinessLogic.interoperabilita.Semplificata.InteroperabilitaSemplificataManager.InteroperabilityCode)
                BusinessLogic.interoperabilita.Semplificata.SimplifiedInteroperabilityProtoManager.SendDocumentReceivedProofToSender(
                    schedaDoc.systemId,
                    infoUtente,
                    schedaDoc.registro.systemId);

            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RICEVUTA_PROTO_PORTALE")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RICEVUTA_PROTO_PORTALE").Equals("1"))
            {
                if (schedaDoc.tipoProto == "A")
                {
                    if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.canalePref != null && ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.canalePref.descrizione.ToUpper() == "PORTALE")
                    {
                        // Genero la ricevuta di protocollazione e la inserisco come nuovo allegato
                        DocsPaVO.documento.FileDocumento ricevuta = BusinessLogic.Modelli.StampaRicevutaProtocolloPdf.Create(infoUtente, schedaDoc.docNumber);

                        if (ricevuta != null && ricevuta.content != null && ricevuta.content.Length > 0)
                        {
                            ricevuta.name = "Ricevuta di protocollazione.pdf";
                            ricevuta.fullName = ricevuta.name;
                            ricevuta.nomeOriginale = ricevuta.name;
                            ricevuta.contentType = "application/pdf";
                            ricevuta.estensioneFile = "pdf";

                            DocsPaVO.documento.Allegato allRicevuta = new DocsPaVO.documento.Allegato();
                            allRicevuta.docNumber = schedaDoc.docNumber;
                            allRicevuta.descrizione = "Ricevuta di protocollazione";
                            allRicevuta.fileName = ricevuta.nomeOriginale;
                            allRicevuta.version = "0";
                            allRicevuta.numeroPagine = 1;

                            DocsPaVO.documento.Allegato allResult = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, allRicevuta);
                            if (allResult != null)
                            {
                                BusinessLogic.Documenti.FileManager.putFile(allResult, ricevuta, infoUtente);
                            }
                        }
                    }
                }
            }

			return schedaDoc;
		}

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="corrispondente"></param>
		/// <returns></returns>
		public static DocsPaVO.utente.Corrispondente addCorrispondente(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Corrispondente corrispondente) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            if (!doc.AddCorrispondente(schedaDoc, ref corrispondente, corrispondente.idAmministrazione))
			{
				//TODO: gestire la throw
				throw new Exception();
			}
			return corrispondente;
		}

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		/*private static DocsPaVO.documento.SchedaDocumento getDataProtocollo(DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			string data = schedaDoc.protocollo.dataProtocollazione;
			if(data!=null)
			{
				data=data.Trim();
			}
			
			// utilizzo la data di apertura del registro solo se non è settata una data di apertura
			if(!(data != null && !data.Equals(""))) 
			{
				if (schedaDoc.registro.dataApertura.IndexOf(" ")>0)
					schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura.Substring(0,schedaDoc.registro.dataApertura.IndexOf(" "));
				else
					schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura;
				data = schedaDoc.protocollo.dataProtocollazione.Trim();
			}

			schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/")+1);
			
			return schedaDoc;
		}*/

        private static DocsPaVO.documento.SchedaDocumento getDataProtocollo(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {

            string data = schedaDoc.protocollo.dataProtocollazione;

            if (data != null)
            {

                data = data.Trim();

            }



            // utilizzo la data di apertura del registro solo se non è settata una data di apertura



            bool protocollazioneLibera = false;

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["PROTOCOLLAZIONE_LIBERA"]) &&

    ConfigurationSettings.AppSettings["PROTOCOLLAZIONE_LIBERA"].ToUpper().Equals("TRUE"))

                protocollazioneLibera = bool.Parse(ConfigurationSettings.AppSettings["PROTOCOLLAZIONE_LIBERA"].ToLower());


            /* MEV 3765 Gestione selettiva integrazione WSPIA
                 * Modifica MCaropreso:
                 * Effettua controllo sul flag associato al registro
                 */
            bool flagWSPIA = false;
            if (schedaDoc != null && schedaDoc.registro.FlagWspia !=null &&
                schedaDoc.registro.FlagWspia.Equals("1")
                ) flagWSPIA = true;
            else
                flagWSPIA = false;

            protocollazioneLibera = protocollazioneLibera && flagWSPIA;


            if (!protocollazioneLibera)
            {

                if (!(data != null && !data.Equals("")))
                {

                    if (schedaDoc.registro.dataApertura.IndexOf(" ") > 0)

                        schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura.Substring(0, schedaDoc.registro.dataApertura.IndexOf(" "));

                    else

                        schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura;

                    data = schedaDoc.protocollo.dataProtocollazione.Trim();

                }
                schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/") + 1);
		
            }



            //protocollazioneLibera
            else
                if (string.IsNullOrEmpty(schedaDoc.protocollo.anno))
                {
                    schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/") + 1);
                }
                  
            return schedaDoc;

        }



      


 
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="debug"></param>
		internal static void checkInputData(string idAmministrazione, DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			#region Codice Commentato
			/*DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
			
			try 
			{
				logger.Debug("idAmministrazione = " + objSicurezza.idAmministrazione);
			} 
			catch (Exception) 
			{
				TODO: throw throwException(db, null, "idAmministrazione non trovata");
			}
			
			if(schedaDoc.registro != null && schedaDoc.protocollo.dataProtocollazione != null) 
			{
				try 
				{
					queryString = "SELECT  CHA_STATO FROM DPA_EL_REGISTRI WHERE SYSTEM_ID=" + schedaDoc.registro.systemId;
					logger.Debug(queryString);
					string result;
					obj.getStatoRegistri(out result, schedaDoc.registro.systemId);
					
					if(!result.Equals("A"))
					{
						throw throwException(db, null, "Il registro non è aperto");
					}
					
				} 
				catch (Exception e) 
				{
					logger.Debug(e.Message);
					throw throwException(db, null, "Lo stato del registro non è corretto");
				}
			}
			if(schedaDoc.protocollo != null) 
			{
				if(schedaDoc.protocollo.dataProtocollazione != null) 
				{
					if(DateTime.Now < DocsPaWS.Utils.DateControl.toDate(schedaDoc.protocollo.dataProtocollazione))
					{
						throw throwException(db, null, "La data di protocollazione è successiva a quella attuale");
					}
				}

				if(schedaDoc.registro == null)
				{
					throw throwException(db, null, "Il registro è obbligatorio");
				}
				
				if(schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata)) 
				{
					if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente == null) 
					{
						throw throwException(db, null, "Il mittente è obbligatorio");
					}
				} 
				else if(schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita)) 
				{
					if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count == 0) 
					{
						throw throwException(db, null, "Il destinatario è obbligatorio");
					}
				}
				queryString = 
					"SELECT COUNT(*) FROM DPA_EL_REGISTRI WHERE SYSTEM_ID = " +
					schedaDoc.registro.systemId + " AND DTA_ULTIMO_PROTO > " +
					DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.dataProtocollazione,false);
				logger.Debug(queryString);
				string res;
				string date = DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.dataProtocollazione,false);
				obj.getNumRegistri(out res, schedaDoc.registro.systemId, date);

				if (!res.Equals("0"))
				{
					// TODO: throw throwException(db, null, "La data di protocollazione non è valida");				
				}
			}

			if (!(schedaDoc.oggetto != null && schedaDoc.oggetto.descrizione != null && !schedaDoc.oggetto.descrizione.Equals("")))
			{
				// TODO: throw throwException(db, null, "L'oggetto è obbligatorio");
			}*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.CheckInputData(idAmministrazione, schedaDoc);
		}

		#region Metodo Commentato
//		/// <summary>
//		/// </summary>
//		/// <param name="db"></param>
//		/// <param name="documento"></param>
//		/// <param name="schedaDoc"></param>
//		/// <param name="objSicurezza"></param>
//		/// <param name="debug"></param>
//		/// <returns></returns>
//		internal static string createPCDDoc(out DocsPaDocManagement.Documentale.Documento documento, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente objSicurezza) 
//		{
//			string idAmministrazione = objSicurezza.idAmministrazione;
//			string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
//			
//			documento = new DocsPaDocManagement.Documentale.Documento(objSicurezza.dst, library, DocsPaDocManagement.Documentale.Tipi.ObjectType.Cyd_CmnProjects);
//
//			/*
//			docObj.SetProperty("DOCNAME", "");	
//			docObj.SetProperty("AUTHOR", schedaDoc.idPeople);	
//			docObj.SetProperty("AUTHOR_ID", schedaDoc.userId);
//			docObj.SetProperty("TYPE_ID", schedaDoc.typeId);				
//			docObj.SetProperty( "TYPIST_ID", schedaDoc.userId);					
//			docObj.SetProperty("APP_ID", schedaDoc.appId);
//			docObj.SetProperty( "%VERIFY_ONLY", "%NO" );
//			*/
//			documento.DocumentName = "";
//			documento.Author = schedaDoc.idPeople;
//			documento.AuthorId = schedaDoc.userId;
//			documento.TypeId = schedaDoc.typeId;
//			documento.TypistId = schedaDoc.userId;
//			documento.AppId = schedaDoc.appId;
//			documento.VerifyOnly = DocsPaDocManagement.Documentale.Tipi.VerifyOnlyType.No;
//
//			// è necessario inserire anche i permessi per l'utente TYPIST_ID
//			documento.SetTrustee(schedaDoc.userId, 2, 0);
//
//			//docObj.SetTrustee("PRO_DIP1_DIRII_UFF",1,255);
//			documento.Create();	
//	
//			/*
//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nella creazione del documento");
//			*/
//			if(documento.GetErrorCode() != 0)
//			{
//				throw new Exception("Errore nella creazione del documento");
//			}
//
//			logger.Debug("documento creato");	
//			/*
//			string 	docNumber = docObj.GetReturnProperty("%OBJECT_IDENTIFIER").ToString();
//			*/
//			string docNumber = documento.ObjectIdentifier;
//
//			//string versionId = docObj.GetReturnProperty("%VERSION_ID").ToString();
//	
//			// unlock del documento			
//			//docObj.SetProperty("%OBJECT_IDENTIFIER", docNumber);		
//			
//			/*
//			docObj.SetProperty("%STATUS", "%UNLOCK");
//			*/
//			documento.Status = DocsPaDocManagement.Documentale.Tipi.StatusType.Unlock;
//
//			documento.Update();	
//	
//			/*
//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nell'update del documento");
//			*/
//			if(documento.GetErrorCode() != 0)
//			{
//				throw new Exception("Errore nell'update del documento");
//			}
//			
//			logger.Debug("unlocked");	
//
//			//aggiorna flag daInviare
//			//string updateStr = "UPDATE VERSIONS SET CHA_DA_INVIARE = '1'";
//			string firstParam = "CHA_DA_INVIARE = '1'";
//			
//			if(schedaDoc.documenti!=null && schedaDoc.documenti.Count>0)
//			{
//				logger.Debug("Documenti presenti");
//				int lastDocNum=schedaDoc.documenti.Count-1;
//				logger.Debug(""+lastDocNum);
//				
//				if(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo!=null && !((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo.Equals(""))
//				{
//					firstParam += ", DTA_ARRIVO ="+DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo,false);
//				}
//			}
//			//updateStr +=" WHERE DOCNUMBER=" + docNumber;
//			//logger.Debug(updateStr);
//			//db.executeNonQuery(updateStr);
//			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
//			doc.UpdateVersions(firstParam, docNumber);
//		
//			return docNumber; 
//		}
		#endregion
		
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="objRuolo"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		internal static DocsPaVO.documento.SchedaDocumento setDocTrustees(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, DocsPaVO.utente.InfoUtente delegato) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            schedaDoc = doc.SetDocTrustees(schedaDoc, objRuolo, out ruoliSuperiori, delegato);
			return schedaDoc;
		}
		/// <summary>
		/// addOggetto, che utilizza una insertlocked, non utilizzare un protocollazione o creazione documenti.
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <param name="schedaDoc"></param>
		/// <returns></returns>
		internal static DocsPaVO.documento.SchedaDocumento addOggettoLocked(string idAmministrazione, DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.InsertOggLocked(idAmministrazione, ref schedaDoc);
			return schedaDoc;
				
			
		}
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		internal static DocsPaVO.documento.SchedaDocumento addOggetto(string idAmministrazione, DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.InsertOgg(idAmministrazione, ref schedaDoc);
			return schedaDoc;
				
			#region Codice Commentato
			/*logger.Debug("addOggetto");
			string sqlString;
			// verifico se è un oggetto occasionale
			string idOggetto = schedaDoc.oggetto.systemId;
			logger.Debug("idOggetto = " + idOggetto);
			string idRegistro = "null";
			
			if (schedaDoc.registro != null)
			{
				idRegistro = schedaDoc.registro.systemId;
			}
			
			if (!(idOggetto != null && !idOggetto.Equals(""))) 
			{
				// inserisco nella tabella DPA_OGGETTARIO
				sqlString = 
					"INSERT INTO DPA_OGGETTARIO (" + DocsPaWS.Utils.dbControl.getSystemIdColName() +
					"ID_REGISTRO, ID_AMM, VAR_DESC_OGGETTO, CHA_OCCASIONALE) " +
					"VALUES (" + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_OGGETTARIO")+ 
					idRegistro + ",'" + objSicurezza.idAmministrazione + "','" +
					schedaDoc.oggetto.descrizione.Replace("'","''")+"','1')";
				logger.Debug(sqlString);									
				schedaDoc.oggetto.systemId = db.insertLocked(sqlString, "DPA_OGGETTARIO");
			}	
			return schedaDoc;*/
			#endregion
		}

        /* ABBATANGELI GIANLUIGI
         * Il metodo getNumeroProtocollo di documenti.cs è private */
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="ruolo"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		//private static DocsPaVO.documento.SchedaDocumento getNumeroProtocollo (DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo) 
		//{
			#region Codice Commentato
			/*string sqlString = "SELECT NUM_RIF FROM DPA_REG_PROTO WHERE ID_REGISTRO=" + schedaDoc.registro.systemId;
			logger.Debug(sqlString);
			int numeroProtocollo = Int32.Parse(db.executeScalar(sqlString).ToString());
			schedaDoc.protocollo.numero = numeroProtocollo.ToString();
			numeroProtocollo += 1;
			string updateString = 
				"UPDATE DPA_REG_PROTO SET NUM_RIF=" + numeroProtocollo.ToString() + 
				" WHERE ID_REGISTRO=" + schedaDoc.registro.systemId;
			logger.Debug(updateString);
			db.executeNonQuery(updateString);
			// segnatura
			schedaDoc.protocollo.segnatura = calcolaSegnatura(db, schedaDoc, ruolo);*/
			#endregion

			//DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			//doc.GetNumeroProtocollo(ref schedaDoc, ruolo);
			//return schedaDoc;
		//}

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="ruolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static bool updateNumeroProtocolloWSPIA(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            bool esito = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            esito = doc.UpdateNumeroProtocolloWSPIA(schedaDoc, ruolo);
            return esito;
        }

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.SchedaDocumento getProfile(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.GetProfile(infoUtente, ref schedaDoc);
			return schedaDoc;

			#region Codice Commentato
			/*string sqlString =
				"SELECT A.SYSTEM_ID, A.DOCNUMBER, A.DOCSERVER_LOC, A.PATH, " +
				"B.TYPE_ID, B.DESCRIPTION " +
				"FROM PROFILE A, DOCUMENTTYPES B " +
				"WHERE A.DOCUMENTTYPE=B.SYSTEM_ID AND A.DOCNUMBER = " + schedaDoc.docNumber;
			logger.Debug(sqlString);
			db.fillTable(sqlString, dataSet, "PROFILE");	
			//dovrebbe tornare una e una sola riga
			if (dataSet.Tables["PROFILE"].Rows.Count > 0) 
			{
				DataRow dataRow = dataSet.Tables["PROFILE"].Rows[0];
				schedaDoc.systemId =  dataRow["SYSTEM_ID"].ToString();
				logger.Debug("idProfile="+schedaDoc.systemId);	
				schedaDoc.dataCreazione = DocsPaWS.Utils.DateControl.getDate(false);
				
				// Documenti
				schedaDoc.documenti = DocManager.getDocumenti(db, dataRow);
			}
			dataSet.Dispose();

			return schedaDoc;*/
			#endregion
		}

		//attenzione da fare!
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		public static void UpdateProfile(DocsPaVO.documento.SchedaDocumento schedaDoc, string sede) 
		{
			//string updateString;
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.UpdateProfile(schedaDoc, sede, String.Empty);
			
			#region Codice Commentato
			/*string img = "0";
			if (!((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).fileSize.Equals("0"))
			{
				img = "1";
			}
			string note = schedaDoc.note;
			if (note == null) 
			{
				note = "";
			}
			string assegnato = "0";
			if (schedaDoc.assegnato != null && !schedaDoc.assegnato.Equals(""))
			{
				assegnato = schedaDoc.assegnato;
			}
			string fascicolato = "0";
			if (schedaDoc.fascicolato != null && !schedaDoc.fascicolato.Equals(""))
			{
				fascicolato = schedaDoc.fascicolato;
			}
			string privato = "0";
			if (schedaDoc.privato != null && !schedaDoc.privato.Equals(""))
			{
				privato = schedaDoc.privato;
			}
			string evidenza = "0";
			if (schedaDoc.evidenza != null && !schedaDoc.evidenza.Equals(""))
			{
				evidenza = schedaDoc.evidenza;
			}
			updateString =
				"UPDATE PROFILE SET " +					
				"VAR_NOTE = '" + note.Replace("'","''") + "'" + 
				", ID_OGGETTO = " + schedaDoc.oggetto.systemId + 
				", CHA_MOD_OGGETTO = '0' " +
				", CHA_IMG = '" + img + "'" +
				", CHA_ASSEGNATO = '" + assegnato + "'" +
				", CHA_FASCICOLATO = '" + fascicolato + "'" +
				", CHA_PRIVATO = '" + privato + "'" + 
				", CHA_EVIDENZA = '" + evidenza + "'";

			string tipoProto = "G";
			string daProtocollare = "0";

			if (schedaDoc.protocollo != null) 
			{
				if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
				{
					tipoProto = "A";
				}
				else if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita)) 
				{
					tipoProto = "P";
				}
				if (schedaDoc.protocollo.daProtocollare != null && !schedaDoc.protocollo.daProtocollare.Equals(""))
				{
					daProtocollare = schedaDoc.protocollo.daProtocollare;
				}
				string idParent = "0";
				if (schedaDoc.protocollo.rispostaProtocollo != null)
				{
					idParent = schedaDoc.protocollo.rispostaProtocollo.idProfile;
				}
				string invioConferma = schedaDoc.protocollo.invioConferma;
				if (invioConferma == null) 
				{
					invioConferma = "0";
				}
				string modMittDest = "0";
				if (schedaDoc.protocollo.modMittDest != null && !schedaDoc.protocollo.modMittDest.Equals(""))
				{
					modMittDest = schedaDoc.protocollo.modMittDest;
				}
				string modMittInt = "0";
				if (schedaDoc.protocollo.modMittInt != null && !schedaDoc.protocollo.modMittInt.Equals(""))
				{
					modMittInt = schedaDoc.protocollo.modMittInt;
				}
				updateString +=		
					", ID_REGISTRO = " + schedaDoc.registro.systemId + 
					", CHA_MOD_MITT_DEST = '" + modMittDest + "'" +
					", CHA_MOD_MITT_INT = '" + modMittInt + "'" +
					", ID_PARENT = " + idParent +
					", CHA_INVIO_CONFERMA = '" + invioConferma + "'";
				
				// DatiEmergenza
				if (schedaDoc.protocollo.datiEmergenza != null) 
				{
					updateString += ", VAR_PROTO_EME = '" + schedaDoc.protocollo.datiEmergenza.protocolloEmergenza.Replace("'","''") + "'";
					updateString += ", DTA_PROTO_EME = " + DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.datiEmergenza.dataProtocollazioneEmergenza, false); 
					updateString += ", VAR_COGNOME_EME = '" + schedaDoc.protocollo.datiEmergenza.cognomeProtocollatoreEmergenza.Replace("'","''") + "'";
					updateString += ", VAR_NOME_EME = '" + schedaDoc.protocollo.datiEmergenza.nomeProtocollatoreEmergenza.Replace("'","''") + "'";
				}

				// Protocollo Mittente
				if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata)) 
				{
					DocsPaVO.documento.ProtocolloEntrata pe	= (DocsPaVO.documento.ProtocolloEntrata) schedaDoc.protocollo;
					updateString += ", VAR_PROTO_IN = '" + pe.descrizioneProtocolloMittente + "'";
					if (pe.dataProtocolloMittente != null && !pe.dataProtocolloMittente.Equals(""))
					{
						updateString += ", DTA_PROTO_IN = " + DocsPaWS.Utils.dbControl.toDate(pe.dataProtocolloMittente, false);
					}
				}

				// Il documento è protocollato
				if (schedaDoc.protocollo.segnatura != null && !schedaDoc.protocollo.segnatura.Equals("")) 
				{
					checkProto(db, schedaDoc);
					updateString +=					
						", VAR_CHIAVE_PROTO = " + calcolaChiaveProto(schedaDoc) +		
						", NUM_PROTO = " + schedaDoc.protocollo.numero +
						", VAR_SEGNATURA = '" + schedaDoc.protocollo.segnatura + "'" +
						", DTA_PROTO = " + DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.dataProtocollazione, false) + 
						", NUM_ANNO_PROTO = " + schedaDoc.protocollo.anno;
				}
			}

			// TipologiaAtto
			string idTipoAtto = null;
			if (schedaDoc.tipologiaAtto != null)					
			{
				idTipoAtto = schedaDoc.tipologiaAtto.systemId;
			}
			if (idTipoAtto != null && !idTipoAtto.Equals(""))
			{
				updateString += ", ID_TIPO_ATTO = " + idTipoAtto;
			}
			updateString += 				
				", CHA_TIPO_PROTO = '" + tipoProto + "'" +
				", CHA_DA_PROTO = '" + daProtocollare + "'" +
				" WHERE DOCNUMBER = " + schedaDoc.docNumber;
				
			logger.Debug(updateString);
			db.executeLocked(updateString);*/
			#endregion
		}

		#region Metodo Commentato
		/*private static string calcolaChiaveProto(DocsPaVO.documento.SchedaDocumento schedaDocumento) 
		{
			// Il documento è protocollato
			if(schedaDocumento.protocollo != null) 
			{
				if(schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")) 
				{
					return "'" + schedaDocumento.protocollo.numero + "_" + schedaDocumento.protocollo.anno + "_" + schedaDocumento.registro.systemId + "'";
				}
			}
			return DateTime.Now.Ticks.ToString();
		
		}*/
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		public static void updateCollegamenti(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			#region Codice Commentato
			/*logger.Debug("updateCollegamenti");
			 string idRoot="";
			 try
			 {
				string selectString="SELECT ID_ROOT FROM DPA_DOC_COLLEGAMENTI WHERE ID_DOCUMENTO="+schedaDoc.protocollo.rispostaProtocollo.idProfile;
				logger.Debug(selectString);	
				 if(db.executeScalar(selectString)!=null)
				 {
					 idRoot=db.executeScalar(selectString).ToString();
				 }
				 else
				 {
					 idRoot=schedaDoc.protocollo.rispostaProtocollo.idProfile;
				 }
				 logger.Debug("idRoot="+idRoot);
				 string updateString="INSERT INTO DPA_DOC_COLLEGAMENTI (ID_DOCUMENTO,ID_DOC_COLLEGATO,ID_TIPO_COLLEGAMENTO,ID_ROOT) VALUES ("+schedaDoc.systemId+","+schedaDoc.protocollo.rispostaProtocollo.idProfile+",1,"+idRoot+")";
				 logger.Debug(updateString);
				 db.executeNonQuery(updateString);
			 }
			 catch(Exception e)
			 {
			   throw e;
			 }*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.UpdateCollegamenti(schedaDoc);
		}

		#region Metodo Commentato
		/*internal static void checkProto(DocsPaWS.Utils.Database db, DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			/*string queryStr =
				"SELECT NUM_PROTO FROM PROFILE WHERE DOCNUMBER=" + schedaDoc.docNumber;
			logger.Debug(queryStr);
			object numProto = db.executeScalar(queryStr);
			if(numProto != null && !numProto.ToString().Equals("") && !numProto.ToString().Equals(schedaDoc.protocollo.numero))
				throw new Exception("Tentativo di aggiornare il numero di protocollo fallito! Protocollo = " + numProto.ToString());
				*/
		/*
			DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
			obj.checkProto(schedaDoc);
		}*/
		#endregion
				
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="idProfile"></param>
		/// <param name="corrispondente"></param>
		/// <param name="tipoCorr"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		internal static string addDocArrivoPar(string idProfile, DocsPaVO.utente.Corrispondente corrispondente, string tipoCorr) 
		{
            //logger.Debug("addDocArrivoPar");

            //#region Codice Commentato
            ///*string sqlString;
            //sqlString = 
            //    "SELECT COUNT(*) FROM DPA_DOC_ARRIVO_PAR WHERE " +
            //    "ID_PROFILE=" + idProfile + " AND ID_MITT_DEST=" + 
            //    corrispondente.systemId + " AND CHA_TIPO_MITT_DEST='" + tipoCorr + "'";
            //logger.Debug(sqlString);
            //if(!db.executeScalar(sqlString).ToString().Equals("0"))
            //{
            //    return "";
            //}
            ////inserisco il corrispondente nella tabella DPA_DOC_ARRIVO_PAR
            //sqlString = 
            //    "INSERT INTO DPA_DOC_ARRIVO_PAR (" + DocsPaWS.Utils.dbControl.getSystemIdColName() +
            //    "ID_PROFILE, ID_MITT_DEST,CHA_TIPO_MITT_DEST) " +
            //    "VALUES (" + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_DOC_ARRIVO_PAR") + 
            //    idProfile + "," + corrispondente.systemId + ",'" + tipoCorr + "')";
            //logger.Debug(sqlString);						
            //String idDocArrivoPar = db.insertLocked(sqlString, "DPA_DOC_ARRIVO_PAR");
            //logger.Debug("idDocArrivoPar = " + idDocArrivoPar);*/
            //#endregion 

            //DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            //string idDocArrivoPar = doc.AddDocArrivoPar(idProfile, corrispondente, tipoCorr);

            //return idDocArrivoPar;
            return "";
		}

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="idProfile"></param>
		/// <param name="paroleChiave"></param>
		/// <param name="debug"></param>
		internal static void addParoleChiave(string idProfile, ArrayList paroleChiave) 
		{
			#region Codice Commentato
			/*logger.Debug("addParoleChiave");
			string sqlString = "";
			string idParola;
			if (paroleChiave == null)
				return;
			//inserisco la parole chiave nella tabella DPA_PROF_PAROLE
			for (int i = 0; i < paroleChiave.Count; i++) 
			{
				idParola = ((DocsPaVO.documento.ParolaChiave)paroleChiave[i]).systemId;
				sqlString = 
					"INSERT INTO DPA_PROF_PAROLE (SYSTEM_ID, ID_PROFILE, ID_PAROLA) " + 
					"VALUES ('1'," + idProfile + "," + idParola + ")";			
				logger.Debug(sqlString);						
				db.executeLocked(sqlString);
			}	
			*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.AddParolaChiave(idProfile, paroleChiave);
		}
		
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static DocsPaVO.documento.SchedaDocumento setDataUltimoProtocollo(DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			#region Codice Commentato
			/*logger.Debug("setDataUltimoProtocollo");
			string newDate = schedaDoc.protocollo.dataProtocollazione.Substring(0,10);
			logger.Debug("newDate="+newDate);
			string oldDate = "";
			if(schedaDoc.registro.dataUltimoProtocollo != null)
			{
				oldDate = schedaDoc.registro.dataUltimoProtocollo.Substring(0,10);
			}
			if(!oldDate.Equals(newDate)) 
			{
				string updateString =
					"UPDATE DPA_EL_REGISTRI SET DTA_ULTIMO_PROTO=" + 
					DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.dataProtocollazione,false) + 
					" WHERE SYSTEM_ID=" + schedaDoc.registro.systemId;				
				logger.Debug(updateString);
				db.executeNonQuery(updateString);
				schedaDoc.registro.dataUltimoProtocollo = schedaDoc.protocollo.dataProtocollazione;

			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.SetDataUltimoProtocollo(ref schedaDoc);

			return schedaDoc;
		}
        
		#region Metodo Commentato
		/*
		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="documento"></param>
		/// <param name="msg"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		internal static Exception throwException (DocsPaWS.Utils.Database db, DocsPaDocManagement.Documentale.Documento documento, string msg) 
		{
			logger.Debug(msg);
			db.rollbackTransaction();
			db.closeConnection();
			documento.Delete();
			logger.Debug("Risultato della delete su Fusion: " + documento.GetErrorCode());
			
			return new Exception(msg);
		}*/
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.RicercaDuplicati.EsitoRicercaDuplicatiEnum cercaDuplicati(DocsPaVO.documento.SchedaDocumento schedaDoc, string cercaDuplicati2, out DocsPaVO.documento.InfoProtocolloDuplicato[] datiProtDupl) 
		{			
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			
			return doc.CercaDuplicati(schedaDoc, cercaDuplicati2, out datiProtDupl);
		}

        /// <summary>
        /// metodo per l'inserimento nella tabella di collegamento tra mezzo di spedizione e documento
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <param name="idUtente"></param>
        /// <param name="idDocumentTypes"></param>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public static bool collegaMezzoSpedizioneDocumento(DocsPaVO.utente.InfoUtente info, string idDocumentTypes, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.collegaMezzoSpedizioneDocumento(info, idDocumentTypes, idProfile);
        }

        /// <summary>
        /// metodo per l'update nella tabella di collegamento tra mezzo di spedizione e documento
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <param name="idUtente"></param>
        /// <param name="idDocumentTypes"></param>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public static bool updateMezzoSpedizioneDocumento(DocsPaVO.utente.InfoUtente info, string oldDocumentTypes, string idDocumentTypes, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.updateMezzoSpedizioneDocumento(info, oldDocumentTypes, idDocumentTypes, idProfile);
        }

        /// <summary>
        /// metodo per la cancellazione nella tabella di collegamento tra mezzo di spedizione e documento
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <param name="idUtente"></param>
        /// <param name="idDocumentTypes"></param>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public static bool deleteMezzoSpedizioneDocumento(DocsPaVO.utente.InfoUtente info, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.deleteMezzoSpedizioneDocumento(info, idProfile);
        }

        /// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static bool cercaDuplicati(DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			#region Codice Commentato
			/*logger.Debug("cercaDuplicati");
			ArrayList lista = new ArrayList();

			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			db.openConnection();
			// gestione dei mittenti/destinatari
			if(schedaDoc.protocollo == null)
				return true;
			if (!schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
				return false;

			DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
			if(!(pe.dataProtocolloMittente != null && !pe.dataProtocolloMittente.Equals("")))
				return false;

			string queryStr =
				"SELECT COUNT(*) FROM PROFILE WHERE SYSTEM_ID IN " +
				"(SELECT ID_PROFILE FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST IN (" + 
				getIdDestinatario(pe.mittente) +
				") AND CHA_TIPO_MITT_DEST IN ('M', 'D')) AND CHA_TIPO_PROTO = 'A' " +
				"AND DTA_PROTO_IN = " + DocsPaWS.Utils.dbControl.toDate(pe.dataProtocolloMittente, false) +
				" AND VAR_PROTO_IN = '";
			if(pe.descrizioneProtocolloMittente != null)
				queryStr += pe.descrizioneProtocolloMittente.Replace("'","''");
			queryStr += "'";
			if(schedaDoc.systemId != null && !schedaDoc.systemId.Equals(""))
				queryStr += " AND SYSTEM_ID != " + schedaDoc.systemId;

			logger.Debug(queryStr);
			string numRes = db.executeScalar(queryStr).ToString();
			db.closeConnection();
			if (numRes.Equals("0"))
				return false;
			else
				return true;*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			
			return doc.CercaDuplicati(schedaDoc);
		}

		/// <summary>
		/// Verifica se l'amministrazione passata come parametro ha la protocollazione interna
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public static bool IsProtoIntEnabled(string idAmm)
		{
			bool result = false;
			DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
			
			if(obj.IsEnabledProtoInt(idAmm)) result = true;

			return result;
		}

        public static bool isDocAnnullato(string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            return doc.isDocAnnullato(docNumber);
        }

        public static DocsPaVO.documento.SchedaDocumento ricercaSchedaByTipoDoc(string numproto, string anno, string codreg, string tipodoc, DocsPaVO.utente.InfoUtente infout)
        {
            DocsPaVO.documento.SchedaDocumento sch = null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();

            //ruolo
            DocsPaVO.utente.Ruolo ruolo = utente.GetRuolo(infout.idCorrGlobali);

            //verifica registro 
            if (ruolo != null)
            {
                for (int i = 0; i < ruolo.registri.Count; i++)
                {
                    if (((DocsPaVO.utente.Registro)ruolo.registri[i]).codRegistro == codreg)
                    {
                        sch = doc.ricercaProtoByTipoDoc(numproto, anno, ((DocsPaVO.utente.Registro)ruolo.registri[i]).systemId, tipodoc, infout);
                    }
                    else
                    {
                        logger.Debug("ricercaSchedaByTipoDoc - Ruolo: " + ruolo.descrizione + " non abilitato al registro: " + ((DocsPaVO.utente.Registro)ruolo.registri[i]).codRegistro);
                    }
                }
            }

            if (sch == null)
            {
                logger.Debug("ricercaSchedaByTipoDoc - Protocollo non trovato.");
            }

            return sch;
        }

		#region Codice Commentato
		/*
		private static string getIdDestinatario(DocsPaVO.utente.Corrispondente corr) 
		{
			if (corr.systemId != null && ! corr.systemId.Equals(""))
				return corr.systemId;
			else 
			{
				string queryStr = 
					"SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE VAR_DESC_CORR='" +
					corr.descrizione.Replace("'", "''") + "'";
				return queryStr;
			}
		}*/
		#endregion

        public static string getDataOraProtocollo(string idProto)
        {
            string retValue = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile0");

            queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("DTA_PROTO",true));
            queryDef.setParam("param2", "SYSTEM_ID = " + idProto);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            dbProvider.ExecuteScalar(out retValue, commandText);
			
            return retValue;
        }

        public static bool IsDocAnnullatoByIdProfile(string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            return doc.IsDocAnnullatoByIdProfile(idProfile);
        }

        public static void updateStampeEffettuate(DocsPaVO.utente.InfoUtente infoUtente, string numStampe, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.UpdateStampeEffettuateProfile(infoUtente, numStampe, idProfile);
        }

        /// <summary>
        /// Metodo per il recupero della segnatura di un documento a partire dal suo ID
        /// </summary>
        /// <param name="idProfile">Id del documento</param>
        /// <returns>Segnatura</returns>
        public static String GetDocumentSignatureByProfileId(String idProfile)
        {
            String retVal = String.Empty;
            using (DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                retVal = docs.GetDocumentSignatureByProfileId(idProfile);
            }

            return retVal;
        }

        public static bool DeleteProtocollo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool success = false;
            using (DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                success = docs.DeleteProtocollo(infoUtente, schedaDocumento);
            }
            return success;
        }
	}
}
