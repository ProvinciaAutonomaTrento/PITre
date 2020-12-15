using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web.UI;
using System.Collections;

namespace NttDataWA.UIManager
{
    public class SignatureProcessesManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        #region Services Backend

        /// <summary>
        /// Metodo per l'estrazione degli eventi di notifica
        /// </summary>
        /// <returns></returns>
        public static List<AnagraficaEventi> GetEventNotification()
        {
            try
            {
                return docsPaWS.GetEventNotification(UserManager.GetInfoUser()).ToList<AnagraficaEventi>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo per l'estrazione degli eventi
        /// </summary>
        /// <returns></returns>
        public static List<AnagraficaEventi> GetEventTypes(string eventType)
        {
            try
            {
                return docsPaWS.GetEventTypes(eventType, UserManager.GetInfoUser()).ToList<AnagraficaEventi>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Estrazione dei processi di firma dell'utente
        /// </summary>
        /// <returns></returns>
        public static List<ProcessoFirma> GetProcessiDiFirma()
        {
            try
            {
                return docsPaWS.GetProcessiDiFirma(UserManager.GetInfoUser()).ToList<ProcessoFirma>();
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Estrazione dei processi di firma dell'utente applicando filtri di ricerca
        /// </summary>
        /// <returns></returns>
        public static List<ProcessoFirma> GetProcessiDiFirmaByFilter(List<FiltroProcessoFirma> filters)
        {
            try
            {
                return docsPaWS.GetProcessiFirmaByFilter(filters.ToArray(), UserManager.GetInfoUser()).ToList<ProcessoFirma>();
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Estrazione dei processi di firma visibili al ruolo
        /// </summary>
        /// <returns></returns>
        public static List<ProcessoFirma> GetProcessesSignatureVisibleRole(bool asProponente, bool includiVisibilitaRimosse, bool asMonitoratore)
        {
            try
            {
                return docsPaWS.GetProcessesSignatureVisibleRole(asProponente, asMonitoratore, includiVisibilitaRimosse, UserManager.GetInfoUser()).ToList<ProcessoFirma>();
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Creazione del processo di firma
        /// </summary>
        /// <param name="processoDiFirma"></param>
        /// <returns></returns>
        public static ProcessoFirma InsertProcessoDiFirma(ProcessoFirma processoDiFirma, out ResultProcessoFirma result)
        {
            result = ResultProcessoFirma.OK;
            try
            {
                return docsPaWS.InsertProcessoDiFirma(processoDiFirma, UserManager.GetInfoUser(), out result);
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Duplica il processo di firma
        /// </summary>
        /// <param name="processoBase"></param>
        /// <param name="nomeNuovoProcesso"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ProcessoFirma DuplicaProcessoFirma(ProcessoFirma processoOld, string nomeNuovoProcesso, bool copiaVisibilita, out ResultProcessoFirma result)
        {
            result = ResultProcessoFirma.OK;
            try
            {
                return docsPaWS.DuplicaProcessoFirma(processoOld, nomeNuovoProcesso, copiaVisibilita, UserManager.GetInfoUser(), out result);
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Copia i processi di firma su un altro ruolo
        /// </summary>
        /// <param name="idProcessi"></param>
        /// <param name="mantieniInRuoloOrigine"></param>
        /// <param name="copiaVisibilita"></param>
        /// <param name="ruoloDestinatario"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<CopiaProcessiFirmaResult> CopiaProcessiFirma(List<ProcessoFirma> processiFirma, bool copiaVisibilita, bool mantieniInRuoloOrigine, string idRuoloDest, string idPeopleDest)
        {
            try
            {
                return docsPaWS.CopiaProcessiFirma(processiFirma.ToArray(), copiaVisibilita, mantieniInRuoloOrigine, idRuoloDest, idPeopleDest, UserManager.GetInfoUser()).ToList();
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso)
        {
            try
            {
                return docsPaWS.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(idIstanzaProcesso, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static PassoFirma InserisciPassoDiFirma(PassoFirma passo)
        {
            try
            {
                return docsPaWS.InserisciPassoDiFirma(passo, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static ProcessoFirma GetProcessoDiFirma(string idProcesso)
        {
            try
            {
                return docsPaWS.GetProcessoDiFirma(idProcesso, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Rimozione del processo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <returns></returns>
        public static bool RimuoviProcessoDiFirma(ProcessoFirma processo)
        {
            try
            {
                return docsPaWS.RimuoviProcessoDiFirma(processo, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Rimozione del processo di firma
        /// </summary>
        /// <param name="processo"></param>
        /// <returns></returns>
        public static bool RimuoviPassoDiFirma(PassoFirma passo)
        {
            try
            {
                return docsPaWS.RimuoviPassoDiFirma(passo, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Rimuove la visibilità per il corrispondente in input
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public static bool RimuoviVisibilitaProcesso(string idProcesso, string idGruppo)
        {
            try
            {
                return docsPaWS.RimuoviVisibilitaProcesso(idProcesso, idGruppo, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Aggiorna il tipo di visibilita che ha il ruolo sul processo di firma
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idGruppo"></param>
        /// <param name="tipoVisibilita"></param>
        /// <returns></returns>
        public static bool UpdateTipoVisibilitaProcesso(VisibilitaProcessoRuolo visibilita)
        {
            try
            {
                return docsPaWS.UpdateTipoVisibilitaProcesso(visibilita, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Aggiorna passo di firma
        /// </summary>
        /// <param name="passo"></param>
        /// <returns></returns>
        public static bool AggiornaPassoDiFirma(PassoFirma passo, int oldNumeroSequenza)
        {
            try
            {
                return docsPaWS.AggiornaPassoDiFirma(passo,oldNumeroSequenza, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// Aggiornamento del processo di firma
        /// </summary>
        /// <param name="processoDiFirma"></param>
        /// <returns></returns>
        public static ProcessoFirma AggiornaProcessoDiFirma(ProcessoFirma processoDiFirma)
        {
            try
            {
                return docsPaWS.AggiornaProcessoDiFirma(processoDiFirma, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Imposta la visibilita del processo per i corrispondenti in input
        /// </summary>
        /// <param name="listaCorr"></param>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        public static bool InsertVisibilitaProcesso(List<VisibilitaProcessoRuolo> visibilita)
        {
            try
            {
                return docsPaWS.InsertVisibilitaProcesso(visibilita.ToArray(), UserManager.GetInfoUser());
            }
            catch(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Estrae la lista dei corrispondenti aventi visibilita sul processo
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        public static List<VisibilitaProcessoRuolo> GetVisibilitaProcesso(string idProcesso, List<FiltroProcessoFirma> filtroRicerca)
        {
            try
            {
                FiltroProcessoFirma[] filtro = null;
                if (filtroRicerca != null)
                    filtro = filtroRicerca.ToArray(); 
                return docsPaWS.GetVisibilitaProcesso(idProcesso, filtro, UserManager.GetInfoUser()).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool StartProccessSignature(ProcessoFirma process, FileRequest fileReq, string note, OpzioniNotifica opzioniNotifiche, out DocsPaWR.ResultProcessoFirma resultAvvioProcesso)
        {
            bool result = false;
            resultAvvioProcesso = ResultProcessoFirma.OK;
            try
            {
                result = docsPaWS.StartProcessoDiFirma(process, fileReq, UserManager.GetInfoUser(), LibroFirmaManager.Modalita.AUTOMATICA, note, opzioniNotifiche, out resultAvvioProcesso);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public static bool SalvaModificaStatoStartSignatureProcess(ProcessoFirma process, FileRequest fileReq, string note, OpzioniNotifica opzioniNotifiche,
            string idStato, DiagrammaStato diagramma, string dataScadenza, Page page,
            out DocsPaWR.ResultProcessoFirma resultAvvioProcesso)
        {
            bool result = false;
            resultAvvioProcesso = ResultProcessoFirma.OK;
            try
            {
                result = docsPaWS.SalvaModificaStatoStartSignatureProcess(process, fileReq, UserManager.GetInfoUser(), 
                    LibroFirmaManager.Modalita.AUTOMATICA, note, opzioniNotifiche,
                    idStato, diagramma, dataScadenza,
                    out resultAvvioProcesso);

                if (result)
                {
                    CompletaCambioStatoDocumento(page);
                }
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        private static void CompletaCambioStatoDocumento(Page page)
        {
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser(); ;
            SchedaDocumento docSel = DocumentManager.getSelectedRecord();
            DocsPaWR.Stato statoAttuale = DiagrammiManager.GetStateDocument(docSel.docNumber);
            //Controllo che lo stato sia uno stato di conversione pdf lato server
            //In caso affermativo faccio partire la conversione
            if (utils.isEnableConversionePdfLatoServer() == "true" &&
                docSel != null && docSel.documenti != null && !String.IsNullOrEmpty(docSel.documenti[0].fileName))
            {
                if (statoAttuale.CONVERSIONE_PDF)
                {
                    FileManager fileManager = new FileManager();
                    DocsPaWR.FileDocumento fileDocumento = fileManager.getFile(page);
                    if (fileDocumento != null && fileDocumento.content != null && fileDocumento.name != null && fileDocumento.name != "")
                    {
                        FileManager.EnqueueServerPdfConversion(UserManager.GetInfoUser(), fileDocumento.content, fileDocumento.name, DocumentManager.getSelectedRecord());
                    }
                }
            }
            InfoDocumento infoDoc = new InfoDocumento();
            Ruolo role = UIManager.RoleManager.GetRoleInSession();
            string idTemplate = string.Empty;
            if (docSel.template != null)
                idTemplate = docSel.template.SYSTEM_ID.ToString();
            ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(infoUtente.idAmministrazione, statoAttuale.SYSTEM_ID.ToString(), idTemplate));
            for (int i = 0; i < modelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                if (mod.SINGLE == "1")
                {
                    infoDoc = DocumentManager.getInfoDocumento(docSel);
                    TrasmManager.effettuaTrasmissioneDocDaModello(mod, statoAttuale.SYSTEM_ID.ToString(), infoDoc, page);
                    if (mod.CEDE_DIRITTI != null && mod.CEDE_DIRITTI.Equals("1"))
                    {
                        docSel = DocumentManager.getDocumentDetails(page, docSel.systemId, docSel.docNumber);
                        DocumentManager.setSelectedRecord(docSel);
                    }
                }
                else
                {
                    for (int k = 0; k < mod.MITTENTE.Length; k++)
                    {
                        if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == role.systemId)
                        {
                            infoDoc = DocumentManager.getInfoDocumento(docSel);
                            TrasmManager.effettuaTrasmissioneDocDaModello(mod, statoAttuale.SYSTEM_ID.ToString(), infoDoc, page);
                            if (mod.CEDE_DIRITTI != null && mod.CEDE_DIRITTI.Equals("1"))
                            {
                                docSel = DocumentManager.getDocumentDetails(page, docSel.systemId, docSel.docNumber);
                                DocumentManager.setSelectedRecord(docSel);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static List<FirmaResult> StartProccessSignatureMassive(ProcessoFirma process, List<FileRequest> fileReq, string note, OpzioniNotifica opzioniNotifiche)
        {
            List<FirmaResult> firmaRsult = new List<FirmaResult>();
            try
            {
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                firmaRsult = docsPaWS.StartProcessoDiFirmaMassive(process, fileReq.ToArray(), UserManager.GetInfoUser(), LibroFirmaManager.Modalita.AUTOMATICA, note, opzioniNotifiche).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            return firmaRsult;
        }

        /// <summary>
        /// Restituisce i tipi ruolo per cui non è presente un ruolo associato e gerarchicamente legato al ruolo che avvia il processo
        /// </summary>
        /// <param name="listTypeRoleToCheck"></param>
        /// <returns></returns>
        public static List<TipoRuolo> CheckExistsRoleSupByTypeRoles(List<TipoRuolo> listTypeRoleToCheck)
        {
            List<TipoRuolo> listTypeRole = new List<TipoRuolo>();
            try
            {
                listTypeRole = docsPaWS.CheckExistsRoleSupByTypeRoles(listTypeRoleToCheck.ToArray(), UserManager.GetInfoUser()).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            return listTypeRole;
        }

        public static bool IsEventoAutomatico(string codiceEvento)
        {
            try
            {
                return docsPaWS.IsEventoAutomatico(codiceEvento);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static AnagraficaEventi GetAnagraficaEventoByCodice(string codiceEvento)
        {
            try
            {
                return docsPaWS.GetAnagraficaEventoByCodice(codiceEvento);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static CasellaRegistro GetCasellaRegistroByIdMail(string idMail)
        {
            try
            {
                return docsPaWS.GetCasellaRegistroByIdMail(idMail);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<IstanzaProcessoDiFirma> GetIstanzeProcessiInErrore(List<string> idIstanzeProcessi)
        {
            try
            {
                return docsPaWS.GetIstanzeProcessiInErrore(idIstanzeProcessi.ToArray(), UserManager.GetInfoUser()).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool RitentaIstanzeProcessiInErrore(List<IstanzaProcessoDiFirma> istanzeProcessi)
        {
            try
            {
                return docsPaWS.RitentaIstanzeProcessiInErrore(istanzeProcessi.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        #endregion
    }
}