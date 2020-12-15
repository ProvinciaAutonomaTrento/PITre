using System;
using System.Data;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.amministrazione;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Collections;
using System.Linq;
using BusinessLogic.Import;
using log4net;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// Classe che fornisce servizi per la gestione di un nodo di titolario
	/// </summary>
	public class TitolarioManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(TitolarioManager));
		#region Public members

		public TitolarioManager()
		{

		}

        public static string AmmGetVisibilitaNodoTit_InRuolo(string idNodoTitolario, string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.GetVisibilitaNodoTit_InRuolo(idNodoTitolario, idGruppo);
        }

        public static System.Collections.ArrayList GetNodiTitolario_InRuolo(string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            DataSet ds = amm.GetDsNodiTitolario_inRuolo(idGruppo);

            return CreateListNodiTitolario_InRuolo(ds.Tables["NODI_TITOLARIO"]);
        }

		/// <summary>
		/// Reperimento dei nodi di titolario relativamente
		/// ad un'amministrazione e ad un titolario padre (0 se root)
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <param name="idNodoTitolarioParent"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList GetNodiTitolario(string codiceAmministrazione,string idNodoTitolarioParent,string idRegistro)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DataSet ds = amm.GetDsNodiTitolario(amm.GetIDAmm(codiceAmministrazione),idNodoTitolarioParent,idRegistro);
            ArrayList nodes = CreateListNodiTitolario(codiceAmministrazione,ds.Tables["NODI_TITOLARIO"]);

            string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
            if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                amm.GetListaTemplateStruttura(ref nodes);

            return nodes;
		}

		/// <summary>
		/// Reperimento di tutti i ruoli che hanno, tramite il registro,
		/// la visibilità su un nodo di titolario.
		/// Se non viene fornito l'idRegistro, verranno ricercati i 
		/// ruoli in base a tutti i registri presenti nell'amministrazione richiesta.
        /// Possibilità di filtraggio tramite codice ruolo (tipo ricerca = COD_RUOLO)
        /// o tramite la descrizione (tiporicerca= DES_RUOLO)
		/// </summary>
		/// <param name="idNodoTitolario"></param>
		/// <param name="idRegistro"></param>
		/// <returns></returns>
        public static System.Collections.ArrayList GetRuoliInTitolario(string idNodoTitolario, string idRegistro, string codiceRicerca, string tipoRicerca)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = amm.GetDsRuoliTitolario(idNodoTitolario, idRegistro, codiceRicerca, tipoRicerca);
			amm=null;

			return CreateListRuoliInTitolario(ds.Tables["RUOLI_TITOLARIO"]);
		}

		/// <summary>
		/// Aggiornamento dati di un nodo di titolario
		/// </summary>
        /// <param name="infoUtente"></param>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione UpdateNodoTitolario(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
		{
            EsitoOperazione retValue = null;

            // Creazione di un nuovo contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                // Verifica dei vincoli per l'aggiornamento dei dati del nodo di titolario
                retValue = CanUpdateTitolario(nodoTitolario);

                if (retValue.Codice == 0)
                {
                    DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

                    if (!titolarioManager.SaveNodoTitolario(nodoTitolario))
                    {
                        retValue.Codice = 1;
                        retValue.Descrizione = "Errore nella modifica dei dati del titolario";
                    }
                    else
                    {
                        string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                        if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                            new DocsPaDB.Query_DocsPAWS.Amministrazione().UpdateRelProjectStructure(nodoTitolario);

                        // Transazione completata con successo
                        transactionContext.Complete();
                    }
                }
            }

			return retValue;
		}

        /// <summary>
        /// Aggiornamento della visibilità dei ruoli su un nodo di titolario
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <param name="bypassCheck">
        /// Se true, indica di non effettuare i controlli nella rimozione dei nodi titolario
        /// </param>
        /// <returns></returns>
        private static DocsPaVO.amministrazione.EsitoOperazione[] UpdateRuoliNodoTitolario(
                                                DocsPaVO.utente.InfoUtente infoUtente,
                                                DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario,
                                                DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario,
                                                bool bypassCheck)
        {
            List<EsitoOperazione> list = new List<EsitoOperazione>();

            DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

            List<OrgRuoloTitolario> ruoliDaAggiornare = new List<OrgRuoloTitolario>();

            if (!bypassCheck)
            {
                EsitoOperazione esito = null;

                foreach (DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario in ruoliTitolario)
                {
                    // Verifica se il ruolo del titolario può essere aggiornato
                    esito = CanUpdateRuoloTitolario(nodoTitolario.ID, ruoloTitolario);

                    if (esito.Codice == 0)
                        // La visibilità del ruolo verso il nodo titolario può essere estesa
                        ruoliDaAggiornare.Add(ruoloTitolario);
                    else
                        list.Add(esito);
                }

                if (ruoliDaAggiornare.Count > 0)
                    list.AddRange(titolarioManager.SetAclNodoTitolario(nodoTitolario, ruoliDaAggiornare.ToArray()));
            }
            else
            {
                list.AddRange(titolarioManager.SetAclNodoTitolario(nodoTitolario, ruoliTitolario));
            }


            return list.ToArray();
        }

        /// <summary>
        /// Aggiornamento della visibilità dei ruoli su un nodo di titolario
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <param name="bypassCheck"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione[] UpdateRuoliNodoTitolario(DocsPaVO.utente.InfoUtente infoUtente,
                                                        string idTitolario,
                                                        DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario,
                                                        bool bypassCheck)
        {
            OrgNodoTitolario nodoTitolario = null;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                nodoTitolario = amm.getNodoTitolario(idTitolario);

            return UpdateRuoliNodoTitolario(infoUtente, nodoTitolario, ruoliTitolario, bypassCheck);
        }

		/// <summary>
		/// Aggiornamento ruoli che hanno la visibilità sul titolario
		/// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idTitolario"></param>
		/// <param name="ruoliTitolario"></param>
		/// <returns></returns>
		public static DocsPaVO.amministrazione.EsitoOperazione[] UpdateRuoliTitolario(
                                DocsPaVO.utente.InfoUtente infoUtente,
								string idTitolario,
								DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario,
                                DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolarioDisattiva,
                                bool allTitolario,
                                string idAmm,
                                string idRegistro)
		{
            List<EsitoOperazione> list = new List<EsitoOperazione>();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

                if (allTitolario && ruoliTitolario != null && ruoliTitolario.Length > 0)
                {
                    // nel caso di richiesta di estensione della visibilità a tutto il 
                    // titolario per un determinato ruolo, forzo a true il param Associato
                    // per forzare l'inserimento in security

                    // sono nel caso in cui si è selezionato tutto il titolario
                    ArrayList nodiTitolario = null;
                    if (!string.IsNullOrEmpty(idRegistro))
                        nodiTitolario = amm.getNodiTitolario(idTitolario, idRegistro);
                    else
                        nodiTitolario = amm.getNodiTitolario(idTitolario);

                    foreach (OrgNodoTitolario nodo in nodiTitolario)
                    {
                        // nel caso di richiesta di estensione della visibilità a tutto il 
                        // titolario per un determinato ruolo, forzo a true il param Associato
                        // per forzare l'inserimento in security
                        foreach (OrgRuoloTitolario ruolo in ruoliTitolario)
                            ruolo.Associato = true;

                        list.AddRange(UpdateRuoliNodoTitolario(infoUtente, nodo, ruoliTitolario, true));
                    }
                }
                else if (allTitolario && ruoliTitolarioDisattiva != null && ruoliTitolarioDisattiva.Length > 0)
                {
                    // questo è il caso in cui è stato richiesta la cancellazione della visibilità
                    // di un intero titolario per un gruppo di ruoli

                    // sono nel caso in cui si è selezionato tutto il titolario
                    ArrayList nodiTitolario = null;
                    if (!string.IsNullOrEmpty(idRegistro))
                        nodiTitolario = amm.getNodiTitolario(idTitolario, idRegistro);
                    else
                        nodiTitolario = amm.getNodiTitolario(idTitolario);

                    foreach (OrgNodoTitolario nodo in nodiTitolario)
                    {
                        // nel caso di richiesta di estensione della visibilità a tutto il 
                        // titolario per un determinato ruolo, forzo a true il param Associato
                        // per forzare l'inserimento in security
                        foreach (OrgRuoloTitolario ruolo in ruoliTitolarioDisattiva)
                            ruolo.Associato = false;

                        list.AddRange(UpdateRuoliNodoTitolario(infoUtente, nodo, ruoliTitolarioDisattiva, true));
                    }


                    //// questo è il caso in cui è stato richiesta la cancellazione della visibilità
                    //// di un intero titolario per un gruppo di ruoli
                    //foreach (DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolarioDisattiva in ruoliTitolarioDisattiva)
                    //{
                    //    DataSet ds = amm.GetDsNodiTitolario(idAmm, idTitolario, idRegistro);
                    //    InfoAmministrazione infAmm = amm.AmmGetInfoAmmCorrente(idAmm);
                    //    System.Collections.ArrayList listaNodiFigli = CreateListNodiTitolario(infAmm.Codice, ds.Tables["NODI_TITOLARIO"]);
                        
                    //    ruoloTitolarioDisattiva.Associato = false;
                    //    if (listaNodiFigli != null && listaNodiFigli.Count > 0)
                    //    {
                    //        foreach (DocsPaVO.amministrazione.OrgNodoTitolario currentNode in listaNodiFigli)
                    //        {
                    //            list = updateCiclicoTitolarioManager(titolarioManager, currentNode, ruoloTitolarioDisattiva, idRegistro, list, idAmm);
                    //        }
                    //        amm.deleteVisibilitaTitolario(idTitolario, ruoloTitolarioDisattiva.ID);
                    //    }
                    //}
                }
                else
                {
                    List<OrgRuoloTitolario> ruoliDaAggiornare = new List<OrgRuoloTitolario>();

                    EsitoOperazione esito = null;

                    if(ruoliTitolario != null)
                        foreach (DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario in ruoliTitolario)
                        {
                            // Verifica se il ruolo del titolario può essere aggiornato
                            esito = CanUpdateRuoloTitolario(idTitolario, ruoloTitolario);

                            if (esito.Codice == 0)
                                // La visibilità del ruolo verso il nodo titolario può essere estesa
                                ruoliDaAggiornare.Add(ruoloTitolario);
                            else
                                list.Add(esito);
                        }

                    if (ruoliTitolarioDisattiva != null)
                        foreach (DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario in ruoliTitolarioDisattiva)
                        {
                            // Verifica se il ruolo del titolario può essere aggiornato
                            esito = CanUpdateRuoloTitolario(idTitolario, ruoloTitolario);

                            if (esito.Codice == 0)
                                // La visibilità del ruolo verso il nodo titolario può essere estesa
                                ruoliDaAggiornare.Add(ruoloTitolario);
                            else
                                list.Add(esito);
                        }

                    if (ruoliDaAggiornare.Count > 0)
                        list.AddRange(UpdateRuoliNodoTitolario(infoUtente, idTitolario, ruoliDaAggiornare.ToArray(), false));
                }

                transactionContext.Complete();
            }

            return list.ToArray();

                //    foreach (DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario in ruoliTitolario)
                //    {
                //        if (allTitolario)
                //        {
                //            // sono nel caso in cui si è selezionato tutto il titolario
                //            ArrayList nodiTitolario = amm.getNodiTitolario(idTitolario);

                //            foreach (OrgNodoTitolario nodo in nodiTitolario)
                //            {
                //                // nel caso di richiesta di estensione della visibilità a tutto il 
                //                // titolario per un determinato ruolo, forzo a true il param Associato
                //                // per forzare l'inserimento in security
                //                ruoloTitolario.Associato = true;

                //                if (!titolarioManager.SetAclRuoloNodoTitolario(nodo, ruoloTitolario))
                //                {
                //                    esito = new EsitoOperazione();
                //                    esito.Codice = Convert.ToInt32(ruoloTitolario.ID);
                //                    esito.Descrizione = "Aggiornamento del ruolo con codice " + ruoloTitolario.Codice + " non andato a buon fine per il nodo " + nodo.Codice;
                //                }
                //                else
                //                    modificaEffettuata = true;

                //                if (esito != null)
                //                {
                //                    list.Add(esito);
                //                    esito = null;
                //                }
                //            }
                //        }
                //        else
                //        {   
                //            // Verifica se il ruolo del titolario può essere aggiornato
                //            esito = CanUpdateRuoloTitolario(idTitolario, ruoloTitolario);

                //            if (esito.Codice == 0)
                //                // La visibilità del ruolo verso il nodo titolario può essere estesa
                //                ruoliDaAggiornare.Add(ruoloTitolario);
                //            else
                //                list.Add(esito);
                //        }
                //    }

                //    list.AddRange(titolarioManager.SetAclNodoTitolario(amm.getNodoTitolario(idTitolario), ruoliDaAggiornare.ToArray()));

                //}


                //// questo è il caso in cui è stato richiesta la cancellazione della visibilità
                //// di un intero titolario per un gruppo di ruoli
                //if (allTitolario && ruoliTitolarioDisattiva != null && ruoliTitolarioDisattiva.Length > 0)
                //{
                //    foreach (DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolarioDisattiva in ruoliTitolarioDisattiva)
                //    {
                //        DataSet ds = amm.GetDsNodiTitolario(idAmm, idTitolario, idRegistro);
                //        InfoAmministrazione infAmm = amm.AmmGetInfoAmmCorrente(idAmm);
                //        System.Collections.ArrayList listaNodiFigli = CreateListNodiTitolario(infAmm.Codice, ds.Tables["NODI_TITOLARIO"]);
                //        DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                //        ruoloTitolarioDisattiva.Associato = false;
                //        if (listaNodiFigli != null && listaNodiFigli.Count > 0)
                //        {
                //            foreach (DocsPaVO.amministrazione.OrgNodoTitolario currentNode in listaNodiFigli)
                //            {
                //                list = updateCiclicoTitolarioManager(titolarioManager, currentNode, ruoloTitolarioDisattiva, idRegistro, list, idAmm);
                //            }
                //            amm.deleteVisibilitaTitolario(idTitolario, ruoloTitolarioDisattiva.ID);
                //            modificaEffettuata = true;
                //        }
                //    }
                //}

                //if (modificaEffettuata)
                    //transactionContext.Complete();

            
		}

        ///// <summary>
        ///// funzione per effettuare l'update ciclico su Titolario Manager
        ///// </summary>
        ///// <returns></returns>
        //public static List<EsitoOperazione> updateCiclicoTitolarioManager(
        //                                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager,
        //                                DocsPaVO.amministrazione.OrgNodoTitolario currentNode,
        //                                DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario,
        //                                string idRegistro,
        //                                List<EsitoOperazione> list,
        //                                string idAmm)
        //{
        //    EsitoOperazione esito = new EsitoOperazione();

        //    if (!titolarioManager.SetAclRuoloNodoTitolario(currentNode, ruoloTitolario))
        //    {
        //        esito.Codice = Convert.ToInt32(ruoloTitolario.ID);;
        //        esito.Descrizione = "Errore nella modifica visibilità del nodo " + currentNode.Codice + " al ruolo " + ruoloTitolario.Codice;
        //    }
        //    else
        //    {
        //        esito.Codice = 0;
        //        esito.Descrizione = "Visibilità aggiornata per il ruolo " + ruoloTitolario.Codice + " sul nodo " + currentNode.Codice;
        //    }
        //    list.Add(esito);

        //    if (currentNode.CountChildNodiTitolario > 0)
        //    {
        //        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                
        //        DataSet ds = amm.GetDsNodiTitolario(idAmm, currentNode.ID, idRegistro);
        //        InfoAmministrazione inAmm = amm.AmmGetInfoAmmCorrente(idAmm);
        //        System.Collections.ArrayList listaNodiFigli = CreateListNodiTitolario(inAmm.Codice, ds.Tables["NODI_TITOLARIO"]);
        //        if (listaNodiFigli != null && listaNodiFigli.Count > 0)
        //        {
        //            foreach (DocsPaVO.amministrazione.OrgNodoTitolario currentNodeFiglio in listaNodiFigli)
        //            {
        //                list = updateCiclicoTitolarioManager(titolarioManager, currentNodeFiglio, ruoloTitolario, idRegistro, list, idAmm);
        //            }
        //        }
        //    }

        //    return list;
        //}

		/// <summary>
		/// Verifica se un ruolo può essere associato ad un titolario
		/// in base ai seguenti vincoli:
		///
		/// - non è possibile associare la visibilità ad un ruolo se, su di esso,
		///   non ha la visibilità il nodo di titolario padre (se presente);
		/// - la visibilità ad un ruolo può essere rimossa solamente se non esistono
		///   fascicoli procedimentali visibili ad esso;
		///   
		/// </summary>
		/// <param name="ruoliTitolario"></param>
		/// <returns></returns>
		public static DocsPaVO.amministrazione.EsitoOperazione CanUpdateRuoloTitolario(
									string idTitolario,
									DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario)
		{
            OrgNodoTitolario nodoTitolario = null;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione ammDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                nodoTitolario = ammDb.getNodoTitolario(idTitolario);

			EsitoOperazione retValue=new EsitoOperazione();

            List<string> descriptions = new List<string>();

			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			
			if (ruoloTitolario.Associato)
			{			
				// Si può impostare la visibilità di un ruolo ad un nodo
				// solamente se il ruolo stesso è già associato al nodo titolario padre
				if (!amm.ExistRuoloInTitolarioPadre(idTitolario,ruoloTitolario.ID))
				{
					retValue.Codice=Convert.ToInt32(ruoloTitolario.ID);
                    descriptions.Add("Non può essere associato al titolario perché non è associato al titolario padre");
				}
			}
			else
			{
				// La visibilità di un ruolo può essere rimossa da un nodo solamente se
				// il ruolo stesso non è associato anche ad uno o più nodi titolario figli
				if (amm.ExistRuoloInTitolarioFiglio(idTitolario,ruoloTitolario.ID))
				{
					retValue.Codice=Convert.ToInt32(ruoloTitolario.ID);
					descriptions.Add("Non può essere rimosso perché presente in almeno un titolario figlio");
				}

				// Verifica se il ruolo è proprietario di almeno un documento
				if (amm.IsRuoloOwnerDocumenti(idTitolario,ruoloTitolario.ID))
				{
					retValue.Codice=Convert.ToInt32(ruoloTitolario.ID);
					descriptions.Add("Non può essere rimosso dal titolario perché è il creatore di almeno un documento");
				}

				// Si può eliminare la visibilità di un ruolo ad un nodo 
				// solamente se, per quel nodo, non esistono sottofascicoli 
				// creati nell'ambito del fascicolo generale
				if (amm.IsRuoloOwnerSottoFascicoliGenerale(idTitolario,ruoloTitolario.ID))
				{
					retValue.Codice=Convert.ToInt32(ruoloTitolario.ID);
					descriptions.Add("Non può essere rimosso dal titolario perché è il creatore di almeno un sottofascicolo nel fascicolo generale");
				}

				// Si può rimuovere la visibilità di un ruolo ad un nodo 
				// solamente se, per quel nodo, non esistono fascicoli 
				// procedimentali creati dal ruolo
				if (amm.IsRuoloOwnerFascicoloProcedimentale(idTitolario,ruoloTitolario.ID))
				{
					retValue.Codice=Convert.ToInt32(ruoloTitolario.ID);
					descriptions.Add("Non può essere rimosso dal titolario perché è il creatore di almeno un fascicolo procedimentale");
				}
			}

            if (retValue.Codice != 0)
            {
                string errorDescription = string.Empty;
                foreach (string item in descriptions)
                {
                    if (errorDescription != string.Empty)
                        errorDescription += Environment.NewLine;
                    errorDescription += " - " + item;
                }

                retValue.Descrizione = string.Format("Ruolo '{0}' in titolario '{1}': {2}{3}", 
                                    ruoloTitolario.Codice, 
                                    nodoTitolario.Codice,
                                    Environment.NewLine,
                                    errorDescription);
            }

			return retValue;
		}

		/// <summary>
		/// Verifica se un nodo di titolario può essere aggiornato in base ai seguenti vincoli:
		/// 
		/// - è possibile modificare l'associazione che il nodo ha con un solo registro
		///   a quella "Tutti i registri", non il contrario;
		/// - non è possibile modificare il codice di un nodo esistente;
		/// - è possibile modificare la descrizione del nodo solo se:
		///		1) non ha documenti
		///		2) non ha fascicoli procedimentali
		///	
		/// </summary>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
		public static DocsPaVO.amministrazione.EsitoOperazione CanUpdateTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
		{
			EsitoOperazione retValue=new EsitoOperazione();
			
			System.Collections.ArrayList listErrorDescription=new System.Collections.ArrayList();

			string errorDescription=string.Empty;
			
			// Verifica se i campi obbligatori del titolario sono stati valorizzati
			if (!IsValidRequiredFieldsTitolario(
												DBActionTypeTitolarioEnum.UpdateMode,
												nodoTitolario,
												out errorDescription))
			{
				retValue.Codice=-1;
				listErrorDescription.Add(errorDescription);
			}
			else
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();

				DataSet infoNodoTitolario=amm.GetDsNodoTitolarioWithChilds(nodoTitolario.ID,nodoTitolario.IDParentNodoTitolario);

				DataView viewTitolario=infoNodoTitolario.Tables["TABLE_TITOLARIO"].DefaultView;
				
				viewTitolario.RowFilter="SYSTEM_ID = " + nodoTitolario.ID;

				if (viewTitolario.Count>0)
				{
					DataRow rowTitolario=viewTitolario[0].Row;

					// Verifica del vincolo sul registro del titolario (da tutti a 1)
					if (nodoTitolario.IDRegistroAssociato!=string.Empty &&
						rowTitolario["ID_REGISTRO"]==DBNull.Value)
					{
						retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
						listErrorDescription.Add(" - Non è possibile associare un registro se il nodo è già comune a tutti i registri");
					}
					
					// Verifica del vincolo sul registro del titolario (da 1 a tutti)
					if (
						(nodoTitolario.IDRegistroAssociato==string.Empty || nodoTitolario.IDRegistroAssociato==null || nodoTitolario.IDRegistroAssociato.Equals("")) &&
						rowTitolario["ID_REGISTRO"]!=DBNull.Value)
					{
						// Verifica se non esiste già un altro nodo con lo stesso codice 
						// su un altro registro della stessa amministrazione
						if(CheckCodiciDuplicatiTitolario(nodoTitolario.ID,nodoTitolario.Codice))
						{
							retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
							listErrorDescription.Add(" - Non è possibile modificare il registro: esiste già il codice del nodo su un altro registro");
						}
					}

//					if (nodoTitolario.IDRegistroAssociato!=string.Empty &&
//							 nodoTitolario.IDRegistroAssociato!=rowTitolario["ID_REGISTRO"].ToString())
//					{
//						// Verifica se per il titolario siano già stati associati dei ruoli. Nel caso in cui
//						// si passa da registro singolo a tutti i registri, non 
//						// è necessario effettuare questo controllo.
//						if (GetCountRuoliAssociatiInTitolario(nodoTitolario.ID) > 0)
//						{
//							retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
//							listErrorDescription.Add(" - Non è possibile modificare il registro del titolario: sono presenti ruoli già associati");
//						}
//					}

					// Verifica vincolo sulla modifica del codice titolario
					if (rowTitolario["VAR_CODICE"].ToString()!=nodoTitolario.Codice)
					{
						retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
						listErrorDescription.Add(" - Non è possibile modificare il codice del titolario");
					}

					// Verifica vincolo modifica descrizione del titolario
					// in base alla presenza di documenti e di fascicoli procedimentali
					if (rowTitolario["DESCRIPTION"].ToString()!=nodoTitolario.Descrizione)
					{
						// Verifica presenza di almeno un documento
						if (amm.GetCountDocumentiInNodoTitolario(nodoTitolario.ID) > 0)
						{
							retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
							listErrorDescription.Add(" - Descrizione non modificabile: sono presenti documenti");
						}

						// Verifica presenza di fascicoli procedimentali associati
						if (amm.GetListIDFascicoliProcedimentaliTitolario(nodoTitolario.ID).Count>0)
						{
							retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
							listErrorDescription.Add(" - Descrizione non modificabile: sono presenti fascicoli procedimentali");
						}
					}

					// Verifica del vincolo sul registro del titolario padre
					viewTitolario.RowFilter=string.Empty;
					viewTitolario.RowFilter="SYSTEM_ID = " + nodoTitolario.IDParentNodoTitolario;
					
					bool isValidRegistroAssociato=true;
					string outParam=string.Empty;

					if (viewTitolario.Count>0)
					{
						isValidRegistroAssociato=IsValidRegistroAssociato(
															nodoTitolario.IDRegistroAssociato,
															viewTitolario[0].Row["ID_REGISTRO"].ToString(),
															out outParam);
					}

					if (!isValidRegistroAssociato)
					{
						listErrorDescription.Add(outParam);
					}
					else
					{
						// Verifica del vincolo sul registro dei titolari figli
						viewTitolario.RowFilter=string.Empty;
						viewTitolario.RowFilter="ID_PARENT = " + nodoTitolario.ID + " AND CHA_TIPO_PROJ='T'";
					
						foreach (DataRowView rowView in viewTitolario)
						{
							isValidRegistroAssociato=IsValidRegistroAssociato(
														rowView.Row["ID_REGISTRO"].ToString(),
														nodoTitolario.IDRegistroAssociato,
														out outParam);

							if (!isValidRegistroAssociato)
							{
								listErrorDescription.Add(outParam);
								break;
							}
						}
					}

					// Verifica vincolo mesi conservazione:
					// il nuovo valore non può essere inferiore al valore esistente
					if (rowTitolario["NUM_MESI_CONSERVAZIONE"]!=DBNull.Value &&
                        nodoTitolario.NumeroMesiConservazione < Convert.ToInt32(rowTitolario["NUM_MESI_CONSERVAZIONE"]))
					{
						retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
						listErrorDescription.Add(" - Il numero dei mesi di conservazione non può essere inferiore a " + rowTitolario["NUM_MESI_CONSERVAZIONE"].ToString());
					}

                    //Protocollo Titolario
                    //Verifico se non esiste già un nodo con lo stesso protocollo di titolario
                    //per amministrazione, eventuale registro e titolario
                    if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"] != null && System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"].ToUpper() != "")
                    {
                        if (!string.IsNullOrEmpty(nodoTitolario.numProtoTit) && amm.existProtocolloTit(nodoTitolario))
                        {
                            retValue.Codice = -1;
                            listErrorDescription.Add(" - Protocollo Titolario " + nodoTitolario.numProtoTit + " già presente.");
                        }
                    }   

					rowTitolario=null;
				}
				else
				{
					// Nodo di titolario non trovato
					retValue.Codice=Convert.ToInt32(nodoTitolario.ID);
					listErrorDescription.Add(" - Nessun titolario presente per l'ID: " + nodoTitolario.ID);
				}

				amm=null;
			}

			if (retValue.Codice!=0)
			{
				errorDescription=string.Empty;
				foreach (string item in listErrorDescription)
				{
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";

					errorDescription += item;
				}
				
				retValue.Descrizione=errorDescription;
			}

			return retValue;
		}

		private enum DBActionTypeTitolarioEnum
		{
			InsertMode,
			UpdateMode,
			DeleteMode
		}

		/// <summary>
		/// Validazione dati obbligatori per l'oggetto nodo titolario
		/// </summary>
		/// <param name="actionType"></param>
		/// <param name="nodoTitolario"></param>
		/// <param name="errorDescription"></param>
		/// <returns></returns>
		private static bool IsValidRequiredFieldsTitolario(
										DBActionTypeTitolarioEnum actionType,
										DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario,
										out string errorDescription)
		{
			bool retValue=true;
			errorDescription=string.Empty;

			if (actionType!=DBActionTypeTitolarioEnum.InsertMode &&
					(nodoTitolario.ID==null ||
					nodoTitolario.ID==string.Empty || 
					nodoTitolario.ID=="0"))
			{
				retValue=false;
				if (errorDescription!=string.Empty)
					errorDescription += "\\n";
				errorDescription += " - ID mancante";
			}
			
			if (actionType!=DBActionTypeTitolarioEnum.DeleteMode)
			{
				if (nodoTitolario.Codice==null || nodoTitolario.Codice==string.Empty)
				{
					retValue=false;
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";
					errorDescription += " - Codice mancante";
				}

				if (nodoTitolario.Descrizione==null || nodoTitolario.Descrizione==string.Empty)
				{
					retValue=false;
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";
					errorDescription += " - Descrizione mancante";
				}

				if (nodoTitolario.CodiceAmministrazione==null || nodoTitolario.CodiceAmministrazione==string.Empty)
				{
					retValue=false;
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";
					errorDescription += " - Codice amministrazione mancante";
				}

				if (nodoTitolario.CodiceLivello==null || nodoTitolario.CodiceLivello==string.Empty)
				{
					retValue=false;
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";
					errorDescription += " - Codice livello mancante";
				}

				if (nodoTitolario.Livello==null || nodoTitolario.Livello==string.Empty)
				{
					retValue=false;
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";
					errorDescription += " - Livello mancante";
				}
			}

			return retValue;
		}

		/// <summary>
		/// Verifica dei vincoli per la cancellazione di un nodo di titolario,
		/// che può essere rimosso solamente se:
		/// 
		/// - non ha nodi titolario figli
		/// - non ha documenti
		/// - non ha fascicoli procedimentali
		/// </summary>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
		public static DocsPaVO.amministrazione.EsitoOperazione CanDeleteTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
		{
			EsitoOperazione retValue=new EsitoOperazione();

			string errorDescription=string.Empty;

			// Verifica se i campi obbligatori del titolario sono stati valorizzati
			if (!IsValidRequiredFieldsTitolario(
								DBActionTypeTitolarioEnum.DeleteMode,
								nodoTitolario,
								out errorDescription))
			{
				retValue.Codice=-1;
			}
			else
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();

				DataSet infoNodoTitolario=amm.GetDsNodoTitolarioWithChilds(nodoTitolario.ID,nodoTitolario.IDParentNodoTitolario);
				
				DataView viewTitolario=infoNodoTitolario.Tables["TABLE_TITOLARIO"].DefaultView;
				
				// Verifica se il titolario ha almeno un titolario figlio
				viewTitolario.RowFilter="ID_PARENT = " + nodoTitolario.ID + " AND CHA_TIPO_PROJ='T'";
					
				if (viewTitolario.Count>0)
				{
					// Il titolario ha almeno un titolario figlio
					retValue.Codice=Convert.ToInt32(nodoTitolario.ID);

					if (errorDescription!=string.Empty)
						errorDescription += "\\n";

					errorDescription += " - Nel titolario con codice " + nodoTitolario.Codice + " sono presenti titolari figli";
				}

				// Verifica presenza di sottofascicoli nel fascicolo generale (root folder)
				if (amm.ExistSottoFascicoliInGenerale(nodoTitolario.ID))
				{
					retValue.Codice=Convert.ToInt32(nodoTitolario.ID);

					if (errorDescription!=string.Empty)
						errorDescription += "\\n";

					errorDescription += " - Nel titolario con codice " + nodoTitolario.Codice + " è presente almeno un sottofascicolo nel fascicolo generale";
				}

				// Verifica presenza di fascicoli procedimentali associati
				if (amm.GetListIDFascicoliProcedimentaliTitolario(nodoTitolario.ID).Count>0)
				{
					retValue.Codice=Convert.ToInt32(nodoTitolario.ID);

					if (errorDescription!=string.Empty)
						errorDescription += "\\n";

					errorDescription += " - Nel titolario con codice " + nodoTitolario.Codice + " è presente almeno un fascicolo procedimentale";
				}

				// Verifica se il titolario ha almeno un documento
				if (amm.GetCountDocumentiInNodoTitolario(nodoTitolario.ID) > 0)
				{
					// Il titolario ha almeno un documento
					retValue.Codice=Convert.ToInt32(nodoTitolario.ID);

					if (errorDescription!=string.Empty)
						errorDescription += "\\n";

					errorDescription += " - Nel titolario con codice " + nodoTitolario.Codice + " sono presenti documenti";
				}

				amm=null;
			}

			if (retValue.Codice!=0)
				retValue.Descrizione=errorDescription;

			return retValue;
		}

		/// <summary>
		/// Rimozione di un nodo titolario
		/// </summary>
        /// <param name="infoUtente"></param>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
        public static EsitoOperazione DeleteNodoTitolario(DocsPaVO.utente.InfoUtente infoUtente, OrgNodoTitolario nodoTitolario)
		{
            EsitoOperazione retValue = null;

            // Creazione di un nuovo contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = CanDeleteTitolario(nodoTitolario);

                // Verifica se i vincoli per la cancellazione del titolario sono rispettati
                if (retValue.Codice == 0)
                {
                    DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

                    if (!titolarioManager.DeleteNodoTitolario(nodoTitolario))
                    {
                        retValue.Codice = -1;
                        retValue.Descrizione = "Errore nella cancellazione del nodo di titolario";
                    }
                    else
                    {
                        string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                        if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                            new DocsPaDB.Query_DocsPAWS.Amministrazione().DeleteAssTemplateStruttura(nodoTitolario.ID);

                        // Completamento contesto transazionale
                        transactionContext.Complete();
                    }
                }
            }

			return retValue;
		}

        /// <summary>
        /// Rimozione di un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public static bool deleteTitolario(DocsPaVO.utente.InfoUtente infoUtente, OrgTitolario nodoTitolario)
        {
            DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
            return titolarioManager.DeleteTitolario(nodoTitolario);
        }

        /// <summary>
        /// Salvataggio di un titolario
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public static bool SaveTitolario(DocsPaVO.utente.InfoUtente infoUtente, OrgTitolario nodoTitolario)
        {
            bool saved = false;

            // Creazione di un nuovo contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                
                saved = titolarioManager.SaveTitolario(nodoTitolario);

                if (saved)
                    // Completamento con successo della transazione
                    transactionContext.Complete();
            }

            return saved;
        }

        /// <summary>
        /// Salvataggio di un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public static bool existTitolarioInDef(string codiceAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool result = amm.existTitolarioInDef(codiceAmm);
            return result;
        }

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public static bool attivaTitolario(DocsPaVO.utente.InfoUtente infoUtente, OrgTitolario titolario)
        {
            bool retValue = false;
            
            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Interfaces.ITitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

                retValue = titolarioManager.AttivaTitolario(titolario);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        public static ArrayList getTitolari(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.getTitolari(idAmministrazione);            
        }

        public static string getTitolarioAttivo(string idAmministrazione, out string codAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.getIDTitolarioAttivo(idAmministrazione, out codAmministrazione);
        }

        public static ArrayList getTitolariUtilizzabili(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.getTitolariUtilizzabili(idAmministrazione);
        }

        public static DocsPaVO.amministrazione.OrgTitolario getTitolarioById(string idTitolario)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.getTitolarioById(idTitolario);            
        }

        public static string getRegTitolarioById(string idTitolario)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.getRegTitolarioById(idTitolario);
        }

        /// <summary>
        /// Reperimento del codice dell'amministrazione cui appartiene il nodo titolario, che deve essere attivo
        /// </summary>
        /// <param name="codiceNodoTitolario"></param>
        /// <returns></returns>
        public static string GetIdAmministrazione(string codiceNodoTitolario)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetIdAmmNodoTitolario(codiceNodoTitolario);
        }

		/// <summary>
		/// Verifica se un titolario può essere inserito
		/// </summary>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
		public static EsitoOperazione CanInsertTitolario(OrgNodoTitolario nodoTitolario)
		{
			EsitoOperazione retValue=new EsitoOperazione();
			
			string errorDescription=string.Empty;
			System.Collections.ArrayList listErrorDescription=new System.Collections.ArrayList();
			
			// Validazione dati obbligatori immessi
			if (!IsValidRequiredFieldsTitolario(DBActionTypeTitolarioEnum.InsertMode,
												nodoTitolario,
												out errorDescription))
			{
				retValue.Codice=-1;
				listErrorDescription.Add(errorDescription);
			}
			else
			{
				// Verifica il livello del titolario, non può essere maggiore di 6
				if (Convert.ToInt32(nodoTitolario.Livello)>6)
				{
					retValue.Codice=-1;
					listErrorDescription.Add(" - Impossibile inserire un titolario con livello maggiore di 6");
				}

				// Verifica univocità del codice titolario, in base all'amministrazione e al registro specifico
				DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				if (nodoTitolario.IDRegistroAssociato!=null && 
					nodoTitolario.IDRegistroAssociato!=string.Empty)
				{
                    if (amm.ExistCodiceTitolario(nodoTitolario.Codice, amm.GetIDAmm(nodoTitolario.CodiceAmministrazione), nodoTitolario.IDRegistroAssociato, nodoTitolario.ID_Titolario))
					{
						retValue.Codice=-1;
						listErrorDescription.Add(" - Il codice " + nodoTitolario.Codice + " immesso è già presente sul registro selezionato");
					}
				}

				// Verifica univocità del codice titolario, in base all'amministrazione e a tutti i registri
				if (nodoTitolario.IDRegistroAssociato==null || 
					nodoTitolario.IDRegistroAssociato==string.Empty ||
					nodoTitolario.IDRegistroAssociato.Equals(""))
				{
					// Verifica se non esiste già un altro nodo con lo stesso codice 
					// su tutti i registri della stessa amministrazione
                    if (CheckCodiciDuplicatiTitAllReg(nodoTitolario.Codice, nodoTitolario.CodiceAmministrazione, nodoTitolario.ID_Titolario))
					{
						retValue.Codice=-1;
						listErrorDescription.Add(" - Il codice " + nodoTitolario.Codice + " immesso è già presente su un altro registro");
					}
				}
                
                //Protocollo Titolario
                //Verifico se non esiste già un nodo con lo stesso protocollo di titolario
                //per amministrazione, eventuale registro e titolario
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario()))
                {
                    if (!string.IsNullOrEmpty(nodoTitolario.numProtoTit) && amm.existProtocolloTit(nodoTitolario))
                    {
                        retValue.Codice = -1;
                        listErrorDescription.Add(" - Protocollo Titolario " + nodoTitolario.numProtoTit + " già presente.");
                    }
                }                                

                //amm=null;
				// Controllo sul registro, solo se non è tutti i registri
//				if (nodoTitolario.IDRegistroAssociato!=null && 
//					nodoTitolario.IDRegistroAssociato!=string.Empty)
//				{
//					// Verifica congruenza registro associato rispetto al registro del titolario padre
//					amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
//					int idRegistroParent=amm.GetIDRegistroAssociatoTitolario(nodoTitolario.IDParentNodoTitolario);
//					amm=null;
//
//					if (idRegistroParent>-1)
//					{
//						if (!IsValidRegistroAssociato(nodoTitolario.IDRegistroAssociato,
//														idRegistroParent.ToString(),
//														out errorDescription))
//						{
//							retValue.Codice=-1;
//							listErrorDescription.Add(errorDescription);
//						}
//					}
//				}
			}

			if (retValue.Codice!=0)
			{
				errorDescription=string.Empty;

				foreach (string item in listErrorDescription)
				{
					if (errorDescription!=string.Empty)
						errorDescription += "\\n";

					errorDescription += item;
				}
				
				retValue.Descrizione=errorDescription;
			}

			return retValue;
		}

		/// <summary>
		/// Inserimento di un nuovo nodo di titolario.
		/// Per default, vengono ereditati il registro associato
		/// e la visibilità dei ruoli del nodo padre (se presente).
		/// </summary>
        /// <param name="infoUtente"></param>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
        public static EsitoOperazione InsertNodoTitolario(DocsPaVO.utente.InfoUtente infoUtente, ref OrgNodoTitolario nodoTitolario)
		{
            EsitoOperazione retValue = null;

            // Creazione di un nuovo contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                // verifica se il titolario può essere inserito
                retValue = CanInsertTitolario(nodoTitolario);

                if (retValue.Codice == 0)
                {
                    DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

                    if (!titolarioManager.SaveNodoTitolario(nodoTitolario))
                    {
                        retValue.Codice = -1;
                        retValue.Descrizione = "Errore nella creazione del nodo di titolario";
                    }
                    else
                    {
                        string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                        if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                            new DocsPaDB.Query_DocsPAWS.Amministrazione().InsertRelTemplateStruttura(nodoTitolario);

                        // Completamento contesto transazionale
                        transactionContext.Complete();
                    }
                }
            }

			return retValue;
		}

        public static DataSet GetNodiTitolario(DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

                //Selezione di tutti i nodi di titolario da copiare
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_GET_NODI");
                queryMng.setParam("idTitolario", titolario.ID);
                if (!string.IsNullOrEmpty(idRegistro))
                    queryMng.setParam("condRegistro", " AND PROJECT.ID_REGISTRO = " + idRegistro + " ");
                else
                    queryMng.setParam("condRegistro", " AND PROJECT.ID_REGISTRO IS NULL ");
                string commandText = queryMng.getSQL();
                logger.Debug("GetNodiTitolario QUERY : " + commandText);
                DataSet ds_nodiTitolario = new DataSet();
                dbProvider.ExecuteQuery(ds_nodiTitolario, commandText);
                
                return ds_nodiTitolario;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - GetNodiTitolario - ERRORE : " + e.Message);
                return null;
            }
        }

        public static DataSet GetIndiceSistematico(DocsPaVO.amministrazione.OrgTitolario titolario, string orderBy)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INDICE_SISTEMATICO");
                queryMng.setParam("idTitolario", titolario.ID);
                queryMng.setParam("orderBy", orderBy);
                
                string commandText = queryMng.getSQL();
                logger.Debug("GetGetIndiceSistematico QUERY : " + commandText);
                DataSet ds_nodiTitolario = new DataSet();
                dbProvider.ExecuteQuery(ds_nodiTitolario, commandText);

                return ds_nodiTitolario;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - GetGetIndiceSistematico - ERRORE : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public static bool CopiaTitolario(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = CopiaTitolarioInternal(infoUtente, titolario, idRegistro);

                if (retValue)
                    // Completamento contesto transazionale
                    transactionContext.Complete();
            }

            return retValue;
        }

        private static bool CopiaTitolarioInternal(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            bool retValue = false;
            bool titolarioInserted = false;

            // Creazione oggetto documentale
            DocsPaDocumentale.Interfaces.ITitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);

            // Creazione copia oggetto titolario con stato in definizione
            OrgTitolario titolarioInDefinizione = null;

            try
            {
                // Creazione copia oggetto titolario con stato in definizione
                titolarioInDefinizione = titolario.CopyInDefinizione();

                // Inserimento nuovo titolario con stato in definizione
                titolarioInserted = titolarioManager.SaveTitolario(titolarioInDefinizione);
                retValue = titolarioInserted;

                if (retValue)
                {
                    DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

                    //Selezione di tutti i nodi di titolario da copiare
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_GET_NODI");
                    queryMng.setParam("idTitolario", titolario.ID);
                    if (!string.IsNullOrEmpty(idRegistro))
                        queryMng.setParam("condRegistro", " AND PROJECT.ID_REGISTRO = " + idRegistro + " ");
                    else
                        queryMng.setParam("condRegistro", " AND PROJECT.ID_REGISTRO IS NULL ");
                
                    string commandText = queryMng.getSQL();
                    logger.Debug("Copia titolario QUERY : " + commandText);

                    DataSet ds_nodiTitolario = new DataSet();
                    dbProvider.ExecuteQuery(ds_nodiTitolario, commandText);

                    //Inserimento dei nodi sotto il nuovo titolario
                    foreach (DataRow row in ds_nodiTitolario.Tables[0].Rows)
                    {
                        //Inserimento nodo di titolario di primo livello
                       // if (row["VAR_CODICE"].ToString().Length == 1)
                        if ( row["NUM_LIVELLO"].ToString() == "1")
                        {
                            //Creo il nodo da inserire
                            DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolarioDaInserire = new DocsPaVO.amministrazione.OrgNodoTitolario();

                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = row["VAR_CODICE"].ToString();
                            nodoTitolarioDaInserire.CodiceAmministrazione = titolario.CodiceAmministrazione;

                            if (row["CHA_RW"].ToString() == "W")
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                            else
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;

                            nodoTitolarioDaInserire.Descrizione = row["DESCRIPTION"].ToString();
                            nodoTitolarioDaInserire.IDParentNodoTitolario = titolarioInDefinizione.ID;
                            nodoTitolarioDaInserire.ID_Titolario = titolarioInDefinizione.ID;

                            if (row["ID_REGISTRO"] != DBNull.Value)
                                nodoTitolarioDaInserire.IDRegistroAssociato = row["ID_REGISTRO"].ToString();
                            else
                                nodoTitolarioDaInserire.IDRegistroAssociato = string.Empty;

                            nodoTitolarioDaInserire.CodiceLivello = row["VAR_COD_LIV1"].ToString();
                            nodoTitolarioDaInserire.Livello = "1";

                            if (row["NUM_MESI_CONSERVAZIONE"] != DBNull.Value)
                                nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(row["NUM_MESI_CONSERVAZIONE"].ToString());
                            else
                                nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                            nodoTitolarioDaInserire.bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                            nodoTitolarioDaInserire.contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                            nodoTitolarioDaInserire.numProtoTit = row["NUM_PROT_TIT"].ToString();

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);

                            if (esito.Codice != 0)
                            {
                                logger.Debug("ERRORE : " + esito.Descrizione);
                                retValue = false;
                                break;
                            }
                            else
                            {
                                //Inserisco il nodo
                                retValue = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);

                                if (retValue)
                                {
                                    logger.Debug("Inserimento Nodo con codice : " + nodoTitolarioDaInserire.Codice + " effettuato correttamente.");
                                }
                                else
                                {
                                    logger.Debug("ERRORE : " + esito.Descrizione);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //Inserimento nodo di titolario di livello superiore al primo

                            //Costruisco il codice del nodo padre
                            string[] codiceNodoSplit = row["VAR_CODICE"].ToString().Split('.');
                            string codicePadre = string.Empty;
                            codicePadre = getCodiceNodoPadre(codiceNodoSplit);
                            string queryRegistro = "AND ID_REGISTRO IS NULL";

                            if (row["ID_REGISTRO"] != DBNull.Value)
                                queryRegistro = "AND (ID_REGISTRO IS NULL OR ID_REGISTRO = " + row["ID_REGISTRO"].ToString() + ")";

                            OrgNodoTitolario nodoTitolarioPadre = getNodoTitolario(codicePadre, titolario.CodiceAmministrazione, queryRegistro, titolarioInDefinizione.ID);

                            //Creo il nodo da inserire
                            OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = row["VAR_CODICE"].ToString();
                            nodoTitolarioDaInserire.CodiceAmministrazione = titolario.CodiceAmministrazione;

                            //Creazione livello nodo
                            int livello = Convert.ToInt32(nodoTitolarioPadre.Livello);
                            livello++;

                            nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = (row["CHA_RW"].ToString() == "W");

                            nodoTitolarioDaInserire.Descrizione = row["DESCRIPTION"].ToString();
                            nodoTitolarioDaInserire.IDParentNodoTitolario = nodoTitolarioPadre.ID;
                            nodoTitolarioDaInserire.ID_Titolario = titolarioInDefinizione.ID;

                            if (row["ID_REGISTRO"] != DBNull.Value)
                                nodoTitolarioDaInserire.IDRegistroAssociato = row["ID_REGISTRO"].ToString();
                            else
                                nodoTitolarioDaInserire.IDRegistroAssociato = string.Empty;

                            nodoTitolarioDaInserire.CodiceLivello = row["VAR_COD_LIV1"].ToString();
                            nodoTitolarioDaInserire.Livello = livello.ToString();

                            if (row["NUM_MESI_CONSERVAZIONE"] != DBNull.Value)
                                nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(row["NUM_MESI_CONSERVAZIONE"].ToString());
                            else
                                nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                            nodoTitolarioDaInserire.bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                            nodoTitolarioDaInserire.contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                            nodoTitolarioDaInserire.numProtoTit = row["NUM_PROT_TIT"].ToString();

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);

                            if (esito.Codice != 0)
                            {
                                logger.Debug("ERRORE : " + esito.Descrizione);
                                retValue = false;
                                break;
                            }
                            else
                            {
                                //Inserisco il nodo
                                retValue = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);

                                if (retValue)
                                {
                                    logger.Debug("Inserimento Nodo con codice : " + nodoTitolarioDaInserire.Codice + " effettuato correttamente.");
                                }
                                else
                                {
                                    logger.Debug("ERRORE : " + esito.Descrizione);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("SQL - CopiaTitolario - ERRORE : " + e.Message);
                retValue = false;
                if (titolarioInserted)
                {
                    // Rollback inserimento nuovo titolario
                    titolarioManager.DeleteTitolario(titolarioInDefinizione);
                }
            }

            return retValue;
        }

        /*
        private static bool CopiaTitolarioDb(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            bool retValue = false;

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                dbProvider.BeginTransaction();
                

                //Reperimento id amministrazione
                DocsPaDB.Query_DocsPAWS.Amministrazione amministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                string idAmministrazione = amministrazione.GetIDAmm(titolario.CodiceAmministrazione);

                //Creazione di un nuovo titolario in definizione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_INSERT");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryMng.setParam("varCodice", "T");
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("varNote", "");
                string commandText = queryMng.getSQL();
                logger.Debug("Copia titolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Reperimento dell system_id
                string system_id_titolario = string.Empty;
                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                dbProvider.ExecuteScalar(out system_id_titolario, commandText);

                //Selezione di tutti i nodi di titolario da copiare
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_GET_NODI");
                queryMng.setParam("idTitolario", titolario.ID);
                commandText = queryMng.getSQL();
                logger.Debug("Copia titolario QUERY : " + commandText);
                DataSet ds_nodiTitolario = new DataSet();
                dbProvider.ExecuteQuery(ds_nodiTitolario, commandText);

                //Inserimento dei nodi sotto il nuovo titolario
                if (ds_nodiTitolario.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_nodiTitolario.Tables[0].Rows.Count; i++)
                    {
                        //Inserimento nodo di titolario di primo livello
                        if (ds_nodiTitolario.Tables[0].Rows[i]["VAR_CODICE"].ToString().Length == 1)
                        {
                            //Creo il nodo da inserire
                            DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolarioDaInserire = new DocsPaVO.amministrazione.OrgNodoTitolario();
                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = ds_nodiTitolario.Tables[0].Rows[i]["VAR_CODICE"].ToString();
                            nodoTitolarioDaInserire.CodiceAmministrazione = titolario.CodiceAmministrazione;
                            if (ds_nodiTitolario.Tables[0].Rows[i]["CHA_RW"].ToString() == "W")
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                            else
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;

                            nodoTitolarioDaInserire.Descrizione = ds_nodiTitolario.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                            nodoTitolarioDaInserire.IDParentNodoTitolario = system_id_titolario;
                            nodoTitolarioDaInserire.ID_Titolario = system_id_titolario;

                            if (ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString() != null && ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString() != "")
                                nodoTitolarioDaInserire.IDRegistroAssociato = ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                            else
                                nodoTitolarioDaInserire.IDRegistroAssociato = "";

                            nodoTitolarioDaInserire.CodiceLivello = ds_nodiTitolario.Tables[0].Rows[i]["VAR_COD_LIV1"].ToString();
                            nodoTitolarioDaInserire.Livello = "1";

                            if (ds_nodiTitolario.Tables[0].Rows[i]["NUM_MESI_CONSERVAZIONE"].ToString() != null && ds_nodiTitolario.Tables[0].Rows[i]["NUM_MESI_CONSERVAZIONE"].ToString() != "")
                                nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(ds_nodiTitolario.Tables[0].Rows[i]["NUM_MESI_CONSERVAZIONE"].ToString());
                            else
                                nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);
                            if (esito.Codice != 0)
                            {
                                logger.Debug("ERRORE : " + esito.Descrizione);
                                retValue = false;
                            }
                            else
                            {
                                //Inserisco il nodo
                                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager();
                                retValue = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);

                                if (retValue)
                                    logger.Debug("Inserimento Nodo con codice : " + nodoTitolarioDaInserire.Codice + " effettuato correttamente.");
                                else
                                    logger.Debug("ERRORE : " + esito.Descrizione);
                            }
                        }
                        else
                        {
                            //Inserimento nodo di titolario di livello superiore al primo

                            //Costruisco il codice del nodo padre
                            string[] codiceNodoSplit = ds_nodiTitolario.Tables[0].Rows[i]["VAR_CODICE"].ToString().Split('.');
                            string codicePadre = string.Empty;
                            codicePadre = getCodiceNodoPadre(codiceNodoSplit);
                            string queryRegistro = "AND ID_REGISTRO IS NULL";
                            if (ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString() != null && ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString() != "")
                                queryRegistro = "AND ID_REGISTRO = " + ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                            OrgNodoTitolario nodoTitolarioPadre = getNodoTitolario(codicePadre, titolario.CodiceAmministrazione, queryRegistro, system_id_titolario);

                            //Creo il nodo da inserire
                            OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = ds_nodiTitolario.Tables[0].Rows[i]["VAR_CODICE"].ToString();
                            nodoTitolarioDaInserire.CodiceAmministrazione = titolario.CodiceAmministrazione;

                            //Creazione livello nodo
                            int livello = Convert.ToInt32(nodoTitolarioPadre.Livello);
                            livello++;

                            if (ds_nodiTitolario.Tables[0].Rows[i]["CHA_RW"].ToString() == "W")
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                            else
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;

                            nodoTitolarioDaInserire.Descrizione = ds_nodiTitolario.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                            nodoTitolarioDaInserire.IDParentNodoTitolario = nodoTitolarioPadre.ID;
                            nodoTitolarioDaInserire.ID_Titolario = system_id_titolario;

                            if (ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString() != null && ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString() != "")
                                nodoTitolarioDaInserire.IDRegistroAssociato = ds_nodiTitolario.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                            else
                                nodoTitolarioDaInserire.IDRegistroAssociato = "";

                            nodoTitolarioDaInserire.CodiceLivello = ds_nodiTitolario.Tables[0].Rows[i]["VAR_COD_LIV1"].ToString();
                            nodoTitolarioDaInserire.Livello = livello.ToString();

                            if (ds_nodiTitolario.Tables[0].Rows[i]["NUM_MESI_CONSERVAZIONE"].ToString() != null && ds_nodiTitolario.Tables[0].Rows[i]["NUM_MESI_CONSERVAZIONE"].ToString() != "")
                                nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(ds_nodiTitolario.Tables[0].Rows[i]["NUM_MESI_CONSERVAZIONE"].ToString());
                            else
                                nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);

                            if (esito.Codice != 0)
                            {
                                logger.Debug("ERRORE : " + esito.Descrizione);
                                retValue = false;
                            }
                            else
                            {
                                //Inserisco il nodo
                                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager();
                                retValue = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);
                                if (retValue)
                                    logger.Debug("Inserimento Nodo con codice : " + nodoTitolarioDaInserire.Codice + " effettuato correttamente.");
                                else
                                    logger.Debug("ERRORE : " + esito.Descrizione);
                            }
                        }
                    }
                }

                dbProvider.CommitTransaction();
            }
            catch (Exception e)
            {
                logger.Debug("SQL - CopiaTitolario - ERRORE : " + e.Message);
                dbProvider.RollbackTransaction();
            }

            return retValue;
        }
         * */

		#endregion

		#region Private members

		/// <summary>
		/// Verifica se un determinato titolario contenga dei ruoli associati
		/// </summary>
		/// <param name="idTitolario"></param>
		/// <returns></returns>
		public static int GetCountRuoliAssociatiInTitolario(string idTitolario)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			return amm.GetCountRuoliAssociatiInTitolario(idTitolario);
		}

		public static bool CheckCodiciDuplicatiTitolario(string idTitolario, string codice)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			return amm.CheckCodiciDuplicatiTitolario(idTitolario, codice);
		}

		public static bool CheckCodiciDuplicatiTitAllReg(string codice, string codAmm, string idTitolario)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.CheckCodiciDuplicatiTitAllReg(codice, codAmm, idTitolario);
		}

		private static System.Collections.ArrayList CreateListNodiTitolario(string codiceAmministrazione,DataTable tableNodiTitolario)
		{
			System.Collections.ArrayList retValue=new System.Collections.ArrayList();
			
			foreach (DataRow row in tableNodiTitolario.Rows)
			{
				OrgNodoTitolario nodoTitolario=new OrgNodoTitolario();
				
				nodoTitolario.ID=row["IDRECORD"].ToString();
				nodoTitolario.Codice=row["CODICE"].ToString();
				nodoTitolario.Descrizione=row["DESCRIZIONE"].ToString();
				nodoTitolario.CodiceAmministrazione=codiceAmministrazione;
				nodoTitolario.Livello=row["LIVELLO"].ToString();
				nodoTitolario.IDRegistroAssociato=row["REGISTRO"].ToString();
				nodoTitolario.IDParentNodoTitolario=row["IDPARENT"].ToString();
				nodoTitolario.CodiceLivello=row["CODLIV"].ToString();
				nodoTitolario.CreazioneFascicoliAbilitata=(row["RW"].ToString()=="W");
                if (row["CHA_CONSENTI_CLASS"] != DBNull.Value)
                    nodoTitolario.consentiClassificazione = row["CHA_CONSENTI_CLASS"].ToString();
                
                if (row["CHA_CONSENTI_FASC"] != DBNull.Value)
                    nodoTitolario.consentiFascicolazione = row["CHA_CONSENTI_FASC"].ToString();

				if (row["NUMMESICONSERVAZIONE"]!=DBNull.Value)
					nodoTitolario.NumeroMesiConservazione=Convert.ToInt32(row["NUMMESICONSERVAZIONE"].ToString());

				nodoTitolario.CountChildNodiTitolario=Convert.ToInt32(row["FIGLIO"].ToString());
                nodoTitolario.ID_TipoFascicolo = row["ID_TIPO_FASC"].ToString();
                nodoTitolario.bloccaTipoFascicolo = row["CHA_BLOCCA_FASC"].ToString();
                nodoTitolario.ID_Titolario = row["ID_TITOLARIO"].ToString();
                if(row["DTA_ATTIVAZIONE"].ToString() != null && row["DTA_ATTIVAZIONE"].ToString() != "")
                    nodoTitolario.dataAttivazione = ((DateTime) row["DTA_ATTIVAZIONE"]).ToString("dd/MM/yyyy");
                if(row["DTA_CESSAZIONE"].ToString() != null && row["DTA_CESSAZIONE"].ToString() != "")
                    nodoTitolario.dataCessazione = ((DateTime) row["DTA_CESSAZIONE"]).ToString("dd/MM/yyyy");
                nodoTitolario.stato = row["CHA_STATO"].ToString();
                nodoTitolario.note = row["VAR_NOTE"].ToString();

                nodoTitolario.bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                nodoTitolario.contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                nodoTitolario.numProtoTit = row["NUM_PROT_TIT"].ToString();

				retValue.Add(nodoTitolario);
				nodoTitolario=null;
			}

			return retValue;
		}

        private static System.Collections.ArrayList CreateListNodiTitolario_InRuolo(DataTable tableNodiTitolario)
        {
            System.Collections.ArrayList retValue = new System.Collections.ArrayList();

            foreach (DataRow row in tableNodiTitolario.Rows)
            {
                OrgNodoTitolario nodoTitolario = new OrgNodoTitolario();

                nodoTitolario.ID = row["IDRECORD"].ToString();
                nodoTitolario.Codice = row["CODICE"].ToString();
                nodoTitolario.Descrizione = row["DESCRIZIONE"].ToString();                
                nodoTitolario.Livello = row["LIVELLO"].ToString();                
                nodoTitolario.CodiceLivello = row["CODLIV"].ToString();
                nodoTitolario.IDParentNodoTitolario = row["IDPARENT"].ToString();
                
                retValue.Add(nodoTitolario);
                nodoTitolario = null;
            }

            return retValue;
        }

		private static System.Collections.ArrayList CreateListRuoliInTitolario(DataTable tableRuoliTitolario)
		{
			System.Collections.ArrayList retValue=new System.Collections.ArrayList();
			
			int rowIndex=0;

			foreach (DataRow row in tableRuoliTitolario.Rows)
			{
				OrgRuoloTitolario ruoloTitolario=new OrgRuoloTitolario();
				
				ruoloTitolario.ID=row["ID_RUOLO"].ToString();
				ruoloTitolario.Codice=row["CODICE_RUOLO"].ToString();
				ruoloTitolario.Descrizione=row["DESCRIZIONE_RUOLO"].ToString();
				ruoloTitolario.Associato=(row["GRUPPOASSOCIATO"]!=DBNull.Value);

				retValue.Add(ruoloTitolario);
				rowIndex++;

				ruoloTitolario=null;
			}

			return retValue;
		}

		/// <summary>
		/// Verifica la congruenza del registro associato ad un titolario
		/// rispetto a quello del titolario padre e degli eventuali titolari figli
		/// </summary>
		/// <param name="nodoTitolario"></param>
		/// <param name="viewTitolario"></param>
		/// <returns></returns>
		private static bool IsValidRegistroAssociato(
										string idRegistroAssociato,
										string idRegistroParent,
										out string errorDescription)
		{
			bool isValid=true;
			errorDescription=string.Empty;

			if (idRegistroParent!=null && idRegistroParent!=string.Empty && idRegistroParent!="0")
			{
				if (idRegistroParent!=idRegistroAssociato || 
					idRegistroAssociato==string.Empty)
				{
					isValid=false;
				}						
			}

			if (!isValid)
			{
				// Il registro associato al titolario è differente
				// rispetto a quello associato al titolario padre
				errorDescription=" - Il registro associato al titolario non è congruente rispetto ai titolari nella gerarchia";
			}

			return isValid;
		}

        /// <summary>
        /// estende la visibilità ad un ruolo dato su tutti i figli di un nodo di titolario dato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNodoTitolario"></param>
        /// <param name="ruoloTitolario"></param>
        /// <param name="idAmm"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static EsitoOperazione AmmExtendToChildNodes(DocsPaVO.utente.InfoUtente infoUtente, string idNodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario, string idAmm, string idRegistro, bool check)
        {
            EsitoOperazione retValue = new EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dmAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue = AmmExtendToChildNodes(infoUtente, idNodoTitolario, ruoloTitolario, idAmm, dmAmm.GetVarCodiceAmm(idAmm), idRegistro, retValue, check);

                if (retValue.Codice == 0)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// estende o  elimina la visibilità ad un ruolo dato su tutti i figli di un nodo di titolario dato
        /// </summary>
        /// <param name="idNodoTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <param name="idAmm"></param>
        /// <param name="idRegistro"></param>
        /// <param name="esito"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private static EsitoOperazione AmmExtendToChildNodes(DocsPaVO.utente.InfoUtente infoUtente, string idNodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario, string idAmm, string codAmm, string idRegistro, DocsPaVO.amministrazione.EsitoOperazione esito, bool check)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            DataSet ds = amm.GetDsNodiTitolario(idAmm, idNodoTitolario, idRegistro);

            System.Collections.ArrayList listaNodiFigli = CreateListNodiTitolario(codAmm, ds.Tables["NODI_TITOLARIO"]);

            if (listaNodiFigli != null && listaNodiFigli.Count > 0)
            {
                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                
                if (check)
                {
                    ruoloTitolario.Associato = true;
                    foreach (DocsPaVO.amministrazione.OrgNodoTitolario currentNode in listaNodiFigli)
                    {
                        if (!existRuoloNodoTitolario(currentNode.ID, ruoloTitolario.ID))
                        {
                            if (!titolarioManager.SetAclRuoloNodoTitolario(currentNode, ruoloTitolario))
                            {
                                esito.Codice = 1;
                                esito.Descrizione += " - impossibile inserire i diritti sul nodo di titolario: [" + currentNode.Codice + " - " + currentNode.Descrizione + "]\\n";
                            }
                        }

                        if (currentNode.CountChildNodiTitolario > 0)
                            esito = AmmExtendToChildNodes(infoUtente, currentNode.ID, ruoloTitolario, idAmm, codAmm, idRegistro, esito, check);
                    }
                }
                else
                {
                    ruoloTitolario.Associato = false;
                    OrgNodoTitolario nodoTitolario = amm.getNodoTitolario(idNodoTitolario);
                    if (titolarioManager.SetAclRuoloNodoTitolario(nodoTitolario, ruoloTitolario))
                        //DocsPaDB.Query_DocsPAWS.Amministrazione dmAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                        amm.UpdateRuoloTitolario(idNodoTitolario, ruoloTitolario.ID, false);
                    //dmAmm.DeleteVisRuoloNodoTitolario(idNodoTitolario, ruoloTitolario.ID);

                    foreach (DocsPaVO.amministrazione.OrgNodoTitolario currentNode in listaNodiFigli)
                    {
                        if (existRuoloNodoTitolario(currentNode.ID, ruoloTitolario.ID))
                        {
                            //dmAmm.DeleteVisRuoloNodoTitolario(currentNode.ID, ruoloTitolario.ID);
                            if (titolarioManager.SetAclRuoloNodoTitolario(currentNode, ruoloTitolario))
                                amm.UpdateRuoloTitolario(currentNode.ID, ruoloTitolario.ID, false);
                        }

                        if (currentNode.CountChildNodiTitolario > 0)
                            esito = AmmExtendToChildNodes(infoUtente, currentNode.ID, ruoloTitolario, idAmm, codAmm, idRegistro, esito, check);
                    }
                }
            }

            return esito;
        }		

		private static bool existRuoloNodoTitolario(string idNodoTitolario, string idRuolo)
		{			
			DocsPaDB.Query_DocsPAWS.Amministrazione amm=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			return amm.AmmExistRecordInSecurity(idNodoTitolario,"255",idRuolo,"P");			
		}

		#endregion

        #region Importazione titolario da foglio excel

        public static bool importaTitolario(byte[] dati, string nomeFile, string serverPath, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario, string provider, string extendedProperty)
        {
            bool warning = false;
            bool error = false;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            OleDbDataReader xlsReader_1 = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione amministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");

            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportazioneTitolario");

            try
            {
                #region Scrittura del file
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
                #endregion Scrittura del file

                #region Inserimento nodi di titolario
                //Comincio la lettura del file appena scritto
                sl.Log("**** Inizio importazione titolario - " + System.DateTime.Now.ToString());
                xlsConn = ImportUtils.ConnectToFile(provider, extendedProperty, serverPath + "\\Modelli\\Import\\" + nomeFile); 
                //xlsConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
                //xlsConn.Open();
 
                OleDbCommand xlsCmd = new OleDbCommand("select * from [Titolario$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                while (xlsReader.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //Controllo campi obbligatori
                    if (get_string(xlsReader, 0) == "" || get_string(xlsReader, 2) == "" || get_string(xlsReader, 8) == "")
                    {
                        sl.Log("");
                        sl.Log("WARNING : Campi obbligatori non inseriti nel modello");
                        warning = true;
                    }
                    else
                    {
                        //Costruisco il codice del nodo
                        string codiceNodo = string.Empty;
                        for (int i = 2; i < 8; i++)
                        {
                            if (get_string(xlsReader, i) != null && get_string(xlsReader, i) != "")
                                codiceNodo += get_string(xlsReader, i) + ".";
                        }

                        codiceNodo = codiceNodo.Substring(0, codiceNodo.Length - 1);
                        string[] codiceNodoSplit = codiceNodo.Split('.');
                        string codiceAmministrazione = get_string(xlsReader, 0);
                        string queryRegistro = "AND ID_REGISTRO IS NULL";
                        if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                            queryRegistro = "AND ( ID_REGISTRO IS NULL OR ID_REGISTRO = " + Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1)) + ") ";

                        //Nodo primo livello
                        //if (codiceNodo.Length == 1)
                        if (codiceNodoSplit.Length == 1)
                        {
                            sl.Log("");
                            sl.Log("Inserimento Nodo con codice : " + codiceNodo);

                            //Creo il nodo da inserire
                            OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = codiceNodo;
                            nodoTitolarioDaInserire.CodiceAmministrazione = get_string(xlsReader, 0);
                            //nodoTitolarioDaInserire.CountChildNodiTitolario
                            if (get_string(xlsReader, 9).ToUpper().Equals("SI"))
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                            else
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;
                            nodoTitolarioDaInserire.Descrizione = get_string(xlsReader, 8);
                            //nodoTitolarioDaInserire.ID
                            //nodoTitolarioDaInserire.ID_TipoFascicolo
                            nodoTitolarioDaInserire.IDParentNodoTitolario = titolario.ID;
                            nodoTitolarioDaInserire.ID_Titolario = titolario.ID;
                            if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                            {
                                nodoTitolarioDaInserire.IDRegistroAssociato = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));
                            }
                            else
                            {
                                nodoTitolarioDaInserire.IDRegistroAssociato = "";
                            }
                            nodoTitolarioDaInserire.CodiceLivello = GetCodiceLivello("", "1", nodoTitolarioDaInserire.CodiceAmministrazione, titolario.ID, nodoTitolarioDaInserire.IDRegistroAssociato);
                            nodoTitolarioDaInserire.Livello = "1";

                            if (get_string(xlsReader, 10) != null && get_string(xlsReader, 10) != string.Empty)
                                nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(get_string(xlsReader, 10));
                            else
                                nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                            if (!string.IsNullOrEmpty(get_string(xlsReader, 12)))
                                nodoTitolarioDaInserire.bloccaNodiFigli = get_string(xlsReader, 12).ToUpper();

                            if (!string.IsNullOrEmpty(get_string(xlsReader, 13)))
                                nodoTitolarioDaInserire.consentiClassificazione = get_string(xlsReader, 13);

                            if (!string.IsNullOrEmpty(get_string(xlsReader, 14)))
                                nodoTitolarioDaInserire.consentiFascicolazione = get_string(xlsReader, 14);

                            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario()))
                            {
                                if (!string.IsNullOrEmpty(get_string(xlsReader, 15)))
                                    nodoTitolarioDaInserire.contatoreAttivo = get_string(xlsReader, 15).ToUpper(); ;

                                if (!string.IsNullOrEmpty(get_string(xlsReader, 16)))
                                    nodoTitolarioDaInserire.numProtoTit = get_string(xlsReader, 16);
                            }

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);
                            if (esito.Codice != 0)
                            {
                                sl.Log("ERRORE : " + esito.Descrizione);
                                error = true;
                            }
                            else
                            {
                                //Inserisco il nodo
                                //esito = amministrazione.InsertNodoTitolario(ref nodoTitolarioDaInserire);
                                bool result = false;
                                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                                {
                                    nodoTitolarioDaInserire.isImport = "1";
                                    result = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);
                                    transactionContext.Complete();
                                }
                                //if (esito.Codice == 0)
                                if (result)
                                {
                                    sl.Log("Inserimento Nodo con codice : " + codiceNodo + " effettuato correttamente.");

                                    //Verifico se impostare o meno la visibilità sul nodo
                                    if (nodoTitolarioDaInserire.IDRegistroAssociato != null)
                                    {
                                        //if (get_string(xlsReader, 11) != null && get_string(xlsReader, 11) != "")
                                        if (get_string(xlsReader, 11) != null)
                                        {
                                            //Imposto la visiblità
                                            impostaVisibilitaNodo(infoUtente, nodoTitolarioDaInserire, get_string(xlsReader, 11), sl, ref warning);
                                        }
                                    }
                                }
                                else
                                {
                                    sl.Log("ERRORE : " + esito.Descrizione);
                                    error = true;
                                }
                            }
                        }

                        //Nodo livello successivo al primo
                        //if (codiceNodo.Length > 1)
                        if (codiceNodoSplit.Length > 1)
                        {
                            //Costruisco il codice del nodo padre
                            string codicePadre = string.Empty;
                            //codicePadre = codiceNodo.Substring(0, codiceNodo.Length - 2);
                            codicePadre = getCodiceNodoPadre(codiceNodoSplit);

                            OrgNodoTitolario nodoTitolarioPadre = getNodoTitolario(codicePadre, codiceAmministrazione, queryRegistro, titolario.ID);
                            if (nodoTitolarioPadre != null)
                            {
                                //Controllo la presenza di tutti di dati del nodo padre necessari 
                                if (nodoTitolarioPadre.Livello != null && nodoTitolarioPadre.Livello != "" &&
                                    nodoTitolarioPadre.ID != null && nodoTitolarioPadre.ID != "")
                                {
                                    sl.Log("");
                                    sl.Log("Inserimento Nodo con codice : " + codiceNodo + " -- Codice Nodo padre : " + codicePadre);

                                    //Creo il nodo da inserire
                                    OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                                    nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                                    nodoTitolarioDaInserire.Codice = codiceNodo;
                                    nodoTitolarioDaInserire.CodiceAmministrazione = get_string(xlsReader, 0);

                                    //Creazione livello nodo
                                    int livello = Convert.ToInt32(nodoTitolarioPadre.Livello);
                                    livello++;

                                    //nodoTitolarioDaInserire.CountChildNodiTitolario
                                    if (get_string(xlsReader, 9).Equals("SI"))
                                        nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                                    else
                                        nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;
                                    nodoTitolarioDaInserire.Descrizione = get_string(xlsReader, 8);
                                    //nodoTitolarioDaInserire.ID
                                    //nodoTitolarioDaInserire.ID_TipoFascicolo
                                    nodoTitolarioDaInserire.IDParentNodoTitolario = nodoTitolarioPadre.ID;
                                    nodoTitolarioDaInserire.ID_Titolario = nodoTitolarioPadre.ID_Titolario;
                                    if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                                    {
                                        nodoTitolarioDaInserire.IDRegistroAssociato = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));
                                    }
                                    else
                                    {
                                        nodoTitolarioDaInserire.IDRegistroAssociato = "";
                                    }
                                    nodoTitolarioDaInserire.CodiceLivello = GetCodiceLivello(nodoTitolarioPadre.CodiceLivello, livello.ToString(), nodoTitolarioDaInserire.CodiceAmministrazione, nodoTitolarioPadre.ID_Titolario, nodoTitolarioDaInserire.IDRegistroAssociato);
                                    nodoTitolarioDaInserire.Livello = livello.ToString();
                                    if (get_string(xlsReader, 10) != null && get_string(xlsReader, 10) != string.Empty)
                                        nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(get_string(xlsReader, 10));
                                    else
                                        nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                                    if (!string.IsNullOrEmpty(get_string(xlsReader, 12)))
                                        nodoTitolarioDaInserire.bloccaNodiFigli = get_string(xlsReader, 12);

                                    if (!string.IsNullOrEmpty(get_string(xlsReader, 13)))
                                        nodoTitolarioDaInserire.consentiClassificazione = get_string(xlsReader, 13);

                                    if (!string.IsNullOrEmpty(get_string(xlsReader, 14)))
                                        nodoTitolarioDaInserire.consentiFascicolazione = get_string(xlsReader, 14);

                                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario()))
                                    {
                                        if (!string.IsNullOrEmpty(get_string(xlsReader, 15)))
                                            nodoTitolarioDaInserire.contatoreAttivo = get_string(xlsReader, 15).ToUpper(); ;

                                        if (!string.IsNullOrEmpty(get_string(xlsReader, 16)))
                                            nodoTitolarioDaInserire.numProtoTit = get_string(xlsReader, 16);
                                    }

                                    //Verifico se è possibile inserire il nodo
                                    DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                                    esito = CanInsertTitolario(nodoTitolarioDaInserire);
                                    if (esito.Codice != 0)
                                    {
                                        sl.Log("ERRORE : " + esito.Descrizione);
                                        error = true;
                                    }
                                    else
                                    {
                                        //Inserisco il nodo
                                        //esito = amministrazione.InsertNodoTitolario(ref nodoTitolarioDaInserire);
                                        DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                                        bool result = false;
                                        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                                        {
                                            nodoTitolarioDaInserire.isImport = "1";
                                            result = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);
                                            transactionContext.Complete();
                                        }
                                        //if (esito.Codice == 0)
                                        if (result)
                                        {
                                            sl.Log("Inserimento Nodo con codice : " + codiceNodo + " effettuato correttamente.");

                                            //Verifico se impostare o meno la visibilità sul nodo
                                            if (nodoTitolarioDaInserire.IDRegistroAssociato != null)
                                            {
                                                //if (get_string(xlsReader, 11) != null && get_string(xlsReader, 11) != "")
                                                if (get_string(xlsReader, 11) != null)
                                                {
                                                    //Imposto la visiblità
                                                    impostaVisibilitaNodo(infoUtente, nodoTitolarioDaInserire, get_string(xlsReader, 11), sl, ref warning);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sl.Log("ERRORE : " + esito.Descrizione);
                                            error = true;
                                        }
                                    }
                                }
                                else
                                {
                                    sl.Log("WARNING : Nodo con codice : " + codiceNodo + " non inserito, dati del nodo padre non tutti presenti o non coerenti");
                                    warning = true;
                                }
                            }
                            else
                            {
                                sl.Log("WARNING : Nodo con codice : " + codiceNodo + " non inserito padre " + codicePadre + " inesistente");
                                warning = true;
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione Titolario - " + System.DateTime.Now.ToString());
                #endregion Inserimento nodi di titolario

                #region Inserimento fascicoli
                /*
                if (!warning && !error)
                {
                    //Importazione fascicoli
                    OleDbCommand xlsCmd_1 = new OleDbCommand("select * from [Fascicoli$]", xlsConn);
                    xlsReader_1 = xlsCmd_1.ExecuteReader();

                    sl.Log("");
                    sl.Log("**** Inizio importazione Fascicoli - " + System.DateTime.Now.ToString());
                    sl.Log("");

                    while (xlsReader_1.Read())
                    {
                        //Verifico l'obbligatorietà dei campi cod_ruolo e cod_utente
                        if (get_string(xlsReader_1, 0) != null && get_string(xlsReader_1, 0) != "" &&
                            get_string(xlsReader_1, 1) != null && get_string(xlsReader_1, 1) != "" &&
                            get_string(xlsReader_1, 2) != null && get_string(xlsReader_1, 2) != "" &&
                            get_string(xlsReader_1, 3) != null && get_string(xlsReader_1, 3) != "" &&
                            get_string(xlsReader_1, 4) != null && get_string(xlsReader_1, 4) != "" &&
                            get_string(xlsReader_1, 5) != null && get_string(xlsReader_1, 5) != ""
                            )
                        {

                            string codiceNodo = get_string(xlsReader_1, 1);
                            OrgNodoTitolario nodoTitolario = null;
                            DocsPaVO.utente.Registro reg = null;
                            string codiceAmministrazione = get_string(xlsReader_1, 0);
                            string queryReg = string.Empty;

                            if (get_string(xlsReader_1, 2) != null && get_string(xlsReader_1, 2) != "")
                            {
                                string systemIdReg = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader_1, 2));
                                reg = Utenti.RegistriManager.getRegistro(systemIdReg);
                                queryReg = " AND (ID_REGISTRO = " + systemIdReg + " OR ID_REGISTRO IS NULL)";
                            }

                            nodoTitolario = getNodoTitolario(codiceNodo, codiceAmministrazione, queryReg);

                            if (nodoTitolario != null && reg != null)
                            {
                                //Creo l'oggetto FasciolazioneClassificazione
                                DocsPaVO.fascicolazione.Classificazione classFasc = new DocsPaVO.fascicolazione.Classificazione();
                                classFasc.codice = codiceNodo;
                                classFasc.codUltimo = Fascicoli.FascicoloManager.getFascNumRif(nodoTitolario.ID, reg.systemId);
                                classFasc.descrizione = get_string(xlsReader_1, 5);
                                classFasc.registro = reg;
                                classFasc.systemID = nodoTitolario.ID;
                                classFasc.varcodliv1 = nodoTitolario.CodiceLivello;

                                //Creo l'oggetto fascicolo
                                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                                fascicolo.descrizione = get_string(xlsReader_1, 5);
                                fascicolo.codUltimo = classFasc.codUltimo;
                                fascicolo.cartaceo = false;
                                fascicolo.privato = "0";
                                fascicolo.note = get_string(xlsReader_1, 6);
                                fascicolo.apertura = System.DateTime.Today.ToString("dd/MM/yyyy");
                                fascicolo.idRegistroNodoTit = "";
                                fascicolo.idRegistro = reg.systemId;

                                //Creo gli oggetti UTENTE - RUOLO - INFOUTENTE
                                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                                string idAmm = amministrazione.GetIDAmm(nodoTitolario.CodiceAmministrazione);
                                DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader_1, 3), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                                DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader_1, 4), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                                //Procedo alla creazione del fascicolo
                                DocsPaVO.fascicolazione.ResultCreazioneFascicolo result = new DocsPaVO.fascicolazione.ResultCreazioneFascicolo();
                                Fascicoli.FascicoloManager.newFascicolo(classFasc, fascicolo, infoUtente, ruolo, false, out result);
                                if (result == DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                                {
                                    sl.Log("Fascicolo : " + fascicolo.descrizione + " inserito correttamente sotto il nodo : " + codiceNodo);
                                }
                                else
                                {
                                    sl.Log("ERRORE creazione fascicolo : " + fascicolo.descrizione + " " + result.ToString());
                                }
                            }
                        }
                        else
                        {
                            sl.Log("WARNING : Campi obbligatori non inseriti nel modello.");
                            warning = true;
                        }
                    }

                    sl.Log("");
                    sl.Log("**** Fine importazione Fascicoli - " + System.DateTime.Now.ToString());
                }
                */
                #endregion Inserimento fascicoli
            }
            catch (Exception ex)
            {
                sl.Log("");
                sl.Log("ERRORE : " + ex.Message);
                return false;
            }
            finally
            {
                if (xlsReader != null)
                    xlsReader.Close();
                if (xlsReader_1 != null)
                    xlsReader_1.Close();
                if (xlsConn != null)
                    xlsConn.Close();
            }

            if (warning || error)
                return false;
            else
                    return true;            
        }

        public static bool importaTitolario(byte[] dati, string nomeFile, string serverPath, DocsPaVO.utente.InfoUtente infoUtente)
        {
            //Metodo utilizzato esclusivamente dalla personalizzazione fatta per Fabio Schiroli 
            bool warning = false;
            bool error = false;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            OleDbDataReader xlsReader_1 = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione amministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");
            
            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportazioneTitolario");
            
            try
            {
                #region Scrittura del file
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
                #endregion Scrittura del file

                #region Inserimento nodi di titolario
                //Comincio la lettura del file appena scritto
                sl.Log("**** Inizio importazione titolario - " + System.DateTime.Now.ToString());

                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1';";
                xlsConn.Open();

                OleDbCommand xlsCmd = new OleDbCommand("select * from [Titolario$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                OrgTitolario titolario = null;

                while (xlsReader.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;
                    
                    //Recupero il titolario attivo per l'amministrazione desiderata
                    //OrgTitolario titolario = null;
                    if (get_string(xlsReader, 0) != "")
                    {
                        string idAmm = amministrazione.GetIDAmm(get_string(xlsReader, 0));
                        if (idAmm != "")
                        {
                            titolario = amministrazione.getTitolarioAttivo(idAmm);
                            titolario.CodiceAmministrazione = get_string(xlsReader, 0);

                        }
                    }
                
                    //Controllo campi obbligatori
                    if (get_string(xlsReader, 0) == "" || get_string(xlsReader, 2) == "" || get_string(xlsReader, 8) == "" || titolario == null)
                    {
                        sl.Log("");
                        sl.Log("WARNING : Campi obbligatori non inseriti nel modello");
                        warning = true;
                    }
                    else
                    {
                        //Costruisco il codice del nodo
                        string codiceNodo = string.Empty;
                        for (int i = 2; i < 8; i++)
                        {
                            if (get_string(xlsReader, i) != null && get_string(xlsReader, i) != "")
                                codiceNodo += get_string(xlsReader, i) + ".";
                        }

                        codiceNodo = codiceNodo.Substring(0, codiceNodo.Length - 1);
                        string[] codiceNodoSplit = codiceNodo.Split('.');
                        string codiceAmministrazione = get_string(xlsReader, 0);
                        string queryRegistro = "AND ID_REGISTRO IS NULL";
                        if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                            queryRegistro = "AND ID_REGISTRO = " + Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));

                        //Nodo primo livello
                        //if (codiceNodo.Length == 1)
                        if (codiceNodoSplit.Length == 1)
                        {
                            sl.Log("");
                            sl.Log("Inserimento Nodo con codice : " + codiceNodo);

                            //Creo il nodo da inserire
                            OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = codiceNodo;
                            nodoTitolarioDaInserire.CodiceAmministrazione = get_string(xlsReader, 0);
                            //nodoTitolarioDaInserire.CountChildNodiTitolario
                            if (get_string(xlsReader, 9).Equals("SI"))
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                            else
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;
                            nodoTitolarioDaInserire.Descrizione = get_string(xlsReader, 8);
                            //nodoTitolarioDaInserire.ID
                            //nodoTitolarioDaInserire.ID_TipoFascicolo
                            nodoTitolarioDaInserire.IDParentNodoTitolario = titolario.ID;
                            nodoTitolarioDaInserire.ID_Titolario = titolario.ID;
                            if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                            {
                                nodoTitolarioDaInserire.IDRegistroAssociato = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));
                            }
                            else
                            {
                                nodoTitolarioDaInserire.IDRegistroAssociato = "";
                            }
                            nodoTitolarioDaInserire.CodiceLivello = GetCodiceLivello("", "1", nodoTitolarioDaInserire.CodiceAmministrazione, titolario.ID, nodoTitolarioDaInserire.IDRegistroAssociato);
                            nodoTitolarioDaInserire.Livello = "1";
                            if (get_string(xlsReader, 10) != null && get_string(xlsReader, 10) != string.Empty)
                                nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(get_string(xlsReader, 10));
                            else
                                nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);
                            if (esito.Codice != 0)
                            {
                                sl.Log("ERRORE : " + esito.Descrizione);
                                error = true;
                            }
                            else
                            {
                                //Inserisco il nodo
                                //esito = amministrazione.InsertNodoTitolario(ref nodoTitolarioDaInserire);
                                DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                                bool result = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);
                                //if (esito.Codice == 0)
                                if (result)
                                {
                                    sl.Log("Inserimento Nodo con codice : " + codiceNodo + " effettuato correttamente.");

                                    //Verifico se impostare o meno la visibilità sul nodo
                                    if (nodoTitolarioDaInserire.IDRegistroAssociato != null)
                                    {
                                        //if (get_string(xlsReader, 11) != null && get_string(xlsReader, 11) != "")
                                        if (get_string(xlsReader, 11) != null)
                                        {
                                            //Imposto la visiblità
                                            impostaVisibilitaNodo(infoUtente, nodoTitolarioDaInserire, get_string(xlsReader, 11), sl, ref warning);
                                        }
                                    }
                                }
                                else
                                {
                                    sl.Log("ERRORE : " + esito.Descrizione);
                                    error = true;
                                }
                            }
                        }

                        //Nodo livello successivo al primo
                        //if (codiceNodo.Length > 1)
                        if (codiceNodoSplit.Length > 1)
                        {
                            //Costruisco il codice del nodo padre
                            string codicePadre = string.Empty;
                            //codicePadre = codiceNodo.Substring(0, codiceNodo.Length - 2);
                            codicePadre = getCodiceNodoPadre(codiceNodoSplit);

                            OrgNodoTitolario nodoTitolarioPadre = getNodoTitolario(codicePadre, codiceAmministrazione, queryRegistro, titolario.ID);
                            if (nodoTitolarioPadre != null)
                            {
                                //Controllo la presenza di tutti di dati del nodo padre necessari 
                                if (nodoTitolarioPadre.Livello != null && nodoTitolarioPadre.Livello != "" &&
                                    nodoTitolarioPadre.ID != null && nodoTitolarioPadre.ID != "")
                                {
                                    sl.Log("");
                                    sl.Log("Inserimento Nodo con codice : " + codiceNodo + " -- Codice Nodo padre : " + codicePadre);

                                    //Creo il nodo da inserire
                                    OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                                    nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                                    nodoTitolarioDaInserire.Codice = codiceNodo;
                                    nodoTitolarioDaInserire.CodiceAmministrazione = get_string(xlsReader, 0);

                                    //Creazione livello nodo
                                    int livello = Convert.ToInt32(nodoTitolarioPadre.Livello);
                                    livello++;

                                    //nodoTitolarioDaInserire.CountChildNodiTitolario
                                    if (get_string(xlsReader, 9).Equals("SI"))
                                        nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                                    else
                                        nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;
                                    nodoTitolarioDaInserire.Descrizione = get_string(xlsReader, 8);
                                    //nodoTitolarioDaInserire.ID
                                    //nodoTitolarioDaInserire.ID_TipoFascicolo
                                    nodoTitolarioDaInserire.IDParentNodoTitolario = nodoTitolarioPadre.ID;
                                    nodoTitolarioDaInserire.ID_Titolario = nodoTitolarioPadre.ID_Titolario;
                                    if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                                    {
                                        nodoTitolarioDaInserire.IDRegistroAssociato = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));
                                    }
                                    else
                                    {
                                        nodoTitolarioDaInserire.IDRegistroAssociato = "";
                                    }
                                    nodoTitolarioDaInserire.CodiceLivello = GetCodiceLivello(nodoTitolarioPadre.CodiceLivello, livello.ToString(), nodoTitolarioDaInserire.CodiceAmministrazione, nodoTitolarioPadre.ID_Titolario, nodoTitolarioDaInserire.IDRegistroAssociato);
                                    nodoTitolarioDaInserire.Livello = livello.ToString();
                                    if (get_string(xlsReader, 10) != null && get_string(xlsReader, 10) != string.Empty)
                                        nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(get_string(xlsReader, 10));
                                    else
                                        nodoTitolarioDaInserire.NumeroMesiConservazione = 0;

                                    //Verifico se è possibile inserire il nodo
                                    DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                                    esito = CanInsertTitolario(nodoTitolarioDaInserire);
                                    if (esito.Codice != 0)
                                    {
                                        sl.Log("ERRORE : " + esito.Descrizione);
                                        error = true;
                                    }
                                    else
                                    {
                                        //Inserisco il nodo
                                        //esito = amministrazione.InsertNodoTitolario(ref nodoTitolarioDaInserire);
                                        DocsPaDocumentale.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale.Documentale.TitolarioManager(infoUtente);
                                        bool result = titolarioManager.SaveNodoTitolario(nodoTitolarioDaInserire);
                                        //if (esito.Codice == 0)
                                        if (result)
                                        {
                                            sl.Log("Inserimento Nodo con codice : " + codiceNodo + " effettuato correttamente.");

                                            //Verifico se impostare o meno la visibilità sul nodo
                                            if (nodoTitolarioDaInserire.IDRegistroAssociato != null)
                                            {
                                                //if (get_string(xlsReader, 11) != null && get_string(xlsReader, 11) != "")
                                                if (get_string(xlsReader, 11) != null)
                                                {
                                                    //Imposto la visiblità
                                                    impostaVisibilitaNodo(infoUtente, nodoTitolarioDaInserire, get_string(xlsReader, 11), sl, ref warning);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sl.Log("ERRORE : " + esito.Descrizione);
                                            error = true;
                                        }
                                    }
                                }
                                else
                                {
                                    sl.Log("WARNING : Nodo con codice : " + codiceNodo + " non inserito, dati del nodo padre non tutti presenti o non coerenti");
                                    warning = true;
                                }
                            }
                            else
                            {
                                sl.Log("WARNING : Nodo con codice : " + codiceNodo + " non inserito padre " + codicePadre + " inesistente");
                                warning = true;
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione Titolario - " + System.DateTime.Now.ToString());
                #endregion Inserimento nodi di titolario

                #region Inserimento nodi di titolario vecchio commentato
                /*
                //Comincio la lettura del file appena scritto
                sl.Log("**** Inizio importazione titolario - " + System.DateTime.Now.ToString());

                xlsConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
                xlsConn.Open();
                
                OleDbCommand xlsCmd = new OleDbCommand("select * from [Titolario$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                while (xlsReader.Read())
                {
                    //Controllo campi obbligatori
                    if (get_string(xlsReader, 0) == "" || get_string(xlsReader, 2) == "" || get_string(xlsReader, 8) == "")
                    {
                        sl.Log("");
                        sl.Log("WARNING : Campi obbligatori non inseriti nel modello");
                        warning = true;
                    }
                    else
                    {
                        //Costruisco il codice del nodo
                        string codiceNodo = string.Empty;
                        for (int i = 2; i < 8; i++)
                        {
                            if (get_string(xlsReader, i) != null && get_string(xlsReader, i) != "")
                                codiceNodo += get_string(xlsReader, i) + ".";
                        }
                        
                        codiceNodo = codiceNodo.Substring(0, codiceNodo.Length - 1);
                        string[] codiceNodoSplit = codiceNodo.Split('.');
                        string codiceAmministrazione = get_string(xlsReader, 0);
                        string queryRegistro = "AND ID_REGISTRO IS NULL";
                        if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                            queryRegistro = "AND ID_REGISTRO = " + Utenti.RegistriManager.getIdRegistro(codiceAmministrazione,get_string(xlsReader, 1));

                        //Nodo primo livello
                        //if (codiceNodo.Length == 1)
                        if (codiceNodoSplit.Length == 1)
                        {
                            sl.Log("");
                            sl.Log("Inserimento Nodo con codice : " + codiceNodo);

                            //Creo il nodo da inserire
                            OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                            nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                            nodoTitolarioDaInserire.Codice = codiceNodo;
                            nodoTitolarioDaInserire.CodiceAmministrazione = get_string(xlsReader, 0);
                            //nodoTitolarioDaInserire.CountChildNodiTitolario
                            if (get_string(xlsReader, 9).Equals("SI"))
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                            else
                                nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;
                            nodoTitolarioDaInserire.Descrizione = get_string(xlsReader, 8);
                            //nodoTitolarioDaInserire.ID
                            //nodoTitolarioDaInserire.ID_TipoFascicolo
                            nodoTitolarioDaInserire.IDParentNodoTitolario = "0";
                            if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                            {
                                nodoTitolarioDaInserire.IDRegistroAssociato = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));
                            }
                            else
                            {
                                nodoTitolarioDaInserire.IDRegistroAssociato = "";
                            }
                            nodoTitolarioDaInserire.CodiceLivello = GetCodiceLivello("", "1", nodoTitolarioDaInserire.CodiceAmministrazione, "", nodoTitolarioDaInserire.IDRegistroAssociato);
                            nodoTitolarioDaInserire.Livello = "1";
                            nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(get_string(xlsReader, 10));

                            //Verifico se è possibile inserire il nodo
                            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                            esito = CanInsertTitolario(nodoTitolarioDaInserire);
                            if (esito.Codice != 0)
                            {
                                sl.Log("ERRORE : " + esito.Descrizione);
                                error = true;
                            }
                            else
                            {
                                //Inserisco il nodo
                                esito = amministrazione.InsertNodoTitolario(ref nodoTitolarioDaInserire);
                                if (esito.Codice == 0)
                                {
                                    sl.Log("Inserimento Nodo con codice : " + codiceNodo + " effettuato correttamente.");

                                    //Verifico se impostare o meno la visibilità sul nodo
                                    if (nodoTitolarioDaInserire.IDRegistroAssociato != null)
                                    {
                                        //if (get_string(xlsReader, 11) != null && get_string(xlsReader, 11) != "")
                                        if (get_string(xlsReader, 11) != null)
                                        {
                                            //Imposto la visiblità
                                            impostaVisibilitaNodo(nodoTitolarioDaInserire, get_string(xlsReader, 11), sl, ref warning);
                                        }
                                    }
                                }
                                else
                                {
                                    sl.Log("ERRORE : " + esito.Descrizione);
                                    error = true;
                                }
                            }
                        }

                        //Nodo livello successivo al primo
                        //if (codiceNodo.Length > 1)
                        if (codiceNodoSplit.Length > 1)
                        {
                            //Costruisco il codice del nodo padre
                            string codicePadre = string.Empty;
                            //codicePadre = codiceNodo.Substring(0, codiceNodo.Length - 2);
                            codicePadre = getCodiceNodoPadre(codiceNodoSplit);

                            OrgNodoTitolario nodoTitolarioPadre = getNodoTitolario(codicePadre, codiceAmministrazione, queryRegistro);
                            if (nodoTitolarioPadre != null)
                            {
                                //Controllo la presenza di tutti di dati del nodo padre necessari 
                                if (nodoTitolarioPadre.Livello != null && nodoTitolarioPadre.Livello != "" &&
                                    nodoTitolarioPadre.ID != null && nodoTitolarioPadre.ID != "")
                                {
                                    sl.Log("");
                                    sl.Log("Inserimento Nodo con codice : " + codiceNodo + " -- Codice Nodo padre : " + codicePadre);

                                    //Creo il nodo da inserire
                                    OrgNodoTitolario nodoTitolarioDaInserire = new OrgNodoTitolario();
                                    nodoTitolarioDaInserire.bloccaTipoFascicolo = "NO";
                                    nodoTitolarioDaInserire.Codice = codiceNodo;
                                    nodoTitolarioDaInserire.CodiceAmministrazione = get_string(xlsReader, 0);

                                    //Creazione livello nodo
                                    int livello = Convert.ToInt32(nodoTitolarioPadre.Livello);
                                    livello++;
                                    
                                    //nodoTitolarioDaInserire.CountChildNodiTitolario
                                    if (get_string(xlsReader, 9).Equals("SI"))
                                        nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = true;
                                    else
                                        nodoTitolarioDaInserire.CreazioneFascicoliAbilitata = false;
                                    nodoTitolarioDaInserire.Descrizione = get_string(xlsReader, 8);
                                    //nodoTitolarioDaInserire.ID
                                    //nodoTitolarioDaInserire.ID_TipoFascicolo
                                    nodoTitolarioDaInserire.IDParentNodoTitolario = nodoTitolarioPadre.ID;
                                    if (get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                                    {
                                        nodoTitolarioDaInserire.IDRegistroAssociato = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader, 1));
                                    }
                                    else
                                    {
                                        nodoTitolarioDaInserire.IDRegistroAssociato = "";
                                    }
                                    nodoTitolarioDaInserire.CodiceLivello = GetCodiceLivello(nodoTitolarioPadre.CodiceLivello, livello.ToString(), nodoTitolarioDaInserire.CodiceAmministrazione, "", nodoTitolarioDaInserire.IDRegistroAssociato);
                                    nodoTitolarioDaInserire.Livello = livello.ToString();
                                    nodoTitolarioDaInserire.NumeroMesiConservazione = Convert.ToInt32(get_string(xlsReader, 10));

                                    //Verifico se è possibile inserire il nodo
                                    DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();
                                    esito = CanInsertTitolario(nodoTitolarioDaInserire);
                                    if (esito.Codice != 0)
                                    {
                                        sl.Log("ERRORE : " + esito.Descrizione);
                                        error = true;
                                    }
                                    else
                                    {
                                        //Inserisco il nodo
                                        esito = amministrazione.InsertNodoTitolario(ref nodoTitolarioDaInserire);
                                        if (esito.Codice == 0)
                                        {
                                            sl.Log("Inserimento Nodo con codice : " + codiceNodo + " effettuato correttamente.");
                                           
                                            //Verifico se impostare o meno la visibilità sul nodo
                                            if (nodoTitolarioDaInserire.IDRegistroAssociato != null)
                                            {
                                                //if (get_string(xlsReader, 11) != null && get_string(xlsReader, 11) != "")
                                                if (get_string(xlsReader, 11) != null)
                                                {
                                                    //Imposto la visiblità
                                                   impostaVisibilitaNodo(nodoTitolarioDaInserire, get_string(xlsReader, 11), sl, ref warning);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sl.Log("ERRORE : " + esito.Descrizione);
                                            error = true;
                                        }
                                    }
                                }
                                else
                                {
                                    sl.Log("WARNING : Nodo con codice : " + codiceNodo + " non inserito, dati del nodo padre non tutti presenti o non coerenti");
                                    warning = true;
                                }    
                            }
                            else
                            {
                                sl.Log("WARNING : Nodo con codice : " + codiceNodo + " non inserito padre " + codicePadre + " inesistente");
                                warning = true;
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione Titolario - " + System.DateTime.Now.ToString());
                */
                #endregion Inserimento nodi di titolario vecchio commentato

                #region Inserimento fascicoli
                if (!warning && !error)
                {
                    //Importazione fascicoli
                    OleDbCommand xlsCmd_1 = new OleDbCommand("select * from [Fascicoli$]", xlsConn);
                    xlsReader_1 = xlsCmd_1.ExecuteReader();

                    sl.Log("");
                    sl.Log("**** Inizio importazione Fascicoli - " + System.DateTime.Now.ToString());
                    sl.Log("");
                                
                    while (xlsReader_1.Read())
                    {
                        //Controllo se siamo arrivati all'ultima riga
                        if (get_string(xlsReader_1, 0) == "/")
                            break;

                        //Verifico l'obbligatorietà dei campi cod_ruolo e cod_utente
                        if (get_string(xlsReader_1, 0) != null && get_string(xlsReader_1, 0) != "" &&
                            get_string(xlsReader_1, 1) != null && get_string(xlsReader_1, 1) != "" &&
                            get_string(xlsReader_1, 2) != null && get_string(xlsReader_1, 2) != "" &&
                            get_string(xlsReader_1, 3) != null && get_string(xlsReader_1, 3) != "" &&
                            get_string(xlsReader_1, 4) != null && get_string(xlsReader_1, 4) != "" &&
                            get_string(xlsReader_1, 5) != null && get_string(xlsReader_1, 5) != ""
                            )
                        {

                            string codiceNodo = get_string(xlsReader_1, 1);
                            OrgNodoTitolario nodoTitolario = null;
                            DocsPaVO.utente.Registro reg = null;
                            string codiceAmministrazione = get_string(xlsReader_1, 0);
                            string queryReg = string.Empty;
                        
                            if (get_string(xlsReader_1, 2) != null && get_string(xlsReader_1, 2) != "")
                            {
                                string systemIdReg = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader_1, 2));
                                reg = Utenti.RegistriManager.getRegistro(systemIdReg);
                                queryReg = " AND (ID_REGISTRO = " + systemIdReg + " OR ID_REGISTRO IS NULL)";
                            }                           

                            if(titolario != null)
                                nodoTitolario = getNodoTitolario(codiceNodo, codiceAmministrazione, queryReg,titolario.ID);

                            if (nodoTitolario != null && reg != null)
                            {
                                //Creo l'oggetto FasciolazioneClassificazione
                                DocsPaVO.fascicolazione.Classificazione classFasc = new DocsPaVO.fascicolazione.Classificazione();
                                classFasc.codice = codiceNodo;
                                classFasc.codUltimo = Fascicoli.FascicoloManager.getFascNumRif(nodoTitolario.ID, reg.systemId);
                                classFasc.descrizione = get_string(xlsReader_1, 5);
                                classFasc.registro = reg;
                                classFasc.systemID = nodoTitolario.ID;
                                classFasc.varcodliv1 = nodoTitolario.CodiceLivello;

                                //Creo l'oggetto fascicolo
                                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                                fascicolo.descrizione = get_string(xlsReader_1, 5);
                                fascicolo.codUltimo = classFasc.codUltimo;
                                fascicolo.cartaceo = false;
                                fascicolo.privato = "0";

                                string note = get_string(xlsReader_1, 6);
                                if (!string.IsNullOrEmpty(note))
                                    fascicolo.noteFascicolo = new DocsPaVO.Note.InfoNota[1] { new DocsPaVO.Note.InfoNota(note) };

                                fascicolo.apertura = System.DateTime.Today.ToString("dd/MM/yyyy");
                                fascicolo.idRegistroNodoTit = "";
                                fascicolo.idRegistro = reg.systemId;

                                //Creo gli oggetti UTENTE - RUOLO - INFOUTENTE
                                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                                string idAmm = amministrazione.GetIDAmm(nodoTitolario.CodiceAmministrazione);
                                DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader_1, 3), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                                DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader_1, 4), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                                DocsPaVO.utente.InfoUtente infoUtenteFascicolo = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                                //Procedo alla creazione del fascicolo
                                DocsPaVO.fascicolazione.ResultCreazioneFascicolo result = new DocsPaVO.fascicolazione.ResultCreazioneFascicolo();
                                Fascicoli.FascicoloManager.newFascicolo(classFasc, fascicolo, infoUtenteFascicolo, ruolo, false, out result);
                                if (result == DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                                {
                                    sl.Log("Fascicolo : " + fascicolo.descrizione + " inserito correttamente sotto il nodo : " + codiceNodo);
                                }
                                else
                                {
                                    sl.Log("ERRORE creazione fascicolo : " + fascicolo.descrizione + " " + result.ToString());                                
                                }
                            }
                        }
                        else
                        {
                            sl.Log("WARNING : Campi obbligatori non inseriti nel modello.");
                            warning = true;
                        }
                    }

                    sl.Log("");
                    sl.Log("**** Fine importazione Fascicoli - " + System.DateTime.Now.ToString());
                }
                #endregion Inserimento fascicoli
            }
            catch( Exception ex ) 
			{
                if (ex.Message.StartsWith("'Fascicoli$' is not a valid name."))
                {
                    return true;
                }
                else
                {
                    sl.Log("");
                    sl.Log("ERRROE : " + ex.Message);
                    return false;
                }
			} 
            finally 
			{
                if(xlsReader != null)
                    xlsReader.Close();
                if(xlsReader_1 != null)
                    xlsReader_1.Close();
                if(xlsConn != null)
				    xlsConn.Close();				
            }
            if (warning || error)
                return false;
            else
                return true;
        }

        private static void impostaVisibilitaNodo(DocsPaVO.utente.InfoUtente infoUtente, OrgNodoTitolario nodo, string codiciRuolo, DocsPaDB.Utils.SimpleLog sl, ref bool warning)
        {
            string[] codici = codiciRuolo.Split(';');
            System.Collections.ArrayList ruoli = GetRuoliInTitolario(nodo.ID, nodo.IDRegistroAssociato, null, null);
            

            if (!codici[0].Equals(""))
            {
                sl.Log("Imposto la visibilità del nodo : " + nodo.Codice + " ai ruoli : " + codiciRuolo);

                //Poichè la proprietà ruolo.Associato è di default = true setto a false 
                //quella dei ruoli che voglio associare ed in seguito inverto le selezioni
                System.Collections.ArrayList ruoliTitolarioAppoggio = new System.Collections.ArrayList();
                for (int i = 0; i < codici.Length; i++)
                {
                    foreach (OrgRuoloTitolario ruolo in ruoli)
                    {
                        if (codici[i].Trim(' ') == ruolo.Codice)
                        {
                            ruolo.Associato = true;
                            ruoliTitolarioAppoggio.Add(ruolo);
                        }
                    }
                }

                OrgRuoloTitolario[] ruoliTitolario = new OrgRuoloTitolario[ruoliTitolarioAppoggio.Count];
                ruoliTitolarioAppoggio.CopyTo(ruoliTitolario);

                EsitoOperazione[] esitiOperazione = UpdateRuoliTitolario(infoUtente, nodo.ID, ruoliTitolario, null, false, "", nodo.IDRegistroAssociato);

                //Verifico l'esito dell'update di visibilità sui nodi
                if (esitiOperazione.Length != 0)
                {
                    for (int i = 0; i < esitiOperazione.Length; i++)
                    {
                        EsitoOperazione esito = (EsitoOperazione)esitiOperazione[i];
                        sl.Log("ERRORE : Impostazione visibilità del nodo : " + nodo.Codice + " : " + esito.Descrizione);
                        warning = true;
                    }
                }
                else
                {
                    sl.Log("Visibilità del nodo : " + nodo.Codice + " impostata correttamente.");
                }
            }
        }

        /// <summary>
        /// Reperimento nodo titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public static OrgNodoTitolario getNodoTitolario(string idTitolario)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                return amm.getNodoTitolario(idTitolario);
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getNodoTitolario\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                return null;
            }
        }

        public static OrgNodoTitolario getNodoTitolario(string codice, string codiceAmministrazione, string queryReg,string idTitolario)
        {
            try
            {
                Model model = new Model();
                string idAmm = model.getIdAmmByCod(codiceAmministrazione);
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                return amm.getNodoTitolario(codice, idAmm, queryReg, idTitolario);
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getNodoTitolarioByCodice\" classe \"TitolarioManager\" ERRORE : " + e.Message); 
                return null;
            }
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString();
        }

        private static string GetCodiceLivello(string codliv_padre, string livello, string codAmm,string idTitolario,string idRegistro)
        {
            string codliv = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                codliv = dbAmm.GetCodiceLivello(codliv_padre, livello, codAmm, idTitolario, idRegistro);

                int lung = codliv.Length;
                switch (lung)
                {
                    case 1:
                        codliv = codliv_padre + "000" + codliv;
                        break;
                    case 2:
                        codliv = codliv_padre + "00" + codliv;
                        break;
                    case 3:
                        codliv = codliv_padre + "0" + codliv;
                        break;
                    case 4:
                        codliv = codliv_padre + codliv;
                        break;
                }
            }
            catch(Exception e)
            {
                logger.Debug("Metodo \"GetCodiceLivello\" classe \"TitolarioManager\" ERRORE : " + e.Message); 
            }

            codliv = codliv.Replace("%", "");
            return codliv;
        }

        public static bool modificaTemplateExcelTitolario(string codice, string descrizione, string nomeFile, string serverPath, DocsPaVO.utente.InfoUtente infoUtente)
        {
            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;

            try
            {
                if (codice != "0")
                {
                    logger.Debug("Stringa connessione template EXCEL : Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "';");
                    xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "';";
                    xlsConn.Open();
                    logger.Debug("Connessione aperta template EXCEL");
                    
                    //Effettuo l'update del template excel con il codice di secondo livello inserito dall'utente
                    OleDbCommand uCmd = new OleDbCommand("UPDATE [Titolario$] SET Liv2=@CODICE", xlsConn);
                    uCmd.Parameters.Add("@CODICE", codice);
                    uCmd.ExecuteNonQuery();

                    //Effettuo l'update del template excel con la descrizione inserita dall'utente, solo per il primo nodo
                    OleDbCommand uCmd_1 = new OleDbCommand("UPDATE [Titolario$] SET DESCRIZIONE=@DESCRIZIONE WHERE Liv3 IS NULL", xlsConn);
                    uCmd_1.Parameters.Add("@DESCRIZIONE", descrizione);
                    uCmd_1.ExecuteNonQuery();

                    //Effettua l'update del template excel per quanto riguarda il codice del nodo del foglio fascicoli
                    OleDbCommand xlsCmd = new OleDbCommand("select * from [Fascicoli$]", xlsConn);
                    xlsReader = xlsCmd.ExecuteReader();

                    logger.Debug("Inizio lettura template EXCEL");
                    while (xlsReader.Read())
                    {
                        //Costruisco il codice del nodo
                        string codiceNodo_1 = get_string(xlsReader, 1);

                        if (codiceNodo_1.Length > 1)
                        {
                            string[] codiceNodo_2 = codiceNodo_1.Split('.');
                            string codiceNodo_3 = String.Empty;

                            for (int i = 0; i < codiceNodo_2.Length; i++)
                            {
                                if (i == 1)
                                    codiceNodo_3 += codice + ".";
                                else
                                    codiceNodo_3 += codiceNodo_2[i] + ".";
                            }
                            codiceNodo_3 = codiceNodo_3.Substring(0, codiceNodo_3.Length - 1);

                            //Effettuo l'update del codice nodo
                            OleDbCommand uCmd_2 = new OleDbCommand("UPDATE [Fascicoli$] SET CODICE_NODO=@NUOVO_COD_NODO WHERE CODICE_NODO = @VECCHIO_COD_NODO", xlsConn);
                            uCmd_2.Parameters.Add("@NUOVO_COD_NODO", codiceNodo_3);
                            uCmd_2.Parameters.Add("@VECCHIO_COD_NODO", codiceNodo_1);
                            uCmd_2.ExecuteNonQuery();
                        }
                    }
                    logger.Debug("Fine lettura template EXCEL");
                        
                    xlsConn.Close();
                    logger.Debug("Connessione chiusa template EXCEL");
                }

                StreamReader sr = new StreamReader(serverPath + "\\Modelli\\Import\\" + nomeFile);
                Stream fs = sr.BaseStream;
                byte[] dati = new byte[fs.Length];
                fs.Read(dati, 0, (int)fs.Length);
                fs.Close();

                bool result = importaTitolario(dati, nomeFile, serverPath, infoUtente);
                return result;
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"modificaTemplateExcelTitolario\" classe \"TitolarioManager\" ERRORE : " + ex.Message);
                return false;
            }
            finally
            {
                xlsConn.Close();
            }
        }

        public static System.Collections.ArrayList getLogImportTitolario(string serverPath)
        {
            System.Collections.ArrayList fileLog = new System.Collections.ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportazioneTitolario.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getLogImportTitolario\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                return fileLog;
            }
        }

        public static string getCodiceNodoPadre(string[] codiceNodoSplit)
        {
            string result = string.Empty;

            switch (codiceNodoSplit.Length)
            {
                case 2:
                    result += codiceNodoSplit[0].ToString();
                    break;

                case 3:
                    result += codiceNodoSplit[0].ToString();
                    result += ".";
                    result += codiceNodoSplit[1].ToString();                    
                    break;

                case 4:
                    result += codiceNodoSplit[0].ToString();
                    result += ".";
                    result += codiceNodoSplit[1].ToString();
                    result += ".";
                    result += codiceNodoSplit[2].ToString();                    
                    break;

                case 5:
                    result += codiceNodoSplit[0].ToString();
                    result += ".";
                    result += codiceNodoSplit[1].ToString();
                    result += ".";
                    result += codiceNodoSplit[2].ToString();
                    result += ".";
                    result += codiceNodoSplit[3].ToString();                    
                    break;

                case 6:
                    result += codiceNodoSplit[0].ToString();
                    result += ".";
                    result += codiceNodoSplit[1].ToString();
                    result += ".";
                    result += codiceNodoSplit[2].ToString();
                    result += ".";
                    result += codiceNodoSplit[3].ToString();
                    result += ".";
                    result += codiceNodoSplit[4].ToString();                    
                    break;
            }

            return result;
        }

        #endregion Fine Importazione titolario da foglio excel

        #region Importazione Fascicoli
        public static bool importFascicoliDocumenti(byte[] dati, string nomeFile, string serverPath)
        {
            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            OleDbDataReader xlsReader_1 = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione amministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");
            
            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportFascicoli");
            
            try
            {
                #region Scrittura del file
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.Write);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
                #endregion Scrittura del file

                #region Inserimento fascicoli
                
                //Importazione fascicoli
                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1';";
                xlsConn.Open();

                OleDbCommand xlsCmd_1 = new OleDbCommand("select * from [Fascicoli$]", xlsConn);
                xlsReader_1 = xlsCmd_1.ExecuteReader();

                sl.Log("");
                sl.Log("**** Inizio importazione Fascicoli - " + System.DateTime.Now.ToString());
                sl.Log("");

                while (xlsReader_1.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader_1, 0) == "/")
                        break;

                    //Verifico l'obbligatorietà dei campi cod_ruolo e cod_utente
                    if (get_string(xlsReader_1, 0) != null && get_string(xlsReader_1, 0) != "" &&
                        get_string(xlsReader_1, 1) != null && get_string(xlsReader_1, 1) != "" &&
                        get_string(xlsReader_1, 2) != null && get_string(xlsReader_1, 2) != "" &&
                        get_string(xlsReader_1, 3) != null && get_string(xlsReader_1, 3) != "" &&
                        get_string(xlsReader_1, 4) != null && get_string(xlsReader_1, 4) != "" &&
                        get_string(xlsReader_1, 5) != null && get_string(xlsReader_1, 5) != ""
                        )
                    {

                        string codiceNodo = get_string(xlsReader_1, 1);
                        DocsPaVO.utente.Registro reg = null;
                        string codiceAmministrazione = get_string(xlsReader_1, 0);
                        string queryReg = string.Empty;

                        if (get_string(xlsReader_1, 2) != null && get_string(xlsReader_1, 2) != "")
                        {
                            string systemIdReg = Utenti.RegistriManager.getIdRegistro(codiceAmministrazione, get_string(xlsReader_1, 2));
                            reg = Utenti.RegistriManager.getRegistro(systemIdReg);
                            queryReg = " AND (ID_REGISTRO = " + systemIdReg + " OR ID_REGISTRO IS NULL)";
                        }

                        //Seleziono il titolario attivo
                        OrgTitolario titolario = new OrgTitolario();
                        string idAmm = amministrazione.GetIDAmm(codiceAmministrazione);
                        titolario = amministrazione.getTitolarioAttivo(idAmm);
                        titolario.CodiceAmministrazione = codiceAmministrazione;

                        //Seleziono il nodo di titolario
                        OrgNodoTitolario nodoTitolario = null;
                        nodoTitolario = getNodoTitolario(codiceNodo, codiceAmministrazione, queryReg, titolario.ID);

                        if (nodoTitolario != null && reg != null)
                        {
                            //Creo l'oggetto FasciolazioneClassificazione
                            DocsPaVO.fascicolazione.Classificazione classFasc = new DocsPaVO.fascicolazione.Classificazione();
                            classFasc.codice = codiceNodo;
                            if (get_string(xlsReader_1, 7) != null && get_string(xlsReader_1, 7) != "")
                                classFasc.codUltimo = get_string(xlsReader_1, 7);
                            else
                                classFasc.codUltimo = Fascicoli.FascicoloManager.getFascNumRif(nodoTitolario.ID, reg.systemId);
                            classFasc.descrizione = get_string(xlsReader_1, 5);
                            classFasc.registro = reg;
                            classFasc.systemID = nodoTitolario.ID;
                            classFasc.varcodliv1 = nodoTitolario.CodiceLivello;

                            //Creo l'oggetto fascicolo
                            DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                            fascicolo.descrizione = get_string(xlsReader_1, 5);
                            fascicolo.codUltimo = classFasc.codUltimo;
                            fascicolo.cartaceo = false;
                            fascicolo.privato = "0";

                            string note = get_string(xlsReader_1, 6);
                            if (!string.IsNullOrEmpty(note))
                                fascicolo.noteFascicolo = new DocsPaVO.Note.InfoNota[1] { new DocsPaVO.Note.InfoNota(note) };

                            fascicolo.apertura = System.DateTime.Today.ToString("dd/MM/yyyy");
                            fascicolo.idRegistroNodoTit = nodoTitolario.IDRegistroAssociato;
                            fascicolo.idRegistro = reg.systemId;

                            //Creo gli oggetti UTENTE - RUOLO - INFOUTENTE
                            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader_1, 3), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                            DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader_1, 4), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                            //Procedo alla creazione del fascicolo
                            DocsPaVO.fascicolazione.ResultCreazioneFascicolo result = new DocsPaVO.fascicolazione.ResultCreazioneFascicolo();
                            Fascicoli.FascicoloManager.newFascicolo(classFasc, fascicolo, infoUtente, ruolo, false, out result);
                            if (result == DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                            {
                                sl.Log("Fascicolo : " + fascicolo.descrizione + " inserito correttamente sotto il nodo : " + codiceNodo);
                            }
                            else
                            {
                                sl.Log("ERRORE creazione fascicolo : " + fascicolo.descrizione + " " + result.ToString());
                            }
                        }
                    }
                    else
                    {
                        sl.Log("ERRORE-ImportFascicoli : Campi obbligatori non inseriti nel modello.");
                        return false;
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione Fascicoli - " + System.DateTime.Now.ToString());

                #endregion Inserimento fascicoli

                #region Inserimento documenti
                //Importazione documenti
                OleDbCommand xlsCmd = new OleDbCommand("select * from [Documenti$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                sl.Log("");
                sl.Log("**** Inizio importazione Documenti - " + System.DateTime.Now.ToString());
                
                while (xlsReader.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //Controllo che esiste un path per il file e che il file esista nel path specificato
                    byte[] content = null;
                    if (get_string(xlsReader, 6) != null && get_string(xlsReader, 6) != "")
                    {
                        try
                        {
                            content = File.ReadAllBytes(get_string(xlsReader, 6));
                        }
                        catch (Exception ex)
                        {
                            sl.Log("");
                            sl.Log("**** File non trovato - " + get_string(xlsReader, 6));
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                    //Verifico l'obbligatorietà dei campi cod_ruolo e cod_utente
                    if (get_string(xlsReader, 0) != null && get_string(xlsReader, 0) != "" &&
                        get_string(xlsReader, 2) != null && get_string(xlsReader, 2) != "" &&
                        get_string(xlsReader, 4) != null && get_string(xlsReader, 4) != "" &&
                        get_string(xlsReader, 5) != null && get_string(xlsReader, 5) != ""
                        )
                    {
                        //Recupero l'idAmministrazione
                        string codiceAmministrazione = get_string(xlsReader, 0);
                        string idAmm = amministrazione.GetIDAmm(codiceAmministrazione);

                        //Creo, valorizzo e salvo una scheda documento (DocumentoGrigio)
                        DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader, 4), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                        DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReader, 5), DocsPaVO.addressbook.TipoUtente.INTERNO, idAmm);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
                        schedaDocumento.appId = "ACROBAT";
                        schedaDocumento.idPeople = infoUtente.idPeople;
                        schedaDocumento.typeId = "LETTERA";
                        schedaDocumento.userId = infoUtente.userId;
                        DocsPaVO.documento.Oggetto oggetto = new DocsPaVO.documento.Oggetto();
                        oggetto.descrizione = get_string(xlsReader, 2);
                        schedaDocumento.oggetto = oggetto;
                        if (get_string(xlsReader, 7) != null && get_string(xlsReader, 7) != "")
                        {
                            string idPeopleDelegato = "";

                            if (infoUtente.delegato != null)
                                idPeopleDelegato = infoUtente.delegato.idPeople;
                            DocsPaVO.Note.InfoNota nota = new DocsPaVO.Note.InfoNota(get_string(xlsReader, 7), utente.systemId, ruolo.systemId, DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti, idPeopleDelegato);


                            schedaDocumento.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                            schedaDocumento.noteDocumento.Add(nota);
                        }

                        DocsPaVO.documento.SchedaDocumento schedaResult = Documenti.DocSave.addDocGrigia(schedaDocumento, infoUtente, ruolo);
                        
                        if (schedaResult != null && schedaResult.docNumber != null && schedaResult.docNumber != "")
                        {
                            sl.Log("");
                            sl.Log("**** Creato Documento (DOCNUMBER) - " + schedaResult.docNumber.ToString());

                            //Acquisisco il file per il documento creato
                            if (get_string(xlsReader, 6) != null && get_string(xlsReader, 6) != "")
                            {
                                string[] fileName = get_string(xlsReader, 6).Split('\\');
                                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
                                fileDoc.name = fileName[fileName.Length - 1].ToString();
                                fileDoc.cartaceo = false;
                                fileDoc.fullName = get_string(xlsReader, 6);
                                fileDoc.content = content;
                                fileDoc.length = fileDoc.content.Length;
                                DocsPaVO.documento.FileRequest fileReq = (DocsPaVO.documento.FileRequest)schedaResult.documenti[0];
                                fileReq.cartaceo = false;
                                fileReq.fileName = fileName[fileName.Length - 1].ToString();
                                Documenti.FileManager.putFile(fileReq, fileDoc, infoUtente);
                                sl.Log("");
                                sl.Log("**** Acquisito Documento - " + fileDoc.name.ToString());
                            }
                            else
                            {
                                sl.Log("");
                                sl.Log("**** WARNING : Per il Documento (oggetto) - " + get_string(xlsReader, 2) + " file non acquisito,  \"Path File\" non presente");
                            }

                            if (get_string(xlsReader, 3) != null && get_string(xlsReader, 3) != "")
                            {
                                //Classifico il documento
                                ArrayList titolari = getTitolari(idAmm);
                                DocsPaVO.amministrazione.OrgTitolario titolarioAttivo = null;
                                foreach (DocsPaVO.amministrazione.OrgTitolario titolario in titolari)
                                {
                                    if (titolario.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                                        titolarioAttivo = titolario;
                                }
                                if (titolarioAttivo != null)
                                {
                                    DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
                                    fascicolo = Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, get_string(xlsReader, 3), null, false, false);
                                    if (fascicolo != null)
                                    {
                                        DocsPaVO.fascicolazione.Folder folder = null;
                                        folder = Fascicoli.FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                                        if (folder != null)
                                        {
                                            string msg = string.Empty;
                                            bool result = false;
                                            result = Fascicoli.FolderManager.addDocFolder(infoUtente, schedaResult.systemId, folder.systemID, false, out msg);
                                            if (result)
                                            {
                                                sl.Log("");
                                                sl.Log("**** Documento : (" + schedaResult.docNumber + ") classificato");
                                            }
                                            else
                                            {
                                                sl.Log("");
                                                sl.Log("**** Documento : (" + schedaResult.docNumber + ") non classificato");
                                            }
                                        }
                                        else
                                        {
                                            sl.Log("");
                                            sl.Log("**** WARNING : Nessun \"Folder\" per il fascicolo selezionato");
                                        }
                                    }
                                    else
                                    {
                                        sl.Log("");
                                        sl.Log("**** WARNING : Sul titolario attivo non è presente un fascicolo con il codice : " + get_string(xlsReader, 3));
                                    }
                                }
                                else
                                {
                                    sl.Log("");
                                    sl.Log("**** WARNING : Nessun titolario attivo sul quale classificare");
                                }
                            }
                            else
                            {
                                sl.Log("");
                                sl.Log("**** WARNING : Codice fascicolo non inserito nel file di importazione");
                            }
                        }
                        else
                        {
                            sl.Log("");
                            sl.Log("**** WARNING : Documento (oggetto) - " + get_string(xlsReader, 2) + " non creato");
                        }
                    }
                    else
                    {
                        sl.Log("ERRORE-ImportDocumenti : Campi obbligatori non inseriti nel modello.");
                        return false;
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione Documenti - " + System.DateTime.Now.ToString());

                #endregion Inserimento documenti
            }
            catch( Exception ex ) 
			{
                sl.Log("");
                sl.Log("ERRRORE : " + ex.Message);
                return false;
            } 
            finally 
			{
                if(xlsReader_1 != null)
                    xlsReader_1.Close();
                if(xlsConn != null)
				    xlsConn.Close();				
            }

            return true;                
        }       

        public static System.Collections.ArrayList getLogImportFascicoli(string serverPath)
        {
            System.Collections.ArrayList fileLog = new System.Collections.ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportFascicoli.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getLogImportFascicoli\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                return fileLog;
            }
        }
        #endregion Importazione Fascicoli

        #region Importazione Indice Sistematico
        public static bool importaIndice(byte[] dati, string nomeFile, string serverPath, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            bool warning = false;
            bool error = false;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");
                
            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportazioneIndice");
            
            try
            {
                #region Scrittura del file
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
                #endregion Scrittura del file

                #region Inserimento indice
                //Comincio la lettura del file appena scritto
                sl.Log("**** Inizio importazione Indice Sistematico - " + System.DateTime.Now.ToString());

                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1';";
                xlsConn.Open();

                OleDbCommand xlsCmd = new OleDbCommand("select * from [NODO-VOCI$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                while (xlsReader.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //Controllo campi obbligatori
                    if (get_string(xlsReader, 0) == "" //|| 
                        //get_string(xlsReader, 1) == "" || 
                        //get_string(xlsReader, 2) == "" ||
                        //get_string(xlsReader, 3) == ""
                        )
                    {
                        sl.Log("");
                        sl.Log("WARNING : Campi obbligatori non inseriti nel modello");
                        warning = true;
                    }
                    else
                    {
                        //Recupero il codice del nodo
                        string codiceNodo = get_string(xlsReader, 0);

                        string queryReg = string.Empty;
                        if (!string.IsNullOrEmpty(get_string(xlsReader, 2)))
                        {
                            //Recupero l'id
                            string idRegistro = Utenti.RegistriManager.getIdRegistro(titolario.CodiceAmministrazione, get_string(xlsReader, 2));
                            queryReg = " AND ID_REGISTRO = " + idRegistro + " ";
                        }
                        
                        //Recupero il nodo di titolario a partire dal codice
                        OrgNodoTitolario nodo = Amministrazione.TitolarioManager.getNodoTitolario(codiceNodo, titolario.CodiceAmministrazione, queryReg, titolario.ID);

                        if (nodo == null)
                        {
                            sl.Log(String.Format("Nodo di titolario {0} non trovato", codiceNodo));
                            error = true;
                        }
                        else
                        {

                            //Recupero l'array delle voci di indice
                            string[] vociIndice = get_string(xlsReader, 3).Split(';');

                            //Provvedo all'inserimento delle voci di titolario
                            foreach (string voce in vociIndice)
                            {
                                //Se la voce esiste effettuo solo l'associazione al nodo specificato,
                                //altrimenti, prima inserisco la voce e poi effettuo l'associazione.
                                DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice = new DocsPaVO.fascicolazione.VoceIndiceSistematico();
                                voceIndice.codiceNodo = codiceNodo;
                                voceIndice.voceIndice = voce;
                                voceIndice.idTitolario = titolario.ID;
                                voceIndice.idProject = nodo.ID;
                                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                                voceIndice.idAmm = amm.GetIDAmm(titolario.CodiceAmministrazione);

                                DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndiceRicercata = BusinessLogic.fascicoli.IndiceSistematico.existVoceIndice(voceIndice);
                                if (voceIndiceRicercata != null)
                                {
                                    voceIndiceRicercata.idProject = nodo.ID;

                                    //Verifico che la voce non è già associata
                                    if (!BusinessLogic.fascicoli.IndiceSistematico.isAssociataVoce(voceIndiceRicercata, true))
                                    {
                                        //Associo la voce al nodo selezionato
                                        BusinessLogic.fascicoli.IndiceSistematico.associaVoceIndice(voceIndiceRicercata);

                                        sl.Log("");
                                        sl.Log("Associata voce di Indice : \"" + voceIndiceRicercata.voceIndice + "\" al nodo : \"" + nodo.Descrizione + "\"");
                                    }
                                }
                                else
                                {
                                    if (voceIndice.voceIndice != null && voceIndice.voceIndice != "")
                                    {
                                        //Inserisco la voce di indice
                                        BusinessLogic.fascicoli.IndiceSistematico.addNuovaVoceIndice(voceIndice);

                                        sl.Log("");
                                        sl.Log("Inserita voce di Indice : \"" + voceIndice.voceIndice);

                                        DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndiceAppenaInserita = BusinessLogic.fascicoli.IndiceSistematico.existVoceIndice(voceIndice);
                                        if (voceIndiceAppenaInserita != null)
                                        {
                                            voceIndiceAppenaInserita.idProject = nodo.ID;

                                            //Associo la voce al nodo selezionato
                                            //Non è necessaria la verifica di associazione in quanto la voce è stata appena inserita
                                            BusinessLogic.fascicoli.IndiceSistematico.associaVoceIndice(voceIndiceAppenaInserita);

                                            sl.Log("");
                                            sl.Log("Associata voce di Indice : \"" + voceIndiceAppenaInserita.voceIndice + "\" al nodo : \"" + nodo.Descrizione + "\"");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione Indice Sistematico - " + System.DateTime.Now.ToString());
                #endregion Inserimento indice

            }
            catch (Exception ex)
            {
                sl.Log("");
                sl.Log("ERRROE : " + ex.Message);
                return false;
            }
            finally
            {
                if (xlsReader != null)
                    xlsReader.Close();
                if (xlsConn != null)
                    xlsConn.Close();
            }
            if (warning || error)
                return false;
            else
                return true;
        }

        public static System.Collections.ArrayList getLogImportIndice(string serverPath)
        {
            System.Collections.ArrayList fileLog = new System.Collections.ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportazioneIndice.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getLogImportIndice\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                return fileLog;
            }
        }

        #endregion Importazione Indice Sistematico

        public static void setEtichetteTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.setEtichetteTitolario(titolario);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Metodo \"setEtichetteTitolario\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                }
            }
        }

        public static int getContatoreProtTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    int numProtTit = amm.getContatoreProtTitolario(nodoTitolario);
                    transactionContext.Complete();
                    return numProtTit;
                }
                catch (Exception e)
                {
                    logger.Debug("Metodo \"getContatoreProtTitolario\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                    return 0;
                }
            }
        }

        public static ArrayList getNodiFromProtoTit(DocsPaVO.utente.Registro registro, string idAmministrazione, string numProtoPratica, string idTitolario)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    return amm.getNodiFromProtoTit(registro, idAmministrazione, numProtoPratica, idTitolario);
                }
                catch (Exception e)
                {
                    logger.Debug("Metodo \"getNodiFromProtoTit\" classe \"TitolarioManager\" ERRORE : " + e.Message);
                    return null;
                }
            }
        }


    }
}
