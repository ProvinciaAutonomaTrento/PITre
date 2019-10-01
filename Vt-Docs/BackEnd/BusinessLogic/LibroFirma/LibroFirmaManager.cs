using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net;
using DocsPaVO.LibroFirma;
using System.Linq;
using System.Collections;
using BusinessLogic.Interoperabilità;

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
        public static List<ProcessoFirma> GetProcessesSignatureVisibleRole(bool asProponente, bool asMonitoratore, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessesSignatureVisibleRole(asProponente, asMonitoratore, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessesSignatureVisibleRole ", e);
            }
            return listProcessiDiFirma;
        }

        /// <summary>
        /// Metodo per l'estrazione degli schemi di processi di firma con i filtri di ricerca selezionati
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<ProcessoFirma> GetProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtri, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessiDiFirmaByFilter(filtri, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByFilter ", e);
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

        #region AMMINISTRAZIONE
        public static List<ProcessoFirma> GetProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            numTotPage = 0;
            nRec = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessiDiFirmaByTitolarePaging(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByTitolarePaging ", e);
            }
            return listProcessiDiFirma;
        }

        public static int GetCountProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            int result = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.GetCountProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCountProcessiDiFirmaByTitolare ", e);
            }
            return result;
        }

        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            List<IstanzaProcessoDiFirma> listProcessiDiFirma = new List<IstanzaProcessoDiFirma>();
            numTotPage = 0;
            nRec = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetIstanzaProcessiDiFirmaByTitolarePaging(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessiDiFirmaByTitolarePaging ", e);
            }
            return listProcessiDiFirma;
        }

        public static int GetCountIstanzaProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            int result = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.GetCountIstanzaProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCountIstanzaProcessiDiFirmaByTitolare ", e);
            }
            return result;
        }

        public static List<string> GetIdRuoliProcessiUltimoUtente(string idPeople)
        {
            List<string> listIdRuoli = new List<string>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listIdRuoli = libroFirma.GetIdRuoliProcessiUltimoUtente(idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIdRuoliProcessiUltimoUtente ", e);
            }
            return listIdRuoli;
        }

        #endregion
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
        public static ProcessoFirma InsertProcessoDiFirma(ProcessoFirma processo, DocsPaVO.utente.InfoUtente infoUtente, out DocsPaVO.LibroFirma.ResultProcessoFirma resultCreazioneProcesso)
        {
            resultCreazioneProcesso = ResultProcessoFirma.OK;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                //Verifico che il nome del processo non sia stato già usato
                if (libroFirma.IsUniqueProcessName(processo.nome, infoUtente))
                    processo = libroFirma.InsertProcessoDiFirma(processo, infoUtente);
                else
                    resultCreazioneProcesso = ResultProcessoFirma.EXISTING_PROCESS_NAME;
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
        public static bool InsertVisibilitaProcesso(List<VisibilitaProcessoRuolo> visibilita, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.InsertVisibilitaProcesso(visibilita, infoUtente);
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
        public static bool RimuoviVisibilitaProcesso(string idProcesso, string idGruppo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.RimuoviVisibilitaProcesso(idProcesso, idGruppo, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: RimuoviVisibilitaProcesso ", e);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Aggiorna il tipo di visibilita che ha il ruolo sul processo di firma
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idGruppo"></param>
        /// <param name="tipoVisibilita"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool UpdateTipoVisibilitaProcesso(string idProcesso, string idGruppo, TipoVisibilita tipoVisibilita, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                retValue = libroFirma.UpdateTipoVisibilitaProcesso(idProcesso, idGruppo, tipoVisibilita, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: UpdateTipoVisibilitaProcesso ", e);
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
        public static List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> GetVisibilitaProcesso(string idProcesso, List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtroRicerca, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> listCorr = new List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listCorr = libroFirma.GetVisibilitaProcesso(idProcesso, filtroRicerca, infoUtente);
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
                //Se il passo è un passo automatico proseguo con la sua esecuzione
                if (retVal && passo.IsAutomatico)
                {
                    EseguiPassoAutomaticoAsync(passo, istanzaProcesso);
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
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementiLibroFirma ", e);
            }
            return listaElementi;
        }

        public static int CountElementiInLibroFirma(DocsPaVO.utente.InfoUtente infoUtente)
        {
            int count = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                count = libroFirma.CountElementiInLibroFirma(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CountElementiInLibroFirma ", e);
            }
            return count;
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
        public static List<ElementoInLibroFirma> GetElementiInLibroFirmaIntoPage(DocsPaVO.utente.InfoUtente infoUtente, int pageSize, int requestedPage, string testoRicerca, DocsPaVO.Mobile.RicercaType tipoRicerca, out int totalRecordCount)
        {
            List<ElementoInLibroFirma> listaElementi = new List<ElementoInLibroFirma>();
            totalRecordCount = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listaElementi = libroFirma.GetElementiInLibroFirmaIntoPage(infoUtente, pageSize, requestedPage, testoRicerca, tipoRicerca, out totalRecordCount);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetElementiInLibroFirmaIntoPage ", e);
            }
            return listaElementi;
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

        private static bool CanAvvioProcessoFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso)
        {
            bool retVal = true;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
            try
            {
                bool isAllegato = false;
                string idDocumentoPrincipale = string.Empty;
                if (file.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                {
                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato() { docNumber = file.docNumber };
                    idDocumentoPrincipale = BusinessLogic.Documenti.AllegatiManager.getIdDocumentoPrincipale(all);
                    isAllegato = true;
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
                #endregion

                #region CONTROLLI SUI PASSI DI FIRMA

                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, file.docNumber);
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                foreach (PassoFirma passo in processoDiFirma.passi)
                {
                    Azione azioneEvento = (Azione)Enum.Parse(typeof(Azione), passo.Evento.CodiceAzione, true);
                    switch (azioneEvento)
                    {
                        case Azione.DOC_SIGNATURE_P:
                            //Non è possibile applicare firma PADES su CADES
                            if (file.firmato.Equals("1") && (file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES) || file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA)))
                            {
                                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PADES_SU_FILE_CADES;
                                return false;
                            }
                            break;
                        case Azione.RECORD_PREDISPOSED:
                            if (!isAllegato)
                            {
                                //Verifico se il documento risulta gia protocollato
                                if (!doc.CheckProto(schedaDocumento))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_DOC_GIA_PROTOCOLLATO;
                                    return false;
                                }
                                if (passo.IsAutomatico)
                                {
                                    //PER IL PASSO AUTOMATICO VERIFICO CHE IL DOCUMENTO è PREDISPOSTO
                                    if (!isPredisposto(schedaDocumento))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_DOC_NON_PREDISPOSTO;
                                        return false;
                                    }
                                    //Il registro del predisposto deve essere lo stesso registro con cui andrò a protocollare
                                    if (!passo.IdAOO.Equals(schedaDocumento.registro.systemId))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO;
                                        return false;
                                    }
                                    //Verifico che il registro selezionato non sia chiuso
                                    if (schedaDocumento.registro.stato.Equals("C"))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_REG_CHIUSO;
                                        return false;
                                    }
                                }
                            }
                            break;
                        case Azione.DOCUMENTO_REPERTORIATO:
                            //Verifico se il documento è repertoriato
                            if (!isAllegato && passo.IsAutomatico)
                            {
                                //DOCUMENTO TIPIZZATO
                                string idTemplate = ProfilazioneDinamica.ProfilazioneDocumenti.getIdTemplate(idDocumentoPrincipale);
                                if (passo.IsAutomatico && string.IsNullOrEmpty(idTemplate))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_DOC_NON_TIPIZZATO;
                                    return false;
                                }

                                //DOCUMENTO REPERTORIATO
                                if (ProfilazioneDinamica.ProfilazioneDocumenti.isDocRepertoriato(idDocumentoPrincipale, idTemplate))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_DOC_GIA_REPERTORIATO;
                                    return false;
                                }
                               
                                //NESSUN CONTATORE PER LA TIPOLOGIA
                                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTemplate);
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = (from ogg in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                                       where ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") || ogg.TIPO.DESCRIZIONE_TIPO.Equals("ContatoreSottocontatore")
                                                                                       select ogg).FirstOrDefault();
                                if (oggetto == null)
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_NESSUN_CONTATORE_TIPO_DOC;
                                    return false;
                                }

                                if (oggetto.TIPO_CONTATORE.Equals("R") && string.IsNullOrEmpty(passo.IdRF))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_RF_MANCANTE;
                                    return false;
                                }

                                //DIRITTI IN SCRITTURA DEL RUOLO COINVOLTO SUL CAMPO CONTATORE
                                List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> dirittiCampiRuolo = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(passo.ruoloCoinvolto.idGruppo, idTemplate).Cast<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
                                bool ruoloInsRepertorio = false;
                                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                                {
                                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggetto.SYSTEM_ID.ToString())
                                    {
                                        if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1")
                                            ruoloInsRepertorio = true;
                                        break;
                                    }
                                }
                                if (!ruoloInsRepertorio)
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_NO_DIRITTI_SCRITTURA_CONTATORE;
                                    return false;
                                }
                            }
                            break;
                        case Azione.DOCUMENTOSPEDISCI:
                            if (!isAllegato)
                            {
                                //Se il passo è automatico, verifico che il documento è protocollato; nel caso in cui non lo sia verifico se la spedizione 
                                //è preceduta da un passo di protocollazione altrimenti blocco l'avvio del processo
                                if(passo.IsAutomatico && (schedaDocumento.protocollo == null || string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura)))
                                {
                                    if((from p1 in processoDiFirma.passi where p1.numeroSequenza < passo.numeroSequenza
                                        && p1.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString()) select p1).FirstOrDefault() == null)
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_SPEDIZIONE_DOC_NON_PROTOCOLLATO;
                                        return false;
                                    }
                                    // Il registro del predisposto deve essere lo stesso registro con cui andrò a spedire
                                    if (!passo.IdAOO.Equals(schedaDocumento.registro.systemId))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO;
                                        return false;
                                    }
                                }
                                string tipoProto = doc.getTipoProto(schedaDocumento.docNumber);
                                if (!string.IsNullOrEmpty(tipoProto) && tipoProto.Equals("A"))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_SPEDIZIONE_PROTO_ARRIVO;
                                    return false;
                                }
                            }
                            break;
                    }
                }

                #endregion
            }
            catch(Exception e)
            {
                retVal = false;
                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.KO;

            }
            return retVal;
        }

        /// <summary>
        /// Avvia un processo di firma
        /// </summary>
        /// <param name="processo">Processo originale</param>
        /// <param name="docNumber">Documento/allegato scelto</param>
        /// /// <param name="versionId">Versione del file</param>
        /// <param name="infoUtente">Utente operatore</param>
        /// <returns>true/false</returns>
        public static bool StartProcessoDiFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, string modalita, string note, DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso, bool daCambioStato = false)
        {
            bool retVal = false;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK; 
            IstanzaProcessoDiFirma istanzaProcesso = null;
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            IstanzaPassoDiFirma istanzaPassoCorrente = new IstanzaPassoDiFirma();

            //Verifico se è possibile avviare il processo di firma
            if(!CanAvvioProcessoFirma(processoDiFirma, file, infoUtente, out resultAvvioProcesso))
            {
                return false;
            }

            bool canExecuteTransmission = false;
            string newIdElemento = string.Empty;
            try
            {
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    if (!libroFirma.IsDocInLibroFirma(file.docNumber) && libroFirma.UpdateLockDocument(file.docNumber, "1"))
                    {
                        //Se la chiave è attiva la notifica si presenza di destintari interoperanti è obbligatoria
                        string attiva = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "NOTIFICA_DEST_NO_INTEROP_OBB");
                        if (!string.IsNullOrEmpty(attiva) && attiva.Equals("1"))
                        {
                            opzioniNotifiche.NotificaPresenzaDestNonInterop = true;
                        }
                        istanzaProcesso = libroFirma.CreateIstanzaFromProcesso(processoDiFirma, file, infoUtente, note, opzioniNotifiche, daCambioStato);
                        DocsPaVO.documento.InfoDocumento infoDoc = doc.GetInfoDocumentoLite(istanzaProcesso.docNumber);
                        istanzaPassoCorrente = libroFirma.GetIstanzaPassoDiFirmaInAttesa(istanzaProcesso.idIstanzaProcesso);

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
                    if(retVal && istanzaPassoCorrente.IsAutomatico)
                    {
                        //Se il passo è un passo proseguo con la sua esecuzione
                        EseguiPassoAutomaticoAsync(istanzaPassoCorrente, istanzaProcesso);
                    }
                    if (retVal)
                    {
                        // Integrazione con portale - cambio di stato del fascicolo
                        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI")) && !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI").Equals("0"))
                        {
                            DocsPaVO.Procedimento.Procedimento proc = null;
                            string idProcedimento = string.Empty;
                            System.Collections.ArrayList listaFasc = Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, file.docNumber);
                            if (listaFasc != null && listaFasc.Count > 0)
                            {
                                foreach (DocsPaVO.fascicolazione.Fascicolo fasc in listaFasc)
                                {
                                    proc = new DocsPaVO.Procedimento.Procedimento();
                                    proc = Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(fasc.systemID);
                                    if (proc != null && proc.Id == fasc.systemID)
                                    {
                                        idProcedimento = fasc.systemID;
                                        break;
                                    }
                                }
                            }

                            if (proc != null)
                            {
                                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, file.docNumber);
                                if (schedaDoc != null && schedaDoc.template != null)
                                {
                                    BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(proc.Id, "FIRMA", schedaDoc.template.ID_TIPO_ATTO, infoUtente);
                                }
                            }
                        }
                        SalvaStoricoIstanzaProcessoFirma(istanzaProcesso.idIstanzaProcesso, istanzaProcesso.docNumber, "Avviato processo di firma", infoUtente);
                        
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
            if (istanzaPasso.IsAutomatico)
                tipoPasso += "_AUTOMATICO";

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

            // Modifica per invio in mail dell'url del frontend
            string urlfrontend = "";
            if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

            trasmSing.trasmissioneUtente = trasmissioniUt;
            trasm.trasmissioniSingole = new System.Collections.ArrayList() { trasmSing };
            return BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, trasm);
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

        public static bool CheckCambioStatoDocDaLF(string idIstanzaProcesso)
        {
            bool retVal = false;

            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            retVal = libroFirma.CheckCambioStatoDocDaLF(idIstanzaProcesso);

            return retVal;
        }

        #region AVANZAMENTO STATO AUTOMATICO DA LIBRO FIRMA
        public static bool SalvaStatoAutomaticoLF(string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;
            try
            {
                //Recupero lo stato del documento e il diagramma a cui appartiene
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                DocsPaVO.DiagrammaStato.Stato statoDoc = diag.getStatoDoc(docnumber);
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDoc.ID_DIAGRAMMA));

                //Seleziono dai passi l'id dello stato automatico LF
                string idStatoAutomaticoSuccessivoLF = (from p in diagramma.PASSI.Cast<DocsPaVO.DiagrammaStato.Passo>()
                                                        where p.STATO_PADRE.SYSTEM_ID == statoDoc.SYSTEM_ID
                                                        select p.ID_STATO_AUTOMATICO_LF).FirstOrDefault();
                logger.Debug("ID_STATO_SUCCESSIVO_LF " + idStatoAutomaticoSuccessivoLF);
                DocsPaVO.DiagrammaStato.Stato statoAutomaticoSuccessivoLF = (from s in diagramma.STATI.Cast<DocsPaVO.DiagrammaStato.Stato>()
                                                                             where s.SYSTEM_ID.ToString().Equals(idStatoAutomaticoSuccessivoLF)
                                                                             select s).FirstOrDefault();

                if (statoAutomaticoSuccessivoLF != null)
                {
                    SalvaStatoDiagrammaDoc(statoAutomaticoSuccessivoLF, diagramma, docnumber, infoUtente);         
                    //Controllo che lo stato sia uno stato di conversione pdf lato server
                    //In caso affermativo faccio partire la conversione
                    DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                    if (BusinessLogic.Documenti.DocManager.isEnabledConversionePdfServer())
                    {
                        if (statoAutomaticoSuccessivoLF.CONVERSIONE_PDF)
                        {
                            ConvertiInPdf(schedaDocumento, infoUtente);
                        }
                    }
                    string idTemplate = schedaDocumento.template.SYSTEM_ID.ToString();

                    DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                    ArrayList modelli = new ArrayList(BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAuto(infoUtente.idAmministrazione, statoAutomaticoSuccessivoLF.SYSTEM_ID.ToString(), idTemplate));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)modelli[i];
                        if (mod.SINGLE == "1")
                        {
                            infoDoc = getInfoDocumento(schedaDocumento);
                            if(infoDoc != null)
                                effettuaTrasmissioneDocDaModello(mod, statoAutomaticoSuccessivoLF.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDocumento);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Count; k++)
                            {
                                if ((mod.MITTENTE[k] as DocsPaVO.Modelli_Trasmissioni.MittDest).ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali)
                                {
                                    infoDoc = getInfoDocumento(schedaDocumento);
                                    effettuaTrasmissioneDocDaModello(mod, statoAutomaticoSuccessivoLF.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDocumento);
                                    break;
                                }
                            }
                        }
                    }
                    //SE è STATO FINALE METTO IL DOCUMENTO IN SOLA LETTURA
                    if(statoAutomaticoSuccessivoLF.STATO_FINALE)
                    {
                        BusinessLogic.Documenti.DocManager.cambiaDirittiDocumenti((Convert.ToInt32(DocsPaVO.Security.SecurityItemInfo.SecurityAccessRightsEnum.ACCESS_RIGHT_45)), schedaDocumento.docNumber);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Errore in SalvaStatoAutomaticoLF " + ex.Message);
            }
            return retVal;
        }

        private static DocsPaVO.documento.InfoDocumento getInfoDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            try
            {
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();

                infoDoc.idProfile = schedaDocumento.systemId;
                infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
                infoDoc.docNumber = schedaDocumento.docNumber;
                infoDoc.tipoProto = schedaDocumento.tipoProto;
                infoDoc.evidenza = schedaDocumento.evidenza;

                if (schedaDocumento.registro != null)
                {
                    infoDoc.codRegistro = schedaDocumento.registro.codRegistro;
                    infoDoc.idRegistro = schedaDocumento.registro.systemId;
                }

                if (schedaDocumento.protocollo != null)
                {
                    infoDoc.numProt = schedaDocumento.protocollo.numero;
                    infoDoc.daProtocollare = schedaDocumento.protocollo.daProtocollare;
                    infoDoc.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
                    infoDoc.segnatura = schedaDocumento.protocollo.segnatura;

                    if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
                    {
                        string[] mittDest = new string[1];
                        DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                        if (pe != null && pe.mittente != null && infoDoc.mittDest != null && infoDoc.mittDest.Count > 0)
                        {
                            mittDest[0] = pe.mittente.descrizione;
                        }
                        infoDoc.mittDest.AddRange(mittDest);
                    }
                    else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
                    {
                        DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                        if (pu.destinatari != null)
                        {
                            string[] mittDest = new string[pu.destinatari.Count];
                            for (int i = 0; i < pu.destinatari.Count; i++)
                                mittDest[i] = ((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione;
                            infoDoc.mittDest.AddRange(mittDest);
                        }
                    }
                }
                else
                {
                    infoDoc.dataApertura = schedaDocumento.dataCreazione;
                }

                infoDoc.privato = schedaDocumento.privato;
                infoDoc.personale = schedaDocumento.personale;

                return infoDoc;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Tramissione di un documento usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneDocDaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, string idStato,
            DocsPaVO.documento.InfoDocumento infoDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            
            try
            {
               DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                trasmissione.infoDocumento = infoDocumento;

                DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

                trasmissione.ruolo = u.GetRuoloByIdGruppo(infoUtente.idGruppo);//istanzaProcesso.RuoloProponente;
                trasmissione.utente = u.getUtenteById(infoUtente.idPeople);//istanzaProcesso.UtenteProponente;
                if (modello != null)
                    trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                        DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDoc, infoUtente, trasmissione.ruolo);
                        }
                        if (corr != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, infoUtente, trasmissione.ruolo);
                        }
                    }
                }
                trasmissione = impostaNotificheUtentiDaModello(trasmissione, modello);

                //
                // Aggiunto codice mancante per segnalazione Zanotti
                if (trasmissione != null && modello.CEDE_DIRITTI.Equals("1"))
                {

                    if (trasmissione.cessione == null)
                    {
                        DocsPaVO.documento.CessioneDocumento cessione = new DocsPaVO.documento.CessioneDocumento();
                        cessione.docCeduto = true;
                        cessione.idPeople = infoUtente.idPeople;
                        cessione.idRuolo = infoUtente.idGruppo;
                        cessione.userId = infoUtente.userId;
                        cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                        cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                        trasmissione.cessione = cessione;
                    }
                }
                //
                // End Aggiunta codice per segnalazione Zanotti


                trasmissione = saveExecuteTrasm(trasmissione, infoUtente);
                if (idStato != null && idStato != "")
                    BusinessLogic.DiagrammiStato.DiagrammiStato.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, infoDocumento.docNumber, idStato);

            }
            catch (System.Exception ex)
            {
                
            }
        }

        public static DocsPaVO.trasmissione.Trasmissione saveExecuteTrasm(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.trasmissione.Trasmissione result = null;
            string desc = string.Empty;
            try
            {
                string urlfrontend = "";
                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                if (infoUtente.delegato != null)
                    trasmissione.delegato = infoUtente.delegato.idPeople;
                result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, trasmissione);
                string notify = "1";
                if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (result != null)
                {
                    // LOG per documento
                    if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (result.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }
                }
                    if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                {
                    if (trasmissione.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();
                    BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO, null);
                }
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneSaveExecuteTrasm - ", ex);
                result = null;
            }
            return result;
        }

        private static DocsPaVO.trasmissione.Trasmissione impostaNotificheUtentiDaModello(DocsPaVO.trasmissione.Trasmissione objTrasm, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            try
            {
                if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Count > 0)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola trasmSingola;
                    for (int cts = 0; cts < objTrasm.trasmissioniSingole.Count; cts++)
                    {
                        trasmSingola = objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola;
                        if ((objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola).trasmissioneUtente.Count > 0)
                        {
                            DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente;
                            for (int ctu = 0; ctu < (objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola).trasmissioneUtente.Count; ctu++)
                            {
                                trasmUtente = trasmSingola.trasmissioneUtente[ctu] as DocsPaVO.trasmissione.TrasmissioneUtente;
                                trasmUtente.daNotificare = daNotificareSuModello(trasmUtente.utente.idPeople, trasmSingola.corrispondenteInterno.systemId, modello);
                            }
                        }
                    }
                }
                return objTrasm;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        private static bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            bool retValue = true;
            try
            {
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                        if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                        {
                            if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Count > 0)
                            {
                                for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Count; cut++)
                                {
                                    if ((mittDest.UTENTI_NOTIFICA[cut] as DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm).ID_PEOPLE.Equals(currentIDPeople))
                                    {
                                        if ((mittDest.UTENTI_NOTIFICA[cut] as DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm).FLAG_NOTIFICA.Equals("1"))
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
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenti(string tipo_destinatario, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            try
            {
                DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
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
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
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
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
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
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);//.getCorrispondenteByIdPeople(idPeople, tipoIE, u);

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
                        idCorr = ruolo.systemId;
                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
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
                        idCorr = ruolo.systemId;
                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //ruolo responsabile uo mittente
                    if (tipo_destinatario == "RSP_M")
                    {
                        string idCorrGlobaliUo = ruolo.uo.systemId;
                        string idCorr = ruolo.systemId;

                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }

                    //ruolo segretario uo mittente
                    if (tipo_destinatario == "S_M")
                    {
                        string idCorrGlobaliUo = ruolo.uo.systemId;
                        string idCorr = ruolo.systemId;

                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                }
                return corr;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            try
            {
                return addTrasmissioneSingola(trasmissione, corr, ragione, note, tipoTrasm, scadenza, false, infoUtente, ruolo);
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Debug("INIZIO addTrasmissioneSingola");
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            //Quando la ragione di trasmissione ha mantieni lettura, anche la trasmissione deve mantenere la lettura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniLettura = true;
                }
            }

            // Mev Cessione Diritti - Mantieni Scrittura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniScrittura = true;
                }
            }
            // End Mev


            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
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
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr, infoUtente);
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;

                    //Andrea
                    //throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                    //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + ".");
                    //End Andrea
                }
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;

                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    //throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + " è inesistente.");

                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = ruolo;
                qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                DocsPaVO.utente.Ruolo[] ruoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo).Cast<DocsPaVO.utente.Ruolo>().ToArray();

                //Andrea
                if (ruoli == null || ruoli.Length == 0)
                {
                    //throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                    //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPaVO.utente.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti, infoUtente, ruolo);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
            logger.Debug("FINE addTrasmissioneSingola");
            return trasmissione;
        }

        private static DocsPaVO.utente.Corrispondente[] queryUtenti(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                //costruzione oggetto queryCorrispondente
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.codiceRubrica = corr.codiceRubrica;
                qco.getChildren = true;
                qco.idAmministrazione = infoUtente.idAmministrazione;
                qco.fineValidita = true;

                //corrispondenti interni
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                return BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco).Cast<DocsPaVO.utente.Corrispondente>().ToArray();
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static ArrayList addTrasmissioneSingola(ArrayList array, DocsPaVO.trasmissione.TrasmissioneSingola nuovoElemento)
        {
            try
            {
                ArrayList nuovaLista = new ArrayList();
                if (array != null)
                {
                    nuovaLista.AddRange(array);
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
                else
                {
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static ArrayList addTrasmissioneUtente(ArrayList array, DocsPaVO.trasmissione.TrasmissioneUtente nuovoElemento)
        {
            try
            {
                ArrayList nuovaLista = new ArrayList();
                if (array != null)
                {
                    nuovaLista.AddRange(array);
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
                else
                {
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Converte in PDF il file
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        private static void ConvertiInPdf(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0], infoUtente);
            if (fileDocumento != null && fileDocumento.content != null && fileDocumento.name != null && fileDocumento.name != "")
            {
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ASPOSE_PDF_CONVERSION")) &&
                   DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ASPOSE_PDF_CONVERSION").ToString().Equals("1"))
                {
                    BusinessLogic.Modelli.AsposeModelProcessor.DocModelProcessor processor = new BusinessLogic.Modelli.AsposeModelProcessor.DocModelProcessor();
                    processor.ConvertToPdfAsync(fileDocumento.content, (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0], infoUtente);
                }
                else
                {
                    DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion = new DocsPaVO.documento.ObjServerPdfConversion();
                    objServerPdfConversion.content = fileDocumento.content;
                    objServerPdfConversion.fileName = (schedaDocumento.documenti[0] as DocsPaVO.documento.FileRequest).fileName;
                    objServerPdfConversion.idProfile = schedaDocumento.systemId;
                    objServerPdfConversion.docNumber = schedaDocumento.docNumber;

                    if (!string.IsNullOrEmpty(objServerPdfConversion.idProfile) && !string.IsNullOrEmpty(objServerPdfConversion.docNumber))
                        BusinessLogic.LiveCycle.LiveCyclePdfConverter.EnqueueServerPdfConversion(infoUtente, objServerPdfConversion, null);
                }
            }
        }
        #endregion
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
        /// Metodo per l'estrazione del dettaglio dell'istanza di processo di firma
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            IstanzaProcessoDiFirma istanzaProcessoDiFirma = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                istanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(idIstanzaProcesso, infoUtente);
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

        public static bool IsTitolarePassoInAttesa(string docnumber, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LibroFirma.Azione azione)
        {
            bool result = true;
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            IstanzaPassoDiFirma istanza = libroFirma.GetIstanzaPassoFirmaInAttesaByDocnumber(docnumber);
            if(istanza != null)
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

        public static IstanzaPassoDiFirma GetIstanzaPassoFirmaInAttesaByDocnumber(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.GetIstanzaPassoFirmaInAttesaByDocnumber(docnumber);
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

        public static bool CanExecuteAction(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {        
                //Se il documento è in libro firma, posso eseguire l'azione solo se sono il titolare del passo e l'azione corrisponde al passo
                if(IsDocInLibroFirma(fileRequest.docNumber))
                {
                    //Caso allegato o doc principale: se sono il titolare del passo posso eseguire l'azione.
                    if (IsTitolare(fileRequest.docNumber, infoUtente))
                        return true;
                    else
                        return false;
                }

                if (IsModificaBloccataPerDocumentoPrincipaleInLF(fileRequest.docNumber, infoUtente.idAmministrazione))
                    return false;

            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CanExecuteAction ", e);
            }
            return result;
        }

        public static bool IsModificaBloccataPerDocumentoPrincipaleInLF(string docnumber, string idAmm)
        {
            bool result = false;
            try
            {
                if (IsAttivoBloccoModificheDocumentoInLibroFirma(idAmm) && IsDocumentoPrincipaleInLF(docnumber))
                    return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsModificaBloccataPerDocumentoPrincipaleInLF ", e);
            }
            return result;
        }

        public static bool IsAttivoBloccoModificheDocumentoInLibroFirma(string idAmm)
        {
            bool result = false;
            try
            {
                string attivo = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BLOCCO_MODIFICHE_DOC_IN_LF");
                if (!string.IsNullOrEmpty(attivo) && !attivo.Equals("0"))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsAttivoBloccoDocumentiInLibroFirma ", e);
                return false;
            }
            return result;
        }

        public static bool IsModelloDiFirma(string idProcesso)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsModelloDiFirma(idProcesso);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsModelloDiFirma ", e);
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

        public static bool CheckAllegatiInLibroFirma(string idDocumentoPrincipale)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.CheckAllegatiInLibroFirma(idDocumentoPrincipale);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CheckAllegatiInLibroFirma ", e);
                return false;
            }
        }

        public static bool IsDocumentoPrincipaleInLF(string docNumber)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsDocumentoPrincipaleInLF(docNumber);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsDocumentoPrincipaleInLF ", e);
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
                        result = libroFirma.InterruptionSignatureProcess(istanza.idIstanzaProcesso, TipoStatoProcesso.STOPPED, istanza.docNumber, noteInterruzione, dateInterruption, interrottoDa, infoUtente);
                        if (result)
                        {

                            SalvaStoricoIstanzaProcessoFirma(istanza.idIstanzaProcesso, istanza.docNumber, "Interruzione del processo di firma", infoUtente);
                            //Verifico se è stato avviato dal passaggio di stato, in caso torno allo stato precedente
                            DocsPaVO.DiagrammaStato.Stato statoDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(istanza.docNumber);
                            if (statoDiagramma != null && !string.IsNullOrEmpty(statoDiagramma.ID_PROCESSO_FIRMA))
                            { 
                                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                                DocsPaVO.DiagrammaStato.Stato statoDiagrammaPrecedente = diag.GetStatoDocPrecedente(istanza.docNumber);
                                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDiagrammaPrecedente.ID_DIAGRAMMA));
                                SalvaStatoDiagrammaDoc(statoDiagrammaPrecedente, diagramma, istanza.docNumber, infoUtente);
                            }
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

        public static void SalvaStatoDiagrammaDoc(DocsPaVO.DiagrammaStato.Stato stato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(docnumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.userId, infoUtente, string.Empty);
                string method = "DOC_CAMBIO_STATO";
                
                //Se non ho il ruolo vuol dire che stò effettuando l'operazione d'amministrazione
                if (string.IsNullOrEmpty(infoUtente.idGruppo))
                    method = "DOC_CAMBIO_STATO_ADMIN";

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, docnumber, String.Format("Stato passato a  {0}", stato.DESCRIZIONE.ToUpper()), DocsPaVO.Logger.CodAzione.Esito.OK,
                    infoUtente.delegato, "1");
            }
            catch(Exception e)
            {
                logger.Error("Errore durante il cambio di stato " + e);
            }
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
                            result = libroFirma.InterruptionSignatureProcess(istanza.idIstanzaProcesso, TipoStatoProcesso.STOPPED, istanza.docNumber, noteInterruzione, dateInterruption, interrottoDa, infoUtente);
                            if (result)
                            {

                                SalvaStoricoIstanzaProcessoFirma(istanza.idIstanzaProcesso, istanza.docNumber, noteInterruzione, infoUtente);
                                //Verifico se è stato avviato dal passaggio di stato, in caso torno allo stato precedente
                                if (istanza.AttivatoPerPassaggioStato)
                                {
                                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                                    DocsPaVO.DiagrammaStato.Stato statoDiagrammaPrecedente = diag.GetStatoDocPrecedente(istanza.docNumber);
                                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDiagrammaPrecedente.ID_DIAGRAMMA));
                                    SalvaStatoDiagrammaDoc(statoDiagrammaPrecedente, diagramma, istanza.docNumber, infoUtente);
                                }

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
                        result = libroFirma.InterruptionSignatureProcess(elemento.IdIstanzaProcesso, TipoStatoProcesso.STOPPED, elemento.InfoDocumento.Docnumber, elemento.MotivoRespingimento, dateInterruption, interrottoDa, infoUtente);
                        if (result)
                        {
                            result = libroFirma.UpdateStatoIstanzaPasso(elemento.IdIstanzaPasso, elemento.InfoDocumento.VersionId, TipoStatoPasso.STUCK.ToString(), infoUtente, dateInterruption);
                        }
                        if (result)
                        {
                            SalvaStoricoIstanzaProcessoFirma(elemento.IdIstanzaProcesso, elemento.InfoDocumento.Docnumber, "Interruzione del processo di firma", infoUtente);
                            //Verifico se è stato avviato dal passaggio di stato, in caso torno allo stato precedente
                            DocsPaVO.DiagrammaStato.Stato statoDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(elemento.InfoDocumento.Docnumber);
                            if (statoDiagramma != null && !string.IsNullOrEmpty(statoDiagramma.ID_PROCESSO_FIRMA))
                            {
                                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                                DocsPaVO.DiagrammaStato.Stato statoDiagrammaPrecedente = diag.GetStatoDocPrecedente(elemento.InfoDocumento.Docnumber);
                                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDiagrammaPrecedente.ID_DIAGRAMMA));
                                SalvaStatoDiagrammaDoc(statoDiagrammaPrecedente, diagramma, elemento.InfoDocumento.Docnumber, infoUtente);
                            }
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

        public static List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtro, int numPage, int pageSize, out int numTotPage, out int nRec, DocsPaVO.utente.InfoUtente infoUtente, out DataSet istanzeProcessi)
        {
            numTotPage = 0;
            nRec = 0;
            istanzeProcessi = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetIstanzaProcessiDiFirmaByFilter(filtro, numPage, pageSize, out numTotPage, out nRec, infoUtente, out istanzeProcessi);
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

        public static ProcessoFirma DuplicaProcessoFirma(string idProcessoOld, string nomeNuovoProcesso, bool copiaVisibilita, DocsPaVO.utente.InfoUtente utente, out DocsPaVO.LibroFirma.ResultProcessoFirma resultCreazioneProcesso)
        {
            resultCreazioneProcesso = ResultProcessoFirma.OK;
            ProcessoFirma newProcesso = new ProcessoFirma();
            DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    newProcesso.nome = nomeNuovoProcesso;
                    newProcesso = InsertProcessoDiFirma(newProcesso, utente, out resultCreazioneProcesso);
                    if (newProcesso != null && resultCreazioneProcesso.Equals(ResultProcessoFirma.OK))
                    {
                        if (!DuplicaPassiFirmaByIdProcesso(newProcesso.idProcesso, idProcessoOld, utente))
                        {
                            resultCreazioneProcesso = ResultProcessoFirma.KO;
                        }
                        else
                        {
                            if (copiaVisibilita && !CopiaVisibilitaByIdProcesso(newProcesso.idProcesso, idProcessoOld, utente))
                                resultCreazioneProcesso = ResultProcessoFirma.KO;
                        }
                    }
                    if (resultCreazioneProcesso.Equals(ResultProcessoFirma.OK))
                    {
                        transactionContext.Complete();
                        transactionContext.Dispose();
                        newProcesso = libro.GetProcessoDiFirmaById(newProcesso.idProcesso, utente);
                            //libro.GetPassiProcessoDiFirma(newProcesso.idProcesso);
                        return newProcesso;
                    }
                    else
                    {
                        transactionContext.Dispose();
                        return null;
                    }
                }
                catch (Exception e)
                {
                    transactionContext.Dispose();
                    logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: DuplicaProcessoFirma ", e);
                    resultCreazioneProcesso = ResultProcessoFirma.KO;
                    return null;
                }
            }
        }

        private static bool DuplicaPassiFirmaByIdProcesso(string idProcessoNew, string idProcessoOld, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.DuplicaPassiDiFirmaByIdProcesso(idProcessoNew, idProcessoOld, infoUtente);
                if (result)
                {
                    result = libroFirma.AggiornaTipoProcessoFirma(idProcessoNew, infoUtente);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: DuplicaPassiFirmaByIdProcesso ", e);
                return false;
            }
            return result;
        }

        private static bool CopiaVisibilitaByIdProcesso(string idProcessoNew, string idProcessoOld, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.CopiaVisibilitaByIdProcesso(idProcessoNew, idProcessoOld, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CopiaVisibilitaByIdProcesso ", e);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Esportazione processi di firma
        /// </summary>
        /// <param name="tipoReport"></param>
        /// <param name="idFunzione"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento GetReportProcessiFirma(string idRuolo, string idUtente, string formato)
        {
            DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
            try
            {

                // filtri report
                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idRuolo", valore = idRuolo });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idUtente", valore = idUtente });

                // request generazione report
                DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
                request.SearchFilters = filters;

                // selezione formato report
                switch (formato)
                {
                    case "XLS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                        break;

                    case "ODS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                        break;
                }

                string descrizione = string.Empty;

                string descrizioneRuolo = string.Empty;
                string descrizioneUtente = string.Empty;

                if (!string.IsNullOrEmpty(idRuolo))
                    descrizioneRuolo = BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdGroup(idRuolo);

                if (!string.IsNullOrEmpty(idUtente))
                    descrizioneUtente = BusinessLogic.Utenti.UserManager.getUtente(idUtente).descrizione;
                
                request.ContextName = "ExportProcessiDiFirma";
                request.ReportKey = "ExportProcessiDiFirma";
                request.Title = string.Empty;

                if(!string.IsNullOrEmpty(descrizioneRuolo) && !string.IsNullOrEmpty(descrizioneUtente))
                    request.SubTitle = string.Format("Utente: {0} - Ruolo: {1}", descrizioneUtente, descrizioneRuolo);
                else if(!string.IsNullOrEmpty(descrizioneRuolo))
                    request.SubTitle = string.Format("Ruolo: {0}", descrizioneRuolo);
                else if (!string.IsNullOrEmpty(descrizioneUtente))
                    request.SubTitle = string.Format("Utente: {0}", descrizioneUtente);

                request.AdditionalInformation = string.Empty;

                report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return report;
        }

        /// <summary>
        /// Invalida i processi di firma ed interrompe le istanze del ruolo/utente specificato
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static bool InvalidaPassiCorrelatiTitolare(string idRuolo, string idPeople, string tipoTick, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                //Inserisco i processi da invalidare in una tabella
                DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                string idReport = libro.InsertReportProcessiTick(idPeople, idRuolo);

                if (InvalidaProcessiTitolare(idRuolo, idPeople, tipoTick))
                    InviaReportCreatoriProcessi(idReport, infoUtente);
                InterrompiIstanzeTitolare(idRuolo, idPeople, infoUtente);

                //libro.DeleteReportProcessiTick(idReport);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaPassiCorrelatiTitolare ", ex);
            }
            return result;
        }

        private static bool InvalidaProcessiTitolare(string idRuolo, string idPeople, string tipoTick)
        {
            bool result = true;
            try
            {         
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                //Se il processo è associato ad un diagramma degli stati lo rimuovo
                libroFirma.DisassociaProcessoDaDiagrammaStato(idRuolo, idPeople);

                result = libroFirma.InvalidaProcessiFirmaTitolare(idRuolo, idPeople, tipoTick);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaProcessiTitolare ", ex);
            }
            return result;
        }

        /// <summary>
        /// Invio un Report con i processi invalidati rispettivamente ad ogni ruolo creatore
        /// </summary>
        /// <param name="idReport"></param>
        /// <param name="infoUtente"></param>
        private static void InviaReportCreatoriProcessi(string idReport, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                List<string> listaIdRuoliCreatori = libro.GetListaIdCreatoriProcessiInvalidati(idReport);
                if (listaIdRuoliCreatori != null && listaIdRuoliCreatori.Count > 0)
                {
                    string basePathFiles = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                    string oggetto = "Report processi invalidati";
                    string body = "A seguito delle modifiche dei ruoli coinvolti nei processi/modelli di firma da Lei creati, viene inoltrato il report dei processi invalidati.";
                    //Ricerca se esiste l'email from notifica dell'amministrazione
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    string idAmministrazionePerMail = string.Empty;
                    string fromEmailAmministra = amm.GetEmailAddress(infoUtente.idAmministrazione);
                    DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                    foreach (string idRuoloCreatore in listaIdRuoliCreatori)
                    {
                        report = GeneraReportCreatoreProcessi(idReport, idRuoloCreatore, infoUtente);

                        //Estraggo gli utenti a cui inviare il report
                        DocsPaVO.utente.Ruolo ruolo = utenti.GetRuoloByIdGruppo(idRuoloCreatore);
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
                        string emailsDest = string.Empty;
                        string emailUser = string.Empty;
                        for (int k = 0; k < listaUtenti.Count; k++)
                        {
                            emailUser = ((DocsPaVO.utente.Utente)listaUtenti[k]).email;
                            if (!string.IsNullOrEmpty(emailUser))
                                emailsDest += string.IsNullOrEmpty(emailsDest) ? emailUser : "," + emailUser; 
                        }
                        if (!string.IsNullOrEmpty(emailsDest))
                        {
                            CMAttachment[] allegato = new CMAttachment[1];
                            allegato[0] = CreaAllegatoMail(report);
                            if (allegato[0] != null)
                            {
                                bool res = false;
                                res = Notifica.notificaByMail(emailsDest, fromEmailAmministra, oggetto, body, string.Empty, ruolo.idAmministrazione, allegato);
                                if (!res)
                                {
                                    logger.Error("Errore durante l'invio della mail contentente il report");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        private static CMAttachment CreaAllegatoMail(DocsPaVO.documento.FileDocumento report)
        {
            CMAttachment allegato ;
            System.IO.FileStream fs = null;
            try
            {
                string basePathFiles = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");

                string pathFiles = System.IO.Path.Combine(basePathFiles, @"ReportProcessiInvalidtati\" + Guid.NewGuid().ToString());
                DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);
                string fileAttPath = pathFiles + "\\" + report.fullName;
                fs = new System.IO.FileStream(fileAttPath, System.IO.FileMode.Create);
                fs.Write(report.content, 0, report.content.Length);
                fs.Close();
                fs = null;

                allegato = new CMAttachment(System.IO.Path.GetFileName(fileAttPath), Interoperabilità.MimeMapper.GetMimeType(System.IO.Path.GetExtension(fileAttPath)), fileAttPath);

            }
            catch (Exception ex)
            {
                logger.Error("Errore nella creazione dell'allegato alla mail" + ex);
                fs.Close();
                fs = null;
                allegato = null;
            }
            return allegato;
        }

        private static DocsPaVO.documento.FileDocumento GeneraReportCreatoreProcessi(string idReport, string idRuoloCreatore, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
            try
            {
                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idReport", valore = idReport });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idRuoloCreatore", valore = idRuoloCreatore });

                // request generazione report
                DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
                request.SearchFilters = filters;
                request.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
                request.Title = "Processi Invalidati";

                request.ContextName = "ExportProcessiDiFirmaInvalidati";
                request.ReportKey = "ExportProcessiDiFirmaInvalidati";
                request.Title = string.Empty;
                request.AdditionalInformation = string.Empty;

                report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                report = null;
            }
            return report;
        }

        private static bool InterrompiIstanzeTitolare(string idRuolo, string idPeople, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                List<IstanzaProcessoDiFirma> istanzaProcessiCoinvolti = libroFirma.GetIstanzaProcessiDiFirmaByTitolare(idRuolo, idPeople);

                if (istanzaProcessiCoinvolti.Count() > 0)
                {
                    string noteInterruzione = "Interrotto processo per modifica da amministrazione di un ruolo coinvolto.";
                    result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByAdministrator(istanzaProcessiCoinvolti.ToArray(), noteInterruzione, infoUtente);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterrompiIstanzeRuoloCoinvolto ", ex);
            }
            return result;
        }

        public static bool AmmSostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            try
            {
                if (!string.IsNullOrEmpty(idRuolo) && !string.IsNullOrEmpty(idOldPeople) && !string.IsNullOrEmpty(idNewPeople))
                {
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        if (!SostituisciUtentePassoProcesso(idRuolo, idOldPeople, idNewPeople))
                        {
                            result = false;
                            throw new Exception("Errore durante la sostituzione dell'utente nei passi del processo");
                        }
                        if (!SostituisciUtentePassoIstanza(idRuolo, idOldPeople, idNewPeople))
                        {
                            result = false;
                            throw new Exception("Errore durante la sostituzione dell'utente nei passi dell'istanza");
                        }
                        if (!SostituisciUtenteElementiInLibroFirma(idRuolo, idOldPeople, idNewPeople))
                        {
                            result = false;
                            throw new Exception("Errore durante la sostituzione dell'utente negli elementi in libro firma");
                        }
                        if (result)
                            transactionContext.Complete();
                    }
                }
                else
                {
                    logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmSostituisciUtentePassiCorrelati: non sono presenti i campi necessari");
                    result = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmSostituisciUtentePassiCorrelati ", ex);
            }
            return result;
        }

        private static bool SostituisciUtentePassoProcesso(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.SostituisciUtentePassoProcesso(idRuolo, idOldPeople, idNewPeople);
            }
            catch(Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: SostituisciUtentePassoProcesso ", ex);
            }
            return result;
        }

        private static bool SostituisciUtentePassoIstanza(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.SostituisciUtentePassoIstanza(idRuolo, idOldPeople, idNewPeople);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: SostituisciUtentePassoIstanza ", ex);
            }
            return result;
        }

        private static bool SostituisciUtenteElementiInLibroFirma(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.SostituisciUtenteElementiInLibroFirma(idRuolo, idOldPeople, idNewPeople);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: SostituisciUtenteElementiInLibroFirma ", ex);
            }
            return result;
        }

        public static bool AmmStoricizzaRuoloPassiCorrelati(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            try
            {
                if (!string.IsNullOrEmpty(idRuoloOld) && !string.IsNullOrEmpty(idRuoloNew))
                {
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        if (!StoricizzaRuoloPassoProcesso(idRuoloOld, idRuoloNew))
                        {
                            result = false;
                            throw new Exception("Errore durante la storicizzazione del ruolo nei passi del processo");
                        }
                        if (!StoricizzaRuoloPassoIstanza(idRuoloOld, idRuoloNew))
                        {
                            result = false;
                            throw new Exception("Errore durante la storicizzazione del ruolo passi dell'istanza");
                        }
                        if (!StoricizzaRuoloElementiInLibroFirma(idRuoloOld, idRuoloNew))
                        {
                            result = false;
                            throw new Exception("Errore durante la storicizzazione del ruolo negli elementi in libro firma");
                        }
                        if (result)
                            transactionContext.Complete();
                    }
                }
                else
                {
                    logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmStoricizzaRuoloPassiCorrelati: non sono presenti i campi necessari");
                    result = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmStoricizzaRuoloPassiCorrelati ", ex);
            }
            return result;
        }


        private static bool StoricizzaRuoloPassoProcesso(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.StoricizzaRuoloPassoProcesso(idRuoloOld, idRuoloNew);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: StoricizzaRuoloPassoProcesso ", ex);
            }
            return result;
        }

        private static bool StoricizzaRuoloPassoIstanza(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.StoricizzaRuoloPassoIstanza(idRuoloOld, idRuoloNew);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: StoricizzaRuoloPassoIstanza ", ex);
            }
            return result;
        }

        private static bool StoricizzaRuoloElementiInLibroFirma(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                result = libroFirma.StoricizzaRuoloElementiInLibroFirma(idRuoloOld, idRuoloNew);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: StoricizzaRuoloElementiInLibroFirma ", ex);
            }
            return result;
        }

        public static List<ProcessoFirma> GetProcessiDiFirmaByIdAmm(string idAmministrazione)
        {
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                listProcessiDiFirma = libroFirma.GetProcessiFirmaByIdAmm(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByIdAmm ", e);
            }
            return listProcessiDiFirma;
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

        public static string GetDescDiagrammiByIdProcesso(string idProcesso)
        {
            string descDiagramma = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                descDiagramma = libroFirma.GetDescDiagrammiByIdProcesso(idProcesso);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetDescDiagrammiByIdProcesso ", e);
            }
            return descDiagramma;
        }

        /// <summary>
        /// Torna true se l'evento in input può essere un evento di passo automatico
        /// </summary>
        /// <param name="codiceAzione"></param>
        /// <returns></returns>
        public static bool IsEventoAutomatico(string codiceAzione)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.IsEventoAutomatico(codiceAzione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsEventoAutomatico ", e);
                return false;
            }
        }

        public static DocsPaVO.amministrazione.CasellaRegistro GetCasellaRegistroByIdMail(string idMail)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.GetCasellaRegistroByIdMail(idMail);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCasellaRegistroByIdMail ", e);
                return null;
            }
        }

        public delegate void EseguiPassoAutomaticoDelegate(IstanzaPassoDiFirma passo, IstanzaProcessoDiFirma istanzaProcesso);


        private static void CallBack(IAsyncResult result)
        {

            var del = result.AsyncState as EseguiPassoAutomaticoDelegate;

            if (del != null)
                del.EndInvoke(result);
        }

        public static void EseguiPassoAutomaticoAsync(IstanzaPassoDiFirma passo, IstanzaProcessoDiFirma istanzaProcesso)
        {
            AsyncCallback callback = new AsyncCallback(CallBack);
            EseguiPassoAutomaticoDelegate esecuzionePassoAutomatico = new EseguiPassoAutomaticoDelegate(EseguiPassoAutomatico);
            esecuzionePassoAutomatico.BeginInvoke(passo, istanzaProcesso, callback, esecuzionePassoAutomatico);
        }

        public static void EseguiPassoAutomatico(IstanzaPassoDiFirma passo, IstanzaProcessoDiFirma istanzaProcesso)
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            try
            {
                Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), passo.CodiceTipoEvento, true);
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(passo.RuoloCoinvolto.idGruppo);
                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.GetUtenteAutomatico(ruolo.idAmministrazione);
                DocsPaVO.utente.InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utente, ruolo);
                string docnumber = istanzaProcesso.docNumber;
                switch (codiceEvento)
                {
                    case Azione.RECORD_PREDISPOSED:
                        string segnatura = string.Empty;
                        if(Protocolla(docnumber, passo, infoUtente, ruolo, out segnatura))
                        {
                            //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                            if(istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                            {
                                libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);
                            }
                            string varDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", docnumber, "Segnatura: ", segnatura);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "RECORDPREDISPOSED", docnumber,
                                    varDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                        }
                        break;
                    case Azione.DOCUMENTO_REPERTORIATO:
                        string numRep = string.Empty;
                        if(Repertoria(docnumber, passo, infoUtente, ruolo, out numRep))
                        {
                            //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                            if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                            {
                                libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);
                            }
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", docnumber, "Repertoriato documento: " + numRep, DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                        break;
                    case Azione.DOCUMENTOSPEDISCI:
                        if(Spedisci(docnumber, passo, infoUtente, ruolo))
                        {
                            //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                            if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                            {
                                libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);
                            }
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", docnumber, string.Format("{0}{1}", "Spedizione del doc: ", docnumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: EseguiPassoAutomatico ", e);
                libro.SetErroreIstanzaPassoFirma(e.Message, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_ERROR);
            }
        }

        private static bool Repertoria(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, out string numRep)
        {
            bool result = false;
            numRep = string.Empty;

            DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
            DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(docnumber);

            if (template == null || template.SYSTEM_ID == 0)
            {
                throw new Exception("Errore repertoriazione: il documento non risulta tipizzato.");
            }
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = (from ogg in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                   where ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") || ogg.TIPO.DESCRIZIONE_TIPO.Equals("ContatoreSottocontatore")
                                                                   select ogg).FirstOrDefault();
            if(oggetto == null)
            {
                throw new Exception("Errore repertoriazione: alla tipologia non risulta associato un contatore di repertorio.");
            }
            /*List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> dirittiCampiRuolo = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(ruolo.idGruppo, idTemplate).Cast<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
            bool ruoloInsRepertorio = false;
            foreach(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if(assDocFascRuoli.ID_OGGETTO_CUSTOM == oggetto.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "2")
                        ruoloInsRepertorio = true;
                    break;
                }
            }
            if (!ruoloInsRepertorio)
            {
                throw new Exception("Errore repertoriazione: il ruolo non è abilitato a far scattare il contatore di repertorio.");
            }
            */
            
            string codiceAOO_RF = string.Empty;
            if(oggetto.TIPO_CONTATORE.Equals("A"))
            {
                codiceAOO_RF = passo.IdAOO;
            }
            if (oggetto.TIPO_CONTATORE.Equals("R"))
            {
                codiceAOO_RF = passo.IdRF;
            }
            oggetto.ID_AOO_RF = codiceAOO_RF;
            oggetto.CONTATORE_DA_FAR_SCATTARE = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                model.salvaInserimentoUtenteProfDim(template, docnumber);
                transactionContext.Complete();
            }
            string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Codice;
            string dataAnnullamento = String.Empty;
            numRep = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(docnumber, codiceAmm, false, out dataAnnullamento);
            if (string.IsNullOrEmpty(numRep))
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber, "Errore nell'esecuzione del passo automatico di repertoriazione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                throw new Exception("Errore repertoriazione.");
            }
            else
            {
                result = true;
            }
            return result;
        }

        private static bool Protocolla(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, out string segnatura)
        {
            bool result = false;
            segnatura = string.Empty;
            DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
            if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
            {
                throw new Exception("Errore protocollazione: il documento è già protocollato.");
            }
            if (!isPredisposto(schedaDoc))
            {
                throw new Exception("Errore protocollazione: il documento non è predisposto per la protocollazione.");
            }
            if(schedaDoc.registro.stato.Equals("C"))
            {
                throw new Exception("Errore protocollazione: il registro è chiuso.");
            }
            //DocsPaVO.utente.Registro registro = BusinessLogic.Utenti.RegistriManager.getRegistro(schedaDoc.re)
            schedaDoc.id_rf_prot = passo.IdRF;
            schedaDoc.id_rf_invio_ricevuta = passo.IdRF;
            schedaDoc.cod_rf_prot = BusinessLogic.Utenti.RegistriManager.getRegistro(passo.IdRF).codRegistro;
            DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
            try
            {
                BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out resultProtocollazione);
            }
            catch (Exception e)
            {
                throw new Exception("Errore protocollazione.");
            }
            if (!resultProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK))
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di protocollazione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                throw new Exception("Errore protocollazione: " + resultProtocollazione.ToString());
            }
            else
            {
                segnatura = schedaDoc.protocollo.segnatura;
                result = true;
            }

            return result;
        }

        private static bool Spedisci(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            bool result = false;
            DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
            if (schedaDoc.protocollo == null || string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
            {
                throw new Exception("Errore spedizione: il documento non è protocollato.");
            }
            if (schedaDoc.protocollo != null && schedaDoc.tipoProto.Equals("A"))
            {
                throw new Exception("Errore spedizione: non è possibile spedire un protocollo in arrivo.");
            }
            //Se uno degli allegati del documento è in Libro Firma blocco la spedizione
            if(CheckAllegatiInLibroFirma(docnumber))
            {
                throw new Exception("Errore spedizione: non è possibile spedire il protocollo poichè è attivo un processo di firma per i suoi allegati.");
            }

            if ((schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza == null)
                (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza = new ArrayList();
            DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetSpedizioneDocumento(infoUtente, schedaDoc);

            //Estraggo la casella da cui effettuare la spedizione
            DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            DocsPaVO.amministrazione.CasellaRegistro casella = libro.GetCasellaRegistroByIdMail(passo.IdMailRegistro);
            infoSpedizione.mailAddress = casella.EmailRegistro;
            infoSpedizione.IdRegistroRfMittente = casella.IdRegistro;
            try
            {
                BusinessLogic.Spedizione.SpedizioneManager.SpedisciDocumento(infoUtente, schedaDoc, infoSpedizione);
            }
            catch (Exception e)
            {
                throw new Exception("Errore spedizione: " + e.Message);
            }

            string destinatariNonRaggiunti = string.Empty;
            DocsPaVO.utente.Corrispondente corr;
            if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
            {
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
                {
                    if (dest.IncludiInSpedizione && !dest.StatoSpedizione.Equals(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito))
                    {
                        corr = dest.DatiDestinatari[0] as DocsPaVO.utente.Corrispondente;
                        destinatariNonRaggiunti += "#DESTINATARIO#" + corr.codiceRubrica + " (" + corr.descrizione + ")#DESCRIZIONE#: " + dest.StatoSpedizione.Descrizione + "";
                    }
                }
            }
            if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
            {
                foreach (DocsPaVO.Spedizione.DestinatarioInterno dest in infoSpedizione.DestinatariInterni)
                {
                    if (dest.IncludiInSpedizione && !dest.StatoSpedizione.Equals(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito))
                    {
                        destinatariNonRaggiunti += "#DESTINATARIO#" + dest.DatiDestinatario.codiceRubrica + " (" + dest.DatiDestinatario.descrizione + ")#DESCRIZIONE#: " + dest.StatoSpedizione.Descrizione + "";
                    }
                }
            }
            if(!string.IsNullOrEmpty(destinatariNonRaggiunti))
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di spedizione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                throw new Exception("Errore di spedizione per i seguenti destinatari: " + destinatariNonRaggiunti);            
            }
            //SE E' PRESENTE UN DESTINATARIO NON INTEROPERANTE ED è STATA RICHIESTA LA NOTITICA, NOTIFICO
            if (libro.NotificaPresenzaDestinatariInterop(passo.idIstanzaProcesso))
            {
                bool presentiDestNonInteroperanti = (from c in infoSpedizione.DestinatariEsterni
                                                     where !c.Interoperante
                                                     select c).FirstOrDefault() != null;
                if (presentiDestNonInteroperanti)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_DESTINATARI_NON_INTEROP", schedaDoc.docNumber, "Presenza di destinatati non interoperanti nella spedizione del documento", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                }
            }

            if (infoSpedizione.Spedito)
                result = true;
            else
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di spedizione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                throw new Exception("Errore spedizione.");
            }
            return result;
        }

        private static bool isPredisposto(DocsPaVO.documento.SchedaDocumento doc)
        {
            bool result = false;

            if (doc.tipoProto.Equals("A") || doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
            {
                if (!(doc.protocollo != null && !(string.IsNullOrEmpty(doc.protocollo.segnatura))))
                    result = true;
            }

            return result;
        }

        public static List<IstanzaProcessoDiFirma> GetIstanzeProcessiInErrore(List<string> idIstanzeProcessi, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<IstanzaProcessoDiFirma> istanze = new List<IstanzaProcessoDiFirma>();
            try
            {
                if (idIstanzeProcessi != null && idIstanzeProcessi.Count > 0)
                {
                    IstanzaProcessoDiFirma istanzaProcesso;
                    DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                    foreach (string idIstanza in idIstanzeProcessi)
                    {
                        istanzaProcesso = libro.GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite(idIstanza, infoUtente);
                        if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                        {                          
                            istanze.Add(istanzaProcesso);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetIstanzeProcessiInErrore " + e.Message);
            }
            return istanze;
        }

        public static bool RitentaIstanzeProcessiInErrore(List<IstanzaProcessoDiFirma> istanzeProcessi, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            logger.Debug("INIZIO RitentaIstanzeProcessiInErrore");
            try
            {
                if (istanzeProcessi != null && istanzeProcessi.Count > 0)
                {
                    IstanzaPassoDiFirma istanzaPasso;
                    DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                    foreach (IstanzaProcessoDiFirma istanzaProcesso in istanzeProcessi)
                    {
                        if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                        {
                            libro.SetStatoistanzaProcessoFirma(istanzaProcesso.idIstanzaProcesso, TipoStatoProcesso.REPLAY);
                            istanzaPasso = libro.GetIstanzaPassoDiFirmaInAttesa(istanzaProcesso.idIstanzaProcesso);

                            EseguiPassoAutomaticoAsync(istanzaPasso, istanzaProcesso);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                logger.Error("ERRORE in RitentaIstanzeProcessiInErrore: " + e.Message);
                result = false;
            }
            logger.Debug("FINE RitentaIstanzeProcessiInErrore");
            return result;
        }

        public static bool ExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.ExistsPassiFirmaByRuoloTitolareAndRegistro(rightRuoloMailReg, idRuoloInUO, idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: ExistsPassiFirmaByRuoloTitolareAndRegistro ", e);
                return false;
            }
        }

        public static bool ExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.ExistsPassiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: ExistsPassiFirmaByIdRegistroAndEmailRegistro ", e);
                return false;
            }
        }

        public static bool InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                string idReport = libroFirma.InsertReportProcessiTickByRegistroAndEmailRegistro(idRegistro, emailRegistro);
                result = libroFirma.InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
                if(result)
                    InviaReportCreatoriProcessi(idReport, infoUtente);
                InterrompiIstanzeByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaProcessiFirmaByIdRegistroAndEmailRegistro ", e);
                return false;
            }
            return result;
        }

        public static bool InvalidaProcessiRegistriCoinvolti(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                //Inserisco i processi da invalidare in una tabella
                string idReport = libroFirma.InsertReportProcessiTickRegistroRuolo(rightRuoloMailReg, idRuoloInUO, idGruppo);
                result = libroFirma.InvalidaProcessiRegistriCoinvolti(rightRuoloMailReg, idRuoloInUO, idGruppo);
                if (result)
                    InviaReportCreatoriProcessi(idReport, infoUtente);
                InterrompiIstanzeRegistriCoinvolti(rightRuoloMailReg, idRuoloInUO, idGruppo, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaProcessiRegistriCoinvolti ", e);
                return false;
            }
            return result;
        }

        private static bool InterrompiIstanzeRegistriCoinvolti(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                List<IstanzaProcessoDiFirma> istanzaProcessiCoinvolti = libroFirma.GetIstanzaProcessiDiFirmaRegistriCoinvolti(rightRuoloMailReg, idRuoloInUO, idGruppo, infoUtente);

                if (istanzaProcessiCoinvolti.Count() > 0)
                {
                    string noteInterruzione = "Interrotto processo per modifica da amministrazione di un ruolo coinvolto.";
                    result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByAdministrator(istanzaProcessiCoinvolti.ToArray(), noteInterruzione, infoUtente);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterrompiIstanzeRegistriCoinvolti ", ex);
            }
            return result;
        }

        private static bool InterrompiIstanzeByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                List<IstanzaProcessoDiFirma> istanzaProcessiCoinvolti = libroFirma.GetIstanzaProcessiDiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente);

                if (istanzaProcessiCoinvolti.Count() > 0)
                {
                    string noteInterruzione = "Interrotto processo per modifica da amministrazione ad un Registro/RF coinvolto.";
                    result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByAdministrator(istanzaProcessiCoinvolti.ToArray(), noteInterruzione, infoUtente);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterrompiIstanzeRegistriCoinvolti ", ex);
            }
            return result;
        }
    }
}
