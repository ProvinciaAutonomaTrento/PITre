using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.LibroFirma;

namespace BusinessLogic.LibroFirma
{
    public class LibroFirmaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(LibroFirmaManager));

        /// <summary>
        /// Metodo per l'estrazione degli eventi di notifica
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<AnagraficaEventi> GetEventNotification(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<AnagraficaEventi> listEventTypes = new List<AnagraficaEventi>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listEventTypes = libroFirma.GetEventNotification(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetEventNotification ", e);
            }
            return listEventTypes;
        }

        /// <summary>
        /// Metodo per l'estrazione dei tipo ruoli
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static List<DocsPaVO.utente.TipoRuolo> GetTypeRole(string idAmm)
        {
            List<DocsPaVO.utente.TipoRuolo> listTypeRole = new List<DocsPaVO.utente.TipoRuolo>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listTypeRole = libroFirma.GetTypeRoleByIdAmm(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetTypeRole ", e);
            }
            return listTypeRole;
        }
        /// <summary>
        /// Metodo per l'estrazione degli eventi
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<AnagraficaEventi> GetEventTypes(string eventType, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<AnagraficaEventi> listEventTypes = new List<AnagraficaEventi>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listEventTypes = libroFirma.GetEventTypes(eventType, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetEventTypes ", e);
            }
            return listEventTypes;
        }

        /// <summary>
        /// Metodo per l'estrazione dei processi di firma visibili al ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<ProcessoFirma> GetProcessesSignatureVisibleRole(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessesSignatureVisibleRole(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessesSignatureVisibleRole ", e);
            }
            return listProcessiDiFirma;
        }

        /// <summary>
        /// Metodo per l'estrazione degli schemi di processi di firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<ProcessoFirma> GetProcessiDiFirma(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessiDiFirma(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirma ", e);
            }
            return listProcessiDiFirma;
        }

        public static List<ProcessoFirma> GetProcessiDiFirmaByRuoloTitolare(string idRuoloTitolare)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessiDiFirmaByRuoloTitolare(idRuoloTitolare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByRuoloTitolare ", e);
            }
            return listProcessiDiFirma;
        }

        public static int GetCountProcessiDiFirmaByRuoloTitolare(string idRuoloTitolare)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetCountProcessiDiFirmaByRuoloTitolare(idRuoloTitolare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByRuoloTitolare ", e);
                return 0;
            }
        }

        public static List<ProcessoFirma> GetProcessiDiFirmaByUtenteTitolare(string idUtenteTitolare, string idRuoloCoinvolto)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessiDiFirmaByUtenteTitolare(idUtenteTitolare, idRuoloCoinvolto);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByUtenteTitolare ", e);
            }
            return listProcessiDiFirma;
        }

        public static int GetCountProcessiDiFirmaByUtenteTitolare(string idUtenteTitolare, string idRuoloCoinvolto)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetCountProcessiDiFirmaByUtenteTitolare(idUtenteTitolare, idRuoloCoinvolto);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByUtenteTitolare ", e);
                return 0;
            }
        }

        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByRuoloTitolare(string idRuoloTitolare)
        {
            List<IstanzaProcessoDiFirma> listProcessiDiFirma = new List<IstanzaProcessoDiFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetIstanzaProcessiDiFirmaByRuoloTitolare(idRuoloTitolare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetPassiDiFirmaByRuoloTitolare ", e);
            }
            return listProcessiDiFirma;
        }

        public static int GetCountIstanzaProcessiDiFirmaByRuoloTitolare(string idRuoloTitolare)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetCountIstanzaProcessiDiFirmaByRuoloTitolare(idRuoloTitolare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetPassiDiFirmaByRuoloTitolare ", e);
                return 0;
            }
        }

        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            List<IstanzaProcessoDiFirma> listProcessiDiFirma = new List<IstanzaProcessoDiFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetIstanzaProcessiDiFirmaByUtenteTitolare(idUtenteCoinvolto, idRuoloCoinvolto);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetPassiDiFirmaByRuoloTitolare ", e);
            }
            return listProcessiDiFirma;
        }

        public static int GetCountIstanzaProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetCountIstanzaProcessiDiFirmaByUtenteTitolare(idUtenteCoinvolto, idRuoloCoinvolto);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCountIstanzaProcessiDiFirmaByUtenteTitolare ", e);
                return 0;
            }
        }

        public static bool TickPasso(string[] idPassi, string tipoTick)
        {
            bool retVal = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retVal = libroFirma.TickPasso(idPassi, tipoTick);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: TickPasso ", e);
            }
            return retVal;
        }
        /// <summary>
        /// Creazione di un processo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="?"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ProcessoFirma InsertProcessoDiFirma(ProcessoFirma processo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                processo = libroFirma.InsertProcessoDiFirma(processo, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InsertProcessoDiFirma ", e);
            }
            return processo;
        }

        /// <summary>
        /// Se esiste un passo di firma/evento per cui non è definito il ruolo coinvolto, allora è un modello di processo 
        /// </summary>
        /// <returns></returns>
        private static bool IsProcessModel(List<PassoFirma> passiFirma)
        {
            bool result = false;

            result = (from p in passiFirma
                      where !p.Evento.TipoEvento.Equals("W")
                          && (p.ruoloCoinvolto == null || string.IsNullOrEmpty(p.ruoloCoinvolto.idGruppo))
                          && (p.TpoRuoloCoinvolto == null || !string.IsNullOrEmpty(p.TpoRuoloCoinvolto.systemId))
                      select p).FirstOrDefault() != null;

            return result;
        }

        /// <summary>
        /// Inserisce la visibilita del processo al corrispondente
        /// </summary>
        /// <param name="listaCorr"></param>
        /// <param name="idProcesso"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool InsertVisibilitaProcesso(List<DocsPaVO.utente.Corrispondente> listaCorr, string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.InsertVisibilitaProcesso(listaCorr, idProcesso, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InsertVisibilitaProcesso ", e);
                return false;
            }
        }

        /// <summary>
        /// Rimuove la visibilita del processo per il corrispondentein input
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idCorr"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RimuoviVisibilitaProcesso(string idProcesso, string idCorr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.RimuoviVisibilitaProcesso(idProcesso, idCorr, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: RimuoviVisibilitaProcesso ", e);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Estrae i corrispondenti aventi visibilità sul processo
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<DocsPaVO.utente.Corrispondente> GetVisibilitaProcesso(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.utente.Corrispondente> listCorr = new List<DocsPaVO.utente.Corrispondente>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listCorr = libroFirma.GetVisibilitaProcesso(idProcesso, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetVisibilitaProcesso ", e);
            }
            return listCorr;
        }

        /// <summary>
        /// Creazione di un passo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="?"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static PassoFirma InserisciPassoDiFirma(PassoFirma passo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    passo = libroFirma.InsertPassoDiFirma(passo, infoUtente);
                    if (!libroFirma.AggiornaTipoProcessoFirma(passo.idProcesso, infoUtente))
                    {
                        transactionContext.Dispose();
                        throw new Exception("Errore nell'aggiornamento del tipo di processo di firma");
                    }

                    transactionContext.Complete();
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InsertProcessoDiFirma ", e);
            }
            return passo;
        }

        public static ProcessoFirma GetProcessoDiFirmaById(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            ProcessoFirma processo = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                processo = libroFirma.GetProcessoDiFirmaById(idProcesso, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessoDiFirma ", e);
            }
            return processo;
        }

        /// <summary>
        /// Creazione di un nuovo elemento in libro firma
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="?"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool InserimentoInLibroFirma(ElementoInLibroFirma elemento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.InsertElementoManualeInLibroFirma(elemento, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InserimentoInLibroFirma ", e);
            }
            return result;
        }
        /// <summary>
        /// Rimuove il processo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RimuoviProcessoDiFirma(ProcessoFirma processo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.RimuoviProcessoDiFirma(processo, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: RimuoviProcessoDiFirma ", e);
                return false;
            }
            return retValue;
        }


        public static bool AggiornaDataEsecuzioneElemento(string docnumber, string stato)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.AggiornaDataEsecuzioneElemento(docnumber, stato);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaDataEsecuzioneElemento ", e);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Metodo per l'aggiornamento della colonna di errore in caso di esito negativo nella procedura di firma
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="msgError"></param>
        public static void AggiornaErroreEsitoFirma(string docnumber, string msgError)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                libroFirma.AggiornaErroreEsitoFirma(docnumber, msgError);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaErroreEsitoFirma ", e);
            }
        }
        /// <summary>
        /// Rimuove il passo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RimuoviPassoDiFirma(PassoFirma passo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                 // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    retValue = libroFirma.RimuoviPassoDiFirma(passo, infoUtente);
                    if (retValue)
                    {
                        if (!libroFirma.AggiornaTipoProcessoFirma(passo.idProcesso, infoUtente))
                        {
                            retValue = false;
                        }
                    }

                    if (retValue)
                        transactionContext.Complete();
                    else
                        transactionContext.Dispose();
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: RimuoviPassoDiFirma ", e);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Aggiorna il passo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool AggiornaPassoDiFirma(PassoFirma passo, int oldNumeroSequenza, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    retValue = libroFirma.AggiornaPassoDiFirma(passo, oldNumeroSequenza, infoUtente);
                    if (retValue)
                    {
                        if (!libroFirma.AggiornaTipoProcessoFirma(passo.idProcesso, infoUtente))
                        {
                            retValue = false;
                        }
                    }

                    if (retValue)
                        transactionContext.Complete();
                    else
                        transactionContext.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaPassoDiFirma ", e);
                return false;
            }
            return retValue;
        }

        public static ProcessoFirma AggiornaProcessoDiFirma(ProcessoFirma processo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                processo = libroFirma.AggiornaProcessoDiFirma(processo, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaProcessoDiFirma ", e);
            }
            return processo;
        }

        /// <summary>
        /// Creazione di un elemento in libro firma a partire dal passo eseguito
        /// </summary>
        /// <param name="processo"></param>
        /// <param name="?"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool InserisciElementoInLibroFirma(IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, string modalita)
        {
            bool retVal = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                string idNewElemento = string.Empty;
                IstanzaProcessoDiFirma istanzaProcesso = libroFirma.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(passo.idIstanzaProcesso);
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                DocsPaVO.documento.InfoDocumento infoDoc = doc.GetInfoDocumentoLite(istanzaProcesso.docNumber);

                bool canExecuteTransmission = true;
                if (!passo.Evento.TipoEvento.Equals("E"))
                {
                    idNewElemento = libroFirma.InsertElementoInLibroFirma(passo, infoUtente, modalita);
                    if (!string.IsNullOrEmpty(idNewElemento))
                    {
                        retVal = true;
                        canExecuteTransmission = libroFirma.CanExecuteTransmission(infoDoc.docNumber, passo.RuoloCoinvolto.idGruppo, passo.UtenteCoinvolto.idPeople, idNewElemento);
                    }
                }
                else
                {
                    retVal = true;
                }
                
                if (canExecuteTransmission)
                {
                    string noteGenerali = string.IsNullOrEmpty(istanzaProcesso.NoteDiAvvio) ? passo.Note : (string.IsNullOrEmpty(passo.Note)) ? istanzaProcesso.NoteDiAvvio : (istanzaProcesso.NoteDiAvvio + " - " + passo.Note);
                    DocsPaVO.trasmissione.Trasmissione result = ExecuteTransmission(istanzaProcesso, passo, infoDoc, infoUtente);
                    string desc = string.Empty;
                    string method = "TRASM_DOC_" + (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).ragione.descrizione.ToUpper().Replace(" ", "_");
                    if (result.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();
                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(idNewElemento))
                            libroFirma.UpdateIdTrasmInElementoLF(idNewElemento, (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId, string.Empty);
                        string checkNotify = libroFirma.EventToBeNotified(passo, "INSERIMENTO_DOCUMENTO_LF") ? "1" : "0";
                        if (passo.Evento.TipoEvento.Equals("E"))
                            checkNotify = "1";
                        BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                            (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), checkNotify, (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);

                    }
                    else
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO,
                               (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0", (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InsertProcessoDiFirma ", e);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Estrae gli elementi in libro firma per la coppia ruolo-utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<ElementoInLibroFirma> GetElementiLibroFirma(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ElementoInLibroFirma> listaElementi = new List<ElementoInLibroFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listaElementi = libroFirma.GetElementiInLibroFirma(infoUtente);

                //Ordino gli elementi mettondo gli allegati vicino al rispettivo documento principale
                List<ElementoInLibroFirma> tmp = new List<ElementoInLibroFirma>();
                foreach (ElementoInLibroFirma e in listaElementi)
                {
                    //Se è un documento, inserisco lo stesso ed i suoi eventuali allegati nella lista temporanea
                    if (string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale))
                    {
                        tmp.Add(e);
                        tmp.AddRange((from e1 in listaElementi where e1.InfoDocumento.IdDocumentoPrincipale.Equals(e.InfoDocumento.Docnumber) select e1).ToList());
                    }//Se è un allegato controllo che non ci sia il suo doc principale; in tal caso lo inserisco immediatamente, altrimenti verrà inserito nella condizione di sopra
                    else if ((from e1 in listaElementi where e1.InfoDocumento.Docnumber.Equals(e.InfoDocumento.IdDocumentoPrincipale) select e1).FirstOrDefault() == null)
                    {
                        tmp.Add(e);
                    }
                }
                listaElementi = tmp;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementiLibroFirma ", e);
            }
            return listaElementi;
        }

                /// <summary>
        /// Estrae gli elementi in libro firma per la coppia ruolo-utente che sono in un determinato stato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<ElementoInLibroFirma> GetElementsInLibroFirmabByState(DocsPaVO.utente.InfoUtente infoUtente, TipoStatoElemento stato)
        {
            List<ElementoInLibroFirma> listaElementi = new List<ElementoInLibroFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listaElementi = libroFirma.GetElementsInLibroFirmabByState(infoUtente, stato);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementsInLibroFirmabByState ", e);
            }
            return listaElementi;
        }

        public static List<string> GetIdDocumentInLibroFirmabBySign(DocsPaVO.utente.InfoUtente infoUtente, TipoStatoElemento stato, string tipoFirma)
        {
            List<string> listIdDocument = new List<string>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listIdDocument = libroFirma.GetIdDocumentInLibroFirmabBySign(infoUtente, stato, tipoFirma);
            }
            catch(Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementsInLibroFirmabByState ", e);
            }
            return listIdDocument;
        }


        public static bool ExistsElementWithTypeSign(string typeSign, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.ExistsElementWithTypeSign(typeSign, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: ExistsElementWithTypeSign ", e);
            }
            return result;
        }
        
               /// <summary>
        /// Estrae gli elementi in libro firma per la coppia ruolo-utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<ElementoInLibroFirma> GetElementiInLibroFirmaIntoPage(DocsPaVO.utente.InfoUtente infoUtente, int pageSize, int requestedPage, string testoRicerca, DocsPaVO.Mobile.RicercaType tipoRicerca, out int totalRecordCount, List<DocsPaVO.filtri.FiltroRicerca> filtriDoc)
             //string oggetto, string proponente, string note, string iddocumento, string numproto, string dataDa, string dataa, string numAnnoProto)
        {
            List<ElementoInLibroFirma> listaElementi = new List<ElementoInLibroFirma>();
            totalRecordCount = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listaElementi = libroFirma.GetElementiInLibroFirmaIntoPage(infoUtente, pageSize, requestedPage, testoRicerca, tipoRicerca, out totalRecordCount, filtriDoc);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementiInLibroFirmaIntoPage ", e);
            }
            return listaElementi;
        }

        public static bool IsTipoFirmaParallela()
        {
            bool result = false;

            string valore = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_SET_TIPO_FIRMA");
            if (!string.IsNullOrEmpty(valore) && (valore.Equals("1") || valore.Equals("3")))
                result = true;

            return result;
        }
        
        public static bool UpdateIstanzaProcessoDiFirma(IstanzaProcessoDiFirma istanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.UpdateIstanzaProcessoDiFirma(istanzaProcesso, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: UpdateIstanzaProcessoDiFirma ", e);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Aggiorna lo stato degli elementi in libro firma
        /// </summary>
        /// <param name="passo"></param>
        /// <param name="oldNumeroSequenza"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool AggiornaStatoElementiInLibroFirma(List<ElementoInLibroFirma> elementi, string nuovoStato, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, out string message)
        {
            bool retValue = true;
            string errore = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.AggiornaStatoElementiInLibroFirma(elementi, nuovoStato, infoUtente, out message);

                //Per ciascun elemento controllo, se il nuovo stato è "da firmare" o "da respingere", controllo che non sia stato già accetta, in caso vado ad accettare
                if (retValue && (nuovoStato.Equals(TipoStatoElemento.DA_FIRMARE.ToString()) || nuovoStato.Equals(TipoStatoElemento.DA_RESPINGERE.ToString())))
                {
                    foreach (ElementoInLibroFirma elemento in elementi)
                    {
                        if (string.IsNullOrEmpty(elemento.DataAccettazione))
                        {
                            //Devo accettare la trasmissione
                            string dataAccettazione = DocsPaDbManagement.Functions.Functions.GetDate();
                            AcceptsTransmission(elemento, dataAccettazione, ruolo, infoUtente, out errore);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaStatoElementiInLibroFirma ", e);
                message = string.Empty;
                return false;
            }
            return retValue;
        }

        public static bool AggiornaStatoElementoInLibroFirma(ElementoInLibroFirma elemento, string nuovoStato, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, out string message)
        {
            bool retValue = true;
            string errore = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.AggiornaStatoElementoInLibroFirma(elemento, nuovoStato, infoUtente, out message);

                //Per ciascun elemento controllo, se il nuovo stato è "da firmare" o "da respingere", controllo che non sia stato già accetta, in caso vado ad accettare
                if (retValue && (nuovoStato.Equals(TipoStatoElemento.DA_FIRMARE.ToString()) || nuovoStato.Equals(TipoStatoElemento.DA_RESPINGERE.ToString())))
                {
                    if (string.IsNullOrEmpty(elemento.DataAccettazione))
                    {
                        //Devo accettare la trasmissione
                        string dataAccettazione = DocsPaDbManagement.Functions.Functions.GetDate();
                        AcceptsTransmission(elemento, dataAccettazione, ruolo, infoUtente, out errore);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaStatoElementoInLibroFirma ", e);
                message = string.Empty;
                return false;
            }
            return retValue;
        }

        private static bool AcceptsTransmission(DocsPaVO.LibroFirma.ElementoInLibroFirma elemento, string dataAccettazione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, out string errore)
        {
            bool result = false;
            errore = string.Empty;
            string mode;
            string idObj;
            string msg;
            try
            {
                DocsPaVO.utente.Utente utente = null;
                string idUtente = infoUtente.idPeople;
                utente = BusinessLogic.Utenti.UserManager.getUtenteById(idUtente);
                DocsPaVO.trasmissione.TrasmissioneUtente[] trasmissioneUtenteInRuolo = BusinessLogic.Trasmissioni.TrasmManager.getTrasmissioneUtenteInRuolo(infoUtente, elemento.IdTrasmSingola, utente);

                trasmissioneUtenteInRuolo[0].dataAccettata = dataAccettazione;
                trasmissioneUtenteInRuolo[0].tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE;

                string idTrasmissione = BusinessLogic.Trasmissioni.TrasmManager.GetIdTrasmissioneByIdTrasmSingola(elemento.IdTrasmSingola);
                result = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmissioneUtenteInRuolo[0], idTrasmissione, ruolo, infoUtente, out errore, out mode, out idObj);
            }
            catch (Exception e)
            {

            }
            return result;
        }

        /// <summary>
        /// Avvia un processo di firma
        /// </summary>
        /// <param name="processo">Processo originale</param>
        /// <param name="docNumber">Documento/allegato scelto</param>
        /// /// <param name="versionId">Versione del file</param>
        /// <param name="infoUtente">Utente operatore</param>
        /// <returns>true/false</returns>
        public static bool StartProcessoDiFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, string modalita, string note, bool notificaInterruzione, bool notificaConclusione, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso, bool daCambioStato = false)
        {
            bool retVal = false;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
            IstanzaProcessoDiFirma istanzaProcesso = null;
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            bool canExecuteTransmission = false;
            string newIdElemento = string.Empty;
            try
            {
                string idDocumentoPrincipale = string.Empty;
                if (file.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                {
                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato() { docNumber = file.docNumber };
                    idDocumentoPrincipale = BusinessLogic.Documenti.AllegatiManager.getIdDocumentoPrincipale(all);
                }
                else
                {
                    idDocumentoPrincipale = file.docNumber;
                }
                #region CONTROLLI SUL DOCUMENTO
                if (!(!string.IsNullOrEmpty(file.fileSize) && Convert.ToUInt32(file.fileSize) > 0))
                {
                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_ACQUISITO;
                    return false;
                }

                if (file.firmato.Equals("1") && (file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES) || file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA)))
                {
                    bool existsPades = (from passo in processoDiFirma.passi
                                        where passo.Evento.CodiceAzione.Equals("DOC_SIGNATURE_P")
                                        select passo).FirstOrDefault() != null;
                    if (existsPades)
                    {
                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PADES_SU_FILE_CADES;
                        return false;
                    }
                }

                //Verifico se il documento è consolidato
                DocsPaVO.documento.DocumentConsolidationStateInfo consolidationState = BusinessLogic.Documenti.DocumentConsolidation.GetState(infoUtente, idDocumentoPrincipale);
                if (consolidationState.State != DocsPaVO.documento.DocumentConsolidationStateEnum.None)
                {
                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_CONSOLIDATO;
                    return false;
                }

                //Verifico se il doucmento è bloccato
                if (BusinessLogic.CheckInOut.CheckInOutServices.IsCheckedOut(file.docNumber, file.docNumber, infoUtente))
                {
                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_BLOCCATO;
                    return false;
                }
                //Verifico se il file è ammesso alla firma
                if (BusinessLogic.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                {
                    DocsPaVO.FormatiDocumento.SupportedFileType[] fileTypes = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(infoUtente.idAmministrazione));
                    string extensionFile = BusinessLogic.Documenti.FileManager.getEstensioneIntoSignedFile(file.fileName);
                    int count = fileTypes.Count(e => e.FileExtension.ToLowerInvariant() == extensionFile.ToLowerInvariant() &&
                                                            e.FileTypeUsed && e.FileTypeSignature);
                    bool retValue = count > 0;
                    if (!retValue)
                    {
                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_AMMESSO_ALLA_FIRMA;
                        return false;
                    }
                }
                //Verifico se il documento è protocollato
                //DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
                //schedaDocumento.docNumber = idDocumentoPrincipale;
                //schedaDocumento.protocollo = new DocsPaVO.documento.Protocollo();
                //if (!doc.CheckProto(schedaDocumento))
                //{
                //    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_PROTOCOLLATO;
                //    return false;
                //}
                //Verifico se il documento è repertoriato
                //string idTemplate = ProfilazioneDinamica.ProfilazioneDocumenti.getIdTemplate(idDocumentoPrincipale);
                // if(ProfilazioneDinamica.ProfilazioneDocumenti.isDocRepertoriato(idDocumentoPrincipale, idTemplate))
                //{
                //  resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_REPERTORIATO;
                // return false;
                //}
                #endregion
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    if (!libroFirma.IsDocInLibroFirma(file.docNumber) && libroFirma.UpdateLockDocument(file.docNumber, "1"))
                    {
                        istanzaProcesso = libroFirma.CreateIstanzaFromProcesso(processoDiFirma, file, infoUtente, note, notificaInterruzione, notificaConclusione, daCambioStato);
                        DocsPaVO.documento.InfoDocumento infoDoc = doc.GetInfoDocumentoLite(istanzaProcesso.docNumber);
                        IstanzaPassoDiFirma istanzaPassoCorrente = libroFirma.GetIstanzaPassoDiFirmaInAttesa(istanzaProcesso.idIstanzaProcesso);

                        //Se il tipo di passo è di ATTESA("WAIT") non devo trasmettere
                        if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("W"))
                        {

                            if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                            {
                                newIdElemento = libroFirma.InsertElementoInLibroFirma(istanzaProcesso, infoUtente, modalita);
                                if (!string.IsNullOrEmpty(newIdElemento))
                                {
                                    retVal = true;
                                    //PER EVITARE DI TRASMETTERE PIù VOLTE LO STESSO DOCUMENTO, CONTROLLO CHE NON ESISTE NEL LIBRO FIRMA DELL'UTENTE UN ALLEGATO DI UN DOCUMENTO GIà TRASMESSO                           
                                    canExecuteTransmission = libroFirma.CanExecuteTransmission(infoDoc.docNumber, istanzaPassoCorrente.RuoloCoinvolto.idGruppo, istanzaPassoCorrente.UtenteCoinvolto.idPeople, newIdElemento);
                                }
                                else
                                {
                                    libroFirma.RollbackStartProcessoDiFirma(istanzaProcesso, file);
                                    return false;
                                }
                            }
                            else
                            {
                                canExecuteTransmission = true;
                                retVal = true;
                            }
                        }
                        else
                        {
                            //Se è passo di wait verifico se ci sono allegati in lf, se non ci sono vado al passo successivo
                            List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> listAllInProcess = libroFirma.GetInfoProcessesStartedForDocument(istanzaProcesso.docNumber);
                            listAllInProcess = (from l in listAllInProcess where l.docAll.Equals("A") select l).ToList();
                            if (listAllInProcess == null || listAllInProcess.Count == 0)
                            {
                                libroFirma.UpdateStatoIstanzaPasso(istanzaPassoCorrente.idIstanzaPasso, string.Empty, DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE.ToString(), infoUtente);
                                istanzaPassoCorrente = libroFirma.GetNextIstanzaPasso(istanzaPassoCorrente.idIstanzaProcesso, istanzaPassoCorrente.numeroSequenza, istanzaProcesso.versionId);
                                if (istanzaPassoCorrente != null && (!string.IsNullOrEmpty(istanzaPassoCorrente.idIstanzaPasso)))
                                {
                                    libroFirma.UpdateStatoIstanzaPasso(istanzaPassoCorrente.idIstanzaPasso, istanzaProcesso.versionId, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString(), infoUtente);
                                }

                                //Devo inserire in libro firma
                                if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                                {
                                    newIdElemento = libroFirma.InsertElementoInLibroFirma(istanzaProcesso, infoUtente, modalita);
                                    if (!string.IsNullOrEmpty(newIdElemento))
                                    {
                                        retVal = true;
                                        //PER EVITARE DI TRASMETTERE PIù VOLTE LO STESSO DOCUMENTO, CONTROLLO CHE NON ESISTE NEL LIBRO FIRMA DELL'UTENTE UN ALLEGATO DI UN DOCUMENTO GIà TRASMESSO                           
                                        canExecuteTransmission = libroFirma.CanExecuteTransmission(infoDoc.docNumber, istanzaPassoCorrente.RuoloCoinvolto.idGruppo, istanzaPassoCorrente.UtenteCoinvolto.idPeople, newIdElemento);
                                    }
                                    else
                                    {
                                        libroFirma.RollbackStartProcessoDiFirma(istanzaProcesso, file);
                                        return false;
                                    }
                                }
                                else
                                {
                                    canExecuteTransmission = true;
                                    retVal = true;
                                }
                            }
                            retVal = true;
                        }

                        #region TRASMISSIONE DEL DOCUMENTO
                        //INVIO TRAMISSIONE

                        if (canExecuteTransmission)
                        {
                            DocsPaVO.trasmissione.Trasmissione result = ExecuteTransmission(istanzaProcesso, istanzaPassoCorrente, infoDoc, infoUtente);
                            string desc = string.Empty;
                            string method = "TRASM_DOC_" + (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (result.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();
                            if (result != null)
                            {
                                if (!string.IsNullOrEmpty(newIdElemento))
                                    libroFirma.UpdateIdTrasmInElementoLF(newIdElemento, (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId, string.Empty);
                                string checkNotify = libroFirma.EventToBeNotified(istanzaPassoCorrente, "INSERIMENTO_DOCUMENTO_LF") ? "1" : "0";
                                if (istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                                    checkNotify = "1";
                                BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                    (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), checkNotify, (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);

                            }
                            else
                            {
                                retVal = false;
                                BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO,
                                        (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0", (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);
                            }

                        }
                        #endregion
                    }
                    else
                    {
                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA;
                    }
                    if (retVal)
                    {
                        // Integrazione con portale - cambio di stato del fascicolo
                        //if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI")) && !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI").Equals("0"))
                        //{
                        //    DocsPaVO.Procedimento.Procedimento proc = null;
                        //    string idProcedimento = string.Empty;
                        //    System.Collections.ArrayList listaFasc = Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, file.docNumber);
                        //    if (listaFasc != null && listaFasc.Count > 0)
                        //    {
                        //        foreach (DocsPaVO.fascicolazione.Fascicolo fasc in listaFasc)
                        //        {
                        //            proc = new DocsPaVO.Procedimento.Procedimento();
                        //            proc = Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(fasc.systemID);
                        //            if (proc != null && proc.Id == fasc.systemID)
                        //            {
                        //                idProcedimento = fasc.systemID;
                        //                break;
                        //            }
                        //        }
                        //    }

                        //    if (proc != null)
                        //    {
                        //        DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, file.docNumber);
                        //        if (schedaDoc != null && schedaDoc.template != null)
                        //        {
                        //            BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(proc.Id, "FIRMA", schedaDoc.template.ID_TIPO_ATTO, infoUtente);
                        //        }
                        //    }
                        //}
                        //SalvaStoricoIstanzaProcessoFirma(istanzaProcesso.idIstanzaProcesso, istanzaProcesso.docNumber, "Avviato processo di firma", infoUtente);

                        transactionContext.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in StartProcessoDiFirma  - ", e);
                libroFirma.RollbackStartProcessoDiFirma(istanzaProcesso, file);
            }

            return retVal;
        }


        /// <summary>
        /// Esegue la trasmissione a segueto dell'inserimento il libro firma. Il mittente della trasmissione è
        /// 1 - per istanza di passo n = 1 il proponente(utente che ha avviato il processo di firma)
        /// 2 - per istanza di passo n > 1  è il titolare del passo n -1.
        /// Il destinatario della trasmissione è il titolare:
        /// 1 - se non è specificato l'i utente titolare, il file è inserito nel libro firma di tutti gli utenti del ruolo titolare e quindi la trasmissione è al ruolo.
        /// 2 - se è specificato l'id utente titolare, il file è inserito nel libro firma del solo utente titolare e quindi sarà una trasmissione utente
        /// </summary>
        private static DocsPaVO.trasmissione.Trasmissione ExecuteTransmission(IstanzaProcessoDiFirma istanzaProcesso, IstanzaPassoDiFirma istanzaPasso, DocsPaVO.documento.InfoDocumento infoDoc, DocsPaVO.utente.InfoUtente infoUtenteMit)
        {
            DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();

            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            trasm.ruolo = u.GetRuoloByIdGruppo(infoUtenteMit.idGruppo);//istanzaProcesso.RuoloProponente;
            trasm.utente = u.getUtenteById(infoUtenteMit.idPeople);//istanzaProcesso.UtenteProponente;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            trasm.infoDocumento = infoDoc;

            string notePasso = string.IsNullOrEmpty(istanzaProcesso.NoteDiAvvio) ? istanzaPasso.Note : (string.IsNullOrEmpty(istanzaPasso.Note)) ? istanzaProcesso.NoteDiAvvio : (istanzaProcesso.NoteDiAvvio + " - " + istanzaPasso.Note);

            string tipoPasso = string.Empty;
            if (istanzaPasso.Evento.TipoEvento.Equals("E"))
            {
                trasm.noteGenerali = "Azione richiesta " + istanzaPasso.Evento.Descrizione + ". " + notePasso;
                tipoPasso = istanzaPasso.Evento.Gruppo;
            }
            else
            {
                trasm.noteGenerali = notePasso;
                tipoPasso = istanzaPasso.Evento.CodiceAzione;
            }

            //INSERISCO LA RAGIONE DI TRASMISSIONE DI SISTEMA PER LIBRO FIRMA
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = utenti.GetRuoloByIdGruppo(istanzaPasso.RuoloCoinvolto.idGruppo);
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = Trasmissioni.RagioniManager.GetRagioneByTipoOperazione(tipoPasso, ruolo.idAmministrazione);

            //CREO LA TRASMISSIONE SINGOLA
            DocsPaVO.trasmissione.TrasmissioneSingola trasmSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmSing.ragione = ragione;
            trasmSing.tipoTrasm = "S";

            trasmSing.corrispondenteInterno = ruolo;
            trasmSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;

            System.Collections.ArrayList listaUtenti = new System.Collections.ArrayList();
            DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
            qc.codiceRubrica = ruolo.codiceRubrica;
            System.Collections.ArrayList registri = ruolo.registri;
            qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            //qc.idRegistri = registri;
            qc.idAmministrazione = ruolo.idAmministrazione;
            qc.getChildren = true;
            qc.fineValidita = true;
            listaUtenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
            System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();

            //Se l'id utente titolare non è specificato, il documento è stato inserito  nel libro firma di tutti gli utenti del ruolo,
            //e quindi la trasmissione andrà notificata a tutti gli utenti, altrimenti al solo utente titolare
            if (string.IsNullOrEmpty(istanzaPasso.UtenteCoinvolto.idPeople))
            {
                for (int k = 0; k < listaUtenti.Count; k++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                    trasmissioniUt.Add(trUt);
                }
            }
            else
            {
                for (int k = 0; k < listaUtenti.Count; k++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                    trUt.daNotificare = (listaUtenti[k] as DocsPaVO.utente.Utente).idPeople.Equals(istanzaPasso.UtenteCoinvolto.idPeople);
                    trasmissioniUt.Add(trUt);
                }
            }
            trasmSing.trasmissioneUtente = trasmissioniUt;
            trasm.trasmissioniSingole = new System.Collections.ArrayList() { trasmSing };
            return BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod("", trasm);
        }

        /// <summary>
        /// Estrae il processo di firma avviato per il documento in input
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessoDiFirmaByDocnumber(string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<IstanzaProcessoDiFirma> istanzaProcessoDiFirma = new List<IstanzaProcessoDiFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                istanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByDocnumber(docnumber, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessoDiFirmaByDocnumber ", e);
            }
            return istanzaProcessoDiFirma;
        }

        public static bool StopPassoWait(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                List<IstanzaProcessoDiFirma> listIstanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByDocnumber(docNumber, infoUtente);
                
                if (listIstanzaProcessoDiFirma != null && listIstanzaProcessoDiFirma.Count() > 0)
                {
                    foreach (IstanzaProcessoDiFirma istanzaProcessoDiFirma in listIstanzaProcessoDiFirma)
                    {
                        if (string.IsNullOrEmpty(istanzaProcessoDiFirma.dataChiusura))
                        {
                            List<IstanzaPassoDiFirma> listIstanzaPassi = libroFirma.GetIstanzePassoDiFirma(istanzaProcessoDiFirma.idIstanzaProcesso);

                            foreach (IstanzaPassoDiFirma istanzaPasso in listIstanzaPassi)
                            {
                                if (istanzaPasso.Evento.TipoEvento.Equals("W"))
                                {
                                    DocsPaDB.Query_Utils.Utils date = new DocsPaDB.Query_Utils.Utils();
                                    string dataEvento = date.GetDBDate(true);

                                    if (istanzaPasso.statoPasso.Equals(TipoStatoPasso.LOOK))
                                    {
                                        libroFirma.StopProcessSteps(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaPasso.numeroSequenza);

                                        //string dataEvento = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt",  new System.Globalization.CultureInfo("it-IT")));

                                        if (libroFirma.SetProcesComplete(istanzaProcessoDiFirma.idIstanzaProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED, istanzaProcessoDiFirma.docNumber, dataEvento))
                                        {

                                            string method_TRONCAMENTO = "TRONCAMENTO_PROCESSO";
                                            string description_TRONCAMENTO = "Anomalia nel processo per interruzione su allegato.";

                                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method_TRONCAMENTO, istanzaProcessoDiFirma.docNumber,
                                            description_TRONCAMENTO, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", dataEvento);

                                            SalvaStoricoIstanzaProcessoFirma(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaProcessoDiFirma.docNumber, description_TRONCAMENTO, infoUtente);

                                            string method_CONCLUSIONE = "CONCLUSIONE_PROCESSO_LF_DOCUMENTO";
                                            string description_CONCLUSIONE = "Conclusione del processo di firma per documento.";

                                            SalvaStoricoIstanzaProcessoFirma(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaProcessoDiFirma.docNumber, description_CONCLUSIONE, infoUtente);

                                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method_CONCLUSIONE, istanzaProcessoDiFirma.docNumber,
                                            description_CONCLUSIONE, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", dataEvento);

                                            retVal = true;
                                        }

                                        break;
                                    }
                                    else if (istanzaPasso.statoPasso.Equals(TipoStatoPasso.NEW))
                                    {

                                        libroFirma.StopProcessSteps(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaPasso.numeroSequenza);

                                        string method_TRONCAMENTO = "TRONCAMENTO_PROCESSO";
                                        string description_TRONCAMENTO = "Anomalia nel processo per interruzione su allegato.";

                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method_TRONCAMENTO, istanzaProcessoDiFirma.docNumber,
                                        description_TRONCAMENTO, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", dataEvento);

                                        SalvaStoricoIstanzaProcessoFirma(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaProcessoDiFirma.docNumber, description_TRONCAMENTO, infoUtente);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessoDiFirmaByDocnumber ", e);
            }
            return retVal;
        }

        /// <summary>
        /// Metodo per l'estrazione dell'istanze di processo create a partire dallo schema di processo definito
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessoDiFirmaByIdProcesso(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<IstanzaProcessoDiFirma> istanzaProcessoDiFirma = new List<IstanzaProcessoDiFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                istanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByIdProcesso(idProcesso, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessoDiFirmaByIdProcesso ", e);
            }
            return istanzaProcessoDiFirma;
        }

       /// <summary>
        /// Metodo per l'estrazione dell'istanze di processo create a partire dallo schema di processo definito
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            IstanzaProcessoDiFirma istanzaProcessoDiFirma = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                istanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(idProcesso);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessoDiFirmaByIdIstanzaProcesso ", e);
            }
            return istanzaProcessoDiFirma;
        }

        public static List<string> GetElementiInLibroFirmaByDestinatario(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<string> listaIdElementi = new List<string>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listaIdElementi = libroFirma.GetElementiInLibroFirmaByDestinatario(corr, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementiInLibroFirmaByDestinatario ", e);
            }
            return listaIdElementi;
        }

        public static bool IsTitolare(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsTitolare(docNumber, infoUtente.idGruppo, infoUtente.idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsTitolare ", e);
                return false;
            }
        }

        public static string GetTypeSignatureToBeEntered(DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetTypeSignatureToBeEntered(fileReq, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetTypeSignatureToBeEntered ", e);
                return null;
            }
        }

        public static bool IsDocInLibroFirma(string docNumber)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsDocInLibroFirma(docNumber);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsDocInLibroFirma ", e);
                return false;
            }
        }

        public static bool IsDocOrAllInLibroFirma(string docNumber)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsDocOrAllInLibroFirma(docNumber);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsDocOrAllInLibroFirma ", e);
                return false;
            }
        }

        public static bool PutElectronicSignature(DocsPaVO.documento.FileRequest approvingFile, DocsPaVO.utente.InfoUtente infoUtente, bool isAdvancementProcess, out string message)
        {
            try
            {
                bool retValue = false;
                message = string.Empty;
                FirmaElettronica firma = new FirmaElettronica();
                bool canToSign = true;

                if (approvingFile.inLibroFirma)
                {
                    DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                    string tipoOperazione = isAdvancementProcess ? "DOC_STEP_OVER" : "DOC_VERIFIED";
                    canToSign = libro.CanExecuteOperation(approvingFile.docNumber, tipoOperazione, infoUtente);
                    if (!canToSign)
                    { 
                        string operazione = isAdvancementProcess ? "avanzamento iter" : "firma elettronica";
                        message = "L'operazione è gia stata eseguita o presa in carico da un altro utente";
                        return false;
                    }
                }

                firma.Docnumber = approvingFile.docNumber;
                firma.Versionid = approvingFile.versionId;
                firma.DocAll = (approvingFile.GetType().Equals(typeof(DocsPaVO.documento.Allegato)) ? "A" : "D");
                firma.NumAll = approvingFile.versionLabel.ToUpper().Replace("A", "");
                firma.NumVersione = approvingFile.version;

                string impronta = string.Empty;
                DocsPaDB.Query_DocsPAWS.Documenti docInfoDB = new DocsPaDB.Query_DocsPAWS.Documenti();
                docInfoDB.GetImpronta(out impronta, firma.Versionid, firma.Docnumber);
                firma.Imponta = impronta;

                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                firma = libroFirma.InsertElectronicSign(firma, infoUtente, isAdvancementProcess);

                retValue = (firma != null && !string.IsNullOrEmpty(firma.IdFirma));
                //libroFirma.UpdateUtenteLockerInLibroFirma(firma, infoUtente);
                if (retValue && !isAdvancementProcess)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                    documenti.UpdateComponentsChaFirmato(approvingFile.versionId, approvingFile.docNumber);
                    documenti.UpdateComponentsChaTipoFirma(approvingFile.versionId, approvingFile.docNumber, approvingFile.tipoFirma, true);
                }
                return retValue;
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsTitolare ", e);
                message = "Errore durante l'operazione richiesta";
                return false;
            }
        }

        public static bool InterruptionSignatureProcessByProponent(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanza, string noteInterruzione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            string msg = string.Empty;
            string varMetodo = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    string idPassoInAttesa = (from i in istanza.istanzePassoDiFirma
                                              where i.statoPasso.Equals(TipoStatoPasso.LOOK)
                                              select i.idIstanzaPasso).FirstOrDefault();
                    string dateInterruption = DateTime.Now.ToString();
                    libroFirma.AggiornaDataEsecuzioneElemento(istanza.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.INTERROTTO.ToString());
                    if (libroFirma.EliminaElementoInLibroFirma(idPassoInAttesa))
                    {
                        string interrottoDa = "P"; //Proponente
                        result = libroFirma.InterruptionSignatureProcess(istanza.idIstanzaProcesso, TipoStatoProcesso.STOPPED, istanza.docNumber, noteInterruzione, dateInterruption, interrottoDa);
                        if (result)
                        {
                            msg = "Interruzione del processo di firma per il file " + istanza.docNumber;
                            varMetodo = istanza.docAll.Equals("D") ? "INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE" : "INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE";
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, varMetodo, istanza.docNumber, msg, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", null, dateInterruption);
                        }
                    }
                    if (result)
                    {
                        transactionContext.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterruptionSignatureProcessByProponent ", ex);
                return false;
            }

            return result;
        }

        public static bool InterruptionSignatureProcessByAdministrator(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma[] istanzeProcessi, string noteInterruzione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            string msg = string.Empty;
            string varMetodo = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                foreach (DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanza in istanzeProcessi)
                {
                    // Contesto transazionale
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        string idPassoInAttesa = (from i in istanza.istanzePassoDiFirma
                                                  where i.statoPasso.Equals(TipoStatoPasso.LOOK)
                                                  select i.idIstanzaPasso).FirstOrDefault();
                        string dateInterruption = DateTime.Now.ToString();
                        libroFirma.AggiornaDataEsecuzioneElemento(istanza.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.INTERROTTO.ToString());
                        ElementoInLibroFirma elemento = libroFirma.GetElementiInLibroFirmaByIdIstanzaPasso(idPassoInAttesa);
                        if (libroFirma.EliminaElementoInLibroFirma(idPassoInAttesa))
                        {
                            string interrottoDa = "A"; //Amministratore
                            result = libroFirma.InterruptionSignatureProcess(istanza.idIstanzaProcesso, TipoStatoProcesso.STOPPED, istanza.docNumber, noteInterruzione, dateInterruption, interrottoDa);
                            if (result)
                            {
                                msg = "Interruzione del processo di firma per il file " + istanza.docNumber;
                                varMetodo = istanza.docAll.Equals("D") ? "INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN" : "INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN";
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, null, infoUtente.idAmministrazione, varMetodo, istanza.docNumber, msg, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", null, dateInterruption);
                            }
                        }
                        if (result)
                        {
                            if (elemento != null && !string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale))
                                BusinessLogic.LibroFirma.LibroFirmaManager.StopPassoWait(elemento.InfoDocumento.IdDocumentoPrincipale, infoUtente);

                            transactionContext.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterruptionSignatureProcessByProponent ", ex);
                return false;
            }

            return result;
        }

        public static List<ElementoInLibroFirma> GetElementsInLibroFirmabByListIdIstanzaProcesso(string[] idIstanzeProcessi, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ElementoInLibroFirma> listaElementi = new List<ElementoInLibroFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listaElementi = libroFirma.GetElementsInLibroFirmabByListIdIstanzaProcesso(infoUtente, idIstanzeProcessi);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementsInLibroFirmabByListIdIstanzaProcesso ", e);
            }
            return listaElementi;
        }

        public static bool InterruptionSignatureProcessByHolder(DocsPaVO.LibroFirma.ElementoInLibroFirma elemento, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, out string errore)
        {
            bool result = false;
            errore = string.Empty;
            string mode;
            string idObj;
            string msg;
            try
            {
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaVO.utente.Utente utente = null;
                    string idUtente = infoUtente.idPeople;
                    utente = BusinessLogic.Utenti.UserManager.getUtenteById(idUtente);
                    DocsPaVO.trasmissione.TrasmissioneUtente[] trasmissioneUtenteInRuolo = BusinessLogic.Trasmissioni.TrasmManager.getTrasmissioneUtenteInRuolo(infoUtente, elemento.IdTrasmSingola, utente);

                    trasmissioneUtenteInRuolo[0].noteRifiuto = "Non di competenza";
                    trasmissioneUtenteInRuolo[0].dataRifiutata = DateTime.Now.ToString();
                    trasmissioneUtenteInRuolo[0].tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.RIFIUTO;

                    string idTrasmissione = BusinessLogic.Trasmissioni.TrasmManager.GetIdTrasmissioneByIdTrasmSingola(elemento.IdTrasmSingola);
                    result = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmissioneUtenteInRuolo[0], idTrasmissione, ruolo, infoUtente, out errore, out mode, out idObj);

                    if (result)
                    {
                        transactionContext.Complete();
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterruptionSignatureProcessByHolder ", e);
            }
            return result;
        }

        /// <summary>
        ///Elimina l'elemento in libro firma e interrompe il processo di firma
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        public static bool RejectElementsSignatureProcess(DocsPaVO.LibroFirma.ElementoInLibroFirma elemento, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            string msg = string.Empty;
            string varMetodo = string.Empty;
            try
            {
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {

                    string dateInterruption = DateTime.Now.ToString();
                    DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                    result = libroFirma.EliminaElementoInLibroFirmaByIdElemento(elemento.IdElemento);
                    if (result && elemento.Modalita.Equals("A"))
                    {
                        libroFirma.AggiornaDataEsecuzioneElemento(elemento.InfoDocumento.Docnumber, DocsPaVO.LibroFirma.TipoStatoElemento.RESPINTO.ToString());
                        //per gli elementi che sono stati inseriti in libro firma tramite la modalità automatica,, interrompo il processo di firma

                        string interrottoDa = "T"; //Titolare
                        result = libroFirma.InterruptionSignatureProcess(elemento.IdIstanzaProcesso, TipoStatoProcesso.STOPPED, elemento.InfoDocumento.Docnumber, elemento.MotivoRespingimento, dateInterruption, interrottoDa);
                        if (result)
                        {
                            result = libroFirma.UpdateStatoIstanzaPasso(elemento.IdIstanzaPasso, elemento.InfoDocumento.VersionId, TipoStatoPasso.STUCK.ToString(), infoUtente, dateInterruption);
                        }
                        if (result)
                        {

                            msg = "Interruzione del processo di firma per il file " + elemento.InfoDocumento.Docnumber;
                            varMetodo = string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale) ? "INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE" : "INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE";
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, varMetodo, elemento.InfoDocumento.Docnumber, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", null, dateInterruption);
                        }
                    }
                    if (result)
                    {
                        transactionContext.Complete();
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: RejectElementsSignatureProcess ", e);
                result = false;
            }

            return result;
        }


        public static List<FirmaElettronica> GetFirmaElettronicaDaFileRequest(DocsPaVO.documento.FileRequest fileRq)
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            List<FirmaElettronica> firmaFound = libroFirma.GetFirmaElettronicaDaFileRequest(fileRq);

            return firmaFound;
        }

        public static FirmaElettronica InserisciFirmaElettronica(FirmaElettronica firma)
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.InsertElectronicSignFromDigitalSign(firma);
        }

        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtro, int numPage, int pageSize, out int numTotPage, out int nRec, DocsPaVO.utente.InfoUtente infoUtente)
        {
            numTotPage = 0;
            nRec = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetIstanzaProcessiDiFirmaByFilter(filtro, numPage, pageSize, out numTotPage, out nRec, infoUtente);
            }
            catch(Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessiDiFirmaByFilter ", e);
                return null;
            }
        }

        /// <summary>
        /// Estrae i  processi di firma avviati e non ancora conclusi per il documento principale ed i suoi allegati
        /// </summary>
        /// <param name="idMainDocument"></param>
        /// <returns></returns>
        public static List<IstanzaProcessoDiFirma> GetInfoProcessesStartedForDocument(string idMainDocument)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetInfoProcessesStartedForDocument(idMainDocument);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetInfoProcessesStartedForDocument ", e);
                return null;
            }
        }

        /// <summary>
        /// Verifica se la versione del documento è stato firmata elettronicamente
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static bool IsElectronicallySigned(string docnumber, string versionId)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsElectronicallySigned(docnumber, versionId);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsElectronicallySigned ", e);
                return false;
            }
        }

        /// <summary>
        /// Verifica se esiste un ruolo legato al ruolo in input per i tipi ruoli specificati
        /// </summary>
        /// <param name="typeRole"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<DocsPaVO.utente.TipoRuolo> CheckExistsRoleSupByTypeRoles(List<DocsPaVO.utente.TipoRuolo> typeRole, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.CheckExistsRoleSupByTypeRoles(typeRole, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CheckExistsRoleSupByTypeRoles ", e);
                return null;
            }
        }

        public static void SalvaStoricoIstanzaProcessoFirma(string idIstanza, string docnumber, string azione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            libroFirma.SalvaStoricoIstanzaProcessoFirma(idIstanza, docnumber, azione, infoUtente);
        }

        public static void SalvaStoricoIstanzaProcessoFirmaByDocnumber(string docnumber, string azione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string idIstanza = GetIdIstanzaProcessoInExec(docnumber);
            if (!string.IsNullOrEmpty(idIstanza))
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                libroFirma.SalvaStoricoIstanzaProcessoFirma(idIstanza, docnumber, azione, infoUtente);
            }
        }

        public static string GetIdIstanzaProcessoInExec(string docnumber)
        {
            string idIstanza = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                idIstanza = libroFirma.GetIdIstanzaProcessoInExec(docnumber);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIdIstanzaProcessoInExec ", e);
            }
            return idIstanza;
        }

        public static bool IsTitolarePassoInAttesa(string docnumber, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LibroFirma.Azione azione)
        {
            bool result = true;
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            IstanzaPassoDiFirma istanza = libroFirma.GetIstanzaPassoFirmaInAttesaByDocnumber(docnumber);
            if (istanza != null)
            {
                if (!infoUtente.idGruppo.Equals(istanza.RuoloCoinvolto.idGruppo))
                    result = false;
                if (!string.IsNullOrEmpty(istanza.UtenteCoinvolto.idPeople) && !infoUtente.idPeople.Equals(istanza.UtenteCoinvolto.idPeople))
                    result = false;
                if (!string.IsNullOrEmpty(istanza.UtenteLocker) && !infoUtente.idPeople.Equals(istanza.UtenteCoinvolto.idPeople))
                    result = false;
                if (!istanza.TipoFirma.Equals(azione.ToString()))
                    result = false;
            }
            return result;
        }
    }
}
